
using Math3D;
using System.Drawing;

namespace CiliaElements
{
    public class TBox : TInternal
    {
        #region Private Fields

        private static readonly Font FT18 = new Font("Verdana", 15);

        private static readonly int[] StaticIndexes = new int[] {
            0,1,2,2,1,3,
            2,3,4,4,3,5,
            4,5,6,6,5,7,
            6,7,8,8,7,9
        };

        private static readonly Vec3[] StaticNormals = new Vec3[]
        {
            new Vec3(1,1,0),
            new Vec3(1,1,0),
            new Vec3(1,-1,0),
            new Vec3(1,-1,0),
            new Vec3(-1,-1,0),
            new Vec3(-1,-1,0),
            new Vec3(-1,1,0),
            new Vec3(-1,1,0),
            new Vec3(1,1,0),
            new Vec3(1,1,0),
        };

        private static readonly Vec3[] StaticPositions = new Vec3[]
        {
            new Vec3(+0.005,+0.005,-0.025),
            new Vec3(+0.005,+0.005,+0.025),
            new Vec3(+0.005,-0.005,-0.025),
            new Vec3(+0.005,-0.005,+0.025),
            new Vec3(-0.005,-0.005,-0.025),
            new Vec3(-0.005,-0.005,+0.025),
            new Vec3(-0.005,+0.005,-0.025),
            new Vec3(-0.005,+0.005,+0.025),
            new Vec3(+0.005,+0.005,-0.025),
            new Vec3(+0.005,+0.005,+0.025),
        };

        private static readonly Vec2[] StaticTextures = new Vec2[] {
            new Vec2(0,0.00),
            new Vec2(1,0.00),
            new Vec2(0,0.25),
            new Vec2(1,0.25),
            new Vec2(0,0.50),
            new Vec2(1,0.50),
            new Vec2(0,0.75),
            new Vec2(1,0.75),
            new Vec2(0,1.00),
            new Vec2(1,1.00),
        };

        #endregion Private Fields

        #region Public Constructors

        public TBox(string iLabel)
            : base(iLabel)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TTexture Texture = SolidElementConstruction.AddTexture(0.25F, 1, 1, 1);
            // ----------------------------------------------------------
            TCloud Positions = SolidElementConstruction.AddCloud(); Positions.Vectors.Push(StaticPositions);
            TCloud Normals = SolidElementConstruction.AddCloud(); Normals.Vectors.Push(StaticNormals);
            TTextureCloud Textures = SolidElementConstruction.AddTextureCloud(); Textures.Vectors.Push(StaticTextures);
            TFGroup Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Normals = Normals;
            Faces.ShapeGroupParameters.Textures = Textures;
            Faces.GroupParameters.Texture = Texture;
            Faces.Indexes.Push(StaticIndexes);
            //
            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            // ----------------------------------------------------------
            Bitmap bmp = new Bitmap(500, 200);
            Graphics grp = Graphics.FromImage(bmp);
            grp.Clear(Color.DarkBlue);
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 1, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 51, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 101, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 151, 490, 90));
            grp.TranslateTransform(500, 200);
            grp.RotateTransform(180);
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 1, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 51, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 101, 490, 90));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT18, Brushes.Yellow, new Rectangle(5, 151, 490, 90));
            grp.Dispose();
            Texture.KDBitmap = bmp;
            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods
    }
}