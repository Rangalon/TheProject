

using CiliaElements;
using System;
namespace CiliaControl
{


    //Inherits OpenTK.GLControl
    partial class KCiliaControl : GLControl.GLControl
    {

        //UserControl overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(Boolean disposing)
        {
            DoerDrawThread?.Abort();
            TManager.StopDoers();
            try
            {
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // KCiliaControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "KCiliaControl";
            this.ResumeLayout(false);

        }
    }








}
