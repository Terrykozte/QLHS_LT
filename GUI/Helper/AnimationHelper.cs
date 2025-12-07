using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Animation and Effects Helper - Smooth animations and visual effects
    /// </summary>
    public static class AnimationHelper
    {
        // Animation timing
        public const int FADE_DURATION = 300;
        public const int SLIDE_DURATION = 400;
        public const int PULSE_DURATION = 600;
        public const int BOUNCE_DURATION = 500;

        /// <summary>
        /// Fade in animation
        /// </summary>
        public static void FadeIn(Control control, int duration = FADE_DURATION, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                control.Opacity = 0;
                control.Visible = true;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    control.Opacity = Math.Min(1.0, (double)elapsed / duration);

                    if (elapsed >= duration)
                    {
                        control.Opacity = 1.0;
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
        /// Fade out animation
        /// </summary>
        public static void FadeOut(Control control, int duration = FADE_DURATION, Action onComplete = null)
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
                        control.Opacity = 0.0;
                        control.Visible = false;
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
        /// Slide in animation from left
        /// </summary>
        public static void SlideInFromLeft(Control control, int duration = SLIDE_DURATION, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int startX = control.Location.X - control.Width;
                int endX = control.Location.X;
                control.Location = new Point(startX, control.Location.Y);
                control.Visible = true;

                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);
                    int newX = startX + (int)((endX - startX) * progress);
                    control.Location = new Point(newX, control.Location.Y);

                    if (elapsed >= duration)
                    {
                        control.Location = new Point(endX, control.Location.Y);
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
        /// Slide in animation from right
        /// </summary>
        public static void SlideInFromRight(Control control, int duration = SLIDE_DURATION, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int startX = control.Location.X + control.Width;
                int endX = control.Location.X;
                control.Location = new Point(startX, control.Location.Y);
                control.Visible = true;

                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);
                    int newX = startX - (int)((startX - endX) * progress);
                    control.Location = new Point(newX, control.Location.Y);

                    if (elapsed >= duration)
                    {
                        control.Location = new Point(endX, control.Location.Y);
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
        /// Pulse animation (scale effect)
        /// </summary>
        public static void Pulse(Control control, int duration = PULSE_DURATION, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;
                int originalWidth = control.Width;
                int originalHeight = control.Height;
                int originalX = control.Location.X;
                int originalY = control.Location.Y;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = (double)elapsed / duration;
                    
                    // Sine wave for smooth pulse
                    double scale = 1.0 + 0.1 * Math.Sin(progress * Math.PI * 2);
                    
                    int newWidth = (int)(originalWidth * scale);
                    int newHeight = (int)(originalHeight * scale);
                    int newX = originalX - (newWidth - originalWidth) / 2;
                    int newY = originalY - (newHeight - originalHeight) / 2;

                    control.Width = newWidth;
                    control.Height = newHeight;
                    control.Location = new Point(newX, newY);

                    if (elapsed >= duration)
                    {
                        control.Width = originalWidth;
                        control.Height = originalHeight;
                        control.Location = new Point(originalX, originalY);
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
        /// Bounce animation
        /// </summary>
        public static void Bounce(Control control, int duration = BOUNCE_DURATION, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int originalY = control.Location.Y;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = (double)elapsed / duration;
                    
                    // Bounce easing
                    double easeProgress = 1.0 - Math.Pow(2.0, -10.0 * progress) * Math.Cos((progress * 10.0 - 0.75) * (2.0 * Math.PI) / 3.0);
                    
                    int newY = originalY - (int)(50 * easeProgress);
                    control.Location = new Point(control.Location.X, newY);

                    if (elapsed >= duration)
                    {
                        control.Location = new Point(control.Location.X, originalY);
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
        /// Color transition animation
        /// </summary>
        public static void ColorTransition(Control control, Color fromColor, Color toColor, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);

                    int r = (int)(fromColor.R + (toColor.R - fromColor.R) * progress);
                    int g = (int)(fromColor.G + (toColor.G - fromColor.G) * progress);
                    int b = (int)(fromColor.B + (toColor.B - fromColor.B) * progress);

                    control.BackColor = Color.FromArgb(r, g, b);

                    if (elapsed >= duration)
                    {
                        control.BackColor = toColor;
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
        /// Shake animation (error effect)
        /// </summary>
        public static void Shake(Control control, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int originalX = control.Location.X;
                int originalY = control.Location.Y;
                var timer = new Timer { Interval = 50 };
                int shakeCount = 0;
                int maxShakes = (duration / 50);

                timer.Tick += (s, e) =>
                {
                    shakeCount++;
                    int offset = (shakeCount % 2 == 0) ? 5 : -5;
                    control.Location = new Point(originalX + offset, originalY);

                    if (shakeCount >= maxShakes)
                    {
                        control.Location = new Point(originalX, originalY);
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
        /// Rotate animation (loading effect)
        /// </summary>
        public static void RotateIcon(Label label, int duration = 2000, Action onComplete = null)
        {
            if (label == null) return;

            try
            {
                var timer = new Timer { Interval = 50 };
                int elapsed = 0;
                int rotation = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    rotation = (rotation + 10) % 360;

                    // Update label with rotating icon
                    string[] icons = { "⏳", "⌛" };
                    label.Text = icons[(rotation / 90) % 2] + " Đang tải...";

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
        /// Highlight animation (attention effect)
        /// </summary>
        public static void Highlight(Control control, Color highlightColor, int duration = 500, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                Color originalColor = control.BackColor;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = (double)elapsed / duration;
                    
                    // Pulse between original and highlight color
                    double pulse = Math.Sin(progress * Math.PI * 2) * 0.5 + 0.5;
                    
                    int r = (int)(originalColor.R + (highlightColor.R - originalColor.R) * pulse);
                    int g = (int)(originalColor.G + (highlightColor.G - originalColor.G) * pulse);
                    int b = (int)(originalColor.B + (highlightColor.B - originalColor.B) * pulse);

                    control.BackColor = Color.FromArgb(r, g, b);

                    if (elapsed >= duration)
                    {
                        control.BackColor = originalColor;
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
        /// Expand animation (grow effect)
        /// </summary>
        public static void Expand(Control control, int targetHeight, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int startHeight = control.Height;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);
                    int newHeight = startHeight + (int)((targetHeight - startHeight) * progress);
                    control.Height = newHeight;

                    if (elapsed >= duration)
                    {
                        control.Height = targetHeight;
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
        /// Collapse animation (shrink effect)
        /// </summary>
        public static void Collapse(Control control, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int startHeight = control.Height;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);
                    int newHeight = startHeight - (int)(startHeight * progress);
                    control.Height = newHeight;

                    if (elapsed >= duration)
                    {
                        control.Height = 0;
                        control.Visible = false;
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
        /// Flip animation (card flip effect)
        /// </summary>
        public static void FlipHorizontal(Control control, int duration = 400, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                int originalWidth = control.Width;
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = (double)elapsed / duration;
                    
                    // Scale from 1 to 0 to 1 (flip effect)
                    double scale = Math.Cos(progress * Math.PI);
                    int newWidth = (int)(originalWidth * Math.Abs(scale));
                    int offsetX = (originalWidth - newWidth) / 2;

                    control.Width = newWidth;
                    control.Location = new Point(control.Location.X + offsetX, control.Location.Y);

                    if (elapsed >= duration)
                    {
                        control.Width = originalWidth;
                        control.Location = new Point(control.Location.X - offsetX, control.Location.Y);
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
        /// Glow effect animation
        /// </summary>
        public static void Glow(Control control, int duration = 1000, Action onComplete = null)
        {
            if (control == null) return;

            try
            {
                Color originalColor = control.BackColor;
                Color glowColor = Color.FromArgb(255, 200, 0); // Yellow glow
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = (double)elapsed / duration;
                    
                    // Smooth glow pulse
                    double glow = (Math.Sin(progress * Math.PI * 2) + 1.0) / 2.0;
                    
                    int r = (int)(originalColor.R + (glowColor.R - originalColor.R) * glow);
                    int g = (int)(originalColor.G + (glowColor.G - originalColor.G) * glow);
                    int b = (int)(originalColor.B + (glowColor.B - originalColor.B) * glow);

                    control.BackColor = Color.FromArgb(r, g, b);

                    if (elapsed >= duration)
                    {
                        control.BackColor = originalColor;
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
        /// Typewriter effect for text
        /// </summary>
        public static void TypewriterEffect(Label label, string text, int duration = 1000, Action onComplete = null)
        {
            if (label == null) return;

            try
            {
                label.Text = "";
                var timer = new Timer { Interval = duration / text.Length };
                int charIndex = 0;

                timer.Tick += (s, e) =>
                {
                    if (charIndex < text.Length)
                    {
                        label.Text += text[charIndex];
                        charIndex++;
                    }
                    else
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
        /// Smooth scroll animation
        /// </summary>
        public static void SmoothScroll(DataGridView dgv, int targetRowIndex, int duration = 500, Action onComplete = null)
        {
            if (dgv == null) return;

            try
            {
                var timer = new Timer { Interval = 16 };
                int elapsed = 0;
                int startIndex = dgv.FirstDisplayedScrollingRowIndex;
                int endIndex = targetRowIndex;

                timer.Tick += (s, e) =>
                {
                    elapsed += timer.Interval;
                    double progress = Math.Min(1.0, (double)elapsed / duration);
                    int newIndex = startIndex + (int)((endIndex - startIndex) * progress);
                    
                    if (newIndex >= 0 && newIndex < dgv.RowCount)
                    {
                        dgv.FirstDisplayedScrollingRowIndex = newIndex;
                    }

                    if (elapsed >= duration)
                    {
                        if (endIndex >= 0 && endIndex < dgv.RowCount)
                        {
                            dgv.FirstDisplayedScrollingRowIndex = endIndex;
                        }
                        timer.Stop();
                        timer.Dispose();
                        onComplete?.Invoke();
                    }
                };

                timer.Start();
            }
            catch { }
        }
    }
}

