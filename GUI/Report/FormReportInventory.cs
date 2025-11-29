using System;
using System.Data;
using System.Windows.Forms;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Report
{
    public partial class FormReportInventory : Form
    {
        private readonly ReportBLL _bll = new ReportBLL();
        private DataTable _originalData;

        public FormReportInventory()
        {
            InitializeComponent();
        }

        private void FormReportInventory_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var inventoryList = _bll.GetInventoryStatusReport();
                _originalData = ConvertToDataTable(inventoryList);
                dgvInventory.DataSource = _originalData;
                
                // Configure columns
                ConfigureColumns();
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
            
            if (list == null || list.Count == 0)
                return dt;

            // Get all properties from the DTO
            var properties = typeof(T).GetProperties();
            
            // Create columns
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop.Name, prop.PropertyType);
            }

            // Add rows
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

        private void ConfigureColumns()
        {
            if (dgvInventory.Columns["SeafoodName"] != null) 
                dgvInventory.Columns["SeafoodName"].HeaderText = "TÊN SẢN PHẨM";
            if (dgvInventory.Columns["StockQuantity"] != null) 
                dgvInventory.Columns["StockQuantity"].HeaderText = "TỒN KHO";
            if (dgvInventory.Columns["Unit"] != null) 
                dgvInventory.Columns["Unit"].HeaderText = "ĐƠN VỊ";
            if (dgvInventory.Columns["LastUpdated"] != null) 
                dgvInventory.Columns["LastUpdated"].HeaderText = "CẬP NHẬT LẦN CUỐI";
        }

        private void UpdateRecordCount()
        {
            lblRecordCount.Text = $"Tổng cộng: {dgvInventory.Rows.Count} bản ghi";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (_originalData == null) return;

            string searchText = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                dgvInventory.DataSource = _originalData;
            }
            else
            {
                DataTable filteredData = _originalData.Clone();

                foreach (DataRow row in _originalData.Rows)
                {
                    bool found = false;
                    foreach (DataColumn column in _originalData.Columns)
                    {
                        if (row[column].ToString().ToLower().Contains(searchText))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                    {
                        filteredData.ImportRow(row);
                    }
                }

                dgvInventory.DataSource = filteredData;
            }

            UpdateRecordCount();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv",
                    FileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}",
                    Title = "Xuất báo cáo"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToFile(saveFileDialog.FileName, saveFileDialog.FilterIndex);
                    MessageBox.Show("Xuất báo cáo thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToFile(string filePath, int filterIndex)
        {
            if (filterIndex == 1) // Excel
            {
                ExportToExcel(filePath);
            }
            else if (filterIndex == 2) // CSV
            {
                ExportToCSV(filePath);
            }
        }

        private void ExportToExcel(string filePath)
        {
            try
            {
                // Using EPPlus or similar library if available
                // For now, using a simple CSV export as fallback
                ExportToCSV(filePath.Replace(".xlsx", ".csv"));
            }
            catch
            {
                ExportToCSV(filePath.Replace(".xlsx", ".csv"));
            }
        }

        private void ExportToCSV(string filePath)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Write headers
                    for (int i = 0; i < dgvInventory.Columns.Count; i++)
                    {
                        writer.Write(dgvInventory.Columns[i].HeaderText);
                        if (i < dgvInventory.Columns.Count - 1)
                            writer.Write(",");
                    }
                    writer.WriteLine();

                    // Write data
                    foreach (DataGridViewRow row in dgvInventory.Rows)
                    {
                        for (int i = 0; i < dgvInventory.Columns.Count; i++)
                        {
                            string value = row.Cells[i].Value?.ToString() ?? "";
                            // Escape quotes and wrap in quotes if contains comma
                            if (value.Contains(",") || value.Contains("\""))
                                value = "\"" + value.Replace("\"", "\"\"") + "\"";
                            writer.Write(value);
                            if (i < dgvInventory.Columns.Count - 1)
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
