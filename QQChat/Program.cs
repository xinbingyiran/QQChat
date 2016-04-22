using System;
using System.Collections.Generic;
using System.Linq;
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

            if (System.Environment.GetCommandLineArgs().Length == 2)
            {
                var qRForm = new QRForm();
                Application.Run(qRForm);
            }
            else
            {
                var mainForm = new MainForm();
                Application.Run(mainForm);
            }
        }
    }
}
