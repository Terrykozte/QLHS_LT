using System;
using System.Drawing;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing.Drawing2D;
using QLTN_LT.DAL;
using System.Data.SqlClient;

namespace QLTN_LT.GUI.Authentication
{
    public partial class FormLogin : Form
    {
        // Responsive split layout: left/right panels
        private const float LeftRatioDefault = 0.42f;    // ~42% at 1024
        private const float LeftRatioFullscreen = 0.45f; // a bit more visual weight on large screens
        private const int LeftMinWidth = 320;            // keep logo area usable
        private const int RightMinWidth = 480;           // keep login form usable

        private readonly AuthBLL _authBLL;
        public UserDTO LoggedInUser { get; private set; }

        private Timer _animationTimer;
        private Timer _fadeInTimer;
        private Timer _capsLockTimer;
        private Timer _inputValidationTimer;

        private float _glowOpacity = 0.3f;
        private bool _glowIncreasing = true;
        private float _pulseScale = 1.0f;
        private bool _pulseIncreasing = true;

        private static readonly Color PrimaryGradientTop = Color.FromArgb(30, 58, 138);
        private static readonly Color PrimaryGradientBottom = Color.FromArgb(59, 130, 246);
        private static readonly Color RightPanelColor = Color.FromArgb(255, 255, 255);
        private static readonly Color TitleBarColor = Color.FromArgb(239, 246, 255);
        private static readonly Color TitleBarBorderColor = Color.FromArgb(191, 219, 254);
        private static readonly Color TitleBarTextColor = Color.FromArgb(30, 58, 138);
        private static readonly Color PrimaryAccentColor = Color.FromArgb(59, 130, 246);
        private static readonly Color PrimaryAccentHoverColor = Color.FromArgb(37, 99, 235);
        private static readonly Color ErrorColor = Color.FromArgb(239, 68, 68);
        private static readonly Color WarningColor = Color.FromArgb(251, 146, 60);
        private static readonly Color SuccessColor = Color.FromArgb(34, 197, 94);
        private static readonly Color InputBorderColor = Color.FromArgb(17, 24, 39);
        private static readonly Color ButtonBorderColor = Color.FromArgb(17, 24, 39);
        private static readonly Color TextPrimaryColor = Color.FromArgb(17, 24, 39);
        private static readonly Color TitleBarButtonHover = Color.FromArgb(219, 234, 254);

        private string _brandName = "Quản Lý Hải Sản";
        private string _logoFileName = "logo.png";

        private bool _usernameValid = false;
        private bool _passwordValid = false;
        private bool _isValidating = false;
        private Size _normalSize = new Size(1024, 640);
        private Point _normalLocation;
        private int _originalBorderRadius = 15;
        private ClientSettings _clientSettings;

        public FormLogin()
        {
            InitializeComponent();
            _authBLL = new AuthBLL();

            // Improve UX
            this.KeyPreview = true; // Capture form-level hotkeys (ESC, Ctrl+1/2)

            InitializeTimers();
            SetupEvents();
            LoadAssets();
            ApplyStyles();

            this.Load += FormLogin_Load;
            this.Resize += FormLogin_Resize;
            this.FormClosing += FormLogin_FormClosing;
            this.Shown += FormLogin_Shown;

            this.Opacity = 0;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = _normalSize;

            // Ensure size fits the current screen and set a sensible minimum
            var wa = Screen.FromControl(this).WorkingArea;
            if (wa.Width < _normalSize.Width || wa.Height < _normalSize.Height)
            {
                _normalSize = new Size(Math.Min(_normalSize.Width, wa.Width), Math.Min(_normalSize.Height, wa.Height));
                this.Size = _normalSize;
            }
            this.MinimumSize = new Size(Math.Min(1024, wa.Width), Math.Min(640, wa.Height));

            // Initialize split docking for left/right panels
            InitializeSplitDocking();

            if (guna2Elipse1 != null)
            {
                _originalBorderRadius = guna2Elipse1.BorderRadius;
            }

            this.Load += (s, e) =>
            {
                Screen screen = Screen.FromControl(this);
                int x = (screen.WorkingArea.Width - _normalSize.Width) / 2 + screen.WorkingArea.Left;
                int y = (screen.WorkingArea.Height - _normalSize.Height) / 2 + screen.WorkingArea.Top;
                this.Location = new Point(x, y);
                _normalLocation = this.Location;
            };
        }

        private void FormLogin_Shown(object sender, EventArgs e)
        {
            try { txtUsername?.Focus(); } catch { }
            // Không hiển thị form cấu hình nữa, sử dụng connection string từ App.config
        }

        private void InitializeTimers()
        {
            _fadeInTimer = new Timer { Interval = 20 };
            _fadeInTimer.Tick += (s, e) =>
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.05f;
                }
                else
                {
                    _fadeInTimer.Stop();
                }
            };
            _fadeInTimer.Start();

            _capsLockTimer = new Timer { Interval = 200 };
            _capsLockTimer.Tick += (s, e) =>
            {
                bool isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
                lblCapsLock.Visible = isCapsLock && txtPassword.Focused && !string.IsNullOrEmpty(txtPassword.Text);
            };
            _capsLockTimer.Start();

            _inputValidationTimer = new Timer { Interval = 500 };
            _inputValidationTimer.Tick += (s, e) =>
            {
                _inputValidationTimer.Stop();
                if (!_isValidating)
                {
                    ValidateInputs();
                }
            };

            _animationTimer = new Timer { Interval = 40 };
            _animationTimer.Tick += (s, e) =>
            {
                if (_glowIncreasing)
                {
                    _glowOpacity += 0.025f;
                    if (_glowOpacity >= 0.7f) _glowIncreasing = false;
                }
                else
                {
                    _glowOpacity -= 0.025f;
                    if (_glowOpacity <= 0.2f) _glowIncreasing = true;
                }
                if (pnlLeft != null) pnlLeft.Invalidate();

                if (_pulseIncreasing)
                {
                    _pulseScale += 0.012f;
                    if (_pulseScale >= 1.05f) _pulseIncreasing = false;
                }
                else
                {
                    _pulseScale -= 0.012f;
                    if (_pulseScale <= 1.0f) _pulseIncreasing = true;
                }

                if (btnLogin != null)
                {
                    int alpha = (int)(100 * _pulseScale);
                    btnLogin.ShadowDecoration.Color = Color.FromArgb(alpha, PrimaryAccentColor);
                }
            };
            _animationTimer.Start();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // Keyboard: Enter submits
            this.AcceptButton = btnLogin;
            // Tab order
            try { if (txtUsername != null) txtUsername.TabIndex = 0; } catch {}
            try { if (txtPassword != null) txtPassword.TabIndex = 1; } catch {}
            try { if (btnLogin != null) btnLogin.TabIndex = 2; } catch {}

            // Ensure maximize respects taskbar and current screen working area
            try { this.MaximizedBounds = Screen.FromControl(this).WorkingArea; } catch { }

            // Load per-user preferences (last username)
            try { _clientSettings = ClientSettings.Load(); if (!string.IsNullOrWhiteSpace(_clientSettings.LastUsername)) txtUsername.Text = _clientSettings.LastUsername; } catch { }

            // Apply layout immediately for current size (1024x640 default)
            ApplySplitLayout();
            CenterLoginPanel();
            CenterLeftBrand();
            CenterTitleInTitleBar();
            // Update visual state
            UpdateLoginButtonState();
        }

        private void FormLogin_Resize(object sender, EventArgs e)
        {
            if (guna2Elipse1 != null)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    guna2Elipse1.BorderRadius = 0;
                }
                else
                {
                    guna2Elipse1.BorderRadius = _originalBorderRadius;
                }
            }
            ApplySplitLayout();
            CenterTitleInTitleBar();
        }

        private float GetLeftRatio()
        {
            return this.WindowState == FormWindowState.Maximized ? LeftRatioFullscreen : LeftRatioDefault;
        }

        private void InitializeSplitDocking()
        {
            try
            {
                if (pnlLeft != null) pnlLeft.Dock = DockStyle.Left;
                if (pnlRight != null) pnlRight.Dock = DockStyle.Fill;
            }
            catch { }
        }

        private void ApplySplitLayout()
        {
            if (pnlLeft == null || pnlRight == null) return;

            var client = this.ClientSize;
            if (client.Width <= 0) return;

            float ratio = GetLeftRatio();
            int desiredLeft = Math.Max(LeftMinWidth, (int)Math.Round(client.Width * ratio));
            // Keep right panel minimally usable
            int minRight = RightMinWidth;
            if (client.Width - desiredLeft < minRight)
            {
                desiredLeft = Math.Max(LeftMinWidth, client.Width - minRight);
            }
            desiredLeft = Math.Min(desiredLeft, client.Width - minRight);
            if (desiredLeft < LeftMinWidth) desiredLeft = LeftMinWidth;

            pnlLeft.Width = desiredLeft;

            // Re-center content in each panel after resize
            CenterLeftBrand();
            CenterLoginPanel();
        }

        private void CenterLoginPanel()
        {
            if (pnlRight == null || pnlLoginContainer == null) return;

            int x = (pnlRight.Width - pnlLoginContainer.Width) / 2;
            int y = (pnlRight.Height - pnlLoginContainer.Height) / 2;

            pnlLoginContainer.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        private void CenterLeftBrand()
        {
            try
            {
                if (pnlLeft == null) return;
                if (picLogo != null)
                {
                    picLogo.Left = (pnlLeft.Width - picLogo.Width) / 2;
                }
                if (lblBrand != null)
                {
                    lblBrand.Left = (pnlLeft.Width - lblBrand.Width) / 2;
                }
            }
            catch { }
        }

        private void CenterTitleInTitleBar()
        {
            try
            {
                if (pnlTitleBar == null || lblTitle == null) return;
                // Only center if the label actually lives in the title bar
                if (!ReferenceEquals(lblTitle.Parent, pnlTitleBar)) return;
                lblTitle.Left = (pnlTitleBar.Width - lblTitle.Width) / 2;
            }
            catch { }
        }

        private void SetupEvents()
        {
            // Textboxes behavior
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { txtPassword.Focus(); e.Handled = true; } };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { btnLogin.PerformClick(); e.Handled = true; } };

            txtUsername.TextChanged += (s, e) => { lblError.Visible = false; ClearFieldError(txtUsername, lblUsernameError); _inputValidationTimer.Stop(); _inputValidationTimer.Start(); UpdateLoginButtonState(); };
            txtPassword.TextChanged += (s, e) => { lblError.Visible = false; ClearFieldError(txtPassword, lblPasswordError); _inputValidationTimer.Stop(); _inputValidationTimer.Start(); lblCapsLock.Visible = Control.IsKeyLocked(Keys.CapsLock) && txtPassword.Focused && !string.IsNullOrEmpty(txtPassword.Text); UpdateLoginButtonState(); };

            txtUsername.Enter += (s, e) => { txtUsername.BorderColor = PrimaryAccentColor; txtUsername.BorderThickness = 2; ClearFieldError(txtUsername, lblUsernameError); };
            txtUsername.Leave += (s, e) => { if (!string.IsNullOrWhiteSpace(txtUsername.Text)) { ValidateInputs(); UpdateLoginButtonState(); } };

            txtPassword.Enter += (s, e) => { txtPassword.BorderColor = PrimaryAccentColor; txtPassword.BorderThickness = 2; ClearFieldError(txtPassword, lblPasswordError); lblCapsLock.Visible = Control.IsKeyLocked(Keys.CapsLock) && !string.IsNullOrEmpty(txtPassword.Text); };
            txtPassword.Leave += (s, e) => { if (!string.IsNullOrWhiteSpace(txtPassword.Text)) { ValidateInputs(); UpdateLoginButtonState(); } lblCapsLock.Visible = false; };

            // Form-level keys
            this.KeyDown += FormLogin_KeyDown;
            // Toggle show/hide password with Ctrl+H
            this.KeyDown += (s, e) => { if (e.Control && e.KeyCode == Keys.H) { TogglePasswordVisibility(); e.Handled = true; } };

            // Title bar buttons (wired in Designer to avoid double subscription)
            if (pnlTitleBar != null) pnlTitleBar.DoubleClick += (s, e) => ToggleFullscreen();
        }

        private void ValidateInputs()
        {
            _isValidating = true;

            string username = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                _usernameValid = false;
                ShowFieldError("Vui lòng nhập tên đăng nhập", txtUsername, lblUsernameError);
            }
            else if (username.Length < 3)
            {
                _usernameValid = false;
                ShowFieldWarning("Tên đăng nhập phải có ít nhất 3 ký tự", txtUsername, lblUsernameError);
            }
            else if (!username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '@'))
            {
                _usernameValid = false;
                ShowFieldError("Tên đăng nhập không hợp lệ", txtUsername, lblUsernameError);
            }
            else
            {
                _usernameValid = true;
                ClearFieldError(txtUsername, lblUsernameError);
                txtUsername.BorderColor = SuccessColor;
            }

            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                _passwordValid = false;
                ShowFieldError("Vui lòng nhập mật khẩu", txtPassword, lblPasswordError);
            }
            else if (password.Length < 4)
            {
                _passwordValid = false;
                ShowFieldWarning("Mật khẩu phải có ít nhất 4 ký tự", txtPassword, lblPasswordError);
            }
            else
            {
                _passwordValid = true;
                ClearFieldError(txtPassword, lblPasswordError);
                txtPassword.BorderColor = SuccessColor;
            }

            _isValidating = false;
        }

        private void ShowFieldError(string message, Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            errorLabel.Text = message;
            errorLabel.ForeColor = ErrorColor;
            errorLabel.Visible = true;
            input.BorderColor = ErrorColor;
            input.FocusedState.BorderColor = ErrorColor;
        }

        private void ShowFieldWarning(string message, Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            errorLabel.Text = message;
            errorLabel.ForeColor = WarningColor;
            errorLabel.Visible = true;
            input.BorderColor = WarningColor;
            input.FocusedState.BorderColor = WarningColor;
        }

        private void ClearFieldError(Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            errorLabel.Text = "";
            errorLabel.Visible = false;
            input.BorderColor = input.Focused ? PrimaryAccentColor : InputBorderColor;
        }

        private void ApplyStyles()
        {
            pnlTitleBar.FillColor = TitleBarColor;
            pnlTitleBar.BorderColor = TitleBarBorderColor;
            pnlTitleBar.BorderThickness = 1;

            btnSettings.ForeColor = TitleBarTextColor;
            btnMinimize.ForeColor = TitleBarTextColor;
            btnMaximize.ForeColor = TitleBarTextColor;
            btnClose.ForeColor = TitleBarTextColor;

            pnlRight.FillColor = RightPanelColor;

            btnLogin.FillColor = PrimaryAccentColor;
            btnLogin.FillColor2 = PrimaryAccentHoverColor;
            btnLogin.BorderColor = ButtonBorderColor;

            txtUsername.BorderColor = InputBorderColor;
            txtPassword.BorderColor = InputBorderColor;

            lblTitle.ForeColor = PrimaryAccentColor;
            lblError.ForeColor = ErrorColor;
            lblCapsLock.ForeColor = WarningColor;
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush brush = new LinearGradientBrush(pnlLeft.ClientRectangle, PrimaryGradientTop, PrimaryGradientBottom, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, pnlLeft.ClientRectangle);
            }

            if (picLogo != null && picLogo.Visible && picLogo.Image != null)
            {
                int logoCenterX = pnlLeft.Width / 2;
                int logoCenterY = picLogo.Top + picLogo.Height / 2;
                int glowRadius = (int)(Math.Min(pnlLeft.Width, pnlLeft.Height) / 2.5f);
                var glowRect = new Rectangle(logoCenterX - glowRadius, logoCenterY - glowRadius, glowRadius * 2, glowRadius * 2);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(glowRect);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        int alpha = (int)(_glowOpacity * 200);
                        brush.CenterColor = Color.FromArgb(alpha, 255, 255, 255);
                        brush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
                        g.FillEllipse(brush, glowRect);
                    }
                }
            }
        }

        private void LoadAssets()
        {
            try
            {
                string resourcesPath = Path.Combine(Application.StartupPath, @"..\\..\\GUI\\Resources");
                string logoPath = Path.Combine(resourcesPath, _logoFileName);
                if (File.Exists(logoPath)) picLogo.Image = Image.FromFile(logoPath);
                lblBrand.Text = _brandName;
                pnlLeft.Paint += PnlLeft_Paint;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading assets: " + ex.Message);
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            ValidateInputs();

            if (!_usernameValid || !_passwordValid)
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    ShowError("Vui lòng nhập tên đăng nhập.");
                }
                else if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowError("Vui lòng nhập mật khẩu.");
                }
                else
                {
                    ShowError("Vui lòng kiểm tra lại thông tin đăng nhập.");
                }
                AnimateErrorShake();
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "ĐANG ĐĂNG NHẬP...";

            try
            {
                await Task.Delay(300);
                var user = await Task.Run(() => _authBLL.Login(txtUsername.Text.Trim(), txtPassword.Text));

                if (user != null)
                {
                    // Save last used username per user/machine
                    try
                    {
                        _clientSettings = _clientSettings ?? ClientSettings.Load();
                        _clientSettings.LastUsername = txtUsername.Text.Trim();
                        ClientSettings.Save(_clientSettings);
                    }
                    catch { }

                    LoggedInUser = user;
                    btnLogin.FillColor = SuccessColor;
                    btnLogin.Text = "✓ THÀNH CÔNG";
                    await Task.Delay(500);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng.");
                    AnimateErrorShake();
                }
            }
            catch (Exception ex)
            {
                // Nếu lỗi kết nối DB → mời mở cấu hình
                if (IsDbConnectivityException(ex))
                {
                    OfferOpenConfig(ex);
                }
                else
                {
                    ShowError("Lỗi: " + ex.Message);
                    AnimateErrorShake();
                }
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Đăng Nhập";
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }

        private void TogglePasswordVisibility()
        {
            try
            {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            }
            catch { }
        }

        private void AnimateErrorShake()
        {
            Point originalPos = pnlLoginContainer.Location;
            Timer shakeTimer = new Timer { Interval = 25 };
            int shakeCount = 0;
            shakeTimer.Tick += (s, e) =>
            {
                if (shakeCount < 6)
                {
                    pnlLoginContainer.Left += (shakeCount % 2 == 0) ? 5 : -5;
                    shakeCount++;
                }
                else
                {
                    pnlLoginContainer.Location = originalPos;
                    shakeTimer.Stop();
                    shakeTimer.Dispose();
                }
            };
            shakeTimer.Start();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                // Mở form cấu hình kết nối database
                var configForm = new FormConfig();
                configForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Lỗi mở form cấu hình: {ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void BtnMinimize_Click(object sender, EventArgs e) { this.WindowState = FormWindowState.Minimized; }
        private void BtnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void ToggleFullscreen()
        {
            var wa = Screen.FromControl(this).WorkingArea;
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = _normalSize;
                this.Location = new Point(
                    (wa.Width - _normalSize.Width) / 2 + wa.Left,
                    (wa.Height - _normalSize.Height) / 2 + wa.Top);
                btnMaximize.Text = "□";
                if (guna2Elipse1 != null) guna2Elipse1.BorderRadius = _originalBorderRadius;
            }
            else
            {
                this.MaximumSize = wa.Size;
                this.WindowState = FormWindowState.Maximized;
                btnMaximize.Text = "❐";
                if (guna2Elipse1 != null) guna2Elipse1.BorderRadius = 0;
            }
        }

        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            ToggleFullscreen();
        }

        private void UpdateLoginButtonState()
        {
            try
            {
                bool canLogin = _usernameValid && _passwordValid;
                // Always allow click; just change visual state
                btnLogin.Enabled = true;
                btnLogin.FillColor = canLogin ? PrimaryAccentColor : Color.Gainsboro;
                btnLogin.FillColor2 = canLogin ? PrimaryAccentHoverColor : Color.Gainsboro;
                btnLogin.ForeColor = canLogin ? Color.White : Color.DimGray;
            }
            catch { }
        }

        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                    e.Handled = true;
                    return;
                }

                if (e.Control && e.KeyCode == Keys.D1)
                {
                    txtUsername.Text = "quanli";
                    txtPassword.Text = "123456";
                    e.Handled = true;
                    return;
                }
                if (e.Control && e.KeyCode == Keys.D2)
                {
                    txtUsername.Text = "staff";
                    txtPassword.Text = "123456";
                    e.Handled = true;
                    return;
                }

                if (e.KeyCode == Keys.F11)
                {
                    ToggleFullscreen();
                    e.Handled = true;
                    return;
                }
            }
            catch { }
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _fadeInTimer?.Stop();
                _capsLockTimer?.Stop();
                _inputValidationTimer?.Stop();
                _animationTimer?.Stop();
                _fadeInTimer?.Dispose();
                _capsLockTimer?.Dispose();
                _inputValidationTimer?.Dispose();
                _animationTimer?.Dispose();
            }
            catch { }
        }

        private static bool IsDbConnectivityException(Exception ex)
        {
            // Walk inner exceptions and check common DB connectivity errors
            Exception cur = ex;
            while (cur != null)
            {
                if (cur is SqlException) return true;
                
                // Check for InvalidOperationException with database connection message
                if (cur is InvalidOperationException)
                {
                    var msg = cur.Message ?? string.Empty;
                    if (msg.IndexOf("kết nối đến cơ sở dữ liệu", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("cannot connect", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("database", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
                
                var msg2 = cur.Message ?? string.Empty;
                if (msg2.IndexOf("network-related", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg2.IndexOf("server was not found", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg2.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg2.IndexOf("login failed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg2.IndexOf("transport-level", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    msg2.IndexOf("kết nối đến cơ sở dữ liệu", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
                cur = cur.InnerException;
            }
            return false;
        }

        private void OfferOpenConfig(Exception ex)
        {
            try
            {
                var result = MessageBox.Show(
                    "❌ Không thể kết nối đến cơ sở dữ liệu.\n\n" +
                    "Chi tiết lỗi:\n" + (ex?.Message ?? "Lỗi không xác định") + "\n\n" +
                    "Bạn có muốn mở form cấu hình kết nối không?",
                    "Lỗi kết nối CSDL",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        var configForm = new FormConfig();
                        configForm.ShowDialog(this);
                    }
                    catch (Exception configEx)
                    {
                        MessageBox.Show(
                            $"Lỗi mở form cấu hình: {configEx.Message}",
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                
                ShowError("Lỗi kết nối CSDL. Vui lòng cấu hình lại kết nối.");
                AnimateErrorShake();
            }
            catch { }
        }
    }
}
