using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Drawing.Drawing2D;
namespace QLTN_LT.GUI.Main
{
    public partial class FormMain : Form
    {
        private readonly UserDTO _currentUser;
        private Guna2Button _currentButton;
        private Form _activeForm;
        private bool _isSidebarExpanded = true;
        private Timer _sidebarTimer;
        private const int DefaultWidth = 1024;
        private const int DefaultHeight = 576;

        public FormMain(UserDTO user)
        {
            InitializeComponent();
            _currentUser = user;
            this.Padding = new Padding(0);
            // Cho phép resize - không cố định kích thước
            this.MinimumSize = new Size(800, 500); // Kích thước tối thiểu hợp lý
            this.Size = new Size(DefaultWidth, DefaultHeight);

            // Sử dụng custom title bar với Guna UI2
            this.FormBorderStyle = FormBorderStyle.None;

            _sidebarTimer = new Timer();
            _sidebarTimer.Interval = 10;
            _sidebarTimer.Tick += SidebarTimer_Tick;
            
            // Setup form events
            this.Resize += FormMain_Resize;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            SetupUserInfo();
            ActivateButton(btnDashboard);
            SetupLayout();
            UpdateBorderRadius();
            UpdateTitle();
        }

        private void SetupLayout()
        {
            // Sidebar bên trái, Content bên phải
            pnlSidebar.Dock = DockStyle.Left;
            pnlContent.Dock = DockStyle.Fill;
            
            // Đảm bảo title bar nằm trên content
            pnlTitleBar.Dock = DockStyle.Top;
            
            // Cải thiện styling
            pnlSidebar.FillColor = Color.FromArgb(17, 24, 39); // Dark sidebar
            pnlContent.FillColor = Color.FromArgb(249, 250, 251); // Light content area
            pnlTitleBar.FillColor = Color.FromArgb(239, 246, 255); // Blue-50 title bar
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            UpdateBorderRadius();
        }

        private void UpdateBorderRadius()
        {
            // Đảm bảo các panel không có border radius
            pnlSidebar.BorderRadius = 0;
            pnlContent.BorderRadius = 0;
            pnlTitleBar.BorderRadius = 0;
        }

        private void UpdateTitle()
        {
            if (lblTitle != null)
            {
                lblTitle.Text = "Quản lý cửa hàng" + (_currentButton != null ? " - " + _currentButton.Text : "");
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (_isSidebarExpanded)
            {
                pnlSidebar.Width -= 10;
                if (pnlSidebar.Width <= 60)
                {
                    _isSidebarExpanded = false;
                    _sidebarTimer.Stop();
                }
            }
            else
            {
                pnlSidebar.Width += 10;
                if (pnlSidebar.Width >= 240)
                {
                    _isSidebarExpanded = true;
                    _sidebarTimer.Stop();
                }
            }
        }

        private void btnToggleSidebar_Click(object sender, EventArgs e)
        {
            _sidebarTimer.Start();
        }

        private void SetupUserInfo()
        {
            if (_currentUser == null) return;

            lblUserName.Text = _currentUser.FullName;
            lblUserRole.Text = _currentUser.Roles?.Count > 0 ? _currentUser.Roles[0] : "User";
        }

        private void NavigationButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Guna2Button button)) return;

            ActivateButton(button);
            // Cập nhật tiêu đề với tên trang hiện tại
            UpdateTitle();

            switch (button.Name)
            {
                case "btnDashboard":
                    OpenChildForm(new QLTN_LT.GUI.Dashboard.FormDashboard());
                    break;
                case "btnSeafood":
                    OpenChildForm(new QLTN_LT.GUI.Seafood.FormSeafoodList());
                    break;
                case "btnCategory":
                    OpenChildForm(new QLTN_LT.GUI.Category.FormCategoryList());
                    break;
                case "btnCustomer":
                    OpenChildForm(new QLTN_LT.GUI.Customer.FormCustomerList());
                    break;
                case "btnOrders":
                    OpenChildForm(new QLTN_LT.GUI.Order.FormOrderList());
                    break;
                case "btnTable":
                    OpenChildForm(new QLTN_LT.GUI.Table.FormTableList());
                    break;
                case "btnMenu":
                    OpenChildForm(new QLTN_LT.GUI.Menu.FormMenuList());
                    break;
                case "btnInventory":
                    OpenChildForm(new QLTN_LT.GUI.Inventory.FormInventoryList());
                    break;
                case "btnSupplier":
                    OpenChildForm(new QLTN_LT.GUI.Supplier.FormSupplierList());
                    break;
                case "btnUser":
                    OpenChildForm(new QLTN_LT.GUI.User.FormUserList());
                    break;
                case "btnReports":
                    OpenChildForm(new QLTN_LT.GUI.Report.FormReportSales());
                    break;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            // Đóng form hiện tại nếu có
            if (_activeForm != null)
            {
                _activeForm.Hide();
                _activeForm.Close();
                _activeForm.Dispose();
            }

            // Tạo form mới
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.BackColor = pnlContent.FillColor; // Match content background
            
            // Clear và add form mới
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;
            
            childForm.BringToFront();
            childForm.Show();
            
            // Cập nhật title
            UpdateTitle();
        }

        private void ActivateButton(Guna2Button button)
        {
            if (_currentButton != null)
            {
                _currentButton.Checked = false;
                _currentButton.ForeColor = Color.FromArgb(156, 163, 175); // Reset to Gray
                _currentButton.FillColor = Color.Transparent;
            }

            _currentButton = button;
            _currentButton.Checked = true;
            _currentButton.ForeColor = Color.FromArgb(59, 130, 246); // Active Blue-500
            _currentButton.FillColor = Color.FromArgb(31, 41, 55); // Active background
            _currentButton.CheckedState.FillColor = Color.FromArgb(31, 41, 55);
            _currentButton.CheckedState.ForeColor = Color.FromArgb(59, 130, 246);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new QLTN_LT.GUI.Authentication.FormConfig())
                {
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở cài đặt: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            // Toggle maximize/restore
            if (this.WindowState == FormWindowState.Normal)
            {
                // Maximize
                Screen screen = Screen.FromControl(this);
                this.Location = screen.WorkingArea.Location;
                this.Size = screen.WorkingArea.Size;
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐"; // Restore icon
            }
            else
            {
                // Restore
                this.WindowState = FormWindowState.Normal;
                // Center form
                Screen screen = Screen.FromControl(this);
                int x = (screen.WorkingArea.Width - this.Width) / 2 + screen.WorkingArea.Left;
                int y = (screen.WorkingArea.Height - this.Height) / 2 + screen.WorkingArea.Top;
                this.Location = new Point(x, y);
                btnMaximize.Text = "□"; // Maximize icon
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

