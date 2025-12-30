namespace GitDockPanelSuite.Property
{
    partial class BinaryProp
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
                if(binRangeTrackbar != null)
                {
                    binRangeTrackbar.RangeChanged -= Range_RangeChanged;
                }

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
            this.grpBinary = new System.Windows.Forms.GroupBox();
            this.cbHighlight = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.binRangeTrackbar = new GitDockPanelSuite.UIControl.RangeTrackbar();
            this.chkUse = new System.Windows.Forms.CheckBox();
            this.grpBinary.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBinary
            // 
            this.grpBinary.Controls.Add(this.cbHighlight);
            this.grpBinary.Controls.Add(this.label1);
            this.grpBinary.Controls.Add(this.binRangeTrackbar);
            this.grpBinary.Location = new System.Drawing.Point(13, 50);
            this.grpBinary.Name = "grpBinary";
            this.grpBinary.Size = new System.Drawing.Size(355, 132);
            this.grpBinary.TabIndex = 4;
            this.grpBinary.TabStop = false;
            this.grpBinary.Text = "이진화";
            // 
            // cbHighlight
            // 
            this.cbHighlight.FormattingEnabled = true;
            this.cbHighlight.Location = new System.Drawing.Point(117, 93);
            this.cbHighlight.Name = "cbHighlight";
            this.cbHighlight.Size = new System.Drawing.Size(232, 26);
            this.cbHighlight.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 5;
            this.label1.Text = "하이라이트";
            // 
            // binRangeTrackbar
            // 
            this.binRangeTrackbar.BackColor = System.Drawing.Color.Transparent;
            this.binRangeTrackbar.Location = new System.Drawing.Point(6, 13);
            this.binRangeTrackbar.Name = "binRangeTrackbar";
            this.binRangeTrackbar.Size = new System.Drawing.Size(343, 92);
            this.binRangeTrackbar.TabIndex = 4;
            this.binRangeTrackbar.ValueLeft = 80;
            this.binRangeTrackbar.ValueRight = 200;
            // 
            // chkUse
            // 
            this.chkUse.AutoSize = true;
            this.chkUse.Location = new System.Drawing.Point(19, 16);
            this.chkUse.Name = "chkUse";
            this.chkUse.Size = new System.Drawing.Size(70, 22);
            this.chkUse.TabIndex = 7;
            this.chkUse.Text = "검사";
            this.chkUse.UseVisualStyleBackColor = true;
            // 
            // BinaryProp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkUse);
            this.Controls.Add(this.grpBinary);
            this.Name = "BinaryProp";
            this.Size = new System.Drawing.Size(388, 304);
            this.grpBinary.ResumeLayout(false);
            this.grpBinary.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBinary;
        private System.Windows.Forms.ComboBox cbHighlight;
        private System.Windows.Forms.Label label1;
        private UIControl.RangeTrackbar binRangeTrackbar;
        private System.Windows.Forms.CheckBox chkUse;
    }
}
