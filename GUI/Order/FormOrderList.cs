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

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderList : BaseForm
    {
        private readonly OrderBLL _orderBLL = new OrderBLL();
        private List<OrderDTO> _allData = new List<OrderDTO>();
        private Timer _searchDebounceTimer;
        private ContextMenuStrip _ordersMenu;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 15;
        private int _totalRecords = 0;

        public FormOrderList()
        {
            InitializeComponent();

            // UX
            this.KeyPreview = true;
            this.KeyDown += FormOrderList_KeyDown;

            // Debounce search
            _searchDebounceTimer = new Timer { Interval = 400 };
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            };

            // Context menu for orders
            InitContextMenu();
        }

        private void FormOrderList_Load(object sender, EventArgs e)
        {
            try
            {
                SetupControls();
                ConfigureGrid();
                LoadData();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormOrderList_Load");
            }
        }

        private void SetupControls()
        {
            dtpFromDate.Value = DateTime.Today.AddDays(-30);
            dtpToDate.Value = DateTime.Today;

            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Tất cả");
            cmbStatus.Items.Add("Pending");
            cmbStatus.Items.Add("Processing");
            cmbStatus.Items.Add("Completed");
            cmbStatus.Items.Add("Cancelled");
            cmbStatus.Items.Add("Reserved");
            cmbStatus.SelectedIndex = 0;

            // Events
            btnReload.Click += (s, e) => LoadData();
            btnCreate.Click += btnCreate_Click;
            btnPrevious.Click += btnPrevious_Click;
            btnNext.Click += btnNext_Click;
            txtSearch.TextChanged += (s, e) => { _searchDebounceTimer.Stop(); _searchDebounceTimer.Start(); };
            dgvOrders.CellDoubleClick += dgvOrders_CellDoubleClick;
            dgvOrders.MouseDown += dgvOrders_MouseDown;
            dgvOrders.CellFormatting += dgvOrders_CellFormatting;
        }

        private void ConfigureGrid()
        {
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.Columns.Clear();

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderID", HeaderText = "ID", Width = 60 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderNumber", HeaderText = "MÃ ĐƠN", Width = 120 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDate", HeaderText = "NGÀY ĐẶT", Width = 140, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "KHÁCH HÀNG", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalAmount", HeaderText = "TỔNG TIỀN", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
        }

        private void LoadData()
        {
            try
            {
                Wait(true);
                _allData = _orderBLL.GetAll(dtpFromDate.Value, dtpToDate.Value, null, null) ?? new List<OrderDTO>();
                _currentPage = 1;
                ApplyFiltersAndPagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Wait(false);
            }
        }

        private void ApplyFiltersAndPagination()
        {
            try
            {
                var status = cmbStatus.SelectedItem?.ToString() == "Tất cả" ? null : cmbStatus.SelectedItem?.ToString();
                var keyword = txtSearch.Text.ToLower();

                var filteredData = _allData.Where(o =>
                    (string.IsNullOrEmpty(status) || string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(keyword) || 
                     (o.OrderNumber?.ToLower().Contains(keyword) ?? false) || 
                     (o.CustomerName?.ToLower().Contains(keyword) ?? false))
                ).ToList();

                _totalRecords = filteredData.Count;

                var pagedData = filteredData
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList();

                dgvOrders.DataSource = pagedData;
                UpdatePagination();
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

            if (_totalRecords == 0)
            {
                lblPageInfo.Text = "Không có dữ liệu";
            }
            else
            {
                int from = (_currentPage - 1) * _pageSize + 1;
                int to = Math.Min(_currentPage * _pageSize, _totalRecords);
                lblPageInfo.Text = $"Hiển thị {from} - {to} / {_totalRecords} đơn";
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                using (var f = new FormOrderCreate())
                {
                    if (UIHelper.ShowFormDialog(this, f) == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi mở form tạo đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                OpenDetailForm();
            }
        }

        private void OpenDetailForm()
        {
            try
            {
                if (dgvOrders?.CurrentRow == null) return;
                var selectedOrder = dgvOrders.CurrentRow.DataBoundItem as OrderDTO;
                if (selectedOrder == null) return;

                using (var form = new FormOrderDetail(selectedOrder.OrderID))
                {
                    if (UIHelper.ShowFormDialog(this, form) == DialogResult.OK)
                    {
                        LoadData(); // Refresh if details were changed
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xem chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            // Enable/disable menu items based on order status
                            _ordersMenu.Items["mPay"].Enabled = !string.Equals(ord.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) && !string.Equals(ord.Status, "Completed", StringComparison.OrdinalIgnoreCase);
                            _ordersMenu.Items["mStart"].Enabled = string.Equals(ord.Status, "Reserved", StringComparison.OrdinalIgnoreCase);
                            _ordersMenu.Items["mComplete"].Enabled = string.Equals(ord.Status, "Processing", StringComparison.OrdinalIgnoreCase);
                            _ordersMenu.Items["mCancel"].Enabled = !string.Equals(ord.Status, "Completed", StringComparison.OrdinalIgnoreCase) && !string.Equals(ord.Status, "Cancelled", StringComparison.OrdinalIgnoreCase);
                            _ordersMenu.Show(dgvOrders, new Point(e.X, e.Y));
                        }
                    }
                }
            }
            catch { }
        }

        private void FormOrderList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) { LoadData(); e.Handled = true; }
            else if (e.Control && e.KeyCode == Keys.N) { btnCreate_Click(sender, EventArgs.Empty); e.Handled = true; }
            else if (e.KeyCode == Keys.Enter && dgvOrders?.CurrentRow != null) { OpenDetailForm(); e.Handled = true; }
            else if (e.KeyCode == Keys.Delete && dgvOrders?.CurrentRow != null) { CancelSelected(); e.Handled = true; }
        }

        private void dgvOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOrders.Columns[e.ColumnIndex].DataPropertyName == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();
                switch (status.ToLower())
                {
                    case "reserved":
                        e.CellStyle.BackColor = Color.FromArgb(254, 243, 199); // amber-100
                        e.CellStyle.ForeColor = Color.FromArgb(120, 53, 15);
                        break;
                    case "processing":
                        e.CellStyle.BackColor = Color.FromArgb(219, 234, 254); // blue-100
                        e.CellStyle.ForeColor = Color.FromArgb(30, 58, 138);
                        break;
                    case "completed":
                        e.CellStyle.BackColor = Color.FromArgb(220, 252, 231); // green-100
                        e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52);
                        break;
                    case "cancelled":
                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226); // red-100
                        e.CellStyle.ForeColor = Color.FromArgb(153, 27, 27);
                        break;
                }
            }
        }

        private void InitContextMenu()
        {
            _ordersMenu = new ContextMenuStrip();
            _ordersMenu.Items.Add(new ToolStripMenuItem("Xem chi tiết (Enter)", null, (s, e) => OpenDetailForm()) { Name = "mDetails" });
            _ordersMenu.Items.Add(new ToolStripMenuItem("Thanh toán", null, (s, e) => PaySelected()) { Name = "mPay" });
            _ordersMenu.Items.Add(new ToolStripSeparator());
            _ordersMenu.Items.Add(new ToolStripMenuItem("Nhận khách (Processing)", null, (s, e) => StartProcessingSelected()) { Name = "mStart" });
            _ordersMenu.Items.Add(new ToolStripMenuItem("Hoàn tất (Completed)", null, (s, e) => CompleteSelected()) { Name = "mComplete" });
            _ordersMenu.Items.Add(new ToolStripSeparator());
            _ordersMenu.Items.Add(new ToolStripMenuItem("Hủy đơn (Delete)", null, (s, e) => CancelSelected()) { Name = "mCancel", ForeColor = Color.Red });
            dgvOrders.ContextMenuStrip = _ordersMenu;
        }

        private void StartProcessingSelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow?.DataBoundItem is OrderDTO ord && ord.Status == "Reserved")
                {
                    _orderBLL.StartProcessing(ord.OrderID);
                    ShowInfo($"Đơn {ord.OrderNumber} đã chuyển sang trạng thái Processing.");
                    LoadData();
                }
                else
                {
                    ShowWarning("Chỉ có thể nhận khách cho đơn ở trạng thái 'Reserved'.");
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "StartProcessingSelected"); }
        }

        private void CompleteSelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow?.DataBoundItem is OrderDTO ord && ord.Status == "Processing")
                {
                    _orderBLL.CompleteOrder(ord.OrderID);
                    ShowInfo($"Đơn {ord.OrderNumber} đã được hoàn tất.");
                    LoadData();
                }
                else
                {
                    ShowWarning("Chỉ có thể hoàn tất đơn ở trạng thái 'Processing'.");
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "CompleteSelected"); }
        }

        private void PaySelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow?.DataBoundItem is OrderDTO ord)
                {
                    if (ord.Status == "Cancelled")
                    {
                        ShowWarning("Không thể thanh toán đơn đã hủy.");
                        return;
                    }
                    using (var payment = new FormPayment(ord.OrderID))
                    {
                        if (UIHelper.ShowFormDialog(this, payment) == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "PaySelected"); }
        }

        private void CancelSelected()
        {
            try
            {
                if (dgvOrders?.CurrentRow?.DataBoundItem is OrderDTO ord)
                {
                    if (ord.Status == "Completed" || ord.Status == "Cancelled")
                    {
                        ShowWarning("Không thể hủy đơn hàng đã hoàn thành hoặc đã bị hủy.");
                        return;
                    }
                    if (ShowConfirm($"Bạn có chắc chắn muốn hủy đơn hàng '{ord.OrderNumber}'?", "Xác nhận hủy"))
                    {
                        _orderBLL.CancelOrder(ord.OrderID);
                        ShowInfo("Hủy đơn hàng thành công!");
                        LoadData();
                    }
                }
            }
            catch (Exception ex) { ExceptionHandler.Handle(ex, "CancelSelected"); }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                ApplyFiltersAndPagination();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage * _pageSize < _totalRecords)
            {
                _currentPage++;
                ApplyFiltersAndPagination();
            }
        }

        protected override void CleanupResources()
        {
            try
            {
                // Xóa timer
                _searchDebounceTimer?.Stop();
                _searchDebounceTimer?.Dispose();
                _searchDebounceTimer = null;

                // Xóa context menu
                _ordersMenu?.Dispose();
                _ordersMenu = null;

                // Xóa dữ liệu
                _allData?.Clear();
                _allData = null;

                // Xóa event handlers nếu cần
                if (dgvOrders != null)
                {
                    dgvOrders.CellDoubleClick -= dgvOrders_CellDoubleClick;
                    dgvOrders.MouseDown -= dgvOrders_MouseDown;
                    dgvOrders.CellFormatting -= dgvOrders_CellFormatting;
                }
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
