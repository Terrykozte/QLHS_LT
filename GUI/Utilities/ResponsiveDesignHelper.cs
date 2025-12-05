using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ responsive design cho các độ phân giải khác nhau
    /// Breakpoints: Mobile (<768px), Tablet (768-1024px), Desktop (>1024px)
    /// </summary>
    public static class ResponsiveDesignHelper
    {
        public enum ScreenSize
        {
            Mobile,      // < 768px
            Tablet,      // 768px - 1024px
            Desktop      // > 1024px
        }

        private const int MOBILE_BREAKPOINT = 768;
        private const int TABLET_BREAKPOINT = 1024;

        /// <summary>
        /// Xác định kích thước màn hình hiện tại
        /// </summary>
        public static ScreenSize GetCurrentScreenSize(Control control)
        {
            int width = control.Width;
            if (width < MOBILE_BREAKPOINT) return ScreenSize.Mobile;
            if (width < TABLET_BREAKPOINT) return ScreenSize.Tablet;
            return ScreenSize.Desktop;
        }

        /// <summary>
        /// Tính toán kích thước font responsive
        /// </summary>
        public static float GetResponsiveFontSize(float baseSize, Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            return screenSize switch
            {
                ScreenSize.Mobile => baseSize * 0.85f,
                ScreenSize.Tablet => baseSize * 0.92f,
                ScreenSize.Desktop => baseSize,
                _ => baseSize
            };
        }

        /// <summary>
        /// Tính toán chiều rộng responsive (% của parent)
        /// </summary>
        public static int GetResponsiveWidth(Control parent, float percentage)
        {
            return (int)(parent.Width * percentage / 100);
        }

        /// <summary>
        /// Tính toán chiều cao responsive (% của parent)
        /// </summary>
        public static int GetResponsiveHeight(Control parent, float percentage)
        {
            return (int)(parent.Height * percentage / 100);
        }

        /// <summary>
        /// Tính toán padding responsive
        /// </summary>
        public static Padding GetResponsivePadding(Control control, int basePadding)
        {
            var screenSize = GetCurrentScreenSize(control);
            int adjustedPadding = screenSize switch
            {
                ScreenSize.Mobile => (int)(basePadding * 0.5f),
                ScreenSize.Tablet => (int)(basePadding * 0.75f),
                ScreenSize.Desktop => basePadding,
                _ => basePadding
            };
            return new Padding(adjustedPadding);
        }

        /// <summary>
        /// Tính toán margin responsive
        /// </summary>
        public static Padding GetResponsiveMargin(Control control, int baseMargin)
        {
            return GetResponsivePadding(control, baseMargin);
        }

        /// <summary>
        /// Điều chỉnh chiều cao hàng DataGridView
        /// </summary>
        public static int GetResponsiveRowHeight(Control control, int baseHeight)
        {
            var screenSize = GetCurrentScreenSize(control);
            return screenSize switch
            {
                ScreenSize.Mobile => (int)(baseHeight * 0.8f),
                ScreenSize.Tablet => (int)(baseHeight * 0.9f),
                ScreenSize.Desktop => baseHeight,
                _ => baseHeight
            };
        }

        /// <summary>
        /// Điều chỉnh số cột hiển thị trong DataGridView
        /// </summary>
        public static void AdjustDataGridViewColumns(DataGridView dgv, Control parent)
        {
            if (dgv == null || dgv.Columns.Count == 0) return;

            var screenSize = GetCurrentScreenSize(parent);
            
            // Ẩn/hiện các cột tùy theo kích thước màn hình
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                // Luôn hiển thị cột đầu tiên và cột cuối cùng
                if (col.Index == 0 || col.Index == dgv.Columns.Count - 1)
                {
                    col.Visible = true;
                    continue;
                }

                // Ẩn các cột "chi tiết" trên mobile
                if (screenSize == ScreenSize.Mobile)
                {
                    col.Visible = col.Name != "Description" && 
                                 col.Name != "Notes" && 
                                 col.Name != "Address" &&
                                 col.Name != "Details";
                }
                // Hiển thị tất cả trên desktop
                else if (screenSize == ScreenSize.Desktop)
                {
                    col.Visible = true;
                }
            }

            // Tự động điều chỉnh chiều rộng cột
            try
            {
                dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch { }
        }

        /// <summary>
        /// Điều chỉnh kích thước button responsive
        /// </summary>
        public static Size GetResponsiveButtonSize(Control control, int baseWidth, int baseHeight)
        {
            var screenSize = GetCurrentScreenSize(control);
            var (widthRatio, heightRatio) = screenSize switch
            {
                ScreenSize.Mobile => (0.8f, 0.8f),
                ScreenSize.Tablet => (0.9f, 0.9f),
                ScreenSize.Desktop => (1f, 1f),
                _ => (1f, 1f)
            };

            return new Size((int)(baseWidth * widthRatio), (int)(baseHeight * heightRatio));
        }

        /// <summary>
        /// Điều chỉnh kích thước input responsive
        /// </summary>
        public static Size GetResponsiveInputSize(Control control, int baseWidth, int baseHeight)
        {
            return GetResponsiveButtonSize(control, baseWidth, baseHeight);
        }

        /// <summary>
        /// Áp dụng responsive design cho Form
        /// </summary>
        public static void ApplyResponsiveDesignToForm(Form form)
        {
            if (form == null) return;

            // Đặt kích thước tối thiểu
            form.MinimumSize = new Size(800, 500);

            // Cho phép resize
            form.FormBorderStyle = FormBorderStyle.Sizable;

            // Thêm event resize
            form.Resize += (s, e) =>
            {
                ApplyResponsiveDesignToControls(form);
            };

            // Áp dụng lần đầu
            ApplyResponsiveDesignToControls(form);
        }

        /// <summary>
        /// Áp dụng responsive design cho tất cả controls trong form
        /// </summary>
        public static void ApplyResponsiveDesignToControls(Control parent)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                // Điều chỉnh font size
                if (control is Label lbl)
                {
                    lbl.Font = new Font(lbl.Font.FontFamily, 
                        GetResponsiveFontSize(lbl.Font.Size, parent), 
                        lbl.Font.Style);
                }
                else if (control is Button btn)
                {
                    btn.Font = new Font(btn.Font.FontFamily, 
                        GetResponsiveFontSize(btn.Font.Size, parent), 
                        btn.Font.Style);
                }
                else if (control is TextBox txt)
                {
                    txt.Font = new Font(txt.Font.FontFamily, 
                        GetResponsiveFontSize(txt.Font.Size, parent), 
                        txt.Font.Style);
                }
                else if (control is DataGridView dgv)
                {
                    AdjustDataGridViewColumns(dgv, parent);
                    dgv.RowTemplate.Height = GetResponsiveRowHeight(parent, 30);
                }

                // Đệ quy cho các control con
                if (control.HasChildren)
                {
                    ApplyResponsiveDesignToControls(control);
                }
            }
        }

        /// <summary>
        /// Tính toán vị trí responsive cho dialog
        /// </summary>
        public static Point GetResponsiveDialogPosition(Form parentForm, Form dialogForm)
        {
            if (parentForm == null || dialogForm == null)
                return new Point(0, 0);

            int x = parentForm.Left + (parentForm.Width - dialogForm.Width) / 2;
            int y = parentForm.Top + (parentForm.Height - dialogForm.Height) / 2;

            return new Point(Math.Max(0, x), Math.Max(0, y));
        }

        /// <summary>
        /// Kiểm tra xem có phải fullscreen mode không
        /// </summary>
        public static bool IsFullscreenMode(Form form)
        {
            return form.WindowState == FormWindowState.Maximized;
        }

        /// <summary>
        /// Bật/tắt fullscreen mode
        /// </summary>
        public static void ToggleFullscreen(Form form)
        {
            if (form.WindowState == FormWindowState.Maximized)
                form.WindowState = FormWindowState.Normal;
            else
                form.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Lấy kích thước màn hình khả dụng
        /// </summary>
        public static Size GetAvailableScreenSize()
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            return new Size(workingArea.Width, workingArea.Height);
        }

        /// <summary>
        /// Tính toán kích thước form tối ưu cho màn hình
        /// </summary>
        public static Size GetOptimalFormSize()
        {
            var screenSize = GetAvailableScreenSize();
            
            // Để lại 5% margin
            int width = (int)(screenSize.Width * 0.95);
            int height = (int)(screenSize.Height * 0.95);

            // Giới hạn tối thiểu
            width = Math.Max(width, 1024);
            height = Math.Max(height, 600);

            return new Size(width, height);
        }

        /// <summary>
        /// Áp dụng responsive design cho sidebar
        /// </summary>
        public static void ApplyResponsiveSidebarDesign(Panel sidebar, Form parentForm)
        {
            if (sidebar == null || parentForm == null) return;

            var screenSize = GetCurrentScreenSize(parentForm);
            
            // Trên mobile: ẩn sidebar hoặc làm hẹp
            if (screenSize == ScreenSize.Mobile)
            {
                sidebar.Width = 60; // Chỉ hiển thị icon
            }
            // Trên tablet: sidebar bình thường
            else if (screenSize == ScreenSize.Tablet)
            {
                sidebar.Width = 200;
            }
            // Trên desktop: sidebar đầy đủ
            else
            {
                sidebar.Width = 250;
            }
        }

        /// <summary>
        /// Tính toán kích thước card responsive
        /// </summary>
        public static Size GetResponsiveCardSize(Control parent, int cardsPerRow)
        {
            int availableWidth = parent.Width - 20; // Trừ padding
            int cardWidth = availableWidth / cardsPerRow;
            int cardHeight = (int)(cardWidth * 0.6); // Tỷ lệ 16:9

            return new Size(cardWidth, cardHeight);
        }

        /// <summary>
        /// Tính toán số card có thể hiển thị trên một hàng
        /// </summary>
        public static int GetResponsiveCardsPerRow(Control parent)
        {
            var screenSize = GetCurrentScreenSize(parent);
            return screenSize switch
            {
                ScreenSize.Mobile => 1,
                ScreenSize.Tablet => 2,
                ScreenSize.Desktop => 3,
                _ => 3
            };
        }
    }
}

