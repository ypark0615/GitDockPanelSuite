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
            this.n = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // n
            // 
            this.n.Font = new System.Drawing.Font("Noto Sans KR", 30F);
            this.n.Location = new System.Drawing.Point(9, 12);
            this.n.Name = "n";
            this.n.Size = new System.Drawing.Size(133, 105);
            this.n.TabIndex = 0;
            this.n.Text = "촬상";
            this.n.UseVisualStyleBackColor = true;
            this.n.Click += new System.EventHandler(this.btnGrab_Click);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(154, 120);
            this.Controls.Add(this.n);
            this.Name = "RunForm";
            this.Text = "RunForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button n;
    }
}