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
        public List<InventoryTransactionDTO> GetAll(DateTime fromDate, DateTime toDate, string type, string keyword)
        {
            var list = new List<InventoryTransactionDTO>();
            const string sql = "hs.sp_GetInventoryTransactions";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@TransactionType", string.IsNullOrEmpty(type) ? (object)DBNull.Value : type);
                cmd.Parameters.AddWithValue("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new InventoryTransactionDTO
                        {
                            InventoryID = (int)rd["InventoryID"],
                            ItemID = (int)rd["ItemID"],
                            ItemName = rd["ItemName"].ToString(),
                            QuantityIn = (int)rd["QuantityIn"],
                            QuantityOut = (int)rd["QuantityOut"],
                            QuantityRemaining = (int)rd["QuantityRemaining"],
                            TransactionDate = (DateTime)rd["TransactionDate"],
                            TransactionType = rd["TransactionType"].ToString(),
                            Notes = rd["Notes"].ToString(),
                            ProcessedBy = rd["ProcessedBy"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void Insert(InventoryTransactionDTO transaction)
        {
            const string sql = "hs.sp_InsertInventoryTransaction";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ItemID", transaction.ItemID);
                cmd.Parameters.AddWithValue("@QuantityIn", transaction.QuantityIn);
                cmd.Parameters.AddWithValue("@QuantityOut", transaction.QuantityOut);
                cmd.Parameters.AddWithValue("@TransactionType", transaction.TransactionType);
                cmd.Parameters.AddWithValue("@Notes", transaction.Notes);
                cmd.Parameters.AddWithValue("@ProcessedBy", transaction.ProcessedBy);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

