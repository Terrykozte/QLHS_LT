using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodAdd : Form
    {
        private readonly SeafoodBLL _seafoodBLL;
        private readonly CategoryBLL _categoryBLL;

        public FormSeafoodAdd()
        {
            InitializeComponent();
            _seafoodBLL = new SeafoodBLL();
            _categoryBLL = new CategoryBLL();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _categoryBLL.GetAll();
                cboCategory.DataSource = categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
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

            var newItem = new SeafoodDTO
            {
                SeafoodName = txtSeafoodName.Text.Trim(),
                CategoryID = (int)cboCategory.SelectedValue,
                UnitPrice = price,
                Quantity = quantity,
                Unit = txtUnit.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Status = "Active"
            };

            try
            {
                _seafoodBLL.Insert(newItem);
                MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
