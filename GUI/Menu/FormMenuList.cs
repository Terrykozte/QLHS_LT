using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuList : BaseForm
    {
        private readonly MenuBLL _bll = new MenuBLL();
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();
        private List<MenuItemDTO> _filteredItems = new List<MenuItemDTO>();
        private Timer _searchDebounceTimer;

        // Pagination
        private int _pageSize = 15;
        private int _currentPage = 1;

        // Context menu
        private ContextMenuStrip _rowMenu;

        public FormMenuList()
        {
            InitializeComponent();

            // UX & Styling
            this.KeyPreview = true;
            this.KeyDown += FormMenuList_KeyDown;
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvMenu != null) UIHelper.ApplyGridStyle(dgvMenu);
                if (btnAdd != null) UIHelper.ApplyGunaButtonStyle(btnAdd, isPrimary: true);
                if (btnReload != null) UIHelper.ApplyGunaButtonStyle(btnReload, isPrimary: false);
            }
            catch { }

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += (s, e) => { _searchDebounceTimer.Stop(); ApplyFilter(); };

            // Events
            this.Load += FormMenuList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                txtSearch.KeyDown += txtSearch_KeyDown;
            }
            if (cboCategory != null) cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;
            if (btnAdd != null) btnAdd.Click += btnAdd_Click;
            if (btnReload != null) btnReload.Click += btnReload_Click;
            if (dgvMenu != null) dgvMenu.CellDoubleClick += dgvMenu_CellDoubleClick;
        }

        private void FormMenuList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                InitRowContextMenu();
                if (btnNext != null) btnNext.Click += BtnNext_Click;
                if (btnPrevious != null) btnPrevious.Click += BtnPrevious_Click;

                LoadData();
                AddHelpButtonAndTooltips();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuList_Load");
            }
        }

        private void AddHelpButtonAndTooltips()
        {
            try
            {
                // Nút Hướng dẫn (F1) góc phải trên
                var btnHelp = new Button
                {
                    Text = "F1",
                    Width = 36,
                    Height = 28,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(107,114,128),
                    ForeColor = System.Drawing.Color.White,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                };
                btnHelp.FlatAppearance.BorderSize = 0;
                btnHelp.Left = this.ClientSize.Width - btnHelp.Width - 12;
                btnHelp.Top = 10;
                btnHelp.Click += (s, e) => { try { new QLTN_LT.GUI.Helper.FormShortcuts().ShowDialog(this); } catch { } };
                this.Controls.Add(btnHelp);

                // Tooltips
                var tip = new ToolTip { AutoPopDelay = 4000, InitialDelay = 400, ReshowDelay = 200, ShowAlways = true };
                if (btnHelp != null) tip.SetToolTip(btnHelp, "Hướng dẫn phím tắt (F1)");
                if (txtSearch != null) tip.SetToolTip(txtSearch, "Tìm kiếm\nMẹo: Enter để áp dụng, Esc để xóa");
                if (cboCategory != null) tip.SetToolTip(cboCategory, "Lọc theo danh mục");
                var miBtnAdd = this.GetType().GetField("btnAdd", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Control;
                var miBtnReload = this.GetType().GetField("btnReload", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(this) as Control;
                if (miBtnAdd != null) tip.SetToolTip(miBtnAdd, "Thêm mới (Ctrl+N)");
                if (miBtnReload != null) tip.SetToolTip(miBtnReload, "Làm mới (F5)");
                if (dgvMenu != null) tip.SetToolTip(dgvMenu, "Double‑click để sửa\nEnter để mở chi tiết");
            }
            catch { }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvMenu == null) return;
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
                _allItems = _bll.GetAllItems() ?? new List<MenuItemDTO>();
                PopulateCategoryFilter();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "LoadData");
            }
            finally { Wait(false); }
        }

        private void PopulateCategoryFilter()
        {
            try
            {
                if (cboCategory == null) return;
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

        private void ApplyFilter()
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

                var list = filtered.ToList();
                if (dgvMenu != null) dgvMenu.DataSource = list;
                if (lblPageInfo != null) lblPageInfo.Text = $"Tổng: {list.Count} món";
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "ApplyFilter");
            }
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ApplyFilter();
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
                        if (form.ShowDialog(this) == DialogResult.OK)
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
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var list = dgvMenu?.DataSource as IEnumerable<MenuItemDTO> ?? _allItems ?? new List<MenuItemDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("ID,TenMon,DanhMuc,Gia,MoTa,TrangThai");
                            foreach (var m in list)
                            {
                                lines.Add($"{m.ItemID},{EscapeCsv(m.ItemName)},{EscapeCsv(m.CategoryName)},{m.UnitPrice},{EscapeCsv(m.Description)},{(m.IsAvailable?"Available":"Unavailable")}");
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
                                sb.AppendLine($"<tr><td>{m.ItemID}</td><td>{Html(m.ItemName)}</td><td>{Html(m.CategoryName)}</td><td align='right'>{m.UnitPrice:N0}</td><td>{Html(m.Description)}</td><td>{(m.IsAvailable?"Available":"Unavailable")}</td></tr>");
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

        /// <summary>
        /// Khởi tạo context menu cho các hàng trong DataGridView
        /// </summary>
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

        /// <summary>
        /// Xử lý nút Next cho phân trang
        /// </summary>
        private void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                _currentPage++;
                ApplyFilter();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "BtnNext_Click");
            }
        }

        /// <summary>
        /// Xử lý nút Previous cho phân trang
        /// </summary>
        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPage > 1)
                {
                    _currentPage--;
                    ApplyFilter();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "BtnPrevious_Click");
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allItems?.Clear();
                _rowMenu?.Dispose();
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
