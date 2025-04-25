
using Math3D;
using System.Drawing;

namespace CiliaElements
{
    public class TPoint : TInternal
    {
        #region Private Fields

        private static readonly Font FT18 = new Font("Verdana", 15);

        private static readonly int[] StaticIndexes = new int[] {
            0,2,5,0,5,3,
            0,3,4,0,4,2,
            1,2,4,1,5,2,
            1,3,5,1,4,3
        };

        private static readonly Vec3[] StaticNormals = new Vec3[]
        {
           new Vec3(+1,0,0),
            new Vec3(-1,0,0),
            new Vec3(0,+1,0),
            new Vec3(0,-1,0),
            new Vec3(0,0,+1),
            new Vec3(0,0,-1)
        };

        private static readonly Vec3[] StaticPositions = new Vec3[]
        {
            new Vec3(+100,0,0),
            new Vec3(-100,0,0),
            new Vec3(0,+100,0),
            new Vec3(0,-100,0),
            new Vec3(0,0,+100),
            new Vec3(0,0,-100)
        };

        #endregion Private Fields

        #region Public Constructors

        public TPoint(string iName)
            : base(iName)
        { }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            //
            TTexture Texture = SolidElementConstruction.AddTexture(0.25F, 1, 1, 1);
            // ----------------------------------------------------------
            TCloud Positions = SolidElementConstruction.AddCloud(); Positions.Vectors.Push(StaticPositions);
            TCloud Normals = SolidElementConstruction.AddCloud(); Normals.Vectors.Push(StaticNormals);
            TFGroup Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Normals = Normals;
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