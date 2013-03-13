using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> strList = new List<string>(textBox1.Text.Split(','));
            var newstr = textBox2.Text;
            var pos = strList.Count;
            for (int i = 0; i < pos; i++)
            {
                var tstr = strList[i];

                var told = Convert.ToInt32(tstr.Split('-')[0]);
                var tnew = Convert.ToInt32(newstr.Split('-')[0]);
                if (tnew < told)
                {
                    pos = i;
                    break;
                }
            }
            strList.Insert(pos, newstr);
            if (checkBox1.Checked)
            {
                string mergeStr = null;
                for (int i = 0; i < strList.Count - 1; )
                {
                    if (MergeTwo(strList[i], strList[i + 1], out mergeStr))
                    {
                        strList.RemoveAt(i);
                        strList.RemoveAt(i);
                        strList.Insert(i, mergeStr);
                        continue;
                    }
                    i++;
                }
            }
            textBox3.Text = string.Join(",",strList);
        }

        private bool MergeTwo(string first,string second,out string newstr)
        {
            newstr = null;
            var firststart = Convert.ToInt32(first.Split('-')[0]);
            var firstend = Convert.ToInt32(first.Split('-')[1]);
            var secondstart = Convert.ToInt32(second.Split('-')[0]);
            var secondend = Convert.ToInt32(second.Split('-')[1]);
            var newend = secondend;
            if (firstend >= secondstart - 1)
            {
                if (firstend > secondend)
                {
                    newend = firstend;
                }
                newstr = firststart + "-" + newend;
                return true;
            }
            return false;
        }
    }
}
