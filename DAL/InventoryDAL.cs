using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class InventoryDAL : IInventoryRepository
    {
        public List<InventoryStatusDTO> GetInventoryStatus()
        {
            var list = new List<InventoryStatusDTO>();
            const string sql = "sp_GetInventoryStatus";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new InventoryStatusDTO
                        {
                            InventoryID = rd.GetInt32(0),
                            SeafoodID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            Quantity = rd.GetInt32(3),
                            ReorderLevel = rd.GetInt32(4),
                            Status = rd.GetString(5),
                            LastUpdated = rd.GetDateTime(6)
                        });
                    }
                }
            }
            return list;
        }

        public InventoryDTO GetBySeafoodId(int seafoodId)
        {
            InventoryDTO inventory = null;
            const string sql = @"
                SELECT i.InventoryID, i.SeafoodID, s.SeafoodName, i.Quantity, i.ReorderLevel, i.LastUpdated
                FROM [dbo].[Inventory] i
                INNER JOIN [dbo].[Seafoods] s ON i.SeafoodID = s.SeafoodID
                WHERE i.SeafoodID = @SeafoodID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@SeafoodID", seafoodId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        inventory = new InventoryDTO
                        {
                            InventoryID = rd.GetInt32(0),
                            SeafoodID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            Quantity = rd.GetInt32(3),
                            ReorderLevel = rd.GetInt32(4),
                            LastUpdated = rd.GetDateTime(5)
                        };
                    }
                }
            }
            return inventory;
        }

        public InventoryDTO GetByInventoryId(int inventoryId)
        {
            InventoryDTO inventory = null;
            const string sql = @"
                SELECT i.InventoryID, i.SeafoodID, s.SeafoodName, i.Quantity, i.ReorderLevel, i.LastUpdated
                FROM [dbo].[Inventory] i
                INNER JOIN [dbo].[Seafoods] s ON i.SeafoodID = s.SeafoodID
                WHERE i.InventoryID = @InventoryID";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        inventory = new InventoryDTO
                        {
                            InventoryID = rd.GetInt32(0),
                            SeafoodID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            Quantity = rd.GetInt32(3),
                            ReorderLevel = rd.GetInt32(4),
                            LastUpdated = rd.GetDateTime(5)
                        };
                    }
                }
            }
            return inventory;
        }

        public int InsertTransaction(InventoryTransactionDTO transaction)
        {
            const string sql = "sp_InsertInventoryTransaction";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InventoryID", transaction.InventoryID);
                cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                cmd.Parameters.AddWithValue("@Quantity", transaction.Quantity);
                cmd.Parameters.AddWithValue("@Reason", string.IsNullOrEmpty(transaction.Reason) ? (object)DBNull.Value : transaction.Reason);
                cmd.Parameters.AddWithValue("@SupplierID", transaction.SupplierID.HasValue ? (object)transaction.SupplierID : DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", transaction.CreatedBy.HasValue ? (object)transaction.CreatedBy : DBNull.Value);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public List<InventoryTransactionDTO> GetTransactions(int inventoryId, int pageNumber = 1, int pageSize = 50)
        {
            var list = new List<InventoryTransactionDTO>();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                SELECT it.TransactionID, it.InventoryID, s.SeafoodName, it.TransactionType, it.Quantity, 
                       it.Reason, it.SupplierID, ISNULL(sup.SupplierName, ''), it.TransactionDate, it.CreatedBy
                FROM [dbo].[InventoryTransactions] it
                INNER JOIN [dbo].[Inventory] i ON it.InventoryID = i.InventoryID
                INNER JOIN [dbo].[Seafoods] s ON i.SeafoodID = s.SeafoodID
                LEFT JOIN [dbo].[Suppliers] sup ON it.SupplierID = sup.SupplierID
                WHERE it.InventoryID = @InventoryID
                ORDER BY it.TransactionDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                cmd.Parameters.AddWithValue("@Offset", offset);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new InventoryTransactionDTO
                        {
                            TransactionID = rd.GetInt32(0),
                            InventoryID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            TransactionType = rd.GetString(3),
                            Quantity = rd.GetInt32(4),
                            Reason = rd.IsDBNull(5) ? null : rd.GetString(5),
                            SupplierID = rd.IsDBNull(6) ? (int?)null : rd.GetInt32(6),
                            SupplierName = rd.GetString(7),
                            TransactionDate = rd.GetDateTime(8),
                            CreatedBy = rd.IsDBNull(9) ? (int?)null : rd.GetInt32(9)
                        });
                    }
                }
            }
            return list;
        }

        public List<InventoryTransactionDTO> GetTransactionsByDateRange(DateTime fromDate, DateTime toDate)
        {
            var list = new List<InventoryTransactionDTO>();
            const string sql = @"
                SELECT it.TransactionID, it.InventoryID, s.SeafoodName, it.TransactionType, it.Quantity, 
                       it.Reason, it.SupplierID, ISNULL(sup.SupplierName, ''), it.TransactionDate, it.CreatedBy
                FROM [dbo].[InventoryTransactions] it
                INNER JOIN [dbo].[Inventory] i ON it.InventoryID = i.InventoryID
                INNER JOIN [dbo].[Seafoods] s ON i.SeafoodID = s.SeafoodID
                LEFT JOIN [dbo].[Suppliers] sup ON it.SupplierID = sup.SupplierID
                WHERE it.TransactionDate >= @FromDate AND it.TransactionDate <= @ToDate
                ORDER BY it.TransactionDate DESC";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new InventoryTransactionDTO
                        {
                            TransactionID = rd.GetInt32(0),
                            InventoryID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            TransactionType = rd.GetString(3),
                            Quantity = rd.GetInt32(4),
                            Reason = rd.IsDBNull(5) ? null : rd.GetString(5),
                            SupplierID = rd.IsDBNull(6) ? (int?)null : rd.GetInt32(6),
                            SupplierName = rd.GetString(7),
                            TransactionDate = rd.GetDateTime(8),
                            CreatedBy = rd.IsDBNull(9) ? (int?)null : rd.GetInt32(9)
                        });
                    }
                }
            }
            return list;
        }

        public int GetTotalTransactionCount(int inventoryId)
        {
            const string sql = "SELECT COUNT(*) FROM [dbo].[InventoryTransactions] WHERE InventoryID = @InventoryID";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}
