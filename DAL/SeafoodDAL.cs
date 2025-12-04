using System;
using System.Collections.Generic;
using System.Data;
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
                       s.UnitPrice, s.Quantity, s.Unit, s.Description, s.ImagePath,
                       s.Status, s.CreatedDate, s.UpdatedDate
                FROM dbo.Seafoods s
                LEFT JOIN dbo.Categories c ON s.CategoryID = c.CategoryID
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
                            Unit = rd.IsDBNull(6) ? null : rd.GetString(6),
                            Description = rd.IsDBNull(7) ? null : rd.GetString(7),
                            ImagePath = rd.IsDBNull(8) ? null : rd.GetString(8),
                            Status = rd.IsDBNull(9) ? null : rd.GetString(9),
                            CreatedDate = rd.GetDateTime(10),
                            UpdatedDate = rd.IsDBNull(11) ? (DateTime?)null : rd.GetDateTime(11)
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
                       s.UnitPrice, s.Quantity, s.Unit, s.Description, s.ImagePath,
                       s.Status, s.CreatedDate, s.UpdatedDate
                FROM dbo.Seafoods s
                LEFT JOIN dbo.Categories c ON s.CategoryID = c.CategoryID
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
                            Unit = rd.IsDBNull(6) ? null : rd.GetString(6),
                            Description = rd.IsDBNull(7) ? null : rd.GetString(7),
                            ImagePath = rd.IsDBNull(8) ? null : rd.GetString(8),
                            Status = rd.IsDBNull(9) ? null : rd.GetString(9),
                            CreatedDate = rd.GetDateTime(10),
                            UpdatedDate = rd.IsDBNull(11) ? (DateTime?)null : rd.GetDateTime(11)
                        };
                    }
                }
            }
            return seafood;
        }

        public void Insert(SeafoodDTO seafood)
        {
            const string sql = @"
                INSERT INTO dbo.Seafoods (SeafoodName, CategoryID, UnitPrice, Quantity, Unit, Description, ImagePath, Status)
                VALUES (@SeafoodName, @CategoryID, @UnitPrice, @Quantity, @Unit, @Description, @ImagePath, @Status)";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SeafoodName", seafood.SeafoodName);
                cmd.Parameters.AddWithValue("@CategoryID", seafood.CategoryID);
                cmd.Parameters.AddWithValue("@UnitPrice", seafood.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", seafood.Quantity);
                cmd.Parameters.AddWithValue("@Unit", (object)seafood.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)seafood.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", (object)seafood.ImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)seafood.Status ?? "Active");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(SeafoodDTO seafood)
        {
            const string sql = @"
                UPDATE dbo.Seafoods
                SET SeafoodName = @SeafoodName,
                    CategoryID = @CategoryID,
                    UnitPrice = @UnitPrice,
                    Quantity = @Quantity,
                    Unit = @Unit,
                    Description = @Description,
                    ImagePath = @ImagePath,
                    Status = @Status,
                    UpdatedDate = GETDATE()
                WHERE SeafoodID = @SeafoodID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SeafoodID", seafood.SeafoodID);
                cmd.Parameters.AddWithValue("@SeafoodName", seafood.SeafoodName);
                cmd.Parameters.AddWithValue("@CategoryID", seafood.CategoryID);
                cmd.Parameters.AddWithValue("@UnitPrice", seafood.UnitPrice);
                cmd.Parameters.AddWithValue("@Quantity", seafood.Quantity);
                cmd.Parameters.AddWithValue("@Unit", (object)seafood.Unit ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Description", (object)seafood.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ImagePath", (object)seafood.ImagePath ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)seafood.Status ?? "Active");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Seafoods WHERE SeafoodID = @SeafoodID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SeafoodID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<SeafoodDTO> Search(string keyword)
        {
            var list = new List<SeafoodDTO>();
            const string sql = @"
                SELECT s.SeafoodID, s.SeafoodName, c.CategoryName, s.UnitPrice, s.Quantity
                FROM dbo.Seafoods s
                LEFT JOIN dbo.Categories c ON s.CategoryID = c.CategoryID
                WHERE s.SeafoodName LIKE '%' + @Keyword + '%' OR c.CategoryName LIKE '%' + @Keyword + '%'
                ORDER BY s.SeafoodName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SeafoodDTO
                        {
                            SeafoodID = rd.GetInt32(0),
                            SeafoodName = rd.GetString(1),
                            CategoryName = rd.IsDBNull(2) ? null : rd.GetString(2),
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
