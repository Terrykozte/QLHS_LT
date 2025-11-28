using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using QLTN_LT.DTO;

using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class SeafoodDAL : ISeafoodRepository
    {
        public List<SeafoodDTO> GetAll()
        {
            var list = new List<SeafoodDTO>();
            const string sql = @"
                SELECT s.SeafoodID, s.SeafoodName, s.CategoryID, c.CategoryName,
                       s.UnitPrice, s.Quantity, s.Description, s.ImagePath,
                       s.Status, s.CreatedDate, s.UpdatedDate
                FROM hs.Seafood s
                LEFT JOIN hs.Categories c ON s.CategoryID = c.CategoryID
                ORDER BY s.SeafoodName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SeafoodDTO
                        {
                            SeafoodID = rd.GetInt32(0),
                            SeafoodName = rd.GetString(1),
                            CategoryID = rd.GetInt32(2),
                            CategoryName = rd.IsDBNull(3) ? string.Empty : rd.GetString(3),
                            UnitPrice = rd.GetDecimal(4),
                            Quantity = rd.GetInt32(5),
                            Description = rd.IsDBNull(6) ? string.Empty : rd.GetString(6),
                            ImagePath = rd.IsDBNull(7) ? string.Empty : rd.GetString(7),
                            Status = rd.IsDBNull(8) ? "Active" : rd.GetString(8),
                            CreatedDate = rd.GetDateTime(9),
                            UpdatedDate = rd.IsDBNull(10) ? (DateTime?)null : rd.GetDateTime(10)
                        });
                    }
                }
            }

            return list;
        }

        public SeafoodDTO GetById(int id)
        {
            SeafoodDTO seafood = null;
            const string sql = @"
                SELECT s.SeafoodID, s.SeafoodName, s.CategoryID, c.CategoryName,
                       s.UnitPrice, s.Quantity, s.Description, s.ImagePath,
                       s.Status, s.CreatedDate, s.UpdatedDate
                FROM hs.Seafood s
                LEFT JOIN hs.Categories c ON s.CategoryID = c.CategoryID
                WHERE s.SeafoodID = @SeafoodID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SeafoodID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        seafood = new SeafoodDTO
                        {
                            SeafoodID = rd.GetInt32(0),
                            SeafoodName = rd.GetString(1),
                            CategoryID = rd.GetInt32(2),
                            CategoryName = rd.IsDBNull(3) ? string.Empty : rd.GetString(3),
                            UnitPrice = rd.GetDecimal(4),
                            Quantity = rd.GetInt32(5),
                            Description = rd.IsDBNull(6) ? string.Empty : rd.GetString(6),
                            ImagePath = rd.IsDBNull(7) ? string.Empty : rd.GetString(7),
                            Status = rd.IsDBNull(8) ? "Active" : rd.GetString(8),
                            CreatedDate = rd.GetDateTime(9),
                            UpdatedDate = rd.IsDBNull(10) ? (DateTime?)null : rd.GetDateTime(10)
                        };
                    }
                }
            }
            return seafood;
        }

        public void Insert(SeafoodDTO seafood)
        {
            const string sql = "hs.sp_InsertSeafood";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeafoodName", seafood.SeafoodName);
                cmd.Parameters.AddWithValue("@CategoryID", seafood.CategoryID);
                cmd.Parameters.AddWithValue("@UnitPrice", seafood.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", seafood.Quantity);
                cmd.Parameters.AddWithValue("@Description", (object)seafood.Description ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(SeafoodDTO seafood)
        {
            const string sql = "hs.sp_UpdateSeafood";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeafoodID", seafood.SeafoodID);
                cmd.Parameters.AddWithValue("@SeafoodName", seafood.SeafoodName);
                cmd.Parameters.AddWithValue("@CategoryID", seafood.CategoryID);
                cmd.Parameters.AddWithValue("@UnitPrice", seafood.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", seafood.Quantity);
                cmd.Parameters.AddWithValue("@Description", (object)seafood.Description ?? DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "hs.sp_DeleteSeafood";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SeafoodID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SeafoodDTO> Search(string keyword)
        {
            var list = new List<SeafoodDTO>();
            const string sql = "hs.sp_SearchSeafood";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Keyword", keyword);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SeafoodDTO
                        {
                            SeafoodID = rd.GetInt32(0),
                            SeafoodName = rd.GetString(1),
                            CategoryName = rd.GetString(2),
                            UnitPrice = rd.GetDecimal(3),
                            Quantity = rd.GetInt32(4)
                        });
                    }
                }
            }
            return list;
        }
    }
}

