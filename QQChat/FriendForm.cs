using QQChat.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        private FaceForm _faceForm;

        public string ID
        {
            get { return "F|" + Friend.uin; }
        }

        public bool HasMessage
        {
            get { return _oldMessage != null && _oldMessage.Count > 0; }
        }

        public void UpdateTitle()
        {
            this.Text = Friend.LongNameWithStatus;
        }
        public FriendForm()
        {
            InitializeComponent();
            _oldMessage = new List<IRichMessage>();
            _faceForm = new FaceForm();
            _faceForm.OnFaceSelected += FaceForm_OnFaceSelected;
        }

        private void FaceForm_OnFaceSelected(object sender, FaceEventArgs e)
        {
            AppendToSend(e.FaceString);
        }

        private void AppendToSend(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(() => AppendToSend(text)));
                return;
            }
            this.richTextBox2.SelectedText = text;
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
            var fstatus = QQStatus.GetQQStatusByInternal(Friend.status);
            richTextBox3.AppendLine(string.Format("status:{0}", fstatus == null ? Friend.status : fstatus.Status));
            richTextBox3.AppendLine(string.Format("is_vip:{0}", Friend.is_vip));
            richTextBox4.Clear();
            richTextBox4.AppendLine(string.Format("昵称:{0}", QQ.User.QQName));
            richTextBox4.AppendLine(string.Format("QQ:{0}", QQ.User.QQNum));
            var ustatus = QQStatus.GetQQStatusByInternal(QQ.User.Status);
            richTextBox4.AppendLine(string.Format("状态:{0}", fstatus == null ? QQ.User.Status : ustatus.Status));
            richTextBox4.AppendLine(string.Format("登录时间:{0}", QQ.User.LoginTime));
        }

        public void SendMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SendMessage(message)));
                return;
            }
            if (string.IsNullOrEmpty(message))
                return;
            new Task(() =>
                {
                    var result = QQ.SendFriendMessage(Friend, MainForm.TransSendMessage(message));
                    var msg = string.Format("发送：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
                    if (result == false)
                    {
                        msg += "[发送失败]";
                    }
                    var imessage = MainForm.mainForm.TransMessage(msg + Environment.NewLine, Friend.uin.ToString());
                    var c = FormHelper.PickColor();
                    foreach (var emessage in imessage)
                    {
                        emessage.MessageColor = c;
                    }
                    if (this.IsHandleCreated)
                    {

                        BeginInvoke(new MethodInvoker(() =>
                            {
                                foreach (var emessage in imessage)
                                {
                                    emessage.AppendTo(richTextBox1);
                                }
                            }));
                    }
                    else
                    {
                        _oldMessage.AddRange(imessage);
                        if (_oldMessage.Count > 50)
                        {
                            _oldMessage.RemoveRange(0, _oldMessage.Count - 50);
                        }
                    }
                }).Start();
        }

        public void AppendMessage(string message, object friend, DateTime time)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(message, friend, time)));
                return;
            }
            if (string.IsNullOrEmpty(message))
                return;
            var qfriend = friend as QQFriend;
            if (qfriend == null)
                return;
            if (qfriend != Friend)
                return;
            string rmessage = string.Format("接收：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", time, Environment.NewLine, message);
            Color c = FormHelper.PickColor();
            //richTextBox1.AppendLine(rmessage, c);
            new Task(() =>
            {
                List<IRichMessage> messages = MainForm.mainForm.TransMessage(rmessage, qfriend.uin.ToString());
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
                    if (_oldMessage.Count > 50)
                    {
                        _oldMessage.RemoveRange(0, _oldMessage.Count - 50);
                    }
                }
            }).Start();
        }

        private void DealOldMessage()
        {
            if (this.IsHandleCreated)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(DealOldMessage));
                    return;
                }
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _faceForm.Show(this);
        }
    }
}
