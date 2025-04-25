using System;
using System.Windows.Forms;
namespace Cilia
{
    partial class FMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(Boolean disposing)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            //            this.kTreeView1 = new CiliaWinControls.KTreeView();
            this.CiliaControl = new CiliaControl.KCiliaControl();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            // CiliaControl
            // 
            this.CiliaControl.BackColor = System.Drawing.Color.Black;
            this.CiliaControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CiliaControl.Location = new System.Drawing.Point(353, 51);
            this.CiliaControl.Margin = new System.Windows.Forms.Padding(1);
            this.CiliaControl.Name = "CiliaControl";
            this.CiliaControl.Size = new System.Drawing.Size(437, 465);
            this.CiliaControl.TabIndex = 1;
            this.CiliaControl.VSync = false;


            // 
            // kViewBar1
            // 
            // 
            // kDebugBar1
            // 

            // 
            // kViewerBar1
            // 
            // 
            // kEntitiesBar1
            // 

            // 
            // propertyGrid1
            // 
            // 
            // FMain
            // 
            //Clock.Tick+=Clock_Tick;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font  ;
            this.BackColor = System.Drawing.Color.Black;

            this.ClientSize = new System.Drawing.Size(1200, 600);
            this.Controls.Add(this.CiliaControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        //private void Clock_Tick(object sender, EventArgs e)
        //{
        //    this.Text = OpenTK.Platform.Windows.WinRawMouse.MouseTag; 
        //}

        #endregion

        //private Timer Clock = new Timer() { Interval = 100, Enabled = true };
        private CiliaControl.KCiliaControl CiliaControl;
    }
}

