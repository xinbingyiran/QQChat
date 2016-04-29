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
            MainForm.BindToParent(ShowGlobalForm(QQ), this);
            this.Hide();
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
            SetInfo("请登录");
            webBrowser1.Navigate("http://xui.ptlogin2.qq.com/cgi-bin/xlogin?appid=549000912&s_url=http%3A//qzs.qq.com/&style=22");
        }
        
        public static Form ShowGlobalForm(QQ_Base qq)
        {
            var form = new GlobalForm();
            form.InitQQ(qq);
            form.Show();
            return form;
        }

        public static void BindToParent(Form form, Form parent)
        {
            form.FormClosed += (s, e) =>
            {
                if (parent != null && !parent.IsDisposed)
                {
                    parent.Show();
                }
            };
            parent.FormClosed += (s, e) =>
            {
                form.Close();
            };
        }

        public static Form ShowQRForm()
        {
            var form = new QRForm();
            form.Show();
            return form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.BindToParent(ShowQRForm(), this);
            this.Hide();
        }
    }
}
