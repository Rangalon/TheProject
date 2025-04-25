#region License

//
// MouseEventArgs.cs
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
using System;

namespace OpenTK.Input
{
    /// <summary>
    /// Defines the event data for <see cref="MouseDevice"/> events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do not cache instances of this type outside their event handler.
    /// If necessary, you can clone an instance using the
    /// <see cref="MouseEventArgs(MouseEventArgs)"/> constructor.
    /// </para>
    /// </remarks>
    public class MouseEventArgs : EventArgs
    {
        #region Fields

        private MouseState state;

        #endregion Fields

        #region Constructors

        public Vec2 Position { get { return state.Position; } internal set { state.Position = value; } }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public MouseEventArgs()
        {
            state.SetIsConnected(true);
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public MouseEventArgs(Vec2 iPosition) : this() { state.Position = iPosition; }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance to clone.</param>
        public MouseEventArgs(MouseEventArgs args)
            : this(args.Position)
        {
        }

        #endregion Constructors

        #region Protected Members

        internal void SetButton(MouseButton button, ButtonState state)
        {
            if (button < 0 || button > MouseButton.LastButton)
                throw new ArgumentOutOfRangeException();

            switch (state)
            {
                case ButtonState.Pressed:
                    this.state.EnableBit((int)button);
                    break;

                case ButtonState.Released:
                    this.state.DisableBit((int)button);
                    break;
            }
        }

        #endregion Protected Members

        #region Public Members

        /// <summary>
        /// Gets a <see cref="System.Drawing.Point"/> representing the location of the mouse for the event.
        /// </summary>
        //public Point Position
        //{
        //    get { return new Point(state.X, state.Y); }
        //    set
        //    {
        //        X = value.X;
        //        Y = value.Y;
        //    }
        //}

        /// <summary>
        /// Gets the current <see cref="OpenTK.Input.MouseState"/>.
        /// </summary>
        public MouseState Mouse
        {
            get { return state; }
            internal set { state = value; }
        }

        #endregion Public Members
    }

    /// <summary>
    /// Defines the event data for <see cref="MouseDevice.Move"/> events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do not cache instances of this type outside their event handler.
    /// If necessary, you can clone an instance using the
    /// <see cref="MouseMoveEventArgs(MouseMoveEventArgs)"/> constructor.
    /// </para>
    /// </remarks>
    public class MouseMoveEventArgs : MouseEventArgs
    {
        #region Fields

        private Vec2 delta;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        public MouseMoveEventArgs() { }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="xDelta">The change in X position produced by this event.</param>
        /// <param name="yDelta">The change in Y position produced by this event.</param>
        public MouseMoveEventArgs(Vec2 iPosition, Vec2 iDelta)
            : base(iPosition)
        {
            delta = iDelta;
        }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseMoveEventArgs"/> instance to clone.</param>
        public MouseMoveEventArgs(MouseMoveEventArgs args)
            : this(args.Position, args.Delta)
        {
        }

        #endregion Constructors

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        public Vec2 Delta { get { return delta; } internal set { delta = value; } }

        #endregion Public Members
    }

    /// <summary>
    /// Defines the event data for <see cref="MouseDevice.ButtonDown"/> and <see cref="MouseDevice.ButtonUp"/> events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Do not cache instances of this type outside their event handler.
    /// If necessary, you can clone an instance using the
    /// <see cref="MouseButtonEventArgs(MouseButtonEventArgs)"/> constructor.
    /// </para>
    /// </remarks>
    public class MouseButtonEventArgs : MouseEventArgs
    {
        #region Fields

        private MouseButton button;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new <see cref="MouseButtonEventArgs"/> instance.
        /// </summary>
        public MouseButtonEventArgs() { }

        /// <summary>
        /// Constructs a new <see cref="MouseButtonEventArgs"/> instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="button">The mouse button for the event.</param>
        /// <param name="pressed">The current state of the button.</param>
        public MouseButtonEventArgs(Vec2 iPosition, MouseButton button, bool pressed)
            : base(iPosition)
        {
            this.button = button;
        }

        #endregion Constructors

        #region Public Members

        /// <summary>
        /// Gets the <see cref="MouseButton"/> that triggered this event.
        /// </summary>
        public MouseButton Button { get { return button; } internal set { button = value; } }

        #endregion Public Members
    }
}