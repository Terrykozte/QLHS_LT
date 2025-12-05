using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections.Generic;
using Guna.UI2.WinForms;
using System.Linq;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Quản lý theme và styling cho toàn bộ ứng dụng
    /// </summary>
    public static class ThemeHelper
    {
        public enum Theme
        {
            Light,
            Dark
        }

        private static Theme _currentTheme = Theme.Light;

        // Color Palette
        public static class Colors
        {
            // Primary Colors
            public static readonly Color Primary = Color.FromArgb(46, 134, 171);      // #2E86AB
            public static readonly Color PrimaryLight = Color.FromArgb(100, 180, 220);
            public static readonly Color PrimaryDark = Color.FromArgb(20, 100, 150);

            // Secondary Colors
            public static readonly Color Secondary = Color.FromArgb(162, 59, 114);    // #A23B72
            public static readonly Color SecondaryLight = Color.FromArgb(200, 120, 160);
            public static readonly Color SecondaryDark = Color.FromArgb(120, 30, 80);

            // Status Colors
            public static readonly Color Success = Color.FromArgb(6, 168, 125);       // #06A77D
            public static readonly Color Warning = Color.FromArgb(241, 143, 1);       // #F18F01
            public static readonly Color Danger = Color.FromArgb(193, 18, 31);        // #C1121F
            public static readonly Color Info = Color.FromArgb(66, 135, 245);         // #4287F5

            // Neutral Colors
            public static readonly Color Background = Color.FromArgb(245, 245, 245);  // #F5F5F5
            public static readonly Color Surface = Color.White;
            public static readonly Color Border = Color.FromArgb(220, 220, 220);
            public static readonly Color TextPrimary = Color.FromArgb(51, 51, 51);    // #333333
            public static readonly Color TextSecondary = Color.FromArgb(102, 102, 102);
            public static readonly Color TextTertiary = Color.FromArgb(153, 153, 153);

            // Dark Theme
            public static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
            public static readonly Color DarkSurface = Color.FromArgb(45, 45, 45);
            public static readonly Color DarkBorder = Color.FromArgb(70, 70, 70);
            public static readonly Color DarkTextPrimary = Color.FromArgb(240, 240, 240);
            public static readonly Color DarkTextSecondary = Color.FromArgb(180, 180, 180);

            // App Navigation (Sidebar)
            public static readonly Color SidebarDark = Color.FromArgb(17, 24, 39);    // Slate-900
            public static readonly Color SidebarDarkHover = Color.FromArgb(31, 41, 55);// Slate-800
            public static readonly Color SidebarSeparator = Color.FromArgb(55, 65, 81);// Slate-700
            public static readonly Color SidebarText = Color.FromArgb(156, 163, 175);  // Slate-400
            public static readonly Color SidebarActiveText = Color.FromArgb(59, 130, 246); // Blue-500
        }

        /// <summary>
        /// Lấy theme hiện tại
        /// </summary>
        public static Theme GetCurrentTheme()
        {
            return _currentTheme;
        }

        /// <summary>
        /// Đặt theme
        /// </summary>
        public static void SetTheme(Theme theme)
        {
            _currentTheme = theme;
        }

        /// <summary>
        /// Chuyển đổi theme
        /// </summary>
        public static void ToggleTheme()
        {
            _currentTheme = _currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
        }

        /// <summary>
        /// Lấy màu nền theo theme
        /// </summary>
        public static Color GetBackgroundColor()
        {
            return _currentTheme == Theme.Light ? Colors.Background : Colors.DarkBackground;
        }

        /// <summary>
        /// Lấy màu bề mặt theo theme
        /// </summary>
        public static Color GetSurfaceColor()
        {
            return _currentTheme == Theme.Light ? Colors.Surface : Colors.DarkSurface;
        }

        /// <summary>
        /// Lấy màu text chính theo theme
        /// </summary>
        public static Color GetTextColor()
        {
            return _currentTheme == Theme.Light ? Colors.TextPrimary : Colors.DarkTextPrimary;
        }

        /// <summary>
        /// Lấy màu text phụ theo theme
        /// </summary>
        public static Color GetTextSecondaryColor()
        {
            return _currentTheme == Theme.Light ? Colors.TextSecondary : Colors.DarkTextSecondary;
        }

        /// <summary>
        /// Lấy màu border theo theme
        /// </summary>
        public static Color GetBorderColor()
        {
            return _currentTheme == Theme.Light ? Colors.Border : Colors.DarkBorder;
        }

        /// <summary>
        /// Áp dụng theme cho Form
        /// </summary>
        public static void ApplyThemeToForm(Form form)
        {
            if (form == null) return;

            form.BackColor = GetBackgroundColor();
            form.ForeColor = GetTextColor();

            ApplyThemeToControls(form);
        }

        /// <summary>
        /// Áp dụng theme cho tất cả controls
        /// </summary>
        public static void ApplyThemeToControls(Control parent)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                ApplyThemeToControl(control);

                if (control.HasChildren)
                {
                    ApplyThemeToControls(control);
                }
            }
        }

        /// <summary>
        /// Áp dụng theme cho một control
        /// </summary>
        public static void ApplyThemeToControl(Control control)
        {
            if (control == null) return;

            // Panel
            if (control is Panel panel)
            {
                panel.BackColor = GetSurfaceColor();
                panel.ForeColor = GetTextColor();
            }
            // Label
            else if (control is Label label)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = GetTextColor();
            }
            // TextBox
            else if (control is TextBox textBox)
            {
                textBox.BackColor = GetSurfaceColor();
                textBox.ForeColor = GetTextColor();
                textBox.BorderStyle = BorderStyle.FixedSingle;
            }
            // Button
            else if (control is Button button)
            {
                button.BackColor = Colors.Primary;
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
            }
            // DataGridView
            else if (control is DataGridView dgv)
            {
                dgv.BackgroundColor = GetBackgroundColor();
                dgv.GridColor = GetBorderColor();
                dgv.DefaultCellStyle.BackColor = GetSurfaceColor();
                dgv.DefaultCellStyle.ForeColor = GetTextColor();
                dgv.DefaultCellStyle.SelectionBackColor = Colors.Primary;
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Colors.Primary;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            }
            // ComboBox
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = GetSurfaceColor();
                comboBox.ForeColor = GetTextColor();
            }
            // CheckBox
            else if (control is CheckBox checkBox)
            {
                checkBox.BackColor = Color.Transparent;
                checkBox.ForeColor = GetTextColor();
            }
            // RadioButton
            else if (control is RadioButton radioButton)
            {
                radioButton.BackColor = Color.Transparent;
                radioButton.ForeColor = GetTextColor();
            }
            // GroupBox
            else if (control is GroupBox groupBox)
            {
                groupBox.BackColor = GetSurfaceColor();
                groupBox.ForeColor = GetTextColor();
            }
            // TabControl
            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = GetBackgroundColor();
                tabControl.ForeColor = GetTextColor();
            }
        }

        /// <summary>
        /// Tạo font cho heading
        /// </summary>
        public static Font GetHeadingFont(float size = 18)
        {
            return new Font("Segoe UI", size, FontStyle.Bold);
        }

        /// <summary>
        /// Tạo font cho body
        /// </summary>
        public static Font GetBodyFont(float size = 11)
        {
            return new Font("Segoe UI", size, FontStyle.Regular);
        }

        /// <summary>
        /// Tạo font cho caption
        /// </summary>
        public static Font GetCaptionFont(float size = 9)
        {
            return new Font("Segoe UI", size, FontStyle.Regular);
        }

        /// <summary>
        /// Tạo brush cho gradient
        /// </summary>
        public static LinearGradientBrush CreateGradientBrush(Rectangle rect, Color color1, Color color2)
        {
            return new LinearGradientBrush(rect, color1, color2, 45f);
        }

        /// <summary>
        /// Tạo shadow effect
        /// </summary>
        public static void ApplyShadowEffect(Control control)
        {
            // Thêm padding để tạo không gian cho shadow
            control.Margin = new Padding(5);
        }

        /// <summary>
        /// Tạo border radius effect
        /// </summary>
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.X + bounds.Width - diameter, bounds.Y + bounds.Height - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Y + bounds.Height - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// Áp dụng border radius cho control
        /// </summary>
        public static void ApplyBorderRadius(Control control, int radius)
        {
            var path = CreateRoundedRectangle(new Rectangle(0, 0, control.Width, control.Height), radius);
            control.Region = new Region(path);
        }

        /// <summary>
        /// Lấy màu status
        /// </summary>
        public static Color GetStatusColor(string status)
        {
            return status?.ToLower() switch
            {
                "success" or "completed" or "active" => Colors.Success,
                "warning" or "pending" or "processing" => Colors.Warning,
                "danger" or "error" or "cancelled" or "inactive" => Colors.Danger,
                "info" or "reserved" => Colors.Info,
                _ => Colors.TextSecondary
            };
        }

        /// <summary>
        /// Tạo style cho button status
        /// </summary>
        public static void StyleStatusButton(Button button, string status)
        {
            button.BackColor = GetStatusColor(status);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }

        /// <summary>
        /// Tạo style cho label status
        /// </summary>
        public static void StyleStatusLabel(Label label, string status)
        {
            label.ForeColor = GetStatusColor(status);
            label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Bold);
        }

        /// <summary>
        /// Áp dụng style cho DataGridView
        /// </summary>
        public static void ApplyDataGridViewStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.BackgroundColor = GetBackgroundColor();
            dgv.GridColor = GetBorderColor();
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Style header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Colors.Primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Style cells
            dgv.DefaultCellStyle.BackColor = GetSurfaceColor();
            dgv.DefaultCellStyle.ForeColor = GetTextColor();
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Padding = new Padding(5);

            // Style selection
            dgv.DefaultCellStyle.SelectionBackColor = Colors.PrimaryLight;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Áp dụng style cho input form
        /// </summary>
        public static void ApplyInputFormStyle(Control parent)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                if (control is TextBox || control is ComboBox)
                {
                    control.BackColor = GetSurfaceColor();
                    control.ForeColor = GetTextColor();
                    control.Font = new Font("Segoe UI", 10);
                }
                else if (control is Label label && label.Tag?.ToString() == "FormLabel")
                {
                    label.ForeColor = GetTextColor();
                    label.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else if (control is Button button && button.Tag?.ToString() == "FormButton")
                {
                    button.BackColor = Colors.Primary;
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                if (control.HasChildren)
                {
                    ApplyInputFormStyle(control);
                }
            }
        }

        /// <summary>
        /// Tạo notification style
        /// </summary>
        public static void StyleNotificationLabel(Label label, string type)
        {
            label.AutoSize = false;
            label.Padding = new Padding(10);
            label.Font = new Font("Segoe UI", 10);

            switch (type?.ToLower())
            {
                case "success":
                    label.BackColor = Color.FromArgb(220, 250, 240);
                    label.ForeColor = Colors.Success;
                    break;
                case "warning":
                    label.BackColor = Color.FromArgb(255, 250, 230);
                    label.ForeColor = Colors.Warning;
                    break;
                case "error":
                    label.BackColor = Color.FromArgb(255, 240, 240);
                    label.ForeColor = Colors.Danger;
                    break;
                case "info":
                    label.BackColor = Color.FromArgb(230, 245, 255);
                    label.ForeColor = Colors.Info;
                    break;
            }
        }

        // Sidebar helpers
        public static Color GetSidebarBackgroundColor() => Colors.SidebarDark; // always dark sidebar per design
        public static Color GetSidebarHoverColor() => Colors.SidebarDarkHover;
        public static Color GetSidebarTextColor() => Colors.SidebarText;
        public static Color GetSidebarActiveTextColor() => Colors.SidebarActiveText;
        public static Color GetSidebarSeparatorColor() => Colors.SidebarSeparator;

        public static void ApplySidebarTheme(
            Guna2Panel pnlSidebar,
            FlowLayoutPanel flowSidebar,
            Guna2Panel pnlUserInfo,
            IEnumerable<Guna2Button> buttons,
            Guna2HtmlLabel lblBrand = null,
            Guna2Separator separator = null)
        {
            try
            {
                var back = GetSidebarBackgroundColor();
                var hover = GetSidebarHoverColor();
                var text = GetSidebarTextColor();
                var activeText = GetSidebarActiveTextColor();
                var sep = GetSidebarSeparatorColor();

                if (pnlSidebar != null)
                {
                    pnlSidebar.BackColor = back;
                    pnlSidebar.FillColor = back;
                }
                if (flowSidebar != null)
                {
                    flowSidebar.BackColor = back;
                }
                if (pnlUserInfo != null)
                {
                    pnlUserInfo.BackColor = hover;
                    pnlUserInfo.FillColor = back;
                }
                if (lblBrand != null)
                {
                    lblBrand.BackColor = Color.Transparent;
                    lblBrand.ForeColor = Color.White;
                }
                if (separator != null)
                {
                    separator.FillColor = sep;
                }

                if (buttons != null)
                {
                    foreach (var btn in buttons.Where(b => b != null))
                    {
                        if (string.Equals(btn.Name, "btnLogout", StringComparison.OrdinalIgnoreCase))
                        {
                            // Keep special red style for Logout
                            btn.FillColor = Color.Transparent;
                            // don't change ForeColor (keeps red)
                            btn.HoverState.FillColor = Color.FromArgb(254, 242, 242); // light red background
                            btn.CheckedState.FillColor = Color.FromArgb(254, 242, 242);
                            btn.BorderRadius = 6;
                            btn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                            btn.ImageAlign = HorizontalAlignment.Left;
                            btn.TextAlign = HorizontalAlignment.Left;
                            btn.TextOffset = new Point(15, 0);
                            continue;
                        }

                        btn.FillColor = Color.Transparent;
                        btn.ForeColor = text;
                        btn.HoverState.FillColor = hover;
                        btn.HoverState.ForeColor = Color.White;
                        btn.CheckedState.FillColor = hover;
                        btn.CheckedState.ForeColor = activeText;
                        btn.BorderRadius = 6;
                        btn.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
                        btn.ImageAlign = HorizontalAlignment.Left;
                        btn.TextAlign = HorizontalAlignment.Left;
                        btn.TextOffset = new Point(15, 0);
                    }
                }
            }
            catch { }
        }
    }
}

