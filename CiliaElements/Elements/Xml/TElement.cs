
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CiliaElements.FormatXml
{
    public class TElement : TSolidElement
    {
        #region Private Fields

        private static readonly Mtx4[] SidesMatrixes = new Mtx4[]{
            Mtx4.CreateRotationX(Math.PI*0.25),
            Mtx4.CreateRotationX(Math.PI*0.75),
            Mtx4.CreateRotationX(Math.PI*1.25),
            Mtx4.CreateRotationX(Math.PI*1.75)};

        private static Vec4 XOffset = new Vec4(0.005F, 0, 0, 0);
        private static Vec4 YOffset = new Vec4(0, 0.005F, 0, 0);
        private static readonly float yRatio = 0.5F;
        private static readonly float zRatio = 0.5F * (float)Math.Sqrt(3);

        private System.Xml.XmlElement XmlElement;

        #endregion Private Fields

        #region Public Constructors

        public TElement(System.Xml.XmlElement e)
        {
            XmlElement = e;
            PartNumber = XmlElement.SelectSingleNode("./@ID").Value + ".element";
            //
            Fi = new System.IO.FileInfo(PartNumber);
            //
            TFile file = new TFile(Fi, null)
            {
                Element = this
            };
            //
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            //System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ".";

            if (XmlElement.Name == "BRA") LoadBranch();
            if (XmlElement.Name == "EXT") LoadExtremity();
            if (XmlElement.Name == "DER") LoadDerivation();
            if (XmlElement.Name == "REN") LoadXRef();
            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private static Vec3 GetPoint(System.Xml.XmlNode e)
        {
            if (e == null) return new Vec3(0, 0, 0);
            return new Vec3(
                 float.Parse(e.Attributes.GetNamedItem("X").Value, CultureInfo.InvariantCulture),
                 float.Parse(e.Attributes.GetNamedItem("Y").Value, CultureInfo.InvariantCulture),
                 float.Parse(e.Attributes.GetNamedItem("Z").Value, CultureInfo.InvariantCulture));
        }

        private void AddPipe(TTexture t, List<Vec3> tmp, float r)
        {
            TCloud c4 = this.SolidElementConstruction.AddCloud();
            TFGroup g4 = this.SolidElementConstruction.AddFGroup();
            g4.ShapeGroupParameters.Positions = c4;
            g4.GroupParameters.Texture = t;
            //Array.Resize(ref c4.Vectors, tmp.Count * 3);
            //Array.Resize(ref g4.Indexes, (tmp.Count - 1) * 18);
            //
            Vec3 x = tmp[1] - tmp[0];
            Vec3 y;
            if (x.X == x.Y && x.Y == x.Z) y = new Vec3(0, 1, 0);
            else y = new Vec3(x.Y, x.Z, x.X);
            Vec3 z = Vec3.Cross(x, y);
            y = Vec3.Cross(z, x);
            y.Normalize();
            z.Normalize();
            //
            y *= r;
            z *= r;
            c4.Vectors.Push(tmp[0] + y);
            c4.Vectors.Push(tmp[0] - 0.5F * y + 0.5F * (float)Math.Sqrt(3) * z);
            c4.Vectors.Push(tmp[0] - 0.5F * y - 0.5F * (float)Math.Sqrt(3) * z);
            //
            for (int i = 1; i < tmp.Count; i++)
            {
                x = tmp[i] - tmp[i - 1];
                y = new Vec3(x.Y, x.Z, -x.X);
                z = Vec3.Cross(x, y);
                y = Vec3.Cross(z, x);
                y.Normalize();
                z.Normalize();
                y *= r * yRatio;
                z *= r * zRatio;
                c4.Vectors.Push(tmp[i] + 2 * y);
                c4.Vectors.Push(tmp[i] - y + z);
                c4.Vectors.Push(tmp[i] - y - z);
                g4.Indexes.Values[i * 18 - 18] = i * 3 - 3;
                g4.Indexes.Values[i * 18 - 17] = i * 3 - 2;
                g4.Indexes.Values[i * 18 - 16] = i * 3 - 0;
                g4.Indexes.Values[i * 18 - 15] = i * 3 - 2;
                g4.Indexes.Values[i * 18 - 14] = i * 3 - 1;
                g4.Indexes.Values[i * 18 - 13] = i * 3 + 1;
                g4.Indexes.Values[i * 18 - 12] = i * 3 - 1;
                g4.Indexes.Values[i * 18 - 11] = i * 3 - 3;
                g4.Indexes.Values[i * 18 - 10] = i * 3 + 2;
                g4.Indexes.Values[i * 18 - 09] = i * 3 - 2;
                g4.Indexes.Values[i * 18 - 08] = i * 3 + 1;
                g4.Indexes.Values[i * 18 - 07] = i * 3 + 0;
                g4.Indexes.Values[i * 18 - 06] = i * 3 - 1;
                g4.Indexes.Values[i * 18 - 05] = i * 3 + 2;
                g4.Indexes.Values[i * 18 - 04] = i * 3 + 1;
                g4.Indexes.Values[i * 18 - 03] = i * 3 - 3;
                g4.Indexes.Values[i * 18 - 02] = i * 3 + 0;
                g4.Indexes.Values[i * 18 - 01] = i * 3 + 2;
            }
            this.SolidElementConstruction.StartGroup.FGroups.Push(g4);
        }

        private void LoadBranch()
        {
            System.Xml.XmlNodeList lst = XmlElement.SelectNodes("./INFO3D_COORD/Point");
            List<Vec3> pts = new List<Vec3>();
            float d = 0.25F;
            int n = (int)(1F / d) + 1;
            Vec3 LastP = GetPoint(lst[0]);
            Vec3 LastT = 0.03F * GetPoint(lst[0].ParentNode.SelectSingleNode("./Vector"));
            for (int i = 1; i < lst.Count; i++)
            {
                Vec3 p = GetPoint(lst[i]);
                Vec3 t = 0.03F * GetPoint(lst[i].ParentNode.SelectSingleNode("./Vector"));
                //
                Vec3 a0 = LastP;
                Vec3 a1 = LastT;
                Vec3 a3 = t - 2 * p + a1 + 2 * a0;
                Vec3 a2 = p - a0 - a1 - a3;
                for (float j = 0; j < n; j++)
                {
                    pts.Add(a0 + d * j * (a1 + d * j * (a2 + d * j * a3)));
                }
                //
                LastP = p;
                LastT = t;
            }
            //
            TTexture t2 = SolidElementConstruction.AddTexture(1, 1, 0, 0);
            TCloud c4;
            TShapeGroup g4;
            //
            c4 = this.SolidElementConstruction.AddCloud();
            g4 = this.SolidElementConstruction.AddFGroup();
            g4.ShapeGroupParameters.Positions = c4;
            g4.GroupParameters.Texture = t2;
            //
            float r = 0.0005F * 5;// Single.Parse(XmlElement.SelectSingleNode("./DIAMETER").InnerText);
            AddPipe(t2, pts, r);
        }

        private void LoadDerivation()
        {
            TTexture t = SolidElementConstruction.AddTexture(1, 0, 1, 1);
            TCloud c = SolidElementConstruction.AddCloud();
            TFGroup g = SolidElementConstruction.AddFGroup();
            g.GroupParameters.Texture = t;
            g.ShapeGroupParameters.Positions = c;
            //Array.Resize(ref c.Vectors, 4);
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            c.Vectors.Values[0].X += 0.007F; c.Vectors.Values[0].Y += 0.007F; c.Vectors.Values[0].Z -= 0.007F;
            c.Vectors.Values[1].X += 0.007F; c.Vectors.Values[1].Y -= 0.007F; c.Vectors.Values[1].Z += 0.007F;
            c.Vectors.Values[2].X -= 0.007F; c.Vectors.Values[2].Y += 0.007F; c.Vectors.Values[2].Z += 0.007F;
            c.Vectors.Values[3].X -= 0.007F; c.Vectors.Values[3].Y -= 0.007F; c.Vectors.Values[3].Z -= 0.007F;
            g.Indexes.Push(new int[] { 0, 1, 3, 1, 2, 3, 2, 3, 0, 0, 1, 2 });
            this.SolidElementConstruction.StartGroup.FGroups.Push(g);
        }

        private void LoadExtremity()
        {
            TTexture t = SolidElementConstruction.AddTexture(1, 0, 0, 1);
            TCloud c = SolidElementConstruction.AddCloud();
            TFGroup g = SolidElementConstruction.AddFGroup();
            g.GroupParameters.Texture = t;
            g.ShapeGroupParameters.Positions = c;
            //Array.Resize(ref c.Vectors, 10);
            c.Vectors.Push(new Vec3(0.131F, 0, 0));
            c.Vectors.Push(new Vec3(0, 0, 0));
            c.Vectors.Push(new Vec3(0.05F, 0.02F, 0));
            c.Vectors.Push(new Vec3(0.05F, -0.02F, 0));
            c.Vectors.Push(new Vec3(0.05F, 0, 0.02F));
            c.Vectors.Push(new Vec3(0.05F, 0, -0.02F));
            c.Vectors.Push(new Vec3(0.13F, 0.02F, 0));
            c.Vectors.Push(new Vec3(0.13F, -0.02F, 0));
            c.Vectors.Push(new Vec3(0.13F, 0, 0.02F));
            c.Vectors.Push(new Vec3(0.13F, 0, -0.02F));
            g.Indexes.Push(new int[] { 2, 1, 4, 0, 6, 8, 7, 0, 8, 1, 3, 4, 1, 2, 5, 6, 0, 9, 0, 7, 9, 3, 1, 5, 5, 2, 9, 3, 5, 7, 4, 3, 8, 2, 4, 6, 7, 5, 9, 8, 3, 7, 6, 4, 8, 9, 2, 6 });
            this.SolidElementConstruction.StartGroup.FGroups.Push(g);
            //
            t = SolidElementConstruction.AddTexture(1, 1, 1, 0);
            Vec4 Offset = new Vec4(0, -0.0144F, -0.0144F, 0);
            foreach (System.Xml.XmlNode n in XmlElement.SelectNodes("./FIN"))
            {
                string s = n.Attributes.GetNamedItem("ID").Value;
                //
                Offset.X = 0.052F;
                Offset += YOffset;
                foreach (char cr in s.ToCharArray())
                {
                    //SLine[] lines = TLetter.LettersDico[cr];
                    //for (int j = 0; j < 4; j++)
                    //{
                    //    c = SolidElementConstruction.AddCloud();
                    //    TLGroup g1 = SolidElementConstruction.AddLGroup();
                    //    g1.GroupParameters.Texture = t;
                    //    g1.ShapeGroupParameters.Positions = c;
                    //    //
                    //    List<Vec3> lst = new List<Vec3>();
                    //    Int32 i = 0;
                    //    //Array.Resize(ref g1.Indexes, lines.Length * 3);
                    //    foreach (SLine line in lines)
                    //    {
                    //        g1.Indexes.Values[i * 3] = i * 2;
                    //        g1.Indexes.Values[i * 3 + 1] = i * 2 + 1;
                    //        g1.Indexes.Values[i * 3 + 2] = -1;
                    //        i++;
                    //        lst.Add((Vec3)(line.StartPoint * -2 * ScaleFactor + Offset));
                    //        lst.Add((Vec3)(line.EndPoint * -2 * ScaleFactor + Offset));
                    //    }
                    //    c.Vectors.Push(lst.ToArray());
                    //    g1.GroupParameters.Matrix = SidesMatrixes[j];
                    //    SolidElementConstruction.StartGroup.LGroups.Push(g1);
                    //}
                    //Offset += XOffset;
                }
                //
                CustomAttributes.Add(new TCustomAttr { Name = "FIN", Value = n.Attributes.GetNamedItem("ID").Value });
            }
            //
        }

        private void LoadXRef()
        {
            TTexture t = SolidElementConstruction.AddTexture(1, 1, 1, 1);
            TCloud c = SolidElementConstruction.AddCloud();
            TCloud cn = SolidElementConstruction.AddCloud();
            TFGroup g = SolidElementConstruction.AddFGroup();
            g.GroupParameters.Texture = t;
            g.ShapeGroupParameters.Positions = c;
            g.ShapeGroupParameters.Normals = cn;
            //Array.Resize(ref c.Vectors, 4);
            //Array.Resize(ref cn.Vectors, 4);
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            c.Vectors.Push(new Vec3());
            cn.Vectors.Push(new Vec3());
            cn.Vectors.Push(new Vec3());
            cn.Vectors.Push(new Vec3());
            cn.Vectors.Push(new Vec3());
            c.Vectors.Values[0].X += 0.015F; c.Vectors.Values[0].Y += 0.015F; c.Vectors.Values[0].Z -= 0.015F;
            c.Vectors.Values[1].X += 0.015F; c.Vectors.Values[1].Y -= 0.015F; c.Vectors.Values[1].Z += 0.015F;
            c.Vectors.Values[2].X -= 0.015F; c.Vectors.Values[2].Y += 0.015F; c.Vectors.Values[2].Z += 0.015F;
            c.Vectors.Values[3].X -= 0.015F; c.Vectors.Values[3].Y -= 0.015F; c.Vectors.Values[3].Z -= 0.015F;
            cn.Vectors.Values[0].X += 0.015F; cn.Vectors.Values[0].Y += 0.015F; cn.Vectors.Values[0].Z -= 0.015F;
            cn.Vectors.Values[1].X += 0.015F; cn.Vectors.Values[1].Y -= 0.015F; cn.Vectors.Values[1].Z += 0.015F;
            cn.Vectors.Values[2].X -= 0.015F; cn.Vectors.Values[2].Y += 0.015F; cn.Vectors.Values[2].Z += 0.015F;
            cn.Vectors.Values[3].X -= 0.015F; cn.Vectors.Values[3].Y -= 0.015F; cn.Vectors.Values[3].Z -= 0.015F;
            g.Indexes.Push(new int[] { 0, 1, 3, 1, 2, 3, 2, 3, 0, 0, 1, 2 });
            this.SolidElementConstruction.StartGroup.FGroups.Push(g);
        }

        #endregion Private Methods
    }
}