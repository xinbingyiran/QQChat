using System;
using System.Windows.Forms;

namespace QQChat
{

    public partial class MainForm : Form
    {
        public QQ_Base QQ
        {
            get;
            private set;
        }
        public WebBrowser WebBroser => webBrowser1;

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

        private void TraceToLoginForm()
        {
            QQ = new QQ_Base();
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
    }
}
