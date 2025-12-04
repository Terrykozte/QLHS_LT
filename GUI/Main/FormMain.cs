using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Guna.UI2.WinForms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Drawing.Drawing2D;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Main
{
    public partial class FormMain : BaseForm
    {
        private readonly UserDTO _currentUser;
        private Guna2Button _currentButton;
        private Form _activeForm;
        private bool _isSidebarExpanded = true;
        private Timer _sidebarTimer;
        private const int DefaultWidth = 1280;
        private const int DefaultHeight = 720;

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
            _sidebarTimer.Interval = 15;
            _sidebarTimer.Tick += SidebarTimer_Tick;
            
            // Setup form events
            this.Resize += FormMain_Resize;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            SetupUserInfo();
            ApplyRolePermissions();
            SetupTooltips();
            SetupKeyboardNavigation();
            SetupAnimations();
            ActivateButton(btnDashboard);
            // Mở trang Tổng quan khi vào MainSystem
            try { OpenChildForm(new QLTN_LT.GUI.Dashboard.FormDashboard()); } catch { }

            UpdateBorderRadius();
            UpdateTitle();
            PositionTitleButtons();
            
            // Apply responsive design
            ApplyResponsiveDesign();
        }



        private void FormMain_Resize(object sender, EventArgs e)
        {
            UpdateBorderRadius();
            PositionTitleButtons();
        }

        private void PositionTitleButtons()
        {
            try
            {
                if (pnlTitleBar == null || btnClose == null || btnMaximize == null || btnMinimize == null || btnSettings == null) return;
                int marginRight = 12;
                int gap = 2;
                // place from right to left
                btnClose.Left = pnlTitleBar.Width - btnClose.Width - marginRight;
                btnMaximize.Left = btnClose.Left - btnMaximize.Width - gap;
                btnMinimize.Left = btnMaximize.Left - btnMinimize.Width - gap;
                btnSettings.Left = btnMinimize.Left - btnSettings.Width - gap;
            }
            catch { }
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

            lblUserName.Text = string.IsNullOrWhiteSpace(_currentUser.FullName) ? _currentUser.Username : _currentUser.FullName;
            lblUserRole.Text = _currentUser.Roles != null && _currentUser.Roles.Count > 0 ? string.Join(", ", _currentUser.Roles) : "User";
        }

        private void ApplyRolePermissions()
        {
            try
            {
                var roles = (_currentUser?.Roles ?? new System.Collections.Generic.List<string>()).Select(r => r?.Trim().ToLower()).ToList();
                bool isAdmin = roles.Contains("admin");
                bool isStaff = roles.Contains("staff") ;

                // Default: show all
                Action<Guna2Button, bool> setVisible = (btn, visible) => { if (btn != null) btn.Visible = visible; };
                Action<Guna2Button, bool> setEnabled = (btn, enabled) => { if (btn != null) btn.Enabled = enabled; };

                if (isAdmin)
                {
                    // Admin full access
                    setVisible(btnUser, true);
                    setVisible(btnReports, true);
                    setVisible(btnSupplier, true);
                    setVisible(btnCategory, true);
                    setVisible(btnMenuQR, true);
                    setEnabled(btnUser, true);
                    setEnabled(btnReports, true);
                    setEnabled(btnSupplier, true);
                    setEnabled(btnCategory, true);
                    setEnabled(btnMenuQR, true);
                }
                else if (isStaff)
                {
                    // Staff limited
                    setVisible(btnUser, false);
                    setVisible(btnReports, false);
                    setVisible(btnSupplier, false);
                    setVisible(btnCategory, false);

                    setEnabled(btnDashboard, true);
                    setEnabled(btnOrders, true);
                    setEnabled(btnSeafood, true);
                    setEnabled(btnCustomer, true);
                    setEnabled(btnTable, true);
                    setEnabled(btnMenu, true);
                    setEnabled(btnInventory, true);
                    setEnabled(btnMenuQR, true);
                }
                else
                {
                    // Unknown role -> safest limited
                    setVisible(btnUser, false);
                    setVisible(btnReports, false);
                    setVisible(btnSupplier, false);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ApplyRolePermissions error: {ex.Message}");
            }
        }

        private bool HasAccessForButton(string buttonName)
        {
            var roles = (_currentUser?.Roles ?? new System.Collections.Generic.List<string>()).Select(r => r?.Trim().ToLower()).ToList();
            bool isAdmin = roles.Contains("admin") || roles.Contains("quanly") || roles.Contains("manager");
            bool isStaff = roles.Contains("staff") || roles.Contains("nhanvien");

            if (isAdmin) return true; // Admin full access

            if (isStaff)
            {
                // Staff allowed pages only:
                switch (buttonName)
                {
                    case "btnDashboard":
                    case "btnOrders":
                    case "btnSeafood":
                    case "btnCustomer":
                    case "btnTable":
                    case "btnMenu":
                    case "btnInventory":
                    case "btnMenuQR":
                        return true;
                    default:
                        return false; // Block: Users, Reports, Supplier, Category, etc.
                }
            }

            // Unknown role: safest is deny except Dashboard
            return buttonName == "btnDashboard";
        }

        private void NavigationButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Guna2Button button)) return;

            try
            {
                // Hard RBAC check
                if (!HasAccessForButton(button.Name))
                {
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này.", "Truy cập bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Show loading
                Wait(true);

                // Validate before navigation
                if (!ValidateBeforeNavigation(button.Name))
                {
                    Wait(false);
                    return;
                }

                ActivateButton(button);
                UpdateTitle();

                Form childForm = CreateFormForButton(button.Name);

                if (childForm != null)
                {
                    OpenChildForm(childForm);
                }
                else
                {
                    Wait(false);
                    MessageBox.Show("Không thể mở trang này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                GUI.Helper.ExceptionHandler.Handle(ex, $"Navigation: {button?.Name}");
            }
        }

        /// <summary>
        /// Creates the appropriate form for the given button.
        /// </summary>
        private Form CreateFormForButton(string buttonName)
        {
            try
            {
                return buttonName switch
                {
                    "btnDashboard" => new QLTN_LT.GUI.Dashboard.FormDashboard(),
                    "btnSeafood" => new QLTN_LT.GUI.Seafood.FormSeafoodList(),
                    "btnCategory" => new QLTN_LT.GUI.Category.FormCategoryList(),
                    "btnCustomer" => new QLTN_LT.GUI.Customer.FormCustomerList(),
                    "btnOrders" => new QLTN_LT.GUI.Order.FormOrderList(),
                    "btnTable" => new QLTN_LT.GUI.Table.FormTableList(),
                    "btnMenu" => new QLTN_LT.GUI.Menu.FormMenuList(),
                    "btnInventory" => new QLTN_LT.GUI.Inventory.FormInventoryManagement(),
                    "btnMenuQR" => new QLTN_LT.GUI.Menu.FormMenuQR(),
                    "btnSupplier" => new QLTN_LT.GUI.Supplier.FormSupplierList(),
                    "btnUser" => new QLTN_LT.GUI.User.FormUserList(),
                    "btnReports" => new QLTN_LT.GUI.Report.FormReportSales(),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating form: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Validates conditions before navigating to a page.
        /// </summary>
        private bool ValidateBeforeNavigation(string buttonName)
        {
            try
            {
                switch (buttonName)
                {
                    case "btnOrders":
                        // Check if there are any tables
                        var tables = new TableBLL().GetAll();
                        if (tables == null || tables.Count == 0)
                        {
                            MessageBox.Show("Vui lòng tạo bàn ăn trước khi tạo đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        break;

                    case "btnMenu":
                        // Check if there are any categories
                        var categories = new CategoryBLL().GetAll();
                        if (categories == null || categories.Count == 0)
                        {
                            MessageBox.Show("Vui lòng tạo danh mục trước khi tạo thực đơn.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating navigation: {ex.Message}");
                return true; // Allow navigation even if validation fails
            }
        }

        private ToolTip _toolTip;

        private void SetupTooltips()
        {
            try
            {
                _toolTip = new ToolTip
                {
                    AutoPopDelay = 4000,
                    InitialDelay = 400,
                    ReshowDelay = 200,
                    ShowAlways = true
                };
                if (btnDashboard != null) _toolTip.SetToolTip(btnDashboard, "Tổng quan");
                if (btnOrders != null) _toolTip.SetToolTip(btnOrders, "Quản lý đơn hàng");
                if (btnSeafood != null) _toolTip.SetToolTip(btnSeafood, "Kho hải sản");
                if (btnCustomer != null) _toolTip.SetToolTip(btnCustomer, "Khách hàng");
                if (btnTable != null) _toolTip.SetToolTip(btnTable, "Bàn ăn");
                if (btnMenu != null) _toolTip.SetToolTip(btnMenu, "Thực đơn");
                if (btnInventory != null) _toolTip.SetToolTip(btnInventory, "Kho hàng");
                if (btnReports != null) _toolTip.SetToolTip(btnReports, "Báo cáo");
                if (btnSupplier != null) _toolTip.SetToolTip(btnSupplier, "Nhà cung cấp");
                if (btnCategory != null) _toolTip.SetToolTip(btnCategory, "Danh mục");
                if (btnUser != null) _toolTip.SetToolTip(btnUser, "Người dùng");
                if (btnLogout != null) _toolTip.SetToolTip(btnLogout, "Đăng xuất");
            }
            catch { }
        }

        private void OpenChildForm(Form childForm)
        {
            try
            {
                Wait(true);

                // Đóng form hiện tại nếu có
                if (_activeForm != null)
                {
                    try
                    {
                        _activeForm.Hide();
                        _activeForm.Close();
                        _activeForm.Dispose();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error closing previous form: {ex.Message}");
                    }
                }

                // Hiển thị form mới (không animation để tránh flicker/màn phụ)
                _activeForm = childForm;
                childForm.TopLevel = false;
                childForm.FormBorderStyle = FormBorderStyle.None;
                childForm.BackColor = pnlContent.FillColor; // Match content background

                pnlContent.Controls.Clear();
                pnlContent.Controls.Add(childForm);
                pnlContent.Tag = childForm;

                childForm.Dock = DockStyle.Fill;
                childForm.Show();
                childForm.BringToFront();

                        UpdateTitle();
                        Wait(false);
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"Lỗi hiển thị trang: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine($"OpenChildForm error: {ex}");
            }
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
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                btnMaximize.Text = "□"; // Maximize icon
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐"; // Restore icon
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Setup keyboard navigation (Tab, Arrow keys)
        /// </summary>
        private void SetupKeyboardNavigation()
        {
            try
            {
                var navigableButtons = new List<Guna2Button>
                {
                    btnDashboard, btnSeafood, btnOrders, btnCustomer, 
                    btnTable, btnMenu, btnInventory, btnReports,
                    btnSupplier, btnCategory, btnUser, btnMenuQR, btnLogout
                };

                KeyboardNavigationHelper.RegisterForm(this, navigableButtons.Cast<Control>().ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up keyboard navigation: {ex.Message}");
            }
        }

        /// <summary>
        /// Setup animations cho các button
        /// </summary>
        private void SetupAnimations()
        {
            try
            {
                var buttons = new[] { btnDashboard, btnSeafood, btnOrders, btnCustomer, 
                    btnTable, btnMenu, btnInventory, btnReports, btnSupplier, 
                    btnCategory, btnUser, btnMenuQR, btnLogout };

                foreach (var btn in buttons)
                {
                    if (btn == null) continue;

                    // Hover effect
                    btn.MouseEnter += (s, e) =>
                    {
                        if (btn != _currentButton)
                        {
                            btn.FillColor = Color.FromArgb(31, 41, 55);
                            AnimationHelper.ScaleIn(btn, 150);
                        }
                    };

                    btn.MouseLeave += (s, e) =>
                    {
                        if (btn != _currentButton)
                        {
                            btn.FillColor = Color.Transparent;
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting up animations: {ex.Message}");
            }
        }

        /// <summary>
        /// Apply responsive design
        /// </summary>
        private void ApplyResponsiveDesign()
        {
            try
            {
                var screenSize = ResponsiveHelper.GetCurrentScreenSize(this);
                
                // Adjust sidebar width
                int sidebarWidth = ResponsiveHelper.GetResponsiveSidebarWidth(this);
                if (pnlSidebar != null)
                {
                    pnlSidebar.Width = sidebarWidth;
                }

                // Adjust font sizes
                if (lblBrand != null)
                {
                    lblBrand.Font = new Font(lblBrand.Font.FontFamily, 
                        ResponsiveHelper.GetResponsiveFontSize(12, this));
                }

                // Adjust button sizes
                var buttons = new[] { btnDashboard, btnSeafood, btnOrders, btnCustomer, 
                    btnTable, btnMenu, btnInventory, btnReports, btnSupplier, 
                    btnCategory, btnUser, btnMenuQR, btnLogout };

                foreach (var btn in buttons)
                {
                    if (btn == null) continue;
                    btn.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 40);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying responsive design: {ex.Message}");
            }
        }

    }
}

