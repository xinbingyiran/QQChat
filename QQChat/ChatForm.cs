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
            this.flowLayoutPanel1.WrapContents = false;
            this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoScrollMinSize = new Size(0, this.flowLayoutPanel1.Height + 1);
            this.flowLayoutPanel1.SizeChanged += (s, v) => this.flowLayoutPanel1.AutoScrollMinSize = new Size(0, this.flowLayoutPanel1.Height + 1);
            this.listBox1.Click += (s, v) => { item = 0; };
            this.listBox2.Click += (s, v) => { item = 1; };
            this.richTextBox2.LinkClicked += RichTextBox2_LinkClicked;
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
                BeginInvoke((Action)(() =>
                {
                    if (this.IsDisposed)
                    {
                        return;
                    }
                    this.Close();
                }));
            });
        }

        private void RichTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            OpenLink(e.LinkText);
        }

        private System.IO.FileStream _fs;
        protected override void OnClosed(EventArgs e)
        {
            if (_fs != null)
            {
                try
                {
                    lock (_fs)
                    {
                        _fs.Flush();
                        _fs.Close();
                    }
                }
                catch (Exception)
                {

                }
            }
            base.OnClosed(e);
        }

        private void WriteLog(string log)
        {
            if (_fs == null && this.QQ != null && this.QQ.User != null)
            {
                try
                {
                    var l = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var path = System.IO.Path.Combine(new System.IO.FileInfo(l).DirectoryName, this.QQ.User.QQNum + ".txt");
                    _fs = System.IO.File.Open(path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);
                    if (_fs.Length == 0)
                    {
                        lock (_fs)
                        {
                            _fs.Write(new byte[] { 0xff, 0xfe }, 0, 2);
                            _fs.Flush();
                        }
                    }
                    Task.Factory.StartNew((a) =>
                    {
                        try
                        {
                            var fs = a as System.IO.FileStream;
                            if (fs == null || fs.CanWrite)
                            {
                                long ps = 0;
                                while (fs.CanWrite)
                                {
                                    lock (fs)
                                    {
                                        var p = fs.Position;
                                        if (p != ps)
                                        {
                                            ps = p;
                                            fs.Flush();
                                        }
                                    }
                                    System.Threading.Thread.Sleep(10000);
                                }
                            }
                        }
                        catch
                        {
                            return;
                        }
                    }, _fs);
                }
                catch { };
            }
            if (_fs != null)
            {
                try
                {
                    var bs = System.Text.Encoding.Unicode.GetBytes(log);
                    lock (_fs)
                    {
                        _fs.Write(bs, 0, bs.Length);
                    }
                }
                catch { }
            }
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
        private void AppendText(string group, string name, string tag, string[] contents)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<string, string, string, string[]>)AppendText, group, name, tag, contents);
                return;
            }
            var now = DateTime.Now;
            var line = richTextBox2.Lines.FirstOrDefault();
            StringBuilder sb = new StringBuilder();
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
            if (this.flowLayoutPanel1.Controls.Count > 100)
            {
                if (this.flowLayoutPanel1.Controls.Count > 80)
                {
                    var pc = this.flowLayoutPanel1.Controls[0];
                    this.flowLayoutPanel1.Controls.RemoveAt(0);
                    while (pc.Controls.Count > 0)
                    {
                        var pcc = pc.Controls[0];
                        pc.Controls.RemoveAt(0);
                        pcc.Dispose();
                    }
                    pc.Dispose();
                }
            }
            var p = new FlowLayoutPanel()
            {
                AutoSize = true,
                Margin = _fp,
                WrapContents = true,
            };
            p.SuspendLayout();
            var s = new Label()
            {
                AutoSize = true,
                Text = now.ToString(),
                ForeColor = Color.DarkGray,
            };
            sb.AppendLine(s.Text);
            p.Controls.Add(s);
            p.SetFlowBreak(s, true);
            var g = new Label()
            {
                AutoSize = true,
                Text = group,
                ForeColor = Color.DarkRed,
            };
            g.Click += Label_Click;
            sb.Append(g.Text);
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
                sb.Append(t.Text);
                p.Controls.Add(t);
            }
            var l = new Label()
            {
                AutoSize = true,
                Text = name,
                ForeColor = Color.DarkBlue,
            };
            l.Click += Label_Click;
            sb.AppendLine(l.Text);
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
                    sb.AppendLine(ll.Text);
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
                    sb.AppendLine(tl.Text);
                    p.Controls.Add(tl);
                    p.SetFlowBreak(tl, true);
                }
            }
            WriteLog(sb.ToString());
            p.SetFlowBreak(p, true);
            this.flowLayoutPanel1.Controls.Add(p);
            p.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            if (this.checkBox1.Checked)
            {
                this.flowLayoutPanel1.ScrollControlIntoView(p);
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            var l = sender as Label;
            if (l != null)
            {
                var p = l.Parent;
                if (p != null)
                {
                    foreach (Control c in p.Controls)
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
            OpenLink((sender as LinkLabel).Text);
        }

        private static void OpenLink(string linkUrl)
        {
            System.Diagnostics.Process.Start(linkUrl);
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
