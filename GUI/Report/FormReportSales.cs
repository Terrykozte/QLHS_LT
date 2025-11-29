using System;
using System.Data;
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
        private DataTable _currentData;

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

            dgvSales.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "OrderDate", 
                HeaderText = "NGÀY", 
                Width = 150, 
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } 
            });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn 
            { 
                DataPropertyName = "TotalRevenue", 
                HeaderText = "DOANH THU", 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, 
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } 
            });
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                var fromDate = dtpFromDate.Value;
                var toDate = dtpToDate.Value;

                if (fromDate > toDate)
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var revenueData = _bll.GetDailyRevenue(fromDate, toDate);
                _currentData = ConvertToDataTable(revenueData);

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
                UpdateRecordCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ConvertToDataTable<T>(System.Collections.Generic.List<T> list)
        {
            DataTable dt = new DataTable();
            if (list.Count == 0) return dt;

            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            foreach (var item in list)
            {
                DataRow row = dt.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        private void UpdateRecordCount()
        {
            if (dgvSales.DataSource is System.Collections.IList list)
            {
                // For generic list
            }
            else if (dgvSales.DataSource is DataTable dt)
            {
                // For DataTable
            }
        }

        private void ExportToCSV(string filePath)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Write headers
                    for (int i = 0; i < dgvSales.Columns.Count; i++)
                    {
                        writer.Write(dgvSales.Columns[i].HeaderText);
                        if (i < dgvSales.Columns.Count - 1)
                            writer.Write(",");
                    }
                    writer.WriteLine();

                    // Write data
                    foreach (DataGridViewRow row in dgvSales.Rows)
                    {
                        for (int i = 0; i < dgvSales.Columns.Count; i++)
                        {
                            string value = row.Cells[i].Value?.ToString() ?? "";
                            if (value.Contains(",") || value.Contains("\""))
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            writer.Write(value);
                            if (i < dgvSales.Columns.Count - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xuất CSV: {ex.Message}");
            }
        }
    }
}

