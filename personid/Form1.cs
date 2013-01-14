using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using personid.Properties;
using System.Web.Script.Serialization;

namespace personid
{
    public partial class Form1 : Form
    {

        private readonly Int32[] _ = new[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DecodeIC(textBox1.Text);
        }

        private void DecodeIC(string text)
        {
            Regex r = new Regex(@"^[0-9]{17}[0-9Xx]$");
            if (!r.IsMatch(text))
            {
                MessageBox.Show("无效号码");
                return;
            }
            var code = text.Substring(0, 6);
            comboBox1.SelectedValue = code;
            var birth = text.Substring(6, 8);
            dateTimePicker1.Value = DateTime.ParseExact(birth, "yyyyMMdd", new DateTimeFormatInfo());
            var check = text.Substring(14, 3);
            var checknum = Convert.ToInt32(check);
            if (checknum % 2 == 0)
            {
                radioButton2.Checked = true;
            }
            else
            {
                radioButton1.Checked = true;
            }
            char find = text[17] == 'x' ? 'X' : text[17];
            char need = GetCheckValue(text);
            label1.Text = find == need ? "校验通过" : "校验失败:" + need;
        }

        private char GetCheckValue(string text)
        {
            int v = 0;
            for (var i = 0; i < 17; i++)
            {
                v += (text[i] - '0') * _[i];
            }
            v = (12 - (v % 11));
            return v == 10 ? 'X' : (char)((v % 10) + '0');
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var json = Resources.station;
            var jss = new JavaScriptSerializer();
            var objects = jss.Deserialize<Dictionary<string, string>>(json);
            comboBox1.DataSource = new BindingSource(objects, null);
            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var code = comboBox1.SelectedValue.ToString();
            if (code.Length != 6)
            {
                comboBox1.SelectedValue = 110000;
                code = "110000";//北京市
            }
            code += dateTimePicker1.Value.ToString("yyyyMMdd");
            var first = radioButton2.Checked ? 0 : 1;
            richTextBox1.Clear();
            for (; first < 1000; first += 2)
            {
                var theid = code + first.ToString("d3");
                var thetrueid = theid + GetCheckValue(theid);
                richTextBox1.AppendText(thetrueid + Environment.NewLine);
            }
        }
    }
}
