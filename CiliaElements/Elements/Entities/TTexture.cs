
using Math3D;

namespace CiliaElements
{
    public class TTexture
    {
        #region Public Fields

        public float A;
        public float B;
        public float G;
        public System.Drawing.Bitmap KDBitmap;
        public System.Drawing.Bitmap KEBitmap;
        public double One;
        public float R;
        public double Zero;

        #endregion Public Fields

        #region Public Methods

        public Vec2f ConvertVec2f(Vec2 v)
        {
            float x = (float)v.X * 0.5F;
            float y = (float)(Zero + One - One * v.Y);
            return new Vec2f(x, y);
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3}", A, R, G, B);
        }

        #endregion Public Methods
    }
}