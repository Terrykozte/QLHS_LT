using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Customer
{
    public partial class FormCustomerList : BaseForm
    {
        private CustomerBLL _bll = new CustomerBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private Timer _searchDebounceTimer;
        private List<CustomerDTO> _allData = new List<CustomerDTO>();

        public FormCustomerList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormCustomerList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvCustomer != null) UIHelper.ApplyGridStyle(dgvCustomer);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
                if (btnImport != null) UIHelper.ApplyGunaButtonStyle(btnImport, isPrimary: false);
                if (btnExport != null) UIHelper.ApplyGunaButtonStyle(btnExport, isPrimary: false);
                if (btnPrevious != null) UIHelper.ApplyGunaButtonStyle(btnPrevious, isPrimary: false);
                if (btnNext != null) UIHelper.ApplyGunaButtonStyle(btnNext, isPrimary: false);
            }
            catch { }

            // Debounce search timer
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                LoadData();
            };

            // Events
            this.Load += FormCustomerList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                txtSearch.KeyDown += txtSearch_KeyDown;
            }
            if (btnPrevious != null) btnPrevious.Click += btnPrevious_Click;
            if (btnNext != null) btnNext.Click += btnNext_Click;
        }

        private void FormCustomerList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCustomerList_Load");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvCustomer == null) return;

                dgvCustomer.AutoGenerateColumns = false;
                dgvCustomer.Columns.Clear();

                // Selection CheckBox Column
                var chkCol = new DataGridViewCheckBoxColumn { HeaderText = "", Width = 40, ReadOnly = true };
                dgvCustomer.Columns.Add(chkCol);

                dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerID", DataPropertyName = "CustomerID", HeaderText = "ID", Width = 60 });
                dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", DataPropertyName = "CustomerName", HeaderText = "TÊN KHÁCH HÀNG", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhoneNumber", DataPropertyName = "PhoneNumber", HeaderText = "SỐ ĐIỆN THOẠI", Width = 150 });
                dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", DataPropertyName = "Address", HeaderText = "ĐỊA CHỈ", Width = 250 });

                // Action Button Column
                var editBtn = new DataGridViewButtonColumn
                {
                    HeaderText = "THAO TÁC",
                    Text = "Sửa",
                    UseColumnTextForButtonValue = true,
                    Name = "colEdit",
                    Width = 80
                };
                dgvCustomer.Columns.Add(editBtn);

                dgvCustomer.CellContentClick += DgvCustomer_CellContentClick;
                dgvCustomer.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        var cellValue = dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value;
                        if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                        {
                            OpenEdit(customerId);
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring grid: {ex.Message}");
            }
        }

        private void DgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvCustomer.Columns[e.ColumnIndex].Name == "colEdit")
                {
                    var cellValue = dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                    {
                        OpenEdit(customerId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in cell content click: {ex.Message}");
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenEdit(int customerId)
        {
            try
            {
                using (var form = new FormCustomerEdit(customerId))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "OpenEdit");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _bll.GetAll() ?? new List<CustomerDTO>();

                // Filter
                string keyword = txtSearch?.Text?.ToLower() ?? string.Empty;
                var filteredData = _allData.FindAll(x =>
                    string.IsNullOrEmpty(keyword) ||
                    (x.CustomerName?.ToLower().Contains(keyword) ?? false) ||
                    (x.PhoneNumber?.Contains(keyword) ?? false) ||
                    (x.Address?.ToLower().Contains(keyword) ?? false)
                );

                _totalRecords = filteredData.Count;
                UpdatePagination();

                // Paging (client-side)
                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvCustomer != null)
                {
                    dgvCustomer.DataSource = pagedData;
                }

                if (lblPageInfo != null)
                {
                    int from = _totalRecords == 0 ? 0 : ((_currentPage - 1) * _pageSize + 1);
                    int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                    lblPageInfo.Text = $"Hiển thị {from} - {to} / {_totalRecords} khách hàng";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
            }
        }

        private void UpdatePagination()
        {
            try
            {
                if (btnPrevious != null)
                    btnPrevious.Enabled = _currentPage > 1;

                if (btnNext != null)
                    btnNext.Enabled = _currentPage * _pageSize < _totalRecords;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating pagination: {ex.Message}");
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormCustomerAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening add form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Import đang phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Export đang phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _currentPage = 1;
                LoadData();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.Handled = true;
            }
        }

        private void FormCustomerList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadData();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvCustomer?.CurrentRow != null)
                {
                    var cellValue = dgvCustomer.CurrentRow.Cells["CustomerID"].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                    {
                        OpenEdit(customerId);
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allData?.Clear();
                _bll = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
