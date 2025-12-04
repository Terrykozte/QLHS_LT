using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormInventoryManagement : BaseForm
    {
        private InventoryBLL _inventoryBLL;
        private SupplierBLL _supplierBLL;
        private List<InventoryStatusDTO> _inventoryList;
        private int _currentPage = 1;
        private int _pageSize = 50;
        private Timer _searchDebounce;

        public FormInventoryManagement()
        {
            InitializeComponent();
            _inventoryBLL = new InventoryBLL();
            _supplierBLL = new SupplierBLL();

            // Styling
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvInventory != null) UIHelper.ApplyGridStyle(dgvInventory);
                if (dgvTransactions != null) UIHelper.ApplyGridStyle(dgvTransactions);
                if (btnStockIn != null) UIHelper.ApplyButtonStyle(btnStockIn, true);
                if (btnViewTransactions != null) UIHelper.ApplyButtonStyle(btnViewTransactions, false);
                if (btnRefresh != null) UIHelper.ApplyButtonStyle(btnRefresh, false);
                if (btnPreviousPage != null) UIHelper.ApplyButtonStyle(btnPreviousPage, false);
                if (btnNextPage != null) UIHelper.ApplyButtonStyle(btnNextPage, false);
            }
            catch { }
        }

        private void FormInventoryManagement_Load(object sender, EventArgs e)
        {
            try
            {
                this.KeyPreview = true;
                this.KeyDown += FormInventoryManagement_KeyDown;

                _searchDebounce = new Timer { Interval = 350 };
                _searchDebounce.Tick += (s, e2) => { _searchDebounce.Stop(); ApplyFilter(); };

                SetupUI();
                LoadInventoryData();

                // Wire extra events
                if (txtSearch != null) txtSearch.TextChanged += (s, e2) => { _searchDebounce.Stop(); _searchDebounce.Start(); };
                if (btnRefresh != null) btnRefresh.Click += (s, e2) => LoadInventoryData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupUI()
        {
            try
            {
                // Setup DataGridView
                if (dgvInventory != null)
                {
                    dgvInventory.AutoGenerateColumns = false;
                    dgvInventory.Columns.Clear();
                    dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "TÊN SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                    dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SỐ LƯỢNG", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
                    dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ReorderLevel", HeaderText = "MỨC TỐI THIỂU", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
                    dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 110 });
                    dgvInventory.Columns.Add(new DataGridViewButtonColumn { Name = "colEdit", HeaderText = "", Text = "Sửa", UseColumnTextForButtonValue = true, Width = 60 });
                    dgvInventory.CellContentClick += DgvInventory_CellContentClick;
                }

                // Setup Transactions DataGridView
                if (dgvTransactions != null)
                {
                    dgvTransactions.AutoGenerateColumns = false;
                    dgvTransactions.Columns.Clear();
                    dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionDate", HeaderText = "NGÀY", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
                    dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionType", HeaderText = "LOẠI", Width = 100 });
                    dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SỐ LƯỢNG", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
                    dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Reason", HeaderText = "LÝ DO", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                    dgvTransactions.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "NHÀ CUNG CẤP", Width = 180 });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SetupUI error: {ex.Message}");
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                Wait(true);
                _inventoryList = _inventoryBLL.GetInventoryStatus() ?? new List<InventoryStatusDTO>();
                if (dgvInventory != null) dgvInventory.DataSource = _inventoryList;

                // Highlight low stock items
                if (dgvInventory != null)
                {
                    foreach (DataGridViewRow row in dgvInventory.Rows)
                    {
                        var status = row.Cells["Status"].Value?.ToString();
                        if (string.Equals(status, "Cần nhập", StringComparison.OrdinalIgnoreCase))
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                        else if (string.Equals(status, "Thấp", StringComparison.OrdinalIgnoreCase))
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LemonChiffon;
                    }
                }

                if (lblTotalItems != null) lblTotalItems.Text = $"Tổng sản phẩm: {_inventoryList.Count}";
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Wait(false); }
        }

        private void ApplyFilter()
        {
            try
            {
                if (_inventoryList == null) return;
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                var filtered = _inventoryList.Where(i =>
                    string.IsNullOrEmpty(keyword) ||
                    (i.SeafoodName?.ToLower().Contains(keyword) ?? false) ||
                    (i.Status?.ToLower().Contains(keyword) ?? false)
                ).ToList();
                if (dgvInventory != null) dgvInventory.DataSource = filtered;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ApplyFilter error: {ex.Message}");
            }
        }

        private void DgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvInventory?.Columns[e.ColumnIndex].Name != "colEdit") return;
                var selectedItem = dgvInventory.Rows[e.RowIndex].DataBoundItem as InventoryStatusDTO;
                if (selectedItem == null) return;
                ShowInventoryTransactionForm(selectedItem.InventoryID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowInventoryTransactionForm(int inventoryId)
        {
            using (var form = new FormInventoryTransaction(inventoryId))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    LoadInventoryData();
                }
            }
        }

        private void btnStockIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory?.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedItem = dgvInventory.SelectedRows[0].DataBoundItem as InventoryStatusDTO;
                if (selectedItem == null) return;
                ShowInventoryTransactionForm(selectedItem.InventoryID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadInventoryData();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnViewTransactions_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvInventory?.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedItem = dgvInventory.SelectedRows[0].DataBoundItem as InventoryStatusDTO;
                if (selectedItem == null) return;
                LoadTransactions(selectedItem.InventoryID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTransactions(int inventoryId)
        {
            try
            {
                var transactions = _inventoryBLL.GetTransactions(inventoryId, _currentPage, _pageSize) ?? new List<InventoryTransactionDTO>();
                if (dgvTransactions != null) dgvTransactions.DataSource = transactions;

                int totalCount = _inventoryBLL.GetTotalTransactionCount(inventoryId);
                int totalPages = (totalCount + _pageSize - 1) / _pageSize;
                if (lblTransactionInfo != null) lblTransactionInfo.Text = $"Trang {_currentPage}/{Math.Max(totalPages, 1)} - Tổng: {totalCount} giao dịch";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                if (dgvInventory?.SelectedRows.Count > 0)
                {
                    var selectedItem = dgvInventory.SelectedRows[0].DataBoundItem as InventoryStatusDTO;
                    if (selectedItem != null) LoadTransactions(selectedItem.InventoryID);
                }
            }
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            _currentPage++;
            if (dgvInventory?.SelectedRows.Count > 0)
            {
                var selectedItem = dgvInventory.SelectedRows[0].DataBoundItem as InventoryStatusDTO;
                if (selectedItem != null) LoadTransactions(selectedItem.InventoryID);
            }
        }

        private void btnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Excel Files (*.xlsx)|*.xlsx|CSV Files (*.csv)|*.csv";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show("Chức năng xuất báo cáo đang được phát triển", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormInventoryManagement_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadInventoryData();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
            }
            catch { }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounce?.Stop();
                _searchDebounce?.Dispose();
                _inventoryList?.Clear();
                _inventoryBLL = null;
                _supplierBLL = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
