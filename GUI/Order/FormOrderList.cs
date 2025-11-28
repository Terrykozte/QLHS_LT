using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderList : Form
    {
        private readonly OrderService _orderService;

        public FormOrderList()
        {
            InitializeComponent();
            
            var dbContext = new DatabaseContext();
            var orderRepo = new OrderRepository(dbContext);
            _orderService = new OrderService(orderRepo);
        }

        private void FormOrderList_Load(object sender, EventArgs e)
        {
            SetupControls();
            ConfigureGrid();
            LoadData();
            dgvOrders.CellDoubleClick += dgvOrders_CellDoubleClick;
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
            cmbStatus.SelectedIndex = 0;
        }

        private void ConfigureGrid()
        {
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.Columns.Clear();

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderNumber", HeaderText = "MÃ ĐƠN", Width = 120 });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "OrderDate", HeaderText = "NGÀY ĐẶT", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "CustomerName", HeaderText = "KHÁCH HÀNG", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalAmount", HeaderText = "TỔNG TIỀN", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "TRẠNG THÁI", Width = 120 });
        }

        private async void LoadData()
        {
            try
            {
                var fromDate = dtpFromDate.Value;
                var toDate = dtpToDate.Value;
                var status = cmbStatus.SelectedItem.ToString() == "Tất cả" ? null : cmbStatus.SelectedItem.ToString();
                var keyword = txtSearch.Text;

                var data = await _orderService.GetAllAsync(fromDate, toDate, status, keyword);
                dgvOrders.DataSource = data.ToList();
                lblPageInfo.Text = $"Showing {data.Count()} entries";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải dữ liệu. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var f = new FormOrderCreate())
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private async void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedOrder = (Order)dgvOrders.CurrentRow.DataBoundItem;
            if (selectedOrder.Status == "Completed" || selectedOrder.Status == "Cancelled")
            {
                MessageBox.Show("Không thể hủy đơn hàng đã hoàn thành hoặc đã bị hủy.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = MessageBox.Show($"Bạn có chắc chắn muốn hủy đơn hàng '{selectedOrder.OrderNumber}'?",
                                     "Xác nhận hủy",
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    var result = await _orderService.CancelOrderAsync(selectedOrder.Id);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("Hủy đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Hủy thất bại: " + result.ErrorMessage, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi hệ thống: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnViewDetails_Click(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng để xem chi tiết.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedOrder = (Order)dgvOrders.CurrentRow.DataBoundItem;
            using (var form = new FormOrderDetail(selectedOrder.Id))
            {
                form.ShowDialog(this);
            }
        }

        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnViewDetails_Click(sender, e);
            }
        }
    }
}
