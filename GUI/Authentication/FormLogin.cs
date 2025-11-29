using System;
using System.Drawing;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Drawing.Drawing2D;

namespace QLTN_LT.GUI.Authentication
{
    public partial class FormLogin : Form
    {
        private readonly AuthBLL _authBLL;
        public UserDTO LoggedInUser { get; private set; }

        // Animation Timers
        private Timer _fadeInTimer;
        private Timer _capsLockTimer;
        private Timer _buttonHoverTimer;
        private Timer _inputValidationTimer;
        private Timer _glowEffectTimer;
        private Timer _pulseTimer;

        // Animation States
        private bool _isButtonHovering = false;
        private Size _originalButtonSize;
        private Font _originalButtonFont;
        private Point _originalButtonLocation;
        private float _glowOpacity = 0.3f;
        private bool _glowIncreasing = true;
        private float _pulseScale = 1.0f;
        private bool _pulseIncreasing = true;

        // Color Palette - Modern & Beautiful
        private static readonly Color PrimaryGradientTop = Color.FromArgb(30, 58, 138);      // Blue-800
        private static readonly Color PrimaryGradientBottom = Color.FromArgb(59, 130, 246);  // Blue-500
        private static readonly Color RightPanelColor = Color.FromArgb(255, 255, 255);       // Pure White
        // Taskbar - Màu xanh nhạt hài hòa với gradient, không trùng với nền trắng
        private static readonly Color TitleBarColor = Color.FromArgb(239, 246, 255);          // Blue-50 (xanh nhạt đẹp)
        private static readonly Color TitleBarBorderColor = Color.FromArgb(191, 219, 254);   // Blue-200 (viền xanh)
        private static readonly Color TitleBarTextColor = Color.FromArgb(30, 58, 138);        // Blue-800 (text đậm)
        private static readonly Color PrimaryAccentColor = Color.FromArgb(59, 130, 246);       // Blue-500
        private static readonly Color PrimaryAccentHoverColor = Color.FromArgb(37, 99, 235);  // Blue-600
        private static readonly Color ErrorColor = Color.FromArgb(239, 68, 68);               // Red-500
        private static readonly Color WarningColor = Color.FromArgb(251, 146, 60);             // Orange-400
        private static readonly Color SuccessColor = Color.FromArgb(34, 197, 94);              // Green-500
        private static readonly Color NeutralBorderColor = Color.FromArgb(17, 24, 39);        // Black/Dark Gray (rõ ràng)
        private static readonly Color InputBorderColor = Color.FromArgb(17, 24, 39);         // Black cho viền input
        private static readonly Color ButtonBorderColor = Color.FromArgb(17, 24, 39);         // Black cho viền button
        private static readonly Color TextPrimaryColor = Color.FromArgb(17, 24, 39);          // Gray-900 (đậm)
        private static readonly Color TextSecondaryColor = Color.FromArgb(75, 85, 99);        // Gray-600 (đậm hơn)
        private static readonly Color TitleBarButtonHover = Color.FromArgb(219, 234, 254);     // Blue-100 (hover đẹp)

        // Config
        private string _brandName = "Quản Lý Hải Sản";
        private string _logoFileName = "logo.png";

        // Validation States
        private bool _usernameValid = false;
        private bool _passwordValid = false;
        private bool _isValidating = false;
        private bool _isMaximized = false;
        private Size _normalSize = new Size(1024, 640);
        private Point _normalLocation;
        private int _originalBorderRadius = 15;

        public FormLogin()
        {
            InitializeComponent();
            _authBLL = new AuthBLL();
            
            InitializeTimers();
            SetupEvents();
            LoadAssets();
            ApplyStyles();
            
            this.Load += FormLogin_Load;
            this.Resize += FormLogin_Resize;
            
            // Fade in animation
            this.Opacity = 0;
            _fadeInTimer.Start();
            
            // Không cố định kích thước - cho phép resize
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = _normalSize;
            
            // Lưu border radius ban đầu
            if (guna2Elipse1 != null)
            {
                _originalBorderRadius = guna2Elipse1.BorderRadius;
            }
            
            // Center form trên màn hình khi khởi động
            this.Load += (s, e) =>
            {
                Screen screen = Screen.FromControl(this);
                int x = (screen.WorkingArea.Width - _normalSize.Width) / 2 + screen.WorkingArea.Left;
                int y = (screen.WorkingArea.Height - _normalSize.Height) / 2 + screen.WorkingArea.Top;
                this.Location = new Point(x, y);
                _normalLocation = this.Location;
            };
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

            _capsLockTimer = new Timer { Interval = 200 };
            _capsLockTimer.Tick += (s, e) =>
            {
                bool isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
                lblCapsLock.Visible = isCapsLock && txtPassword.Focused && !string.IsNullOrEmpty(txtPassword.Text);
            };
            _capsLockTimer.Start();

            _buttonHoverTimer = new Timer { Interval = 15 };
            _buttonHoverTimer.Tick += ButtonHoverTimer_Tick;

            _inputValidationTimer = new Timer { Interval = 500 }; // Debounce validation
            _inputValidationTimer.Tick += (s, e) =>
            {
                _inputValidationTimer.Stop();
                if (!_isValidating)
                {
                    ValidateInputs();
                }
            };

            // Glow effect timer cho logo
            _glowEffectTimer = new Timer { Interval = 50 };
            _glowEffectTimer.Tick += (s, e) =>
            {
                if (_glowIncreasing)
                {
                    _glowOpacity += 0.02f;
                    if (_glowOpacity >= 0.6f)
                        _glowIncreasing = false;
                }
                else
                {
                    _glowOpacity -= 0.02f;
                    if (_glowOpacity <= 0.3f)
                        _glowIncreasing = true;
                }
                if (pnlLeft != null)
                    pnlLeft.Invalidate();
            };
            _glowEffectTimer.Start();

            // Pulse effect timer cho login button
            _pulseTimer = new Timer { Interval = 30 };
            _pulseTimer.Tick += (s, e) =>
            {
                if (_pulseIncreasing)
                {
                    _pulseScale += 0.01f;
                    if (_pulseScale >= 1.05f)
                        _pulseIncreasing = false;
                }
                else
                {
                    _pulseScale -= 0.01f;
                    if (_pulseScale <= 1.0f)
                        _pulseIncreasing = true;
                }
                // Chỉ pulse khi button không hover - pulse effect với shadow
                if (!_isButtonHovering && btnLogin != null)
                {
                    // Pulse effect với shadow color opacity
                    int alpha = (int)(100 * _pulseScale);
                    btnLogin.ShadowDecoration.Color = Color.FromArgb(alpha, PrimaryAccentColor);
                }
            };
            _pulseTimer.Start();
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            _originalButtonSize = btnLogin.Size;
            _originalButtonFont = btnLogin.Font;
            _originalButtonLocation = btnLogin.Location;
            this.AcceptButton = btnLogin;
            CenterLoginControls();
            UpdateSettingsButtonPosition();
        }

        private void FormLogin_Resize(object sender, EventArgs e)
        {
            // Cập nhật border radius dựa trên trạng thái maximize
            if (guna2Elipse1 != null)
            {
                if (_isMaximized || this.WindowState == FormWindowState.Maximized || 
                    this.Width >= Screen.FromControl(this).WorkingArea.Width - 10)
                {
                    guna2Elipse1.BorderRadius = 0; // Không bo góc khi maximize hoặc gần fullscreen
                }
                else
                {
                    guna2Elipse1.BorderRadius = _originalBorderRadius; // Bo góc khi normal
                }
            }
            
            // Cập nhật layout khi resize - sử dụng BeginInvoke để tránh conflict
            this.BeginInvoke(new Action(() =>
            {
                CenterLoginControls();
                UpdateSettingsButtonPosition();
                
                // Đảm bảo các panel resize đúng
                if (pnlLeft != null && pnlRight != null)
                {
                    // Các panel đã dock nên sẽ tự động resize
                    // Chỉ cần center lại controls
                    CenterLoginControls();
                }
            }));
        }

        private void UpdateSettingsButtonPosition()
        {
            // Không cần nữa vì đã dùng Anchor trong Designer
        }

        private void CenterLoginControls()
        {
            if (pnlRight == null) return;

            // Tính toán vị trí center dựa trên kích thước thực tế của panel
            int centerX = pnlRight.Width / 2;
            
            // Tính toán startY dựa trên chiều cao của panel để center theo chiều dọc
            // Responsive: điều chỉnh spacing dựa trên kích thước form
            int spacing = _isMaximized ? 60 : 40; // Spacing lớn hơn khi fullscreen
            int fieldSpacing = _isMaximized ? 15 : 10;
            
            int totalHeight = lblTitle.Height + spacing + 
                            txtUsername.Height + 4 + lblUsernameError.Height + fieldSpacing +
                            txtPassword.Height + 4 + lblPasswordError.Height + 4 + lblCapsLock.Height + fieldSpacing +
                            btnLogin.Height + 20;
            
            int startY = Math.Max(80, (pnlRight.Height - totalHeight) / 2);

            // Title - Responsive font size
            if (_isMaximized)
            {
                lblTitle.Font = new Font("Segoe UI", 40F, FontStyle.Bold);
            }
            else
            {
                lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            }
            lblTitle.Location = new Point(centerX - (lblTitle.Width / 2), startY);
            startY += lblTitle.Height + spacing;

            // Username - Responsive width khi fullscreen
            int inputWidth = _isMaximized ? Math.Min(450, pnlRight.Width - 120) : 392;
            txtUsername.Width = inputWidth;
            txtUsername.Location = new Point(centerX - (txtUsername.Width / 2), startY);
            startY += txtUsername.Height + 4;
            lblUsernameError.Location = new Point(txtUsername.Left, startY);
            startY += lblUsernameError.Height + fieldSpacing;

            // Password - Responsive width khi fullscreen
            txtPassword.Width = inputWidth;
            txtPassword.Location = new Point(centerX - (txtPassword.Width / 2), startY);
            startY += txtPassword.Height + 4;
            lblPasswordError.Location = new Point(txtPassword.Left, startY);
            startY += lblPasswordError.Height + 4;
            lblCapsLock.Location = new Point(txtPassword.Left, startY);
            startY += lblCapsLock.Height + fieldSpacing;

            // Login Button - Responsive width khi fullscreen
            btnLogin.Width = inputWidth;
            btnLogin.Location = new Point(centerX - (btnLogin.Width / 2), startY);
            _originalButtonLocation = btnLogin.Location;
            startY += btnLogin.Height + 20;

            // Error Label
            lblError.Location = new Point(centerX - (lblError.Width / 2), startY);
        }

        private void SetupEvents()
        {
            // Enter key navigation
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPassword.Focus();
                    e.Handled = true;
                }
            };

            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnLogin.PerformClick();
                    e.Handled = true;
                }
            };

            // Real-time validation với debounce
            txtUsername.TextChanged += (s, e) =>
            {
                ClearFieldError(txtUsername, lblUsernameError);
                _inputValidationTimer.Stop();
                _inputValidationTimer.Start();
            };

            txtPassword.TextChanged += (s, e) =>
            {
                ClearFieldError(txtPassword, lblPasswordError);
                _inputValidationTimer.Stop();
                _inputValidationTimer.Start();
                lblCapsLock.Visible = Control.IsKeyLocked(Keys.CapsLock) && txtPassword.Focused && !string.IsNullOrEmpty(txtPassword.Text);
            };

            // Focus events - Cải thiện UX
            txtUsername.Enter += (s, e) =>
            {
                txtUsername.BorderColor = PrimaryAccentColor;
                txtUsername.BorderThickness = 2;
                ClearFieldError(txtUsername, lblUsernameError);
            };

            txtUsername.Leave += (s, e) =>
            {
                if (_usernameValid)
                {
                    txtUsername.BorderColor = SuccessColor;
                    txtUsername.BorderThickness = 2;
                }
                else
                {
                    txtUsername.BorderColor = InputBorderColor; // Đen nếu không hợp lệ
                    txtUsername.BorderThickness = 2;
                }
                // Validate khi leave
                if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    ValidateInputs();
                }
            };

            txtPassword.Enter += (s, e) =>
            {
                txtPassword.BorderColor = PrimaryAccentColor;
                txtPassword.BorderThickness = 2;
                ClearFieldError(txtPassword, lblPasswordError);
                lblCapsLock.Visible = Control.IsKeyLocked(Keys.CapsLock) && !string.IsNullOrEmpty(txtPassword.Text);
            };

            txtPassword.Leave += (s, e) =>
            {
                if (_passwordValid)
                {
                    txtPassword.BorderColor = SuccessColor;
                    txtPassword.BorderThickness = 2;
                }
                else
                {
                    txtPassword.BorderColor = InputBorderColor; // Đen nếu không hợp lệ
                    txtPassword.BorderThickness = 2;
                }
                lblCapsLock.Visible = false;
                // Validate khi leave
                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ValidateInputs();
                }
            };

            // Button hover
            btnLogin.MouseEnter += (s, e) =>
            {
                _isButtonHovering = true;
                _buttonHoverTimer.Start();
            };

            btnLogin.MouseLeave += (s, e) =>
            {
                _isButtonHovering = false;
                _buttonHoverTimer.Start();
            };

        }

        private void ButtonHoverTimer_Tick(object sender, EventArgs e)
        {
            const int maxGrow = 8;
            const int step = 2;

            if (_isButtonHovering)
            {
                if (btnLogin.Width < _originalButtonSize.Width + maxGrow)
                {
                    btnLogin.Width += step;
                    btnLogin.Height += step / 2;
                    btnLogin.Left -= step / 2;
                    btnLogin.Top -= step / 4;
                }
                else
                {
                    _buttonHoverTimer.Stop();
                }
            }
            else
            {
                if (btnLogin.Width > _originalButtonSize.Width)
                {
                    btnLogin.Width -= step;
                    btnLogin.Height -= step / 2;
                    btnLogin.Left += step / 2;
                    btnLogin.Top += step / 4;
                }
                else
                {
                    btnLogin.Size = _originalButtonSize;
                    btnLogin.Location = _originalButtonLocation;
                    _buttonHoverTimer.Stop();
                }
            }
        }

        private void ValidateInputs()
        {
            _isValidating = true;

            // Validate Username - Cải thiện logic
            string username = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                _usernameValid = false;
                if (!txtUsername.Focused || txtUsername.Text.Length == 0)
                {
                    ShowFieldError("Vui lòng nhập tên đăng nhập", txtUsername, lblUsernameError);
                }
                else
                {
                    txtUsername.BorderColor = InputBorderColor; // Reset về màu đen
                }
            }
            else if (username.Length < 3)
            {
                _usernameValid = false;
                ShowFieldWarning("Tên đăng nhập phải có ít nhất 3 ký tự", txtUsername, lblUsernameError);
            }
            else if (username.Length > 50)
            {
                _usernameValid = false;
                ShowFieldError("Tên đăng nhập không được quá 50 ký tự", txtUsername, lblUsernameError);
            }
            else if (!username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '.' || c == '@'))
            {
                _usernameValid = false;
                ShowFieldError("Tên đăng nhập chỉ được chứa chữ, số, dấu gạch dưới, dấu chấm và @", txtUsername, lblUsernameError);
            }
            else
            {
                _usernameValid = true;
                ClearFieldError(txtUsername, lblUsernameError);
                txtUsername.BorderColor = SuccessColor;
                txtUsername.BorderThickness = 2;
            }

            // Validate Password - Cải thiện logic
            string password = txtPassword.Text;
            if (string.IsNullOrWhiteSpace(password))
            {
                _passwordValid = false;
                if (!txtPassword.Focused || txtPassword.Text.Length == 0)
                {
                    ShowFieldError("Vui lòng nhập mật khẩu", txtPassword, lblPasswordError);
                }
                else
                {
                    txtPassword.BorderColor = InputBorderColor; // Reset về màu đen
                }
            }
            else if (password.Length < 4)
            {
                _passwordValid = false;
                ShowFieldWarning("Mật khẩu phải có ít nhất 4 ký tự", txtPassword, lblPasswordError);
            }
            else if (password.Length > 100)
            {
                _passwordValid = false;
                ShowFieldError("Mật khẩu không được quá 100 ký tự", txtPassword, lblPasswordError);
            }
            else
            {
                _passwordValid = true;
                ClearFieldError(txtPassword, lblPasswordError);
                txtPassword.BorderColor = SuccessColor;
                txtPassword.BorderThickness = 2;
            }

            _isValidating = false;
        }

        private void ShowFieldError(string message, Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            if (errorLabel != null)
            {
                errorLabel.Text = message;
                errorLabel.ForeColor = ErrorColor;
                errorLabel.Visible = true;
            }
            if (input != null)
            {
                input.BorderColor = ErrorColor;
                input.FocusedState.BorderColor = ErrorColor;
            }
            CenterLoginControls();
        }

        private void ShowFieldWarning(string message, Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            if (errorLabel != null)
            {
                errorLabel.Text = message;
                errorLabel.ForeColor = WarningColor;
                errorLabel.Visible = true;
            }
            if (input != null)
            {
                input.BorderColor = WarningColor;
                input.FocusedState.BorderColor = WarningColor;
            }
            CenterLoginControls();
        }

        private void ClearFieldError(Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            if (errorLabel != null)
            {
                errorLabel.Text = "";
                errorLabel.Visible = false;
            }
            if (input != null)
            {
                if (input.Focused)
                {
                    input.BorderColor = PrimaryAccentColor; // Blue khi focus
                    input.BorderThickness = 2;
                }
                else
                {
                    input.BorderColor = InputBorderColor; // Đen khi không focus
                    input.BorderThickness = 2;
                }
            }
        }

        private void ApplyStyles()
        {
            // Title Bar - Màu xanh nhạt đẹp, hài hòa với gradient
            pnlTitleBar.FillColor = TitleBarColor; // Blue-50
            pnlTitleBar.BackColor = TitleBarColor;
            pnlTitleBar.BorderColor = TitleBarBorderColor; // Blue-200
            pnlTitleBar.BorderThickness = 1;
            
            // Title Bar Buttons - Màu xanh đậm trên nền xanh nhạt
            btnSettings.FillColor = Color.Transparent;
            btnSettings.ForeColor = TitleBarTextColor; // Blue-800
            btnSettings.HoverState.FillColor = TitleBarButtonHover; // Blue-100
            btnSettings.HoverState.ForeColor = PrimaryAccentColor; // Blue-500
            
            btnMinimize.FillColor = Color.Transparent;
            btnMinimize.ForeColor = TitleBarTextColor;
            btnMinimize.HoverState.FillColor = TitleBarButtonHover;
            btnMinimize.HoverState.ForeColor = PrimaryAccentColor;
            
            btnMaximize.FillColor = Color.Transparent;
            btnMaximize.ForeColor = TitleBarTextColor;
            btnMaximize.HoverState.FillColor = TitleBarButtonHover;
            btnMaximize.HoverState.ForeColor = PrimaryAccentColor;
            
            btnClose.FillColor = Color.Transparent;
            btnClose.ForeColor = TitleBarTextColor;
            btnClose.HoverState.FillColor = Color.FromArgb(239, 68, 68); // Red-500
            btnClose.HoverState.ForeColor = Color.White;
            
            // Right Panel
            pnlRight.FillColor = RightPanelColor;
            pnlRight.BackColor = RightPanelColor;

            // Login Button - Gradient với viền đen và shadow effect
            btnLogin.FillColor = PrimaryAccentColor;
            btnLogin.FillColor2 = PrimaryAccentHoverColor;
            btnLogin.ForeColor = Color.White;
            btnLogin.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnLogin.BorderColor = ButtonBorderColor;
            btnLogin.BorderThickness = 2;
            btnLogin.ShadowDecoration.Enabled = true;
            btnLogin.ShadowDecoration.Color = Color.FromArgb(100, PrimaryAccentColor);
            btnLogin.ShadowDecoration.Shadow = new Padding(0, 0, 5, 5);

            // Inputs - Chữ đậm, viền đen rõ ràng với shadow
            txtUsername.FillColor = Color.White;
            txtUsername.ForeColor = TextPrimaryColor; // Đậm, không mờ
            txtUsername.BorderColor = InputBorderColor; // Đen, rõ ràng
            txtUsername.BorderThickness = 2;
            txtUsername.FocusedState.BorderColor = PrimaryAccentColor;
            txtUsername.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            txtUsername.ShadowDecoration.Enabled = true;
            txtUsername.ShadowDecoration.Color = Color.FromArgb(50, 0, 0, 0);
            txtUsername.ShadowDecoration.Shadow = new Padding(0, 0, 3, 3);

            txtPassword.FillColor = Color.White;
            txtPassword.ForeColor = TextPrimaryColor; // Đậm, không mờ
            txtPassword.BorderColor = InputBorderColor; // Đen, rõ ràng
            txtPassword.BorderThickness = 2;
            txtPassword.FocusedState.BorderColor = PrimaryAccentColor;
            txtPassword.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            txtPassword.ShadowDecoration.Enabled = true;
            txtPassword.ShadowDecoration.Color = Color.FromArgb(50, 0, 0, 0);
            txtPassword.ShadowDecoration.Shadow = new Padding(0, 0, 3, 3);

            // Labels
            lblTitle.ForeColor = PrimaryAccentColor;
            lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblUsernameError.ForeColor = ErrorColor;
            lblPasswordError.ForeColor = ErrorColor;
            lblError.ForeColor = ErrorColor;
            lblCapsLock.ForeColor = WarningColor;
            lblCapsLock.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Modern gradient background - từ Blue-800 đến Blue-500
            using (LinearGradientBrush brush = new LinearGradientBrush(
                pnlLeft.ClientRectangle,
                PrimaryGradientTop,      // Blue-800
                PrimaryGradientBottom,   // Blue-500
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, pnlLeft.ClientRectangle);
            }

            // Subtle highlight overlay để tạo chiều sâu
            Color highlightStart = Color.FromArgb(60, 255, 255, 255);
            Color highlightEnd = Color.FromArgb(0, 255, 255, 255);
            using (LinearGradientBrush highlightBrush = new LinearGradientBrush(
                new Rectangle(0, 0, pnlLeft.Width, pnlLeft.Height / 2),
                highlightStart,
                highlightEnd,
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(highlightBrush, new Rectangle(0, 0, pnlLeft.Width, pnlLeft.Height / 2));
            }

            // KHUNG TRANG TRÍ CHO LOGO - Nổi bật và đẹp
            if (picLogo != null && picLogo.Visible && picLogo.Image != null)
            {
                int logoCenterX = pnlLeft.Width / 2;
                int logoCenterY = picLogo.Top + picLogo.Height / 2;
                
                // Khung ngoài lớn - với gradient border
                int frameSize = Math.Max(picLogo.Width, picLogo.Height) + 80;
                int frameX = logoCenterX - frameSize / 2;
                int frameY = logoCenterY - frameSize / 2;
                Rectangle frameRect = new Rectangle(frameX, frameY, frameSize, frameSize);

                // Vẽ khung với nhiều lớp để tạo hiệu ứng nổi bật
                
                // Lớp 1: Outer glow
                using (GraphicsPath outerPath = new GraphicsPath())
                {
                    outerPath.AddEllipse(frameRect);
                    using (PathGradientBrush outerBrush = new PathGradientBrush(outerPath))
                    {
                        int outerAlpha = (int)(_glowOpacity * 150);
                        outerBrush.CenterColor = Color.FromArgb(outerAlpha, 255, 255, 255);
                        outerBrush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
                        g.FillEllipse(outerBrush, frameRect);
                    }
                }

                // Lớp 2: Frame border với gradient
                Rectangle borderRect = new Rectangle(frameX + 10, frameY + 10, frameSize - 20, frameSize - 20);
                using (Pen gradientPen = new Pen(new LinearGradientBrush(
                    borderRect,
                    Color.FromArgb(200, 255, 255, 255),
                    Color.FromArgb(100, 255, 255, 255),
                    LinearGradientMode.Vertical), 4))
                {
                    g.DrawEllipse(gradientPen, borderRect);
                }

                // Lớp 3: Inner frame với pattern
                Rectangle innerRect = new Rectangle(frameX + 25, frameY + 25, frameSize - 50, frameSize - 50);
                using (Pen innerPen = new Pen(Color.FromArgb(150, 255, 255, 255), 2))
                {
                    innerPen.DashStyle = DashStyle.Dash;
                    g.DrawEllipse(innerPen, innerRect);
                }

                // Lớp 4: Decorative dots xung quanh
                int dotCount = 12;
                for (int i = 0; i < dotCount; i++)
                {
                    double angle = (2 * Math.PI * i) / dotCount;
                    int dotRadius = frameSize / 2 - 15;
                    int dotX = logoCenterX + (int)(dotRadius * Math.Cos(angle));
                    int dotY = logoCenterY + (int)(dotRadius * Math.Sin(angle));
                    
                    using (SolidBrush dotBrush = new SolidBrush(Color.FromArgb(180, 255, 255, 255)))
                    {
                        g.FillEllipse(dotBrush, dotX - 4, dotY - 4, 8, 8);
                    }
                }

                // Animated Glow effect behind logo - với opacity động
                int glowRadius = Math.Min(pnlLeft.Width, pnlLeft.Height) / 3;
                var glowRect = new Rectangle(
                    logoCenterX - glowRadius / 2,
                    logoCenterY - glowRadius / 2,
                    glowRadius,
                    glowRadius);

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(glowRect);
                    using (PathGradientBrush brush = new PathGradientBrush(path))
                    {
                        int alpha = (int)(_glowOpacity * 255);
                        brush.CenterColor = Color.FromArgb(alpha, 255, 255, 255);
                        brush.SurroundColors = new[] { Color.FromArgb(0, 255, 255, 255) };
                        g.FillEllipse(brush, glowRect);
                    }
                }
            }

            // Decorative corner elements
            int cornerSize = 60;
            using (Pen cornerPen = new Pen(Color.FromArgb(100, 255, 255, 255), 2))
            {
                // Top-left corner
                g.DrawLine(cornerPen, 20, 20, 20 + cornerSize, 20);
                g.DrawLine(cornerPen, 20, 20, 20, 20 + cornerSize);
                
                // Top-right corner
                g.DrawLine(cornerPen, pnlLeft.Width - 20, 20, pnlLeft.Width - 20 - cornerSize, 20);
                g.DrawLine(cornerPen, pnlLeft.Width - 20, 20, pnlLeft.Width - 20, 20 + cornerSize);
                
                // Bottom-left corner
                g.DrawLine(cornerPen, 20, pnlLeft.Height - 20, 20 + cornerSize, pnlLeft.Height - 20);
                g.DrawLine(cornerPen, 20, pnlLeft.Height - 20, 20, pnlLeft.Height - 20 - cornerSize);
                
                // Bottom-right corner
                g.DrawLine(cornerPen, pnlLeft.Width - 20, pnlLeft.Height - 20, pnlLeft.Width - 20 - cornerSize, pnlLeft.Height - 20);
                g.DrawLine(cornerPen, pnlLeft.Width - 20, pnlLeft.Height - 20, pnlLeft.Width - 20, pnlLeft.Height - 20 - cornerSize);
            }
        }

        private void LoadAssets()
        {
            try
            {
                string resourcesPath = Path.Combine(Application.StartupPath, @"..\..\GUI\Resources");
                
                string logoPath = Path.Combine(resourcesPath, _logoFileName);
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
                
                lblBrand.Text = _brandName;

                // Setup paint event for left panel
                pnlLeft.Paint += PnlLeft_Paint;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading assets: " + ex.Message);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // Clear previous errors
            lblError.Text = "";
            lblError.Visible = false;

            // Validate before submit - Cải thiện validation
            ValidateInputs();

            // Kiểm tra từng field cụ thể
            if (string.IsNullOrWhiteSpace(txtUsername.Text.Trim()))
            {
                ShowFieldError("Vui lòng nhập tên đăng nhập", txtUsername, lblUsernameError);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowFieldError("Vui lòng nhập mật khẩu", txtPassword, lblPasswordError);
                txtPassword.Focus();
                return;
            }

            if (!_usernameValid || !_passwordValid)
            {
                ShowError("Vui lòng kiểm tra lại thông tin đăng nhập.");
                // Focus vào field đầu tiên có lỗi
                if (!_usernameValid)
                {
                    txtUsername.Focus();
                }
                else if (!_passwordValid)
                {
                    txtPassword.Focus();
                }
                return;
            }

            // Disable button and show loading
            btnLogin.Enabled = false;
            string originalText = btnLogin.Text;
            Color originalFillColor = btnLogin.FillColor;
            Color originalFillColor2 = btnLogin.FillColor2;
            
            btnLogin.Text = "ĐANG ĐĂNG NHẬP...";
            btnLogin.FillColor = Color.FromArgb(156, 163, 175); // Gray khi loading
            btnLogin.FillColor2 = Color.FromArgb(156, 163, 175);

            try
            {
                await Task.Delay(300); // Small delay for UX

                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                var user = _authBLL.Login(username, password);

                if (user != null)
                {
                    LoggedInUser = user;
                    // Success animation
                    btnLogin.FillColor = SuccessColor;
                    btnLogin.FillColor2 = Color.FromArgb(34, 197, 94);
                    btnLogin.Text = "✓ ĐĂNG NHẬP THÀNH CÔNG";
                    
                    // Reset border colors về success
                    txtUsername.BorderColor = SuccessColor;
                    txtPassword.BorderColor = SuccessColor;
                    
                    await Task.Delay(500);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    ShowError("Tên đăng nhập hoặc mật khẩu không đúng.");
                    // Reset border colors về error
                    txtUsername.BorderColor = ErrorColor;
                    txtPassword.BorderColor = ErrorColor;
                    AnimateErrorShake();
                    txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowError("Lỗi hệ thống: " + ex.Message);
                txtUsername.BorderColor = ErrorColor;
                txtPassword.BorderColor = ErrorColor;
                AnimateErrorShake();
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = originalText;
                btnLogin.FillColor = originalFillColor;
                btnLogin.FillColor2 = originalFillColor2;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
            CenterLoginControls();
        }

        private void AnimateErrorShake()
        {
            Point originalPos = pnlRight.Location;
            Random rnd = new Random();
            const int shakeAmplitude = 6;
            int shakeCount = 0;
            
            Timer shakeTimer = new Timer { Interval = 25 };
            shakeTimer.Tick += (s, e) =>
            {
                if (shakeCount < 6)
                {
                    pnlRight.Location = new Point(
                        originalPos.X + rnd.Next(-shakeAmplitude, shakeAmplitude + 1),
                        originalPos.Y
                    );
                    shakeCount++;
                }
                else
                {
                    pnlRight.Location = originalPos;
                    shakeTimer.Stop();
                    shakeTimer.Dispose();
                }
            };
            shakeTimer.Start();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var frm = new FormConfig())
            {
                frm.ShowDialog();
            }
        }

        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, EventArgs e)
        {
            // Toggle maximize/restore
            if (!_isMaximized)
            {
                // Lưu kích thước và vị trí hiện tại trước khi maximize
                _normalSize = this.Size;
                _normalLocation = this.Location;
                
                // Maximize - chiếm toàn màn hình (working area)
                Screen screen = Screen.FromControl(this);
                Rectangle workingArea = screen.WorkingArea;
                
                // Suspend layout để tránh flicker
                this.SuspendLayout();
                
                // Tắt border radius khi maximize (không bo góc)
                if (guna2Elipse1 != null)
                {
                    guna2Elipse1.BorderRadius = 0;
                }
                
                // Đặt form vào vị trí và kích thước của working area
                this.Location = workingArea.Location;
                this.Size = workingArea.Size;
                
                _isMaximized = true;
                btnMaximize.Text = "❐"; // Restore icon
                
                // Resume layout và cập nhật lại controls
                this.ResumeLayout();
                this.PerformLayout();
                
                // Cập nhật lại layout sau khi maximize (delay nhỏ để đảm bảo resize xong)
                this.BeginInvoke(new Action(() =>
                {
                    CenterLoginControls();
                    this.Invalidate();
                }));
            }
            else
            {
                // Restore về kích thước ban đầu
                this.SuspendLayout();
                
                // Khôi phục border radius khi restore
                if (guna2Elipse1 != null)
                {
                    guna2Elipse1.BorderRadius = _originalBorderRadius;
                }
                
                // Restore size trước
                this.Size = _normalSize;
                
                // Center form trên màn hình khi restore
                Screen screen = Screen.FromControl(this);
                int x = (screen.WorkingArea.Width - _normalSize.Width) / 2 + screen.WorkingArea.Left;
                int y = (screen.WorkingArea.Height - _normalSize.Height) / 2 + screen.WorkingArea.Top;
                this.Location = new Point(x, y);
                
                _isMaximized = false;
                btnMaximize.Text = "□"; // Maximize icon
                
                // Resume layout
                this.ResumeLayout();
                this.PerformLayout();
                
                // Cập nhật lại layout sau khi restore
                this.BeginInvoke(new Action(() =>
                {
                    CenterLoginControls();
                    this.Invalidate();
                }));
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
