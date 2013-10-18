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
using System.Threading;
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
        private FaceForm _faceForm;
        private int findIndex;

        public string ID
        {
            get { return "G|" + Group.gid; }
        }

        public bool HasMessage
        {
            get { return _oldMessage != null && _oldMessage.Count > 0; }
        }

        public void UpdateTitle()
        {
            this.Text = Group.LongName;
        }

        public GroupForm()
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
                    var result = QQ.SendGroupMessage(Group, MainForm.TransSendMessage(message));
                    var msg = string.Format("发送：{0:yyyy-MM-dd HH:mm:ss}{1}{2}", DateTime.Now, Environment.NewLine, message);
                    if (result == false)
                    {
                        msg += "[发送失败]";
                    }
                    var imessage = MainForm.mainForm.TransMessage(msg + Environment.NewLine, "000000", 0);
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

        public void AppendMessage(object sender, DateTime time, params IRichMessage[] messages)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AppendMessage(sender, time, messages)));
                return;
            }
            if (messages == null || messages.Length == 0)
                return;
            QQGroupMember gmember = sender as QQGroupMember;
            IRichMessage hmessage = new RichMessageText(string.Format("接收[{0}]：{1:yyyy-MM-dd HH:mm:ss}{2}", gmember == null ? "" : string.Format("{0}[{1}]", gmember.card ?? gmember.nick, gmember.num), time, Environment.NewLine))
            {
                MessageColor = messages[0].MessageColor
            };
            if (this.IsHandleCreated)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    hmessage.AppendTo(richTextBox1);
                    foreach (IRichMessage msg in messages)
                    {
                        msg.AppendTo(richTextBox1);
                    }
                    richTextBox1.AppendLine("");
                }));
            }
            else
            {
                _oldMessage.Add(hmessage);
                _oldMessage.AddRange(messages);
                _oldMessage.Add(new RichMessageText(Environment.NewLine) { MessageColor = hmessage.MessageColor });
                if (_oldMessage.Count > 50)
                {
                    _oldMessage.RemoveRange(0, _oldMessage.Count - 50);
                }
            }
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
                    catch (Exception)
                    { }
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
            var item = new ListViewItem(new string[] { member.LongName })
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
                    new Task(() =>
                        {
                            if (member.num == 0)
                            {
                                GetQQMemberNum(member);

                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    e.Item.Text = member.LongName;
                                }));
                            }
                            BeginInvoke(new MethodInvoker(() =>
                            {
                                e.Item.Text = member.LongName;
                                richTextBox3.Clear();
                                richTextBox3.AppendLine(string.Format("昵称:{0}", member.nick));
                                richTextBox3.AppendLine(string.Format("名片:{0}", member.card));
                                richTextBox3.AppendLine(string.Format("号码:{0}", member.num));
                                richTextBox3.AppendLine(string.Format("地址:{0}|{1}|{2}", member.country, member.province, member.city));
                                richTextBox3.AppendLine(string.Format("mflag:{0}", member.mflag));
                                richTextBox3.AppendLine(string.Format("stat:{0}", member.stat));
                            }));
                        }).Start();
                }
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                richTextBox3.Clear();
                richTextBox3.AppendLine(string.Format("名称:{0}", Group.LongName));
                richTextBox3.AppendLine(string.Format("号码:{0}", Group.num));
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

        private void GetQQMemberNum(QQGroupMember member)
        {
            QQ.GetGroupMemberQQNum(Group, member);
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                var member = listView1.SelectedItems[0].Tag as QQGroupMember;
                if (member != null)
                {
                    new Task(() =>
                    {
                        if (member.num == 0)
                        {
                            GetQQMemberNum(member);
                        }
                        var friend = QQ.User.GetUserSess(member.uin);

                        friend.uin = member.uin;
                        friend.num = member.num;
                        friend.nick = member.nick;
                        friend.vip_level = member.vip_level;
                        friend.is_vip = member.is_vip;
                        friend.id = Group.gid;
                        MainForm.mainForm.SetSessText(friend, null, DateTime.Now, 0);
                    }).Start();
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _faceForm.Show(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var items = listView1.Items;
            var count = items.Count;
            if (findIndex >= count)
            {
                findIndex = -1;
            }
            var firstindex = -1;
            var find = textBox1.Text;
            if (find == "")
                return;
            for (int i = 0; i < count; i++)
            {
                var item = items[i];
                var isfind = item.Text.Contains(find);
                if (!isfind)
                {
                    foreach (ListViewItem.ListViewSubItem sitem in item.SubItems)
                    {
                        if (sitem.Text.Contains(find))
                        {
                            isfind = true;
                            break;
                        }
                    }
                }
                if (isfind)
                {
                    if (findIndex < item.Index)
                    {
                        firstindex = item.Index;
                        break;
                    }
                    if (firstindex == -1)
                    {
                        firstindex = item.Index;
                    }
                }
            }
            if (firstindex != -1)
            {
                items[firstindex].EnsureVisible();
                items[firstindex].Selected = true;
                listView1.Focus();
                findIndex = firstindex;
                return;
            }
            MessageBox.Show("未找到");
            return;
        }

        private CancellationTokenSource _getMemberCTS;

        private void buttongmget_Click(object sender, EventArgs e)
        {
            if (_getMemberCTS != null)
            {
                _getMemberCTS.Cancel();
                _getMemberCTS = null;
            }
            if (_getMemberCTS == null)
            {
                _getMemberCTS = new CancellationTokenSource();
            }
            Dictionary<ListViewItem, QQGroupMember> items = new Dictionary<ListViewItem, QQGroupMember>();
            foreach (ListViewItem item in listView1.Items)
            {
                QQGroupMember tagMember = item.Tag as QQGroupMember;
                if (tagMember != null)
                {
                    items.Add(item, tagMember);
                }
            }
            new Task(() =>
            {
                int tasknum = 0;
                foreach (var item in items)
                {
                    while (tasknum > 10)
                    {
                        _getMemberCTS.Token.WaitHandle.WaitOne(100);
                        _getMemberCTS.Token.ThrowIfCancellationRequested();
                    }
                    tasknum++;
                    new Task(() =>
                        {
                            try
                            {
                                _getMemberCTS.Token.ThrowIfCancellationRequested();
                                GetQQMemberNum(item.Value);
                                _getMemberCTS.Token.ThrowIfCancellationRequested();
                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    item.Key.Text = item.Value.LongName;
                                }));
                            }
                            catch (Exception)
                            { }
                            finally
                            {
                                tasknum--;
                            }
                        }).Start();
                }
            }).Start();
        }

        private void buttongmd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|所有文件|*.*";
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            lines.Add(Group.LongName);
            lines.Add("所有者\t" + Group.owner.LongName);
            var result = Group.leaders;
            foreach (var g in result.Values)
            {
                lines.Add("管理\t" + g.LongName);
            }
            result = Group.members;
            foreach (var g in result.Values)
            {
                lines.Add("\t" + g.LongName);
            }
            File.AppendAllLines(filename, lines);
        }
    }
}
