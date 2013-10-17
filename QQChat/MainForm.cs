using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Collections;
using Newtonsoft.Json;

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
        private List<SessForm> _sesss;
        private SystemForm _system;
        private PluginForm _plugin;
        private object _createlock;
        private string _fileName;
        public string[] Paras
        {
            get;
            private set;
        }

        private LoginForm _loginForm;

        public LoginForm LoginForm
        {
            get { return _loginForm; }
            private set { _loginForm = value; }
        }

        private TreeNode _vfzNode = new TreeNode() { Text = @"未分组", Tag = -1, Name = "-1" };
        private TreeNode _msrNode = new TreeNode() { Text = @"陌生人", Tag = 999999, Name = "999999" };

        private Dictionary<string, string> _settings;

        public MainForm()
        {
            InitializeComponent();
            LoginForm = new LoginForm();
            _plugin = new PluginForm();
            mainForm = this;
            _createlock = new object();
            _fileName = Application.StartupPath + "\\QQUser.dat";
            try
            {
                Paras = File.ReadAllLines(_fileName);
                _settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(Paras[2]);
            }
            catch (Exception)
            {
            }
        }


        public void SaveToFile()
        {
            try
            {
                string[] lines = new[] { 
                    LoginForm.UserString,
                    LoginForm.PassString,
                    JsonConvert.SerializeObject(_settings)
                };
                File.WriteAllLines(_fileName, lines);
            }
            catch (Exception) { }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"QQ聊天程序,
可编写自己的自动回复插件。
Designed by XBYR", @"QQ聊天程序");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _groups = new List<GroupForm>();
            _friends = new List<FriendForm>();
            _sesss = new List<SessForm>();
            if (LoginForm.QQ == null)
            {
                if (LoginForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            LoadStatusMenu();
            LoadPlugins();
            InitUser(LoginForm.QQ);
        }

        private void treeViewF_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var pnode = treeViewF.SelectedNode;
                if (pnode != _msrNode)
                {
                    while (pnode != null && pnode != _msrNode)
                    {
                        pnode = pnode.Parent;
                    }
                    if (pnode == null)
                    {
                        var uin = Convert.ToInt64(treeViewF.SelectedNode.Tag);
                        new Task(() => GetFriendInfo(uin)).Start();
                    }
                }
            }
        }

        private void treeViewG_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeViewG.SelectedNode != null)
            {
                var uin = Convert.ToInt64(treeViewG.SelectedNode.Tag);
                new Task(() => GetGroupInfo(uin)).Start();
            }
        }

        private void treeViewF_DoubleClick(object sender, EventArgs e)
        {
            if (treeViewF.SelectedNode != null)
            {
                var pnode = treeViewF.SelectedNode;
                if (pnode != _msrNode)
                {
                    while (pnode != null && pnode != _msrNode)
                    {
                        pnode = pnode.Parent;
                    }
                    var uin = Convert.ToInt64(treeViewF.SelectedNode.Tag);
                    if (pnode == null)
                    {
                        var f = _qq.User.GetUserFriend(uin, false);
                        if (f != null)
                        {
                            SetFriendText(f, null, DateTime.Now, 0);
                        }
                    }
                    else
                    {
                        var f = _qq.User.GetUserSess(uin);
                        if (f != null)
                        {
                            SetSessText(f, null, DateTime.Now, 0);
                        }
                    }
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
                    SetGroupText(g, null, null, DateTime.Now, 0);
                }
            }
        }

        private void LoadStatusMenu()
        {
            ToolStripMenuItem loginitem = new ToolStripMenuItem("重新登录");
            loginitem.Click += (sender, args) => ReturnToLogIn();
            状态ToolStripMenuItem.DropDownItems.Add(loginitem);
            foreach (QQStatus status in QQStatus.AllStatus)
            {
                ToolStripMenuItem newitem = new ToolStripMenuItem(status.Status)
                {
                    Tag = status
                };
                newitem.Click += (sender, e) => new Task(() =>
                    {
                        string result = _qq.ChangeStatus(status.StatusInternal);
                        if (result != null)
                        {
                            BeginInvoke(new MethodInvoker(() => MessageBox.Show(result, @"状态更新出错。")));
                        }
                        else
                        {
                            if (status.StatusInternal != QQStatus.StatusOffline.StatusInternal)
                                _qq.StartGetMessage();
                        }

                        CheckQQStatus();
                    }).Start();
                状态ToolStripMenuItem.DropDownItems.Add(newitem);
            }
        }

        private void ReturnToLogIn()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(ReturnToLogIn));
                return;
            }
            if (LoginForm.Visible)
            {
                return;
            }
            this.Clear();
            this.Hide();
            if (LoginForm.ShowDialog() != DialogResult.OK)
            {
                this.Close();
                return;
            }
            this.Show();
            this.InitUser(LoginForm.QQ);
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
                        if (!t.IsClass || t.IsAbstract || t.GetInterface("MessageDeal.IMessageDeal") == null)
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
            if (o == null)
            {
                return;
            }
            ToolStripMenuItem newitem = new ToolStripMenuItem(o.PluginName);
            var key = t.FullName;
            if (_settings != null && _settings.ContainsKey(key))
            {
                o.Setting = _settings[key];
            }
            ToolStripMenuItem statusitem = new ToolStripMenuItem("激活");
            statusitem.Checked = o.Enabled;
            statusitem.Click += (sender, e) =>
            {
                o.Enabled = !o.Enabled;
                statusitem.Checked = o.Enabled;
            };
            newitem.DropDownItems.Add(statusitem);
            ToolStripMenuItem pluginItem = new ToolStripMenuItem("设置");
            pluginItem.Click += (sender, e) =>
            {
                _plugin.InitPlugin(o);
                _plugin.Show();
                _plugin.BringToFront();
            };
            newitem.DropDownItems.Add(pluginItem);
            菜单ToolStripMenuItem.DropDownItems.Add(newitem);
            o.OnMessage += new EventHandler<EventArgs>(Plugin_OnMessage);
        }



        private void Plugin_OnMessage(object sender, EventArgs e)
        {
            MessageBox.Show((sender as dynamic).LastMessage);
        }

        public void InitUser(QQ qq)
        {
            if (qq == null)
                throw new ArgumentNullException();
            _qq = qq;
            _qq.MessageFriendReceived += QQMessageFriendReceived;
            _qq.MessageGroupReceived += QQMessageGroupReceived;
            _qq.GetMessageError += QQ_GetMessageError;
            CheckQQStatus();
            new Task(() =>
            {
                GetAllFriends();
                Thread.Sleep(500);
                GetAllGroups();
                Thread.Sleep(500);
                _qq.StartGetMessage();
            }).Start();
        }

        private void QQ_GetMessageError(object sender, ErrorEventArgs e)
        {
            if (e.GetException().Message == "网络中断")
            {
                ReturnToLogIn();
            }
            else
            {
                new Task(() =>
                    {
                        try
                        {
                            foreach (ToolStripItem item in 状态ToolStripMenuItem.DropDownItems)
                            {
                                if (item.Tag is QQStatus && (item as ToolStripMenuItem) != null)
                                {
                                    if ((item as ToolStripMenuItem).Checked)
                                    {
                                        _qq.LoginQQ2((item.Tag as QQStatus).StatusInternal);
                                        GetAllFriends();
                                        Thread.Sleep(500);
                                        GetAllGroups();
                                        Thread.Sleep(500);
                                        _qq.StartGetMessage();
                                        break;
                                    }
                                }

                            }
                        }
                        catch (Exception)
                        {
                        }
                        BeginInvoke(new MethodInvoker(ReturnToLogIn));
                    }).Start();
            }
        }

        private void GetAllFriends()
        {
            _qq.RefreshFriendList();
            _qq.GetOnlineUsers();
            new Task(ShowFriendlist).Start();
        }

        private void GetFriendInfo(long uin)
        {
            var f = _qq.User.GetUserFriend(uin, false);
            if (f != null && _qq.User.QQFriends.FriendList.Values.Contains(f))
            {
                CheckFriend(f,true);
            }
        }

        private void GetGroupInfo(long uin)
        {
            var g = _qq.User.GetUserGroup(uin);
            if (g != null && _qq.User.QQGroups.GroupList.Values.Contains(g))
            {
                CheckGroup(g, true);
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
            TreeNode newSess = null;
            if (list.Length == 1)
            {
                newSess = list[0];
            }
            else if (list.Length == 0)
            {
                newSess = new TreeNode();
                newSess.Text = friend.LongNameWithStatus;
                newSess.Tag = friend.uin;
                newSess.Name = newSess.Tag.ToString();
                if (friend.categories < 0)
                {
                    _vfzNode.Nodes.Add(newSess);
                }
                else
                {
                    _msrNode.Nodes.Add(newSess);
                }
            }
            if (newSess != null)
            {
                newSess.Text = friend.LongNameWithStatus;
                if (isMessage.HasValue)
                {
                    newSess.ForeColor = isMessage.Value ? Color.Red : treeViewF.ForeColor;
                    var pnode = newSess.Parent;
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
                        pnode.ForeColor = find ? Color.Red : treeViewF.ForeColor;
                        pnode = pnode.Parent;
                    }

                }
            }
            var f = _friends.Find(ele => ele.ID == "F|" + friend.uin);
            if (f != null)
                f.UpdateTitle();
            var f2 = _sesss.Find(ele => ele.ID == "S|" + friend.uin);
            if (f2 != null)
                f2.UpdateTitle();
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
                if (isMessage.HasValue)
                {
                    list[0].ForeColor = isMessage.Value ? Color.Red : treeViewF.ForeColor;
                }
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
            _vfzNode.Nodes.Clear();
            treeViewF.Nodes.Add(_vfzNode);
            foreach (var f in result.FriendList)
            {
                if (f.Value.tag as int? == 0)
                {
                    TreeNode e = new TreeNode();
                    e.Text = f.Value.LongNameWithStatus;
                    e.Tag = f.Value.uin;
                    e.Name = e.Tag.ToString();
                    _vfzNode.Nodes.Add(e);
                }
            }
            _msrNode.Nodes.Clear();
            treeViewF.Nodes.Add(_msrNode);
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
                        new Task(() =>
                        {
                            if (e.Group.num == 0)
                            {
                                _qq.GetGroupQQNum(e.Group);
                            }
                            if (e.Member.num == 0)
                            {
                                _qq.GetGroupMemberInfos(e.Group, e.Member);
                            }
                            SetGroupText(e.Group, e.Member, e.MsgContent, e.Time, e.Msg_id);
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
                                        {TranslateMessageGroup.GroupNum.Key,e.Group == null?0:e.Group.num},
                                        {TranslateMessageGroup.GroupName.Key,e.Group == null?"":e.Group.name},
                                        {TranslateMessageGroup.MemberNum.Key,e.Member == null?0:e.Member.num},
                                        {TranslateMessageGroup.MemberNick.Key,e.Member == null?"":e.Member.nick},
                                        {TranslateMessageGroup.MemberCard.Key,e.Member == null?"":e.Member.card},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageGroup, info, e.MsgContent);
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
                        //else
                        //{
                        //    //is member message
                        //}
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
                        SetFriendText(e.User, e.MsgContent, e.Time, e.Msg_id);
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
                                        _qq.GetFriendInfos(e.User);
                                    }
                                    var info = new Dictionary<string, object>
                                    {
                                        {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                        {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                        {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageFriend, info, e.MsgContent);
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
                case MessageEventType.MESSAGE_SESS:
                    {
                        SetSessText(e.User, e.MsgContent, e.Time, e.Msg_id);
                        if (e.Time > _qq.User.LoginTime)
                        {
                            string rmsg = InternalPickMessage(e.MsgContent);
                            if (rmsg != null)
                            {
                                SendSessMessage(e.User, rmsg);
                            }
                            else
                            {
                                new Task(() =>
                                {
                                    var info = new Dictionary<string, object>
                                    {
                                        {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                        {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                        {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                                    };
                                    foreach (var p in Plugins)
                                    {
                                        if (!p.Value.Enabled)
                                        {
                                            continue;
                                        }
                                        rmsg = p.Value.DealMessage(MessageType.MessageFriend, info, e.MsgContent);
                                        if (rmsg != null)
                                        {
                                            SendSessMessage(e.User, rmsg);
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
                            SetFriendText(e.User, msg, e.Time, e.Msg_id);
                            //告知对方发送离线文件
                            msg = string.Format("不能接收文件[{0}],请发离线或邮箱。", e.Msgs["name"]);
                            SendFriendMessage(e.User, msg);
                        }
                        else if (e.Msgs["mode"].ToString() == "refuse")
                        {
                            SetFriendText(e.User, string.Format("对方取消发送文件[{0}]", e.Msgs["session_id"]), e.Time, e.Msg_id);
                        }
                    }
                    break;
                case MessageEventType.MESSAGE_OFFLINE:
                    {
                        string accurl = _qq.GetOfffileURL(e.Msgs["ip"].ToString(), e.Msgs["port"].ToString(), e.Msgs["name"].ToString(), e.Msgs["rkey"].ToString());
                        //string refurl = _user.RefuleOfffileURL(e.Msgs["from_uin"].ToString(), e.Msgs["name"].ToString(), e.Msgs["size"].ToString());
                        string msg = string.Format("对方发送离线文件:{0}\r\n下载:{1}", e.Msgs["name"], accurl);
                        SetFriendText(e.User, msg, e.Time, e.Msg_id);
                    }
                    break;
                case MessageEventType.MESSAGE_STATUS:
                    {
                        new Task(() =>
                        {
                            if (e.User.num == 0)
                            {
                                _qq.GetFriendInfos(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                            };
                            if (e.User != null)
                            {
                                var state = QQStatus.GetQQStatusByInternal(e.User.status);
                                string messagestate = string.Format("状态更改：{0} => {1} @ {2}", e.User.LongName, state == null ? e.User.status : state.Status, e.Time);
                                SetSystemText(messagestate, e.User, e.Time, e.Msg_id);
                                RefreshUser(e.User);
                                foreach (var p in Plugins)
                                {
                                    if (!p.Value.Enabled)
                                    {
                                        continue;
                                    }
                                    string rmsg = p.Value.DealMessage(MessageType.MessageStatus, info, QQStatus.GetQQStatusByInternal(e.User.status).Status);
                                    if (rmsg != null)
                                    {
                                        SendFriendMessage(e.User, rmsg);
                                        break;
                                    }
                                }
                            }
                        }).Start();
                    }
                    break;
                case MessageEventType.MESSAGE_SHAKE:
                    {
                        const string msg = "抖动";
                        SetFriendText(e.User, msg, e.Time, e.Msg_id);
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
                                _qq.GetFriendInfos(e.User);
                            }
                            var info = new Dictionary<string, object>
                            {
                                {TranslateMessageUser.UserNum.Key,e.User == null?0:e.User.num},
                                {TranslateMessageUser.UserNick.Key,e.User == null?"":e.User.nick},
                                {TranslateMessageUser.UserMarkName.Key,e.User == null?"":e.User.markname},
                            };
                            //string msg = "-----正在输入-----";
                            //SetFriendText(e.User, msg, e.Time);
                            foreach (var p in Plugins)
                            {
                                if (!p.Value.Enabled)
                                {
                                    continue;
                                }
                                string rmsg = p.Value.DealMessage(MessageType.MessageInput, info, null);
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
                        SetSystemText(msg, null, e.Time, e.Msg_id);
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
                                    SetSystemText("新用户添加", null, e.Time, e.Msg_id);
                                    new Task(GetAllFriends).Start();
                                    break;
                                }
                            }
                        }
                        else if (e.Msgs["poll_type"] as string == "sys_g_message")
                        {
                            if (e.Msgs["type"] as string == "group_join")
                            {
                                SetSystemText("新群添加 by " + e.Msgs["admin_nick"] as string, null, e.Time, e.Msg_id);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                            else if (e.Msgs["type"] as string == "group_leave")
                            {
                                SetSystemText("旧群移除 by " + e.Msgs["admin_nick"] as string, null, e.Time, e.Msg_id);
                                new Task(GetAllGroups).Start();
                                break;
                            }
                        }
                        SetSystemText(JsonConvert.SerializeObject(e.Msgs), null, e.Time, e.Msg_id);
                    }
                    break;
                default:
                    {
                        SetSystemText(JsonConvert.SerializeObject(e.Msgs), null, e.Time, e.Msg_id);
                    }
                    break;
            }
        }

        public void SetSystemText(string message, QQFriend friend, DateTime time, long msg_id)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetSystemText(message, friend, time, msg_id)));
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
            _system.AppendMessage(friend, time, TransMessage(message, friend == null ? "" : friend.uin.ToString(), msg_id).ToArray());
        }

        public GroupForm GetGroupForm(QQGroup group)
        {
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
            return f;
        }

        public void SetGroupText(QQGroup group, QQGroupMember member, string msg, DateTime time, long msg_id)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetGroupText(@group, member, msg, time, msg_id)));
                return;
            }
            var f = GetGroupForm(group);
            if (msg != null)
            {
                if (!f.HasMessage && f.Visible == false)
                {
                    RefreshGroup(group, true);
                }
                f.AppendMessage(member, time, TransMessage(msg, member.uin.ToString(), msg_id).ToArray());
            }
            else if (f.Visible == false)
            {
                f.Show();
                RefreshGroup(group, false);
                f.BringToFront();
            }
            else
            {
                f.WindowState = FormWindowState.Normal;
                f.BringToFront();
            }
            if (群组弹窗ToolStripMenuItem.Checked && f.Visible == false)
            {
                f.Show();
                RefreshGroup(group, false);
                f.BringToFront();
            }
        }

        public void CheckFriend(QQFriend friend, bool awaysCheck = false)
        {
            if (friend == null)
                return;
            if (friend.num == 0 || awaysCheck)
            {
                _qq.GetFriendInfos(friend);
            }
            if (friend.birthday == null || awaysCheck)
            {
                _qq.GetFriendStrangeInfo(friend);
            }
        }

        public void CheckGroup(QQGroup group, bool awaysCheck = false)
        {
            if (group == null)
                return;
            if (group.num == 0 || awaysCheck)
            {
                _qq.GetGroupQQNum(group);
            }
        }

        public void CheckGroupMember(QQGroup group, QQGroupMember member, bool awaysCheck = false)
        {
            if (group == null || member == null)
                return;
            if (member.num == 0 || awaysCheck)
            {
                _qq.GetGroupMemberInfos(group,member);
            }
        }

        public FriendForm GetFriendForm(QQFriend friend)
        {
            var f = _friends.Find(g => g.ID == "F|" + friend.uin);
            lock (_createlock)
            {
                if (f == null)
                {
                    CheckFriend(friend);
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
            return f;
        }

        public void SetFriendText(QQFriend friend, string msg, DateTime time, long msg_id)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetFriendText(friend, msg, time, msg_id)));
                return;
            }
            var f = GetFriendForm(friend);

            if (msg != null)
            {
                if (!f.HasMessage && f.Visible == false)
                {
                    RefreshUser(friend, true);
                }
                f.AppendMessage(friend, time, TransMessage(msg, friend.uin.ToString(), msg_id).ToArray());
            }
            else if (f.Visible == false)
            {
                f.Show();
                RefreshUser(friend, false);
                f.BringToFront();
            }
            else
            {
                f.WindowState = FormWindowState.Normal;
                f.BringToFront();
            }
            if (好友弹窗ToolStripMenuItem.Checked && f.Visible == false)
            {
                f.Show();
                RefreshUser(friend, false);
                f.BringToFront();
            }
        }

        public SessForm GetSessForm(QQFriend friend)
        {
            var f = _sesss.Find(g => g.ID == "S|" + friend.uin);
            lock (_createlock)
            {
                if (f == null)
                {
                    CheckFriend(friend);
                    f = new SessForm()
                    {
                        Friend = friend,
                        QQ = _qq,
                    };
                    f.FormClosed += SessForm_FormClosed;
                    _sesss.Add(f);
                    f.UpdateTitle();
                }
            }
            return f;
        }

        public void SetSessText(QQFriend friend, string msg, DateTime time, long msg_id)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => SetSessText(friend, msg, time, msg_id)));
                return;
            }
            var f = GetSessForm(friend);

            if (msg != null)
            {
                if (!f.HasMessage && f.Visible == false)
                {
                    RefreshUser(friend, true);
                }
                f.AppendMessage(friend, time, TransMessage(msg, friend.uin.ToString(), msg_id).ToArray());
            }
            else if (f.Visible == false)
            {
                f.Show();
                RefreshUser(friend, false);
                f.BringToFront();
            }
            else
            {
                f.WindowState = FormWindowState.Normal;
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

        private void SessForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            var f = sender as SessForm;
            if (f != null)
                _sesss.Remove(f);
        }

        public bool SendGroupMessage(QQGroup group, QQGroupMember member, string msg)
        {
            if (msg == null) return false;
            var f = GetGroupForm(group);
            f.SendMessage(GetGroupMsg(group, member, msg));
            return true;
        }

        public bool SendFriendMessage(QQFriend friend, string msg)
        {
            if (msg == null) return false;
            var f = GetFriendForm(friend);
            f.SendMessage(GetUserMsg(friend, msg));
            return true;
        }

        public bool SendSessMessage(QQFriend friend, string msg)
        {
            if (msg == null) return false;
            var f = GetSessForm(friend);
            f.SendMessage(GetUserMsg(friend, msg));
            return true;
        }

        private string GetUserMsg(QQFriend user, string msg)
        {
            if (msg == null) return null;
            msg = msg.Replace(TranslateMessageUser.UserName.Key, user.Name);
            msg = msg.Replace(TranslateMessageUser.UserNick.Key, user.nick);
            msg = msg.Replace(TranslateMessageUser.UserMarkName.Key, user.markname);
            msg = msg.Replace(TranslateMessageUser.UserNum.Key, user.num.ToString());
            msg = msg.Replace(TranslateMessageUser.UserShortName.Key, user.ShortName);
            msg = msg.Replace(TranslateMessageUser.UserLongName.Key, user.LongName);
            msg = msg.Replace(TranslateMessageGroup.GroupName.Key, "");
            msg = msg.Replace(TranslateMessageGroup.GroupNum.Key, "");
            msg = msg.Replace(TranslateMessageGroup.GroupShortName.Key, "");
            msg = msg.Replace(TranslateMessageGroup.GroupLongName.Key, "");
            msg = msg.Replace(TranslateMessageGroup.GroupMemo.Key, "");
            msg = msg.Replace(TranslateMessageGroup.MemberNum.Key, user.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.MemberNick.Key, user.nick);
            msg = msg.Replace(TranslateMessageGroup.MemberCard.Key, user.markname);
            return msg;
        }

        private string GetGroupMsg(QQGroup group, QQGroupMember member, string msg)
        {
            if (msg == null) return null;
            msg = msg.Replace(TranslateMessageUser.UserName.Key, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserNick.Key, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserMarkName.Key, member.card);
            msg = msg.Replace(TranslateMessageUser.UserNum.Key, member.num.ToString());
            msg = msg.Replace(TranslateMessageUser.UserShortName.Key, member.nick);
            msg = msg.Replace(TranslateMessageUser.UserLongName.Key, member.nick);
            msg = msg.Replace(TranslateMessageGroup.GroupName.Key, group.name);
            msg = msg.Replace(TranslateMessageGroup.GroupNum.Key, group.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.GroupShortName.Key, group.ShortName);
            msg = msg.Replace(TranslateMessageGroup.GroupLongName.Key, group.LongName);
            msg = msg.Replace(TranslateMessageGroup.GroupMemo.Key, group.memo);
            msg = msg.Replace(TranslateMessageGroup.MemberNum.Key, member.num.ToString());
            msg = msg.Replace(TranslateMessageGroup.MemberNick.Key, member.nick);
            msg = msg.Replace(TranslateMessageGroup.MemberCard.Key, member.card);
            return msg;
        }

        public string InternalPickMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return null;
            message = message.Trim();
            switch (message)
            {
                case "--help":
                case "--帮助":
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
            }
            return null;
        }

        public List<IRichMessage> TransMessageFace(List<IRichMessage> oldmessages)
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
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                bm = GetFace(QQ.T_TRANSFER_TABLE[m.Groups["faceid"].Value]);
                            }
                            catch (Exception) { bm = null; }
                            if (bm == null)
                            {
                                try
                                {
                                    string url = QQ.GetFaceUrl(m.Groups["faceid"]);
                                    Stream s = _qq.GetUrlStream(url);
                                    Bitmap nbm = Image.FromStream(s) as Bitmap;
                                    bm = nbm;
                                }
                                catch (Exception) { bm = null; }
                            }
                            if (bm == null)
                            {
                                bm = QQChat.Properties.Resources.err;
                            }
                            if (strMsg != null)
                            {
                                string[] t = strMsg.Split(new string[] { m.Value }, 2, StringSplitOptions.None);
                                if (t[0].Length > 0)
                                {
                                    newmessages.Add(new RichMessageText(t[0]));
                                }
                                newmessages.Add(new RichMessageByte(bm));
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new RichMessageText(strMsg));
                        }
                    }
                }
            }
            return newmessages;
        }


        public List<IRichMessage> TransMessageCFace(List<IRichMessage> oldmessages, string uin, long msg_id)
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
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                string url = m.Groups["name"].Value;
                                if (url.StartsWith("{"))
                                {
                                    url = _qq.GetCFaceUrl(url, m.Groups["file_id"].Value);
                                }
                                else
                                {
                                    url = _qq.GetCFace2Url(msg_id.ToString(), url, uin);
                                }
                                Stream s = _qq.GetUrlStream(url);
                                bm = (Bitmap)Image.FromStream(s);
                            }
                            catch (Exception)
                            {
                                bm = null;
                            }
                            if (strMsg != null)
                            {
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
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new RichMessageText(strMsg));
                        }
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
                if (strMsg != null)
                {
                    MatchCollection mc = r.Matches(strMsg);
                    if (mc.Count == 0)
                    {
                        newmessages.Add(oldmessage);
                    }
                    else
                    {
                        foreach (Match m in mc)
                        {
                            Bitmap bm;
                            try
                            {
                                Stream s = _qq.GetOffPic(m.Groups["picpath"].Value, uin);
                                bm = (Bitmap)Image.FromStream(s);
                            }
                            catch (Exception)
                            {
                                bm = null;
                            }
                            if (strMsg != null)
                            {
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
                                strMsg = t.Length > 1 ? t[1] : "";
                            }
                        }
                        if (!string.IsNullOrEmpty(strMsg))
                        {
                            newmessages.Add(new RichMessageText(strMsg));
                        }
                    }
                }
            }
            return newmessages;
        }

        public List<IRichMessage> TransMessage(string msg, string uin, long msg_id)
        {
            if (msg == null)
                return null;
            List<IRichMessage> messages = new List<IRichMessage>();
            messages.Add(new RichMessageText(msg));
            return TransMessageOffpic(TransMessageCFace(TransMessageFace(messages), uin, msg_id), uin);
        }


        public static ArrayList TransSendFaceMessage(ArrayList oldmessages)
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
                            strMsg = t.Length > 1 ? t[1] : "";
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

        public static ArrayList TransSendMessage(string msg)
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
            return Properties.Resources.ResourceManager.GetObject("_" + faceid) as Bitmap;
        }

        private void buttonf_Click(object sender, EventArgs e)
        {
            new Task(GetAllFriends).Start();
        }

        private void buttong_Click(object sender, EventArgs e)
        {
            new Task(GetAllGroups).Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != System.Windows.Forms.DialogResult.Retry)
            {
                if (MessageBox.Show(@"你确定要退出吗？", @"退出确认", MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.Hide();
                    this.Clear();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void Clear()
        {
            _qq.MessageFriendReceived -= QQMessageFriendReceived;
            _qq.MessageGroupReceived -= QQMessageGroupReceived;
            _qq.GetMessageError -= QQ_GetMessageError;
            _qq.LogOutQQ2();
            if (_system != null)
            {
                _system.Close();
                _system = null;
            }
            foreach (var s in _sesss.ToArray())
            {
                s.Close();
            }
            _sesss.Clear();
            foreach (var f in _friends.ToArray())
            {
                f.Close();
            }
            _friends.Clear();
            foreach (var g in _groups.ToArray())
            {
                g.Close();
            }
            _groups.Clear();
            this.GetSettings();
            this.SaveToFile();
            foreach (var p in Plugins)
            {
                p.Value.OnExited();
            }
            treeViewF.Nodes.Clear();
            treeViewG.Nodes.Clear();
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

        internal Dictionary<string, string> GetSettings()
        {
            if (_settings == null)
                _settings = new Dictionary<string, string>();
            foreach (var p in Plugins)
            {
                if (_settings.ContainsKey(p.Key))
                {
                    _settings[p.Key] = p.Value.Setting;
                }
                else
                {
                    _settings.Add(p.Key, p.Value.Setting);
                }
            }
            return _settings;
        }

        private void buttonfd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            QQFriends result = _qq.User.QQFriends;
            foreach (var f in result.FriendList)
            {
                f.Value.tag = 0;
            }
            foreach (var g in result.GroupList)
            {
                lines.Add(g.Value.name + ":");
                foreach (var f in result.FriendList)
                {
                    if (f.Value.categories == g.Value.index)
                    {
                        f.Value.tag = 1;
                        lines.Add("\t" + f.Value.LongName);
                    }
                }
            }
            lines.Add(_vfzNode.Text + ":");
            foreach (var f in result.FriendList)
            {
                if (f.Value.tag as int? == 0)
                {
                    lines.Add("\t" + f.Value.LongName);
                }
            }
            File.WriteAllLines(filename, lines);
        }

        private void buttongd_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            string filename = sfd.FileName;
            List<string> lines = new List<string>();
            QQGroups result = _qq.User.QQGroups;
            foreach (var g in result.GroupList)
            {
                lines.Add(g.Value.LongName);
            }
            File.WriteAllLines(filename, lines);
        }

        private CancellationTokenSource _getFriendCTS;
        private CancellationTokenSource _getGroupCTS;

        private void buttonfget_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                if (_getFriendCTS != null)
                {
                    _getFriendCTS.Cancel();
                    _getFriendCTS = null;
                }
                if (_getFriendCTS == null)
                {
                    _getFriendCTS = new CancellationTokenSource();
                }
                foreach (var friend in _qq.User.QQFriends.FriendList)
                {
                    _getFriendCTS.Token.ThrowIfCancellationRequested();
                    CheckFriend(friend.Value);
                }
            }).Start();
        }

        private void buttongget_Click(object sender, EventArgs e)
        {
            new Task(() =>
            {
                if (_getGroupCTS != null)
                {
                    _getGroupCTS.Cancel();
                    _getGroupCTS = null;
                }
                if (_getGroupCTS == null)
                {
                    _getGroupCTS = new CancellationTokenSource();
                }
                foreach (var group in _qq.User.QQGroups.GroupList)
                {
                    _getGroupCTS.Token.ThrowIfCancellationRequested();
                    CheckGroup(group.Value);
                }
            }).Start();
        }
    }
}
