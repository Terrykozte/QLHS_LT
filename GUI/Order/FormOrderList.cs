using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderList : BaseForm
    {
        private OrderBLL _orderBLL;
        private Timer _searchDebounceTimer;
        private List<OrderDTO> _allData = new List<OrderDTO>();
        private ContextMenuStrip _ordersMenu;

        public FormOrderList()
        {
            InitializeComponent();
            _orderBLL = new OrderBLL();

            // UX
            this.KeyPreview = true;
            this.KeyDown += FormOrderList_KeyDown;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                LoadData();
            };

            // Styling
            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvOrders != null) UIHelper.ApplyGridStyle(dgvOrders);
                if (btnReload != null) UIHelper.ApplyGunaButtonStyle(btnReload, isPrimary: false);
                if (btnCreate != null) UIHelper.ApplyGunaButtonStyle(btnCreate, isPrimary: true);
                if (btnViewDetails != null) UIHelper.ApplyGunaButtonStyle(btnViewDetails, isPrimary: false);
                if (btnCancelOrder != null) UIHelper.ApplyGunaButtonStyle(btnCancelOrder, isPrimary: false);
            }
            catch { }

            // Context menu for orders
            _ordersMenu = new ContextMenuStrip();
            var mStart = new ToolStripMenuItem("Nhận khách (Processing)") { Name = "mStart" };
            mStart.Click += (s, e) => StartProcessingSelected();
            _ordersMenu.Items.Add(mStart);

            var mComplete = new ToolStripMenuItem("Hoàn tất (Completed)") { Name = "mComplete" };
            mComplete.Click += (s, e) => CompleteSelected();
            _ordersMenu.Items.Add(mComplete);

            // Events
            this.Load += FormOrderList_Load;
            this.FormClosing += (s,e)=> { try { _ordersMenu?.Dispose(); } catch { } };
            if (txtSearch != null) txtSearch.TextChanged += txtSearch_TextChanged;
            if (btnReload != null) btnReload.Click += btnReload_Click;
            if (btnCreate != null) btnCreate.Click += btnCreate_Click;
            if (btnViewDetails != null) btnViewDetails.Click += btnViewDetails_Click;
            if (btnCancelOrder != null) btnCancelOrder.Click += btnCancelOrder_Click;
            if (dgvOrders != null) dgvOrders.MouseDown += dgvOrders_MouseDown;
        }

        private void FormOrderList_Load(object sender, EventArgs e)
        {
            try
            {
                // Setup keyboard navigation
                KeyboardNavigationHelper.RegisterForm(this, new List<Control>
                {
                    txtSearch, dtpFromDate, dtpToDate, cmbStatus, btnReload, btnCreate, dgvOrders
                });

                // Setup animations
                AnimationHelper.FadeIn(this, 300);

                // Setup responsive
                ApplyResponsiveDesign();

                SetupControls();
                ConfigureGrid();
                LoadData();
                
                if (dgvOrders != null) 
                {
                    dgvOrders.CellDoubleClick += dgvOrders_CellDoubleClick;
                    dgvOrders.MouseDown += dgvOrders_MouseDown;
                    dgvOrders.CellFormatting += dgvOrders_CellFormatting;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormOrderList_Load");
            }
        }

        private void SetupControls()
        {
            try
            {
                if (dtpFromDate != null) dtpFromDate.Value = DateTime.Today.AddDays(-30);
                if (dtpToDate != null) dtpToDate.Value = DateTime.Today;

                if (cmbStatus != null)
                {
                    cmbStatus.Items.Clear();
                    cmbStatus.Items.Add("Tất cả");
                    cmbStatus.Items.Add("Pending");
                    cmbStatus.Items.Add("Processing");
                    cmbStatus.Items.Add("Completed");
                    cmbStatus.Items.Add("Cancelled");
                    cmbStatus.Items.Add("Reserved");
                    cmbStatus.SelectedIndex = 0;
                    cmbStatus.SelectedIndexChanged += (s, e) => 
                    {
                        _searchDebounceTimer.Stop();
                        _searchDebounceTimer.Start();
                    };
                }

                if (dtpFromDate != null) dtpFromDate.ValueChanged += (s, e) => 
                {
                    _searchDebounceTimer.Stop();
                    _searchDebounceTimer.Start();
                };
                
                if (dtpToDate != null) dtpToDate.ValueChanged += (s, e) => 
                {
                    _searchDebounceTimer.Stop();
                    _searchDebounceTimer.Start();
                };

                // Setup button hover effects
                if (btnCreate is Guna2Button createBtn)
                {
                    UXInteractionHelper.AddHoverEffect(createBtn,
                        Color.FromArgb(34, 197, 94),
                        Color.FromArgb(22, 163, 74));
                    UXInteractionHelper.AddClickEffect(createBtn);
                }

                if (btnReload is Guna2Button reloadBtn)
                {
                    UXInteractionHelper.AddHoverEffect(reloadBtn,
                        Color.FromArgb(59, 130, 246),
                        Color.FromArgb(37, 99, 235));
                    UXInteractionHelper.AddClickEffect(reloadBtn);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up controls: {ex.Message}");
            }
        }

        private void ApplyResponsiveDesign()
        {
            try
            {
                // Adjust grid row height
                if (dgvOrders != null)
                {
                    dgvOrders.RowTemplate.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 30);
                    ResponsiveHelper.AdjustDataGridViewColumns(dgvOrders, this);
                }

                // Adjust font sizes
                if (lblPageInfo != null)
                    lblPageInfo.Font = new Font(lblPageInfo.Font.FontFamily,
                        ResponsiveHelper.GetResponsiveFontSize(10, this));
            }
            catch { }
        }

        private void ConfigureGrid()
        {
            try
            {
                if (dgvOrders == null) return;

                dgvOrders.AutoGenerateColumns = false;
                dgvOrders.Columns.Clear();

                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderID", HeaderText = "ID", Width = 60 });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderNumber", HeaderText = "MÃ ĐƠN", Width = 120 });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDate", HeaderText = "NGÀY ĐẶT", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "KHÁCH HÀNG", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalAmount", HeaderText = "TỔNG TIỀN", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
                dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error configuring grid: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                Wait(true);

                var fromDate = dtpFromDate?.Value ?? DateTime.Today.AddDays(-30);
                var toDate = dtpToDate?.Value ?? DateTime.Today;
                var status = cmbStatus?.SelectedItem?.ToString() == "Tất cả" ? null : cmbStatus?.SelectedItem?.ToString();
                var keyword = txtSearch?.Text ?? string.Empty;

                // Check cache first
                string cacheKey = $"orders_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}_{status}_{keyword}";
                var cachedData = DataBindingHelper.GetCachedData<List<OrderDTO>>(cacheKey);
                
                List<OrderDTO> data;
                if (cachedData != null)
                {
                    data = cachedData;
                }
                else
                {
                    data = _orderBLL.GetAll(fromDate, toDate, status, keyword)?.ToList() ?? new List<OrderDTO>();
                    DataBindingHelper.CacheData(cacheKey, data);
                }

                _allData = data;

                if (dgvOrders != null)
                {
                    dgvOrders.DataSource = data;
                    AnimationHelper.FadeIn(dgvOrders, 300);
                }

                if (lblPageInfo != null)
                {
                    lblPageInfo.Text = $"Tổng: {data.Count} đơn";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                UXInteractionHelper.ShowError("Lỗi", $"Không thể tải dữ liệu. Chi tiết: {ex.Message}");
            }
            finally
            {
                Wait(false);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // Designer-bound alias
        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FormOrderCreate())
                {
                    if (f.ShowDialog(this) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening create form: {ex.Message}");
                MessageBox.Show($"Lỗi mở form tạo đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvOrders?.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một đơn hàng để hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedOrder = dgvOrders.CurrentRow.DataBoundItem as OrderDTO;
                if (selectedOrder == null)
                {
                    MessageBox.Show("Không thể lấy thông tin đơn hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (selectedOrder.Status == "Completed" || selectedOrder.Status == "Cancelled")
                {
                    MessageBox.Show("Không thể hủy đơn hàng đã hoàn thành hoặc đã bị hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!ShowConfirm($"Bạn có chắc chắn muốn hủy đơn hàng '{selectedOrder.OrderNumber}'?", "Xác nhận hủy"))
                {
                    return;
                }

                _orderBLL.CancelOrder(selectedOrder.OrderID);
                ShowInfo("Hủy đơn hàng thành công!");
                LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error canceling order: {ex.Message}");
                MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvOrders?.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một đơn hàng để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedOrder = dgvOrders.CurrentRow.DataBoundItem as OrderDTO;
                if (selectedOrder == null)
                {
                    MessageBox.Show("Không thể lấy thông tin đơn hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var form = new FormOrderDetail(selectedOrder.OrderID))
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error viewing details: {ex.Message}");
                MessageBox.Show($"Lỗi xem chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    btnViewDetails_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in cell double click: {ex.Message}");
            }
        }

        private void dgvOrders_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && dgvOrders != null)
                {
                    var hit = dgvOrders.HitTest(e.X, e.Y);
                    if (hit.RowIndex >= 0)
                    {
                        dgvOrders.ClearSelection();
                        dgvOrders.Rows[hit.RowIndex].Selected = true;
                        var ord = dgvOrders.Rows[hit.RowIndex].DataBoundItem as OrderDTO;
                        if (ord != null)
                        {
                            // Enable only when Reserved
                            foreach (ToolStripItem it in _ordersMenu.Items)
                            {
                                if (it.Name == "mStart") it.Enabled = string.Equals(ord.Status, "Reserved", StringComparison.OrdinalIgnoreCase);
                                if (it.Name == "mComplete") it.Enabled = string.Equals(ord.Status, "Processing", StringComparison.OrdinalIgnoreCase);
                            }
                            _ordersMenu.Show(dgvOrders, new Point(e.X, e.Y));
                        }
                    }
                }
            }
            catch { }
        }

        private void StartProcessingSelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow == null) return;
                var ord = dgvOrders.CurrentRow.DataBoundItem as OrderDTO;
                if (ord == null) return;
                if (!string.Equals(ord.Status, "Reserved", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Chỉ có thể nhận khách cho đơn ở trạng thái Reserved.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _orderBLL.StartProcessing(ord.OrderID);
                UXInteractionHelper.ShowToast(this, $"Đơn {ord.OrderNumber} chuyển sang Processing", 2000, Color.FromArgb(34,197,94), Color.White);
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "StartProcessingSelected");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchDebounceTimer.Stop();
            _searchDebounceTimer.Start();
        }

        private void FormOrderList_KeyDown(object sender, KeyEventArgs e)
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
                    btnCreate_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.E)
                {
                    ExportOrders();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter && dgvOrders?.CurrentRow != null)
                {
                    btnViewDetails_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete && dgvOrders?.CurrentRow != null)
                {
                    btnCancelOrder_Click(sender, EventArgs.Empty);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"KeyDown error: {ex.Message}");
            }
        }

        private void ExportOrders()
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Excel (*.xls)|*.xls|CSV (*.csv)|*.csv";
                    sfd.FileName = $"DonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xls";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        var list = dgvOrders?.DataSource as IEnumerable<OrderDTO> ?? _allData ?? new List<OrderDTO>();
                        if (sfd.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                        {
                            var lines = new List<string>();
                            lines.Add("OrderID,MaDon,Ngay,KhachHang,TongTien,TrangThai");
                            foreach (var o in list)
                            {
                                lines.Add($"{o.OrderID},{EscapeCsv(o.OrderNumber)},{o.OrderDate:dd/MM/yyyy HH:mm},{EscapeCsv(o.CustomerName)},{o.TotalAmount},{EscapeCsv(o.Status)}");
                            }
                            System.IO.File.WriteAllLines(sfd.FileName, lines, System.Text.Encoding.UTF8);
                        }
                        else
                        {
                            var sb = new System.Text.StringBuilder();
                            sb.AppendLine("<html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body>");
                            sb.AppendLine("<h3>Danh sách đơn hàng</h3>");
                            sb.AppendLine("<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Segoe UI;font-size:11pt'>");
                            sb.AppendLine("<tr style='background:#e5e7eb'><th>ID</th><th>Mã đơn</th><th>Ngày</th><th>Khách hàng</th><th>Tổng tiền</th><th>Trạng thái</th></tr>");
                            foreach (var o in list)
                            {
                                sb.AppendLine($"<tr><td>{o.OrderID}</td><td>{Html(o.OrderNumber)}</td><td>{o.OrderDate:dd/MM/yyyy HH:mm}</td><td>{Html(o.CustomerName)}</td><td align='right'>{o.TotalAmount:N0}</td><td>{Html(o.Status)}</td></tr>");
                            }
                            sb.AppendLine("</table></body></html>");
                            System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                        }
                        MessageBox.Show("Xuất danh sách đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgvOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                if (dgvOrders == null) return;
                var col = dgvOrders.Columns[e.ColumnIndex];
                if (col != null && string.Equals(col.DataPropertyName, "Status", StringComparison.OrdinalIgnoreCase) && e.Value != null)
                {
                    string st = e.Value.ToString();
                    if (string.Equals(st, "Reserved", StringComparison.OrdinalIgnoreCase))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(254, 243, 199); // amber-100
                        e.CellStyle.ForeColor = Color.FromArgb(120, 53, 15);
                    }
                    else if (string.Equals(st, "Processing", StringComparison.OrdinalIgnoreCase))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(219, 234, 254); // blue-100
                        e.CellStyle.ForeColor = Color.FromArgb(30, 58, 138);
                    }
                    else if (string.Equals(st, "Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(220, 252, 231); // green-100
                        e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                    }
                    else if (string.Equals(st, "Cancelled", StringComparison.OrdinalIgnoreCase))
                    {
                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226); // red-100
                        e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                    }
                }
            }
            catch { }
        }

        private void CompleteSelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow == null) return;
                var ord = dgvOrders.CurrentRow.DataBoundItem as OrderDTO;
                if (ord == null) return;
                if (!string.Equals(ord.Status, "Processing", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Chỉ hoàn tất đơn ở trạng thái Processing.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                _orderBLL.CompleteOrder(ord.OrderID);
                UXInteractionHelper.ShowToast(this, $"Đơn {ord.OrderNumber} chuyển sang Completed", 2000, Color.FromArgb(34,197,94), Color.White);
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "CompleteSelected");
            }
        }

        private void FormOrderList_FormClosing(object sender, FormClosingEventArgs e)
        {
            _searchDebounceTimer?.Stop();
            _searchDebounceTimer?.Dispose();
            KeyboardNavigationHelper.UnregisterForm(this);
            DataBindingHelper.ClearCache();
        }

        protected override void CleanupResources()
        {
            try
            {
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _allData?.Clear();
                _orderBLL = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
