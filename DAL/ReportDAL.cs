using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;

namespace QLTN_LT.DAL
{
    public class ReportDAL
    {
        public List<DailyRevenueDTO> GetDailyRevenue(DateTime fromDate, DateTime toDate)
        {
            {
                var list = new List<DailyRevenueDTO>();
                const string sql = "hs.sp_GetDailyRevenue";

                using (var conn = DatabaseHelper.CreateConnection())
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@ToDate", toDate);
                    conn.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            list.Add(new DailyRevenueDTO
                            {
                                OrderDate = (DateTime)rd["OrderDate"],
                                OrderCount = (int)rd["OrderCount"],
                                TotalRevenue = (decimal)rd["TotalRevenue"]
                            });
                        }
                    }
                }
                return list;
            }
        }

        public int GetOrderCount(DateTime fromDate, DateTime toDate)
        {
            const string sql = "hs.sp_GetOrderCountByDateRange";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        public int GetNewCustomersCount(DateTime fromDate, DateTime toDate)
        {
            const string sql = "hs.sp_GetNewCustomersCountByDateRange";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

                public List<InventoryStatusDTO> GetInventoryStatusReport()
        {
            var list = new List<InventoryStatusDTO>();
            const string sql = "hs.sp_GetInventoryStatusReport";

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
                            ItemID = rd.GetInt32(0),
                            ItemName = rd.GetString(1),
                            QuantityRemaining = rd.GetInt32(2)
                        });
                    }
                }
            }
            return list;
        }

        public List<TopSellingItemDTO> GetTopSellingItems(int topCount, DateTime fromDate, DateTime toDate)
        {
            var list = new List<TopSellingItemDTO>();
            const string sql = "hs.sp_GetTopSellingSeafood";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TopCount", topCount);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new TopSellingItemDTO
                        {
                            ItemID = (int)rd["SeafoodID"],
                            ItemName = rd["SeafoodName"].ToString(),
                            CategoryName = rd["CategoryName"].ToString(),
                            TotalQuantitySold = (int)rd["TotalQuantitySold"],
                            TotalRevenue = (decimal)rd["TotalRevenue"]
                        });
                    }
                }
            }
            return list;
        }
    }
}

