using System;
using System.Windows.Forms;
using System.Drawing;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Utilities
{
    /// <summary>
    /// Hỗ trợ animation mượt mà cho các control
    /// </summary>
    public static class AnimationHelper
    {
        // Animation types
        public enum AnimationType
        {
            FadeIn,
            FadeOut,
            SlideInLeft,
            SlideInRight,
            SlideInUp,
            SlideInDown,
            ScaleIn,
            ScaleOut,
            Bounce,
            Pulse
        }

        /// <summary>
        /// Fade In animation
        /// </summary>
        public static void FadeIn(Control control, int duration = 300)
        {
            if (control == null) return;
            
            control.Opacity = 0;
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                control.Opacity = progress;
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Fade Out animation
        /// </summary>
        public static void FadeOut(Control control, int duration = 300, Action onComplete = null)
        {
            if (control == null) return;
            
            control.Opacity = 1;
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                control.Opacity = 1f - progress;
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    onComplete?.Invoke();
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Slide In animation từ trái
        /// </summary>
        public static void SlideInLeft(Control control, int duration = 400)
        {
            if (control == null) return;
            
            int startX = -control.Width;
            int endX = control.Left;
            int startLeft = control.Left;
            control.Left = startX;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                // Easing: ease-out
                progress = 1f - (float)Math.Pow(1f - progress, 3);
                
                control.Left = startX + (int)((endX - startX) * progress);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Left = endX;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Slide In animation từ phải
        /// </summary>
        public static void SlideInRight(Control control, int duration = 400)
        {
            if (control == null) return;
            
            int startX = control.Parent.Width;
            int endX = control.Left;
            control.Left = startX;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                progress = 1f - (float)Math.Pow(1f - progress, 3);
                
                control.Left = startX + (int)((endX - startX) * progress);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Left = endX;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Slide In animation từ trên
        /// </summary>
        public static void SlideInUp(Control control, int duration = 400)
        {
            if (control == null) return;
            
            int startY = -control.Height;
            int endY = control.Top;
            control.Top = startY;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                progress = 1f - (float)Math.Pow(1f - progress, 3);
                
                control.Top = startY + (int)((endY - startY) * progress);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Top = endY;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Slide In animation từ dưới
        /// </summary>
        public static void SlideInDown(Control control, int duration = 400)
        {
            if (control == null) return;
            
            int startY = control.Parent.Height;
            int endY = control.Top;
            control.Top = startY;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                progress = 1f - (float)Math.Pow(1f - progress, 3);
                
                control.Top = startY + (int)((endY - startY) * progress);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Top = endY;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Scale In animation
        /// </summary>
        public static void ScaleIn(Control control, int duration = 300)
        {
            if (control == null) return;
            
            int originalWidth = control.Width;
            int originalHeight = control.Height;
            int originalX = control.Left;
            int originalY = control.Top;
            
            control.Width = 0;
            control.Height = 0;
            control.Left = originalX + originalWidth / 2;
            control.Top = originalY + originalHeight / 2;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                progress = (float)Math.Pow(progress, 2); // ease-in
                
                control.Width = (int)(originalWidth * progress);
                control.Height = (int)(originalHeight * progress);
                control.Left = originalX + (int)((originalWidth - control.Width) / 2f);
                control.Top = originalY + (int)((originalHeight - control.Height) / 2f);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Width = originalWidth;
                    control.Height = originalHeight;
                    control.Left = originalX;
                    control.Top = originalY;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Pulse animation (nhấp nháy)
        /// </summary>
        public static void Pulse(Control control, int duration = 600, int cycles = 1)
        {
            if (control == null) return;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            int totalDuration = duration * cycles;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = ((float)elapsed % duration) / duration;
                
                // Sine wave for smooth pulse
                float scale = 1f + 0.2f * (float)Math.Sin(progress * Math.PI * 2);
                
                if (elapsed >= totalDuration)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Opacity = 1f;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Bounce animation
        /// </summary>
        public static void Bounce(Control control, int duration = 500)
        {
            if (control == null) return;
            
            int originalY = control.Top;
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                
                // Bounce easing function
                float bounce = (float)(Math.Sin(progress * Math.PI * 2) * Math.Pow(1 - progress, 2));
                control.Top = originalY - (int)(bounce * 30);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Top = originalY;
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Animate button hover effect
        /// </summary>
        public static void AnimateButtonHover(Guna2Button button, bool isHovering, int duration = 200)
        {
            if (button == null) return;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            Color startColor = button.FillColor;
            Color endColor = isHovering 
                ? Color.FromArgb(31, 41, 55)  // Darker on hover
                : Color.FromArgb(17, 24, 39); // Normal
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                
                button.FillColor = InterpolateColor(startColor, endColor, progress);
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Interpolate between two colors
        /// </summary>
        private static Color InterpolateColor(Color start, Color end, float progress)
        {
            int r = (int)(start.R + (end.R - start.R) * progress);
            int g = (int)(start.G + (end.G - start.G) * progress);
            int b = (int)(start.B + (end.B - start.B) * progress);
            int a = (int)(start.A + (end.A - start.A) * progress);
            
            return Color.FromArgb(
                Math.Max(0, Math.Min(255, a)),
                Math.Max(0, Math.Min(255, r)),
                Math.Max(0, Math.Min(255, g)),
                Math.Max(0, Math.Min(255, b))
            );
        }

        /// <summary>
        /// Animate value change (e.g., for labels showing numbers)
        /// </summary>
        public static void AnimateValue(Label label, int startValue, int endValue, int duration = 500)
        {
            if (label == null) return;
            
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                
                int currentValue = (int)(startValue + (endValue - startValue) * progress);
                label.Text = currentValue.ToString();
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    label.Text = endValue.ToString();
                }
            };
            
            timer.Start();
        }

        /// <summary>
        /// Shake animation (cho lỗi)
        /// </summary>
        public static void Shake(Control control, int duration = 300)
        {
            if (control == null) return;
            
            int originalX = control.Left;
            var timer = new Timer { Interval = 15 };
            int elapsed = 0;
            
            timer.Tick += (s, e) =>
            {
                elapsed += timer.Interval;
                float progress = Math.Min(1f, (float)elapsed / duration);
                
                // Shake effect
                int shake = (int)(Math.Sin(progress * Math.PI * 8) * 10 * (1 - progress));
                control.Left = originalX + shake;
                
                if (progress >= 1f)
                {
                    timer.Stop();
                    timer.Dispose();
                    control.Left = originalX;
                }
            };
            
            timer.Start();
        }
    }
}

