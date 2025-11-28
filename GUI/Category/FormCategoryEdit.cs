        private readonly Category _category;

        public FormCategoryEdit(Category category)
        {
            InitializeComponent();
            _category = category;
            
            var dbContext = new DatabaseContext();
            var categoryRepo = new CategoryRepository(dbContext);
            _categoryService = new CategoryService(categoryRepo);

            LoadData();
        }

        private void LoadData()
        {
            txtCategoryName.Text = _category.CategoryName;
            txtDescription.Text = _category.Description;
        }

        private async void btnSave_Click(object sender, EventArgs e)
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
                var result = await _categoryService.UpdateAsync(_category);
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
