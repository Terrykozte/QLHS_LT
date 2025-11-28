using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderDetail : Form
    {
        private readonly int _orderId;
        private readonly OrderService _orderService;

        public FormOrderDetail(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            
            var dbContext = new DatabaseContext();
            _orderService = new OrderService(new OrderRepository(dbContext));
        }

        private async void FormOrderDetail_Load(object sender, EventArgs e)
        {
            await LoadOrderDetails();
        }

        private async System.Threading.Tasks.Task LoadOrderDetails()
        {
            try
            {
                var order = await _orderService.GetByIdAsync(_orderId);

                if (order == null)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                lblOrderNumber.Text = $"Mã đơn: {order.OrderNumber}";
                lblCustomerName.Text = $"Khách hàng: {order.CustomerName ?? "Khách lẻ"}";
                lblOrderDate.Text = $"Ngày đặt: {order.OrderDate:dd/MM/yyyy HH:mm}";
                lblStatus.Text = $"Trạng thái: {order.Status}";
                lblTotalAmount.Text = $"Tổng tiền: {order.TotalAmount:N0} VNĐ";

                dgvOrderDetails.DataSource = order.OrderDetails;
                
                dgvOrderDetails.AutoGenerateColumns = false;
                dgvOrderDetails.Columns.Clear();
                dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "TÊN SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
                dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SỐ LƯỢNG", Width = 100 });
                dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitPrice", HeaderText = "ĐƠN GIÁ", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
                dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalPrice", HeaderText = "THÀNH TIỀN", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải chi tiết đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
