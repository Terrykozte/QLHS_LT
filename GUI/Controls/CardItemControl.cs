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
        private Guna2Button btnMinus;
        private Guna2Button btnPlus;
        private bool _showQuantityControls = true;

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
        public event EventHandler PlusClicked;
        public event EventHandler MinusClicked;

        public bool ShowQuantityControls
        {
            get => _showQuantityControls;
            set
            {
                _showQuantityControls = value;
                if (btnMinus != null) btnMinus.Visible = value;
                if (btnPlus != null) btnPlus.Visible = value;
            }
        }

        public string ActionText
        {
            get => btnAction?.Text;
            set { if (btnAction != null) btnAction.Text = value; }
        }

        public CardItemControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Size = new Size(240, 320);
            this.BackColor = Color.Transparent;
            this.Margin = new Padding(10);

            // Main card panel
            pnlCard = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                BorderRadius = 12,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(229, 231, 235),
            };
            try
            {
                pnlCard.ShadowDecoration.Enabled = true;
                pnlCard.ShadowDecoration.Color = Color.FromArgb(20, 0, 0, 0);
                pnlCard.ShadowDecoration.Depth = 5;
            }
            catch { }

            // Image (larger)
            picIcon = new PictureBox
            {
                Dock = DockStyle.Top,
                Height = 160,
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.FromArgb(243, 244, 246),
                Margin = new Padding(0)
            };

            // Title
            lblTitle = new Label
            {
                Text = "Tên Món Ăn",
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new Point(15, 175),
                Size = new Size(this.Width - 30, 25)
            };

            // Subtitle/Category
            lblSubtitle = new Label
            {
                Text = "Danh mục",
                ForeColor = Color.FromArgb(107, 114, 128),
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(15, 200),
                Size = new Size(this.Width - 30, 20)
            };

            // Price
            lblValue = new Label
            {
                Text = "0 VNĐ",
                ForeColor = Color.FromArgb(17, 24, 39),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 230),
                Size = new Size(this.Width - 30, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Action button (now a general add button)
            btnAction = new Guna2Button
            {
                Text = "+",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 55, 265),
                FillColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BorderRadius = 8,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Hide old +/- buttons
            btnMinus = new Guna2Button { Visible = false };
            btnPlus = new Guna2Button { Visible = false };

            btnAction.Click += (s, e) => ActionButtonClick?.Invoke(this, e);

            pnlCard.Controls.Add(picIcon);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Controls.Add(lblSubtitle);
            pnlCard.Controls.Add(lblValue);
            pnlCard.Controls.Add(btnAction);

            this.Controls.Add(pnlCard);

            // Hover effect
            this.MouseEnter += (s, e) => { pnlCard.BorderColor = Color.FromArgb(59, 130, 246); };
            this.MouseLeave += (s, e) => { pnlCard.BorderColor = Color.FromArgb(229, 231, 235); };
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}

