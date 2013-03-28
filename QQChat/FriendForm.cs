using QQChat.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class FriendForm : Form, ChatFormMethod
    {
        public QQ QQ { get; set; }
        public QQFriend Friend { get; set; }
        private List<IRichMessage> _oldMessage;

        public string ID
        {
            get { return "F|" + Friend.uin; }
        }

        public void UpdateTitle()
        {
            this.Text = Friend.LongNameWithStatus;
        }
        public FriendForm()
        {
            InitializeComponent();
            _oldMessage = new List<IRichMessage>();
        }

        private void FriendForm_Load(object sender, EventArgs e)
        {
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            richTextBox3.Clear();
            richTextBox3.AppendLine(string.Format("昵称:{0}", Friend.nick));
            richTextBox3.AppendLine(string.Format("备注:{0}", Friend.markname));
            richTextBox3.AppendLine(string.Format("QQ:{0}", Friend.num));
            richTextBox3.AppendLine(string.Format("status:{0}", Friend.status));
            richTextBox3.AppendLine(string.Format("is_vip:{0}", Friend.is_vip));
            richTextBox4.Clear();
            richTextBox4.AppendLine(string.Format("昵称:{0}", QQ.User.QQName));
            richTextBox4.AppendLine(string.Format("QQ:{0}", QQ.User.QQNum));
            richTextBox4.AppendLine(string.Format("状态:{0}", QQ.User.Status));
            richTextBox4.AppendLine(string.Format("登录时间:{0}", QQ.User.LoginTime));
        }

        public void SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            new Task(() =>
                {
                    var result = QQ.SendFriendMessage(Friend, message);
                    var msg = string.Format("发送：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
                    if (result == false)
                    {
                        msg += "[发送失败]";
                    }
                    BeginInvoke(new MethodInvoker(() => richTextBox1.AppendLine(msg, FormHelper.PickColor())));
                }).Start();
        }

        public void AppendMessage(string message, object friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(message, friend)));
                return;
            }
            if (string.IsNullOrEmpty(message))
                return;
            QQFriend qfriend = friend as QQFriend;
            if (qfriend != this.Friend)
                return;
            string rmessage = string.Format("接收：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
            Color c = FormHelper.PickColor();
            //richTextBox1.AppendLine(rmessage, c);
            new Task(() =>
            {
                List<IRichMessage> messages = MainForm.mainForm.TransMessage(richTextBox1, rmessage, qfriend.uin.ToString());
                foreach (IRichMessage msg in messages)
                {
                    msg.MessageColor = c;
                }
                if (this.IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() =>
                     {
                         foreach (IRichMessage msg in messages)
                         {
                             msg.AppendTo(richTextBox1);
                         }
                         richTextBox1.AppendLine("", c);
                     }));
                }
                else
                {
                    _oldMessage.AddRange(messages);
                    _oldMessage.Add(new RichMessageText(Environment.NewLine) { MessageColor = c });
                }
            }).Start();
        }

        private void DealOldMessage()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(DealOldMessage));
                return;
            }
            if (this.IsHandleCreated)
            {
                foreach (IRichMessage msg in _oldMessage)
                {
                    msg.AppendTo(richTextBox1);
                }
                _oldMessage.Clear();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendMessage(richTextBox2.Text);
            richTextBox2.Text = null;
        }

        private void FriendForm_Shown(object sender, EventArgs e)
        {
            DealOldMessage();
        }


    }
}
