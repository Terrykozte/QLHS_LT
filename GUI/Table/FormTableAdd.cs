using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableAdd : Form
    {
        private readonly TableBLL _bll = new TableBLL();

        public FormTableAdd()
        {
            InitializeComponent();
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

                var newTable = new TableDTO
                {
                    TableName = txtName.Text.Trim(),
                    Status = "Trống" // Default status
                };

                _bll.Insert(newTable);

                MessageBox.Show("Thêm bàn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu bàn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
