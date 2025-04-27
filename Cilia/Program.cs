using CiliaElements;
using Math3D;
using System;
using System.Windows.Forms;
using Universe;

namespace Cilia
{
    internal static class Program
    {
        #region Private Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
     
            //if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //FSplash.ShowSplash();
            TUniverse.Setup ();
            Application.Run(new FMain());
            TManager.StopDoers();
        }

        #endregion Private Methods

        //[System.Runtime.InteropServices.DllImport("user32.dll")]
        //private static extern bool SetProcessDPIAware();
    }
}