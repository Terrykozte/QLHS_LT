using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Interaction Helper - Enhanced user interactions and feedback
    /// </summary>
    public static class InteractionHelper
    {
        /// <summary>
        /// Show action feedback with animation
        /// </summary>
        public static void ShowActionFeedback(Control parent, string message, ActionType type, int duration = 3000)
        {
            try
            {
                var feedbackForm = new Form
                {
                    Text = "",
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = GetActionColor(type),
                    Width = 400,
                    Height = 60,
                    TopMost = true,
                    ShowInTaskbar = false,
                    StartPosition = FormStartPosition.Manual,
                    Opacity = 0.95
                };

                var label = new Label
                {
                    Text = GetActionIcon(type) + " " + message,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };

                feedbackForm.Controls.Add(label);

                // Position at bottom right
                if (parent != null)
                {
                    feedbackForm.Location = new Point(
                        parent.Right - feedbackForm.Width - 16,
                        parent.Bottom - feedbackForm.Height - 16);
                }

                // Fade in
                AnimationHelper.FadeIn(feedbackForm, 200);
                feedbackForm.Show();

                // Auto close with fade out
                var timer = new Timer { Interval = duration };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    AnimationHelper.FadeOut(feedbackForm, 200, () =>
                    {
                        feedbackForm.Close();
                        feedbackForm.Dispose();
                    });
                };
                timer.Start();
            }
            catch { }
        }

        /// <summary>
        /// Show loading overlay on control
        /// </summary>
        public static void ShowLoadingOverlay(Control parent)
        {
            try
            {
                var overlay = new Panel
                {
                    BackColor = Color.FromArgb(0, 0, 0),
                    Opacity = 0.3,
                    Dock = DockStyle.Fill,
                    Name = "LoadingOverlay"
                };

                var spinner = new Label
                {
                    Text = "⏳ Đang tải...",
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false
                };

                overlay.Controls.Add(spinner);
                parent.Controls.Add(overlay);
                overlay.BringToFront();

                // Animate spinner
                AnimationHelper.RotateIcon(spinner, 5000);
            }
            catch { }
        }

        /// <summary>
        /// Hide loading overlay
        /// </summary>
        public static void HideLoadingOverlay(Control parent)
        {
            try
            {
                var overlay = parent.Controls["LoadingOverlay"];
                if (overlay != null)
                {
                    AnimationHelper.FadeOut(overlay, 200, () =>
                    {
                        parent.Controls.Remove(overlay);
                        overlay.Dispose();
                    });
                }
            }
            catch { }
        }

        /// <summary>
        /// Highlight row with animation
        /// </summary>
        public static void HighlightRow(DataGridView dgv, int rowIndex, int duration = 500)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgv.RowCount) return;

                var row = dgv.Rows[rowIndex];
                Color originalColor = row.DefaultCellStyle.BackColor;
                Color highlightColor = Color.FromArgb(255, 255, 200); // Light yellow

                AnimationHelper.ColorTransition(row, originalColor, highlightColor, duration / 2, () =>
                {
                    AnimationHelper.ColorTransition(row, highlightColor, originalColor, duration / 2);
                });
            }
            catch { }
        }

        /// <summary>
        /// Show confirmation dialog with animation
        /// </summary>
        public static DialogResult ShowConfirmationDialog(IWin32Window owner, string message, string title = "Xác nhận")
        {
            try
            {
                var result = MessageBox.Show(owner, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                return result;
            }
            catch
            {
                return DialogResult.No;
            }
        }

        /// <summary>
        /// Show success message
        /// </summary>
        public static void ShowSuccess(Control parent, string message)
        {
            ShowActionFeedback(parent, message, ActionType.Success);
        }

        /// <summary>
        /// Show error message
        /// </summary>
        public static void ShowError(Control parent, string message)
        {
            ShowActionFeedback(parent, message, ActionType.Error);
        }

        /// <summary>
        /// Show warning message
        /// </summary>
        public static void ShowWarning(Control parent, string message)
        {
            ShowActionFeedback(parent, message, ActionType.Warning);
        }

        /// <summary>
        /// Show info message
        /// </summary>
        public static void ShowInfo(Control parent, string message)
        {
            ShowActionFeedback(parent, message, ActionType.Info);
        }

        /// <summary>
        /// Enable/disable all controls in container with visual feedback
        /// </summary>
        public static void SetControlsEnabled(Control container, bool enabled, int duration = 200)
        {
            try
            {
                foreach (Control ctrl in container.Controls)
                {
                    ctrl.Enabled = enabled;
                    
                    if (!enabled)
                    {
                        AnimationHelper.ColorTransition(ctrl, 
                            ModernUIHelper.ModernColors.Surface,
                            ModernUIHelper.ModernColors.Gray100,
                            duration);
                    }
                    else
                    {
                        AnimationHelper.ColorTransition(ctrl,
                            ModernUIHelper.ModernColors.Gray100,
                            ModernUIHelper.ModernColors.Surface,
                            duration);
                    }

                    if (ctrl.HasChildren)
                    {
                        SetControlsEnabled(ctrl, enabled, duration);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Animate button click
        /// </summary>
        public static void AnimateButtonClick(Button btn)
        {
            try
            {
                AnimationHelper.Pulse(btn, 200);
            }
            catch { }
        }

        /// <summary>
        /// Show tooltip with animation
        /// </summary>
        public static void ShowTooltipAnimated(Control control, string text, int duration = 3000)
        {
            try
            {
                var tooltip = new ToolTip
                {
                    AutoPopDelay = duration,
                    InitialDelay = 0,
                    ReshowDelay = 0,
                    ShowAlways = true
                };

                tooltip.SetToolTip(control, text);
            }
            catch { }
        }

        /// <summary>
        /// Animate data grid row addition
        /// </summary>
        public static void AnimateRowAddition(DataGridView dgv, int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgv.RowCount) return;

                var row = dgv.Rows[rowIndex];
                row.Height = 0;

                AnimationHelper.Expand(row, 40, 300, () =>
                {
                    HighlightRow(dgv, rowIndex, 400);
                });
            }
            catch { }
        }

        /// <summary>
        /// Animate data grid row removal
        /// </summary>
        public static void AnimateRowRemoval(DataGridView dgv, int rowIndex, Action onComplete = null)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgv.RowCount) return;

                var row = dgv.Rows[rowIndex];
                AnimationHelper.Collapse(row, 300, () =>
                {
                    dgv.Rows.RemoveAt(rowIndex);
                    onComplete?.Invoke();
                });
            }
            catch { }
        }

        /// <summary>
        /// Show search results animation
        /// </summary>
        public static void AnimateSearchResults(DataGridView dgv, int resultCount)
        {
            try
            {
                if (resultCount == 0)
                {
                    AnimationHelper.Shake(dgv, 300);
                }
                else
                {
                    AnimationHelper.Highlight(dgv, Color.FromArgb(200, 255, 200), 500);
                }
            }
            catch { }
        }

        /// <summary>
        /// Disable control with fade effect
        /// </summary>
        public static void DisableWithFade(Control control, int duration = 200)
        {
            try
            {
                control.Enabled = false;
                AnimationHelper.ColorTransition(control,
                    ModernUIHelper.ModernColors.Surface,
                    ModernUIHelper.ModernColors.Gray100,
                    duration);
            }
            catch { }
        }

        /// <summary>
        /// Enable control with fade effect
        /// </summary>
        public static void EnableWithFade(Control control, int duration = 200)
        {
            try
            {
                control.Enabled = true;
                AnimationHelper.ColorTransition(control,
                    ModernUIHelper.ModernColors.Gray100,
                    ModernUIHelper.ModernColors.Surface,
                    duration);
            }
            catch { }
        }

        /// <summary>
        /// Get action type color
        /// </summary>
        private static Color GetActionColor(ActionType type)
        {
            return type switch
            {
                ActionType.Success => ModernUIHelper.ModernColors.Success500,
                ActionType.Error => ModernUIHelper.ModernColors.Error500,
                ActionType.Warning => ModernUIHelper.ModernColors.Warning500,
                ActionType.Info => ModernUIHelper.ModernColors.Primary500,
                _ => ModernUIHelper.ModernColors.Gray600
            };
        }

        /// <summary>
        /// Get action type icon
        /// </summary>
        private static string GetActionIcon(ActionType type)
        {
            return type switch
            {
                ActionType.Success => "✅",
                ActionType.Error => "❌",
                ActionType.Warning => "⚠️",
                ActionType.Info => "ℹ️",
                _ => "📌"
            };
        }
    }

    /// <summary>
    /// Action type enumeration
    /// </summary>
    public enum ActionType
    {
        Success,
        Error,
        Warning,
        Info
    }
}

