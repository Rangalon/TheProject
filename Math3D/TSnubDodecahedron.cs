using System;
using System.Collections.Generic;
using System.Linq;

namespace Math3D
{
    public class TSnubDodecahedron
    {
        #region Public Fields

        public const float CrossDelta = 0.01F;

        public TSDDHFacet[] Facets;

        public TSDDHPoint[] Points;

        #endregion Public Fields

        #region Public Constructors

        public TSnubDodecahedron(int iLevel)
        {
            double f = (1.0 + System.Math.Sqrt(5)) * 0.5;
            double ff = 0.5 * System.Math.Sqrt(f - 5.0 / 27.0);
            double e = System.Math.Pow(0.5 * f + ff, 1.0 / 3.0) + System.Math.Pow(0.5 * f - ff, 1.0 / 3.0);

            double a = e - 1 / e;
            double b = e * f + f * f + f / e;

            List<TSDDHPoint> SDDHPoints = new List<TSDDHPoint>();
            List<TSDDHFacet> SDDHFacets = new List<TSDDHFacet>();

            PushPosition(SDDHPoints, 2 * a, 2, 2 * b);
            PushPosition(SDDHPoints, a + b / f + f, -a * f + b + 1 / f, a / f + b * f - 1);
            PushPosition(SDDHPoints, -a / f + b * f + 1, -a + b / f - f, a * f + b - 1 / f);
            PushPosition(SDDHPoints, -a / f + b * f - 1, a - b / f - f, a * f + b + 1 / f);
            PushPosition(SDDHPoints, a + b / f - f, a * f - b + 1 / f, a / f + b * f + 1);

            TSDDHPoint[] initPoints = SDDHPoints.ToArray(); for (int i = 0; i < initPoints.Length; i++) { initPoints[i].Coord.Normalize(); initPoints[i].Id = i; }

            double r = initPoints[0].Coord.Length;
            double r1 = 1.01 * (initPoints[12] - initPoints[0]).Length;
            double r2 = 1.01 * (initPoints[24] - initPoints[0]).Length;

            List<int[]> lst = new List<int[]>();
            TSDDHPoint pi, pj, pk;
            for (int i = 0; i < 58; i++)
            {
                pi = initPoints[i];
                for (int j = i + 1; j < 59; j++)
                {
                    pj = initPoints[j];
                    for (int k = j + 1; k < 60; k++)
                    {
                        pk = initPoints[k];
                        double dij = (pi - pj).Length;
                        double djk = (pj - pk).Length;
                        double dki = (pk - pi).Length;
                        if (dij < r1 && djk < r1 && dki < r1)
                        {
                            if (Vec3.Dot(pi.Coord, Vec3.Cross(pj - pi, pk - pi)) < 0)
                                SDDHFacets.Add(new TSDDHFacet(pi, pj, pk));
                            else
                                SDDHFacets.Add(new TSDDHFacet(pj, pi, pk));
                        }
                        else if (dij < r1 && djk < r1 && dki < r2)
                            lst.Add(new int[] { i, j, k });
                        else if (djk < r1 && dki < r1 && dij < r2)
                            lst.Add(new int[] { j, k, i });
                        else if (dki < r1 && dij < r1 && djk < r2)
                            lst.Add(new int[] { k, i, j });
                    }
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
                //
                Vec3 p = initPoints[pos[0]] + initPoints[pos[1]] + initPoints[pos[2]] + initPoints[pos[3]] + initPoints[pos[4]];
                p.Normalize();
                p *= r;
                TSDDHPoint p0 = new TSDDHPoint() { Coord = p };
                SDDHPoints.Add(p0);
                //

                SDDHFacets.Add(new TSDDHFacet(p0, initPoints[pos[0]], initPoints[pos[1]]));
                SDDHFacets.Add(new TSDDHFacet(p0, initPoints[pos[1]], initPoints[pos[2]]));
                SDDHFacets.Add(new TSDDHFacet(p0, initPoints[pos[2]], initPoints[pos[3]]));
                SDDHFacets.Add(new TSDDHFacet(p0, initPoints[pos[3]], initPoints[pos[4]]));
                SDDHFacets.Add(new TSDDHFacet(p0, initPoints[pos[4]], initPoints[pos[0]]));
            }

            initPoints = SDDHPoints.ToArray(); for (int i = 0; i < initPoints.Length; i++) { initPoints[i].Coord.Normalize(); initPoints[i].Id = i; }
            for (int nbSplit = 0; nbSplit < iLevel; nbSplit++)
            {
                Console.WriteLine("Split " + nbSplit.ToString());
                int nb = SDDHFacets.Count;
                int i = nb;

                foreach (TSDDHFacet fc in SDDHFacets.ToArray())
                {
                    TSDDHPoint p12 = GetOrCreatePoint(fc.P1, fc.P2, SDDHPoints);// 0.5 * (fc.P1 + fc.P2), NewPoints);
                    TSDDHPoint p23 = GetOrCreatePoint(fc.P2, fc.P3, SDDHPoints);
                    TSDDHPoint p31 = GetOrCreatePoint(fc.P3, fc.P1, SDDHPoints);
                    SDDHFacets.Add(new TSDDHFacet(fc.P1, p12, p31));
                    SDDHFacets.Add(new TSDDHFacet(p12, fc.P2, p23));
                    SDDHFacets.Add(new TSDDHFacet(p31, p23, fc.P3));
                    fc.P1 = p12; fc.P2 = p23; fc.P3 = p31;
                    i--;
                    if (i % 100000 == 0)
                        Console.WriteLine("Split " + nbSplit.ToString() + ": " + i.ToString() + "/" + nb.ToString());
                }

                Console.WriteLine("Points: " + SDDHPoints.Count.ToString());
            }

            initPoints = SDDHPoints.ToArray(); for (int i = 0; i < initPoints.Length; i++) { initPoints[i].Coord.Normalize(); initPoints[i].Id = i; }

            foreach (TSDDHPoint p in initPoints)
            {
                p.A = Math.Atan2(p.Coord.Y, p.Coord.X);
                if (Math.Abs(Math.Sin(p.A)) > 0.5)
                    p.B = Math.Atan2(p.Coord.Z, p.Coord.Y / Math.Sin(p.A));
                else
                    p.B = Math.Atan2(p.Coord.Z, p.Coord.X / Math.Cos(p.A));
                //Vec3 n = p.Coord; n.Normalize();
                //Vec3 v = new Vec3(Math.Cos(p.B) * Math.Cos(p.A), Math.Cos(p.B) * Math.Sin(p.A), Math.Sin(p.B));
                //if ((n - v).LengthSquared > 1e-9)
                //{
                //}
            }

            //
            Facets = SDDHFacets.ToArray();
            Points = SDDHPoints.ToArray();
        }

        #endregion Public Constructors

        #region Private Methods

        private static TSDDHPoint GetOrCreatePoint(TSDDHPoint p1, TSDDHPoint p2, List<TSDDHPoint> SDDHPoints)
        {
            Vec3 p = 0.5 * (p1 + p2);
            TSDDHPoint p0;
            p0 = p1.Kids.FirstOrDefault(o => (o - p).LengthSquared < 1e-6);
            if (p0 == null) p0 = p2.Kids.FirstOrDefault(o => (o - p).LengthSquared < 1e-6);
            if (p0 == null)
            {
                p0 = new TSDDHPoint() { Coord = p };
                SDDHPoints.Add(p0);
            }
            p1.Kids.Add(p0);
            p2.Kids.Add(p0);
            return p0;
        }

        private static TSDDHPoint GetOrCreatePoint(Vec3 p, List<TSDDHPoint> SDDHPoints)
        {
            TSDDHPoint[] pts = SDDHPoints.Where(o => Math.Abs(o.Coord.X - p.X) < 1e2 && Math.Abs(o.Coord.Y - p.Y) < 1e2 && Math.Abs(o.Coord.Z - p.Z) < 1e2 && (o - p).LengthSquared < 1e-4).ToArray();
            TSDDHPoint p0;
            if (pts.Length > 0)
                p0 = pts[0];
            else
            {
                p0 = new TSDDHPoint() { Coord = p };
                SDDHPoints.Add(p0);
            }
            return p0;
        }

        private static void PushPosition(List<TSDDHPoint> SDDHPoints, double x, double y, double z)
        {
            //x *= Ept.EarthBigRadius;y *= Ept.EarthBigRadius;z *= Ept.EarthSmallRadius;
            //Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            SDDHPoints.Add(new Vec3(x, y, z)); SDDHPoints.Add(new Vec3(z, x, y)); SDDHPoints.Add(new Vec3(y, z, x));
            x *= -1; y *= -1;
            //Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            SDDHPoints.Add(new Vec3(x, y, z)); SDDHPoints.Add(new Vec3(z, x, y)); SDDHPoints.Add(new Vec3(y, z, x));
            x *= -1; z *= -1;
            //Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            SDDHPoints.Add(new Vec3(x, y, z)); SDDHPoints.Add(new Vec3(z, x, y)); SDDHPoints.Add(new Vec3(y, z, x));
            y *= -1; x *= -1;
            //Positions.Vectors.Push(new Vec3(x, y, z)); Positions.Vectors.Push(new Vec3(z, x, y)); Positions.Vectors.Push(new Vec3(y, z, x));
            SDDHPoints.Add(new Vec3(x, y, z)); SDDHPoints.Add(new Vec3(z, x, y)); SDDHPoints.Add(new Vec3(y, z, x));
        }

        #endregion Private Methods

        #region Public Classes

        public class TFacet
        {
            #region Public Fields

            public static double Radius = 0;
            public Mtx4 Matrix;

            #endregion Public Fields
        }

        public class TSDDHFacet
        {
            #region Public Fields

            public TSDDHPoint P1;
            public TSDDHPoint P2;
            public TSDDHPoint P3;

            #endregion Public Fields

            #region Public Constructors

            public TSDDHFacet(TSDDHPoint pi, TSDDHPoint pj, TSDDHPoint pk)
            {
                P1 = pi; P2 = pj; P3 = pk;
                P1.Facets.Add(this);
                P2.Facets.Add(this);
                P3.Facets.Add(this);
            }

            #endregion Public Constructors

            #region Public Methods

            public override string ToString()
            {
                return string.Format("{0} {1} {2}", P1.Id, P2.Id, P3.Id);
            }

            #endregion Public Methods
        }

        public class TSDDHPoint
        {
            #region Public Fields

            public double A;
            public double B;
            public Vec3 Coord;
            public double Delta;
            public List<TSDDHFacet> Facets = new List<TSDDHFacet>();
            public int Id;
            public List<TSDDHPoint> Kids = new List<TSDDHPoint>();
            public List<TSDDHPoint> Neighbours = new List<TSDDHPoint>();
            public Vec2? Text;

            #endregion Public Fields

            #region Private Fields

            private static readonly Random Rnd = new Random();

            #endregion Private Fields

            #region Public Methods

            public static implicit operator TSDDHPoint(Vec3 v)
            {
                return new TSDDHPoint() { Coord = v };
            }

            public static Vec3 operator -(TSDDHPoint p1, TSDDHPoint p2) => p1.Coord - p2.Coord;

            public static Vec3 operator +(TSDDHPoint p1, TSDDHPoint p2) => p1.Coord + p2.Coord;

            public void SetText()
            {
                List<Vec2> vecs = new List<Vec2>();
                foreach (TSDDHPoint n in Neighbours.Where(o => o.Text.HasValue)) vecs.Add(n.Text.Value);
                Vec2 v = new Vec2(Rnd.NextDouble() * 0.333, Rnd.NextDouble());
                while (vecs.Count(o => Math.Abs(o.X - v.X) < 0.02 || Math.Abs(o.Y - v.Y) < 0.02) > 0)
                    v = new Vec2(Rnd.NextDouble() * 0.333, Rnd.NextDouble());
                v.X += Delta; v.Y *= 10;
                Text = v;
            }

            public override string ToString()
            {
                return Coord.ToString();
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}