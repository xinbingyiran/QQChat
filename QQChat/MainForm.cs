using WebQQ2.WebQQ2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace QQChat
{
    public partial class MainForm : Form
    {
        private GlobalForm GlobalForm;
        public QQ_Web QQ
        {
            get;
            private set;
        }
        public MainForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            webBrowser1.Navigating += webBrowser1_Navigating;
            TraceToLoginForm();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.Url);
            if (e.Url.AbsoluteUri.StartsWith("http://qzs.qq.com"))
            {
                this.QQ.AnylizeCookie(webBrowser1.Document.Cookie);
                webBrowser1.Navigate("about:blank");
                LoginOk();
            }
        }

        private void LoginOk()
        {
            SetInfo(QQ.User.QQNum + "登录成功");
            ShowGlobalForm();
        }

        private void SetInfo(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetInfo(text)));
                return;
            }
            label4.Text = DateTime.Now.ToString("HH:mm:ss:") + text;
        }

        private void InitMainForm()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(InitMainForm));
                return;
            }
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TraceToLoginForm();
        }

        private void TraceToLoginForm()
        {
            QQ = new QQ_Web();
            if (GlobalForm != null)
            {
                GlobalForm.Close();
            }
            SetInfo("请登录");
            webBrowser1.Navigate("http://xui.ptlogin2.qq.com/cgi-bin/xlogin?appid=549000912&s_url=http%3A//qzs.qq.com/&style=22");
        }

        private void ShowGlobalForm()
        {
            if (QQ == null || !QQ.IsPreLoged)
            {
                SetInfo("请先登录...");
                return;
            }
            if (GlobalForm == null)
            {
                GlobalForm = new GlobalForm();
            }
            GlobalForm.InitQQ(QQ);
            GlobalForm.Show();
            if (GlobalForm.WindowState == FormWindowState.Minimized)
            {
                GlobalForm.WindowState = FormWindowState.Normal;
            }
            GlobalForm.Activate();
        }

    }
}
