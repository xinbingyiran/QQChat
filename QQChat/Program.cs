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
            var mainForm = new MainForm();
            if (mainForm.LoginForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(mainForm);
            }
        }
    }
}
