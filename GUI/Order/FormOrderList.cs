using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using System.Diagnostics;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderList : BaseForm
    {
        private OrderBLL _orderBLL;
        private Timer _searchDebounceTimer;
        private List<OrderDTO> _allData = new List<OrderDTO>();

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

            // Events
            this.Load += FormOrderList_Load;
            if (txtSearch != null) txtSearch.TextChanged += txtSearch_TextChanged;
            if (btnReload != null) btnReload.Click += btnReload_Click;
            if (btnCreate != null) btnCreate.Click += btnCreate_Click;
            if (btnViewDetails != null) btnViewDetails.Click += btnViewDetails_Click;
            if (btnCancelOrder != null) btnCancelOrder.Click += btnCancelOrder_Click;
        }

        private void FormOrderList_Load(object sender, EventArgs e)
        {
            try
            {
                SetupControls();
                ConfigureGrid();
                LoadData();
                if (dgvOrders != null) dgvOrders.CellDoubleClick += dgvOrders_CellDoubleClick;
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
                    cmbStatus.SelectedIndex = 0;
                    cmbStatus.SelectedIndexChanged += (s, e) => LoadData();
                }

                if (dtpFromDate != null) dtpFromDate.ValueChanged += (s, e) => LoadData();
                if (dtpToDate != null) dtpToDate.ValueChanged += (s, e) => LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting up controls: {ex.Message}");
            }
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

                var data = _orderBLL.GetAll(fromDate, toDate, status, keyword)?.ToList() ?? new List<OrderDTO>();
                _allData = data;

                if (dgvOrders != null)
                    dgvOrders.DataSource = data;

                if (lblPageInfo != null)
                    lblPageInfo.Text = $"Tổng: {data.Count} đơn";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
                MessageBox.Show($"Không thể tải dữ liệu. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
