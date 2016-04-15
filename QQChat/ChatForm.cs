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
    public partial class ChatForm : Form
    {
        public ChatForm()
        {
            InitializeComponent();
        }

        public QQ_Smart QQ { get; internal set; }
        private int item = 0;

        private void ChatForm_Load(object sender, EventArgs e)
        {
            this.listBox1.Click += (s, v) => { item = 0; };
            this.listBox2.Click += (s, v) => { item = 1; };
            Task.Factory.StartNew(() =>
            {
                QQ.MessageFriendReceived += QQ_MessageFriendReceived;
                QQ.MessageGroupReceived += QQ_MessageGroupReceived;
                QQ.GetUserList();
                QQ.GetUserOnlineList();
                QQ.GetGroupList();
                foreach (var group in QQ.User.QQGroups.GroupList)
                {
                    QQ.RefreshGroupInfo(group.Value);
                }
                BeginInvoke((Action)RefreshList);
                foreach (var msg in QQ.DoMessageLoop())
                {

                }
            });
        }

        private void RefreshList()
        {
            this.listBox1.Items.Clear();
            this.listBox1.DisplayMember = "LongNameWithStatus";
            foreach (var f in QQ.User.QQFriends.FriendList)
            {
                this.listBox1.Items.Add(f.Value);
            }
            this.listBox2.Items.Clear();
            this.listBox2.DisplayMember = "LongName";
            foreach (var f in QQ.User.QQGroups.GroupList)
            {
                this.listBox2.Items.Add(f.Value);
            }
        }

        private void QQ_MessageGroupReceived(object sender, GroupEventArgs e)
        {
            this.AppendText(e.Group.LongName + ":" + e.Member.LongName + ":" + e.MsgContent);
        }

        private void QQ_MessageFriendReceived(object sender, FriendEventArgs e)
        {
            this.AppendText(e.User.nick + ":" + e.MsgContent);
        }

        private void AppendText(string text)
        {
            if(InvokeRequired)
            {
                BeginInvoke((Action<string>)AppendText,text);
                return;
            }
            this.richTextBox1.AppendText(text);
            this.richTextBox1.AppendText("\r\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch(item)
            {
                case 1:
                    {
                        var grp = listBox2.SelectedItem as QQGroup;
                        if (grp != null)
                        {
                            if (QQ.SendQunMessage(grp.gid, new QQ_Smart.StringContent { Content = richTextBox2.Text }))
                            {
                                richTextBox2.Clear();
                            }
                        }
                    }
                    break;
                default:
                    {
                        var frd = listBox1.SelectedItem as QQFriend;
                        if (frd != null)
                        {
                            if (QQ.SendBuddyMessage(frd.uin, new QQ_Smart.StringContent { Content = richTextBox2.Text }))
                            {
                                richTextBox2.Clear();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
