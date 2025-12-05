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
        private static bool? _hasIsActiveColumn = null;

        private bool ColumnExists(SqlConnection conn, string tableName, string columnName)
        {
            if (_hasIsActiveColumn.HasValue)
                return _hasIsActiveColumn.Value;

            try
            {
                using (var cmd = new SqlCommand(
                    "SELECT CASE WHEN COL_LENGTH(@TableName, @ColumnName) IS NOT NULL THEN 1 ELSE 0 END",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@ColumnName", columnName);
                    var result = (int)cmd.ExecuteScalar();
                    _hasIsActiveColumn = result == 1;
                    return _hasIsActiveColumn.Value;
                }
            }
            catch
            {
                _hasIsActiveColumn = false;
                return false;
            }
        }

        public List<SupplierDTO> GetAll()
        {
            var list = new List<SupplierDTO>();
            string sql;

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                bool hasIsActive = ColumnExists(conn, "dbo.Suppliers", "IsActive");
                
                if (hasIsActive)
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive FROM dbo.Suppliers ORDER BY SupplierName";
                }
                else
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address FROM dbo.Suppliers ORDER BY SupplierName";
                }

                using (var cmd = new SqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var supplier = new SupplierDTO
                        {
                            SupplierID = (int)rd["SupplierID"],
                            SupplierName = rd["SupplierName"].ToString(),
                            ContactPerson = rd.IsDBNull(rd.GetOrdinal("ContactPerson")) ? null : rd["ContactPerson"].ToString(),
                            PhoneNumber = rd.IsDBNull(rd.GetOrdinal("PhoneNumber")) ? null : rd["PhoneNumber"].ToString(),
                            Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd["Email"].ToString(),
                            Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? null : rd["Address"].ToString(),
                            IsActive = true // Default to true if column doesn't exist
                        };

                        if (hasIsActive)
                        {
                            supplier.IsActive = (bool)rd["IsActive"];
                        }

                        list.Add(supplier);
                    }
                }
            }
            return list;
        }

        public SupplierDTO GetById(int id)
        {
            SupplierDTO supplier = null;
            string sql;

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                bool hasIsActive = ColumnExists(conn, "dbo.Suppliers", "IsActive");
                
                if (hasIsActive)
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive FROM dbo.Suppliers WHERE SupplierID = @SupplierID";
                }
                else
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address FROM dbo.Suppliers WHERE SupplierID = @SupplierID";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupplierID", id);
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
                                IsActive = true // Default to true if column doesn't exist
                            };

                            if (hasIsActive)
                            {
                                supplier.IsActive = (bool)rd["IsActive"];
                            }
                        }
                    }
                }
            }
            return supplier;
        }

        public void Insert(SupplierDTO supplier)
        {
            string sql;

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                bool hasIsActive = ColumnExists(conn, "dbo.Suppliers", "IsActive");
                
                if (hasIsActive)
                {
                    sql = @"INSERT INTO dbo.Suppliers (SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive) 
                            VALUES (@SupplierName, @ContactPerson, @PhoneNumber, @Email, @Address, @IsActive)";
                }
                else
                {
                    sql = @"INSERT INTO dbo.Suppliers (SupplierName, ContactPerson, PhoneNumber, Email, Address) 
                            VALUES (@SupplierName, @ContactPerson, @PhoneNumber, @Email, @Address)";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                    cmd.Parameters.AddWithValue("@ContactPerson", (object)supplier.ContactPerson ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)supplier.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)supplier.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)supplier.Address ?? DBNull.Value);
                    if (hasIsActive)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(SupplierDTO supplier)
        {
            string sql;

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                bool hasIsActive = ColumnExists(conn, "dbo.Suppliers", "IsActive");
                
                if (hasIsActive)
                {
                    sql = @"UPDATE dbo.Suppliers 
                            SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, PhoneNumber = @PhoneNumber, 
                                Email = @Email, Address = @Address, IsActive = @IsActive 
                            WHERE SupplierID = @SupplierID";
                }
                else
                {
                    sql = @"UPDATE dbo.Suppliers 
                            SET SupplierName = @SupplierName, ContactPerson = @ContactPerson, PhoneNumber = @PhoneNumber, 
                                Email = @Email, Address = @Address 
                            WHERE SupplierID = @SupplierID";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@SupplierID", supplier.SupplierID);
                    cmd.Parameters.AddWithValue("@SupplierName", supplier.SupplierName);
                    cmd.Parameters.AddWithValue("@ContactPerson", (object)supplier.ContactPerson ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PhoneNumber", (object)supplier.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)supplier.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", (object)supplier.Address ?? DBNull.Value);
                    if (hasIsActive)
                    {
                        cmd.Parameters.AddWithValue("@IsActive", supplier.IsActive);
                    }
                    cmd.ExecuteNonQuery();
                }
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
            string sql;

            using (var conn = DatabaseHelper.CreateConnection())
            {
                conn.Open();
                bool hasIsActive = ColumnExists(conn, "dbo.Suppliers", "IsActive");
                
                if (hasIsActive)
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address, IsActive 
                            FROM dbo.Suppliers 
                            WHERE SupplierName LIKE '%' + @Keyword + '%' OR PhoneNumber LIKE '%' + @Keyword + '%' OR ContactPerson LIKE '%' + @Keyword + '%' 
                            ORDER BY SupplierName";
                }
                else
                {
                    sql = @"SELECT SupplierID, SupplierName, ContactPerson, PhoneNumber, Email, Address 
                            FROM dbo.Suppliers 
                            WHERE SupplierName LIKE '%' + @Keyword + '%' OR PhoneNumber LIKE '%' + @Keyword + '%' OR ContactPerson LIKE '%' + @Keyword + '%' 
                            ORDER BY SupplierName";
                }

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            var supplier = new SupplierDTO
                            {
                                SupplierID = (int)rd["SupplierID"],
                                SupplierName = rd["SupplierName"].ToString(),
                                ContactPerson = rd.IsDBNull(rd.GetOrdinal("ContactPerson")) ? null : rd["ContactPerson"].ToString(),
                                PhoneNumber = rd.IsDBNull(rd.GetOrdinal("PhoneNumber")) ? null : rd["PhoneNumber"].ToString(),
                                Email = rd.IsDBNull(rd.GetOrdinal("Email")) ? null : rd["Email"].ToString(),
                                Address = rd.IsDBNull(rd.GetOrdinal("Address")) ? null : rd["Address"].ToString(),
                                IsActive = true // Default to true if column doesn't exist
                            };

                            if (hasIsActive)
                            {
                                supplier.IsActive = (bool)rd["IsActive"];
                            }

                            list.Add(supplier);
                        }
                    }
                }
            }
            return list;
        }
    }
}
