using System.Collections.Generic;
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
            const string sql = "hs.sp_GetAllCategories";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1)
                        });
                    }
                }
            }
            return list;
        }

                public CategoryDTO GetById(int id)
        {
            CategoryDTO category = null;
                        const string sql = "hs.sp_GetCategoryById";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        category = new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1)
                        };
                    }
                }
            }
            return category;
        }

        public void Insert(CategoryDTO category)
        {
            const string sql = "hs.sp_InsertCategory";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(CategoryDTO category)
        {
            const string sql = "hs.sp_UpdateCategory";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "hs.sp_DeleteCategory";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<CategoryDTO> Search(string keyword)
        {
            var list = new List<CategoryDTO>();
            const string sql = "hs.sp_SearchCategories";

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
                        list.Add(new CategoryDTO
                        {
                            CategoryID = rd.GetInt32(0),
                            CategoryName = rd.GetString(1)
                        });
                    }
                }
            }
            return list;
        }
    }
}

