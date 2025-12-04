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
            const string sql = @"SELECT TableID, TableName, ISNULL(BranchID, 0) AS BranchID, TableNumber, QrData, Capacity, Status, IsActive FROM dbo.Tables ORDER BY TableNumber";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new TableDTO
                        {
                            TableID = rd.GetInt32(0),
                            TableName = rd.GetString(1),
                            BranchID = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                            TableNumber = rd.GetInt32(3),
                            QrData = rd.IsDBNull(4) ? null : rd.GetString(4),
                            Capacity = rd.GetInt32(5),
                            Status = rd.IsDBNull(6) ? null : rd.GetString(6),
                            IsActive = rd.GetBoolean(7)
                        });
                    }
                }
            }
            return list;
        }

        public TableDTO GetById(int id)
        {
            TableDTO table = null;
            const string sql = @"SELECT TableID, TableName, ISNULL(BranchID, 0) AS BranchID, TableNumber, QrData, Capacity, Status, IsActive FROM dbo.Tables WHERE TableID = @TableID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
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
                            BranchID = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                            TableNumber = rd.GetInt32(3),
                            QrData = rd.IsDBNull(4) ? null : rd.GetString(4),
                            Capacity = rd.GetInt32(5),
                            Status = rd.IsDBNull(6) ? null : rd.GetString(6),
                            IsActive = rd.GetBoolean(7)
                        };
                    }
                }
            }
            return table;
        }

        public void Insert(TableDTO table)
        {
            const string sql = @"INSERT INTO dbo.Tables (TableName, BranchID, TableNumber, QrData, Capacity, Status, IsActive) 
                                 VALUES (@TableName, @BranchID, @TableNumber, @QrData, @Capacity, @Status, @IsActive)";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@BranchID", table.BranchID);
                cmd.Parameters.AddWithValue("@TableNumber", table.TableNumber);
                cmd.Parameters.AddWithValue("@QrData", (object)table.QrData ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
                cmd.Parameters.AddWithValue("@Status", (object)table.Status ?? "Available");
                cmd.Parameters.AddWithValue("@IsActive", table.IsActive);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(TableDTO table)
        {
            const string sql = @"UPDATE dbo.Tables 
                                 SET TableName = @TableName, BranchID = @BranchID, TableNumber = @TableNumber, QrData = @QrData, 
                                     Capacity = @Capacity, Status = @Status, IsActive = @IsActive
                                 WHERE TableID = @TableID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableID", table.TableID);
                cmd.Parameters.AddWithValue("@TableName", table.TableName);
                cmd.Parameters.AddWithValue("@BranchID", table.BranchID);
                cmd.Parameters.AddWithValue("@TableNumber", table.TableNumber);
                cmd.Parameters.AddWithValue("@QrData", (object)table.QrData ?? System.DBNull.Value);
                cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
                cmd.Parameters.AddWithValue("@Status", (object)table.Status ?? "Available");
                cmd.Parameters.AddWithValue("@IsActive", table.IsActive);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            const string sql = "DELETE FROM dbo.Tables WHERE TableID = @TableID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@TableID", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<TableDTO> Search(string keyword)
        {
            var list = new List<TableDTO>();
            const string sql = @"SELECT TableID, TableName, ISNULL(BranchID, 0) AS BranchID, TableNumber, QrData, Capacity, Status, IsActive 
                                 FROM dbo.Tables 
                                 WHERE TableName LIKE '%' + @Keyword + '%' OR CAST(TableNumber AS NVARCHAR(10)) LIKE '%' + @Keyword + '%' 
                                 ORDER BY TableNumber";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Keyword", keyword ?? string.Empty);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new TableDTO
                        {
                            TableID = rd.GetInt32(0),
                            TableName = rd.GetString(1),
                            BranchID = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                            TableNumber = rd.GetInt32(3),
                            QrData = rd.IsDBNull(4) ? null : rd.GetString(4),
                            Capacity = rd.GetInt32(5),
                            Status = rd.IsDBNull(6) ? null : rd.GetString(6),
                            IsActive = rd.GetBoolean(7)
                        });
                    }
                }
            }
            return list;
        }
    }
}
