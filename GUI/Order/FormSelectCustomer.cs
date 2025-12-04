using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Order
{
    public partial class FormSelectCustomer : BaseForm
    {
        private readonly CustomerBLL _customerBLL = new CustomerBLL();
        private List<CustomerDTO> _all = new List<CustomerDTO>();
        public CustomerDTO SelectedCustomer { get; private set; }

        public FormSelectCustomer()
        {
            InitializeComponent();
            this.Load += FormSelectCustomer_Load;
            try { UIHelper.ApplyFormStyle(this); if (dgvCustomers != null) UIHelper.ApplyGridStyle(dgvCustomers); } catch { }
            this.KeyPreview = true;
            this.KeyDown += (s,e)=>{
                if (e.KeyCode == Keys.Enter) { btnSelect.PerformClick(); e.Handled=true; }
                else if (e.KeyCode == Keys.Escape) { btnCancel.PerformClick(); e.Handled=true; }
            };
        }

        private void FormSelectCustomer_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCustomers();
                SetupGrid();
                ApplyFilter();
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải khách hàng: "+ ex.Message);
            }
        }

        private void LoadCustomers()
        {
            _all = _customerBLL.GetAll() ?? new List<CustomerDTO>();
        }

        private void SetupGrid()
        {
            dgvCustomers.AutoGenerateColumns = false;
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = nameof(CustomerDTO.CustomerID), HeaderText = "ID", Width = 60 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = nameof(CustomerDTO.CustomerName), HeaderText = "Tên khách hàng", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = nameof(CustomerDTO.PhoneNumber), HeaderText = "Số điện thoại", Width = 120 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = nameof(CustomerDTO.Email), HeaderText = "Email", Width = 150 });
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.CellDoubleClick += (s,e)=>{ if (e.RowIndex>=0) { DoSelect(); } };
        }

        private void ApplyFilter()
        {
            string kw = (txtSearch.Text ?? string.Empty).Trim().ToLower();
            var filtered = string.IsNullOrEmpty(kw) ? _all : _all.Where(c =>
                (c.CustomerName??string.Empty).ToLower().Contains(kw) ||
                (c.PhoneNumber??string.Empty).Contains(kw) ||
                (c.Email??string.Empty).ToLower().Contains(kw)
            ).ToList();
            dgvCustomers.DataSource = filtered;
            lblInfo.Text = $"Tìm thấy {filtered.Count} khách hàng";
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            DoSelect();
        }

        private void DoSelect()
        {
            if (dgvCustomers.SelectedRows.Count == 0) { MessageBox.Show("Vui lòng chọn khách hàng"); return; }
            SelectedCustomer = dgvCustomers.SelectedRows[0].DataBoundItem as CustomerDTO;
            if (SelectedCustomer == null) return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
    }
}

