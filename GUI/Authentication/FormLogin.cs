using System;
using System.Drawing;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.Infrastructure.Database;
using QLTN_LT.Infrastructure.Repositories;
using QLTN_LT.Core.Models;

namespace QLTN_LT.GUI.Authentication
{
    public partial class FormLogin : Form
    {
        private readonly AuthService _authService;
        private Timer _fadeTimer;

        public AppUser LoggedInUser { get; private set; }

        public FormLogin()
        {
            InitializeComponent();
            
            // Initialize Dependencies
            var dbContext = new DatabaseContext();
            var userRepo = new UserRepository(dbContext);
            _authService = new AuthService(userRepo);

            SetupUI();
        }

        private void SetupUI()
        {
            // Center the panel
            pnlMain.Location = new Point(
                (this.ClientSize.Width - pnlMain.Width) / 2,
                (this.ClientSize.Height - pnlMain.Height) / 2
            );

            // Fade In Animation
            this.Opacity = 0;
            _fadeTimer = new Timer();
            _fadeTimer.Interval = 10;
            _fadeTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 1)
                    this.Opacity += 0.05;
                else
                    _fadeTimer.Stop();
            };
            _fadeTimer.Start();

            // Load Logo if exists
            try
            {
                string logoPath = System.IO.Path.Combine(Application.StartupPath, @"..\..\GUI\Resources\logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
                else
                {
                    // Fallback or hide if no logo
                    picLogo.Visible = false;
                    lblTitle.Location = new Point(lblTitle.Location.X, lblTitle.Location.Y - 50);
                }
            }
            catch { picLogo.Visible = false; }

            // Enter key to login
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
        }

        private void txtPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtPassword.PasswordChar == '•')
            {
                txtPassword.PasswordChar = '\0';
                txtPassword.IconRight = null; // Or change to 'hide' icon if available
            }
            else
            {
                txtPassword.PasswordChar = '•';
                txtPassword.IconRight = null; // Or change to 'show' icon
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            btnLogin.Enabled = false;
            btnLogin.Text = "VERIFYING...";

            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowError("Please enter username and password.");
                    return;
                }

                var result = await _authService.LoginAsync(txtUsername.Text, txtPassword.Text);

                if (result.IsSuccess)
                {
                    LoggedInUser = result.Data;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowError(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ShowError("System Error: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "LOGIN";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            // lblError.Location logic is removed as Guna2HtmlLabel handles alignment or is docked/autosized differently
            // If needed, we can set TextAlignment in Designer or here.
            
            // Shake Animation
            var originalPos = pnlMain.Location;
            var rnd = new Random();
            const int shakeAmplitude = 10;
            for (int i = 0; i < 10; i++)
            {
                pnlMain.Location = new Point(originalPos.X + rnd.Next(-shakeAmplitude, shakeAmplitude), originalPos.Y);
                System.Threading.Thread.Sleep(20);
            }
            pnlMain.Location = originalPos;
            
            // Optional: Show dialog for critical errors if needed, but inline label is better for login
            // messageDialog.Show(message, "Login Error"); 
        }

        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            messageDialog.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
            messageDialog.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
            messageDialog.Show("Please contact the administrator to reset your password.", "Forgot Password");
        }
    }
}
