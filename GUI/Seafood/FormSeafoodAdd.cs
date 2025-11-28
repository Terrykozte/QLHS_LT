using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodAdd : Form
    {
        private readonly SeafoodService _seafoodService;
        private readonly CategoryService _categoryService;

        public FormSeafoodAdd()
        {
            InitializeComponent();
            
            var dbContext = new DatabaseContext();
            var seafoodRepo = new SeafoodRepository(dbContext);
            _seafoodService = new SeafoodService(seafoodRepo);

            var categoryRepo = new CategoryRepository(dbContext);
            _categoryService = new CategoryService(categoryRepo);

            LoadCategories();
        }

        private async void LoadCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                cboCategory.DataSource = categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
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

            var newItem = new Seafood
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
                var result = await _seafoodService.CreateAsync(newItem);
                if (result > 0)
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
