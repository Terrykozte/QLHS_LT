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

        private Timer _hoverTimer;
        private Timer _uiTimer; // For Caps Lock check
        private Timer _fadeInTimer; // For Fade In animation
        private bool _isHovering;
        private Size _originalButtonSize;
        private Font _originalButtonFont;
        private Point _originalContentLocation;
        
        // Settings Button Animation
        private Timer _settingsHoverTimer;
        private bool _isSettingsHovering;
        private Size _originalSettingsSize;
        private Font _originalSettingsFont;

        private Panel pnlContent; // Container for Logo and Brand

        // Color Palette
        private static readonly Color PrimaryGradientTop = Color.FromArgb(18, 24, 61);
        private static readonly Color PrimaryGradientBottom = Color.FromArgb(37, 99, 235);
        
        private static readonly Color DefaultBackgroundTopColor = Color.FromArgb(249, 250, 255);
        private static readonly Color DefaultBackgroundBottomColor = Color.FromArgb(232, 238, 255);
        
        private static readonly Color PrimaryAccentColor = Color.FromArgb(59, 130, 246);
        private static readonly Color PrimaryAccentHoverColor = Color.FromArgb(37, 99, 235);
        
        private static readonly Color InputBackColor = Color.FromArgb(255, 255, 255);
        private static readonly Color InputTextColor = Color.FromArgb(17, 24, 39);
        private static readonly Color NeutralBorderColor = Color.FromArgb(90, 90, 90);
        private static readonly Color FocusBorderColor = Color.FromArgb(59, 130, 246); // Primary Accent for focus

        private static readonly Color ErrorColor = Color.FromArgb(255, 107, 107);
        private static readonly Color WarningColor = Color.FromArgb(255, 167, 38);

        private static readonly Color TextColorTitle = Color.FromArgb(59, 130, 246); // Primary Accent

        // Configurable Left Panel Content
        private string _brandName = "Quản Lý Hải Sản"; // Updated to specific domain
        private string _logoFileName = "logo.png"; // Default logo file

        public FormLogin()
        {
            InitializeComponent();
            _authBLL = new AuthBLL();
            
            // Setup UI Events
            SetupEvents();
            
            // Load Assets
            LoadAssets();

            // Initialize Animation Timer
            _hoverTimer = new Timer();
            _hoverTimer.Interval = 15;
            _hoverTimer.Tick += HoverTimer_Tick;

            // Initialize UI Timer (Caps Lock)
            _uiTimer = new Timer();
            _uiTimer.Interval = 200; // Check every 200ms
            _uiTimer.Tick += UiTimer_Tick;
            _uiTimer.Start();

            // Setup Layout
            this.Resize += (s, e) => UpdateLayout();
            this.Load += (s, e) => {
                // Initialize Content Panel
                pnlContent = new Panel();
                pnlContent.BackColor = Color.Transparent;
                pnlContent.AutoSize = true;
                pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                pnlLeft.Controls.Add(pnlContent);
                
                // Re-parent controls
                picLogo.Parent = pnlContent;
                lblBrand.Parent = pnlContent;
                
                // Ensure Brand AutoSize for correct centering
                lblBrand.AutoSize = true;
                lblBrand.TextAlign = ContentAlignment.MiddleCenter;

                UpdateLayout();
                _originalButtonSize = btnLogin.Size;
                _originalButtonFont = btnLogin.Font;
                this.AcceptButton = btnLogin; // Enter key support
            };

            // Setup Paint for Gradient
            // Setup Paint for Gradient
            pnlLeft.Paint += PnlLeft_Paint;
            
            // Parallax Effect (Re-enabled for premium feel)
            pnlLeft.MouseMove += PnlLeft_MouseMove;
            this.MouseMove += PnlLeft_MouseMove;  

            // Apply Colors to Controls
            ApplyColors();

            // Setup Button Animation
            btnLogin.MouseEnter += (s, e) => { _isHovering = true; _hoverTimer.Start(); };
            btnLogin.MouseLeave += (s, e) => { _isHovering = false; _hoverTimer.Start(); };

            // Fade In Animation
            this.Opacity = 0;
            _fadeInTimer = new Timer();
            _fadeInTimer.Interval = 10;
            _fadeInTimer.Tick += FadeInTimer_Tick;
            _fadeInTimer.Start();

            // Initialize Settings Button Animation
            _settingsHoverTimer = new Timer();
            _settingsHoverTimer.Interval = 15;
            _settingsHoverTimer.Tick += SettingsHoverTimer_Tick;
            
            _originalSettingsSize = btnSettings.Size;
            _originalSettingsFont = btnSettings.Font;

            btnSettings.MouseEnter += (s, e) => { _isSettingsHovering = true; _settingsHoverTimer.Start(); };
            btnSettings.MouseLeave += (s, e) => { _isSettingsHovering = false; _settingsHoverTimer.Start(); };
        }

        private void SetupEvents()
        {
            // Enter key to login
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };

            // Real-time Validation (Clear errors on type)
            txtUsername.TextChanged += (s, e) => 
            {
                if (lblUsernameError.Visible)
                {
                    lblUsernameError.Visible = false;
                    lblUsernameError.Text = "";
                    txtUsername.BorderColor = NeutralBorderColor;
                }
                ClearGlobalError();
            };

            txtPassword.TextChanged += (s, e) => 
            {
                if (lblPasswordError.Visible)
                {
                    lblPasswordError.Visible = false;
                    lblPasswordError.Text = "";
                    txtPassword.BorderColor = NeutralBorderColor;
                }
                ClearGlobalError();
            };

            // Settings Button
            btnSettings.Click += (s, e) => 
            {
                using (var frm = new FormConfig())
                {
                    frm.ShowDialog();
                }
            };
        }

        private void ClearGlobalError()
        {
            if (lblError.Text != "") lblError.Text = "";
        }

        private void ApplyColors()
        {
            // Right Panel Background
            pnlRight.FillColor = DefaultBackgroundTopColor; // Guna2Panel supports FillColor
            pnlRight.BackColor = Color.Transparent; // Let FillColor show if using gradient, or just set BackColor

            // Login Button
            btnLogin.FillColor = PrimaryAccentColor;
            btnLogin.FillColor2 = PrimaryAccentHoverColor; // Gradient for button
            btnLogin.ForeColor = Color.White;

            // Inputs
            txtUsername.FillColor = InputBackColor;
            txtUsername.ForeColor = InputTextColor;
            txtUsername.BorderColor = NeutralBorderColor;
            txtUsername.FocusedState.BorderColor = FocusBorderColor;
            txtUsername.Font = new Font("Segoe UI", 11F); // Increased Font Size

            txtPassword.FillColor = InputBackColor;
            txtPassword.ForeColor = InputTextColor;
            txtPassword.BorderColor = NeutralBorderColor;
            txtPassword.FocusedState.BorderColor = FocusBorderColor;
            txtPassword.Font = new Font("Segoe UI", 11F); // Increased Font Size

            // Labels
            lblTitle.ForeColor = TextColorTitle;
            lblUsernameError.ForeColor = ErrorColor;
            lblPasswordError.ForeColor = ErrorColor;
            lblError.ForeColor = ErrorColor;
        }

        private void UpdateLayout()
        {
            if (this.WindowState == FormWindowState.Minimized) return;

            int halfWidth = this.ClientSize.Width / 2;
            int fullHeight = this.ClientSize.Height;

            // 1. Setup Panels (50/50 Split)
            pnlLeft.Size = new Size(halfWidth, fullHeight);
            pnlLeft.Location = new Point(0, 0);

            pnlRight.Size = new Size(this.ClientSize.Width - halfWidth, fullHeight); // Ensure full coverage
            pnlRight.Location = new Point(halfWidth, 0);

            // 2. Center Content in Left Panel
            // 2. Center Content in Left Panel (using pnlContent container)
            // 2. Center Content in Left Panel (using pnlContent container)
            if (pnlContent != null)
            {
                int logoHeight = 0;
                int logoWidth = 0;

                if (picLogo.Visible)
                {
                    if (picLogo.Image != null)
                    {
                        // Scale logo to 50% of panel width
                        int targetLogoWidth = (int)(pnlLeft.Width * 0.5);
                        float ratio = (float)picLogo.Image.Height / picLogo.Image.Width;
                        int targetLogoHeight = (int)(targetLogoWidth * ratio);
                        
                        picLogo.Size = new Size(targetLogoWidth, targetLogoHeight);
                    }
                    else
                    {
                        // Default placeholder size if no image
                        picLogo.Size = new Size(200, 200);
                    }
                    logoHeight = picLogo.Height;
                    logoWidth = picLogo.Width;
                }

                // Ensure Brand is AutoSize
                lblBrand.AutoSize = true;
                
                // Layout inside pnlContent
                // 1. Logo at top center
                // We need to determine the width of pnlContent. It should be at least as wide as the widest element.
                int contentWidth = Math.Max(logoWidth, lblBrand.Width);
                
                // Position Logo
                picLogo.Location = new Point((contentWidth - logoWidth) / 2, 0);
                
                // Position Brand
                // User requested 30px spacing to prevent overlap
                int brandSpacing = 30;
                lblBrand.Location = new Point((contentWidth - lblBrand.Width) / 2, logoHeight + brandSpacing);

                // Explicitly set pnlContent size to wrap both controls
                // Height = Logo Height + Spacing + Brand Height
                int contentHeight = logoHeight + brandSpacing + lblBrand.Height;
                pnlContent.Size = new Size(contentWidth, contentHeight);

                // Center pnlContent in pnlLeft
                int startX = (pnlLeft.Width - pnlContent.Width) / 2;
                int startY = (pnlLeft.Height - pnlContent.Height) / 2;
                
                _originalContentLocation = new Point(startX, startY);
                pnlContent.Location = _originalContentLocation;
            }

            // 3. Center Content in Right Panel
            // Calculate total height of login controls
            int spacing = 15; // Reduced spacing
            int smallSpacing = 5; // For error labels
            
            int loginContentHeight = lblTitle.Height + spacing + 
                                     txtUsername.Height + smallSpacing + lblUsernameError.Height + spacing + 
                                     txtPassword.Height + smallSpacing + lblPasswordError.Height + 
                                     (lblCapsLock.Visible ? lblCapsLock.Height + smallSpacing : 0) + 
                                     spacing + // Spacing before button
                                     btnLogin.Height + spacing + 
                                     lblError.Height;

            int currentY = (pnlRight.Height - loginContentHeight) / 2;

            // Center horizontally
            int centerX = pnlRight.Width / 2;

            lblTitle.Location = new Point(centerX - (lblTitle.Width / 2), currentY);
            currentY += lblTitle.Height + spacing;

            txtUsername.Width = (int)(pnlRight.Width * 0.65); // Slightly wider inputs
            txtUsername.Location = new Point(centerX - (txtUsername.Width / 2), currentY);
            currentY += txtUsername.Height + smallSpacing;

            lblUsernameError.Location = new Point(txtUsername.Left, currentY); // Align with input
            currentY += lblUsernameError.Height + spacing;

            txtPassword.Width = txtUsername.Width;
            txtPassword.Location = new Point(centerX - (txtPassword.Width / 2), currentY);
            currentY += txtPassword.Height + smallSpacing;

            lblPasswordError.Location = new Point(txtPassword.Left, currentY); // Align with input
            currentY += lblPasswordError.Height; // No extra spacing if no error, but layout reserves it or we adjust dynamically. 
            // For dynamic layout, we should re-calc if error is visible, but for now fixed slot is fine or we adjust Y based on visibility.
            // Let's stick to the flow:
            
            if (lblCapsLock.Visible)
            {
                currentY += smallSpacing;
                lblCapsLock.Location = new Point(centerX - (lblCapsLock.Width / 2), currentY);
                currentY += lblCapsLock.Height;
            }

            currentY += 10; // Reduced distance to button (was spacing=20)

            btnLogin.Width = txtUsername.Width;
            // Note: Button location is updated in animation, so we set base location here
            btnLogin.Location = new Point(centerX - (btnLogin.Width / 2), currentY);
            currentY += btnLogin.Height + spacing;

            lblError.Location = new Point(centerX - (lblError.Width / 2), currentY);

            // Update original size for animation to work correctly at new resolution
            _originalButtonSize = btnLogin.Size;
            _originalButtonFont = btnLogin.Font;
        }

        private void PnlLeft_MouseMove(object sender, MouseEventArgs e)
        {
            // Parallax Effect
            if (pnlContent == null) return;

            int centerX = pnlLeft.Width / 2;
            int centerY = pnlLeft.Height / 2;

            // Calculate offset (-10 to 10 pixels)
            int offsetX = (e.X - centerX) / 40; // Smoother factor
            int offsetY = (e.Y - centerY) / 40;

            // Move entire content panel
            pnlContent.Location = new Point(_originalContentLocation.X - offsetX, _originalContentLocation.Y - offsetY);
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            // Check Caps Lock
            bool isCapsLock = Control.IsKeyLocked(Keys.CapsLock);
            if (lblCapsLock.Visible != isCapsLock)
            {
                lblCapsLock.Visible = isCapsLock;
            }
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            // Create a premium linear gradient
            // Create a "Blue Sea" gradient
            // Create a premium "Blue Sea" linear gradient
            // 1. Base Gradient: PrimaryGradientTop -> PrimaryGradientBottom
            using (System.Drawing.Drawing2D.LinearGradientBrush baseBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                pnlLeft.ClientRectangle,
                PrimaryGradientTop,
                PrimaryGradientBottom,
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(baseBrush, pnlLeft.ClientRectangle);
            }

            // 2. Inner Highlight (Glass/Glow Effect)
            // User request: Color.FromArgb(80, Color.White) -> Color.FromArgb(15, Color.White)
            Color highlightStart = Color.FromArgb(80, 255, 255, 255);
            Color highlightEnd = Color.FromArgb(15, 255, 255, 255);

            // Draw a top-down gradient overlay
            using (System.Drawing.Drawing2D.LinearGradientBrush highlightBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                pnlLeft.ClientRectangle,
                highlightStart,
                highlightEnd,
                System.Drawing.Drawing2D.LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(highlightBrush, pnlLeft.ClientRectangle);
            }

            // 3. Optional: Add a subtle "Glow" circle behind the logo if desired, but sticking to user request first.
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            int zoomInStep = 2;
            int zoomOutStep = 2; // Equal speed for symmetric animation
            int maxGrow = 10;
            
            // Robust Hover Logic:
            // 1. If disabled (processing), treat as hovered (keep zoomed).
            // 2. If enabled, check real mouse position to fix "lost hover" after re-enabling.
            bool effectiveHover = _isHovering;
            
            if (!btnLogin.Enabled)
            {
                effectiveHover = true; // Keep zoomed while processing
            }
            else
            {
                // Re-validate hover state if enabled (fixes state loss after disable/enable cycle)
                Point mousePos = btnLogin.PointToClient(Cursor.Position);
                if (btnLogin.ClientRectangle.Contains(mousePos))
                {
                    effectiveHover = true;
                }
            }

            if (effectiveHover)
            {
                // Zoom In
                if (btnLogin.Width < _originalButtonSize.Width + maxGrow)
                {
                    btnLogin.Width += zoomInStep;
                    btnLogin.Height += zoomInStep / 4; // Keep aspect ratio roughly
                    btnLogin.Left -= zoomInStep / 2; // Center expansion
                    btnLogin.Top -= zoomInStep / 8;
                    
                    // Zoom Font
                    // Synchronized: 10px / 2px = 5 frames. 1.5pt / 0.3pt = 5 frames.
                    if (btnLogin.Font.Size < _originalButtonFont.Size + 1.5f)
                    {
                         btnLogin.Font = new Font(_originalButtonFont.FontFamily, btnLogin.Font.Size + 0.3f, _originalButtonFont.Style);
                    }
                }
                else
                {
                    // Stop timer only if we are not processing (to keep checking mouse pos)
                    if (btnLogin.Enabled) _hoverTimer.Stop();
                }
            }
            else
            {
                // Zoom Out (Symmetric)
                if (btnLogin.Width > _originalButtonSize.Width)
                {
                    btnLogin.Width -= zoomOutStep;
                    btnLogin.Height -= zoomOutStep / 4;
                    btnLogin.Left += zoomOutStep / 2;
                    btnLogin.Top += zoomOutStep / 8;

                    // Shrink Font
                    if (btnLogin.Font.Size > _originalButtonFont.Size)
                    {
                         btnLogin.Font = new Font(_originalButtonFont.FontFamily, btnLogin.Font.Size - 0.3f, _originalButtonFont.Style); // Symmetric font shrink
                    }
                }
                else
                {
                    // Reset to exact original to prevent drift
                    btnLogin.Size = _originalButtonSize;
                    btnLogin.Font = _originalButtonFont;
                    // Re-center based on current layout (optional, but safe)
                    UpdateLayout(); 
                    _hoverTimer.Stop();
                }
            }
        }

        private void ShowFieldError(string message, Guna.UI2.WinForms.Guna2TextBox input, Label errorLabel)
        {
            if (errorLabel != null)
            {
                errorLabel.Text = message;
                errorLabel.Visible = true;
            }
            if (input != null)
            {
                input.BorderColor = ErrorColor;
                input.FocusedState.BorderColor = ErrorColor; // Keep red even when focused if error exists
            }
        }

        private void LoadAssets()
        {
            try
            {
                string resourcesPath = Path.Combine(Application.StartupPath, @"..\..\GUI\Resources");
                
                // Load Logo
                string logoPath = Path.Combine(resourcesPath, _logoFileName);
                if (File.Exists(logoPath))
                {
                    picLogo.Image = Image.FromFile(logoPath);
                }
                
                // Set Brand Name
                lblBrand.Text = _brandName;

                // Load Background
                // Load Background - Commented out to use the "Blue Sea" gradient
                /*
                string bgPath = Path.Combine(resourcesPath, "login_bg.png");
                if (File.Exists(bgPath))
                {
                    pnlLeft.BackgroundImage = Image.FromFile(bgPath);
                    pnlLeft.BackgroundImageLayout = ImageLayout.Stretch;
                }
                */

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

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            btnLogin.Enabled = false;
            string originalText = btnLogin.Text;
            btnLogin.Text = "ĐANG ĐĂNG NHẬP...";

            try
            {
                // Simulate small delay for UX (so user sees the "Processing" state)
                await System.Threading.Tasks.Task.Delay(500);

                bool hasError = false;
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    ShowFieldError("Vui lòng nhập tên đăng nhập", txtUsername, lblUsernameError);
                    hasError = true;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowFieldError("Vui lòng nhập mật khẩu", txtPassword, lblPasswordError);
                    hasError = true;
                }

                if (hasError) return;

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
                btnLogin.Text = originalText;
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
        private void FadeInTimer_Tick(object sender, EventArgs e)
        {
            if (this.Opacity < 1)
            {
                this.Opacity += 0.05;
            }
            else
            {
                _fadeInTimer.Stop();
            }
        }

        private void SettingsHoverTimer_Tick(object sender, EventArgs e)
        {
            int zoomStep = 1;
            int maxGrow = 5;

            if (_isSettingsHovering)
            {
                if (btnSettings.Width < _originalSettingsSize.Width + maxGrow)
                {
                    btnSettings.Width += zoomStep;
                    btnSettings.Height += zoomStep;
                    btnSettings.Left -= zoomStep / 2;
                    btnSettings.Top -= zoomStep / 2;
                    
                    if (btnSettings.Font.Size < _originalSettingsFont.Size + 1.0f)
                    {
                         btnSettings.Font = new Font(_originalSettingsFont.FontFamily, btnSettings.Font.Size + 0.2f, _originalSettingsFont.Style);
                    }
                }
                else
                {
                    _settingsHoverTimer.Stop();
                }
            }
            else
            {
                if (btnSettings.Width > _originalSettingsSize.Width)
                {
                    btnSettings.Width -= zoomStep;
                    btnSettings.Height -= zoomStep;
                    btnSettings.Left += zoomStep / 2;
                    btnSettings.Top += zoomStep / 2;

                    if (btnSettings.Font.Size > _originalSettingsFont.Size)
                    {
                         btnSettings.Font = new Font(_originalSettingsFont.FontFamily, btnSettings.Font.Size - 0.2f, _originalSettingsFont.Style);
                    }
                }
                else
                {
                    btnSettings.Size = _originalSettingsSize;
                    btnSettings.Font = _originalSettingsFont;
                    _settingsHoverTimer.Stop();
                }
            }
        }
    }
}
