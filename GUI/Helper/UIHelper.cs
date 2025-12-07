using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Helper class for consistent UI styling across the application.
    /// </summary>
    public static class UIHelper
    {
        // Color scheme
        public static class Colors
        {
            public static readonly Color Primary = Color.FromArgb(59, 130, 246);      // Blue-500
            public static readonly Color PrimaryHover = Color.FromArgb(37, 99, 235);  // Blue-600
            public static readonly Color Secondary = Color.FromArgb(107, 114, 128);   // Gray-500
            public static readonly Color Success = Color.FromArgb(34, 197, 94);       // Green-500
            public static readonly Color Warning = Color.FromArgb(251, 146, 60);      // Orange-500
            public static readonly Color Error = Color.FromArgb(239, 68, 68);         // Red-500
            public static readonly Color Background = Color.FromArgb(249, 250, 251);  // Gray-50
            public static readonly Color Surface = Color.White;
            public static readonly Color Border = Color.FromArgb(229, 231, 235);      // Gray-200
            public static readonly Color TextPrimary = Color.FromArgb(17, 24, 39);    // Gray-900
            public static readonly Color TextSecondary = Color.FromArgb(107, 114, 128); // Gray-500
            public static readonly Color Disabled = Color.FromArgb(209, 213, 219);    // Gray-300
        }

        /// <summary>
        /// Applies consistent styling to a form.
        /// </summary>
        public static void ApplyFormStyle(Form form)
        {
            if (form == null) return;

            try
            {
                form.BackColor = Colors.Background;
                form.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                form.Padding = new Padding(15);
                form.FormBorderStyle = FormBorderStyle.None;
                form.StartPosition = FormStartPosition.CenterScreen;
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a button.
        /// </summary>
        public static void ApplyButtonStyle(Button btn, bool isPrimary = false)
        {
            if (btn == null) return;

            try
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Padding = new Padding(15, 8, 15, 8);
                btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                btn.Cursor = Cursors.Hand;

                if (isPrimary)
                {
                    btn.BackColor = Colors.Primary;
                    btn.ForeColor = Color.White;
                    btn.FlatAppearance.MouseOverBackColor = Colors.PrimaryHover;
                }
                else
                {
                    btn.BackColor = Colors.Border;
                    btn.ForeColor = Colors.TextPrimary;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(209, 213, 219);
                }
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a Guna button.
        /// </summary>
        public static void ApplyGunaButtonStyle(Guna2Button btn, bool isPrimary = false)
        {
            if (btn == null) return;

            try
            {
                btn.Font = new Font("Segoe UI", 10, FontStyle.Regular);
                btn.BorderRadius = 6;
                btn.Cursor = Cursors.Hand;

                if (isPrimary)
                {
                    btn.FillColor = Colors.Primary;
                    btn.ForeColor = Color.White;
                    btn.HoverState.FillColor = Colors.PrimaryHover;
                }
                else
                {
                    btn.FillColor = Colors.Border;
                    btn.ForeColor = Colors.TextPrimary;
                    btn.HoverState.FillColor = Color.FromArgb(209, 213, 219);
                }
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a DataGridView.
        /// </summary>
        public static void ApplyGridStyle(DataGridView dgv)
        {
            if (dgv == null) return;

            try
            {
                dgv.BackgroundColor = Colors.Surface;
                dgv.GridColor = Colors.Border;
                dgv.DefaultCellStyle.BackColor = Colors.Surface;
                dgv.DefaultCellStyle.ForeColor = Colors.TextPrimary;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgv.DefaultCellStyle.Padding = new Padding(5);
                
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Colors.Background;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Colors.TextPrimary;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
                
                dgv.ColumnHeadersHeight = 40;
                dgv.RowTemplate.Height = 35;
                dgv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.ReadOnly = true;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.MultiSelect = false;
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a TextBox.
        /// </summary>
        public static void ApplyTextBoxStyle(TextBox txt)
        {
            if (txt == null) return;

            try
            {
                txt.Font = new Font("Segoe UI", 10);
                txt.BackColor = Colors.Surface;
                txt.ForeColor = Colors.TextPrimary;
                txt.BorderStyle = BorderStyle.FixedSingle;
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a ComboBox.
        /// </summary>
        public static void ApplyComboBoxStyle(ComboBox cmb)
        {
            if (cmb == null) return;

            try
            {
                cmb.Font = new Font("Segoe UI", 10);
                cmb.BackColor = Colors.Surface;
                cmb.ForeColor = Colors.TextPrimary;
                cmb.FlatStyle = FlatStyle.Flat;
            }
            catch { }
        }

        /// <summary>
        /// Applies consistent styling to a Label.
        /// </summary>
        public static void ApplyLabelStyle(Label lbl, bool isBold = false)
        {
            if (lbl == null) return;

            try
            {
                lbl.Font = new Font("Segoe UI", 10, isBold ? FontStyle.Bold : FontStyle.Regular);
                lbl.ForeColor = Colors.TextPrimary;
                lbl.BackColor = Color.Transparent;
            }
            catch { }
        }

        /// <summary>
        /// Shows a loading indicator on a control.
        /// </summary>
        public static void ShowLoading(Control parent, bool show = true)
        {
            if (parent == null) return;

            try
            {
                if (show)
                {
                    parent.Enabled = false;
                    parent.Cursor = Cursors.WaitCursor;
                }
                else
                {
                    parent.Enabled = true;
                    parent.Cursor = Cursors.Default;
                }
                Application.DoEvents();
            }
            catch { }
        }

        /// <summary>
        /// Highlights a control to draw attention.
        /// </summary>
        public static void HighlightControl(Control ctrl, bool highlight = true)
        {
            if (ctrl == null) return;

            try
            {
                if (highlight)
                {
                    ctrl.BackColor = Color.FromArgb(255, 250, 205); // Light yellow
                }
                else
                {
                    ctrl.BackColor = Colors.Surface;
                }
            }
            catch { }
        }

        /// <summary>
        /// Shows validation error on a control.
        /// </summary>
        public static void ShowValidationError(Control ctrl, string message)
        {
            if (ctrl == null) return;

            try
            {
                ctrl.BackColor = Color.FromArgb(254, 226, 226); // Light red
                var errorProvider = new ErrorProvider();
                errorProvider.SetError(ctrl, message);
            }
            catch { }
        }

        /// <summary>
        /// Clears validation error from a control.
        /// </summary>
        public static void ClearValidationError(Control ctrl)
        {
            if (ctrl == null) return;

            try
            {
                ctrl.BackColor = Colors.Surface;
                var errorProvider = new ErrorProvider();
                errorProvider.SetError(ctrl, "");
            }
            catch { }
        }

        /// <summary>
        /// Formats a number as currency.
        /// </summary>
        public static string FormatCurrency(decimal amount)
        {
            return amount.ToString("N0") + " VNĐ";
        }

        /// <summary>
        /// Formats a date for display.
        /// </summary>
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Formats a datetime for display.
        /// </summary>
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Enables or disables all controls in a container.
        /// </summary>
        public static void EnableControls(Control container, bool enable)
        {
            if (container == null) return;

            try
            {
                foreach (Control ctrl in container.Controls)
                {
                    ctrl.Enabled = enable;
                    if (ctrl.HasChildren)
                    {
                        EnableControls(ctrl, enable);
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Clears all text inputs in a container.
        /// </summary>
        public static void ClearInputs(Control container)
        {
            if (container == null) return;

            try
            {
                foreach (Control ctrl in container.Controls)
                {
                    if (ctrl is TextBox txt)
                        txt.Clear();
                    else if (ctrl is ComboBox cmb)
                        cmb.SelectedIndex = -1;
                    else if (ctrl is CheckBox chk)
                        chk.Checked = false;
                    else if (ctrl.HasChildren)
                        ClearInputs(ctrl);
                }
            }
            catch { }
        }

        /// <summary>
        /// Centers a form on screen.
        /// </summary>
        public static void CenterOnScreen(Form form)
        {
            if (form == null) return;

            try
            {
                Screen screen = Screen.FromControl(form);
                form.Location = new Point(
                    (screen.WorkingArea.Width - form.Width) / 2 + screen.WorkingArea.Left,
                    (screen.WorkingArea.Height - form.Height) / 2 + screen.WorkingArea.Top);
            }
            catch { }
        }

        /// <summary>
        /// Hiển thị SaveFileDialog một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowSaveFileDialog(Form owner, SaveFileDialog dialog)
        {
            try
            {
                // Tạm thời vô hiệu hóa KeyPreview của form chính
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null)
                    owner.KeyPreview = false;

                try
                {
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    // Khôi phục KeyPreview
                    if (owner != null)
                        owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Hiển thị OpenFileDialog một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowOpenFileDialog(Form owner, OpenFileDialog dialog)
        {
            try
            {
                // Tạm thời vô hiệu hóa KeyPreview của form chính
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null)
                    owner.KeyPreview = false;

                try
                {
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    // Khôi phục KeyPreview
                    if (owner != null)
                        owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Hiển thị FolderBrowserDialog một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowFolderBrowserDialog(Form owner, FolderBrowserDialog dialog)
        {
            try
            {
                // Tạm thời vô hiệu hóa KeyPreview của form chính
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null)
                    owner.KeyPreview = false;

                try
                {
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    // Khôi phục KeyPreview
                    if (owner != null)
                        owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Hiển thị MessageBox một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowMessageBox(Form owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            try
            {
                // Tạm thời vô hiệu hóa KeyPreview của form chính
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null)
                    owner.KeyPreview = false;

                try
                {
                    return MessageBox.Show(owner, text, caption, buttons, icon);
                }
                finally
                {
                    // Khôi phục KeyPreview
                    if (owner != null)
                        owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Hiển thị form dialog một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowFormDialog(Form owner, Form dialog)
        {
            try
            {
                // Tạm thời vô hiệu hóa KeyPreview của form chính
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null)
                    owner.KeyPreview = false;

                try
                {
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    // Khôi phục KeyPreview
                    if (owner != null)
                        owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Hiển thị PrintDialog một cách an toàn (tránh vấn đề dialog hiển thị lại)
        /// </summary>
        public static DialogResult ShowPrintDialog(Form owner, PrintDialog dialog)
        {
            try
            {
                bool originalKeyPreview = owner?.KeyPreview ?? false;
                if (owner != null) owner.KeyPreview = false;
                try
                {
                    return dialog.ShowDialog(owner);
                }
                finally
                {
                    if (owner != null) owner.KeyPreview = originalKeyPreview;
                }
            }
            catch
            {
                return DialogResult.Cancel;
            }
        }
    }
}

