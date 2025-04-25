
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TLine : TCurve, ICurve
    {
        #region Public Fields

        public double N;

        public Vec3 P1 = new Vec3();

        public Vec3 P2 = new Vec3();

        public Vec2 U = new Vec2();

        public Vec2 uvP1;

        public Vec2 uvP2;

        public Vec2 V = new Vec2();

        #endregion Public Fields

        #region Public Constructors

        public TLine(TSolidElement iElement)
            : base(iElement)
        {
            ControlPoints = new TControlPoints(1, iElement);
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
            double r = N * Vec2.Dot((iUV - uvP1), U);
            if (r < 0) r = 0; else if (r > 1) r = 1;
            Vec2 uv = uvP1 + r * U / N;
            iNormal = V;
            return uv;
        }

        public override void Push2DBoundary(List<Vec2> iPts, bool iReverse)
        {
            Points = new List<Vec3>();
            Points.Add((Vec3)P1);
            iPts.Add((Vec2)P1);
            Points.Add((Vec3)P2);
            iPts.Add((Vec2)P2);
        }

        public override void Push3DBoundary(List<Vec2> iPts, bool iReverse, TSurface iSurface)
        {
            Points = new List<Vec3>();
            Vec3 pt = new Vec3();
            Vec3 n = new Vec3();
            iSurface.GetPointAndNormal(P1, ref pt, ref n, ref uvP1, 1);
            Points.Add((Vec3)pt);
            iPts.Add(uvP1);
            iSurface.GetPointAndNormal(P2, ref pt, ref n, ref uvP2, 1);
            Points.Add((Vec3)pt);
            iPts.Add(uvP2);
            U = (Vec2)(uvP2 - uvP1);
            N = 1 / U.Length;
            U *= N;
            V.X = U.Y;
            V.Y = -U.X;
        }

        public override void PushGeometry()
        {
        }

        #endregion Public Methods
    }
}