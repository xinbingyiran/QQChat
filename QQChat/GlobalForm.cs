using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.Extends;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class GlobalForm : Form
    {
        class QzoneFriend
        {
            public long uin;
            public string name;
            public long index;
            public long chang_pos;
            public long score;
            public string special_flag;
            public string uncare_flag;
            public string img;
        }
        class QunGroup
        {
            public long auth;
            public long flag;
            public long groupid;
            public string groupname;
            public long alpha;
            public long bbscount;
            public long classvalue;
            public long create_time;
            public long filecount;
            public string finger_memo;
            public string group_memo;
            public long level;
            public long option;
            public long total;
        }
        class QunGroupMember
        {
            public long iscreator;
            public long ismanager;
            public string nick;
            public long uin;
        }

        private List<QzoneFriend> _flist = new List<QzoneFriend>();
        private List<QunGroup> _glist = new List<QunGroup>();
        private List<QunGroupMember> _gmlist = new List<QunGroupMember>();
        private QQ _qq;
        private QunGroup _currentGroup;
        public GlobalForm()
        {
            InitializeComponent();
        }

        public void InitQQ(QQ qq)
        {
            _qq = qq;
        }

        private void GlobalForm_Load(object sender, EventArgs e)
        {
        }
        private void GlobalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void GlobalForm_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Text = string.Format("{0}[{1}]", _qq.User.QQName, _qq.User.QQNum);
            }
        }

        private void buttonf_Click(object sender, EventArgs e)
        {
            new Task(GetQzoneFriend).Start();
        }

        private void GetQzoneFriend()
        {
            var list = _qq.GetFriendInfoFromZone();
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                _flist.Clear();
                var items = data["items_list"] as ArrayList;
                if (items != null)
                {
                    foreach (Dictionary<string, object> item in items)
                    {
                        var friend = new QzoneFriend
                        {
                            uin = Convert.ToInt64(item["uin"]),
                            name = (string)item["name"],
                            index = Convert.ToInt64(item["index"]),
                            chang_pos = Convert.ToInt64(item["chang_pos"]),
                            score = Convert.ToInt64(item["score"]),
                            special_flag = (string)item["special_flag"],
                            uncare_flag = (string)item["uncare_flag"],
                            img = (string)item["img"]
                        };
                        _flist.Add(friend);
                    }
                    _flist.Sort((l, r) => l.uin.CompareTo(r.uin));
                }
            }
            RefreshFriendUI();
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
            foreach (var f in _flist)
            {
                treeViewF.Nodes.Add(new TreeNode(string.Format("{0}[{1}]", f.name, f.uin)) { Tag = f });
            }
            treeViewF.EndUpdate();
        }

        private void buttonfd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|所有文件|*.*";
            sfd.FileName = "QFriend_" + this.Text;
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 好友列表：", _qq.User.QQName, _qq.User.QQNum));
            foreach (var f in _flist)
            {
                lines.Add(string.Format("\t{0}[{1}]", f.name, f.uin));
            }
            File.WriteAllLines(filename, lines);
        }

        private void buttong_Click(object sender, EventArgs e)
        {
            new Task(GetQunGroup).Start();
        }

        private void GetQunGroup()
        {
            var list = _qq.GetGroupInfoFromQun();
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                _glist.Clear();
                var items = data["group"] as ArrayList;
                if (items != null)
                {
                    foreach (Dictionary<string, object> item in items)
                    {
                        var friend = new QunGroup
                        {
                            auth = Convert.ToInt64(item["auth"]),
                            flag = Convert.ToInt64(item["flag"]),
                            groupid = Convert.ToInt64(item["groupid"]),
                            groupname = (string)item["groupname"],
                        };
                        _glist.Add(friend);
                    }
                    _glist.Sort((l, r) => l.groupid.CompareTo(r.groupid));
                }
            }
            RefreshGroupUI();
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
            foreach (var g in _glist)
            {
                treeViewG.Nodes.Add(new TreeNode(string.Format("{0}[{1}]", g.groupname, g.groupid)) { Tag = g });
            }
            treeViewG.EndUpdate();
        }

        private void buttongd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|所有文件|*.*";
            sfd.FileName = "QGroup_" + this.Text;
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 群列表：", _qq.User.QQName, _qq.User.QQNum));
            foreach (var g in _glist)
            {
                lines.Add(string.Format("\t{0}[{1}]", g.groupname, g.groupid));
            }
            File.WriteAllLines(filename, lines);
        }

        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var f = e.Node.Tag as QzoneFriend;
            richTextBox1.Text = string.Format(@"uin:         {0}
name:        {1}
index:       {2}
chang_pos:   {3}
score:       {4}
special_flag:{5}
uncare_flag: {6}
img:         {7}",
                 f.uin, f.name, f.index, f.chang_pos, f.score, f.special_flag, f.uncare_flag, f.img);
        }

        private void treeViewG_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var group = e.Node.Tag as QunGroup;
            if (group == null)
            {
                return;
            }
            _currentGroup = group;
            new Task(() => GetQunMember(_currentGroup.groupid.ToString())).Start();
        }

        private void buttonmf_Click(object sender, EventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("请先选择群");
                return;
            }
            new Task(() => GetQunMember(_currentGroup.groupid.ToString())).Start();
        }

        private void GetQunMember(string groupid)
        {
            var list = _qq.GetMemberInfoFromQun(groupid);
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                _currentGroup.alpha = Convert.ToInt64(data["alpha"]);
                _currentGroup.bbscount = Convert.ToInt64(data["bbscount"]);
                _currentGroup.classvalue = Convert.ToInt64(data["class"]);
                _currentGroup.create_time = Convert.ToInt64(data["create_time"]);
                _currentGroup.filecount = Convert.ToInt64(data["filecount"]);
                _currentGroup.finger_memo = (string)data["finger_memo"];
                _currentGroup.group_memo = (string)data["group_memo"];
                _currentGroup.level = Convert.ToInt64(data["level"]);
                _currentGroup.option = Convert.ToInt64(data["option"]);
                _currentGroup.total = Convert.ToInt64(data["total"]);
                _gmlist.Clear();
                var items = data["item"] as ArrayList;
                if (items != null)
                {
                    foreach (Dictionary<string, object> item in items)
                    {
                        var member = new QunGroupMember
                        {
                            iscreator = Convert.ToInt64(item["iscreator"]),
                            ismanager = Convert.ToInt64(item["ismanager"]),
                            uin = Convert.ToInt64(item["uin"]),
                            nick = (string)item["nick"],
                        };
                        _gmlist.Add(member);
                    }
                    _gmlist.Sort((l, r) =>
                    {
                        if (l == r)
                            return 0;
                        else if (l.iscreator != 0)
                            return -1;
                        else if (r.iscreator != 0)
                            return 1;
                        else if (l.ismanager != 0)
                        {
                            if (r.ismanager == 0)
                                return -1;
                        }
                        else if (r.ismanager != 0)
                        {
                            if (l.ismanager == 0)
                                return 1;
                        }
                        return l.uin.CompareTo(r.uin);
                    });
                }
            }
            RefreshMemberUI();
        }
        private void RefreshMemberUI()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(RefreshMemberUI));
                return;
            }
            var g = _currentGroup;
            richTextBox1.Text = string.Format(@"groupid:     {0}
groupname:   {1}
level:       {2}
total:       {3}
create_time: {4}
filecount:   {5}
finger_memo: {6}
group_memo:  {7}",
                 g.groupid, g.groupname, g.level, g.total, QQHelper.ToTime(g.create_time).ToString("yyyy-MM-dd HH:mm:ss"), g.filecount, g.finger_memo, g.group_memo);
            treeViewm.Nodes.Clear();
            treeViewm.BeginUpdate();
            foreach (var gm in _gmlist)
            {
                treeViewm.Nodes.Add(new TreeNode(string.Format("{0}[{1}]", gm.nick, gm.uin)) { Tag = gm });
            }
            treeViewm.EndUpdate();
        }
        private void buttonmd_Click(object sender, EventArgs e)
        {
            if (_currentGroup == null)
            {
                MessageBox.Show("请先选择群");
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|所有文件|*.*";
            sfd.FileName = string.Format("Member_{0}[{1}]", _currentGroup.groupname, _currentGroup.groupid);
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 群信息：", _currentGroup.groupname, _currentGroup.groupid));
            lines.Add(string.Format("群介绍：{0}", _currentGroup.finger_memo));
            lines.Add(string.Format("群公告：{0}", _currentGroup.group_memo));
            int tag = 0;
            foreach (var gm in _gmlist)
            {
                if (tag < 3)
                {
                    if (tag == 0)
                    {
                        lines.Add("创建者:");
                        tag = 1;
                    }
                    else if (tag == 1 && gm.ismanager != 0)
                    {
                        lines.Add("管理员:");
                        tag = 2;
                    }
                    else if (gm.ismanager == 0)
                    {
                        lines.Add("成员:");
                        tag = 3;
                    }
                }
                lines.Add(string.Format("\t{0}[{1}]", gm.nick, gm.uin));
            }
            File.WriteAllLines(filename, lines);
        }

        private void treeViewm_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var f = e.Node.Tag as QunGroupMember;
            richTextBox1.Text = string.Format(@"uin:         {0}
nick:        {1}
iscreator:   {2}
ismanager:   {3}",
                 f.uin, f.nick, f.iscreator, f.ismanager);
        }
    }
}
