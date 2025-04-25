
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TTrimmedSurface : TSurface, ISurface
    {
        #region Public Fields

        public TCurveOnParametricSurface[] InnerBoundaries;
        public int Nb;
        public TCurveOnParametricSurface OuterBoundary;
        public int Prof;
        public ISurface Surface;

        #endregion Public Fields

        #region Private Fields

        private List<Vec2>[] Boundaries;
        private List<Vec2> BoundariesPoints = new List<Vec2>();
        private TFGroup Group;
        private List<int> Indexes = new List<int>();
        private TCloud Normals;
        private TCloud Positions;
        private Dictionary<Vec2, int> Splits = new Dictionary<Vec2, int>();
        private TTexture Texture;

        #endregion Private Fields

        #region Public Constructors

        public TTrimmedSurface(int iNb, TSolidElement iElement)
            : base(iElement)
        {
            Nb = iNb - 1;
            InnerBoundaries = new TCurveOnParametricSurface[Nb + 1];
        }

        #endregion Public Constructors

        #region Public Methods

        public override Vec3 CheckRay(Mtx4 RayMatrix, int I1, int I2, int I3, Vec3 p)
        {
            Vec3 p1 = new Vec3();
            Vec3 p2 = new Vec3();
            Vec3 p3 = new Vec3();
            Vec3 n = new Vec3();
            Surface.GetPointAndNormal(Surface.SurfaceUVCoordinates[I1], ref p1, ref n, 1000);
            Surface.GetPointAndNormal(Surface.SurfaceUVCoordinates[I2], ref p2, ref n, 1000);
            Surface.GetPointAndNormal(Surface.SurfaceUVCoordinates[I3], ref p3, ref n, 1000);
            p2 -= p1; p3 -= p1; p -= p1;

            Vec2 r = new Vec2
            {
                X = Vec3.Dot(p, p2),
                Y = Vec3.Dot(p, p3)
            };
            Mtx2 m = new Mtx2();
            m.Row0.X = Vec3.Dot(p2, p2);
            m.Row0.Y = Vec3.Dot(p2, p3);
            m.Row1.X = Vec3.Dot(p2, p3);
            m.Row1.Y = Vec3.Dot(p3, p3);
            p2 += p1;
            p3 += p1;
            p += p1;
            Mtx2 mi = new Mtx2();
            double d = m.Row0.X * m.Row1.Y - m.Row1.X * m.Row0.Y;
            d = 1 / d;
            mi.Row0.X = m.Row1.Y * d;
            mi.Row0.Y = -m.Row0.Y * d;
            mi.Row1.X = -m.Row1.X * d;
            mi.Row1.Y = m.Row0.X * d;
            //Mtx2 mm = m * mi;
            double a = Vec2.Dot(mi.Row0, r);
            double b = Vec2.Dot(mi.Row1, r);
            Vec2 uv = Surface.SurfaceUVCoordinates[I1] * (1 - a - b) + a * Surface.SurfaceUVCoordinates[I2] + b * Surface.SurfaceUVCoordinates[I3];
            Vec3 p4 = new Vec3();
            Vec3 l = new Vec3(1, 1, 1);
            while (l.LengthSquared > 1e-12)
            {
                Vec3 p5 = new Vec3();
                Surface.GetPointAndNormal(uv, ref p4, ref n, 1000);
                a = (p4.X - RayMatrix.Column1.X) * RayMatrix.Column0.X + (p4.Y - RayMatrix.Column1.Y) * RayMatrix.Column0.Y + (p4.Z - RayMatrix.Column1.Z) * RayMatrix.Column0.Z;
                b = RayMatrix.Column0.LengthSquared;
                a /= b;
                p5.X = RayMatrix.Column1.X + a * RayMatrix.Column0.X;
                p5.Y = RayMatrix.Column1.Y + a * RayMatrix.Column0.Y;
                p5.Z = RayMatrix.Column1.Z + a * RayMatrix.Column0.Z;
                l = (p5 - p4);
                //
                uv.X += 0.001;
                Surface.GetPointAndNormal(uv, ref p4, ref n, 1000);
                a = (p4.X - RayMatrix.Column1.X) * RayMatrix.Column0.X + (p4.Y - RayMatrix.Column1.Y) * RayMatrix.Column0.Y + (p4.Z - RayMatrix.Column1.Z) * RayMatrix.Column0.Z;
                b = RayMatrix.Column0.LengthSquared;
                a /= b;
                p5.X = RayMatrix.Column1.X + a * RayMatrix.Column0.X;
                p5.Y = RayMatrix.Column1.Y + a * RayMatrix.Column0.Y;
                p5.Z = RayMatrix.Column1.Z + a * RayMatrix.Column0.Z;
                uv.X -= 0.001;
                Vec3 lx = (p5 - p4);
                //
                uv.Y += 0.001;
                Surface.GetPointAndNormal(uv, ref p4, ref n, 1000);
                a = (p4.X - RayMatrix.Column1.X) * RayMatrix.Column0.X + (p4.Y - RayMatrix.Column1.Y) * RayMatrix.Column0.Y + (p4.Z - RayMatrix.Column1.Z) * RayMatrix.Column0.Z;
                b = RayMatrix.Column0.LengthSquared;
                a /= b;
                p5.X = RayMatrix.Column1.X + a * RayMatrix.Column0.X;
                p5.Y = RayMatrix.Column1.Y + a * RayMatrix.Column0.Y;
                p5.Z = RayMatrix.Column1.Z + a * RayMatrix.Column0.Z;
                uv.Y -= 0.001;
                Vec3 ly = (p5 - p4);
                //
                lx = (lx - l) / 0.001;
                ly = (ly - l) / 0.001;
                //
                r = new Vec2
                {
                    X = Vec3.Dot(l, lx),
                    Y = Vec3.Dot(l, ly)
                };
                m = new Mtx2();
                m.Row0.X = Vec3.Dot(lx, lx);
                m.Row0.Y = Vec3.Dot(lx, ly);
                m.Row1.X = Vec3.Dot(lx, ly);
                m.Row1.Y = Vec3.Dot(ly, ly);
                mi = new Mtx2();
                d = m.Row0.X * m.Row1.Y - m.Row1.X * m.Row0.Y;
                d = 1 / d;
                mi.Row0.X = m.Row1.Y * d;
                mi.Row0.Y = -m.Row0.Y * d;
                mi.Row1.X = -m.Row1.X * d;
                mi.Row1.Y = m.Row0.X * d;
                a = Vec2.Dot(mi.Row0, r);
                b = Vec2.Dot(mi.Row1, r);
                //
                uv.X -= 0.2 * a;
                uv.Y -= 0.2 * b;
            }
            if (l.LengthSquared > 1e-12) throw new Exception("satisfying point not found");
            return p4;
        }

        public override Vec3 GetClosestPointForPointFromSurface(Vec3 iPt, double iAlpha, double iBeta, int i1, int i2, int i3)
        {
            return Surface.GetClosestPointForPointFromSurface(iPt, iAlpha, iBeta, i1, i2, i3);
        }

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
            //If Nb <> 1 Then Return
            //If Nb >= 0 Then Nb = -1
            //Element.LastColor = New Vector4(Rnd, Rnd, Rnd, 1)
            Texture = Element.SolidElementConstruction.Textures.Values[Element.SolidElementConstruction.Textures.Max - 1];
            Positions = Element.SolidElementConstruction.AddCloud();
            Normals = Element.SolidElementConstruction.AddCloud();
            Group = Element.SolidElementConstruction.AddFGroup();
            Group.ShapeGroupParameters.Positions = Positions;
            Group.ShapeGroupParameters.Normals = Normals;
            Group.GroupParameters.Texture = Texture;
            //Group.TrimmedSurface = Me
            Element.SolidElementConstruction.StartGroup.FGroups.Push(Group);

            Boundaries = new List<Vec2>[Nb + 2];
            for (int i = 0; i < Nb + 2; i++)
                Boundaries[i] = new List<Vec2>();
            //
            //
            //if (OuterBoundary.CurveEntity == null) OuterBoundary.CurveEntity = OuterBoundary.DefinitionEntity;
            //
            if (Surface is TRationalBSplineSurface)
            {
                OuterBoundary.CurveEntity.Push2DBoundary(Boundaries[0], false);
                BoundariesPoints.AddRange(Boundaries[0]);
                for (int i = 1; i < Nb + 2; i++)
                {
                    InnerBoundaries[i - 1].CurveEntity.Push2DBoundary(Boundaries[i], true);
                    BoundariesPoints.AddRange(Boundaries[i]);
                }

                TRationalBSplineSurface s = (TRationalBSplineSurface)Surface;
                Vec2 suv;
                for (int i = 0; i <= s.K1 * FragNumber; i++)
                {
                    suv.X = s.UMin + i * (s.UMax - s.UMin) / (s.K1 * FragNumber);
                    for (int j = 0; j <= s.K2 * FragNumber; j++)
                    {
                        suv.Y = s.VMin + j * (s.VMax - s.VMin) / (s.K2 * FragNumber);
                        s.BuildPoint(suv, Element.SolidElementConstruction.LastColor, Positions, Normals);
                    }
                }
                //
                for (int i = 0; i < (s.K1 * FragNumber); i++)
                {
                    for (int j = 0; j < (s.K2 * FragNumber); j++)
                    {
                        GenerateFragment(i * ((s.K2 * FragNumber) + 1) + j, i * ((s.K2 * FragNumber) + 1) + j + 1, i * ((s.K2 * FragNumber) + 1) + j + ((s.K2 * FragNumber) + 1));
                        GenerateFragment(i * ((s.K2 * FragNumber) + 1) + j + 1, i * ((s.K2 * FragNumber) + 1) + j + ((s.K2 * FragNumber) + 1), i * ((s.K2 * FragNumber) + 1) + j + ((s.K2 * FragNumber) + 1) + 1);
                    }
                }
                //
            }
            else if (Surface is TPlane)
            {
                TPlane s = (TPlane)Surface;
                OuterBoundary.DefinitionEntity.Push3DBoundary(Boundaries[0], false, s);
                BoundariesPoints.AddRange(Boundaries[0]);
                for (int i = 1; i < Nb + 2; i++)
                {
                    InnerBoundaries[i - 1].DefinitionEntity.Push3DBoundary(Boundaries[i], true, s);
                    BoundariesPoints.AddRange(Boundaries[i]);
                }

                foreach (Vec2 v in BoundariesPoints)
                {
                    if (s.UMax < v.X) s.UMax = v.X;
                    if (s.UMin > v.X) s.UMin = v.X;
                    if (s.VMax < v.Y) s.VMax = v.Y;
                    if (s.VMin > v.Y) s.VMin = v.Y;
                }
                s.UMax *= 2;
                s.UMin *= 2;
                s.VMax *= 2;
                s.VMin *= 2;
                s.BuildPoint(new Vec2(s.UMax, s.VMax), Element.SolidElementConstruction.LastColor, Positions, Normals);
                s.BuildPoint(new Vec2(s.UMax, s.VMin), Element.SolidElementConstruction.LastColor, Positions, Normals);
                s.BuildPoint(new Vec2(s.UMin, s.VMax), Element.SolidElementConstruction.LastColor, Positions, Normals);
                s.BuildPoint(new Vec2(s.UMin, s.VMin), Element.SolidElementConstruction.LastColor, Positions, Normals);
                GenerateFragment(0, 1, 2);
                GenerateFragment(1, 3, 2);
                //for (int i = 0; i < OuterBoundary.DefinitionEntity.Points.Count; i++)
                //{
                //    s.BuildPoint(OuterBoundary.DefinitionEntity.Points[i], Element.LastColor, Positions, Normals);
                //}
                //
                //for(int i=OuterBoundary.DefinitionEntity.Points.Count-1;i>0;i--)
                //    if (OuterBoundary.DefinitionEntity.Points[i] == OuterBoundary.DefinitionEntity.Points[i - 1])
                //    {
                //        s.SurfaceUVCoordinates.Remove(i);
                //        OuterBoundary.DefinitionEntity.Points.RemoveAt(i);
                //    }
                //int i1 = 0;
                //int i2 = OuterBoundary.DefinitionEntity.Points.Count - 2;
                //int i3 = i1;
                //int i4 = i2;
                //while (i2 > i1 + 2)
                //{
                //    while (OuterBoundary.DefinitionEntity.Points[i1] == OuterBoundary.DefinitionEntity.Points[i3]) i3++;
                //    while (OuterBoundary.DefinitionEntity.Points[i2] == OuterBoundary.DefinitionEntity.Points[i4]) i4--;

                //    PublishFragment(i1, i2, i3);
                //    PublishFragment(i2, i4, i3);
                //    i1 = i3; i2 = i4;
                //}
                //for (int i = 2; i < OuterBoundary.DefinitionEntity.Points.Count; i++)
                //    PublishFragment(0, i - 1, i);
            }
            //
            TCurveOnParametricSurface[] AllBoundaries = new TCurveOnParametricSurface[1 + InnerBoundaries.Length];
            AllBoundaries[0] = OuterBoundary;
            Array.Copy(InnerBoundaries, 0, AllBoundaries, 1, InnerBoundaries.Length);
            for (int j = Indexes.Count - 3; j > -1; j += -3)
            {
                Vec2 suv = Surface.SurfaceUVCoordinates[Indexes[j]];
                suv += Surface.SurfaceUVCoordinates[Indexes[j + 1]];
                suv += Surface.SurfaceUVCoordinates[Indexes[j + 2]];
                suv /= 3;
                foreach (TCurveOnParametricSurface entity in AllBoundaries)
                {
                    //if (entity.CurveEntity == null) entity.CurveEntity = entity.DefinitionEntity;
                    if (entity.CurveEntity != null && entity.CurveEntity.Check2DGoodSide(suv))
                    {
                        Indexes.RemoveAt(j);
                        Indexes.RemoveAt(j);
                        Indexes.RemoveAt(j);
                        break; // TODO: might not be correct. Was : Exit For
                    }
                    //else if (entity.DefinitionEntity != null && entity.DefinitionEntity.Check3DGoodSide(suv,Surface))
                    //{
                    //    Indexes.RemoveAt(j);
                    //    Indexes.RemoveAt(j);
                    //    Indexes.RemoveAt(j);
                    //    break; // TODO: might not be correct. Was : Exit For
                    //}
                }
            }
            //
            Group.Indexes.Push(Indexes.ToArray());
            Group.ShapeGroupParameters.Entity = this;
        }

        public int Split(int iToSplit, int iFromSplit)
        {
            Vec2 key = new Vec2(iToSplit, iFromSplit);
            if (Splits.ContainsKey(key))
                return Splits[key];
            Vec2 suv1 = Surface.SurfaceUVCoordinates[iToSplit];
            Vec2 suv2 = Surface.SurfaceUVCoordinates[iFromSplit];
            if ((suv2 - suv1).LengthSquared < MinimumUVDistance)
                return -1;
            foreach (List<Vec2> BoundaryCoordinate in Boundaries)
            {
                Vec2 buv1;
                Vec2 buv2 = BoundaryCoordinate[0];
                for (int i = 1; i < BoundaryCoordinate.Count; i++)
                {
                    buv1 = buv2;
                    buv2 = BoundaryCoordinate[i];
                    Mtx2 mt = new Mtx2(buv2.X - buv1.X, suv1.X - suv2.X, buv2.Y - buv1.Y, suv1.Y - suv2.Y);
                    double d = mt.Determinant;
                    if (Math.Abs(d) > MinimumMatrixDeterminant)
                    {
                        double a = (mt.Row1.Y * (suv1.X - buv1.X) - mt.Row0.Y * (suv1.Y - buv1.Y)) / d;
                        double b = (-mt.Row1.X * (suv1.X - buv1.X) + mt.Row0.X * (suv1.Y - buv1.Y)) / d;
                        if (Math.Abs(a - 0.5) <= 0.5 && Math.Abs(b - 0.5) < 0.5)
                        {
                            Vec2 buv = buv1 + a * (buv2 - buv1);
                            if ((buv - suv1).LengthSquared < MinimumUVDistance | (buv - suv2).LengthSquared < MinimumUVDistance)
                                continue;
                            int iNew = Surface.BuildPoint(buv, Element.SolidElementConstruction.LastColor, Positions, Normals);
                            Splits.Add(key, iNew);
                            return iNew;
                        }
                    }
                }
            }
            Splits.Add(key, -1);
            return -1;
        }

        #endregion Public Methods

        #region Private Methods

        private void GenerateFragment(int i1, int i2, int i3)
        {
            //
            int ni1 = Split(i1, i2);
            int ni2 = Split(i2, i3);
            int ni3 = Split(i3, i1);
            //
            if (ni1 > -1)
            {
                if ((Surface.SurfaceUVCoordinates[ni1] - Surface.SurfaceUVCoordinates[i1]).LengthSquared < MinimumUVDistance)
                    ni1 = -1;
                if ((Surface.SurfaceUVCoordinates[ni1] - Surface.SurfaceUVCoordinates[i2]).LengthSquared < MinimumUVDistance)
                    ni1 = -1;
                if ((Surface.SurfaceUVCoordinates[ni1] - Surface.SurfaceUVCoordinates[i3]).LengthSquared < MinimumUVDistance)
                    ni1 = -1;
            }
            if (ni2 > -1)
            {
                if ((Surface.SurfaceUVCoordinates[ni2] - Surface.SurfaceUVCoordinates[i2]).LengthSquared < MinimumUVDistance)
                    ni2 = -1;
                if ((Surface.SurfaceUVCoordinates[ni2] - Surface.SurfaceUVCoordinates[i3]).LengthSquared < MinimumUVDistance)
                    ni2 = -1;
                if ((Surface.SurfaceUVCoordinates[ni2] - Surface.SurfaceUVCoordinates[i1]).LengthSquared < MinimumUVDistance)
                    ni2 = -1;
            }
            if (ni3 > -1)
            {
                if ((Surface.SurfaceUVCoordinates[ni3] - Surface.SurfaceUVCoordinates[i3]).LengthSquared < MinimumUVDistance)
                    ni3 = -1;
                if ((Surface.SurfaceUVCoordinates[ni3] - Surface.SurfaceUVCoordinates[i1]).LengthSquared < MinimumUVDistance)
                    ni3 = -1;
                if ((Surface.SurfaceUVCoordinates[ni3] - Surface.SurfaceUVCoordinates[i2]).LengthSquared < MinimumUVDistance)
                    ni3 = -1;
            }
            if (Prof > 250 & (ni1 > -1 | ni2 > -1 | ni3 > -1))
            {
                return;
            }
            //
            Prof++;
            //
            if (ni1 < 0 && ni2 < 0 && ni3 < 0)
            { PublishFragment(i1, i2, i3); }
            else if (ni1 > -1 && ni2 < 0 && ni3 < 0)
            {
                GenerateFragment(ni1, i2, i3);
                GenerateFragment(i1, ni1, i3);
            }
            else if (ni1 > -1 && ni2 > -1 && ni3 < 0)
            {
                GenerateFragment(i1, ni1, i3);
                GenerateFragment(ni1, ni2, i3);
                GenerateFragment(ni1, i2, ni2);
            }
            else if (ni3 > -1 && ni1 < 0 && ni2 < 0)
            {
                GenerateFragment(i1, i2, ni3);
                GenerateFragment(ni3, i2, i3);
            }
            else if (ni3 > -1 && ni1 > -1 && ni2 < 0)
            {
                GenerateFragment(i3, ni3, i2);
                GenerateFragment(ni3, ni1, i2);
                GenerateFragment(ni3, i1, ni1);
            }
            else if (ni2 > -1 && ni3 < 0 && ni1 < 0)
            {
                GenerateFragment(i1, ni2, i3);
                GenerateFragment(i1, i2, ni2);
            }
            else if (ni2 > -1 && ni3 > -1 && ni1 < 0)
            {
                GenerateFragment(i2, ni2, i1);
                GenerateFragment(ni2, ni3, i1);
                GenerateFragment(ni2, i3, ni3);
            }
            else if (ni2 > -1 && ni3 > -1 && ni1 > -1)
            {
                GenerateFragment(i1, ni1, ni3);
                GenerateFragment(i2, ni2, ni1);
                GenerateFragment(i3, ni3, ni2);
                GenerateFragment(ni1, ni2, ni3);
            }
            else
            { PublishFragment(i1, i2, i3); }

            Prof--;
        }

        private void PublishFragment(int i1, int i2, int i3)
        {
            //
            Vec2 suv1 = Surface.SurfaceUVCoordinates[i1];
            Vec2 suv2 = Surface.SurfaceUVCoordinates[i2];
            Vec2 suv3 = Surface.SurfaceUVCoordinates[i3];
            if (suv1 == suv2 || suv2 == suv3 || suv3 == suv1) return;
            //
            Mtx2 mt = new Mtx2(suv2.X - suv1.X, suv3.X - suv1.X, suv2.Y - suv1.Y, suv3.Y - suv1.Y);
            double d = mt.Determinant;
            //
            if (Math.Abs(d) > MinimumMatrixDeterminant)
            {
                Vec2 v;
                foreach (Vec2 buv in BoundariesPoints)
                {
                    if ((buv - suv1).LengthSquared > MinimumUVDistance && (buv - suv2).LengthSquared > MinimumUVDistance && (buv - suv3).LengthSquared > MinimumUVDistance)
                    {
                        v.X = +mt.Row1.Y * (buv.X - suv1.X) - mt.Row0.Y * (buv.Y - suv1.Y);
                        v.Y = -mt.Row1.X * (buv.X - suv1.X) + mt.Row0.X * (buv.Y - suv1.Y);
                        v /= d;
                        if (Math.Abs(0.5 - v.X) < 0.5 && Math.Abs(0.5 - v.Y) < 0.5 & (v.X + v.Y) < 1)
                        {
                            int iNew = Surface.BuildPoint(buv, Element.SolidElementConstruction.LastColor, Positions, Normals);
                            GenerateFragment(i1, i2, iNew);
                            GenerateFragment(i2, i3, iNew);
                            GenerateFragment(i3, i1, iNew);
                            BoundariesPoints.Remove(buv);
                            return;
                        }
                    }
                }
            }
            //

            Indexes.Add(i1);
            Indexes.Add(i2);
            Indexes.Add(i3);
            //
        }

        #endregion Private Methods
    }

    //======================================================
}