using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebQQ2.Extends;
using WebQQ2.WebQQ2;

namespace QQChat
{
    public partial class SignForm : Form
    {
        private QQ_Base _qq;

        public SignForm()
        {
            InitializeComponent();
            this.button1.Click += Button1_Click;
            this.button2.Click += Button2_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.QunSign(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.QunSign(true);
        }

        private void QunSign(bool doSign)
        {
            if(listView1.SelectedItems.Count == 0)
            {
                return;
            }
            var items = new List<ListViewItem>();
            foreach(ListViewItem item in this.listView1.SelectedItems)
            {
                if (item == null)
                {
                    continue;
                }
                var qg = item.Tag as QunGroup;
                if(qg == null)
                {
                    continue;
                }
                items.Add(item);
            }
            Task.Factory.StartNew(()=>QunSignTask(items, doSign));
        }

        private void QunSignTask(List<ListViewItem> items, bool doSign)
        {
            foreach(var item in items)
            {
                var qg = item.Tag as QunGroup;
                var dc = _qq.QunSign(qg.groupid.ToString(),doSign);
                if(dc != null && dc.ContainsKey("ec") && dc["ec"].ToString() == "0")
                {
                    var is_sign = dc["is_sign"].ToString();

                    this.UpdateListViewItem(item,new string[] {
                        item.SubItems[0].Text,
                        item.SubItems[1].Text,
                        dc["today_count"].ToString(),
                        dc["total_count"].ToString(),
                        dc["conti_count"].ToString(),
                        is_sign,
                        is_sign == "0" ? "-" : dc["rank"].ToString(),
                        is_sign == "0" ? "-" : QQHelper.ToTime(long.Parse(dc["sign_time"].ToString())).ToLongTimeString(),
                    });
                }
            }
        }

        private void UpdateListViewItem(ListViewItem item,string[] texts)
        {
            if(InvokeRequired)
            {
                this.BeginInvoke((Action<ListViewItem,String[]>)UpdateListViewItem,new object[] { item, texts });
                return;
            }
            for(int i = 0;i < item.SubItems.Count && i < texts.Length;i ++)
            {
                item.SubItems[i].Text = texts[i];
            }
        }

        internal void InitParas(QQ_Base qq,List<QunGroup> glist)
        {
            this._qq = qq;
            this.listView1.Items.Clear();
            foreach (var g in glist)
            {
                if(g != null)
                {
                    this.listView1.Items.Add(new ListViewItem(new string[] { g.groupname, g.groupid.ToString(),"-", "-", "-", "-", "-", "-" }) { Tag = g });
                }
            }
        }
    }
}
