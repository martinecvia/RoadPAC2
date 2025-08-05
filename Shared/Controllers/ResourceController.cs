#pragma warning disable CS8600
#pragma warning disable CS8603
#pragma warning disable CS8604
#pragma warning disable CS8634

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing; // To test if file is loadable as Image
using System.IO;
using System.Linq;
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
        /// Loads embedded image resources from the executing assembly into the `_cachedBitMaps` dictionary.
        /// Only resources starting with "rp_" and recognized as image files are considered.
        /// The file name (without extension) is used as the dictionary key.
        /// </summary>
        public static void LoadEmbeddedResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            // First we have to filter out files that does not start with prefix rp_
            string[] manifestResources = assembly.GetManifestResourceNames()
                .Where(resource => resource.Contains("rp_")).ToArray();
            Debug.WriteLine($"LoadEmbeddedResources: {manifestResources}");
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
                catch (Exception)
                {
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
                using (Image _ = Image.FromStream(stream))
                    return true;
            } catch {
                return false;
            }
        }

        [RPPrivateUseOnly]
        private static BitmapImage LoadResourceImage(string resourceName)
        {
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
                throw exception; // pass
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
            Debug.WriteLine($"LoadResourceRibbon: {manifestResource}");
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
                    Debug.WriteLine($"LoadResourceRibbon: InvalidOperationException: {exception.InnerException?.Message}");
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