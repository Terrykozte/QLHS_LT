using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Service quản lý thanh toán toàn diện
    /// </summary>
    public class PaymentManagementService
    {
        private readonly PaymentDAL _paymentDAL = new PaymentDAL();
        private readonly OrderDAL _orderDAL = new OrderDAL();
        private readonly VietQRService _vietQRService = new VietQRService();

        /// <summary>
        /// Tạo thanh toán mới
        /// </summary>
        public Result<PaymentDTO> CreatePayment(long orderId, decimal amount, string paymentMethod, string transactionCode = null)
        {
            try
            {
                // Kiểm tra đơn hàng
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<PaymentDTO> { Success = false, Message = "Đơn hàng không tồn tại" };

                // Kiểm tra số tiền
                if (amount <= 0)
                    return new Result<PaymentDTO> { Success = false, Message = "Số tiền không hợp lệ" };

                if (amount > order.TotalAmount)
                    return new Result<PaymentDTO> { Success = false, Message = "Số tiền vượt quá tổng đơn hàng" };

                var payment = new PaymentDTO
                {
                    OrderID = orderId,
                    Amount = amount,
                    PaymentMethod = paymentMethod,
                    TransactionCode = transactionCode,
                    PaymentDate = DateTime.Now,
                    Status = "Pending"
                };

                var result = _paymentDAL.AddPayment(payment);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<PaymentDTO> { Success = false, Message = $"Lỗi tạo thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Xác nhận thanh toán
        /// </summary>
        public Result<bool> ConfirmPayment(long paymentId)
        {
            try
            {
                var payment = _paymentDAL.GetPaymentById(paymentId);
                if (payment == null)
                    return new Result<bool> { Success = false, Message = "Thanh toán không tồn tại" };

                var result = _paymentDAL.UpdatePaymentStatus(paymentId, "Completed");
                
                if (result.Success)
                {
                    // Kiểm tra xem đơn hàng đã thanh toán đủ chưa
                    CheckAndUpdateOrderPaymentStatus(payment.OrderID);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi xác nhận thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Hủy thanh toán
        /// </summary>
        public Result<bool> CancelPayment(long paymentId)
        {
            try
            {
                var payment = _paymentDAL.GetPaymentById(paymentId);
                if (payment == null)
                    return new Result<bool> { Success = false, Message = "Thanh toán không tồn tại" };

                if (payment.Status == "Completed")
                    return new Result<bool> { Success = false, Message = "Không thể hủy thanh toán đã hoàn tất" };

                var result = _paymentDAL.UpdatePaymentStatus(paymentId, "Cancelled");
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi hủy thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Hoàn tiền
        /// </summary>
        public Result<bool> RefundPayment(long paymentId, decimal refundAmount, string reason = null)
        {
            try
            {
                var payment = _paymentDAL.GetPaymentById(paymentId);
                if (payment == null)
                    return new Result<bool> { Success = false, Message = "Thanh toán không tồn tại" };

                if (payment.Status != "Completed")
                    return new Result<bool> { Success = false, Message = "Chỉ có thể hoàn tiền cho thanh toán đã hoàn tất" };

                if (refundAmount > payment.Amount)
                    return new Result<bool> { Success = false, Message = "Số tiền hoàn vượt quá số tiền thanh toán" };

                // Tạo ghi nhận hoàn tiền
                var refund = new PaymentDTO
                {
                    OrderID = payment.OrderID,
                    Amount = -refundAmount,
                    PaymentMethod = payment.PaymentMethod,
                    PaymentDate = DateTime.Now,
                    Status = "Refund",
                    Note = $"Hoàn tiền: {reason}"
                };

                var result = _paymentDAL.AddPayment(refund);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi hoàn tiền: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách thanh toán theo đơn hàng
        /// </summary>
        public Result<List<PaymentDTO>> GetPaymentsByOrder(long orderId)
        {
            try
            {
                var result = _paymentDAL.GetPaymentsByOrderId(orderId);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<PaymentDTO>> { Success = false, Message = $"Lỗi lấy danh sách thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách thanh toán theo trạng thái
        /// </summary>
        public Result<List<PaymentDTO>> GetPaymentsByStatus(string status, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _paymentDAL.GetPaymentsByStatus(status, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<PaymentDTO>> { Success = false, Message = $"Lỗi lấy danh sách thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách thanh toán theo phương thức
        /// </summary>
        public Result<List<PaymentDTO>> GetPaymentsByMethod(string method, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var result = _paymentDAL.GetPaymentsByMethod(method, fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<PaymentDTO>> { Success = false, Message = $"Lỗi lấy danh sách thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tính tổng thanh toán
        /// </summary>
        public Result<decimal> CalculateTotalPayments(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var result = _paymentDAL.CalculateTotalPayments(fromDate, toDate);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<decimal> { Success = false, Message = $"Lỗi tính tổng thanh toán: {ex.Message}" };
            }
        }

        /// <summary>
        /// Tạo thanh toán VietQR
        /// </summary>
        public Result<PaymentDTO> CreateVietQRPayment(long orderId, decimal amount, string bankCode, string accountNo)
        {
            try
            {
                // Tạo QR code
                var qrResult = _vietQRService.GenerateQRCode(amount, bankCode, accountNo);
                if (!qrResult.Success)
                    return new Result<PaymentDTO> { Success = false, Message = qrResult.Message };

                var payment = new PaymentDTO
                {
                    OrderID = orderId,
                    Amount = amount,
                    PaymentMethod = "VietQR",
                    PaymentDate = DateTime.Now,
                    Status = "Pending",
                    Note = qrResult.Data
                };

                var result = _paymentDAL.AddPayment(payment);
                return result;
            }
            catch (Exception ex)
            {
                return new Result<PaymentDTO> { Success = false, Message = $"Lỗi tạo thanh toán VietQR: {ex.Message}" };
            }
        }

        /// <summary>
        /// Kiểm tra và cập nhật trạng thái thanh toán đơn hàng
        /// </summary>
        private void CheckAndUpdateOrderPaymentStatus(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null) return;

                var payments = _paymentDAL.GetPaymentsByOrderId(orderId);
                if (payments.Data == null || payments.Data.Count == 0) return;

                decimal totalPaid = payments.Data
                    .Where(p => p.Status == "Completed")
                    .Sum(p => p.Amount);

                if (totalPaid >= order.TotalAmount)
                {
                    // Đơn hàng đã thanh toán đủ
                    _orderDAL.UpdateOrderStatus(orderId, "Completed");
                }
            }
            catch { }
        }

        /// <summary>
        /// Lấy thống kê thanh toán
        /// </summary>
        public Result<Dictionary<string, object>> GetPaymentStatistics(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var stats = new Dictionary<string, object>();

                // Tổng thanh toán
                var totalResult = CalculateTotalPayments(fromDate, toDate);
                stats["TotalAmount"] = totalResult.Data;

                // Thanh toán theo phương thức
                var methods = new[] { "Cash", "Card", "VietQR", "Transfer" };
                var methodStats = new Dictionary<string, decimal>();

                foreach (var method in methods)
                {
                    var methodPayments = GetPaymentsByMethod(method, fromDate, toDate);
                    decimal total = methodPayments.Data?.Sum(p => p.Amount) ?? 0;
                    methodStats[method] = total;
                }

                stats["ByMethod"] = methodStats;

                // Thanh toán theo trạng thái
                var completedPayments = GetPaymentsByStatus("Completed", fromDate, toDate);
                var pendingPayments = GetPaymentsByStatus("Pending", fromDate, toDate);

                stats["Completed"] = completedPayments.Data?.Count ?? 0;
                stats["Pending"] = pendingPayments.Data?.Count ?? 0;

                return new Result<Dictionary<string, object>> { Success = true, Data = stats };
            }
            catch (Exception ex)
            {
                return new Result<Dictionary<string, object>> { Success = false, Message = $"Lỗi lấy thống kê: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy danh sách phương thức thanh toán
        /// </summary>
        public Result<List<PaymentMethodDTO>> GetPaymentMethods()
        {
            try
            {
                var result = _paymentDAL.GetPaymentMethods();
                return result;
            }
            catch (Exception ex)
            {
                return new Result<List<PaymentMethodDTO>> { Success = false, Message = $"Lỗi lấy danh sách phương thức: {ex.Message}" };
            }
        }

        /// <summary>
        /// Kiểm tra xem đơn hàng đã thanh toán đủ chưa
        /// </summary>
        public Result<bool> IsOrderFullyPaid(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<bool> { Success = false, Message = "Đơn hàng không tồn tại" };

                var payments = _paymentDAL.GetPaymentsByOrderId(orderId);
                if (payments.Data == null || payments.Data.Count == 0)
                    return new Result<bool> { Success = true, Data = false };

                decimal totalPaid = payments.Data
                    .Where(p => p.Status == "Completed")
                    .Sum(p => p.Amount);

                bool isPaid = totalPaid >= order.TotalAmount;
                return new Result<bool> { Success = true, Data = isPaid };
            }
            catch (Exception ex)
            {
                return new Result<bool> { Success = false, Message = $"Lỗi kiểm tra: {ex.Message}" };
            }
        }

        /// <summary>
        /// Lấy số tiền còn phải thanh toán
        /// </summary>
        public Result<decimal> GetRemainingAmount(long orderId)
        {
            try
            {
                var order = _orderDAL.GetOrderById(orderId);
                if (order == null)
                    return new Result<decimal> { Success = false, Message = "Đơn hàng không tồn tại" };

                var payments = _paymentDAL.GetPaymentsByOrderId(orderId);
                decimal totalPaid = payments.Data?.Where(p => p.Status == "Completed").Sum(p => p.Amount) ?? 0;

                decimal remaining = order.TotalAmount - totalPaid;
                return new Result<decimal> { Success = true, Data = Math.Max(0, remaining) };
            }
            catch (Exception ex)
            {
                return new Result<decimal> { Success = false, Message = $"Lỗi tính toán: {ex.Message}" };
            }
        }
    }
}

