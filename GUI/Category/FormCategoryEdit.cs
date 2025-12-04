using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryEdit : FormTemplate
    {
        private CategoryBLL _categoryBLL;
        private CategoryDTO _category;

        public FormCategoryEdit(CategoryDTO category)
        {
            InitializeComponent();
            _category = category ?? throw new ArgumentNullException(nameof(category));
            _categoryBLL = new CategoryBLL();

            this.Load += (s, e) =>
            {
                try
                {
                    InitializeEditMode();
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, "FormCategoryEdit_Load");
                    this.Close();
                }
            };
        }

        protected override void LoadData()
        {
            try
            {
                txtCategoryName.Text = _category.CategoryName ?? string.Empty;
                txtDescription.Text = _category.Description ?? string.Empty;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormCategoryEdit.LoadData");
            }
        }

        protected override bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                UIHelper.ShowValidationError(txtCategoryName, "Vui lòng nhập tên danh mục");
                return false;
            }
            UIHelper.ClearValidationError(txtCategoryName);
            return true;
        }

        protected override void SaveData()
        {
            if (_category == null) throw new InvalidOperationException("Dữ liệu danh mục không hợp lệ.");

            _category.CategoryName = txtCategoryName.Text.Trim();
            _category.Description = txtDescription.Text?.Trim();
            _categoryBLL.Update(_category);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        protected override void CleanupResources()
        {
            try
            {
                _categoryBLL = null;
                _category = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
