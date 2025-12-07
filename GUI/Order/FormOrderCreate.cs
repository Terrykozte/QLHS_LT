using System;
using System.Collections.Generic;
using System.Globalization;
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
            BuildRuntimeUI();
            LoadData();
        }

        // Overload to preselect a customer (start ordering from Customer List)
        public FormOrderCreate(CustomerDTO preselectedCustomer, UserDTO user = null) : this(user)
        {
            _selectedCustomer = preselectedCustomer;
            UpdateCustomerLabel();
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
            if (pnlMenuContainer != null) pnlMenuContainer.BackColor = Color.White;
            if (pnlCartContainer != null) pnlCartContainer.BackColor = Color.White;
            if (pnlCustomerInfo != null) pnlCustomerInfo.BackColor = Color.White;
            
            // Button styles
            if (btnAddOrder != null)
            {
            btnAddOrder.FillColor = Color.FromArgb(59, 130, 246);
            btnAddOrder.ForeColor = Color.White;
            }
            if (btnClear != null)
            {
            btnClear.FillColor = Color.FromArgb(239, 68, 68);
            btnClear.ForeColor = Color.White;
            }
        }

        // In case some controls are missing from Designer in certain builds, build minimal UI at runtime
        private void BuildRuntimeUI()
        {
            try
            {
                // Nothing required for now (Designer provides controls). Keep stub to avoid missing method issues.
            }
            catch { }
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
                if (dgvMenu == null) return;
                
                dgvMenu.DataSource = null;
                dgvMenu.DataSource = _allMenuItems;
                
                if (dgvMenu.Columns.Count > 0)
                {
                    if (dgvMenu.Columns["ItemID"] != null)
                        dgvMenu.Columns["ItemID"].HeaderText = "ID";
                    if (dgvMenu.Columns["ItemName"] != null)
                        dgvMenu.Columns["ItemName"].HeaderText = "Tên món";
                    if (dgvMenu.Columns["UnitPrice"] != null)
                        dgvMenu.Columns["UnitPrice"].HeaderText = "Giá";
                    if (dgvMenu.Columns["CategoryName"] != null)
                        dgvMenu.Columns["CategoryName"].HeaderText = "Danh mục";
                    if (dgvMenu.Columns["Description"] != null)
                        dgvMenu.Columns["Description"].HeaderText = "Mô tả";
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi tải danh sách menu: {ex.Message}");
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
                dgvCart.CellEndEdit += DgvCart_CellEndEdit;
            }
        }

        private void FilterMenuItems()
        {
            try
            {
                if (_allMenuItems == null || _allMenuItems.Count == 0) return;
                
                var filtered = _allMenuItems.AsEnumerable();

                // Filter by category
                if (cmbCategory != null && cmbCategory.SelectedIndex > 0)
                {
                    string category = cmbCategory.SelectedItem?.ToString();
                    if (!string.IsNullOrEmpty(category))
                    {
                        filtered = filtered.Where(m => m.CategoryName == category);
                    }
                }

                // Filter by search text
                if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    filtered = filtered.Where(m => m.ItemName != null && m.ItemName.ToLower().Contains(search));
                }

                if (dgvMenu != null)
                {
                    dgvMenu.DataSource = filtered.ToList();
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi lọc menu: {ex.Message}");
            }
        }

        private void DgvMenu_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvMenu == null) return;
                
                dgvMenu.ClearSelection();
                dgvMenu.Rows[e.RowIndex].Selected = true;
                AddMenuItemToCart();
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private void AddMenuItemToCart()
        {
            try
            {
                if (dgvMenu == null || dgvMenu.SelectedRows.Count == 0)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng chọn một món ăn từ danh sách");
                    return;
                }

                var selectedRow = dgvMenu.SelectedRows[0];
                if (selectedRow.Cells["ItemID"]?.Value == null)
                {
                    FormManagementHelper.ShowWarningMessage("Không thể lấy thông tin sản phẩm");
                    return;
                }

                int menuItemId = (int)selectedRow.Cells["ItemID"].Value;
                var menuItem = _allMenuItems?.FirstOrDefault(m => m.ItemID == menuItemId);

                if (menuItem == null)
                {
                    FormManagementHelper.ShowWarningMessage("Không tìm thấy sản phẩm trong danh sách");
                    return;
                }

                int quantity = 1;
                if (nudQuantity != null) 
                {
                    quantity = (int)nudQuantity.Value;
                }
                if (quantity <= 0) quantity = 1;

                _cartService.AddItem(menuItem, quantity);
                if (nudQuantity != null) nudQuantity.Value = 1;
                
                FormManagementHelper.ShowSuccessMessage($"Đã thêm {menuItem.ItemName} vào đơn hàng");
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi thêm sản phẩm vào giỏ: {ex.Message}");
            }
        }

        private void DgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvCart == null) return;
                if (dgvCart.Columns[e.ColumnIndex].Name != "colDelete") return;
                
                var item = dgvCart.Rows[e.RowIndex].DataBoundItem as CartItem;
                if (item == null) return;
                
                if (MessageBox.Show(
                    $"Bạn có chắc muốn xóa '{item.ProductName}' khỏi đơn hàng?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _cartService.RemoveItem(item.ProductId);
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi xóa sản phẩm: {ex.Message}");
            }
        }

        private void DgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvCart == null) return;
                var col = dgvCart.Columns[e.ColumnIndex];
                if (col == null) return;
                if (!string.Equals(col.DataPropertyName ?? col.Name, "Quantity", StringComparison.OrdinalIgnoreCase)) return;

                var row = dgvCart.Rows[e.RowIndex];
                var item = row.DataBoundItem as CartItem;
                if (item == null) return;

                int newQty = item.Quantity;
                // Try parse from cell value in case binding not updated yet
                var cellVal = row.Cells[e.ColumnIndex].Value;
                if (cellVal != null && int.TryParse(cellVal.ToString(), out int parsed))
                {
                    newQty = parsed;
                }

                if (newQty <= 0)
                {
                    _cartService.RemoveItem(item.ProductId);
                }
                else
                {
                    _cartService.UpdateQuantity(item.ProductId, newQty);
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi cập nhật số lượng: {ex.Message}");
            }
        }

        private void RemoveFromCart()
        {
            try
            {
                if (dgvCart == null || dgvCart.SelectedRows.Count == 0)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng chọn sản phẩm để xóa");
                    return;
                }

                var selectedRow = dgvCart.SelectedRows[0];
                if (selectedRow.Cells["ProductId"]?.Value == null)
                {
                    FormManagementHelper.ShowWarningMessage("Không thể lấy thông tin sản phẩm");
                    return;
                }

                int productId = (int)selectedRow.Cells["ProductId"].Value;
                _cartService.RemoveItem(productId);
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi xóa sản phẩm khỏi giỏ: {ex.Message}");
            }
        }

        private void ClearCart()
        {
            if (MessageBox.Show(
                "Bạn có chắc muốn xóa tất cả sản phẩm khỏi đơn hàng?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
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
                if (lblTotal != null)
                {
                    lblTotal.Text = $"Tổng tiền: {total:N0} VNĐ";
                }

                if (btnAddOrder != null)
                {
                    btnAddOrder.Enabled = items.Count > 0;
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi cập nhật giỏ hàng: {ex.Message}");
            }
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = _customerBLL.GetAll() ?? new List<CustomerDTO>();

                // Hỗ trợ khách lẻ: nếu chưa có khách hàng nào, vẫn cho phép tạo đơn
                if (customers.Count == 0)
                {
                    cmbCustomer.DataSource = null;
                    _selectedCustomer = null; // Khách lẻ
                    if (lblCustomer != null) lblCustomer.Text = "Khách hàng: Khách lẻ";
                    return;
                }
                
                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = nameof(CustomerDTO.CustomerName);
                cmbCustomer.ValueMember = nameof(CustomerDTO.CustomerID);
                
                _selectedCustomer = customers.FirstOrDefault();
                    UpdateCustomerLabel();
            }
            catch (Exception ex)
            {
                // Nếu lỗi, vẫn cho phép khách lẻ
                _selectedCustomer = null;
                if (lblCustomer != null) lblCustomer.Text = "Khách hàng: Khách lẻ";
                FormManagementHelper.ShowWarningMessage($"Không tải được danh sách khách hàng. Sẽ dùng khách lẻ. Chi tiết: {ex.Message}");
            }
        }

        private void LoadTables()
        {
            try
            {
                var tblBll = new TableBLL();
                var tables = tblBll.GetAll() ?? new List<TableDTO>();
                
                // Thêm option "Không chọn bàn"
                var tablesWithNone = new List<TableDTO> { new TableDTO { TableID = 0, TableName = "-- Không chọn bàn --" } };
                tablesWithNone.AddRange(tables);
                
                cmbTable.DataSource = tablesWithNone;
                cmbTable.DisplayMember = nameof(TableDTO.TableName);
                cmbTable.ValueMember = nameof(TableDTO.TableID);
                
                if (tables.Count > 0 && lblTable != null)
                {
                    lblTable.Text = "Bàn";
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi tải danh sách bàn: {ex.Message}");
            }
        }

        private void OnCustomerChanged()
        {
            try
            {
                if (cmbCustomer != null && cmbCustomer.SelectedItem != null)
                {
                    _selectedCustomer = cmbCustomer.SelectedItem as CustomerDTO;
                    UpdateCustomerLabel();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thay đổi khách hàng: {ex.Message}");
            }
        }

        private void UpdateCustomerLabel()
        {
            try
            {
                if (lblCustomer != null)
                {
                    if (_selectedCustomer != null)
                        lblCustomer.Text = $"Khách hàng: {_selectedCustomer.CustomerName}";
                    else
                        lblCustomer.Text = "Chưa chọn khách hàng";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật label khách hàng: {ex.Message}");
            }
        }

        private void SelectCustomer()
        {
            try
            {
                using (var dlg = new FormSelectCustomer())
                {
                    if (UIHelper.ShowFormDialog(this, dlg) == DialogResult.OK && dlg.SelectedCustomer != null)
                    {
                        _selectedCustomer = dlg.SelectedCustomer;
                        UpdateCustomerLabel();
                        
                        // Cập nhật combobox để hiển thị khách hàng đã chọn
                        if (cmbCustomer != null && cmbCustomer.Items.Count > 0)
                        {
                            for (int i = 0; i < cmbCustomer.Items.Count; i++)
                            {
                                var customer = cmbCustomer.Items[i] as CustomerDTO;
                                if (customer != null && customer.CustomerID == _selectedCustomer.CustomerID)
                                {
                                    cmbCustomer.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi chọn khách hàng: {ex.Message}");
            }
        }

        private void CreateOrder()
        {
            try
            {
                if (_cartService.IsEmpty())
                {
                    FormManagementHelper.ShowWarningMessage("Giỏ hàng trống. Vui lòng thêm sản phẩm vào đơn hàng.");
                    return;
                }

                // Hỗ trợ khách lẻ: nếu chưa chọn khách, dùng null và tên "Khách lẻ"
                int? customerId = _selectedCustomer?.CustomerID;
                string customerName = _selectedCustomer?.CustomerName ?? "Khách lẻ";
                string customerPhone = _selectedCustomer?.PhoneNumber;

                // Lấy TableID nếu có chọn bàn
                int? tableId = null;
                if (cmbTable != null && cmbTable.SelectedItem != null)
                {
                    var selectedTable = cmbTable.SelectedItem as TableDTO;
                    if (selectedTable != null && selectedTable.TableID > 0)
                    {
                        tableId = selectedTable.TableID;
                    }
                }

                var order = _cartService.CreateOrder(
                    customerId,
                    customerName,
                    customerPhone,
                    tableId,
                    txtNotes?.Text ?? ""
                );

                if (order == null)
                {
                    FormManagementHelper.ShowErrorMessage("Không thể tạo đơn hàng. Vui lòng thử lại.");
                    return;
                }

                int orderId = _orderBLL.Create(order);
                if (orderId > 0)
                {
                    FormManagementHelper.ShowSuccessMessage($"Tạo đơn hàng thành công!\nMã đơn: {orderId}");
                    
                    // Tự động mở màn hình thanh toán ngay sau khi tạo đơn
                    using (var payment = new QLTN_LT.GUI.Order.FormPayment(orderId))
                    {
                        UIHelper.ShowFormDialog(this, payment);
                    }
                    
                    _cartService.Clear();
                    ClearForm();
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    FormManagementHelper.ShowErrorMessage("Lỗi tạo đơn hàng. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi tạo đơn hàng: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            _selectedCustomer = null;
            if (txtNotes != null) txtNotes.Clear();
            if (lblCustomer != null) lblCustomer.Text = "Chưa chọn khách hàng";
            if (cmbCustomer != null && cmbCustomer.Items.Count > 0)
            {
                cmbCustomer.SelectedIndex = 0;
            }
            if (cmbTable != null && cmbTable.Items.Count > 0)
            {
                cmbTable.SelectedIndex = 0;
            }
        }
    }
}
