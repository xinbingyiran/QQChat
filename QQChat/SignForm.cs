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
        private CancellationTokenSource _cts;
        private CancellationTokenSource _cts2;
        public SignForm()
        {
            InitializeComponent();
            this.button1.Click += Button1_Click;
            this.button2.Click += Button2_Click;
            this.button3.Click += Button3_Click;
            this.listView1.HideSelection = false;
            this.FormClosed += SignForm_FormClosed;
        }

        private void SignForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this._cts != null && !this._cts.IsCancellationRequested)
            {
                this._cts.Cancel();
            }
            if (this._cts2 != null && !this._cts2.IsCancellationRequested)
            {
                this._cts2.Cancel();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.QunSign(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.QunSign(true);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.QunAdvSign();
        }

        private void QunSign(bool doSign)
        {
            if (this._cts != null && !this._cts.IsCancellationRequested)
            {
                this._cts.Cancel();
                return;
            }
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            var items = new List<ListViewItem>();
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                if (item == null)
                {
                    continue;
                }
                var qg = item.Tag as QunGroup;
                if (qg == null)
                {
                    continue;
                }
                items.Add(item);
            }
            this._cts = new CancellationTokenSource();
            Task.Factory.StartNew(() => QunSignTask(items, doSign));
        }
        private void QunAdvSign()
        {
            if (this._cts2 != null && !this._cts2.IsCancellationRequested)
            {
                this._cts2.Cancel();
                return;
            }
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            var items = new List<ListViewItem>();
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                if (item == null)
                {
                    continue;
                }
                var qg = item.Tag as QunGroup;
                if (qg == null)
                {
                    continue;
                }
                items.Add(item);
            }
            int number = 0;
            if (!int.TryParse(textBox1.Text, out number))
            {
                MessageBox.Show("请输入一个数字");
                return;
            }
            if (number <= 0 || number > 2000)
            {
                MessageBox.Show("请输入有效数字");
                return;
            }
            this._cts2 = new CancellationTokenSource();
            Task.Factory.StartNew(() => QunSignAdvTask(items, number));
        }

        private void QunSignTask(List<ListViewItem> items, bool doSign)
        {
            var cts = this._cts;
            if (cts == null || cts.IsCancellationRequested)
            {
                return;
            }
            foreach (var item in items)
            {
                if (cts == null || cts.IsCancellationRequested)
                {
                    return;
                }
                var qg = item.Tag as QunGroup;
                var dc = this.DoSign(qg, doSign);
                if (dc != null && dc.ContainsKey("ec") && dc["ec"].ToString() == "0")
                {
                    var is_sign = dc["is_sign"].ToString();

                    this.UpdateListViewItem(item, new string[] {
                        item.SubItems[0].Text,
                        item.SubItems[1].Text,
                        dc["today_count"].ToString(),
                        dc["total_count"].ToString(),
                        dc["conti_count"].ToString(),
                        is_sign == "0" ? "-" : dc["rank"].ToString(),
                        is_sign == "0" ? "-" : QQHelper.ToTime(long.Parse(dc["sign_time"].ToString())).ToLongTimeString(),
                        DateTime.Now.ToLongTimeString()
                    });
                }
                else
                {
                    break;
                }
            }
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
        }
        private void QunSignAdvTask(List<ListViewItem> items, int number)
        {
            var cts = this._cts2;
            if (cts == null || cts.IsCancellationRequested)
            {
                return;
            }
            if (items.Count == 0)
            {
                return;
            }
            var item = items[0];
            var lastqd = -1;
            while (true)
            {
                if (cts == null || cts.IsCancellationRequested)
                {
                    return;
                }
                var qg = item.Tag as QunGroup;
                var dc = this.DoSign(qg, false);
                var qd = 0;
                if (dc != null && dc.ContainsKey("ec") && dc["ec"].ToString() == "0")
                {
                    var is_sign = dc["is_sign"].ToString();
                    this.UpdateListViewItem(item, new string[] {
                        item.SubItems[0].Text,
                        item.SubItems[1].Text,
                        dc["today_count"].ToString(),
                        dc["total_count"].ToString(),
                        dc["conti_count"].ToString(),
                        is_sign == "0" ? "-" : dc["rank"].ToString(),
                        is_sign == "0" ? "-" : QQHelper.ToTime(long.Parse(dc["sign_time"].ToString())).ToLongTimeString(),
                        DateTime.Now.ToLongTimeString()
                        });
                    if (is_sign != "0")
                    {
                        break;
                    }
                    if (!int.TryParse(dc["today_count"].ToString(), out qd))
                    {
                        break;
                    }
                    if (qd < number - 1)
                    {
                        if (cts == null || cts.IsCancellationRequested)
                        {
                            return;
                        }
                        var waittime = 10;
                        if (lastqd == qd)
                        {
                            waittime = (number - 1 - qd) * 500;
                            if (waittime > 3000)
                            {
                                waittime = 3000;
                            }
                            if (waittime < 0)
                            {
                                waittime = 10;
                            }
                            cts.Token.WaitHandle.WaitOne(waittime);
                        }
                        else
                        {
                            lastqd = qd;
                        }
                        continue;
                    }
                    else if (qd > number - 1)
                    {
                        break;
                    }
                    else
                    {
                        dc = this.DoSign(qg, true);
                        if (dc != null && dc.ContainsKey("ec") && dc["ec"].ToString() == "0")
                        {
                            is_sign = dc["is_sign"].ToString();
                            this.UpdateListViewItem(item, new string[] {
                            item.SubItems[0].Text,
                            item.SubItems[1].Text,
                            dc["today_count"].ToString(),
                            dc["total_count"].ToString(),
                            dc["conti_count"].ToString(),
                            is_sign == "0" ? "-" : dc["rank"].ToString(),
                            is_sign == "0" ? "-" : QQHelper.ToTime(long.Parse(dc["sign_time"].ToString())).ToLongTimeString(),
                            DateTime.Now.ToLongTimeString()
                            });
                        }
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
            }
        }

        private Dictionary<string,object> DoSign(QunGroup group, bool doSign = false)
        {
            return _qq.QunSign(group.gcode, doSign);
        }

        private void UpdateListViewItem(ListViewItem item, string[] texts)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke((Action<ListViewItem, String[]>)UpdateListViewItem, new object[] { item, texts });
                return;
            }
            if (this.IsDisposed)
            {
                return;
            }
            for (int i = 0; i < item.SubItems.Count && i < texts.Length; i++)
            {
                item.SubItems[i].Text = texts[i];
            }
        }

        internal void InitParas(QQ_Base qq, List<QunGroup> glist)
        {
            this._qq = qq;
            this.listView1.Items.Clear();
            foreach (var g in glist)
            {
                if (g != null)
                {
                    this.listView1.Items.Add(new ListViewItem(new string[] { g.gname, g.gcode.ToString(), "-", "-", "-", "-", "-", "-" }) { Tag = g });
                }
            }
        }
    }
}
