using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenTK.Platform.Windows
{
    internal abstract class WinInputBase : IDisposable
    {
        #region Protected Fields

        protected bool Disposed;

        #endregion Protected Fields

        #region Private Fields

        private static readonly IntPtr Unhandled = new IntPtr(-1);

        private readonly AutoResetEvent InputReady = new AutoResetEvent(false);
        private readonly Thread InputThread;
        private readonly WindowProcedure WndProc;
        private INativeWindow native;
        private IntPtr OldWndProc;

        #endregion Private Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public WinInputBase()
        {
            WndProc = WindowProcedure;

            InputThread = new Thread(ProcessEvents);
            InputThread.SetApartmentState(ApartmentState.STA);
            InputThread.IsBackground = true;
            InputThread.Start();

            InputReady.WaitOne();
        }

        #endregion Public Constructors

        #region Private Destructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        ~WinInputBase()
        {
            Console.WriteLine("[Warning] Resource leaked: {0}.", this);
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Properties

        public abstract MappedGamePadDriver GamePadDriver { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract WinMMJoystick JoystickDriver { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract WinRawKeyboard KeyboardDriver { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract WinRawMouse MouseDriver { get; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract WinRawTouch TouchDriver { get; }

        #endregion Public Properties

        #region Protected Properties

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected INativeWindow Native { get { return native; } private set { native = value; } }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected WinWindowInfo Parent { get { return (WinWindowInfo)Native.WindowInfo; } }

        #endregion Protected Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            native?.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #region Protected Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected abstract void CreateDrivers();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual void Dispose(bool manual)
        {
            if (!Disposed)
            {
                if (manual)
                {
                    if (Native != null)
                    {
                        Native.Close();
                        Native.Dispose();
                    }
                }

                Disposed = true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual IntPtr WindowProcedure(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            return Unhandled;
        }

        #endregion Protected Methods

        #region Private Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private INativeWindow ConstructMessageWindow()
        {
            Debug.WriteLine("Initializing input driver.");
            Debug.Indent();

            // Create a new message-only window to retrieve WM_INPUT messages.
            native = new NativeWindow();
            native.ProcessEvents();
            WinWindowInfo parent = native.WindowInfo as WinWindowInfo;
            Functions.SetParent(parent.Handle, Constants.MESSAGE_ONLY);
            native.ProcessEvents();

            Debug.Unindent();
            return native;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void ProcessEvents()
        {
            Native = ConstructMessageWindow();
            CreateDrivers();

            // Subclass the window to retrieve the events we are interested in.
            OldWndProc = Functions.SetWindowLong(Parent.Handle, WndProc);
            Console.WriteLine("Input window attached to {0}", Parent);

            InputReady.Set();

            MSG msg = new MSG();
            while (Native.Exists)
            {
                int ret = Functions.GetMessage(ref msg, Parent.Handle, 0, 0);
                if (ret == -1)
                {
                    throw new PlatformException(string.Format(CultureInfo.InvariantCulture,
                        "An error happened while processing the message queue. Windows error: {0}",
                        Marshal.GetLastWin32Error()));
                }

                Functions.TranslateMessage(ref msg);
                Functions.DispatchMessage(ref msg);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private IntPtr WndProcHandler(
            IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            IntPtr ret = WindowProcedure(handle, message, wParam, lParam);
            if (ret == Unhandled)
                return Functions.CallWindowProc(OldWndProc, handle, message, wParam, lParam);
            else
                return ret;
        }

        #endregion Private Methods
    }
}