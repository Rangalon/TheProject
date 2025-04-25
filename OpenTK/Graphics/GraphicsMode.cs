/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */

using System;
using System.Globalization;

namespace OpenTK.Graphics
{
    /// <summary>Defines the format for graphics operations.</summary>
    public class GraphicsMode : IEquatable<GraphicsMode>
    {
        #region Private Fields

        private static readonly object SyncRoot = new object();
        private static GraphicsMode defaultMode;
        private ColorFormat color_format, accumulator_format;
        private int depth, stencil, buffers, samples;
        private IntPtr? index = null;
        private bool stereo;

        #endregion Private Fields

        #region Public Constructors

        // The id of the pixel format or visual.
        // Disable BeforeFieldInit
        static GraphicsMode() { }

        /// <summary>Constructs a new GraphicsMode with sensible default parameters.</summary>
        public GraphicsMode()
            : this(Default)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        public GraphicsMode(ColorFormat color)
            : this(color, Default.Depth, Default.Stencil, Default.Samples, Default.AccumulatorFormat, Default.Buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        public GraphicsMode(ColorFormat color, int depth)
            : this(color, depth, Default.Stencil, Default.Samples, Default.AccumulatorFormat, Default.Buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        /// <param name="stencil">The number of bits in the stencil buffer.</param>
        public GraphicsMode(ColorFormat color, int depth, int stencil)
            : this(color, depth, stencil, Default.Samples, Default.AccumulatorFormat, Default.Buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        /// <param name="stencil">The number of bits in the stencil buffer.</param>
        /// <param name="samples">The number of samples for FSAA.</param>
        public GraphicsMode(ColorFormat color, int depth, int stencil, int samples)
            : this(color, depth, stencil, samples, Default.AccumulatorFormat, Default.Buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        /// <param name="stencil">The number of bits in the stencil buffer.</param>
        /// <param name="samples">The number of samples for FSAA.</param>
        /// <param name="accum">The ColorFormat of the accumilliary buffer.</param>
        public GraphicsMode(ColorFormat color, int depth, int stencil, int samples, ColorFormat accum)
            : this(color, depth, stencil, samples, accum, Default.Buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        /// <param name="stencil">The number of bits in the stencil buffer.</param>
        /// <param name="samples">The number of samples for FSAA.</param>
        /// <param name="accum">The ColorFormat of the accumilliary buffer.</param>
        /// <param name="buffers">The number of render buffers. Typical values include one (single-), two (Double-) or three (triple-buffering).</param>
        public GraphicsMode(ColorFormat color, int depth, int stencil, int samples, ColorFormat accum, int buffers)
            : this(color, depth, stencil, samples, accum, buffers, Default.Stereo)
        { }

        /// <summary>Constructs a new GraphicsMode with the specified parameters.</summary>
        /// <param name="color">The ColorFormat of the color buffer.</param>
        /// <param name="depth">The number of bits in the depth buffer.</param>
        /// <param name="stencil">The number of bits in the stencil buffer.</param>
        /// <param name="samples">The number of samples for FSAA.</param>
        /// <param name="accum">The ColorFormat of the accumilliary buffer.</param>
        /// <param name="stereo">Set to true for a GraphicsMode with stereographic capabilities.</param>
        /// <param name="buffers">The number of render buffers. Typical values include one (single-), two (Double-) or three (triple-buffering).</param>
        public GraphicsMode(ColorFormat color, int depth, int stencil, int samples, ColorFormat accum, int buffers, bool stereo)
            : this(null, color, depth, stencil, samples, accum, buffers, stereo) { }

        #endregion Public Constructors

        #region Internal Constructors

        internal GraphicsMode(GraphicsMode mode)
            : this(mode.ColorFormat, mode.Depth, mode.Stencil, mode.Samples, mode.AccumulatorFormat, mode.Buffers, mode.Stereo) { }

        internal GraphicsMode(IntPtr? index, ColorFormat color, int depth, int stencil, int samples, ColorFormat accum,
                              int buffers, bool stereo)
        {
            if (depth < 0) throw new ArgumentOutOfRangeException("depth", "Must be greater than, or equal to zero.");
            if (stencil < 0) throw new ArgumentOutOfRangeException("stencil", "Must be greater than, or equal to zero.");
            if (buffers < 0) throw new ArgumentOutOfRangeException("buffers", "Must be greater than, or equal to zero.");
            if (samples < 0) throw new ArgumentOutOfRangeException("samples", "Must be greater than, or equal to zero.");

            this.Index = index;
            this.ColorFormat = color;
            this.Depth = depth;
            this.Stencil = stencil;
            this.Samples = samples;
            this.AccumulatorFormat = accum;
            this.Buffers = buffers;
            this.Stereo = stereo;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>Returns an OpenTK.GraphicsFormat compatible with the underlying platform.</summary>
        public static GraphicsMode Default
        {
            get
            {
                lock (SyncRoot)
                {
                    if (defaultMode == null)
                    {
                        //defaultMode = new GraphicsMode(null, 1, 1, 0, 8, 0, 0, false);
                        defaultMode = new GraphicsMode(null, 32, 16, 0, 32, 0, 0, false);
                        Console.WriteLine("GraphicsMode.Default = {0}", defaultMode.ToString());
                    }
                    return defaultMode;
                }
            }
        }

        /// <summary>
        /// Gets an OpenTK.Graphics.ColorFormat that describes the accumulator format for this GraphicsFormat.
        /// </summary>
        public ColorFormat AccumulatorFormat
        {
            get
            {
                return accumulator_format;
            }
            private set { accumulator_format = value; }
        }

        /// <summary>
        /// Gets a System.Int32 containing the number of buffers associated with this
        /// DisplayMode.
        /// </summary>
        public int Buffers
        {
            get
            {
                return buffers;
            }
            private set { buffers = value; }
        }

        /// <summary>
        /// Gets an OpenTK.Graphics.ColorFormat that describes the color format for this GraphicsFormat.
        /// </summary>
        public ColorFormat ColorFormat
        {
            get
            {
                return color_format;
            }
            private set { color_format = value; }
        }

        /// <summary>
        /// Gets a System.Int32 that contains the bits per pixel for the depth buffer
        /// for this GraphicsFormat.
        /// </summary>
        public int Depth
        {
            get
            {
                return depth;
            }
            private set { depth = value; }
        }

        /// <summary>
        /// Gets a nullable <see cref="System.IntPtr"/> value, indicating the platform-specific index for this GraphicsMode.
        /// </summary>
        public IntPtr? Index
        {
            get
            {
                return index;
            }
            set { index = value; }
        }

        /// <summary>
        /// Gets a System.Int32 that contains the number of FSAA samples per pixel for this GraphicsFormat.
        /// </summary>
        public int Samples
        {
            get
            {
                return samples;
            }
            private set
            {
                if (value > 64)
                    samples = 64;
                else if (value < 0)
                    samples = 0;
                else
                    samples = value;
            }
        }

        /// <summary>
        /// Gets a System.Int32 that contains the bits per pixel for the stencil buffer
        /// of this GraphicsFormat.
        /// </summary>
        public int Stencil
        {
            get
            {
                return stencil;
            }
            private set { stencil = value; }
        }

        /// <summary>
        /// Gets a System.Boolean indicating whether this DisplayMode is stereoscopic.
        /// </summary>
        public bool Stereo
        {
            get
            {
                return stereo;
            }
            private set { stereo = value; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Indicates whether obj is equal to this instance.
        /// </summary>
        /// <param name="obj">An object instance to compare for equality.</param>
        /// <returns>True, if obj equals this instance; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is GraphicsMode)
            {
                return Equals((GraphicsMode)obj);
            }
            return false;
        }

        /// <summary>
        /// Indicates whether other represents the same mode as this instance.
        /// </summary>
        /// <param name="other">The GraphicsMode to compare to.</param>
        /// <returns>True, if other is equal to this instance; false otherwise.</returns>
        public bool Equals(GraphicsMode other)
        {
            return Index.HasValue && Index == other.Index;
        }

        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return Index.GetHashCode();
        }

        /// <summary>Returns a System.String describing the current GraphicsFormat.</summary>
        /// <returns>! System.String describing the current GraphicsFormat.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Index: {0}, Color: {1}, Depth: {2}, Stencil: {3}, Samples: {4}, Accum: {5}, Buffers: {6}, Stereo: {7}",
                Index, ColorFormat, Depth, Stencil, Samples, AccumulatorFormat, Buffers, Stereo);
        }

        #endregion Public Methods
    }
}