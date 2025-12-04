using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormInventoryList : Form
    {
        private readonly InventoryBLL _bll = new InventoryBLL();
        private int _totalRecords = 0;

        public FormInventoryList()
        {
            InitializeComponent();
        }

        private void FormInventoryList_Load(object sender, EventArgs e)
        {
            SetupControls();
            ConfigureGrid();
            LoadData();
        }

        private void SetupControls()
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;
            cmbType.SelectedIndex = 0;
        }

        private void ConfigureGrid()
        {
            dgvInventory.AutoGenerateColumns = false;
            dgvInventory.Columns.Clear();

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionID", HeaderText = "ID", Width = 60 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionDate", HeaderText = "NGÀY", Width = 120 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "LOẠI", Width = 100 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SỐ LƯỢNG", Width = 100 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Note", HeaderText = "GHI CHÚ", Width = 200 });
        }

        private void LoadData()
        {
            try
            {
                var fromDate = dtpFromDate.Value;
                var toDate = dtpToDate.Value;
                var type = cmbType.SelectedItem.ToString() == "Tất cả" ? null : cmbType.SelectedItem.ToString();
                var keyword = txtSearch.Text;

                var data = _bll.GetTransactionsByDateRange(fromDate, toDate);
                if (!string.IsNullOrEmpty(type))
                {
                    data = data.Where(t => string.Equals(t.Type, type, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    data = data.Where(t => (t.ProductName ?? "").IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
                
                _totalRecords = data.Count;
                // Implement client-side paging for now
                // In a real app, pass page/pageSize to BLL
                
                dgvInventory.DataSource = data;
                lblPageInfo.Text = $"Showing {data.Count} entries"; 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnStockIn_Click(object sender, EventArgs e)
        {
            using (var form = new FormStockIn())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnStockOut_Click(object sender, EventArgs e)
        {
            using (var form = new FormStockOut())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }
    }
}
