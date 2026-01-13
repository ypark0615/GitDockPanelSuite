namespace GitDockPanelSuite
{
    partial class RunForm
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
            this.btnGrab = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.chkLive = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnGrab
            // 
            this.btnGrab.Location = new System.Drawing.Point(17, 18);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(131, 74);
            this.btnGrab.TabIndex = 0;
            this.btnGrab.Text = "촬상";
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(297, 18);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(131, 74);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "검사";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // chkLive
            // 
            this.chkLive.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkLive.Font = new System.Drawing.Font("굴림", 9F);
            this.chkLive.Location = new System.Drawing.Point(156, 18);
            this.chkLive.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkLive.Name = "chkLive";
            this.chkLive.Size = new System.Drawing.Size(133, 74);
            this.chkLive.TabIndex = 2;
            this.chkLive.Text = "Live";
            this.chkLive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkLive.UseVisualStyleBackColor = true;
            this.chkLive.CheckedChanged += new System.EventHandler(this.chkLive_CheckedChanged);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 108);
            this.Controls.Add(this.chkLive);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnGrab);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "RunForm";
            this.Text = "RunForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox chkLive;
    }
}