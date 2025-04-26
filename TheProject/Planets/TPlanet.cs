using CiliaElements;
using Math3D;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using TheProject.Snub;
using static TheProject.Snub.TSnubDodecahedron;

namespace TheProject.Planets
{
    public class TPlanet : TAssemblyElement, IHyperObject
    {
        public TLink Link { get; set; }
        public readonly TLink GroundLink;
        public readonly TPlanetGround Ground;
        public readonly string Name;

        public Vec4 Position { get; set; }
        public Mtx4 Orientation { get; set; } = Mtx4.Identity;
        public Vec4 Speed { get; set; }

        private static TRandom NamesRandom = new TRandom();

        public TRandom Random;

        public TPlanet(int rank)
        {
            Random = new TRandom(rank);
            Name = NamesRandom.PopString();
            if (!Name.EndsWith("a")) Name += "a";
            Link = TManager.AttachElmt((new TAssemblyElement() { PartNumber = Name }).OwnerLink, TUniverse.Universe, null);
            Link.NodeName = Name;
            Link.ToBeReplaced = false;
            Ground = new TPlanetGround(this);
            GroundLink = TManager.AttachElmt(Ground.OwnerLink, Link, null);
            GroundLink.ToBeReplaced = false;
            //GroundLink.Pickable = false;
            GroundLink.NodeName = Ground.PartNumber;
        }

        public override void LaunchLoad()
        {
            ElementLoader.Publish();
        }
    }

    public class TPlanetGround : TInternal
    {
        public readonly TPlanet Planet;
        public readonly TRandom TopologyRnd;
        public Vec3f Clr;

        public TPlanetGround(TPlanet planet) : base(planet.Name + "-Ground")
        {
            Planet = planet;
            TopologyRnd = new TRandom(planet.Random.PopInt());
            Clr = TopologyRnd.PopColor(0.2f, 0.6f);

        }
        public override void LaunchLoad()
        {
            TTexture Texture = BuildTexture();

            TFGroup Faces = BuildFaces();
            Faces.GroupParameters.Texture = Texture;

            SolidElementConstruction.StartGroup.FGroups.Push(Faces);

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

        private TFGroup BuildFaces()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            TFGroup Faces = null;
            Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Textures = new TTextureCloud();
            //TLGroup Lines = null;
            //Lines = SolidElementConstruction.AddLGroup();
            //Lines.ShapeGroupParameters.Positions = Positions;
            //Lines.GroupParameters.Texture = SolidElementConstruction.AddTexture(1, 0, 0, 0);
            //
            double a1 = 1 * (int)(10 * TopologyRnd.PopDouble());
            double a2 = 1 * (int)(10 * TopologyRnd.PopDouble());
            double a3 = 1 * (int)(10 * TopologyRnd.PopDouble());
            double b1 = 2 * (int)(10 * TopologyRnd.PopDouble());
            double b2 = 2 * (int)(10 * TopologyRnd.PopDouble());
            double b3 = 2 * (int)(10 * TopologyRnd.PopDouble());
            double c1 = 2 * (int)(10 * TopologyRnd.PopDouble());
            double c2 = 2 * (int)(10 * TopologyRnd.PopDouble());
            double c3 = 2 * (int)(10 * TopologyRnd.PopDouble());

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
                Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0.1 + 0.8 * TopologyRnd.PopDouble(), 0.1 + 0.8 * TopologyRnd.PopDouble()));
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
                gf.IMatrix = Mtx4.InvertL(gf.Matrix);
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

            return Faces;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        TTexture BuildTexture()
        {
            byte R = (byte)(Clr.X * 256);
            byte G = (byte)(Clr.Y * 256);
            byte B = (byte)(Clr.Z * 256);

            TTexture Texture = SolidElementConstruction.AddTexture(1, Clr.X, Clr.Y, Clr.Z);
            Texture.KDBitmap = new System.Drawing.Bitmap(200, 200);
            for (int i = 0; i < 200; i++)
                for (int j = 0; j < 200; j++)
                    Texture.KDBitmap.SetPixel(i, j, Color.FromArgb(R + TopologyRnd.PopByte(10), G + TopologyRnd.PopByte(10), B + TopologyRnd.PopByte(10)));

            return Texture;
        }
    }
}