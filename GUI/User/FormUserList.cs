using System;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserList : Form
    {
        private readonly UserBLL _userBLL;

        public FormUserList()
        {
            InitializeComponent();
            _userBLL = new UserBLL();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var users = _userBLL.GetAll();
                
                string keyword = txtSearch.Text.ToLower();
                if (!string.IsNullOrEmpty(keyword))
                {
                    users = users.Where(u => 
                        u.Username.ToLower().Contains(keyword) || 
                        u.FullName.ToLower().Contains(keyword) ||
                        u.Email.ToLower().Contains(keyword)
                    ).ToList();
                }

                dgvUser.DataSource = users;
                
                // Configure Columns if not already done in Designer or here
                if (dgvUser.Columns["PasswordHash"] != null) dgvUser.Columns["PasswordHash"].Visible = false;
                if (dgvUser.Columns["PasswordSalt"] != null) dgvUser.Columns["PasswordSalt"].Visible = false;
                if (dgvUser.Columns["Roles"] != null) dgvUser.Columns["Roles"].Visible = false;
                
                // Set Headers
                if (dgvUser.Columns["Username"] != null) dgvUser.Columns["Username"].HeaderText = "Tên đăng nhập";
                if (dgvUser.Columns["FullName"] != null) dgvUser.Columns["FullName"].HeaderText = "Họ tên";
                if (dgvUser.Columns["Email"] != null) dgvUser.Columns["Email"].HeaderText = "Email";
                if (dgvUser.Columns["Phone"] != null) dgvUser.Columns["Phone"].HeaderText = "SĐT";
                if (dgvUser.Columns["IsActive"] != null) dgvUser.Columns["IsActive"].HeaderText = "Hoạt động";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormUserAdd())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void dgvUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var user = dgvUser.Rows[e.RowIndex].DataBoundItem as UserDTO;
                if (user != null)
                {
                    using (var form = new FormUserEdit(user))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
