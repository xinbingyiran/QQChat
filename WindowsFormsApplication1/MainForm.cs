using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowForm(new Form1());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowForm(new Form2());
        }

        private void ShowForm(Form f)
        {
            try
            {
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //var a = new { a = 1, b = 2, c = 3, d = 4 };
            //var b = new { a = 1, b = 3, c = 6, d = 5 };
            //var c = new { a = 2, b = 3, c = 7, d = 2 };
            //var d = new { a = 2, b = 4, c = 4, d = 8 };
            //var list = new[] { a, b, c, d };
            //var group =
            //     list.GroupBy(ele => ele.a).Select(ele => new { c = ele.Max(ele2 => ele2.c), d = ele.Max(ele2 => ele2.d) });
            //foreach (var g in group)
            //{
            //    System.Diagnostics.Trace.WriteLine(string.Format("{0} - {1}", g.c, g.d));
            //    //6 - 5
            //    //7 - 8
            //}
        }
    }
}
