using System;
using System.Collections.Generic;

using System.Threading.Tasks;

using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Payment Confirmation Service - Xác nhận thanh toán từ VietQR hoặc các cổng thanh toán khác
    /// </summary>
    public class PaymentConfirmationService
    {
        private readonly PaymentBLL _paymentBLL;
        private readonly OrderBLL _orderBLL;
        private readonly string _webhookSecret;

        public PaymentConfirmationService(string webhookSecret = "your-secret-key")
        {
            _paymentBLL = new PaymentBLL();
            _orderBLL = new OrderBLL();
            _webhookSecret = webhookSecret;
        }

        /// <summary>
        /// Xác nhận thanh toán từ webhook của VietQR
        /// </summary>
        public async Task<PaymentConfirmationResponse> ConfirmPaymentFromVietQRAsync(VietQRWebhookData webhookData)
        {
            try
            {
                // Validate webhook signature
                if (!ValidateWebhookSignature(webhookData))
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Invalid webhook signature",
                        ErrorCode = "INVALID_SIGNATURE"
                    };
                }

                // Parse transaction code to get order ID
                int orderId = ExtractOrderIdFromTransactionCode(webhookData.TransactionCode);
                if (orderId <= 0)
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Invalid transaction code format",
                        ErrorCode = "INVALID_TRANSACTION_CODE"
                    };
                }

                // Get order
                var order = _orderBLL.GetById(orderId);
                if (order == null)
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Order not found",
                        ErrorCode = "ORDER_NOT_FOUND"
                    };
                }

                // Validate amount
                if (webhookData.Amount != (long)order.TotalAmount)
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Payment amount mismatch",
                        ErrorCode = "AMOUNT_MISMATCH",
                        OrderId = orderId,
                        ReceivedAmount = webhookData.Amount,
                        ExpectedAmount = (long)order.TotalAmount
                    };
                }

                // Create payment record
                var payment = new PaymentDTO
                {
                    OrderID = orderId,
                    PaymentMethodID = 3, // VietQR
                    Amount = webhookData.Amount,
                    PaymentDate = DateTime.Now,
                    TransactionCode = webhookData.TransactionCode,
                    BankAccount = webhookData.BankAccount,
                    BankName = webhookData.BankName,
                    Status = "Completed",
                    Notes = $"VietQR Payment - {webhookData.Description}"
                };

                int paymentId = _paymentBLL.CreatePayment(payment);

                // Update order payment status
                UpdateOrderPaymentStatus(orderId);

                return new PaymentConfirmationResponse
                {
                    Success = true,
                    Message = "Payment confirmed successfully",
                    OrderId = orderId,
                    PaymentId = paymentId,
                    TransactionCode = webhookData.TransactionCode,
                    Amount = webhookData.Amount,
                    PaymentDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new PaymentConfirmationResponse
                {
                    Success = false,
                    Message = $"Error confirming payment: {ex.Message}",
                    ErrorCode = "CONFIRMATION_ERROR"
                };
            }
        }

        /// <summary>
        /// Xác nhận thanh toán thủ công
        /// </summary>
        public PaymentConfirmationResponse ConfirmPaymentManual(int orderId, int paymentMethodId, decimal amount, string transactionCode = null)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null)
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Order not found",
                        ErrorCode = "ORDER_NOT_FOUND"
                    };
                }

                if (amount <= 0)
                {
                    return new PaymentConfirmationResponse
                    {
                        Success = false,
                        Message = "Invalid payment amount",
                        ErrorCode = "INVALID_AMOUNT"
                    };
                }

                var payment = new PaymentDTO
                {
                    OrderID = orderId,
                    PaymentMethodID = paymentMethodId,
                    Amount = amount,
                    PaymentDate = DateTime.Now,
                    TransactionCode = transactionCode ?? GenerateTransactionCode(orderId),
                    Status = "Completed"
                };

                int paymentId = _paymentBLL.CreatePayment(payment);

                // Update order payment status
                UpdateOrderPaymentStatus(orderId);

                return new PaymentConfirmationResponse
                {
                    Success = true,
                    Message = "Payment confirmed successfully",
                    OrderId = orderId,
                    PaymentId = paymentId,
                    TransactionCode = payment.TransactionCode,
                    Amount = amount,
                    PaymentDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                return new PaymentConfirmationResponse
                {
                    Success = false,
                    Message = $"Error confirming payment: {ex.Message}",
                    ErrorCode = "CONFIRMATION_ERROR"
                };
            }
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán của đơn hàng
        /// </summary>
        private void UpdateOrderPaymentStatus(int orderId)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                var totalPaid = _paymentBLL.CalculateTotalPaid(orderId);

                string paymentStatus;
                if (totalPaid >= order.TotalAmount)
                {
                    paymentStatus = "Paid";
                    // Tự động hoàn thành đơn hàng nếu thanh toán đầy đủ
                    _orderBLL.CompleteOrder(orderId);
                }
                else if (totalPaid > 0)
                {
                    paymentStatus = "Partial";
                }
                else
                {
                    paymentStatus = "Unpaid";
                }

                // Update payment status in database
                UpdateOrderPaymentStatusInDB(orderId, paymentStatus);
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                System.Diagnostics.Debug.WriteLine($"Error updating order payment status: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái thanh toán trong database
        /// </summary>
        private void UpdateOrderPaymentStatusInDB(int orderId, string paymentStatus)
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
        /// Validate webhook signature
        /// </summary>
        private bool ValidateWebhookSignature(VietQRWebhookData webhookData)
        {
            // TODO: Implement signature validation based on VietQR documentation
            // For now, return true (implement proper validation in production)
            return true;
        }

        /// <summary>
        /// Extract order ID from transaction code
        /// Format: TXN_ORD{OrderID}_{Timestamp}
        /// </summary>
        private int ExtractOrderIdFromTransactionCode(string transactionCode)
        {
            try
            {
                if (string.IsNullOrEmpty(transactionCode))
                    return -1;

                // Format: TXN_ORD123_20240102120000
                var parts = transactionCode.Split('_');
                if (parts.Length >= 2 && parts[1].StartsWith("ORD"))
                {
                    var orderPart = parts[1].Substring(3); // Remove "ORD"
                    if (int.TryParse(orderPart, out int orderId))
                        return orderId;
                }

                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Generate transaction code
        /// </summary>
        private string GenerateTransactionCode(int orderId)
        {
            return $"TXN_ORD{orderId}_{DateTime.Now:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// Lấy trạng thái thanh toán của đơn hàng
        /// </summary>
        public PaymentStatusInfo GetPaymentStatus(int orderId)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null)
                    return null;

                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                var totalPaid = _paymentBLL.CalculateTotalPaid(orderId);
                var remaining = order.TotalAmount - totalPaid;

                return new PaymentStatusInfo
                {
                    OrderId = orderId,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = order.TotalAmount,
                    TotalPaid = totalPaid,
                    RemainingAmount = remaining,
                    PaymentStatus = _orderBLL.GetPaymentStatus(orderId),
                    Payments = payments,
                    PaymentCount = payments.Count,
                    LastPaymentDate = payments.Count > 0 ? payments[0].PaymentDate : (DateTime?)null
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting payment status: {ex.Message}");
                return null;
            }
        }
    }

    /// <summary>
    /// VietQR Webhook Data
    /// </summary>
    public class VietQRWebhookData
    {
        public string TransactionCode { get; set; }
        public long Amount { get; set; }
        public string BankAccount { get; set; }
        public string BankName { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Signature { get; set; }
    }

    /// <summary>
    /// Payment Confirmation Response
    /// </summary>
    public class PaymentConfirmationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    /// <summary>
    /// Payment Status Info
    /// </summary>
    public class PaymentStatusInfo
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal RemainingAmount { get; set; }
        public string PaymentStatus { get; set; }
        public List<PaymentDTO> Payments { get; set; }
        public int PaymentCount { get; set; }
        public DateTime? LastPaymentDate { get; set; }
    }
}

