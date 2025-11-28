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
        private static string _cachedConnectionString;

        /// <summary>
        /// Lấy connection string theo tên trong App.config hoặc từ file cấu hình động.
        /// </summary>
        public static string GetConnectionString(string name = DefaultConnName)
        {
            // 1. Try to load from cached or dynamic config first
            if (!string.IsNullOrEmpty(_cachedConnectionString)) return _cachedConnectionString;

            var settings = ConnectionSettings.Load();
            if (settings != null)
            {
                _cachedConnectionString = settings.GetConnectionString();
                return _cachedConnectionString;
            }

            // 2. Fallback to App.config
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString;
        }

        /// <summary>
        /// Tạo SqlConnection (chưa mở). Người gọi chịu trách nhiệm mở/đóng.
        /// </summary>
        public static SqlConnection CreateConnection(string name = DefaultConnName)
        {
            var cs = GetConnectionString(name);
            return new SqlConnection(cs);
        }
        
        /// <summary>
        /// Xóa cache để load lại setting mới
        /// </summary>
        public static void ResetConnection()
        {
            _cachedConnectionString = null;
        }
    }
}

