using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TempTest
{
    public partial class Form1 : Form
    {

        private SaveLoadManager _saveLoadManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog()
                {
                    CheckPathExists = true,
                    CheckFileExists = true,
                    Multiselect = false
                };
            if (opf.ShowDialog(this) == DialogResult.OK)
            {
                FileInfo fi = new FileInfo(opf.FileName);
                _saveLoadManager = new SaveLoadManager(fi.DirectoryName, fi.Name);
                _saveLoadManager.LoadXML();
                richTextBox1.Text = _saveLoadManager.Data;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_saveLoadManager != null)
            {
                _saveLoadManager.Data = richTextBox1.Text;
                _saveLoadManager.CreateXML();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
