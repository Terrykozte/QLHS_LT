using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QRCoder;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuQR : BaseForm
    {
        private readonly MenuBLL _menuBLL = new MenuBLL();
        private readonly CartService _cart = new CartService();
        private List<MenuItemDTO> _allItems = new List<MenuItemDTO>();

        // Hardcode account info as requested
        private const string VietQR_BankCode = "970422"; // Techcombank (default)
        private const string VietQR_AccountNo = "1031839610";
        private const string VietQR_AccountName = "PHAM HOAI THUONG";

        public FormMenuQR()
        {
            InitializeComponent();
            this.Load += FormMenuQR_Load;
        }

        private void FormMenuQR_Load(object sender, EventArgs e)
        {
            try
            {
                LoadMenu();
                SetupGridStyles();
                txtSearch.Focus();
                _cart.CartChanged += (s, ev) => RefreshCart();
                pnlRight_Resize(pnlRight, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải Menu QR: {ex.Message}");
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
            }
            catch { }
        }

        private void SetupGridStyles()
        {
            dgvMenu.AutoGenerateColumns = false;
            dgvMenu.Columns.Clear();
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ItemID", HeaderText = "ID", Width = 60 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ItemName", HeaderText = "Món", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "CategoryName", HeaderText = "Danh mục", Width = 150 });
            dgvMenu.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "UnitPrice", HeaderText = "Giá", Width = 120, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0" }});
            dgvMenu.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMenu.MultiSelect = false;

            dgvCart.AutoGenerateColumns = false;
            dgvCart.Columns.Clear();
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "ProductName", HeaderText = "Món", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "Quantity", HeaderText = "SL", Width = 60 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "UnitPrice", HeaderText = "Đơn giá", Width = 120, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0" }});
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn{ DataPropertyName = "Total", HeaderText = "Thành tiền", Width = 140, DefaultCellStyle = new DataGridViewCellStyle{ Format = "N0" }});

            var colDelete = new DataGridViewButtonColumn{ Name = "colDelete", HeaderText = "", Text = "✕", UseColumnTextForButtonValue = true, Width = 40 };
            dgvCart.Columns.Add(colDelete);
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
            var filtered = string.IsNullOrEmpty(keyword) ? _allItems : _allItems.Where(i =>
                (i.ItemName?.ToLower().Contains(keyword) ?? false) ||
                (i.CategoryName?.ToLower().Contains(keyword) ?? false) ||
                (i.ItemCode?.ToLower().Contains(keyword) ?? false)
            ).ToList();
            dgvMenu.DataSource = filtered;
            lblFound.Text = $"Tìm thấy {filtered.Count} món";
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (dgvMenu.SelectedRows.Count == 0) { MessageBox.Show("Chọn một món"); return; }
            var row = dgvMenu.SelectedRows[0];
            var itemId = (int)row.Cells["ItemID"].Value;
            var item = _allItems.FirstOrDefault(x => x.ItemID == itemId);
            if (item == null) return;
            var qty = (int)nudQty.Value;
            if (qty <= 0) qty = 1;
            _cart.AddItem(item, qty);
            nudQty.Value = 1;
        }

        private void RefreshCart()
        {
            var items = _cart.GetItems();
            dgvCart.DataSource = null;
            dgvCart.DataSource = items;
            lblTotal.Text = $"Tổng: {_cart.GetTotalAmount():N0} VNĐ";
        }

        private void DgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvCart.Columns[e.ColumnIndex].Name == "colDelete")
            {
                var item = dgvCart.Rows[e.RowIndex].DataBoundItem as CartItem;
                if (item != null)
                {
                    _cart.RemoveItem(item.ProductId);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void btnGenerateQR_Click(object sender, EventArgs e)
        {
            try
            {
                var total = _cart.GetTotalAmount();
                if (total <= 0) { MessageBox.Show("Giỏ hàng trống"); return; }

                var desc = $"Thanh toan MENU {DateTime.Now:HHmmss}";
                var vietQR = new VietQRService(VietQR_BankCode, VietQR_AccountNo, VietQR_AccountName, total, desc);
                var data = vietQR.GenerateQRCode();
                using (var gen = new QRCodeGenerator())
                {
                    var qrData = gen.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                    using (var qr = new QRCode(qrData))
                    {
                        var bmp = qr.GetGraphic(10);
                        picQR.Image = bmp;
                    }
                }
                txtQRInfo.Text = vietQR.GeneratePaymentInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo QR: " + ex.Message);
            }
        }
    }
}


