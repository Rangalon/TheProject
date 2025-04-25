
using CiliaElements.FormatJson;
using Math3D;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace CiliaElements.Elements.Internals
{
    public class TAnnotation : TSolidElement
    {

        public TAnnotation(string pn) : base()
        {
            PartNumber = pn;
            Fi = new FileInfo(pn + ".annotation");
        }

        private static Mtx4 StartMatrix = Mtx4.Scale(0.001, 0.001, 0.001);

        public override void LaunchLoad()
        {
            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;
            //
            ElementLoader.Publish();
        }

        static Regex RgxRGB = new Regex(@"RGB[(]([0-9.]*),([0-9.]*),([0-9.]*)[)]");

        internal void AddJsonBloc(TJBloc b)
        {
            TJBloc pb;
            pb = b.FindRecursive("FontStyle");
            TTexture TexFont = null;
            Font Ft = null;
            if (pb != null)
            {
                pb = pb.FindRecursive("color");
                Match m = RgxRGB.Match(pb.Value);
                TexFont = SolidElementConstruction.AddTexture(1, double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture));
                string FtName = "Verdana";
                if (pb.Parent.Find("familyName") != null) FtName = pb.Parent.Find("familyName").Value;
                float FtSize = 20;
                if (pb.Parent.Find("fontSize") != null) FtSize = 2.56F * float.Parse(pb.Parent.Find("fontSize").Value, CultureInfo.InvariantCulture);
                Ft = new Font(FtName, FtSize);
            }
            pb = b.FindRecursive("FillStyle");
            TTexture TexFill = null;
            if (pb != null)
            {
                pb = pb.FindRecursive("color");
                Match m = RgxRGB.Match(pb.Value);
                TexFill = SolidElementConstruction.AddTexture(1, double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture));
            }
            pb = b.FindRecursive("LeaderStyle");
            TTexture TexLeader = null;
            if (pb != null)
            {
                pb = pb.FindRecursive("color");
                Match m = RgxRGB.Match(pb.Value);
                TexLeader = SolidElementConstruction.AddTexture(1, double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture));
            }
            pb = b.FindRecursive("LineStyle");
            TTexture TexLine = null;
            if (pb != null)
            {
                pb = pb.FindRecursive("color");
                Match m = RgxRGB.Match(pb.Value);
                TexLine = SolidElementConstruction.AddTexture(1, double.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture), double.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture));
            }
            // ---------------------------------------
            Mtx4 mtx = Mtx4.Identity;
            pb = b.FindRecursive("RelativePosMatrix");
            if (pb != null)
            {
                Vec4 v;
                pb = pb.Find("Origin");
                v.X = double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture);
                v.Y = double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture);
                v.Z = double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture);
                v.W = 1;
                mtx.Row3 = v;
                pb = pb.Parent.Find("X");
                v.X = double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture);
                v.Y = double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture);
                v.Z = double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture);
                v.W = 1;
                mtx.Row0 = v;
                pb = pb.Parent.Find("Y");
                v.X = double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture);
                v.Y = double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture);
                v.Z = double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture);
                v.W = 1;
                mtx.Row1 = v;
                pb = pb.Parent.Find("Z");
                v.X = double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture);
                v.Y = double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture);
                v.Z = double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture);
                v.W = 1;
                mtx.Row2 = v;
            }
            // ---------------------------------------
            pb = b.FindRecursive("LeaderPointingPoints");
            Vec3 Anchor = new Vec3();
            if (TexLine != null && pb != null)
            {
                TCloud cloud = SolidElementConstruction.AddCloud();
                pb = pb.FirstChild;
                cloud.Vectors.Push(new Vec3(double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture), double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture), double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture)));
                pb = b.FindRecursive("LeaderBreakPoint3D");
                cloud.Vectors.Push(new Vec3(double.Parse(pb.Blocs[0].Name, CultureInfo.InvariantCulture), double.Parse(pb.Blocs[1].Name, CultureInfo.InvariantCulture), double.Parse(pb.Blocs[2].Name, CultureInfo.InvariantCulture)));
                TPGroup pgroup = SolidElementConstruction.AddPGroup();
                SolidElementConstruction.StartGroup.PGroups.Push(pgroup);
                pgroup.GroupParameters.Texture = TexLine;
                pgroup.ShapeGroupParameters.Positions = cloud;
                pgroup.Indexes.Push(0);
                pgroup.Indexes.Push(1);
                //
                TLGroup lgroup = SolidElementConstruction.AddLGroup();
                SolidElementConstruction.StartGroup.LGroups.Push(lgroup);
                lgroup.GroupParameters.Texture = TexLine;
                lgroup.ShapeGroupParameters.Positions = cloud;
                lgroup.Indexes.Push(0);
                lgroup.Indexes.Push(1);

                Anchor = cloud.Vectors.Values[1];
            }

            //
            pb = b.FindRecursive("imgSrc");
            if (pb != null)
            {
                string base64 = pb.Value.Substring(23, pb.Value.Length - 24);
                byte[] bytes = System.Convert.FromBase64String(base64);
                //byte[] buffer = File.ReadAllBytes(fileName);
                MemoryStream memoryStream = new MemoryStream(bytes)
                {
                    Position = 0
                };

                Bitmap pic = new Bitmap(memoryStream);

                //pic.Save("c:\\temp\\" + DateTime.Now.Ticks.ToString() + ".png", ImageFormat.Png);


                TFGroup fgroup = SolidElementConstruction.AddFGroup();
                SolidElementConstruction.StartGroup.FGroups.Push(fgroup);
                fgroup.GroupParameters.Texture = SolidElementConstruction.AddTexture(TexFill.A, TexFill.R, TexFill.G, TexFill.B);
                fgroup.GroupParameters.Texture.KDBitmap = pic;
                fgroup.ShapeGroupParameters.Positions = SolidElementConstruction.AddCloud();
                fgroup.ShapeGroupParameters.Normals = SolidElementConstruction.AddCloud();
                fgroup.ShapeGroupParameters.Textures = SolidElementConstruction.AddTextureCloud();

                Vec4 vu = Vec4.Transform(new Vec4(1, 0, 0, 0), mtx);
                Vec4 vv = Vec4.Transform(new Vec4(0, 1, 0, 0), mtx);
                Vec4 vw = Vec4.Transform(new Vec4(0, 0, 1, 0), mtx);
                pb = pb.Parent.Find("imgHeight");
                double h = double.Parse(pb.Value, CultureInfo.InvariantCulture);
                pb = pb.Parent.Find("imgWidth");
                double w = double.Parse(pb.Value, CultureInfo.InvariantCulture);

                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor);
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(w * vu));
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(w * vu + h * vv));
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(h * vv));

                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.Indexes.Push(0);
                fgroup.Indexes.Push(1);
                fgroup.Indexes.Push(2);
                fgroup.Indexes.Push(0);
                fgroup.Indexes.Push(2);
                fgroup.Indexes.Push(3);
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0, 0));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0, 1));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(1, 1));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(1, 0));

                //pic.Dispose();
                memoryStream.Dispose();
            }
            //LaunchLoad();
            //
            pb = b.FindRecursive("TextContents");
            if (pb != null && pb.FirstChild.Name != "")
            {

                string s = pb.FirstChild.Name;

                pb = pb.Parent.Find("TextBoxLength");
                double h = double.Parse(pb.Value, CultureInfo.InvariantCulture);
                pb = pb.Parent.Find("TextBoxWidth");
                double w = double.Parse(pb.Value, CultureInfo.InvariantCulture);



                Bitmap pic = new Bitmap(256, (int)(256 * h / w));
                Graphics grp = Graphics.FromImage(pic);
                grp.Clear(Color.FromArgb((int)(TexFill.A * 255), (int)(TexFill.R * 255), (int)(TexFill.G * 255), (int)(TexFill.B * 255)));
                grp.DrawString(s, Ft, new SolidBrush(Color.FromArgb((int)(TexLine.A * 255), (int)(TexLine.R * 255), (int)(TexLine.G * 255), (int)(TexLine.B * 255))), 0, 0);
                grp.Dispose();
                //pic.Save("c:\\temp\\" + DateTime.Now.Ticks.ToString() + ".png", ImageFormat.Png);


                TFGroup fgroup = SolidElementConstruction.AddFGroup();
                SolidElementConstruction.StartGroup.FGroups.Push(fgroup);
                fgroup.GroupParameters.Texture = SolidElementConstruction.AddTexture(TexFill.A, TexFill.R, TexFill.G, TexFill.B);
                fgroup.GroupParameters.Texture.KDBitmap = pic;
                fgroup.ShapeGroupParameters.Positions = SolidElementConstruction.AddCloud();
                fgroup.ShapeGroupParameters.Normals = SolidElementConstruction.AddCloud();
                fgroup.ShapeGroupParameters.Textures = SolidElementConstruction.AddTextureCloud();

                Vec4 vu = Vec4.Transform(new Vec4(1, 0, 0, 0), mtx);
                Vec4 vv = Vec4.Transform(new Vec4(0, 1, 0, 0), mtx);
                Vec4 vw = Vec4.Transform(new Vec4(0, 0, 1, 0), mtx);

                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor);
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(w * vu));
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(w * vu + h * vv));
                fgroup.ShapeGroupParameters.Positions.Vectors.Push(Anchor + (Vec3)(h * vv));

                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.ShapeGroupParameters.Normals.Vectors.Push((Vec3)vw);
                fgroup.Indexes.Push(0);
                fgroup.Indexes.Push(1);
                fgroup.Indexes.Push(2);
                fgroup.Indexes.Push(0);
                fgroup.Indexes.Push(2);
                fgroup.Indexes.Push(3);
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0, 0));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(1, 0));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(1, 1));
                fgroup.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0, 1));

            }
        }
    }
}
