using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Shared.Controllers
{
    public static class ResourceController
    {
        [RPPrivateUseOnly]
        private static readonly Dictionary<string, CachedBitmap> _cachedBitMaps 
            = new Dictionary<string, CachedBitmap>();

        public static void LoadEmbeddedResources()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var manifestResources = assembly.GetManifestResourceNames()
                .Where(resource => resource.StartsWith("rp_", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var resourceName in manifestResources)
            {
                
            }
        }

        [RPPrivateUseOnly]
        private static ImageSource LoadResourceImage(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resourceName)
                ?? assembly.GetManifestResourceStream(assembly.GetManifestResourceNames()
                    .FirstOrDefault(resource => resource.Contains("rp_")
                        && resource.EndsWith("rp_img_btn_default_32.png", StringComparison.OrdinalIgnoreCase))))
            {
                Assert.IsNotNull(stream, nameof(stream)); // This is a no-no, if default file was not found
                                                          // then something must happend during build process
                BitmapImage bitMap = new BitmapImage();
                bitMap.BeginInit();
                bitMap.StreamSource = stream;
                bitMap.CacheOption = BitmapCacheOption.OnLoad; // Load fully and then detach stream
                bitMap.EndInit();
                // To make it thread safe and immutable
                bitMap.Freeze();
                return bitMap;
            }
        }
    }
}