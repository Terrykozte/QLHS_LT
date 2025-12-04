using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Order
{
    public partial class FormOrderCreate : BaseForm
    {
        // Aliases and stub controls to match Designer and code-behind expectations
        private Guna2DataGridView dgvMenu => dgvProducts;
        private Guna2DataGridView dgvCart => dgvOrderDetails;
        private Guna2TextBox txtSearch => txtSearchProduct;
        private Guna2Button btnAddOrder => btnSave;
        private Guna2Button btnClear => btnClose;
        private Guna2HtmlLabel lblTotal => lblTotalAmount;
        private ComboBox cmbCategory = new ComboBox();
        // Optional buttons (currently not present in Designer) are removed to keep code clean
        private TextBox txtNotes = new TextBox();
        private Guna2Panel pnlMenuContainer => pnlLeft;
        private Guna2Panel pnlCartContainer => pnlRight;
        private Guna2Panel pnlCustomerInfo => pnlRight;
        private readonly MenuBLL _menuBLL;
        private readonly OrderBLL _orderBLL;
        private readonly CustomerBLL _customerBLL;
        private readonly CartService _cartService;
        private readonly UserDTO _currentUser;

        private List<MenuItemDTO> _allMenuItems;
        private List<MenuCategoryDTO> _categories;
        private CustomerDTO _selectedCustomer;

        public FormOrderCreate(UserDTO user = null)
        {
            InitializeComponent();
            _currentUser = user;
            _menuBLL = new MenuBLL();
            _orderBLL = new OrderBLL();
            _customerBLL = new CustomerBLL();
            _cartService = new CartService();

            _cartService.CartChanged += CartService_CartChanged;

            try
            {
                UIHelper.ApplyFormStyle(this);
                if (dgvMenu != null) UIHelper.ApplyGridStyle(dgvMenu);
                if (dgvCart != null) UIHelper.ApplyGridStyle(dgvCart);
                if (btnAddOrder != null) UIHelper.ApplyGunaButtonStyle(btnAddOrder, true);
                if (btnClear != null) UIHelper.ApplyGunaButtonStyle(btnClear, false);

            }
            catch { }

            this.KeyPreview = true;
            this.KeyDown += FormOrderCreate_KeyDown;

            ApplyStyles();
            LoadData();
        }

        private void FormOrderCreate_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadCategories();
            LoadMenuItems();
            LoadCustomers();
            LoadTables();
            SetupEventHandlers();
        }

        // Designer-bound handlers
        private void txtSearchProduct_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FilterMenuItems();
                e.Handled = true;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            CreateOrder();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void FormOrderCreate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                LoadMenuItems();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && dgvCart?.SelectedRows?.Count > 0)
            {
                RemoveFromCart();
                e.Handled = true;
            }
        }

        private void ApplyStyles()
        {
            this.BackColor = Color.FromArgb(245, 245, 245);
            
            // Panel styles
            pnlMenuContainer.BackColor = Color.White;
            pnlCartContainer.BackColor = Color.White;
            pnlCustomerInfo.BackColor = Color.White;
            
            // Button styles
            btnAddOrder.FillColor = Color.FromArgb(59, 130, 246);
            btnAddOrder.ForeColor = Color.White;
            btnClear.FillColor = Color.FromArgb(239, 68, 68);
            btnClear.ForeColor = Color.White;
        }

        private void LoadData()
        {
            try
            {
                _allMenuItems = _menuBLL.GetAllItems().ToList();
                _categories = _menuBLL.GetAllCategories().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                cmbCategory.Items.Clear();
                cmbCategory.Items.Add("Tất cả");
                foreach (var category in _categories)
                {
                    cmbCategory.Items.Add(category.CategoryName);
                }
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void LoadMenuItems()
        {
            try
            {
                dgvMenu.DataSource = null;
                dgvMenu.DataSource = _allMenuItems;
                dgvMenu.Columns["ItemID"].HeaderText = "ID";
                dgvMenu.Columns["ItemName"].HeaderText = "Tên món";
                dgvMenu.Columns["UnitPrice"].HeaderText = "Giá";
                dgvMenu.Columns["CategoryName"].HeaderText = "Danh mục";
                dgvMenu.Columns["Description"].HeaderText = "Mô tả";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải menu: " + ex.Message);
            }
        }

        private void SetupEventHandlers()
        {
            cmbCategory.SelectedIndexChanged += (s, e) => FilterMenuItems();
            txtSearch.TextChanged += (s, e) => FilterMenuItems();
            dgvMenu.CellDoubleClick += DgvMenu_CellDoubleClick;
            btnClear.Click += (s, e) => ClearCart();
            btnAddOrder.Click += (s, e) => CreateOrder();

            if (nudQuantity != null)
            {
                nudQuantity.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { AddMenuItemToCart(); e.Handled = true; } };
            }

            if (cmbCustomer != null)
            {
                cmbCustomer.SelectedIndexChanged += (s, e) => OnCustomerChanged();
            }

            if (lblCustomer != null)
            {
                lblCustomer.Cursor = Cursors.Hand;
                lblCustomer.Click += (s, e) => SelectCustomer();
            }

            if (dgvCart != null)
            {
                dgvCart.CellContentClick += DgvCart_CellContentClick;
            }
        }

        private void FilterMenuItems()
        {
            try
            {
                var filtered = _allMenuItems.AsEnumerable();

                // Filter by category
                if (cmbCategory.SelectedIndex > 0)
                {
                    string category = cmbCategory.SelectedItem.ToString();
                    filtered = filtered.Where(m => m.CategoryName == category);
                }

                // Filter by search text
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    filtered = filtered.Where(m => m.ItemName.ToLower().Contains(search));
                }

                dgvMenu.DataSource = filtered.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lọc menu: " + ex.Message);
            }
        }

        private void DgvMenu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                dgvMenu.ClearSelection();
                dgvMenu.Rows[e.RowIndex].Selected = true;
                AddMenuItemToCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void AddMenuItemToCart()
        {
            try
            {
                if (dgvMenu.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn một món ăn");
                    return;
                }

                var selectedRow = dgvMenu.SelectedRows[0];
                int menuItemId = (int)selectedRow.Cells["ItemID"].Value;
                var menuItem = _allMenuItems.FirstOrDefault(m => m.ItemID == menuItemId);

                if (menuItem != null)
                {
                    int quantity = 1;
                    if (nudQuantity != null) quantity = (int)nudQuantity.Value;
                    if (quantity <= 0) quantity = 1;

                    _cartService.AddItem(menuItem, quantity);
                    if (nudQuantity != null) nudQuantity.Value = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thêm vào giỏ: " + ex.Message);
            }
        }

        private void DgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvCart.Columns[e.ColumnIndex].Name != "colDelete") return;
                var item = dgvCart.Rows[e.RowIndex].DataBoundItem as CartItem;
                if (item == null) return;
                if (MessageBox.Show($"Xóa '{item.ProductName}' khỏi giỏ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _cartService.RemoveItem(item.ProductId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void RemoveFromCart()
        {
            try
            {
                if (dgvCart.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để xóa");
                    return;
                }

                var selectedRow = dgvCart.SelectedRows[0];
                int productId = (int)selectedRow.Cells["ProductId"].Value;
                _cartService.RemoveItem(productId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xóa khỏi giỏ: " + ex.Message);
            }
        }

        private void ClearCart()
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa tất cả sản phẩm?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _cartService.Clear();
            }
        }

        private void CartService_CartChanged(object sender, EventArgs e)
        {
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            try
            {
                var items = _cartService.GetItems();
                dgvCart.DataSource = null;
                dgvCart.DataSource = items;

                // Add delete button column once
                if (dgvCart.Columns["colDelete"] == null)
                {
                    var colDel = new DataGridViewButtonColumn
                    {
                        Name = "colDelete",
                        HeaderText = "",
                        Text = "✕",
                        UseColumnTextForButtonValue = true,
                        Width = 40
                    };
                    dgvCart.Columns.Add(colDel);
                }

                decimal total = _cartService.GetTotalAmount();
                lblTotal.Text = $"Tổng tiền: {total:N0} VNĐ";

                btnAddOrder.Enabled = items.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật giỏ: " + ex.Message);
            }
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerBLL.GetAll();
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = nameof(CustomerDTO.CustomerName);
                cmbCustomer.ValueMember = nameof(CustomerDTO.CustomerID);
                if (customers != null && customers.Count > 0)
                {
                    _selectedCustomer = customers.First();
                    UpdateCustomerLabel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải khách hàng: " + ex.Message);
            }
        }

        private void LoadTables()
        {
            try
            {
                var tblBll = new TableBLL();
                var tables = tblBll.GetAll() ?? new List<TableDTO>();
                cmbTable.DataSource = tables;
                cmbTable.DisplayMember = nameof(TableDTO.TableName);
                cmbTable.ValueMember = nameof(TableDTO.TableID);
                if (tables.Count > 0 && lblTable != null)
                {
                    lblTable.Text = "Bàn";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải bàn: " + ex.Message);
            }
        }

        private void OnCustomerChanged()
        {
            try
            {
                _selectedCustomer = cmbCustomer.SelectedItem as CustomerDTO;
                UpdateCustomerLabel();
            }
            catch { }
        }

        private void UpdateCustomerLabel()
        {
            try
            {
                if (_selectedCustomer != null)
                    lblCustomer.Text = $"Khách hàng: {_selectedCustomer.CustomerName}";
                else
                    lblCustomer.Text = "Chưa chọn khách hàng";
            }
            catch { }
        }

        private void SelectCustomer()
        {
            try
            {
                using (var dlg = new FormSelectCustomer())
                {
                    if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedCustomer != null)
                    {
                        _selectedCustomer = dlg.SelectedCustomer;
                        UpdateCustomerLabel();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void CreateOrder()
        {
            try
            {
                if (_cartService.IsEmpty())
                {
                    MessageBox.Show("Giỏ hàng trống");
                    return;
                }

                if (_selectedCustomer == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng");
                    return;
                }

                var order = _cartService.CreateOrder(
                    _selectedCustomer.CustomerID,
                    _selectedCustomer.CustomerName,
                    _selectedCustomer.PhoneNumber,
                    txtNotes.Text
                );

                int orderId = _orderBLL.Create(order);
                if (orderId > 0)
                {
                    if (MessageBox.Show($"Tạo đơn hàng thành công!\nMã đơn: {orderId}\n\nThanh toán ngay?", "Thành công", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        using (var payment = new QLTN_LT.GUI.Order.FormPayment(orderId))
                        {
                            payment.ShowDialog(this);
                        }
                    }
                    _cartService.Clear();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Lỗi tạo đơn hàng");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void ClearForm()
        {
            _selectedCustomer = null;
            txtNotes.Clear();
            lblCustomer.Text = "Chưa chọn khách hàng";
        }
    }
}
