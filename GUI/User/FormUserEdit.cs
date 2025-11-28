using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserEdit : Form
    {
        private readonly UserBLL _userBLL;
        private readonly UserDTO _user;

        public FormUserEdit(UserDTO user)
        {
            InitializeComponent();
            _user = user;
            _userBLL = new UserBLL();
            LoadUserData();
        }

        private void LoadUserData()
        {
            txtUsername.Text = _user.Username;
            txtFullName.Text = _user.FullName;
            txtEmail.Text = _user.Email;
            txtPhone.Text = _user.Phone;
            swActive.Checked = _user.IsActive;
            
            // Basic Role Selection Logic
            if (_user.Roles != null && _user.Roles.Count > 0)
            {
                cboRole.SelectedItem = _user.Roles[0];
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _user.FullName = txtFullName.Text.Trim();
            _user.Email = txtEmail.Text.Trim();
            _user.Phone = txtPhone.Text.Trim();
            _user.IsActive = swActive.Checked;
            
            if (cboRole.SelectedItem != null)
            {
                _user.Roles = new List<string> { cboRole.SelectedItem.ToString() };
            }

            try
            {
                var result = _userBLL.Update(_user);
                if (result)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
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
    }
}
