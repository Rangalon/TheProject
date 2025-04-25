using Math3D;
using System;
using System.Drawing;

namespace CiliaElements.Elements.Internals
{
    public class TArrow : TInternal
    {
        Vec3f color;

        public TArrow(string pn, Color c) : base(pn)
        {
            color = new Vec3f((float)c.R / 255.0f, (float)c.G / 255.0f, (float)c.B / 255.0f);
        }

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            Positions.Vectors.Push(new Vec3(0, 0, 0));
            Positions.Vectors.Push(new Vec3(1, 0, 0));
            Positions.Vectors.Push(new Vec3(0.8, 0.1, 0));
            Positions.Vectors.Push(new Vec3(0.8, -0.05, 0.05 * Math.Sqrt(3)));
            Positions.Vectors.Push(new Vec3(0.8, -0.05, -0.05 * Math.Sqrt(3)));
            //
            TTexture Texture;
            TLGroup Lines;
            //
            Texture = SolidElementConstruction.AddTexture(1, color.X, color.Y, color.Z);
            //
            Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            Lines.Indexes.Push(new int[] { 0, 1, 1, 2, 1, 3, 1, 4 });
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            ElementLoader.Publish();
            //
        }
    }
}
