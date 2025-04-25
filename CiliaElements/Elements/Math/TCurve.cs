
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]

namespace CiliaElements
{
    public abstract class TCurve : TEntity, ICurve
    {
        #region Public Constructors

        public TCurve(TSolidElement iElement)
            : base(iElement)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        public TControlPoints ControlPoints { get; set; }
        public List<Vec3> Points { get; set; }

        #endregion Public Properties

        #region Public Methods

        public abstract bool Check2DGoodSide(Vec2 iUV);

        public abstract bool Check3DGoodSide(Vec2 iUV, TSurface iSurface);

        public abstract Vec2? GetClosest2DPointAndNormalFor2DPointFromCurve(Vec2 iUV, ref Vec2 iNormal);

        public virtual void Push2DBoundary(List<Vec2> iPts, bool iReverse)
        {
            throw new NotImplementedException();
        }

        public virtual void Push3DBoundary(List<Vec2> iPts, bool iReverse, TSurface iSurface)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}