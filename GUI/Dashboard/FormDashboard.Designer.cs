namespace QLTN_LT.GUI.Dashboard
{
    partial class FormDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private LiveCharts.WinForms.CartesianChart revenueChart;

        private System.Windows.Forms.FlowLayoutPanel flowTopSelling;
        private Guna.UI2.WinForms.Guna2Panel pnlExpirationCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblExpirationTitle;
        private Guna.UI2.WinForms.Guna2DataGridView dgvExpiration;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalRevenue;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRevenueTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTopSellingTitle;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private Guna.UI2.WinForms.Guna2Panel pnlRevenueCard;
        private Guna.UI2.WinForms.Guna2Panel pnlOrdersCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblOrdersCount;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblOrdersTitle;
        private Guna.UI2.WinForms.Guna2Panel pnlCustomersCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCustomersCount;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCustomersTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblRevenueTrend;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblOrdersTrend;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCustomersTrend;
        private Guna.UI2.WinForms.Guna2Panel pnlRevenueChartCard;
        private Guna.UI2.WinForms.Guna2Panel pnlTopSellingChartCard;
        private Guna.UI2.WinForms.Guna2Panel pnlFilterBar;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpStartDate;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpEndDate;
        private Guna.UI2.WinForms.Guna2Button btnApplyFilter;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblFrom;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTo;
        private Guna.UI2.WinForms.Guna2PictureBox picRevenue;
        private Guna.UI2.WinForms.Guna2PictureBox picOrders;
        private Guna.UI2.WinForms.Guna2PictureBox picCustomers;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDashboard));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlRevenueCard = new Guna.UI2.WinForms.Guna2Panel();
            this.picRevenue = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblTotalRevenue = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblRevenueTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlOrdersCard = new Guna.UI2.WinForms.Guna2Panel();
            this.picOrders = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblOrdersCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblOrdersTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlCustomersCard = new Guna.UI2.WinForms.Guna2Panel();
            this.lblExpirationTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dgvExpiration = new Guna.UI2.WinForms.Guna2DataGridView();
            this.pnlFilterBar = new Guna.UI2.WinForms.Guna2Panel();
            this.btnApplyFilter = new Guna.UI2.WinForms.Guna2Button();
            this.lblTo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dtpEndDate = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.lblFrom = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dtpStartDate = new Guna.UI2.WinForms.Guna2DateTimePicker();
            this.flowLayoutPanel1.SuspendLayout();
            this.pnlRevenueCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRevenue)).BeginInit();
            this.pnlOrdersCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOrders)).BeginInit();
            this.pnlCustomersCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCustomers)).BeginInit();
            this.pnlRevenueChartCard.SuspendLayout();
            this.pnlRevenueChartCard.SuspendLayout();
            this.pnlTopSellingChartCard.SuspendLayout();
            this.pnlExpirationCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpiration)).BeginInit();
            this.pnlFilterBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this.flowLayoutPanel1.Controls.Add(this.pnlRevenueCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlOrdersCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlCustomersCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlRevenueChartCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlRevenueChartCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlTopSellingChartCard);
            this.flowLayoutPanel1.Controls.Add(this.pnlExpirationCard);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 60);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(15);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(984, 541);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // pnlRevenueCard
            // 
            this.pnlRevenueCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlRevenueCard.BorderRadius = 10;
            this.pnlRevenueCard.Controls.Add(this.lblRevenueTrend);
            this.pnlRevenueCard.Controls.Add(this.lblTotalRevenue);
            this.pnlRevenueCard.Controls.Add(this.lblRevenueTitle);
            this.pnlRevenueCard.FillColor = System.Drawing.Color.White;
            this.pnlRevenueCard.Location = new System.Drawing.Point(20, 20);
            this.pnlRevenueCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlRevenueCard.Name = "pnlRevenueCard";
            this.pnlRevenueCard.ShadowDecoration.BorderRadius = 10;
            this.pnlRevenueCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlRevenueCard.ShadowDecoration.Enabled = true;
            this.pnlRevenueCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlRevenueCard.Size = new System.Drawing.Size(300, 120);
            this.pnlRevenueCard.TabIndex = 0;
            // 
            // 
            // lblRevenueTrend
            // 
            this.lblRevenueTrend.BackColor = System.Drawing.Color.Transparent;
            this.lblRevenueTrend.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRevenueTrend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.lblRevenueTrend.Location = new System.Drawing.Point(20, 90);
            this.lblRevenueTrend.Name = "lblRevenueTrend";
            this.lblRevenueTrend.Size = new System.Drawing.Size(43, 17);
            this.lblRevenueTrend.TabIndex = 2;
            this.lblRevenueTrend.Text = "+5.2%";
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTotalRevenue.Location = new System.Drawing.Point(20, 45);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(89, 34);
            this.lblTotalRevenue.TabIndex = 1;
            this.lblTotalRevenue.Text = "15.2M";
            // 
            // lblRevenueTitle
            // 
            this.lblRevenueTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblRevenueTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRevenueTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblRevenueTitle.Location = new System.Drawing.Point(20, 20);
            this.lblRevenueTitle.Name = "lblRevenueTitle";
            this.lblRevenueTitle.Size = new System.Drawing.Size(127, 19);
            this.lblRevenueTitle.TabIndex = 0;
            this.lblRevenueTitle.Text = "Doanh thu hôm nay";
            // 
            // pnlOrdersCard
            // 
            this.pnlOrdersCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlOrdersCard.BorderRadius = 10;
            this.pnlOrdersCard.Controls.Add(this.lblOrdersTrend);
            this.pnlOrdersCard.Controls.Add(this.lblOrdersCount);
            this.pnlOrdersCard.Controls.Add(this.lblOrdersTitle);
            this.pnlOrdersCard.FillColor = System.Drawing.Color.White;
            this.pnlOrdersCard.Location = new System.Drawing.Point(330, 20);
            this.pnlOrdersCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlOrdersCard.Name = "pnlOrdersCard";
            this.pnlOrdersCard.ShadowDecoration.BorderRadius = 10;
            this.pnlOrdersCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlOrdersCard.ShadowDecoration.Enabled = true;
            this.pnlOrdersCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlOrdersCard.Size = new System.Drawing.Size(300, 120);
            this.pnlOrdersCard.TabIndex = 1;
            // 
            // 
            // lblOrdersTrend
            // 
            this.lblOrdersTrend.BackColor = System.Drawing.Color.Transparent;
            this.lblOrdersTrend.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOrdersTrend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.lblOrdersTrend.Location = new System.Drawing.Point(20, 90);
            this.lblOrdersTrend.Name = "lblOrdersTrend";
            this.lblOrdersTrend.Size = new System.Drawing.Size(43, 17);
            this.lblOrdersTrend.TabIndex = 2;
            this.lblOrdersTrend.Text = "-1.5%";
            // 
            // lblOrdersCount
            // 
            this.lblOrdersCount.BackColor = System.Drawing.Color.Transparent;
            this.lblOrdersCount.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblOrdersCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblOrdersCount.Location = new System.Drawing.Point(20, 45);
            this.lblOrdersCount.Name = "lblOrdersCount";
            this.lblOrdersCount.Size = new System.Drawing.Size(17, 34);
            this.lblOrdersCount.TabIndex = 1;
            this.lblOrdersCount.Text = "85";
            // 
            // lblOrdersTitle
            // 
            this.lblOrdersTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblOrdersTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblOrdersTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblOrdersTitle.Location = new System.Drawing.Point(20, 20);
            this.lblOrdersTitle.Name = "lblOrdersTitle";
            this.lblOrdersTitle.Size = new System.Drawing.Size(122, 19);
            this.lblOrdersTitle.TabIndex = 0;
            this.lblOrdersTitle.Text = "Số đơn hôm nay";
            // 
            // pnlCustomersCard
            // 
            this.pnlCustomersCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlCustomersCard.BorderRadius = 10;
            this.pnlCustomersCard.Controls.Add(this.lblCustomersTrend);
            this.pnlCustomersCard.Controls.Add(this.lblCustomersCount);
            this.pnlCustomersCard.Controls.Add(this.lblCustomersTitle);
            this.pnlCustomersCard.FillColor = System.Drawing.Color.White;
            this.pnlCustomersCard.Location = new System.Drawing.Point(640, 20);
            this.pnlCustomersCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlCustomersCard.Name = "pnlCustomersCard";
            this.pnlCustomersCard.ShadowDecoration.BorderRadius = 10;
            this.pnlCustomersCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlCustomersCard.ShadowDecoration.Enabled = true;
            this.pnlCustomersCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlCustomersCard.Size = new System.Drawing.Size(300, 120);
            this.pnlCustomersCard.TabIndex = 2;
            // 
            // 
            // lblCustomersTrend
            // 
            this.lblCustomersTrend.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomersTrend.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCustomersTrend.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.lblCustomersTrend.Location = new System.Drawing.Point(20, 90);
            this.lblCustomersTrend.Name = "lblCustomersTrend";
            this.lblCustomersTrend.Size = new System.Drawing.Size(43, 17);
            this.lblCustomersTrend.TabIndex = 2;
            this.lblCustomersTrend.Text = "+12.8%";
            // 
            // lblCustomersCount
            // 
            this.lblCustomersCount.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomersCount.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.lblCustomersCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblCustomersCount.Location = new System.Drawing.Point(20, 45);
            this.lblCustomersCount.Name = "lblCustomersCount";
            this.lblCustomersCount.Size = new System.Drawing.Size(17, 34);
            this.lblCustomersCount.TabIndex = 1;
            this.lblCustomersCount.Text = "210.5M";
            // 
            // lblCustomersTitle
            // 
            this.lblCustomersTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblCustomersTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCustomersTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblCustomersTitle.Location = new System.Drawing.Point(20, 20);
            this.lblCustomersTitle.Name = "lblCustomersTitle";
            this.lblCustomersTitle.Size = new System.Drawing.Size(113, 19);
            this.lblCustomersTitle.TabIndex = 0;
            this.lblCustomersTitle.Text = "Doanh thu tháng này";
            // 
            // pnlRevenueChartCard
            // 
            this.pnlRevenueChartCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlRevenueChartCard.BorderRadius = 10;
            this.pnlRevenueChartCard.Controls.Add(this.revenueChart);
            this.pnlRevenueChartCard.FillColor = System.Drawing.Color.White;
            this.pnlRevenueChartCard.Location = new System.Drawing.Point(20, 150);
            this.pnlRevenueChartCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlRevenueChartCard.Name = "pnlRevenueChartCard";
            this.pnlRevenueChartCard.ShadowDecoration.BorderRadius = 10;
            this.pnlRevenueChartCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlRevenueChartCard.ShadowDecoration.Enabled = true;
            this.pnlRevenueChartCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlRevenueChartCard.Size = new System.Drawing.Size(610, 350);
            this.pnlRevenueChartCard.TabIndex = 3;
            // 
            // revenueChart
            // 
            this.revenueChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.revenueChart.Location = new System.Drawing.Point(0, 0);
            this.revenueChart.Name = "revenueChart";
            this.revenueChart.Size = new System.Drawing.Size(610, 350);
            this.revenueChart.TabIndex = 0;
            this.revenueChart.Text = "cartesianChart1";
            // 
            // pnlTopSellingChartCard
            // 
            this.pnlTopSellingChartCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlTopSellingChartCard.BorderRadius = 10;

            this.pnlTopSellingChartCard.Controls.Add(this.lblTopSellingTitle);
            this.pnlTopSellingChartCard.FillColor = System.Drawing.Color.White;
            this.pnlTopSellingChartCard.Location = new System.Drawing.Point(640, 150);
            this.pnlTopSellingChartCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlTopSellingChartCard.Name = "pnlTopSellingChartCard";
            this.pnlTopSellingChartCard.ShadowDecoration.BorderRadius = 10;
            this.pnlTopSellingChartCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlTopSellingChartCard.ShadowDecoration.Enabled = true;
            this.pnlTopSellingChartCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlTopSellingChartCard.Size = new System.Drawing.Size(300, 350);
            this.pnlTopSellingChartCard.TabIndex = 4;
            // 
            // 
            // flowTopSelling
            // 
            this.flowTopSelling.AutoScroll = true;
            this.flowTopSelling.Location = new System.Drawing.Point(0, 50);
            this.flowTopSelling.Name = "flowTopSelling";
            this.flowTopSelling.Size = new System.Drawing.Size(300, 280);
            this.flowTopSelling.TabIndex = 1;
            // 
            // lblTopSellingTitle
            // 
            this.lblTopSellingTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTopSellingTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTopSellingTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this.lblTopSellingTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTopSellingTitle.Name = "lblTopSellingTitle";
            this.lblTopSellingTitle.Size = new System.Drawing.Size(140, 19);
            this.lblTopSellingTitle.TabIndex = 0;
            this.lblTopSellingTitle.Text = "Top 5 món bán chạy";
            // 
            // pnlExpirationCard
            // 
            this.pnlExpirationCard.BackColor = System.Drawing.Color.Transparent;
            this.pnlExpirationCard.BorderRadius = 10;
            this.pnlExpirationCard.Controls.Add(this.dgvExpiration);
            this.pnlExpirationCard.Controls.Add(this.lblExpirationTitle);
            this.pnlExpirationCard.FillColor = System.Drawing.Color.White;
            this.pnlExpirationCard.Location = new System.Drawing.Point(20, 510);
            this.pnlExpirationCard.Margin = new System.Windows.Forms.Padding(5);
            this.pnlExpirationCard.Name = "pnlExpirationCard";
            this.pnlExpirationCard.ShadowDecoration.BorderRadius = 10;
            this.pnlExpirationCard.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.pnlExpirationCard.ShadowDecoration.Enabled = true;
            this.pnlExpirationCard.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.pnlExpirationCard.Size = new System.Drawing.Size(920, 250);
            this.pnlExpirationCard.TabIndex = 5;
            // 
            // lblExpirationTitle
            // 
            this.lblExpirationTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblExpirationTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblExpirationTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblExpirationTitle.Location = new System.Drawing.Point(20, 20);
            this.lblExpirationTitle.Name = "lblExpirationTitle";
            this.lblExpirationTitle.Size = new System.Drawing.Size(140, 19);
            this.lblExpirationTitle.TabIndex = 0;
            this.lblExpirationTitle.Text = "Hàng sắp hết hạn";
            // 
            // dgvExpiration
            // 
            this.dgvExpiration.AllowUserToAddRows = false;
            this.dgvExpiration.AllowUserToDeleteRows = false;
            this.dgvExpiration.BackgroundColor = System.Drawing.Color.White;
            this.dgvExpiration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvExpiration.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExpiration.Location = new System.Drawing.Point(20, 50);
            this.dgvExpiration.Name = "dgvExpiration";
            this.dgvExpiration.ReadOnly = true;
            this.dgvExpiration.RowHeadersVisible = false;
            this.dgvExpiration.Size = new System.Drawing.Size(880, 180);
            this.dgvExpiration.TabIndex = 1;
            // 
            // pnlFilterBar
            // 
            this.pnlFilterBar.Controls.Add(this.btnApplyFilter);
            this.pnlFilterBar.Controls.Add(this.lblTo);
            this.pnlFilterBar.Controls.Add(this.dtpEndDate);
            this.pnlFilterBar.Controls.Add(this.lblFrom);
            this.pnlFilterBar.Controls.Add(this.dtpStartDate);
            this.pnlFilterBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFilterBar.Location = new System.Drawing.Point(0, 0);
            this.pnlFilterBar.Name = "pnlFilterBar";
            this.pnlFilterBar.Padding = new System.Windows.Forms.Padding(15, 0, 15, 0);
            this.pnlFilterBar.Size = new System.Drawing.Size(984, 60);
            this.pnlFilterBar.TabIndex = 1;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.BorderRadius = 6;
            this.btnApplyFilter.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(165)))), ((int)(((byte)(233)))));
            this.btnApplyFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnApplyFilter.ForeColor = System.Drawing.Color.White;
            this.btnApplyFilter.Location = new System.Drawing.Point(447, 12);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(120, 36);
            this.btnApplyFilter.TabIndex = 4;
            this.btnApplyFilter.Text = "Áp dụng";
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            // 
            // lblTo
            // 
            this.lblTo.BackColor = System.Drawing.Color.Transparent;
            this.lblTo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTo.Location = new System.Drawing.Point(230, 22);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(55, 17);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "Đến ngày:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.BorderRadius = 6;
            this.dtpEndDate.Checked = true;
            this.dtpEndDate.FillColor = System.Drawing.Color.White;
            this.dtpEndDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(291, 12);
            this.dtpEndDate.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpEndDate.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(140, 36);
            this.dtpEndDate.TabIndex = 3;
            this.dtpEndDate.Value = new System.DateTime(2023, 10, 27, 0, 0, 0, 0);
            // 
            // lblFrom
            // 
            this.lblFrom.BackColor = System.Drawing.Color.Transparent;
            this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFrom.Location = new System.Drawing.Point(20, 22);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(48, 17);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "Từ ngày:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.BorderRadius = 6;
            this.dtpStartDate.Checked = true;
            this.dtpStartDate.FillColor = System.Drawing.Color.White;
            this.dtpStartDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(74, 12);
            this.dtpStartDate.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.dtpStartDate.MinDate = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(140, 36);
            this.dtpStartDate.TabIndex = 1;
            this.dtpStartDate.Value = new System.DateTime(2023, 1, 1, 0, 0, 0, 0);
            // 
            // FormDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(984, 601);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.pnlFilterBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.FormDashboard_Load);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.pnlRevenueCard.ResumeLayout(false);
            this.pnlRevenueCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picRevenue)).EndInit();
            this.pnlOrdersCard.ResumeLayout(false);
            this.pnlOrdersCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOrders)).EndInit();
            this.pnlCustomersCard.ResumeLayout(false);
            this.pnlCustomersCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCustomers)).EndInit();
            this.pnlRevenueChartCard.ResumeLayout(false);
            this.pnlTopSellingChartCard.ResumeLayout(false);
            this.pnlTopSellingChartCard.PerformLayout();
            this.pnlExpirationCard.ResumeLayout(false);
            this.pnlExpirationCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExpiration)).EndInit();
            this.pnlFilterBar.ResumeLayout(false);
            this.pnlFilterBar.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
