using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserEdit : FormTemplate
    {
        private UserBLL _userBLL;
        private UserDTO _user;

        public FormUserEdit(UserDTO user)
        {
            InitializeComponent();
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _userBLL = new UserBLL();

            // UX & styling
            try
            {
                UIHelper.ApplyFormStyle(this);
            }
            catch { }

            // Keyboard shortcuts
            this.KeyPreview = true;
            this.KeyDown += FormUserEdit_KeyDown;

            // Wire designer buttons to base template handlers
            if (btnSave != null) btnSave.Click += (s, e) => BtnSave_Click(s, e);
            if (btnCancel != null) btnCancel.Click += (s, e) => BtnCancel_Click(s, e);
            if (btnResetPassword != null) btnResetPassword.Click += btnResetPassword_Click;

            // Load mode
            this.Load += (s, e) =>
            {
                try
                {
                    InitializeEditMode();
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, "FormUserEdit_Load");
                    this.Close();
                }
            };
        }

        private void FormUserEdit_KeyDown(object sender, KeyEventArgs e)
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

        protected override void LoadData()
        {
            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            txtPhone.Text = _user.Phone;
            swActive.Checked = _user.IsActive;

            if (_user.Roles != null && _user.Roles.Count > 0 && cboRole != null)
            {
                cboRole.SelectedItem = _user.Roles[0];
            }
        }

        protected override bool ValidateInputs()
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(txtFullName?.Text))
            {
                UIHelper.ShowValidationError(txtFullName, "Họ tên không được để trống");
                valid = false;
            }
            else UIHelper.ClearValidationError(txtFullName);

            if (!string.IsNullOrWhiteSpace(txtEmail?.Text) && !IsValidEmail(txtEmail.Text))
            {
                UIHelper.ShowValidationError(txtEmail, "Email không hợp lệ");
                valid = false;
            }
            else UIHelper.ClearValidationError(txtEmail);

            if (!string.IsNullOrWhiteSpace(txtPhone?.Text) && !Regex.IsMatch(txtPhone.Text, "^\\+?[0-9]{8,15}$"))
            {
                UIHelper.ShowValidationError(txtPhone, "SĐT không hợp lệ");
                valid = false;
            }
            else UIHelper.ClearValidationError(txtPhone);

            if (cboRole == null || cboRole.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn vai trò", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                valid = false;
            }

            return valid;
        }

        protected override void SaveData()
        {
            _user.FullName = txtFullName?.Text?.Trim();
            _user.Email = txtEmail?.Text?.Trim();
            _user.Phone = txtPhone?.Text?.Trim();
            _user.IsActive = swActive != null && swActive.Checked;
            if (cboRole?.SelectedItem != null)
            {
                _user.Roles = new List<string> { cboRole.SelectedItem.ToString() };
            }

            var ok = _userBLL.Update(_user);
            if (!ok)
            {
                throw new InvalidOperationException("Không thể cập nhật người dùng.");
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = Microsoft.VisualBasic.Interaction.InputBox("Nhập mật khẩu mới:", "Đặt lại mật khẩu", "");
            if (string.IsNullOrWhiteSpace(newPassword)) return;

            try
            {
                var result = _userBLL.ChangePassword(_user.UserID, newPassword);
                if (result)
                {
                    MessageBox.Show("Đặt lại mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Đặt lại mật khẩu thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Designer-bound button handlers
        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                return Regex.IsMatch(email,
                    @"^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$",
                    RegexOptions.IgnoreCase);
            }
            catch { return false; }
        }

        protected override void CleanupResources()
        {
            try { _userBLL = null; _user = null; }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
