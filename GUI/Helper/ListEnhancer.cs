using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Diagnostics;

namespace QLTN_LT.GUI.Helper
{
    /// <summary>
    /// ListEnhancer - Công cụ hỗ trợ chung cho tất cả form danh sách
    /// Cung cấp: debounce search, sort theo header, chọn nhiều, batch delete, export, context menu, phím tắt
    /// </summary>
    public class ListEnhancer
    {
        private DataGridView _dgv;
        private TextBox _txtSearch;
        private Label _lblPageInfo;
        private Timer _searchDebounceTimer;
        private HashSet<int> _selectedIds = new HashSet<int>();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private int _lastSortColumn = -1;
        private bool _sortAscending = true;
        private ContextMenuStrip _ctxMenu;
        private Action<int> _onLoadDataCallback;
        private Func<int, bool> _onDeleteCallback;
        private Func<List<int>, bool> _onBatchDeleteCallback;
        private Action _onRefreshCallback;

        public ListEnhancer(DataGridView dgv, TextBox txtSearch, Label lblPageInfo, int pageSize = 10)
        {
            _dgv = dgv ?? throw new ArgumentNullException(nameof(dgv));
            _txtSearch = txtSearch;
            _lblPageInfo = lblPageInfo;
            _pageSize = pageSize;

            InitializeSearchDebounce();
            InitializeGridEvents();
            BuildContextMenu();
        }

        #region Search & Debounce

        private void InitializeSearchDebounce()
        {
            if (_txtSearch == null) return;

            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                _onLoadDataCallback?.Invoke(_currentPage);
            };

            _txtSearch.TextChanged += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            };

            _txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    _searchDebounceTimer.Stop();
                    _currentPage = 1;
                    _onLoadDataCallback?.Invoke(_currentPage);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    _txtSearch.Clear();
                    e.Handled = true;
                }
            };

            _txtSearch.Enter += (s, e) => { _txtSearch.BackColor = Color.FromArgb(230, 245, 255); };
            _txtSearch.Leave += (s, e) => { _txtSearch.BackColor = Color.White; };
        }

        public string GetSearchKeyword()
        {
            return _txtSearch?.Text?.Trim() ?? string.Empty;
        }

        #endregion

        #region Grid Events & Sorting

        private void InitializeGridEvents()
        {
            if (_dgv == null) return;

            _dgv.ColumnHeaderMouseClick += DgvColumnHeaderMouseClick;
            _dgv.KeyDown += DgvKeyDown;
            _dgv.MouseDown += DgvMouseDown;
            _dgv.CurrentCellDirtyStateChanged += DgvCurrentCellDirtyStateChanged;
            _dgv.CellValueChanged += DgvCellValueChanged;
            _dgv.DataBindingComplete += DgvDataBindingComplete;
        }

        private void DgvColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.ColumnIndex >= _dgv.Columns.Count) return;

                string columnName = _dgv.Columns[e.ColumnIndex].Name;

                // Handle select-all when clicking checkbox header
                if (columnName == "colCheck")
                {
                    ToggleSelectAllOnPage();
                    return;
                }

                // Skip action columns
                if (columnName.StartsWith("col") && !IsDataColumn(columnName)) return;

                // Toggle sort direction if same column clicked
                if (_lastSortColumn == e.ColumnIndex)
                {
                    _sortAscending = !_sortAscending;
                }
                else
                {
                    _lastSortColumn = e.ColumnIndex;
                    _sortAscending = true;
                }

                OnColumnSort?.Invoke(columnName, _sortAscending);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in column header click: {ex.Message}");
            }
        }

        private bool IsDataColumn(string columnName)
        {
            return !columnName.Equals("colCheck", StringComparison.OrdinalIgnoreCase) &&
                   !columnName.Equals("colSTT", StringComparison.OrdinalIgnoreCase) &&
                   !columnName.Equals("colEdit", StringComparison.OrdinalIgnoreCase) &&
                   !columnName.Equals("colDelete", StringComparison.OrdinalIgnoreCase) &&
                   !columnName.Equals("colAction", StringComparison.OrdinalIgnoreCase);
        }

        private void DgvCurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (_dgv.IsCurrentCellDirty)
                {
                    _dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch { }
        }

        private void DgvCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (_dgv.Columns[e.ColumnIndex].Name != "colCheck") return;

                var row = _dgv.Rows[e.RowIndex];
                int id = GetRowId(row);
                if (id <= 0) return;

                bool isChecked = Convert.ToBoolean(row.Cells["colCheck"].Value ?? false);
                if (isChecked) _selectedIds.Add(id); else _selectedIds.Remove(id);

                UpdatePageInfoFooter();
            }
            catch { }
        }

        private void DgvDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                int baseIndex = (_currentPage - 1) * _pageSize;
                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    // STT
                    if (_dgv.Columns.Contains("colSTT"))
                    {
                        row.Cells["colSTT"].Value = baseIndex + row.Index + 1;
                    }

                    // Checkbox state
                    int id = GetRowId(row);
                    if (id > 0 && _dgv.Columns.Contains("colCheck"))
                    {
                        row.Cells["colCheck"].Value = _selectedIds.Contains(id);
                    }
                }

                // Ensure only checkbox is editable
                foreach (DataGridViewColumn col in _dgv.Columns)
                {
                    col.ReadOnly = col.Name != "colCheck";
                }
            }
            catch { }
        }

        private void DgvMouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && _dgv != null)
                {
                    var hit = _dgv.HitTest(e.X, e.Y);
                    if (hit.RowIndex >= 0)
                    {
                        _dgv.ClearSelection();
                        _dgv.Rows[hit.RowIndex].Selected = true;
                        _ctxMenu?.Show(_dgv, new Point(e.X, e.Y));
                    }
                }
            }
            catch { }
        }

        private void DgvKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.Shift && e.KeyCode == Keys.A)
                {
                    SelectAllOnPage(false);
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    SelectAllOnPage(true);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Space && _dgv?.CurrentRow != null)
                {
                    var row = _dgv.CurrentRow;
                    if (row.Cells.Contains(row.Cells["colCheck"]))
                    {
                        var current = Convert.ToBoolean(row.Cells["colCheck"].Value ?? false);
                        bool next = !current;
                        row.Cells["colCheck"].Value = next;

                        int id = GetRowId(row);
                        if (id > 0)
                        {
                            if (next) _selectedIds.Add(id); else _selectedIds.Remove(id);
                            UpdatePageInfoFooter();
                        }
                    }
                    e.Handled = true;
                }
            }
            catch { }
        }

        #endregion

        #region Selection Management

        public int GetRowId(DataGridViewRow row)
        {
            try
            {
                // Try common ID column names
                foreach (var colName in new[] { "ID", "CustomerID", "SupplierID", "CategoryID", "SeafoodID", "MenuID", "InventoryID", "UserID", "OrderID" })
                {
                    if (row.Cells.Contains(colName))
                    {
                        var cellValue = row.Cells[colName].Value;
                        if (cellValue != null && int.TryParse(cellValue.ToString(), out int id))
                            return id;
                    }
                }
            }
            catch { }
            return 0;
        }

        private void ToggleSelectAllOnPage()
        {
            try
            {
                bool anyUnchecked = false;
                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    int id = GetRowId(row);
                    if (id <= 0) continue;
                    bool isChecked = Convert.ToBoolean(row.Cells["colCheck"].EditedFormattedValue ?? row.Cells["colCheck"].Value ?? false);
                    if (!isChecked) { anyUnchecked = true; break; }
                }

                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    int id = GetRowId(row);
                    if (id <= 0) continue;
                    bool target = anyUnchecked;
                    row.Cells["colCheck"].Value = target;
                    if (target) _selectedIds.Add(id); else _selectedIds.Remove(id);
                }

                UpdatePageInfoFooter();
            }
            catch { }
        }

        public void SelectAllOnPage(bool select)
        {
            try
            {
                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    int id = GetRowId(row);
                    if (id <= 0) continue;
                    row.Cells["colCheck"].Value = select;
                    if (select) _selectedIds.Add(id); else _selectedIds.Remove(id);
                }
                UpdatePageInfoFooter();
            }
            catch { }
        }

        public void SelectAllAll(bool select)
        {
            try
            {
                if (select)
                {
                    // Select all from current data
                    var allIds = GetAllDataIds();
                    _selectedIds = new HashSet<int>(allIds);
                }
                else
                {
                    _selectedIds.Clear();
                }
                // Refresh page checkboxes
                foreach (DataGridViewRow row in _dgv.Rows)
                {
                    int id = GetRowId(row);
                    if (id <= 0) continue;
                    row.Cells["colCheck"].Value = _selectedIds.Contains(id);
                }
                UpdatePageInfoFooter();
            }
            catch { }
        }

        public HashSet<int> GetSelectedIds()
        {
            return _selectedIds;
        }

        public void ClearSelection()
        {
            _selectedIds.Clear();
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                row.Cells["colCheck"].Value = false;
            }
            UpdatePageInfoFooter();
        }

        #endregion

        #region Pagination & Info

        public void SetPaginationInfo(int currentPage, int pageSize, int totalRecords)
        {
            _currentPage = currentPage;
            _pageSize = pageSize;
            _totalRecords = totalRecords;
            UpdatePageInfoFooter();
        }

        public void UpdatePageInfoFooter()
        {
            try
            {
                if (_lblPageInfo == null) return;

                int selected = _selectedIds?.Count ?? 0;
                string text = _lblPageInfo.Text;

                // Remove previous selection info
                int idx = text.IndexOf(" • Đã chọn");
                if (idx >= 0) text = text.Substring(0, idx);

                if (selected > 0) text += $" • Đã chọn: {selected}";
                _lblPageInfo.Text = text;
            }
            catch { }
        }

        public void UpdatePageInfoDisplay(string baseInfo)
        {
            try
            {
                if (_lblPageInfo == null) return;
                _lblPageInfo.Text = baseInfo;
                UpdatePageInfoFooter();
            }
            catch { }
        }

        #endregion

        #region Context Menu

        private void BuildContextMenu()
        {
            _ctxMenu = new ContextMenuStrip();

            var miEdit = new ToolStripMenuItem("✏️ Sửa");
            miEdit.Click += (s, e) => OnEditClick?.Invoke();

            var miDelete = new ToolStripMenuItem("🗑️ Xóa");
            miDelete.Click += (s, e) => DeleteSelected();

            var miCopy = new ToolStripMenuItem("📋 Sao chép");
            miCopy.Click += (s, e) => CopyRowInfo();

            // Select submenu
            var miSelect = new ToolStripMenuItem("✅ Chọn");
            var miSelectPage = new ToolStripMenuItem("Chọn tất cả (trang hiện tại)");
            miSelectPage.Click += (s, e) => SelectAllOnPage(true);
            var miDeselectPage = new ToolStripMenuItem("Bỏ chọn (trang hiện tại)");
            miDeselectPage.Click += (s, e) => SelectAllOnPage(false);
            var miSelectAll = new ToolStripMenuItem("Chọn tất cả (toàn bộ)");
            miSelectAll.Click += (s, e) => SelectAllAll(true);
            var miDeselectAll = new ToolStripMenuItem("Bỏ chọn tất cả");
            miDeselectAll.Click += (s, e) => SelectAllAll(false);
            miSelect.DropDownItems.AddRange(new ToolStripItem[] { miSelectPage, miDeselectPage, new ToolStripSeparator(), miSelectAll, miDeselectAll });

            // Export submenu
            var miExport = new ToolStripMenuItem("📤 Xuất");
            var miExportCurrent = new ToolStripMenuItem("Trang hiện tại (CSV/Excel)");
            miExportCurrent.Click += (s, e) => OnExportCurrentPage?.Invoke();
            var miExportSelected = new ToolStripMenuItem("Mục đã chọn (CSV/Excel)");
            miExportSelected.Click += (s, e) => OnExportSelected?.Invoke();
            var miExportAll = new ToolStripMenuItem("Tất cả (CSV/Excel)");
            miExportAll.Click += (s, e) => OnExportAll?.Invoke();
            miExport.DropDownItems.AddRange(new ToolStripItem[] { miExportCurrent, miExportSelected, miExportAll });

            var miRefresh = new ToolStripMenuItem("🔄 Làm mới");
            miRefresh.Click += (s, e) => _onRefreshCallback?.Invoke();

            _ctxMenu.Items.AddRange(new ToolStripItem[] { miEdit, miDelete, miCopy, miSelect, new ToolStripSeparator(), miExport, new ToolStripSeparator(), miRefresh });
        }

        public ContextMenuStrip GetContextMenu()
        {
            return _ctxMenu;
        }

        #endregion

        #region Delete Operations

        private void DeleteSelected()
        {
            try
            {
                // If multiple selected via checkbox
                if (_selectedIds != null && _selectedIds.Count > 0)
                {
                    OnBatchDeleteRequested?.Invoke(_selectedIds.ToList());
                    return;
                }

                // Otherwise: delete current row
                if (_dgv?.CurrentRow == null) return;
                int id = GetRowId(_dgv.CurrentRow);
                if (id <= 0) return;

                OnDeleteRequested?.Invoke(id);
            }
            catch { }
        }

        public void OnDeleteSuccess()
        {
            _selectedIds.Clear();
            _currentPage = 1;
            _onLoadDataCallback?.Invoke(_currentPage);
        }

        public void OnBatchDeleteSuccess()
        {
            _selectedIds.Clear();
            _currentPage = 1;
            _onLoadDataCallback?.Invoke(_currentPage);
        }

        #endregion

        #region Copy & Export

        private void CopyRowInfo()
        {
            try
            {
                if (_dgv?.CurrentRow == null) return;

                var row = _dgv.CurrentRow;
                var sb = new StringBuilder();

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex >= 0 && cell.ColumnIndex < _dgv.Columns.Count)
                    {
                        string colName = _dgv.Columns[cell.ColumnIndex].Name;
                        if (colName == "colCheck" || colName == "colSTT") continue;

                        string header = _dgv.Columns[cell.ColumnIndex].HeaderText;
                        string value = cell.Value?.ToString() ?? "";
                        sb.AppendLine($"{header}: {value}");
                    }
                }

                Clipboard.SetText(sb.ToString());
                OnCopySuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error copying: {ex.Message}");
            }
        }

        #endregion

        #region Keyboard Shortcuts

        public void HandleFormKeyDown(KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    _onRefreshCallback?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    OnAddNew?.Invoke();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && _dgv?.CurrentRow != null)
                {
                    OnEditClick?.Invoke();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelected();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    OnExportAll?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.I)
                {
                    OnImport?.Invoke();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.C)
                {
                    CopyRowInfo();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        #endregion

        #region Utility Methods

        public static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            var v = s.Replace("\"", "\"\"");
            if (v.Contains(",") || v.Contains("\n") || v.Contains("\r")) v = "\"" + v + "\"";
            return v;
        }

        public static string HtmlEncode(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input);
            sb.Replace("&", "&amp;");
            sb.Replace("<", "&lt;");
            sb.Replace(">", "&gt;");
            sb.Replace("\"", "&quot;");
            sb.Replace("'", "&#39;");
            return sb.ToString();
        }

        #endregion

        #region Callbacks & Events

        public Action<string, bool> OnColumnSort { get; set; }
        public Action OnEditClick { get; set; }
        public Action<int> OnDeleteRequested { get; set; }
        public Action<List<int>> OnBatchDeleteRequested { get; set; }
        public Action OnCopySuccess { get; set; }
        public Action OnExportCurrentPage { get; set; }
        public Action OnExportSelected { get; set; }
        public Action OnExportAll { get; set; }
        public Action OnAddNew { get; set; }
        public Action OnImport { get; set; }

        public void SetLoadDataCallback(Action<int> callback)
        {
            _onLoadDataCallback = callback;
        }

        public void SetRefreshCallback(Action callback)
        {
            _onRefreshCallback = callback;
        }

        public void SetDeleteCallback(Func<int, bool> callback)
        {
            _onDeleteCallback = callback;
        }

        public void SetBatchDeleteCallback(Func<List<int>, bool> callback)
        {
            _onBatchDeleteCallback = callback;
        }

        #endregion

        #region Data Helpers

        private List<int> GetAllDataIds()
        {
            var ids = new List<int>();
            foreach (DataGridViewRow row in _dgv.Rows)
            {
                int id = GetRowId(row);
                if (id > 0) ids.Add(id);
            }
            return ids;
        }

        #endregion

        #region Cleanup

        public void Dispose()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _ctxMenu?.Dispose();
                _selectedIds?.Clear();
            }
            catch { }
        }

        #endregion
    }
}

