
using Math3D;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TCircularArc : TCurve, ICurve
    {
        #region Public Constructors

        public TCircularArc(TSolidElement iElement)
            : base(iElement)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override bool Check2DGoodSide(Vec2 iUV)
        {
            throw new NotImplementedException();
        }

        public override bool Check3DGoodSide(Vec2 iUV, TSurface iSurface)
        {
            throw new NotImplementedException();
        }

        public override Vec2? GetClosest2DPointAndNormalFor2DPointFromCurve(Vec2 iUV, ref Vec2 iNormal)
        {
            throw new NotImplementedException();
        }

        public override void Push2DBoundary(List<Vec2> iPts, bool iReverse)
        {
            throw new NotImplementedException();
        }

        public override void PushGeometry()
        {
        }

        #endregion Public Methods
    }
}