using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QLTN_LT.DTO;
using QLTN_LT.DAL.Interfaces;

namespace QLTN_LT.DAL
{
    public class OrderDAL : IOrderRepository
    {
        public List<OrderDTO> GetAll(DateTime fromDate, DateTime toDate, string status, string keyword)
        {
            var list = new List<OrderDTO>();
            const string sql = "hs.sp_GetOrders";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);
                cmd.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(status) ? (object)DBNull.Value : status);
                cmd.Parameters.AddWithValue("@Keyword", string.IsNullOrEmpty(keyword) ? (object)DBNull.Value : keyword);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new OrderDTO
                        {
                            OrderID = rd.GetInt32(0),
                            OrderNumber = rd.GetString(1),
                            CustomerName = rd.GetString(2),
                            OrderDate = rd.GetDateTime(3),
                            TotalAmount = rd.GetDecimal(4),
                            Status = rd.GetString(5)
                        });
                    }
                }
            }
            return list;
        }

                public OrderDTO GetById(int id)
        {
            OrderDTO order = null;
            const string sql = "hs.sp_GetOrderById";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", id);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        order = new OrderDTO
                        {
                            OrderID = rd.GetInt32(0),
                            OrderNumber = rd.GetString(1),
                            CustomerName = rd.GetString(2),
                            OrderDate = rd.GetDateTime(3),
                            TotalAmount = rd.GetDecimal(4),
                            Status = rd.GetString(5)
                        };
                    }
                }
            }
            return order;
        }

        public int Insert(OrderDTO order)
        {
            const string sql = "hs.sp_InsertOrder";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerID", order.CustomerID);
                cmd.Parameters.AddWithValue("@TableID", order.TableID);
                cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                cmd.Parameters.AddWithValue("@Status", order.Status);
                conn.Open();
                return (int)cmd.ExecuteScalar(); // Returns the new OrderID
            }
        }

        public void UpdateStatus(int id, string status)
        {
            const string sql = "hs.sp_UpdateOrderStatus";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", id);
                cmd.Parameters.AddWithValue("@Status", status);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void AddDetail(OrderDetailDTO detail)
        {
            const string sql = "hs.sp_InsertOrderDetail";
            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", detail.OrderID);
                cmd.Parameters.AddWithValue("@SeafoodID", detail.SeafoodID);
                cmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                cmd.Parameters.AddWithValue("@UnitPrice", detail.UnitPrice);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

                public List<OrderDetailDTO> GetDetails(int orderId)
        {
            var list = new List<OrderDetailDTO>();
            const string sql = "hs.sp_GetOrderDetails";

            using (var conn = DatabaseHelper.CreateConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                conn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new OrderDetailDTO
                        {
                            OrderDetailID = rd.GetInt32(0),
                            OrderID = rd.GetInt32(1),
                            SeafoodName = rd.GetString(2),
                            Quantity = rd.GetInt32(3),
                            UnitPrice = rd.GetDecimal(4)
                        });
                    }
                }
            }
            return list;
        }
    }
}

