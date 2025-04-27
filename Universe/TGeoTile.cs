using CiliaElements;
using SRTM_Geoid;
using Math3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Universe
{
    internal class TGeoTile : TSolidElement
    {
        #region Public Fields

        public static readonly double Delta = -10;

        public Vec3 P1, P2, P3;

        #endregion Public Fields

        #region Private Fields

        private static readonly int BmpBytes;
        private static readonly Rectangle BmpRec;

        private static readonly SolidBrush[] Brushes = new SolidBrush[]{
                new SolidBrush(Color.FromArgb(96, 96, 255)),
                new SolidBrush(Color.FromArgb(64, 192, 64)),
                new SolidBrush(Color.FromArgb(110, 199, 115)),
                new SolidBrush(Color.FromArgb(151,207,166)),
                new SolidBrush(Color.FromArgb(236,219,192)),
                new SolidBrush(Color.FromArgb(192, 192, 192)),
            };

        private static readonly Bitmap ForeGround;
        private static readonly int[] Indexes;
        private static readonly int m;
        private static readonly int mbitmap = 2;
        private static readonly int n;
        private static readonly int NbPoints;
        private static readonly double ninversed;
        private static readonly double pas;
        private static readonly PointF[] pnts;
        private static readonly DirectoryInfo Repository;
        private static readonly Bitmap SkyGround;
        private static readonly Vec2[] Texts;
        private static readonly int TileVersion = 1;
        private static readonly string TileVersionWord;
        private readonly double height = 0;
        private Ept E1, E2, E3;

        #endregion Private Fields

        #region Public Constructors

        static TGeoTile()
        {
            m = 128 / mbitmap;
            NbPoints = m * (m - 1) / 2;

            Repository = new DirectoryInfo("C:\\Users\\Public\\Documents\\GeoTiles");
            if (!Repository.Exists) Repository.Create();

            TileVersionWord = String.Format("{0:X4}_{1:X4}", TileVersion, m);

            BmpRec = new Rectangle(0, 0, m * mbitmap, m * mbitmap);
            BmpBytes = BmpRec.Width * BmpRec.Height * 4;

            n = m - 2;
            ninversed = 1.0 / (double)n;

            FileInfo fi = new FileInfo(Repository.FullName + "\\" + TileVersionWord + "_Foreground.png");

            Color c;
            Random rnd = new Random();
            if (!fi.Exists)
            {
                ForeGround = new Bitmap(BmpRec.Width, BmpRec.Height);

                for (int i = 0; i < ForeGround.Width; i++)
                    for (int j = 0; j < ForeGround.Height; j++)
                    {
                        c = Color.FromArgb(128,
                             128 + (byte)(64 * rnd.NextDouble()),
                             128 + (byte)(64 * rnd.NextDouble()),
                             128 + (byte)(64 * rnd.NextDouble()));
                        ForeGround.SetPixel(i, j, c);
                    }

                ForeGround.Save(fi.FullName);
            }
            else
                ForeGround = new Bitmap(fi.FullName);

            pas = 1.0 / (double)(n + 2.0);

            pnts = new PointF[6];

            pnts[0] = new PointF(-0.333333f, -0.333333f);
            pnts[1] = new PointF(+0.333333f, -0.666667f);
            pnts[2] = new PointF(+0.666667f, -0.333333f);
            pnts[3] = new PointF(+0.333333f, +0.333333f);
            pnts[4] = new PointF(-0.333333f, +0.666667f);
            pnts[5] = new PointF(-0.666667f, +0.333333f);

            int[][] indexes = new int[n + 1][];
            int[] indx;

            int nidx = 0;

            List<Vec2> lst = new List<Vec2>();

            for (int ni = 0; ni <= n; ni++)
            {
                indx = new int[n - ni + 1];
                indexes[ni] = indx;
                for (int nj = 0; nj <= n - ni; nj++)
                {
                    indx[nj] = nidx; nidx++;
                    lst.Add(new Vec2(pas + ni * pas, 1 - pas - pas * nj));
                }
            }

            Texts = lst.ToArray();

            List<int> lsti = new List<int>();

            for (int i = 0; i <= n; i++)
                for (int j = n - i - 1; j >= 0; j--)
                {
                    lsti.Add(indexes[i][j]);
                    lsti.Add(indexes[i][j + 1]);
                    lsti.Add(indexes[i + 1][j]);
                }
            for (int i = 1; i <= n; i++)
                for (int j = n - i - 1; j >= 0; j--)
                {
                    lsti.Add(indexes[i][j]);
                    lsti.Add(indexes[i][j + 1]);
                    lsti.Add(indexes[i - 1][j + 1]);
                }

            Indexes = lsti.ToArray();

            fi = new FileInfo(Repository.FullName + "\\" + TileVersionWord + "_Sky.png");

            if (!fi.Exists)
            {
                SkyGround = new Bitmap(512, 512);

                double d;
                for (int i = 0; i < SkyGround.Width; i++)
                    for (int j = 0; j < SkyGround.Height; j++)
                    {
                        d = rnd.NextDouble();//d *= d;
                        c = Color.FromArgb((byte)(112 + 40 * d), (byte)(145 + 22 * d), (byte)(255 + 0 * d));
                        SkyGround.SetPixel(i, j, c);
                    }
                SkyGround.Save(fi.FullName);
            }
            else
                SkyGround = new Bitmap(fi.FullName);
        }

        public TGeoTile(Vec3 p1, Vec3 p2, Vec3 p3, double iheight = 0)// : base("GeoTile-" + iheight.ToString() + "-" + p1.ToString() + ";" + p2.ToString() + ";" + p3.ToString())
        {
            PartNumber = "GeoTile-" + iheight.ToString() + "-" + p1.ToString() + ";" + p2.ToString() + ";" + p3.ToString();
            height = iheight;
            P1 = p1; P2 = p2; P3 = p3;
            E1 = new Ept(p1, 0); TSRTMGeoid.SetGround(ref E1, height + Delta); P1 = (Vec3)E1;
            E2 = new Ept(p2, 0); TSRTMGeoid.SetGround(ref E2, height + Delta); P2 = (Vec3)E2;
            E3 = new Ept(p3, 0); TSRTMGeoid.SetGround(ref E3, height + Delta); P3 = (Vec3)E3;
            TFile file = new TFile(new FileInfo("c:\\tmp\\" + PartNumber), null)
            {
                Element = this
            };
        }

        #endregion Public Constructors

        #region Private Properties

        private string TileName
        {
            get
            {
                return Repository.FullName + "\\" +
                    TileVersionWord + "_" +
                    string.Format("{0:0}_{1:0}_{2:0}", P1.X, P1.Y, P1.Z) + "_" +
                    string.Format("{0:0}_{1:0}_{2:0}", P2.X, P2.Y, P2.Z) + "_" +
                    string.Format("{0:0}_{1:0}_{2:0}", P3.X, P3.Y, P3.Z);
            }
        }

        #endregion Private Properties

        #region Public Methods

        public override void LaunchLoad()
        {
            Bitmap bmp;
            TCloud Positions = SolidElementConstruction.AddCloud();
            TCloud Normals = SolidElementConstruction.AddCloud();
            TFGroup faces = SolidElementConstruction.AddFGroup();
            TTextureCloud Textures;
            SolidElementConstruction.StartGroup.FGroups.Push(faces);
            faces.ShapeGroupParameters.Positions = Positions;
            faces.ShapeGroupParameters.Normals = Normals;
            faces.GroupParameters.Texture = SolidElementConstruction.AddTexture(1, 0.6, 0.6, 0.6);
            faces.ShapeGroupParameters.Textures = SolidElementConstruction.AddTextureCloud();
            Textures = faces.ShapeGroupParameters.Textures;

            Vec3 v;
            if (height > 0)
            {
                bmp = SkyGround;
                v = P1; Positions.Vectors.Push(new Vec3(0, 0, 0)); v.Normalize(); Normals.Vectors.Push(v);
                v = P2; Positions.Vectors.Push(P2 - P1); v.Normalize(); Normals.Vectors.Push(v);
                v = P3; Positions.Vectors.Push(P3 - P1); v.Normalize(); Normals.Vectors.Push(v);
                faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0.1, 0.1));
                faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0.9, 0.9));
                faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2(0.1, 0.9));
                faces.Indexes.Push(new int[] { 0, 1, 2 });
            }
            else
            {
                Stopwatch sw = new Stopwatch(); sw.Start();
                FileInfo fi = new FileInfo(TileName);
                if (!fi.Exists)
                {
                    bmp = new Bitmap(m * mbitmap, m * mbitmap);

                    Graphics grp = Graphics.FromImage(bmp);
                    grp.Clear(Color.Red);
                    grp.ScaleTransform(mbitmap, mbitmap);

                    Ept e;

                    Vec3 Pi;

                    Vec3[] Pjs = new Vec3[n + 1];
                    for (int nj = 0; nj <= n; nj++)
                        Pjs[nj] = nj * (P1 - P3) * ninversed;

                    grp.TranslateTransform(1, 1);
                    for (int ni = 0, mi = n; ni <= n; ni++, mi--)
                    {
                        Pi = P3 + ni * (P2 - P3) * ninversed;
                        for (int nj = 0; nj <= mi; nj++)
                        {
                            //
                            e = new Ept(Pi + Pjs[nj], 0);

                            TSRTMGeoid.SetGround(ref e, Delta);
                            e.Hei -= Ept.Geoid.ConvertHeightToEllipsoid(e.Lat, e.Lon, 0);
                            if (e.Hei < Delta)
                            {
                                //e.Hei = Delta;
                                grp.FillPolygon(Brushes[0], pnts);
                            }
                            else
                            {
                                grp.FillPolygon(Brushes[(int)(Math.Min(4, e.Hei * 0.002)) + 1], pnts);
                            }
                            //
                            v = (Vec3)e; Positions.Vectors.Push(v - P1);
                            v = P1; v.Normalize();
                            Normals.Vectors.Push(v);
                            grp.TranslateTransform(0, 1);
                        }
                        grp.TranslateTransform(1, -mi - 1);
                    }
                    grp.ResetTransform();

                    grp.DrawImage(ForeGround, 0, 0);

                    grp.Dispose();

                    Textures.Vectors.Push(Texts);
                    faces.Indexes.Push(Indexes);

                    StreamWriter wtr = new StreamWriter(fi.FullName);
                    BinaryWriter bwtr = new BinaryWriter(wtr.BaseStream);

                    bwtr.Write(P1.X); bwtr.Write(P1.Y); bwtr.Write(P1.Z);
                    for (int i = 0; i < NbPoints; i++)
                    {
                        v = Positions.Vectors.Values[i];
                        bwtr.Write(v.X); bwtr.Write(v.Y); bwtr.Write(v.Z);
                        v = Normals.Vectors.Values[i];
                        bwtr.Write(v.X); bwtr.Write(v.Y); bwtr.Write(v.Z);
                    }

                    BitmapData bdata = bmp.LockBits(BmpRec, ImageLockMode.ReadOnly, bmp.PixelFormat);
                    byte[] bts = new byte[BmpBytes];
                    Marshal.Copy(bdata.Scan0, bts, 0, BmpBytes);
                    bmp.UnlockBits(bdata);

                    bwtr.Write(bts);
                    wtr.Close(); wtr.Dispose();
                    GC.SuppressFinalize(bts); GC.SuppressFinalize(bdata);

                    sw.Stop();

                    //TEnvironment.LogText("GeoTile Generation", "Built in " + sw.ElapsedMilliseconds.ToString() + "ms");
                }
                else
                {
                    Fi = fi;
                    v = new Vec3();
                    StreamReader rdr = new StreamReader(fi.FullName);
                    BinaryReader brdr = new BinaryReader(rdr.BaseStream);
                    P1.X = brdr.ReadDouble(); P1.Y = brdr.ReadDouble(); P1.Z = brdr.ReadDouble();
                    Vec3[] vs = new Vec3[NbPoints];
                    Vec3[] ns = new Vec3[NbPoints];
                    for (int i = 0; i < NbPoints; i++)
                    {
                        v.X = brdr.ReadDouble(); v.Y = brdr.ReadDouble(); v.Z = brdr.ReadDouble();
                        vs[i] = v;
                        v.X = brdr.ReadDouble(); v.Y = brdr.ReadDouble(); v.Z = brdr.ReadDouble();
                        ns[i] = v;
                    }
                    Positions.Vectors.Push(vs);

                    Normals.Vectors.Push(ns);

                    bmp = new Bitmap(BmpRec.Width, BmpRec.Height);

                    BitmapData bdata = bmp.LockBits(BmpRec, ImageLockMode.WriteOnly, bmp.PixelFormat);
                    byte[] bts = brdr.ReadBytes(BmpBytes);
                    Marshal.Copy(bts, 0, bdata.Scan0, BmpBytes);
                    bmp.UnlockBits(bdata);

                    rdr.Close(); rdr.Dispose();
                    GC.SuppressFinalize(bts); GC.SuppressFinalize(bdata);

                    Textures.Vectors.Push(Texts);
                    faces.Indexes.Push(Indexes);
                    sw.Stop();

                    //TEnvironment.LogText("GeoTile Generation", "Loaded in " + sw.ElapsedMilliseconds.ToString() + "ms");
                }
            }

            SolidElementConstruction.TextRange = bmp.Width;
            faces.GroupParameters.Texture.KDBitmap = bmp;

            //  ToBeTrashed = true;

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}