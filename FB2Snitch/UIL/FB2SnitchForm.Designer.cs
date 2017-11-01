namespace FB2Snitch
{
    partial class FB2SnitchForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FB2SnitchForm));
            this.tvMain = new System.Windows.Forms.TreeView();
            this.ssMain = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.slBookCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.slAuthorCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuMain = new System.Windows.Forms.MenuStrip();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tsLangCB = new System.Windows.Forms.ToolStripComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ilMain = new System.Windows.Forms.ImageList(this.components);
            this.panelBookInfo = new System.Windows.Forms.Panel();
            this.pbti_coverpage = new System.Windows.Forms.PictureBox();
            this.verifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ssMain.SuspendLayout();
            this.menuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelBookInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbti_coverpage)).BeginInit();
            this.SuspendLayout();
            // 
            // tvMain
            // 
            this.tvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvMain.Location = new System.Drawing.Point(0, 0);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(303, 719);
            this.tvMain.TabIndex = 0;
            this.tvMain.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tvMain_AfterCollapse);
            this.tvMain.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvMain_BeforeExpand);
            this.tvMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvMain_AfterSelect);
            // 
            // ssMain
            // 
            this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.slBookCount,
            this.toolStripStatusLabel2,
            this.slAuthorCount});
            this.ssMain.Location = new System.Drawing.Point(0, 772);
            this.ssMain.Name = "ssMain";
            this.ssMain.Size = new System.Drawing.Size(1168, 22);
            this.ssMain.TabIndex = 2;
            this.ssMain.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabel1.Text = "Книг:";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // slBookCount
            // 
            this.slBookCount.AutoSize = false;
            this.slBookCount.Name = "slBookCount";
            this.slBookCount.Size = new System.Drawing.Size(60, 17);
            this.slBookCount.Text = "1000000";
            this.slBookCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabel2.Text = "Авторов:";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // slAuthorCount
            // 
            this.slAuthorCount.AutoSize = false;
            this.slAuthorCount.Name = "slAuthorCount";
            this.slAuthorCount.Size = new System.Drawing.Size(60, 17);
            this.slAuthorCount.Text = "1000000";
            this.slAuthorCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // menuMain
            // 
            this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.actionsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.menuitemAbout});
            this.menuMain.Location = new System.Drawing.Point(0, 0);
            this.menuMain.Name = "menuMain";
            this.menuMain.Size = new System.Drawing.Size(1168, 24);
            this.menuMain.TabIndex = 3;
            this.menuMain.Text = "menuStrip1";
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemImport,
            this.verifyToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // menuitemImport
            // 
            this.menuitemImport.Name = "menuitemImport";
            this.menuitemImport.Size = new System.Drawing.Size(152, 22);
            this.menuitemImport.Text = "Import";
            this.menuitemImport.Click += new System.EventHandler(this.menuitemImport_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemProperties});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // menuitemProperties
            // 
            this.menuitemProperties.Name = "menuitemProperties";
            this.menuitemProperties.Size = new System.Drawing.Size(127, 22);
            this.menuitemProperties.Text = "Properties";
            this.menuitemProperties.Click += new System.EventHandler(this.menuitemProperties_Click);
            // 
            // menuitemAbout
            // 
            this.menuitemAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.menuitemAbout.Name = "menuitemAbout";
            this.menuitemAbout.Size = new System.Drawing.Size(44, 20);
            this.menuitemAbout.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvMain);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.AutoScrollMinSize = new System.Drawing.Size(100, 100);
            this.splitContainer1.Panel2.Controls.Add(this.panelBookInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1164, 719);
            this.splitContainer1.SplitterDistance = 303;
            this.splitContainer1.TabIndex = 7;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.tsLangCB});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1168, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.AutoSize = false;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(50, 22);
            this.toolStripLabel1.Text = "Язык:";
            this.toolStripLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tsLangCB
            // 
            this.tsLangCB.AutoSize = false;
            this.tsLangCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tsLangCB.Items.AddRange(new object[] {
            "Ru",
            "En"});
            this.tsLangCB.Name = "tsLangCB";
            this.tsLangCB.Size = new System.Drawing.Size(50, 23);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1168, 723);
            this.panel1.TabIndex = 9;
            // 
            // ilMain
            // 
            this.ilMain.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.ilMain.ImageSize = new System.Drawing.Size(16, 16);
            this.ilMain.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelBookInfo
            // 
            this.panelBookInfo.Controls.Add(this.pbti_coverpage);
            this.panelBookInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBookInfo.Location = new System.Drawing.Point(0, 0);
            this.panelBookInfo.Name = "panelBookInfo";
            this.panelBookInfo.Size = new System.Drawing.Size(857, 719);
            this.panelBookInfo.TabIndex = 5;
            this.panelBookInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.panelBookInfo_Paint);
            // 
            // pbti_coverpage
            // 
            this.pbti_coverpage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbti_coverpage.Location = new System.Drawing.Point(13, 15);
            this.pbti_coverpage.Name = "pbti_coverpage";
            this.pbti_coverpage.Size = new System.Drawing.Size(260, 346);
            this.pbti_coverpage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbti_coverpage.TabIndex = 0;
            this.pbti_coverpage.TabStop = false;
            // 
            // verifyToolStripMenuItem
            // 
            this.verifyToolStripMenuItem.Name = "verifyToolStripMenuItem";
            this.verifyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.verifyToolStripMenuItem.Text = "Verify";
            this.verifyToolStripMenuItem.Click += new System.EventHandler(this.verifyToolStripMenuItem_Click);
            // 
            // FB2SnitchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1168, 794);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.ssMain);
            this.Controls.Add(this.menuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuMain;
            this.Name = "FB2SnitchForm";
            this.Text = "FB2 Snitch";
            this.Load += new System.EventHandler(this.FB2SnitchForm_Load);
            this.ssMain.ResumeLayout(false);
            this.ssMain.PerformLayout();
            this.menuMain.ResumeLayout(false);
            this.menuMain.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelBookInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbti_coverpage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvMain;
        private System.Windows.Forms.StatusStrip ssMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripStatusLabel slBookCount;
        private System.Windows.Forms.MenuStrip menuMain;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel slAuthorCount;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuitemImport;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuitemProperties;
        private System.Windows.Forms.ToolStripMenuItem menuitemAbout;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox tsLangCB;
        private System.Windows.Forms.ImageList ilMain;
        private System.Windows.Forms.PictureBox pbti_coverpage;
        private System.Windows.Forms.Panel panelBookInfo;
        private System.Windows.Forms.ToolStripMenuItem verifyToolStripMenuItem;
    }
}

