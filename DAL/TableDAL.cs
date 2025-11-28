using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class TableDAL : ITableRepository
    {
        public List<TableDTO> GetAll()
        {
            var list = new List<TableDTO>();
            const string sql = "hs.sp_GetAllTables";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new TableDTO
                        {
                            TableID = rd.GetInt32(0),
                            TableName = rd.GetString(1),
                            Status = rd.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

                public TableDTO GetById(int id)
        {
            TableDTO table = null;
            const string sql = "hs.sp_GetTableById";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        table = new TableDTO
                        {
                            TableID = rd.GetInt32(0),
                            TableName = rd.GetString(1),
                            Status = rd.GetString(2)
                        };
                    }
                }
            }
            return table;
        }

        public void Insert(TableDTO table)
        {
            const string sql = "hs.sp_InsertTable";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@Status", table.Status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(TableDTO table)
        {
            const string sql = "hs.sp_UpdateTable";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableID", table.TableID);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@Status", table.Status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "hs.sp_DeleteTable";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TableID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<TableDTO> Search(string keyword)
        {
            var list = new List<TableDTO>();
            const string sql = "hs.sp_SearchTables";

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
                        list.Add(new TableDTO
                        {
                            TableID = rd.GetInt32(0),
                            TableName = rd.GetString(1),
                            Status = rd.GetString(2)
                        });
                    }
                }
            }
            return list;
        }
    }
}
