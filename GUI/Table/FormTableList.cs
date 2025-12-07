using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableList : BaseForm
    {
        private readonly TableBLL _bll = new TableBLL();
        private List<TableDTO> _allData = new List<TableDTO>();
        private Timer _searchDebounceTimer;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        public FormTableList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormTableList_KeyDown;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); _currentPage = 1; ApplyFiltersAndPagination(); };

            // Events
            this.Load += FormTableList_Load;
            txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
            txtSearch.KeyDown += TxtSearch_KeyDown;
            btnAdd.Click += BtnAdd_Click;
            btnGenerateQR.Click += btnGenerateQr_Click;
            dgvTable.CellDoubleClick += DgvTable_CellDoubleClick;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
        }

        private void FormTableList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormTableList_Load");
            }
        }

        private void ConfigureGrid()
        {
            dgvTable.AutoGenerateColumns = false;
            dgvTable.Columns.Clear();

            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableID", HeaderText = "ID", Width = 60 });
            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TableName", HeaderText = "TÊN BÀN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvTable.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 150 });
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _bll.GetAll() ?? new List<TableDTO>();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void ApplyFiltersAndPagination()
        {
            try
            {
                var keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                var filteredData = string.IsNullOrEmpty(keyword)
                    ? _allData
                    : _allData.Where(t => (t.TableName?.ToLower().Contains(keyword) ?? false) || (t.Status?.ToLower().Contains(keyword) ?? false)).ToList();

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvTable.DataSource = pagedData;
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
                lblPageInfo.Text = $"Hiển thị {from} - {to} / {_totalRecords} bàn";
            }
        }

        private void DgvTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvTable?.Rows[e.RowIndex].DataBoundItem is TableDTO table)
                {
                    using (var form = new FormTableEdit(table.TableID))
                    {
                        if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "DgvTable_CellDoubleClick"); }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormTableAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "BtnAdd_Click"); }
        }

        private void btnGenerateQr_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTable?.CurrentRow?.DataBoundItem is TableDTO selected)
                {
                    using (var f = new FormGenerateQR(selected.TableName))
                    {
                        UIHelper.ShowFormDialog(this, f);
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bàn để tạo mã QR.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "btnGenerateQr_Click"); }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { _currentPage = 1; ApplyFiltersAndPagination(); e.Handled = true; }
            else if (e.KeyCode == Keys.Escape) { txtSearch.Clear(); e.Handled = true; }
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

        private void FormTableList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) { LoadData(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.N) { BtnAdd_Click(sender, EventArgs.Empty); e.Handled = true; }
            else if (e.KeyCode == Keys.Enter && dgvTable?.CurrentRow != null)
            {
                DgvTable_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvTable.CurrentRow.Index));
                e.Handled = true;
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
                System.Diagnostics.Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally { base.CleanupResources(); }
        }
    }
}
