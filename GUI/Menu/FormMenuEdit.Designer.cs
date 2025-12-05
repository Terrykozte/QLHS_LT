namespace QLTN_LT.GUI.Menu
{
    partial class FormMenuEdit
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2TextBox txtName;
        private Guna.UI2.WinForms.Guna2TextBox txtCode;
        private Guna.UI2.WinForms.Guna2ComboBox cboCategory;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudPrice;
        private Guna.UI2.WinForms.Guna2TextBox txtDescription;
        private Guna.UI2.WinForms.Guna2CheckBox chkIsAvailable;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2BorderlessForm borderlessForm;
        private Guna.UI2.WinForms.Guna2DragControl dragControl;
                private Guna.UI2.WinForms.Guna2HtmlLabel lblNameError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCategoryError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNameLabel;
        private Guna.UI2.WinForms.Guna2ControlBox controlBoxClose;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCodeLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCategoryLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblPriceLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDescriptionLabel;

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
            this.txtCode = new Guna.UI2.WinForms.Guna2TextBox();
            this.cboCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            this.nudPrice = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.txtDescription = new Guna.UI2.WinForms.Guna2TextBox();
            this.chkIsAvailable = new Guna.UI2.WinForms.Guna2CheckBox();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.lblNameError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCategoryError = new Guna.UI2.WinForms.Guna2HtmlLabel();
                        this.lblNameLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCodeLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCategoryLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPriceLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblDescriptionLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.borderlessForm = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.dragControl = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.controlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(141, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thêm món ăn";
            // 
            // lblNameLabel
            // 
            this.lblNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNameLabel.Location = new System.Drawing.Point(24, 70);
            this.lblNameLabel.Name = "lblNameLabel";
            this.lblNameLabel.Size = new System.Drawing.Size(66, 17);
            this.lblNameLabel.TabIndex = 1;
            this.lblNameLabel.Text = "Tên món ăn";
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
            this.txtName.PlaceholderText = "Nhập tên món ăn";
            this.txtName.Size = new System.Drawing.Size(420, 36);
            this.txtName.TabIndex = 2;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblNameError
            // 
            this.lblNameError.BackColor = System.Drawing.Color.Transparent;
            this.lblNameError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblNameError.ForeColor = System.Drawing.Color.Red;
            this.lblNameError.Location = new System.Drawing.Point(24, 128);
            this.lblNameError.Name = "lblNameError";
            this.lblNameError.Size = new System.Drawing.Size(3, 2);
            this.lblNameError.TabIndex = 3;
            this.lblNameError.Text = null;
            this.lblNameError.Visible = false;
            // 
            // lblCodeLabel
            // 
            this.lblCodeLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblCodeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCodeLabel.Location = new System.Drawing.Point(24, 140);
            this.lblCodeLabel.Name = "lblCodeLabel";
            this.lblCodeLabel.Size = new System.Drawing.Size(47, 17);
            this.lblCodeLabel.TabIndex = 4;
            this.lblCodeLabel.Text = "Mã món";
            // 
            // txtCode
            // 
            this.txtCode.BorderRadius = 6;
            this.txtCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCode.DefaultText = "";
            this.txtCode.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtCode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCode.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtCode.Location = new System.Drawing.Point(24, 160);
            this.txtCode.Name = "txtCode";
            this.txtCode.PlaceholderText = "Nhập mã món";
            this.txtCode.Size = new System.Drawing.Size(200, 36);
            this.txtCode.TabIndex = 5;
            // 
            // lblCategoryLabel
            // 
            this.lblCategoryLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblCategoryLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCategoryLabel.Location = new System.Drawing.Point(244, 140);
            this.lblCategoryLabel.Name = "lblCategoryLabel";
            this.lblCategoryLabel.Size = new System.Drawing.Size(59, 17);
            this.lblCategoryLabel.TabIndex = 6;
            this.lblCategoryLabel.Text = "Danh mục";
            // 
            // cboCategory
            // 
            this.cboCategory.BackColor = System.Drawing.Color.Transparent;
            this.cboCategory.BorderRadius = 6;
            this.cboCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.cboCategory.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.cboCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cboCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cboCategory.ItemHeight = 30;
            this.cboCategory.Location = new System.Drawing.Point(244, 160);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(200, 36);
            this.cboCategory.TabIndex = 7;
            this.cboCategory.SelectedIndexChanged += new System.EventHandler(this.cboCategory_SelectedIndexChanged);
            // 
            // lblCategoryError
            // 
            this.lblCategoryError.BackColor = System.Drawing.Color.Transparent;
            this.lblCategoryError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblCategoryError.ForeColor = System.Drawing.Color.Red;
            this.lblCategoryError.Location = new System.Drawing.Point(244, 198);
            this.lblCategoryError.Name = "lblCategoryError";
            this.lblCategoryError.Size = new System.Drawing.Size(3, 2);
            this.lblCategoryError.TabIndex = 8;
            this.lblCategoryError.Text = null;
            this.lblCategoryError.Visible = false;
            // 
            // lblPriceLabel
            // 
            this.lblPriceLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblPriceLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPriceLabel.Location = new System.Drawing.Point(24, 210);
            this.lblPriceLabel.Name = "lblPriceLabel";
            this.lblPriceLabel.Size = new System.Drawing.Size(20, 17);
            this.lblPriceLabel.TabIndex = 9;
            this.lblPriceLabel.Text = "Giá";
            // 
            // nudPrice
            // 
            this.nudPrice.BackColor = System.Drawing.Color.Transparent;
            this.nudPrice.BorderRadius = 6;
            this.nudPrice.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nudPrice.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.nudPrice.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudPrice.Location = new System.Drawing.Point(24, 230);
            this.nudPrice.Maximum = new decimal(new int[] { 100000000, 0, 0, 0 });
            this.nudPrice.Name = "nudPrice";
            this.nudPrice.Size = new System.Drawing.Size(200, 36);
            this.nudPrice.TabIndex = 10;
            // 
            // lblDescriptionLabel
            // 
            this.lblDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblDescriptionLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDescriptionLabel.Location = new System.Drawing.Point(24, 280);
            this.lblDescriptionLabel.Name = "lblDescriptionLabel";
            this.lblDescriptionLabel.Size = new System.Drawing.Size(36, 17);
            this.lblDescriptionLabel.TabIndex = 11;
            this.lblDescriptionLabel.Text = "Mô tả";
            // 
            // txtDescription
            // 
            this.txtDescription.BorderRadius = 6;
            this.txtDescription.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDescription.DefaultText = "";
            this.txtDescription.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDescription.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtDescription.Location = new System.Drawing.Point(24, 300);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.PlaceholderText = "Nhập mô tả món ăn";
            this.txtDescription.Size = new System.Drawing.Size(420, 80);
            this.txtDescription.TabIndex = 12;
            // 
            // chkIsAvailable
            // 
            this.chkIsAvailable.AutoSize = true;
            this.chkIsAvailable.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.chkIsAvailable.CheckedState.BorderRadius = 2;
            this.chkIsAvailable.CheckedState.BorderThickness = 0;
            this.chkIsAvailable.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.chkIsAvailable.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkIsAvailable.Location = new System.Drawing.Point(24, 395);
            this.chkIsAvailable.Name = "chkIsAvailable";
            this.chkIsAvailable.Size = new System.Drawing.Size(83, 19);
            this.chkIsAvailable.TabIndex = 13;
            this.chkIsAvailable.Text = "Có sẵn bán";
            this.chkIsAvailable.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.chkIsAvailable.UncheckedState.BorderRadius = 2;
            this.chkIsAvailable.UncheckedState.BorderThickness = 0;
            this.chkIsAvailable.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 6;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(224, 430);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 6;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnCancel.Location = new System.Drawing.Point(344, 430);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.controlBoxClose.Click += new System.EventHandler(this.controlBoxClose_Click);
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
            // FormMenuEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(468, 490);
            this.Controls.Add(this.controlBoxClose);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkIsAvailable);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblDescriptionLabel);
            this.Controls.Add(this.nudPrice);
            this.Controls.Add(this.lblPriceLabel);
            this.Controls.Add(this.lblCategoryError);
            this.Controls.Add(this.cboCategory);
            this.Controls.Add(this.lblCategoryLabel);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCodeLabel);
            this.Controls.Add(this.lblNameError);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblNameLabel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMenuEdit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thêm/Sửa món ăn";
            this.Load += new System.EventHandler(this.FormMenuEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
