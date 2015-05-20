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
        private Dictionary<long, HashSet<long>> _uing = new Dictionary<long, HashSet<long>>();
        private Dictionary<long, QzoneFriend> _flist = new Dictionary<long, QzoneFriend>();
        private Dictionary<long, QunGroup> _glist = new Dictionary<long, QunGroup>();
        private Dictionary<long, Dictionary<long, QunGroupMember>> _gmlist = new Dictionary<long, Dictionary<long, QunGroupMember>>();
        public QunMemberManager()
        {
            InitializeComponent();
            InitEvent();
        }

        private void InitEvent()
        {
            this.FormClosing += QunMemberManager_FormClosing;
            this.buttonrefresh.Click += buttonrefresh_Click;
            this.listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            this.VisibleChanged += QunMemberManager_VisibleChanged;
        }

        void QunMemberManager_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
            }
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
                    listBox2.Items.Add(string.Format("{0} - {1}", g.groupname, gm.nick));
                }
            }
        }

        private void QunMemberManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void buttonrefresh_Click(object sender, EventArgs e)
        {
            var items = _uing.Select(u => new kcv { Key = u.Key, Count = u.Value.Count, Value = u.Value }).OrderByDescending(u => u.Count).ThenBy(u=>u.Key);
            listBox1.Items.AddRange(items.ToArray());
        }
        internal void InitParas(List<QzoneFriend> flist, List<QunGroup> glist, Dictionary<long, HashSet<long>> gldict)
        {
            this._flist = flist.ToDictionary(e => e.uin);
            this._glist.Clear();
            this._gmlist.Clear();
            foreach (var g in glist)
            {
                if (g.gmlist != null)
                {
                    this._glist.Add(g.groupid, g);
                    this._gmlist.Add(g.groupid, g.gmlist.ToDictionary(e => e.uin));
                }
            }
            this._uing = gldict;
        }
    }

    internal class kcv
    {
        public long Key { get; set; }
        public long Count { get; set; }
        public HashSet<long> Value { get; set; }

        public override string ToString()
        {
            return Key + "  -  " + Count;
        }
    }
}
