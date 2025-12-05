using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserAdd : FormTemplate
    {
        private UserBLL _userBLL;

        public FormUserAdd()
        {
            InitializeComponent();
            _userBLL = new UserBLL();

            // UX & styling
            try
            {
                UIHelper.ApplyFormStyle(this);
            }
            catch { }

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += FormUserAdd_KeyDown;

            // Wire designer buttons to base template handlers (designer also wires to btnSave_Click/btnCancel_Click)
            if (btnSave != null) btnSave.Click += BtnSave_Click;
            if (btnCancel != null) btnCancel.Click += BtnCancel_Click;

            // Minor realtime validations
            if (txtUsername != null) txtUsername.TextChanged += (s, e) => UIHelper.ClearValidationError(txtUsername);
            if (txtPassword != null) txtPassword.TextChanged += (s, e) => UIHelper.ClearValidationError(txtPassword);
            if (txtEmail != null) txtEmail.TextChanged += (s, e) => UIHelper.ClearValidationError(txtEmail);
            if (txtPhone != null) txtPhone.TextChanged += (s, e) => UIHelper.ClearValidationError(txtPhone);
        }

        // Keep these to satisfy Designer's event bindings
        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormUserAdd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSave_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    BtnCancel_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
            }
            catch { }
        }

        protected override bool ValidateInputs()
        {
            bool valid = true;

            // Username
            if (string.IsNullOrWhiteSpace(txtUsername?.Text))
            {
                UIHelper.ShowValidationError(txtUsername, "Vui lòng nhập tên đăng nhập");
                valid = false;
            }
            else if (txtUsername.Text.Length < 3)
            {
                UIHelper.ShowValidationError(txtUsername, "Tên đăng nhập tối thiểu 3 ký tự");
                valid = false;
            }
            else
            {
                UIHelper.ClearValidationError(txtUsername);
            }

            // Password
            if (string.IsNullOrWhiteSpace(txtPassword?.Text))
            {
                UIHelper.ShowValidationError(txtPassword, "Vui lòng nhập mật khẩu");
                valid = false;
            }
            else if (txtPassword.Text.Length < 4)
            {
                UIHelper.ShowValidationError(txtPassword, "Mật khẩu tối thiểu 4 ký tự");
                valid = false;
            }
            else
            {
                UIHelper.ClearValidationError(txtPassword);
            }

            // Email (optional but if present must be valid)
            if (!string.IsNullOrWhiteSpace(txtEmail?.Text))
            {
                if (!IsValidEmail(txtEmail.Text))
                {
                    UIHelper.ShowValidationError(txtEmail, "Email không hợp lệ");
                    valid = false;
                }
                else UIHelper.ClearValidationError(txtEmail);
            }

            // Phone (optional but if present must be digits and length >= 8)
            if (!string.IsNullOrWhiteSpace(txtPhone?.Text))
            {
                if (!Regex.IsMatch(txtPhone.Text, "^\\+?[0-9]{8,15}$"))
                {
                    UIHelper.ShowValidationError(txtPhone, "SĐT không hợp lệ");
                    valid = false;
                }
                else UIHelper.ClearValidationError(txtPhone);
            }

            // Role
            if (cboRole == null || cboRole.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn vai trò", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                valid = false;
            }

            return valid;
        }

        protected override void SaveData()
        {
            var newUser = new UserDTO
            {
                Username = txtUsername?.Text?.Trim(),
                FullName = txtFullName?.Text?.Trim(),
                Email = txtEmail?.Text?.Trim(),
                Phone = txtPhone?.Text?.Trim(),
                IsActive = swActive != null && swActive.Checked,
                Roles = new List<string> { cboRole.SelectedItem.ToString() }
            };

            var ok = _userBLL.Insert(newUser, txtPassword?.Text);
            if (!ok)
            {
                throw new InvalidOperationException("Không thể thêm người dùng.");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                // Simple RFC 5322 compliant pattern (lightweight)
                return Regex.IsMatch(email,
                    @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$",
                    RegexOptions.IgnoreCase);
            }
            catch { return false; }
        }

        protected override void CleanupResources()
        {
            try { _userBLL = null; }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
