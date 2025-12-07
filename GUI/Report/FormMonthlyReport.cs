using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Report
{
    public partial class FormMonthlyReport : BaseForm
    {
        private readonly MonthlyReportService _reportService;
        private MonthlyRevenueReport _currentReport;

        public FormMonthlyReport()
        {
            InitializeComponent();
            _reportService = new MonthlyReportService();
            ApplyStyles();
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvTopProducts != null) UIHelper.ApplyGridStyle(dgvTopProducts);
                if (dgvOrderStatus != null) UIHelper.ApplyGridStyle(dgvOrderStatus);
                if (dgvDailyRevenue != null) UIHelper.ApplyGridStyle(dgvDailyRevenue);
            }
            catch { }
            
            this.KeyPreview = true;
            this.KeyDown += FormMonthlyReport_KeyDown;
        }

        private void FormMonthlyReport_Load(object sender, EventArgs e)
        {
            InitializeControls();
            if (dtpMonth != null)
            {
                dtpMonth.ValueChanged += (s, ev) => { LoadReport(); };
            }
            LoadCurrentMonthReport();
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
            pnlTop.BackColor = Color.White;
            pnlStats.BackColor = Color.White;
            pnlChart.BackColor = Color.White;
            pnlTopProducts.BackColor = Color.White;
        }

        private void InitializeControls()
        {
            // Setup date pickers
            dtpMonth.Value = DateTime.Now;
            dtpMonth.Format = DateTimePickerFormat.Custom;
            dtpMonth.CustomFormat = "MM/yyyy";

            // Setup buttons
            btnViewReport.Click += (s, e) => LoadReport();
            btnExportCSV.Click += (s, e) => ExportToCSV();
            btnExportExcel.Click += (s, e) => ExportToExcel();
            btnPrint.Click += (s, e) => PrintReport();
            btnCompare.Click += (s, e) => CompareMonths();
        }

        private void LoadCurrentMonthReport()
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            LoadMonthlyReport(year, month);
        }

        private void LoadReport()
        {
            int year = dtpMonth.Value.Year;
            int month = dtpMonth.Value.Month;
            LoadMonthlyReport(year, month);
        }

        private void LoadMonthlyReport(int year, int month)
        {
            try
            {
                _currentReport = _reportService.GetMonthlyRevenue(year, month);
                if (_currentReport == null)
                {
                    MessageBox.Show("Không có dữ liệu cho tháng này");
                    return;
                }

                DisplayReportStatistics();
                DisplayTopProducts();
                DisplayOrderStatus();
                DisplayDailyRevenue();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải báo cáo: " + ex.Message);
            }
        }

        private void DisplayReportStatistics()
        {
            try
            {
                // Tổng doanh thu
                var lblRevenue = new Label
                {
                    Text = $"Tổng doanh thu: {_currentReport.TotalRevenue:N0} VNĐ",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(34, 197, 94)
                };

                // Tổng đơn hàng
                var lblOrders = new Label
                {
                    Text = $"Tổng đơn hàng: {_currentReport.TotalOrders}",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(59, 130, 246)
                };

                // Đơn hoàn thành
                var lblCompleted = new Label
                {
                    Text = $"Đơn hoàn thành: {_currentReport.CompletedOrders}",
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(34, 197, 94)
                };

                // Đơn chờ xử lý
                var lblPending = new Label
                {
                    Text = $"Đơn chờ xử lý: {_currentReport.PendingOrders}",
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(251, 146, 60)
                };

                // Đã thanh toán
                var lblPaid = new Label
                {
                    Text = $"Đã thanh toán: {_currentReport.TotalPaid:N0} VNĐ",
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(34, 197, 94)
                };

                // Còn lại
                var lblRemaining = new Label
                {
                    Text = $"Còn lại: {_currentReport.TotalRemaining:N0} VNĐ",
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(239, 68, 68)
                };

                // Giá trị đơn trung bình
                var lblAverage = new Label
                {
                    Text = $"Giá trị đơn trung bình: {_currentReport.AverageOrderValue:N0} VNĐ",
                    Font = new Font("Arial", 11),
                    ForeColor = Color.FromArgb(59, 130, 246)
                };

                // Clear and add to panel
                pnlStats.Controls.Clear();
                pnlStats.Controls.Add(lblRevenue);
                pnlStats.Controls.Add(lblOrders);
                pnlStats.Controls.Add(lblCompleted);
                pnlStats.Controls.Add(lblPending);
                pnlStats.Controls.Add(lblPaid);
                pnlStats.Controls.Add(lblRemaining);
                pnlStats.Controls.Add(lblAverage);

                // Auto layout
                int y = 10;
                foreach (Control ctrl in pnlStats.Controls)
                {
                    ctrl.Location = new Point(10, y);
                    ctrl.Width = pnlStats.Width - 20;
                    y += 30;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị thống kê: " + ex.Message);
            }
        }

        private void DisplayTopProducts()
        {
            try
            {
                dgvTopProducts.DataSource = null;
                dgvTopProducts.DataSource = _currentReport.TopProducts;

                dgvTopProducts.Columns["Rank"].HeaderText = "Xếp hạng";
                dgvTopProducts.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                dgvTopProducts.Columns["Quantity"].HeaderText = "Số lượng";
                dgvTopProducts.Columns["Revenue"].HeaderText = "Doanh thu";
                dgvTopProducts.Columns["AveragePrice"].HeaderText = "Giá trung bình";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị sản phẩm bán chạy: " + ex.Message);
            }
        }

        private void DisplayOrderStatus()
        {
            try
            {
                dgvOrderStatus.DataSource = null;
                var statusData = _currentReport.OrdersByStatus.Select(s => new
                {
                    Status = s.Key,
                    Count = s.Value,
                    Percentage = _currentReport.TotalOrders > 0 ? (s.Value * 100.0 / _currentReport.TotalOrders).ToString("F1") + "%" : "0%"
                }).ToList();

                dgvOrderStatus.DataSource = statusData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị trạng thái đơn: " + ex.Message);
            }
        }

        private void DisplayDailyRevenue()
        {
            try
            {
                var dailyData = _currentReport.DailyRevenue
                    .Where(d => d.Value > 0)
                    .Select(d => new { Day = d.Key, Revenue = d.Value })
                    .ToList();

                dgvDailyRevenue.DataSource = null;
                dgvDailyRevenue.DataSource = dailyData;

                dgvDailyRevenue.Columns["Day"].HeaderText = "Ngày";
                dgvDailyRevenue.Columns["Revenue"].HeaderText = "Doanh thu (VNĐ)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hiển thị doanh thu hàng ngày: " + ex.Message);
            }
        }

        private void ExportToCSV()
        {
            try
            {
                if (_currentReport == null)
                {
                    MessageBox.Show("Vui lòng tải báo cáo trước");
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "CSV Files|*.csv",
                    FileName = $"BaoCao_Thang_{_currentReport.Month}_{_currentReport.Year}.csv"
                };

                if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                {
                    string csv = _reportService.ExportMonthlyReportToCSV(_currentReport.Year, _currentReport.Month);
                    File.WriteAllText(sfd.FileName, csv, System.Text.Encoding.UTF8);
                    MessageBox.Show("Xuất báo cáo thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất CSV: " + ex.Message);
            }
        }

        private void ExportToExcel()
        {
            try
            {
                MessageBox.Show("Chức năng xuất Excel sẽ được thêm (cần thêm library EPPlus)");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void PrintReport()
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                if (UIHelper.ShowPrintDialog(this, pd) == DialogResult.OK)
                {
                    MessageBox.Show("Chức năng in ấn sẽ được thêm");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi in ấn: " + ex.Message);
            }
        }

        private void CompareMonths()
        {
            try
            {
                MessageBox.Show("Chức năng so sánh tháng sẽ được thêm");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}


