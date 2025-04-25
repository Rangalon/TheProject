using MySolarSystem;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CiliaElements.Elements.SolarSystem
{
    public class TPlanet : TInternal
    {

        public TPlanet(Bitmap iBmp, int iId)
            : base("Planet."+iId.ToString())
        {
            bmp = iBmp;
        }

        Bitmap bmp;
        TBody body;

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            TCloud Textures = SolidElementConstruction.AddCloud();
            //TCloud Angles = SolidElementConstruction.AddCloud();
            //Array.Resize(ref Positions.Vectors, 18);
            Positions.Vectors.Push(new Vec3(1, 0, 0));
            Positions.Vectors.Push(new Vec3(0, 0, 1));
            Positions.Vectors.Push(new Vec3(0, 0, -1));
            //
            Positions.Vectors.Push(new Vec3(0, 1, 0));
            Positions.Vectors.Push(new Vec3(0, 0, 1));
            Positions.Vectors.Push(new Vec3(0, 0, -1));
            //
            Positions.Vectors.Push(new Vec3(-1, 0, 0));
            Positions.Vectors.Push(new Vec3(0, 0, 1));
            Positions.Vectors.Push(new Vec3(0, 0, -1));
            //
            Positions.Vectors.Push(new Vec3(0, -1, 0));
            Positions.Vectors.Push(new Vec3(0, 0, 1));
            Positions.Vectors.Push(new Vec3(0, 0, -1));
            //
            Positions.Vectors.Push(new Vec3(1, 0, 0));
            //
            Textures.Vectors.Push(new Vec3(0.000, 0.5, 0));
            Textures.Vectors.Push(new Vec3(0.125, 1.0, 0));
            Textures.Vectors.Push(new Vec3(0.125, 0.0, 0));
            //
            Textures.Vectors.Push(new Vec3(0.250, 0.5, 0));
            Textures.Vectors.Push(new Vec3(0.375, 1.0, 0));
            Textures.Vectors.Push(new Vec3(0.375, 0.0, 0));
            //
            Textures.Vectors.Push(new Vec3(0.500, 0.5, 0));
            Textures.Vectors.Push(new Vec3(0.625, 1.0, 0));
            Textures.Vectors.Push(new Vec3(0.625, 0.0, 0));
            //
            Textures.Vectors.Push(new Vec3(0.750, 0.5, 0));
            Textures.Vectors.Push(new Vec3(0.875, 1.0, 0));
            Textures.Vectors.Push(new Vec3(0.875, 0.0, 0));
            //
            Textures.Vectors.Push(new Vec3(1.000, 0.5, 0));
            //
            TTexture Texture = SolidElementConstruction.AddTexture();
            if (bmp != null) Texture.KDBitmap = bmp;
            Texture.Color = new Vec4f(0.6F, 0.6F, 0.6F, 1);
            //
            TFGroup Facets = SolidElementConstruction.AddFGroup();
            Facets.ShapeGroupParameters.Positions = Positions;
            Facets.ShapeGroupParameters.Normals = Normals;
            Facets.ShapeGroupParameters.Textures = Textures;
            Facets.GroupParameters.Texture = Texture;
            Facets.Indexes.Push(new int[] { 0, 2, 3, 3, 1, 0, 3, 5, 6, 6, 4, 3, 6, 8, 9, 9, 7, 6, 9, 11, 12, 12, 10, 9 });
            //
            for (int j = 1; j < 8; j++)
            {
                int k = Facets.Indexes.Max;
                for (int i = 0; i < k; i += 3)
                {
                    int i1 = Facets.Indexes.Values[i];
                    int i2 = Facets.Indexes.Values[i + 1];
                    int i3 = Facets.Indexes.Values[i + 2];
                    //
                    Vec3 p1 = Positions.Vectors.Values[i1];
                    Vec3 p2 = Positions.Vectors.Values[i2];
                    Vec3 p3 = Positions.Vectors.Values[i3];
                    Vec3 p4;
                    Vec3 p5;
                    Vec3 p6;
                    //
                    Vec3 t1 = Textures.Vectors.Values[i1];
                    Vec3 t2 = Textures.Vectors.Values[i2];
                    Vec3 t3 = Textures.Vectors.Values[i3];
                    Vec3 t4;
                    Vec3 t5;
                    Vec3 t6;
                    //
                    //if (Math.Abs(t1.Y - 0.5) < Math.Abs(t2.Y - 0.5))
                    //{
                    //    t4 = (2 * t1 + t2) / 3;
                    //    t5 = (t2 + 2 * t3) / 3;
                    //    t6 = (t3 + t1) * 0.5;
                    //    p4 = (2 * p1 + p2) / 3;
                    //    p5 = (p2 + 2 * p3) / 3;
                    //    p6 = (p3 + p1) * 0.5;
                    //}
                    //else
                    //{
                    //    t4 = (t1 + 2 * t2) / 3;
                    //    t5 = (2 * t2 + t3) / 3;
                    //    t6 = (t3 + t1) * 0.5;
                    //    p4 = (p1 + 2 * p2) / 3;
                    //    p5 = (2 * p2 + p3) / 3;
                    //    p6 = (p3 + p1) * 0.5;
                    //}
                    //
                    t4 = (t1 + t2) * 0.5;
                    t5 = (t2 + t3) * 0.5;
                    t6 = (t3 + t1) * 0.5;
                    p4 = (p1 + p2) * 0.5;
                    p5 = (p2 + p3) * 0.5;
                    p6 = (p3 + p1) * 0.5;
                    //
                    p4.Normalize();
                    p5.Normalize();
                    p6.Normalize();
                    //if (p1.Y == 0 || p1.Y == 1) { p4.X = p2.X; p6.X = p3.X; }
                    if (t2.Y == 0 || t2.Y == 1) { t5.X = t3.X; t4.X = t1.X; }
                    //if (p3.Y == 0 || p3.Y == 1) { p6.X = p1.X; p5.X = p2.X; }
                    //
                    int i4 = Positions.Vectors.Max; Positions.Vectors.Push(p4);
                    int i5 = Positions.Vectors.Max; Positions.Vectors.Push(p5);
                    int i6 = Positions.Vectors.Max; Positions.Vectors.Push(p6);
                    //
                    Textures.Vectors.Push(t4);
                    Textures.Vectors.Push(t5);
                    Textures.Vectors.Push(t6);
                    //
                    Facets.Indexes.Values[i + 0] = i1;
                    Facets.Indexes.Values[i + 1] = i4;
                    Facets.Indexes.Values[i + 2] = i6;
                    //
                    Facets.Indexes.Push(i4);
                    Facets.Indexes.Push(i2);
                    Facets.Indexes.Push(i5);
                    //
                    Facets.Indexes.Push(i6);
                    Facets.Indexes.Push(i5);
                    Facets.Indexes.Push(i3);
                    //
                    Facets.Indexes.Push(i5);
                    Facets.Indexes.Push(i6);
                    Facets.Indexes.Push(i4);
                }
            }
            //
            //Angles.Vectors.Close();
            //Angles.Vectors.Reverse();
            //while (Angles.Vectors.NotEmpty)
            //{
            //    Vec3 v = Angles.Vectors.Pop();
            //    v.X /= 2 * Math.PI;
            //    v.Y /= 2 * Math.PI;
            //    v.Y += 0.5;
            //    Textures.Vectors.Push(v);
            //}
            //
            Normals.Vectors = Positions.Vectors;
            //
            SolidElementConstruction.StartGroup.FGroups.Push(Facets);
            //
            Positions = SolidElementConstruction.AddCloud();
            TPGroup Points = SolidElementConstruction.AddPGroup();
            //Array.Resize(ref Positions.Vectors, 1);
            //Array.Resize(ref Points.Indexes, 1);
            Positions.Vectors.Push(new Vec3());
            Positions.Vectors.Push(new Vec3(0, 0, 2));
            Positions.Vectors.Push(new Vec3(-Math.Sqrt(5) * 0.5, 0, 0));
            Points.Indexes.Push(0);
            Points.ShapeGroupParameters.Positions = Positions;
            Points.GroupParameters.Texture = Texture;
            SolidElementConstruction.StartGroup.PGroups.Push(Points);
            //

            //
            Texture = SolidElementConstruction.AddTexture();
            Texture.Color = new Vec4f(0, 1, 0, 1);
            //
            TLGroup Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //Array.Resize(ref Lines.Indexes, (n + 1) * 6);

            Lines.Indexes.Push(0);
            Lines.Indexes.Push(1);
            Lines.Indexes.Push(-1);
            Lines.Indexes.Push(1);
            Lines.Indexes.Push(2);

            //
            //Dim Faces As New TGroup With {.PositionsId = Positions.Id, .TextureId = Texture.Id, .Kind = EGroupKind.F}
            //AddGroup(Faces)
            //StartGroup.AddGroupId(Faces.Id)
            //Faces.Indexes.Add(0)
            //Faces.Indexes.Add(1)
            //Faces.Indexes.Add(2)
            //Faces.Indexes.Add(4 * (2 * Infinity / GroundStep + 1) - 3)
            //Faces.Indexes.Add(0)
            //Faces.Indexes.Add(1)
            //
            ElementLoader.Publish();
            //
        }

    }
}
