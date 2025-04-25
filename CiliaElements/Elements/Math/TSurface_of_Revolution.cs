
using Math3D;
using OpenTK;
using System;

namespace CiliaElements
{
    public class TSurfaceofRevolution : TSurface, ISurface
    {
        #region Public Constructors

        public TSurfaceofRevolution(TSolidElement iElement)
            : base(iElement)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void GetPointAndNormal(Vec2 iUV, ref Vec3 oPt, ref Vec3 oNormal, double iUnit)
        {
            throw new NotImplementedException();
        }

        public override void GetPointAndNormal(Vec3 iPt, ref Vec3 oPt, ref Vec3 oNormal, ref Vec2 oUV, double iUnit)
        {
            throw new NotImplementedException();
        }

        public override void PushGeometry()
        {
        }

        #endregion Public Methods
    }
}