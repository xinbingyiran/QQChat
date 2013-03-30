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
        private List<IRichMessage> _oldMessage;

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
            _oldMessage = new List<IRichMessage>();
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {
            LoadMembers();
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
                    var result = QQ.SendGroupMessage(Group, message);
                    var msg = string.Format("发送：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
                    if (result == false)
                    {
                        msg += "[发送失败]";
                    }
                    IRichMessage imessage = new RichMessageText(msg + Environment.NewLine)
                    {
                        MessageColor = FormHelper.PickColor()
                    };
                    if (this.IsHandleCreated)
                    {

                        BeginInvoke(new MethodInvoker(() => { imessage.AppendTo(richTextBox1); }));
                    }
                    else
                    {
                        _oldMessage.Add(imessage);
                        if (_oldMessage.Count > 50)
                        {
                            _oldMessage.RemoveRange(0, _oldMessage.Count - 50);
                        }
                    }
                }).Start();
        }

        public void AppendMessage(string message, object member)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(message, member)));
                return;
            }
            QQGroupMember gmember = member as QQGroupMember;
            if (string.IsNullOrEmpty(message))
                return;
            string rmessage = string.Format("接收[{3}]：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message, gmember == null ? "" : string.Format("{0}[{1}]", gmember.card == null ? gmember.nick : gmember.card, gmember.uin));
            Color c = FormHelper.PickColor();
            //richTextBox1.AppendLine(rmessage, c);
            new Task(() =>
            {
                List<IRichMessage> messages = MainForm.mainForm.TransMessage(richTextBox1, rmessage, gmember.uin.ToString());

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

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            new Task(() =>
                {
                    try
                    {
                        QQ.RefreshGroupInfo(Group);
                    }
                    catch (Exception) { }
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        LoadMembers();
                        button2.Enabled = true;
                    }));
                }).Start();
        }

        private void LoadMembers()
        {
            listView1.Items.Clear();
            InsertQQGroupMember(Group.owner, 0);
            InsertQQGroupMembers(Group.leaders.Values, 1);
            InsertQQGroupMembers(Group.members.Values, 2);
        }

        private Color[] colors = new Color[4] { Color.Red, Color.Blue, Color.Black, Color.Gray };

        private void InsertQQGroupMembers(IEnumerable<QQGroupMember> members, int type)
        {
            foreach (QQGroupMember member in members)
            {
                InsertQQGroupMember(member, type);
            }
        }

        private void InsertQQGroupMember(QQGroupMember member, int type)
        {
            if (member == null) return;
            var item = new ListViewItem(new string[] { member.card == null ? member.nick : member.card, member.uin.ToString() })
            {
                ForeColor = colors[type],
                Tag = member
            };
            listView1.Items.Add(item);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var member = e.Item.Tag as QQGroupMember;
                if (member != null)
                {
                    richTextBox3.Clear();
                    richTextBox3.AppendLine(string.Format("昵称:{0}", member.nick));
                    richTextBox3.AppendLine(string.Format("备注:{0}", member.card));
                    richTextBox3.AppendLine(string.Format("地址:{0}|{1}|{2}", member.country, member.province, member.city));
                    richTextBox3.AppendLine(string.Format("mflag:{0}", member.mflag));
                    richTextBox3.AppendLine(string.Format("stat:{0}", member.stat));
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                richTextBox3.Clear();
                richTextBox3.AppendLine(string.Format("名称:{0}", Group.LongName));
                richTextBox3.AppendLine(string.Format("Level:{0}", Group.level));
                richTextBox3.AppendLine(string.Format("备注:{0}", Group.memo));
                richTextBox3.AppendLine(string.Format("fmemo:{0}", Group.fingermemo));
                richTextBox3.AppendLine(string.Format("flag:{0}", Group.flag));
            }
        }

        private void GroupForm_Shown(object sender, EventArgs e)
        {
            DealOldMessage();
        }
    }
}
