using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebQQ2.WebQQ2;

namespace QQChat
{

    public partial class FaceForm : Form
    {

        public FaceForm()
        {
            InitializeComponent();
        }

        public event EventHandler<FaceEventArgs> OnFaceSelected;

        private void FaceForm_Load(object sender, EventArgs e)
        {
            foreach (var item in QQ.T_TRANSFER_TABLE)
            {
                var bmp = MainForm.GetFace(item.Value);
                PictureBox p = new PictureBox();
                p.Image = bmp;
                p.Size = bmp.Size;
                p.Click += p_Click;
                p.Tag = item.Key;
                flowLayoutPanel1.Controls.Add(p);
            }
        }

        private void p_Click(object sender, EventArgs e)
        {
            if (OnFaceSelected != null)
            {
                OnFaceSelected(this, new FaceEventArgs((sender as PictureBox).Tag as string));
            }
        }

        private void FaceForm_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
    public class FaceEventArgs : EventArgs
    {
        public string FaceString { get; private set; }
        internal FaceEventArgs(int faceid)
        {
            FaceString = string.Format("[face,{0}]", faceid);
        }
        internal FaceEventArgs(string faceidstr)
        {
            FaceString = string.Format("[face,{0}]", faceidstr);
        }
    }
}
