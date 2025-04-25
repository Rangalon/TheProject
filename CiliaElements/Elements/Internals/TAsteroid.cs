
using Math3D;
using CiliaElements.Elements.Math;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace CiliaElements
{
    public class TAsteroid : TInternal
    {
        #region Public Fields

        public const float CrossDelta = 0.01F;

        public static TFacet[] Facets = { };

        public static Bitmap TextureBitmap;

        #endregion Public Fields

        #region Private Fields

        private static readonly double Ro;
        private readonly TRandom rnd = new TRandom();

        static readonly double f;
        static readonly double ff;
        static readonly double e;

        static readonly double a;
        static readonly double b;

        static TAsteroid()
        {
            f = (1.0 + System.Math.Sqrt(5)) * 0.5;
            ff = 0.5 * System.Math.Sqrt(f - 5.0 / 27.0);
            e = System.Math.Pow(0.5 * f + ff, 1.0 / 3.0) + System.Math.Pow(0.5 * f - ff, 1.0 / 3.0);

            a = e - 1 / e;
            b = e * f + f * f + f / e;

            Ro = Ept.EarthBigRadius / Math.Sqrt((2 * a) * (2 * a) + 4 + (2 * b) * (2 * b));
        }

        #endregion Private Fields

        #region Public Constructors

        public TAsteroid()
                   : base("Asteroid")
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {


            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            PushPosition(Positions, 2 * a, 2, 2 * b);
            PushPosition(Positions, a + b / f + f, -a * f + b + 1 / f, a / f + b * f - 1);
            PushPosition(Positions, -a / f + b * f + 1, -a + b / f - f, a * f + b - 1 / f);
            PushPosition(Positions, -a / f + b * f - 1, a - b / f - f, a * f + b + 1 / f);
            PushPosition(Positions, a + b / f - f, a * f - b + 1 / f, a / f + b * f + 1);

            double r = Positions.Vectors.Values[0].Length;
            double r1 = 1.01 * (Positions.Vectors.Values[12] - Positions.Vectors.Values[0]).Length;
            double r2 = 1.01 * (Positions.Vectors.Values[24] - Positions.Vectors.Values[0]).Length;

            foreach (Vec3 p in Positions.Vectors.Values.Where(o => o != null))
            {
                Vec3 n = p;
                n.Normalize();
                Normals.Vectors.Push(n);
            }

            //
            TTexture Texture = null;
            //TLGroup Lines = null;
            TFGroup Faces = null;

            //
            Texture = SolidElementConstruction.AddTexture(0.5F, 1, 0, 0);
            //
            //Lines = SolidElementConstruction.AddLGroup();
            //Lines.ShapeGroupParameters.Positions = Positions;
            //Lines.GroupParameters.Texture = Texture;
            //for (int i = 0; i < 59; i++)
            //    for (int j = i + 1; j < 60; j++)
            //    {
            //        double dij = (Positions.Vectors.Values[i] - Positions.Vectors.Values[j]).Length;
            //        if (dij < r1)
            //        {
            //            //Lines.Indexes.Push(new int[] { i, j, -1 });
            //        }
            //    }

            //SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            Texture = SolidElementConstruction.AddTexture(1, 0, 1, 0);
            Texture.KDBitmap = Properties.Resources.Sea;
            //Graphics grp = Graphics.FromImage(Texture.KDBitmap);
            //grp.Clear(Color.DarkGreen);
            //int dr = 100;
            //for (int i = 0; i < 10; i++)
            //{
            //    SolidBrush sb = new SolidBrush(Color.FromArgb((byte)(rnd.NextUFloat() * 256), (byte)(rnd.NextUFloat() * 256), (byte)(rnd.NextUFloat() * 256), (byte)(rnd.NextUFloat() * 256)));
            //    int x = (int)(200 * rnd.NextUFloat()); if (x > 100) x -= 200;
            //    int y = (int)(200 * rnd.NextUFloat()); if (y > 100) y -= 200;
            //    grp.FillEllipse(sb, x - dr, y - dr, 2 * dr, 2 * dr);
            //    grp.FillEllipse(sb, 200 + x - dr, y - dr, 2 * dr, 2 * dr);
            //    grp.FillEllipse(sb, x - dr, 200 + y - dr, 2 * dr, 2 * dr);
            //    grp.FillEllipse(sb, 200 + x - dr, 200 + y - dr, 2 * dr, 2 * dr);
            //    sb.Dispose();
            //}
            //grp.Dispose();
            //
            Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Textures = new TTextureCloud();
            //Faces.ShapeGroupParameters.Normals = Normals;
            Faces.GroupParameters.Texture = Texture;

            Faces.GroupParameters.Matrix = Mtx4.CreateScale(Ro, Ro, Ro * Ept.EarthSmallRadius / Ept.EarthBigRadius);

            List<int[]> lst = new List<int[]>();
            for (int i = 0; i < 58; i++)
                for (int j = i + 1; j < 59; j++)
                    for (int k = j + 1; k < 60; k++)
                    {
                        double dij = (Positions.Vectors.Values[i] - Positions.Vectors.Values[j]).Length;
                        double djk = (Positions.Vectors.Values[j] - Positions.Vectors.Values[k]).Length;
                        double dki = (Positions.Vectors.Values[k] - Positions.Vectors.Values[i]).Length;
                        if (dij < r1 && djk < r1 && dki < r1)
                        {
                            if (Vec3.Dot(Positions.Vectors.Values[i], Vec3.Cross(Positions.Vectors.Values[j] - Positions.Vectors.Values[i], Positions.Vectors.Values[k] - Positions.Vectors.Values[i])) < 0)
                                Faces.Indexes.Push(new int[] { i, j, k });
                            else
                                Faces.Indexes.Push(new int[] { j, i, k });
                        }
                        else if (dij < r1 && djk < r1 && dki < r2)
                        {
                            lst.Add(new int[] { i, j, k });
                        }
                        else if (djk < r1 && dki < r1 && dij < r2)
                        {
                            lst.Add(new int[] { j, k, i });
                        }
                        else if (dki < r1 && dij < r1 && djk < r2)
                        {
                            lst.Add(new int[] { k, i, j });
                        }
                    }

            while (lst.Count > 0)
            {
                int[] pos = new int[5];
                pos[0] = lst[0][1];
                lst.RemoveAt(0);
                for (int i = 1; i < 5; i++)
                {
                    int[] tbl = lst.First(o => o[0] == pos[i - 1]);
                    lst.Remove(tbl);
                    pos[i] = tbl[1];
                }
                int k = Positions.Vectors.Max;
                Vec3 p =
                    Positions.Vectors.Values[pos[0]] +
                    Positions.Vectors.Values[pos[1]] +
                    Positions.Vectors.Values[pos[2]] +
                    Positions.Vectors.Values[pos[3]] +
                    Positions.Vectors.Values[pos[4]];
                p.Normalize();
                p *= r;
                Positions.Vectors.Push(p);

                Faces.Indexes.Push(new int[] {
                    k,pos[0], pos[1],
                    k,pos[1], pos[2],
                    k,pos[2], pos[3],
                    k,pos[3], pos[4],
                    k,pos[4],pos[0] });
                p -= Positions.Vectors.Values[pos[0]];
            }

            for (int nbSplit = 0; nbSplit < 0; nbSplit++)
            {
                for (int rk = Faces.Indexes.Max - 3; rk >= 0; rk -= 3)
                {
                    Vec3 p1 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 0]];
                    Vec3 p2 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 1]];
                    Vec3 p3 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 2]];
                    Vec3 p12 = 0.5 * (p1 + p2); p12.Normalize(); p12 *= r;
                    Vec3 p23 = 0.5 * (p2 + p3); p23.Normalize(); p23 *= r;
                    Vec3 p31 = 0.5 * (p3 + p1); p31.Normalize(); p31 *= r;
                    int k12 = Array.IndexOf(Positions.Vectors.Values, p12);
                    if (k12 < 0) { k12 = Positions.Vectors.Max; Positions.Vectors.Push(p12); }
                    int k23 = Array.IndexOf(Positions.Vectors.Values, p23);
                    if (k23 < 0) { k23 = Positions.Vectors.Max; Positions.Vectors.Push(p23); }
                    int k31 = Array.IndexOf(Positions.Vectors.Values, p31);
                    if (k31 < 0) { k31 = Positions.Vectors.Max; Positions.Vectors.Push(p31); }
                    //Lines.Indexes.Push(new int[] {
                    //    Faces.Indexes.Values[rk + 0],k12,-1,
                    //    Faces.Indexes.Values[rk + 0],k31,-1,
                    //    Faces.Indexes.Values[rk + 1],k23,-1,
                    //    Faces.Indexes.Values[rk + 1],k12,-1,
                    //    Faces.Indexes.Values[rk + 2],k31,-1,
                    //    Faces.Indexes.Values[rk + 2],k23,-1,
                    //    k12,k23,-1,
                    //    k23,k31,-1,
                    //    k31,k12
                    //});
                    Faces.Indexes.Push(new int[] { k31, Faces.Indexes.Values[rk + 0], k12 });
                    Faces.Indexes.Push(new int[] { k12, Faces.Indexes.Values[rk + 1], k23 });
                    Faces.Indexes.Push(new int[] { k23, Faces.Indexes.Values[rk + 2], k31 });
                    Faces.Indexes.Values[rk + 0] = k12;
                    Faces.Indexes.Values[rk + 1] = k23;
                    Faces.Indexes.Values[rk + 2] = k31;
                }
            }

            double dx = 150, dy = 150 * System.Math.Sqrt(3);

            List<double> rs = new List<double>();
            List<TFacet> fcs = new List<TFacet>();
            for (int rk = Faces.Indexes.Max - 3; rk >= 0; rk -= 3)
            {
                Vec3 p1 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 0]];
                Vec3 p2 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 1]];
                Vec3 p3 = Positions.Vectors.Values[Faces.Indexes.Values[rk + 2]];
                Vec3 u12 = p2 - p1; u12.Normalize();
                Vec3 u23 = p3 - p2; u23.Normalize();
                Vec3 u31 = p1 - p3; u31.Normalize();
                //
                Vec3 u, v, w;
                u = u23 - u31;
                v = u23 - u12;
                w = p3 - p2;
                Vec2 vr;
                vr.X = Vec3.Dot(v, w);
                vr.Y = Vec3.Dot(u, w);
                Mtx2 mr;
                mr.Row0.X = Vec3.Dot(v, v); mr.Row1.X = Vec3.Dot(v, u);
                mr.Row0.Y = Vec3.Dot(u, v); mr.Row1.Y = Vec3.Dot(u, u);
                mr.Invert();

                Vec3 p = p2 + Vec2.Dot(mr.Row0, vr) * v;
                w = Vec3.Cross(-u, v); w.Normalize();
                u = Vec3.Cross(v, w); u.Normalize();
                v.Normalize();

                int k = Positions.Vectors.Max;
                Positions.Vectors.Push(p);
                Positions.Vectors.Push(p + 0.1 * u);
                Positions.Vectors.Push(p + 0.1 * v);
                Positions.Vectors.Push(p + 0.1 * w);
                //Lines.Indexes.Push(new int[] { k, k + 1, -1, k, k + 2, -1, k, k + 3, -1 });
                TFacet face = new TFacet();
                face.Matrix.Row0 = new Vec4(u, 0);
                face.Matrix.Row1 = new Vec4(v, 0);
                face.Matrix.Row2 = new Vec4(w, 0);
                face.Matrix.Row3 = new Vec4(p, 1);
                face.GroundMatrix.Row0 = new Vec4(p2 - 0.5 * (p2 + p1), 0) / dx;
                face.GroundMatrix.Row1 = new Vec4(p3 - 0.5 * (p2 + p1), 0) / dy;
                face.GroundMatrix.Row2 = new Vec4(w, 0);
                face.GroundMatrix.Row3 = new Vec4(0.5 * (p2 + p1), 1);
                fcs.Add(face);
                u = p2 - p1; u.Normalize(); v = p - p1; rs.Add(System.Math.Round((p - p1 - u * Vec3.Dot(v, u)).Length, 4));
            }
            rs = rs.Distinct().ToList();
            rs.Sort();
            TFacet.Radius = 0.9 * rs[0];
            Console.WriteLine("Radius: " + TFacet.Radius.ToString());

            for (int i = 0; i < Positions.Vectors.Max; i++)
                Positions.Vectors.Values[i] *= (Ro - 1) / Ro;

            for (int i = 0; i < Positions.Vectors.Max; i++)
                Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(rnd.NextUDouble(), rnd.NextUDouble()));

            Facets = fcs.ToArray();
            //
            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods

        #region Private Methods

        private void PushPosition(TCloud Positions, double x, double y, double z)
        {
            //x *= Ro; y *= Ro; z *= Ro;
            Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            x *= -1; y *= -1;
            Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            x *= -1; z *= -1;
            Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            y *= -1; x *= -1;
            Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
        }

        #endregion Private Methods

        #region Public Classes

        public class TFacet
        {
            #region Public Fields

            public static double Radius = 0;
            public Mtx4 GroundMatrix;
            public Mtx4 Matrix;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}