using System;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using QLTN_LT.DTO;

namespace QLTN_LT
{
    internal static class Program
    {
        private static bool SkipLogin = true; // Đổi về false nếu muốn bật lại màn hình đăng nhập

        [STAThread]
        static void Main()
        {
            // Global exception handling to prevent app crash and show friendly errors
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (SkipLogin)
            {
                // Khởi động thẳng MainSystem với user Admin mặc định
                var defaultUser = new UserDTO
                {
                    UserID = 0,
                    Username = "admin",
                    FullName = "Administrator",
                    IsActive = true,
                    Roles = new List<string> { "Admin" }
                };
                Application.Run(new GUI.Main.FormMain(defaultUser));
            }
            else
            {
                var loginForm = new GUI.Authentication.FormLogin();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new GUI.Main.FormMain(loginForm.LoggedInUser));
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                GUI.Helper.ExceptionHandler.Handle(e.Exception, "UI Thread Exception");
            }
            catch
            {
                MessageBox.Show("Đã xảy ra lỗi không mong muốn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                    GUI.Helper.ExceptionHandler.Handle(ex, "Non-UI Thread Exception");
            }
            catch
            {
                // Fallback
            }
        }
    }
}
