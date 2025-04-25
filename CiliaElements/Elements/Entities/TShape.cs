
using Math3D;

namespace CiliaElements
{
    public struct TShapeParameters
    {
        #region Public Fields

        public TShapeGroup Group;
        public Mtx4 Matrix;

        #endregion Public Fields
    }

    public class TShape
    {
        #region Public Fields

        public SBoundingBox3 BoundingBox;
        public TEntity Entity;
        public int IndexesEnd;
        public int IndexesStart;
        public int PositionsStart;
        public TShapeParameters ShapeParameters;

        #endregion Public Fields

        //public TSolidElement Solid;

        #region Public Methods

        public int OffsetIndice(int i)
        {
            return i + PositionsStart;
        }

        #endregion Public Methods

        //public bool IsVisible(Mtx4 iMatrix)
        //{
        //    TBoundingBox box = TBoundingBox.Transform(BoundingBox, iMatrix);
        //    //
        //    box = box.Get2DBox(TViewManager.BaseLayer.PMatrix);
        //    Vec4 v1 = box.MinPosition;
        //    Vec4 v2 = box.MaxPosition;
        //    return (v1.X < 1 && v1.Y < 1 && v2.X > -1 && v2.Y > -1 && v2.X - v1.X > 2*TViewManager.WidthCulling && v2.Y - v1.Y >2* TViewManager.HeightCulling);
        //}
    }
}