using System;
using System.Windows.Forms;
using QLTN_LT.BLL;
using QLTN_LT.DTO;

namespace QLTN_LT.GUI.Customer
{
    public partial class FormCustomerAdd : Form
    {
        private readonly CustomerBLL _bll = new CustomerBLL();

        public FormCustomerAdd()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                lblNameError.Visible = false;
                lblPhoneError.Visible = false;

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    lblNameError.Text = "Tên khách hàng không được để trống.";
                    lblNameError.Visible = true;
                    isValid = false;
                }

                if (string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    lblPhoneError.Text = "Số điện thoại không được để trống.";
                    lblPhoneError.Visible = true;
                    isValid = false;
                }

                if (!isValid)
                {
                    return;
                }

                var newCustomer = new CustomerDTO
                {
                    CustomerName = txtName.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };

                _bll.Insert(newCustomer);

                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            lblPhoneError.Visible = false;
        }
    }
}

