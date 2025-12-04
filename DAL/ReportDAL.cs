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
            var list = new List<DailyRevenueDTO>();
            const string sql = @"
                SELECT CAST(o.OrderDate AS DATE) AS OrderDate,
                       COUNT(*) AS OrderCount,
                       SUM(o.TotalAmount) AS TotalRevenue
                FROM dbo.Orders o
                WHERE o.OrderDate >= @FromDate AND o.OrderDate < @ToDate
                  AND o.Status = 'Completed'
                GROUP BY CAST(o.OrderDate AS DATE)
                ORDER BY CAST(o.OrderDate AS DATE)";

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
                        list.Add(new DailyRevenueDTO
                        {
                            OrderDate = rd.GetDateTime(0),
                            OrderCount = rd.GetInt32(1),
                            TotalRevenue = rd.GetDecimal(2)
                        });
                    }
                }
            }
            return list;
        }

        public int GetOrderCount(DateTime fromDate, DateTime toDate)
        {
            const string sql = @"SELECT COUNT(*) FROM dbo.Orders WHERE OrderDate >= @FromDate AND OrderDate < @ToDate";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);
            }
        }

        public int GetNewCustomersCount(DateTime fromDate, DateTime toDate)
        {
            const string sql = @"SELECT COUNT(*) FROM dbo.Customers WHERE CreatedDate >= @FromDate AND CreatedDate < @ToDate";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
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
            const string sql = @"
                SELECT i.InventoryID AS ItemID, s.SeafoodName AS ItemName, i.Quantity AS QuantityRemaining
                FROM dbo.Inventory i
                INNER JOIN dbo.Seafoods s ON i.SeafoodID = s.SeafoodID
                ORDER BY i.Quantity ASC";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
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
            const string sql = @"
                SELECT TOP (@TopCount)
                       s.SeafoodID AS ItemID,
                       s.SeafoodName AS ItemName,
                       c.CategoryName,
                       SUM(od.Quantity) AS TotalQuantitySold,
                       SUM(od.Quantity * od.UnitPrice) AS TotalRevenue
                FROM dbo.OrderDetails od
                INNER JOIN dbo.Orders o ON od.OrderID = o.OrderID
                INNER JOIN dbo.Seafoods s ON od.SeafoodID = s.SeafoodID
                LEFT JOIN dbo.Categories c ON s.CategoryID = c.CategoryID
                WHERE o.OrderDate >= @FromDate AND o.OrderDate < @ToDate AND o.Status = 'Completed'
                GROUP BY s.SeafoodID, s.SeafoodName, c.CategoryName
                ORDER BY TotalQuantitySold DESC, TotalRevenue DESC";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
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
                            ItemID = rd.GetInt32(0),
                            ItemName = rd.GetString(1),
                            CategoryName = rd.IsDBNull(2) ? null : rd.GetString(2),
                            TotalQuantitySold = rd.GetInt32(3),
                            TotalRevenue = rd.GetDecimal(4)
                        });
                    }
                }
            }
            return list;
        }
    }
}
