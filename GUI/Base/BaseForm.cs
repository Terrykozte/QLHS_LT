using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Diagnostics;

namespace QLTN_LT.GUI.Base
{
    /// <summary>
    /// Base form class with common styling and error handling for all forms in the application.
    /// </summary>
    public class BaseForm : Form
    {
        protected Guna2BorderlessForm BorderlessForm;
        protected Guna2ShadowForm ShadowForm;

        private Timer _fadeTimer;

        public BaseForm()
        {
            try
            {
                // Initialize Guna Components
                BorderlessForm = new Guna2BorderlessForm(this.Container);
                ShadowForm = new Guna2ShadowForm(this.Container);

                // Default Style
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.White;
                this.Opacity = 0; // for fade-in
                
                // Borderless Settings
                BorderlessForm.BorderRadius = 16;
                BorderlessForm.ShadowColor = Color.Gray;
                BorderlessForm.ContainerControl = this;
                BorderlessForm.DockIndicatorTransparencyValue = 0.6;
                BorderlessForm.TransparentWhileDrag = true;

                // Fade-in timer
                _fadeTimer = new Timer { Interval = 15 };
                _fadeTimer.Tick += (s, e) =>
                {
                    try
                    {
                        if (this.Opacity < 1)
                            this.Opacity += 0.05;
                        else
                            _fadeTimer.Stop();
                    }
                    catch { _fadeTimer.Stop(); }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing BaseForm: {ex.Message}");
                // Continue with basic form setup if Guna initialization fails
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.BackColor = Color.White;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                _fadeTimer?.Start();
                // Log form load
                Debug.WriteLine($"Form loaded: {this.GetType().Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in BaseForm.OnLoad: {ex.Message}");
                MessageBox.Show($"Lỗi tải form: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        protected void ShowError(string message, string title = "Lỗi")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine($"[ERROR] {title}: {message}");
        }

        /// <summary>
        /// Shows an information message to the user.
        /// </summary>
        protected void ShowInfo(string message, string title = "Thông báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Debug.WriteLine($"[INFO] {title}: {message}");
        }

        /// <summary>
        /// Shows a warning message to the user.
        /// </summary>
        protected void ShowWarning(string message, string title = "Cảnh báo")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Debug.WriteLine($"[WARNING] {title}: {message}");
        }

        /// <summary>
        /// Shows a confirmation dialog to the user.
        /// </summary>
        protected bool ShowConfirm(string message, string title = "Xác nhận")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// Enables or disables wait cursor for the form and its controls.
        /// </summary>
        protected void Wait(bool isWaiting)
        {
            try
            {
                this.UseWaitCursor = isWaiting;
                Cursor.Current = isWaiting ? Cursors.WaitCursor : Cursors.Default;
                Application.DoEvents();
            }
            catch { }
        }

        /// <summary>
        /// Safely closes the form with error handling.
        /// </summary>
        protected void SafeClose()
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing form: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles form closing event - cleanup resources.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                CleanupResources();
                base.OnFormClosing(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnFormClosing: {ex.Message}");
            }
        }

        /// <summary>
        /// Cleanup resources when form is closing.
        /// </summary>
        protected virtual void CleanupResources()
        {
            try
            {
                // Stop and dispose timers
                if (_fadeTimer != null)
                    {
                    _fadeTimer.Stop();
                    _fadeTimer.Dispose();
                }

                // Dispose Guna components
                if (BorderlessForm != null)
                {
                    BorderlessForm.Dispose();
                    BorderlessForm = null;
                }

                if (ShadowForm != null)
                {
                    ShadowForm.Dispose();
                    ShadowForm = null;
                }

                // Dispose fade timer
                if (_fadeTimer != null)
                {
                    _fadeTimer.Stop();
                    _fadeTimer.Dispose();
                    _fadeTimer = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    CleanupResources();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in Dispose: {ex.Message}");
                }
            }
            base.Dispose(disposing);
        }
    }
}
