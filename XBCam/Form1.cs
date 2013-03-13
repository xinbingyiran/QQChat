using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XBCam.Classes;

namespace XBCam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XBCamHelper helper = new XBCamHelper(panel1.Handle,panel1.Width,panel1.Height);
            helper.StartWebCam();
        }
    }
}
