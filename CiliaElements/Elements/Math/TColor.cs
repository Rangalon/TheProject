
using Math3D;
using OpenTK;

namespace CiliaElements
{
    public class TColor : TEntity
    {
        #region Public Fields

        public Vec4f Value;

        #endregion Public Fields

        #region Public Constructors

        public TColor(TSolidElement iElement)
            : base(iElement)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void PushGeometry()
        {
            TTexture texture = Element.SolidElementConstruction.AddTexture(Value.W, Value.X, Value.Y, Value.Z);
        }

        #endregion Public Methods
    }
}