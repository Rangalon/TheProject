
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public interface ICurve
    {
        #region Public Properties

        TControlPoints ControlPoints { get; set; }

        List<Vec3> Points { get; set; }

        #endregion Public Properties

        #region Public Methods

        bool Check2DGoodSide(Vec2 iUV);

        bool Check3DGoodSide(Vec2 iUV, TSurface iSurface);

        Vec2? GetClosest2DPointAndNormalFor2DPointFromCurve(Vec2 iUV, ref Vec2 iNormal);

        void Push2DBoundary(List<Vec2> iPts, bool iReverse);

        void Push3DBoundary(List<Vec2> iPts, bool iReverse, TSurface iSurface);

        #endregion Public Methods
    }
}