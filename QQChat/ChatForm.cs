using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        private Size ss = Size.Empty;

        private void ChatForm_Load(object sender, EventArgs e)
        {
            this.Resize += ChatForm_Resize;
            ss = this.Size - this.flowLayoutPanel1.Size;
            this.flowLayoutPanel1.WrapContents = false;
            this.flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.AutoScrollMinSize = new Size(0, this.flowLayoutPanel1.Height + 10);
            this.listBox1.Click += (s, v) => { item = 0; };
            this.listBox2.Click += (s, v) => { item = 1; };
            this.richTextBox2.LinkClicked += RichTextBox2_LinkClicked;
            this.richTextBox2.TextChanged += RichTextBox2_TextChanged;
            Task.Factory.StartNew(() =>
            {
                QQ.MessageFriendReceived += QQ_MessageFriendReceived;
                QQ.MessageGroupReceived += QQ_MessageGroupReceived;
                QQ.GetUserList();
                QQ.GetUserOnlineList();
                QQ.GetGroupList();
                BeginInvoke((Action)RefreshList);
                Task.Factory.StartNew(() =>
                {
                    foreach (var group in QQ.User.QQGroups.GroupList.Values.ToArray())
                    {
                        QQ.RefreshGroupInfo(group);
                    }
                });
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

        private void ChatForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                return;
            }
            var s = this.Size - ss;
            if (s.Width < 0)
            {
                s.Width = 0;
            }
            if (s.Height < 0)
            {
                s.Height = 0;
            }
            this.flowLayoutPanel1.Size = s;
            this.flowLayoutPanel1.AutoScrollMinSize = new Size(0, this.flowLayoutPanel1.Height + 10);
        }

        private void RichTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            OpenLink(e.LinkText);
        }

        private Dictionary<string, FileStream> _fsDict = new Dictionary<string, FileStream>();
        protected override void OnClosed(EventArgs e)
        {
            lock (_fsDict)
            {
                foreach (var item in _fsDict)
                {
                    lock (item.Value)
                    {
                        if (item.Value.CanWrite)
                        {
                            try
                            {
                                item.Value.Flush();
                            }
                            finally
                            {
                                item.Value.Close();
                            }
                        }
                    }
                }
                _fsDict.Clear();
            }
            base.OnClosed(e);
        }

        private void WriteLog(string log, string logname)
        {
            FileStream fs = null;
            lock (_fsDict)
            {
                if (_fsDict.ContainsKey(logname))
                {
                    fs = _fsDict[logname];
                }
            }
            if (fs == null && this.QQ != null && this.QQ.User != null)
            {
                try
                {
                    var l = System.Reflection.Assembly.GetEntryAssembly().Location;
                    var path = System.IO.Path.Combine(new System.IO.FileInfo(l).DirectoryName, this.QQ.User.QQNum + "_" + logname + ".txt");
                    fs = System.IO.File.Open(path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.Read);
                    if (fs.Length == 0)
                    {
                        fs.Write(new byte[] { 0xff, 0xfe }, 0, 2);
                        fs.Flush();
                    }
                    Task.Factory.StartNew((a) =>
                    {
                        try
                        {
                            var tfs = a as System.IO.FileStream;
                            if (tfs == null || tfs.CanWrite)
                            {
                                long ps = 0;
                                while (tfs.CanWrite)
                                {
                                    lock (tfs)
                                    {
                                        var p = tfs.Position;
                                        if (p != ps)
                                        {
                                            ps = p;
                                            tfs.Flush();
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
                    }, fs);
                    lock (_fsDict)
                    {
                        _fsDict.Add(logname, fs);
                    }
                }
                catch { };
            }
            if (fs != null)
            {
                try
                {
                    var bs = System.Text.Encoding.Unicode.GetBytes(log);
                    lock (fs)
                    {
                        fs.Write(bs, 0, bs.Length);
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
            StringBuilder sb = new StringBuilder();
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
                        Margin = _cp,
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
                        Margin = _cp,
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
            WriteLog(sb.ToString(), "chat");
            p.SetFlowBreak(p, true);
            this.flowLayoutPanel1.Controls.Add(p);
            p.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            PickFilter(p, this.richTextBox2);
            if (this.checkBox1.Checked)
            {
                this.flowLayoutPanel1.ScrollControlIntoView(p);
            }
        }

        private string _filterString = null;
        private Regex _filter = null;
        private Regex _reg = new Regex("^#(.+)#$");

        private void RichTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox2.Lines.Length == 0)
            {
                _filterString = null;
                _filter = null;
                return;
            }
            var line = richTextBox2.Lines[0];
            var match = _reg.Match(line);
            if (match.Success)
            {
                line = match.Groups[1].Value;
                if (_filterString == line)
                {
                    return;
                }
                if (line.Length > 0)
                {
                    try
                    {
                        _filter = new Regex(line);
                        _filterString = line;
                    }
                    catch (Exception)
                    {
                        _filterString = null;
                        _filter = null;
                    }
                }
                else
                {
                    _filterString = null;
                    _filter = null;
                }
            }
            else
            {
                _filterString = null;
                _filter = null;
            }
        }

        private void PickFilter(FlowLayoutPanel panel, RichTextBox box)
        {
            var filterString = this._filterString;
            var filter = this._filter;
            if (panel == null || box == null || filterString == null || filter == null)
            {
                return;
            }
            bool find = false;
            foreach (Control c in panel.Controls)
            {
                if (c.Margin == _cp)
                {
                    try
                    {
                        if (filter.IsMatch(c.Text))
                        {
                            find = true;
                            break;
                        }
                    }
                    catch
                    {

                    }
                }
            }
            if (find)
            {
                CopyPanelToRichTextBox(panel, box, "filter");
            }
        }

        private void Label_Click(object sender, EventArgs e)
        {
            var l = sender as Label;
            if (l != null)
            {
                CopyPanelToRichTextBox(l.Parent as FlowLayoutPanel, this.richTextBox2, "click");
            }
        }

        private void CopyPanelToRichTextBox(FlowLayoutPanel panel, RichTextBox box, string logname)
        {
            if (panel == null || box == null)
            {
                return;
            }
            var lines = richTextBox2.Lines;
            if (lines.Length > 500)
            {
                var start = richTextBox2.GetFirstCharIndexFromLine(1);
                var end = richTextBox2.GetFirstCharIndexFromLine(lines.Length - 500 + 102);
                richTextBox2.Select(start, end - start);
                richTextBox2.SelectedText = "";
            }
            StringBuilder sb = new StringBuilder();
            box.AppendText(System.Environment.NewLine);
            sb.AppendLine();
            foreach (Control c in panel.Controls)
            {
                box.SelectionColor = c.ForeColor;
                box.AppendText(c.Text);
                sb.Append(c.Text);
                if (panel.GetFlowBreak(c))
                {
                    box.AppendText(System.Environment.NewLine);
                    sb.AppendLine();
                }
            }
            WriteLog(sb.ToString(), logname);
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
        
        private void button2_Click(object sender, EventArgs e)
        {
            MainForm.Instance.ShowGlobalForm(QQ);
        }
    }
}
