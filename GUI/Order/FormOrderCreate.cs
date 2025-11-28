using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderCreate : Form
    {
        private readonly OrderService _orderService;
        private readonly CustomerService _customerService;
        private readonly TableService _tableService;
        private readonly SeafoodService _seafoodService;

        private BindingList<OrderDetailDTO> _currentOrderDetails = new BindingList<OrderDetailDTO>();

        public FormOrderCreate()
        {
            InitializeComponent();
            
            var dbContext = new DatabaseContext();
            _orderService = new OrderService(new OrderRepository(dbContext));
            _customerService = new CustomerService(new CustomerRepository(dbContext));
            _tableService = new TableService(new TableRepository(dbContext));
            _seafoodService = new SeafoodService(new SeafoodRepository(dbContext));
        }

        private async void FormOrderCreate_Load(object sender, EventArgs e)
        {
            await LoadInitialData();
            SetupGrids();
        }

        private async System.Threading.Tasks.Task LoadInitialData()
        {
            try
            {
                var customers = await _customerService.GetAllAsync();
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = "FullName";
                cmbCustomer.ValueMember = "Id";

                var tables = await _tableService.GetAllAsync();
                cmbTable.DataSource = tables;
                cmbTable.DisplayMember = "TableName";
                cmbTable.ValueMember = "Id";

                var seafoods = await _seafoodService.GetAllAsync();
                dgvProducts.DataSource = seafoods;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu ban đầu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupGrids()
        {
            // Products Grid
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "TÊN SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Price", HeaderText = "GIÁ", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvProducts.CellDoubleClick += dgvProducts_CellDoubleClick;

            // Order Details Grid
            dgvOrderDetails.DataSource = _currentOrderDetails;
            dgvOrderDetails.AutoGenerateColumns = false;
            dgvOrderDetails.Columns.Clear();
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SeafoodName", HeaderText = "TÊN SẢN PHẨM", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            dgvOrderDetails.Columns.Add(new DataGridViewButtonColumn { Name = "colDecrease", HeaderText = "", Text = "-", UseColumnTextForButtonValue = true, Width = 30 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "SL", Width = 50 });
            dgvOrderDetails.Columns.Add(new DataGridViewButtonColumn { Name = "colIncrease", HeaderText = "", Text = "+", UseColumnTextForButtonValue = true, Width = 30 });
            dgvOrderDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TotalPrice", HeaderText = "THÀNH TIỀN", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }, ReadOnly = true });
            dgvOrderDetails.Columns.Add(new DataGridViewButtonColumn { Name = "colDelete", HeaderText = "", Text = "Xóa", UseColumnTextForButtonValue = true, Width = 60 });
            dgvOrderDetails.CellContentClick += dgvOrderDetails_CellContentClick;
        }

        private void AddProductToOrder(Seafood selectedProduct)
        {
            if (selectedProduct == null) return;

            var existingDetail = _currentOrderDetails.FirstOrDefault(d => d.SeafoodID == selectedProduct.Id);

            if (existingDetail != null)
            {
                existingDetail.Quantity++;
            }
            else
            {
                _currentOrderDetails.Add(new OrderDetailDTO
                {
                    SeafoodID = selectedProduct.Id,
                    SeafoodName = selectedProduct.SeafoodName,
                    Quantity = 1,
                    UnitPrice = selectedProduct.Price
                });
            }
            _currentOrderDetails.ResetBindings();
            UpdateTotalAmount();
        }

        private async void txtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    var allSeafood = await _seafoodService.GetAllAsync();
                    var filtered = allSeafood.Where(s => s.SeafoodName.ToLower().Contains(txtSearchProduct.Text.ToLower())).ToList();
                    dgvProducts.DataSource = filtered;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                e.SuppressKeyPress = true;
            }
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedProduct = (Seafood)dgvProducts.Rows[e.RowIndex].DataBoundItem;
                AddProductToOrder(selectedProduct);
            }
        }

        private void dgvOrderDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var detail = _currentOrderDetails[e.RowIndex];

            if (dgvOrderDetails.Columns[e.ColumnIndex].Name == "colIncrease")
            {
                detail.Quantity++;
            }
            else if (dgvOrderDetails.Columns[e.ColumnIndex].Name == "colDecrease")
            {
                if (detail.Quantity > 1)
                {
                    detail.Quantity--;
                }
                else
                {
                    _currentOrderDetails.Remove(detail);
                }
            }
            else if (dgvOrderDetails.Columns[e.ColumnIndex].Name == "colDelete")
            {
                _currentOrderDetails.Remove(detail);
            }

            _currentOrderDetails.ResetBindings();
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            decimal total = _currentOrderDetails.Sum(d => d.TotalPrice);
            lblTotalAmount.Text = $"Tổng tiền: {total:N0} VNĐ";
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentOrderDetails.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm sản phẩm vào đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var order = new Order
                {
                    CustomerID = (int?)cmbCustomer.SelectedValue,
                    TableID = (int?)cmbTable.SelectedValue,
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    OrderDetails = _currentOrderDetails.Select(d => new OrderDetail
                    {
                        SeafoodID = d.SeafoodID,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice
                    }).ToList()
                };

                await _orderService.CreateAsync(order);

                MessageBox.Show("Tạo đơn hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
