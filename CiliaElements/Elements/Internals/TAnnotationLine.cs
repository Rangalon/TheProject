using OpenTK;
using System;

namespace CiliaElements
{
    public class TAnnotationLine : TInternal
    {
        #region Public Constructors

        public TAnnotationLine(Vec4f iColor, String iPrefix)
            : base(iPrefix + "AnnotationLine")
        {
            TTexture Texture = SolidElementConstruction.AddTexture(iColor.W, iColor.X, iColor.Y, iColor.Z);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            //Array.Resize(ref Positions.Vectors, 2);

            Positions.Vectors.Push(new Vec3(0, 0, 0));
            Positions.Vectors.Push(new Vec3(1, 0, 0));
            //
            //
            TLGroup Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = SolidElementConstruction.Textures.Values[0];
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            Lines.Indexes.Push(0);
            Lines.Indexes.Push(1);
            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods
    }
}