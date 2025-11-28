using System;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodList : Form
    {
        private readonly SeafoodBLL _seafoodBLL;

        public FormSeafoodList()
        {
            InitializeComponent();
            _seafoodBLL = new SeafoodBLL();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var list = _seafoodBLL.GetAll();
                
                string keyword = txtSearch.Text.ToLower();
                if (!string.IsNullOrEmpty(keyword))
                {
                    list = list.Where(s => 
                        s.SeafoodName.ToLower().Contains(keyword) || 
                        s.CategoryName.ToLower().Contains(keyword)
                    ).ToList();
                }

                dgvSeafood.DataSource = list;
                
                // Configure Columns
                if (dgvSeafood.Columns["CategoryID"] != null) dgvSeafood.Columns["CategoryID"].Visible = false;
                if (dgvSeafood.Columns["Description"] != null) dgvSeafood.Columns["Description"].Visible = false;
                if (dgvSeafood.Columns["ImagePath"] != null) dgvSeafood.Columns["ImagePath"].Visible = false;
                if (dgvSeafood.Columns["IsDeleted"] != null) dgvSeafood.Columns["IsDeleted"].Visible = false;
                if (dgvSeafood.Columns["CreatedDate"] != null) dgvSeafood.Columns["CreatedDate"].Visible = false;
                if (dgvSeafood.Columns["UpdatedDate"] != null) dgvSeafood.Columns["UpdatedDate"].Visible = false;
                if (dgvSeafood.Columns["Price"] != null) dgvSeafood.Columns["Price"].Visible = false; // Hide duplicate Price property

                if (dgvSeafood.Columns["SeafoodName"] != null) dgvSeafood.Columns["SeafoodName"].HeaderText = "Tên hải sản";
                if (dgvSeafood.Columns["CategoryName"] != null) dgvSeafood.Columns["CategoryName"].HeaderText = "Danh mục";
                if (dgvSeafood.Columns["UnitPrice"] != null) 
                {
                    dgvSeafood.Columns["UnitPrice"].HeaderText = "Đơn giá";
                    dgvSeafood.Columns["UnitPrice"].DefaultCellStyle.Format = "N0";
                }
                if (dgvSeafood.Columns["Quantity"] != null) dgvSeafood.Columns["Quantity"].HeaderText = "Số lượng";
                if (dgvSeafood.Columns["Unit"] != null) dgvSeafood.Columns["Unit"].HeaderText = "Đơn vị";
                if (dgvSeafood.Columns["Status"] != null) dgvSeafood.Columns["Status"].HeaderText = "Trạng thái";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var form = new FormSeafoodAdd())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
        }

        private void dgvSeafood_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var item = dgvSeafood.Rows[e.RowIndex].DataBoundItem as SeafoodDTO;
                if (item != null)
                {
                    using (var form = new FormSeafoodEdit(item))
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
