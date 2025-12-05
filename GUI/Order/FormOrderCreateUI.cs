using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using Guna.UI2.WinForms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Order
{
    /// <summary>
    /// Form tạo đơn hàng với giao diện đẹp, hiệu ứng và tương tác tốt
    /// </summary>
    public partial class FormOrderCreateUI : BaseForm
    {
        private MenuBLL _menuBLL = new MenuBLL();
        private OrderBLL _orderBLL = new OrderBLL();
        private List<MenuItemDTO> _selectedItems = new List<MenuItemDTO>();
        private decimal _totalAmount = 0;

        // UI Components
        private Guna2DataGridView dgvMenu;
        private Guna2DataGridView dgvSelectedItems;
        private Label lblTotal;
        private Label lblItemCount;
        private TextBox txtSearch;
        private ComboBox cmbCategory;
        private Guna2Button btnAddItem;
        private Guna2Button btnRemoveItem;
        private Guna2Button btnCreateOrder;
        private Guna2Button btnCancel;
        private Panel pnlMenuSection;
        private Panel pnlOrderSection;
        private Label lblMenuTitle;
        private Label lblOrderTitle;
        private PictureBox imgItemImage;
        private Label lblItemDetails;
        private Panel pnlItemPreview;
        private Guna2ProgressBar progressLoading;

        public FormOrderCreateUI()
        {
            // InitializeComponent(); // no designer; UI built fully in code
            this.Text = "Tạo Đơn Hàng Mới";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeHelper.GetBackgroundColor();
            this.FormBorderStyle = FormBorderStyle.None;
            
            SetupUI();
            ApplyThemeAndResponsive();
            LoadMenuItems();
        }

        private void SetupUI()
        {
            // Panel chính
            var pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(0)
            };

            // ===== HEADER =====
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ThemeHelper.Colors.Primary,
                Padding = new Padding(20)
            };

            var lblHeader = new Label
            {
                Text = "TẠO ĐƠN HÀNG MỚI",
                Dock = DockStyle.Left,
                Font = ThemeHelper.GetHeadingFont(16),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlHeader.Controls.Add(lblHeader);

            var btnClose = new Guna2Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 50,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                BorderColor = Color.Transparent,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            btnClose.Click += (s, e) => this.Close();
            pnlHeader.Controls.Add(btnClose);

            pnlMain.Controls.Add(pnlHeader);

            // ===== CONTENT =====
            var pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(15),
                AutoScroll = true
            };

            // ===== LEFT SECTION: MENU =====
            pnlMenuSection = new Panel
            {
                Dock = DockStyle.Left,
                Width = 600,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 10, 0)
            };

            // Menu Title
            lblMenuTitle = new Label
            {
                Text = "🍽️ DANH SÁCH MENU",
                Dock = DockStyle.Top,
                Height = 35,
                Font = ThemeHelper.GetHeadingFont(13),
                ForeColor = ThemeHelper.GetTextColor(),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlMenuSection.Controls.Add(lblMenuTitle);

            // Search & Filter Panel
            var pnlSearchFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtSearch = new TextBox
            {
                Text = "Tìm kiếm sản phẩm...", // placeholder
                Dock = DockStyle.Left,
                Width = 250,
                Height = 35,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 10, 0)
            };
            txtSearch.TextChanged += (s, e) => FilterMenuItems();
            pnlSearchFilter.Controls.Add(txtSearch);

            cmbCategory = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 150,
                BackColor = ThemeHelper.GetSurfaceColor(),
                ForeColor = ThemeHelper.GetTextColor(),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 10, 0)
            };
            cmbCategory.Items.Add("Tất cả");
            cmbCategory.SelectedIndex = 0;
            cmbCategory.SelectedIndexChanged += (s, e) => FilterMenuItems();
            pnlSearchFilter.Controls.Add(cmbCategory);

            pnlMenuSection.Controls.Add(pnlSearchFilter);

            // Menu DataGridView
            dgvMenu = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                BackgroundColor = ThemeHelper.GetBackgroundColor(),
                GridColor = ThemeHelper.GetBorderColor(),
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 0, 0, 10)
            };

            dgvMenu.ColumnHeadersDefaultCellStyle.BackColor = ThemeHelper.Colors.Primary;
            dgvMenu.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMenu.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvMenu.DefaultCellStyle.BackColor = ThemeHelper.GetSurfaceColor();
            dgvMenu.DefaultCellStyle.ForeColor = ThemeHelper.GetTextColor();
            dgvMenu.DefaultCellStyle.SelectionBackColor = ThemeHelper.Colors.PrimaryLight;
            dgvMenu.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvMenu.Columns.Add("ItemID", "ID");
            dgvMenu.Columns.Add("ItemName", "Tên Sản Phẩm");
            dgvMenu.Columns.Add("UnitPrice", "Giá");
            dgvMenu.Columns.Add("UnitName", "Đơn Vị");

            dgvMenu.Columns["ItemID"].Width = 50;
            dgvMenu.Columns["ItemName"].Width = 200;
            dgvMenu.Columns["UnitPrice"].Width = 100;
            dgvMenu.Columns["UnitName"].Width = 80;

            dgvMenu.CellDoubleClick += (s, e) => AddItemToOrder();
            pnlMenuSection.Controls.Add(dgvMenu);

            pnlContent.Controls.Add(pnlMenuSection);

            // ===== RIGHT SECTION: ORDER =====
            pnlOrderSection = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeHelper.GetBackgroundColor(),
                Padding = new Padding(10)
            };

            // Order Title
            lblOrderTitle = new Label
            {
                Text = "🛒 ĐƠN HÀNG",
                Dock = DockStyle.Top,
                Height = 35,
                Font = ThemeHelper.GetHeadingFont(13),
                ForeColor = ThemeHelper.GetTextColor(),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlOrderSection.Controls.Add(lblOrderTitle);

            // Item Preview
            pnlItemPreview = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(10),
                Margin = new Padding(0, 0, 0, 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            imgItemImage = new PictureBox
            {
                Dock = DockStyle.Left,
                Width = 80,
                Height = 80,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 10, 0)
            };
            pnlItemPreview.Controls.Add(imgItemImage);

            lblItemDetails = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9),
                ForeColor = ThemeHelper.GetTextColor(),
                TextAlign = ContentAlignment.TopLeft,
                AutoSize = false,
                // WordWrap not supported; using AutoSize=false
                Text = "Chọn sản phẩm từ danh sách để xem chi tiết"
            };
            pnlItemPreview.Controls.Add(lblItemDetails);

            pnlOrderSection.Controls.Add(pnlItemPreview);

            // Selected Items DataGridView
            dgvSelectedItems = new Guna2DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                BackgroundColor = ThemeHelper.GetBackgroundColor(),
                GridColor = ThemeHelper.GetBorderColor(),
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 0, 0, 10)
            };

            dgvSelectedItems.ColumnHeadersDefaultCellStyle.BackColor = ThemeHelper.Colors.Secondary;
            dgvSelectedItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSelectedItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSelectedItems.DefaultCellStyle.BackColor = ThemeHelper.GetSurfaceColor();
            dgvSelectedItems.DefaultCellStyle.ForeColor = ThemeHelper.GetTextColor();
            dgvSelectedItems.DefaultCellStyle.SelectionBackColor = ThemeHelper.Colors.SecondaryLight;
            dgvSelectedItems.DefaultCellStyle.SelectionForeColor = Color.White;

            dgvSelectedItems.Columns.Add("ItemName", "Sản Phẩm");
            dgvSelectedItems.Columns.Add("UnitPrice", "Giá");
            dgvSelectedItems.Columns.Add("Quantity", "Số Lượng");
            dgvSelectedItems.Columns.Add("LineTotal", "Thành Tiền");
            dgvSelectedItems.Columns.Add("Action", "");

            dgvSelectedItems.Columns["ItemName"].Width = 150;
            dgvSelectedItems.Columns["UnitPrice"].Width = 80;
            dgvSelectedItems.Columns["Quantity"].Width = 80;
            dgvSelectedItems.Columns["LineTotal"].Width = 100;
            dgvSelectedItems.Columns["Action"].Width = 50;

            dgvSelectedItems.CellValueChanged += (s, e) => UpdateOrderTotal();
            pnlOrderSection.Controls.Add(dgvSelectedItems);

            // Total Section
            var pnlTotal = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(15),
                Margin = new Padding(0, 10, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblTotalLabel = new Label
            {
                Text = "Tổng Tiền:",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10),
                ForeColor = ThemeHelper.GetTextSecondaryColor(),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlTotal.Controls.Add(lblTotalLabel);

            lblTotal = new Label
            {
                Text = "0 ₫",
                Dock = DockStyle.Top,
                Height = 35,
                Font = ThemeHelper.GetHeadingFont(18),
                ForeColor = ThemeHelper.Colors.Success,
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlTotal.Controls.Add(lblTotal);

            lblItemCount = new Label
            {
                Text = "Số lượng: 0 sản phẩm",
                Dock = DockStyle.Bottom,
                Height = 20,
                Font = new Font("Segoe UI", 9),
                ForeColor = ThemeHelper.GetTextSecondaryColor(),
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlTotal.Controls.Add(lblItemCount);

            pnlOrderSection.Controls.Add(pnlTotal);

            pnlContent.Controls.Add(pnlOrderSection);
            pnlMain.Controls.Add(pnlContent);

            // ===== FOOTER: BUTTONS =====
            var pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = ThemeHelper.GetSurfaceColor(),
                Padding = new Padding(15),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnCreateOrder = new Guna2Button
            {
                Text = "✓ Tạo Đơn Hàng",
                Dock = DockStyle.Right,
                Width = 150,
                Height = 40,
                BackColor = ThemeHelper.Colors.Success,
                ForeColor = Color.White,
                BorderColor = Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0)
            };
            btnCreateOrder.Click += BtnCreateOrder_Click;
            pnlFooter.Controls.Add(btnCreateOrder);

            btnRemoveItem = new Guna2Button
            {
                Text = "✕ Xóa Item",
                Dock = DockStyle.Right,
                Width = 120,
                Height = 40,
                BackColor = ThemeHelper.Colors.Danger,
                ForeColor = Color.White,
                BorderColor = Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0)
            };
            btnRemoveItem.Click += BtnRemoveItem_Click;
            pnlFooter.Controls.Add(btnRemoveItem);

            btnAddItem = new Guna2Button
            {
                Text = "+ Thêm Item",
                Dock = DockStyle.Right,
                Width = 120,
                Height = 40,
                BackColor = ThemeHelper.Colors.Info,
                ForeColor = Color.White,
                BorderColor = Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0)
            };
            btnAddItem.Click += (s, e) => AddItemToOrder();
            pnlFooter.Controls.Add(btnAddItem);

            btnCancel = new Guna2Button
            {
                Text = "Hủy",
                Dock = DockStyle.Right,
                Width = 100,
                Height = 40,
                BackColor = ThemeHelper.Colors.TextSecondary,
                ForeColor = Color.White,
                BorderColor = Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCancel.Click += (s, e) => this.Close();
            pnlFooter.Controls.Add(btnCancel);

            pnlMain.Controls.Add(pnlFooter);

            // Initialize progress loading indicator
            progressLoading = new Guna2ProgressBar
            {
                Dock = DockStyle.Fill,
                Visible = false,
                Style = ProgressBarStyle.Marquee
            };
            pnlMain.Controls.Add(progressLoading);
            progressLoading.BringToFront();

            this.Controls.Add(pnlMain);
        }

        private void ApplyThemeAndResponsive()
        {
            ThemeHelper.ApplyThemeToForm(this);
            ResponsiveDesignHelper.ApplyResponsiveDesignToForm(this);
        }

        private void LoadMenuItems()
        {
            try
            {
                if (progressLoading != null) progressLoading.Visible = true;
                var items = _menuBLL.GetAllItems();

                if (items != null && items.Count > 0)
                {
                    dgvMenu.DataSource = items;

                    // Load categories
                    var categories = items.Select(x => x.CategoryName).Distinct().ToList();
                    cmbCategory.Items.Clear();
                    cmbCategory.Items.Add("Tất cả");
                    foreach (var cat in categories)
                        cmbCategory.Items.Add(cat);
                    cmbCategory.SelectedIndex = 0;
                }
                else
                {
                    FormManagementHelper.ShowWarningMessage("Không có sản phẩm nào trong menu");
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
            finally
            {
                if (progressLoading != null) progressLoading.Visible = false;
            }
        }

        private void FilterMenuItems()
        {
            try
            {
                var items = _menuBLL.GetAllItems();
                if (items == null || items.Count == 0) return;

                var filtered = items.AsEnumerable();

                // Filter by category
                if (cmbCategory.SelectedIndex > 0)
                    filtered = filtered.Where(x => x.CategoryName == cmbCategory.SelectedItem.ToString());

                // Filter by search
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchText = txtSearch.Text.ToLower();
                    filtered = filtered.Where(x => x.ItemName.ToLower().Contains(searchText));
                }

                dgvMenu.DataSource = filtered.ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi filter: {ex.Message}");
            }
        }

        private void AddItemToOrder()
        {
            try
            {
                if (dgvMenu.SelectedRows.Count == 0)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng chọn sản phẩm");
                    return;
                }

                var row = dgvMenu.SelectedRows[0];
                var itemId = (int)row.Cells["ItemID"].Value;
                var itemName = row.Cells["ItemName"].Value.ToString();
                var unitPrice = (decimal)row.Cells["UnitPrice"].Value;

                // Get the MenuItemDTO from the DataSource
                var menuItem = (dgvMenu.DataSource as List<MenuItemDTO>)?.FirstOrDefault(x => x.ItemID == itemId);
                if (menuItem != null && !_selectedItems.Any(x => x.ItemID == itemId))
                {
                    _selectedItems.Add(menuItem);
                }

                // Hiển thị preview
                lblItemDetails.Text = $"Sản phẩm: {itemName}\nGiá: {unitPrice:N0} ₫\nĐơn vị: {row.Cells["UnitName"].Value}";

                // Thêm vào danh sách
                dgvSelectedItems.Rows.Add(itemName, unitPrice, 1, unitPrice, "Xóa");

                UpdateOrderTotal();
                FormManagementHelper.ShowSuccessMessage("Đã thêm sản phẩm");
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private void BtnRemoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSelectedItems.SelectedRows.Count == 0)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng chọn item để xóa");
                    return;
                }

                dgvSelectedItems.Rows.RemoveAt(dgvSelectedItems.SelectedRows[0].Index);
                UpdateOrderTotal();
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }

        private void UpdateOrderTotal()
        {
            try
            {
                _totalAmount = 0;
                int itemCount = 0;

                foreach (DataGridViewRow row in dgvSelectedItems.Rows)
                {
                    if (row.Cells["UnitPrice"].Value != null && row.Cells["Quantity"].Value != null)
                    {
                        decimal price = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
                        decimal qty = Convert.ToDecimal(row.Cells["Quantity"].Value);
                        decimal lineTotal = price * qty;

                        row.Cells["LineTotal"].Value = lineTotal;
                        _totalAmount += lineTotal;
                        itemCount++;
                    }
                }

                lblTotal.Text = $"{_totalAmount:N0} ₫";
                lblItemCount.Text = $"Số lượng: {itemCount} sản phẩm";

                // Đổi màu nếu tổng tiền > 0
                lblTotal.ForeColor = _totalAmount > 0 ? ThemeHelper.Colors.Success : ThemeHelper.Colors.Warning;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi update total: {ex.Message}");
            }
        }

        private void BtnCreateOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvSelectedItems.Rows.Count == 0)
                {
                    FormManagementHelper.ShowWarningMessage("Vui lòng thêm sản phẩm vào đơn hàng");
                    return;
                }

                if (FormManagementHelper.ShowDeleteConfirmation("tạo đơn hàng này") == DialogResult.Yes)
                {
                    // Build order details from selected items
                    var orderDetails = new List<OrderDetailDTO>();
                    foreach (DataGridViewRow row in dgvSelectedItems.Rows)
                    {
                        if (row.Cells["UnitPrice"].Value != null && row.Cells["Quantity"].Value != null)
                        {
                            // Get the original menu item to get ItemID
                            var itemName = row.Cells["ItemName"].Value?.ToString();
                            var menuItem = _selectedItems.FirstOrDefault(x => x.ItemName == itemName);
                            
                            if (menuItem != null)
                            {
                                orderDetails.Add(new OrderDetailDTO
                                {
                                    SeafoodID = menuItem.ItemID,
                                    SeafoodName = menuItem.ItemName,
                                    ProductName = menuItem.ItemName,
                                    Quantity = Convert.ToInt32(row.Cells["Quantity"].Value),
                                    UnitPrice = Convert.ToDecimal(row.Cells["UnitPrice"].Value)
                                });
                            }
                        }
                    }

                    if (orderDetails.Count == 0)
                    {
                        FormManagementHelper.ShowWarningMessage("Không có sản phẩm hợp lệ để tạo đơn hàng");
                        return;
                    }

                    // Create OrderDTO
                    var order = new OrderDTO
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = _totalAmount,
                        Status = "Pending",
                        Notes = "",
                        TableID = null,
                        CustomerID = null,
                        OrderDetails = orderDetails
                    };

                    // Create order using OrderBLL
                    int orderId = _orderBLL.Create(order);

                    FormManagementHelper.ShowSuccessMessage($"Tạo đơn hàng thành công! Mã đơn: {orderId}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                FormManagementHelper.ShowErrorMessage($"Lỗi: {ex.Message}");
            }
        }
    }
}

