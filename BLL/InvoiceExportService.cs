using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QLTN_LT.DTO;
using QLTN_LT.DAL;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Dịch vụ xuất hóa đơn sang PDF, Excel, và in ấn
    /// </summary>
    public class InvoiceExportService
    {
        private readonly OrderBLL _orderBLL;
        private readonly OrderDAL _orderDAL;
        private readonly PaymentBLL _paymentBLL;

        public InvoiceExportService()
        {
            _orderBLL = new OrderBLL();
            _orderDAL = new OrderDAL();
            _paymentBLL = new PaymentBLL();
        }

        /// <summary>
        /// Xuất hóa đơn sang Excel
        /// </summary>
        public bool ExportToExcel(int orderId, string filePath)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return false;

                var sb = new StringBuilder();
                
                // Header
                sb.AppendLine("CÔNG TY QUẢN LÝ NHÀ HÀNG HẢI SẢN");
                sb.AppendLine("HÓA ĐƠN BÁN HÀNG");
                sb.AppendLine("=====================================");
                sb.AppendLine($"Số hóa đơn: {order.OrderID}");
                sb.AppendLine($"Ngày lập: {order.OrderDate:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Khách hàng: {order.CustomerName}");
                sb.AppendLine("=====================================");
                sb.AppendLine();

                // Items
                sb.AppendLine("STT | Sản phẩm | SL | Đơn giá | Thành tiền");
                sb.AppendLine("----+----------+----+---------+----------");
                
                decimal totalAmount = 0;
                int stt = 1;
                foreach (var item in order.OrderDetails)
                {
                    decimal itemTotal = item.Quantity * item.UnitPrice;
                    totalAmount += itemTotal;
                    sb.AppendLine($"{stt:D2}  | {item.SeafoodName,-20} | {item.Quantity:D3} | {item.UnitPrice:N0} | {itemTotal:N0}");
                    stt++;
                }

                sb.AppendLine("=====================================");
                sb.AppendLine($"Tổng tiền: {totalAmount:N0} VNĐ");
                
                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                decimal paidAmount = payments.Sum(p => p.Amount);
                sb.AppendLine($"Đã thanh toán: {paidAmount:N0} VNĐ");
                sb.AppendLine($"Còn lại: {(totalAmount - paidAmount):N0} VNĐ");
                sb.AppendLine($"Trạng thái: {order.Status}");
                sb.AppendLine("=====================================");
                sb.AppendLine($"Ghi chú: {order.Notes}");

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất Excel: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Xuất hóa đơn sang PDF (cần thêm library PDF)
        /// </summary>
        public bool ExportToPDF(int orderId, string filePath)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return false;

                // TODO: Cần thêm iTextSharp hoặc PdfSharp
                // Tạm thời xuất sang text
                return ExportToExcel(orderId, filePath.Replace(".pdf", ".txt"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất PDF: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Lấy dữ liệu hóa đơn để in ấn
        /// </summary>
        public InvoiceData GetInvoiceData(int orderId)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return null;

                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                decimal paidAmount = payments.Sum(p => p.Amount);
                decimal totalAmount = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);

                return new InvoiceData
                {
                    OrderId = order.OrderID,
                    OrderDate = order.OrderDate,
                    CustomerName = order.CustomerName,
                    Items = order.OrderDetails.ToList(),
                    TotalAmount = totalAmount,
                    PaidAmount = paidAmount,
                    RemainingAmount = totalAmount - paidAmount,
                    Status = order.Status,
                    Notes = order.Notes,
                    Payments = payments.ToList()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy dữ liệu hóa đơn: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Xuất báo cáo hóa đơn hàng loạt
        /// </summary>
        public bool ExportBatchInvoices(List<int> orderIds, string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                foreach (var orderId in orderIds)
                {
                    string fileName = $"HoaDon_{orderId}_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    string filePath = Path.Combine(folderPath, fileName);
                    ExportToExcel(orderId, filePath);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất hàng loạt: " + ex.Message);
                return false;
            }
        }
    }

    /// <summary>
    /// Model dữ liệu hóa đơn
    /// </summary>
    public class InvoiceData
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public List<OrderDetailDTO> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public List<PaymentDTO> Payments { get; set; }
    }
}
