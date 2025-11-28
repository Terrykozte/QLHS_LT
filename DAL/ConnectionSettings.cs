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

        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "connection_config.xml");

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
            var serializer = new XmlSerializer(typeof(ConnectionSettings));
            using (var writer = new StreamWriter(ConfigPath))
            {
                serializer.Serialize(writer, settings);
            }
        }

        public static ConnectionSettings Load()
        {
            if (!File.Exists(ConfigPath)) return null;

            try
            {
                var serializer = new XmlSerializer(typeof(ConnectionSettings));
                using (var reader = new StreamReader(ConfigPath))
                {
                    return (ConnectionSettings)serializer.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
