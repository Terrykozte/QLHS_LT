using System;
using System.IO;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace QLTN_LT.DAL
{
    public class ConnectionSettings
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Portable + per-user config resolution
        // 1) If connection_config.xml exists beside the executable => use it (portable mode)
        // 2) Else use per-user LocalAppData (no admin rights required, different for each machine/user)
        private static string GetConfigPath()
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string portable = Path.Combine(exeDir, "connection_config.xml");
                if (File.Exists(portable)) return portable;

                string appDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QLHS_LT");
                if (!Directory.Exists(appDir)) Directory.CreateDirectory(appDir);
                return Path.Combine(appDir, "connection_config.xml");
            }
            catch
            {
                // Fallback: keep old behavior if anything goes wrong
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection_config.xml");
            }
        }

        public string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                IntegratedSecurity = IntegratedSecurity,
                TrustServerCertificate = true // Important for newer SQL Servers
            };

            if (!IntegratedSecurity)
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            return builder.ConnectionString;
        }

        public static void Save(ConnectionSettings settings)
        {
            string path = GetConfigPath();
            var serializer = new XmlSerializer(typeof(ConnectionSettings));
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, settings);
            }
        }

        public static ConnectionSettings Load()
        {
            string path = GetConfigPath();
            if (!File.Exists(path)) return null;

            try
            {
                var serializer = new XmlSerializer(typeof(ConnectionSettings));
                using (var reader = new StreamReader(path))
                {
                    return (ConnectionSettings)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool Exists()
        {
            try { return File.Exists(GetConfigPath()); } catch { return false; }
        }
    }
}
