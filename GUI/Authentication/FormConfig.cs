using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.DAL;
using System.Data.SqlClient;
using QLTN_LT.GUI.Helper;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Authentication
{
    public partial class FormConfig : Form
    {
        private Guna2TextBox txtServer;
        private Guna2TextBox txtDatabase;
        private Guna2ComboBox cboAuth;
        private Guna2TextBox txtUsername;
        private Guna2TextBox txtPassword;
        private Guna2Button btnTest;
        private Guna2Button btnSave;
        private Guna2Button btnCancel;
        // Theme
        private Guna2ComboBox cboTheme;
        // Shortcuts helper
        private Guna2Button btnShortcuts;
        private Guna2TextBox txtShortcutHints;

        public FormConfig()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;
            
            this.BackColor = Color.White;
            
            // Initialize components container first
            this.components = new System.ComponentModel.Container();

            // Border
            var border = new Guna2BorderlessForm(this.components);
            border.ContainerControl = this;
            border.BorderRadius = 10;

            // Title
            var lblTitle = new Label();
            lblTitle.Text = "Cấu hình Kết nối";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(94, 148, 255);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;
            this.Controls.Add(lblTitle);

            // Server
            AddLabel("Server Name:", 70);
            txtServer = CreateTextBox(100);
            this.Controls.Add(txtServer);

            // Database
            AddLabel("Database Name:", 150);
            txtDatabase = CreateTextBox(180);
            this.Controls.Add(txtDatabase);

            // Auth
            AddLabel("Authentication:", 230);
            cboAuth = new Guna2ComboBox();
            cboAuth.Location = new Point(20, 260);
            cboAuth.Size = new Size(360, 36);
            cboAuth.Items.Add("Windows Authentication");
            cboAuth.Items.Add("SQL Server Authentication");
            cboAuth.SelectedIndex = 0;
            cboAuth.SelectedIndexChanged += CboAuth_SelectedIndexChanged;
            cboAuth.BorderColor = Color.FromArgb(213, 218, 223);
            cboAuth.BorderRadius = 5;
            this.Controls.Add(cboAuth);

            // Username
            AddLabel("User Name:", 310);
            txtUsername = CreateTextBox(340);
            txtUsername.Enabled = false;
            this.Controls.Add(txtUsername);

            // Password
            AddLabel("Password:", 390);
            txtPassword = CreateTextBox(420);
            txtPassword.PasswordChar = '*';
            txtPassword.Enabled = false;
            this.Controls.Add(txtPassword);

            // Buttons
            btnTest = CreateButton("Test Connection", 470, Color.Gray);
            btnTest.Click += BtnTest_Click;
            btnTest.Size = new Size(110, 40);
            btnTest.Location = new Point(20, 470); // Adjust Y
            
            btnSave = CreateButton("Save", 470, Color.FromArgb(94, 148, 255));
            btnSave.Click += BtnSave_Click;
            btnSave.Size = new Size(110, 40);
            btnSave.Location = new Point(145, 470);

            btnCancel = CreateButton("Cancel", 470, Color.FromArgb(255, 128, 128));
            btnCancel.Click += (s, e) => this.Close();
            btnCancel.Size = new Size(110, 40);
            btnCancel.Location = new Point(270, 470);

            this.Controls.Add(btnTest);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            // Theme selection
            AddLabel("Giao diện (Theme):", 520);
            cboTheme = new Guna2ComboBox();
            cboTheme.Location = new Point(20, 550);
            cboTheme.Size = new Size(360, 36);
            cboTheme.Items.Add("Light");
            cboTheme.Items.Add("Dark");
            cboTheme.BorderColor = Color.FromArgb(213, 218, 223);
            cboTheme.BorderRadius = 5;
            var prefsInit = UserPreferences.Load();
            cboTheme.SelectedIndex = (!string.IsNullOrWhiteSpace(prefsInit?.Theme) && prefsInit.Theme.Equals("Dark", StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
            this.Controls.Add(cboTheme);

            // Shortcuts section
            AddShortcutsSection();

            // Resize form to fit
            this.Size = new Size(400, 820);
        }

        private void AddShortcutsSection()
        {
            // Title
            var lblShortcuts = new Label();
            lblShortcuts.Text = "Gợi ý & phím tắt";
            lblShortcuts.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblShortcuts.ForeColor = Color.FromArgb(94, 148, 255);
            lblShortcuts.Location = new Point(20, 520);
            lblShortcuts.AutoSize = true;
            this.Controls.Add(lblShortcuts);

            // Multiline hints
            txtShortcutHints = new Guna2TextBox();
            txtShortcutHints.Location = new Point(20, 550);
            txtShortcutHints.Size = new Size(360, 90);
            txtShortcutHints.Multiline = true;
            txtShortcutHints.ReadOnly = true;
            txtShortcutHints.BorderRadius = 6;
            txtShortcutHints.Text = "F1: Hướng dẫn phím tắt\r\nCtrl + D: Đổi theme Light/Dark\r\nCtrl + Q: Tạo VietQR (Thanh toán)\r\nF5: Làm mới dữ liệu\r\nEnter: Xác nhận\r\nEsc: Đóng";
            this.Controls.Add(txtShortcutHints);

            // Open full guide button
            btnShortcuts = CreateButton("Xem đầy đủ phím tắt (F1)", 650, Color.MediumSlateBlue);
            btnShortcuts.Size = new Size(360, 36);
            btnShortcuts.Location = new Point(20, 650);
            btnShortcuts.Click += (s, e) =>
            {
                try { QLTN_LT.GUI.Helper.UIHelper.ShowFormDialog(this, new FormShortcuts()); } catch { }
            };
            this.Controls.Add(btnShortcuts);
        }

        private void AddLabel(string text, int y)
        {
            var lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.Gray;
            lbl.Location = new Point(20, y);
            lbl.AutoSize = true;
            this.Controls.Add(lbl);
        }

        private Guna2TextBox CreateTextBox(int y)
        {
            var txt = new Guna2TextBox();
            txt.Location = new Point(20, y);
            txt.Size = new Size(360, 36);
            txt.BorderRadius = 5;
            txt.BorderColor = Color.FromArgb(213, 218, 223);
            return txt;
        }

        private Guna2Button CreateButton(string text, int y, Color color)
        {
            var btn = new Guna2Button();
            btn.Text = text;
            btn.FillColor = color;
            btn.ForeColor = Color.White;
            btn.BorderRadius = 5;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            return btn;
        }

        private void CboAuth_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isSqlAuth = cboAuth.SelectedIndex == 1;
            txtUsername.Enabled = isSqlAuth;
            txtPassword.Enabled = isSqlAuth;
        }

        private void LoadSettings()
        {
            var settings = ConnectionSettings.Load();
            if (settings != null)
            {
                txtServer.Text = settings.Server;
                txtDatabase.Text = settings.Database;
                cboAuth.SelectedIndex = settings.IntegratedSecurity ? 0 : 1;
                txtUsername.Text = settings.Username;
                txtPassword.Text = settings.Password;
            }
            else
            {
                // Defaults
                txtServer.Text = @"TERRYKOZTE\SQLEXPRESS";
                txtDatabase.Text = "QLHS_LT";
                cboAuth.SelectedIndex = 0;
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs first
                if (!ValidateSettings())
                {
                    return;
                }

                var settings = GetSettingsFromUI();
                
                // Show loading state
                btnTest.Enabled = false;
                string originalText = btnTest.Text;
                btnTest.Text = "Đang kiểm tra...";

                try
                {
                    // Use a short timeout for test connection
                    var csb = new SqlConnectionStringBuilder(settings.GetConnectionString()) { ConnectTimeout = 5 };
                    using (var conn = new SqlConnection(csb.ConnectionString))
                    {
                        conn.Open();
                        
                        MessageBox.Show("✓ Kết nối thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                finally
                {
                    btnTest.Enabled = true;
                    btnTest.Text = originalText;
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Lỗi kết nối SQL:\n{sqlEx.Message}\n\nVui lòng kiểm tra:\n- Tên server\n- Tên database\n- Thông tin xác thực", 
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs first
                if (!ValidateSettings())
                {
                    return;
                }

                var settings = GetSettingsFromUI();
                
                // Test connection before saving
                try
                {
                    var csb = new SqlConnectionStringBuilder(settings.GetConnectionString()) { ConnectTimeout = 5 };
                    using (var conn = new SqlConnection(csb.ConnectionString))
                    {
                        conn.Open();
                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(
                        $"Không thể kết nối với cài đặt này:\n{ex.Message}\n\nBạn có muốn lưu dù sao không?",
                        "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    {
                        return;
                    }
                }

                // Save settings
                ConnectionSettings.Save(settings);

                // Save theme preference
                try
                {
                    var prefs = UserPreferences.Load() ?? new UserPreferences();
                    prefs.Theme = cboTheme.SelectedIndex == 1 ? "Dark" : "Light";
                    UserPreferences.Save(prefs);
                }
                catch { }
                
                MessageBox.Show("✓ Cài đặt đã được lưu thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cài đặt: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateSettings()
        {
            // Validate Server
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MessageBox.Show("Vui lòng nhập tên server.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServer.Focus();
                return false;
            }

            // Validate Database
            if (string.IsNullOrWhiteSpace(txtDatabase.Text))
            {
                MessageBox.Show("Vui lòng nhập tên database.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDatabase.Focus();
                return false;
            }

            // Validate SQL Auth credentials if selected
            if (cboAuth.SelectedIndex == 1) // SQL Server Authentication
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên người dùng cho SQL Server Authentication.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUsername.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu cho SQL Server Authentication.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return false;
                }
            }

            return true;
        }

        private ConnectionSettings GetSettingsFromUI()
        {
            return new ConnectionSettings
            {
                Server = txtServer.Text.Trim(),
                Database = txtDatabase.Text.Trim(),
                IntegratedSecurity = cboAuth.SelectedIndex == 0,
                Username = txtUsername.Text.Trim(),
                Password = txtPassword.Text
            };
        }

        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
