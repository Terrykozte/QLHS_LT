using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Globalization;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryList : BaseForm
    {
        private CategoryBLL _categoryBLL;
        private List<CategoryDTO> _allData = new List<CategoryDTO>();
        private Label _lblEmptyState;
        private ListEnhancer _listEnhancer;
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        public FormCategoryList()
        {
            InitializeComponent();
            _categoryBLL = new CategoryBLL();

            // UX
            this.KeyPreview = true;
            this.KeyDown += FormCategoryList_KeyDown;

            // Initialize ListEnhancer
            _listEnhancer = new ListEnhancer(dgvCategory, txtSearch, lblPageInfo, _pageSize);
            _listEnhancer.SetLoadDataCallback(page => { _currentPage = page; ApplyFilters(); });
            _listEnhancer.SetRefreshCallback(() => LoadData());
            _listEnhancer.OnColumnSort += (colName, ascending) => ApplySorting(colName, ascending);
            _listEnhancer.OnEditClick += OpenEditForSelectedRow;
            _listEnhancer.OnDeleteRequested += DeleteSingleCategory;
            _listEnhancer.OnBatchDeleteRequested += DeleteMultipleCategories;
            _listEnhancer.OnExportCurrentPage += ExportCurrentPage;
            _listEnhancer.OnExportSelected += ExportSelected;
            _listEnhancer.OnExportAll += ExportAll;
            _listEnhancer.OnAddNew += () => btnAdd_Click(null, EventArgs.Empty);
            _listEnhancer.OnCopySuccess += () => ShowInfo("✅ Đã sao chép!");

            // Events
            this.Load += FormCategoryList_Load;
        }

        private void FormCategoryList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                BuildEmptyState();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCategoryList_Load");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _categoryBLL.GetAll() ?? new List<CategoryDTO>();
                _currentPage = 1;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading category data: {ex.Message}");
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvCategory == null) return;

                dgvCategory.AutoGenerateColumns = false;
                dgvCategory.Columns.Clear();

                // Grid styling
                dgvCategory.EnableHeadersVisualStyles = false;
                dgvCategory.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = System.Drawing.Color.FromArgb(41, 128, 185),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Padding = new Padding(5),
                    WrapMode = DataGridViewTriState.False
                };
                dgvCategory.ColumnHeadersHeight = 45;
                dgvCategory.RowTemplate.Height = 38;
                dgvCategory.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 248, 250);
                dgvCategory.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(52, 152, 219);
                dgvCategory.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
                dgvCategory.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9);
                dgvCategory.DefaultCellStyle.Padding = new Padding(5);
                dgvCategory.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
                dgvCategory.AllowUserToAddRows = false;
                dgvCategory.AllowUserToDeleteRows = false;
                dgvCategory.AllowUserToResizeRows = false;
                dgvCategory.ReadOnly = false;
                dgvCategory.MultiSelect = false;
                dgvCategory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

                // Checkbox column
                var chkCol = new DataGridViewCheckBoxColumn { HeaderText = "✓", Width = 40, ReadOnly = false, Name = "colCheck", ThreeState = false };
                chkCol.TrueValue = true; chkCol.FalseValue = false;
                chkCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvCategory.Columns.Add(chkCol);

                // STT column
                var sttCol = new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "STT", Width = 50, ReadOnly = true };
                dgvCategory.Columns.Add(sttCol);

                dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryID", DataPropertyName = "CategoryID", HeaderText = "ID", Width = 60 });
                dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { Name = "CategoryName", DataPropertyName = "CategoryName", HeaderText = "TÊN DANH MỤC", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", DataPropertyName = "Description", HeaderText = "MÔ TẢ", Width = 240 });
                dgvCategory.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring columns: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            try
            {
                string keyword = _listEnhancer.GetSearchKeyword();
                string kwNorm = ListEnhancer.RemoveDiacritics(keyword).ToLowerInvariant();

                var filteredData = _allData.FindAll(x =>
                {
                    if (string.IsNullOrEmpty(kwNorm)) return true;
                    string name = ListEnhancer.RemoveDiacritics(x.CategoryName ?? string.Empty).ToLowerInvariant();
                    string desc = ListEnhancer.RemoveDiacritics(x.Description ?? string.Empty).ToLowerInvariant();
                    return name.Contains(kwNorm) || desc.Contains(kwNorm);
                });

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvCategory != null)
                {
                    dgvCategory.DataSource = pagedData;
                    UpdatePagination();
                    UpdateEmptyState(pagedData.Count == 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filters: {ex.Message}");
            }
        }

        private void UpdatePagination()
        {
            string pageInfo;
            if (_totalRecords == 0)
            {
                pageInfo = "Tong cong: 0 danh muc";
            }
            else
            {
                int from = (_currentPage - 1) * _pageSize + 1;
                int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                int totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);
                pageInfo = $"Hien thi {from} - {to} / {_totalRecords} danh muc | Trang {_currentPage}/{totalPages}";
            }
            _listEnhancer.UpdatePageInfoDisplay(pageInfo);
        }

        private void ApplySorting(string columnName, bool ascending)
        {
            try
            {
                if (_allData == null || _allData.Count == 0) return;

                switch (columnName)
                {
                    case "CategoryID":
                        _allData = ascending ? _allData.OrderBy(x => x.CategoryID).ToList() : _allData.OrderByDescending(x => x.CategoryID).ToList();
                        break;
                    case "CategoryName":
                        _allData = ascending ? _allData.OrderBy(x => x.CategoryName).ToList() : _allData.OrderByDescending(x => x.CategoryName).ToList();
                        break;
                    case "Description":
                        _allData = ascending ? _allData.OrderBy(x => x.Description).ToList() : _allData.OrderByDescending(x => x.Description).ToList();
                        break;
                    case "Status":
                        _allData = ascending ? _allData.OrderBy(x => x.Status).ToList() : _allData.OrderByDescending(x => x.Status).ToList();
                        break;
                }

                _currentPage = 1;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying sorting: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormCategoryAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
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

        private void OpenEditForSelectedRow()
        {
            try
            {
                if (dgvCategory?.CurrentRow?.DataBoundItem is CategoryDTO item)
                {
                    using (var form = new FormCategoryEdit(item))
                    {
                        if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                        {
                            LoadData();
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

        private bool DeleteSingleCategory(int categoryId)
        {
            try
            {
                var category = _allData.FirstOrDefault(x => x.CategoryID == categoryId);
                if (category == null) return false;

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa danh mục '{category.CategoryName}'?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return false;

                Wait(true);
                _categoryBLL.Delete(categoryId);
                ShowInfo("✅ Xóa danh mục thành công!");
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

        private bool DeleteMultipleCategories(List<int> categoryIds)
        {
            try
            {
                if (categoryIds == null || categoryIds.Count == 0) return false;

                if (!ShowConfirm($"🗑️ Bạn có chắc muốn xóa {categoryIds.Count} danh mục đã chọn?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa"))
                    return false;

                Wait(true);
                int ok = 0, fail = 0;
                foreach (var id in categoryIds)
                {
                    try { _categoryBLL.Delete(id); ok++; }
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

        private void ExportCurrentPage()
        {
            try
            {
                var current = dgvCategory?.DataSource as IEnumerable<CategoryDTO> ?? Enumerable.Empty<CategoryDTO>();
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
                var selected = _allData.Where(x => _listEnhancer.GetSelectedIds().Contains(x.CategoryID));
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

        private void DoExport(IEnumerable<CategoryDTO> list, string baseFileName)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"{baseFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        Wait(true);
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string> { "ID,Tên danh mục,Mô tả,Trạng thái" };
                            foreach (var c in list)
                            {
                                lines.Add($"{c.CategoryID},{ListEnhancer.EscapeCsv(c.CategoryName)},{ListEnhancer.EscapeCsv(c.Description)},{ListEnhancer.EscapeCsv(c.Status)}");
                            }
                            File.WriteAllLines(sfd.FileName, lines, Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><style>body{font-family:Segoe UI;margin:20px;background:#f9f9f9;} h3{color:#2980b9;border-bottom:2px solid #2980b9;padding-bottom:10px;} table{border-collapse:collapse;width:100%;background:white;box-shadow:0 2px 4px rgba(0,0,0,0.1);} th{background:#2980b9;color:white;padding:12px;text-align:left;font-weight:bold;} td{border:1px solid #ddd;padding:10px;} tr:nth-child(even){background:#f5f5f5;} tr:hover{background:#e8f4f8;transition:background 0.3s;} p{color:#666;font-size:12px;}</style></head><body>");
                            sb.AppendLine($"<h3>📋 Danh sách danh mục ({list.Count()} bản ghi)</h3>");
                            sb.AppendLine($"<p>Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6'>");
                            sb.AppendLine("<tr><th>ID</th><th>Tên danh mục</th><th>Mô tả</th><th>Trạng thái</th></tr>");
                            foreach (var c in list)
                            {
                                sb.AppendLine($"<tr><td>{c.CategoryID}</td><td>{ListEnhancer.HtmlEncode(c.CategoryName)}</td><td>{ListEnhancer.HtmlEncode(c.Description)}</td><td>{ListEnhancer.HtmlEncode(c.Status)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        }
                        Wait(false);
                        MessageBox.Show($"✅ Xuất danh sách danh mục thành công!\n\nFile: {Path.GetFileName(sfd.FileName)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Wait(false);
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildEmptyState()
        {
            _lblEmptyState = new Label
            {
                Text = "📭 Không có dữ liệu danh mục\n\nNhấn '➕ Thêm danh mục' để bắt đầu",
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

        private void FormCategoryList_KeyDown(object sender, KeyEventArgs e)
        {
            _listEnhancer.HandleFormKeyDown(e);
        }

        protected override void CleanupResources()
        {
            try
            {
                _listEnhancer?.Dispose();
                _listEnhancer = null;

                _allData?.Clear();
                _allData = null;

                _lblEmptyState?.Dispose();
                _lblEmptyState = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
