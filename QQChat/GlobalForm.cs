using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQChat
{
    public partial class GlobalForm : Form
    {
        private readonly List<QunFriendGroup> _qfgList = new List<QunFriendGroup>();
        private readonly List<QunGroup> _qglist = new List<QunGroup>();
        private QQ_Base _qq;
        public GlobalForm()
        {
            InitializeComponent();
            InitEvent();
        }

        private void InitEvent()
        {
            this.treeViewF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewF_AfterSelect);
            this.buttonfd.Click += new System.EventHandler(this.buttonfd_Click);
            this.buttonf.Click += new System.EventHandler(this.buttonf_Click);
            this.treeViewG.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewG_AfterSelect);
            this.buttongd.Click += new System.EventHandler(this.buttongd_Click);
            this.buttong.Click += new System.EventHandler(this.buttong_Click);
            this.treeViewm.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewm_AfterSelect);
            this.buttonmd.Click += new System.EventHandler(this.buttonmd_Click);
            this.buttonmf.Click += new System.EventHandler(this.buttonmf_Click);
            this.buttona.Click += new System.EventHandler(this.buttona_Click);
            this.buttonad.Click += new System.EventHandler(this.buttonad_Click);
            this.buttonc.Click += new System.EventHandler(this.buttonc_Click);
            this.button1.Click += new System.EventHandler(this.button1_Click);
        }
        public void InitQQ(QQ_Base qq)
        {
            _qq = qq;
            this.Text = string.Format("{0}[{1}]", _qq.User.QQName, _qq.User.QQNum);
        }

        private void buttonf_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                GetQunFriend();
                RefreshFriendUI();
                SetInfo("Refresh Friend OK!");
            }).Start();
        }

        private void GetQunFriend()
        {
            var list = _qq.GetFriendInfoFromQun();
            if ((int)list["ec"] == 0)
            {
                _qfgList.Clear();
                if (list["result"] is Dictionary<string, object>datas && datas.Count > 0)
                {
                    foreach (var data in datas)
                    {
                        var friends = new List<QunFriend>();
                        var groupDefine = data.Value as Dictionary<string, object>;
                        var gname = groupDefine.ContainsKey("gname") ? (string)groupDefine["gname"] : "未分组";
                        if (groupDefine.ContainsKey("mems"))
                        {
                            foreach (var item in groupDefine["mems"] as ArrayList)
                            {
                                var fitem = item as Dictionary<string, object>;
                                var friend = new QunFriend
                                {
                                    uin = Convert.ToInt64(fitem["uin"]),
                                    name = (string)fitem["name"],
                                };
                                friends.Add(friend);
                            }
                        }
                        _qfgList.Add(new QunFriendGroup() { gname = gname, friends = friends });
                    }
                }
            }
            SetInfo("GetFriend OK:" + _qfgList.Count);
        }

        private void RefreshFriendUI()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RefreshFriendUI));
                return;
            }
            treeViewF.Nodes.Clear();
            treeViewF.BeginUpdate();
            foreach (var g in _qfgList)
            {
                var node = treeViewF.Nodes.Add(g.gname);
                node.Tag = g;
                foreach (var f in g.friends)
                {
                    var fnode = node.Nodes.Add(string.Format("{0}[{1}]", f.name, f.uin));
                    fnode.Tag = f;
                }
            }
            treeViewF.EndUpdate();
        }

        private void buttonfd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "文本文件|*.txt|所有文件|*.*",
                FileName = "QFriend_" + _qq.User.QQNum
            };
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            ExportQFriend(sfd.FileName);
        }

        private void ExportQFriend(string filename)
        {
            List<string> lines = new List<string>
            {
                string.Format("{0}[{1}] 好友列表：", _qq.User.QQName, _qq.User.QQNum)
            };
            foreach (var g in _qfgList)
            {
                lines.Add(g.gname + ":");
                foreach (var f in g.friends)
                {
                    lines.Add(string.Format("\t{0}[{1}]", f.name, f.uin));
                }
            }
            File.WriteAllLines(filename, lines);
            SetInfo("ExportFriend OK:" + filename);
        }

        private void buttong_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                GetQunGroup();
                RefreshGroupUI();
                //foreach (var group in _qglist.ToArray())
                //{
                //    GetQunMember(group);
                //}
                SetInfo("Refresh Group OK!");
            }).Start();
        }

        private static readonly Dictionary<string, string> QunGroupRole = new Dictionary<string, string>
        {
            { "create","我创建的" },
            { "manage","我管理的" },
            { "join","我加入的" },
        };
        private void GetQunGroup()
        {
            var list = _qq.GetGroupInfoFromQun();
            if ((int)list["ec"] == 0)
            {
                _qglist.Clear();
                foreach (var role in QunGroupRole)
                {
                    if (list.ContainsKey(role.Key))
                    {
                        var items = list[role.Key] as ArrayList;
                        if (items != null)
                        {
                            foreach (Dictionary<string, object> item in items)
                            {
                                var friend = new QunGroup
                                {
                                    role = role.Key,
                                    owner = Convert.ToInt64(item["owner"]),
                                    gcode = Convert.ToInt64(item["gc"]),
                                    gname = (string)item["gn"],
                                };
                                _qglist.Add(friend);
                            }
                        }
                    }
                }
            }
            SetInfo("GetGroup OK:" + _qglist.Count);
        }
        private void RefreshGroupUI()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RefreshGroupUI));
                return;
            }
            treeViewG.Nodes.Clear();
            treeViewG.BeginUpdate();
            foreach (var role in QunGroupRole)
            {
                var node = treeViewG.Nodes.Add(role.Value);
                node.Tag = role.Key;
                var list = _qglist.Where(e => e.role == role.Key).ToArray();
                if (list.Length > 0)
                {
                    foreach (var g in list)
                    {
                        var gnode = node.Nodes.Add(string.Format("{0}[{1}]", g.gname, g.gcode));
                        gnode.Tag = g;
                    }
                }
            }
            treeViewG.EndUpdate();
        }

        private void buttongd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "文本文件|*.txt|所有文件|*.*",
                FileName = "QGroup_" + _qq.User.QQNum
            };
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            ExportQGroup(filename);
        }

        private void ExportQGroup(string filename)
        {
            List<string> lines = new List<string>
            {
                string.Format("{0}[{1}] 群列表：", _qq.User.QQName, _qq.User.QQNum)
            };
            foreach (var role in QunGroupRole)
            {
                lines.Add(role.Value + ":");
                var list = _qglist.Where(e => e.role == role.Key).ToArray();
                foreach (var g in list)
                {
                    lines.Add(string.Format("\t{0}[{1}]", g.gname, g.gcode));
                }
            }
            File.WriteAllLines(filename, lines);
            SetInfo("ExportGroup OK:" + filename);
        }

        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var f = e.Node.Tag as QunFriend;
            if (f != null)
            {
                richTextBox1.Text = string.Format(@"uin:         {0}
name:        {1}",
                     f.uin, f.name);
                return;
            }
            var g = e.Node.Tag as QunFriendGroup;
            if (g != null)
            {
                richTextBox1.Text = string.Format(@"name:         {0}
friends:        {1}",
                     g.gname, g.friends.Count);
                return;
            }
        }

        private void treeViewG_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var group = e.Node.Tag as QunGroup;
            if (group == null)
            {
                return;
            }
            new Task(() =>
            {
                if (group.gmlist == null || group.gmlist.Count == 0)
                {
                    GetQunMember(group);
                }
                RefreshGroupinfoUI(group);
                RefreshMemberUI(group);
            }).Start();
        }

        private void buttonmf_Click(object sender, EventArgs e)
        {
            if (treeViewG.SelectedNode == null || treeViewG.SelectedNode.Tag as QunGroup == null)
            {
                MessageBox.Show("请先选择群");
                return;
            }
            var group = treeViewG.SelectedNode.Tag as QunGroup;
            new Task(() =>
            {
                GetQunMember(group);
                RefreshGroupinfoUI(group);
                RefreshMemberUI(group);
            }).Start();
        }

        private void GetQunMember(QunGroup group)
        {
            var gmlist = new List<QunGroupMember>();
            gmlist.Clear();
            var st = 0;
            var per = 20;
            var count = per + 1;
            var end = 0;
            while (st < count)
            {
                if (count - st > per)
                {
                    end = st + per;
                }
                else
                {
                    end = count - 1;
                }
                var list = _qq.GetMemberInfoFromQun(group.gcode, st, end);
                st = end + 1;
                if ((int)list["ec"] == 0)
                {
                    if (list.ContainsKey("count"))
                    {
                        count = Convert.ToInt32(list["count"]);
                    }
                    if (list.ContainsKey("mems") && list["mems"] is ArrayList mems)
                    {
                        foreach (Dictionary<string, object> item in mems)
                        {
                            var member = new QunGroupMember
                            {
                                role = (int)item["role"],
                                card = (string)item["card"],
                                uin = Convert.ToInt64(item["uin"]),
                                nick = (string)item["nick"],
                                jtime = QQHelper.ToTime(Convert.ToInt64(item["join_time"])).ToString("yyyy-MM-dd HH:mm:ss"),
                                stime = QQHelper.ToTime(Convert.ToInt64(item["last_speak_time"])).ToString("yyyy-MM-dd HH:mm:ss"),
                            };
                            gmlist.Add(member);
                        }
                    }
                }
                System.Threading.Thread.Sleep(3000);
            }
            gmlist.Sort((l, r) =>
            {
                if (l == r)
                    return 0;
                else if (l.role != r.role)
                {
                    return l.role.CompareTo(r.role);
                }
                return l.uin.CompareTo(r.uin);
            });
            group.gmlist = gmlist;
            SetInfo("GetGroupMember OK:" + group.gmlist.Count + "-" + group.gname + "[" + group.gcode + "]");
        }

        private void RefreshMemberUI(QunGroup group)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<QunGroup>(RefreshMemberUI), group);
                return;
            }
            treeViewm.Nodes.Clear();
            treeViewm.BeginUpdate();
            if (group != null && group.gmlist != null)
            {
                foreach (var gm in group.gmlist)
                {
                    treeViewm.Nodes.Add(new TreeNode(string.Format("{0}[{1}]", string.IsNullOrEmpty(gm.card) ? gm.nick : gm.card, gm.uin)) { Tag = gm });
                }
            }
            treeViewm.EndUpdate();
        }

        private void RefreshGroupinfoUI(QunGroup group)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<QunGroup>(RefreshGroupinfoUI), group);
                return;
            }
            if (group == null)
            {
                richTextBox1.Text = string.Empty;
                return;
            }
            richTextBox1.Text = string.Format(@"groupcode:     {0}
groupname:   {1}",
                 group.gcode, group.gname);

        }
        private void buttonmd_Click(object sender, EventArgs e)
        {
            if (treeViewG.SelectedNode == null || treeViewG.SelectedNode.Tag as QunGroup == null)
            {
                MessageBox.Show("请先选择群");
                return;
            }
            var group = treeViewG.SelectedNode.Tag as QunGroup;
            if (group == null)
            {
                MessageBox.Show("请先选择群");
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "文本文件|*.txt|所有文件|*.*",
                FileName = string.Format("Member_{0}[{1}]", GetFileName(group.gname), group.gcode)
            };
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            ExportGroupMember(group, filename);
        }

        private string GetFileName(string filename)
        {
            Regex r = new Regex(@"[^a-zA-Z0-9\u4e00-\u9fa5\s+#]");
            return r.Replace(filename, "");
        }

        private void ExportGroupMember(QunGroup group, string filename)
        {
            List<string> lines = new List<string>
            {
                string.Format("{0}[{1}] 群信息：", group.gname, group.gcode)
            };
            var lastRole = -1;
            if (group.gmlist != null)
            {
                foreach (var gm in group.gmlist)
                {
                    if (gm.role != lastRole)
                    {
                        if (gm.role == 0)
                        {
                            lines.Add("创建者:");
                        }
                        else if (gm.role == 1)
                        {
                            lines.Add("管理员:");
                        }
                        else
                        {
                            lines.Add("成员:");
                        }
                        lastRole = gm.role;
                    }
                    lines.Add($"\t{gm.card}({gm.nick})[{gm.uin}] - {gm.jtime} - {gm.stime}");
                }
            }
            File.WriteAllLines(filename, lines);
            SetInfo("ExportGroupMember OK:" + filename);
        }

        private void treeViewm_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var f = e.Node.Tag as QunGroupMember;
            richTextBox1.Text = $@"uin:        {f.uin}
nick:       {f.nick}
card:       {f.card}
role:       {f.role}
jtime:       {f.jtime}
stime:       {f.stime}";
        }

        private void buttona_Click(object sender, EventArgs e)
        {
            new Task(() =>
                {
                    GetQunFriend();
                    RefreshFriendUI();
                    GetQunGroup();
                    RefreshGroupUI();
                    foreach (var group in _qglist.ToArray())
                    {
                        GetQunMember(group);
                    }
                    SetInfo("Refresh All OK!");
                }).Start();
        }

        private void buttonad_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            bool cancel = false;
            var path = fbd.SelectedPath;
            var fname = path + "\\QFriend_" + _qq.User.QQNum + ".txt";
            if (CanSave(fname, ref cancel))
            {
                ExportQFriend(fname);
            }
            if (cancel) { return; }
            var gname = path + "\\QGroup_" + _qq.User.QQNum + ".txt";
            if (CanSave(gname, ref cancel))
            {
                ExportQGroup(gname);
            }
            if (cancel) { return; }
            foreach (var group in _qglist)
            {
                var mname = path + string.Format("\\Member_{0}_{1}[{2}].txt", _qq.User.QQNum, GetFileName(group.gname), group.gcode);
                if (CanSave(mname, ref cancel))
                {
                    ExportGroupMember(group, mname);
                }
                if (cancel) { return; }
            }
            SetInfo("ExportAll OK:" + path);
        }

        private bool CanSave(string name, ref bool cancel)
        {
            if (!File.Exists(name))
            {
                return true;
            }
            var result = MessageBox.Show("文件已存在：" + name + " 是否覆盖", "覆盖确认", MessageBoxButtons.YesNoCancel);
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                cancel = true;
                return false;
            }
            return result == System.Windows.Forms.DialogResult.Yes;
        }

        private void buttonc_Click(object sender, EventArgs e)
        {
            richTextBox2.Clear();
        }

        private void SetInfo(string text)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action<string>)SetInfo, text);
                return;
            }
            richTextBox2.AppendText(DateTime.Now.ToString("HH:mm:ss:") + text + Environment.NewLine);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new QunMemberManager();
            MainForm.BindToParent(manager, this);
            manager.Show();
            manager.InitParas(this._qfgList, this._qglist);
        }
    }

    internal class QunFriendGroup
    {
        public string gname;
        public List<QunFriend> friends;
    }

    internal class QunFriend
    {
        public long uin;
        public string name;
    }
    internal class QunGroup
    {
        public string role;
        public long gcode;
        public string gname;
        public long owner;
        public List<QunGroupMember> gmlist;
    }
    internal class QunGroupMember
    {
        public long uin;
        public string nick;
        public string card;
        public int role;
        public string jtime;
        public string stime;
    }

}
