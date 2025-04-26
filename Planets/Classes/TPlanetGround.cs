using CiliaElements;
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Planets.Classes.TSnubDodecahedron;

namespace Planets.Classes
{
    public class TPlanetGround : TInternal
    {
        Vec3f Clr;

        public readonly TRandom TopologyRnd;


        public TPlanetGround(string name, Vec3f color, TRandom topologyRandom) : base(name)
        {
            TopologyRnd = topologyRandom;
            Clr = color;
        }

        public override void LaunchLoad()
        {
            TTexture Texture = SolidElementConstruction.AddTexture(1, Clr.X, Clr.Y, Clr.Z);
            Texture.KDBitmap = new System.Drawing.Bitmap(200, 200);

            System.Drawing.Drawing2D.LinearGradientBrush brs = new System.Drawing.Drawing2D.LinearGradientBrush(new Point(0, 0), new Point(200, 200),
                Color.FromArgb(32, (byte)(Clr.X * 255) - 32, (byte)(Clr.Y * 255) - 32, (byte)(Clr.Z * 255) + 32),
                Color.FromArgb(32, (byte)(Clr.X * 255) + 32, (byte)(Clr.Y * 255) + 32, (byte)(Clr.Z * 255) - 32));

            Graphics grp = Graphics.FromImage(Texture.KDBitmap);
            grp.Clear(Color.Gray );
            grp.FillRectangle(brs, 0, 0, 200, 200);
            for (int ii = 0; ii < 100; ii++)
            {
                Rectangle r = new Rectangle(
                    (int)(200 * TopologyRnd.Next()), (int)(50 * TopologyRnd.Next()),
                    (int)(200 * TopologyRnd.Next()), (int)(50 * TopologyRnd.Next()));
                r.X -= r.Width / 2;
                r.Y -= r.Height / 2;
                r.X = r.X >= 0 ? r.X : 0;
                r.Y = r.Y >= 0 ? r.Y : 0;
                r.Width = r.Right > 200 ? r.Width + 200 - r.Right : r.Width;
                r.Height = r.Bottom > 200 ? r.Height + 200 - r.Bottom : r.Height;
                grp.FillEllipse(brs, r);
            }
            grp.Dispose();
            brs.Dispose();

            TCloud Positions = SolidElementConstruction.AddCloud(); 
            TCloud Normals = SolidElementConstruction.AddCloud();
            TFGroup Faces = null;
            Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.GroupParameters.Texture = Texture;
            Faces.ShapeGroupParameters.Textures = new TTextureCloud();
            //TLGroup Lines = null;
            //Lines = SolidElementConstruction.AddLGroup();
            //Lines.ShapeGroupParameters.Positions = Positions;
            //Lines.GroupParameters.Texture = SolidElementConstruction.AddTexture(1, 0, 0, 0);
            //
            double a1 = 1 * (int)(10 * TopologyRnd.Next());
            double a2 = 1 * (int)(10 * TopologyRnd.Next());
            double a3 = 1 * (int)(10 * TopologyRnd.Next());
            double b1 = 2 * (int)(10 * TopologyRnd.Next());
            double b2 = 2 * (int)(10 * TopologyRnd.Next());
            double b3 = 2 * (int)(10 * TopologyRnd.Next());
            double c1 = 2 * (int)(10 * TopologyRnd.Next());
            double c2 = 2 * (int)(10 * TopologyRnd.Next());
            double c3 = 2 * (int)(10 * TopologyRnd.Next());

            GroundPoints = new TGroundPoint[TSnubDodecahedron.Points.Length];

            int i = 0;



            foreach (TSDDHPoint p in TSnubDodecahedron.Points)
            {
                TGroundPoint gp = new TGroundPoint();
                double r = 100 + 0.8 * (
                    Math.Cos(a1 * p.A) + Math.Cos(a2 * p.A) + Math.Cos(a3 * p.A) +
                    Math.Cos(b1 * p.B) + Math.Cos(b2 * p.B) + Math.Cos(b3 * p.B) +
                    Math.Cos(c1 * (p.A + p.B)) + Math.Cos(c2 * (p.A + p.B)) + Math.Cos(c3 * (p.A + p.B))
                    );
                gp.Coord = new Vec3(r * Math.Cos(p.B) * Math.Cos(p.A), r * Math.Cos(p.B) * Math.Sin(p.A), r * Math.Sin(p.B));
                Positions.Vectors.Push(gp.Coord);
                Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0.1 + 0.8 * TopologyRnd.Next(), 0.1 + 0.8 * TopologyRnd.Next()));
                GroundPoints[i] = gp; i++;
            }

            GroundFacets = new TGroundFacet[Facets.Length];

            i = 0;

            foreach (TSDDHFacet fc in Facets)
            {
                Faces.Indexes.Push(fc.P1.Id); Faces.Indexes.Push(fc.P2.Id); Faces.Indexes.Push(fc.P3.Id);
                //Lines.Indexes.Push(fc.P1.Id); Lines.Indexes.Push(fc.P2.Id); Lines.Indexes.Push(-1);
                //Lines.Indexes.Push(fc.P2.Id); Lines.Indexes.Push(fc.P3.Id); Lines.Indexes.Push(-1);
                //Lines.Indexes.Push(fc.P3.Id); Lines.Indexes.Push(fc.P1.Id); Lines.Indexes.Push(-1);
                TGroundFacet gf = new TGroundFacet();
                gf.P1 = GroundPoints[fc.P1.Id];
                gf.P2 = GroundPoints[fc.P2.Id];
                gf.P3 = GroundPoints[fc.P3.Id];
                gf.Matrix.Row3 = new Vec4((gf.P1 + gf.P2 + gf.P3) / 3, 1);
                gf.Matrix.Row2 = new Vec4(Vec3.Cross(gf.P2 - (Vec3)gf.Matrix.Row3, gf.P1 - (Vec3)gf.Matrix.Row3), 0); gf.Matrix.Row2.Normalize();
                gf.Matrix.Row1 = new Vec4(gf.P1 - (Vec3)gf.Matrix.Row3, 0); gf.Matrix.Row1.Normalize();
                gf.Matrix.Row0 = Vec4.Cross(gf.Matrix.Row1, gf.Matrix.Row2); gf.Matrix.Row0.Normalize();
                //
                gf.IMatrix = gf.Matrix; gf.IMatrix.Invert();
                //
                GroundFacets[i] = gf; i++;
            }
            //
            foreach (TGroundPoint p in GroundPoints)
            {
                TGroundFacet[] fcs = GroundFacets.Where(o => o.P1 == p || o.P2 == p || o.P3 == p).ToArray();
                if (fcs.Length != 5 && fcs.Length != 6)
                {
                }
                Vec4 n = new Vec4();
                foreach (TGroundFacet fc in fcs)
                    n += fc.Matrix.Row2;
                n.Normalize();
                p.Norm = (Vec3)n;
                Normals.Vectors.Push(p.Norm);
            }


            //
            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            //SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            ElementLoader.Publish();
        }

        public TGroundPoint[] GroundPoints;
        public TGroundFacet[] GroundFacets;

        public class TGroundPoint
        {
            public Vec3 Coord;
            public Vec3 Norm;

            public static Vec3 operator -(TGroundPoint p1, TGroundPoint p2) => p1.Coord - p2.Coord;
            public static Vec3 operator -(TGroundPoint p1, Vec3 p2) => p1.Coord - p2;
            public static Vec3 operator +(TGroundPoint p1, TGroundPoint p2) => p1.Coord + p2.Coord;
            public static Vec3 operator +(Vec3 p1, TGroundPoint p2) => p1 + p2.Coord;
        }

        public class TGroundFacet
        {
            public TGroundPoint P1;
            public TGroundPoint P2;
            public TGroundPoint P3;

            public Mtx4 Matrix;
            public Mtx4 IMatrix;
        }
    }
}
