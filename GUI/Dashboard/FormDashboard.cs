using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using System.Collections.Generic;
using LiveCharts;
using LiveCharts.Wpf;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Dashboard
{
    public partial class FormDashboard : BaseForm
    {
        private readonly ReportBLL _bll = new ReportBLL();

        private LiveCharts.Wpf.CartesianChart _wpfRevenueChart;

        public FormDashboard()
        {
            InitializeComponent();
            try
            {
                _wpfRevenueChart = new LiveCharts.Wpf.CartesianChart();
                if (revenueChart != null) revenueChart.Child = _wpfRevenueChart;
            }
            catch { _wpfRevenueChart = new LiveCharts.Wpf.CartesianChart(); }

            // UX & Styling
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvExpiration != null) UIHelper.ApplyGridStyle(dgvExpiration);
            }
            catch { }

            this.KeyPreview = true;
            this.KeyDown += FormDashboard_KeyDown;
        }

        private Timer _debounceTimer;

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            try
            {
                // Setup keyboard navigation
                KeyboardNavigationHelper.RegisterForm(this, new List<Control>
                {
                    dtpStartDate, dtpEndDate, btnApplyFilter
                });

                // Setup animations
                AnimationHelper.FadeIn(this, 300);

                // Setup responsive
                ApplyResponsiveDesign();

                // Debounce for filters
                _debounceTimer = new Timer { Interval = 350 };
                _debounceTimer.Tick += (s, e2) => { _debounceTimer.Stop(); LoadAllDashboardData(); };

                // Set default date range (e.g., last 7 days)
                if (dtpEndDate != null) dtpEndDate.Value = DateTime.Today;
                if (dtpStartDate != null) dtpStartDate.Value = DateTime.Today.AddDays(-6);

                // Wire quick filter changes
                if (dtpStartDate != null) dtpStartDate.ValueChanged += (s, e2) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
                if (dtpEndDate != null) dtpEndDate.ValueChanged += (s, e2) => { _debounceTimer.Stop(); _debounceTimer.Start(); };
                if (btnApplyFilter != null) btnApplyFilter.Click += btnApplyFilter_Click;

                // Load initial data
                LoadAllDashboardData();
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tải bảng điều khiển: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"FormDashboard_Load error: {ex}");
            }
        }

        private void ApplyResponsiveDesign()
        {
            try
            {
                // Adjust font sizes
                if (lblOrdersCount != null)
                    lblOrdersCount.Font = new Font(lblOrdersCount.Font.FontFamily,
                        ResponsiveHelper.GetResponsiveFontSize(16, this), FontStyle.Bold);

                if (lblTotalRevenue != null)
                    lblTotalRevenue.Font = new Font(lblTotalRevenue.Font.FontFamily,
                        ResponsiveHelper.GetResponsiveFontSize(16, this), FontStyle.Bold);

                // Adjust grid row height
                if (dgvExpiration != null)
                {
                    dgvExpiration.RowTemplate.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 30);
                    ResponsiveHelper.AdjustDataGridViewColumns(dgvExpiration, this);
                }
            }
            catch { }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            try
            {
                LoadAllDashboardData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi áp dụng bộ lọc: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"btnApplyFilter_Click error: {ex}");
            }
        }

        private void FormDashboard_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadAllDashboardData();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    LoadAllDashboardData();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormDashboard_KeyDown error: {ex.Message}");
            }
        }

        private void FormDashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            _debounceTimer?.Stop();
            _debounceTimer?.Dispose();
            KeyboardNavigationHelper.UnregisterForm(this);
            DataBindingHelper.ClearCache();
        }

        protected override void CleanupResources()
        {
            try
            {
                _debounceTimer?.Stop();
                _debounceTimer?.Dispose();
                try
                {
                    if (revenueChart != null && !revenueChart.IsDisposed)
                {
                    revenueChart.Child = null;
                    }
                }
                catch { }
                if (_wpfRevenueChart != null)
                {
                    _wpfRevenueChart.Series = new SeriesCollection();
                    _wpfRevenueChart = null;
                }
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }

        private void LoadAllDashboardData()
        {
            try
            {
                DateTime startDate = dtpStartDate.Value;
                DateTime endDate = dtpEndDate.Value;

                if (startDate > endDate)
                {
                    UXInteractionHelper.ShowWarning("Cảnh báo", "Ngày bắt đầu không được lớn hơn ngày kết thúc.");
                    return;
                }

                // Show loading state
                ShowLoadingState(true);

                try
                {
                    // Check cache first
                    string cacheKey = $"dashboard_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
                    var cachedData = DataBindingHelper.GetCachedData<object>(cacheKey);

                    if (cachedData == null)
                    {
                        LoadStatCards(startDate, endDate);
                        LoadRevenueChart(startDate, endDate);
                        LoadTopSellingList(startDate, endDate);
                        LoadLowStockList();
                    }
                    else
                    {
                        // Use cached data
                        LoadStatCards(startDate, endDate);
                    }

                    // Animate cards
                    AnimationHelper.FadeIn(this, 300);
                }
                finally
                {
                    ShowLoadingState(false);
                }
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tải dữ liệu: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"LoadAllDashboardData error: {ex}");
            }
        }

        private void ShowLoadingState(bool isLoading)
        {
            if (isLoading)
            {
                progressIndicator.Visible = true;
                progressIndicator.Start();
                tlpMain.Enabled = false;
            }
            else
            {
                progressIndicator.Visible = false;
                progressIndicator.Stop();
                tlpMain.Enabled = true;
            }
            btnApplyFilter.Enabled = !isLoading;
        }

        private void LoadStatCards(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Set default values
                lblOrdersCount.Text = "0";
                lblCustomersCount.Text = "0";
                lblTotalRevenue.Text = "0 VNĐ";

                // Load data
                int orderCount = _bll.GetOrderCount(startDate, endDate);
                lblOrdersCount.Text = orderCount.ToString();

                int customerCount = _bll.GetNewCustomersCount(startDate, endDate);
                lblCustomersCount.Text = customerCount.ToString();

                // Update titles
                string dateRangeStr = startDate.ToString("dd/MM") == endDate.ToString("dd/MM") ? "hôm nay" : $"từ {startDate:dd/MM} đến {endDate:dd/MM}";
                lblRevenueTitle.Text = $"Doanh thu {dateRangeStr}";
                lblOrdersTitle.Text = $"Đơn hàng {dateRangeStr}";
                lblCustomersTitle.Text = $"Khách hàng mới {dateRangeStr}";

                // Dummy trends
                lblRevenueTrend.Text = "+5.2%";
                lblOrdersTrend.Text = "-1.5%";
                lblCustomersTrend.Text = "+12.8%";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadStatCards error: {ex.Message}");
                lblOrdersCount.Text = "N/A";
                lblCustomersCount.Text = "N/A";
                lblTotalRevenue.Text = "N/A";
            }
        }

        private void LoadRevenueChart(DateTime startDate, DateTime endDate)
        {
            try
            {
                var revenueData = _bll.GetDailyRevenue(startDate, endDate);
                
                if (revenueData == null || revenueData.Count == 0)
                {
                    lblTotalRevenue.Text = "0 VNĐ";
                    if (_wpfRevenueChart != null)
                    {
                        _wpfRevenueChart.Series = new SeriesCollection();
                    }
                    return;
                }

                decimal totalRevenue = revenueData.Sum(r => r.TotalRevenue);
                if (lblTotalRevenue != null)
                    lblTotalRevenue.Text = $"{totalRevenue:N0} VNĐ";

                var dayLabels = revenueData.Select(r => r.OrderDate.ToString("dd/MM")).ToArray();
                var revenueValues = new ChartValues<decimal>(revenueData.Select(r => r.TotalRevenue));

                if (_wpfRevenueChart != null)
                {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadRevenueChart error: {ex.Message}");
                if (lblTotalRevenue != null)
                    lblTotalRevenue.Text = "N/A";
            }
        }

        private void LoadTopSellingList(DateTime startDate, DateTime endDate)
        {
            try
            {
                var topItems = _bll.GetTopSellingItems(5, startDate, endDate);
                
                if (flowTopSelling != null)
                    flowTopSelling.Controls.Clear();

                if (topItems != null && topItems.Any())
                {
                    foreach (var item in topItems)
                    {
                        if (flowTopSelling != null)
                        {
                            var itemPanel = CreateTopSellingItemPanel(item);
                            flowTopSelling.Controls.Add(itemPanel);
                        }
                    }
                }

                // Update title
                string dateRangeStr = startDate.Date == endDate.Date ? "hôm nay" : $"từ {startDate:dd/MM} đến {endDate:dd/MM}";
                if (lblTopSellingTitle != null)
                    lblTopSellingTitle.Text = $"Top 5 món bán chạy {dateRangeStr}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadTopSellingList error: {ex.Message}");
            }
        }

        private void LoadLowStockList()
        {
            try
            {
                var inventory = _bll.GetInventoryStatusReport();
                
                if (inventory == null)
                {
                    if (dgvExpiration != null)
                        dgvExpiration.DataSource = null;
                    return;
                }

                // Filter for low stock, prefer QuantityRemaining else fallback to Quantity
                var lowStockItems = inventory.Where(i =>
                {
                    try
                    {
                        var type = i.GetType();
                        var prop = type.GetProperty("QuantityRemaining") ?? type.GetProperty("Quantity");
                        if (prop == null) return false;
                        var val = prop.GetValue(i);
                        int qty = 0;
                        if (val is int iv) qty = iv;
                        else if (val != null && int.TryParse(val.ToString(), out var p)) qty = p;
                        return qty < 20;
                    }
                    catch { return false; }
                }).ToList();

                if (dgvExpiration != null)
                {
                    dgvExpiration.DataSource = lowStockItems;
                    
                    // Configure Columns
                    if (dgvExpiration.Columns.Count > 0)
                    {
                        if (dgvExpiration.Columns["ItemID"] != null)
                            dgvExpiration.Columns["ItemID"].Visible = false;
                        
                        if (dgvExpiration.Columns["ItemName"] != null)
                            dgvExpiration.Columns["ItemName"].HeaderText = "Tên sản phẩm";
                        
                        if (dgvExpiration.Columns["QuantityRemaining"] != null)
                            dgvExpiration.Columns["QuantityRemaining"].HeaderText = "Tồn kho";
                        
                        // Style
                        dgvExpiration.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Default;
                        dgvExpiration.ColumnHeadersHeight = 30;
                        dgvExpiration.RowTemplate.Height = 30;
                        dgvExpiration.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                        dgvExpiration.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    }
                }

                if (lblExpirationTitle != null)
                    lblExpirationTitle.Text = "Cảnh báo tồn kho thấp (< 20)";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadLowStockList error: {ex.Message}");
            }
        }

        private Guna2Panel CreateTopSellingItemPanel(TopSellingItemDTO item)
        {
            try
            {
                Guna2Panel pnlItem = new Guna2Panel
                {
                    Size = new Size(280, 60),
                    BackColor = System.Drawing.Color.White,
                    Margin = new Padding(0, 0, 0, 10)
                };

                Guna2PictureBox picItem = new Guna2PictureBox
                {
                    Size = new Size(40, 40),
                    Location = new Point(10, 10),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BorderRadius = 5,
                    FillColor = System.Drawing.Color.FromArgb(241, 245, 249)
                };

                Label lblName = new Label
                {
                    Text = item.ItemName ?? "N/A",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(51, 65, 85),
                    Location = new Point(60, 10),
                    AutoSize = true
                };

                Label lblSub = new Label
                {
                    Text = item.CategoryName ?? "N/A",
                    Font = new Font("Segoe UI", 8, FontStyle.Regular),
                    ForeColor = System.Drawing.Color.FromArgb(100, 116, 139),
                    Location = new Point(60, 30),
                    AutoSize = true
                };

                Label lblSales = new Label
                {
                    Text = $"{item.TotalQuantitySold} bán",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = System.Drawing.Color.FromArgb(15, 23, 42),
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleRight
                };
                lblSales.Location = new Point(pnlItem.Width - lblSales.Width - 10, 20);

                pnlItem.Controls.Add(picItem);
                pnlItem.Controls.Add(lblName);
                pnlItem.Controls.Add(lblSub);
                pnlItem.Controls.Add(lblSales);

                return pnlItem;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating top selling item panel: {ex.Message}");
                return new Guna2Panel();
            }
        }
    }
}
