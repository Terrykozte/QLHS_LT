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
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Customer
{
    /// <summary>
    /// Enhanced Customer List Form with Modern UI/UX
    /// Features:
    /// - Modern color scheme and typography
    /// - Improved spacing and layout
    /// - Better visual hierarchy
    /// - Enhanced user feedback
    /// - Smooth animations
    /// - Better accessibility
    /// </summary>
    public partial class FormCustomerList_Enhanced : BaseForm
    {
        private CustomerBLL _bll = new CustomerBLL();
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalRecords = 0;
        private Timer _searchDebounceTimer;
        private List<CustomerDTO> _allData = new List<CustomerDTO>();
        private ContextMenuStrip _ctxMenu;
        private Label _lblEmptyState;
        private Label _lblLoadingIndicator;
        private Panel _pnlHeader;
        private Panel _pnlToolbar;
        private Panel _pnlContent;
        private Panel _pnlFooter;

        public FormCustomerList_Enhanced()
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

            // Debounce search timer
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                LoadData();
            };

            // Events
            this.Load += FormCustomerList_Load;
            if (txtSearch != null)
            {
                txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
            }

            // Context menu
            BuildContextMenu();
        }

        private void FormCustomerList_Load(object sender, EventArgs e)
        {
            try
            {
                BuildLayout();
                ConfigureGrid();
                BuildEmptyState();
                BuildLoadingIndicator();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCustomerList_Load");
            }
        }

        /// <summary>
        /// Builds the modern layout structure
        /// </summary>
        private void BuildLayout()
        {
            try
            {
                // Header Panel
                _pnlHeader = ModernUIHelper.CreateModernPanel(ModernUIHelper.ModernColors.Surface);
                _pnlHeader.Dock = DockStyle.Top;
                _pnlHeader.Height = 80;
                _pnlHeader.BorderStyle = BorderStyle.FixedSingle;
                _pnlHeader.BorderColor = ModernUIHelper.ModernColors.Border;

                var lblTitle = new Label
                {
                    Text = "👥 Danh Sách Khách Hàng",
                    Font = new Font("Segoe UI", ModernUIHelper.FontSizes.XXL, FontStyle.Bold),
                    ForeColor = ModernUIHelper.ModernColors.TextPrimary,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(ModernUIHelper.Spacing.LG),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                _pnlHeader.Controls.Add(lblTitle);
                this.Controls.Add(_pnlHeader);

                // Toolbar Panel
                _pnlToolbar = ModernUIHelper.CreateModernPanel(ModernUIHelper.ModernColors.Gray50);
                _pnlToolbar.Dock = DockStyle.Top;
                _pnlToolbar.Height = 60;
                _pnlToolbar.AutoScroll = true;

                BuildToolbar(_pnlToolbar);
                this.Controls.Add(_pnlToolbar);

                // Content Panel
                _pnlContent = ModernUIHelper.CreateModernPanel();
                _pnlContent.Dock = DockStyle.Fill;
                this.Controls.Add(_pnlContent);

                // Footer Panel
                _pnlFooter = ModernUIHelper.CreateModernPanel(ModernUIHelper.ModernColors.Gray50);
                _pnlFooter.Dock = DockStyle.Bottom;
                _pnlFooter.Height = 60;
                _pnlFooter.BorderStyle = BorderStyle.FixedSingle;
                _pnlFooter.BorderColor = ModernUIHelper.ModernColors.Border;

                BuildFooter(_pnlFooter);
                this.Controls.Add(_pnlFooter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building layout: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds the toolbar with action buttons
        /// </summary>
        private void BuildToolbar(Panel toolbar)
        {
            try
            {
                // Search Box
                var searchBox = new Guna2TextBox
                {
                    PlaceholderText = "🔍 Tìm kiếm theo tên, SĐT, địa chỉ...",
                    Width = 300,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.MD),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top
                };
                ModernUIHelper.ApplyModernSearchBoxStyle(searchBox);
                searchBox.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
                toolbar.Controls.Add(searchBox);
                txtSearch = searchBox;

                // Add Button
                var btnAdd = new Guna2Button
                {
                    Text = "➕ Thêm Khách Hàng",
                    Width = 150,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.MD),
                    Location = new Point(320, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnAdd, ButtonType.Primary);
                btnAdd.Click += btnAdd_Click;
                toolbar.Controls.Add(btnAdd);

                // Import Button
                var btnImport = new Guna2Button
                {
                    Text = "📥 Nhập Dữ Liệu",
                    Width = 150,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.MD),
                    Location = new Point(480, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnImport, ButtonType.Secondary);
                btnImport.Click += btnImport_Click;
                toolbar.Controls.Add(btnImport);

                // Export Button
                var btnExport = new Guna2Button
                {
                    Text = "📤 Xuất Dữ Liệu",
                    Width = 150,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.MD),
                    Location = new Point(640, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnExport, ButtonType.Secondary);
                btnExport.Click += btnExport_Click;
                toolbar.Controls.Add(btnExport);

                // Refresh Button
                var btnRefresh = new Guna2Button
                {
                    Text = "🔄 Làm Mới",
                    Width = 120,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.MD),
                    Location = new Point(800, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnRefresh, ButtonType.Ghost);
                btnRefresh.Click += (s, e) => LoadData();
                toolbar.Controls.Add(btnRefresh);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building toolbar: {ex.Message}");
            }
        }

        /// <summary>
        /// Builds the footer with pagination info
        /// </summary>
        private void BuildFooter(Panel footer)
        {
            try
            {
                // Page Info Label
                lblPageInfo = new Label
                {
                    Text = "📊 Tổng cộng: 0 khách hàng",
                    Font = new Font("Segoe UI", ModernUIHelper.FontSizes.SM),
                    ForeColor = ModernUIHelper.ModernColors.TextSecondary,
                    Dock = DockStyle.Left,
                    Padding = new Padding(ModernUIHelper.Spacing.LG),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false,
                    Width = 300
                };
                footer.Controls.Add(lblPageInfo);

                // Pagination Controls
                var pnlPagination = new Panel
                {
                    Dock = DockStyle.Right,
                    Width = 300,
                    Padding = new Padding(ModernUIHelper.Spacing.MD)
                };

                btnPrevious = new Guna2Button
                {
                    Text = "⬅ Trước",
                    Width = 100,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.SM),
                    Location = new Point(10, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnPrevious, ButtonType.Secondary);
                btnPrevious.Click += btnPrevious_Click;
                pnlPagination.Controls.Add(btnPrevious);

                btnNext = new Guna2Button
                {
                    Text = "Tiếp ➜",
                    Width = 100,
                    Height = 40,
                    Margin = new Padding(ModernUIHelper.Spacing.SM),
                    Location = new Point(120, 10)
                };
                ModernUIHelper.ApplyModernButtonStyle(btnNext, ButtonType.Secondary);
                btnNext.Click += btnNext_Click;
                pnlPagination.Controls.Add(btnNext);

                footer.Controls.Add(pnlPagination);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error building footer: {ex.Message}");
            }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvCustomer == null) return;

                dgvCustomer.AutoGenerateColumns = false;
                dgvCustomer.Columns.Clear();
                dgvCustomer.Dock = DockStyle.Fill;
                dgvCustomer.Parent = _pnlContent;

                // Apply modern grid styling
                ModernUIHelper.ApplyModernGridStyle(dgvCustomer);

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
                        Font = new Font("Segoe UI", ModernUIHelper.FontSizes.SM, FontStyle.Bold)
                    }
                };
                dgvCustomer.Columns.Add(editBtn);

                // Grid events
                dgvCustomer.CellContentClick += DgvCustomer_CellContentClick;
                dgvCustomer.CellDoubleClick += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        var cellValue = dgvCustomer.Rows[e.RowIndex].Cells["CustomerID"].Value;
                        if (cellValue != null && int.TryParse(cellValue.ToString(), out int customerId))
                        {
                            OpenEdit(customerId);
                        }
                    }
                };
                dgvCustomer.MouseDown += DgvCustomer_MouseDown;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring grid: {ex.Message}");
            }
        }

        private void DgvCustomer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0 && dgvCustomer.Columns[e.ColumnIndex].Name == "colEdit")
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
                Debug.WriteLine($"Error in cell content click: {ex.Message}");
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenEdit(int customerId)
        {
            try
            {
                using (var form = new FormCustomerEdit(customerId))
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "OpenEdit");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                ShowLoadingState(true);

                _allData = _bll.GetAll() ?? new List<CustomerDTO>();

                // Filter
                string keyword = txtSearch?.Text?.ToLower() ?? string.Empty;
                var filteredData = _allData.FindAll(x =>
                    string.IsNullOrEmpty(keyword) ||
                    (x.CustomerName?.ToLower().Contains(keyword) ?? false) ||
                    (x.PhoneNumber?.Contains(keyword) ?? false) ||
                    (x.Address?.ToLower().Contains(keyword) ?? false)
                );

                _totalRecords = filteredData.Count;
                UpdatePagination();

                // Paging (client-side)
                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                if (dgvCustomer != null)
                {
                    dgvCustomer.DataSource = pagedData;
                }

                // Update page info with better formatting
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

                UpdateEmptyState(pagedData.Count == 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"❌ Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
                ShowLoadingState(false);
            }
        }

        private void UpdatePagination()
        {
            try
            {
                if (btnPrevious != null)
                    btnPrevious.Enabled = _currentPage > 1;

                if (btnNext != null)
                    btnNext.Enabled = _currentPage * _pageSize < _totalRecords;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating pagination: {ex.Message}");
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadData();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                LoadData();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new FormCustomerAdd())
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
                MessageBox.Show($"❌ Lỗi mở form thêm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("📥 Chức năng Nhập dữ liệu đang phát triển.\n\nVui lòng quay lại sau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                        _ctxMenu?.Show(dgvCustomer, new System.Drawing.Point(e.X, e.Y));
                    }
                }
            }
            catch { }
        }

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

        private void BuildEmptyState()
        {
            _lblEmptyState = new Label
            {
                Text = "📭 Không có dữ liệu khách hàng\n\nNhấn '➕ Thêm khách hàng' để bắt đầu",
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = ModernUIHelper.ModernColors.TextTertiary,
                Font = new Font("Segoe UI", ModernUIHelper.FontSizes.LG, FontStyle.Regular),
                Visible = false
            };
            _pnlContent.Controls.Add(_lblEmptyState);
            _lblEmptyState.BringToFront();
        }

        private void BuildLoadingIndicator()
        {
            _lblLoadingIndicator = new Label
            {
                Text = "⏳ Đang tải dữ liệu...",
                AutoSize = false,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = ModernUIHelper.ModernColors.TextSecondary,
                Font = new Font("Segoe UI", ModernUIHelper.FontSizes.MD, FontStyle.Regular),
                BackColor = ModernUIHelper.ModernColors.Gray50,
                Visible = false
            };
            _pnlContent.Controls.Add(_lblLoadingIndicator);
            _lblLoadingIndicator.BringToFront();
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            if (_lblEmptyState == null) return;
            _lblEmptyState.Visible = isEmpty;
        }

        private void ShowLoadingState(bool isLoading)
        {
            if (_lblLoadingIndicator == null) return;
            _lblLoadingIndicator.Visible = isLoading;
        }

        private void DeleteSelected()
        {
            try
            {
                if (dgvCustomer?.CurrentRow == null)
                {
                    MessageBox.Show("⚠️ Vui lòng chọn khách hàng để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                var val = dgvCustomer.CurrentRow.Cells["CustomerID"].Value;
                if (val == null || !int.TryParse(val.ToString(), out int id)) return;

                if (!ShowConfirm("🗑️ Bạn có chắc muốn xóa khách hàng này?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa")) return;
                _bll.Delete(id);
                ShowInfo("✅ Xóa khách hàng thành công!");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xóa: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"DanhSachKhachHang_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (UIHelper.ShowSaveFileDialog(this, sfd) == DialogResult.OK)
                    {
                        var list = dgvCustomer?.DataSource as IEnumerable<CustomerDTO> ?? _allData ?? new List<CustomerDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("ID,Tên,SĐT,Địa chỉ");
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
                        MessageBox.Show($"✅ Xuất danh sách khách hàng thành công!\n\nFile: {Path.GetFileName(sfd.FileName)}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Lỗi xuất: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void FormCustomerList_KeyDown(object sender, KeyEventArgs e)
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

