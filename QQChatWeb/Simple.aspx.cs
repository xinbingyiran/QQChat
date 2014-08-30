using QQChatWeb.App_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebQQ2.Extends;
using WebQQ2.WebQQ2;

namespace QQChatWeb
{
    public partial class Simple : System.Web.UI.Page
    {
        private QQ QQItem
        {
            get { return ServiceCore.Instance.GetQQ(Session.SessionID); }
            set { ServiceCore.Instance.AddQQ(Session.SessionID, value); }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitParas();
            }
        }


        private void InitParas()
        {
            if(QQItem == null ||!QQItem.IsPreLoged)
            {
                Server.Transfer("Default.aspx",false);
            }
            else
            {
                userName.Text = string.Format("{0}[{1}]", QQItem.User.QQName, QQItem.User.QQNum);
            }
        }

        protected void refreshUser_Click(object sender, EventArgs e)
        {
            GetQzoneFriend();
        }

        protected void refreshGroup_Click(object sender, EventArgs e)
        {
            GetQunGroup();
        }

        protected void refreshMember_Click(object sender, EventArgs e)
        {
            if (_currentGroup == null)
            {
                ShowMessage("请先选择群");
                return;
            }
            GetQunMember(_currentGroup.groupid.ToString());
        }

        private void ShowMessage(string msg)
        {
            info.Text = msg;
        }


        [Serializable]
        class QzoneFriend
        {
            public long uin{get;set;}
            public string name { get; set; }
            public long index { get; set; }
            public long chang_pos { get; set; }
            public long score { get; set; }
            public string special_flag { get; set; }
            public string uncare_flag { get; set; }
            public string img { get; set; }
        }
        [Serializable]
        class QunGroup
        {
            public long auth { get; set; }
            public long flag { get; set; }
            public long groupid { get; set; }
            public string groupname { get; set; }
            public long alpha{get;set;}
            public long bbscount{get;set;}
            public long classvalue{get;set;}
            public long create_time{get;set;}
            public long filecount{get;set;}
            public string finger_memo{get;set;}
            public string group_memo{get;set;}
            public long level{get;set;}
            public long option{get;set;}
            public long total{get;set;}
        }
        [Serializable]
        class QunGroupMember
        {
            public long iscreator{get;set;}
            public long ismanager{get;set;}
            public string nick{get;set;}
            public long uin{get;set;}
        }

        private List<QzoneFriend> _flist
        {
            get
            {
                return ViewState["f"] as List<QzoneFriend>;
            }
            set
            {
                ViewState["f"] = value; 
            }
        }
        private List<QunGroup> _glist
        {
            get
            {
                return ViewState["g"] as List<QunGroup>;
            }
            set
            {
                ViewState["g"] = value; 
            }
        }
        private List<QunGroupMember> _gmlist
        {
            get
            {
                return ViewState["gm"] as List<QunGroupMember>;
            }
            set
            {
                ViewState["gm"] = value;
            }
        }
        private QunGroup _currentGroup
        {
            get
            {
                return ViewState["cg"] as QunGroup;
            }
            set
            {
                ViewState["cg"] = value;
            }
        }


        private void GetQzoneFriend()
        {
            var list = QQItem.GetFriendInfoFromZone();
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                _flist = new List<QzoneFriend>();
                foreach (Dictionary<string, object> item in data["items_list"] as ArrayList)
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
            RefreshFriendUI();
        }

        private void RefreshFriendUI()
        {
            userList.DataSource = _flist;
            userList.DataBind();
        }

        //private void buttonfd_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog sfd = new SaveFileDialog();
        //    sfd.Filter = "文本文件|*.txt|所有文件|*.*";
        //    sfd.FileName = "QFriend_" + this.Text;
        //    if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }
        //    string filename = sfd.FileName;
        //    List<string> lines = new List<string>();
        //    lines.Add(string.Format("{0}[{1}] 好友列表：", QQItem.User.QQName, QQItem.User.QQNum));
        //    foreach (var f in _flist)
        //    {
        //        lines.Add(string.Format("\t{0}[{1}]", f.name, f.uin));
        //    }
        //    File.WriteAllLines(filename, lines);
        //}

        private void GetQunGroup()
        {
            var list = QQItem.GetGroupInfoFromQun();
            if ((int)list["code"] == 0 && (int)list["subcode"] == 0)
            {
                var data = list["data"] as Dictionary<string, object>;
                _glist = new List<QunGroup>();
                foreach (Dictionary<string, object> item in data["group"] as ArrayList)
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
            RefreshGroupUI();
        }
        private void RefreshGroupUI()
        {
            groupList.DataSource = _glist;
            groupList.DataBind();
        }

        //private void buttongd_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog sfd = new SaveFileDialog();
        //    sfd.Filter = "文本文件|*.txt|所有文件|*.*";
        //    sfd.FileName = "QGroup_" + this.Text;
        //    if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }
        //    string filename = sfd.FileName;
        //    List<string> lines = new List<string>();
        //    lines.Add(string.Format("{0}[{1}] 群列表：", QQItem.User.QQName, QQItem.User.QQNum));
        //    foreach (var g in _glist)
        //    {
        //        lines.Add(string.Format("\t{0}[{1}]", g.groupname, g.groupid));
        //    }
        //    File.WriteAllLines(filename, lines);
        //}

//        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            var f = e.Node.Tag as QzoneFriend;
//            ShowMessage(string.Format(@"uin:         {0}
//name:        {1}
//index:       {2}
//chang_pos:   {3}
//score:       {4}
//special_flag:{5}
//uncare_flag: {6}
//img:         {7}",
//                 f.uin, f.name, f.index, f.chang_pos, f.score, f.special_flag, f.uncare_flag, f.img));
//        }

//        private void treeViewG_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            var group = e.Node.Tag as QunGroup;
//            if (group == null)
//            {
//                return;
//            }
//            _currentGroup = group;
//            GetQunMember(_currentGroup.groupid.ToString());
//        }


        private void GetQunMember(string groupid)
        {
            var list = QQItem.GetMemberInfoFromQun(groupid);
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
                _gmlist = new List<QunGroupMember>();
                foreach (Dictionary<string, object> item in data["item"] as ArrayList)
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
            RefreshMemberUI();
        }
        private void RefreshMemberUI()
        {
            var g = _currentGroup;
            ShowMessage(string.Format(@"groupid:     {0}
groupname:   {1}
level:       {2}
total:       {3}
create_time: {4}
filecount:   {5}
finger_memo: {6}
group_memo:  {7}",
                 g.groupid, g.groupname, g.level, g.total, QQHelper.ToTime(g.create_time).ToString("yyyy-MM-dd HH:mm:ss"), g.filecount, g.finger_memo, g.group_memo));

            memberList.DataSource = _gmlist;
            memberList.DataBind();
        }

        protected void userList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }
        //private void buttonmd_Click(object sender, EventArgs e)
        //{
        //    if (_currentGroup == null)
        //    {
        //        MessageBox.Show("请先选择群");
        //    }
        //    SaveFileDialog sfd = new SaveFileDialog();
        //    sfd.Filter = "文本文件|*.txt|所有文件|*.*";
        //    sfd.FileName = string.Format("Member_{0}[{1}]", _currentGroup.groupname, _currentGroup.groupid);
        //    if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }
        //    string filename = sfd.FileName;
        //    List<string> lines = new List<string>();
        //    lines.Add(string.Format("{0}[{1}] 群信息：", _currentGroup.groupname, _currentGroup.groupid));
        //    lines.Add(string.Format("群介绍：{0}", _currentGroup.finger_memo));
        //    lines.Add(string.Format("群公告：{0}", _currentGroup.group_memo));
        //    int tag = 0;
        //    foreach (var gm in _gmlist)
        //    {
        //        if (tag < 3)
        //        {
        //            if (tag == 0)
        //            {
        //                lines.Add("创建者:");
        //                tag = 1;
        //            }
        //            else if (tag == 1 && gm.ismanager != 0)
        //            {
        //                lines.Add("管理员:");
        //                tag = 2;
        //            }
        //            else if (gm.ismanager == 0)
        //            {
        //                lines.Add("成员:");
        //                tag = 3;
        //            }
        //        }
        //        lines.Add(string.Format("\t{0}[{1}]", gm.nick, gm.uin));
        //    }
        //    File.WriteAllLines(filename, lines);
        //}

//        private void treeViewm_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            var f = e.Node.Tag as QunGroupMember;
//            ShowMessage(string.Format(@"uin:         {0}
//nick:        {1}
//iscreator:   {2}
//ismanager:   {3}",
//                 f.uin, f.nick, f.iscreator, f.ismanager));
//        }

    }
}