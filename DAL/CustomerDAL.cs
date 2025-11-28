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
            const string sql = "hs.sp_GetAllCustomers";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDTO
                        {
                            CustomerID = rd.GetInt32(0),
                            CustomerName = rd.GetString(1),
                            PhoneNumber = rd.GetString(2),
                            Address = rd.GetString(3)
                        });
                    }
                }
            }
            return list;
        }

        public CustomerDTO GetById(int id)
        {
            CustomerDTO customer = null;
            const string sql = "hs.sp_GetCustomerById";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
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
                            PhoneNumber = rd.GetString(2),
                            Address = rd.GetString(3)
                        };
                    }
                }
            }
            return customer;
        }

        public void Insert(CustomerDTO customer)
        {
            const string sql = "hs.sp_InsertCustomer";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(CustomerDTO customer)
        {
            const string sql = "hs.sp_UpdateCustomer";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "hs.sp_DeleteCustomer";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<CustomerDTO> Search(string keyword)
        {
            var list = new List<CustomerDTO>();
            const string sql = "hs.sp_SearchCustomers";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Keyword", keyword);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CustomerDTO
                        {
                            CustomerID = rd.GetInt32(0),
                            CustomerName = rd.GetString(1),
                            PhoneNumber = rd.GetString(2),
                            Address = rd.GetString(3)
                        });
                    }
                }
            }
            return list;
        }
    }
}

