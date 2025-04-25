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

using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace GLControl
{
    internal class WinGLControl : IGLControl
    {
        private class NativeMethods
        {
            [System.Security.SuppressUnmanagedCodeSecurity]
            [System.Runtime.InteropServices.DllImport("User32.dll")]
            private static extern Boolean PeekMessage(ref MSG msg, IntPtr hWnd, Int32 messageFilterMin, Int32 messageFilterMax, Int32 flags);

            public static Boolean GetPeekMessage(ref MSG msg, IntPtr hWnd, Int32 messageFilterMin, Int32 messageFilterMax, Int32 flags)
            {
                return PeekMessage(ref msg, hWnd, messageFilterMin, messageFilterMax, flags);
            }
        }

        #region P/Invoke declarations

        #region Message

        private struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Point;
            //internal object RefObject;

            public override String ToString()
            {
                return String.Format(CultureInfo.InvariantCulture, "msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (Int32)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
            }
        }

        #endregion Message

        #region Point

        private struct POINT
        {
            public Int32 X;
            public Int32 Y;

            public POINT(Int32 x, Int32 y)
            {
                this.X = x;
                this.Y = y;
            }

            public override String ToString()
            {
                return "Point {" + X.ToString() + ", " + Y.ToString() + ")";
            }
        }

        #endregion Point

        #region PeekMessage

        #endregion PeekMessage

        #region

        #endregion P/Invoke declarations

        #endregion

        #region Fields

        private MSG msg = new MSG();
        private IWindowInfo window_info;
        public GraphicsMode mode;

        #endregion

        #region Constructors

        public WinGLControl(GraphicsMode iMode, Control control)
        {
            this.mode = iMode;

            window_info = Utilities.CreateWindowsWindowInfo(control.Handle);
        }

        #endregion

        #region IGLControl Members

        public IGraphicsContext CreateContext(Int32 major, Int32 minor, GraphicsContextFlags flags)
        {
            return new GraphicsContext(mode, window_info, major, minor, flags);
        }

        public Boolean IsIdle
        {
            get { return !NativeMethods.GetPeekMessage(ref msg, IntPtr.Zero, 0, 0, 0); }
        }

        public IWindowInfo WindowInfo
        {
            get
            {
                // This method forces the creation of the control. Beware of this side-effect!
                return window_info;
            }
        }

        #endregion
    }
}