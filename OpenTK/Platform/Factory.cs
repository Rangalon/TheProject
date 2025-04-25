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

using System;

namespace OpenTK.Platform
{
    using Graphics;
    using Input;
    using OpenTK.Platform.Windows;

    internal sealed class Factory : IPlatformFactory
    {
        #region Private Fields

        private static IPlatformFactory default_implementation, embedded_implementation;
        private bool disposed;

        #endregion Private Fields

        #region Public Constructors

        static Factory()
        {
            Toolkit.Init();
        }

        public Factory()
        {
            // Ensure we are correctly initialized.
            Toolkit.Init();

            // Create regular platform backend
            //if (Configuration.RunningOnSdl2) Default = new SDL2.Sdl2Factory();
            //else if (Configuration.RunningOnX11) Default = new X11.X11Factory();
            //else if (Configuration.RunningOnLinux) Default = new Linux.LinuxFactory();
            //else if (Configuration.RunningOnWindows) Default = new Windows.WinFactory();
            //else if (Configuration.RunningOnMacOS) Default = new MacOS.MacOSFactory();
            //else Default = new UnsupportedPlatform();
            Default = new Windows.WinFactory();

            // Create embedded platform backend for EGL / OpenGL ES.
            // Todo: we could probably delay this until the embedded
            // factory is actually accessed. This might improve startup
            // times slightly.
            //if (Configuration.RunningOnSdl2)
            //{
            //    // SDL supports both EGL and desktop backends
            //    // using the same API.
            //    Embedded = Default;
            //}
            //else if (Egl.Egl.IsSupported)
            //{
            //    //if (Configuration.RunningOnLinux) Embedded = Default;
            //    //else if (Configuration.RunningOnX11) Embedded = new Egl.EglX11PlatformFactory();
            //    //else if (Configuration.RunningOnWindows) Embedded = new Egl.EglWinPlatformFactory();
            //    //else if (Configuration.RunningOnMacOS) Embedded = new Egl.EglMacPlatformFactory();
            //    //else Embedded = new UnsupportedPlatform();
            //}
            //else
            //{
            //    Embedded = new UnsupportedPlatform();
            //}
            Embedded = new UnsupportedPlatform();

            if (Default is UnsupportedPlatform && !(Embedded is UnsupportedPlatform))
                Default = Embedded;
        }

        #endregion Public Constructors

        #region Private Destructors

        ~Factory()
        {
            Dispose(false);
        }

        #endregion Private Destructors

        #region Public Properties

        public static IPlatformFactory Default
        {
            get { return default_implementation; }
            private set { default_implementation = value; }
        }

        public static IPlatformFactory Embedded
        {
            get { return embedded_implementation; }
            private set { embedded_implementation = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public DisplayDeviceBase CreateDisplayDeviceDriver()
        {
            return default_implementation.CreateDisplayDeviceDriver();
        }

        public MappedGamePadDriver CreateGamePadDriver()
        {
            return default_implementation.CreateGamePadDriver();
        }

        public GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            return default_implementation.CreateGetCurrentGraphicsContext();
        }

        public IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            return default_implementation.CreateGLContext(mode, window, shareContext, directRendering, major, minor, flags);
        }

        public IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            return default_implementation.CreateGLContext(handle, window, shareContext, directRendering, major, minor, flags);
        }

        public WinMMJoystick CreateJoystickDriver()
        {
            return default_implementation.CreateJoystickDriver();
        }

        public WinRawKeyboard CreateKeyboardDriver()
        {
            return default_implementation.CreateKeyboardDriver();
        }

        public WinRawMouse CreateMouseDriver()
        {
            return default_implementation.CreateMouseDriver();
        }

        public INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title,
                                                    GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
        {
            return default_implementation.CreateNativeWindow(x, y, width, height, title, mode, options, device);
        }

        public WinRawTouch CreateTouchDriver()
        {
            return default_implementation.CreateTouchDriver();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RegisterResource(IDisposable resource)
        {
            default_implementation.RegisterResource(resource);
        }

        #endregion Public Methods

        #region Private Methods

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    Default.Dispose();
                    if (Embedded != Default)
                    {
                        Embedded.Dispose();
                    }
                }
                else
                {
                    Console.WriteLine("{0} leaked, did you forget to call Dispose()?", GetType());
                }
                disposed = true;
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class UnsupportedPlatform : PlatformFactoryBase
        {
            #region Private Fields

            private const string error_string = "Please, refer to http://www.opentk.com for more information.";

            #endregion Private Fields

            #region Public Methods

            public override DisplayDeviceBase CreateDisplayDeviceDriver()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override WinMMJoystick CreateJoystickDriver()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override WinRawKeyboard CreateKeyboardDriver()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override WinRawMouse CreateMouseDriver()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device)
            {
                throw new PlatformNotSupportedException(error_string);
            }

            public override WinRawTouch CreateTouchDriver()
            {
                throw new PlatformNotSupportedException(error_string);
            }

            #endregion Public Methods

            //public override ITouchDriver2 CreateTouchDriver()
            //{
            //    throw new PlatformNotSupportedException(error_string);
            //}

            //public override IJoystickDriver2 CreateJoystickDriver()
            //{
            //    throw new PlatformNotSupportedException(error_string);
            //}
        }

        #endregion Private Classes
    }
}