using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Diagnostics;

namespace QLTN_LT.GUI.Base
{
    /// <summary>
    /// BaseForm - Lớp cơ sở cho tất cả form trong ứng dụng
    /// - Xử lý lifecycle form an toàn
    /// - Ngăn chặn double-close
    /// - Quản lý resources tự động
    /// - Hỗ trợ fade-in animation
    /// </summary>
    public class BaseForm : Form
    {
        #region Fields

        protected Guna2BorderlessForm BorderlessForm;
        protected Guna2ShadowForm ShadowForm;

        private Timer _fadeTimer;
        private bool _allowClose = false;
        private bool _isClosing = false;
        private bool _isDisposed = false;

        #endregion

        #region Properties

        public bool CloseOnEsc { get; set; } = true;
        public string ConfirmationMessage { get; set; } = null;

        #endregion

        #region Constructor

        public BaseForm()
        {
            try
            {
                InitializeFormSettings();
                InitializeGuna2Components();
                InitializeFadeAnimation();
                InitializeKeyboardHandling();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing BaseForm: {ex.Message}");
                FallbackFormInitialization();
            }
        }

        #endregion

        #region Initialization

        private void InitializeFormSettings()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.Opacity = 0;
            this.DoubleBuffered = true;
            this.KeyPreview = true;
        }

        private void InitializeGuna2Components()
        {
            BorderlessForm = new Guna2BorderlessForm();
            ShadowForm = new Guna2ShadowForm();

            BorderlessForm.BorderRadius = 16;
            BorderlessForm.ShadowColor = Color.Gray;
            BorderlessForm.ContainerControl = this;
            BorderlessForm.DockIndicatorTransparencyValue = 0.6;
            BorderlessForm.TransparentWhileDrag = true;
        }

        private void InitializeFadeAnimation()
        {
            _fadeTimer = new Timer { Interval = 15 };
            _fadeTimer.Tick += FadeTimer_Tick;
        }

        private void InitializeKeyboardHandling()
        {
            this.KeyDown += BaseForm_KeyDown;
        }

        private void FallbackFormInitialization()
        {
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.White;
        }

        #endregion

        #region Event Handlers

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.05;
                }
                else
                {
                    _fadeTimer?.Stop();
                }
            }
            catch
            {
                _fadeTimer?.Stop();
            }
        }

        private void BaseForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && CloseOnEsc && this.TopLevel && !_isClosing)
            {
                e.Handled = true;
                this.Close();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!this.TopLevel)
            {
                // Form con - không cần animation
                DisposeFadeAnimation();
                DisposeGuna2Components();
                this.Opacity = 1;
            }
            else
            {
                // Form độc lập - bắt đầu fade-in
                _fadeTimer?.Start();
            }

            Debug.WriteLine($"Form loaded: {this.GetType().Name}");
        }

        #endregion

        #region Message Dialogs

        protected void ShowError(string message, string title = "Lỗi")
        {
            if (_isDisposed) return;
            try
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Debug.WriteLine($"[ERROR] {title}: {message}");
            }
            catch { }
        }

        protected void ShowInfo(string message, string title = "Thông báo")
        {
            if (_isDisposed) return;
            try
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Debug.WriteLine($"[INFO] {title}: {message}");
            }
            catch { }
        }

        protected void ShowWarning(string message, string title = "Cảnh báo")
        {
            if (_isDisposed) return;
            try
            {
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Debug.WriteLine($"[WARNING] {title}: {message}");
            }
            catch { }
        }

        protected bool ShowConfirm(string message, string title = "Xác nhận")
        {
            if (_isDisposed) return false;
            try
            {
                return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
            catch { return false; }
        }

        #endregion

        #region Utility Methods

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

        public void ForceClose()
        {
            _allowClose = true;
            this.Close();
        }

        #endregion

        #region Cleanup & Disposal

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Ngăn chặn double-close
            if (_isClosing)
            {
                e.Cancel = true;
                return;
            }

            _isClosing = true;

            // Kiểm tra confirmation message
            if (!_allowClose && !string.IsNullOrEmpty(ConfirmationMessage) && e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show(ConfirmationMessage, "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    _isClosing = false;
                    return;
                }
            }

            try
            {
                CleanupResources();
                base.OnFormClosing(e);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnFormClosing: {ex.Message}");
                _isClosing = false;
            }
        }

        protected virtual void CleanupResources()
        {
            DisposeFadeAnimation();
            DisposeGuna2Components();
        }

        private void DisposeFadeAnimation()
        {
            try
            {
                _fadeTimer?.Stop();
                _fadeTimer?.Dispose();
                _fadeTimer = null;
            }
            catch { }
        }

        private void DisposeGuna2Components()
        {
            try
            {
                BorderlessForm?.Dispose();
                BorderlessForm = null;

                ShadowForm?.Dispose();
                ShadowForm = null;
            }
            catch { }
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

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

            _isDisposed = true;
            base.Dispose(disposing);
        }

        #endregion
    }
}
