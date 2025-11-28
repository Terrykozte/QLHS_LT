using System;
using System.Drawing;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.IO;

namespace QLTN_LT.GUI.Authentication
{
    public partial class FormLogin : Form
    {
        private readonly AuthBLL _authBLL;

        public UserDTO LoggedInUser { get; private set; }

        public FormLogin()
        {
            InitializeComponent();
            _authBLL = new AuthBLL();
            
            // Setup UI Events
            SetupEvents();
            
            // Load Assets
            LoadAssets();
        }

        private void SetupEvents()
        {
            // Enter key to login
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
        }

        private void LoadAssets()
        {
            try
            {
                string resourcesPath = Path.Combine(Application.StartupPath, @"..\..\GUI\Resources");
                
                // Load Logo
                string logoPath = Path.Combine(resourcesPath, "logo.png");
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }

                // Load Background
                string bgPath = Path.Combine(resourcesPath, "login_bg.png");
                if (File.Exists(bgPath))
                {
                    pnlLeft.BackgroundImage = Image.FromFile(bgPath);
                    pnlLeft.BackgroundImageLayout = ImageLayout.Stretch;
                }

                // Load Icons
                string userIconPath = Path.Combine(resourcesPath, "user_icon.png");
                if (File.Exists(userIconPath))
                {
                    txtUsername.IconLeft = Image.FromFile(userIconPath);
                }

                string passIconPath = Path.Combine(resourcesPath, "pass_icon.png");
                if (File.Exists(passIconPath))
                {
                    txtPassword.IconLeft = Image.FromFile(passIconPath);
                }
            }
            catch (Exception ex)
            {
                // Log error or silently fail for assets
                Console.WriteLine("Error loading assets: " + ex.Message);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            btnLogin.Enabled = false;
            btnLogin.Text = "ĐANG XÁC THỰC...";

            try
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowError("Vui lòng nhập tên đăng nhập và mật khẩu.");
                    return;
                }

                var user = _authBLL.Login(txtUsername.Text, txtPassword.Text);

                if (user != null)
                {
                    LoggedInUser = user;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi hệ thống: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            
            // Shake Animation for the right panel
            var originalPos = pnlRight.Location;
            var rnd = new Random();
            const int shakeAmplitude = 5;
            for (int i = 0; i < 5; i++)
            {
                pnlRight.Location = new Point(originalPos.X + rnd.Next(-shakeAmplitude, shakeAmplitude), originalPos.Y);
                System.Threading.Thread.Sleep(20);
            }
            pnlRight.Location = originalPos;
        }
    }
}
