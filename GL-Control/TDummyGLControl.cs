#region License

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

#endregion License

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;

namespace GLControl
{
    internal class DummyGLControl : IGLControl
    {
        #region IGLControl Members

        public IGraphicsContext CreateContext(Int32 major, Int32 minor, GraphicsContextFlags flags)
        {
            return new DummyContext();
        }

        public Boolean IsIdle
        {
            get { return false; }
        }

        public IWindowInfo WindowInfo
        {
            get { return Utilities.CreateDummyWindowInfo(); }
        }

        #endregion IGLControl Members

        #region class DummyContext

        private class DummyContext : IGraphicsContext, IGraphicsContextInternal
        {
            private static Int32 instance_count;

            private readonly ContextHandle handle = new ContextHandle(new IntPtr(
                System.Threading.Interlocked.Increment(ref instance_count)));

            private IWindowInfo current_window;
            private Boolean is_disposed;
            private Int32 swap_interval;

            #region IGraphicsContext Members

            public void SwapBuffers()
            {
            }

            public void MakeCurrent(IWindowInfo window)
            {
                current_window = window;
            }

            public Boolean IsCurrent
            {
                get { return current_window != null; }
            }

            public Boolean IsDisposed
            {
                get { return is_disposed; }
            }

            public Boolean VSync
            {
                get
                {
                    return SwapInterval != 0;
                }
                set
                {
                    SwapInterval = value ? 1 : 0;
                }
            }

            public Int32 SwapInterval
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

            public void Update(IWindowInfo window)
            {
            }

            public GraphicsMode GraphicsMode
            {
                get { return GraphicsMode.Default; }
            }

            public Boolean ErrorChecking
            {
                get
                {
                    return false;
                }
                set
                {
                }
            }

            public void LoadAll()
            {
            }

            public void Dispose()
            {
                is_disposed = true;
            }

            #endregion IGraphicsContext Members

            #region IGraphicsContextInternal

            public ContextHandle Context
            {
                get { return handle; }
            }

            public IntPtr GetAddress(IntPtr function)
            {
                return IntPtr.Zero;
            }

            public IntPtr GetAddress(String function)
            {
                return IntPtr.Zero;
            }

            public IGraphicsContext Implementation
            {
                get { return this; }
            }

            #endregion IGraphicsContextInternal
        }

        #endregion class DummyContext
    }
}