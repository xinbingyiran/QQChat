using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.Extends;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class GlobalForm : Form
    {
        private List<QzoneFriend> _flist = new List<QzoneFriend>();
        private List<QunGroup> _glist = new List<QunGroup>();
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
            this.Load += new System.EventHandler(this.GlobalForm_Load);
        }

        public void InitQQ(QQ_Base qq)
        {
            _qq = qq;
        }

        private void GlobalForm_Load(object sender, EventArgs e)
        {
        }

        private void buttonf_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                GetQzoneFriend();
                RefreshFriendUI();
                SetInfo("Refresh Friend OK!");
            }).Start();
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
            SetInfo("GetFriend OK:" + _flist.Count);
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
            sfd.FileName = "QFriend_" + _qq.User.QQNum;
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            ExportQFriend(sfd.FileName);
        }

        private void ExportQFriend(string filename)
        {
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 好友列表：", _qq.User.QQName, _qq.User.QQNum));
            foreach (var f in _flist)
            {
                lines.Add(string.Format("\t{0}[{1}]", f.name, f.uin));
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
                foreach (var group in _glist.ToArray())
                {
                    GetQunMember(group);
                }
                SetInfo("Refresh Group OK!");
            }).Start();
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
            SetInfo("GetGroup OK:" + _glist.Count);
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
            sfd.FileName = "QGroup_" + _qq.User.QQNum;
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            ExportQGroup(filename);
        }

        private void ExportQGroup(string filename)
        {
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 群列表：", _qq.User.QQName, _qq.User.QQNum));
            foreach (var g in _glist)
            {
                lines.Add(string.Format("\t{0}[{1}]", g.groupname, g.groupid));
            }
            File.WriteAllLines(filename, lines);
            SetInfo("ExportGroup OK:" + filename);
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
            RefreshGroupinfoUI(group);
            RefreshMemberUI(group);
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
            var list = _qq.GetMemberInfoFromQun(group.groupid.ToString());
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                group.alpha = Convert.ToInt64(data["alpha"]);
                group.bbscount = Convert.ToInt64(data["bbscount"]);
                group.classvalue = Convert.ToInt64(data["class"]);
                group.create_time = Convert.ToInt64(data["create_time"]);
                group.filecount = Convert.ToInt64(data["filecount"]);
                group.finger_memo = (string)data["finger_memo"];
                group.group_memo = (string)data["group_memo"];
                group.level = Convert.ToInt64(data["level"]);
                group.option = Convert.ToInt64(data["option"]);
                group.total = Convert.ToInt64(data["total"]);
                List<QunGroupMember> gmlist = new List<QunGroupMember>();
                gmlist.Clear();
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
                        gmlist.Add(member);
                    }
                    gmlist.Sort((l, r) =>
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
                group.gmlist = gmlist;
            }
            SetInfo("GetGroupMember OK:" + group.gmlist.Count + "-" + group.groupname + "[" + group.groupid + "]");
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
            if (group!=null && group.gmlist != null)
            {
                foreach (var gm in group.gmlist)
                {
                    treeViewm.Nodes.Add(new TreeNode(string.Format("{0}[{1}]", gm.nick, gm.uin)) { Tag = gm });
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
            if(group == null)
            {
                richTextBox1.Text = string.Empty;
                return;
            }
            richTextBox1.Text = string.Format(@"groupid:     {0}
groupname:   {1}
level:       {2}
total:       {3}
create_time: {4}
filecount:   {5}
finger_memo: {6}
group_memo:  {7}",
                 group.groupid, group.groupname, group.level, group.total, QQHelper.ToTime(group.create_time).ToString("yyyy-MM-dd HH:mm:ss"), group.filecount, group.finger_memo, group.group_memo);

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
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|所有文件|*.*";
            sfd.FileName = string.Format("Member_{0}[{1}]", GetFileName(group.groupname), group.groupid);
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
            List<string> lines = new List<string>();
            lines.Add(string.Format("{0}[{1}] 群信息：", group.groupname, group.groupid));
            lines.Add(string.Format("群介绍：{0}", group.finger_memo));
            lines.Add(string.Format("群公告：{0}", group.group_memo));
            int tag = 0;
            if (group.gmlist != null)
            {
                foreach (var gm in group.gmlist)
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
            }
            File.WriteAllLines(filename, lines);
            SetInfo("ExportGroupMember OK:" + filename);
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

        private void buttona_Click(object sender, EventArgs e)
        {
            new Task(() =>
                {
                    GetQzoneFriend();
                    RefreshFriendUI();
                    GetQunGroup();
                    RefreshGroupUI();
                    foreach (var group in _glist.ToArray())
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
            var fname = fbd.SelectedPath + "\\QFriend_" + _qq.User.QQNum + ".txt";
            if (CanSave(fname, ref cancel))
            {
                ExportQFriend(fname);
            }
            if (cancel) { return; }
            var gname = fbd.SelectedPath + "\\QGroup_" + _qq.User.QQNum + ".txt";
            if (CanSave(gname, ref cancel))
            {
                ExportQGroup(gname);
            }
            if (cancel) { return; }
            foreach (var group in _glist)
            {
                var mname = fbd.SelectedPath + string.Format("\\Member_{0}[{1}].txt", GetFileName(group.groupname), group.groupid);
                if (CanSave(mname, ref cancel))
                {
                    ExportGroupMember(group, mname);
                }
                if (cancel) { return; }
            }
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
            MainForm.BindToParent(manager,this);
            manager.Show();
            manager.InitParas(this._flist, this._glist);
        }
    }

    internal class QzoneFriend
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
    internal class QunGroup
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
        public List<QunGroupMember> gmlist;
    }
    internal class QunGroupMember
    {
        public long iscreator;
        public long ismanager;
        public string nick;
        public long uin;
    }

}
