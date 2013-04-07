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

        public string Message;

        private void button1_Click(object sender, EventArgs e)
        {
            Message = richTextBox1.Text;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = Message;
        }

        private void setting_Shown(object sender, EventArgs e)
        {
            richTextBox1.Text = Message;
        }
    }
}
