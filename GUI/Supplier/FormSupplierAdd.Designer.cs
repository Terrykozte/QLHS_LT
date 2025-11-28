namespace QLTN_LT.GUI.Supplier
{
    partial class FormSupplierAdd
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtName;
        private Guna.UI2.WinForms.Guna2TextBox txtContactPerson;
        private Guna.UI2.WinForms.Guna2TextBox txtPhone;
        private Guna.UI2.WinForms.Guna2TextBox txtEmail;
        private Guna.UI2.WinForms.Guna2TextBox txtAddress;
        private Guna.UI2.WinForms.Guna2CheckBox chkIsActive;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2BorderlessForm borderlessForm;
        private Guna.UI2.WinForms.Guna2DragControl dragControl;
        private Guna.UI2.WinForms.Guna2ControlBox controlBoxClose;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNameError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPhoneError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNameLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblContactLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPhoneLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEmailLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblAddressLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtName = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtContactPerson = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtPhone = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtEmail = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtAddress = new Guna.UI2.WinForms.Guna2TextBox();
            this.chkIsActive = new Guna.UI2.WinForms.Guna2CheckBox();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.lblNameError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPhoneError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblNameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblContactLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPhoneLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblEmailLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblAddressLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.borderlessForm = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.dragControl = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.controlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(256, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thêm mới nhà cung cấp";
            // 
            // lblNameLabel
            // 
            this.lblNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNameLabel.Location = new System.Drawing.Point(24, 70);
            this.lblNameLabel.Name = "lblNameLabel";
            this.lblNameLabel.Size = new System.Drawing.Size(98, 17);
            this.lblNameLabel.TabIndex = 1;
            this.lblNameLabel.Text = "Tên nhà cung cấp";
            // 
            // txtName
            // 
            this.txtName.BorderRadius = 6;
            this.txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtName.DefaultText = "";
            this.txtName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtName.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtName.Location = new System.Drawing.Point(24, 90);
            this.txtName.Name = "txtName";
            this.txtName.PlaceholderText = "Nhập tên nhà cung cấp";
            this.txtName.Size = new System.Drawing.Size(420, 36);
            this.txtName.TabIndex = 2;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblContactLabel
            // 
            this.lblContactLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblContactLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblContactLabel.Location = new System.Drawing.Point(24, 140);
            this.lblContactLabel.Name = "lblContactLabel";
            this.lblContactLabel.Size = new System.Drawing.Size(80, 17);
            this.lblContactLabel.TabIndex = 3;
            this.lblContactLabel.Text = "Người liên hệ";
            // 
            // txtContactPerson
            // 
            this.txtContactPerson.BorderRadius = 6;
            this.txtContactPerson.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContactPerson.DefaultText = "";
            this.txtContactPerson.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtContactPerson.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtContactPerson.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtContactPerson.Location = new System.Drawing.Point(24, 160);
            this.txtContactPerson.Name = "txtContactPerson";
            this.txtContactPerson.PlaceholderText = "Nhập tên người liên hệ";
            this.txtContactPerson.Size = new System.Drawing.Size(420, 36);
            this.txtContactPerson.TabIndex = 4;
            // 
            // lblPhoneLabel
            // 
            this.lblPhoneLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblPhoneLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPhoneLabel.Location = new System.Drawing.Point(24, 210);
            this.lblPhoneLabel.Name = "lblPhoneLabel";
            this.lblPhoneLabel.Size = new System.Drawing.Size(77, 17);
            this.lblPhoneLabel.TabIndex = 5;
            this.lblPhoneLabel.Text = "Số điện thoại";
            // 
            // txtPhone
            // 
            this.txtPhone.BorderRadius = 6;
            this.txtPhone.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPhone.DefaultText = "";
            this.txtPhone.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtPhone.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPhone.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtPhone.Location = new System.Drawing.Point(24, 230);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.PlaceholderText = "Nhập số điện thoại";
            this.txtPhone.Size = new System.Drawing.Size(200, 36);
            this.txtPhone.TabIndex = 6;
            this.txtPhone.TextChanged += new System.EventHandler(this.txtPhone_TextChanged);
            // 
            // lblEmailLabel
            // 
            this.lblEmailLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblEmailLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblEmailLabel.Location = new System.Drawing.Point(244, 210);
            this.lblEmailLabel.Name = "lblEmailLabel";
            this.lblEmailLabel.Size = new System.Drawing.Size(32, 17);
            this.lblEmailLabel.TabIndex = 7;
            this.lblEmailLabel.Text = "Email";
            // 
            // txtEmail
            // 
            this.txtEmail.BorderRadius = 6;
            this.txtEmail.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEmail.DefaultText = "";
            this.txtEmail.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtEmail.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtEmail.Location = new System.Drawing.Point(244, 230);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.PlaceholderText = "Nhập email";
            this.txtEmail.Size = new System.Drawing.Size(200, 36);
            this.txtEmail.TabIndex = 8;
            // 
            // lblAddressLabel
            // 
            this.lblAddressLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblAddressLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAddressLabel.Location = new System.Drawing.Point(24, 280);
            this.lblAddressLabel.Name = "lblAddressLabel";
            this.lblAddressLabel.Size = new System.Drawing.Size(42, 17);
            this.lblAddressLabel.TabIndex = 9;
            this.lblAddressLabel.Text = "Địa chỉ";
            // 
            // txtAddress
            // 
            this.txtAddress.BorderRadius = 6;
            this.txtAddress.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAddress.DefaultText = "";
            this.txtAddress.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtAddress.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAddress.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtAddress.Location = new System.Drawing.Point(24, 300);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.PlaceholderText = "Nhập địa chỉ";
            this.txtAddress.Size = new System.Drawing.Size(420, 60);
            this.txtAddress.TabIndex = 10;
            // 
            // chkIsActive
            // 
            this.chkIsActive.AutoSize = true;
            this.chkIsActive.Checked = true;
            this.chkIsActive.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.chkIsActive.CheckedState.BorderRadius = 2;
            this.chkIsActive.CheckedState.BorderThickness = 0;
            this.chkIsActive.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIsActive.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkIsActive.Location = new System.Drawing.Point(24, 370);
            this.chkIsActive.Name = "chkIsActive";
            this.chkIsActive.Size = new System.Drawing.Size(83, 19);
            this.chkIsActive.TabIndex = 11;
            this.chkIsActive.Text = "Hoạt động";
            this.chkIsActive.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkIsActive.UncheckedState.BorderRadius = 2;
            this.chkIsActive.UncheckedState.BorderThickness = 0;
            this.chkIsActive.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 6;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(224, 400);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 6;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnCancel.Location = new System.Drawing.Point(344, 400);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblNameError
            // 
            this.lblNameError.BackColor = System.Drawing.Color.Transparent;
            this.lblNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblNameError.ForeColor = System.Drawing.Color.Red;
            this.lblNameError.Location = new System.Drawing.Point(24, 128);
            this.lblNameError.Name = "lblNameError";
            this.lblNameError.Size = new System.Drawing.Size(3, 2);
            this.lblNameError.TabIndex = 14;
            this.lblNameError.Text = null;
            this.lblNameError.Visible = false;
            // 
            // lblPhoneError
            // 
            this.lblPhoneError.BackColor = System.Drawing.Color.Transparent;
            this.lblPhoneError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblPhoneError.ForeColor = System.Drawing.Color.Red;
            this.lblPhoneError.Location = new System.Drawing.Point(24, 268);
            this.lblPhoneError.Name = "lblPhoneError";
            this.lblPhoneError.Size = new System.Drawing.Size(3, 2);
            this.lblPhoneError.TabIndex = 15;
            this.lblPhoneError.Text = null;
            this.lblPhoneError.Visible = false;
            // 
            // borderlessForm
            // 
            this.borderlessForm.ContainerControl = this;
            this.borderlessForm.DockIndicatorTransparencyValue = 0.6D;
            this.borderlessForm.TransparentWhileDrag = true;
            // 
            // dragControl
            // 
            this.dragControl.DockIndicatorTransparencyValue = 0.6D;
            this.dragControl.TargetControl = this;
            this.dragControl.UseTransparentDrag = true;
            // 
            // controlBoxClose
            // 
            this.controlBoxClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlBoxClose.FillColor = System.Drawing.Color.White;
            this.controlBoxClose.IconColor = System.Drawing.Color.Black;
            this.controlBoxClose.Location = new System.Drawing.Point(431, 12);
            this.controlBoxClose.Name = "controlBoxClose";
            this.controlBoxClose.Size = new System.Drawing.Size(25, 25);
            this.controlBoxClose.TabIndex = 16;
            // 
            // FormSupplierAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(468, 460);
            this.Controls.Add(this.controlBoxClose);
            this.Controls.Add(this.lblPhoneError);
            this.Controls.Add(this.lblNameError);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkIsActive);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblAddressLabel);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblEmailLabel);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblPhoneLabel);
            this.Controls.Add(this.txtContactPerson);
            this.Controls.Add(this.lblContactLabel);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblNameLabel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormSupplierAdd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm nhà cung cấp";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
