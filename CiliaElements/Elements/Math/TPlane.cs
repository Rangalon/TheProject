
using Math3D;
using Math3D;
using OpenTK;
using System;

namespace CiliaElements
{
    public class TPlane : TSurface, ISurface
    {
        #region Public Fields

        public Vec3 O = new Vec3();

        public Vec3 U = new Vec3();

        public Vec3 V = new Vec3();

        public Vec3 W = new Vec3();

        #endregion Public Fields

        #region Public Constructors

        public TPlane(TSolidElement iElement)
            : base(iElement)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void GetPointAndNormal(Vec2 iUV, ref Vec3 oPt, ref Vec3 oNormal, double iUnit)
        {
            oNormal = U;
            oPt = (O + iUV.X * V + iUV.Y * W) / iUnit;
        }

        public override void GetPointAndNormal(Vec3 iPt, ref Vec3 oPt, ref Vec3 oNormal, ref Vec2 oUV, double iUnit)
        {
            //iPt -= (Vec3)O;
            double n = U.X * (iPt.X - O.X) + U.Y * (iPt.Y - O.Y) + U.Z * (iPt.Z - O.Z);
            if (n >= 0) oNormal = U; else oNormal = -U;
            oPt = (iPt - n * U) / iUnit;
            oUV.X = Vec3.Dot(iPt, V);
            oUV.Y = Vec3.Dot(iPt, W);
        }

        public override void PushGeometry()
        {
        }

        #endregion Public Methods
    }
}