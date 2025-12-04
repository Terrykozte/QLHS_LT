using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodList : BaseForm
    {
        private SeafoodBLL _seafoodBLL;
        private Timer _searchDebounceTimer;
        private List<SeafoodDTO> _allData = new List<SeafoodDTO>();

        public FormSeafoodList()
        {
            InitializeComponent();

            // Init services
            _seafoodBLL = new SeafoodBLL();

            // Improve UX
            this.KeyPreview = true;
            this.KeyDown += FormSeafoodList_KeyDown;

            // Debounce search timer
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                ApplyFilters();
            };

            // Styling
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvSeafood != null) UIHelper.ApplyGridStyle(dgvSeafood);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
            }
            catch { }

            // Events
            this.Load += FormSeafoodList_Load;
            this.Shown += (s, e) => { try { txtSearch?.Focus(); } catch { } };
            if (txtSearch != null) txtSearch.TextChanged += txtSearch_TextChanged;
            if (dgvSeafood != null) dgvSeafood.CellDoubleClick += dgvSeafood_CellDoubleClick;
        }

        private void FormSeafoodList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSeafoodList_Load");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);

                _allData = _seafoodBLL.GetAll() ?? new List<SeafoodDTO>();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading seafood data: {ex.Message}");
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
            }
        }

        private void ApplyFilters()
        {
            try
            {
                var list = _allData;

                // Apply search filter
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                if (!string.IsNullOrEmpty(keyword))
                {
                    list = list.Where(s =>
                        (s.SeafoodName?.ToLower().Contains(keyword) ?? false) ||
                        (s.CategoryName?.ToLower().Contains(keyword) ?? false) ||
                        (s.Unit?.ToLower().Contains(keyword) ?? false)
                    ).ToList();
                }

                if (dgvSeafood != null)
                {
                    dgvSeafood.DataSource = list;
                    ConfigureColumns();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void ConfigureColumns()
        {
            try
            {
                if (dgvSeafood == null) return;

                // Auto-generate off when we control columns
                if (dgvSeafood.AutoGenerateColumns)
                {
                    dgvSeafood.AutoGenerateColumns = false;
                    dgvSeafood.Columns.Clear();
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodID", HeaderText = "ID", Width = 60 });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "Tên hải sản", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "Danh mục", Width = 160 });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Unit", HeaderText = "Đơn vị", Width = 80 });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Số lượng", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "Đơn giá", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
                    dgvSeafood.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Trạng thái", Width = 120 });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring columns: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormSeafoodAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening add form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvSeafood_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvSeafood?.Rows.Count > e.RowIndex)
                {
                    var item = dgvSeafood.Rows[e.RowIndex].DataBoundItem as SeafoodDTO;
                    if (item != null)
                    {
                        using (var form = new FormSeafoodEdit(item))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                LoadData();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening edit form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form sửa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            // Debounce search to avoid excessive filtering
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void FormSeafoodList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadData();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvSeafood?.CurrentRow != null)
                {
                    // Quick open edit
                    var item = dgvSeafood.CurrentRow.DataBoundItem as SeafoodDTO;
                    if (item != null)
                    {
                        using (var form = new FormSeafoodEdit(item))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                                LoadData();
                        }
                        e.Handled = true;
                    }
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    ExportSeafood();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        private void ExportSeafood()
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"HaiSan_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var list = dgvSeafood?.DataSource as IEnumerable<SeafoodDTO> ?? _allData ?? new List<SeafoodDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("ID,Ten,Danhmuc,Donvi,Soluong,Dongia,Trangthai");
                            foreach (var s in list)
                            {
                                lines.Add($"{s.SeafoodID},{EscapeCsv(s.SeafoodName)},{EscapeCsv(s.CategoryName)},{EscapeCsv(s.Unit)},{s.Quantity},{s.UnitPrice},{EscapeCsv(s.Status)}");
                            }
                            System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                            sb.AppendLine("<h3>Danh sách hải sản</h3>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Segoe UI;font-size:11pt'>");
                            sb.AppendLine("<tr style='background:#e5e7eb'><th>ID</th><th>Tên</th><th>Danh mục</th><th>Đơn vị</th><th>Số lượng</th><th>Đơn giá</th><th>Trạng thái</th></tr>");
                            foreach (var s in list)
                            {
                                sb.AppendLine($"<tr><td>{s.SeafoodID}</td><td>{Html(s.SeafoodName)}</td><td>{Html(s.CategoryName)}</td><td>{Html(s.Unit)}</td><td align='right'>{s.Quantity}</td><td align='right'>{s.UnitPrice:N0}</td><td>{Html(s.Status)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        }
                        MessageBox.Show("Xuất danh sách hải sản thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var v = s.Replace("\"", "\"\"");
            if (v.Contains(",") || v.Contains("\n") || v.Contains("\r")) v = "\"" + v + "\"";
            return v;
        }
        private static string Html(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new System.Text.StringBuilder(input);
            sb.Replace("&", "&amp;"); sb.Replace("<", "&lt;"); sb.Replace(">", "&gt;"); sb.Replace("\"", "&quot;"); sb.Replace("'", "&#39;");
            return sb.ToString();
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allData?.Clear();
                _seafoodBLL = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
