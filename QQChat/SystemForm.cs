using QQChat.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class SystemForm : Form, ChatFormMethod
    {

        public string ID
        {
            get { return "S|0"; }
        }

        public bool HasMessage
        {
            get { return false; }
        }

        public void UpdateTitle()
        {
            this.Text = @"系统消息";
        }

        public SystemForm()
        {
            InitializeComponent();
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {
        }

        public void SendMessage(string message)
        {
        }

        public void AppendMessage(object sender, DateTime time, params IRichMessage[] messages)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(sender, time, messages)));
                return;
            }
            if (messages == null || messages.Length == 0)
                return;
            string rmessage = string.Format("接收：{0:yyyy-MM-dd HH:mm:ss}{1}", time, Environment.NewLine);

            richTextBox1.AppendLine(rmessage);
            foreach (IRichMessage msg in messages)
            {
                msg.AppendTo(richTextBox1);
            }
            richTextBox1.AppendLine("");
        }
    }
}
