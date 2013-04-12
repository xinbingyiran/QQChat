using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MeIn
{
    public partial class setting : Form
    {
        public setting()
        {
            InitializeComponent();
        }

        internal iniItem SaveItem;

        private void button1_Click(object sender, EventArgs e)
        {
            Int32 min, max, span;
            if (!Int32.TryParse(textBoxMin.Text, out min))
            {
                MessageBox.Show("请输入有效最小值");
                textBoxMin.Focus();
                return;
            }
            if (!Int32.TryParse(textBoxMax.Text, out max))
            {
                MessageBox.Show("请输入有效最大值");
                textBoxMax.Focus();
                return;
            }
            if (max < 0 || min > max)
            {
                MessageBox.Show("请输入有效数值范围");
                textBoxMin.Focus();
                return;
            }
            if (!Int32.TryParse(textBoxSpan.Text, out span))
            {
                MessageBox.Show("请输入有效时间间隔");
                textBoxSpan.Focus();
                return;
            }
            if (span < 0)
            {
                MessageBox.Show("请输入有效时间间隔");
                textBoxSpan.Focus();
                return;
            }
            SaveItem.min = min;
            SaveItem.mintomax = max - min;
            SaveItem.timespan = TimeSpan.TicksPerMinute * span;
            SaveItem.item = textBoxName.Text;
            SaveItem.pdata = richTextBox1.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Reset()
        {
            textBoxMin.Text = SaveItem.min.ToString();
            textBoxMax.Text = (SaveItem.min + SaveItem.mintomax).ToString();
            textBoxSpan.Text = ((Int32)new TimeSpan(SaveItem.timespan).TotalMinutes).ToString();
            textBoxName.Text = SaveItem.item;
            richTextBox1.Text = SaveItem.pdata;
            this.DialogResult = DialogResult.None;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void setting_Shown(object sender, EventArgs e)
        {
            Reset();
        }

        private void setting_Load(object sender, EventArgs e)
        {

        }
    }
}
