﻿#region License

//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
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

using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    /// \internal
    /// <summary>Describes a win32 window.</summary>
    internal sealed class WinWindowInfo : IWindowInfo
    {
        private IntPtr handle, dc;
        private WinWindowInfo parent;
        private bool disposed;

        #region --- Constructors ---

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public WinWindowInfo()
        {
        }

        /// <summary>
        /// Constructs a new instance with the specified window handle and paren.t
        /// </summary>
        /// <param name="handle">The window handle for this instance.</param>
        /// <param name="parent">The parent window of this instance (may be null).</param>
        public WinWindowInfo(IntPtr handle, WinWindowInfo parent)
        {
            this.handle = handle;
            this.parent = parent;
        }

        #endregion --- Constructors ---

        #region --- Public Methods ---

        /// <summary>
        /// Gets or sets the handle of the window.
        /// </summary>
        public IntPtr Handle { get { return handle; } set { handle = value; } }

        /// <summary>
        /// Gets or sets the Parent of the window (may be null).
        /// </summary>
        public WinWindowInfo Parent { get { return parent; } set { parent = value; } }

        /// <summary>
        /// Gets the device context for this window instance.
        /// </summary>
        public IntPtr DeviceContext
        {
            get
            {
                if (dc == IntPtr.Zero)
                    dc = Functions.GetDC(this.Handle);
                //dc = Functions.GetWindowDC(this.Handle);
                return dc;
            }
        }

        // For compatibility with whoever thought it would be
        // a good idea to access internal APIs through reflection
        // (e.g. MonoGame)
        public IntPtr WindowHandle { get { return Handle; } set { Handle = value; } }

        #region public override String ToString()

        /// <summary>Returns a System.String that represents the current window.</summary>
        /// <returns>A System.String that represents the current window.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Windows.WindowInfo: Handle {0}, Parent ({1})",
                this.Handle, this.Parent != null ? this.Parent.ToString() : "null");
        }

        /// <summary>Checks if <c>this</c> and <c>obj</c> reference the same win32 window.</summary>
        /// <param name="obj">The object to check against.</param>
        /// <returns>True if <c>this</c> and <c>obj</c> reference the same win32 window; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            WinWindowInfo info = (WinWindowInfo)obj;

            if (info == null) return false;
            // TODO: Assumes windows will always have unique handles.
            return handle.Equals(info.handle);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A hash code for the current <c>WinWindowInfo</c>.</returns>
        public override int GetHashCode()
        {
            return handle.GetHashCode();
        }

        #endregion public override String ToString()

        #endregion --- Public Methods ---

        #region --- IDisposable ---

        #region public void Dispose()

        /// <summary>Releases the unmanaged resources consumed by this instance.</summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion public void Dispose()

        #region void Dispose(Boolean manual)

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (this.dc != IntPtr.Zero)
                    if (!Functions.ReleaseDC(this.handle, this.dc))
                        Console.WriteLine("[Warning] Failed to release device context {0}. Windows error: {1}.", this.dc, Marshal.GetLastWin32Error());

                if (manual)
                {
                    if (parent != null)
                        parent.Dispose();
                }

                disposed = true;
            }
        }

        #endregion void Dispose(Boolean manual)

        #region ~WinWindowInfo()

        ~WinWindowInfo()
        {
            this.Dispose(false);
        }

        #endregion ~WinWindowInfo()

        #endregion --- IDisposable ---
    }
}