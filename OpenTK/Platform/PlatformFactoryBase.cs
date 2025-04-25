//
// PlatformFactoryBase.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2006-2014 Stefanos Apostolopoulos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Platform.Windows;
using System;
using System.Collections.Generic;

namespace OpenTK.Platform
{
    /// \internal
    /// <summary>
    /// Implements IPlatformFactory functionality that is common
    /// for all platform backends. IPlatformFactory implementations
    /// should inherit from this class.
    /// </summary>
    internal abstract class PlatformFactoryBase : IPlatformFactory
    {
        #region Protected Fields

        protected bool IsDisposed;

        #endregion Protected Fields

        #region Private Fields

        private static readonly object sync = new object();

        private readonly List<IDisposable> Resources = new List<IDisposable>();

        #endregion Private Fields

        #region Public Constructors

        public PlatformFactoryBase()
        {
        }

        #endregion Public Constructors

        #region Private Destructors

        ~PlatformFactoryBase()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Methods

        public abstract DisplayDeviceBase CreateDisplayDeviceDriver();

        public virtual MappedGamePadDriver CreateGamePadDriver()
        {
            return new MappedGamePadDriver();
        }

        public abstract GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext();

        public abstract IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        public virtual IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            throw new NotImplementedException();
        }

        public abstract WinMMJoystick CreateJoystickDriver();

        public abstract WinRawKeyboard CreateKeyboardDriver();

        public abstract WinRawMouse CreateMouseDriver();

        public abstract INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device);

        public abstract WinRawTouch CreateTouchDriver();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterResource(IDisposable resource)
        {
            lock (sync)
            {
                Resources.Add(resource);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool manual)
        {
            if (!IsDisposed)
            {
                if (manual)
                {
                    lock (sync)
                    {
                        foreach (var resource in Resources)
                        {
                            resource.Dispose();
                        }
                        Resources.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("[OpenTK] {0} leaked with {1} live resources, did you forget to call Dispose()?",
                        GetType().FullName, Resources.Count);
                }
                IsDisposed = true;
            }
        }

        #endregion Protected Methods
    }
}