using CiliaElements.Elements.Math;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Internals
{
    public class TWater : TInternal
    {
        #region Public Fields

        public static readonly TWater Default = new TWater();

        #endregion Public Fields

        #region Private Fields

        private TRandom rnd = new TRandom();

        #endregion Private Fields

        #region Public Constructors

        public TWater() : base("Water")
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            TTexture Texture = null;
            TFGroup Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Normals = Normals;
            Faces.ShapeGroupParameters.Textures = new TTextureCloud();
            //Faces.ShapeGroupParameters.Normals = Normals;
            Texture = SolidElementConstruction.AddTexture(1, 0.1, 0.1, 1);
            //Texture.KDBitmap = Properties.Resources.grass_grass_0131_01_s;
            Faces.GroupParameters.Texture = Texture;

            Bitmap bmp = new Bitmap(1000, 1000);
            Graphics grp = Graphics.FromImage(bmp);
            grp.Clear(Color.Cyan);
            SolidBrush backBrs = new SolidBrush(Color.FromArgb(16, 0, 0, 128));

            for (int i = 0; i < 100; i++)
                grp.FillEllipse(backBrs, (float)(1000 * rnd.NextUDouble() - 200), (float)(1000 * rnd.NextUDouble() - 200), 400, 400);

            grp.Dispose();
            Faces.GroupParameters.Texture.KDBitmap = bmp;

            //Positions.Vectors.Push(new Vec3[] {
            //    new Vec3(0,0,5),
            //    new Vec3(100,0,5),
            //    new Vec3(0,100,5),
            //    new Vec3(0,-0.4*100,-50),
            //    new Vec3(1.26727*100,-0.4*100,-50),
            //    new Vec3(1.4233*100,-0.0233*100,-50),
            //    new Vec3(-0.0233*100,1.4233*100,-50),
            //    new Vec3(-0.4*100,1.26727*100,-50),
            //    new Vec3(-0.4*100,0,-50)
            //});
            //Normals.Vectors.Push(new Vec3[] {
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1),
            //    new Vec3(0,0,1)
            //});

            //double MaxX = Positions.Vectors.Values.Max(o => o.X);
            //double MinX = Positions.Vectors.Values.Min(o => o.X);
            //MaxX = 1 / (MaxX - MinX);
            //double MaxY = Positions.Vectors.Values.Max(o => o.Y);
            //double MinY = Positions.Vectors.Values.Min(o => o.Y);
            //MaxY = 1 / (MaxY - MinY);

            //for (int i = 0; i < Positions.Vectors.Max; i++)
            //{
            //    Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2((Positions.Vectors.Values[i].X - MinX) * MaxX, (Positions.Vectors.Values[i].Y - MinY) * MaxY));
            //}

            Normals.Vectors.Push(new Vec3[] {
                new Vec3(0,0,1),
                new Vec3(0,0,1),
                new Vec3(0,0,1)
            });
            Positions.Vectors.Push(new Vec3[] {
                new Vec3(-150,0,0),
                new Vec3(150,0,0),
                new Vec3(0,150*System.Math.Sqrt(3),0),
            });
            Vec2 ptxt1 = new Vec2(0.1, 0.1);
            Vec2 ptxt2 = new Vec2(0.9, 0.1);
            Vec2 ptxt3 = new Vec2(0.5, 0.9);
            Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2[] { ptxt1, ptxt2, ptxt3 });

            //Faces.Indexes.Push(new int[] {
            //    0,1,2,
            //    0,3,4,0,4,1,
            //    1,4,5,
            //    1,5,6,1,6,2,
            //    2,6,7,
            //    2,7,8,2,8,0,
            //    0,8,3,
            //    9,10,11
            //});
            Faces.Indexes.Push(new int[] {
                0,1,2
            });

            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            //
            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}