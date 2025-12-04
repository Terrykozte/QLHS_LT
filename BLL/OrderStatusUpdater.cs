using System;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Order Status Updater - Cập nhật trạng thái đơn hàng dựa trên thanh toán
    /// </summary>
    public class OrderStatusUpdater
    {
        private readonly OrderBLL _orderBLL;
        private readonly PaymentBLL _paymentBLL;

        public OrderStatusUpdater()
        {
            _orderBLL = new OrderBLL();
            _paymentBLL = new PaymentBLL();
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng sau khi thanh toán
        /// </summary>
        public OrderStatusUpdateResult UpdateOrderStatus(int orderId)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null)
                {
                    return new OrderStatusUpdateResult
                    {
                        Success = false,
                        Message = "Order not found",
                        ErrorCode = "ORDER_NOT_FOUND"
                    };
                }

                var totalPaid = _paymentBLL.CalculateTotalPaid(orderId);
                var paymentStatus = DeterminePaymentStatus(order.TotalAmount, totalPaid);

                // Update payment status
                UpdateOrderPaymentStatus(orderId, paymentStatus);

                // Nếu thanh toán đầy đủ, hoàn thành đơn hàng
                if (paymentStatus == "Paid")
                {
                    _orderBLL.CompleteOrder(orderId);
                }

                return new OrderStatusUpdateResult
                {
                    Success = true,
                    Message = "Order status updated successfully",
                    OrderId = orderId,
                    PaymentStatus = paymentStatus,
                    TotalAmount = order.TotalAmount,
                    TotalPaid = totalPaid,
                    RemainingAmount = order.TotalAmount - totalPaid,
                    OrderStatus = paymentStatus == "Paid" ? "Completed" : order.Status
                };
            }
            catch (Exception ex)
            {
                return new OrderStatusUpdateResult
                {
                    Success = false,
                    Message = $"Error updating order status: {ex.Message}",
                    ErrorCode = "UPDATE_ERROR"
                };
            }
        }

        /// <summary>
        /// Xác định trạng thái thanh toán dựa trên số tiền đã thanh toán
        /// </summary>
        private string DeterminePaymentStatus(decimal totalAmount, decimal totalPaid)
        {
            if (totalPaid >= totalAmount)
                return "Paid";
            else if (totalPaid > 0)
                return "Partial";
            else
                return "Unpaid";
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán trong database
        /// </summary>
        private void UpdateOrderPaymentStatus(int orderId, string paymentStatus)
        {
            const string sql = @"
                UPDATE [dbo].[Orders]
                SET PaymentStatus = @PaymentStatus
                WHERE OrderID = @OrderID";

            using (var conn = DAL.DatabaseHelper.CreateConnection())
            using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết trạng thái đơn hàng
        /// </summary>
        public OrderStatusDetail GetOrderStatusDetail(int orderId)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null)
                    return null;

                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                var totalPaid = _paymentBLL.CalculateTotalPaid(orderId);

                return new OrderStatusDetail
                {
                    OrderId = orderId,
                    OrderNumber = order.OrderNumber,
                    OrderStatus = order.Status,
                    OrderDate = order.OrderDate,
                    CompletedDate = GetCompletedDate(orderId),
                    TotalAmount = order.TotalAmount,
                    TotalPaid = totalPaid,
                    RemainingAmount = order.TotalAmount - totalPaid,
                    PaymentStatus = DeterminePaymentStatus(order.TotalAmount, totalPaid),
                    PaymentCount = payments.Count,
                    Payments = payments,
                    CanBePaid = order.Status != "Cancelled" && totalPaid < order.TotalAmount,
                    IsFullyPaid = totalPaid >= order.TotalAmount,
                    IsPartiallyPaid = totalPaid > 0 && totalPaid < order.TotalAmount
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting order status detail: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Lấy ngày hoàn thành đơn hàng
        /// </summary>
        private DateTime? GetCompletedDate(int orderId)
        {
            const string sql = "SELECT CompletedDate FROM [dbo].[Orders] WHERE OrderID = @OrderID";
            using (var conn = DAL.DatabaseHelper.CreateConnection())
            using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OrderID", orderId);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return (DateTime)result;
                return null;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái tất cả đơn hàng chưa hoàn thành
        /// </summary>
        public int UpdateAllPendingOrders()
        {
            int updatedCount = 0;
            try
            {
                const string sql = "SELECT OrderID FROM [dbo].[Orders] WHERE Status != 'Completed' AND Status != 'Cancelled'";
                using (var conn = DAL.DatabaseHelper.CreateConnection())
                using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                {
                    conn.Open();
                    using (var rd = cmd.ExecuteReader())
                    {
                        var orderIds = new List<int>();
                        while (rd.Read())
                        {
                            orderIds.Add(rd.GetInt32(0));
                        }

                        foreach (var orderId in orderIds)
                        {
                            var result = UpdateOrderStatus(orderId);
                            if (result.Success)
                                updatedCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating pending orders: {ex.Message}");
            }

            return updatedCount;
        }
    }

    /// <summary>
    /// Order Status Update Result
    /// </summary>
    public class OrderStatusUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
    }

    /// <summary>
    /// Order Status Detail
    /// </summary>
    public class OrderStatusDetail
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentStatus { get; set; }
        public int PaymentCount { get; set; }
        public List<PaymentDTO> Payments { get; set; }
        public bool CanBePaid { get; set; }
        public bool IsFullyPaid { get; set; }
        public bool IsPartiallyPaid { get; set; }
    }
}

