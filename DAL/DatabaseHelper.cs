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
        /// Lấy connection string theo tên trong App.config
        /// </summary>
        public static string GetConnectionString(string name = DefaultConnName)
        {
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
    }
}

