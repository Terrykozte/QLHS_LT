using System;
using System.Collections.Generic;
using System.Linq;
using QLTN_LT.DTO;

namespace QLTN_LT.BLL
{
    /// <summary>
    /// Dịch vụ báo cáo thống kê theo tháng
    /// </summary>
    public class MonthlyReportService
    {
        private readonly OrderBLL _orderBLL;
        private readonly PaymentBLL _paymentBLL;
        private readonly SeafoodBLL _seafoodBLL;

        public MonthlyReportService()
        {
            _orderBLL = new OrderBLL();
            _paymentBLL = new PaymentBLL();
            _seafoodBLL = new SeafoodBLL();
        }

        /// <summary>
        /// Lấy báo cáo doanh thu theo tháng
        /// </summary>
        public MonthlyRevenueReport GetMonthlyRevenue(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var orders = _orderBLL.GetAll(startDate, endDate, null, null);
                
                var report = new MonthlyRevenueReport
                {
                    Year = year,
                    Month = month,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalOrders = orders.Count,
                    CompletedOrders = orders.Count(o => o.Status == "Hoàn thành"),
                    PendingOrders = orders.Count(o => o.Status == "Chờ xử lý"),
                    CancelledOrders = orders.Count(o => o.Status == "Hủy"),
                    TotalRevenue = orders.Sum(o => o.OrderDetails.Sum(d => d.Quantity * d.UnitPrice)),
                    TotalPaid = 0,
                    TotalRemaining = 0,
                    DailyRevenue = new Dictionary<int, decimal>(),
                    TopProducts = GetTopProducts(orders, 10),
                    OrdersByStatus = GetOrdersByStatus(orders),
                    AverageOrderValue = 0
                };

                // Tính toán doanh thu theo ngày
                for (int day = 1; day <= DateTime.DaysInMonth(year, month); day++)
                {
                    var dayOrders = orders.Where(o => o.OrderDate.Day == day).ToList();
                    decimal dayRevenue = dayOrders.Sum(o => o.OrderDetails.Sum(d => d.Quantity * d.UnitPrice));
                    report.DailyRevenue[day] = dayRevenue;
                }

                // Tính toán thanh toán
                foreach (var order in orders)
                {
                    var payments = _paymentBLL.GetPaymentsByOrderId(order.OrderId);
                    decimal paidAmount = payments.Sum(p => p.Amount);
                    decimal orderTotal = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);
                    
                    report.TotalPaid += paidAmount;
                    report.TotalRemaining += (orderTotal - paidAmount);
                }

                report.AverageOrderValue = report.TotalOrders > 0 ? report.TotalRevenue / report.TotalOrders : 0;

                return report;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy báo cáo doanh thu: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy báo cáo so sánh giữa hai tháng
        /// </summary>
        public MonthlyComparisonReport CompareMonths(int year1, int month1, int year2, int month2)
        {
            try
            {
                var report1 = GetMonthlyRevenue(year1, month1);
                var report2 = GetMonthlyRevenue(year2, month2);

                if (report1 == null || report2 == null) return null;

                decimal revenueChange = report2.TotalRevenue - report1.TotalRevenue;
                decimal revenueChangePercent = report1.TotalRevenue > 0 ? (revenueChange / report1.TotalRevenue) * 100 : 0;

                return new MonthlyComparisonReport
                {
                    Report1 = report1,
                    Report2 = report2,
                    RevenueChange = revenueChange,
                    RevenueChangePercent = revenueChangePercent,
                    OrderChange = report2.TotalOrders - report1.TotalOrders,
                    OrderChangePercent = report1.TotalOrders > 0 ? ((report2.TotalOrders - report1.TotalOrders) / (decimal)report1.TotalOrders) * 100 : 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi so sánh tháng: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy báo cáo quý
        /// </summary>
        public QuarterlyReportDTO GetQuarterlyReport(int year, int quarter)
        {
            try
            {
                if (quarter < 1 || quarter > 4) return null;

                int startMonth = (quarter - 1) * 3 + 1;
                int endMonth = quarter * 3;

                var startDate = new DateTime(year, startMonth, 1);
                var endDate = new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth));

                var orders = _orderBLL.GetAll(startDate, endDate, null, null);

                var monthlyReports = new List<MonthlyRevenueReport>();
                for (int month = startMonth; month <= endMonth; month++)
                {
                    monthlyReports.Add(GetMonthlyRevenue(year, month));
                }

                return new QuarterlyReportDTO
                {
                    Year = year,
                    Quarter = quarter,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalRevenue = orders.Sum(o => o.OrderDetails.Sum(d => d.Quantity * d.UnitPrice)),
                    TotalOrders = orders.Count,
                    MonthlyReports = monthlyReports,
                    AverageMonthlyRevenue = monthlyReports.Average(m => m.TotalRevenue),
                    TopProducts = GetTopProducts(orders, 15)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy báo cáo quý: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy
        /// </summary>
        private List<TopProductDTO> GetTopProducts(List<OrderDTO> orders, int topCount)
        {
            try
            {
                var productSales = new Dictionary<string, (int quantity, decimal revenue)>();

                foreach (var order in orders)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        if (productSales.ContainsKey(item.ProductName))
                        {
                            var current = productSales[item.ProductName];
                            productSales[item.ProductName] = (
                                current.quantity + item.Quantity,
                                current.revenue + (item.Quantity * item.UnitPrice)
                            );
                        }
                        else
                        {
                            productSales[item.ProductName] = (item.Quantity, item.Quantity * item.UnitPrice);
                        }
                    }
                }

                return productSales
                    .OrderByDescending(p => p.Value.revenue)
                    .Take(topCount)
                    .Select((p, index) => new TopProductDTO
                    {
                        Rank = index + 1,
                        ProductName = p.Key,
                        Quantity = p.Value.quantity,
                        Revenue = p.Value.revenue,
                        AveragePrice = p.Value.revenue / p.Value.quantity
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lấy sản phẩm bán chạy: " + ex.Message);
                return new List<TopProductDTO>();
            }
        }

        /// <summary>
        /// Lấy thống kê đơn hàng theo trạng thái
        /// </summary>
        private Dictionary<string, int> GetOrdersByStatus(List<OrderDTO> orders)
        {
            return new Dictionary<string, int>
            {
                { "Chờ xử lý", orders.Count(o => o.Status == "Chờ xử lý") },
                { "Đang chuẩn bị", orders.Count(o => o.Status == "Đang chuẩn bị") },
                { "Hoàn thành", orders.Count(o => o.Status == "Hoàn thành") },
                { "Hủy", orders.Count(o => o.Status == "Hủy") }
            };
        }

        /// <summary>
        /// Xuất báo cáo tháng sang CSV
        /// </summary>
        public string ExportMonthlyReportToCSV(int year, int month)
        {
            try
            {
                var report = GetMonthlyRevenue(year, month);
                if (report == null) return null;

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("BÁNG CÁO DOANH THU THÁNG");
                csv.AppendLine($"Tháng {month}/{year}");
                csv.AppendLine();
                csv.AppendLine("THỐNG KÊ CHUNG");
                csv.AppendLine($"Tổng đơn hàng,{report.TotalOrders}");
                csv.AppendLine($"Đơn hoàn thành,{report.CompletedOrders}");
                csv.AppendLine($"Đơn chờ xử lý,{report.PendingOrders}");
                csv.AppendLine($"Đơn hủy,{report.CancelledOrders}");
                csv.AppendLine($"Tổng doanh thu,{report.TotalRevenue:N0}");
                csv.AppendLine($"Đã thanh toán,{report.TotalPaid:N0}");
                csv.AppendLine($"Còn lại,{report.TotalRemaining:N0}");
                csv.AppendLine($"Giá trị đơn trung bình,{report.AverageOrderValue:N0}");
                csv.AppendLine();
                csv.AppendLine("TOP SẢN PHẨM BÁN CHẠY");
                csv.AppendLine("Xếp hạng,Tên sản phẩm,Số lượng,Doanh thu,Giá trung bình");
                foreach (var product in report.TopProducts)
                {
                    csv.AppendLine($"{product.Rank},{product.ProductName},{product.Quantity},{product.Revenue:N0},{product.AveragePrice:N0}");
                }

                return csv.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi xuất CSV: " + ex.Message);
                return null;
            }
        }
    }

    /// <summary>
    /// Model báo cáo doanh thu tháng
    /// </summary>
    public class MonthlyRevenueReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int PendingOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<int, decimal> DailyRevenue { get; set; }
        public List<TopProductDTO> TopProducts { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }
    }

    /// <summary>
    /// Model so sánh hai tháng
    /// </summary>
    public class MonthlyComparisonReport
    {
        public MonthlyRevenueReport Report1 { get; set; }
        public MonthlyRevenueReport Report2 { get; set; }
        public decimal RevenueChange { get; set; }
        public decimal RevenueChangePercent { get; set; }
        public int OrderChange { get; set; }
        public decimal OrderChangePercent { get; set; }
    }

    /// <summary>
    /// Model báo cáo quý
    /// </summary>
    public class QuarterlyReportDTO
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public List<MonthlyRevenueReport> MonthlyReports { get; set; }
        public decimal AverageMonthlyRevenue { get; set; }
        public List<TopProductDTO> TopProducts { get; set; }
    }

    /// <summary>
    /// Model sản phẩm bán chạy
    /// </summary>
    public class TopProductDTO
    {
        public int Rank { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
        public decimal AveragePrice { get; set; }
    }
}


