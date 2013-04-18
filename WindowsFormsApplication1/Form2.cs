using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {

        private UInt32 _num;
        private Int32 _index;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                _num = Convert.ToUInt32(textBox1.Text);
                SplitIt(_num);
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }

        private void SplitIt(uint num)
        {
            richTextBox1.Clear();
            _index = 0;
            var list = new List<uint>();
            SplitNum(num, 1, list);
        }
        private void SplitNum(uint num, uint min, List<uint> list)
        {
            if (num == 0)
            {
                showList(list);
                return;
            }
            while (min <= num)
            {
                list.Add(min);
                SplitNum(num - min, min, list);
                list.RemoveAt(list.Count - 1);
                min++;
            }
        }

        private void showList(List<uint> list)
        {
            if (list.Count > 1)
            {
                _index++;
                richTextBox1.AppendText(string.Format("和式{0:d4} : {1} = {2}\r\n", _index, _num, string.Join(" + ", list)));
            }
        }
    }
}
