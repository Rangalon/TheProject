using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace JaadWinControls
{
    public abstract class TCreator : ICreator, ITip
    {
        #region Public Fields

        public DoDelegate Do;
        public RectangleF Rect;
        public TextRenderer Renderer = new TextRenderer();

        #endregion Public Fields

        #region Private Fields

        private Pen NotStartedPen = new Pen(Color.LightGray, 2);
        private Pen StartedPen = new Pen(Color.LightGreen, 2);

        #endregion Private Fields

        #region Public Constructors

        public TCreator()
        {
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void DoDelegate(TCreator creator);

        #endregion Public Delegates

        #region Public Properties

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual KJaadControl Control { get; set; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual Rectangle Rct { get; set; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual bool Started { get { return false; } }

        public string Tip { get; set; }
        public virtual bool Visible { get; set; }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Draw(Graphics grp)
        {
            //Point[] path=new Point[] { new Point(Rct.Left, Rct.Top), new Point(Rct.Right, Rct.Top), new Point(Rct.Right, Rct.Bottom), new Point(Rct.Left, Rct.Bottom) };
            //grp.TranslateTransform(3, 3);
            //grp.FillClosedCurve(ExeStationScan.ShadowBrush, path,System.Drawing.Drawing2D.FillMode.Winding,0.35F);
            //grp.TranslateTransform(-3, -3);
            //grp.FillClosedCurve(ExeStationScan.BtnBackBrush, path, System.Drawing.Drawing2D.FillMode.Winding, 0.35F);
            //if (Started)
            //    grp.DrawClosedCurve(StartedPen, path, 0.35F, System.Drawing.Drawing2D.FillMode.Winding);
            //else
            //    grp.DrawClosedCurve(NotStartedPen, path, 0.35F, System.Drawing.Drawing2D.FillMode.Winding);
            //grp.DrawRectangle(Pens.LightGray, Rct);
        }

        public virtual void FinalizeUpdate()
        {
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public virtual void Stop() { }

        public virtual void Update()
        {
            if (Renderer.Texture == 0)
            {
                //gfx = Graphics.FromImage(bmp);
                //gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                Renderer.Texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, Renderer.Texture);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Renderer.bmp.Width, Renderer.bmp.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

                Rect.Width = -Renderer.bmp.Width * Rect.Height / Renderer.bmp.Height;
            }
            Renderer.gfx = Graphics.FromImage(Renderer.bmp);
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}