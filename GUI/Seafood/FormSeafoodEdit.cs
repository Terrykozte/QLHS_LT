using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodEdit : Form
    {
        private readonly SeafoodBLL _seafoodBLL;
        private readonly CategoryBLL _categoryBLL;
        private readonly SeafoodDTO _seafood;

        public FormSeafoodEdit(SeafoodDTO seafood)
        {
            InitializeComponent();
            _seafood = seafood;
            _seafoodBLL = new SeafoodBLL();
            _categoryBLL = new CategoryBLL();
            
            LoadCategories();
            LoadData();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryBLL.GetAll();
                cboCategory.DataSource = categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";

                // Set selected item after loading
                cboCategory.SelectedValue = _seafood.CategoryID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void LoadData()
        {
            txtSeafoodName.Text = _seafood.SeafoodName;
            txtUnitPrice.Text = _seafood.UnitPrice.ToString("N0");
            txtQuantity.Text = _seafood.Quantity.ToString();
            txtUnit.Text = _seafood.Unit;
            txtDescription.Text = _seafood.Description;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSeafoodName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên hải sản.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal.TryParse(txtUnitPrice.Text, out decimal price);
            int.TryParse(txtQuantity.Text, out int quantity);

            _seafood.SeafoodName = txtSeafoodName.Text.Trim();
            _seafood.UnitPrice = price;
            _seafood.Quantity = quantity;
            _seafood.Unit = txtUnit.Text.Trim();
            _seafood.Description = txtDescription.Text.Trim();
            _seafood.CategoryID = (int)cboCategory.SelectedValue;

            try
            {
                _seafoodBLL.Update(_seafood);
                MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
