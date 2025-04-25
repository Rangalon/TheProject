using System.Drawing;

namespace JaadWinControls
{
    public class TLabel : TCreator
    {
        #region Public Fields

        public bool Checked;
        public string Text;

        #endregion Public Fields

        #region Private Fields

        private static StringFormat StrfNN = new StringFormat() { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };

        #endregion Private Fields

        #region Public Constructors

        public TLabel(int rows, int columns)
        {
            Renderer.bmp = new Bitmap(columns * 10, rows * 20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Update()
        {
            base.Update();

            if (!Checked)
            {
                Renderer.gfx.Clear(TextRenderer.BackColor);
            }
            else
            {
                Renderer.gfx.Clear(Color.DarkGreen);
            }

            Renderer.gfx.DrawRectangle(Pens.White, 0, 0, Renderer.bmp.Width, Renderer.bmp.Height);
            Renderer.gfx.DrawString(Text, TextRenderer.FT6, Brushes.White, new Rectangle(0, 0, Renderer.bmp.Width, Renderer.bmp.Height), StrfNN);

            Renderer.gfx.Dispose();
            Renderer.dirty_region = new Rectangle(0, 0, Renderer.bmp.Width, Renderer.bmp.Height);
        }

        #endregion Public Methods
    }
}