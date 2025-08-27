#pragma warning disable CS8600

using System; // Keep for .NET 4.6
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml; // Keep for .NET 4.6
using System.Xml.Serialization;

namespace Shared.Controllers
{
    [Guid("68AC376D-B484-8325-843A-3DC093543A51")]
    public static class ConfigController
    {
        private static readonly ConcurrentDictionary<string, object> _cache 
            = new ConcurrentDictionary<string, object>();
#pragma warning disable CS8604 // Může jít o argument s odkazem null.
        public static string GetConfigPath(string lsFileName = "ANetRoadPAC2.config.xml")
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), lsFileName);
#pragma warning restore CS8604 // Může jít o argument s odkazem null.
        public static void SaveConfig<T>(T obj, string lsFileName = "ANetRoadPAC2.config.xml")
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            string path = GetConfigPath(lsFileName);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                    serializer.Serialize(writer, obj);
                if (obj != null) 
                    _cache[lsFileName] = obj;
            } catch(Exception exception) {
                throw new InvalidOperationException($"Failed to save config to '{path}'", exception);
            }
        }

        public static T LoadConfig<T>(string lsFileName = "ANetRoadPAC2.config.xml") where T : class, new()
        {
            if (_cache.TryGetValue(lsFileName, out object cached) && cached is T hwConfig)
                return hwConfig;
            string path = GetConfigPath(lsFileName);
            if (!File.Exists(path))
                return new T();
            T obj;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var reader = new StreamReader(path))
                    obj = serializer.Deserialize(reader) as T ?? new T();
            }
            catch (Exception)
            { obj = new T(); }
            _cache[lsFileName] = obj;
            return obj;
        }
    }
}
