using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Customer
{
    public partial class FormCustomerEdit : FormTemplate
    {
        private int _customerId;
        private CustomerBLL _bll;

        public FormCustomerEdit(int customerId)
        {
            InitializeComponent();
            _customerId = customerId;
            _bll = new CustomerBLL();
            try { UIHelper.ApplyFormStyle(this); } catch { }

            this.Load += (s, e) =>
            {
                try
                {
                    InitializeEditMode();
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, "FormCustomerEdit_Load");
                    this.Close();
                }
            };
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
        private void FormCustomerEdit_Load(object sender, EventArgs e)
        {
            InitializeEditMode();
            LoadData();
        }

        protected override void LoadData()
        {
            try
            {
                var customer = _bll.GetById(_customerId);
                if (customer == null)
                {
                    ShowError("Không tìm thấy khách hàng.");
                    this.Close();
                    return;
                }
                txtName.Text = customer.CustomerName;
                txtPhone.Text = customer.PhoneNumber;
                txtAddress.Text = customer.Address;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadCustomerData");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
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
            var updatedCustomer = new CustomerDTO
            {
                CustomerID = _customerId,
                CustomerName = txtName.Text?.Trim(),
                PhoneNumber = txtPhone.Text?.Trim(),
                Address = txtAddress.Text?.Trim(),
                // Email field not present on this form; leave as-is or handle elsewhere
            };

            _bll.Update(updatedCustomer);
        }

        protected override void CleanupResources()
        {
            try { _bll = null; } catch { }
            finally { base.CleanupResources(); }
        }
    }
}
