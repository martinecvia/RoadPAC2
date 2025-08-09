#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS8634

#pragma warning disable IDE0063 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8
#pragma warning disable IDE0028 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8
#pragma warning disable IDE0090 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Diagnostics;
using System.Drawing; // To test if file is loadable as Image
using System.IO;
using System.Linq; // Keep for .NET 4.6
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

using Shared.Controllers.Models.RibbonXml;

namespace Shared.Controllers
{
    public static class ResourceController
    {
        [RPPrivateUseOnly]
        private static readonly Dictionary<string, BitmapImage> _cachedBitMaps
            = new Dictionary<string, BitmapImage>();

        [RPPrivateUseOnly]
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
        public static void LoadEmbeddedResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // First we have to filter out files that does not start with prefix rp_
            string[] manifestResources = assembly.GetManifestResourceNames()
                .Where(resource => resource.Contains("rp_")).ToArray();
            foreach (string resourceName in manifestResources
                .Where(IsImage).ToArray())
            {
                // Image loading and caching
                try
                {
                    _cachedBitMaps.Add(resourceName.Split('.').Reverse().Skip(1).First(),
                        LoadResourceImage(resourceName));
                } catch (Exception) {
                    continue; // Seems like file was not loaded correctly, thus must be skipped
                }
            }
            foreach (string resourceName in manifestResources
                .Where(IsXml).ToArray())
            {
                // Xml loading and caching
                try
                {
                    _cachedXml.Add(resourceName.Split('.').Reverse().Skip(1).First());
                }
                catch (Exception) {
                    continue; // Seems like file was not loaded correctly, thus must be skipped
                }
            }
        }
        #region ImageResources
        /// <summary>
        /// Gets a BitmapImage by resource key (file name without extension).
        /// Returns null if not found.
        /// </summary>
        public static BitmapImage GetImageSource(string resourceName)
            => _cachedBitMaps.TryGetValue(resourceName, out BitmapImage bitMap) ? bitMap : null;

        [RPPrivateUseOnly]
        private static bool IsImage(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (Image _ = Image.FromStream(stream)) // Keep for .NET 4.6, System.Drawing must be as dependency,
                                                           // sole purpose of this change is to assert if file was loaded successfully as Image
                                                           // and not as something that is not image-like
                    return !IsXml(resourceName);           // XML can be loaded as Image too, so we want to check if this is really not a XML file
            } catch {
                return false;
            }
        }

        [RPPrivateUseOnly]
        private static BitmapImage LoadResourceImage(string resourceName)
        {
            BitmapImage _cachedBitMap = GetImageSource(resourceName);
            if (_cachedBitMap != null)
                return _cachedBitMap; // We don't want to load same file twice by accident, right?
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName)
                    ?? assembly.GetManifestResourceStream(assembly.GetManifestResourceNames()
                        .FirstOrDefault(resource => resource.EndsWith("Icons.rp_img_default_32.ico", StringComparison.OrdinalIgnoreCase))))
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
                    return bitMap;
                }
            }
            catch (Exception exception) {
                Debug.WriteLine($"LoadResourceImage: Something went horribly wrong ! " +
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
        public static T LoadResourceRibbon<T>(string resourceName) where T : BaseRibbonXml
        {
            if (!_cachedXml.Contains(resourceName))
                return default;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string manifestResource = assembly.GetManifestResourceNames()
                .FirstOrDefault(resource => resource.EndsWith($"Ribbons.{resourceName}.xml", StringComparison.OrdinalIgnoreCase));
            using (Stream stream = assembly.GetManifestResourceStream(manifestResource))
            {
                Assert.IsNotNull(stream, nameof(stream));      // This is a no-no, if default file was not found
                                                               // then something must happend during build process
                try
                {
                    Type type = typeof(T);
                    Assert.IsNotNull(type, nameof(type));
                    XmlSerializer serializer = new XmlSerializer(type);
                    return (T) serializer.Deserialize(stream);
                }
                catch (InvalidOperationException exception)
                {
                    // Log all nested exceptions
                    int currentNest = 1;
                    Exception currentException = exception;
                    while (currentException != null)
                    {
                        Debug.WriteLine($"{currentNest}:LoadResourceRibbon({resourceName}(InvalidOperationException)): {currentException.Message}");
                        currentException = currentException.InnerException;
                        currentNest++;
                    }
                    return null;
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"LoadResourceRibbon: {exception.Message}");
                    return default;
                }
            }
        }

        [RPPrivateUseOnly]
        private static bool IsXml(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings 
                    { IgnoreComments = true, IgnoreWhitespace = true, CloseInput = true }))
                    return true;
            } catch {
                return false;
            }
        }
        #endregion
    }
}