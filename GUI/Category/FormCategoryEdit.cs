using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryEdit : Form
    {
        private readonly CategoryBLL _categoryBLL;
        private readonly CategoryDTO _category;

        public FormCategoryEdit(CategoryDTO category)
        {
            InitializeComponent();
            _category = category;
            _categoryBLL = new CategoryBLL();
            LoadData();
        }

        private void LoadData()
        {
            txtCategoryName.Text = _category.CategoryName;
            txtDescription.Text = _category.Description; // Assuming Description exists in DTO, if not remove
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _category.CategoryName = txtCategoryName.Text.Trim();
            _category.Description = txtDescription.Text.Trim();

            try
            {
                _categoryBLL.Update(_category);
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
