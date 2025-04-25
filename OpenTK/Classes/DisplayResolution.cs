#region --- License ---

/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the OpenTK team.
 * This notice may not be removed.
 * See license.txt for licensing detailed licensing details.
 */

#endregion --- License ---

using System;

#if !MINIMAL

using System.Drawing;

#endif

namespace OpenTK
{
    /// <summary>Contains information regarding a monitor's display resolution.</summary>
    public class DisplayResolution
    {
        private Rectangle bounds;
        private int bits_per_pixel;
        private float refresh_rate;

        #region --- Constructors ---

        internal DisplayResolution()
        {
        }

        #region public DisplayResolution(Int32 width, Int32 height, Int32 bitsPerPixel, Single refreshRate)

        // Creates a new DisplayResolution object for the primary DisplayDevice.
        internal DisplayResolution(int x, int y, int width, int height, int bitsPerPixel, float refreshRate)
        {
            // Refresh rate may be zero, since this information may not be available on some platforms.
            if (width <= 0) throw new ArgumentOutOfRangeException("width", "Must be greater than zero.");
            if (height <= 0) throw new ArgumentOutOfRangeException("height", "Must be greater than zero.");
            if (bitsPerPixel <= 0) throw new ArgumentOutOfRangeException("bitsPerPixel", "Must be greater than zero.");
            if (refreshRate < 0) throw new ArgumentOutOfRangeException("refreshRate", "Must be greater than, or equal to zero.");

            this.bounds = new Rectangle(x, y, width, height);
            this.bits_per_pixel = bitsPerPixel;
            this.refresh_rate = refreshRate;
        }

        #endregion public DisplayResolution(Int32 width, Int32 height, Int32 bitsPerPixel, Single refreshRate)

        #region public DisplayResolution(Int32 width, Int32 height, Int32 bitsPerPixel, Single refreshRate, DisplayDevice device)

#if false

        /// <summary>
        /// Creates a new DisplayResolution object for the specified DisplayDevice.
        /// </summary>
        /// <param name="width">The requested width in pixels.</param>
        /// <param name="height">The requested height in pixels.</param>
        /// <param name="bitsPerPixel">The requested bits per pixel in bits.</param>
        /// <param name="refreshRate">The requested refresh rate in hertz.</param>
        /// <remarks>OpenTK will select the closest match between all available resolutions on the specified DisplayDevice.</remarks>
        ///
        public DisplayResolution(Int32 width, Int32 height, Int32 bitsPerPixel, Single refreshRate, DisplayDevice device)
        {
            // Refresh rate may be zero, since this information may not be available on some platforms.
            if (width <= 0) throw new ArgumentOutOfRangeException("width", "Must be greater than zero.");
            if (height <= 0) throw new ArgumentOutOfRangeException("height", "Must be greater than zero.");
            if (bitsPerPixel <= 0) throw new ArgumentOutOfRangeException("bitsPerPixel", "Must be greater than zero.");
            if (refreshRate < 0) throw new ArgumentOutOfRangeException("refreshRate", "Must be greater than, or equal to zero.");
            if (device == null) throw new ArgumentNullException("DisplayDevice", "Must be a valid DisplayDevice");

            DisplayResolution res = device.SelectResolution(width, height, bitsPerPixel, refreshRate);

            this.width = res.width;
            this.height = res.height;
            this.bits_per_pixel = res.bits_per_pixel;
            this.refresh_rate = res.refresh_rate;
        }
#endif

        #endregion public DisplayResolution(Int32 width, Int32 height, Int32 bitsPerPixel, Single refreshRate, DisplayDevice device)

        #endregion --- Constructors ---

        #region --- Public Methods ---

        #region public Int32 Width

        /// <summary>Gets a System.Int32 that contains the width of this display in pixels.</summary>
        public int Width
        {
            get { return bounds.Width; }
            internal set { bounds.Width = value; }
        }

        #endregion public Int32 Width

        #region public Int32 Height

        /// <summary>Gets a System.Int32 that contains the height of this display in pixels.</summary>
        public int Height
        {
            get { return bounds.Height; }
            internal set { bounds.Height = value; }
        }

        #endregion public Int32 Height

        #region public Int32 BitsPerPixel

        /// <summary>Gets a System.Int32 that contains number of bits per pixel of this display. Typical values include 8, 16, 24 and 32.</summary>
        public int BitsPerPixel
        {
            get { return bits_per_pixel; }
            internal set { bits_per_pixel = value; }
        }

        #endregion public Int32 BitsPerPixel

        #region public Single RefreshRate

        /// <summary>
        /// Gets a System.Single representing the vertical refresh rate of this display.
        /// </summary>
        public float RefreshRate
        {
            get { return refresh_rate; }
            internal set { refresh_rate = value; }
        }

        #endregion public Single RefreshRate

        #endregion --- Public Methods ---

        #region --- Overrides ---

        #region public override Boolean Equals(object obj)

        /// <summary>Determines whether the specified resolutions are equal.</summary>
        /// <param name="obj">The System.Object to check against.</param>
        /// <returns>True if the System.Object is an equal DisplayResolution; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() == obj.GetType())
            {
                DisplayResolution res = (DisplayResolution)obj;
                return
                    Width == res.Width &&
                    Height == res.Height &&
                    BitsPerPixel == res.BitsPerPixel &&
                    RefreshRate == res.RefreshRate;
            }

            return false;
        }

        #endregion public override Boolean Equals(object obj)

        #region public override Int32 GetHashCode()

        /// <summary>Returns a unique hash representing this resolution.</summary>
        /// <returns>A System.Int32 that may serve as a hash code for this resolution.</returns>
        public override int GetHashCode()
        {
#pragma warning disable 612, 618
            return bits_per_pixel ^ refresh_rate.GetHashCode();
#pragma warning restore 612, 618
        }

        #endregion public override Int32 GetHashCode()

        #endregion --- Overrides ---

        #region --- Operator Overloads ---

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(DisplayResolution left, DisplayResolution right)
        {
            if (((object)left) == null && ((object)right) == null)
                return true;
            else if ((((object)left) == null && ((object)right) != null) ||
                     (((object)left) != null && ((object)right) == null))
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(DisplayResolution left, DisplayResolution right)
        {
            return !(left == right);
        }

        #endregion --- Operator Overloads ---
    }
}