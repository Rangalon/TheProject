﻿#region --- License ---

/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */

#endregion --- License ---

using OpenTK.Graphics;
using System;
using System.Threading;

namespace OpenTK.Platform.Dummy
{
    /// \internal
    /// <summary>
    /// An empty IGraphicsContext implementation to be used inside the Visual Studio designer.
    /// This class supports OpenTK, and is not intended for use by OpenTK programs.
    /// </summary>
    internal sealed class DummyGLContext : DesktopGraphicsContext
    {
        private readonly GraphicsContext.GetAddressDelegate Loader;

        private int swap_interval;
        private static int handle_count;
        private Thread current_thread;

        #region --- Constructors ---

        public DummyGLContext()
        {
            Handle = new ContextHandle(
                new IntPtr(Interlocked.Increment(
                    ref handle_count)));
        }

        public DummyGLContext(ContextHandle handle, GraphicsContext.GetAddressDelegate loader)
            : this()
        {
            if (handle != ContextHandle.Zero)
            {
                Handle = handle;
            }
            Loader = loader;
            Mode = new GraphicsMode(new IntPtr(2), 32, 16, 0, 0, 0, 2, false);
        }

        #endregion --- Constructors ---

        #region --- IGraphicsContext Members ---

        public override void SwapBuffers()
        {
        }

        public override void MakeCurrent(IWindowInfo info)
        {
            Thread new_thread = Thread.CurrentThread;
            // A context may be current only on one thread at a time.
            if (current_thread != null && new_thread != current_thread)
            {
                throw new GraphicsContextException(
                    "Cannot make context current on two threads at the same time");
            }

            if (info != null)
            {
                current_thread = Thread.CurrentThread;
            }
            else
            {
                current_thread = null;
            }
        }

        public override bool IsCurrent
        {
            get { return current_thread != null && current_thread == Thread.CurrentThread; }
        }

        public override IntPtr GetAddress(IntPtr function)
        {
            string str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(function);
            return Loader(str);
        }

        public override int SwapInterval
        {
            get
            {
                return swap_interval;
            }
            set
            {
                swap_interval = value;
            }
        }

        public override void Update(IWindowInfo window)
        { }

        public override void LoadAll()
        {
            new OpenTK.Graphics.OpenGL.GL().LoadEntryPoints();
            //new OpenTK.Graphics.OpenGL4.GL().LoadEntryPoints();
            //new OpenTK.Graphics.ES11.GL().LoadEntryPoints();
            //new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
            //new OpenTK.Graphics.ES30.GL().LoadEntryPoints();
        }

        #endregion --- IGraphicsContext Members ---

        #region --- IDisposable Members ---

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
        }

        #endregion --- IDisposable Members ---
    }
}