using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using System.Diagnostics;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormInventoryList : BaseForm
    {
        private readonly InventoryBLL _bll = new InventoryBLL();
        private List<InventoryTransactionDTO> _allData = new List<InventoryTransactionDTO>();
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;
        private Timer _searchDebounceTimer;

        public FormInventoryList()
        {
            InitializeComponent();

            // UX
            this.KeyPreview = true;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            };
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

            // Events
            btnFilter.Click += btnApplyFilter_Click;
            btnStockIn.Click += btnStockIn_Click;
            btnStockOut.Click += btnStockOut_Click;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
        }

        private void ConfigureGrid()
        {
            dgvInventory.AutoGenerateColumns = false;
            dgvInventory.Columns.Clear();

            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionID", HeaderText = "ID", Width = 60 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TransactionDate", HeaderText = "NGÀY", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Type", HeaderText = "LOẠI", Width = 100 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductName", HeaderText = "SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SỐ LƯỢNG", Width = 100 });
            dgvInventory.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Note", HeaderText = "GHI CHÚ", Width = 200 });
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _bll.GetTransactionsByDateRange(dtpFromDate.Value, dtpToDate.Value) ?? new List<InventoryTransactionDTO>();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                Wait(false);
            }
        }

        private void ApplyFiltersAndPagination()
        {
            try
            {
                var type = cmbType.SelectedItem.ToString() == "Tất cả" ? null : cmbType.SelectedItem.ToString();
                var keyword = txtSearch.Text.ToLower();

                var filteredData = _allData.Where(t =>
                    (string.IsNullOrEmpty(type) || string.Equals(t.Type, type, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(keyword) || (t.ProductName ?? "").ToLower().Contains(keyword))
                ).ToList();

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvInventory.DataSource = pagedData;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void UpdatePagination()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage * _pageSize < _totalRecords;

            if (_totalRecords == 0)
            {
                lblPageInfo.Text = "Không có dữ liệu";
            }
            else
            {
                int from = (_currentPage - 1) * _pageSize + 1;
                int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                lblPageInfo.Text = $"Hiển thị {from} - {to} / {_totalRecords} mục";
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void btnStockIn_Click(object sender, EventArgs e)
        {
            using (var form = new FormStockIn())
            {
                if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnStockOut_Click(object sender, EventArgs e)
        {
            using (var form = new FormStockOut())
            {
                if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = null;

                _allData?.Clear();
                _allData = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
