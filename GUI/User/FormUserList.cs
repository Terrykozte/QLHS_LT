using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.User
{
    public partial class FormUserList : BaseForm
    {
        private UserBLL _userBLL;
        private Timer _searchDebounce;

        public FormUserList()
        {
            InitializeComponent();
            _userBLL = new UserBLL();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormUserList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvUser != null) UIHelper.ApplyGridStyle(dgvUser);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
            }
            catch { }

            _searchDebounce = new Timer { Interval = 350 };
            _searchDebounce.Tick += (s, e) => { _searchDebounce.Stop(); LoadData(); };

            this.Load += (s, e) =>
            {
                try
                {
                    LoadData();
                    if (txtSearch != null) txtSearch.TextChanged += txtSearch_TextChanged;
                    if (btnAdd != null) btnAdd.Click += btnAdd_Click;
                    if (dgvUser != null) dgvUser.CellDoubleClick += dgvUser_CellDoubleClick;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, "FormUserList_Load");
                }
            };
        }

        // Designer-bound handler
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounce?.Stop();
            _searchDebounce?.Start();
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                var users = _userBLL.GetAll();

                string keyword = txtSearch?.Text?.ToLower() ?? string.Empty;
                if (!string.IsNullOrEmpty(keyword))
                {
                    users = users.Where(u =>
                        (u.Username?.ToLower().Contains(keyword) ?? false) ||
                        (u.FullName?.ToLower().Contains(keyword) ?? false) ||
                        (u.Email?.ToLower().Contains(keyword) ?? false)
                    ).ToList();
                }

                if (dgvUser != null)
                {
                    dgvUser.AutoGenerateColumns = true; // assuming designer columns already set; otherwise we can configure
                    dgvUser.DataSource = users;

                    if (dgvUser.Columns["PasswordHash"] != null) dgvUser.Columns["PasswordHash"].Visible = false;
                    if (dgvUser.Columns["PasswordSalt"] != null) dgvUser.Columns["PasswordSalt"].Visible = false;
                    if (dgvUser.Columns["Roles"] != null) dgvUser.Columns["Roles"].Visible = false;

                    if (dgvUser.Columns["Username"] != null) dgvUser.Columns["Username"].HeaderText = "Tên đăng nhập";
                    if (dgvUser.Columns["FullName"] != null) dgvUser.Columns["FullName"].HeaderText = "Họ tên";
                    if (dgvUser.Columns["Email"] != null) dgvUser.Columns["Email"].HeaderText = "Email";
                    if (dgvUser.Columns["Phone"] != null) dgvUser.Columns["Phone"].HeaderText = "SĐT";
                    if (dgvUser.Columns["IsActive"] != null) dgvUser.Columns["IsActive"].HeaderText = "Hoạt động";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadUserData");
            }
            finally { Wait(false); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormUserAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnAdd_Click");
            }
        }

        private void dgvUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvUser?.Rows.Count > e.RowIndex)
                {
                    var user = dgvUser.Rows[e.RowIndex].DataBoundItem as UserDTO;
                    if (user != null)
                    {
                        using (var form = new FormUserEdit(user))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                LoadData();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "dgvUser_CellDoubleClick");
            }
        }

        private void FormUserList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadData();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvUser?.CurrentRow != null)
                {
                    dgvUser_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvUser.CurrentRow.Index));
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    txtSearch?.Clear();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounce?.Stop();
                _searchDebounce?.Dispose();
                _userBLL = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
