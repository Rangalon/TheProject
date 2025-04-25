
using Math3D;
using OpenTK;
using System;

namespace CiliaElements
{
    public class TControlPoints : TEntity
    {
       
        #region Public Fields

        public Vec3[] Points;

        #endregion Public Fields

        #region Public Constructors

        public TControlPoints(int iK, TSolidElement iElement)
            : base(iElement)
        {
            Points = new Vec3[iK + 1];
        }

        #endregion Public Constructors

        #region Public Methods

        public override void PushGeometry()
        {
        }

        #endregion Public Methods

    }
}