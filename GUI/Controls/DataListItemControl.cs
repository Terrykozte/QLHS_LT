using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Controls
{
    /// <summary>
    /// Custom list item control cho hiển thị dữ liệu trong list
    /// </summary>
    public partial class DataListItemControl : UserControl
    {
        private Guna2Panel pnlItem;
        private Label lblPrimaryText;
        private Label lblSecondaryText;
        private Label lblTertiaryText;
        private PictureBox picThumbnail;
        private Guna2Button btnEdit;
        private Guna2Button btnDelete;

        public string PrimaryText
        {
            get => lblPrimaryText?.Text ?? "";
            set { if (lblPrimaryText != null) lblPrimaryText.Text = value; }
        }

        public string SecondaryText
        {
            get => lblSecondaryText?.Text ?? "";
            set { if (lblSecondaryText != null) lblSecondaryText.Text = value; }
        }

        public string TertiaryText
        {
            get => lblTertiaryText?.Text ?? "";
            set { if (lblTertiaryText != null) lblTertiaryText.Text = value; }
        }

        public Image Thumbnail
        {
            get => picThumbnail?.Image;
            set { if (picThumbnail != null) picThumbnail.Image = value; }
        }

        public event EventHandler EditButtonClick;
        public event EventHandler DeleteButtonClick;

        public DataListItemControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Size = new Size(500, 80);
            this.BackColor = Color.Transparent;

            // Main item panel
            pnlItem = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(17, 24, 39),
                BorderRadius = 8,
                Padding = new Padding(10),
                BorderColor = Color.FromArgb(55, 65, 81),
                BorderThickness = 1
            };

            // Thumbnail
            picThumbnail = new PictureBox
            {
                Size = new Size(60, 60),
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.FromArgb(31, 41, 55),
                BorderStyle = BorderStyle.None
            };

            // Primary text
            lblPrimaryText = new Label
            {
                Text = "Item Name",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(80, 10),
                Size = new Size(300, 20)
            };

            // Secondary text
            lblSecondaryText = new Label
            {
                Text = "Secondary info",
                ForeColor = Color.FromArgb(156, 163, 175),
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Location = new Point(80, 32),
                Size = new Size(300, 18)
            };

            // Tertiary text
            lblTertiaryText = new Label
            {
                Text = "Tertiary info",
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 8),
                AutoSize = false,
                Location = new Point(80, 50),
                Size = new Size(300, 16)
            };

            // Edit button
            btnEdit = new Guna2Button
            {
                Text = "Sửa",
                Size = new Size(60, 30),
                Location = new Point(390, 25),
                FillColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8),
                BorderRadius = 4,
                Cursor = Cursors.Hand
            };

            btnEdit.Click += (s, e) => EditButtonClick?.Invoke(this, e);

            // Delete button
            btnDelete = new Guna2Button
            {
                Text = "Xóa",
                Size = new Size(60, 30),
                Location = new Point(455, 25),
                FillColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8),
                BorderRadius = 4,
                Cursor = Cursors.Hand
            };

            btnDelete.Click += (s, e) => DeleteButtonClick?.Invoke(this, e);

            pnlItem.Controls.Add(picThumbnail);
            pnlItem.Controls.Add(lblPrimaryText);
            pnlItem.Controls.Add(lblSecondaryText);
            pnlItem.Controls.Add(lblTertiaryText);
            pnlItem.Controls.Add(btnEdit);
            pnlItem.Controls.Add(btnDelete);

            this.Controls.Add(pnlItem);

            // Hover effect
            this.MouseEnter += (s, e) =>
            {
                pnlItem.FillColor = Color.FromArgb(31, 41, 55);
                pnlItem.BorderColor = Color.FromArgb(59, 130, 246);
            };

            this.MouseLeave += (s, e) =>
            {
                pnlItem.FillColor = Color.FromArgb(17, 24, 39);
                pnlItem.BorderColor = Color.FromArgb(55, 65, 81);
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

