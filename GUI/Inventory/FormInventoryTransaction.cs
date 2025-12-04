using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormInventoryTransaction : QLTN_LT.GUI.Base.BaseForm
    {
        private readonly InventoryBLL _inventoryBLL;
        private readonly SupplierBLL _supplierBLL;
        private int _inventoryId;
        private InventoryDTO _inventory;
        private List<SupplierDTO> _suppliers;

        public FormInventoryTransaction(int inventoryId)
        {
            InitializeComponent();
            _inventoryId = inventoryId;
            _inventoryBLL = new InventoryBLL();
            _supplierBLL = new SupplierBLL();
        }

        private void FormInventoryTransaction_Load(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                SetupUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            _inventory = _inventoryBLL.GetBySeafoodId(_inventoryId);
            if (_inventory == null)
                throw new Exception("Không tìm thấy sản phẩm");

            _suppliers = _supplierBLL.GetAll();

            lblSeafoodName.Text = $"Sản phẩm: {_inventory.SeafoodName}";
            lblCurrentQuantity.Text = $"Số lượng hiện tại: {_inventory.Quantity}";
        }

        private void SetupUI()
        {
            // Setup transaction type
            cmbTransactionType.Items.Add("Nhập hàng");
            cmbTransactionType.Items.Add("Xuất hàng");
            cmbTransactionType.Items.Add("Điều chỉnh");
            cmbTransactionType.SelectedIndex = 0;

            // Setup suppliers
            cmbSupplier.DataSource = _suppliers;
            cmbSupplier.DisplayMember = "SupplierName";
            cmbSupplier.ValueMember = "SupplierID";

            // Setup event handlers
            cmbTransactionType.SelectedIndexChanged += CmbTransactionType_SelectedIndexChanged;
        }

        private void CmbTransactionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = cmbTransactionType.SelectedItem?.ToString();
            
            if (selectedType == "Nhập hàng")
            {
                cmbSupplier.Visible = true;
                lblSupplier.Visible = true;
                lblReason.Text = "Lý do nhập:";
            }
            else if (selectedType == "Xuất hàng")
            {
                cmbSupplier.Visible = false;
                lblSupplier.Visible = false;
                lblReason.Text = "Lý do xuất:";
            }
            else if (selectedType == "Điều chỉnh")
            {
                cmbSupplier.Visible = false;
                lblSupplier.Visible = false;
                lblReason.Text = "Lý do điều chỉnh:";
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                string transactionType = cmbTransactionType.SelectedItem?.ToString();
                int quantity = int.Parse(txtQuantity.Text);
                string reason = txtReason.Text;
                int? supplierId = null;

                if (transactionType == "Nhập hàng")
                {
                    supplierId = (int)cmbSupplier.SelectedValue;
                    _inventoryBLL.StockIn(_inventoryId, quantity, supplierId, reason);
                }
                else if (transactionType == "Xuất hàng")
                {
                    _inventoryBLL.StockOut(_inventoryId, quantity, reason);
                }
                else if (transactionType == "Điều chỉnh")
                {
                    _inventoryBLL.AdjustInventory(_inventoryId, quantity, reason);
                }

                MessageBox.Show("Cập nhật kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi cập nhật kho: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Vui lòng nhập số lượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity))
            {
                MessageBox.Show("Số lượng phải là số nguyên", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (quantity <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string transactionType = cmbTransactionType.SelectedItem?.ToString();
            if (transactionType == "Xuất hàng" && quantity > _inventory.Quantity)
            {
                MessageBox.Show($"Số lượng xuất không được vượt quá {_inventory.Quantity}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

