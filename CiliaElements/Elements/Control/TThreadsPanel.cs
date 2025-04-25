
using CiliaElements.Utilities;
using Math3D;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace CiliaElements.Elements.Control
{
    public class TThreadsPanel : TControl
    {
        #region Public Fields

        public Mtx5 C4D;
        public string Message;
        public string Title;

        #endregion Public Fields

        #region Private Fields


        #endregion Private Fields

        #region Public Constructors

        public TThreadsPanel(string iPartNumber) : base(iPartNumber)
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
            //Thread th = TManager.ThreadsList[(Overflown + TManager.ThreadsList.Count) % TManager.ThreadsList.Count];
            //if (th.ThreadState == System.Threading.ThreadState.Suspended) th.Resume();
            //Overflown++;
            //th = TManager.ThreadsList[(Overflown + TManager.ThreadsList.Count) % TManager.ThreadsList.Count];
            //if (th.IsAlive)
            //{
            //    Console.WriteLine(th.Name);
            //    th.Suspend();
            //}
        }
        public override void Compute(Graphics grp)
        {
            Process p = Process.GetCurrentProcess();
            int y = 0;


            grp.DrawString(p.Threads.Count.ToString(), Font, Brushes.White, 150, y += 12);

            foreach (ProcessThread th in p.Threads)
            {
                if (th.TotalProcessorTime.TotalSeconds > 1)
                    grp.DrawString(th.Id + " " + th.BasePriority + " " + th.TotalProcessorTime, Font, Brushes.White, 150, y += 12);
            }



            y = 0;
            foreach (Thread th in TManager.ThreadsList)
            {
                grp.DrawString(th.Name, Font, Brushes.White, 0, y += 12);
            }
        }




        public override bool Visible
        {
            get => base.Visible; set
            {
                if (base.Visible == value) return;
                base.Visible = value;
                if (value)
                {
                    RedrawThread = TManager.CreateThread(ToRedrawTimer, "ToRedrawTimer Threads", ThreadPriority.Lowest);
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