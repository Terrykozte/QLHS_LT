using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Inventory
{
    public partial class FormStockIn : BaseForm
    {
        private readonly InventoryBLL _inventoryBll = new InventoryBLL();
        private readonly SeafoodBLL _seafoodBll = new SeafoodBLL();

        public FormStockIn()
        {
            InitializeComponent();
            try { UIHelper.ApplyFormStyle(this); } catch { }
        }

        private void FormStockIn_Load(object sender, EventArgs e)
        {
            LoadSeafoodData();
            this.KeyPreview = true;
            this.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    btnSave_Click(s, EventArgs.Empty);
                    ev.Handled = true;
                }
                else if (ev.KeyCode == System.Windows.Forms.Keys.Escape)
                {
                    this.Close();
                    ev.Handled = true;
                }
            };
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

                // Map seafood -> inventory
                var inv = _inventoryBll.GetBySeafoodId((int)cmbProduct.SelectedValue);
                if (inv == null)
                {
                    MessageBox.Show("Không tìm thấy tồn kho cho sản phẩm này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _inventoryBll.StockIn(inv.InventoryID, (int)nudQuantity.Value, null, txtNotes.Text?.Trim());

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

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
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

        protected override void CleanupResources()
        {
            try { }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
