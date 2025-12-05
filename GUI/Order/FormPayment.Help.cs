using System;
using System.Windows.Forms;

namespace QLTN_LT.GUI.Order
{
    public partial class FormPayment
    {
        private void btnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new QLTN_LT.GUI.Helper.FormShortcuts();
                frm.ShowDialog(this);
            }
            catch { }
        }
    }
}










