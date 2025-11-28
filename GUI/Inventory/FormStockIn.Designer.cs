namespace QLTN_LT.GUI.Inventory
{
    partial class FormStockIn
    {
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2ComboBox cmbProduct;
        private Guna.UI2.WinForms.Guna2NumericUpDown nudQuantity;
        private Guna.UI2.WinForms.Guna2TextBox txtNotes;
        private Guna.UI2.WinForms.Guna2Button btnSave;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2BorderlessForm borderlessForm;
        private Guna.UI2.WinForms.Guna2DragControl dragControl;
        private Guna.UI2.WinForms.Guna2ControlBox controlBoxClose;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblProductError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblQuantityError;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblProductLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblQuantityLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNotesLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.cmbProduct = new Guna.UI2.WinForms.Guna2ComboBox();
            this.nudQuantity = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.txtNotes = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnSave = new Guna.UI2.WinForms.Guna2Button();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.lblProductError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblQuantityError = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblProductLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblQuantityLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblNotesLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.borderlessForm = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.dragControl = new Guna.UI2.WinForms.Guna2DragControl(this.components);
            this.controlBoxClose = new Guna.UI2.WinForms.Guna2ControlBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(107, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Nhập kho";
            // 
            // lblProductLabel
            // 
            this.lblProductLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblProductLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProductLabel.Location = new System.Drawing.Point(24, 70);
            this.lblProductLabel.Name = "lblProductLabel";
            this.lblProductLabel.Size = new System.Drawing.Size(59, 17);
            this.lblProductLabel.TabIndex = 1;
            this.lblProductLabel.Text = "Sản phẩm";
            // 
            // cmbProduct
            // 
            this.cmbProduct.BackColor = System.Drawing.Color.Transparent;
            this.cmbProduct.BorderRadius = 6;
            this.cmbProduct.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProduct.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.cmbProduct.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.cmbProduct.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbProduct.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbProduct.ItemHeight = 30;
            this.cmbProduct.Location = new System.Drawing.Point(24, 90);
            this.cmbProduct.Name = "cmbProduct";
            this.cmbProduct.Size = new System.Drawing.Size(420, 36);
            this.cmbProduct.TabIndex = 2;
            this.cmbProduct.SelectedIndexChanged += new System.EventHandler(this.cmbProduct_SelectedIndexChanged);
            // 
            // lblQuantityLabel
            // 
            this.lblQuantityLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblQuantityLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblQuantityLabel.Location = new System.Drawing.Point(24, 140);
            this.lblQuantityLabel.Name = "lblQuantityLabel";
            this.lblQuantityLabel.Size = new System.Drawing.Size(55, 17);
            this.lblQuantityLabel.TabIndex = 3;
            this.lblQuantityLabel.Text = "Số lượng";
            // 
            // nudQuantity
            // 
            this.nudQuantity.BackColor = System.Drawing.Color.Transparent;
            this.nudQuantity.BorderRadius = 6;
            this.nudQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.nudQuantity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.nudQuantity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.nudQuantity.Location = new System.Drawing.Point(24, 160);
            this.nudQuantity.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.nudQuantity.Name = "nudQuantity";
            this.nudQuantity.Size = new System.Drawing.Size(420, 36);
            this.nudQuantity.TabIndex = 4;
            this.nudQuantity.ValueChanged += new System.EventHandler(this.nudQuantity_ValueChanged);
            // 
            // lblNotesLabel
            // 
            this.lblNotesLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblNotesLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNotesLabel.Location = new System.Drawing.Point(24, 210);
            this.lblNotesLabel.Name = "lblNotesLabel";
            this.lblNotesLabel.Size = new System.Drawing.Size(46, 17);
            this.lblNotesLabel.TabIndex = 5;
            this.lblNotesLabel.Text = "Ghi chú";
            // 
            // txtNotes
            // 
            this.txtNotes.BorderRadius = 6;
            this.txtNotes.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNotes.DefaultText = "";
            this.txtNotes.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtNotes.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtNotes.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.txtNotes.Location = new System.Drawing.Point(24, 230);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.PlaceholderText = "Nhập ghi chú (tùy chọn)";
            this.txtNotes.Size = new System.Drawing.Size(420, 80);
            this.txtNotes.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.BorderRadius = 6;
            this.btnSave.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(224, 330);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 36);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Lưu";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BorderRadius = 6;
            this.btnCancel.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(244)))), ((int)(((byte)(246)))));
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39)))));
            this.btnCancel.Location = new System.Drawing.Point(344, 330);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 36);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblProductError
            // 
            this.lblProductError.BackColor = System.Drawing.Color.Transparent;
            this.lblProductError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblProductError.ForeColor = System.Drawing.Color.Red;
            this.lblProductError.Location = new System.Drawing.Point(24, 128);
            this.lblProductError.Name = "lblProductError";
            this.lblProductError.Size = new System.Drawing.Size(3, 2);
            this.lblProductError.TabIndex = 9;
            this.lblProductError.Text = null;
            this.lblProductError.Visible = false;
            // 
            // lblQuantityError
            // 
            this.lblQuantityError.BackColor = System.Drawing.Color.Transparent;
            this.lblQuantityError.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblQuantityError.ForeColor = System.Drawing.Color.Red;
            this.lblQuantityError.Location = new System.Drawing.Point(24, 198);
            this.lblQuantityError.Name = "lblQuantityError";
            this.lblQuantityError.Size = new System.Drawing.Size(3, 2);
            this.lblQuantityError.TabIndex = 10;
            this.lblQuantityError.Text = null;
            this.lblQuantityError.Visible = false;
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
            this.controlBoxClose.TabIndex = 11;
            // 
            // FormStockIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(468, 390);
            this.Controls.Add(this.controlBoxClose);
            this.Controls.Add(this.lblProductError);
            this.Controls.Add(this.lblQuantityError);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotesLabel);
            this.Controls.Add(this.nudQuantity);
            this.Controls.Add(this.lblQuantityLabel);
            this.Controls.Add(this.cmbProduct);
            this.Controls.Add(this.lblProductLabel);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormStockIn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhập kho";
            this.Load += new System.EventHandler(this.FormStockIn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
