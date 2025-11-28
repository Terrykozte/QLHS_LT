using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Customer
{
    public partial class FormCustomerList : Form
    {
        private readonly CustomerBLL _bll = new CustomerBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;

        public FormCustomerList()
        {
            InitializeComponent();
        }

        private void FormCustomerList_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
        }

        private void ConfigureGrid()
        {
            dgvCustomer.AutoGenerateColumns = false;
            dgvCustomer.Columns.Clear();

            // CheckBox Column
            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.HeaderText = "";
            chkCol.Width = 40;
            dgvCustomer.Columns.Add(chkCol);

            dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerID", DataPropertyName = "CustomerID", HeaderText = "ID", Width = 60 });
            dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "CustomerName", DataPropertyName = "CustomerName", HeaderText = "TÊN KHÁCH HÀNG", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhoneNumber", DataPropertyName = "PhoneNumber", HeaderText = "SỐ ĐIỆN THOẠI", Width = 150 });
            dgvCustomer.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", DataPropertyName = "Address", HeaderText = "ĐỊA CHỈ", Width = 250 });

            // Edit Button Column
            DataGridViewButtonColumn editBtn = new DataGridViewButtonColumn();
            editBtn.HeaderText = "THAO TÁC";
            editBtn.Text = "Sửa";
            editBtn.UseColumnTextForButtonValue = true;
            editBtn.Name = "colEdit";
            editBtn.Width = 80;
            dgvCustomer.Columns.Add(editBtn);

            dgvCustomer.CellContentClick += DgvCustomer_CellContentClick;
        }

        private void DgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCustomer.Columns[e.ColumnIndex].Name == "colEdit")
            {
                int customerId = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value); // Ensure DataPropertyName matches
                using (var form = new FormCustomerEdit(customerId))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
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
                    x.CustomerName.ToLower().Contains(keyword) || 
                    x.PhoneNumber.Contains(keyword)
                );

                _totalRecords = filteredData.Count;
                UpdatePagination();

                // Paging logic (Client-side)
                var pagedData = filteredData; // Implement Skip/Take if needed
                
                dgvCustomer.DataSource = pagedData;
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
            using (var form = new FormCustomerAdd())
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
