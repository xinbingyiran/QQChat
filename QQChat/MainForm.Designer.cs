namespace QQChat
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlF = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonf = new System.Windows.Forms.Button();
            this.treeViewF = new System.Windows.Forms.TreeView();
            this.tabPageG = new System.Windows.Forms.TabPage();
            this.buttong = new System.Windows.Forms.Button();
            this.treeViewG = new System.Windows.Forms.TreeView();
            this.menuStrip1.SuspendLayout();
            this.tabControlF.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageG.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.菜单ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(264, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 菜单ToolStripMenuItem
            // 
            this.菜单ToolStripMenuItem.Checked = true;
            this.菜单ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.菜单ToolStripMenuItem.Name = "菜单ToolStripMenuItem";
            this.菜单ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.菜单ToolStripMenuItem.Text = "插件";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // tabControlF
            // 
            this.tabControlF.Controls.Add(this.tabPage1);
            this.tabControlF.Controls.Add(this.tabPageG);
            this.tabControlF.Location = new System.Drawing.Point(12, 27);
            this.tabControlF.Name = "tabControlF";
            this.tabControlF.SelectedIndex = 0;
            this.tabControlF.Size = new System.Drawing.Size(240, 348);
            this.tabControlF.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.buttonf);
            this.tabPage1.Controls.Add(this.treeViewF);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(232, 322);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "好友";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // buttonf
            // 
            this.buttonf.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonf.Location = new System.Drawing.Point(3, 3);
            this.buttonf.Name = "buttonf";
            this.buttonf.Size = new System.Drawing.Size(226, 23);
            this.buttonf.TabIndex = 1;
            this.buttonf.Text = "刷新";
            this.buttonf.UseVisualStyleBackColor = true;
            this.buttonf.Click += new System.EventHandler(this.buttonf_Click);
            // 
            // treeViewF
            // 
            this.treeViewF.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeViewF.FullRowSelect = true;
            this.treeViewF.Location = new System.Drawing.Point(3, 32);
            this.treeViewF.Name = "treeViewF";
            this.treeViewF.Size = new System.Drawing.Size(226, 287);
            this.treeViewF.TabIndex = 0;
            this.treeViewF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewF_AfterSelect);
            this.treeViewF.DoubleClick += new System.EventHandler(this.treeViewF_DoubleClick);
            // 
            // tabPageG
            // 
            this.tabPageG.Controls.Add(this.buttong);
            this.tabPageG.Controls.Add(this.treeViewG);
            this.tabPageG.Location = new System.Drawing.Point(4, 22);
            this.tabPageG.Name = "tabPageG";
            this.tabPageG.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageG.Size = new System.Drawing.Size(232, 322);
            this.tabPageG.TabIndex = 1;
            this.tabPageG.Text = "群";
            this.tabPageG.UseVisualStyleBackColor = true;
            // 
            // buttong
            // 
            this.buttong.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttong.Location = new System.Drawing.Point(3, 3);
            this.buttong.Name = "buttong";
            this.buttong.Size = new System.Drawing.Size(226, 23);
            this.buttong.TabIndex = 2;
            this.buttong.Text = "刷新";
            this.buttong.UseVisualStyleBackColor = true;
            this.buttong.Click += new System.EventHandler(this.buttong_Click);
            // 
            // treeViewG
            // 
            this.treeViewG.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeViewG.FullRowSelect = true;
            this.treeViewG.Location = new System.Drawing.Point(3, 32);
            this.treeViewG.Name = "treeViewG";
            this.treeViewG.Size = new System.Drawing.Size(226, 287);
            this.treeViewG.TabIndex = 0;
            this.treeViewG.DoubleClick += new System.EventHandler(this.treeViewG_DoubleClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 387);
            this.Controls.Add(this.tabControlF);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControlF.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPageG.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 菜单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControlF;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView treeViewF;
        private System.Windows.Forms.TabPage tabPageG;
        private System.Windows.Forms.TreeView treeViewG;
        private System.Windows.Forms.Button buttonf;
        private System.Windows.Forms.Button buttong;
    }
}

