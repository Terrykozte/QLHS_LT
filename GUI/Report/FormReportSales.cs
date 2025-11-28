using System;
using System.Linq;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Report
{
    public partial class FormReportSales : Form
    {
        private readonly ReportBLL _bll = new ReportBLL();

        private LiveCharts.Wpf.CartesianChart _wpfSalesChart;

        public FormReportSales()
        {
            InitializeComponent();
            _wpfSalesChart = new LiveCharts.Wpf.CartesianChart();
            salesChart.Child = _wpfSalesChart;
        }

        private void FormReportSales_Load(object sender, EventArgs e)
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;
            ConfigureGrid();
            btnGenerate_Click(sender, e); // Load initial data
        }

        private void ConfigureGrid()
        {
            dgvSales.AutoGenerateColumns = false;
            dgvSales.Columns.Clear();

            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDate", HeaderText = "NGÀY", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalRevenue", HeaderText = "DOANH THU", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            // Assuming OrderCount might be available, if not, just these two are fine for now.
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var fromDate = dtpFromDate.Value;
                var toDate = dtpToDate.Value;

                var revenueData = _bll.GetDailyRevenue(fromDate, toDate);

                // Chart configuration
                _wpfSalesChart.Series.Clear();
                _wpfSalesChart.AxisX.Clear();
                _wpfSalesChart.AxisY.Clear();

                _wpfSalesChart.Series = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Doanh thu",
                        Values = new ChartValues<decimal>(revenueData.Select(r => r.TotalRevenue))
                    }
                };

                _wpfSalesChart.AxisX.Add(new Axis
                {
                    Title = "Ngày",
                    Labels = revenueData.Select(r => r.OrderDate.ToString("dd/MM")).ToArray()
                });

                _wpfSalesChart.AxisY.Add(new Axis
                {
                    Title = "Doanh thu (VNĐ)",
                    LabelFormatter = value => value.ToString("N0")
                });

                // Grid configuration
                dgvSales.DataSource = revenueData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

