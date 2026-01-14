namespace GitDockPanelSuite
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelOpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modelSaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.imageOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.inspectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cycleModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.inspectToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(1143, 35);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modelNewMenuItem,
            this.modelOpenMenuItem,
            this.modelSaveMenuItem,
            this.modelSaveAsMenuItem,
            this.toolStripSeparator1,
            this.imageOpenToolStripMenuItem,
            this.imageSaveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(55, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // modelNewMenuItem
            // 
            this.modelNewMenuItem.Name = "modelNewMenuItem";
            this.modelNewMenuItem.Size = new System.Drawing.Size(237, 34);
            this.modelNewMenuItem.Text = "Model New";
            this.modelNewMenuItem.Click += new System.EventHandler(this.modelNewMenuItem_Click);
            // 
            // modelOpenMenuItem
            // 
            this.modelOpenMenuItem.Name = "modelOpenMenuItem";
            this.modelOpenMenuItem.Size = new System.Drawing.Size(237, 34);
            this.modelOpenMenuItem.Text = "Model Open";
            this.modelOpenMenuItem.Click += new System.EventHandler(this.modelOpenMenuItem_Click);
            // 
            // modelSaveMenuItem
            // 
            this.modelSaveMenuItem.Name = "modelSaveMenuItem";
            this.modelSaveMenuItem.Size = new System.Drawing.Size(237, 34);
            this.modelSaveMenuItem.Text = "Model Save";
            this.modelSaveMenuItem.Click += new System.EventHandler(this.modelSaveMenuItem_Click);
            // 
            // modelSaveAsMenuItem
            // 
            this.modelSaveAsMenuItem.Name = "modelSaveAsMenuItem";
            this.modelSaveAsMenuItem.Size = new System.Drawing.Size(237, 34);
            this.modelSaveAsMenuItem.Text = "Model Save As";
            this.modelSaveAsMenuItem.Click += new System.EventHandler(this.modelSaveAsMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(234, 6);
            // 
            // imageOpenToolStripMenuItem
            // 
            this.imageOpenToolStripMenuItem.Name = "imageOpenToolStripMenuItem";
            this.imageOpenToolStripMenuItem.Size = new System.Drawing.Size(237, 34);
            this.imageOpenToolStripMenuItem.Text = "Image Open";
            this.imageOpenToolStripMenuItem.Click += new System.EventHandler(this.imageOpenToolStripMenuItem_Click);
            // 
            // imageSaveToolStripMenuItem
            // 
            this.imageSaveToolStripMenuItem.Name = "imageSaveToolStripMenuItem";
            this.imageSaveToolStripMenuItem.Size = new System.Drawing.Size(237, 34);
            this.imageSaveToolStripMenuItem.Text = "Image Save";
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setupToolStripMenuItem1});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(75, 29);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // setupToolStripMenuItem1
            // 
            this.setupToolStripMenuItem1.Name = "setupToolStripMenuItem1";
            this.setupToolStripMenuItem1.Size = new System.Drawing.Size(270, 34);
            this.setupToolStripMenuItem1.Text = "Setup";
            this.setupToolStripMenuItem1.Click += new System.EventHandler(this.SetupMenuItem_Click);
            // 
            // inspectToolStripMenuItem
            // 
            this.inspectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cycleModeToolStripMenuItem});
            this.inspectToolStripMenuItem.Name = "inspectToolStripMenuItem";
            this.inspectToolStripMenuItem.Size = new System.Drawing.Size(87, 29);
            this.inspectToolStripMenuItem.Text = "Inspect";
            // 
            // cycleModeToolStripMenuItem
            // 
            this.cycleModeToolStripMenuItem.Name = "cycleModeToolStripMenuItem";
            this.cycleModeToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.cycleModeToolStripMenuItem.Text = "Cycle Mode";
            this.cycleModeToolStripMenuItem.Click += new System.EventHandler(this.cycleModeToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1143, 675);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageSaveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem modelNewMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelOpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modelSaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inspectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cycleModeToolStripMenuItem;
    }
}