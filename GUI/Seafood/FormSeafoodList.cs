using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodList : BaseForm
    {
        private readonly SeafoodBLL _seafoodBLL = new SeafoodBLL();
        private Timer _searchDebounceTimer;
        private List<SeafoodDTO> _allData = new List<SeafoodDTO>();

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        public FormSeafoodList()
        {
            InitializeComponent();

            // Improve UX
            this.KeyPreview = true;
            this.KeyDown += FormSeafoodList_KeyDown;

            // Debounce search timer
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            };

            // Events
            this.Load += FormSeafoodList_Load;
            this.Shown += (s, e) => { try { txtSearch?.Focus(); } catch { } };
            txtSearch.TextChanged += txtSearch_TextChanged;
            dgvSeafood.CellDoubleClick += dgvSeafood_CellDoubleClick;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
        }

        private void FormSeafoodList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureColumns();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSeafoodList_Load");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _seafoodBLL.GetAll() ?? new List<SeafoodDTO>();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                var filteredData = _allData.Where(s =>
                    string.IsNullOrEmpty(keyword) ||
                    (s.SeafoodName?.ToLower().Contains(keyword) ?? false) ||
                    (s.CategoryName?.ToLower().Contains(keyword) ?? false) ||
                    (s.Unit?.ToLower().Contains(keyword) ?? false)
                ).ToList();

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvSeafood.DataSource = pagedData;
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

        private void ConfigureColumns()
        {
            dgvSeafood.AutoGenerateColumns = false;
            dgvSeafood.Columns.Clear();
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodID", HeaderText = "ID", Width = 60 });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "Tên hải sản", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Danh mục", Width = 160 });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Unit", HeaderText = "Đơn vị", Width = 80 });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số lượng", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn giá", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Trạng thái", Width = 120 });
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormSeafoodAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSeafood_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvSeafood?.Rows[e.RowIndex].DataBoundItem is SeafoodDTO item)
                {
                    using (var form = new FormSeafoodEdit(item))
                    {
                        if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form sửa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
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

        private void FormSeafoodList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) { LoadData(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.N) { btnAdd_Click(sender, EventArgs.Empty); e.Handled = true; }
            else if (e.KeyCode == Keys.Enter && dgvSeafood?.CurrentRow != null)
            {
                dgvSeafood_CellDoubleClick(dgvSeafood, new DataGridViewCellEventArgs(0, dgvSeafood.CurrentRow.Index));
                e.Handled = true;
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                // Xóa timer
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = null;

                // Xóa event handlers
                if (dgvSeafood != null)
                {
                    dgvSeafood.CellDoubleClick -= dgvSeafood_CellDoubleClick;
                }

                // Xóa dữ liệu
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
