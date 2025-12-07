using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// FormListHelper - Hỗ trợ chuẩn hóa các form danh sách
    /// - Cấu hình DataGridView chuẩn
    /// - Xử lý layout responsive
    /// - Quản lý pagination
    /// </summary>
    public static class FormListHelper
    {
        /// <summary>
        /// Cấu hình DataGridView với style chuẩn
        /// </summary>
        public static void ConfigureDataGridView(DataGridView dgv, int headerHeight = 45, int rowHeight = 38)
        {
            try
            {
                dgv.AutoGenerateColumns = false;
                dgv.EnableHeadersVisualStyles = false;
                dgv.AllowUserToAddRows = false;
                dgv.AllowUserToDeleteRows = false;
                dgv.AllowUserToResizeRows = false;
                dgv.ReadOnly = false;
                dgv.MultiSelect = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Header style
                dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(41, 128, 185),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.False
                };
                dgv.ColumnHeadersHeight = headerHeight;

                // Row style
                dgv.RowTemplate.Height = rowHeight;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 250);
                dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
                dgv.DefaultCellStyle.SelectionForeColor = Color.White;
                dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                dgv.DefaultCellStyle.Padding = new Padding(5);
                dgv.GridColor = Color.FromArgb(220, 220, 220);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error configuring DataGridView: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm cột checkbox cho multi-select
        /// </summary>
        public static void AddCheckBoxColumn(DataGridView dgv, string columnName = "colCheck", int width = 40)
        {
            try
            {
                var chkCol = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "✓",
                    Width = width,
                    ReadOnly = false,
                    Name = columnName,
                    ThreeState = false
                };
                chkCol.TrueValue = true;
                chkCol.FalseValue = false;
                chkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns.Add(chkCol);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding checkbox column: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm cột STT (số thứ tự)
        /// </summary>
        public static void AddRowNumberColumn(DataGridView dgv, string columnName = "colSTT", int width = 50)
        {
            try
            {
                var sttCol = new DataGridViewTextBoxColumn
                {
                    Name = columnName,
                    HeaderText = "STT",
                    Width = width,
                    ReadOnly = true
                };
                dgv.Columns.Add(sttCol);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding row number column: {ex.Message}");
            }
        }

        /// <summary>
        /// Thêm cột button hành động
        /// </summary>
        public static void AddActionButtonColumn(DataGridView dgv, string columnName, string buttonText, 
            Color backgroundColor, int width = 100)
        {
            try
            {
                var btnCol = new DataGridViewButtonColumn
                {
                    HeaderText = "THAO TÁC",
                    Text = buttonText,
                    UseColumnTextForButtonValue = true,
                    Name = columnName,
                    Width = width,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = backgroundColor,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Padding = new Padding(3)
                    }
                };
                dgv.Columns.Add(btnCol);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding action button column: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật số thứ tự trong cột STT
        /// </summary>
        public static void UpdateRowNumbers(DataGridView dgv, string columnName = "colSTT", int startIndex = 1)
        {
            try
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    dgv.Rows[i].Cells[columnName].Value = startIndex + i;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating row numbers: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy danh sách ID của các dòng được chọn
        /// </summary>
        public static List<int> GetSelectedIds(DataGridView dgv, string idColumnName)
        {
            var ids = new List<int>();
            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.Cells["colCheck"].Value != null && (bool)row.Cells["colCheck"].Value)
                    {
                        if (int.TryParse(row.Cells[idColumnName].Value?.ToString(), out int id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting selected IDs: {ex.Message}");
            }
            return ids;
        }

        /// <summary>
        /// Hiển thị trạng thái trống
        /// </summary>
        public static Label CreateEmptyStateLabel(Control parent, string message = "📭 Không có dữ liệu")
        {
            var lbl = new Label
            {
                Text = message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(150, 150, 150),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                Visible = false,
                BackColor = Color.FromArgb(250, 250, 250)
            };
            parent.Controls.Add(lbl);
            lbl.BringToFront();
            return lbl;
        }

        /// <summary>
        /// Hiển thị trạng thái đang tải
        /// </summary>
        public static Label CreateLoadingLabel(Control parent, string message = "⏳ Đang tải dữ liệu...")
        {
            var lbl = new Label
            {
                Text = message,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(52, 152, 219),
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                BackColor = Color.FromArgb(240, 248, 255),
                Visible = false
            };
            parent.Controls.Add(lbl);
            lbl.BringToFront();
            return lbl;
        }

        /// <summary>
        /// Tính toán thông tin pagination
        /// </summary>
        public static string GetPaginationInfo(int currentPage, int pageSize, int totalRecords, string keyword = "")
        {
            if (totalRecords == 0)
                return "📊 Tổng cộng: 0 bản ghi";

            int from = (currentPage - 1) * pageSize + 1;
            int to = Math.Min(currentPage * pageSize, totalRecords);
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            string searchInfo = string.IsNullOrEmpty(keyword) ? "" : $" (Tìm: '{keyword}')";

            return $"📊 Hiển thị {from} - {to} / {totalRecords} | Trang {currentPage}/{totalPages}{searchInfo}";
        }

        /// <summary>
        /// Cấu hình button phân trang
        /// </summary>
        public static void UpdatePaginationButtons(Button btnPrevious, Button btnNext, 
            int currentPage, int pageSize, int totalRecords, bool isLoading = false)
        {
            try
            {
                if (btnPrevious != null)
                    btnPrevious.Enabled = currentPage > 1 && !isLoading;

                if (btnNext != null)
                    btnNext.Enabled = currentPage * pageSize < totalRecords && !isLoading;
            }
            catch { }
        }

        /// <summary>
        /// Loại bỏ diacritics từ chuỗi (để tìm kiếm không dấu)
        /// </summary>
        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}

