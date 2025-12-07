using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Modern UI Helper - Enhanced styling and components for modern UI/UX
    /// </summary>
    public static class ModernUIHelper
    {
        // Extended Color Palette
        public static class ModernColors
        {
            // Primary Colors (Blue)
            public static readonly Color Primary50 = Color.FromArgb(239, 246, 255);
            public static readonly Color Primary100 = Color.FromArgb(219, 234, 254);
            public static readonly Color Primary200 = Color.FromArgb(191, 219, 254);
            public static readonly Color Primary300 = Color.FromArgb(147, 197, 253);
            public static readonly Color Primary400 = Color.FromArgb(96, 165, 250);
            public static readonly Color Primary500 = Color.FromArgb(59, 130, 246);
            public static readonly Color Primary600 = Color.FromArgb(37, 99, 235);
            public static readonly Color Primary700 = Color.FromArgb(29, 78, 216);
            public static readonly Color Primary900 = Color.FromArgb(15, 23, 42);

            // Success Colors (Green)
            public static readonly Color Success50 = Color.FromArgb(240, 253, 244);
            public static readonly Color Success500 = Color.FromArgb(34, 197, 94);
            public static readonly Color Success600 = Color.FromArgb(22, 163, 74);

            // Warning Colors (Orange)
            public static readonly Color Warning50 = Color.FromArgb(255, 247, 237);
            public static readonly Color Warning500 = Color.FromArgb(251, 146, 60);
            public static readonly Color Warning600 = Color.FromArgb(234, 88, 12);

            // Error Colors (Red)
            public static readonly Color Error50 = Color.FromArgb(254, 242, 242);
            public static readonly Color Error500 = Color.FromArgb(239, 68, 68);
            public static readonly Color Error600 = Color.FromArgb(220, 38, 38);

            // Neutral Colors (Gray)
            public static readonly Color Gray50 = Color.FromArgb(249, 250, 251);
            public static readonly Color Gray100 = Color.FromArgb(243, 244, 246);
            public static readonly Color Gray200 = Color.FromArgb(229, 231, 235);
            public static readonly Color Gray300 = Color.FromArgb(209, 213, 219);
            public static readonly Color Gray400 = Color.FromArgb(156, 163, 175);
            public static readonly Color Gray500 = Color.FromArgb(107, 114, 128);
            public static readonly Color Gray600 = Color.FromArgb(75, 85, 99);
            public static readonly Color Gray700 = Color.FromArgb(55, 65, 81);
            public static readonly Color Gray900 = Color.FromArgb(17, 24, 39);

            // Semantic Colors
            public static readonly Color Background = Gray50;
            public static readonly Color Surface = Color.White;
            public static readonly Color SurfaceVariant = Gray100;
            public static readonly Color Border = Gray200;
            public static readonly Color TextPrimary = Gray900;
            public static readonly Color TextSecondary = Gray600;
            public static readonly Color TextTertiary = Gray500;
            public static readonly Color Disabled = Gray300;
        }

        // Spacing System
        public static class Spacing
        {
            public const int XS = 4;
            public const int SM = 8;
            public const int MD = 12;
            public const int LG = 16;
            public const int XL = 24;
            public const int XXL = 32;
        }

        // Border Radius
        public static class BorderRadius
        {
            public const int SM = 4;
            public const int MD = 6;
            public const int LG = 8;
            public const int XL = 12;
        }

        // Font Sizes
        public static class FontSizes
        {
            public const int XS = 11;
            public const int SM = 12;
            public const int MD = 13;
            public const int LG = 14;
            public const int XL = 16;
            public const int XXL = 20;
            public const int XXXL = 24;
        }

        /// <summary>
        /// Applies modern form styling with better spacing and colors
        /// </summary>
        public static void ApplyModernFormStyle(Form form)
        {
            if (form == null) return;

            try
            {
                form.BackColor = ModernColors.Background;
                form.Font = new Font("Segoe UI", FontSizes.MD, FontStyle.Regular);
                form.Padding = new Padding(Spacing.LG);
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.DoubleBuffered = true;
            }
            catch { }
        }

        /// <summary>
        /// Applies modern button styling with rounded corners and smooth transitions
        /// </summary>
        public static void ApplyModernButtonStyle(Guna2Button btn, ButtonType type = ButtonType.Primary)
        {
            if (btn == null) return;

            try
            {
                btn.Font = new Font("Segoe UI", FontSizes.MD, FontStyle.Medium);
                btn.BorderRadius = BorderRadius.MD;
                btn.Cursor = Cursors.Hand;
                btn.FocusedState.BorderColor = ModernColors.Primary500;
                btn.FocusedState.FillColor = ModernColors.Primary600;

                switch (type)
                {
                    case ButtonType.Primary:
                        btn.FillColor = ModernColors.Primary500;
                        btn.ForeColor = Color.White;
                        btn.HoverState.FillColor = ModernColors.Primary600;
                        btn.HoverState.ForeColor = Color.White;
                        break;

                    case ButtonType.Secondary:
                        btn.FillColor = ModernColors.Gray200;
                        btn.ForeColor = ModernColors.TextPrimary;
                        btn.HoverState.FillColor = ModernColors.Gray300;
                        btn.HoverState.ForeColor = ModernColors.TextPrimary;
                        break;

                    case ButtonType.Success:
                        btn.FillColor = ModernColors.Success500;
                        btn.ForeColor = Color.White;
                        btn.HoverState.FillColor = ModernColors.Success600;
                        break;

                    case ButtonType.Danger:
                        btn.FillColor = ModernColors.Error500;
                        btn.ForeColor = Color.White;
                        btn.HoverState.FillColor = ModernColors.Error600;
                        break;

                    case ButtonType.Warning:
                        btn.FillColor = ModernColors.Warning500;
                        btn.ForeColor = Color.White;
                        btn.HoverState.FillColor = ModernColors.Warning600;
                        break;

                    case ButtonType.Ghost:
                        btn.FillColor = Color.Transparent;
                        btn.ForeColor = ModernColors.Primary500;
                        btn.HoverState.FillColor = ModernColors.Primary50;
                        btn.HoverState.ForeColor = ModernColors.Primary600;
                        btn.BorderColor = ModernColors.Primary500;
                        btn.BorderThickness = 1;
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// Applies modern DataGridView styling with better readability
        /// </summary>
        public static void ApplyModernGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.BackgroundColor = ModernColors.Surface;
                dgv.GridColor = ModernColors.Border;
                dgv.DefaultCellStyle.BackColor = ModernColors.Surface;
                dgv.DefaultCellStyle.ForeColor = ModernColors.TextPrimary;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", FontSizes.SM);
                dgv.DefaultCellStyle.Padding = new Padding(Spacing.MD);
                dgv.DefaultCellStyle.SelectionBackColor = ModernColors.Primary100;
                dgv.DefaultCellStyle.SelectionForeColor = ModernColors.TextPrimary;

                dgv.ColumnHeadersDefaultCellStyle.BackColor = ModernColors.Gray50;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = ModernColors.TextPrimary;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", FontSizes.SM, FontStyle.SemiBold);
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(Spacing.MD);
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = ModernColors.Gray100;

                dgv.ColumnHeadersHeight = 44;
                dgv.RowTemplate.Height = 40;
                dgv.BorderStyle = BorderStyle.FixedSingle;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
                dgv.DoubleBuffered = true;

                // Alternating row colors for better readability
                dgv.AlternatingRowsDefaultCellStyle.BackColor = ModernColors.Gray50;
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = ModernColors.TextPrimary;
            }
            catch { }
        }

        /// <summary>
        /// Applies modern TextBox styling
        /// </summary>
        public static void ApplyModernTextBoxStyle(Guna2TextBox txt)
        {
            if (txt == null) return;

            try
            {
                txt.Font = new Font("Segoe UI", FontSizes.MD);
                txt.FillColor = ModernColors.Surface;
                txt.ForeColor = ModernColors.TextPrimary;
                txt.BorderColor = ModernColors.Border;
                txt.BorderRadius = BorderRadius.MD;
                txt.BorderThickness = 1;
                txt.FocusedState.BorderColor = ModernColors.Primary500;
                txt.FocusedState.FillColor = ModernColors.Surface;
                txt.PlaceholderText = txt.PlaceholderText ?? "";
                txt.PlaceholderForeColor = ModernColors.TextTertiary;
            }
            catch { }
        }

        /// <summary>
        /// Applies modern ComboBox styling
        /// </summary>
        public static void ApplyModernComboBoxStyle(Guna2ComboBox cmb)
        {
            if (cmb == null) return;

            try
            {
                cmb.Font = new Font("Segoe UI", FontSizes.MD);
                cmb.FillColor = ModernColors.Surface;
                cmb.ForeColor = ModernColors.TextPrimary;
                cmb.BorderColor = ModernColors.Border;
                cmb.BorderRadius = BorderRadius.MD;
                cmb.BorderThickness = 1;
                cmb.FocusedState.BorderColor = ModernColors.Primary500;
                cmb.FocusedState.FillColor = ModernColors.Surface;
            }
            catch { }
        }

        /// <summary>
        /// Applies modern Label styling
        /// </summary>
        public static void ApplyModernLabelStyle(Label lbl, LabelType type = LabelType.Body)
        {
            if (lbl == null) return;

            try
            {
                lbl.BackColor = Color.Transparent;

                switch (type)
                {
                    case LabelType.Heading1:
                        lbl.Font = new Font("Segoe UI", FontSizes.XXXL, FontStyle.Bold);
                        lbl.ForeColor = ModernColors.TextPrimary;
                        break;

                    case LabelType.Heading2:
                        lbl.Font = new Font("Segoe UI", FontSizes.XXL, FontStyle.Bold);
                        lbl.ForeColor = ModernColors.TextPrimary;
                        break;

                    case LabelType.Heading3:
                        lbl.Font = new Font("Segoe UI", FontSizes.XL, FontStyle.Bold);
                        lbl.ForeColor = ModernColors.TextPrimary;
                        break;

                    case LabelType.Subheading:
                        lbl.Font = new Font("Segoe UI", FontSizes.LG, FontStyle.SemiBold);
                        lbl.ForeColor = ModernColors.TextSecondary;
                        break;

                    case LabelType.Body:
                        lbl.Font = new Font("Segoe UI", FontSizes.MD);
                        lbl.ForeColor = ModernColors.TextPrimary;
                        break;

                    case LabelType.BodySmall:
                        lbl.Font = new Font("Segoe UI", FontSizes.SM);
                        lbl.ForeColor = ModernColors.TextSecondary;
                        break;

                    case LabelType.Caption:
                        lbl.Font = new Font("Segoe UI", FontSizes.XS);
                        lbl.ForeColor = ModernColors.TextTertiary;
                        break;

                    case LabelType.Error:
                        lbl.Font = new Font("Segoe UI", FontSizes.SM);
                        lbl.ForeColor = ModernColors.Error500;
                        break;

                    case LabelType.Success:
                        lbl.Font = new Font("Segoe UI", FontSizes.SM);
                        lbl.ForeColor = ModernColors.Success500;
                        break;
                }
            }
            catch { }
        }

        /// <summary>
        /// Creates a modern panel with shadow effect
        /// </summary>
        public static Panel CreateModernPanel(Color? backgroundColor = null)
        {
            var panel = new Panel
            {
                BackColor = backgroundColor ?? ModernColors.Surface,
                Margin = new Padding(Spacing.MD),
                Padding = new Padding(Spacing.LG)
            };

            return panel;
        }

        /// <summary>
        /// Applies modern search box styling
        /// </summary>
        public static void ApplyModernSearchBoxStyle(Guna2TextBox searchBox)
        {
            if (searchBox == null) return;

            try
            {
                searchBox.Font = new Font("Segoe UI", FontSizes.MD);
                searchBox.FillColor = ModernColors.Surface;
                searchBox.ForeColor = ModernColors.TextPrimary;
                searchBox.BorderColor = ModernColors.Border;
                searchBox.BorderRadius = BorderRadius.LG;
                searchBox.BorderThickness = 1;
                searchBox.FocusedState.BorderColor = ModernColors.Primary500;
                searchBox.FocusedState.FillColor = ModernColors.Surface;
                searchBox.PlaceholderText = "🔍 Tìm kiếm...";
                searchBox.PlaceholderForeColor = ModernColors.TextTertiary;
                searchBox.Padding = new Padding(Spacing.MD);
            }
            catch { }
        }

        /// <summary>
        /// Creates a modern status badge
        /// </summary>
        public static Label CreateStatusBadge(string text, StatusType status)
        {
            var badge = new Label
            {
                Text = text,
                AutoSize = true,
                Padding = new Padding(Spacing.SM, Spacing.XS, Spacing.SM, Spacing.XS),
                Font = new Font("Segoe UI", FontSizes.XS, FontStyle.SemiBold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            switch (status)
            {
                case StatusType.Success:
                    badge.BackColor = ModernColors.Success50;
                    badge.ForeColor = ModernColors.Success600;
                    break;
                case StatusType.Warning:
                    badge.BackColor = ModernColors.Warning50;
                    badge.ForeColor = ModernColors.Warning600;
                    break;
                case StatusType.Error:
                    badge.BackColor = ModernColors.Error50;
                    badge.ForeColor = ModernColors.Error600;
                    break;
                case StatusType.Info:
                    badge.BackColor = ModernColors.Primary50;
                    badge.ForeColor = ModernColors.Primary600;
                    break;
                default:
                    badge.BackColor = ModernColors.Gray100;
                    badge.ForeColor = ModernColors.Gray600;
                    break;
            }

            return badge;
        }

        /// <summary>
        /// Applies smooth fade-in animation to a control
        /// </summary>
        public static void FadeIn(Control control, int duration = 300)
        {
            if (control == null) return;

            try
            {
                control.Opacity = 0;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    control.Opacity = Math.Min(1.0, (double)elapsed / duration);

                    if (elapsed >= duration)
                    {
                        timer.Stop();
                        timer.Dispose();
                    }
                };

                timer.Start();
            }
            catch { }
        }

        /// <summary>
        /// Applies smooth fade-out animation to a control
        /// </summary>
        public static void FadeOut(Control control, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    control.Opacity = Math.Max(0.0, 1.0 - (double)elapsed / duration);

                    if (elapsed >= duration)
                    {
                        timer.Stop();
                        timer.Dispose();
                        onComplete?.Invoke();
                    }
                };

                timer.Start();
            }
            catch { }
        }

        /// <summary>
        /// Shows a modern toast notification
        /// </summary>
        public static void ShowToast(Form parent, string message, ToastType type = ToastType.Info, int duration = 3000)
        {
            try
            {
                var toastForm = new Form
                {
                    Text = "",
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = GetToastBackColor(type),
                    Width = 400,
                    Height = 60,
                    TopMost = true,
                    ShowInTaskbar = false,
                    StartPosition = FormStartPosition.Manual,
                    Opacity = 0.95
                };

                var label = new Label
                {
                    Text = message,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", FontSizes.MD),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };

                toastForm.Controls.Add(label);

                // Position at bottom right
                if (parent != null)
                {
                    toastForm.Location = new Point(
                        parent.Right - toastForm.Width - Spacing.LG,
                        parent.Bottom - toastForm.Height - Spacing.LG);
                }

                toastForm.Show();

                var timer = new Timer { Interval = duration };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    toastForm.Close();
                    toastForm.Dispose();
                };
                timer.Start();
            }
            catch { }
        }

        private static Color GetToastBackColor(ToastType type)
        {
            return type switch
            {
                ToastType.Success => ModernColors.Success500,
                ToastType.Error => ModernColors.Error500,
                ToastType.Warning => ModernColors.Warning500,
                _ => ModernColors.Primary500
            };
        }
    }

    /// <summary>
    /// Button type enumeration
    /// </summary>
    public enum ButtonType
    {
        Primary,
        Secondary,
        Success,
        Danger,
        Warning,
        Ghost
    }

    /// <summary>
    /// Label type enumeration
    /// </summary>
    public enum LabelType
    {
        Heading1,
        Heading2,
        Heading3,
        Subheading,
        Body,
        BodySmall,
        Caption,
        Error,
        Success
    }

    /// <summary>
    /// Status type enumeration
    /// </summary>
    public enum StatusType
    {
        Default,
        Success,
        Warning,
        Error,
        Info
    }

    /// <summary>
    /// Toast type enumeration
    /// </summary>
    public enum ToastType
    {
        Info,
        Success,
        Warning,
        Error
    }
}

