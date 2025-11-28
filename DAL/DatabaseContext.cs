using System;
using System.Data.SqlClient;

namespace QLTN_LT.DAL
{
    public class DatabaseContext : IDisposable
    {
        private readonly SqlConnection _connection;

        public DatabaseContext()
        {
            _connection = DatabaseHelper.CreateConnection();
        }

        public SqlConnection Connection => _connection;

        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
                _connection.Dispose();
            }
        }
    }
}
