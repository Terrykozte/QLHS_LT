using System;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.GUI.Base;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodEdit : FormTemplate
    {
        private SeafoodBLL _seafoodBLL;
        private CategoryBLL _categoryBLL;
        private SeafoodDTO _seafood;

        public FormSeafoodEdit(SeafoodDTO seafood)
        {
            InitializeComponent();
            _seafood = seafood ?? throw new ArgumentNullException(nameof(seafood));
            _seafoodBLL = new SeafoodBLL();
            _categoryBLL = new CategoryBLL();

            this.Load += (s, e) =>
            {
                try
                {
                    InitializeEditMode();
                    LoadCategories();
                    LoadData();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, "FormSeafoodEdit_Load");
                    this.Close();
                }
            };
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
                ExceptionHandler.Handle(ex, "FormSeafoodEdit.LoadCategories");
            }
            finally { Wait(false); }
        }

        protected override void LoadData()
        {
            try
            {
                if (_seafood == null) return;
                txtSeafoodName.Text = _seafood.SeafoodName ?? string.Empty;
                txtUnitPrice.Text = _seafood.UnitPrice.ToString("N0");
                txtQuantity.Text = _seafood.Quantity.ToString();
                txtUnit.Text = _seafood.Unit ?? string.Empty;
                txtDescription.Text = _seafood.Description ?? string.Empty;
                cboCategory.SelectedValue = _seafood.CategoryID;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormSeafoodEdit.LoadData");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            if (_seafood == null) throw new InvalidOperationException("Dữ liệu hải sản không hợp lệ.");

            decimal.TryParse(txtUnitPrice.Text, out decimal price);
            int.TryParse(txtQuantity.Text, out int quantity);

            _seafood.SeafoodName = txtSeafoodName.Text.Trim();
            _seafood.UnitPrice = price;
            _seafood.Quantity = quantity;
            _seafood.Unit = txtUnit.Text?.Trim();
            _seafood.Description = txtDescription.Text?.Trim();
            _seafood.CategoryID = (int)cboCategory.SelectedValue;

            _seafoodBLL.Update(_seafood);
        }

        protected override void CleanupResources()
        {
            try
            {
                _seafoodBLL = null;
                _categoryBLL = null;
                _seafood = null;
            }
            catch { }
            finally { base.CleanupResources(); }
        }
    }
}
