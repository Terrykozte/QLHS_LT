using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Menu
{
    public partial class FormMenuEdit : Form
    {
        private readonly MenuBLL _menuBll = new MenuBLL();
        private readonly CategoryBLL _categoryBll = new CategoryBLL();
        private readonly int _itemId;

        public FormMenuEdit(int itemId = 0)
        {
            InitializeComponent();
            _itemId = itemId;
        }

        private void FormMenuEdit_Load(object sender, EventArgs e)
        {
            LoadCategories();
            if (_itemId > 0)
            {
                this.Text = "Sửa món ăn";
                lblTitle.Text = "Sửa món ăn";
                LoadItemData();
            }
            else
            {
                this.Text = "Thêm món ăn";
                lblTitle.Text = "Thêm món ăn";
                chkIsAvailable.Checked = true;
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

        private void LoadItemData()
        {
            try
            {
                var item = _menuBll.GetItemById(_itemId);
                if (item == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn này.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải thông tin món ăn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                lblNameError.Visible = false;
                lblCategoryError.Visible = false;

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    lblNameError.Text = "Tên món không được để trống.";
                    lblNameError.Visible = true;
                    isValid = false;
                }

                if (cboCategory.SelectedValue == null || !(cboCategory.SelectedValue is int) || (int)cboCategory.SelectedValue <= 0)
                {
                    lblCategoryError.Text = "Vui lòng chọn danh mục.";
                    lblCategoryError.Visible = true;
                    isValid = false;
                }

                if (!isValid) return;

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

                if (_itemId > 0)
                {
                    _menuBll.UpdateItem(menuItem);
                    MessageBox.Show("Cập nhật món ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _menuBll.InsertItem(menuItem);
                    MessageBox.Show("Thêm món ăn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            lblNameError.Visible = false;
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblCategoryError.Visible = false;
        }
    }
}
