
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CiliaElements.FormatIGES
{
    public class TIgsEntity
    {
        #region Public Fields

        public string Line;

        public int Parameter_Data;

        public int Parameter_Line_Count;

        public string[] Parameters = { };

        public EIGSEntity Type;

        public int DirectoryEntry;

        #endregion Public Fields

        #region Public Constructors

        public TIgsEntity(string iLine, List<string> prmtrs, List<string> suffixes)
        {
            int j;
            int.TryParse(iLine.Substring(0, 8), out j);
            Type = (EIGSEntity)j;
            int.TryParse(iLine.Substring(8, 8), out Parameter_Data);
            int.TryParse(iLine.Substring(96, 8), out Parameter_Line_Count);
            Line = iLine;
            //
            DirectoryEntry = int.Parse(suffixes[Parameter_Data], CultureInfo.InvariantCulture);
            //
            for (int i = 0; i < Parameter_Line_Count; i++)
            {
                string[] tbl = prmtrs[i + Parameter_Data].Split(new char[3] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
                int n = Parameters.Length;
                Array.Resize(ref Parameters, n + tbl.Length);
                Array.Copy(tbl, 0, Parameters, n, tbl.Length);
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public static void FillEntity(TEntity iEntity, string iLine)
        {
            int.TryParse(iLine.Substring(16, 8), out iEntity.Structure);
            int.TryParse(iLine.Substring(24, 8), out  iEntity.Line_Font_Pattern);
            int.TryParse(iLine.Substring(32, 8), out  iEntity.Level);
            int.TryParse(iLine.Substring(40, 8), out  iEntity.View);
            int.TryParse(iLine.Substring(48, 8), out  iEntity.Transformation_Matrix);
            int.TryParse(iLine.Substring(56, 8), out  iEntity.Label_Display_Associativity);
            int.TryParse(iLine.Substring(64, 8), out  iEntity.Status);
            int.TryParse(iLine.Substring(80, 8), out  iEntity.Line_Weight);
            int.TryParse(iLine.Substring(88, 8), out  iEntity.Color);
            int.TryParse(iLine.Substring(104, 8), out  iEntity.Form);
            int.TryParse(iLine.Substring(112, 8), out  iEntity.Reserved1);
            int.TryParse(iLine.Substring(120, 8), out  iEntity.Reserved2);
            iEntity.Entity_Label = iLine.Substring(128, 8);
            int.TryParse(iLine.Substring(136, 8), out  iEntity.Entity_Subscript);
        }

        public void Fill(TIgsElement iElement)
        {
            switch (Type)
            {
                case EIGSEntity.RationalBSplineSurface:
                    Fill_Rational_BSpline_Surface(iElement);
                    break;

                case EIGSEntity.ColorDefinition:
                    Fill_Color(iElement);
                    break;

                case EIGSEntity.RationalBSplineCurve:
                    Fill_Rational_BSpline_Curve(iElement);
                    break;

                case EIGSEntity.TrimmedSurface:
                    Fill_Trimmed_Surface(iElement);
                    break;

                case EIGSEntity.CompositeCurve:
                    Fill_Composite_Curve(iElement);
                    break;

                case EIGSEntity.CurveOnAParametricSurface:
                    Fill_Curve_on_a_Parametric_Surface(iElement);
                    break;

                case EIGSEntity.Line:
                    Fill_Line(iElement);
                    break;

                case EIGSEntity.Plane:
                    Fill_Plane(iElement);
                    break;

                case EIGSEntity.TransformationMatrix:
                    break;

                case EIGSEntity.CircularArc:
                    Fill_Circular_Arc(iElement);
                    break;

                case EIGSEntity.SurfaceOfRevolution:
                    Fill_Surface_of_Revolution(iElement);
                    break;

                case EIGSEntity.Point:
                case EIGSEntity.Weird:
                    break;

                default:
                    throw new Exception("Unmanaged type!");
            }
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        private void Fill_Circular_Arc(TIgsElement iElement)
        {
            TCircularArc Curve = new TCircularArc(iElement);
            iElement.GeometricalEntities.Add(DirectoryEntry, Curve);
        }

        private void Fill_Color(TIgsElement iElement)
        {
            TColor color = new TColor(iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, color);
            //
            color.Value = new Vec4f(float.Parse(this.Parameters[1], CultureInfo.InvariantCulture) / 100, float.Parse(this.Parameters[2], CultureInfo.InvariantCulture) / 100, float.Parse(this.Parameters[3], CultureInfo.InvariantCulture) / 100, 1);
        }

        private void Fill_Composite_Curve(TIgsElement iElement)
        {
            TCompositeCurve Curve = new TCompositeCurve(int.Parse(Parameters[1], CultureInfo.InvariantCulture), iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, Curve);
            //
            for (int i = 0; i <= Curve.Nb; i++)
            {
                Curve.Entities[i] = (ICurve)iElement.GeometricalEntities[int.Parse(Parameters[i + 2], CultureInfo.InvariantCulture)];
            }
        }

        private void Fill_Curve_on_a_Parametric_Surface(TIgsElement iElement)
        {
            TCurveOnParametricSurface curve = new TCurveOnParametricSurface((EType)int.Parse(Parameters[1], CultureInfo.InvariantCulture), (ERepresentation)int.Parse(Parameters[5], CultureInfo.InvariantCulture), iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, curve);
            //
            curve.SurfaceEntity = (ISurface)iElement.GeometricalEntities[int.Parse(Parameters[2], CultureInfo.InvariantCulture)];
            if (iElement.GeometricalEntities.ContainsKey(int.Parse(Parameters[3], CultureInfo.InvariantCulture))) curve.CurveEntity = (TCompositeCurve)iElement.GeometricalEntities[int.Parse(Parameters[3], CultureInfo.InvariantCulture)];
            curve.DefinitionEntity = (ICurve)iElement.GeometricalEntities[int.Parse(Parameters[4], CultureInfo.InvariantCulture)];
        }

        private void Fill_Line(TIgsElement iElement)
        {
            TLine Curve = new TLine(iElement);
            Curve.P1.X = double.Parse(Parameters[1], CultureInfo.InvariantCulture);
            Curve.P1.Y = double.Parse(Parameters[2], CultureInfo.InvariantCulture);
            Curve.P1.Z = double.Parse(Parameters[3], CultureInfo.InvariantCulture);
            Curve.P2.X = double.Parse(Parameters[4], CultureInfo.InvariantCulture);
            Curve.P2.Y = double.Parse(Parameters[5], CultureInfo.InvariantCulture);
            Curve.P2.Z = double.Parse(Parameters[6], CultureInfo.InvariantCulture);
            Curve.ControlPoints.Points[0] = (Vec3)Curve.P1;
            Curve.ControlPoints.Points[1] = (Vec3)Curve.P2;
            iElement.GeometricalEntities.Add(DirectoryEntry, Curve);
        }

        private void Fill_Plane(TIgsElement iElement)
        {
            TPlane surface = new TPlane(iElement);
            surface.U.X = double.Parse(Parameters[1], CultureInfo.InvariantCulture);
            surface.U.Y = double.Parse(Parameters[2], CultureInfo.InvariantCulture);
            surface.U.Z = double.Parse(Parameters[3], CultureInfo.InvariantCulture);
            surface.O = surface.U * double.Parse(Parameters[4], CultureInfo.InvariantCulture);
            surface.V.X = +surface.U.Y * surface.U.Y - surface.U.X * surface.U.Z;
            surface.V.Y = -surface.U.Z * surface.U.Z - surface.U.Y * surface.U.X;
            surface.V.Z = +surface.U.X * surface.U.X + surface.U.Z * surface.U.Y;
            surface.V.Normalize();
            surface.W.X = surface.U.Y * surface.V.Z - surface.U.Z * surface.V.Y;
            surface.W.Y = surface.U.Z * surface.V.X - surface.U.X * surface.V.Z;
            surface.W.Z = surface.U.X * surface.V.Y - surface.U.Y * surface.V.X;
            //double n;
            //n = Vec4.Dot(surface.U, surface.V);
            //n = Vec4.Dot(surface.V, surface.W);
            //n = Vec4.Dot(surface.W, surface.U);
            iElement.GeometricalEntities.Add(DirectoryEntry, surface);
        }

        private void Fill_Rational_BSpline_Curve(TIgsElement iElement)
        {
            TRationalBSplineCurve Curve = new TRationalBSplineCurve(int.Parse(Parameters[1], CultureInfo.InvariantCulture), int.Parse(Parameters[2], CultureInfo.InvariantCulture), iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, Curve);
            //
            Curve.Planar = (double.Parse(Parameters[3], CultureInfo.InvariantCulture) == 1);
            Curve.Closed = (double.Parse(Parameters[4], CultureInfo.InvariantCulture) == 1);
            Curve.Polynomial = (double.Parse(Parameters[5], CultureInfo.InvariantCulture) == 1);
            Curve.Periodic = (double.Parse(Parameters[6], CultureInfo.InvariantCulture) == 1);
            //
            for (int i = 0; i < 2 + Curve.K - Curve.M + 2 * Curve.M; i++)
            {
                Curve.KnotSequence[i] = double.Parse(Parameters[7 + i], CultureInfo.InvariantCulture);
            }
            //
            for (int i = 0; i <= Curve.K; i++)
            {
                Curve.Weights[i] = double.Parse(Parameters[8 + 1 + Curve.K - Curve.M + 2 * Curve.M + i], CultureInfo.InvariantCulture);
            }
            //
            for (int i = 0; i <= Curve.K; i++)
            {
                Curve.ControlPoints.Points[i].X = double.Parse(Parameters[9 + 1 + Curve.K - Curve.M + 2 * Curve.M + Curve.K + i * 3 + 0], CultureInfo.InvariantCulture);
                Curve.ControlPoints.Points[i].Y = double.Parse(Parameters[9 + 1 + Curve.K - Curve.M + 2 * Curve.M + Curve.K + i * 3 + 1], CultureInfo.InvariantCulture);
                Curve.ControlPoints.Points[i].Z = double.Parse(Parameters[9 + 1 + Curve.K - Curve.M + 2 * Curve.M + Curve.K + i * 3 + 2], CultureInfo.InvariantCulture);
            }
            //
            Curve.UMin = double.Parse(Parameters[12 + 1 + Curve.K - Curve.M + 2 * Curve.M + 4 * Curve.K], CultureInfo.InvariantCulture);
            Curve.UMax = double.Parse(Parameters[13 + 1 + Curve.K - Curve.M + 2 * Curve.M + 4 * Curve.K], CultureInfo.InvariantCulture);
        }

        private void Fill_Rational_BSpline_Surface(TIgsElement iElement)
        {
            //
            TRationalBSplineSurface Surface = new TRationalBSplineSurface(int.Parse(Parameters[1], CultureInfo.InvariantCulture), int.Parse(Parameters[2], CultureInfo.InvariantCulture), int.Parse(Parameters[3], CultureInfo.InvariantCulture), int.Parse(Parameters[4], CultureInfo.InvariantCulture), iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, Surface);
            //
            Surface.FirstDirectionClosed = (double.Parse(Parameters[5], CultureInfo.InvariantCulture) == 1);
            Surface.SecondDirectionClosed = (double.Parse(Parameters[6], CultureInfo.InvariantCulture) == 1);
            Surface.Polynomial = (double.Parse(Parameters[7], CultureInfo.InvariantCulture) == 1);
            Surface.FirstDirectionPeriodic = (double.Parse(Parameters[8], CultureInfo.InvariantCulture) == 1);
            Surface.SecondDirectionPeriodic = (double.Parse(Parameters[9], CultureInfo.InvariantCulture) == 1);
            //
            for (int I = 0; I < 2 + Surface.K1 - Surface.M1 + 2 * Surface.M1; I++)
            {
                Surface.FirstKnotSequence[I] = double.Parse(Parameters[10 + I], CultureInfo.InvariantCulture);
            }
            for (int I = 0; I < 2 + Surface.K2 - Surface.M2 + 2 * Surface.M2; I++)
            {
                Surface.SecondKnotSequence[I] = double.Parse(Parameters[11 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + I], CultureInfo.InvariantCulture);
            }
            for (int i = 0; i <= Surface.K1; i++)
            {
                for (int j = 0; j <= Surface.K2; j++)
                {
                    Surface.Weights[i, j] = double.Parse(Parameters[12 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + i + (Surface.K1 + 1) * j], CultureInfo.InvariantCulture);
                    Vec3 pt = new Vec3
                    {
                        X = double.Parse(Parameters[12 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + (1 + Surface.K1) * (1 + Surface.K2) + 0 + 3 * i + 3 * (Surface.K1 + 1) * j], CultureInfo.InvariantCulture),
                        Y = double.Parse(Parameters[12 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + (1 + Surface.K1) * (1 + Surface.K2) + 1 + 3 * i + 3 * (Surface.K1 + 1) * j], CultureInfo.InvariantCulture),
                        Z = double.Parse(Parameters[12 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + (1 + Surface.K1) * (1 + Surface.K2) + 2 + 3 * i + 3 * (Surface.K1 + 1) * j], CultureInfo.InvariantCulture)
                    };
                    Surface.ControlPoints[j].Points[i] = pt;
                }
            }
            Surface.UMin = double.Parse(Parameters[12 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + 4 * (1 + Surface.K1) * (1 + Surface.K2)], CultureInfo.InvariantCulture);
            Surface.UMax = double.Parse(Parameters[13 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + 4 * (1 + Surface.K1) * (1 + Surface.K2)], CultureInfo.InvariantCulture);
            Surface.VMin = double.Parse(Parameters[14 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + 4 * (1 + Surface.K1) * (1 + Surface.K2)], CultureInfo.InvariantCulture);
            Surface.VMax = double.Parse(Parameters[15 + 1 + Surface.K1 - Surface.M1 + 2 * Surface.M1 + 1 + Surface.K2 - Surface.M2 + 2 * Surface.M2 + 4 * (1 + Surface.K1) * (1 + Surface.K2)], CultureInfo.InvariantCulture);
        }

        private void Fill_Surface_of_Revolution(TIgsElement iElement)
        {
            TSurfaceofRevolution surface = new TSurfaceofRevolution(iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, surface);
        }

        private void Fill_Trimmed_Surface(TIgsElement iElement)
        {
            TTrimmedSurface surface = new TTrimmedSurface(int.Parse(Parameters[3], CultureInfo.InvariantCulture), iElement);
            //
            iElement.GeometricalEntities.Add(DirectoryEntry, surface);
            //
            surface.Surface = (ISurface)iElement.GeometricalEntities[int.Parse(Parameters[1], CultureInfo.InvariantCulture)];
            surface.OuterBoundary = (TCurveOnParametricSurface)iElement.GeometricalEntities[int.Parse(Parameters[4], CultureInfo.InvariantCulture)];
            //
            for (int i = 0; i <= surface.Nb; i++)
            {
                surface.InnerBoundaries[i] = (TCurveOnParametricSurface)iElement.GeometricalEntities[int.Parse(Parameters[5 + i], CultureInfo.InvariantCulture)];
            }
        }

        #endregion Private Methods
    }
}