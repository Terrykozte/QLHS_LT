using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserAdd : Form
    {
        private readonly UserBLL _userBLL;

        public FormUserAdd()
        {
            InitializeComponent();
            _userBLL = new UserBLL();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            var newUser = new UserDTO
            {
                Username = txtUsername.Text.Trim(),
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                IsActive = swActive.Checked,
                Roles = new List<string> { cboRole.SelectedItem.ToString() }
            };

            try
            {
                var result = _userBLL.Insert(newUser, txtPassword.Text);
                if (result)
                {
                    MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
