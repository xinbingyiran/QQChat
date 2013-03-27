using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using WebQQ2.WebQQ2;
using System.Threading.Tasks;
using System.Threading;

namespace QQChat
{
    public partial class MainForm : Form
    {
        private static readonly string PluginPath = Application.StartupPath + "\\plugin";
        private static readonly Dictionary<string, dynamic> Plugins = new Dictionary<string, dynamic>();

        private QQ _qq;

        private List<GroupForm> _groups;
        private List<FriendForm> _friends;
        private SystemForm _system;

        public MainForm()
        {
            InitializeComponent();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("演示程序");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadPlugins();
            _groups = new List<GroupForm>();
            _friends = new List<FriendForm>();
        }

        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var uin = treeViewF.SelectedNode.Tag.ToString();
                new Task(() => GetFriendNum(uin)).Start();
            }
        }

        private void treeViewF_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var uin = treeViewF.SelectedNode.Tag.ToString();
                var f = _qq.User.GetUserFriend(uin);
                if (f != null)
                {
                    SetFriendText(f, null);
                }
            }
        }

        private void treeViewG_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewG.SelectedNode != null)
            {
                var gid = treeViewG.SelectedNode.Tag.ToString();
                var g = _qq.User.GetUserGroup(gid);
                if (g != null)
                {
                    SetGroupText(g, null, null);
                }
            }
        }

        private void LoadPlugins()
        {
            string[] files = Directory.GetFiles(PluginPath);
            Array.Sort(files);
            Plugins.Clear();
            foreach (string file in files)
            {
                if (!file.ToLower().EndsWith(".dll"))
                {
                    continue;
                }
                try
                {
                    Assembly ab = Assembly.LoadFrom(file);
                    Type[] types = ab.GetTypes();
                    foreach (Type t in types)
                    {
                        if (!t.IsClass || t.GetInterface("MessageDeal.IMessageDeal") == null)
                        {
                            continue;
                        }
                        if (Plugins.Keys.Contains(t.FullName))//已包含
                        {
                            continue;
                        }
                        dynamic o = ab.CreateInstance(t.FullName);
                        AddMenu(t, o);
                        Plugins.Add(t.FullName, o);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void AddMenu(Type t, dynamic o)
        {
            Dictionary<string, string> menus = o.Menus; ;
            ToolStripItem[] subitems = new ToolStripItem[menus.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> menu in menus)
            {
                ToolStripItem item = new ToolStripMenuItem(menu.Key);
                item.Click += (sender, e) => { o.MenuClicked(menu.Value); };
                subitems[i] = item;
                i++;
            }
            ToolStripMenuItem newitem = new ToolStripMenuItem(o.IName);
            newitem.DropDownItems.AddRange(subitems);
            菜单ToolStripMenuItem.DropDownItems.Add(newitem);
        }

        public void InitUser(QQ qq)
        {
            if (qq == null)
                throw new ArgumentNullException();
            _qq = qq;
            _qq.MessageFriendReceived += _user_MessageFriendReceived;
            _qq.MessageGroupReceived += _user_MessageGroupReceived;
            new Task(() =>
            {
                GetAllFriends();
                Thread.Sleep(500);
                GetAllGroups();
                Thread.Sleep(500);
                _qq.StartGetMessage();
            }).Start();
        }

        private void GetAllFriends()
        {
            _qq.RefreshFriendList();
            _qq.GetOnlineUsers();
            new Task(() =>
            {
                ShowFriendlist();
            }).Start();
        }

        private void GetAllFriendNum()
        {
            var list = _qq.User.QQFriends.FriendList.Values.ToArray();
            foreach (QQFriend f in list)
            {
                if (_qq.User.QQFriends.FriendList.Values.Contains(f))
                {
                    _qq.GetFriendQQNum(f);
                    RefreshUser(f);
                }
                else
                {
                    break;
                }
            }
        }

        private void GetFriendNum(string uin)
        {
            var f = _qq.User.GetUserFriend(uin);
            if (f != null && _qq.User.QQFriends.FriendList.Values.Contains(f))
            {
                _qq.GetFriendQQNum(f);
                RefreshUser(f);
            }
        }


        private void RefreshUser(QQFriend friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => RefreshUser(friend)));
                return;
            }
            TreeNode[] list = treeViewF.Nodes.Find(friend.uin.ToString(), true);
            if (list.Length == 1)
            {
                list[0].Text = friend.LongNameWithStatus;
            }
            var f = _friends.Find(ele => ele.ID == "F|" + friend.uin);
            if (f != null)
                f.UpdateTitle();
        }

        private void GetAllGroups()
        {
            _qq.RefreshGroupList();
            ShowGrouplist();
        }


        private void ShowFriendlist()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(ShowFriendlist));
                return;
            }
            treeViewF.Nodes.Clear();
            QQFriends result = _qq.User.QQFriends;
            foreach (var f in result.FriendList)
            {
                f.Value.tag = 0;
            }
            foreach (var g in result.GroupList)
            {
                TreeNode t = new TreeNode();
                t.Text = g.Value.name;
                t.Tag = g.Value.index;
                t.Name = t.Tag.ToString();
                foreach (var f in result.FriendList)
                {
                    if (f.Value.categories == g.Value.index)
                    {
                        f.Value.tag = 1;
                        TreeNode e = new TreeNode();
                        e.Text = f.Value.LongNameWithStatus;
                        e.Tag = f.Value.uin;
                        e.Name = e.Tag.ToString();
                        t.Nodes.Add(e);
                    }
                }
                treeViewF.Nodes.Add(t);
            }
            TreeNode vfz = new TreeNode() { Text = "未分组", Tag = -1, Name = "-1" };
            treeViewF.Nodes.Add(vfz);
            foreach (var f in result.FriendList)
            {
                if (f.Value.tag as int? == 0)
                {
                    TreeNode e = new TreeNode();
                    e.Text = f.Value.LongNameWithStatus;
                    e.Tag = f.Value.uin;
                    e.Name = e.Tag.ToString();
                    vfz.Nodes.Add(e);
                }
            }
        }

        private void ShowGrouplist()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(ShowGrouplist));
                return;
            }
            treeViewG.Nodes.Clear();
            QQGroups result = _qq.User.QQGroups;
            foreach (var f in result.GroupList)
            {
                TreeNode e = new TreeNode();
                e.Text = f.Value.LongName;
                e.Tag = f.Value.gid;
                e.Name = e.Tag.ToString();
                treeViewG.Nodes.Add(e);
            }
        }

        private void _user_MessageGroupReceived(object sender, GroupEventArgs e)
        {
            SetGroupText(e.Group, e.User, e.MsgContent);

            if (e.Time > _qq.User.LoginTime)
            {
                foreach (var p in Plugins)
                {
                    string rmsg = p.Value.DealGroupMessage(e.MsgContent);
                    if (rmsg != null)
                    {
                        SendGroupMessage(e.Group, rmsg);
                        break;
                    }
                }
            }
        }

        private void _user_MessageFriendReceived(object sender, FriendEventArgs e)
        {
            switch (e.Mtype)
            {
                case MessageEventType.MESSAGE_COMMON:
                    {
                        SetFriendText(e.User, e.MsgContent);
                        if (e.Time > _qq.User.LoginTime)
                        {
                            foreach (var p in Plugins)
                            {
                                string rmsg = p.Value.DealFriendMessage(e.MsgContent);
                                if (rmsg != null)
                                {
                                    SendFriendMessage(e.User, rmsg);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_FILE:
                    {
                        if (e.Msgs["mode"].ToString() == "recv")
                        {
                            //string accurl = _user.GetFileURL(e.Msgs["session_id"].ToString(), e.Msgs["name"].ToString(), e.Msgs["from_uin"].ToString());
                            //accurl = _user.GetFileTrueUrl(accurl);
                            //string refurl = _user.RefuseFileURL(e.Msgs["from_uin"].ToString(), e.Msgs["session_id"].ToString());
                            //refurl = _user.GetFileTrueUrl(refurl);
                            string msg = string.Format("对方尝试发送文件[{0}]:{1}", e.Msgs["session_id"], e.Msgs["name"]);
                            SetFriendText(e.User, msg);
                            //告知对方发送离线文件
                            msg = string.Format("不能接收文件[{0}],请发离线或邮箱。", e.Msgs["name"]);
                            SendFriendMessage(e.User, msg);
                        }
                        else if (e.Msgs["mode"].ToString() == "refuse")
                        {
                            SetFriendText(e.User, string.Format("对方取消发送文件[{0}]", e.Msgs["session_id"]));
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_OFFLINE:
                    {
                        string accurl = _qq.GetOfffileURL(e.Msgs["ip"].ToString(), e.Msgs["port"].ToString(), e.Msgs["name"].ToString(), e.Msgs["rkey"].ToString());
                        //string refurl = _user.RefuleOfffileURL(e.Msgs["from_uin"].ToString(), e.Msgs["name"].ToString(), e.Msgs["size"].ToString());
                        string msg = string.Format("对方发送离线文件:{0}\r\n下载:{1}", e.Msgs["name"].ToString(), accurl);
                        SetFriendText(e.User, msg);
                    }
                    break;
                case MessageEventType.MESSAGE_STATUS:
                    {
                        string messagestate = string.Format("状态更改：{0} => {1} @ {2}", e.User.LongName, e.User.status, e.Time);
                        SetSystemText(messagestate, e.User);
                        RefreshUser(e.User);
                        foreach (var p in Plugins)
                        {
                            string rmsg = p.Value.StatusChanged(e.User.status);
                            if (rmsg != null)
                            {
                                SendFriendMessage(e.User, rmsg);
                                break;
                            }
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_SHAKE:
                    {
                        string msg = "抖动";
                        SetFriendText(e.User, msg);
                    }
                    break;
                case MessageEventType.MESSAGE_USER:
                    {
                        RefreshUser(e.User);
                    }
                    break;
                case MessageEventType.MESSAGE_INPUT:
                    {
                        string msg = "输入";
                        SetFriendText(e.User, msg);
                    }
                    break;
                case MessageEventType.MESSAGE_KICK:
                    {
                        string msg = string.Format("掉线：@ {0}\r\n{1}", e.Time, e.Msgs["reason"]);
                        SetSystemText(msg, null);
                    }
                    break;
                case MessageEventType.MESSAGE_SYSTEM:
                case MessageEventType.MESSAGE_UNKNOW:
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var i in e.Msgs)
                        {
                            sb.AppendLine(string.Format("{0}:{1}", i.Key, i.Value));
                        }
                        SetSystemText(sb.ToString().Trim(), null);
                    }
                    break;
                default:
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var i in e.Msgs)
                        {
                            sb.AppendLine(string.Format("{0}:{1}", i.Key, i.Value));
                        }
                        SetSystemText(sb.ToString().Trim(), null);
                    }
                    break;
            }
        }

        public void SetSystemText(string message, QQFriend friend)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetSystemText(message, friend); }));
                return;
            }
            if (_system == null)
            {
                _system = new SystemForm();
                _system.FormClosed += SystemForm_FormClosed;
            }
            _system.Show();
            _system.UpdateTitle();
            _system.AppendMessage(message, friend);
        }

        public void SetGroupText(QQGroup group, QQFriend friend, string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetGroupText(group, friend, msg); }));
                return;
            }
            var f = _groups.Find(g => g.ID == "G|" + group.gid);
            if (f == null)
            {
                f = new GroupForm()
                {
                    Group = group,
                    QQ = _qq,
                };
                f.FormClosed += GroupForm_FormClosed;
                _groups.Add(f);
                f.UpdateTitle();
            }
            f.Show();
            if (msg != null)
            {
                f.AppendMessage(msg, friend);
            }
        }

        public void SetFriendText(QQFriend friend, string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetFriendText(friend, msg); }));
                return;
            }
            var f = _friends.Find(g => g.ID == "F|" + friend.uin);
            if (f == null)
            {
                f = new FriendForm()
                {
                    Friend = friend,
                    QQ = _qq,
                };
                f.FormClosed += FriendForm_FormClosed;
                _friends.Add(f);
                f.UpdateTitle();
            }
            f.Show();
            if (msg != null)
            {
                f.AppendMessage(msg, friend);
            }
        }

        private void SystemForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _system = null;
        }

        private void GroupForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var f = sender as GroupForm;
            if (f != null)
                _groups.Remove(f);
        }

        private void FriendForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var f = sender as FriendForm;
            if (f != null)
                _friends.Remove(f);
        }

        public bool SendGroupMessage(QQGroup group, string msg)
        {
            var f = _groups.Find(g => g.ID == "G|" + group.gid);
            f.SendMessage(GetGroupMsg(group,msg));
            return true;
        }

        public bool SendFriendMessage(QQFriend friend, string msg)
        {
            if (friend.num == 0)
            {
                _qq.GetFriendQQNum(friend);
            }
            var f = _friends.Find(g => g.ID == "F|" + friend.uin);
            f.SendMessage(GetUserMsg(friend,msg));
            return true;
        }

        private string GetUserMsg(QQFriend user, string msg)
        {
            msg = msg.Replace("[name]", user.Name);
            msg = msg.Replace("[nick]", user.nick);
            msg = msg.Replace("[mark]", user.markname);
            msg = msg.Replace("[num]", user.ToString());
            msg = msg.Replace("[sname]", user.ShortName);
            msg = msg.Replace("[lname]", user.LongName);
            return msg;
        }

        private string GetGroupMsg(QQGroup group, string msg)
        {
            msg = msg.Replace("[name]", group.name);
            msg = msg.Replace("[nick]", group.name);
            msg = msg.Replace("[mark]", group.name);
            msg = msg.Replace("[num]", group.ToString());
            msg = msg.Replace("[sname]", group.ShortName);
            msg = msg.Replace("[lname]", group.LongName);
            return msg;
        }
    }
}
