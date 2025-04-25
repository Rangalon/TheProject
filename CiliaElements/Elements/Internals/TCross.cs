
using Math3D;

namespace CiliaElements
{
    public class TCross : TInternal
    {
        #region Public Fields

        public const float CrossDelta = 0.01F;

        #endregion Public Fields

        #region Public Constructors

        public TCross()
            : base("Cross")
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            Positions.Vectors.Push(new Vec3[6] {
            new Vec3(-Infinity*100, 0, -CrossDelta),
            new Vec3(+Infinity*100, 0, -CrossDelta),
            new Vec3(0, -Infinity*100, -CrossDelta),
            new Vec3(0, +Infinity*100, -CrossDelta),
            new Vec3(0, 0, -Infinity*100),
            new Vec3(0, 0, +Infinity*100)
        });
            //
            TTexture Texture;
            TLGroup Lines;

            //
            Texture = SolidElementConstruction.AddTexture(0.5F, 1, 0, 0);
            //
            Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            Lines.Indexes.Push(new int[2] { 0, 1 });
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            Texture = SolidElementConstruction.AddTexture(0.5F, 0, 1, 0);
            //
            Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            Lines.Indexes.Push(new int[2] { 2, 3 });
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            Texture = SolidElementConstruction.AddTexture(0.5F, 0, 0, 1);
            //
            Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            Lines.Indexes.Push(new int[2] { 4, 5 });
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods
    }
}