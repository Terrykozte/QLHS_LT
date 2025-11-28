using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodEdit : Form
    {
        private readonly SeafoodService _seafoodService;
        private readonly CategoryService _categoryService;
        private readonly Seafood _seafood;

        public FormSeafoodEdit(Seafood seafood)
        {
            InitializeComponent();
            _seafood = seafood;
            
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

            _seafood.SeafoodName = txtSeafoodName.Text.Trim();
            _seafood.UnitPrice = price;
            _seafood.Quantity = quantity;
            _seafood.Unit = txtUnit.Text.Trim();
            _seafood.Description = txtDescription.Text.Trim();
            _seafood.CategoryID = (int)cboCategory.SelectedValue;

            try
            {
                var result = await _seafoodService.UpdateAsync(_seafood);
                if (result)
                {
                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Cập nhật thất bại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
