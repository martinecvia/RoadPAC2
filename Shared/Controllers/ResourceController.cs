#pragma warning disable CS8600, CS8603, CS8604, CS8634

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Diagnostics;
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

using Shared.Controllers.Models.RibbonXml;

namespace Shared.Controllers
{
    public static class ResourceController
    {
        private static readonly Dictionary<string, BitmapImage> _cachedBitMaps
            = new Dictionary<string, BitmapImage>();
        private static readonly Dictionary<string, string> _cachedURI = new Dictionary<string, string>();
        private static readonly List<string> _cachedXml = new List<string>();

        /// <summary>
        /// Loads embedded resources with the <c>rp_</c> prefix from the executing assembly
        /// and caches them for later use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// </para>
        /// <list type="number">
        /// <item>Retrieves all manifest resource names from the executing assembly.</item>
        /// <item>Filters resources to those whose names contain the <c>rp_</c> prefix.</item>
        /// <item>
        /// For resources recognized as images (<see cref="IsImage"/> returns <c>true</c>),
        /// attempts to load them into memory via <see cref="LoadResourceImage"/> and stores
        /// them in the <c>_cachedBitMaps</c> collection, keyed by the resource’s base filename.
        /// </item>
        /// <item>
        /// For resources recognized as XML (<see cref="IsXml"/> returns <c>true</c>),
        /// adds their base filenames to the <c>_cachedXml</c> collection.
        /// </item>
        /// </list>
        /// <para>
        /// If a resource fails to load (e.g., due to format errors or corruption), it is skipped.
        /// </para>
        /// </remarks>
        [RPInternalUseOnly]
        internal static void LoadEmbeddedResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // First we have to filter out files that does not start with prefix rp_
            string[] manifestResources = assembly.GetManifestResourceNames()
                .Where(resource => resource.Contains("rp_")).ToArray();
            foreach (string resourceName in manifestResources)
            {
                string key = resourceName.Split('.').Reverse().Skip(1).FirstOrDefault();
                if (string.IsNullOrEmpty(key))
                    continue;
                try
                {
                    if (IsImage(resourceName))
                    {
                        var img = LoadResourceImage(resourceName);
                        if (img != null)
                            _cachedBitMaps[key] = img;
                        continue;
                    }
                    if (IsXml(resourceName))
                        _cachedXml.Add(key);
                }
                catch (Exception)
                { continue; }
            }
        }
        #region ImageResources
        /// <summary>
        /// Gets a BitmapImage by resource key (file name without extension).
        /// If resource was not found, we attempt to return the default 32x32 icon.
        /// If directive was wrongly build, likewise default image is not present or corruptted
        /// Returns null if both {resourceName} and default not found.
        /// </summary>
        public static BitmapImage GetImageSource(string resourceName)
            => _cachedBitMaps.TryGetValue(resourceName, out BitmapImage bitMap) 
            ? bitMap : (_cachedBitMaps.TryGetValue("rp_img_default_32.ico", out BitmapImage defaultBitMap) ? defaultBitMap : null);

        public static string GetImageURI(string resourceName)
            => _cachedURI.TryGetValue(resourceName, out string URI)
            ? URI : (_cachedURI.TryGetValue("rp_img_default_32.ico", out string defaultURI) ? defaultURI : null);

        /// <summary>
        /// Loads an embedded XML resource and deserializes it into an object of type <typeparamref name="T"/>.
        /// Returns null if the resource is not found or deserialization fails.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the XML into.</typeparam>
        /// <param name="resourceName">Full name of the embedded resource, including namespace and folder path.</param>
        /// <returns>An instance of <typeparamref name="T"/> or null if unsuccessful.</returns>
        public static T GetXmlSource<T>(string resourceName) 
            where T : class
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) 
                        return default(T);
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return serializer.Deserialize(stream) as T;
                }
            }
            catch (Exception)
            { return default(T); }
        }
        
        [RPPrivateUseOnly]
        private static bool IsImage(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null || stream.Length < 4) return false;
                    byte[] header = new byte[4];
                    stream.Read(header, 0, header.Length);
                    if ((header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47) || // PNG (89 50 4E 47)
                        (header[0] == 0xFF && header[1] == 0xD8) ||                                           // JPG (FF D8)
                        (header[0] == 0x42 && header[1] == 0x4D) ||                                           // BMP (42 4D = "BM")
                        (header[0] == 0x00 && header[1] == 0x00 && header[2] == 0x01 && header[3] == 0x00) || // ICO (00 00 01 00)
                        (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38) || // GIF (ASCII "GIF8")
                        (header[0] == 0x49 && header[1] == 0x49 && header[2] == 0x2A) ||
                        (header[0] == 0x4D && header[1] == 0x4D && header[2] == 0x00 && header[3] == 0x2A))   // TIFF ("II*" or "MM*")
                        return true;
                    /*
                    stream.Position = 0; // Resets the stream back to the position it was before read,
                                         // this way we can check other formats not caught by header-types
                    */
                    return false;
                }
            } catch(Exception) 
            { return false; }
        }

        [RPPrivateUseOnly]
        private static BitmapImage LoadResourceImage(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string peekURI = assembly.GetManifestResourceNames()
                    .FirstOrDefault(r => r.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                    ?? assembly.GetManifestResourceNames()
                        .FirstOrDefault(r => r.EndsWith("Icons.rp_img_default_32.ico", StringComparison.OrdinalIgnoreCase));
                if (peekURI == null)
                    throw new FileNotFoundException("Resource not found", resourceName);
                using (Stream stream = assembly.GetManifestResourceStream(peekURI))
                {
                    Assert.IsNotNull(stream, nameof(stream));      // This is a no-no, if default file was not found
                                                                   // then something must happend during build process
                    BitmapImage bitMap = new BitmapImage();
                    bitMap.BeginInit();
                    // InOpt::CS7096: Stream is not usable as viable ImageSource stream
                    bitMap.StreamSource = stream;
                    bitMap.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    bitMap.CacheOption = BitmapCacheOption.OnLoad; // Load fully and then detach stream
                    bitMap.EndInit();
                    // To make it thread safe and immutable
                    bitMap.Freeze();
                    string key = resourceName.Split('.').Reverse().Skip(1).FirstOrDefault();
                    int index = resourceName.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                    if (index < 0) throw new FileNotFoundException("Resource key not found", resourceName);
                    string packURI = string.Format("pack://application:,,,/{0}{1}",
                        peekURI.Substring(0, index).Replace('.', '/'),
                        peekURI.Substring(index));
                    _cachedURI[key] = packURI;
                    return bitMap;
                }
            }
            catch (Exception exception) {
                Debug.WriteLine($"[&] LoadResourceImage: Something went horribly wrong ! " +
                    $"{resourceName}: {exception.GetType().Name}/{exception.Message}");
                throw; // pass
            }
        }

        #endregion
        #region RibbonResources
        /// <summary>
        /// Loads an embedded XML resource and deserializes it into the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the XML into.</typeparam>
        /// <param name="resourceName">The logical name (without extension) of the embedded XML resource.</param>
        /// <returns>Deserialized object of type T, or default(T) if the resource is not cached or fails to load.</returns>
        public static RibbonTabDef LoadResourceRibbon(string resourceName)
        {
            if (!_cachedXml.Contains(resourceName))
                return null;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string manifestResource = assembly.GetManifestResourceNames()
                .FirstOrDefault(resource => resource.EndsWith($"Ribbons.{resourceName}.xml", StringComparison.OrdinalIgnoreCase));
            using (Stream stream = assembly.GetManifestResourceStream(manifestResource))
            {
                Assert.IsNotNull(stream, nameof(stream));      // This is a no-no, if default file was not found
                                                               // then something must happend during build process
                try
                {
                    Type type = typeof(RibbonTabDef);
                    Assert.IsNotNull(type, nameof(type));
                    XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute("RibbonTab"));
                    return (RibbonTabDef) serializer.Deserialize(stream);
                }
                catch (InvalidOperationException exception)
                {
                    LogException(exception, resourceName);
                    return default;
                }
                catch (Exception exception)
                {
                    LogException(exception, resourceName);
                    return default; 
                }
            }
        }

        private static void LogException(Exception exception, string resourceName)
        {
            // Log all nested exceptions
            int currentNest = 1;
            Exception currentException = exception;
            while (currentException != null)
            {
                Debug.WriteLine($"[&] {currentNest}:LoadResourceRibbon({resourceName}(InvalidOperationException)): {currentException.Message}");
                currentException = currentException.InnerException;
                currentNest++;
            }
        }

        [RPPrivateUseOnly]
        private static bool IsXml(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
                {
                    // We just check couple of bytes to ensure it contains <,
                    // which in fact is how XML starts, even comments starts with this tag
                    char[] buffer = new char[5];
                    int read = reader.Read(buffer, 0, buffer.Length);
                    return new string(buffer, 0, read).TrimStart().StartsWith("<");
                }
            } catch(Exception) 
            { return false; }
        }
        #endregion
    }
}