
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TCompositeCurve : TCurve, ICurve
    {
      
        #region Public Fields

        public bool Direct = true;
        public ICurve[] Entities;

        public int Nb;

        #endregion Public Fields

        #region Public Constructors

        public TCompositeCurve(int iNb, TSolidElement iElement)
            : base(iElement)
        {
            Nb = iNb - 1;
            Entities = new ICurve[Nb + 1];
        }

        #endregion Public Constructors

        #region Public Methods

        public override bool Check2DGoodSide(Vec2 iUV)
        {
            Vec2? BestPt = null;
            Vec2 BestN = new Vec2();
            double hMin = double.MaxValue;
            double dMin = double.MaxValue;

            foreach (ICurve curve in Entities)
            {
                Vec2 n = new Vec2();
                Vec2? pt = curve.GetClosest2DPointAndNormalFor2DPointFromCurve(iUV, ref n);
                if (pt.HasValue)
                {
                    double a = Math.Pow(((iUV.X - pt.Value.X) * n.X + (iUV.Y - pt.Value.Y) * n.Y), 2);
                    double d = (iUV - pt.Value).LengthSquared;
                    double h = d - a;
                    double r = Math.Round((d - dMin) / dMin, 3);
                    if (r < 0 | (r == 0 & h < hMin))
                    {
                        BestPt = pt;
                        BestN = n;
                        hMin = h;
                        dMin = d;
                    }
                }
            }

            BestPt -= iUV;
            //Return False
            double nn = BestPt.Value.X * BestN.X + BestPt.Value.Y * BestN.Y;
            if ((nn > 0) == Direct)
            {
            }
            return (BestPt.Value.X * BestN.X + BestPt.Value.Y * BestN.Y > 0) == Direct;
        }

        public override bool Check3DGoodSide(Vec2 iUV, TSurface iSurface)
        {
            Vec2? BestPt = null;
            Vec2 BestN = new Vec2();

            //foreach (ICurve curve in Entities)
            //{
            //    //Vec2 n = new Vec2();
            //    //Nullable<Vec2> pt = curve.GetClosest2DPointAndNormalFor2DPointFromCurve(iUV, ref n, Surface);
            //    //if (pt.HasValue)
            //    //{
            //    //    Double a = Math.Pow(((iUV.X - pt.Value.X) * n.X + (iUV.Y - pt.Value.Y) * n.Y), 2);
            //    //    Double d = (iUV - pt.Value).LengthSquared;
            //    //    Double h = d - a;
            //    //    Double r = Math.Round((d - dMin) / dMin, 3);
            //    //    if (r < 0 | (r == 0 & h < hMin))
            //    //    {
            //    //        BestPt = pt;
            //    //        BestN = n;
            //    //        hMin = h;
            //    //        dMin = d;
            //    //    }
            //    //}
            //}

            BestPt -= iUV;
            //Return False
            double nn = BestPt.Value.X * BestN.X + BestPt.Value.Y * BestN.Y;
            if ((nn > 0) == Direct)
            {
            }
            return (BestPt.Value.X * BestN.X + BestPt.Value.Y * BestN.Y > 0) == Direct;
        }

        public bool CheckDirection(bool iReverse)
        {
            //Dim pts As New List(Of Vector3d)

            Vec2 uvMax = new Vec2(double.MinValue, double.MinValue);
            Vec2 uvMin = new Vec2(double.MaxValue, double.MaxValue);

            foreach (ICurve curve in Entities)
            {
                foreach (Vec3 pt in curve.ControlPoints.Points)
                {
                    //pts.Add(pt)
                    if (pt.X > uvMax.X)
                        uvMax.X = pt.X;
                    if (pt.Y > uvMax.Y)
                        uvMax.Y = pt.Y;
                    if (pt.X < uvMin.X)
                        uvMin.X = pt.X;
                    if (pt.Y < uvMin.Y)
                        uvMin.Y = pt.Y;
                }
            }

            uvMax += (uvMax - uvMin) * 0.1;

            //Dim r As Vector2d = uvMax - uvMin
            //r.X = 400 / r.X
            //r.Y = 400 / r.Y
            //'If r.X < r.Y Then r.Y = r.X Else r.X = r.Y

            //Dim bmp As New Bitmap(440, 440)
            //Dim grp As Drawing.Graphics = Drawing.Graphics.FromImage(bmp)
            //grp.Clear(Drawing.Color.FromArgb(Element.LastColor.X * 255, Element.LastColor.Y * 255, Element.LastColor.Z * 255, 255))
            //grp.TranslateTransform(20, 20)
            //Dim cpt As Integer = 0

            //grp.FillEllipse(Brushes.Green, CSng((uvMax.X - uvMin.X) * r.X) - 8, CSng((uvMax.Y - uvMin.Y) * r.Y) - 8, 16, 16)

            //For Each curve As TRationalBSplineCurve In Entities
            //    Dim LastPt As Nullable(Of Vector2d) = Nothing
            //    For Each pt As Vector3d In curve.ControlPoints.Points
            //        Dim pt2 As Vector2d = (pt.Xy - uvMin)
            //        pt2.X *= r.X
            //        pt2.Y *= r.Y
            //        If LastPt.HasValue Then
            //            grp.DrawLine(Pens.Black, CSng(LastPt.Value.X), CSng(LastPt.Value.Y), CSng(pt2.X), CSng(pt2.Y))
            //            grp.DrawString(cpt.ToString, FMain.DefaultFont, Brushes.Blue, CSng(pt2.X) + 5, CSng(pt2.Y) + 5)
            //        End If
            //        grp.DrawEllipse(Pens.Red, CSng(pt2.X) - 2, CSng(pt2.Y) - 2, 4, 4)
            //        LastPt = pt2
            //        cpt += 1
            //    Next
            //Next

            Direct = Check2DGoodSide(uvMax);
            if (iReverse)
                Direct = !Direct;

            //grp.DrawString(Direct.ToString, FMain.DefaultFont, Brushes.Black, -20, -20)
            //grp.Dispose()

            //bmp.Save("C:\Temp\IGES\" + Compteur.ToString + ".bmp")
            //bmp.Dispose()

            //Id = Compteur
            //Compteur += 1

            return Direct;
        }

        public override Vec2? GetClosest2DPointAndNormalFor2DPointFromCurve(Vec2 iUV, ref Vec2 iNormal)
        {
            throw new NotImplementedException();
        }

        public override void Push2DBoundary(List<Vec2> iPts, bool iReverse)
        {
            Points = new List<Vec3>();
            foreach (ICurve curve in Entities)
            {
                curve.Push2DBoundary(iPts, false);
                Points.AddRange(curve.Points);
            }
            if (CheckDirection(iReverse))
            {
                iPts.Reverse();
                Points.Reverse();
            }
        }

        public override void Push3DBoundary(List<Vec2> iPts, bool iReverse, TSurface iSurface)
        {
            Points = new List<Vec3>();
            foreach (ICurve curve in Entities)
            {
                curve.Push3DBoundary(iPts, false, iSurface);
                Points.AddRange(curve.Points);
            }
            if (CheckDirection(iReverse))
            {
                iPts.Reverse();
                Points.Reverse();
            }
        }

        public override void PushGeometry()
        {
            foreach (TEntity curve in Entities)
            {
                curve.PushGeometry();
            }
        }

        #endregion Public Methods
    }
}