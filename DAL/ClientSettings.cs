using System;
using System.IO;
using System.Xml.Serialization;

namespace QLTN_LT.DAL
{
    [Serializable]
    public class ClientSettings
    {
        public string LastUsername { get; set; }
        public bool FirstRunConfigShown { get; set; }

        private static string GetPath()
        {
            try
            {
                // Share same base folder with ConnectionSettings
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QLHS_LT");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return Path.Combine(dir, "client_prefs.xml");
            }
            catch
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "client_prefs.xml");
            }
        }

        public static ClientSettings Load()
        {
            string path = GetPath();
            if (!File.Exists(path)) return new ClientSettings();
            try
            {
                var ser = new XmlSerializer(typeof(ClientSettings));
                using (var sr = new StreamReader(path))
                {
                    return (ClientSettings)ser.Deserialize(sr);
                }
            }
            catch
            {
                return new ClientSettings();
            }
        }

        public static void Save(ClientSettings settings)
        {
            try
            {
                string path = GetPath();
                var ser = new XmlSerializer(typeof(ClientSettings));
                using (var sw = new StreamWriter(path))
                {
                    ser.Serialize(sw, settings);
                }
            }
            catch { }
        }
    }
}

