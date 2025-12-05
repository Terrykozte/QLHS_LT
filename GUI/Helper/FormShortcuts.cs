using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Helper
{
    public class FormShortcuts : BaseForm
    {
        private Guna2TextBox txtGuide;
        private Guna2Button btnCopy;
        private Guna2Button btnClose;
        private Guna2HtmlLabel lblTitle;

        public FormShortcuts()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Hướng dẫn phím tắt & gợi ý thao tác";
            this.Size = new Size(720, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            ThemeHelper.ApplyThemeToForm(this);

            lblTitle = new Guna2HtmlLabel
            {
                Text = "Hướng dẫn phím tắt & gợi ý thao tác",
                Font = ThemeHelper.GetHeadingFont(16),
                ForeColor = ThemeHelper.GetTextColor(),
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = Color.Transparent,
                Padding = new Padding(12, 12, 12, 6)
            };

            txtGuide = new Guna2TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                ScrollBars = ScrollBars.Vertical,
                BorderRadius = 8,
                Font = ThemeHelper.GetBodyFont(11),
                ForeColor = ThemeHelper.GetTextColor(),
                FillColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10)
            };

            btnCopy = new Guna2Button
            {
                Text = "Sao chép hướng dẫn",
                Dock = DockStyle.Bottom,
                Height = 44,
                BorderRadius = 6,
                FillColor = ThemeHelper.Colors.Primary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(8)
            };
            btnCopy.Click += (s, e) => { try { Clipboard.SetText(txtGuide.Text); UXInteractionHelper.ShowToast(this, "Đã sao chép", 1500, ThemeHelper.Colors.Success, Color.White); } catch { } };

            btnClose = new Guna2Button
            {
                Text = "Đóng (Esc)",
                Dock = DockStyle.Bottom,
                Height = 40,
                BorderRadius = 6,
                FillColor = ThemeHelper.Colors.TextSecondary,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(8)
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(txtGuide);
            this.Controls.Add(btnCopy);
            this.Controls.Add(btnClose);
            this.Controls.Add(lblTitle);

            LoadContent();
        }

        private void LoadContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("I. Phím tắt chung");
            sb.AppendLine("- F1: Mở hướng dẫn phím tắt (màn hình chính, thanh toán)");
            sb.AppendLine("- F5: Làm mới dữ liệu (nhiều màn hình danh sách)");
            sb.AppendLine("- Esc: Đóng cửa sổ hiện tại");
            sb.AppendLine("- Enter: Xác nhận trong form hành động (ví dụ thanh toán)");
            sb.AppendLine("- Ctrl + D: Đổi theme Light/Dark và tự lưu lựa chọn");
            sb.AppendLine();
            sb.AppendLine("II. Tạo/Chỉnh sửa dữ liệu");
            sb.AppendLine("- Ctrl + N: Thêm mới (nhiều màn hình danh sách)");
            sb.AppendLine("- Ctrl + S: Lưu nhanh (khi đang ở form thêm/sửa)");
            sb.AppendLine("- Ctrl + E: Xuất báo cáo/ dữ liệu (một số màn hình như Menu, Báo cáo)");
            sb.AppendLine();
            sb.AppendLine("III. Đơn hàng & Thanh toán");
            sb.AppendLine("- Trong Thanh toán: Ctrl + Q: Tạo VietQR (Quick Link - compact2) theo số HĐ, ngày");
            sb.AppendLine("- Trong Thanh toán: VietQR sẽ tự cập nhật khi tổng tiền còn lại thay đổi");
            sb.AppendLine();
            sb.AppendLine("IV. Điều hướng");
            sb.AppendLine("- Mũi tên/Tab: Di chuyển giữa các nút trong sidebar/ô nhập liệu");
            sb.AppendLine("- Chuột: Double-click một dòng trong danh sách để mở chi tiết/sửa");
            sb.AppendLine();
            sb.AppendLine("V. Gợi ý thao tác nhanh");
            sb.AppendLine("- Theme: Ctrl + D khi cần chuyển nhanh chế độ sáng/tối");
            sb.AppendLine("- Tìm kiếm: Gõ từ khóa tại thanh tìm kiếm, nhấn Enter để lọc ngay");
            sb.AppendLine("- Xuất: Dùng Ctrl + E hoặc nút Export để xuất Excel/PDF (tùy màn hình)");
            sb.AppendLine("- QR: Dùng Ctrl + Q trong thanh toán để hiển thị QR theo số tiền còn lại");

            txtGuide.Text = sb.ToString();
        }
    }
}

