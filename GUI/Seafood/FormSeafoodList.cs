using System;
using System.Linq;
using System.Windows.Forms;
using QLTN_LT.DTO;
using QLTN_LT.BLL;
using QLTN_LT.DAL;

namespace QLTN_LT.GUI.Seafood
{
    public partial class FormSeafoodList : Form
    {
        private readonly SeafoodService _seafoodService;

        public FormSeafoodList()
        {
            InitializeComponent();
            
            var dbContext = new DatabaseContext();
            var seafoodRepo = new SeafoodRepository(dbContext);
            _seafoodService = new SeafoodService(seafoodRepo);

            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                var list = await _seafoodService.GetAllAsync();
                
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
                if (dgvSeafood.Columns["CreatedAt"] != null) dgvSeafood.Columns["CreatedAt"].Visible = false;
                if (dgvSeafood.Columns["UpdatedAt"] != null) dgvSeafood.Columns["UpdatedAt"].Visible = false;

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
                var item = dgvSeafood.Rows[e.RowIndex].DataBoundItem as Seafood;
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
