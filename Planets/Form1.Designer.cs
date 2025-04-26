
namespace Planets
{
    partial class Form1
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
            this.kCiliaControl1 = new CiliaControl.KCiliaControl();
            this.SuspendLayout();
            // 
            // kCiliaControl1
            // 
            this.kCiliaControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kCiliaControl1.Location = new System.Drawing.Point(0, 0);
            this.kCiliaControl1.Name = "kCiliaControl1";
            this.kCiliaControl1.Size = new System.Drawing.Size(800, 450);
            this.kCiliaControl1.TabIndex = 0;
            this.kCiliaControl1.VSync = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.kCiliaControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private CiliaControl.KCiliaControl kCiliaControl1;
    }
}

