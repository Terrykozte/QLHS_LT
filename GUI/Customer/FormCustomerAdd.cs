using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Customer
{
    public partial class FormCustomerAdd : FormTemplate
    {
        private CustomerBLL _bll;

        public FormCustomerAdd()
        {
            InitializeComponent();
            _bll = new CustomerBLL();
            try { UIHelper.ApplyFormStyle(this); } catch { }
        }

        // Designer-bound handlers (stubs)
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control c) UIHelper.ClearValidationError(c);
        }
        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control c) UIHelper.ClearValidationError(c);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ValidateInputs()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                UIHelper.ShowValidationError(txtName, "Tên khách hàng không được để trống");
                valid = false;
            }
            else UIHelper.ClearValidationError(txtName);

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                UIHelper.ShowValidationError(txtPhone, "Số điện thoại không được để trống");
                valid = false;
            }
            else UIHelper.ClearValidationError(txtPhone);

            return valid;
        }

        protected override void SaveData()
        {
            var newCustomer = new CustomerDTO
            {
                CustomerName = txtName.Text?.Trim(),
                PhoneNumber = txtPhone.Text?.Trim(),
                Address = txtAddress.Text?.Trim()
            };
            _bll.Insert(newCustomer);
        }

        protected override void CleanupResources()
        {
            try { _bll = null; } catch { }
            finally { base.CleanupResources(); }
        }
    }
}
