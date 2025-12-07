using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableEdit : FormTemplate
    {
        private readonly TableBLL _bll = new TableBLL();
        private readonly int _tableId;

        public FormTableEdit(int tableId)
        {
            InitializeComponent();
            _tableId = tableId;
            this.Load += FormTableEdit_Load;
        }

        private void FormTableEdit_Load(object sender, EventArgs e)
        {
            InitializeEditMode();
            LoadData();
        }

        protected override void LoadData()
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

        protected override bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                UIHelper.ShowValidationError(txtName, "Tên bàn không được để trống.");
                return false;
            }
            UIHelper.ClearValidationError(txtName);
            return true;
        }

        protected override void SaveData()
        {
            var table = new TableDTO
            {
                TableID = _tableId,
                TableName = txtName.Text.Trim()
            };

            _bll.Update(table);
        }
    }
}
