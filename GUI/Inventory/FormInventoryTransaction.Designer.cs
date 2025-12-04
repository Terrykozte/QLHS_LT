namespace QLTN_LT.GUI.Inventory
{
    partial class FormInventoryTransaction
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblSeafoodName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblCurrentQuantity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.label1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.cmbTransactionType = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtQuantity = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblSupplier = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.cmbSupplier = new Guna.UI2.WinForms.Guna2ComboBox();
            this.lblReason = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.txtReason = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.borderlessForm = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.dragControl = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.SuspendLayout();

            // lblSeafoodName
            // 
            // lblSeafoodName
            // 
            this.lblSeafoodName.BackColor = System.Drawing.Color.Transparent;
            this.lblSeafoodName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblSeafoodName.Location = new System.Drawing.Point(24, 20);
            this.lblSeafoodName.Name = "lblSeafoodName";
            this.lblSeafoodName.Size = new System.Drawing.Size(100, 23);
            this.lblSeafoodName.TabIndex = 0;
            this.lblSeafoodName.Text = "Sản phẩm: ";
            // 
            // lblCurrentQuantity
            // 
            this.lblCurrentQuantity.BackColor = System.Drawing.Color.Transparent;
            this.lblCurrentQuantity.Location = new System.Drawing.Point(24, 49);
            this.lblCurrentQuantity.Name = "lblCurrentQuantity";
            this.lblCurrentQuantity.Size = new System.Drawing.Size(100, 17);
            this.lblCurrentQuantity.TabIndex = 1;
            this.lblCurrentQuantity.Text = "Số lượng hiện tại: 0";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(24, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Loại giao dịch:";
            // 
            // cmbTransactionType
            // 
            this.cmbTransactionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTransactionType.BackColor = System.Drawing.Color.Transparent;
            this.cmbTransactionType.BorderRadius = 6;
            this.cmbTransactionType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbTransactionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransactionType.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTransactionType.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbTransactionType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbTransactionType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbTransactionType.ItemHeight = 30;
            this.cmbTransactionType.Location = new System.Drawing.Point(124, 80);
            this.cmbTransactionType.Name = "cmbTransactionType";
            this.cmbTransactionType.Size = new System.Drawing.Size(300, 36);
            this.cmbTransactionType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Location = new System.Drawing.Point(24, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Số lượng:";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuantity.BorderRadius = 6;
            this.txtQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtQuantity.DefaultText = "";
            this.txtQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtQuantity.Location = new System.Drawing.Point(124, 128);
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(300, 36);
            this.txtQuantity.TabIndex = 5;
            // 
            // lblSupplier
            // 
            this.lblSupplier.BackColor = System.Drawing.Color.Transparent;
            this.lblSupplier.Location = new System.Drawing.Point(24, 182);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(80, 17);
            this.lblSupplier.TabIndex = 6;
            this.lblSupplier.Text = "Nhà cung cấp:";
            // 
            // cmbSupplier
            // 
            this.cmbSupplier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSupplier.BackColor = System.Drawing.Color.Transparent;
            this.cmbSupplier.BorderRadius = 6;
            this.cmbSupplier.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSupplier.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbSupplier.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbSupplier.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbSupplier.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbSupplier.ItemHeight = 30;
            this.cmbSupplier.Location = new System.Drawing.Point(124, 176);
            this.cmbSupplier.Name = "cmbSupplier";
            this.cmbSupplier.Size = new System.Drawing.Size(300, 36);
            this.cmbSupplier.TabIndex = 7;
            // 
            // lblReason
            // 
            this.lblReason.BackColor = System.Drawing.Color.Transparent;
            this.lblReason.Location = new System.Drawing.Point(24, 230);
            this.lblReason.Name = "lblReason";
            this.lblReason.Size = new System.Drawing.Size(80, 17);
            this.lblReason.TabIndex = 8;
            this.lblReason.Text = "Lý do:";
            // 
            // txtReason
            // 
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.BorderRadius = 6;
            this.txtReason.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtReason.DefaultText = "";
            this.txtReason.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtReason.Location = new System.Drawing.Point(124, 224);
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(300, 80);
            this.txtReason.TabIndex = 9;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.BorderRadius = 6;
            this.btnConfirm.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(165)))), ((int)(((byte)(233)))));
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(194, 318);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(110, 40);
            this.btnConfirm.TabIndex = 10;
            this.btnConfirm.Text = "Xác nhận";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BorderRadius = 6;
            this.btnCancel.FillColor = System.Drawing.Color.Gainsboro;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.Black;
            this.btnCancel.Location = new System.Drawing.Point(314, 318);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 40);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            // FormInventoryTransaction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(450, 380);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.cmbSupplier);
            this.Controls.Add(this.lblSupplier);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbTransactionType);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblCurrentQuantity);
            this.Controls.Add(this.lblSeafoodName);
            this.MinimumSize = new System.Drawing.Size(460, 420);
            this.Name = "FormInventoryTransaction";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Giao dịch kho hàng";
            this.Load += new System.EventHandler(this.FormInventoryTransaction_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Guna.UI2.WinForms.Guna2HtmlLabel lblSeafoodName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCurrentQuantity;
        private Guna.UI2.WinForms.Guna2HtmlLabel label1;
        private Guna.UI2.WinForms.Guna2ComboBox cmbTransactionType;
        private Guna.UI2.WinForms.Guna2HtmlLabel label2;
        private Guna.UI2.WinForms.Guna2TextBox txtQuantity;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSupplier;
        private Guna.UI2.WinForms.Guna2ComboBox cmbSupplier;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblReason;
        private Guna.UI2.WinForms.Guna2TextBox txtReason;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2BorderlessForm borderlessForm;
        private Guna.UI2.WinForms.Guna2DragControl dragControl;
    }
}

