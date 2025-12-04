using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Helper;
using QLTN_LT.GUI.Base;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodAdd : FormTemplate
    {
        private SeafoodBLL _seafoodBLL;
        private CategoryBLL _categoryBLL;

        public FormSeafoodAdd()
        {
            InitializeComponent();
            _seafoodBLL = new SeafoodBLL();
            _categoryBLL = new CategoryBLL();
            this.Load += (s, e) => { try { LoadCategories(); } catch (Exception ex) { ExceptionHandler.Handle(ex, "LoadCategories"); } };
        }

        private void LoadCategories()
        {
            try
            {
                Wait(true);
                var categories = _categoryBLL.GetAll();
                cboCategory.DataSource = categories;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSeafoodAdd.LoadCategories");
            }
            finally { Wait(false); }
        }

        // Designer wired event - delegate to base template handler
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
            if (string.IsNullOrWhiteSpace(txtSeafoodName.Text))
            {
                UIHelper.ShowValidationError(txtSeafoodName, "Vui lòng nhập tên hải sản");
                return false;
            }
            UIHelper.ClearValidationError(txtSeafoodName);

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(txtUnitPrice.Text, out var price) || price < 0)
            {
                UIHelper.ShowValidationError(txtUnitPrice, "Giá phải là số không âm");
                return false;
            }
            UIHelper.ClearValidationError(txtUnitPrice);

            if (!int.TryParse(txtQuantity.Text, out var quantity) || quantity < 0)
            {
                UIHelper.ShowValidationError(txtQuantity, "Số lượng phải là số không âm");
                return false;
            }
            UIHelper.ClearValidationError(txtQuantity);

            return true;
        }

        protected override void SaveData()
        {
            decimal.TryParse(txtUnitPrice.Text, out decimal price);
            int.TryParse(txtQuantity.Text, out int quantity);

            var newItem = new SeafoodDTO
            {
                SeafoodName = txtSeafoodName.Text.Trim(),
                CategoryID = (int)cboCategory.SelectedValue,
                UnitPrice = price,
                Quantity = quantity,
                Unit = txtUnit.Text?.Trim(),
                Description = txtDescription.Text?.Trim(),
                Status = "Active"
            };

            _seafoodBLL.Insert(newItem);
        }

        protected override void CleanupResources()
        {
            try
            {
                _seafoodBLL = null;
                _categoryBLL = null;
            }
            catch { }
            finally
            {
                base.CleanupResources();
            }
        }
    }
}
