using Math3D;
using System;
using System.Drawing;

namespace CiliaElements.Elements.Internals
{
    public class TBeam : TInternal
    {
        Vec3f color;
        double radius;

        public TBeam(string pn, Color c, double r) : base(pn)
        {
            color = new Vec3f((float)c.R / 255.0f, (float)c.G / 255.0f, (float)c.B / 255.0f);
            radius = r;
        }

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            Positions.Vectors.Push(new Vec3(0, radius, 0));
            Positions.Vectors.Push(new Vec3(0, radius * -0.5, radius * 0.5 * Math.Sqrt(3)));
            Positions.Vectors.Push(new Vec3(0, radius * -0.5, radius * -0.5 * Math.Sqrt(3)));
            Positions.Vectors.Push(new Vec3(1, radius, 0));
            Positions.Vectors.Push(new Vec3(1, radius * 0.5, radius * -0.5 * Math.Sqrt(3)));
            Positions.Vectors.Push(new Vec3(1, radius * 0.5, radius * 0.5 * Math.Sqrt(3)));
            //
            TTexture Texture;
            TFGroup Triangles;
            //
            Texture = SolidElementConstruction.AddTexture(1, color.X, color.Y, color.Z);
            //
            Triangles = SolidElementConstruction.AddFGroup();
            Triangles.ShapeGroupParameters.Positions = Positions;
            Triangles.GroupParameters.Texture = Texture;
            Triangles.Indexes.Push(new int[] {
                0, 1, 2,
                0, 1, 5, 1, 2, 3, 2, 0, 4,
                3, 4, 5,
                3, 4, 2, 4, 5, 0, 5, 3, 1
                });
            //
            SolidElementConstruction.StartGroup.FGroups.Push(Triangles);
            //
            TLGroup Lines;
            //
            Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            Lines.Indexes.Push(new int[] {
                0, 1, 1, 2, 2, 0,
                0, 5, 1, 5, 1,3,2,3,2,4,0,4,
                3,4,4,5,5,3
                });
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            ElementLoader.Publish();
            //
        }

    }
}
