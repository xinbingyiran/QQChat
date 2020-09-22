using System;
using System.Windows.Forms;

namespace QQChat
{

    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void SetInfo(string text)
        {
            label4.Text = DateTime.Now.ToString("HH:mm:ss:") + text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var qq = new QQ_Base();
            qq.AnylizeCookie(this.richTextBox1.Text);
            qq.GetMyInfo();
            if (!string.IsNullOrWhiteSpace(qq.User.QQNum))
            {
                SetInfo(qq.User.QQNum + "登录成功");
                var form = new GlobalForm();
                form.InitQQ(qq);
                form.Show();
                BindToParent(form, this);
                this.Hide();
            }
            else
            {
                SetInfo(qq.User.QQNum + "不正确的Coocie");
            }
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
    }
}
