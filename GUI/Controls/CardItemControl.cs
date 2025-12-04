using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using QLTN_LT.GUI.Utilities;

namespace QLTN_LT.GUI.Controls
{
    /// <summary>
    /// Custom card control cho hiển thị dữ liệu
    /// </summary>
    public partial class CardItemControl : UserControl
    {
        private Guna2Panel pnlCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblValue;
        private PictureBox picIcon;
        private Guna2Button btnAction;

        public string Title
        {
            get => lblTitle?.Text ?? "";
            set { if (lblTitle != null) lblTitle.Text = value; }
        }

        public string Subtitle
        {
            get => lblSubtitle?.Text ?? "";
            set { if (lblSubtitle != null) lblSubtitle.Text = value; }
        }

        public string Value
        {
            get => lblValue?.Text ?? "";
            set { if (lblValue != null) lblValue.Text = value; }
        }

        public Image Icon
        {
            get => picIcon?.Image;
            set { if (picIcon != null) picIcon.Image = value; }
        }

        public Color CardColor
        {
            get => pnlCard?.FillColor ?? Color.White;
            set { if (pnlCard != null) pnlCard.FillColor = value; }
        }

        public event EventHandler ActionButtonClick;

        public CardItemControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Size = new Size(250, 150);
            this.BackColor = Color.Transparent;

            // Main card panel
            pnlCard = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.FromArgb(31, 41, 55),
                BorderRadius = 10,
                Padding = new Padding(15)
            };
            // Enable shadow decoration if available
            try
            {
                pnlCard.ShadowDecoration.Enabled = true;
                pnlCard.ShadowDecoration.Color = Color.FromArgb(50, 0, 0, 0);
            }
            catch { }

            // Icon
            picIcon = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(15, 15),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent
            };

            // Title
            lblTitle = new Label
            {
                Text = "Title",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(65, 15),
                Size = new Size(170, 25)
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Subtitle",
                ForeColor = Color.FromArgb(156, 163, 175),
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Location = new Point(65, 40),
                Size = new Size(170, 20)
            };

            // Value
            lblValue = new Label
            {
                Text = "0",
                ForeColor = Color.FromArgb(59, 130, 246),
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                AutoSize = false,
                Location = new Point(15, 70),
                Size = new Size(220, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Action button
            btnAction = new Guna2Button
            {
                Text = "Chi tiết",
                Size = new Size(220, 30),
                Location = new Point(15, 115),
                FillColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                BorderRadius = 5,
                Cursor = Cursors.Hand
            };

            btnAction.Click += (s, e) => ActionButtonClick?.Invoke(this, e);

            pnlCard.Controls.Add(picIcon);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(lblSubtitle);
            pnlCard.Controls.Add(lblValue);
            pnlCard.Controls.Add(btnAction);

            this.Controls.Add(pnlCard);

            // Hover effect
            this.MouseEnter += (s, e) =>
            {
                pnlCard.FillColor = Color.FromArgb(41, 51, 65);
                AnimationHelper.ScaleIn(pnlCard, 200);
            };

            this.MouseLeave += (s, e) =>
            {
                pnlCard.FillColor = Color.FromArgb(31, 41, 55);
            };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

