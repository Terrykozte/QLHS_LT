using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace QLTN_LT.GUI.Base
{
    public class BaseForm : Form
    {
        protected Guna2BorderlessForm BorderlessForm;
        protected Guna2ShadowForm ShadowForm;

        public BaseForm()
        {
            // Initialize Guna Components
            BorderlessForm = new Guna2BorderlessForm(this.Container);
            ShadowForm = new Guna2ShadowForm(this.Container);

            // Default Style
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            
            // Borderless Settings
            BorderlessForm.BorderRadius = 20;
            BorderlessForm.ShadowColor = Color.Gray;
            BorderlessForm.ContainerControl = this;
            BorderlessForm.DockIndicatorTransparencyValue = 0.6;
            BorderlessForm.TransparentWhileDrag = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Apply common load logic here (e.g. Fade In)
            // Note: If we use Guna2Transition, we can do it here.
        }
    }
}
