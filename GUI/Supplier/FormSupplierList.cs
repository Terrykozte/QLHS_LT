using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Supplier
{
    public partial class FormSupplierList : BaseForm
    {
        private SupplierBLL _bll = new SupplierBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private List<SupplierDTO> _allData = new List<SupplierDTO>();
        private Timer _searchDebounceTimer;

        public FormSupplierList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormSupplierList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvSuppliers != null) UIHelper.ApplyGridStyle(dgvSuppliers);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
                if (btnImport != null) UIHelper.ApplyGunaButtonStyle(btnImport, isPrimary: false);
                if (btnExport != null) UIHelper.ApplyGunaButtonStyle(btnExport, isPrimary: false);
                if (btnPrevious != null) UIHelper.ApplyGunaButtonStyle(btnPrevious, isPrimary: false);
                if (btnNext != null) UIHelper.ApplyGunaButtonStyle(btnNext, isPrimary: false);
            }
            catch { }

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); _currentPage = 1; LoadData(); };

            // Events
            this.Load += FormSupplierList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                txtSearch.KeyDown += txtSearch_KeyDown;
            }
            if (btnPrevious != null) btnPrevious.Click += btnPrevious_Click;
            if (btnNext != null) btnNext.Click += btnNext_Click;
            if (btnAdd != null) btnAdd.Click += btnAdd_Click;
            if (btnImport != null) btnImport.Click += btnImport_Click;
            if (btnExport != null) btnExport.Click += btnExport_Click;
        }

        private void FormSupplierList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSupplierList_Load");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvSuppliers == null) return;
                dgvSuppliers.AutoGenerateColumns = false;
                dgvSuppliers.Columns.Clear();

                // Selection
                var chkCol = new DataGridViewCheckBoxColumn { HeaderText = "", Width = 40, ReadOnly = true };
                dgvSuppliers.Columns.Add(chkCol);

                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierID", HeaderText = "ID", Width = 60 });
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SupplierName", HeaderText = "NHÀ CUNG CẤP", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ContactPerson", HeaderText = "NGƯỜI LIÊN HỆ", Width = 150 });
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhoneNumber", HeaderText = "SĐT", Width = 120 });
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "EMAIL", Width = 180 });
                dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Address", HeaderText = "ĐỊA CHỈ", Width = 200 });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ConfigureGrid");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _bll.GetAll() ?? new List<SupplierDTO>();

                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;
                var filtered = _allData.FindAll(x =>
                    string.IsNullOrEmpty(keyword) ||
                    (x.SupplierName?.ToLower().Contains(keyword) ?? false) ||
                    (x.PhoneNumber?.Contains(keyword) ?? false) ||
                    (x.Email?.ToLower().Contains(keyword) ?? false) ||
                    (x.Address?.ToLower().Contains(keyword) ?? false)
                );

                _totalRecords = filtered.Count;
                UpdatePagination();

                var paged = filtered
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvSuppliers != null) dgvSuppliers.DataSource = paged;
                if (lblPageInfo != null)
                {
                    int from = _totalRecords == 0 ? 0 : ((_currentPage - 1) * _pageSize + 1);
                    int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                    lblPageInfo.Text = $"Hiển thị {from}-{to}/{_totalRecords} nhà cung cấp";
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void UpdatePagination()
        {
            try
            {
                if (btnPrevious != null) btnPrevious.Enabled = _currentPage > 1;
                if (btnNext != null) btnNext.Enabled = _currentPage * _pageSize < _totalRecords;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Pagination error: {ex.Message}");
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1) { _currentPage--; LoadData(); }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords) { _currentPage++; LoadData(); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormSupplierAdd())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnAdd_Click");
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Import đang phát triển.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"NhaCungCap_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var list = dgvSuppliers?.DataSource as IEnumerable<SupplierDTO> ?? _allData ?? new List<SupplierDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("ID,NhaCungCap,NguoiLienHe,SDT,Email,DiaChi");
                            foreach (var s in list)
                            {
                                lines.Add($"{s.SupplierID},{EscapeCsv(s.SupplierName)},{EscapeCsv(s.ContactPerson)},{EscapeCsv(s.PhoneNumber)},{EscapeCsv(s.Email)},{EscapeCsv(s.Address)}");
                            }
                            System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                            sb.AppendLine("<h3>Danh sách nhà cung cấp</h3>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Segoe UI;font-size:11pt'>");
                            sb.AppendLine("<tr style='background:#e5e7eb'><th>ID</th><th>Nhà cung cấp</th><th>Người liên hệ</th><th>SĐT</th><th>Email</th><th>Địa chỉ</th></tr>");
                            foreach (var s in list)
                            {
                                sb.AppendLine($"<tr><td>{s.SupplierID}</td><td>{Html(s.SupplierName)}</td><td>{Html(s.ContactPerson)}</td><td>{Html(s.PhoneNumber)}</td><td>{Html(s.Email)}</td><td>{Html(s.Address)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        }
                        MessageBox.Show("Xuất danh sách nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnExport_Click");
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

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _currentPage = 1;
                LoadData();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.Handled = true;
            }
        }

        private void FormSupplierList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadData();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allData?.Clear();
                _bll = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
