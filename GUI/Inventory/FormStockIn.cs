using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormStockIn : Form
    {
        private readonly InventoryBLL _inventoryBll = new InventoryBLL();
        private readonly SeafoodBLL _seafoodBll = new SeafoodBLL();

        public FormStockIn()
        {
            InitializeComponent();
        }

        private void FormStockIn_Load(object sender, EventArgs e)
        {
            LoadSeafoodData();
        }

        private void LoadSeafoodData()
        {
            try
            {
                cmbProduct.DataSource = _seafoodBll.GetAll();
                cmbProduct.DisplayMember = "SeafoodName";
                cmbProduct.ValueMember = "SeafoodID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh sách sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                lblProductError.Visible = false;
                lblQuantityError.Visible = false;

                if (cmbProduct.SelectedValue == null || !(cmbProduct.SelectedValue is int) || (int)cmbProduct.SelectedValue <= 0)
                {
                    lblProductError.Text = "Vui lòng chọn một sản phẩm.";
                    lblProductError.Visible = true;
                    isValid = false;
                }

                if (nudQuantity.Value <= 0)
                {
                    lblQuantityError.Text = "Số lượng phải lớn hơn 0.";
                    lblQuantityError.Visible = true;
                    isValid = false;
                }

                if (!isValid) return;

                var transaction = new InventoryTransactionDTO
                {
                    ItemID = (int)cmbProduct.SelectedValue,
                    QuantityIn = (int)nudQuantity.Value,
                    QuantityOut = 0,
                    TransactionType = "In",
                    TransactionDate = DateTime.Now,
                    Notes = txtNotes.Text.Trim(),
                    ProcessedBy = "CurrentUser" // Placeholder, will be replaced with actual user later
                };

                _inventoryBll.AddTransaction(transaction);

                MessageBox.Show("Nhập kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu giao dịch: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblProductError.Visible = false;
        }

        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            lblQuantityError.Visible = false;
        }
    }
}
