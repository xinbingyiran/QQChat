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
using QQChat.User;
using System.Threading.Tasks;
using System.Threading;

namespace QQChat
{
    public partial class MainForm : Form
    {
        private static readonly string PluginPath = Application.StartupPath + "\\plugin";
        private static readonly Dictionary<string, dynamic> Plugins = new Dictionary<string, dynamic>();

        private QQUser _user;

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
                item.Click += (sender, e) => { o.OnMenu(menu.Value); };
                subitems[i] = item;
                i++;
            }
            ToolStripMenuItem newitem = new ToolStripMenuItem(o.IName);
            newitem.DropDownItems.AddRange(subitems);
            菜单ToolStripMenuItem.DropDownItems.Add(newitem);
        }

        public void InitUser(QQUser user)
        {
            if (user == null)
                throw new ArgumentNullException();
            _user = user;
            _user.MessageFriendReceived += _user_MessageFriendReceived;
            _user.MessageGroupReceived += _user_MessageGroupReceived;
            new Task(() => { GetAllFriends(); Thread.Sleep(500); GetAllGroups(); }).Start();
            _user.StartGetMessage();
        }

        private void GetAllFriends()
        {
            _user.RefreshFriendList();
            _user.GetOnlineUsers();
            new Task(() =>
            {
                ShowFriendlist();
                GetAllFriendNum();
            }).Start();
        }

        private void GetAllFriendNum()
        {
            var list = _user.Friends.Friends.Values.ToArray();
            foreach (QQUser_Friend f in list)
            {
                if (_user.Friends.Friends.Values.Contains(f))
                {
                    _user.GetFriendQQNum(f);
                    RefreshUser(f);
                }
                else
                {
                    break;
                }
            }
        }
        private void RefreshUser(QQUser_Friend friend)
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
        }

        private void GetAllGroups()
        {
            _user.RefreshGroupList();
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
            QQUserFriends result = _user.Friends;
            foreach (var f in result.Friends)
            {
                f.Value.tag = 0;
            }
            foreach (var g in result.Groups)
            {
                TreeNode t = new TreeNode();
                t.Text = g.Value.name;
                t.Tag = g.Value.index;
                t.Name = t.Tag.ToString();
                foreach (var f in result.Friends)
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
            foreach (var f in result.Friends)
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
            QQUserGroups result = _user.Groups;
            foreach (var f in result.Groups)
            {
                TreeNode e = new TreeNode();
                e.Text = f.Value.LongName;
                e.Tag = f.Value.gid;
                e.Name = e.Tag.ToString();
                treeViewG.Nodes.Add(e);
            }
        }

        void _user_MessageGroupReceived(object sender, GroupEventArgs e)
        {
            
        }

        void _user_MessageFriendReceived(object sender, FriendEventArgs e)
        {

        }
    }
}
