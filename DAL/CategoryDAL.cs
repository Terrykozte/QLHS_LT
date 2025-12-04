using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class CategoryDAL : ICategoryRepository
    {
        public List<CategoryDTO> GetAll()
        {
            var list = new List<CategoryDTO>();
            const string sql = @"SELECT CategoryID, CategoryName, Description, CreatedDate, UpdatedDate, Status FROM dbo.Categories ORDER BY CategoryName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1),
                            Description = rd.IsDBNull(2) ? null : rd.GetString(2),
                            CreatedDate = rd.GetDateTime(3),
                            UpdatedDate = rd.IsDBNull(4) ? (System.DateTime?)null : rd.GetDateTime(4),
                            Status = rd.IsDBNull(5) ? null : rd.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

        public CategoryDTO GetById(int id)
        {
            CategoryDTO category = null;
            const string sql = @"SELECT CategoryID, CategoryName, Description, CreatedDate, UpdatedDate, Status FROM dbo.Categories WHERE CategoryID = @CategoryID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        category = new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1),
                            Description = rd.IsDBNull(2) ? null : rd.GetString(2),
                            CreatedDate = rd.GetDateTime(3),
                            UpdatedDate = rd.IsDBNull(4) ? (System.DateTime?)null : rd.GetDateTime(4),
                            Status = rd.IsDBNull(5) ? null : rd.GetString(5)
                        };
                    }
                }
            }
            return category;
        }

        public void Insert(CategoryDTO category)
        {
            const string sql = @"INSERT INTO dbo.Categories (CategoryName, Description, Status) VALUES (@CategoryName, @Description, @Status)";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)category.Status ?? "Active");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(CategoryDTO category)
        {
            const string sql = @"UPDATE dbo.Categories SET CategoryName = @CategoryName, Description = @Description, Status = @Status, UpdatedDate = GETDATE() WHERE CategoryID = @CategoryID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                cmd.Parameters.AddWithValue("@Description", (object)category.Description ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", (object)category.Status ?? "Active");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Categories WHERE CategoryID = @CategoryID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CategoryID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<CategoryDTO> Search(string keyword)
        {
            var list = new List<CategoryDTO>();
            const string sql = @"SELECT CategoryID, CategoryName, Description, CreatedDate, UpdatedDate, Status FROM dbo.Categories WHERE CategoryName LIKE '%' + @Keyword + '%' ORDER BY CategoryName";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1),
                            Description = rd.IsDBNull(2) ? null : rd.GetString(2),
                            CreatedDate = rd.GetDateTime(3),
                            UpdatedDate = rd.IsDBNull(4) ? (System.DateTime?)null : rd.GetDateTime(4),
                            Status = rd.IsDBNull(5) ? null : rd.GetString(5)
                        });
                    }
                }
            }
            return list;
        }
    }
}
