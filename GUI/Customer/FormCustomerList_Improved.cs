using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Drawing;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Customer
{
    /// <summary>
    /// Improved Customer List Form with Enhanced Logic, Animations & Interactions
    /// Features:
    /// - Smooth animations and transitions
    /// - Enhanced user feedback
    /// - Improved data loading logic
    /// - Better error handling
    /// - Interactive effects
    /// - Optimized performance
    /// </summary>
    public partial class FormCustomerList_Improved : BaseForm
    {
        private CustomerBLL _bll = new CustomerBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private Timer _searchDebounceTimer;
        private List<CustomerDTO> _allData = new List<CustomerDTO>();
        private List<CustomerDTO> _filteredData = new List<CustomerDTO>();
        private ContextMenuStrip _ctxMenu;
        private Label _lblEmptyState;
        private Label _lblLoadingIndicator;
        private bool _isLoading = false;
        private string _lastSearchKeyword = "";
        private DateTime _lastLoadTime = DateTime.MinValue;

        public FormCustomerList_Improved()
        {
            InitializeComponent();

            // Apply modern styling
            this.KeyPreview = true;
            this.KeyDown += FormCustomerList_KeyDown;
            try
            {
                ModernUIHelper.ApplyModernFormStyle(this);
            }
            catch { }

            // Debounce search timer with optimized interval
            _searchDebounceTimer = new Timer { Interval = 300 };
            _searchDebounceTimer.Tick += SearchDebounceTimer_Tick;

            // Events
            this.Load += FormCustomerList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += TxtSearch_TextChanged;
                txtSearch.KeyDown += TxtSearch_KeyDown;
            }

            // Context menu
            BuildContextMenu();
        }

        private void FormCustomerList_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigureGrid();
                BuildEmptyState();
                BuildLoadingIndicator();
                
                // Load data with animation
                AnimationHelper.FadeIn(this, 300);
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCustomerList_Load");
                InteractionHelper.ShowError(this, "Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvCustomer == null) return;

                dgvCustomer.AutoGenerateColumns = false;
                dgvCustomer.Columns.Clear();

                // Selection CheckBox Column
                var chkCol = new DataGridViewCheckBoxColumn
                {
                    HeaderText = "✓",
                    Width = 40,
                    ReadOnly = true,
                    Name = "colCheck"
                };
                dgvCustomer.Columns.Add(chkCol);

                // ID Column
                var idCol = new DataGridViewTextBoxColumn
                {
                    Name = "CustomerID",
                    DataPropertyName = "CustomerID",
                    HeaderText = "ID",
                    Width = 60,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(idCol);

                // Name Column
                var nameCol = new DataGridViewTextBoxColumn
                {
                    Name = "CustomerName",
                    DataPropertyName = "CustomerName",
                    HeaderText = "TÊN KHÁCH HÀNG",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(nameCol);

                // Phone Column
                var phoneCol = new DataGridViewTextBoxColumn
                {
                    Name = "PhoneNumber",
                    DataPropertyName = "PhoneNumber",
                    HeaderText = "SỐ ĐIỆN THOẠI",
                    Width = 150,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(phoneCol);

                // Address Column
                var addressCol = new DataGridViewTextBoxColumn
                {
                    Name = "Address",
                    DataPropertyName = "Address",
                    HeaderText = "ĐỊA CHỈ",
                    Width = 250,
                    ReadOnly = true
                };
                dgvCustomer.Columns.Add(addressCol);

                // Action Button Column
                var editBtn = new DataGridViewButtonColumn
                {
                    HeaderText = "THAO TÁC",
                    Text = "✏️ Sửa",
                    UseColumnTextForButtonValue = true,
                    Name = "colEdit",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = ModernUIHelper.ModernColors.Primary500,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    }
                };
                dgvCustomer.Columns.Add(editBtn);

                // Apply modern grid styling
                ModernUIHelper.ApplyModernGridStyle(dgvCustomer);

                // Grid events
                dgvCustomer.CellContentClick += DgvCustomer_CellContentClick;
                dgvCustomer.CellDoubleClick += DgvCustomer_CellDoubleClick;
                dgvCustomer.MouseDown += DgvCustomer_MouseDown;
                dgvCustomer.CellMouseEnter += DgvCustomer_CellMouseEnter;
                dgvCustomer.CellMouseLeave += DgvCustomer_CellMouseLeave;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring grid: {ex.Message}");
            }
        }

        /// <summary>
        /// Enhanced cell content click with animation
        /// </summary>
        private void DgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvCustomer.Columns[e.ColumnIndex].Name == "colEdit")
                {
                    var cellValue = dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                    {
                        // Animate button click
                        AnimationHelper.Pulse(dgvCustomer.Rows[e.RowIndex], 200);
                        OpenEdit(customerId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in cell content click: {ex.Message}");
                InteractionHelper.ShowError(this, "Lỗi: " + ex.Message);
            }
        }

        /// <summary>
        /// Double click to edit
        /// </summary>
        private void DgvCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    var cellValue = dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                    {
                        OpenEdit(customerId);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in double click: {ex.Message}");
            }
        }

        /// <summary>
        /// Highlight row on mouse enter
        /// </summary>
        private void DgvCustomer_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    dgvCustomer.Rows[e.RowIndex].DefaultCellStyle.BackColor = ModernUIHelper.ModernColors.Gray50;
                }
            }
            catch { }
        }

        /// <summary>
        /// Remove highlight on mouse leave
        /// </summary>
        private void DgvCustomer_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    dgvCustomer.Rows[e.RowIndex].DefaultCellStyle.BackColor = ModernUIHelper.ModernColors.Surface;
                }
            }
            catch { }
        }

        /// <summary>
        /// Open edit form with animation
        /// </summary>
        private void OpenEdit(int customerId)
        {
            try
            {
                using (var form = new FormCustomerEdit(customerId))
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        InteractionHelper.ShowSuccess(this, "Cập nhật khách hàng thành công!");
                        LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "OpenEdit");
                InteractionHelper.ShowError(this, "Lỗi mở form: " + ex.Message);
            }
        }

        /// <summary>
        /// Async data loading with better logic
        /// </summary>
        private async void LoadDataAsync()
        {
            try
            {
                if (_isLoading) return;
                _isLoading = true;

                Wait(true);
                ShowLoadingState(true);

                // Simulate async operation
                await System.Threading.Tasks.Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(300);
                    _allData = _bll.GetAll() ?? new List<CustomerDTO>();
                });

                // Apply filter
                ApplyFilter();

                // Update UI
                UpdateGridData();
                UpdatePagination();
                UpdatePageInfo();
                UpdateEmptyState(_filteredData.Count == 0);

                _lastLoadTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                InteractionHelper.ShowError(this, "Lỗi tải dữ liệu: " + ex.Message);
            }
            finally
            {
                _isLoading = false;
                Wait(false);
                ShowLoadingState(false);
            }
        }

        /// <summary>
        /// Apply filter logic
        /// </summary>
        private void ApplyFilter()
        {
            try
            {
                string keyword = txtSearch?.Text?.ToLower() ?? string.Empty;

                if (keyword == _lastSearchKeyword && _filteredData.Count > 0)
                {
                    return; // Use cached results
                }

                _lastSearchKeyword = keyword;

                _filteredData = _allData.FindAll(x =>
                    string.IsNullOrEmpty(keyword) ||
                    (x.CustomerName?.ToLower().Contains(keyword) ?? false) ||
                    (x.PhoneNumber?.Contains(keyword) ?? false) ||
                    (x.Address?.ToLower().Contains(keyword) ?? false)
                );

                _totalRecords = _filteredData.Count;
                _currentPage = 1; // Reset to first page
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error applying filter: {ex.Message}");
            }
        }

        /// <summary>
        /// Update grid with paging
        /// </summary>
        private void UpdateGridData()
        {
            try
            {
                var pagedData = _filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvCustomer != null)
                {
                    dgvCustomer.DataSource = pagedData;
                    
                    // Animate rows
                    for (int i = 0; i < dgvCustomer.RowCount; i++)
                    {
                        AnimationHelper.FadeIn(dgvCustomer.Rows[i], 200 + (i * 50));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating grid: {ex.Message}");
            }
        }

        /// <summary>
        /// Update pagination state
        /// </summary>
        private void UpdatePagination()
        {
            try
            {
                if (btnPrevious != null)
                {
                    bool canPrev = _currentPage > 1;
                    if (btnPrevious.Enabled != canPrev)
                    {
                        if (canPrev)
                            InteractionHelper.EnableWithFade(btnPrevious);
                        else
                            InteractionHelper.DisableWithFade(btnPrevious);
                    }
                }

                if (btnNext != null)
                {
                    bool canNext = _currentPage * _pageSize < _totalRecords;
                    if (btnNext.Enabled != canNext)
                    {
                        if (canNext)
                            InteractionHelper.EnableWithFade(btnNext);
                        else
                            InteractionHelper.DisableWithFade(btnNext);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating pagination: {ex.Message}");
            }
        }

        /// <summary>
        /// Update page info label
        /// </summary>
        private void UpdatePageInfo()
        {
            try
            {
                if (lblPageInfo != null)
                {
                    if (_totalRecords == 0)
                    {
                        lblPageInfo.Text = "📊 Tổng cộng: 0 khách hàng";
                    }
                    else
                    {
                        int from = (_currentPage - 1) * _pageSize + 1;
                        int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                        lblPageInfo.Text = $"📊 Hiển thị {from} - {to} / {_totalRecords} khách hàng | Trang {_currentPage}";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating page info: {ex.Message}");
            }
        }

        /// <summary>
        /// Search debounce timer tick
        /// </summary>
        private void SearchDebounceTimer_Tick(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _currentPage = 1;
            LoadDataAsync();
        }

        /// <summary>
        /// Search text changed
        /// </summary>
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        /// <summary>
        /// Search key down
        /// </summary>
        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                LoadDataAsync();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtSearch.Clear();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Previous page with animation
        /// </summary>
        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                AnimationHelper.Pulse(btnPrevious, 200);
                _currentPage--;
                UpdateGridData();
                UpdatePagination();
                UpdatePageInfo();
            }
        }

        /// <summary>
        /// Next page with animation
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                AnimationHelper.Pulse(btnNext, 200);
                _currentPage++;
                UpdateGridData();
                UpdatePagination();
                UpdatePageInfo();
            }
        }

        /// <summary>
        /// Add new customer
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                AnimationHelper.Pulse(btnAdd, 200);
                
                using (var form = new FormCustomerAdd())
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        InteractionHelper.ShowSuccess(this, "Thêm khách hàng thành công!");
                        LoadDataAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening add form: {ex.Message}");
                InteractionHelper.ShowError(this, "Lỗi mở form: " + ex.Message);
            }
        }

        /// <summary>
        /// Import data
        /// </summary>
        private void btnImport_Click(object sender, EventArgs e)
        {
            AnimationHelper.Pulse(btnImport, 200);
            InteractionHelper.ShowInfo(this, "Chức năng Nhập dữ liệu đang phát triển");
        }

        /// <summary>
        /// Right click context menu
        /// </summary>
        private void DgvCustomer_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && dgvCustomer != null)
                {
                    var hit = dgvCustomer.HitTest(e.X, e.Y);
                    if (hit.RowIndex >= 0)
                    {
                        dgvCustomer.ClearSelection();
                        dgvCustomer.Rows[hit.RowIndex].Selected = true;
                        _ctxMenu?.Show(dgvCustomer, new Point(e.X, e.Y));
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Build context menu
        /// </summary>
        private void BuildContextMenu()
        {
            _ctxMenu = new ContextMenuStrip();

            var miEdit = new ToolStripMenuItem("✏️ Sửa khách hàng");
            miEdit.Click += (s, e) =>
            {
                if (dgvCustomer?.CurrentRow == null) return;
                var val = dgvCustomer.CurrentRow.Cells["CustomerID"].Value;
                if (val != null && int.TryParse(val.ToString(), out int id)) OpenEdit(id);
            };

            var miDelete = new ToolStripMenuItem("🗑️ Xóa khách hàng");
            miDelete.Click += (s, e) => DeleteSelected();

            var miExport = new ToolStripMenuItem("[object Object]CSV/Excel)");
            miExport.Click += (s, e) => btnExport_Click(s, e);

            _ctxMenu.Items.AddRange(new ToolStripItem[] { miEdit, miDelete, new ToolStripSeparator(), miExport });
        }

        /// <summary>
        /// Build empty state
        /// </summary>
        private void BuildEmptyState()
        {
            _lblEmptyState = new Label
            {
                Text = "📭 Không có dữ liệu khách hàng\n\nNhấn '➕ Thêm khách hàng' để bắt đầu",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = ModernUIHelper.ModernColors.TextTertiary,
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                Visible = false
            };
            this.Controls.Add(_lblEmptyState);
            _lblEmptyState.BringToFront();
        }

        /// <summary>
        /// Build loading indicator
        /// </summary>
        private void BuildLoadingIndicator()
        {
            _lblLoadingIndicator = new Label
            {
                Text = "⏳ Đang tải dữ liệu...",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = ModernUIHelper.ModernColors.TextSecondary,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BackColor = ModernUIHelper.ModernColors.Gray50,
                Visible = false
            };
            this.Controls.Add(_lblLoadingIndicator);
            _lblLoadingIndicator.BringToFront();
        }

        /// <summary>
        /// Update empty state visibility
        /// </summary>
        private void UpdateEmptyState(bool isEmpty)
        {
            if (_lblEmptyState == null) return;
            
            if (isEmpty && !_lblEmptyState.Visible)
            {
                AnimationHelper.FadeIn(_lblEmptyState, 300);
            }
            else if (!isEmpty && _lblEmptyState.Visible)
            {
                AnimationHelper.FadeOut(_lblEmptyState, 300);
            }
        }

        /// <summary>
        /// Show loading state
        /// </summary>
        private void ShowLoadingState(bool isLoading)
        {
            if (_lblLoadingIndicator == null) return;
            
            if (isLoading && !_lblLoadingIndicator.Visible)
            {
                AnimationHelper.FadeIn(_lblLoadingIndicator, 200);
            }
            else if (!isLoading && _lblLoadingIndicator.Visible)
            {
                AnimationHelper.FadeOut(_lblLoadingIndicator, 200);
            }
        }

        /// <summary>
        /// Delete selected customer
        /// </summary>
        private void DeleteSelected()
        {
            try
            {
                if (dgvCustomer?.CurrentRow == null)
                {
                    InteractionHelper.ShowWarning(this, "Vui lòng chọn khách hàng để xóa");
                    return;
                }

                var val = dgvCustomer.CurrentRow.Cells["CustomerID"].Value;
                if (val == null || !int.TryParse(val.ToString(), out int id)) return;

                if (InteractionHelper.ShowConfirmationDialog(this, 
                    "🗑️ Bạn có chắc muốn xóa khách hàng này?\n\nHành động này không thể hoàn tác!", 
                    "Xác nhận xóa") != DialogResult.Yes)
                    return;

                _bll.Delete(id);
                InteractionHelper.ShowSuccess(this, "Xóa khách hàng thành công!");
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                InteractionHelper.ShowError(this, "Lỗi xóa: " + ex.Message);
            }
        }

        /// <summary>
        /// Export data
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                AnimationHelper.Pulse(btnExport, 200);
                
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var list = _filteredData ?? _allData ?? new List<CustomerDTO>();
                        
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string> { "ID,Tên,SĐT,Địa chỉ" };
                            foreach (var c in list)
                            {
                                lines.Add($"{c.CustomerID},{EscapeCsv(c.CustomerName)},{EscapeCsv(c.PhoneNumber)},{EscapeCsv(c.Address)}");
                            }
                            File.WriteAllLines(sfd.FileName, lines, Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><style>body{font-family:Segoe UI;margin:20px;} h3{color:#2980b9;} table{border-collapse:collapse;width:100%;} th{background:#2980b9;color:white;padding:12px;text-align:left;} td{border:1px solid #ddd;padding:10px;} tr:nth-child(even){background:#f5f5f5;} tr:hover{background:#e8f4f8;}</style></head><body>");
                            sb.AppendLine($"<h3>📋 Danh sách khách hàng ({list.Count()} bản ghi)</h3>");
                            sb.AppendLine($"<p>Xuất lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6'>");
                            sb.AppendLine("<tr><th>ID</th><th>Tên khách hàng</th><th>SĐT</th><th>Địa chỉ</th></tr>");
                            foreach (var c in list)
                            {
                                sb.AppendLine($"<tr><td>{c.CustomerID}</td><td>{Html(c.CustomerName)}</td><td>{Html(c.PhoneNumber)}</td><td>{Html(c.Address)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        }
                        
                        InteractionHelper.ShowSuccess(this, $"Xuất thành công: {Path.GetFileName(sfd.FileName)}");
                    }
                }
            }
            catch (Exception ex)
            {
                InteractionHelper.ShowError(this, "Lỗi xuất: " + ex.Message);
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
            var sb = new StringBuilder(input);
            sb.Replace("&", "&amp;"); sb.Replace("<", "&lt;"); sb.Replace(">", "&gt;"); sb.Replace("\"", "&quot;"); sb.Replace("'", "&#39;");
            return sb.ToString();
        }

        /// <summary>
        /// Keyboard shortcuts
        /// </summary>
        private void FormCustomerList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F5)
                {
                    LoadDataAsync();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.N)
                {
                    btnAdd_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvCustomer?.CurrentRow != null)
                {
                    var cellValue = dgvCustomer.CurrentRow.Cells["CustomerID"].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                    {
                        OpenEdit(customerId);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    DeleteSelected();
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    btnExport_Click(sender, EventArgs.Empty);
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
                _filteredData?.Clear();
                _bll = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}

