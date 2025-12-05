using System;
using System.Configuration;
using System.Data.SqlClient;

namespace QLTN_LT.DAL
{
    /// <summary>
    /// Lớp trợ giúp cho việc tạo kết nối tới CSDL.
    /// Chỉ DAL biết về kết nối, BLL/GUI hoàn toàn không phụ thuộc DB cụ thể.
    /// </summary>
    public static class DatabaseHelper
    {
        private const string DefaultConnName = "DefaultConnection";

        /// <summary>
        /// Lấy connection string từ App.config hoặc từ file cấu hình portable.
        /// </summary>
        public static string GetConnectionString(string name = DefaultConnName)
        {
            // Ưu tiên: 1) Portable config 2) App.config
            var portableSettings = ConnectionSettings.Load();
            if (portableSettings != null)
            {
                return portableSettings.GetConnectionString();
            }
            
            // Fallback: App.config
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }

        /// <summary>
        /// Tạo SqlConnection (chưa mở). Người gọi chịu trách nhiệm mở/đóng.
        /// </summary>
        public static SqlConnection CreateConnection(string name = DefaultConnName)
        {
            var cs = GetConnectionString(name);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException("Không tìm thấy connection string. Vui lòng cấu hình kết nối database.");
            }
            return new SqlConnection(cs);
        }
        
        /// <summary>
        /// Tạo và mở SqlConnection. Người gọi chịu trách nhiệm đóng.
        /// </summary>
        public static SqlConnection CreateAndOpenConnection(string name = DefaultConnName)
        {
            var connection = CreateConnection(name);
            connection.Open();
            return connection;
        }
    }
}

