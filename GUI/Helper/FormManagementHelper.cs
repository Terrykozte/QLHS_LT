using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// Helper cho các form quản lý (List, Add, Edit, Delete)
    /// </summary>
    public static class FormManagementHelper
    {
        /// <summary>
        /// Tạo layout chuẩn cho form danh sách
        /// </summary>
        public static void CreateStandardListLayout(Form form, out Panel pnlSearch, out DataGridView dgvData, out Panel pnlButtons)
        {
            form.SuspendLayout();

            // Panel tìm kiếm
            pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10)
            };

            // DataGridView
            dgvData = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
            };
            ThemeHelper.ApplyDataGridViewStyle(dgvData);

            // Panel nút
            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10)
            };

            form.Controls.Add(dgvData);
            form.Controls.Add(pnlSearch);
            form.Controls.Add(pnlButtons);

            form.ResumeLayout(false);
            form.PerformLayout();
        }

        /// <summary>
        /// Tạo layout chuẩn cho form thêm/sửa
        /// </summary>
        public static void CreateStandardFormLayout(Form form, out Panel pnlForm, out Panel pnlButtons)
        {
            form.SuspendLayout();

            // Panel form
            pnlForm = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(20)
            };

            // Panel nút
            pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10)
            };

            form.Controls.Add(pnlForm);
            form.Controls.Add(pnlButtons);

            form.ResumeLayout(false);
            form.PerformLayout();
        }

        /// <summary>
        /// Tạo search bar
        /// </summary>
        public static TextBox CreateSearchBar(Panel parent, string placeholder = "Tìm kiếm...")
        {
            var txtSearch = new TextBox
            {
                Dock = DockStyle.Left,
                Width = 250,
                Text = placeholder,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextSecondaryColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtSearch.GotFocus += (s, e) =>
            {
                if (txtSearch.Text == placeholder)
                {
                    txtSearch.Text = "";
                    txtSearch.ForeColor = ThemeHelper.GetTextColor();
                }
            };

            txtSearch.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = placeholder;
                    txtSearch.ForeColor = ThemeHelper.GetTextSecondaryColor();
                }
            };

            parent.Controls.Add(txtSearch);
            return txtSearch;
        }

        /// <summary>
        /// Tạo filter combo box
        /// </summary>
        public static ComboBox CreateFilterCombo(Panel parent, string label, List<string> items)
        {
            var lbl = new Label
            {
                Text = label,
                Dock = DockStyle.Left,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10)
            };

            var combo = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 150,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            if (items != null)
                combo.Items.AddRange(items.ToArray());

            parent.Controls.Add(combo);
            parent.Controls.Add(lbl);

            return combo;
        }

        /// <summary>
        /// Tạo button chuẩn
        /// </summary>
        public static Button CreateStandardButton(Panel parent, string text, Color? backColor = null, DockStyle dock = DockStyle.Right)
        {
            var btn = new Button
            {
                Text = text,
                Dock = dock,
                Width = 100,
                Height = 40,
                BackColor = backColor ?? ThemeHelper.Colors.Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(5)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ThemeHelper.Colors.PrimaryDark;
            btn.FlatAppearance.MouseDownBackColor = ThemeHelper.Colors.PrimaryDark;

            parent.Controls.Add(btn);
            return btn;
        }

        /// <summary>
        /// Tạo form field (Label + TextBox)
        /// </summary>
        public static TextBox CreateFormField(Panel parent, string label, bool isMultiline = false, int height = 30)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 5)
            };

            var txt = new TextBox
            {
                Multiline = isMultiline,
                Height = height,
                Dock = DockStyle.Top,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 10)
            };

            parent.Controls.Add(txt);
            parent.Controls.Add(lbl);

            return txt;
        }

        /// <summary>
        /// Tạo form field (Label + ComboBox)
        /// </summary>
        public static ComboBox CreateFormCombo(Panel parent, string label, List<string> items = null)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 5)
            };

            var combo = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 10)
            };

            if (items != null)
                combo.Items.AddRange(items.ToArray());

            parent.Controls.Add(combo);
            parent.Controls.Add(lbl);

            return combo;
        }

        /// <summary>
        /// Tạo form field (Label + NumericUpDown)
        /// </summary>
        public static NumericUpDown CreateFormNumeric(Panel parent, string label, decimal min = 0, decimal max = 1000)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 5)
            };

            var numeric = new NumericUpDown
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                Minimum = min,
                Maximum = max,
                Margin = new Padding(0, 0, 0, 10)
            };

            parent.Controls.Add(numeric);
            parent.Controls.Add(lbl);

            return numeric;
        }

        /// <summary>
        /// Tạo form field (Label + DateTimePicker)
        /// </summary>
        public static DateTimePicker CreateFormDatePicker(Panel parent, string label)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Margin = new Padding(0, 10, 0, 5)
            };

            var dtp = new DateTimePicker
            {
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Margin = new Padding(0, 0, 0, 10)
            };

            parent.Controls.Add(dtp);
            parent.Controls.Add(lbl);

            return dtp;
        }

        /// <summary>
        /// Tạo form field (Label + CheckBox)
        /// </summary>
        public static CheckBox CreateFormCheckBox(Panel parent, string label)
        {
            var chk = new CheckBox
            {
                Text = label,
                Dock = DockStyle.Top,
                Height = 30,
                BackColor = Color.Transparent,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 10, 0, 10)
            };

            parent.Controls.Add(chk);

            return chk;
        }

        /// <summary>
        /// Hiển thị dialog xác nhận xóa
        /// </summary>
        public static DialogResult ShowDeleteConfirmation(string itemName = "mục này")
        {
            return MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa {itemName}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
        }

        /// <summary>
        /// Hiển thị thông báo thành công
        /// </summary>
        public static void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Hiển thị thông báo lỗi
        /// </summary>
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hiển thị thông báo cảnh báo
        /// </summary>
        public static void ShowWarningMessage(string message)
        {
            MessageBox.Show(message, "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Tạo status bar
        /// </summary>
        public static StatusStrip CreateStatusBar(Form form)
        {
            var statusBar = new StatusStrip
            {
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor()
            };

            var lblStatus = new ToolStripStatusLabel("Sẵn sàng")
            {
                ForeColor = ThemeHelper.GetTextColor()
            };

            statusBar.Items.Add(lblStatus);
            form.Controls.Add(statusBar);

            return statusBar;
        }

        /// <summary>
        /// Tạo progress bar
        /// </summary>
        public static ProgressBar CreateProgressBar(Panel parent, string label)
        {
            var lbl = new Label
            {
                Text = label,
                AutoSize = true,
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 10, 0, 5)
            };

            var progressBar = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 20,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.Colors.Primary,
                Margin = new Padding(0, 0, 0, 10)
            };

            parent.Controls.Add(progressBar);
            parent.Controls.Add(lbl);

            return progressBar;
        }

        /// <summary>
        /// Tạo tab control chuẩn
        /// </summary>
        public static TabControl CreateStandardTabControl(Form form)
        {
            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetBackgroundColor(),
                ForeColor = ThemeHelper.GetTextColor()
            };

            form.Controls.Add(tabControl);
            return tabControl;
        }

        /// <summary>
        /// Thêm tab page
        /// </summary>
        public static TabPage AddTabPage(TabControl tabControl, string text)
        {
            var tabPage = new TabPage(text)
            {
                BackColor = ThemeHelper.GetBackgroundColor(),
                ForeColor = ThemeHelper.GetTextColor()
            };

            tabControl.TabPages.Add(tabPage);
            return tabPage;
        }

        /// <summary>
        /// Tạo group box
        /// </summary>
        public static GroupBox CreateGroupBox(Panel parent, string text)
        {
            var groupBox = new GroupBox
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(10),
                Margin = new Padding(0, 10, 0, 10)
            };

            parent.Controls.Add(groupBox);
            return groupBox;
        }

        /// <summary>
        /// Tạo separator
        /// </summary>
        public static Panel CreateSeparator(Panel parent)
        {
            var separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = ThemeHelper.GetBorderColor(),
                Margin = new Padding(0, 10, 0, 10)
            };

            parent.Controls.Add(separator);
            return separator;
        }

        /// <summary>
        /// Áp dụng responsive design cho form
        /// </summary>
        public static void ApplyResponsiveDesign(Form form)
        {
            ResponsiveDesignHelper.ApplyResponsiveDesignToForm(form);
            form.Resize += (s, e) =>
            {
                ResponsiveDesignHelper.ApplyResponsiveDesignToControls(form);
            };
        }

        /// <summary>
        /// Áp dụng theme cho form
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            ThemeHelper.ApplyThemeToForm(form);
        }
    }
}

