using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;ntication;

namespace QLTN_LT.GUI.Main
{
    public partial class FormMain : Form
    {
        private readonly AppUser _currentUser;
        private Guna2Button _currentButton;
        private Form _activeForm;

        public FormMain(AppUser user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            SetupUserInfo();
            ActivateButton(btnDashboard);
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
            lblPageTitle.Text = button.Text; // Updated from lblTitle

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
            if (_activeForm != null)
                _activeForm.Close();

            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(childForm);
            pnlContent.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void ActivateButton(Guna2Button button)
        {
            if (_currentButton != null)
            {
                _currentButton.Checked = false;
                _currentButton.ForeColor = Color.FromArgb(156, 163, 175); // Reset to Gray
            }

            _currentButton = button;
            _currentButton.Checked = true;
            _currentButton.ForeColor = Color.FromArgb(19, 146, 236); // Active Blue
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Cài đặt đang được phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

