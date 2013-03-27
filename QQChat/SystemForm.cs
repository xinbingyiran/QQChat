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
    public partial class SystemForm : Form, ChatFormMethod
    {

        public string ID
        {
            get { return "S|0"; }
        }

        public void UpdateTitle()
        {
            this.Text = "系统消息";
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

        public void AppendMessage(string message, QQFriend friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(message, friend)));
                return;
            }
            if (string.IsNullOrEmpty(message))
                return;

            string rmessage = string.Format("接收[{3}]：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message, friend == null?"":friend.LongName);
            Color c = FormHelper.PickColor();
            richTextBox1.AppendLine(rmessage, c);
        }
    }
}
