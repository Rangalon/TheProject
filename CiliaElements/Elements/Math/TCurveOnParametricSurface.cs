
using Math3D;
using OpenTK;
using System;

namespace CiliaElements
{
    public class TCurveOnParametricSurface : TCurve, ICurve
    {
    
        #region Public Fields

        public ICurve CurveEntity;
        public ICurve DefinitionEntity;
        public ERepresentation Representation;
        public ISurface SurfaceEntity;

        public EType Type;

        #endregion Public Fields

        #region Public Constructors

        public TCurveOnParametricSurface(EType iType, ERepresentation iRepresentation, TSolidElement iElement)
            : base(iElement)
        {
            Type = iType;
            Representation = iRepresentation;
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

        public override void PushGeometry()
        {
            //Dim pts As New List(Of Vector2d)
            //CurveEntity.PushBoundary(pts, False)
            //'CurveEntity.PushGeometry(iElement)
            //PointsOffset = Element.PointsCount
            //For Each ptuv As Vector2d In pts
            //    Dim pt As Vector3d
            //    Dim n As Vector3d
            //    SurfaceEntity.GetPointAndNormal(ptuv, pt, n, Element.Unit)
            //    Element.AddPointAndNormal(pt, New Vector3d, TBaseElement.BlackColor, False)
            //Next
            //For i = PointsOffset To Element.PointsCount - 2
            //    Element.LinesIndexes.Add(i)
            //    Element.LinesIndexes.Add(i + 1)
            //Next
        }

        #endregion Public Methods
    }
}