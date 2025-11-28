using System;
using System.Windows.Forms;
using QLTN_LT.BLL;

namespace QLTN_LT.GUI.Report
{
    public partial class FormReportInventory : Form
    {
        private readonly ReportBLL _bll = new ReportBLL();

        public FormReportInventory()
        {
            InitializeComponent();
        }

        private void FormReportInventory_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgvInventory.DataSource = _bll.GetInventoryStatusReport();
                
                // Optional: Configure columns if needed
                if (dgvInventory.Columns["SeafoodName"] != null) dgvInventory.Columns["SeafoodName"].HeaderText = "TÊN SẢN PHẨM";
                if (dgvInventory.Columns["StockQuantity"] != null) dgvInventory.Columns["StockQuantity"].HeaderText = "TỒN KHO";
                if (dgvInventory.Columns["Unit"] != null) dgvInventory.Columns["Unit"].HeaderText = "ĐƠN VỊ";
                if (dgvInventory.Columns["LastUpdated"] != null) dgvInventory.Columns["LastUpdated"].HeaderText = "CẬP NHẬT LẦN CUỐI";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
