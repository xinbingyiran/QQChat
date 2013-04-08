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
using QQChat.Classes;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Collections;

namespace QQChat
{
    public partial class MainForm : Form
    {
        private static readonly string PluginPath = Application.StartupPath + "\\plugin";
        private static readonly string FacePath = Application.StartupPath + "\\face";
        private static readonly Dictionary<string, dynamic> Plugins = new Dictionary<string, dynamic>();

        private QQ _qq;

        public static MainForm mainForm;
        private List<GroupForm> _groups;
        private List<FriendForm> _friends;
        private SystemForm _system;
        private object _createlock;

        private JavaScriptSerializer _jss;

        public MainForm()
        {
            InitializeComponent();
            mainForm = this;
            _createlock = new object();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("QQ聊天程序,\r\n可编写自己的自动回复插件。\r\nDesigned by XBYR", "QQ聊天程序");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadStatus();
            LoadPlugins();
            CheckQQStatus();
            _groups = new List<GroupForm>();
            _friends = new List<FriendForm>();
            _jss = new JavaScriptSerializer();
        }

        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var uin = Convert.ToInt64(treeViewF.SelectedNode.Tag);
                new Task(() => GetFriendNum(uin)).Start();
            }
        }

        private void treeViewF_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var uin = Convert.ToInt64(treeViewF.SelectedNode.Tag);
                var f = _qq.User.GetUserFriend(uin);
                if (f != null && f.IsValid)
                {
                    SetFriendText(f, null, DateTime.Now);
                }
            }
        }

        private void treeViewG_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewG.SelectedNode != null)
            {
                var gid = Convert.ToInt64(treeViewG.SelectedNode.Tag);
                var g = _qq.User.GetUserGroup(gid);
                if (g != null)
                {
                    SetGroupText(g, null, null, DateTime.Now);
                }
            }
        }

        private void LoadStatus()
        {
            ToolStripMenuItem loginitem = new ToolStripMenuItem("重新登录");
            loginitem.Click += (sender, e) =>
            {
                _qq.MessageFriendReceived -= QQMessageFriendReceived;
                _qq.MessageGroupReceived -= QQMessageGroupReceived;
                _qq.LogOutQQ2();
                this.DialogResult = System.Windows.Forms.DialogResult.Retry;
                this.Close();
            };
            状态ToolStripMenuItem.DropDownItems.Add(loginitem);
            foreach (QQStatus status in QQStatus.AllStatus)
            {
                ToolStripMenuItem newitem = new ToolStripMenuItem(status.Status)
                {
                    Tag = status
                };
                newitem.Click += (sender, e) =>
                {
                    CheckQQStatus();
                    new Task(() =>
                        {
                            string result = _qq.ChangeStatus(status.StatusInternal);
                            if (result != null)
                            {
                                BeginInvoke(new MethodInvoker(() =>
                                    {
                                        MessageBox.Show(result, "状态更新出错。");
                                    }));
                            }
                            else
                            {
                                if (status.StatusInternal != QQStatus.StatusOffline.StatusInternal)
                                    _qq.StartGetMessage();
                            }
                        }).Start();
                };
                状态ToolStripMenuItem.DropDownItems.Add(newitem);
            }
        }

        private void CheckQQStatus()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(CheckQQStatus));
                return;
            }
            foreach (ToolStripItem item in 状态ToolStripMenuItem.DropDownItems)
            {
                if (item.Tag is QQStatus && (item as ToolStripMenuItem) != null)
                {
                    (item as ToolStripMenuItem).Checked = (item.Tag as QQStatus).StatusInternal == _qq.User.Status;
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
            ToolStripItem[] subitems = new ToolStripItem[menus.Count + 2];
            int i = 0;
            foreach (KeyValuePair<string, string> menu in menus)
            {
                ToolStripItem item = new ToolStripMenuItem(menu.Key);
                item.Click += (sender, e) => { o.MenuClicked(menu.Value); };
                subitems[i] = item;
                i++;
            }
            o.Enabled = false;
            ToolStripMenuItem statusitem = new ToolStripMenuItem("激活状态");
            statusitem.Click += (sender, e) =>
            {
                o.Enabled = !o.Enabled;
                statusitem.Checked = o.Enabled;
            };
            subitems[i] = statusitem;
            i++;

            ToolStripMenuItem helpitem = new ToolStripMenuItem("命令列表");
            helpitem.Click += (sender, e) =>
            {
                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<string, string> filter in o.Filters)
                {
                    sb.AppendFormat("{0} {1}{2}", filter.Key, filter.Value, Environment.NewLine);
                }
                MessageBox.Show(sb.ToString());
            };
            subitems[i] = helpitem;
            ToolStripMenuItem newitem = new ToolStripMenuItem(o.IName);
            newitem.DropDownItems.AddRange(subitems);
            菜单ToolStripMenuItem.DropDownItems.Add(newitem);
        }

        public void InitUser(QQ qq)
        {
            if (qq == null)
                throw new ArgumentNullException();
            _qq = qq;
            _qq.MessageFriendReceived += QQMessageFriendReceived;
            _qq.MessageGroupReceived += QQMessageGroupReceived;
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

        private void GetFriendNum(long uin)
        {
            var f = _qq.User.GetUserFriend(uin);
            if (f != null && f.IsValid && _qq.User.QQFriends.FriendList.Values.Contains(f))
            {
                _qq.GetFriendQQNum(f);
                RefreshUser(f);
            }
        }


        private void RefreshUser(QQFriend friend, bool? isMessage = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => RefreshUser(friend, isMessage)));
                return;
            }
            TreeNode[] list = treeViewF.Nodes.Find(friend.uin.ToString(), true);
            if (list.Length == 1)
            {
                list[0].Text = friend.LongNameWithStatus;
            }
            if (isMessage.HasValue)
            {
                list[0].ForeColor = isMessage.Value ? Color.Red : treeViewF.ForeColor;
                var pnode = list[0].Parent;
                while (pnode != null)
                {
                    var find = false;
                    foreach (TreeNode node in pnode.Nodes)
                    {
                        if (node.ForeColor == Color.Red)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (find)
                    {
                        pnode.ForeColor = Color.Red;
                    }
                    else
                    {
                        pnode.ForeColor = treeViewF.ForeColor;
                    }
                    pnode = pnode.Parent;
                }

            }
            var f = _friends.Find(ele => ele.ID == "F|" + friend.uin);
            if (f != null)
                f.UpdateTitle();
        }


        private void RefreshGroup(QQGroup group, bool? isMessage = null)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => RefreshGroup(group, isMessage)));
                return;
            }
            TreeNode[] list = treeViewG.Nodes.Find(group.gid.ToString(), true);
            if (list.Length == 1)
            {
                list[0].Text = group.LongName;
            }
            if (isMessage.HasValue)
            {
                list[0].ForeColor = isMessage.Value ? Color.Red : treeViewF.ForeColor;
            }
            var g = _groups.Find(ele => ele.ID == "G|" + group.gid);
            if (g != null)
                g.UpdateTitle();
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

        private void QQMessageGroupReceived(object sender, GroupEventArgs e)
        {
            switch (e.Mtype)
            {
                case MessageEventType.MESSAGE_COMMON:
                    {

                        if (e.MsgContent == null)
                            return;
                        SetGroupText(e.Group, e.Member, e.MsgContent, e.Time);
                        new Task(() =>
                        {
                            if (e.Group.num == 0)
                            {
                                _qq.GetGroupQQNum(e.Group);
                            }
                            if (e.Member.num == 0)
                            {
                                _qq.GetGroupMemberQQNum(e.Group, e.Member);
                            }

                            if (e.Time > _qq.User.LoginTime)
                            {
                                string rmsg = InternalPickMessage(e.MsgContent);
                                if (rmsg != null)
                                {
                                    SendGroupMessage(e.Group, e.Member, rmsg);
                                }
                                else
                                {
                                    var info = new Dictionary<string, object>
                                {
                                    {"gnum",e.Group == null?0:e.Group.num},
                                    {"gname",e.Group == null?null:e.Group.name},
                                    {"fnum",e.Member == null?0:e.Member.num},
                                    {"fnick",e.Member == null?null:e.Member.card},
                                    {"fcard",e.Member == null?null:e.Member.card},
                                };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealGroupMessage(info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendGroupMessage(e.Group, e.Member, rmsg);
                                            break;
                                        }
                                    }
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_USER:
                    {
                        if (e.Member == null)
                        {
                            //is group message
                            RefreshGroup(e.Group);
                        }
                        else
                        {
                            //is member message
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void QQMessageFriendReceived(object sender, FriendEventArgs e)
        {
            switch (e.Mtype)
            {
                case MessageEventType.MESSAGE_COMMON:
                    {
                        SetFriendText(e.User, e.MsgContent, e.Time);
                        if (e.Time > _qq.User.LoginTime)
                        {
                            string rmsg = InternalPickMessage(e.MsgContent);
                            if (rmsg != null)
                            {
                                SendFriendMessage(e.User, rmsg);
                            }
                            else
                            {
                                new Task(() =>
                                {
                                    if (e.User.num == 0)
                                    {
                                        _qq.GetFriendQQNum(e.User);
                                    }
                                    var info = new Dictionary<string, object>
                                    {
                                        {"num",e.User == null?0:e.User.num},
                                        {"nick",e.User == null?null:e.User.nick},
                                        {"mark",e.User == null?null:e.User.markname},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealFriendMessage(info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendFriendMessage(e.User, rmsg);
                                            break;
                                        }
                                    }
                                }).Start();
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
                            SetFriendText(e.User, msg, e.Time);
                            //告知对方发送离线文件
                            msg = string.Format("不能接收文件[{0}],请发离线或邮箱。", e.Msgs["name"]);
                            SendFriendMessage(e.User, msg);
                        }
                        else if (e.Msgs["mode"].ToString() == "refuse")
                        {
                            SetFriendText(e.User, string.Format("对方取消发送文件[{0}]", e.Msgs["session_id"]), e.Time);
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_OFFLINE:
                    {
                        string accurl = _qq.GetOfffileURL(e.Msgs["ip"].ToString(), e.Msgs["port"].ToString(), e.Msgs["name"].ToString(), e.Msgs["rkey"].ToString());
                        //string refurl = _user.RefuleOfffileURL(e.Msgs["from_uin"].ToString(), e.Msgs["name"].ToString(), e.Msgs["size"].ToString());
                        string msg = string.Format("对方发送离线文件:{0}\r\n下载:{1}", e.Msgs["name"].ToString(), accurl);
                        SetFriendText(e.User, msg, e.Time);
                    }
                    break;
                case MessageEventType.MESSAGE_STATUS:
                    {
                        new Task(() =>
                        {
                            if (e.User.num == 0)
                            {
                                _qq.GetFriendQQNum(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {"num",e.User == null?0:e.User.num},
                                {"nick",e.User == null?null:e.User.nick},
                                {"mark",e.User == null?null:e.User.markname},
                            };
                            string messagestate = string.Format("状态更改：{0} => {1} @ {2}", e.User.LongName, e.User.status, e.Time);
                            SetSystemText(messagestate, e.User, e.Time);
                            RefreshUser(e.User);
                            foreach (var p in Plugins)
                            {
                                if (!p.Value.Enabled)
                                {
                                    continue;
                                }
                                string rmsg = p.Value.StatusChanged(info, e.User.status);
                                if (rmsg != null)
                                {
                                    SendFriendMessage(e.User, rmsg);
                                    break;
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_SHAKE:
                    {
                        string msg = "抖动";
                        SetFriendText(e.User, msg, e.Time);
                    }
                    break;
                case MessageEventType.MESSAGE_USER:
                    {
                        RefreshUser(e.User);
                    }
                    break;
                case MessageEventType.MESSAGE_INPUT:
                    {
                        new Task(() =>
                        {
                            if (e.User.num == 0)
                            {
                                _qq.GetFriendQQNum(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {"num",e.User == null?0:e.User.num},
                                {"nick",e.User == null?null:e.User.nick},
                                {"mark",e.User == null?null:e.User.markname},
                            };
                            //string msg = "-----正在输入-----";
                            //SetFriendText(e.User, msg, e.Time);
                            foreach (var p in Plugins)
                            {
                                if (!p.Value.Enabled)
                                {
                                    continue;
                                }
                                string rmsg = p.Value.Input(info);
                                if (rmsg != null)
                                {
                                    SendFriendMessage(e.User, rmsg);
                                    break;
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_KICK:
                    {
                        string msg = string.Format("掉线：@ {0}\r\n{1}", e.Time, e.Msgs["reason"]);
                        SetSystemText(msg, null, e.Time);
                    }
                    break;
                case MessageEventType.MESSAGE_SYSTEM:
                case MessageEventType.MESSAGE_UNKNOW:
                    {
                        if (e.Msgs["poll_type"] as string == "system_message")
                        {
                            if (e.Msgs["type"] as string == "verify_required")
                            {
                                long gid = _qq.User.QQFriends.GroupList.Keys.FirstOrDefault();
                                if (gid < 0) gid = 0;
                                long uin = _qq.AllowFriendAddAndAddFriend(Convert.ToInt64(e.Msgs["account"]), gid, "");
                                if (uin > 0)
                                {
                                    SetSystemText("新用户添加", null, e.Time);
                                    new Task(GetAllFriends).Start();
                                    break;
                                }
                            }
                        }
                        else if (e.Msgs["poll_type"] as string == "sys_g_message")
                        {
                            if (e.Msgs["type"] as string == "group_join")
                            {
                                SetSystemText("新群添加 by " + e.Msgs["admin_nick"] as string, null, e.Time);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                            else if (e.Msgs["type"] as string == "group_leave")
                            {
                                SetSystemText("旧群移除 by " + e.Msgs["admin_nick"] as string, null, e.Time);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                        }
                        SetSystemText(_jss.Serialize(e.Msgs), null, e.Time);
                    }
                    break;
                default:
                    {
                        SetSystemText(_jss.Serialize(e.Msgs), null, e.Time);
                    }
                    break;
            }
        }

        public void SetSystemText(string message, QQFriend friend, DateTime time)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetSystemText(message, friend, time); }));
                return;
            }
            lock (_createlock)
            {
                if (_system == null)
                {
                    _system = new SystemForm();
                    _system.FormClosed += SystemForm_FormClosed;
                }
            }
            if (!_system.Visible)
            {
                _system.Show();
                _system.BringToFront();
                _system.UpdateTitle();
            }
            _system.AppendMessage(message, friend, time);
        }

        public void SetGroupText(QQGroup group, QQGroupMember member, string msg, DateTime time)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetGroupText(group, member, msg, time); }));
                return;
            }
            var f = _groups.Find(g => g.ID == "G|" + group.gid);
            lock (_createlock)
            {
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
            }
            if (msg != null)
            {
                if (!f.HasMessage && f.Visible == false)
                {
                    RefreshGroup(group, true);
                }
                f.AppendMessage(msg, member, time);
            }
            else if (f.Visible == false)
            {
                f.Show();
                RefreshGroup(group, false);
                f.BringToFront();
            }
            else
            {
                f.BringToFront();
            }
            if (群组弹窗ToolStripMenuItem.Checked && f.Visible == false)
            {
                f.Show();
                RefreshGroup(group, false);
                f.BringToFront();
            }
        }

        public void SetFriendText(QQFriend friend, string msg, DateTime time)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => { SetFriendText(friend, msg, time); }));
                return;
            }
            var f = _friends.Find(g => g.ID == "F|" + friend.uin);
            lock (_createlock)
            {
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
            }

            if (msg != null)
            {
                if (!f.HasMessage && f.Visible == false)
                {
                    RefreshUser(friend, true);
                }
                f.AppendMessage(msg, friend, time);
            }
            else if (f.Visible == false)
            {
                f.Show();
                RefreshUser(friend, false);
                f.BringToFront();
            }
            else
            {
                f.BringToFront();
            }
            if (好友弹窗ToolStripMenuItem.Checked && f.Visible == false)
            {
                f.Show();
                RefreshUser(friend, false);
                f.BringToFront();
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

        public bool SendGroupMessage(QQGroup group, QQGroupMember member, string msg)
        {
            if (msg == null) return false;
            var f = _groups.Find(g => g.ID == "G|" + group.gid);
            f.SendMessage(GetGroupMsg(group, member, msg));
            return true;
        }

        public bool SendFriendMessage(QQFriend friend, string msg)
        {
            if (msg == null) return false;
            if (friend.num == 0)
            {
                _qq.GetFriendQQNum(friend);
            }
            var f = _friends.Find(g => g.ID == "F|" + friend.uin);
            f.SendMessage(GetUserMsg(friend, msg));
            return true;
        }

        private string GetUserMsg(QQFriend user, string msg)
        {
            if (msg == null) return null;
            msg = msg.Replace(TranslateMessageUser.UserName, user.Name);
            msg = msg.Replace(TranslateMessageUser.UserNick, user.nick);
            msg = msg.Replace(TranslateMessageUser.UserMarkName, user.markname);
            msg = msg.Replace(TranslateMessageUser.UserNum, user.num.ToString());
            msg = msg.Replace(TranslateMessageUser.UserShortName, user.ShortName);
            msg = msg.Replace(TranslateMessageUser.UserLongName, user.LongName);
            msg = msg.Replace(TranslateMessageGroup.GroupName, "");
            msg = msg.Replace(TranslateMessageGroup.GroupNum, "");
            msg = msg.Replace(TranslateMessageGroup.GroupShortName, "");
            msg = msg.Replace(TranslateMessageGroup.GroupLongName, "");
            msg = msg.Replace(TranslateMessageGroup.GroupMemo, "");
            msg = msg.Replace(TranslateMessageGroup.MemberNum, user.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.MemberNick, user.nick);
            msg = msg.Replace(TranslateMessageGroup.MemberCard, user.markname);
            return msg;
        }

        private string GetGroupMsg(QQGroup group, QQGroupMember member, string msg)
        {
            if (msg == null) return null;
            msg = msg.Replace(TranslateMessageUser.UserName, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserNick, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserMarkName, member.card);
            msg = msg.Replace(TranslateMessageUser.UserNum, member.num.ToString());
            msg = msg.Replace(TranslateMessageUser.UserShortName, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserLongName, member.nick);
            msg = msg.Replace(TranslateMessageGroup.GroupName, group.name);
            msg = msg.Replace(TranslateMessageGroup.GroupNum, group.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.GroupShortName, group.ShortName);
            msg = msg.Replace(TranslateMessageGroup.GroupLongName, group.LongName);
            msg = msg.Replace(TranslateMessageGroup.GroupMemo, group.memo);
            msg = msg.Replace(TranslateMessageGroup.MemberNum, member.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.MemberNick, member.nick);
            msg = msg.Replace(TranslateMessageGroup.MemberCard, member.card);
            return msg;
        }

        public string InternalPickMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            if (message == "--help"
                || message == "--帮助"
                )
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("--help/帮助  ->  \"显示此帮助\"");
                foreach (var plugin in Plugins.Values)
                {
                    if (!plugin.Enabled)
                    {
                        continue;
                    }
                    foreach (KeyValuePair<string, string> filter in plugin.Filters)
                    {
                        sb.AppendFormat("{0}  ->  \"{1}\"{2}", filter.Key, filter.Value, Environment.NewLine);
                    }
                }
                return sb.ToString();
            }
            return null;
        }

        public List<IRichMessage> TransMessageFace(List<IRichMessage> oldmessages, string uin)
        {
            Regex r = new Regex(@"\[face,(?<faceid>\d*)\]", RegexOptions.IgnoreCase);
            List<IRichMessage> newmessages = new List<IRichMessage>();
            foreach (IRichMessage oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != RichMessageType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                MatchCollection mc = r.Matches(strMsg);
                if (mc.Count == 0)
                {
                    newmessages.Add(oldmessage);
                }
                else
                {
                    foreach (Match m in mc)
                    {
                        Bitmap bm = null;
                        try
                        {
                            bm = GetFace(QQ.T_TRANSFER_TABLE[m.Groups["faceid"].Value]);
                        }
                        catch (Exception) { bm = null; }
                        if (bm == null)
                        {
                            try
                            {
                                string url = _qq.GetFaceUrl(m.Groups["faceid"]);
                                Stream s = _qq.GetUrlStream(url);
                                Bitmap nbm = Bitmap.FromStream(s) as Bitmap;
                                bm = nbm;
                            }
                            catch (Exception) { bm = null; }
                        }
                        if (bm == null)
                        {
                            bm = QQChat.Properties.Resources.err;
                        }
                        string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                        if (t[0].Length > 0)
                        {
                            newmessages.Add(new RichMessageText(t[0]));
                        }
                        newmessages.Add(new RichMessageByte(bm));
                        if (t.Length > 1)
                        {
                            strMsg = t[1];
                        }
                        else
                        {
                            strMsg = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg))
                    {
                        newmessages.Add(new RichMessageText(strMsg));
                    }
                }
            }
            return newmessages;
        }


        public List<IRichMessage> TransMessageCFace(List<IRichMessage> oldmessages, string uin)
        {
            Regex r = new Regex(@"\[cface,name:(?<name>[^\]]*)\r\nfile_id:(?<file_id>\d*)\r\nkey:(?<key>[^\]]*)\r\nserver:(?<server>[^\]]*)\]", RegexOptions.IgnoreCase);
            List<IRichMessage> newmessages = new List<IRichMessage>();
            foreach (IRichMessage oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != RichMessageType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                MatchCollection mc = r.Matches(strMsg);
                if (mc.Count == 0)
                {
                    newmessages.Add(oldmessage);
                }
                else
                {
                    foreach (Match m in mc)
                    {
                        Bitmap bm = null;
                        try
                        {
                            string url = _qq.GetCFaceUrl(m.Groups["name"].Value, m.Groups["file_id"].Value);
                            Stream s = _qq.GetUrlStream(url);
                            bm = (Bitmap)Bitmap.FromStream(s);
                        }
                        catch (Exception)
                        {
                            bm = null;
                        }
                        string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                        if (t[0].Length > 0)
                        {
                            newmessages.Add(new RichMessageText(t[0]));
                        }
                        if (bm == null)
                        {
                            newmessages.Add(new RichMessageText(m.Value));
                        }
                        else
                        {
                            newmessages.Add(new RichMessageByte(bm));
                        }
                        if (t.Length > 1)
                        {
                            strMsg = t[1];
                        }
                        else
                        {
                            strMsg = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg))
                    {
                        newmessages.Add(new RichMessageText(strMsg));
                    }
                }
            }
            return newmessages;
        }


        public List<IRichMessage> TransMessageOffpic(List<IRichMessage> oldmessages, string uin)
        {
            Regex r = new Regex(@"\[offpic,success:1\r\nfile_path:(?<picpath>\/[^\]]*)\]", RegexOptions.IgnoreCase);
            List<IRichMessage> newmessages = new List<IRichMessage>();
            foreach (IRichMessage oldmessage in oldmessages)
            {
                if (oldmessage.MessageType != RichMessageType.TYPETEXT)
                {
                    newmessages.Add(oldmessage);
                    continue;
                }
                string strMsg = oldmessage.Message as string;
                MatchCollection mc = r.Matches(strMsg);
                if (mc.Count == 0)
                {
                    newmessages.Add(oldmessage);
                }
                else
                {
                    foreach (Match m in mc)
                    {
                        Bitmap bm = null;
                        try
                        {
                            Stream s = _qq.GetOffPic(m.Groups["picpath"].Value, uin);
                            bm = (Bitmap)Bitmap.FromStream(s);
                        }
                        catch (Exception)
                        {
                            bm = null;
                        }
                        string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                        if (t[0].Length > 0)
                        {
                            newmessages.Add(new RichMessageText(t[0]));
                        }
                        if (bm == null)
                        {
                            newmessages.Add(new RichMessageText(m.Value));
                        }
                        else
                        {
                            newmessages.Add(new RichMessageByte(bm));
                        }
                        if (t.Length > 1)
                        {
                            strMsg = t[1];
                        }
                        else
                        {
                            strMsg = "";
                        }
                    }
                    if (!string.IsNullOrEmpty(strMsg))
                    {
                        newmessages.Add(new RichMessageText(strMsg));
                    }
                }
            }
            return newmessages;
        }

        public List<IRichMessage> TransMessage(string msg, string uin)
        {
            if (msg == null)
                return null;
            List<IRichMessage> messages = new List<IRichMessage>();
            messages.Add(new RichMessageText(msg));
            return TransMessageOffpic(TransMessageCFace(TransMessageFace(messages, uin), uin), uin);
        }


        public ArrayList TransSendFaceMessage(ArrayList oldmessages)
        {
            if (oldmessages == null || oldmessages.Count == 0)
                return oldmessages;
            Regex r = new Regex(@"\[face,(?<faceid>\d*)\]", RegexOptions.IgnoreCase);
            ArrayList newmessages = new ArrayList();
            foreach (var oldmessage in oldmessages)
            {
                if (oldmessage is string)
                {
                    string strMsg = oldmessage as string;
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                            if (t[0].Length > 0)
                            {
                                newmessages.Add(t[0]);
                            }
                            newmessages.Add(new ArrayList { "face", Convert.ToInt64(m.Groups["faceid"].Value) });
                            if (t.Length > 1)
                            {
                                strMsg = t[1];
                            }
                            else
                            {
                                strMsg = "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(strMsg);
                        }
                    }
                }
                else
                {
                    newmessages.Add(oldmessage);
                }
            }
            return newmessages;
        }

        public ArrayList TransSendMessage(string msg)
        {
            if (msg == null)
                return null;
            ArrayList oldmessages = new ArrayList { msg };
            return TransSendFaceMessage(oldmessages);
        }

        public static Bitmap GetFace(int faceid)
        {
            var filePath = string.Format("{0}\\{1}.gif", FacePath, faceid);
            if (File.Exists(filePath))
            {
                return new Bitmap(filePath);
            }
            else
            {
                return QQChat.Properties.Resources.ResourceManager.GetObject("_" + faceid) as Bitmap;
            }
        }

        private void buttonf_Click(object sender, EventArgs e)
        {
            new Task(() =>
                {
                    GetAllFriends();
                }).Start();
        }

        private void buttong_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                GetAllGroups();
            }).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != System.Windows.Forms.DialogResult.Retry)
            {
                if (MessageBox.Show("你确定要退出吗？", "退出确认", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Hide();
                    this.DialogResult = DialogResult.Abort;
                    _qq.LogOutQQ2();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (_system != null)
                _system.Close();
            foreach (var f in _friends.ToArray())
            {
                f.Close();
            }
            foreach (var g in _groups.ToArray())
            {
                g.Close();
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (_qq != null)
                this.Text = string.Format("{0}[{1}]", _qq.User.QQName, _qq.User.QQNum);
        }

        private void 好友弹窗ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            好友弹窗ToolStripMenuItem.Checked = !好友弹窗ToolStripMenuItem.Checked;
        }

        private void 群组弹窗ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            群组弹窗ToolStripMenuItem.Checked = !群组弹窗ToolStripMenuItem.Checked;
        }
    }
}
