using System.Drawing;

namespace JaadWinControls
{
    public enum EZoomMode
    {
        In, Out, All, O
    }

    public class TButton : TCreator
    {
        #region Public Fields

        public string Text;

        #endregion Public Fields

        #region Internal Fields

        internal Bitmap Picture;

        #endregion Internal Fields

        #region Public Constructors

        public TButton()
        {
            Renderer.bmp = new Bitmap(30, 30, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public TButton(int Width, int Height)
        {
            Renderer.bmp = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Update()
        {
            base.Update();
            Renderer.gfx.Clear(TextRenderer.BackColor);
            if (Picture != null)
            {
                Renderer.gfx.DrawImage(Picture, 2, 2, Renderer.bmp.Width - 4, Renderer.bmp.Height - 4);
            }

            Renderer.gfx.DrawRectangle(Pens.White, 0, 0, Renderer.bmp.Width, Renderer.bmp.Height);
            Renderer.gfx.DrawString(Text, TextRenderer.FT6, Brushes.White, 0, 0);
            Renderer.gfx.Dispose();
            Renderer.dirty_region = new Rectangle(0, 0, Renderer.bmp.Width, Renderer.bmp.Height);
        }

        #endregion Public Methods
    }
}