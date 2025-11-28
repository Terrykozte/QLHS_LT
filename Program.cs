using System;
using System.Windows.Forms;

namespace QLTN_LT
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var loginForm = new GUI.Authentication.FormLogin();
            if (loginForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Application.Run(new GUI.Main.FormMain(loginForm.LoggedInUser));
            }
        }
    }
}

