using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace JaadWinControls
{
    public abstract class TRenderer
    {
        #region Protected Fields

        protected static readonly StringFormat StrCC = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        protected static readonly StringFormat StrCN = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        protected static readonly StringFormat StrFF = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far };
        protected static readonly StringFormat StrFN = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near };
        protected static readonly StringFormat StrNF = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far };
        protected static readonly StringFormat StrNN = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
        protected static SolidBrush BackBrush = new SolidBrush(Color.FromArgb(32, 0, 0, 0));
        protected static Font FT10 = new Font("Verdana", 10);
        protected static Font FT12 = new Font("Verdana", 12);
        protected static Font FT12b = new Font("Verdana", 12, FontStyle.Bold);
        protected static Font FT14 = new Font("Verdana", 14);
        protected static Font FT14b = new Font("Verdana", 14, FontStyle.Bold);
        protected static Font FT16 = new Font("Verdana", 16);
        protected static Font FT18 = new Font("Verdana", 18);
        protected static Font FT18b = new Font("Verdana", 18, FontStyle.Bold);
        protected static Font FT24 = new Font("Verdana", 24);
        protected static Font FT24b = new Font("Verdana", 24, FontStyle.Bold);
        protected static Font FT30 = new Font("Verdana", 30);
        protected static Font FT36b = new Font("Verdana", 36, FontStyle.Bold);
        protected static Font FT6 = new Font("Verdana", 6);
        protected static Font FT60b = new Font("Verdana", 60, FontStyle.Bold);
        protected static Font FT7 = new Font("Verdana", 7);
        protected static Font FT8 = new Font("Verdana", 8);
        protected static Font FT8b = new Font("Verdana", 8, FontStyle.Bold);
        protected Bitmap bmp;
        protected bool IsDirty;

        #endregion Protected Fields

        #region Private Fields

        private static readonly double LessRatio = -13;

        private static readonly double MoreRatio = +23;

        private Rectangle BmpRectangle = new Rectangle();
        private bool disposed;

        private int texture = -1;

        #endregion Private Fields

        #region Public Constructors

        public TRenderer(int width, int height)
        {
            //if (GraphicsContext.CurrentContext == null)
            //{
            //    throw new InvalidOperationException("No GraphicsContext is current on the calling thread.");
            //}
            BmpRectangle.Width = width; BmpRectangle.Height = height;
            bmp = new Bitmap(BmpRectangle.Width, BmpRectangle.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //gfx = Graphics.FromImage(bmp);
            //gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            //Update();
        }

        #endregion Public Constructors

        #region Private Destructors

        ~TRenderer()
        {
            Console.WriteLine("[Warning] Resource leaked: {0}.", this.GetType().Name);
        }

        #endregion Private Destructors

        #region Public Methods

        public void Apply()
        {
            int W = BmpRectangle.Width, H = BmpRectangle.Height;

            if (texture < 0)
            {
                texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, W, H, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            }

            if (IsDirty)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, W, H, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
                //

                lock (bmp)
                {
                    System.Drawing.Imaging.BitmapData data = bmp.LockBits(BmpRectangle, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, W, H, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    bmp.UnlockBits(data);
                }

                //
                IsDirty = false;
            }

            GL.BindTexture(TextureTarget.Texture2D, texture);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Bitmap Prepare()
        {
            lock (bmp) return (Bitmap)bmp.Clone();
        }

        public abstract void Update();

        public virtual void UpdateBmpSize(float w)
        {
            lock (this)
            {
                if (bmp != null) bmp.Dispose();
                BmpRectangle.Width = (int)(w * 200 / bmp.Height);
                bmp = new Bitmap(BmpRectangle.Width, BmpRectangle.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            Update();
        }

        #endregion Public Methods

        #region Protected Methods

        protected static void ChangeColor(int[] t, double r, double g, double b)
        {
            for (int i = 0; i < t.Length; i++)
            {
                byte[] bts = BitConverter.GetBytes(t[i]);
                bts[2] = (byte)Math.Min(255, (r * bts[2]));
                bts[1] = (byte)Math.Min(255, (g * bts[1]));
                bts[0] = (byte)Math.Min(255, (b * bts[0]));
                t[i] = BitConverter.ToInt32(bts, 0);
            }
        }

        protected static void DeEmbossRectangle(int[] g, Rectangle rec, int width, int w)
        {
            byte[] bts;
            int xl, xr, yb, yt;
            for (int i = 0; i < w; i++)
            {
                xl = rec.Left + i;
                xr = rec.Right - 1 - i;
                for (int y = rec.Top + i * 0; y < rec.Bottom - i * 0; y++)
                {
                    bts = BitConverter.GetBytes(g[y * width + xr]);
                    bts[0] = (byte)Math.Max(0, (LessRatio + bts[0]));
                    bts[1] = (byte)Math.Max(0, (LessRatio + bts[1]));
                    bts[2] = (byte)Math.Max(0, (LessRatio + bts[2]));
                    g[y * width + xr] = BitConverter.ToInt32(bts, 0);
                    bts = BitConverter.GetBytes(g[y * width + xl]);
                    bts[0] = (byte)Math.Min(255, (MoreRatio + bts[0]));
                    bts[1] = (byte)Math.Min(255, (MoreRatio + bts[1]));
                    bts[2] = (byte)Math.Min(255, (MoreRatio + bts[2]));
                    g[y * width + xl] = BitConverter.ToInt32(bts, 0);
                }

                yt = rec.Top + i;
                yb = rec.Bottom - 1 - i;
                for (int x = rec.Left + i * 0; x < rec.Right - i * 0; x++)
                {
                    bts = BitConverter.GetBytes(g[yb * width + x]);
                    bts[0] = (byte)Math.Max(0, (LessRatio + bts[0]));
                    bts[1] = (byte)Math.Max(0, (LessRatio + bts[1]));
                    bts[2] = (byte)Math.Max(0, (LessRatio + bts[2]));
                    g[yb * width + x] = BitConverter.ToInt32(bts, 0);
                    bts = BitConverter.GetBytes(g[yt * width + x]);
                    bts[0] = (byte)Math.Min(255, (MoreRatio + bts[0]));
                    bts[1] = (byte)Math.Min(255, (MoreRatio + bts[1]));
                    bts[2] = (byte)Math.Min(255, (MoreRatio + bts[2]));
                    g[yt * width + x] = BitConverter.ToInt32(bts, 0);
                }
            }
        }

        protected static void EmbossRectangle(int[] g, Rectangle rec, int width)
        {
            EmbossRectangle(g, rec, width, 2);
        }

        protected static void EmbossRectangle(int[] g, Rectangle rec, int width, int w)
        {
            byte[] bts;
            int xl, xr, yb, yt;
            for (int i = 0; i < w; i++)
            {
                xl = rec.Left - w + i;
                xr = rec.Right + w - 1 - i;
                for (int y = rec.Top - i - 1; y < rec.Bottom + w + i - 1; y++)
                {
                    bts = BitConverter.GetBytes(g[y * width + xl]);
                    bts[0] = (byte)Math.Max(0, (LessRatio + bts[0]));
                    bts[1] = (byte)Math.Max(0, (LessRatio + bts[1]));
                    bts[2] = (byte)Math.Max(0, (LessRatio + bts[2]));
                    g[y * width + xl] = BitConverter.ToInt32(bts, 0);
                    bts = BitConverter.GetBytes(g[y * width + xr]);
                    bts[0] = (byte)Math.Min(255, (MoreRatio + bts[0]));
                    bts[1] = (byte)Math.Min(255, (MoreRatio + bts[1]));
                    bts[2] = (byte)Math.Min(255, (MoreRatio + bts[2]));
                    g[y * width + xr] = BitConverter.ToInt32(bts, 0);
                }

                yt = rec.Top - w + i;
                yb = rec.Bottom + w - 1 - i;
                for (int x = rec.Left - i - 1; x < rec.Right + w + i - 1; x++)
                {
                    bts = BitConverter.GetBytes(g[yt * width + x]);
                    bts[0] = (byte)Math.Max(0, (LessRatio + bts[0]));
                    bts[1] = (byte)Math.Max(0, (LessRatio + bts[1]));
                    bts[2] = (byte)Math.Max(0, (LessRatio + bts[2]));
                    g[yt * width + x] = BitConverter.ToInt32(bts, 0);
                    bts = BitConverter.GetBytes(g[yb * width + x]);
                    bts[0] = (byte)Math.Min(255, (MoreRatio + bts[0]));
                    bts[1] = (byte)Math.Min(255, (MoreRatio + bts[1]));
                    bts[2] = (byte)Math.Min(255, (MoreRatio + bts[2]));
                    g[yb * width + x] = BitConverter.ToInt32(bts, 0);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void PasteBackground(int[] BackGround, Bitmap bmp, Rectangle RenderingRectangle, int BackGroundLength, out Graphics grp)
        {
            BitmapData bmpdata = bmp.LockBits(RenderingRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            System.Runtime.InteropServices.Marshal.Copy(BackGround, 0, bmpdata.Scan0, BackGroundLength);
            bmp.UnlockBits(bmpdata);
            grp = Graphics.FromImage(bmp);
            grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            grp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            grp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            grp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static void PrepareBackground(Bitmap img, Rectangle RenderingRectangle, ref int BackGroundLength, ref int[] BackGround)
        {
            BitmapData d = img.LockBits(RenderingRectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BackGroundLength = RenderingRectangle.Height * (d.Stride >> 2);
            BackGround = new int[BackGroundLength];
            System.Runtime.InteropServices.Marshal.Copy(d.Scan0, BackGround, 0, BackGroundLength);
            img.UnlockBits(d);
        }

        protected void Finalize(Bitmap bmptmp)
        {
            BitmapData d = bmptmp.LockBits(BmpRectangle, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            int BackGroundLength = BmpRectangle.Height * (d.Stride >> 2);
            int[] BackGround = new int[BackGroundLength];
            System.Runtime.InteropServices.Marshal.Copy(d.Scan0, BackGround, 0, BackGroundLength);
            bmptmp.UnlockBits(d);
            bmptmp.Dispose();
            //
            lock (bmp)
            {
                BitmapData bmpdata = bmp.LockBits(BmpRectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                System.Runtime.InteropServices.Marshal.Copy(BackGround, 0, bmpdata.Scan0, BackGroundLength);
                bmp.UnlockBits(bmpdata);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    bmp.Dispose();
                    //if (GraphicsContext.CurrentContext != null)
                    //    GL.DeleteTexture(texture);
                }

                disposed = true;
            }
        }

        #endregion Private Methods
    }
}