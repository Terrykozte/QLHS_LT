using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ responsive design cho các độ phân giải khác nhau
    /// </summary>
    public static class ResponsiveHelper
    {
        // Breakpoints
        public enum ScreenSize
        {
            Mobile,      // < 768px
            Tablet,      // 768px - 1023px
            Desktop1024, // 1024px - 1365px
            Desktop1366, // 1366px - 1919px
            FullHD,      // >= 1920px
        }

        /// <summary>
        /// Lấy kích thước màn hình hiện tại
        /// </summary>
        public static ScreenSize GetCurrentScreenSize(Control control)
        {
            int width = control?.Width ?? Screen.PrimaryScreen.Bounds.Width;
            
            if (width < 768) return ScreenSize.Mobile;
            if (width < 1024) return ScreenSize.Tablet;
            if (width < 1366) return ScreenSize.Desktop1024;
            if (width < 1920) return ScreenSize.Desktop1366;
            return ScreenSize.FullHD;
        }

        /// <summary>
        /// Điều chỉnh kích thước font theo độ phân giải
        /// </summary>
        public static float GetResponsiveFontSize(float baseSize, Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => baseSize * 0.8f,
                ScreenSize.Tablet => baseSize * 0.9f,
                ScreenSize.Desktop1024 => baseSize,
                ScreenSize.Desktop1366 => baseSize * 1.05f,
                ScreenSize.FullHD => baseSize * 1.1f,
                _ => baseSize
            };
        }

        /// <summary>
        /// Điều chỉnh padding theo độ phân giải
        /// </summary>
        public static Padding GetResponsivePadding(int basePadding, Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            int padding = screenSize switch
            {
                ScreenSize.Mobile => (int)(basePadding * 0.5f),
                ScreenSize.Tablet => (int)(basePadding * 0.75f),
                ScreenSize.Desktop1024 => basePadding,
                ScreenSize.Desktop1366 => (int)(basePadding * 1.1f),
                ScreenSize.FullHD => (int)(basePadding * 1.2f),
                _ => basePadding
            };
            
            return new Padding(padding);
        }

        /// <summary>
        /// Điều chỉnh margin theo độ phân giải
        /// </summary>
        public static Padding GetResponsiveMargin(int baseMargin, Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            int margin = screenSize switch
            {
                ScreenSize.Mobile => (int)(baseMargin * 0.5f),
                ScreenSize.Tablet => (int)(baseMargin * 0.75f),
                ScreenSize.Desktop1024 => baseMargin,
                ScreenSize.Desktop1366 => (int)(baseMargin * 1.1f),
                ScreenSize.FullHD => (int)(baseMargin * 1.2f),
                _ => baseMargin
            };
            
            return new Padding(margin);
        }

        /// <summary>
        /// Điều chỉnh kích thước control theo độ phân giải
        /// </summary>
        public static Size GetResponsiveSize(Size baseSize, Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            float scale = screenSize switch
            {
                ScreenSize.Mobile => 0.8f,
                ScreenSize.Tablet => 0.9f,
                ScreenSize.Desktop1024 => 1.0f,
                ScreenSize.Desktop1366 => 1.05f,
                ScreenSize.FullHD => 1.1f,
                _ => 1.0f
            };
            
            return new Size((int)(baseSize.Width * scale), (int)(baseSize.Height * scale));
        }

        /// <summary>
        /// Áp dụng responsive layout cho grid
        /// </summary>
        public static int GetResponsiveColumnCount(Control control, int baseColumns = 4)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => 1,
                ScreenSize.Tablet => 2,
                ScreenSize.Desktop1024 => baseColumns,
                ScreenSize.Desktop1366 => baseColumns + 1,
                ScreenSize.FullHD => baseColumns + 2,
                _ => baseColumns
            };
        }

        /// <summary>
        /// Điều chỉnh chiều rộng cột DataGridView
        /// </summary>
        public static void AdjustDataGridViewColumns(DataGridView dgv, Control parentControl)
        {
            if (dgv == null) return;
            
            var screenSize = GetCurrentScreenSize(parentControl);
            float scale = screenSize switch
            {
                ScreenSize.Mobile => 0.8f,
                ScreenSize.Tablet => 0.9f,
                ScreenSize.Desktop1024 => 1.0f,
                ScreenSize.Desktop1366 => 1.05f,
                ScreenSize.FullHD => 1.1f,
                _ => 1.0f
            };
            
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Width > 0)
                {
                    col.Width = (int)(col.Width * scale);
                }
            }
        }

        /// <summary>
        /// Điều chỉnh kích thước icon theo độ phân giải
        /// </summary>
        public static int GetResponsiveIconSize(Control control, int baseSize = 24)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => (int)(baseSize * 0.75f),
                ScreenSize.Tablet => (int)(baseSize * 0.85f),
                ScreenSize.Desktop1024 => baseSize,
                ScreenSize.Desktop1366 => (int)(baseSize * 1.1f),
                ScreenSize.FullHD => (int)(baseSize * 1.2f),
                _ => baseSize
            };
        }

        /// <summary>
        /// Điều chỉnh chiều rộng sidebar
        /// </summary>
        public static int GetResponsiveSidebarWidth(Control control, int baseWidth = 240)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => 60,      // Chỉ icon
                ScreenSize.Tablet => 100,
                ScreenSize.Desktop1024 => baseWidth,
                ScreenSize.Desktop1366 => baseWidth,
                ScreenSize.FullHD => (int)(baseWidth * 1.1f),
                _ => baseWidth
            };
        }

        /// <summary>
        /// Điều chỉnh chiều cao row trong DataGridView
        /// </summary>
        public static int GetResponsiveRowHeight(Control control, int baseHeight = 30)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => (int)(baseHeight * 0.9f),
                ScreenSize.Tablet => (int)(baseHeight * 0.95f),
                ScreenSize.Desktop1024 => baseHeight,
                ScreenSize.Desktop1366 => (int)(baseHeight * 1.05f),
                ScreenSize.FullHD => (int)(baseHeight * 1.1f),
                _ => baseHeight
            };
        }

        /// <summary>
        /// Điều chỉnh kích thước button theo độ phân giải
        /// </summary>
        public static Size GetResponsiveButtonSize(Control control, Size baseSize)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            float scale = screenSize switch
            {
                ScreenSize.Mobile => 0.85f,
                ScreenSize.Tablet => 0.9f,
                ScreenSize.Desktop1024 => 1.0f,
                ScreenSize.Desktop1366 => 1.05f,
                ScreenSize.FullHD => 1.1f,
                _ => 1.0f
            };
            
            return new Size((int)(baseSize.Width * scale), (int)(baseSize.Height * scale));
        }

        /// <summary>
        /// Điều chỉnh spacing giữa các control
        /// </summary>
        public static int GetResponsiveSpacing(Control control, int baseSpacing = 10)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => (int)(baseSpacing * 0.5f),
                ScreenSize.Tablet => (int)(baseSpacing * 0.75f),
                ScreenSize.Desktop1024 => baseSpacing,
                ScreenSize.Desktop1366 => (int)(baseSpacing * 1.1f),
                ScreenSize.FullHD => (int)(baseSpacing * 1.2f),
                _ => baseSpacing
            };
        }

        /// <summary>
        /// Áp dụng responsive layout cho FlowLayoutPanel
        /// </summary>
        public static void AdjustFlowLayoutPanel(FlowLayoutPanel panel, Control parentControl)
        {
            if (panel == null) return;
            
            var screenSize = GetCurrentScreenSize(parentControl);
            
            panel.AutoScroll = true;
            
            switch (screenSize)
            {
                case ScreenSize.Mobile:
                    panel.FlowDirection = FlowDirection.TopDown;
                    panel.Margin = new Padding(5);
                    break;
                case ScreenSize.Tablet:
                    panel.FlowDirection = FlowDirection.LeftToRight;
                    panel.Margin = new Padding(8);
                    break;
                case ScreenSize.Desktop1024:
                case ScreenSize.Desktop1366:
                case ScreenSize.FullHD:
                    panel.FlowDirection = FlowDirection.LeftToRight;
                    panel.Margin = new Padding(10);
                    break;
            }
        }

        /// <summary>
        /// Kiểm tra xem có nên hiển thị control hay không dựa trên độ phân giải
        /// </summary>
        public static bool ShouldShowControl(Control control, ScreenSize minScreenSize)
        {
            var currentSize = GetCurrentScreenSize(control);
            return currentSize >= minScreenSize;
        }

        /// <summary>
        /// Lấy chiều rộng tối ưu cho content area
        /// </summary>
        public static int GetOptimalContentWidth(Control control)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            return screenSize switch
            {
                ScreenSize.Mobile => control.Width - 20,
                ScreenSize.Tablet => control.Width - 30,
                ScreenSize.Desktop1024 => control.Width - 40,
                ScreenSize.Desktop1366 => control.Width - 50,
                ScreenSize.FullHD => control.Width - 60,
                _ => control.Width - 40
            };
        }

        /// <summary>
        /// Điều chỉnh kích thước modal dialog
        /// </summary>
        public static Size GetResponsiveDialogSize(Control control, Size baseSize)
        {
            var screenSize = GetCurrentScreenSize(control);
            
            float widthPercent = screenSize switch
            {
                ScreenSize.Mobile => 0.95f,
                ScreenSize.Tablet => 0.9f,
                ScreenSize.Desktop1024 => 0.8f,
                ScreenSize.Desktop1366 => 0.75f,
                ScreenSize.FullHD => 0.7f,
                _ => 0.8f
            };
            
            float heightPercent = screenSize switch
            {
                ScreenSize.Mobile => 0.95f,
                ScreenSize.Tablet => 0.9f,
                ScreenSize.Desktop1024 => 0.85f,
                ScreenSize.Desktop1366 => 0.8f,
                ScreenSize.FullHD => 0.75f,
                _ => 0.85f
            };
            
            int width = (int)(Screen.PrimaryScreen.Bounds.Width * widthPercent);
            int height = (int)(Screen.PrimaryScreen.Bounds.Height * heightPercent);
            
            return new Size(width, height);
        }
    }
}

