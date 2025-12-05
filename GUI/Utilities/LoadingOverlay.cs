using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hiển thị lớp phủ Loading (overlay) với progress để chờ tác vụ dài.
    /// </summary>
    public class LoadingOverlay : IDisposable
    {
        private Control _parent;
        private Panel _overlay;
        private Label _label;
        private Guna2ProgressBar _progress;

        public LoadingOverlay(Control parent, string text = "Đang tải...")
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            _overlay = new Panel
            {
                BackColor = Color.FromArgb(120, 0, 0, 0),
                Dock = DockStyle.Fill,
                Visible = false
            };

            var container = new Guna2Panel
            {
                Size = new Size(260, 90),
                BorderRadius = 8,
                FillColor = Color.White,
                BackColor = Color.Transparent
            };

            _label = new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            _progress = new Guna2ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 8,
                ProgressColor = ThemeHelper.Colors.Primary,
                ProgressColor2 = ThemeHelper.Colors.PrimaryLight,
                FillColor = Color.Gainsboro,
                // Marquee style not available on Guna2ProgressBar by default; simple indeterminate look
            };

            container.Controls.Add(_progress);
            container.Controls.Add(_label);

            _overlay.Controls.Add(container);
            _parent.Controls.Add(_overlay);
            _overlay.BringToFront();

            // Center container
            _overlay.Resize += (s, e) =>
            {
                container.Left = (_overlay.Width - container.Width) / 2;
                container.Top = (_overlay.Height - container.Height) / 2;
            };
        }

        public void Show(string text = null)
        {
            if (!string.IsNullOrWhiteSpace(text)) _label.Text = text;
            _overlay.Visible = true;
            _overlay.BringToFront();
            Application.DoEvents();
        }

        public void Hide()
        {
            _overlay.Visible = false;
            Application.DoEvents();
        }

        public void Dispose()
        {
            try
            {
                if (_overlay != null)
                {
                    _parent?.Controls.Remove(_overlay);
                    _overlay.Dispose();
                    _overlay = null;
                }
            }
            catch { }
        }
    }
}

