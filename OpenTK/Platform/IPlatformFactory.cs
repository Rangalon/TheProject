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

using OpenTK.Graphics;
using OpenTK.Platform.Windows;
using System;

namespace OpenTK.Platform
{
    internal interface IPlatformFactory : IDisposable
    {
        #region Public Methods

        DisplayDeviceBase CreateDisplayDeviceDriver();

        MappedGamePadDriver CreateGamePadDriver();

        GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext();

        IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags);

        WinMMJoystick CreateJoystickDriver();

       WinRawKeyboard CreateKeyboardDriver();

        WinRawMouse CreateMouseDriver();

        INativeWindow CreateNativeWindow(int x, int y, int width, int height, string title, GraphicsMode mode, GameWindowFlags options, DisplayDevice device);

        WinRawTouch CreateTouchDriver();

        void RegisterResource(IDisposable resource);

        #endregion Public Methods
    }
}