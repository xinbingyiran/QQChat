using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            this.flowLayoutPanel1.WrapContents = true;
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.SizeChanged += FlowLayoutPanel1_SizeChanged;
            this.listBox1.Click += (s, v) => { item = 0; };
            this.listBox2.Click += (s, v) => { item = 1; };
            Task.Factory.StartNew(() =>
            {
                QQ.MessageFriendReceived += QQ_MessageFriendReceived;
                QQ.MessageGroupReceived += QQ_MessageGroupReceived;
                QQ.GetUserList();
                QQ.GetUserOnlineList();
                QQ.GetGroupList();
                Task.Factory.StartNew(() =>
                {
                    foreach (var group in QQ.User.QQGroups.GroupList.Values.ToArray())
                    {
                        QQ.RefreshGroupInfo(group);
                    }
                });
                BeginInvoke((Action)RefreshList);
                foreach (var msg in QQ.DoMessageLoop())
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }
                }
            });
        }

        private void FlowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Refresh();
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
            string tag = string.Empty;
            if (e.Group.owner.uin == e.Member.uin)
            {
                tag = "[群主]";
            }
            if (e.Group.leaders.ContainsKey(e.Member.uin))
            {
                tag = "[管理员]";
            }
            this.AppendText(e.Group.ShortName, e.Member.ShortName, tag, e.MsgContents);
        }

        private void QQ_MessageFriendReceived(object sender, FriendEventArgs e)
        {
            this.AppendText("来自好友", e.User.nick, "", e.MsgContents);
        }

        private static Padding _fp = new Padding(0, 5, 0, 0);
        private static Padding _cp = new Padding(20, 0, 0, 0);
        private DateTime _lt = DateTime.MinValue;
        private void AppendText(string group, string name, string tag, string[] contents)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<string, string, string, string[]>)AppendText, group, name, tag, contents);
                return;
            }
            var line = richTextBox2.Lines.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(line))
            {
                try
                {
                    Regex r = new Regex(line);
                    var str = string.Join(System.Environment.NewLine, contents);
                    if (r.IsMatch(str))
                    {
                        richTextBox2.AppendText(System.Environment.NewLine + str);
                    }
                }
                catch (Exception) { }
            }
            this.flowLayoutPanel1.SuspendLayout();
            while (this.flowLayoutPanel1.Controls.Count > 1000)
            {
                this.flowLayoutPanel1.Controls.RemoveAt(0);
            }
            var now = DateTime.Now;
            var p = new FlowLayoutPanel()
            {
                Margin = _fp,
                AutoSize = true,
            };
            _lt = now;
            var s = new Label()
            {
                AutoSize = true,
                Text = now.ToString(),
                ForeColor = Color.DarkGray,
            };
            p.Controls.Add(s);
            p.SetFlowBreak(s, true);
            var g = new Label()
            {
                AutoSize = true,
                Text = group,
                ForeColor = Color.DarkRed,
            };
            g.Click += Label_Click;
            p.Controls.Add(g);
            if (!string.IsNullOrWhiteSpace(tag))
            {
                var t = new Label()
                {
                    AutoSize = true,
                    Text = tag,
                    ForeColor = Color.DarkGreen,
                };
                t.Click += Label_Click;
                p.Controls.Add(t);
            }
            var l = new Label()
            {
                AutoSize = true,
                Text = name,
                ForeColor = Color.DarkBlue,
            };
            l.Click += Label_Click;
            p.Controls.Add(l);
            p.SetFlowBreak(l, true);
            foreach (var content in contents)
            {
                if (Uri.IsWellFormedUriString(content, UriKind.Absolute))
                {
                    var ll = new LinkLabel()
                    {
                        AutoSize = true,
                        Text = content,
                        ForeColor = Color.Black,
                    };
                    ll.Click += Label_Click;
                    ll.LinkClicked += Ll_LinkClicked;
                    p.Controls.Add(ll);
                    p.SetFlowBreak(ll, true);
                }
                else
                {
                    var tl = new Label()
                    {
                        AutoSize = true,
                        Text = content,
                        ForeColor = Color.Black,
                    };
                    tl.Click += Label_Click;
                    p.Controls.Add(tl);
                    p.SetFlowBreak(tl, true);
                }
            }
            this.flowLayoutPanel1.Controls.Add(p);
            p.SetFlowBreak(p, true);
            this.flowLayoutPanel1.ResumeLayout();
            if (this.checkBox1.Checked)
            {
                this.flowLayoutPanel1.VerticalScroll.Value = this.flowLayoutPanel1.VerticalScroll.Maximum;
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            var l = sender as Label;
            if (l != null)
            {
                var p = l.Parent;
                if(p != null)
                {
                    foreach(Control c in p.Controls)
                    {
                        if (c.ForeColor == Color.Black)
                        {
                            this.richTextBox2.AppendText(System.Environment.NewLine + c.Text);
                        }
                    }
                }
            }
        }

        private void Ll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start((sender as LinkLabel).Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (item)
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
