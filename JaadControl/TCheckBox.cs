using System;
using System.Drawing;

namespace JaadWinControls
{
    public class TCheckBox : TCreator
    {
        #region Public Fields

        public bool Checked;
        public string Text;

        #endregion Public Fields

        #region Public Constructors

        public TCheckBox()
        {
            Renderer.bmp = new Bitmap(80, 20, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Do = Switch;
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void CheckChangedEventHandler(TCheckBox c, System.EventArgs e);

        #endregion Public Delegates

        #region Public Events

        public event CheckChangedEventHandler CheckChanged;

        #endregion Public Events

        #region Public Methods

        public void Switch(TCreator c)
        {
            Checked = !Checked;
            CheckChanged?.Invoke(this, new EventArgs());
            Update();
        }

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
            Renderer.gfx.DrawString(Text, TextRenderer.FT6, Brushes.White, 0, 0);

            Renderer.gfx.Dispose();
            Renderer.dirty_region = new Rectangle(0, 0, Renderer.bmp.Width, Renderer.bmp.Height);
        }

        #endregion Public Methods
    }
}