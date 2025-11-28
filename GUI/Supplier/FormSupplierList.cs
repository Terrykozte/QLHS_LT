using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Supplier
{
    public partial class FormSupplierList : Form
    {
        private readonly SupplierBLL _bll = new SupplierBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;

        public FormSupplierList()
        {
            InitializeComponent();
        }

        private void FormSupplierList_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
        }

        private void ConfigureGrid()
        {
            dgvSuppliers.AutoGenerateColumns = false;
            dgvSuppliers.Columns.Clear();

            // CheckBox Column
            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.HeaderText = "";
            chkCol.Width = 40;
            dgvSuppliers.Columns.Add(chkCol);

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierID", HeaderText = "ID", Width = 60 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "NHÀ CUNG CẤP", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ContactPerson", HeaderText = "NGƯỜI LIÊN HỆ", Width = 150 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "SỐ ĐIỆN THOẠI", Width = 120 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "EMAIL", Width = 180 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Address", HeaderText = "ĐỊA CHỈ", Width = 200 });
        }

        private void LoadData()
        {
            try
            {
                var allData = _bll.GetAll();
                
                // Filter logic
                string keyword = txtSearch.Text.ToLower();
                var filteredData = allData.FindAll(x => 
                    string.IsNullOrEmpty(keyword) || 
                    x.SupplierName.ToLower().Contains(keyword) || 
                    x.PhoneNumber.Contains(keyword) ||
                    x.Email.ToLower().Contains(keyword)
                );

                _totalRecords = filteredData.Count;
                UpdatePagination();

                // Paging logic (Client-side)
                var pagedData = filteredData; // Implement Skip/Take if needed
                
                dgvSuppliers.DataSource = pagedData;
                lblPageInfo.Text = $"Showing {Math.Min((_currentPage - 1) * _pageSize + 1, _totalRecords)} to {Math.Min(_currentPage * _pageSize, _totalRecords)} of {_totalRecords} Entries";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}");
            }
        }

        private void UpdatePagination()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage * _pageSize < _totalRecords;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormSupplierAdd())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Import đang phát triển.");
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Export đang phát triển.");
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _currentPage = 1;
                LoadData();
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
    }
}
