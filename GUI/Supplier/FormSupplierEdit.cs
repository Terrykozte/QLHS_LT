using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Supplier
{
    public partial class FormSupplierEdit : FormTemplate
    {
        private int _supplierId;
        private SupplierBLL _bll;

        public FormSupplierEdit(int supplierId)
        {
            InitializeComponent();
            _supplierId = supplierId;
            _bll = new SupplierBLL();
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
                    ExceptionHandler.Handle(ex, "FormSupplierEdit_Load");
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
        private void FormSupplierEdit_Load(object sender, EventArgs e)
        {
            InitializeEditMode();
            LoadData();
        }

        protected override void LoadData()
        {
            try
            {
                var supplier = _bll.GetById(_supplierId);
                if (supplier == null)
                {
                    ShowError("Không tìm thấy nhà cung cấp.");
                    this.Close();
                    return;
                }
                txtName.Text = supplier.SupplierName;
                txtContactPerson.Text = supplier.ContactPerson;
                txtPhone.Text = supplier.PhoneNumber;
                txtEmail.Text = supplier.Email;
                txtAddress.Text = supplier.Address;
                chkIsActive.Checked = supplier.IsActive;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadSupplierData");
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
                UIHelper.ShowValidationError(txtName, "Tên nhà cung cấp không được để trống");
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
            var updatedSupplier = new SupplierDTO
            {
                SupplierID = _supplierId,
                SupplierName = txtName.Text?.Trim(),
                ContactPerson = txtContactPerson.Text?.Trim(),
                PhoneNumber = txtPhone.Text?.Trim(),
                Email = txtEmail.Text?.Trim(),
                Address = txtAddress.Text?.Trim(),
                IsActive = chkIsActive.Checked
            };

            _bll.Update(updatedSupplier);
        }

        protected override void CleanupResources()
        {
            try { _bll = null; } catch { }
            finally { base.CleanupResources(); }
        }
    }
}
