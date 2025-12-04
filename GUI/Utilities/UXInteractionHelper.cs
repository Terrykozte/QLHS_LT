using System;
using System.Windows.Forms;
using System.Drawing;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ UX interactions (hover, click, loading states, etc.)
    /// </summary>
    public static class UXInteractionHelper
    {
        /// <summary>
        /// Thêm hover effect cho button
        /// </summary>
        public static void AddHoverEffect(Guna2Button button, Color normalColor, Color hoverColor)
        {
            if (button == null) return;

            button.MouseEnter += (s, e) =>
            {
                button.FillColor = hoverColor;
                button.Cursor = Cursors.Hand;
            };

            button.MouseLeave += (s, e) =>
            {
                button.FillColor = normalColor;
                button.Cursor = Cursors.Default;
            };
        }

        /// <summary>
        /// Thêm click effect cho button
        /// </summary>
        public static void AddClickEffect(Guna2Button button)
        {
            if (button == null) return;

            Color originalColor = button.FillColor;

            button.MouseDown += (s, e) =>
            {
                button.FillColor = Color.FromArgb(
                    Math.Max(0, originalColor.R - 30),
                    Math.Max(0, originalColor.G - 30),
                    Math.Max(0, originalColor.B - 30)
                );
            };

            button.MouseUp += (s, e) =>
            {
                button.FillColor = originalColor;
            };
        }

        /// <summary>
        /// Thêm ripple effect cho button
        /// </summary>
        public static void AddRippleEffect(Guna2Button button, Color rippleColor)
        {
            if (button == null) return;

            button.MouseDown += (s, e) =>
            {
                var ripple = new Panel
                {
                    BackColor = rippleColor,
                    Location = e.Location,
                    Size = new Size(10, 10),
                    BorderStyle = BorderStyle.None
                };

                button.Controls.Add(ripple);

                var timer = new Timer { Interval = 15 };
                int elapsed = 0;
                int duration = 600;
                int maxRadius = Math.Max(button.Width, button.Height);

                timer.Tick += (s2, e2) =>
                {
                    elapsed += timer.Interval;
                    float progress = (float)elapsed / duration;

                    if (progress >= 1f)
                    {
                        timer.Stop();
                        timer.Dispose();
                        try { button.Controls.Remove(ripple); } catch { }
                        ripple.Dispose();
                    }
                    else
                    {
                        int radius = (int)(maxRadius * progress);
                        ripple.Size = new Size(radius, radius);
                        ripple.Location = new Point(
                            e.Location.X - radius / 2,
                            e.Location.Y - radius / 2
                        );
                        // Opacity not supported on Panel; skipping alpha fade
                    }
                };

                timer.Start();
            };
        }

        /// <summary>
        /// Hiển thị loading state cho button
        /// </summary>
        public static void ShowLoadingState(Guna2Button button, string loadingText = "Đang xử lý...")
        {
            if (button == null) return;

            button.Enabled = false;
            button.Text = loadingText;
            button.Cursor = Cursors.WaitCursor;
        }

        /// <summary>
        /// Ẩn loading state cho button
        /// </summary>
        public static void HideLoadingState(Guna2Button button, string originalText)
        {
            if (button == null) return;

            button.Enabled = true;
            button.Text = originalText;
            button.Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Thêm tooltip cho control
        /// </summary>
        public static void AddTooltip(Control control, string tooltipText, int autoPopDelay = 3000)
        {
            if (control == null) return;

            var tooltip = new ToolTip
            {
                AutoPopDelay = autoPopDelay,
                InitialDelay = 500,
                ReshowDelay = 200,
                ShowAlways = false
            };

            tooltip.SetToolTip(control, tooltipText);
        }

        /// <summary>
        /// Hiển thị notification toast
        /// </summary>
        public static void ShowToast(Form parentForm, string message, int duration = 3000, 
            Color? backgroundColor = null, Color? textColor = null)
        {
            if (parentForm == null) return;

            var toastPanel = new Guna2Panel
            {
                Size = new Size(300, 60),
                Location = new Point(parentForm.Width - 320, parentForm.Height - 80),
                FillColor = backgroundColor ?? Color.FromArgb(31, 41, 55),
                BorderRadius = 10,
                Visible = false
            };

            var toastLabel = new Label
            {
                Text = message,
                ForeColor = textColor ?? Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10)
            };

            toastPanel.Controls.Add(toastLabel);
            parentForm.Controls.Add(toastPanel);
            toastPanel.BringToFront();

            // Slide in animation
            AnimationHelper.SlideInUp(toastPanel, 300);
            toastPanel.Visible = true;

            // Auto hide
            var timer = new Timer { Interval = duration };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                AnimationHelper.SlideInDown(toastPanel, 300);
                timer = new Timer { Interval = 300 };
                timer.Tick += (s2, e2) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    try { parentForm.Controls.Remove(toastPanel); } catch { }
                    toastPanel.Dispose();
                };
                timer.Start();
            };
            timer.Start();
        }

        /// <summary>
        /// Hiển thị loading spinner
        /// </summary>
        public static void ShowLoadingSpinner(Guna2Panel container, int size = 40)
        {
            if (container == null) return;

            var spinner = new PictureBox
            {
                Size = new Size(size, size),
                Location = new Point(
                    (container.Width - size) / 2,
                    (container.Height - size) / 2
                ),
                SizeMode = PictureBoxSizeMode.CenterImage
            };

            container.Controls.Add(spinner);

            // Animate rotation
            var timer = new Timer { Interval = 30 };
            float rotation = 0;

            timer.Tick += (s, e) =>
            {
                rotation += 10;
                if (rotation >= 360) rotation = 0;
                // Note: Cần implement rotation graphics
            };

            timer.Start();
        }

        /// <summary>
        /// Thêm input validation visual feedback
        /// </summary>
        public static void AddValidationFeedback(Guna2TextBox textBox, bool isValid)
        {
            if (textBox == null) return;

            if (isValid)
            {
                textBox.BorderColor = Color.FromArgb(34, 197, 94); // Green
                textBox.FocusedState.BorderColor = Color.FromArgb(34, 197, 94);
            }
            else
            {
                textBox.BorderColor = Color.FromArgb(239, 68, 68); // Red
                textBox.FocusedState.BorderColor = Color.FromArgb(239, 68, 68);
                
                // Shake animation
                AnimationHelper.Shake(textBox, 300);
            }
        }

        /// <summary>
        /// Thêm focus effect cho TextBox
        /// </summary>
        public static void AddFocusEffect(Guna2TextBox textBox, Color focusColor)
        {
            if (textBox == null) return;

            Color originalColor = textBox.BorderColor;

            textBox.Enter += (s, e) =>
            {
                textBox.BorderColor = focusColor;
            };

            textBox.Leave += (s, e) =>
            {
                textBox.BorderColor = originalColor;
            };
        }

        /// <summary>
        /// Thêm character counter cho TextBox
        /// </summary>
        public static void AddCharacterCounter(Guna2TextBox textBox, Label counterLabel, int maxChars)
        {
            if (textBox == null || counterLabel == null) return;

            textBox.TextChanged += (s, e) =>
            {
                int remaining = Math.Max(0, maxChars - textBox.Text.Length);
                counterLabel.Text = $"{textBox.Text.Length}/{maxChars}";
                
                if (remaining <= 10)
                {
                    counterLabel.ForeColor = Color.FromArgb(239, 68, 68); // Red
                }
                else if (remaining <= 50)
                {
                    counterLabel.ForeColor = Color.FromArgb(245, 158, 11); // Orange
                }
                else
                {
                    counterLabel.ForeColor = Color.FromArgb(107, 114, 128); // Gray
                }
            };
        }

        /// <summary>
        /// Thêm dropdown animation cho ComboBox
        /// </summary>
        public static void AddDropdownAnimation(Guna2ComboBox comboBox)
        {
            if (comboBox == null) return;

            comboBox.DropDown += (s, e) =>
            {
                // Có thể thêm animation ở đây
            };
        }

        /// <summary>
        /// Hiển thị confirmation dialog
        /// </summary>
        public static DialogResult ShowConfirmation(string title, string message, string yesText = "Có", string noText = "Không")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Hiển thị error message
        /// </summary>
        public static void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị success message
        /// </summary>
        public static void ShowSuccess(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị warning message
        /// </summary>
        public static void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Thêm smooth scroll cho Panel
        /// </summary>
        public static void AddSmoothScroll(Panel panel)
        {
            if (panel == null) return;

            panel.MouseWheel += (s, e) =>
            {
                int scrollAmount = e.Delta > 0 ? -10 : 10;
                panel.AutoScrollPosition = new Point(
                    panel.AutoScrollPosition.X,
                    panel.AutoScrollPosition.Y - scrollAmount
                );
            };
        }

        /// <summary>
        /// Thêm expand/collapse animation cho panel
        /// </summary>
        public static void AddExpandCollapseAnimation(Guna2Panel panel, Guna2Button toggleButton, int expandedHeight)
        {
            if (panel == null || toggleButton == null) return;

            bool isExpanded = true;
            int collapsedHeight = 0;

            toggleButton.Click += (s, e) =>
            {
                isExpanded = !isExpanded;
                
                var timer = new Timer { Interval = 15 };
                int elapsed = 0;
                int duration = 300;
                int startHeight = panel.Height;
                int endHeight = isExpanded ? expandedHeight : collapsedHeight;

                timer.Tick += (s2, e2) =>
                {
                    elapsed += timer.Interval;
                    float progress = Math.Min(1f, (float)elapsed / duration);
                    
                    panel.Height = startHeight + (int)((endHeight - startHeight) * progress);

                    if (progress >= 1f)
                    {
                        timer.Stop();
                        timer.Dispose();
                        panel.Height = endHeight;
                    }
                };

                timer.Start();
            };
        }

        /// <summary>
        /// Thêm progress bar animation
        /// </summary>
        public static void AnimateProgressBar(Guna2ProgressBar progressBar, int targetValue, int duration = 1000)
        {
            if (progressBar == null) return;

            int startValue = progressBar.Value;
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;

            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                
                progressBar.Value = startValue + (int)((targetValue - startValue) * progress);

                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    progressBar.Value = targetValue;
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Thêm disabled state effect
        /// </summary>
        public static void SetDisabledState(Control control, bool disabled)
        {
            if (control == null) return;

            control.Enabled = !disabled;
            // Opacity not supported on Control; simulate with cursor only or grayscale if needed
            control.Cursor = disabled ? Cursors.No : Cursors.Default;
        }
    }
}

