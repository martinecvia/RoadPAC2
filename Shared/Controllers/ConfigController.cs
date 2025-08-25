using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Shared.Controllers
{
    [Guid("68AC376D-B484-8325-843A-3DC093543A51")]
    public static class ConfigController
    {
#pragma warning disable CS8604 // Může jít o argument s odkazem null.
        public static string GetConfigPath(string lsFileName = "ANetRoadPAC2.config.xml")
            => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), lsFileName);
#pragma warning restore CS8604 // Může jít o argument s odkazem null.
        public static void SaveConfig<T>(T obj, string lsFileName = "ANetRoadPAC2.config.xml")
        {
            if (obj == null) return;
            string path = GetConfigPath(lsFileName);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(path))
                serializer.Serialize(writer, obj);
        }
        public static T LoadConfig<T>(string lsFileName = "ANetRoadPAC2.config.xml") where T : class, new()
        {
            string path = GetConfigPath(lsFileName);
            if (!File.Exists(path))
                return new T();
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(path))
                return serializer.Deserialize(reader) as T ?? new T();
        }
    }
}
