
using Math3D;
using OpenTK;
using System;

namespace CiliaElements
{
    public class TRationalBSplineSurface : TSurface, ISurface
    {
      
        #region Public Fields

        public const double RdmFactor = 2;
        public static Vec2 dU = new Vec2(Epsilon, 0);

        public static Vec2 dV = new Vec2(0, Epsilon);

        public TControlPoints[] ControlPoints;
        public bool FirstDirectionClosed;
        public bool FirstDirectionPeriodic;
        public double[] FirstKnotSequence;
        public int K1;
        public int K2;
        public int M1;
        public int M2;
        public bool Polynomial;
        public bool SecondDirectionClosed;
        public bool SecondDirectionPeriodic;
        public double[] SecondKnotSequence;

        public double[,] Weights;

        #endregion Public Fields

        #region Public Constructors

        public TRationalBSplineSurface(int iK1, int iK2, int iM1, int iM2, TSolidElement iElement)
            : base(iElement)
        {
            K1 = iK1;
            K2 = iK2;
            M1 = iM1;
            M2 = iM2;
            ControlPoints = new TControlPoints[K2 + 1];
            for (int i = 0; i <= K2; i++)
            {
                ControlPoints[i] = new TControlPoints(K1, iElement);
            }
            FirstKnotSequence = new double[1 + K1 - M1 + 2 * M1 + 1];
            SecondKnotSequence = new double[1 + K2 - M2 + 2 * M2 + 1];
            Weights = new double[K1 + 1, K2 + 1];
        }

        #endregion Public Constructors

        #region Public Methods

        public override Vec3 GetClosestPointForPointFromSurface(Vec3 iPt, double iAlpha, double iBeta, int i1, int i2, int i3)
        {
            //
            Vec2 suv1 = SurfaceUVCoordinates[i1];
            Vec2 suv2 = SurfaceUVCoordinates[i2];
            Vec2 suv3 = SurfaceUVCoordinates[i3];
            Vec2 uv = iAlpha * suv2 + iBeta * suv3 + (1 - iAlpha - iBeta) * suv1;
            //
            Vec3 pt = default(Vec3);
            Vec3 n = default(Vec3);
            Vec3 ptu = default(Vec3);
            Vec3 ptv = default(Vec3);
            Vec3 cp = default(Vec3);
            //
            Vec3 lp = default(Vec3);

            do
            {
                lp = pt;
                GetPointAndNormal(uv, ref pt, ref n, Element.SolidElementConstruction.Unit);
                cp = Vec3.Cross(n, iPt - pt);
                GetPointAndNormal(uv + dU, ref ptu, ref n, Element.SolidElementConstruction.Unit);
                if (double.IsNaN(n.LengthSquared))
                {
                    GetPointAndNormal(uv - dU, ref ptu, ref n, Element.SolidElementConstruction.Unit);
                    ptu = -(cp - Vec3.Cross(n, iPt - ptu)) / Epsilon;
                }
                else
                {
                    ptu = (cp - Vec3.Cross(n, iPt - ptu)) / Epsilon;
                }

                GetPointAndNormal(uv + dV, ref ptv, ref n, Element.SolidElementConstruction.Unit);
                if (double.IsNaN(n.LengthSquared))
                {
                    GetPointAndNormal(uv - dV, ref ptv, ref n, Element.SolidElementConstruction.Unit);
                    ptv = -(cp - Vec3.Cross(n, iPt - ptv)) / Epsilon;
                }
                else
                {
                    ptv = (cp - Vec3.Cross(n, iPt - ptv)) / Epsilon;
                }

                Mtx2 mt2 = new Mtx2(Vec3.Dot(ptu, ptu), Vec3.Dot(ptu, ptv), Vec3.Dot(ptv, ptu), Vec3.Dot(ptv, ptv));
                double d = mt2.Determinant;
                //If d < MinimumMatrixDeterminant Then Exit Do
                if (double.IsNaN(d))
                    break; // TODO: might not be correct. Was : Exit Do
                //If mt2.Determinant < MinimumMatrixDeterminant Then Exit Do
                try
                {
                    mt2.Invert();
                }
                catch 
                {
                }
                Vec2 v2 = new Vec2(Vec3.Dot(ptu, cp), Vec3.Dot(ptv, cp));
                double deltaU = Vec2.Dot(mt2.Row0, v2);
                double deltaV = Vec2.Dot(mt2.Row1, v2);
                if (uv.X + deltaU > UMax)
                    break; // TODO: might not be correct. Was : Exit Do
                if (uv.X + deltaU < UMin)
                    break; // TODO: might not be correct. Was : Exit Do
                if (uv.Y + deltaV > VMax)
                    break; // TODO: might not be correct. Was : Exit Do
                if (uv.Y + deltaV < VMin)
                    break; // TODO: might not be correct. Was : Exit Do
                uv.X += deltaU;
                // * 0.5
                uv.Y += deltaV;
                // * 0.5
            } while ((lp - pt).LengthSquared > Math.Pow((1E-11), 2));
            if (cp.LengthSquared > 1E-06)
                return new Vec3();
            return new Vec3(pt.X, pt.Y, pt.Z);
        }

        public override void GetPointAndNormal(Vec2 iUV, ref Vec3 oPt, ref Vec3 oNormal, double iUnit)
        {
            double qt = 0;
            double qt2 = 0;
            Vec3 pt = new Vec3();
            Vec3 du1 = new Vec3();
            Vec3 du2 = new Vec3();
            double qtu1 = 0;
            Vec3 dv1 = new Vec3();
            Vec3 dv2 = new Vec3();
            double qtv1 = 0;
            //
            double[] ci = new double[K1 + 1];
            double[] cip = new double[K1 + 1];
            double[] cj = new double[K2 + 1];
            double[] cjp = new double[K2 + 1];
            //
            if (Math.Abs(iUV.X - UMin) < Epsilon * 0.01)
                iUV.X = UMin + Epsilon * 0.01;
            if (Math.Abs(iUV.Y - VMin) < Epsilon * 0.01)
                iUV.Y = VMin + Epsilon * 0.01;
            if (Math.Abs(iUV.X - UMax) < Epsilon * 0.01)
                iUV.X = UMax - Epsilon * 0.01;
            if (Math.Abs(iUV.Y - VMax) < Epsilon * 0.01)
                iUV.Y = VMax - Epsilon * 0.01;
            //
            for (int i = 0; i <= K1; i++)
            {
                BSplineBasis(iUV.X, i, M1, FirstKnotSequence, ref ci[i], ref cip[i]);
            }
            for (int j = 0; j <= K2; j++)
            {
                BSplineBasis(iUV.Y, j, M2, SecondKnotSequence, ref cj[j], ref cjp[j]);
            }
            //
            for (int i = 0; i <= K1; i++)
            {
                for (int j = 0; j <= K2; j++)
                {
                    double r = 0;
                    double rp = 0;
                    //
                    r = Weights[i, j] * ci[i] * cj[j];
                    qt += r;
                    qt2 += r * r;
                    pt += r * ControlPoints[j].Points[i];
                    //
                    rp = Weights[i, j] * cip[i] * cj[j];
                    du1 += rp * ControlPoints[j].Points[i];
                    du2 += r * ControlPoints[j].Points[i];
                    qtu1 += rp;
                    //
                    rp = Weights[i, j] * ci[i] * cjp[j];
                    dv1 += rp * ControlPoints[j].Points[i];
                    dv2 += r * ControlPoints[j].Points[i];
                    qtv1 += rp;
                }
            }
            //
            oNormal = Vec3.Cross(du1 / qt - du2 * qtu1 / qt2, dv1 / qt - dv2 * qtv1 / qt2);
            //oNormal += New Vector3d(2 - Rnd(), 2 - Rnd(), 2 - Rnd()) * RdmFactor
            if (oNormal.LengthSquared > 0)
            {
                oNormal.Normalize();
            }

            oPt = pt / (qt * iUnit);
        }

        public override void GetPointAndNormal(Vec3 iPt, ref Vec3 oPt, ref Vec3 oNormal, ref Vec2 oUV, double iUnit)
        {
            throw new NotImplementedException();
        }

        public override void PushGeometry()
        {
            //
            //PointsOffset = Element.PointsCount
            //Dim uv As Vector2d
            //For i As Integer = 0 To K1 * FragNumber
            //    uv.X = UMin + i * (UMax - UMin) / (K1 * FragNumber)
            //    For j As Integer = 0 To K2 * FragNumber
            //        uv.Y = VMin + j * (VMax - VMin) / (K2 * FragNumber)
            //        Dim pt As Vector3d
            //        Dim n As Vector3d
            //        GetPointAndNormal(uv, pt, n, Element.Unit)
            //        Element.AddPointAndNormal(pt, n, Element.LastColor, False)
            //    Next
            //Next
            //For i As Integer = 0 To (K1 * FragNumber) - 1
            //    For j As Integer = 0 To (K2 * FragNumber) - 1
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j)
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + 1)
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1))
            //        ''
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + 1)
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1))
            //        'iElement.FacesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1) + 1)
            //        '
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j)
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + 1)
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1))
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j)
            //        '
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + 1)
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1))
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + ((K2 * FragNumber) + 1) + 1)
            //        Element.LinesIndexes.Add(PointsOffset + i * ((K2 * FragNumber) + 1) + j + 1)
            //    Next
            //Next
        }

        #endregion Public Methods
    }
}