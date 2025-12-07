using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;
using QLTN_LT.GUI.Helper;

namespace QLTN_LT.GUI.Table
{
    public partial class FormTableAdd : FormTemplate
    {
        private readonly TableBLL _bll = new TableBLL();

        public FormTableAdd()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            BtnSave_Click(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BtnCancel_Click(sender, e);
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (sender is Control c) UIHelper.ClearValidationError(c);
        }

        private void controlBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
            var newTable = new TableDTO
            {
                TableName = txtName.Text.Trim(),
                Status = "Trống" // Default status
            };

            _bll.Insert(newTable);
        }
    }
}
