using CiliaElements;
using CiliaElements.Utilities;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CiliaControl
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public partial class KCiliaControl : GLControl.GLControl
    {
        //public static Boolean GLInitialized = false;

        #region Public Fields

        public Thread DoerDrawThread;

        #endregion Public Fields

        #region Private Fields

        private static readonly OpenFileDialog OFD = new OpenFileDialog { Filter = TManager.FilesFilter, Multiselect = true };
        private RECT r = new RECT();

        #endregion Private Fields

        #region Public Constructors

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public KCiliaControl()
        {
            if (Environment.OSVersion.Version.Major >= 6) SetProcessDPIAware();
            InitializeComponent();

            if (DesignMode)
            {
                //BackgroundImage = Properties.Resources.C;
                //BackgroundImageLayout = ImageLayout.Zoom;
                return;
            }

            SetStyle(ControlStyles.Selectable, true);

            TManager.RequestSwitchBorders += TViewManager_RequestSwitchBorders;
            TManager.RequestSwitchWindowState += TViewManager_RequestSwitchWindowState;
            TManager.RequestOpenFiles += TViewManager_RequestOpenFiles;

            Load += KGLControl_Load;

            this.Focus();
        }

        #endregion Public Constructors

        #region Private Delegates

        private delegate void TViewManager_RequestOpenFiles_CB(System.Collections.Generic.List<string> iFiles);

        #endregion Private Delegates

        #region Public Methods

        static TThreadGoverner DrawGovervor = TThreadGoverner.Draw;
        static TThreadGoverner CPUGovernor = TThreadGoverner.CPU;
        static TThreadGoverner GPUGovernor = TThreadGoverner.GPU;

        public void RenderFrame()
        {
          
            DrawGovervor.Reset();
            //TManager.Focused = (ParentForm.Handle == NativeMethods.ForegroundWindow);
            // 
            CPUGovernor.Reset();

            TManager.DrawView();

            CPUGovernor.Check();
            //
            GPUGovernor.Reset();


            SwapBuffers();
            GL.Finish();


            GPUGovernor.Check();


            //
            Refresh();


            DrawGovervor.Check();
        }

        #endregion Public Methods

        #region Private Methods

        [DllImport("User32.dll")]
        private static extern bool RegisterTouchWindow(IntPtr hWnd, int flags);

        [DllImport("User32.dll")]
        private static extern bool UnregisterTouchWindow(IntPtr hWnd);

        private void DoerDraw()
        {
            Refresh_CB refresher = new KCiliaControl.Refresh_CB(RenderFrame);
            Stopwatch sww = new Stopwatch();
            while (true)
            {
                sww.Restart();
                //Thread.Sleep(10);

                this.Invoke(refresher);
                sww.Stop();
            }
        }

        private void KCiliaControl_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void KCiliaControl_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        private void KCiliaControl_Resize(object sender, EventArgs e)
        {
            NativeMethods.WindowRect(this.Handle, ref r);
            TManager.Left = r.Left;
            TManager.Top = r.Top;

            Size sz = new Size((int)TManager.Width, (int)TManager.Height);

            TManager.Height = r.Bottom - r.Top;
            TManager.Width = r.Right - r.Left;
            //

            if (TManager.Height != sz.Height || TManager.Width != sz.Width)
                ViewerSizeChanged?.Invoke(new Size((int)TManager.Width, (int)TManager.Height));
        }

        public delegate void SizeChangedCallBack(Size sz);
        public   event SizeChangedCallBack ViewerSizeChanged;

        private void KGLControl_Load(object sender, System.EventArgs e)
        {
            if (DesignMode) return;

            RegisterTouchWindow(this.Handle, 0);

            this.MouseEnter += KCiliaControl_MouseEnter;
            this.MouseLeave += KCiliaControl_MouseLeave;

            Load -= KGLControl_Load;
            //
            TManager.PushSetup();

            this.Resize += KCiliaControl_Resize;
            this.ParentForm.Resize += KCiliaControl_Resize;
            this.LostFocus += ParentForm_LostFocus;
            this.ParentForm.LostFocus += ParentForm_LostFocus;
            this.GotFocus += ParentForm_GotFocus;
            this.ParentForm.GotFocus += ParentForm_GotFocus;

            KCiliaControl_Resize(this, null);
            //
            this.Focus();
            TManager.Focused = true;

            DoerDrawThread = TManager.CreateThread(DoerDraw, "DoerDraw", ThreadPriority.Lowest     );
        }

        private void ParentForm_GotFocus(object sender, EventArgs e)
        {
            TManager.Focused = true;
        }

        private void ParentForm_LostFocus(object sender, EventArgs e)
        {
            TManager.Focused = false;
        }

        private void TViewManager_RequestOpenFiles(System.Collections.Generic.List<string> iFiles)
        {
            if (this.InvokeRequired)
                this.Invoke(new TViewManager_RequestOpenFiles_CB(TViewManager_RequestOpenFiles), new object[] { iFiles });
            else
            {
                Cursor.Show();
                OFD.ShowDialog();
                foreach (string s in OFD.FileNames) iFiles.Add(s);
                Cursor.Hide();
            }
        }

        private void TViewManager_RequestSwitchBorders()
        {
            if (this.InvokeRequired)
                this.Invoke(new Recall(TViewManager_RequestSwitchBorders), null);
            else
            {
                if (this.ParentForm.FormBorderStyle == FormBorderStyle.Sizable)
                    this.ParentForm.FormBorderStyle = FormBorderStyle.None;
                else
                    this.ParentForm.FormBorderStyle = FormBorderStyle.Sizable;
            }
        }

        private void TViewManager_RequestSwitchWindowState()
        {
            if (this.InvokeRequired)
                this.Invoke(new Recall(TViewManager_RequestSwitchWindowState), null);
            else
            {
                switch (this.ParentForm.WindowState)
                {
                    case FormWindowState.Maximized: this.ParentForm.WindowState = FormWindowState.Normal; break;
                    default: this.ParentForm.WindowState = FormWindowState.Maximized; break;
                }
            }
        }

        #endregion Private Methods


    }

    internal class NativeMethods
    {
        #region Public Properties

        public static IntPtr ForegroundWindow
        { get { return GetForegroundWindow(); } }

        #endregion Public Properties

        #region Public Methods

        public static void WindowRect(IntPtr handle, ref RECT r)
        {
            GetWindowRect(handle, ref r);
        }

        #endregion Public Methods

        #region Private Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        #endregion Private Methods
    }
}