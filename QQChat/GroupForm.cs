using QQChat.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class GroupForm : Form, ChatFormMethod
    {
        public QQ QQ { get; set; }
        public QQGroup Group { get; set; }

        public string ID
        {
            get { return "G|" + Group.gid; }
        }

        public void UpdateTitle()
        {
            this.Text = Group.LongName;
        }

        public GroupForm()
        {
            InitializeComponent();
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            new Task(() =>
                {
                    var result = QQ.SendGroupMessage(Group, message);
                    var msg = string.Format("发送：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
                    if (result == false)
                    {
                        msg += "[发送失败]";
                    }
                    BeginInvoke(new MethodInvoker(() => richTextBox1.AppendLine(msg, FormHelper.PickColor())));
                }).Start();
        }

        public void AppendMessage(string message, QQFriend friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(message, friend)));
                return;
            }
            if (string.IsNullOrEmpty(message))
                return;
            string rmessage = string.Format("接收[{3}]：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message, friend == null ? "" : friend.LongName);
            Color c = FormHelper.PickColor();
            richTextBox1.AppendLine(rmessage, c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage(richTextBox2.Text);
            richTextBox2.Text = null;
        }
    }
}
