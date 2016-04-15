using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class QRForm : Form
    {
        public QQ_Smart QQ
        {
            get;
            private set;
        }
        public QRForm()
        {
            InitializeComponent();
        }

        private void QRForm_Load(object sender, EventArgs e)
        {
            this.RefreshQRCode();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.RefreshQRCode();
        }

        private void RefreshQRCode()
        {
            var qq = new QQ_Smart();
            Task.Factory.StartNew(() =>
            {
                if (!qq.SmartPreLogin())
                {
                    SetInfo("预登录失败");
                    return;
                }
                var img = qq.GetQrImage();
                this.Invoke((Action<Bitmap>)((bm) => this.pictureBox1.Image = bm), new object[] { img });
                foreach (var str in qq.DoSmartLogin(true))
                {
                    SetInfo(str);
                }
                this.QQ = qq;
                this.Invoke((Action)LoginOk);
            });
        }

        private void LoginOk()
        {
            SetInfo(QQ.User.QQNum + "登录成功");
            new ChatForm() { QQ = QQ }.Show();
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
    }
}
