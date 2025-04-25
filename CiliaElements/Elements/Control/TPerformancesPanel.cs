
using CiliaElements.Utilities;
using Math3D;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace CiliaElements.Elements.Control
{
    public class TPerformancesPanel : TControl
    {
        #region Public Fields

        public Mtx5 C4D;
        public string Message;
        public string Title;

        #endregion Public Fields

        #region Private Fields

        private int LineGap;

        #endregion Private Fields

        #region Public Constructors

        public TPerformancesPanel(string iPartNumber) : base(iPartNumber)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();
        [DllImport("Kernel32", EntryPoint = "GetThreadId", ExactSpelling = true)]
        public static extern Int32 GetWin32ThreadId(int handle);

        public override void Click()
        {
        }
       
        public   override  void Compute(Graphics grp)
        {
            LineGap = (int)(Font.Size * 2);

            int y = 0;
            int x = 0;
            //

            //
            grp.DrawString(string.Format("Frames/s    : {0:##0}", TThreadGoverner.Draw.PossiblePS), Font, Brushes.LightBlue, x, y);
            y += 12;
            grp.DrawString(string.Format("CPU Frames/s: {0:##0}", TThreadGoverner.CPU.PossiblePS), Font, Brushes.LightBlue, x, y);
            y += 12;
            grp.DrawString(string.Format("GPU Frames/s: {0:##0}", TThreadGoverner.GPU.PossiblePS), Font, Brushes.LightBlue, x, y);
            y += 12;
            grp.DrawString(string.Format("Triangles/s : {0:### ### ### ##0} ", TThreadGoverner.Draw.PossiblePS * TManager.FacetsNumber / 3), Font, Brushes.LightBlue, x, y);
            y += 12;
            // 
            grp.DrawString(string.Format("Pickings/s: {0:##0}       ", TThreadGoverner.Pick.PossiblePS), Font, Brushes.Magenta, x, y);
            y += 12;
            grp.DrawString(string.Format("Compute/s: {0:##0}       ", TThreadGoverner.Compute.PossiblePS), Font, Brushes.Magenta, x, y);
            y += 12;
            //
            grp.DrawString(string.Format("Vertexs: {0:### ### ##0} ", TManager.TotalPositions), Font, Brushes.DarkGreen, x, y);
            y += 12;
            grp.DrawString(string.Format("Facets : {0:### ### ##0} ", TManager.TotalFacets), Font, Brushes.DarkGreen, x, y);
            y += 12;
            // --------------------------------------------------------------------------
            grp.DrawString(string.Format("Displayed Solids: {0:### ### ##0} ", TManager.SolidsNumber), Font, Brushes.Green, x, y);
            y += 12;
            grp.DrawString(string.Format("Displayed Facets: {0:### ### ##0} ", TManager.FacetsNumber / 3), Font, Brushes.Green, x, y);
            y += 12;
            // --------------------------------------------------------------------------
            grp.DrawString(string.Format("Memory in Ko: {0:### ### ##0} ", System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64 / 1024), Font, Brushes.DarkSalmon, x, y);
            y += 12;
            grp.DrawString(string.Format("Threads     : {0:##0} ", Process.GetCurrentProcess().Threads.Count), Font, Brushes.DarkSalmon, x, y);
            y = 0; x += 200;
            // --------------------------------------------------------------------------
            grp.DrawString(string.Format("Files Usage in Ko : {0:### ### ##0} ", TManager.FilesVolume / 1024), Font, Brushes.Yellow, x, y);
            y += 12;
            grp.DrawString(string.Format("Loading time in ms: {0:### ### ##0} ", TManager.BuildingWatch.ElapsedMilliseconds), Font, Brushes.Yellow, x, y);
            y += 12;
            // --------------------------------------------------------------------------
            TFile[][] fss = TManager.FilesByStates;
            for (int j = 0; j < fss.Length; j++)
            {
                grp.DrawString(string.Format("{0,14}: ", TManager.ElementStates[j].ToString()) + fss[j].Length.ToString(CultureInfo.InvariantCulture), Font, Brushes.Yellow, x, y);
                y += 12;
            }
            y += 12;
            // --------------------------------------------------------------------------
            grp.DrawString(string.Format("Push Stack: {0,9}", (TManager.SolidsToBePushed.Max - TManager.SolidsToBePushed.Pos).ToString() + "/" + TManager.SolidsToBePushed.Max.ToString()), Font, Brushes.Yellow, x, y);
            y += 12;
            grp.DrawString(string.Format("Garb Stack: {0,9}", (TManager.SolidsToBeGarbaged.Max - TManager.SolidsToBeGarbaged.Pos).ToString() + "/" + TManager.SolidsToBeGarbaged.Max.ToString()), Font, Brushes.Yellow, x, y);
            y += 12;
            grp.DrawString(string.Format("File Stack: {0,9}", (TManager.FilesToload.Max - TManager.FilesToload.Pos).ToString() + "/" + TManager.FilesToload.Max.ToString()), Font, Brushes.Yellow, x, y);
            y += 12;
            grp.DrawString(string.Format("Loading   : {0,9}", TManager.LoadingCounter), Font, Brushes.Yellow, x, y);
            // --------------------------------------------------------------------------
            y += 12;
            y += 12;
            //grp.DrawString(string.Format("Lon   : {0,9}", C4D.Lon), Font, Brushes.Purple, x, y);
            //y += 12;
            //grp.DrawString(string.Format("Lat   : {0,9}", C4D.Lat), Font, Brushes.Purple, x, y);
            //y += 12;
            //grp.DrawString(string.Format("Hyp   : {0,9}", C4D.Hyp), Font, Brushes.Purple, x, y);
            //
        }

     
      

        public override bool Visible
        {
            get => base.Visible; set
            {
                if (base.Visible == value) return;
                base.Visible = value; 
                if (value)
                {
                    RedrawThread  = TManager. CreateThread(ToRedrawTimer, "ToRedrawTimer Perfos", ThreadPriority.Lowest );
                }
                else
                    RedrawThread?.Abort();
            }
        }

         
        public override void MouseMove(Vec3 p)
        {
        }

        #endregion Public Methods

        #region Internal Methods

        public override void MouseDrag(double dx, double dy)
        {
        }

        #endregion Internal Methods
    }
}