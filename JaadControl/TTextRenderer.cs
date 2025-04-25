using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace JaadWinControls
{
    public class TextRenderer : IDisposable
    {
        #region Public Fields

        public static Color BackColor = Color.FromArgb(50, 50, 50);
        public static Font FT6 = new Font("Verdana", 12);
        public static Font FT8 = new Font("Verdana", 16);
        public Bitmap bmp;
        public Rectangle dirty_region;
        public bool disposed;
        public Graphics gfx;

        #endregion Public Fields

        #region Private Fields

        private int texture;

        #endregion Private Fields

        #region Public Constructors

        public TextRenderer()
        {
        }

        #endregion Public Constructors

        #region Private Destructors

        ~TextRenderer()
        {
            Console.WriteLine("[Warning] Resource leaked: {0}.", typeof(TextRenderer));
        }

        #endregion Private Destructors

        #region Public Properties

        /// <summary>
        /// Gets a <see cref="System.Int32"/> that represents an OpenGL 2d texture handle.
        /// The texture contains a copy of the backing store. Bind this texture to TextureTarget.Texture2d
        /// in order to render the drawn text on screen.
        /// </summary>
        public int Texture
        {
            get
            {
                UploadBitmap();
                return texture;
            }
            set { texture = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Public Methods

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

        // Uploads the dirty regions of the backing store to the OpenGL texture.
        private void UploadBitmap()
        {
            if (dirty_region != RectangleF.Empty)
            {
                System.Drawing.Imaging.BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0,
                    0, 0, bmp.Width, bmp.Height,
                    PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

                bmp.UnlockBits(data);

                dirty_region = Rectangle.Empty;
            }
        }

        #endregion Private Methods
    }
}