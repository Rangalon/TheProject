﻿#region License

//
// NativeWindowBase.cs
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

#endregion License

using Math3D;
using OpenTK.Input;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace OpenTK.Platform
{
    // Common base class for all INativeWindow implementations
    internal abstract class NativeWindowBase : INativeWindow
    {
        //#pragma warning disable 612,618
        //        readonly LegacyInputDriver LegacyInputDriver;
        //#pragma warning restore 612,618

        private readonly MouseButtonEventArgs MouseDownArgs = new MouseButtonEventArgs();
        private readonly MouseButtonEventArgs MouseUpArgs = new MouseButtonEventArgs();
        private readonly MouseMoveEventArgs MouseMoveArgs = new MouseMoveEventArgs();

        private readonly KeyboardKeyEventArgs KeyDownArgs = new KeyboardKeyEventArgs();
        private readonly KeyboardKeyEventArgs KeyUpArgs = new KeyboardKeyEventArgs();
        private readonly KeyPressEventArgs KeyPressArgs = new KeyPressEventArgs((char)0);

        //readonly FingerEventArgs FingerDownArgs = new FingerEventArgs();
        //readonly FingerEventArgs FingerUpArgs = new FingerEventArgs();
        //readonly FingerEventArgs FingerMoveArgs = new FingerEventArgs();

        // In order to simplify mouse event implementation,
        // we can store the current mouse state here.
        protected MouseState MouseState = new MouseState();

        protected KeyboardState KeyboardState = new KeyboardState();

        private MouseState PreviousMouseState = new MouseState();

        internal NativeWindowBase()
        {
            //#pragma warning disable 612,618
            //            LegacyInputDriver = new LegacyInputDriver(this);
            //#pragma warning restore 612,618
            MouseState.SetIsConnected(true);
            KeyboardState.SetIsConnected(true);
            PreviousMouseState.SetIsConnected(true);
        }

        #region Protected Members

        //protected void OnFingerDown(Int32 id, Single x, Single y, Single dx, Single dy, Single pressure)
        //{
        //    FingerDownArgs.Finger = new TouchFinger(id,
        //        new Vector2(x, y),
        //        new Vector2(dx, dy),
        //        pressure);
        //    FingerDown(this, FingerDownArgs);
        //}

        //protected void OnFingerUp(Int32 id, Single x, Single y, Single dx, Single dy, Single pressure)
        //{
        //    FingerUpArgs.Finger = new TouchFinger(id,
        //        new Vector2(x, y),
        //        new Vector2(dx, dy),
        //        pressure);
        //    FingerUp(this, FingerUpArgs);
        //}

        //protected void OnFingerMove(Int32 id, Single x, Single y, Single dx, Single dy, Single pressure)
        //{
        //    FingerMoveArgs.Finger = new TouchFinger(id,
        //        new Vector2(x, y),
        //        new Vector2(dx, dy),
        //        pressure);
        //    FingerMove(this, FingerMoveArgs);
        //}

        protected void OnMove(EventArgs e)
        {
            Move(this, e);
        }

        protected void OnResize(EventArgs e)
        {
            Resize(this, e);
        }

        protected void OnClosing(CancelEventArgs e)
        {
            Closing(this, e);
        }

        protected void OnClosed(EventArgs e)
        {
            Closed(this, e);
        }

        protected void OnDisposed(EventArgs e)
        {
            Disposed(this, e);
        }

        protected void OnIconChanged(EventArgs e)
        {
            IconChanged(this, e);
        }

        protected void OnTitleChanged(EventArgs e)
        {
            TitleChanged(this, e);
        }

        protected void OnVisibleChanged(EventArgs e)
        {
            VisibleChanged(this, e);
        }

        protected void OnFocusedChanged(EventArgs e)
        {
            FocusedChanged(this, e);
        }

        protected void OnWindowBorderChanged(EventArgs e)
        {
            WindowBorderChanged(this, e);
        }

        protected void OnWindowStateChanged(EventArgs e)
        {
            WindowStateChanged(this, e);
        }

        protected void OnKeyDown(Key key, bool repeat)
        {
            KeyboardState.SetKeyState(key, true);

            var e = KeyDownArgs;
            e.Keyboard = KeyboardState;
            e.Key = key;
            e.IsRepeat = repeat;
            KeyDown(this, e);
        }

        protected void OnKeyPress(char c)
        {
            var e = KeyPressArgs;
            e.KeyChar = c;
            KeyPress(this, e);
        }

        protected void OnKeyUp(Key key)
        {
            KeyboardState.SetKeyState(key, false);

            var e = KeyUpArgs;
            e.Keyboard = KeyboardState;
            e.Key = key;
            e.IsRepeat = false;
            KeyUp(this, e);
        }

        /// \internal
        /// <summary>
        /// Call this method to simulate KeyDown/KeyUp events
        /// on platforms that do not generate key events for
        /// modifier flags (e.g. Mac/Cocoa).
        /// Note: this method does not distinguish between the
        /// left and right variants of modifier keys.
        /// </summary>
        /// <param name="mods">Mods.</param>
        protected void UpdateModifierFlags(KeyModifiers mods)
        {
            bool alt = (mods & KeyModifiers.Alt) != 0;
            bool control = (mods & KeyModifiers.Control) != 0;
            bool shift = (mods & KeyModifiers.Shift) != 0;

            if (alt)
            {
                OnKeyDown(Key.AltLeft, KeyboardState[Key.AltLeft]);
                OnKeyDown(Key.AltRight, KeyboardState[Key.AltLeft]);
            }
            else
            {
                if (KeyboardState[Key.AltLeft])
                {
                    OnKeyUp(Key.AltLeft);
                }
                if (KeyboardState[Key.AltRight])
                {
                    OnKeyUp(Key.AltRight);
                }
            }

            if (control)
            {
                OnKeyDown(Key.ControlLeft, KeyboardState[Key.AltLeft]);
                OnKeyDown(Key.ControlRight, KeyboardState[Key.AltLeft]);
            }
            else
            {
                if (KeyboardState[Key.ControlLeft])
                {
                    OnKeyUp(Key.ControlLeft);
                }
                if (KeyboardState[Key.ControlRight])
                {
                    OnKeyUp(Key.ControlRight);
                }
            }

            if (shift)
            {
                OnKeyDown(Key.ShiftLeft, KeyboardState[Key.AltLeft]);
                OnKeyDown(Key.ShiftRight, KeyboardState[Key.AltLeft]);
            }
            else
            {
                if (KeyboardState[Key.ShiftLeft])
                {
                    OnKeyUp(Key.ShiftLeft);
                }
                if (KeyboardState[Key.ShiftRight])
                {
                    OnKeyUp(Key.ShiftRight);
                }
            }
        }

        protected void OnMouseLeave(EventArgs e)
        {
            MouseLeave(this, e);
        }

        protected void OnMouseEnter(EventArgs e)
        {
            MouseEnter(this, e);
        }

        protected void OnMouseMove(int x, int y)
        {
            MouseState.Position = new Vec2(x, y);

            var e = MouseMoveArgs;
            e.Mouse = MouseState;
            e.Delta = MouseState.Position - PreviousMouseState.Position;

            if (e.Delta.X == 0 && e.Delta.Y == 0)
            {
                Debug.WriteLine("OnMouseMove called without moving the mouse");
                return;
            }

            PreviousMouseState = MouseState;
            MouseMove(this, e);
        }

        #endregion Protected Members

        #region INativeWindow Members

        public event EventHandler<EventArgs> Move = delegate { };

        public event EventHandler<EventArgs> Resize = delegate { };

        public event EventHandler<System.ComponentModel.CancelEventArgs> Closing = delegate { };

        public event EventHandler<EventArgs> Closed = delegate { };

        public event EventHandler<EventArgs> Disposed = delegate { };

        public event EventHandler<EventArgs> IconChanged = delegate { };

        public event EventHandler<EventArgs> TitleChanged = delegate { };

        public event EventHandler<EventArgs> VisibleChanged = delegate { };

        public event EventHandler<EventArgs> FocusedChanged = delegate { };

        public event EventHandler<EventArgs> WindowBorderChanged = delegate { };

        public event EventHandler<EventArgs> WindowStateChanged = delegate { };

        public event EventHandler<KeyboardKeyEventArgs> KeyDown = delegate { };

        public event EventHandler<KeyPressEventArgs> KeyPress = delegate { };

        public event EventHandler<KeyboardKeyEventArgs> KeyUp = delegate { };

        public event EventHandler<EventArgs> MouseLeave = delegate { };

        public event EventHandler<EventArgs> MouseEnter = delegate { };

        public event EventHandler<MouseButtonEventArgs> MouseDown = delegate { };

        public event EventHandler<MouseButtonEventArgs> MouseUp = delegate { };

        public event EventHandler<MouseMoveEventArgs> MouseMove = delegate { };

        //public event EventHandler<FingerEventArgs> FingerDown = delegate { };
        //public event EventHandler<FingerEventArgs> FingerUp = delegate { };
        //public event EventHandler<FingerEventArgs> FingerMove = delegate { };

        public abstract void Close();

        public virtual void ProcessEvents()
        {
            if (!Focused)
            {
                // Clear keyboard state, otherwise KeyUp
                // events may be missed resulting in stuck
                // keys.
                for (Key key = 0; key < Key.LastKey; key++)
                {
                    if (KeyboardState[key])
                    {
                        OnKeyUp(key);
                    }
                }
            }
        }

        public abstract Point PointToClient(Point point);

        public abstract Point PointToScreen(Point point);

        public abstract Icon Icon { get; set; }

        public abstract string Title { get; set; }

        public abstract bool Focused { get; }

        public abstract bool Visible { get; set; }

        public abstract bool Exists { get; }

        public abstract IWindowInfo WindowInfo { get; }

        public abstract WindowState WindowState { get; set; }

        public abstract WindowBorder WindowBorder { get; set; }

        public abstract Rectangle Bounds { get; set; }

        public virtual Point Location
        {
            get
            {
                return Bounds.Location;
            }
            set
            {
                Bounds = new Rectangle(value, Bounds.Size);
            }
        }

        public virtual Size Size
        {
            get
            {
                return Bounds.Size;
            }
            set
            {
                Bounds = new Rectangle(Bounds.Location, value);
            }
        }

        public int X
        {
            get
            {
                return Bounds.X;
            }
            set
            {
                Rectangle old = Bounds;
                Bounds = new Rectangle(value, old.Y, old.Width, old.Height);
            }
        }

        public int Y
        {
            get
            {
                return Bounds.Y;
            }
            set
            {
                Rectangle old = Bounds;
                Bounds = new Rectangle(old.X, value, old.Width, old.Height);
            }
        }

        public int Width
        {
            get
            {
                return ClientSize.Width;
            }
            set
            {
                Rectangle old = ClientRectangle;
                ClientRectangle = new Rectangle(old.X, old.Y, value, old.Height);
            }
        }

        public int Height
        {
            get
            {
                return ClientSize.Height;
            }
            set
            {
                Rectangle old = ClientRectangle;
                Bounds = new Rectangle(old.X, old.Y, old.Width, value);
            }
        }

        public Rectangle ClientRectangle
        {
            get
            {
                return new Rectangle(Point.Empty, ClientSize);
            }
            set
            {
                ClientSize = value.Size;
            }
        }

        public abstract Size ClientSize { get; set; }

        public abstract bool CursorVisible { get; set; }

        public abstract MouseCursor Cursor { get; set; }

        #endregion INativeWindow Members

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        ~NativeWindowBase()
        {
            Console.WriteLine("NativeWindowBase leaked, did you forget to call Dispose()?");
            Dispose(false);
        }

        #endregion IDisposable Members
    }
}