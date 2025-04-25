
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CiliaElements.FormatIGES
{
    public class TIgsElement : TSolidElement
    {
        #region Public Fields

        public static int NBfds = 0;

        public Dictionary<TEntity, int> EntitiesFacesNumber = new Dictionary<TEntity, int>();
        public Dictionary<TEntity, int> EntitiesFacesStart = new Dictionary<TEntity, int>();
        public Dictionary<int, TEntity> GeometricalEntities = new Dictionary<int, TEntity>();

        #endregion Public Fields

        #region Public Constructors

        //iFi As IO.FileInfo, iPartNumber As String, iParentLink As TLink, iNodeName As String, iMatrix As Matrix4d, ifixed As Boolean) ', iMatrix As matrix4)
        public TIgsElement()
            : base()
        {
            //iPartNumber, iParentLink, iNodeName, iMatrix, ifixed) ', iMatrix)
        }

        #endregion Public Constructors

        #region Public Methods

        public override Vec4 GetClosestPointForPointFromSurface(Vec4 iPt)
        {
            Vec3 minV = new Vec3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vec4? BestPt = null;

            //For Each entity As TEntity In GeometricalEntities.Values
            //    Dim hMin As Double = Double.PositiveInfinity
            //    Dim dMin As Double = Double.PositiveInfinity
            //    Dim iMin As Integer = -1
            //    Dim rMin As Vector3d
            //    For I As Integer = entity.FacesOffset To entity.FacesOffset + entity.FacesNumber - 1 Step 3
            //        Dim pt1 As Vector3d = GetPoint(DataIndices(I))
            //        Dim pt2 As Vector3d = GetPoint(DataIndices(I + 1))
            //        Dim pt3 As Vector3d = GetPoint(DataIndices(I + 2))
            //        Dim v0 As Vector3d = pt2 - pt1
            //        Dim v1 As Vector3d = pt3 - pt1
            //        Dim v2 As Vector3d = Vector3d.Cross(v0, v1)
            //        Dim v As Vector3d = iPt.Xyz - pt1
            //        Dim mat As New Matrix3d(v0, v1, v2)
            //        If mat.Determinant <> 0 Then
            //            Try
            //                mat.Invert()
            //            Catch ex As Exception
            //                ex = ex
            //            End Try
            //            Dim r As New Vector3d(Vector3d.Dot(mat.Column0, v), Vector3d.Dot(mat.Column1, v), Vector3d.Dot(mat.Column2, v))
            //            Dim h As Double
            //            Dim d As Double
            //            If r.X < 0 AndAlso r.Y < 0 Then
            //                h = (r.X ^ 2) * v0.LengthSquared + (r.Y ^ 2) * v1.LengthSquared
            //                r.X = 0
            //                r.Y = 0
            //            ElseIf r.X < 0 AndAlso r.Y <= 1 Then
            //                h = (r.X ^ 2) * v0.LengthSquared
            //                r.X = 0
            //            ElseIf r.Y < 0 AndAlso r.X <= 1 Then
            //                h = (r.Y ^ 2) * v1.LengthSquared
            //                r.Y = 0
            //            ElseIf r.Y > r.X + 1 Then
            //                h = (r.X ^ 2) * v0.LengthSquared + ((r.Y - 1) ^ 2) * v1.LengthSquared
            //                r.X = 0
            //                r.Y = 1
            //            ElseIf r.Y < r.X - 1 Then
            //                h = ((r.X - 1) ^ 2) * v0.LengthSquared + (r.Y ^ 2) * v1.LengthSquared
            //                r.X = 1
            //                r.Y = 0
            //            ElseIf r.Y + r.X > 1 Then
            //                Dim rt As New Vector2d
            //                rt.X = (r.X + 1 - r.Y) * 0.5
            //                rt.Y = 1 - rt.X
            //                h = ((r.X - rt.X) ^ 2) * v0.LengthSquared + ((r.Y - rt.Y) ^ 2) * v1.LengthSquared
            //                r.X = rt.X
            //                r.Y = rt.Y
            //            ElseIf r.X > -1 AndAlso r.Y >= 0 AndAlso r.X <= 1 AndAlso r.Y <= 1 AndAlso r.X + r.Y <= 1 Then
            //                h = 0
            //            Else
            //                h = Double.PositiveInfinity
            //            End If
            //            d = (pt1 + r.X * v0 + r.Y * v0 - iPt.Xyz).LengthSquared
            //            If d < dMin Then
            //                iMin = I
            //                dMin = d
            //                hMin = h
            //                rMin = r
            //            ElseIf d = dMin And h < hMin Then
            //                iMin = I
            //                dMin = d
            //                hMin = h
            //                rMin = r
            //            End If
            //        End If
            //    Next
            //    If iMin >= 0 Then
            //        Dim pt As Vector4d = entity.GetClosestPointForPointFromSurface(iPt, rMin.X, rMin.Y, DataIndices(iMin), DataIndices(iMin + 1), DataIndices(iMin + 2)) '  iPt - New vector4d(r.Z * v2, 0)
            //        If Not Double.IsNaN(pt.X) AndAlso Not Double.IsNaN(pt.Y) AndAlso Not Double.IsNaN(pt.Z) Then
            //            If Not BestPt.HasValue Then
            //                BestPt = pt
            //            ElseIf (BestPt.Value - iPt).LengthSquared > (pt - iPt).LengthSquared Then
            //                BestPt = pt
            //            End If
            //        End If
            //    End If
            //Next
            //If Not BestPt.HasValue Then Return Nothing
            return BestPt.Value;
        }

        public override void LaunchLoad()
        {
            //

            if (Fi.Exists)
            {
                //Dim Pgr As KProgress = FrmProgress.Add("Loading IGS " + PartNumber, Color.LightGray)
                //Pgr.SetMinimum(2)

                TTexture texture = SolidElementConstruction.AddTexture(1, 0.5F, 0.5F, 0.5F);

                //
                System.IO.StreamReader reader = Fi.OpenText();
                string Start = "";
                string Global = "";
                string Directory = "";
                //Dim Parameter As String = ""
                string Terminate = "";
                //
                List<string> Prmtrs = new List<string>(new string[1] { "" });
                List<string> Suffixes = new List<string>(new string[1] { "" });
                while (!reader.EndOfStream)
                {
                    string ligne = reader.ReadLine();
                    char letter = ligne.ToCharArray()[72];
                    ligne = ligne.Substring(0, 72);
                    switch (letter)
                    {
                        case 'S':
                            Start += ligne;
                            break;

                        case 'G':
                            Global += ligne;
                            break;

                        case 'D':
                            Directory += ligne;
                            break;

                        case 'P':
                            Prmtrs.Add(ligne.Substring(0, 64).Trim());
                            Suffixes.Add(ligne.Substring(64).Trim());
                            break;

                        case 'T':
                            Terminate += ligne;
                            break;
                    }
                }
                //

                reader.Close();
                reader.Dispose();

                //
                string[] HeaderParameters = Global.Split(new char[2] { ',', ';' }, StringSplitOptions.None);
                switch (int.Parse(HeaderParameters[15], CultureInfo.InvariantCulture))
                {
                    case 1:
                        SolidElementConstruction.Unit = 1000 / 2.54;
                        break;

                    case 2:
                        SolidElementConstruction.Unit = 1000;
                        break;

                    case 3:
                        SolidElementConstruction.Unit = 1;
                        break;

                    case 4:
                        SolidElementConstruction.Unit = 1000 / (12 * 2.54);
                        break;

                    case 5:
                        SolidElementConstruction.Unit = 1000 / (6000 * 12 * 2.54);
                        break;

                    case 6:
                        SolidElementConstruction.Unit = 1;
                        break;

                    case 7:
                        SolidElementConstruction.Unit = 0.001;
                        break;

                    case 8:
                        SolidElementConstruction.Unit = 1000 / 0.00254;
                        break;

                    case 9:
                        SolidElementConstruction.Unit = 1000000;
                        break;

                    case 10:
                        SolidElementConstruction.Unit = 100;
                        break;

                    case 11:
                        SolidElementConstruction.Unit = 1000 / 2.54E-06;
                        break;
                }
                //
                while (!string.IsNullOrEmpty(Directory))
                {
                    TIgsEntity entity = new TIgsEntity(Directory.Substring(0, 144), Prmtrs, Suffixes);
                    entity.Fill(this);
                    Directory = Directory.Substring(144);
                }

                foreach (TEntity entity in GeometricalEntities.Values)
                {
                    if (entity is TTrimmedSurface)
                    {
                        EntitiesFacesStart.Add(entity, SolidElementConstruction.FacesIndexes.Length);
                        entity.PushGeometry();
                        EntitiesFacesNumber.Add(entity, SolidElementConstruction.FacesIndexes.Length - EntitiesFacesStart[entity]);
                        //
                    }
                    else if (entity is TRationalBSplineSurface) { }
                    else if (entity is TRationalBSplineCurve) { }
                    else if (entity is TCompositeCurve) { }
                    else if (entity is TCurveOnParametricSurface) { }
                    else if (entity is TPlane) { }
                    else if (entity is TLine) { }
                    else if (entity is TCircularArc) { }
                    else if (entity is TSurfaceofRevolution) { }
                    else if (entity is TColor)
                    {
                        entity.PushGeometry();
                    }
                    else
                    {
                        throw new Exception("Unmanaged Element!");
                    }
                }
            }

            ElementLoader.Publish();

            NBfds++;
        }

        #endregion Public Methods
    }
}