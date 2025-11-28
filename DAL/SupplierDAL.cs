using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class SupplierDAL : ISupplierRepository
    {
        public List<SupplierDTO> GetAll()
        {
            var list = new List<SupplierDTO>();
            const string sql = "hs.sp_GetAllSuppliers";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd["ContactPerson"].ToString(),
                            PhoneNumber = rd["PhoneNumber"].ToString(),
                            Email = rd["Email"].ToString(),
                            Address = rd["Address"].ToString(),
                            IsActive = rd["Status"].ToString() == "Active"
                        });
                    }
                }
            }
            return list;
        }

        public SupplierDTO GetById(int id)
        {
            SupplierDTO supplier = null;
            const string sql = "hs.sp_GetSupplierById";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SupplierID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        supplier = new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd["ContactPerson"].ToString(),
                            PhoneNumber = rd["PhoneNumber"].ToString(),
                            Email = rd["Email"].ToString(),
                            Address = rd["Address"].ToString(),
                            IsActive = rd["Status"].ToString() == "Active"
                        };
                    }
                }
            }
            return supplier;
        }

        public void Insert(SupplierDTO supplier)
        {
            const string sql = "hs.sp_InsertSupplier";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson);
                cmd.Parameters.AddWithValue("@PhoneNumber", supplier.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", supplier.Email);
                cmd.Parameters.AddWithValue("@Address", supplier.Address);
                cmd.Parameters.AddWithValue("@Status", supplier.IsActive ? "Active" : "Inactive");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(SupplierDTO supplier)
        {
            const string sql = "hs.sp_UpdateSupplier";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", supplier.ContactPerson);
                cmd.Parameters.AddWithValue("@PhoneNumber", supplier.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", supplier.Email);
                cmd.Parameters.AddWithValue("@Address", supplier.Address);
                cmd.Parameters.AddWithValue("@Status", supplier.IsActive ? "Active" : "Inactive");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "hs.sp_DeleteSupplier";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SupplierID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SupplierDTO> Search(string keyword)
        {
            var list = new List<SupplierDTO>();
            const string sql = "hs.sp_SearchSuppliers";

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
                        list.Add(new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd["ContactPerson"].ToString(),
                            PhoneNumber = rd["PhoneNumber"].ToString(),
                            Email = rd["Email"].ToString(),
                            Address = rd["Address"].ToString(),
                            IsActive = rd["Status"].ToString() == "Active"
                        });
                    }
                }
            }
            return list;
        }
    }
}

