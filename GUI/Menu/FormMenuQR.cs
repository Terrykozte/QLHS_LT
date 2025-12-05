using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using QRCoder;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Utilities;
using QLTN_LT.DAL;
using QLTN_LT.GUI.Controls;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuQR : BaseForm
    {
        private readonly MenuBLL _menuBLL = new MenuBLL();
        private readonly CartService _cart = new CartService();
        private bool _cardView = true;
        private string _selectedCategory;
        private FlowLayoutPanel _catTabs;
        private Guna2Button _btnToggleView;
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();

        private Timer _searchTimer;
        private ContextMenuStrip _qrMenu;
        private bool _isGenerating = false;

        // Hardcode account info as requested
        private const string VietQR_BankCode = "970436"; // Vietcombank (default)
        private const string VietQR_AccountNo = "1031839610";
        private const string VietQR_AccountName = "PHAM HOAI THUONG";

        private string _lastQrPayload;
        private Bitmap _lastQrBitmap;

        public FormMenuQR()
        {
            InitializeComponent();
            this.Load += FormMenuQR_Load;
            this.KeyPreview = true;
            this.KeyDown += FormMenuQR_KeyDown;
            this.FormClosing += FormMenuQR_FormClosing;
            this.Resize += (s, e) => { try { AdjustBreakpoints(); UpdateCardWidths(); } catch { } };
        }

        private void FormMenuQR_Load(object sender, EventArgs e)
        {
            try
            {
                // Debounce search
                _searchTimer = new Timer { Interval = 300 };
                _searchTimer.Tick += (s, e2) => { _searchTimer.Stop(); ApplyFilter(); };

                // Keyboard nav for grids
                KeyboardNavigationHelper.RegisterForm(this, new List<Control>
                {
                    txtSearch, dgvMenu, nudQty, btnAddToCart, dgvCart, btnGenerateQR
                });

                // Animations on show
                AnimationHelper.FadeIn(this, 250);

                // Setup QR context menu
                SetupQrContextMenu();

                // Wire fixed QR buttons
                try
                {
                    if (btnCopyPayload != null) btnCopyPayload.Click += (s, e2) => CopyPayloadToClipboard();
                    if (btnSaveQR != null) btnSaveQR.Click += (s, e2) => SaveQrToFile();
                }
                catch { }

                // Toggle Card/Grid button
                try
                {
                    _btnToggleView = new Guna2Button
                    {
                        Text = "Grid",
                        Width = 70,
                        Height = 30,
                        BorderRadius = 6
                    };
                    _btnToggleView.Left = Math.Max(0, txtSearch.Right - _btnToggleView.Width);
                    _btnToggleView.Top = txtSearch.Top + 5;
                    _btnToggleView.Click += (s, e2) => ToggleView();
                    pnlLeft.Controls.Add(_btnToggleView);
                }
                catch { }

                BuildCategoryTabs();

                LoadMenu();
                SetupGridStyles();
                txtSearch.Focus();

                // Responsive tweaks
                ApplyResponsiveDesign();
                AdjustBreakpoints();
                UpdateCardWidths();

                _cart.CartChanged += (s, ev) => RefreshCart();

                // Events
                dgvMenu.CellDoubleClick += (s, e2) => { btnAddToCart_Click(s, EventArgs.Empty); };

                // Reservation UI toggle
                if (chkReserve != null) chkReserve.CheckedChanged += (s2, e2) => UpdateReserveUI();
                UpdateReserveUI();

                // Style QR info box
                try
                {
                    if (txtQRInfo != null)
                    {
                        txtQRInfo.Font = new Font("Consolas", 9f);
                        txtQRInfo.FillColor = Color.FromArgb(245, 247, 250);
                        txtQRInfo.ForeColor = Color.FromArgb(31, 41, 55);
                        txtQRInfo.PlaceholderText = "Thông tin thanh toán sẽ hiển thị tại đây...";
                    }
                    if (picQR != null)
                    {
                        picQR.BorderStyle = BorderStyle.FixedSingle;
                        picQR.BackColor = Color.White;
                    }
                }
                catch { }

                // Initial layout
                pnlRight_Resize(pnlRight, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi tải Menu QR: {ex.Message}");
            }
        }

        private void pnlRight_Resize(object sender, EventArgs e)
        {
            try
            {
                // Layout right panel responsively
                int padding = 10;
                int width = pnlRight.ClientSize.Width;
                int height = pnlRight.ClientSize.Height;

                // Cart grid on top
                dgvCart.Left = padding;
                dgvCart.Top = 20;
                dgvCart.Width = width - padding * 2;
                dgvCart.Height = Math.Max(240, (int)(height * 0.45));

                // Quantity and add button
                nudQty.Top = dgvCart.Bottom + 10;
                nudQty.Left = padding;
                btnAddToCart.Top = nudQty.Top;
                btnAddToCart.Left = nudQty.Right + 8;

                // Total and Generate button
                lblTotal.Top = btnAddToCart.Bottom + 10;
                lblTotal.Left = padding;
                btnGenerateQR.Top = lblTotal.Top - 6;
                btnGenerateQR.Left = width - btnGenerateQR.Width - padding;

                // QR and info at bottom
                int bottomHeight = height - (lblTotal.Bottom + 10) - padding;
                int qrWidth = Math.Max(260, (int)(width * 0.45));
                int infoWidth = Math.Max(180, width - qrWidth - padding * 3);

                picQR.Top = lblTotal.Bottom + 10;
                picQR.Left = padding;
                picQR.Width = qrWidth;
                picQR.Height = bottomHeight;

                txtQRInfo.Top = picQR.Top;
                txtQRInfo.Left = picQR.Right + padding;
                txtQRInfo.Width = infoWidth;
                txtQRInfo.Height = bottomHeight;

                // Place fixed QR action buttons at bottom-right
                if (btnSaveQR != null && btnCopyPayload != null)
                {
                    int btnTop = picQR.Bottom - btnSaveQR.Height - 4;
                    btnSaveQR.Top = btnTop;
                    btnSaveQR.Left = width - btnSaveQR.Width - padding;

                    btnCopyPayload.Top = btnTop;
                    btnCopyPayload.Left = btnSaveQR.Left - btnCopyPayload.Width - 8;
                }
            }
            catch { }
        }

        private void ApplyResponsiveDesign()
        {
            try
            {
                // Adjust grid row heights
                if (dgvMenu != null)
                {
                    dgvMenu.RowTemplate.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 32);
                    ResponsiveHelper.AdjustDataGridViewColumns(dgvMenu, this);
                }
                if (dgvCart != null)
                {
                    dgvCart.RowTemplate.Height = ResponsiveHelper.GetResponsiveRowHeight(this, 32);
                    ResponsiveHelper.AdjustDataGridViewColumns(dgvCart, this);
                }

                // Adjust font sizes
                lblCart.Font = new Font(lblCart.Font.FontFamily, ResponsiveHelper.GetResponsiveFontSize(11, this), FontStyle.Bold);
                lblFound.Font = new Font(lblFound.Font.FontFamily, ResponsiveHelper.GetResponsiveFontSize(9, this));
                lblTotal.Font = new Font(lblTotal.Font.FontFamily, ResponsiveHelper.GetResponsiveFontSize(11, this), FontStyle.Bold);
            }
            catch { }
        }

        private void SetupQrContextMenu()
        {
            _qrMenu = new ContextMenuStrip();
            _qrMenu.Items.Add("Lưu ảnh QR...").Name = "save";
            _qrMenu.Items.Add("Sao chép ảnh QR").Name = "copyimg";
            _qrMenu.Items.Add("Sao chép chuỗi thanh toán").Name = "copypayload";
            _qrMenu.Items.Add(new ToolStripSeparator());
            _qrMenu.Items.Add("Tạo lại QR").Name = "refresh";
            _qrMenu.Items.Add("Xóa QR").Name = "clear";
            _qrMenu.ItemClicked += QrMenu_ItemClicked;
            picQR.ContextMenuStrip = _qrMenu;
        }

        private void QrMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                switch (e.ClickedItem.Name)
                {
                    case "save":
                        SaveQrToFile();
                        break;
                    case "copyimg":
                        CopyQrImageToClipboard();
                        break;
                    case "copypayload":
                        CopyPayloadToClipboard();
                        break;
                    case "refresh":
                        btnGenerateQR_Click(btnGenerateQR, EventArgs.Empty);
                        break;
                    case "clear":
                        ClearQr();
                        break;
                }
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", ex.Message);
            }
        }

        private void SaveQrToFile()
        {
            if (_lastQrBitmap == null)
            {
                UXInteractionHelper.ShowWarning("QR", "Chưa có QR để lưu");
                return;
            }
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image (*.png)|*.png";
                sfd.FileName = $"VietQR_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    _lastQrBitmap.Save(sfd.FileName, ImageFormat.Png);
                    UXInteractionHelper.ShowToast(this, "Đã lưu ảnh QR", 2000,
                        Color.FromArgb(34, 197, 94), Color.White);
                }
            }
        }

        private void CopyQrImageToClipboard()
        {
            if (_lastQrBitmap == null)
            {
                UXInteractionHelper.ShowWarning("QR", "Chưa có QR để sao chép");
                return;
            }
            try
            {
                Clipboard.SetImage(_lastQrBitmap);
                UXInteractionHelper.ShowToast(this, "Đã sao chép ảnh QR vào clipboard", 2000,
                    Color.FromArgb(59, 130, 246), Color.White);
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", "Không thể sao chép ảnh: " + ex.Message);
            }
        }

        private void CopyPayloadToClipboard()
        {
            if (string.IsNullOrEmpty(_lastQrPayload))
            {
                UXInteractionHelper.ShowWarning("QR", "Chưa có dữ liệu QR để sao chép");
                return;
            }
            try
            {
                Clipboard.SetText(_lastQrPayload);
                UXInteractionHelper.ShowToast(this, "Đã sao chép chuỗi thanh toán", 2000,
                    Color.FromArgb(59, 130, 246), Color.White);
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", "Không thể sao chép: " + ex.Message);
            }
        }

        private void ClearQr()
        {
            try
            {
                picQR.Image = null;
                _lastQrPayload = null;
                _notifiedCartChange = false;
                _lastQrBitmap?.Dispose();
                _lastQrBitmap = null;
            }
            catch { }
        }

        private void FormMenuQR_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    txtSearch.Focus();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (txtSearch.Focused || dgvMenu.Focused || nudQty.Focused)
                    {
                        btnAddToCart_Click(sender, EventArgs.Empty);
                    }
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.G)
                {
                    btnGenerateQR_Click(btnGenerateQR, EventArgs.Empty);
                    e.Handled = true;
                }
                else if (e.Control && e.KeyCode == Keys.S)
                {
                    SaveQrToFile();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete && dgvCart?.CurrentRow != null)
                {
                    var item = dgvCart.CurrentRow.DataBoundItem as CartItem;
                    if (item != null) _cart.RemoveItem(item.ProductId);
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void FormMenuQR_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                _searchTimer?.Stop();
                _searchTimer?.Dispose();
                _lastQrBitmap?.Dispose();
                _lastQrBitmap = null;
                KeyboardNavigationHelper.UnregisterForm(this);
            }
            catch { }
        }

        private void UpdateReserveUI()
        {
            bool on = (chkReserve != null && chkReserve.Checked);
            if (lblReserveTime != null) lblReserveTime.Enabled = on;
            if (dtpReserveTime != null) dtpReserveTime.Enabled = on;
            if (lblGuests != null) lblGuests.Enabled = on;
            if (nudGuests != null) nudGuests.Enabled = on;
            if (lblContactName != null) lblContactName.Enabled = on;
            if (txtContactName != null) txtContactName.Enabled = on;
            if (lblContactPhone != null) lblContactPhone.Enabled = on;
            if (txtContactPhone != null) txtContactPhone.Enabled = on;
            if (lblContactNote != null) lblContactNote.Enabled = on;
            if (txtContactNote != null) txtContactNote.Enabled = on;
        }

        private void SetupGridStyles()
        {
            // Setup Menu Grid
            dgvMenu.AutoGenerateColumns = false;
            dgvMenu.Columns.Clear();
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ItemID", HeaderText = "ID", Width = 50 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ItemName", HeaderText = "Tên Món", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "CategoryName", HeaderText = "Danh mục", Width = 120 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "UnitPrice", HeaderText = "Giá", Width = 100, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }});
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.MultiSelect = false;
            dgvMenu.AllowUserToAddRows = false;
            dgvMenu.AllowUserToDeleteRows = false;
            dgvMenu.ReadOnly = true;
            dgvMenu.RowHeadersVisible = false;

            // Setup Cart Grid
            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ProductName", HeaderText = "Tên Sản Phẩm", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "Quantity", HeaderText = "SL", Width = 50, DefaultCellStyle = new DataGridViewCellStyle{ Alignment = DataGridViewContentAlignment.MiddleCenter }});
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "UnitPrice", HeaderText = "Đơn Giá", Width = 100, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }});
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "Total", HeaderText = "Thành Tiền", Width = 120, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }});

            var colDelete = new DataGridViewButtonColumn
            {
                Name = "colDelete",
                HeaderText = "",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                Width = 40,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dgvCart.Columns.Add(colDelete);
            dgvCart.AllowUserToAddRows = false;
            dgvCart.AllowUserToDeleteRows = false;
            dgvCart.ReadOnly = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.CellContentClick += DgvCart_CellContentClick;
        }

        private void LoadMenu()
        {
            _allItems = _menuBLL.GetAllItems() ?? new List<MenuItemDTO>();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var keyword = (txtSearch.Text ?? string.Empty).Trim().ToLower();
            IEnumerable<MenuItemDTO> query = _allItems;
            if (!string.IsNullOrEmpty(_selectedCategory))
            {
                query = query.Where(i => string.Equals(i.CategoryName, _selectedCategory, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(i =>
                    (i.ItemName?.ToLower().Contains(keyword) ?? false) ||
                    (i.CategoryName?.ToLower().Contains(keyword) ?? false) ||
                    (i.ItemCode?.ToLower().Contains(keyword) ?? false));
            }
            var filtered = query.ToList();

            // Grid/Card toggle
            if (dgvMenu != null)
            {
                dgvMenu.DataSource = filtered;
                dgvMenu.Visible = !_cardView;
            }
            if (flpMenuCards != null) flpMenuCards.Visible = _cardView;

            // Card view
            if (_cardView)
            {
                BindMenuCards(filtered);
                UpdateCardWidths();
            }
            lblFound.Text = $"Tìm thấy {filtered.Count} món";
        }

        private void BindMenuCards(List<MenuItemDTO> items)
        {
            if (flpMenuCards == null) return;
            flpMenuCards.SuspendLayout();
            try
            {
                flpMenuCards.Controls.Clear();
                foreach (var it in items)
                {
                    var card = CreateMenuCard(it);
                    flpMenuCards.Controls.Add(card);
                }
            }
            finally
            {
                flpMenuCards.ResumeLayout();
            }
        }

        private Control CreateMenuCard(MenuItemDTO item)
        {
            var card = new CardItemControl
            {
                Title = item.ItemName,
                Subtitle = string.IsNullOrWhiteSpace(item.Description) ? item.CategoryName : item.Description,
                Value = string.Format("{0:N0} VNĐ", item.UnitPrice),
                ActionText = "Chọn",
                Width = 240,
                Height = 150,
                Margin = new Padding(8)
            };

            // Image binding with fallback
            try
            {
                if (!string.IsNullOrWhiteSpace(item.ImagePath) && File.Exists(item.ImagePath))
                {
                    using (var bmp = new Bitmap(item.ImagePath))
                    {
                        card.Icon = new Bitmap(bmp);
                    }
                }
                else
                {
                    card.Icon = SystemIcons.Application.ToBitmap();
                }
            }
            catch { card.Icon = SystemIcons.Application.ToBitmap(); }

            card.ActionButtonClick += (s, e) =>
            {
                try
                {
                    int qty = 1;
                    try { qty = (int)nudQty.Value; } catch { }
                    if (qty <= 0) qty = 1;
                    _cart.AddItem(item, qty);
                }
                catch { }
            };

            card.PlusClicked += (s, e) =>
            {
                try { _cart.AddItem(item, 1); } catch { }
            };
            card.MinusClicked += (s, e) =>
            {
                try { _cart.DecrementItem(item.ItemID, 1); } catch { }
            };

            return card;
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                MenuItemDTO item = null;

                // Try to get from grid if visible
                if (!_cardView && dgvMenu.SelectedRows.Count > 0)
                {
                    var row = dgvMenu.SelectedRows[0];
                    item = row?.DataBoundItem as MenuItemDTO;
                }

                if (item == null)
                {
                    UXInteractionHelper.ShowWarning("Chọn món", "Vui lòng chọn một món từ danh sách hoặc nhấn nút trên thẻ");
                    return;
                }

                var qty = (int)nudQty.Value;
                if (qty <= 0) qty = 1;

                _cart.AddItem(item, qty);
                AnimationHelper.Pulse(btnAddToCart, 200);

                // Show success toast with item details
                UXInteractionHelper.ShowToast(this,
                    $"✓ Đã thêm {qty}x {item.ItemName} ({qty * item.UnitPrice:N0} VNĐ)",
                    2000,
                    Color.FromArgb(34, 197, 94), Color.White);

                nudQty.Value = 1;
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Không thể thêm vào giỏ: {ex.Message}");
            }
        }

        private bool _notifiedCartChange = false;

        private void RefreshCart()
        {
            try
            {
                var items = _cart.GetItems();
                dgvCart.DataSource = null;
                dgvCart.DataSource = items;

                // Calculate and display total with breakdown
                decimal totalAmount = _cart.GetTotalAmount();
                int totalItems = _cart.GetTotalQuantity();
                int itemTypes = items.Count;

                // Format total with breakdown
                string totalDisplay = $"Tổng: {totalAmount:N0} VNĐ";
                if (itemTypes > 0)
                {
                    totalDisplay += $" ({itemTypes} loại, {totalItems} món)";
                }

                lblTotal.Text = totalDisplay;
                lblTotal.ForeColor = totalAmount > 0 ? Color.FromArgb(34, 197, 94) : Color.FromArgb(107, 114, 128);

                // Notify user to re-generate QR when cart changes
                if (_lastQrPayload != null && !_notifiedCartChange)
                {
                    _lastQrPayload = null; // invalidate previous payload
                    _notifiedCartChange = true;
                    UXInteractionHelper.ShowToast(this,
                        $"⚠ Giỏ hàng thay đổi ({totalItems} món, {totalAmount:N0} VNĐ) - Vui lòng tạo lại mã QR",
                        2500,
                        Color.FromArgb(245, 158, 11), Color.White);
                }
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Lỗi cập nhật giỏ hàng: {ex.Message}");
            }
        }

        private void DgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvCart.Columns[e.ColumnIndex].Name == "colDelete")
                {
                    var item = dgvCart.Rows[e.RowIndex].DataBoundItem as CartItem;
                    if (item != null)
                    {
                        var itemName = item.ProductName;
                        var itemQty = item.Quantity;
                        var itemTotal = item.Total;

                        _cart.RemoveItem(item.ProductId);

                        // Show removal toast
                        UXInteractionHelper.ShowToast(this,
                            $"✓ Đã xóa {itemQty}x {itemName} ({itemTotal:N0} VNĐ)",
                            1500,
                            Color.FromArgb(239, 68, 68), Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi", $"Không thể xóa sản phẩm: {ex.Message}");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            _searchTimer?.Stop();
            _searchTimer?.Start();
        }

        private void btnGenerateQR_Click(object sender, EventArgs e)
        {
            try
            {
                if (_isGenerating) return;

                var items = _cart.GetItems();
                var totalAmount = _cart.GetTotalAmount();
                var totalQty = _cart.GetTotalQuantity();

                // Validate cart
                if (items.Count == 0 || totalAmount <= 0)
                {
                    UXInteractionHelper.ShowWarning("Giỏ hàng trống",
                        "Vui lòng thêm ít nhất một sản phẩm vào giỏ hàng trước khi tạo QR");
                    return;
                }

                _isGenerating = true;
                if (sender is Guna2Button btn)
                {
                    UXInteractionHelper.ShowLoadingState(btn, "Đang tạo...");
                }

                // Prefer config from DB if available
                string bankCode = VietQR_BankCode, acc = VietQR_AccountNo, accName = VietQR_AccountName;
                TryLoadVietQrConfig(ref bankCode, ref acc, ref accName);

                var desc = $"Thanh toan MENU {DateTime.Now:HHmmss}";
                var vietQR = new VietQRService(bankCode, acc, accName, totalAmount, desc);
                var data = vietQR.GenerateQRCode();
                _lastQrPayload = data;

                using (var gen = new QRCodeGenerator())
                {
                    var qrData = gen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (var qr = new QRCode(qrData))
                    {
                        _lastQrBitmap?.Dispose();
                        _lastQrBitmap = qr.GetGraphic(10);
                        picQR.Image = _lastQrBitmap;
                        AnimationHelper.ScaleIn(picQR, 250);
                    }
                }

                // Build detailed payment info
                var paymentInfo = vietQR.GeneratePaymentInfo();
                var cartDetails = BuildCartDetails(items);
                txtQRInfo.Text = $"{paymentInfo}\n\n--- CHI TIẾT GIỎ HÀNG ---\n{cartDetails}";
                AnimationHelper.FadeIn(txtQRInfo, 200);

                // Show success with details
                UXInteractionHelper.ShowToast(this,
                    $"✓ QR tạo thành công!\n{items.Count} loại, {totalQty} món, {totalAmount:N0} VNĐ\n(Nhấp chuột phải để lưu/copy)",
                    3000,
                    Color.FromArgb(34, 197, 94), Color.White);

                _notifiedCartChange = false; // Reset notification flag
            }
            catch (Exception ex)
            {
                AnimationHelper.Shake(btnGenerateQR, 300);
                UXInteractionHelper.ShowError("Lỗi tạo QR",
                    $"{ex.Message}\n\nVui lòng kiểm tra:\n- Giỏ hàng không trống\n- Kết nối mạng\n- Cấu hình VietQR");
            }
            finally
            {
                _isGenerating = false;
                if (btnGenerateQR is Guna2Button btn)
                {
                    UXInteractionHelper.HideLoadingState(btn, "Tạo mã QR");
                }
            }
        }

        private string BuildCartDetails(List<CartItem> items)
        {
            try
            {
                var sb = new System.Text.StringBuilder();
                int index = 1;
                foreach (var item in items)
                {
                    sb.AppendLine($"{index}. {item.ProductName}");
                    sb.AppendLine($"   SL: {item.Quantity} x {item.UnitPrice:N0} = {item.Total:N0} VNĐ");
                    index++;
                }
                return sb.ToString();
            }
            catch
            {
                return "Không thể hiển thị chi tiết giỏ hàng";
            }
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            try
            {
                var total = _cart.GetTotalAmount();
                if (total <= 0) { UXInteractionHelper.ShowWarning("Giỏ hàng", "Giỏ hàng trống"); return; }
                var branchCode = (txtBranchCode?.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(branchCode)) { UXInteractionHelper.ShowWarning("Thiếu dữ liệu", "Vui lòng nhập BranchCode"); return; }
                int? tableNo = null;
                try { var val = (int)nudTableNo.Value; if (val > 0) tableNo = val; } catch { }

                // Validate reservation fields if needed
                bool isReserveFlag = (chkReserve != null && chkReserve.Checked);
                if (isReserveFlag)
                {
                    if (string.IsNullOrWhiteSpace(txtContactPhone?.Text))
                    {
                        UXInteractionHelper.ShowWarning("Thiếu dữ liệu", "Vui lòng nhập SĐT liên hệ khi đặt trước");
                        return;
                    }
                    var when = dtpReserveTime?.Value ?? DateTime.Now;
                    if (when < DateTime.Now.AddMinutes(-5))
                    {
                        UXInteractionHelper.ShowWarning("Thời gian không hợp lệ", "Thời gian đặt trước phải lớn hơn hiện tại");
                        return;
                    }
                    int guests = 0; try { guests = (int)nudGuests.Value; } catch { }
                    if (guests <= 0)
                    {
                        UXInteractionHelper.ShowWarning("Thiếu dữ liệu", "Số khách phải >= 1");
                        return;
                    }
                }

                using (var conn = DatabaseHelper.CreateConnection())
                {
                    conn.Open();

                    // Ensure table exists (optional)
                    int? tableId = null;
                    if (tableNo.HasValue)
                    {
                        tableId = EnsureTable(conn, branchCode, tableNo.Value);
                    }

                    // Create order via dbo.sp_InsertOrder to integrate with existing UI
                    bool isReserve = false;
                    DateTime? reserveTime = null;
                    int? guests = null;
                    string contactName = null, contactPhone = null, contactNote = null;
                    try
                    {
                        isReserve = (chkReserve != null && chkReserve.Checked);
                        if (isReserve)
                        {
                            reserveTime = dtpReserveTime?.Value;
                            guests = (int?)((nudGuests != null) ? (int)nudGuests.Value : 0);
                            contactName = txtContactName?.Text;
                            contactPhone = txtContactPhone?.Text;
                            contactNote = txtContactNote?.Text;
                        }
                    }
                    catch { }

                    using (var cmd = new SqlCommand("dbo.sp_InsertOrder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerID", DBNull.Value);
                        if (tableId.HasValue) cmd.Parameters.AddWithValue("@TableID", tableId.Value); else cmd.Parameters.AddWithValue("@TableID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalAmount", total);
                        cmd.Parameters.AddWithValue("@Status", isReserve ? "Reserved" : "Pending");
                        cmd.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
                        var newId = (int)cmd.ExecuteScalar();

                        if (isReserve)
                        {
                            using (var up = new SqlCommand("UPDATE dbo.Orders SET IsReservation=1, ReservationTime=@rt, Guests=@gu, ContactName=@cn, ContactPhone=@cp, ContactNote=@note WHERE OrderID=@id", conn))
                            {
                                up.Parameters.AddWithValue("@rt", (object)reserveTime ?? DBNull.Value);
                                up.Parameters.AddWithValue("@gu", (object)guests ?? DBNull.Value);
                                up.Parameters.AddWithValue("@cn", (object)contactName ?? DBNull.Value);
                                up.Parameters.AddWithValue("@cp", (object)contactPhone ?? DBNull.Value);
                                up.Parameters.AddWithValue("@note", (object)contactNote ?? DBNull.Value);
                                up.Parameters.AddWithValue("@id", newId);
                                up.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Optionally store VietQR payment pending (if QR already generated)
                            TryInsertPendingVietQrPayment(conn, newId, total);
                        }

                        UXInteractionHelper.ShowToast(this, $"Đã tạo đơn hàng #{newId}" + (isReserve?" (Đặt trước)":""), 2500, Color.FromArgb(34, 197, 94), Color.White);
                    }
                }
            }
            catch (Exception ex)
            {
                UXInteractionHelper.ShowError("Lỗi tạo đơn hàng", ex.Message);
            }
        }

        private void TryLoadVietQrConfig(ref string bankCode, ref string accountNo, ref string accountName)
        {
            try
            {
                using (var conn = DatabaseHelper.CreateConnection())
                using (var cmd = new SqlCommand("SELECT TOP 1 BankCode, AccountNo, AccountName FROM dbo.VietQRConfig WHERE IsActive = 1 ORDER BY UpdatedAt DESC", conn))
                {
                    conn.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            bankCode = r.GetString(0);
                            accountNo = r.GetString(1);
                            accountName = r.GetString(2);
                        }
                    }
                }
            }
            catch { }
        }

        private int? EnsureTable(SqlConnection conn, string branchCode, int tableNumber)
        {
            // Find BranchID
            int? branchId = null;
            using (var cmd = new SqlCommand("SELECT BranchID FROM dbo.Branches WHERE BranchCode = @code", conn))
            {
                cmd.Parameters.AddWithValue("@code", branchCode);
                var o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value) return null;
                branchId = Convert.ToInt32(o);
            }

            // Try find existing table
            using (var cmd = new SqlCommand("SELECT TableID FROM dbo.Tables WHERE BranchID = @bid AND TableNumber = @no", conn))
            {
                cmd.Parameters.AddWithValue("@bid", branchId.Value);
                cmd.Parameters.AddWithValue("@no", tableNumber);
                var o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value) return Convert.ToInt32(o);
            }

            // Insert into dbo.Tables if missing
            using (var cmd = new SqlCommand("INSERT INTO dbo.Tables (TableName, BranchID, TableNumber, QrData, Capacity, Status, IsActive)\nVALUES (@name, @bid, @no, @qr, @cap, @status, @act); SELECT SCOPE_IDENTITY();", conn))
            {
                cmd.Parameters.AddWithValue("@name", $"Bàn {tableNumber}");
                cmd.Parameters.AddWithValue("@bid", branchId.Value);
                cmd.Parameters.AddWithValue("@no", tableNumber);
                cmd.Parameters.AddWithValue("@qr", branchCode + '-' + tableNumber);
                cmd.Parameters.AddWithValue("@cap", 4);
                cmd.Parameters.AddWithValue("@status", "Available");
                cmd.Parameters.AddWithValue("@act", 1);
                var o = cmd.ExecuteScalar();
                if (o != null && o != DBNull.Value) return Convert.ToInt32(o);
            }

            // Final attempt to re-query
            using (var cmd = new SqlCommand("SELECT T.TableID FROM dbo.Tables T JOIN dbo.Branches B ON T.BranchID=B.BranchID WHERE B.BranchCode=@code AND T.TableNumber=@no", conn))
            {
                cmd.Parameters.AddWithValue("@code", branchCode);
                cmd.Parameters.AddWithValue("@no", tableNumber);
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? (int?)null : Convert.ToInt32(o);
            }
        }

        private void TryInsertPendingVietQrPayment(SqlConnection conn, int orderId, decimal amount)
        {
            try
            {
                // Ensure PaymentMethodID for VietQR
                int? methodId = null;
                using (var cmd = new SqlCommand("SELECT TOP 1 PaymentMethodID FROM dbo.PaymentMethods WHERE MethodName=N'VietQR' AND IsActive=1", conn))
                {
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value) methodId = Convert.ToInt32(o);
                }
                if (!methodId.HasValue) return;

                // Ensure payload
                string bankCode = VietQR_BankCode, acc = VietQR_AccountNo, accName = VietQR_AccountName;
                TryLoadVietQrConfig(ref bankCode, ref acc, ref accName);
                var desc = $"Thanh toan MENU {DateTime.Now:HHmmss}";
                var vietQR = new VietQRService(bankCode, acc, accName, amount, desc);
                var payload = vietQR.GenerateQRCode();

                using (var cmd = new SqlCommand("dbo.sp_InsertPayment", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OrderID", orderId);
                    cmd.Parameters.AddWithValue("@PaymentMethodID", methodId.Value);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@TransactionCode", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@QRCode", payload);
                    cmd.Parameters.AddWithValue("@BankAccount", acc);
                    cmd.Parameters.AddWithValue("@BankName", bankCode);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    cmd.ExecuteScalar();
            }
            }
            catch { }
        }
        private void ToggleView()
        {
            try
            {
                _cardView = !_cardView;
                if (_btnToggleView != null) _btnToggleView.Text = _cardView ? "Grid" : "Card";
                ApplyFilter();
            }
            catch { }
        }

        private void BuildCategoryTabs()
        {
            try
            {
                var categories = _menuBLL.GetAllCategories() ?? new List<MenuCategoryDTO>();
                if (_catTabs == null)
                {
                    _catTabs = new FlowLayoutPanel
                    {
                        AutoScroll = true,
                        WrapContents = false,
                        Height = 36,
                        Left = 0,
                        Top = 60,
                        Width = pnlLeft.Width - 10,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                    };
                    pnlLeft.Controls.Add(_catTabs);
                }
                else
                {
                    _catTabs.Controls.Clear();
                }

                void addTab(string text, string cat)
                {
                    var b = new Guna2Button
                    {
                        Text = text,
                        Height = 30,
                        Width = Math.Max(80, TextRenderer.MeasureText(text, new Font("Segoe UI", 9)).Width + 24),
                        BorderRadius = 6,
                        Margin = new Padding(6, 3, 0, 3),
                        Tag = cat,
                        FillColor = string.Equals(_selectedCategory, cat, StringComparison.OrdinalIgnoreCase) ? Color.FromArgb(59,130,246) : Color.FromArgb(229,231,235),
                        ForeColor = string.Equals(_selectedCategory, cat, StringComparison.OrdinalIgnoreCase) ? Color.White : Color.FromArgb(31,41,55)
                    };
                    b.Click += (s, e) =>
                    {
                        _selectedCategory = (string)((Guna2Button)s).Tag;
                        // reposition card list below tabs
                        if (flpMenuCards != null)
                        {
                            flpMenuCards.Top = _catTabs.Bottom + 6;
                            flpMenuCards.Height = pnlLeft.Height - flpMenuCards.Top - 10;
                        }
                        ApplyFilter();
                    };
                    _catTabs.Controls.Add(b);
                }

                addTab("Tất cả", null);
                foreach (var c in categories.OrderBy(x => x.DisplayOrder).ThenBy(x => x.CategoryName))
                {
                    addTab(c.CategoryName, c.CategoryName);
                }

                // Adjust positions
                if (flpMenuCards != null)
                {
                    flpMenuCards.Top = _catTabs.Bottom + 6;
                    flpMenuCards.Height = pnlLeft.Height - flpMenuCards.Top - 10;
                }
            }
            catch { }
        }

        private void AdjustBreakpoints()
        {
            try { UpdateCardWidths(); } catch { }
        }

        private int GetCardColumns()
        {
            try
            {
                int w = this.Width;
                if (w <= 1100) return 2; // ~1024
                if (w <= 1600) return 3; // ~1366
                return 4; // >=1920
            }
            catch { return 3; }
        }

        private void UpdateCardWidths()
        {
            try
            {
                if (flpMenuCards == null || !_cardView) return;
                int cols = GetCardColumns();
                int padding = 8;
                int avail = flpMenuCards.ClientSize.Width - padding * (cols + 1);
                int itemW = Math.Max(220, avail / cols);
                foreach (Control c in flpMenuCards.Controls)
                {
                    c.Width = itemW;
                }
                flpMenuCards.PerformLayout();
            }
            catch { }
        }
    }
}


