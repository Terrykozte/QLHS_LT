using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using System.Diagnostics;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuList : BaseForm
    {
        private readonly MenuBLL _bll = new MenuBLL();
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();
        private Timer _searchDebounceTimer;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        // Context menu
        private ContextMenuStrip _rowMenu;

        // UI helpers
        private Label _lblEmptyState;
        private Label _lblLoadingIndicator;

        public FormMenuList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormMenuList_KeyDown;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            };

            // Events
            this.Load += FormMenuList_Load;
            this.Shown += (s, e) => { try { txtSearch?.Focus(); } catch { } };
            txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
            txtSearch.KeyDown += txtSearch_KeyDown;
            cboCategory.SelectedIndexChanged += (s, e) => { _currentPage = 1; ApplyFiltersAndPagination(); };
            btnAdd.Click += btnAdd_Click;
            btnReload.Click += btnReload_Click;
            dgvMenu.CellDoubleClick += dgvMenu_CellDoubleClick;
            btnNext.Click += BtnNext_Click;
            btnPrevious.Click += BtnPrevious_Click;
        }

        private void FormMenuList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                InitRowContextMenu();
                BuildEmptyState();
                BuildLoadingIndicator();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuList_Load");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                dgvMenu.AutoGenerateColumns = false;
                dgvMenu.Columns.Clear();
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemID", HeaderText = "ID", Width = 60 });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ItemName", HeaderText = "TÊN MÓN", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CategoryName", HeaderText = "DANH MỤC", Width = 150 });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "GIÁ", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
                dgvMenu.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
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
                ShowLoadingState(true);
                _allItems = _bll.GetAllItems() ?? new List<MenuItemDTO>();
                PopulateCategoryFilter();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { ShowLoadingState(false); Wait(false); }
        }

        private void PopulateCategoryFilter()
        {
            try
            {
                var categories = _allItems.Select(item => item.CategoryName).Distinct().Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                categories.Insert(0, "Tất cả");
                cboCategory.DataSource = categories;
                cboCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "PopulateCategoryFilter");
            }
        }

        private void ApplyFiltersAndPagination()
        {
            try
            {
                string selectedCategory = cboCategory?.SelectedItem?.ToString() ?? "Tất cả";
                string keyword = txtSearch?.Text?.Trim().ToLower() ?? string.Empty;

                var filtered = _allItems.AsEnumerable();

                if (selectedCategory != "Tất cả")
                    filtered = filtered.Where(item => string.Equals(item.CategoryName, selectedCategory, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(keyword))
                    filtered = filtered.Where(item => (item.ItemName?.ToLower().Contains(keyword) ?? false) || (item.Description?.ToLower().Contains(keyword) ?? false));

                var filteredList = filtered.ToList();
                _totalRecords = filteredList.Count;

                var pagedData = filteredList
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvMenu.DataSource = pagedData;
                UpdatePagination();
                UpdateEmptyState(_totalRecords == 0);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ApplyFiltersAndPagination");
            }
        }

        private void UpdatePagination()
        {
            btnPrevious.Enabled = _currentPage > 1;
            btnNext.Enabled = _currentPage * _pageSize < _totalRecords;

            if (_totalRecords == 0)
            {
                lblPageInfo.Text = "Không có dữ liệu";
            }
            else
            {
                int from = (_currentPage - 1) * _pageSize + 1;
                int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                lblPageInfo.Text = $"Hiển thị {from} - {to} / {_totalRecords} món";
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFiltersAndPagination();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.SuppressKeyPress = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormMenuEdit(0)) // 0 => new item
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
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

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearch?.Clear();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "btnReload_Click");
            }
        }

        private void dgvMenu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvMenu?.CurrentRow != null)
                {
                    var selectedItem = dgvMenu.CurrentRow.DataBoundItem as MenuItemDTO;
                    if (selectedItem == null) return;
                    using (var form = new FormMenuEdit(selectedItem.ItemID))
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
                ExceptionHandler.Handle(ex, "dgvMenu_CellDoubleClick");
            }
        }

        private void FormMenuList_KeyDown(object sender, KeyEventArgs e)
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
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    ExportMenu();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvMenu?.CurrentRow != null)
                {
                    dgvMenu_CellDoubleClick(sender, new DataGridViewCellEventArgs(0, dgvMenu.CurrentRow.Index));
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuList_KeyDown");
            }
        }

        private void ExportMenu()
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"ThucDon_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var list = dgvMenu?.DataSource as IEnumerable<MenuItemDTO> ?? _allItems ?? new List<MenuItemDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("ID,TenMon,DanhMuc,Gia,MoTa,TrangThai");
                            foreach (var m in list)
                            {
                                lines.Add($"{m.ItemID},{EscapeCsv(m.ItemName)},{EscapeCsv(m.CategoryName)},{m.UnitPrice},{EscapeCsv(m.Description)},{(m.IsAvailable ? "Available" : "Unavailable")}");
                            }
                            System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                            sb.AppendLine("<h3>Danh sách thực đơn</h3>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Segoe UI;font-size:11pt'>");
                            sb.AppendLine("<tr style='background:#e5e7eb'><th>ID</th><th>Tên món</th><th>Danh mục</th><th>Giá</th><th>Mô tả</th><th>Trạng thái</th></tr>");
                            foreach (var m in list)
                            {
                                sb.AppendLine($"<tr><td>{m.ItemID}</td><td>{Html(m.ItemName)}</td><td>{Html(m.CategoryName)}</td><td align='right'>{m.UnitPrice:N0}</td><td>{Html(m.Description)}</td><td>{(m.IsAvailable ? "Available" : "Unavailable")}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        }
                        MessageBox.Show("Xuất thực đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void InitRowContextMenu()
        {
            try
            {
                if (dgvMenu == null) return;

                _rowMenu = new ContextMenuStrip();

                var itemEdit = new ToolStripMenuItem("Sửa (Enter)", null, (s, e) =>
                {
                    if (dgvMenu.CurrentRow != null)
                    {
                        dgvMenu_CellDoubleClick(dgvMenu, new DataGridViewCellEventArgs(0, dgvMenu.CurrentRow.Index));
                    }
                });

                var itemDelete = new ToolStripMenuItem("Xóa (Delete)", null, (s, e) =>
                {
                    try
                    {
                        if (dgvMenu.CurrentRow != null)
                        {
                            var selectedItem = dgvMenu.CurrentRow.DataBoundItem as MenuItemDTO;
                            if (selectedItem != null)
                            {
                                if (MessageBox.Show($"Xóa món '{selectedItem.ItemName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    try
                                    {
                                        _bll.DeleteItem(selectedItem.ItemID);
                                        LoadData();
                                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    catch (Exception deleteEx)
                                    {
                                        MessageBox.Show($"Xóa thất bại: {deleteEx.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Handle(ex, "Delete menu item");
                    }
                });

                var itemRefresh = new ToolStripMenuItem("Làm mới (F5)", null, (s, e) => LoadData());

                _rowMenu.Items.Add(itemEdit);
                _rowMenu.Items.Add(itemDelete);
                _rowMenu.Items.Add(new ToolStripSeparator());
                _rowMenu.Items.Add(itemRefresh);

                dgvMenu.ContextMenuStrip = _rowMenu;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "InitRowContextMenu");
            }
        }

        private void BtnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void BuildEmptyState()
        {
            try
            {
                _lblEmptyState = new Label
                {
                    Text = "📭 Không có dữ liệu thực đơn",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.FromArgb(150, 150, 150),
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    Visible = false,
                    BackColor = Color.FromArgb(250, 250, 250)
                };
                this.Controls.Add(_lblEmptyState);
                _lblEmptyState.BringToFront();
            }
            catch { }
        }

        private void BuildLoadingIndicator()
        {
            try
            {
                _lblLoadingIndicator = new Label
                {
                    Text = "⏳ Đang tải dữ liệu...",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    ForeColor = Color.FromArgb(52, 152, 219),
                    Font = new Font("Segoe UI", 11, FontStyle.Regular),
                    BackColor = Color.FromArgb(240, 248, 255),
                    Visible = false
                };
                this.Controls.Add(_lblLoadingIndicator);
                _lblLoadingIndicator.BringToFront();
            }
            catch { }
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            try { if (_lblEmptyState != null) _lblEmptyState.Visible = isEmpty; } catch { }
        }

        private void ShowLoadingState(bool isLoading)
        {
            try { if (_lblLoadingIndicator != null) _lblLoadingIndicator.Visible = isLoading; } catch { }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = null;

                _allItems?.Clear();
                _allItems = null;

                _rowMenu?.Dispose();
                _rowMenu = null;

                _lblEmptyState?.Dispose();
                _lblEmptyState = null;

                _lblLoadingIndicator?.Dispose();
                _lblLoadingIndicator = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CleanupResources: {ex.Message}");
            }
            finally { base.CleanupResources(); }
        }
    }
}
