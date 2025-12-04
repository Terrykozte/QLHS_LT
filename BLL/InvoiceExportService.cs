using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
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
        /// Xuất hóa đơn dạng văn bản (TXT)
        /// </summary>
        public bool ExportToExcel(int orderId, string filePath)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return false;

                // Xuất HTML table dưới phần mở rộng .xls để Excel mở trực tiếp
                var sb = new StringBuilder();
                sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                sb.AppendLine("<h2 style='font-family:Segoe UI'>HÓA ĐƠN BÁN HÀNG</h2>");
                sb.AppendLine("<table style='font-family:Segoe UI; font-size:12pt'>");
                sb.AppendLine($"<tr><td><b>Số hóa đơn:</b></td><td>{order.OrderID}</td></tr>");
                sb.AppendLine($"<tr><td><b>Ngày lập:</b></td><td>{order.OrderDate:dd/MM/yyyy HH:mm}</td></tr>");
                sb.AppendLine($"<tr><td><b>Khách hàng:</b></td><td>{Html(order.CustomerName)}</td></tr>");
                sb.AppendLine("</table><br/>");

                sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse; font-family:Segoe UI; font-size:11pt'>");
                sb.AppendLine("<tr style='background:#e5e7eb'><th>STT</th><th>Sản phẩm</th><th>SL</th><th>Đơn giá</th><th>Thành tiền</th></tr>");
                
                decimal totalAmount = 0; int stt = 1;
                foreach (var item in order.OrderDetails)
                {
                    decimal itemTotal = item.Quantity * item.UnitPrice; totalAmount += itemTotal;
                    sb.AppendLine($"<tr><td align='center'>{stt}</td><td>{Html(item.SeafoodName)}</td><td align='right'>{item.Quantity}</td><td align='right'>{item.UnitPrice:N0}</td><td align='right'>{itemTotal:N0}</td></tr>");
                    stt++;
                }
                sb.AppendLine("</table><br/>");
                
                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                decimal paidAmount = payments.Sum(p => p.Amount);

                sb.AppendLine("<table style='font-family:Segoe UI; font-size:12pt'>");
                sb.AppendLine($"<tr><td><b>Tổng tiền:</b></td><td>{totalAmount:N0} VNĐ</td></tr>");
                sb.AppendLine($"<tr><td><b>Đã thanh toán:</b></td><td>{paidAmount:N0} VNĐ</td></tr>");
                sb.AppendLine($"<tr><td><b>Còn lại:</b></td><td>{(totalAmount - paidAmount):N0} VNĐ</td></tr>");
                sb.AppendLine($"<tr><td><b>Trạng thái:</b></td><td>{Html(order.Status)}</td></tr>");
                sb.AppendLine($"<tr><td><b>Ghi chú:</b></td><td>{Html(order.Notes)}</td></tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</body></html>");

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất Excel(HTML): " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Xuất hóa đơn sang PDF bằng Microsoft Print to PDF
        /// </summary>
        public bool ExportToPdf(int orderId, string filePath)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return false;

                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                decimal paidAmount = payments.Sum(p => p.Amount);
                decimal totalAmount = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);

                var doc = new PrintDocument();
                doc.PrintController = new StandardPrintController(); // No dialog
                doc.PrinterSettings = new PrinterSettings
                {
                    PrinterName = "Microsoft Print to PDF",
                    PrintToFile = true,
                    PrintFileName = filePath
                };

                int y = 40;
                doc.PrintPage += (s, e) =>
                {
                    var g = e.Graphics; var fontTitle = new Font("Segoe UI", 16, FontStyle.Bold); var font = new Font("Segoe UI", 10);
                    g.DrawString("HÓA ĐƠN BÁN HÀNG", fontTitle, Brushes.Black, 40, y); y += 35;
                    g.DrawString($"Số: {order.OrderID}", font, Brushes.Black, 40, y); y += 20;
                    g.DrawString($"Ngày: {order.OrderDate:dd/MM/yyyy HH:mm}", font, Brushes.Black, 40, y); y += 20;
                    g.DrawString($"Khách: {order.CustomerName}", font, Brushes.Black, 40, y); y += 30;

                    g.DrawString("Chi tiết:", font, Brushes.Black, 40, y); y += 20;
                    g.DrawString("STT   Sản phẩm                           SL   Đơn giá      Thành tiền", font, Brushes.Black, 40, y); y += 18;

                    int stt = 1;
                    foreach (var item in order.OrderDetails)
                    {
                        decimal itemTotal = item.Quantity * item.UnitPrice;
                        g.DrawString($"{stt,2}    {item.SeafoodName,-30}   {item.Quantity,3}   {item.UnitPrice,10:N0}   {itemTotal,12:N0}", font, Brushes.Black, 40, y);
                        y += 18; stt++;
                        if (y > e.MarginBounds.Bottom - 100) { e.HasMorePages = true; y = 40; return; }
                    }
                    y += 10;
                    g.DrawString($"Tổng tiền: {totalAmount:N0} VNĐ", font, Brushes.Black, 40, y); y += 18;
                    g.DrawString($"Đã thanh toán: {paidAmount:N0} VNĐ", font, Brushes.Black, 40, y); y += 18;
                    g.DrawString($"Còn lại: {(totalAmount - paidAmount):N0} VNĐ", font, Brushes.Black, 40, y); y += 18;
                    g.DrawString($"Ghi chú: {order.Notes}", font, Brushes.Black, 40, y);
                };

                doc.Print();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất PDF: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Xuất hóa đơn dạng CSV (mở tốt bằng Excel)
        /// </summary>
        public bool ExportToCsv(int orderId, string filePath)
        {
            try
            {
                var order = _orderBLL.GetById(orderId);
                if (order == null) return false;

                var payments = _paymentBLL.GetPaymentsByOrderId(orderId);
                decimal totalAmount = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);
                decimal paidAmount = payments.Sum(p => p.Amount);

                var lines = new List<string>();
                lines.Add("Hoa don,,,");
                lines.Add($"So hoa don,{order.OrderID},Ngay,{order.OrderDate:dd/MM/yyyy HH:mm}");
                lines.Add($"Khach hang,{Escape(order.CustomerName)},,,");
                lines.Add("");
                lines.Add("STT,San pham,So luong,Don gia,Thanh tien");

                int stt = 1;
                foreach (var item in order.OrderDetails)
                {
                    var itemTotal = item.Quantity * item.UnitPrice;
                    lines.Add($"{stt},{Escape(item.SeafoodName)},{item.Quantity},{item.UnitPrice},{itemTotal}");
                    stt++;
                }

                lines.Add("");
                lines.Add($"Tong tien,,,{totalAmount}");
                lines.Add($"Da thanh toan,,,{paidAmount}");
                lines.Add($"Con lai,,,{totalAmount - paidAmount}");
                lines.Add($"Trang thai,,,{Escape(order.Status)}");
                lines.Add($"Ghi chu,,,{Escape(order.Notes)}");

                File.WriteAllLines(filePath, lines, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loi xuat CSV: " + ex.Message);
                return false;
            }
        }

        private static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var val = input.Replace("\"", "\"\"");
            if (val.Contains(",") || val.Contains("\n") || val.Contains("\r"))
            {
                val = "\"" + val + "\"";
            }
            return val;
        }

        // Simple HTML encoder (avoid System.Web dependency)
        private static string Html(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&#39;");
            return sb.ToString();
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
        private static string Html(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&#39;");
            return sb.ToString();
        }
    }
}
