using System;
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
            const string sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive FROM dbo.Suppliers ORDER BY SupplierName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd.IsDBNull(rd.GetOrdinal("ContactPerson")) ? null : rd["ContactPerson"].ToString(),
                            PhoneNumber = rd.IsDBNull(rd.GetOrdinal("PhoneNumber")) ? null : rd["PhoneNumber"].ToString(),
                            Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd["Email"].ToString(),
                            Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? null : rd["Address"].ToString(),
                            IsActive = (bool)rd["IsActive"]
                        });
                    }
                }
            }
            return list;
        }

        public SupplierDTO GetById(int id)
        {
            SupplierDTO supplier = null;
            const string sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive FROM dbo.Suppliers WHERE SupplierID = @SupplierID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
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
                            ContactPerson = rd.IsDBNull(rd.GetOrdinal("ContactPerson")) ? null : rd["ContactPerson"].ToString(),
                            PhoneNumber = rd.IsDBNull(rd.GetOrdinal("PhoneNumber")) ? null : rd["PhoneNumber"].ToString(),
                            Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd["Email"].ToString(),
                            Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? null : rd["Address"].ToString(),
                            IsActive = (bool)rd["IsActive"]
                        };
                    }
                }
            }
            return supplier;
        }

        public void Insert(SupplierDTO supplier)
        {
            const string sql = @"INSERT INTO dbo.Suppliers (SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive) 
                                 VALUES (@SupplierName, @ContactPerson, @PhoneNumber, @Email, @Address, @IsActive)";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", (object)supplier.ContactPerson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)supplier.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)supplier.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)supplier.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(SupplierDTO supplier)
        {
            const string sql = @"UPDATE dbo.Suppliers 
                                 SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, PhoneNumber = @PhoneNumber, 
                                     Email = @Email, Address = @Address, IsActive = @IsActive 
                                 WHERE SupplierID = @SupplierID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
                cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                cmd.Parameters.AddWithValue("@ContactPerson", (object)supplier.ContactPerson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)supplier.PhoneNumber ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)supplier.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", (object)supplier.Address ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Suppliers WHERE SupplierID = @SupplierID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SupplierID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SupplierDTO> Search(string keyword)
        {
            var list = new List<SupplierDTO>();
            const string sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive 
                                 FROM dbo.Suppliers 
                                 WHERE SupplierName LIKE '%' + @Keyword + '%' OR PhoneNumber LIKE '%' + @Keyword + '%' OR ContactPerson LIKE '%' + @Keyword + '%' 
                                 ORDER BY SupplierName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd.IsDBNull(rd.GetOrdinal("ContactPerson")) ? null : rd["ContactPerson"].ToString(),
                            PhoneNumber = rd.IsDBNull(rd.GetOrdinal("PhoneNumber")) ? null : rd["PhoneNumber"].ToString(),
                            Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd["Email"].ToString(),
                            Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? null : rd["Address"].ToString(),
                            IsActive = (bool)rd["IsActive"]
                        });
                    }
                }
            }
            return list;
        }
    }
}
