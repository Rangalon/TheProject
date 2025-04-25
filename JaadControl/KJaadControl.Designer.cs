using System.Windows.Forms;
namespace JaadWinControls
{
    partial class KJaadControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
      
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //lock (DoerDrawLocker)
                    DoerDrawThread?.Abort();

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
            this.SuspendLayout();
            // 
            // KJaadControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "KJaadControl";
            this.ResumeLayout(false);

        }

        #endregion
    }
}
