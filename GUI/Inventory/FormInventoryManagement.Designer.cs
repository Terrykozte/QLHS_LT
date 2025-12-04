namespace QLTN_LT.GUI.Inventory
{
    partial class FormInventoryManagement
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
            this.dgvInventory = new System.Windows.Forms.DataGridView();
            this.dgvTransactions = new System.Windows.Forms.DataGridView();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnStockIn = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnViewTransactions = new System.Windows.Forms.Button();
            this.btnPreviousPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnExportReport = new System.Windows.Forms.Button();
            this.lblTotalItems = new System.Windows.Forms.Label();
            this.lblTransactionInfo = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();

            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();

            // dgvInventory
            this.dgvInventory.AllowUserToAddRows = false;
            this.dgvInventory.AllowUserToDeleteRows = false;
            this.dgvInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInventory.Location = new System.Drawing.Point(10, 80);
            this.dgvInventory.Name = "dgvInventory";
            this.dgvInventory.ReadOnly = true;
            this.dgvInventory.Size = new System.Drawing.Size(900, 250);
            this.dgvInventory.TabIndex = 0;

            // dgvTransactions
            this.dgvTransactions.AllowUserToAddRows = false;
            this.dgvTransactions.AllowUserToDeleteRows = false;
            this.dgvTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTransactions.Location = new System.Drawing.Point(10, 80);
            this.dgvTransactions.Name = "dgvTransactions";
            this.dgvTransactions.ReadOnly = true;
            this.dgvTransactions.Size = new System.Drawing.Size(900, 200);
            this.dgvTransactions.TabIndex = 1;

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(10, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(300, 23);
            this.txtSearch.TabIndex = 2;
            // Placeholder not supported on .NET Framework TextBox; omitted

            // btnSearch
            this.btnSearch.Location = new System.Drawing.Point(320, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            // btnStockIn
            this.btnStockIn.Location = new System.Drawing.Point(410, 20);
            this.btnStockIn.Name = "btnStockIn";
            this.btnStockIn.Size = new System.Drawing.Size(80, 23);
            this.btnStockIn.TabIndex = 4;
            this.btnStockIn.Text = "Nhập hàng";
            this.btnStockIn.Click += new System.EventHandler(this.btnStockIn_Click);

            // btnRefresh
            this.btnRefresh.Location = new System.Drawing.Point(500, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnViewTransactions
            this.btnViewTransactions.Location = new System.Drawing.Point(590, 20);
            this.btnViewTransactions.Name = "btnViewTransactions";
            this.btnViewTransactions.Size = new System.Drawing.Size(120, 23);
            this.btnViewTransactions.TabIndex = 6;
            this.btnViewTransactions.Text = "Xem giao dịch";
            this.btnViewTransactions.Click += new System.EventHandler(this.btnViewTransactions_Click);

            // btnExportReport
            this.btnExportReport.Location = new System.Drawing.Point(720, 20);
            this.btnExportReport.Name = "btnExportReport";
            this.btnExportReport.Size = new System.Drawing.Size(100, 23);
            this.btnExportReport.TabIndex = 7;
            this.btnExportReport.Text = "Xuất báo cáo";
            this.btnExportReport.Click += new System.EventHandler(this.btnExportReport_Click);

            // lblTotalItems
            this.lblTotalItems.AutoSize = true;
            this.lblTotalItems.Location = new System.Drawing.Point(10, 50);
            this.lblTotalItems.Name = "lblTotalItems";
            this.lblTotalItems.Size = new System.Drawing.Size(100, 15);
            this.lblTotalItems.TabIndex = 8;
            this.lblTotalItems.Text = "Tổng sản phẩm: 0";

            // lblTransactionInfo
            this.lblTransactionInfo.AutoSize = true;
            this.lblTransactionInfo.Location = new System.Drawing.Point(10, 290);
            this.lblTransactionInfo.Name = "lblTransactionInfo";
            this.lblTransactionInfo.Size = new System.Drawing.Size(100, 15);
            this.lblTransactionInfo.TabIndex = 9;
            this.lblTransactionInfo.Text = "Trang 1/1";

            // btnPreviousPage
            this.btnPreviousPage.Location = new System.Drawing.Point(10, 310);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(80, 23);
            this.btnPreviousPage.TabIndex = 10;
            this.btnPreviousPage.Text = "Trang trước";
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);

            // btnNextPage
            this.btnNextPage.Location = new System.Drawing.Point(100, 310);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(80, 23);
            this.btnNextPage.TabIndex = 11;
            this.btnNextPage.Text = "Trang sau";
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);

            // groupBox1
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.btnStockIn);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Controls.Add(this.btnViewTransactions);
            this.groupBox1.Controls.Add(this.btnExportReport);
            this.groupBox1.Controls.Add(this.lblTotalItems);
            this.groupBox1.Controls.Add(this.dgvInventory);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(920, 350);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quản lý kho hàng";

            // groupBox2
            this.groupBox2.Controls.Add(this.dgvTransactions);
            this.groupBox2.Controls.Add(this.lblTransactionInfo);
            this.groupBox2.Controls.Add(this.btnPreviousPage);
            this.groupBox2.Controls.Add(this.btnNextPage);
            this.groupBox2.Location = new System.Drawing.Point(12, 370);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(920, 350);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Lịch sử giao dịch";

            // FormInventoryManagement
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 732);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "FormInventoryManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Quản lý kho hàng";
            this.Load += new System.EventHandler(this.FormInventoryManagement_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTransactions)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView dgvInventory;
        private System.Windows.Forms.DataGridView dgvTransactions;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnStockIn;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnViewTransactions;
        private System.Windows.Forms.Button btnPreviousPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnExportReport;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.Label lblTransactionInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

