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
            this.btnInit = new System.Windows.Forms.Button();
            this.ComboBox_CameraType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnGrab
            // 
            this.btnGrab.Font = new System.Drawing.Font("Noto Sans KR", 20F);
            this.btnGrab.Location = new System.Drawing.Point(227, 51);
            this.btnGrab.Margin = new System.Windows.Forms.Padding(4);
            this.btnGrab.Name = "btnGrab";
            this.btnGrab.Size = new System.Drawing.Size(190, 62);
            this.btnGrab.TabIndex = 0;
            this.btnGrab.Text = "촬상";
            this.btnGrab.UseVisualStyleBackColor = true;
            this.btnGrab.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // btnInit
            // 
            this.btnInit.Font = new System.Drawing.Font("Noto Sans KR", 20F);
            this.btnInit.Location = new System.Drawing.Point(15, 51);
            this.btnInit.Margin = new System.Windows.Forms.Padding(4);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(190, 62);
            this.btnInit.TabIndex = 1;
            this.btnInit.Text = "연결";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // ComboBox_CameraType
            // 
            this.ComboBox_CameraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox_CameraType.FormattingEnabled = true;
            this.ComboBox_CameraType.Items.AddRange(new object[] {
            "None",
            "WebCam",
            "HikRobotCam"});
            this.ComboBox_CameraType.Location = new System.Drawing.Point(15, 18);
            this.ComboBox_CameraType.Name = "ComboBox_CameraType";
            this.ComboBox_CameraType.Size = new System.Drawing.Size(190, 26);
            this.ComboBox_CameraType.TabIndex = 2;
            this.ComboBox_CameraType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_CameraType_SelectedIndexChanged);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(789, 180);
            this.Controls.Add(this.ComboBox_CameraType);
            this.Controls.Add(this.btnInit);
            this.Controls.Add(this.btnGrab);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RunForm";
            this.Text = "RunForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnGrab;
        private System.Windows.Forms.Button btnInit;
        private System.Windows.Forms.ComboBox ComboBox_CameraType;
    }
}