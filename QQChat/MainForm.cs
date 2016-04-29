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
        private static MainForm _instance;
        public static MainForm Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new MainForm();
                }
                return _instance;
            }
        }
        private MainForm()
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
            ShowGlobalForm(QQ).FormClosing += _form_FormClosing;
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

        private GlobalForm _form = null;
        public Form ShowGlobalForm(QQ_Base qq)
        {
            if (qq == null || !qq.IsPreLoged)
            {
                return _form;
            }
            if (_form == null)
            {
                _form = new GlobalForm();
                _form.InitQQ(qq);
            }
            _form.Show();
            _form.BringToFront();
            return _form;
        }

        private void _form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.HideGlobalForm();
                this.HideQrForm();
                this.Show();
                this.BringToFront();
                this.TraceToLoginForm();
            }
        }

        public void HideQrForm()
        {
            if(_qrform != null)
            {
                _qrform.Hide();
            }
        }

        public void HideGlobalForm()
        {
            if (_form != null)
            {
                _form.Hide();
            }
        }

        private QRForm _qrform = null;
        public Form ShowQRForm()
        {
            if (_qrform == null)
            {
                _qrform = new QRForm();
            }
            _qrform.Show();
            _qrform.BringToFront();
            return _qrform;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowQRForm().FormClosing += _form_FormClosing;
            this.Hide();
        }
    }
}
