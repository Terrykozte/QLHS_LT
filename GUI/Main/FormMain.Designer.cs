namespace QLTN_LT.GUI.Main
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        // Layout Panels
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar;
        private Guna.UI2.WinForms.Guna2Panel pnlContent;

        // Sidebar Controls
        private Guna.UI2.WinForms.Guna2Button btnToggleSidebar;

        // Sidebar Controls
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblBrand;
        private System.Windows.Forms.FlowLayoutPanel flowSidebar;
        private Guna.UI2.WinForms.Guna2Separator separatorSidebar;

        // Navigation Buttons
        private Guna.UI2.WinForms.Guna2Button btnDashboard;
        private Guna.UI2.WinForms.Guna2Button btnSeafood;
        private Guna.UI2.WinForms.Guna2Button btnOrders;
        private Guna.UI2.WinForms.Guna2Button btnReports;
        private Guna.UI2.WinForms.Guna2Button btnCategory;
        private Guna.UI2.WinForms.Guna2Button btnCustomer;
        private Guna.UI2.WinForms.Guna2Button btnTable;
        private Guna.UI2.WinForms.Guna2Button btnMenu;
        private Guna.UI2.WinForms.Guna2Button btnInventory;
        private Guna.UI2.WinForms.Guna2Button btnSupplier;
        private Guna.UI2.WinForms.Guna2Button btnUser;
        private Guna.UI2.WinForms.Guna2Button btnMenuQR;
        private Guna.UI2.WinForms.Guna2Button btnLogout;

        // User Info
        private Guna.UI2.WinForms.Guna2Panel pnlUserInfo;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picUserAvatar;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblUserName;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblUserRole;

        // Custom Title Bar
        private Guna.UI2.WinForms.Guna2Panel pnlTitleBar;
        private Guna.UI2.WinForms.Guna2Button btnMinimize;
        private Guna.UI2.WinForms.Guna2Button btnMaximize;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2Button btnSettings;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2DragControl dragControl;

        // Advanced
        private Guna.UI2.WinForms.Guna2ShadowForm shadowForm;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.btnToggleSidebar = new Guna.UI2.WinForms.Guna2Button();
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.separatorSidebar = new Guna.UI2.WinForms.Guna2Separator();
            this.picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            this.lblBrand = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.flowSidebar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDashboard = new Guna.UI2.WinForms.Guna2Button();
            this.btnSeafood = new Guna.UI2.WinForms.Guna2Button();
            this.btnOrders = new Guna.UI2.WinForms.Guna2Button();
            this.btnReports = new Guna.UI2.WinForms.Guna2Button();
            this.btnCategory = new Guna.UI2.WinForms.Guna2Button();
            this.btnCustomer = new Guna.UI2.WinForms.Guna2Button();
            this.btnTable = new Guna.UI2.WinForms.Guna2Button();
            this.btnMenu = new Guna.UI2.WinForms.Guna2Button();
            this.btnInventory = new Guna.UI2.WinForms.Guna2Button();
            this.btnMenuQR = new Guna.UI2.WinForms.Guna2Button();
            this.btnSupplier = new Guna.UI2.WinForms.Guna2Button();
            this.btnUser = new Guna.UI2.WinForms.Guna2Button();
            this.btnLogout = new Guna.UI2.WinForms.Guna2Button();
            this.pnlUserInfo = new Guna.UI2.WinForms.Guna2Panel();
            this.picUserAvatar = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.lblUserName = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblUserRole = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.pnlContent = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlTitleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.btnSettings = new Guna.UI2.WinForms.Guna2Button();
            this.btnMinimize = new Guna.UI2.WinForms.Guna2Button();
            this.btnMaximize = new Guna.UI2.WinForms.Guna2Button();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.dragControl = new Guna.UI2.WinForms.Guna2DragControl();
            this.shadowForm = new Guna.UI2.WinForms.Guna2ShadowForm(this.components);

            this.pnlSidebar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.flowSidebar.SuspendLayout();
            this.pnlUserInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).BeginInit();
            this.SuspendLayout();

            // 
            // btnToggleSidebar - Trong sidebar, bên cạnh logo
            // 
            this.btnToggleSidebar.CheckedState.Parent = this.btnToggleSidebar;
            this.btnToggleSidebar.CustomImages.Parent = this.btnToggleSidebar;
            this.btnToggleSidebar.FillColor = System.Drawing.Color.Transparent;
            this.btnToggleSidebar.Font = new System.Drawing.Font("Segoe UI", 16F);
            this.btnToggleSidebar.ForeColor = System.Drawing.Color.White;
            this.btnToggleSidebar.HoverState.Parent = this.btnToggleSidebar;
            this.btnToggleSidebar.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnToggleSidebar.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnToggleSidebar.Text = "☰";
            this.btnToggleSidebar.Location = new System.Drawing.Point(10, 20);
            this.btnToggleSidebar.Name = "btnToggleSidebar";
            this.btnToggleSidebar.ShadowDecoration.Parent = this.btnToggleSidebar;
            this.btnToggleSidebar.Size = new System.Drawing.Size(35, 35);
            this.btnToggleSidebar.TabIndex = 5;
            this.btnToggleSidebar.Click += new System.EventHandler(this.btnToggleSidebar_Click);
            // 
            // pnlSidebar - Dock từ top (dùng thanh tiêu đề Windows)
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(24)))), ((int)(((byte)(39))))); // Dark Theme
            this.pnlSidebar.BorderRadius = 0;
            this.pnlSidebar.Controls.Add(this.pnlUserInfo);
            this.pnlSidebar.Controls.Add(this.flowSidebar);
            this.pnlSidebar.Controls.Add(this.separatorSidebar);
            this.pnlSidebar.Controls.Add(this.lblBrand);
            this.pnlSidebar.Controls.Add(this.picLogo);
            this.pnlSidebar.Controls.Add(this.btnToggleSidebar);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(240, 576);
            this.pnlSidebar.TabIndex = 1;
            // 
            // picLogo - Điều chỉnh vị trí để có chỗ cho btnToggleSidebar
            // 
            this.picLogo.Image = null;
            this.picLogo.ImageRotate = 0F;
            this.picLogo.Location = new System.Drawing.Point(55, 20);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(35, 35);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // lblBrand
            // 
            this.lblBrand.BackColor = System.Drawing.Color.Transparent;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(100, 23);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(85, 27);
            this.lblBrand.TabIndex = 1;
            this.lblBrand.Text = "QLHS_LT";
            // 
            // separatorSidebar
            // 
            this.separatorSidebar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this.separatorSidebar.Location = new System.Drawing.Point(20, 70);
            this.separatorSidebar.Name = "separatorSidebar";
            this.separatorSidebar.Size = new System.Drawing.Size(200, 10);
            this.separatorSidebar.TabIndex = 2;
            // 
            // flowSidebar
            // 
            this.flowSidebar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowSidebar.Controls.Add(this.btnDashboard);
            this.flowSidebar.Controls.Add(this.btnSeafood);
            this.flowSidebar.Controls.Add(this.btnOrders);
            this.flowSidebar.Controls.Add(this.btnReports);
            this.flowSidebar.Controls.Add(this.btnCategory);
            this.flowSidebar.Controls.Add(this.btnCustomer);
            this.flowSidebar.Controls.Add(this.btnTable);
            this.flowSidebar.Controls.Add(this.btnMenu);
            this.flowSidebar.Controls.Add(this.btnInventory);
            this.flowSidebar.Controls.Add(this.btnMenuQR);
            this.flowSidebar.Controls.Add(this.btnSupplier);
            this.flowSidebar.Controls.Add(this.btnUser);
            this.flowSidebar.Controls.Add(this.btnLogout);
            this.flowSidebar.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowSidebar.Location = new System.Drawing.Point(0, 90);
            this.flowSidebar.Name = "flowSidebar";
            this.flowSidebar.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.flowSidebar.Size = new System.Drawing.Size(240, 550);
            this.flowSidebar.TabIndex = 3;
            this.flowSidebar.WrapContents = false;
            // 
            // btnDashboard
            // 
            this.btnDashboard.BorderRadius = 6;
            this.btnDashboard.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnDashboard.Checked = true;
            this.btnDashboard.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnDashboard.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnDashboard.CheckedState.Image = null;
            this.btnDashboard.FillColor = System.Drawing.Color.Transparent;
            this.btnDashboard.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDashboard.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnDashboard.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnDashboard.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Image = null;
            this.btnDashboard.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnDashboard.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnDashboard.Location = new System.Drawing.Point(13, 3);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(215, 45);
            this.btnDashboard.TabIndex = 0;
            this.btnDashboard.Text = "Tổng quan";
            this.btnDashboard.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnDashboard.TextOffset = new System.Drawing.Point(15, 0);
            this.btnDashboard.Click += new System.EventHandler(this.NavigationButton_Click);
            
            // btnSeafood
            // 
            this.btnSeafood.BorderRadius = 6;
            this.btnSeafood.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnSeafood.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSeafood.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnSeafood.FillColor = System.Drawing.Color.Transparent;
            this.btnSeafood.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSeafood.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnSeafood.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSeafood.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnSeafood.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSeafood.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnSeafood.Location = new System.Drawing.Point(13, 54);
            this.btnSeafood.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnSeafood.Name = "btnSeafood";
            this.btnSeafood.Size = new System.Drawing.Size(215, 45);
            this.btnSeafood.TabIndex = 1;
            this.btnSeafood.Text = "Hải sản";
            this.btnSeafood.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSeafood.TextOffset = new System.Drawing.Point(15, 0);
            this.btnSeafood.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnOrders
            // 
            this.btnOrders.BorderRadius = 6;
            this.btnOrders.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnOrders.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnOrders.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnOrders.FillColor = System.Drawing.Color.Transparent;
            this.btnOrders.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOrders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnOrders.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnOrders.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnOrders.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnOrders.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnOrders.Location = new System.Drawing.Point(13, 105);
            this.btnOrders.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnOrders.Name = "btnOrders";
            this.btnOrders.Size = new System.Drawing.Size(215, 45);
            this.btnOrders.TabIndex = 2;
            this.btnOrders.Text = "Đơn hàng";
            this.btnOrders.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnOrders.TextOffset = new System.Drawing.Point(15, 0);
            this.btnOrders.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnReports
            // 
            this.btnReports.BorderRadius = 6;
            this.btnReports.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnReports.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnReports.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnReports.FillColor = System.Drawing.Color.Transparent;
            this.btnReports.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnReports.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnReports.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnReports.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReports.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnReports.Location = new System.Drawing.Point(13, 156);
            this.btnReports.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnReports.Name = "btnReports";
            this.btnReports.Size = new System.Drawing.Size(215, 45);
            this.btnReports.TabIndex = 3;
            this.btnReports.Text = "Báo cáo";
            this.btnReports.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReports.TextOffset = new System.Drawing.Point(15, 0);
            this.btnReports.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnCategory
            // 
            this.btnCategory.BorderRadius = 6;
            this.btnCategory.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnCategory.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCategory.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnCategory.FillColor = System.Drawing.Color.Transparent;
            this.btnCategory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnCategory.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCategory.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCategory.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCategory.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnCategory.Location = new System.Drawing.Point(13, 207);
            this.btnCategory.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnCategory.Name = "btnCategory";
            this.btnCategory.Size = new System.Drawing.Size(215, 45);
            this.btnCategory.TabIndex = 4;
            this.btnCategory.Text = "Danh mục";
            this.btnCategory.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCategory.TextOffset = new System.Drawing.Point(15, 0);
            this.btnCategory.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnCustomer
            // 
            this.btnCustomer.BorderRadius = 6;
            this.btnCustomer.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnCustomer.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCustomer.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnCustomer.FillColor = System.Drawing.Color.Transparent;
            this.btnCustomer.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCustomer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnCustomer.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnCustomer.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnCustomer.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomer.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnCustomer.Location = new System.Drawing.Point(13, 258);
            this.btnCustomer.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnCustomer.Name = "btnCustomer";
            this.btnCustomer.Size = new System.Drawing.Size(215, 45);
            this.btnCustomer.TabIndex = 5;
            this.btnCustomer.Text = "Khách hàng";
            this.btnCustomer.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnCustomer.TextOffset = new System.Drawing.Point(15, 0);
            this.btnCustomer.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnTable
            // 
            this.btnTable.BorderRadius = 6;
            this.btnTable.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnTable.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnTable.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnTable.FillColor = System.Drawing.Color.Transparent;
            this.btnTable.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnTable.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnTable.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnTable.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnTable.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnTable.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnTable.Location = new System.Drawing.Point(13, 309);
            this.btnTable.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnTable.Name = "btnTable";
            this.btnTable.Size = new System.Drawing.Size(215, 45);
            this.btnTable.TabIndex = 6;
            this.btnTable.Text = "Bàn ăn";
            this.btnTable.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnTable.TextOffset = new System.Drawing.Point(15, 0);
            this.btnTable.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnMenu
            // 
            this.btnMenu.BorderRadius = 6;
            this.btnMenu.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnMenu.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnMenu.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnMenu.FillColor = System.Drawing.Color.Transparent;
            this.btnMenu.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMenu.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnMenu.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnMenu.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnMenu.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnMenu.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnMenu.Location = new System.Drawing.Point(13, 360);
            this.btnMenu.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(215, 45);
            this.btnMenu.TabIndex = 7;
            this.btnMenu.Text = "Thực đơn";
            this.btnMenu.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnMenu.TextOffset = new System.Drawing.Point(15, 0);
            this.btnMenu.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnInventory
            // 
            this.btnInventory.BorderRadius = 6;
            this.btnInventory.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnInventory.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInventory.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnInventory.FillColor = System.Drawing.Color.Transparent;
            this.btnInventory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnInventory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnInventory.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnInventory.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnInventory.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInventory.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnInventory.Location = new System.Drawing.Point(13, 411);
            this.btnInventory.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(215, 45);
            this.btnInventory.TabIndex = 8;
            this.btnInventory.Text = "Kho hàng";
            this.btnInventory.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInventory.TextOffset = new System.Drawing.Point(15, 0);
            this.btnInventory.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnMenuQR
            // 
            this.btnMenuQR.BorderRadius = 6;
            this.btnMenuQR.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnMenuQR.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnMenuQR.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnMenuQR.FillColor = System.Drawing.Color.Transparent;
            this.btnMenuQR.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnMenuQR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnMenuQR.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnMenuQR.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnMenuQR.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnMenuQR.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnMenuQR.Location = new System.Drawing.Point(13, 462);
            this.btnMenuQR.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnMenuQR.Name = "btnMenuQR";
            this.btnMenuQR.Size = new System.Drawing.Size(215, 45);
            this.btnMenuQR.TabIndex = 8;
            this.btnMenuQR.Text = "Menu QR";
            this.btnMenuQR.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnMenuQR.TextOffset = new System.Drawing.Point(15, 0);
            this.btnMenuQR.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnSupplier
            // 
            this.btnSupplier.BorderRadius = 6;
            this.btnSupplier.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnSupplier.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSupplier.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnSupplier.FillColor = System.Drawing.Color.Transparent;
            this.btnSupplier.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSupplier.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnSupplier.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnSupplier.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnSupplier.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSupplier.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnSupplier.Location = new System.Drawing.Point(13, 462);
            this.btnSupplier.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnSupplier.Name = "btnSupplier";
            this.btnSupplier.Size = new System.Drawing.Size(215, 45);
            this.btnSupplier.TabIndex = 9;
            this.btnSupplier.Text = "Nhà cung cấp";
            this.btnSupplier.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnSupplier.TextOffset = new System.Drawing.Point(15, 0);
            this.btnSupplier.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnUser
            // 
            this.btnUser.BorderRadius = 6;
            this.btnUser.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnUser.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnUser.CheckedState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(19)))), ((int)(((byte)(146)))), ((int)(((byte)(236)))));
            this.btnUser.FillColor = System.Drawing.Color.Transparent;
            this.btnUser.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.btnUser.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.btnUser.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnUser.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnUser.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnUser.Location = new System.Drawing.Point(13, 513);
            this.btnUser.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnUser.Name = "btnUser";
            this.btnUser.Size = new System.Drawing.Size(215, 45);
            this.btnUser.TabIndex = 10;
            this.btnUser.Text = "Người dùng";
            this.btnUser.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnUser.TextOffset = new System.Drawing.Point(15, 0);
            this.btnUser.Click += new System.EventHandler(this.NavigationButton_Click);

            // btnLogout
            // 
            this.btnLogout.BorderRadius = 6;
            this.btnLogout.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.btnLogout.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnLogout.CheckedState.ForeColor = System.Drawing.Color.Red;
            this.btnLogout.FillColor = System.Drawing.Color.Transparent;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68))))); // Red color
            this.btnLogout.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
            this.btnLogout.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLogout.ImageOffset = new System.Drawing.Point(5, 0);
            this.btnLogout.Location = new System.Drawing.Point(13, 564);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(215, 45);
            this.btnLogout.TabIndex = 11;
            this.btnLogout.Text = "Đăng xuất";
            this.btnLogout.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnLogout.TextOffset = new System.Drawing.Point(15, 0);
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);

            // 
            // pnlUserInfo
            // 
            this.pnlUserInfo.Controls.Add(this.lblUserRole);
            this.pnlUserInfo.Controls.Add(this.lblUserName);
            this.pnlUserInfo.Controls.Add(this.picUserAvatar);
            this.pnlUserInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlUserInfo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55))))); // Slightly lighter dark
            this.pnlUserInfo.Location = new System.Drawing.Point(0, 650);
            this.pnlUserInfo.Name = "pnlUserInfo";
            this.pnlUserInfo.Size = new System.Drawing.Size(240, 70);
            this.pnlUserInfo.TabIndex = 4;
            // 
            // picUserAvatar
            // 
            this.picUserAvatar.Image = null;
            this.picUserAvatar.ImageRotate = 0F;
            this.picUserAvatar.Location = new System.Drawing.Point(15, 15);
            this.picUserAvatar.Name = "picUserAvatar";
            this.picUserAvatar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            this.picUserAvatar.Size = new System.Drawing.Size(40, 40);
            this.picUserAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picUserAvatar.TabIndex = 0;
            this.picUserAvatar.TabStop = false;
            // 
            // lblUserName
            // 
            this.lblUserName.BackColor = System.Drawing.Color.Transparent;
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.White;
            this.lblUserName.Location = new System.Drawing.Point(65, 15);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(60, 17);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "Admin";
            // 
            // lblUserRole
            // 
            this.lblUserRole.BackColor = System.Drawing.Color.Transparent;
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblUserRole.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(163)))), ((int)(((byte)(175)))));
            this.lblUserRole.Location = new System.Drawing.Point(65, 35);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(35, 15);
            this.lblUserRole.TabIndex = 2;
            this.lblUserRole.Text = "Role";

            // 
            // pnlContent - Panel chính bên phải, chứa các form con
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251))))); // Gray-50
            this.pnlContent.BorderRadius = 0;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this.pnlContent.Location = new System.Drawing.Point(240, 40);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(15); // Padding cho đẹp
            this.pnlContent.Size = new System.Drawing.Size(784, 536);
            this.pnlContent.TabIndex = 2;
            // 
            // pnlTitleBar - Custom Title Bar với Guna UI2 (màu xanh nhạt đẹp)
            // 
            this.pnlTitleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnlTitleBar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(254)))));
            this.pnlTitleBar.BorderThickness = 1;
            this.pnlTitleBar.Controls.Add(this.lblTitle);
            this.pnlTitleBar.Controls.Add(this.btnSettings);
            this.pnlTitleBar.Controls.Add(this.btnMinimize);
            this.pnlTitleBar.Controls.Add(this.btnMaximize);
            this.pnlTitleBar.Controls.Add(this.btnClose);
            this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTitleBar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(246)))), ((int)(((byte)(255)))));
            this.pnlTitleBar.Location = new System.Drawing.Point(240, 0);
            this.pnlTitleBar.Name = "pnlTitleBar";
            this.pnlTitleBar.Size = new System.Drawing.Size(784, 40);
            this.pnlTitleBar.TabIndex = 3;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.lblTitle.Location = new System.Drawing.Point(15, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(300, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Quản lý cửa hàng";
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BorderRadius = 0;
            this.btnSettings.CheckedState.Parent = this.btnSettings;
            this.btnSettings.CustomImages.Parent = this.btnSettings;
            this.btnSettings.FillColor = System.Drawing.Color.Transparent;
            this.btnSettings.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnSettings.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.btnSettings.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnSettings.HoverState.Parent = this.btnSettings;
            this.btnSettings.Location = new System.Drawing.Point(624, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.ShadowDecoration.Parent = this.btnSettings;
            this.btnSettings.Size = new System.Drawing.Size(40, 40);
            this.btnSettings.TabIndex = 1;
            this.btnSettings.Text = "⚙";
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.BorderRadius = 0;
            this.btnMinimize.CheckedState.Parent = this.btnMinimize;
            this.btnMinimize.CustomImages.Parent = this.btnMinimize;
            this.btnMinimize.FillColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnMinimize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnMinimize.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.btnMinimize.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnMinimize.HoverState.Parent = this.btnMinimize;
            this.btnMinimize.Location = new System.Drawing.Point(624, 0);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.ShadowDecoration.Parent = this.btnMinimize;
            this.btnMinimize.Size = new System.Drawing.Size(40, 40);
            this.btnMinimize.TabIndex = 2;
            this.btnMinimize.Text = "─";
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // btnMaximize
            // 
            this.btnMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMaximize.BorderRadius = 0;
            this.btnMaximize.CheckedState.Parent = this.btnMaximize;
            this.btnMaximize.CustomImages.Parent = this.btnMaximize;
            this.btnMaximize.FillColor = System.Drawing.Color.Transparent;
            this.btnMaximize.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnMaximize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnMaximize.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this.btnMaximize.HoverState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this.btnMaximize.HoverState.Parent = this.btnMaximize;
            this.btnMaximize.Location = new System.Drawing.Point(724, 0);
            this.btnMaximize.Name = "btnMaximize";
            this.btnMaximize.ShadowDecoration.Parent = this.btnMaximize;
            this.btnMaximize.Size = new System.Drawing.Size(40, 40);
            this.btnMaximize.TabIndex = 3;
            this.btnMaximize.Text = "□";
            this.btnMaximize.Click += new System.EventHandler(this.BtnMaximize_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BorderRadius = 0;
            this.btnClose.CheckedState.Parent = this.btnClose;
            this.btnClose.CustomImages.Parent = this.btnClose;
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(58)))), ((int)(((byte)(138)))));
            this.btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnClose.HoverState.ForeColor = System.Drawing.Color.White;
            this.btnClose.HoverState.Parent = this.btnClose;
            this.btnClose.Location = new System.Drawing.Point(764, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.ShadowDecoration.Parent = this.btnClose;
            this.btnClose.Size = new System.Drawing.Size(40, 40);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "✕";
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // dragControl
            // 
            this.dragControl.DockIndicatorTransparencyValue = 0.6D;
            this.dragControl.TargetControl = this.pnlTitleBar;
            this.dragControl.TransparentWhileDrag = false;
            // 
            // shadowForm
            // 
            this.shadowForm.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.shadowForm.TargetForm = this;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1024, 576);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTitleBar);
            this.Controls.Add(this.pnlSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            // Cho phép resize - không cố định kích thước
            this.MinimumSize = new System.Drawing.Size(800, 500); // Kích thước tối thiểu hợp lý
            this.Name = "FormMain";
            this.Padding = new System.Windows.Forms.Padding(0);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý cửa hàng";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlSidebar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.flowSidebar.ResumeLayout(false);
            this.pnlUserInfo.ResumeLayout(false);
            this.pnlUserInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picUserAvatar)).EndInit();
            this.pnlTitleBar.ResumeLayout(false);
            this.pnlTitleBar.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
