using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuEdit : FormTemplate
    {
        private readonly MenuBLL _menuBll = new MenuBLL();
        private readonly CategoryBLL _categoryBll = new CategoryBLL();
        private readonly int _itemId;

        public FormMenuEdit(int itemId = 0)
        {
            InitializeComponent();
            _itemId = itemId;
            this.Load += FormMenuEdit_Load;
        }

        private void FormMenuEdit_Load(object sender, EventArgs e)
        {
            try
            {
                LoadCategories();
                if (_itemId > 0)
                {
                    InitializeEditMode();
                    LoadData();
                }
                else
                {
                    InitializeAddMode();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, "FormMenuEdit_Load");
                this.Close();
            }
        }

        private void LoadCategories()
        {
            try
            {
                cboCategory.DataSource = _categoryBll.GetAll();
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải danh mục: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void LoadData()
        {
            var item = _menuBll.GetItemById(_itemId);
            if (item == null)
            {
                ShowError("Không tìm thấy món ăn này.");
                this.Close();
                return;
            }
            txtName.Text = item.ItemName;
            txtCode.Text = item.ItemCode;
            nudPrice.Value = item.UnitPrice;
            cboCategory.SelectedValue = item.CategoryID;
            txtDescription.Text = item.Description;
            chkIsAvailable.Checked = item.IsAvailable;
        }

        protected override void InitializeAddMode()
        {
            base.InitializeAddMode();
            lblTitle.Text = "Thêm món ăn";
            chkIsAvailable.Checked = true;
        }

        protected override void InitializeEditMode()
        {
            base.InitializeEditMode();
            lblTitle.Text = "Sửa món ăn";
        }

        protected override bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                UIHelper.ShowValidationError(txtName, "Tên món không được để trống.");
                return false;
            }
            UIHelper.ClearValidationError(txtName);

            if (cboCategory.SelectedValue == null || !(cboCategory.SelectedValue is int) || (int)cboCategory.SelectedValue <= 0)
            {
                UIHelper.ShowValidationError(cboCategory, "Vui lòng chọn danh mục.");
                return false;
            }
            UIHelper.ClearValidationError(cboCategory);

            return true;
        }

        protected override void SaveData()
        {
            var menuItem = new MenuItemDTO
            {
                ItemID = _itemId,
                ItemName = txtName.Text.Trim(),
                ItemCode = txtCode.Text.Trim(),
                UnitPrice = nudPrice.Value,
                CategoryID = (int)cboCategory.SelectedValue,
                Description = txtDescription.Text.Trim(),
                IsAvailable = chkIsAvailable.Checked
            };

            if (IsEditMode)
            {
                _menuBll.UpdateItem(menuItem);
            }
            else
            {
                _menuBll.InsertItem(menuItem);
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

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control c) UIHelper.ClearValidationError(c);
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Control c) UIHelper.ClearValidationError(c);
        }
    }
}
