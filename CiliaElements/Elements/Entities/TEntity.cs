
using Math3D;
using System;

namespace CiliaElements
{
    public abstract class TEntity
    {
        #region Public Fields

        public const double Epsilon = 0.001;
        public const int FragNumber = 1;
        public static double MinimumMatrixDeterminant = Math.Pow(0.1, 8);
        public static double MinimumUVDistance = Math.Pow(0.1, 2);

        public static double MinimumZDistance = Math.Pow(0.1, 8);

        public  TSolidElement Element;

        public int Color;

        public string Entity_Label;

        public int Entity_Subscript;

        public int Form;

        public int Label_Display_Associativity;

        public int Level;

        public int Line_Font_Pattern;

        public int Line_Weight;

        public int Reserved1;

        public int Reserved2;

        public int Status;

        public int Structure;

        public int Transformation_Matrix;

        public int View;

        #endregion Public Fields

        #region Public Constructors

        public TEntity(TSolidElement iElement)
        {
            Element = iElement;
        }

        #endregion Public Constructors

        #region Public Methods

        public static void BSplineBasis(double u, int i, int n, double[] knots, ref double oValue, ref double oDerivate)
        {
            if (n == 0)
            {
                if (u < knots[i])
                {
                    oValue = 0;
                }
                else if (u > knots[i + n + 1])
                {
                    oValue = 0;
                }
                else
                {
                    oValue = 1;
                }
                oDerivate = 0;
            }
            else
            {
                double f = 0;
                double fp = 0;
                if ((knots[i + n] - knots[i]) > 0)
                {
                    f = (u - knots[i]) / (knots[i + n] - knots[i]);
                    fp = 1 / (knots[i + n] - knots[i]);
                }
                double g = 0;
                double gp = 0;
                if ((knots[i + n + 1] - knots[i + 1]) > 0)
                {
                    g = (knots[i + n + 1] - u) / (knots[i + n + 1] - knots[i + 1]);
                    gp = -1 / (knots[i + n + 1] - knots[i + 1]);
                }
                double nf = 0;
                double nfp = 0;
                double ng = 0;
                double ngp = 0;
                BSplineBasis(u, i, n - 1, knots, ref nf, ref nfp);
                BSplineBasis(u, i + 1, n - 1, knots, ref ng, ref ngp);
                oValue = f * nf + g * ng;
                //If n = 1 Then
                //oDerivate = f * nfp + g * ngp + fp * nf + gp * ng
                //If i > 0 AndAlso u >= knots[i - n) AndAlso u < knots[i) Then
                //    oDerivate = 1
                //ElseIf u > knots[i) AndAlso u < knots[i + n + 1) Then
                //    oDerivate = -1
                //Else
                //    oDerivate = 0
                //End If
                //Else
                oDerivate = f * nfp + g * ngp + fp * nf + gp * ng;
                //End If
            }
        }

        public virtual Vec3 CheckRay(Mtx4 RayMatrix, int I1, int I2, int I3, Vec3 p)
        {
            return p;
        }

        public virtual Vec3 GetClosestPointForPointFromCurve(Vec3 iPt, double iAlpha, int i1, int i2)
        {
            return new Vec3();
        }

        public virtual Vec3 GetClosestPointForPointFromSurface(Vec3 iPt, double iAlpha, double iBeta, int i1, int i2, int i3)
        {
            return new Vec3();
        }

        public abstract void PushGeometry();

        #endregion Public Methods

        #region Private Methods

        private void WriteStructureText(string iText, Mtx4 iMtx, Vec3 iVx)
        {
            //Dim tablelignes() As String = iText.Split(Chr(10))
            //Dim h As Double = (tablelignes.Length * LetterHeigth) * 0.5
            //For J As Integer = 0 To tablelignes.GetUpperBound(0)
            //    iText = tablelignes(J).ToUpper
            //    Dim l As Double = (iText.Length * LetterWidth) * 0.5
            //    For i As Integer = 0 To iText.Length - 1
            //        Dim c As Char = iText.Substring(i, 1)
            //        If Not LettersDico.ContainsKey(c) Then
            //            c = c
            //        End If
            //        Dim table() As SLine = LettersDico(c)
            //        For Each line As SLine In table
            //            Element.AddPointAndNormal(Vector4d.Transform(line.StartPoint + New Vector4d(0, l, h, 0), iMtx).Xyz, New Vector3d, TBaseElement.BlackColor, False)
            //            Element.AddPointAndNormal(Vector4d.Transform(line.EndPoint + New Vector4d(0, l, h, 0), iMtx).Xyz, New Vector3d, TBaseElement.BlackColor, False)
            //            Element.LinesIndexes.Add(Element.PointsCount - 2)
            //            Element.LinesIndexes.Add(Element.PointsCount - 1)
            //        Next
            //        l -= LetterWidth
            //    Next
            //    h -= LetterHeigth
            //Next
        }

        #endregion Private Methods
    }
}