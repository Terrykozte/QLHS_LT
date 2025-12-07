using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Globalization;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Supplier
{
    public partial class FormSupplierList : BaseForm
    {
        private readonly SupplierBLL _bll = new SupplierBLL();
        private List<SupplierDTO> _allData = new List<SupplierDTO>();
        private Label _lblEmptyState;
        private ListEnhancer _listEnhancer;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        public FormSupplierList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormSupplierList_KeyDown;

            // Initialize ListEnhancer
            _listEnhancer = new ListEnhancer(dgvSuppliers, txtSearch, lblPageInfo, _pageSize);
            _listEnhancer.SetLoadDataCallback(page => { _currentPage = page; ApplyFiltersAndPagination(); });
            _listEnhancer.SetRefreshCallback(() => LoadData());
            _listEnhancer.OnColumnSort += (colName, ascending) => ApplySorting(colName, ascending);
            _listEnhancer.OnEditClick += OpenEditForSelectedRow;
            _listEnhancer.OnDeleteRequested += DeleteSingleSupplier;
            _listEnhancer.OnBatchDeleteRequested += DeleteMultipleSuppliers;
            _listEnhancer.OnExportCurrentPage += ExportCurrentPage;
            _listEnhancer.OnExportSelected += ExportSelected;
            _listEnhancer.OnExportAll += ExportAll;
            _listEnhancer.OnAddNew += () => btnAdd_Click(null, EventArgs.Empty);
            _listEnhancer.OnImport += () => btnImport_Click(null, EventArgs.Empty);
            _listEnhancer.OnCopySuccess += () => ShowInfo("✅ Đã sao chép thông tin!");

            // Events
            this.Load += FormSupplierList_Load;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            btnAdd.Click += btnAdd_Click;
            btnImport.Click += btnImport_Click;
            btnExport.Click += btnExport_Click;
            dgvSuppliers.CellDoubleClick += (s, e) => OpenEditForSelectedRow();
        }

        private void FormSupplierList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                BuildEmptyState();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSupplierList_Load");
            }
        }

        private void ConfigureGrid()
        {
            dgvSuppliers.AutoGenerateColumns = false;
            dgvSuppliers.Columns.Clear();

            // Grid styling
            dgvSuppliers.EnableHeadersVisualStyles = false;
            dgvSuppliers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = System.Drawing.Color.FromArgb(41, 128, 185),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5),
                WrapMode = DataGridViewTriState.False
            };
            dgvSuppliers.ColumnHeadersHeight = 45;
            dgvSuppliers.RowTemplate.Height = 38;
            dgvSuppliers.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 250);
            dgvSuppliers.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            dgvSuppliers.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            dgvSuppliers.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9);
            dgvSuppliers.DefaultCellStyle.Padding = new Padding(5);
            dgvSuppliers.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
            dgvSuppliers.AllowUserToAddRows = false;
            dgvSuppliers.AllowUserToDeleteRows = false;
            dgvSuppliers.AllowUserToResizeRows = false;
            dgvSuppliers.ReadOnly = false;
            dgvSuppliers.MultiSelect = false;
            dgvSuppliers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            var chkCol = new DataGridViewCheckBoxColumn { HeaderText = "✓", Width = 40, ReadOnly = false, Name = "colCheck", ThreeState = false };
            chkCol.TrueValue = true; chkCol.FalseValue = false;
            chkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSuppliers.Columns.Add(chkCol);

            // STT column
            var sttCol = new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", Width = 50, ReadOnly = true };
            dgvSuppliers.Columns.Add(sttCol);

            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "SupplierID", DataPropertyName = "SupplierID", HeaderText = "ID", Width = 60 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "SupplierName", DataPropertyName = "SupplierName", HeaderText = "NHÀ CUNG CẤP", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "ContactPerson", DataPropertyName = "ContactPerson", HeaderText = "NGƯỜI LIÊN HỆ", Width = 150 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhoneNumber", DataPropertyName = "PhoneNumber", HeaderText = "SĐT", Width = 120 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", DataPropertyName = "Email", HeaderText = "EMAIL", Width = 180 });
            dgvSuppliers.Columns.Add(new DataGridViewTextBoxColumn { Name = "Address", DataPropertyName = "Address", HeaderText = "ĐỊA CHỈ", Width = 200 });
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _bll.GetAll() ?? new List<SupplierDTO>();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void ApplyFiltersAndPagination()
        {
            try
            {
                string keyword = _listEnhancer.GetSearchKeyword();
                string kwNorm = ListEnhancer.RemoveDiacritics(keyword).ToLowerInvariant();

                var filteredData = _allData.FindAll(x =>
                {
                    if (string.IsNullOrEmpty(kwNorm)) return true;
                    string name = ListEnhancer.RemoveDiacritics(x.SupplierName ?? string.Empty).ToLowerInvariant();
                    string phone = ListEnhancer.RemoveDiacritics(x.PhoneNumber ?? string.Empty).ToLowerInvariant();
                    string email = ListEnhancer.RemoveDiacritics(x.Email ?? string.Empty).ToLowerInvariant();
                    string addr = ListEnhancer.RemoveDiacritics(x.Address ?? string.Empty).ToLowerInvariant();
                    return name.Contains(kwNorm) || phone.Contains(kwNorm) || email.Contains(kwNorm) || addr.Contains(kwNorm);
                });

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvSuppliers.DataSource = pagedData;
                UpdatePagination();
                UpdateEmptyState(pagedData.Count == 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void UpdatePagination()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage * _pageSize < _totalRecords;

            string pageInfo;
            if (_totalRecords == 0)
            {
                pageInfo = "📊 Tổng cộng: 0 nhà cung cấp";
            }
            else
            {
                int from = (_currentPage - 1) * _pageSize + 1;
                int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                int totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                pageInfo = $"📊 Hiển thị {from} - {to} / {_totalRecords} nhà cung cấp | Trang {_currentPage}/{totalPages}";
            }
            _listEnhancer.UpdatePageInfoDisplay(pageInfo);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1) { _currentPage--; ApplyFiltersAndPagination(); }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords) { _currentPage++; ApplyFiltersAndPagination(); }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormSupplierAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "btnAdd_Click"); }
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
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var list = dgvSuppliers?.DataSource as IEnumerable<SupplierDTO> ?? _allData ?? new List<SupplierDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string> { "ID,NhaCungCap,NguoiLienHe,SDT,Email,DiaChi" };
                            lines.AddRange(list.Select(s => $"{s.SupplierID},{EscapeCsv(s.SupplierName)},{EscapeCsv(s.ContactPerson)},{EscapeCsv(s.PhoneNumber)},{EscapeCsv(s.Email)},{EscapeCsv(s.Address)}"));
                            File.WriteAllLines(sfd.FileName, lines, Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                            sb.AppendLine("<h3>Danh sách nhà cung cấp</h3>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Segoe UI;font-size:11pt'>");
                            sb.AppendLine("<tr style='background:#e5e7eb'><th>ID</th><th>Nhà cung cấp</th><th>Người liên hệ</th><th>SĐT</th><th>Email</th><th>Địa chỉ</th></tr>");
                            foreach (var s in list)
                            {
                                sb.AppendLine($"<tr><td>{s.SupplierID}</td><td>{Html(s.SupplierName)}</td><td>{Html(s.ContactPerson)}</td><td>{Html(s.PhoneNumber)}</td><td>{Html(s.Email)}</td><td>{Html(s.Address)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        }
                        MessageBox.Show("Xuất danh sách nhà cung cấp thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "btnExport_Click"); }
        }

        private static string EscapeCsv(string s) => string.IsNullOrEmpty(s) ? string.Empty : (s.Contains(",") || s.Contains("\n") || s.Contains("\r")) ? '"' + s.Replace("\"", "\"\"") + '"' : s;
        private static string Html(string i) => string.IsNullOrEmpty(i) ? string.Empty : new StringBuilder(i).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&#39;").ToString();

        private void ApplySorting(string columnName, bool ascending)
        {
            try
            {
                if (_allData == null || _allData.Count == 0) return;

                switch (columnName)
                {
                    case "SupplierID":
                        _allData = ascending ? _allData.OrderBy(x => x.SupplierID).ToList() : _allData.OrderByDescending(x => x.SupplierID).ToList();
                        break;
                    case "SupplierName":
                        _allData = ascending ? _allData.OrderBy(x => x.SupplierName).ToList() : _allData.OrderByDescending(x => x.SupplierName).ToList();
                        break;
                    case "ContactPerson":
                        _allData = ascending ? _allData.OrderBy(x => x.ContactPerson).ToList() : _allData.OrderByDescending(x => x.ContactPerson).ToList();
                        break;
                    case "PhoneNumber":
                        _allData = ascending ? _allData.OrderBy(x => x.PhoneNumber).ToList() : _allData.OrderByDescending(x => x.PhoneNumber).ToList();
                        break;
                    case "Email":
                        _allData = ascending ? _allData.OrderBy(x => x.Email).ToList() : _allData.OrderByDescending(x => x.Email).ToList();
                        break;
                    case "Address":
                        _allData = ascending ? _allData.OrderBy(x => x.Address).ToList() : _allData.OrderByDescending(x => x.Address).ToList();
                        break;
                }

                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying sorting: {ex.Message}");
            }
        }

        private void BuildEmptyState()
        {
            _lblEmptyState = new Label
            {
                Text = "📭 Không có dữ liệu nhà cung cấp",
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = System.Drawing.Color.Gray,
                Font = new System.Drawing.Font("Segoe UI", 12),
                Visible = false
            };
            this.Controls.Add(_lblEmptyState);
            _lblEmptyState.BringToFront();
        }

        private void UpdateEmptyState(bool isEmpty) => _lblEmptyState.Visible = isEmpty;

        private void OpenEditForSelectedRow()
        {
            if (dgvSuppliers?.CurrentRow?.DataBoundItem is SupplierDTO item)
            {
                OpenEdit(item.SupplierID);
            }
        }

        private void OpenEdit(int supplierId)
        {
            try
            {
                using (var form = new FormSupplierEdit(supplierId))
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "OpenEdit"); }
        }

        private bool DeleteSingleSupplier(int supplierId)
        {
            try
            {
                var supplier = _allData.FirstOrDefault(x => x.SupplierID == supplierId);
                if (supplier == null) return false;

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa nhà cung cấp '{supplier.SupplierName}'?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return false;

                Wait(true);
                _bll.Delete(supplierId);
                ShowInfo("✅ Xóa nhà cung cấp thành công!");
                _listEnhancer.OnDeleteSuccess();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally { Wait(false); }
        }

        private bool DeleteMultipleSuppliers(List<int> supplierIds)
        {
            try
            {
                if (supplierIds == null || supplierIds.Count == 0) return false;

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa {supplierIds.Count} nhà cung cấp đã chọn?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return false;

                Wait(true);
                int ok = 0, fail = 0;
                foreach (var id in supplierIds)
                {
                    try { _bll.Delete(id); ok++; }
                    catch { fail++; }
                }
                ShowInfo($"✅ Đã xóa {ok} | ❌ Lỗi {fail}");
                _listEnhancer.OnBatchDeleteSuccess();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally { Wait(false); }
        }

        private void FormSupplierList_KeyDown(object sender, KeyEventArgs e)
        {
            _listEnhancer.HandleFormKeyDown(e);
        }

        private void ExportCurrentPage()
        {
            try
            {
                var current = dgvSuppliers?.DataSource as IEnumerable<SupplierDTO> ?? Enumerable.Empty<SupplierDTO>();
                DoExport(current, "TrangHienTai");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSelected()
        {
            try
            {
                var selected = _allData.Where(x => _listEnhancer.GetSelectedIds().Contains(x.SupplierID));
                if (!selected.Any())
                {
                    MessageBox.Show("⚠️ Chưa có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                DoExport(selected, "DaChon");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportAll()
        {
            try
            {
                DoExport(_allData, "TatCa");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _listEnhancer?.Dispose();
                _listEnhancer = null;

                _allData?.Clear();
                _allData = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally { base.CleanupResources(); }
        }
    }
}
