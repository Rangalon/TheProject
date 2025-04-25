#region License

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

using Math3D;
using System;
using System.Globalization;

namespace OpenTK.Input
{
    /// <summary>
    /// Encapsulates the state of a mouse device.
    /// </summary>
    public struct MouseState : IEquatable<MouseState>
    {
        #region Fields

        internal const int MaxButtons = 16; // we are storing in an ushort
        private Vec2 position;
        private int buttons;
        private bool is_connected;

        public int Buttons { get { return buttons; } private set { buttons = value; } }

        #endregion Fields

        #region Public Members

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return is_connected; }
            internal set { is_connected = value; }
        }

        /// <summary>
        /// Checks whether two <see cref="MouseState" /> instances are equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is equal to right; false otherwise.
        /// </returns>
        public static bool operator ==(MouseState left, MouseState right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="MouseState" /> instances are not equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="MouseState"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is not equal to right; false otherwise.
        /// </returns>
        public static bool operator !=(MouseState left, MouseState right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares to an object instance for equality.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare to.
        /// </param>
        /// <returns>
        /// True if this instance is equal to obj; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is MouseState)
            {
                return this == (MouseState)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a hashcode for the current instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/> represting the hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return buttons.GetHashCode() ^ position.X.GetHashCode() ^ position.Y.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.MouseState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="OpenTK.Input.MouseState"/>.</returns>
        public override string ToString()
        {
            string b = Convert.ToString(buttons, 2).PadLeft(10, '0');
            return string.Format(CultureInfo.InvariantCulture, "[X={0}, Y={1}, Buttons={2}, IsConnected={3}]",
                position.X, position.Y, b, IsConnected);
        }

        #endregion Public Members

        #region Internal Members

        public Vec2 Position
        {
            get { return position; }
            set { position = value; }
        }

        internal void EnableBit(int offset)
        {
            ValidateOffset(offset);
            buttons |= unchecked((ushort)(1 << offset));
        }

        internal void DisableBit(int offset)
        {
            ValidateOffset(offset);
            buttons &= unchecked((ushort)(~(1 << offset)));
        }

        internal void MergeBits(MouseState other)
        {
            buttons |= other.buttons;
            position += other.position;
            IsConnected |= other.IsConnected;
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        #endregion Internal Members

        #region Private Members

        private static void ValidateOffset(int offset)
        {
            if (offset < 0 || offset >= 16)
                throw new ArgumentOutOfRangeException("offset");
        }

        #endregion Private Members

        #region IEquatable<MouseState> Members

        /// <summary>
        /// Compares two MouseState instances.
        /// </summary>
        /// <param name="other">The instance to compare two.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(MouseState other)
        {
            return
                buttons == other.buttons &&
                position == other.position;
        }

        #endregion IEquatable<MouseState> Members
    }
}