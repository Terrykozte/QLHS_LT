using System;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Category
{
    public partial class FormCategoryList : Form
    {
        private readonly CategoryBLL _categoryBLL;

        public FormCategoryList()
        {
            InitializeComponent();
            _categoryBLL = new CategoryBLL();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _categoryBLL.GetAll();
                
                string keyword = txtSearch.Text.ToLower();
                if (!string.IsNullOrEmpty(keyword))
                {
                    list = list.Where(c => c.CategoryName.ToLower().Contains(keyword)).ToList();
                }

                dgvCategory.DataSource = list;
                
                // Configure Columns
                if (dgvCategory.Columns["IsDeleted"] != null) dgvCategory.Columns["IsDeleted"].Visible = false;
                if (dgvCategory.Columns["CreatedDate"] != null) dgvCategory.Columns["CreatedDate"].Visible = false;
                if (dgvCategory.Columns["UpdatedDate"] != null) dgvCategory.Columns["UpdatedDate"].Visible = false;

                if (dgvCategory.Columns["CategoryName"] != null) dgvCategory.Columns["CategoryName"].HeaderText = "Tên danh mục";
                if (dgvCategory.Columns["Description"] != null) dgvCategory.Columns["Description"].HeaderText = "Mô tả";
                if (dgvCategory.Columns["Status"] != null) dgvCategory.Columns["Status"].HeaderText = "Trạng thái";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormCategoryAdd())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void dgvCategory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var item = dgvCategory.Rows[e.RowIndex].DataBoundItem as CategoryDTO;
                if (item != null)
                {
                    using (var form = new FormCategoryEdit(item))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            LoadData();
                        }
                    }
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
