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
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.好友弹窗ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.群组弹窗ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.状态ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControlF = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.buttonfget = new System.Windows.Forms.Button();
            this.buttonfd = new System.Windows.Forms.Button();
            this.buttonf = new System.Windows.Forms.Button();
            this.treeViewF = new System.Windows.Forms.TreeView();
            this.tabPageG = new System.Windows.Forms.TabPage();
            this.buttongget = new System.Windows.Forms.Button();
            this.buttongd = new System.Windows.Forms.Button();
            this.buttong = new System.Windows.Forms.Button();
            this.treeViewG = new System.Windows.Forms.TreeView();
            this.全局功能ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.tabControlF.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPageG.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置ToolStripMenuItem,
            this.状态ToolStripMenuItem,
            this.菜单ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(264, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.好友弹窗ToolStripMenuItem,
            this.群组弹窗ToolStripMenuItem,
            this.全局功能ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 好友弹窗ToolStripMenuItem
            // 
            this.好友弹窗ToolStripMenuItem.Name = "好友弹窗ToolStripMenuItem";
            this.好友弹窗ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.好友弹窗ToolStripMenuItem.Text = "好友弹窗";
            this.好友弹窗ToolStripMenuItem.Click += new System.EventHandler(this.好友弹窗ToolStripMenuItem_Click);
            // 
            // 群组弹窗ToolStripMenuItem
            // 
            this.群组弹窗ToolStripMenuItem.Name = "群组弹窗ToolStripMenuItem";
            this.群组弹窗ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.群组弹窗ToolStripMenuItem.Text = "群组弹窗";
            this.群组弹窗ToolStripMenuItem.Click += new System.EventHandler(this.群组弹窗ToolStripMenuItem_Click);
            // 
            // 状态ToolStripMenuItem
            // 
            this.状态ToolStripMenuItem.Name = "状态ToolStripMenuItem";
            this.状态ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.状态ToolStripMenuItem.Text = "状态";
            // 
            // 菜单ToolStripMenuItem
            // 
            this.菜单ToolStripMenuItem.Checked = true;
            this.菜单ToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.菜单ToolStripMenuItem.Name = "菜单ToolStripMenuItem";
            this.菜单ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.菜单ToolStripMenuItem.Text = "插件";
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
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
            this.tabPage1.Controls.Add(this.buttonfget);
            this.tabPage1.Controls.Add(this.buttonfd);
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
            // buttonfget
            // 
            this.buttonfget.Location = new System.Drawing.Point(62, 6);
            this.buttonfget.Name = "buttonfget";
            this.buttonfget.Size = new System.Drawing.Size(50, 23);
            this.buttonfget.TabIndex = 3;
            this.buttonfget.Text = "获取";
            this.buttonfget.UseVisualStyleBackColor = true;
            this.buttonfget.Click += new System.EventHandler(this.buttonfget_Click);
            // 
            // buttonfd
            // 
            this.buttonfd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonfd.Location = new System.Drawing.Point(173, 6);
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
            // treeViewF
            // 
            this.treeViewF.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeViewF.FullRowSelect = true;
            this.treeViewF.Location = new System.Drawing.Point(3, 35);
            this.treeViewF.Name = "treeViewF";
            this.treeViewF.Size = new System.Drawing.Size(226, 284);
            this.treeViewF.TabIndex = 0;
            this.treeViewF.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewF_AfterSelect);
            this.treeViewF.DoubleClick += new System.EventHandler(this.treeViewF_DoubleClick);
            // 
            // tabPageG
            // 
            this.tabPageG.Controls.Add(this.buttongget);
            this.tabPageG.Controls.Add(this.buttongd);
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
            // buttongget
            // 
            this.buttongget.Location = new System.Drawing.Point(63, 6);
            this.buttongget.Name = "buttongget";
            this.buttongget.Size = new System.Drawing.Size(61, 23);
            this.buttongget.TabIndex = 4;
            this.buttongget.Text = "获取";
            this.buttongget.UseVisualStyleBackColor = true;
            this.buttongget.Click += new System.EventHandler(this.buttongget_Click);
            // 
            // buttongd
            // 
            this.buttongd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttongd.Location = new System.Drawing.Point(169, 6);
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
            // treeViewG
            // 
            this.treeViewG.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treeViewG.FullRowSelect = true;
            this.treeViewG.Location = new System.Drawing.Point(3, 35);
            this.treeViewG.Name = "treeViewG";
            this.treeViewG.Size = new System.Drawing.Size(226, 284);
            this.treeViewG.TabIndex = 0;
            this.treeViewG.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewG_AfterSelect);
            this.treeViewG.DoubleClick += new System.EventHandler(this.treeViewG_DoubleClick);
            // 
            // 全局功能ToolStripMenuItem
            // 
            this.全局功能ToolStripMenuItem.Name = "全局功能ToolStripMenuItem";
            this.全局功能ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.全局功能ToolStripMenuItem.Text = "全局功能";
            this.全局功能ToolStripMenuItem.Click += new System.EventHandler(this.全局功能ToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem 状态ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 好友弹窗ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 群组弹窗ToolStripMenuItem;
        private System.Windows.Forms.Button buttonfd;
        private System.Windows.Forms.Button buttongd;
        private System.Windows.Forms.Button buttonfget;
        private System.Windows.Forms.Button buttongget;
        private System.Windows.Forms.ToolStripMenuItem 全局功能ToolStripMenuItem;
    }
}

