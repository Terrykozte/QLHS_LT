using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryAdd : FormTemplate
    {
        private CategoryBLL _categoryBLL;

        public FormCategoryAdd()
        {
            InitializeComponent();
            _categoryBLL = new CategoryBLL();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
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
            var newItem = new CategoryDTO
            {
                CategoryName = txtCategoryName.Text.Trim(),
                Description = txtDescription.Text?.Trim(),
                Status = "Active"
            };
            _categoryBLL.Insert(newItem);
        }

        protected override void CleanupResources()
        {
            try { _categoryBLL = null; }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
