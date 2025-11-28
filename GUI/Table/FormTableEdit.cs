using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableEdit : Form
    {
        private readonly TableBLL _bll = new TableBLL();
        private readonly int _tableId;

        public FormTableEdit(int tableId)
        {
            InitializeComponent();
            _tableId = tableId;
        }

        private void FormTableEdit_Load(object sender, EventArgs e)
        {
            LoadTableData();
        }

        private void LoadTableData()
        {
            try
            {
                var table = _bll.GetById(_tableId);
                if (table == null)
                {
                    MessageBox.Show("Không tìm thấy bàn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                txtName.Text = table.TableName;
            }
            catch (Exception ex)
            { 
                MessageBox.Show($"Không thể tải dữ liệu bàn. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                lblNameError.Visible = false;
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    lblNameError.Text = "Tên bàn không được để trống.";
                    lblNameError.Visible = true;
                    txtName.Focus();
                    return;
                }

                var table = new TableDTO
                {
                    TableID = _tableId,
                    TableName = txtName.Text.Trim()
                };

                _bll.Update(table);
                MessageBox.Show("Cập nhật bàn thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể cập nhật bàn. Chi tiết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            lblNameError.Visible = false;
        }
    }
}
