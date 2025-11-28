using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Dashboard
{
    public partial class FormDashboard : Form
    {
        private readonly ReportBLL _bll = new ReportBLL();

        private LiveCharts.Wpf.CartesianChart _wpfRevenueChart;

        public FormDashboard()
        {
            InitializeComponent();
            _wpfRevenueChart = new LiveCharts.Wpf.CartesianChart();
            revenueChart.Child = _wpfRevenueChart;
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            // Set default date range (e.g., last 7 days)
            dtpEndDate.Value = DateTime.Today;
            dtpStartDate.Value = DateTime.Today.AddDays(-6);

            // Load initial data
            LoadAllDashboardData();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadAllDashboardData();
        }

        private void LoadAllDashboardData()
        {
            DateTime startDate = dtpStartDate.Value;
            DateTime endDate = dtpEndDate.Value;

            if (startDate > endDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadStatCards(startDate, endDate);
            LoadRevenueChart(startDate, endDate);
            LoadTopSellingList(startDate, endDate);
            LoadLowStockList();
        }

        private void LoadStatCards(DateTime startDate, DateTime endDate)
        {
            try
            {
                lblOrdersCount.Text = _bll.GetOrderCount(startDate, endDate).ToString();
                lblCustomersCount.Text = _bll.GetNewCustomersCount(startDate, endDate).ToString();

                // Update titles
                string dateRangeStr = startDate.ToString("dd/MM") == endDate.ToString("dd/MM") ? "hôm nay" : $"từ {startDate:dd/MM} đến {endDate:dd/MM}";
                lblRevenueTitle.Text = $"Doanh thu {dateRangeStr}";
                lblOrdersTitle.Text = $"Đơn hàng {dateRangeStr}";
                lblCustomersTitle.Text = $"Khách hàng mới {dateRangeStr}";

                // Dummy trends for now as we don't have historical comparison logic yet
                lblRevenueTrend.Text = "+5.2%";
                lblOrdersTrend.Text = "-1.5%";
                lblCustomersTrend.Text = "+12.8%";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu thống kê: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblOrdersCount.Text = "N/A";
                lblCustomersCount.Text = "N/A";
            }
        }

        private void LoadRevenueChart(DateTime startDate, DateTime endDate)
        {
            var revenueData = _bll.GetDailyRevenue(startDate, endDate);
            decimal totalRevenue = revenueData.Sum(r => r.TotalRevenue);
            lblTotalRevenue.Text = $"{totalRevenue:N0} VNĐ";

            var dayLabels = revenueData.Select(r => r.OrderDate.ToString("dd/MM")).ToArray();
            var revenueValues = new ChartValues<decimal>(revenueData.Select(r => r.TotalRevenue));

            _wpfRevenueChart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = revenueValues,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(19, 146, 236)) // Primary Blue
                }
            };

            _wpfRevenueChart.AxisX.Clear();
            _wpfRevenueChart.AxisY.Clear();

            _wpfRevenueChart.AxisX.Add(new Axis
            {
                Title = "Ngày",
                Labels = dayLabels,
                Separator = new Separator { Step = 1, IsEnabled = false },
                LabelsRotation = dayLabels.Length > 7 ? 45 : 0
            });

            _wpfRevenueChart.AxisY.Add(new Axis
            {
                Title = "Doanh thu (VNĐ)",
                LabelFormatter = value => value.ToString("N0")
            });
        }

        private void LoadTopSellingList(DateTime startDate, DateTime endDate)
        {
            var topItems = _bll.GetTopSellingItems(5, startDate, endDate);
            flowTopSelling.Controls.Clear();

            if (topItems != null && topItems.Any())
            {
                foreach (var item in topItems)
                {
                    // Create Item Panel
                    Guna2Panel pnlItem = new Guna2Panel
                    {
                        Size = new Size(280, 60),
                        BackColor = System.Drawing.Color.White,
                        Margin = new Padding(0, 0, 0, 10)
                    };

                    // Icon/Image Placeholder
                    Guna2PictureBox picItem = new Guna2PictureBox
                    {
                        Size = new Size(40, 40),
                        Location = new Point(10, 10),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderRadius = 5,
                        FillColor = System.Drawing.Color.FromArgb(241, 245, 249) // Slate-100
                    };
                    // In a real app, load image from path. For now, use a colored box or default icon.
                    
                    // Name Label
                    Label lblName = new Label
                    {
                        Text = item.ItemName,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = System.Drawing.Color.FromArgb(51, 65, 85), // Slate-700
                        Location = new Point(60, 10),
                        AutoSize = true
                    };

                    // Category/Price Label
                    Label lblSub = new Label
                    {
                        Text = item.CategoryName, // Or Price if available
                        Font = new Font("Segoe UI", 8, FontStyle.Regular),
                        ForeColor = System.Drawing.Color.FromArgb(100, 116, 139), // Slate-500
                        Location = new Point(60, 30),
                        AutoSize = true
                    };

                    // Sales Count Label
                    Label lblSales = new Label
                    {
                        Text = $"{item.TotalQuantitySold} bán",
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        ForeColor = System.Drawing.Color.FromArgb(15, 23, 42), // Slate-900
                        Location = new Point(200, 20),
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleRight
                    };
                    // Adjust location to right align
                    lblSales.Location = new Point(pnlItem.Width - lblSales.Width - 10, 20);

                    pnlItem.Controls.Add(picItem);
                    pnlItem.Controls.Add(lblName);
                    pnlItem.Controls.Add(lblSub);
                    pnlItem.Controls.Add(lblSales);

                    flowTopSelling.Controls.Add(pnlItem);
                }
            }

            // Update title
            string dateRangeStr = startDate.Date == endDate.Date ? "hôm nay" : $"từ {startDate:dd/MM} đến {endDate:dd/MM}";
            lblTopSellingTitle.Text = $"Top 5 món bán chạy {dateRangeStr}";
        }

        private void LoadLowStockList()
        {
            var inventory = _bll.GetInventoryStatusReport();
            // Filter for low stock, e.g., < 20
            var lowStockItems = inventory.Where(i => i.QuantityRemaining < 20).ToList();

            dgvExpiration.DataSource = lowStockItems;
            
            // Configure Columns
            if (dgvExpiration.Columns.Count > 0)
            {
                dgvExpiration.Columns["ItemID"].Visible = false;
                dgvExpiration.Columns["ItemName"].HeaderText = "Tên sản phẩm";
                dgvExpiration.Columns["QuantityRemaining"].HeaderText = "Tồn kho";
                
                // Style
                dgvExpiration.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
                dgvExpiration.ColumnHeadersHeight = 30;
                dgvExpiration.RowTemplate.Height = 30;
                dgvExpiration.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                dgvExpiration.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            }

            lblExpirationTitle.Text = "Cảnh báo tồn kho thấp (< 20)";
        }
    }
}
