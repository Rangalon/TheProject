
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml;

namespace CiliaElements
{
    public class TWire : TInternal
    {
        #region Public Fields

        public Mtx4[] Path = { };

        #endregion Public Fields

        #region Private Fields

        private static readonly Font FT9 = new Font("Verdana", 9);
        private readonly XmlNodeList points;

        #endregion Private Fields

        #region Public Constructors

        public TWire(string iLabel, XmlNodeList iPoints)
            : base(iLabel)
        {
            points = iPoints;
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TTexture Texture = SolidElementConstruction.AddTexture(0.25F, 1, 1, 1);
            // ----------------------------------------------------------
            List<Vec3> lst = new List<Vec3>();
            foreach (XmlElement e in points)
                lst.Add(new Vec3(double.Parse(e.GetAttribute("x"), CultureInfo.InvariantCulture) * 0.001, double.Parse(e.GetAttribute("y"), CultureInfo.InvariantCulture) * 0.001, double.Parse(e.GetAttribute("z"), CultureInfo.InvariantCulture) * 0.001));
            // ----------------------------------------------------------
            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            TTextureCloud Textures = SolidElementConstruction.AddTextureCloud();
            List<Vec3> lst2 = new List<Vec3>();
            List<Vec3> lst3 = new List<Vec3>();
            List<Mtx4> mtxs = new List<Mtx4>();
            for (int i = 0; i < lst.Count - 1; i++)
            {
                Vec3 p1 = lst[i];
                Vec3 p2 = lst[i + 1];
                Vec3 u = (p2 - p1);
                Vec3 w = new Vec3(u.Y, u.Z, -u.X);
                Vec3 v = Vec3.Cross(w, u); v.Normalize(); v *= 0.005;
                w = Vec3.Cross(u, v); w.Normalize(); w *= 0.0025 * Math.Sqrt(3);
                lst2.Add(p1 + v);
                lst2.Add(p1 - v * 0.5 + w);
                lst2.Add(p1 - v * 0.5 - w);
                lst2.Add(p1 + v);
                lst2.Add(p2 + v);
                lst2.Add(p2 - v * 0.5 + w);
                lst2.Add(p2 - v * 0.5 - w);
                lst2.Add(p2 + v);
                lst3.Add(v);
                lst3.Add(-v * 0.5 + w);
                lst3.Add(-v * 0.5 - w);
                lst3.Add(v);
                lst3.Add(v);
                lst3.Add(-v * 0.5 + w);
                lst3.Add(-v * 0.5 - w);
                lst3.Add(v);
                Textures.Vectors.Push(new Vec2(1, 0.000));
                Textures.Vectors.Push(new Vec2(1, 0.333));
                Textures.Vectors.Push(new Vec2(1, 0.666));
                Textures.Vectors.Push(new Vec2(1, 0.999));
                Textures.Vectors.Push(new Vec2(0, 0.000));
                Textures.Vectors.Push(new Vec2(0, 0.333));
                Textures.Vectors.Push(new Vec2(0, 0.666));
                Textures.Vectors.Push(new Vec2(0, 0.999));
                //
                Mtx4 m = Mtx4.Identity;
                m.Row0 = -new Vec4(w, 0);
                m.Row1 = -new Vec4(v, 0);
                m.Row2 = -new Vec4(u, 0);
                m.Row0.Normalize();
                m.Row1.Normalize();
                m.Row2.Normalize();
                Vec4 vv = Vec4.Cross(m.Row0, m.Row1);
                vv.X += 0;
                m.Row3 = new Vec4(p1, 1) + 0.005 * m.Row1;
                mtxs.Add(m);
            }
            {
                Mtx4 m = mtxs[mtxs.Count - 1];
                m.Row3 = new Vec4(lst[lst.Count - 1], 1) + 0.005 * m.Row1;
                mtxs.Add(m);
            }
            Path = mtxs.ToArray();
            for (int i = 1; i < lst.Count - 1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vec3 v1 = lst2[i * 8 - 4 + j];
                    Vec3 v2 = lst2[i * 8 + j];
                    v1 = (v1 + v2) * 0.5;
                    lst2.RemoveAt(i * 8 + j);
                    lst2.RemoveAt(i * 8 - 4 + j);
                    lst2.Insert(i * 8 - 4 + j, v1);
                    lst2.Insert(i * 8 + j, v1);
                }
            }
            Positions.Vectors.Push(lst2.ToArray());
            foreach (Vec3 v in lst3)
            {
                Vec3 n = v; n.Normalize();
                Normals.Vectors.Push(n);
            }

            TFGroup Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Normals = Normals;
            Faces.ShapeGroupParameters.Textures = Textures;
            Faces.GroupParameters.Texture = Texture;
            ////
            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            for (int i = 0; i < lst.Count - 1; i++)
            {
                Faces.Indexes.Push(i * 8 + 0);
                Faces.Indexes.Push(i * 8 + 1);
                Faces.Indexes.Push(i * 8 + 4);
                Faces.Indexes.Push(i * 8 + 4);
                Faces.Indexes.Push(i * 8 + 1);
                Faces.Indexes.Push(i * 8 + 5);
                //
                Faces.Indexes.Push(i * 8 + 1);
                Faces.Indexes.Push(i * 8 + 2);
                Faces.Indexes.Push(i * 8 + 5);
                Faces.Indexes.Push(i * 8 + 5);
                Faces.Indexes.Push(i * 8 + 2);
                Faces.Indexes.Push(i * 8 + 6);
                //
                Faces.Indexes.Push(i * 8 + 2);
                Faces.Indexes.Push(i * 8 + 3);
                Faces.Indexes.Push(i * 8 + 6);
                Faces.Indexes.Push(i * 8 + 6);
                Faces.Indexes.Push(i * 8 + 3);
                Faces.Indexes.Push(i * 8 + 7);
            }
            //
            // ----------------------------------------------------------
            Bitmap bmp = new Bitmap(300, 300);
            Graphics grp = Graphics.FromImage(bmp);
            grp.Clear(Color.White);
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 0, 300, 100));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 100, 300, 100));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 200, 300, 100));
            grp.TranslateTransform(300, 300);
            grp.RotateTransform(180);
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 0, 300, 100));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 100, 300, 100));
            grp.DrawString(PartNumber.Replace(".internal", ""), FT9, Brushes.Black, new Rectangle(0, 200, 300, 100));
            grp.Dispose();
            Texture.KDBitmap = bmp;
            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods
    }
}