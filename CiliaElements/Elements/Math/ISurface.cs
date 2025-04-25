
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public interface ISurface
    {
        #region Public Properties

        Dictionary<int, Vec2> SurfaceUVCoordinates { get; set; }

        #endregion Public Properties

        #region Public Methods

        int BuildPoint(Vec2 iUV, Vec4 iColor, TCloud iPositions, TCloud iNormals);

        Vec3 GetClosestPointForPointFromSurface(Vec3 iPt, double iAlpha, double iBeta, int i1, int i2, int i3);

        void GetPointAndNormal(Vec2 iUV, ref Vec3 oPt, ref Vec3 oNormal, double iUnit);

        #endregion Public Methods
    }
}