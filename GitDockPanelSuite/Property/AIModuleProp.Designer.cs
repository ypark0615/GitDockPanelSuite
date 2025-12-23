namespace GitDockPanelSuite.Property
{
    partial class AIModuleProp
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

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.TextBox_ModelPath = new System.Windows.Forms.TextBox();
            this.Btn_ModelFileLoad = new System.Windows.Forms.Button();
            this.ComboBox_AIMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_ModelApply = new System.Windows.Forms.Button();
            this.Btn_ModelInit = new System.Windows.Forms.Button();
            this.Btn_InspAI = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Model Path";
            // 
            // TextBox_ModelPath
            // 
            this.TextBox_ModelPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TextBox_ModelPath.Location = new System.Drawing.Point(121, 60);
            this.TextBox_ModelPath.Name = "TextBox_ModelPath";
            this.TextBox_ModelPath.ReadOnly = true;
            this.TextBox_ModelPath.Size = new System.Drawing.Size(161, 28);
            this.TextBox_ModelPath.TabIndex = 1;
            // 
            // Btn_ModelFileLoad
            // 
            this.Btn_ModelFileLoad.Location = new System.Drawing.Point(291, 61);
            this.Btn_ModelFileLoad.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_ModelFileLoad.Name = "Btn_ModelFileLoad";
            this.Btn_ModelFileLoad.Size = new System.Drawing.Size(75, 28);
            this.Btn_ModelFileLoad.TabIndex = 2;
            this.Btn_ModelFileLoad.Text = "파일";
            this.Btn_ModelFileLoad.UseVisualStyleBackColor = true;
            this.Btn_ModelFileLoad.Click += new System.EventHandler(this.Btn_ModelFileLoad_Click);
            // 
            // ComboBox_AIMode
            // 
            this.ComboBox_AIMode.FormattingEnabled = true;
            this.ComboBox_AIMode.Items.AddRange(new object[] {
            "SEG",
            "DET",
            "IAD"});
            this.ComboBox_AIMode.Location = new System.Drawing.Point(121, 25);
            this.ComboBox_AIMode.Name = "ComboBox_AIMode";
            this.ComboBox_AIMode.Size = new System.Drawing.Size(245, 26);
            this.ComboBox_AIMode.TabIndex = 3;
            this.ComboBox_AIMode.SelectedIndexChanged += new System.EventHandler(this.ComboBox_AIMode_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Mode";
            // 
            // Btn_ModelApply
            // 
            this.Btn_ModelApply.Location = new System.Drawing.Point(246, 108);
            this.Btn_ModelApply.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_ModelApply.Name = "Btn_ModelApply";
            this.Btn_ModelApply.Size = new System.Drawing.Size(120, 40);
            this.Btn_ModelApply.TabIndex = 5;
            this.Btn_ModelApply.Text = "모델 적용";
            this.Btn_ModelApply.UseVisualStyleBackColor = true;
            this.Btn_ModelApply.Click += new System.EventHandler(this.Btn_ModelApply_Click);
            // 
            // Btn_ModelInit
            // 
            this.Btn_ModelInit.Location = new System.Drawing.Point(19, 108);
            this.Btn_ModelInit.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_ModelInit.Name = "Btn_ModelInit";
            this.Btn_ModelInit.Size = new System.Drawing.Size(120, 40);
            this.Btn_ModelInit.TabIndex = 6;
            this.Btn_ModelInit.Text = "모델 초기화";
            this.Btn_ModelInit.UseVisualStyleBackColor = true;
            this.Btn_ModelInit.Click += new System.EventHandler(this.Btn_ModelInit_Click);
            // 
            // Btn_InspAI
            // 
            this.Btn_InspAI.Location = new System.Drawing.Point(19, 164);
            this.Btn_InspAI.Margin = new System.Windows.Forms.Padding(0);
            this.Btn_InspAI.Name = "Btn_InspAI";
            this.Btn_InspAI.Size = new System.Drawing.Size(347, 40);
            this.Btn_InspAI.TabIndex = 7;
            this.Btn_InspAI.Text = "검사";
            this.Btn_InspAI.UseVisualStyleBackColor = true;
            this.Btn_InspAI.Click += new System.EventHandler(this.Btn_InspAI_Click);
            // 
            // AIModuleProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Btn_InspAI);
            this.Controls.Add(this.Btn_ModelInit);
            this.Controls.Add(this.Btn_ModelApply);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ComboBox_AIMode);
            this.Controls.Add(this.Btn_ModelFileLoad);
            this.Controls.Add(this.TextBox_ModelPath);
            this.Controls.Add(this.label1);
            this.Name = "AIModuleProp";
            this.Size = new System.Drawing.Size(389, 280);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TextBox_ModelPath;
        private System.Windows.Forms.Button Btn_ModelFileLoad;
        private System.Windows.Forms.ComboBox ComboBox_AIMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_ModelApply;
        private System.Windows.Forms.Button Btn_ModelInit;
        private System.Windows.Forms.Button Btn_InspAI;
    }
}
