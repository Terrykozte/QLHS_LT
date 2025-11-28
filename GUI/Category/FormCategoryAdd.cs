using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryAdd : Form
    {
        private readonly CategoryBLL _categoryBLL;

        public FormCategoryAdd()
        {
            InitializeComponent();
            _categoryBLL = new CategoryBLL();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên danh mục.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newItem = new CategoryDTO
            {
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                Status = "Active"
            };

            try
            {
                _categoryBLL.Insert(newItem);
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
