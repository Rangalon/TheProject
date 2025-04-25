//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GLControl
{
    /// <summary>
    /// OpenGL-aware WinForms control.
    /// The WinForms designer will always call the default constructor.
    /// Inherit from this class and call one of its specialized constructors
    /// to enable antialiasing or custom <see cref="GraphicsMode"/>s.
    /// </summary>
    public partial class GLControl : UserControl
    {
        #region Private Fields

        // Indicates whether the control is in design mode. Due to issues
        // wiith the DesignMode property and nested controls,we need to
        // evaluate this in the constructor.
        private readonly bool design_mode;

        private readonly GraphicsContextFlags flags;
        private readonly Int32 major, minor;
        private readonly GraphicsMode mode;
        private IGraphicsContext context;
        private IGLControl implementation;

        private bool? initial_vsync_value;

        // Indicates that OnResize was called before OnHandleCreated.
        // To avoid issues with missing OpenGL contexts, we suppress
        // the premature Resize event and raise it as soon as the handle
        // is ready.
        private bool resize_event_suppressed;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GLControl()
            : this(GraphicsMode.Default)
        {
        }

        /// <summary>
        /// Constructs a new instance with the specified GraphicsMode.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
        public GLControl(GraphicsMode mode)
            : this(mode, 1, 0, GraphicsContextFlags.Default)
        {
        }

        Toolkit toolkit;

        /// <summary>
        /// Constructs a new instance with the specified GraphicsMode.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
        /// <param name="major">The major version for the OpenGL GraphicsContext.</param>
        /// <param name="minor">The minor version for the OpenGL GraphicsContext.</param>
        /// <param name="flags">The GraphicsContextFlags for the OpenGL GraphicsContext.</param>
        public GLControl(GraphicsMode iMode, Int32 major, Int32 minor, GraphicsContextFlags flags)
        {
            if (DesignMode)
            {
                InitializeComponent();
                return;
            }

            // SDL does not currently support embedding
            // on external windows. If Open.Toolkit is not yet
            // initialized, we'll try to request a native backend
            // that supports embedding.
            // Most people are using GLControl through the
            // WinForms designer in Visual Studio. This approach
            // works perfectly in that case.
            toolkit = Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative
            });

            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = false;

            //iMode = new GraphicsMode(0, 0, 0, 0, 0, 0);
            this.mode = iMode ?? throw new ArgumentNullException("mode");
            this.major = major;
            this.minor = minor;
            this.flags = flags;

            // Note: the DesignMode property may be incorrect when nesting controls.
            // We use LicenseManager.UsageMode as a workaround (this only works in
            // the constructor).
            design_mode =
                DesignMode ||
                LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            InitializeComponent();

            //if (OpenTK.Input.Touch.GetTouchesState ().IsConnected)
            //{
            //    System.Windows.Forms.MessageBox.Show("Registration...");
            //    RegisterTouchWindow(this., TouchWindowFlag.None);
            //    System.Windows.Forms.MessageBox.Show("Registered!");
            //}
        }

        #endregion Public Constructors

        #region Public Delegates

        /// <summary>
        /// Needed to delay the invoke on OS X. Also needed because OpenTK is .NET 2, otherwise I'd use an inline Action.
        /// </summary>
        public delegate void DelayUpdate();

        public delegate void Recall();

        #endregion Public Delegates

        #region Protected Delegates

        protected delegate void Refresh_CB();

        #endregion Protected Delegates

        #region Public Enums

        public enum TouchWindowFlag : uint
        {
            None = 0x0,
            FineTouch = 0x1,
            WantPalm = 0x2
        }

        #endregion Public Enums

        #region Public Properties

        /// <summary>
        /// Gets the <c>IGraphicsContext</c> instance that is associated with the <c>GLControl</c>.
        /// The associated <c>IGraphicsContext</c> is updated whenever the <c>GLControl</c>
        /// handle is created or recreated.
        /// When using multiple <c>GLControl</c>s, ensure that <c>Context</c>
        /// is current before performing any OpenGL operations.
        /// <seealso cref="MakeCurrent"/>
        /// </summary>
        public IGraphicsContext Context
        {
            get
            {
                ValidateState();
                return context;
            }
            private set { context = value; }
        }

        /// <summary>
        /// Gets the <c>GraphicsMode</c> of the <c>IGraphicsContext</c> associated with
        /// this <c>GLControl</c>. If you wish to change <c>GraphicsMode</c>, you must
        /// destroy and recreate the <c>GLControl</c>.
        /// </summary>
        [Browsable(false)]
        public GraphicsMode GraphicsMode
        {
            get
            {
                ValidateState();
                return Context.GraphicsMode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current thread contains pending system messages.
        /// </summary>
        [Browsable(false)]
        public bool IsIdle
        {
            get
            {
                ValidateState();
                return Implementation.IsIdle;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vsync is active for this <c>GLControl</c>.
        /// When using multiple <c>GLControl</c>s, ensure that <see cref="Context"/>
        /// is current before accessing this property.
        /// <seealso cref="Context"/>
        /// <seealso cref="MakeCurrent"/>
        /// </summary>
        [Browsable(false)]
        public bool VSync
        {
            get
            {
                if (!IsHandleCreated)
                {
                    return initial_vsync_value ?? true;
                }

                ValidateState();
                //ValidateContext("VSync");
                if (Context == null)
                    return false;
                else
                    return Context.SwapInterval != 0;
            }
            set
            {
                // The winforms designer sets this to false by default which forces control creation.
                // However, events are typically connected after the VSync = false assignment, which
                // can lead to "event xyz is not fired" issues.
                // Work around this issue by deferring VSync mode setting to the HandleCreated event.
                if (!IsHandleCreated)
                {
                    initial_vsync_value = value;
                    return;
                }

                ValidateState();
                //ValidateContext("VSync");
                Context.SwapInterval = value ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="OpenTK.Platform.IWindowInfo"/> for this instance.
        /// </summary>
        [Browsable(false)]
        public IWindowInfo WindowInfo
        {
            get { return implementation.WindowInfo; }
        }

        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Gets the <c>CreateParams</c> instance for this <c>GLControl</c>
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                if (DesignMode)
                {
                    return base.CreateParams;
                }

                const Int32 CS_VREDRAW = 0x1;
                const Int32 CS_HREDRAW = 0x2;
                const Int32 CS_OWNDC = 0x20;

                CreateParams cp = base.CreateParams;
                if (Configuration.RunningOnWindows)
                {
                    // Setup necessary class style for OpenGL on windows
                    cp.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                }
                return cp;
            }
        }

        #endregion Protected Properties

        #region Private Properties

        private IGLControl Implementation
        {
            get
            {
                ValidateState();

                return implementation;
            }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// <para>
        /// Makes <see cref="GLControl.Context"/> current in the calling thread.
        /// All OpenGL commands issued are hereafter interpreted by this context.
        /// </para>
        /// <para>
        /// When using multiple <c>GLControl</c>s, calling <c>MakeCurrent</c> on
        /// one control will make all other controls non-current in the calling thread.
        /// </para>
        /// <seealso cref="Context"/>
        /// <para>
        /// A <c>GLControl</c> can only be current in one thread at a time.
        /// To make a control non-current, call <c>GLControl.Context.MakeCurrent(null)</c>.
        /// </para>
        /// </summary>
        public void MakeCurrent()
        {
            ValidateState();
            Context.MakeCurrent(Implementation.WindowInfo);
        }

        /// <summary>
        /// Execute the delayed context update
        /// </summary>
        public void PerformContextUpdate()
        {
            if (context != null)
                context.Update(Implementation.WindowInfo);
        }

        /// <summary>
        /// Swaps the front and back buffers, presenting the rendered scene to the screen.
        /// This method will have no effect on a single-buffered <c>GraphicsMode</c>.
        /// </summary>
        public void SwapBuffers()
        {
            ValidateState();
            Context.SwapBuffers();
            //
            //ValidateContext("GrabScreenshot()");

            //Stopwatch w = new Stopwatch();
            //w.Reset();
            //w.Start();

            //Bitmap bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            ////System.Drawing.Imaging.BitmapData data = bmp.LockBits(this.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            ////GL.ReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            ////bmp.UnlockBits(data);
            //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            ////bmp.Save("c:\\temp\\test" + DateTime.Now.Ticks.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //bmp.Dispose();
            //w.Stop();
            //w.Reset();
        }

        //void ISwapControl.SwapBuffers()
        //{
        //    ValidateState();
        //    //Context.SwapBuffers();
        //}

        #endregion Public Methods

        #region Protected Methods

        /// <summary>Raises the HandleCreated event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (DesignMode)
            {
                base.OnHandleCreated(e);
                return;
            }
            if (context != null)
                context.Dispose();

            if (implementation != null)
                implementation.WindowInfo.Dispose();

            if (design_mode)
                implementation = new DummyGLControl();
            else
                implementation = new GLControlFactory().CreateGLControl(mode, this);

            context = implementation.CreateContext(major, minor, flags);
            MakeCurrent();

            if (!design_mode)
                ((IGraphicsContextInternal)Context).LoadAll();

            // Deferred setting of vsync mode. See VSync property for more information.
            if (initial_vsync_value.HasValue)
            {
                Context.SwapInterval = initial_vsync_value.Value ? 1 : 0;
                initial_vsync_value = null;
            }

            base.OnHandleCreated(e);

            if (resize_event_suppressed)
            {
                OnResize(EventArgs.Empty);
                resize_event_suppressed = false;
            }
        }

        /// <summary>Raises the HandleDestroyed event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (DesignMode)
            {
                base.OnHandleDestroyed(e);
                return;
            }
            if (context != null)
            {
                context.Dispose();
                context = null;
            }

            if (implementation != null)
            {
                implementation.WindowInfo.Dispose();
                implementation = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.Paint event.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //if (DesignMode)
            //{
            //    base.OnPaint(e);
            //    return;
            //}
            ValidateState();

            //if (design_mode)
            //    e.Graphics.Clear(BackColor);

            base.OnPaint(e);
        }

        /// <summary>
        /// Raises the ParentChanged event.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            if (DesignMode)
            {
                base.OnParentChanged(e);
                return;
            }

            if (context != null)
                context.Update(Implementation.WindowInfo);

            base.OnParentChanged(e);
        }

        /// <summary>
        /// Raises the Resize event.
        /// Note: this method may be called before the OpenGL context is ready.
        /// Check that IsHandleCreated is true before using any OpenGL methods.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            if (DesignMode)
            {
                base.OnResize(e);
                return;
            }

            // Do not raise OnResize event before the handle and context are created.
            if (!IsHandleCreated)
            {
                resize_event_suppressed = true;
                return;
            }

            context.Update(Implementation.WindowInfo);

            base.OnResize(e);
        }

        //protected override void WndProc(ref Message m)
        //{
        //    //if (DesignMode)
        //    //{
        //    //    base.WndProc(ref m);
        //    //    return;
        //    //}
        //    if (m.ToString().ToLower().Contains("touch")) System.Windows.Forms.MessageBox.Show("Touch!");
        //    base.WndProc(ref m);
        //}

        #endregion Protected Methods

        //[DllImport("user32")]
        //public static extern bool RegisterTouchWindow(System.IntPtr hWnd, TouchWindowFlag flags);
        //private void ValidateContext(String message)
        //{
        //    if (!Context.IsCurrent)
        //    {
        //        Debug.Print("[GLControl] Attempted to access {0} on a non-current context. Results undefined.", message);
        //    }
        //}

        #region Private Methods

        private void ValidateState()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);

            if (!IsHandleCreated)
                CreateControl();

            if ((implementation == null || context == null || context.IsDisposed) && !DesignMode)
                RecreateHandle();
        }

        #endregion Private Methods
    }
}