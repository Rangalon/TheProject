
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TRationalBSplineCurve : TCurve, ICurve
    {
       
        #region Public Fields

        public bool Closed;

        public Dictionary<int, double> CurveUCoordinates = new Dictionary<int, double>();

        public int K;

        public double[] KnotSequence;

        public int M;

        public bool Periodic;

        public bool Planar;

        public bool Polynomial;

        public double UMax;

        public double UMin;

        public double[] Weights;

        #endregion Public Fields

        #region Public Constructors

        public TRationalBSplineCurve(int iK, int iM, TSolidElement iElement)
            : base(iElement)
        {
            K = iK;
            M = iM;
            ControlPoints = new TControlPoints(K, iElement);
            KnotSequence = new double[1 + K - M + 2 * M + 1];
            Weights = new double[K + 1];
        }

        #endregion Public Constructors

        #region Public Methods

        public static double BSplineBasisValue(double u, int i, int n, double[] knots)
        {
            if (n == 0)
            {
                if (u < knots[i])
                    return 0;
                if (u > knots[i + n + 1])
                    return 0;
                return 1;
            }
            else
            {
                double f = 0;
                if ((knots[i + n] - knots[i]) > 0)
                {
                    f = (u - knots[i]) / (knots[i + n] - knots[i]);
                }
                double g = 0;
                if ((knots[i + n + 1] - knots[i + 1]) > 0)
                {
                    g = (knots[i + n + 1] - u) / (knots[i + n + 1] - knots[i + 1]);
                }
                return f * BSplineBasisValue(u, i, n - 1, knots) + g * BSplineBasisValue(u, i + 1, n - 1, knots);
            }
        }

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
            int iMin = -1;
            double dMin = double.PositiveInfinity;
            double hMin = double.PositiveInfinity;
            double aMin = 0;
            Vec2 uvMin = new Vec2(0, 0);
            //
            //Dim Pts As New List(Of Vector2d)
            //PushGeometry(Pts)
            //
            Vec2 b1;
            Vec2 b2 = (Vec2)Points[0];
            for (int i = 1; i < Points.Count; i++)
            {
                b1 = b2;
                b2 = (Vec2)Points[i];
                if ((b2 - b1).LengthSquared == 0)
                    continue;
                double a = ((b2.X - b1.X) * (iUV.X - b1.X) + (b2.Y - b1.Y) * (iUV.Y - b1.Y)) / (Math.Pow((b2.X - b1.X), 2) + Math.Pow((b2.Y - b1.Y), 2));
                double h = 0;
                if (a < 0)
                {
                    h = (b2 - b1).LengthSquared * (Math.Pow(a, 2));
                    a = 0;
                }
                else if (a > 1)
                {
                    h = (b2 - b1).LengthSquared * (Math.Pow((a - 1), 2));
                    a = 1;
                }
                else
                {
                    h = 0;
                }
                Vec2 uv = b1 + a * (b2 - b1);
                double d = Math.Pow((iUV.X - uv.X), 2) + Math.Pow((iUV.Y - uv.Y), 2);
                if (d < dMin)
                {
                    iMin = i - 1;
                    dMin = d;
                    uvMin = uv;
                    hMin = h;
                    aMin = a;
                }
                else if (d == dMin & h < hMin)
                {
                    iMin = i - 1;
                    dMin = d;
                    uvMin = uv;
                    hMin = h;
                    aMin = a;
                }
            }
            b1 = (Vec2)ControlPoints.Points[(int)(iMin / FragNumber)];
            b2 = (Vec2)ControlPoints.Points[(int)(iMin / FragNumber) + 1];
            double z = (b2.X - b1.X) * (iUV.Y - b1.Y) - (b2.Y - b1.Y) * (iUV.X - b1.X);
            z /= (b2 - b1).Length;
            z /= Math.Sqrt(Math.Pow((iUV.Y - b1.Y), 2) + Math.Pow((iUV.X - b1.X), 2));
            //
            double u = CurveUCoordinates[iMin] + aMin * (CurveUCoordinates[iMin + 1] - CurveUCoordinates[iMin]);
            //
            Vec2 n = new Vec2();
            Vec2 pt = new Vec2();
            Vec2 nu = new Vec2();
            Vec2 ptu = new Vec2();
            double cp = 0;
            double cpu = 0;
            //
            Vec2 lp;

            //Dim deltaU As Double

            do
            {
                lp = pt;
                GetPointAndNormal(u, ref pt, ref n);
                cp = n.X * (iUV.Y - pt.Y) - n.Y * (iUV.X - pt.X);
                // Vector3d.Cross(n, (iPt.Xyz - pt))
                GetPointAndNormal(u + Epsilon, ref ptu, ref nu);
                if (double.IsNaN(nu.LengthSquared))
                {
                    GetPointAndNormal(u - Epsilon, ref ptu, ref nu);
                    cpu = ((nu.X * (iUV.Y - ptu.Y) - nu.Y * (iUV.X - ptu.X)) - cp) / Epsilon;
                    // Vector3d.Cross(n, (iPt.Xyz - pt))
                }
                else
                {
                    cpu = (cp - (nu.X * (iUV.Y - ptu.Y) - nu.Y * (iUV.X - ptu.X))) / Epsilon;
                    // Vector3d.Cross(n, (iPt.Xyz - pt))
                }
                //
                //deltaU = cp / cpu
                //Do While Math.Abs(deltaU) > (UMax - UMin) / 100
                //    deltaU /= 2
                //Loop
                //If u + deltaU > UMax Then u = UMax : Exit Do
                //If u + deltaU < UMin Then u = UMin : Exit Do
                u += cp / cpu;
                if (u > CurveUCoordinates[iMin + 1])
                {
                    u = CurveUCoordinates[iMin + 1]; iMin++; if (iMin == CurveUCoordinates.Count - 1)
                        break; // TODO: might not be correct. Was : Exit Do
                }
                //+ u - deltaU) * 0.5
                if (u < CurveUCoordinates[iMin])
                {
                    u = CurveUCoordinates[iMin]; iMin--; if (iMin == -1)
                        break; // TODO: might not be correct. Was : Exit Do
                }
                // + u - deltaU) * 0.5
            } while ((lp - pt).LengthSquared > Math.Pow((0.1), (10 * 2)));
            //If Math.Abs(cp) > 0.000001 Then Return Nothing
            GetPointAndNormal(u, ref pt, ref n);
            iNormal = n;

            return pt;
        }

        public override Vec3 GetClosestPointForPointFromCurve(Vec3 iPt, double iAlpha, int i1, int i2)
        {
            double u1 = CurveUCoordinates[i1];
            double u2 = CurveUCoordinates[i2];
            double u = iAlpha * u2 + (1 - iAlpha) * u1;
            return new Vec3();
        }

        public void GetPointAndNormal(double iU, ref Vec2 iPt, ref Vec2 iNormal)
        {
            Vec3 pt = new Vec3();
            double qt = 0;
            double qt2 = 0;
            double qtu1 = 0;
            Vec2 dU1 = new Vec2();
            Vec2 dU2 = new Vec2();

            double[] ci = new double[K + 1];
            double[] cip = new double[K + 1];

            if (Math.Abs(iU - UMin) < Epsilon * 0.01)
                iU = UMin + Epsilon * 0.01;
            if (Math.Abs(iU - UMax) < Epsilon * 0.01)
                iU = UMax - Epsilon * 0.01;

            for (int i = 0; i <= K; i++)
            {
                BSplineBasis(iU, i, M, KnotSequence, ref ci[i], ref cip[i]);
            }

            for (int i = 0; i <= K; i++)
            {
                double r = 0;
                double rp = 0;
                //
                r = Weights[i] * ci[i];
                qt += r;
                qt2 += r * r;
                pt += r * ControlPoints.Points[i];
                //
                rp = Weights[i] * cip[i];
                dU1 += rp * (Vec2)ControlPoints.Points[i];
                dU2 += r * (Vec2)ControlPoints.Points[i];
                qtu1 += rp;
                //
            }

            pt /= qt;
            iPt = (Vec2)pt;
            Vec2 n = dU1 / qt - dU2 * qtu1 / qt2;
            iNormal = new Vec2(n.Y, -n.X);
            iNormal.Normalize();
        }

        public override void Push2DBoundary(List<Vec2> iPts, bool iReverse)
        {
            //If Not Points Is Nothing AndAlso Points.Count > 0 Then
            //    For Each pt As Vector3d In Points
            //        iPts.Add(pt.Xy)
            //    Next
            //    Return
            //End If
            Points = new List<Vec3>();
            for (int j = 0; j <= K * FragNumber; j++)
            {
                double su = UMin + j * (UMax - UMin) / (K * FragNumber);
                if (su > UMax)
                    su = UMax - Epsilon;
                Vec3 pt = new Vec3();
                double qt = 0;
                for (int i = 0; i <= K; i++)
                {
                    double r = Weights[i] * BSplineBasisValue(su, i, M, KnotSequence);
                    qt += r;
                    pt += r * ControlPoints.Points[i];
                }
                pt /= qt;
                if (!CurveUCoordinates.ContainsKey(Points.Count))
                    CurveUCoordinates.Add(Points.Count, su);
                Points.Add(pt);
                iPts.Add((Vec2)pt);
            }
        }

        public override void Push3DBoundary(List<Vec2> iPts, bool iReverse, TSurface iSurface)
        {
            Points = new List<Vec3>();
            for (int j = 0; j <= K * FragNumber; j++)
            {
                double su = UMin + j * (UMax - UMin) / (K * FragNumber);
                if (su > UMax)
                    su = UMax - Epsilon;
                Vec3 pt = new Vec3();
                double qt = 0;
                for (int i = 0; i <= K; i++)
                {
                    double r = Weights[i] * BSplineBasisValue(su, i, M, KnotSequence);
                    qt += r;
                    pt += r * ControlPoints.Points[i];
                }
                pt /= qt;
                if (!CurveUCoordinates.ContainsKey(Points.Count))
                    CurveUCoordinates.Add(Points.Count, su);
                Vec3 v = new Vec3();
                Vec3 n = new Vec3();
                Vec2 uv = new Vec2();
                iSurface.GetPointAndNormal(pt, ref v, ref n, ref uv, 1);
                Points.Add((Vec3)pt);
                iPts.Add(uv);
            }
        }

        public override void PushGeometry()
        {
            //PointsOffset = Element.PointsCount
            //'
            //For j As Integer = 0 To K * FragNumber
            //    Dim su As Double = UMin + j * (UMax - UMin) / (K * FragNumber)
            //    If su > UMax Then su = UMax - Epsilon
            //    Dim pt As New Vector3d
            //    Dim qt As Double = 0
            //    For i As Integer = 0 To K
            //        Dim r As Double = Weights[i) * BSplineBasisValue(su, i, M, KnotSequence)
            //        qt += r
            //        pt += r * ControlPoints.Points[i)
            //    Next
            //    pt /= qt
            //    Element.AddPointAndNormal(pt / Element.Unit, New Vector3d(10, 10, 10), Element.LastColor, False)
            //    If j < K * FragNumber Then
            //        'iElement.LinesIndexes.Add(iElement.Points.Count - 1)
            //        'iElement.LinesIndexes.Add(iElement.Points.Count)
            //    End If
            //Next
        }

        #endregion Public Methods
    }
}