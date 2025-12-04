using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class CustomerDAL : ICustomerRepository
    {
        public List<CustomerDTO> GetAll()
        {
            var list = new List<CustomerDTO>();
            const string sql = @"SELECT CustomerID, CustomerName, PhoneNumber, Email, Address, City, District, CreatedDate, UpdatedDate 
                                 FROM dbo.Customers ORDER BY CreatedDate DESC";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDTO
                        {
                            CustomerID = rd.GetInt32(0),
                            CustomerName = rd.GetString(1),
                            PhoneNumber = rd.IsDBNull(2) ? null : rd.GetString(2),
                            Email = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Address = rd.IsDBNull(4) ? null : rd.GetString(4),
                            City = rd.IsDBNull(5) ? null : rd.GetString(5),
                            District = rd.IsDBNull(6) ? null : rd.GetString(6),
                            CreatedDate = rd.GetDateTime(7),
                            UpdatedDate = rd.IsDBNull(8) ? (System.DateTime?)null : rd.GetDateTime(8)
                        });
                    }
                }
            }
            return list;
        }

        public CustomerDTO GetById(int id)
        {
            CustomerDTO customer = null;
            const string sql = @"SELECT CustomerID, CustomerName, PhoneNumber, Email, Address, City, District, CreatedDate, UpdatedDate 
                                 FROM dbo.Customers WHERE CustomerID = @CustomerID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        customer = new CustomerDTO
                        {
                            CustomerID = rd.GetInt32(0),
                            CustomerName = rd.GetString(1),
                            PhoneNumber = rd.IsDBNull(2) ? null : rd.GetString(2),
                            Email = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Address = rd.IsDBNull(4) ? null : rd.GetString(4),
                            City = rd.IsDBNull(5) ? null : rd.GetString(5),
                            District = rd.IsDBNull(6) ? null : rd.GetString(6),
                            CreatedDate = rd.GetDateTime(7),
                            UpdatedDate = rd.IsDBNull(8) ? (System.DateTime?)null : rd.GetDateTime(8)
                        };
                    }
                }
            }
            return customer;
        }

        public void Insert(CustomerDTO customer)
        {
            const string sql = @"INSERT INTO dbo.Customers (CustomerName, PhoneNumber, Email, Address, City, District) 
                                 VALUES (@CustomerName, @PhoneNumber, @Email, @Address, @City, @District)";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object)customer.City ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@District", (object)customer.District ?? System.DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(CustomerDTO customer)
        {
            const string sql = @"UPDATE dbo.Customers 
                                 SET CustomerName = @CustomerName, PhoneNumber = @PhoneNumber, Email = @Email, 
                                     Address = @Address, City = @City, District = @District, UpdatedDate = GETDATE()
                                 WHERE CustomerID = @CustomerID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)customer.Address ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@City", (object)customer.City ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@District", (object)customer.District ?? System.DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Customers WHERE CustomerID = @CustomerID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CustomerID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<CustomerDTO> Search(string keyword)
        {
            var list = new List<CustomerDTO>();
            const string sql = @"SELECT CustomerID, CustomerName, PhoneNumber, Email, Address, City, District 
                                 FROM dbo.Customers 
                                 WHERE CustomerName LIKE '%' + @Keyword + '%' OR PhoneNumber LIKE '%' + @Keyword + '%' OR Email LIKE '%' + @Keyword + '%' 
                                 ORDER BY CustomerName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDTO
                        {
                            CustomerID = rd.GetInt32(0),
                            CustomerName = rd.GetString(1),
                            PhoneNumber = rd.IsDBNull(2) ? null : rd.GetString(2),
                            Email = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Address = rd.IsDBNull(4) ? null : rd.GetString(4),
                            City = rd.IsDBNull(5) ? null : rd.GetString(5),
                            District = rd.IsDBNull(6) ? null : rd.GetString(6)
                        });
                    }
                }
            }
            return list;
        }
    }
}
