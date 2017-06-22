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
using WebQQ2.Extends;

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
            if (e.Url.AbsoluteUri.StartsWith("http://ptlogin2.qun.qq.com"))
            {
                e.Cancel = true;
                webBrowser1.Navigate("about:blank");
                this.QQ.AnylizeCookie(webBrowser1.Document.Cookie);
                this.QQ.VisitUrl(e.Url.AbsoluteUri);
                this.QQ.GetMyInfo();
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

        private void button3_Click(object sender, EventArgs e)
        {
            string url = string.Empty;
            if((ModifierKeys & Keys.Control) == Keys.Control)
            {
                url = "http://zc.qq.com/iframe/9/reg.html";
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                url = "http://zc.qq.com/";
            }
            else
            {
                MainForm.BindToParent(ShowForm<RegForm>(), this);
                this.Hide();
                return;
            }
            System.Diagnostics.Process.Start(url);
        }

        private void TraceToLoginForm()
        {
            QQ = new QQ_Web();
            SetInfo("请登录");
            webBrowser1.Navigate("https://xui.ptlogin2.qq.com/cgi-bin/xlogin?appid=715030901&daid=73&hide_close_icon=1&pt_no_auth=1&s_url=http%3A%2F%2Fqun.qq.com%2Fmember.html%23");
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

        public static Form ShowForm<T>()
            where T : Form, new()
        {
            var form = new T();
            form.Show();
            return form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.BindToParent(ShowForm<QRForm>(), this);
            this.Hide();
        }
    }
}
