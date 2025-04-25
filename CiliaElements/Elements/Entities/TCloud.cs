
using Math3D;

namespace CiliaElements
{
    public class TCloud
    {
        #region Public Fields

        public int?[] Indexes;
        public TQuickStc<Vec3> Vectors = new TQuickStc<Vec3>();

        #endregion Public Fields
    }

    public class TTextureCloud
    {
        #region Public Fields

        public int?[] Indexes;
        public TQuickStc<Vec2> Vectors = new TQuickStc<Vec2>();

        #endregion Public Fields
    }
}