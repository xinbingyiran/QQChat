using System;
using System.Windows.Forms;

namespace QQChat
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new MainForm();
            MainWebBrowser = form.WebBroser;
            Application.Run(form);
        }

        public static WebBrowser MainWebBrowser;
    }
}
