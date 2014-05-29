namespace QQChat
{
    partial class GlobalForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControlF = new System.Windows.Forms.TabControl();
            this.tabPageF = new System.Windows.Forms.TabPage();
            this.treeViewF = new System.Windows.Forms.TreeView();
            this.buttonfd = new System.Windows.Forms.Button();
            this.buttonf = new System.Windows.Forms.Button();
            this.tabPageG = new System.Windows.Forms.TabPage();
            this.treeViewG = new System.Windows.Forms.TreeView();
            this.buttongd = new System.Windows.Forms.Button();
            this.buttong = new System.Windows.Forms.Button();
            this.tabPageM = new System.Windows.Forms.TabPage();
            this.treeViewm = new System.Windows.Forms.TreeView();
            this.buttonmd = new System.Windows.Forms.Button();
            this.buttonmf = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tabControlF.SuspendLayout();
            this.tabPageF.SuspendLayout();
            this.tabPageG.SuspendLayout();
            this.tabPageM.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlF
            // 
            this.tabControlF.Controls.Add(this.tabPageF);
            this.tabControlF.Controls.Add(this.tabPageG);
            this.tabControlF.Controls.Add(this.tabPageM);
            this.tabControlF.Location = new System.Drawing.Point(12, 12);
            this.tabControlF.Name = "tabControlF";
            this.tabControlF.SelectedIndex = 0;
            this.tabControlF.Size = new System.Drawing.Size(208, 289);
            this.tabControlF.TabIndex = 2;
            // 
            // tabPageF
            // 
            this.tabPageF.Controls.Add(this.treeViewF);
            this.tabPageF.Controls.Add(this.buttonfd);
            this.tabPageF.Controls.Add(this.buttonf);
            this.tabPageF.Location = new System.Drawing.Point(4, 22);
            this.tabPageF.Name = "tabPageF";
            this.tabPageF.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageF.Size = new System.Drawing.Size(200, 263);
            this.tabPageF.TabIndex = 0;
            this.tabPageF.Text = "好友";
            this.tabPageF.UseVisualStyleBackColor = true;
            // 
            // treeViewF
            // 
            this.treeViewF.HideSelection = false;
            this.treeViewF.Location = new System.Drawing.Point(6, 35);
            this.treeViewF.Name = "treeViewF";
            this.treeViewF.Size = new System.Drawing.Size(188, 222);
            this.treeViewF.TabIndex = 3;
            this.treeViewF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewF_AfterSelect);
            // 
            // buttonfd
            // 
            this.buttonfd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonfd.Location = new System.Drawing.Point(141, 6);
            this.buttonfd.Name = "buttonfd";
            this.buttonfd.Size = new System.Drawing.Size(53, 23);
            this.buttonfd.TabIndex = 2;
            this.buttonfd.Text = "导出";
            this.buttonfd.UseVisualStyleBackColor = true;
            this.buttonfd.Click += new System.EventHandler(this.buttonfd_Click);
            // 
            // buttonf
            // 
            this.buttonf.Location = new System.Drawing.Point(6, 6);
            this.buttonf.Name = "buttonf";
            this.buttonf.Size = new System.Drawing.Size(50, 23);
            this.buttonf.TabIndex = 1;
            this.buttonf.Text = "刷新";
            this.buttonf.UseVisualStyleBackColor = true;
            this.buttonf.Click += new System.EventHandler(this.buttonf_Click);
            // 
            // tabPageG
            // 
            this.tabPageG.Controls.Add(this.treeViewG);
            this.tabPageG.Controls.Add(this.buttongd);
            this.tabPageG.Controls.Add(this.buttong);
            this.tabPageG.Location = new System.Drawing.Point(4, 22);
            this.tabPageG.Name = "tabPageG";
            this.tabPageG.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageG.Size = new System.Drawing.Size(200, 263);
            this.tabPageG.TabIndex = 1;
            this.tabPageG.Text = "群";
            this.tabPageG.UseVisualStyleBackColor = true;
            // 
            // treeViewG
            // 
            this.treeViewG.HideSelection = false;
            this.treeViewG.Location = new System.Drawing.Point(6, 35);
            this.treeViewG.Name = "treeViewG";
            this.treeViewG.Size = new System.Drawing.Size(188, 222);
            this.treeViewG.TabIndex = 4;
            this.treeViewG.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewG_AfterSelect);
            // 
            // buttongd
            // 
            this.buttongd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttongd.Location = new System.Drawing.Point(137, 6);
            this.buttongd.Name = "buttongd";
            this.buttongd.Size = new System.Drawing.Size(57, 23);
            this.buttongd.TabIndex = 3;
            this.buttongd.Text = "导出";
            this.buttongd.UseVisualStyleBackColor = true;
            this.buttongd.Click += new System.EventHandler(this.buttongd_Click);
            // 
            // buttong
            // 
            this.buttong.Location = new System.Drawing.Point(6, 6);
            this.buttong.Name = "buttong";
            this.buttong.Size = new System.Drawing.Size(51, 23);
            this.buttong.TabIndex = 2;
            this.buttong.Text = "刷新";
            this.buttong.UseVisualStyleBackColor = true;
            this.buttong.Click += new System.EventHandler(this.buttong_Click);
            // 
            // tabPageM
            // 
            this.tabPageM.Controls.Add(this.treeViewm);
            this.tabPageM.Controls.Add(this.buttonmd);
            this.tabPageM.Controls.Add(this.buttonmf);
            this.tabPageM.Location = new System.Drawing.Point(4, 22);
            this.tabPageM.Name = "tabPageM";
            this.tabPageM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageM.Size = new System.Drawing.Size(200, 263);
            this.tabPageM.TabIndex = 2;
            this.tabPageM.Text = "群友";
            this.tabPageM.UseVisualStyleBackColor = true;
            // 
            // treeViewm
            // 
            this.treeViewm.HideSelection = false;
            this.treeViewm.Location = new System.Drawing.Point(6, 35);
            this.treeViewm.Name = "treeViewm";
            this.treeViewm.Size = new System.Drawing.Size(188, 222);
            this.treeViewm.TabIndex = 7;
            this.treeViewm.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewm_AfterSelect);
            // 
            // buttonmd
            // 
            this.buttonmd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonmd.Location = new System.Drawing.Point(137, 6);
            this.buttonmd.Name = "buttonmd";
            this.buttonmd.Size = new System.Drawing.Size(57, 23);
            this.buttonmd.TabIndex = 6;
            this.buttonmd.Text = "导出";
            this.buttonmd.UseVisualStyleBackColor = true;
            this.buttonmd.Click += new System.EventHandler(this.buttonmd_Click);
            // 
            // buttonmf
            // 
            this.buttonmf.Location = new System.Drawing.Point(6, 6);
            this.buttonmf.Name = "buttonmf";
            this.buttonmf.Size = new System.Drawing.Size(51, 23);
            this.buttonmf.TabIndex = 5;
            this.buttonmf.Text = "刷新";
            this.buttonmf.UseVisualStyleBackColor = true;
            this.buttonmf.Click += new System.EventHandler(this.buttonmf_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(226, 34);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(220, 267);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // GlobalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 313);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.tabControlF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "GlobalForm";
            this.Text = "GlobalForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GlobalForm_FormClosing);
            this.Load += new System.EventHandler(this.GlobalForm_Load);
            this.VisibleChanged += new System.EventHandler(this.GlobalForm_VisibleChanged);
            this.tabControlF.ResumeLayout(false);
            this.tabPageF.ResumeLayout(false);
            this.tabPageG.ResumeLayout(false);
            this.tabPageM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlF;
        private System.Windows.Forms.TabPage tabPageF;
        private System.Windows.Forms.Button buttonfd;
        private System.Windows.Forms.Button buttonf;
        private System.Windows.Forms.TabPage tabPageG;
        private System.Windows.Forms.Button buttongd;
        private System.Windows.Forms.Button buttong;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.TreeView treeViewF;
        private System.Windows.Forms.TreeView treeViewG;
        private System.Windows.Forms.TabPage tabPageM;
        private System.Windows.Forms.TreeView treeViewm;
        private System.Windows.Forms.Button buttonmd;
        private System.Windows.Forms.Button buttonmf;
    }
}