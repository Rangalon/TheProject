namespace Cilia
{
    partial class UserControl1
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CiliaControl = new CiliaControl.KCiliaControl();
            this.SuspendLayout();
            // 
            // CiliaControl
            // 
            this.CiliaControl.BackColor = System.Drawing.Color.Black;
            this.CiliaControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CiliaControl.Location = new System.Drawing.Point(0, 0);
            this.CiliaControl.Margin = new System.Windows.Forms.Padding(1);
            this.CiliaControl.Name = "CiliaControl";
            this.CiliaControl.Size = new System.Drawing.Size(150, 150);
            this.CiliaControl.TabIndex = 2;
            this.CiliaControl.VSync = false;
            // 
            // UserControl1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CiliaControl);
            this.Name = "UserControl1";
            this.ResumeLayout(false);

        }

        #endregion

        public CiliaControl.KCiliaControl CiliaControl;
    }
}
