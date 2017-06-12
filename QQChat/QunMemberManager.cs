using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQChat
{
    public partial class QunMemberManager : Form
    {
        private Dictionary<long, QunFriend> _flist = new Dictionary<long, QunFriend>();
        private Dictionary<long, QunGroup> _glist = new Dictionary<long, QunGroup>();
        private Dictionary<long, Dictionary<long, QunGroupMember>> _gmlist = new Dictionary<long, Dictionary<long, QunGroupMember>>();
        public QunMemberManager()
        {
            InitializeComponent();
            InitEvent();
        }

        private void InitEvent()
        {
            this.buttonrefresh.Click += buttonrefresh_Click;
            this.listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            var item = this.listBox1.SelectedItem as kcv;
            ShowKCV(item);
        }

        private void ShowKCV(kcv item)
        {
            if (item != null)
            {
                foreach (var v in item.Value)
                {
                    if (!_glist.ContainsKey(v))
                    {
                        if (!_flist.ContainsKey(item.Key))
                        {
                            continue;
                        }
                        var f = _flist[item.Key];
                        listBox2.Items.Add(string.Format("{0} - {1}", "QQ好友", f.name));
                        continue;
                    }
                    var g = _glist[v];
                    if (!_gmlist.ContainsKey(v))
                    {
                        continue;
                    }
                    var gmdict = _gmlist[v];
                    if (!gmdict.ContainsKey(item.Key))
                    {
                        continue;
                    }
                    var gm = gmdict[item.Key];
                    listBox2.Items.Add(string.Format("{0}[{1}] - {2}", g.gname, g.gcode, gm.nick));
                }
            }
        }

        private void buttonrefresh_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            var uins = checkedComboBox1.CheckedItems.Cast<groupdefine>().Select(e => e.uin).ToList();
            var sgroup = this._glist.Where(g => uins.Contains(g.Key));
            var nsgroup = this._glist.Where(g => !uins.Contains(g.Key));
            var dict = new Dictionary<long, kcv>();
            if (uins.Contains(-1))
            {
                foreach (var m in _flist)
                {
                    BuidDict(dict, m.Value.name, -1, m.Key);
                }
            }
            foreach (var g in sgroup)
            {
                foreach (var m in g.Value.gmlist)
                {
                    BuidDict(dict, m.nick, g.Key, m.uin);
                }
            }
            if (!uins.Contains(-1))
            {
                foreach (var m in _flist)
                {
                    BuidDict(dict, m.Value.name, -1, m.Key, true);
                }
            }
            foreach (var g in nsgroup)
            {
                foreach (var m in g.Value.gmlist)
                {
                    BuidDict(dict, m.nick, g.Key, m.uin, true);
                }
            }
            var list = dict.Values.OrderByDescending(e => e.Count).ThenByDescending(e => e.Count2).ThenBy(e => e.Key);
            listBox1.Items.AddRange(list.ToArray());
        }

        private static void BuidDict(Dictionary<long, kcv> dict, string name, long guin, long muin, bool ext = false)
        {
            if (dict.ContainsKey(muin))
            {
                dict[muin].Value.Add(guin);
                if (ext)
                {
                    dict[muin].Count2++;
                }
                else
                {
                    dict[muin].Count++;
                }
            }
            else if (!ext)
            {
                dict.Add(muin, new kcv { Key = muin, Name = name, Count = 1, Value = new HashSet<long>(new[] { guin }) });
            }
        }
        internal void InitParas(List<QunFriendGroup> fglist, List<QunGroup> glist)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            checkedComboBox1.Items.Clear();
            var flist = new List<QunFriend>();
            foreach(var g in fglist)
            {
                flist.AddRange(g.friends);
            }
            this._flist = flist.ToDictionary(e => e.uin);
            this._glist.Clear();
            this._gmlist.Clear();
            foreach (var g in glist)
            {
                if (g.gmlist != null)
                {
                    this._glist.Add(g.gcode, g);
                    this._gmlist.Add(g.gcode, g.gmlist.ToDictionary(e => e.uin));
                }
            }
            var groups = _glist.Values.Select(u => new groupdefine { Name = u.gname, uin = u.gcode }).ToList();
            groups.Insert(0, new groupdefine { Name = "我的好友", uin = -1 });
            checkedComboBox1.Items.AddRange(groups.ToArray());
            RefreshView();
        }
    }

    internal class kcv
    {
        public long Key { get; set; }
        public string Name { get; set; }
        public long Count { get; set; }
        public long Count2 { get; set; }
        public HashSet<long> Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[{1}] - {2}/{3}", Name, Key, Count, Count2);
        }
    }

    internal class groupdefine
    {
        public string Name { get; set; }
        public long uin { get; set; }
        public override string ToString()
        {
            return Name + "  -  " + uin;
        }
    }
}
