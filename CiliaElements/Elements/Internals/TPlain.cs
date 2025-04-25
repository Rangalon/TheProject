using CiliaElements.Elements.Math;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Internals
{
    public class TPlain : TInternal
    {
        #region Public Fields

        public static readonly TPlain Default = new TPlain(0);
        public static readonly TPlain[] Resources = new TPlain[8];

        #endregion Public Fields

        #region Private Fields

        private static readonly TRandom Rnd = new TRandom();

        private readonly int rank = 0;

        #endregion Private Fields

        #region Public Constructors

        static TPlain()
        {
            Default = new TPlain(0); Default.Prepare();
            for (int i = 0; i < 8; i++)
            { Resources[i] = new TPlain(i + 1); Resources[i].Prepare(); }
        }

        public TPlain(int i) : base("Plain" + i.ToString())
        {
            rank = i;
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private static double GetExternalLevel()
        {
            return 0.05 * (Rnd.NextUDouble() - 1.1);
        }

        private void Prepare()
        {
            lock (Rnd)
            {
                double sqrt3 = System.Math.Sqrt(3);

                TTexture Texture = null;
                TFGroup Faces = SolidElementConstruction.AddFGroup();
                SolidElementConstruction.StartGroup.FGroups.Push(Faces);
                Faces.ShapeGroupParameters.Positions = SolidElementConstruction.AddCloud();
                Faces.ShapeGroupParameters.Normals = SolidElementConstruction.AddCloud();
                Faces.ShapeGroupParameters.Textures = SolidElementConstruction.AddTextureCloud();// new TTextureCloud();
                //Faces.ShapeGroupParameters.Normals = Normals;
                Texture = SolidElementConstruction.AddTexture(1, 0, 1, 0);
                Texture.KDBitmap = Properties.Resources.grass_grass_0131_01_s;
                Faces.GroupParameters.Texture = Texture;

                int nb = 27;
                int Nb = nb * 3 + 3;

                Vec3[] lstout = new Vec3[Nb];
                Vec3[] lstend = new Vec3[Nb];
                Vec3[] lstin = new Vec3[Nb];
                Vec3[] tmplst = new Vec3[nb];

                double defect = 0.1;// 35;

                double radius = 1.1 * System.Math.Sqrt(2) / 2;

                // ---------------------------------------------------------------------------------------------------------------------------------------------------

                Vec3 middle = new Vec3(0, 50 * System.Math.Sqrt(3), 0);
                Mtx4 mtr = Mtx4.CreateTranslation(0, System.Math.Sqrt(3) / 6, 0);
                int kj = 0;
                for (int j = 0; j < 3; j++)
                {
                    Mtx4 mrot = Mtx4.CreateRotationZ(System.Math.PI * j * 2 / 3) * mtr;
                    for (int i = 0; i < nb + 1; i++, kj++)
                    {
                        double a = -1 * System.Math.PI / 12 + (i + 1) * (2 * System.Math.PI / 3) / (nb + 1);
                        double ratio = System.Math.Abs((double)i - (double)(nb - 1) * 0.5) / ((double)nb + 1) + 0.5;
                        //ratio *= ratio * (1 - System.Math.Sqrt(2) * 0.5); ratio += System.Math.Sqrt(2) * 0.5;
                        ratio *= 1 + Rnd.NextUDouble() * 0.1;
                        lstout[kj] = Vec3.Transform(new Vec3((radius + defect) * System.Math.Cos(a) * ratio, (radius + defect) * System.Math.Sin(a) * ratio, GetExternalLevel()), mrot);
                        lstin[kj] = Vec3.Transform(new Vec3((radius - 0.0 * defect) * System.Math.Cos(a) * ratio, (radius - 0.0 * defect) * System.Math.Sin(a) * ratio, GetExternalLevel()), mrot);
                        lstend[kj] = Vec3.Transform(new Vec3((radius + 2 * defect) * System.Math.Cos(a) * ratio, (radius + 2 * defect) * System.Math.Sin(a) * ratio, -0.18), mrot);
                    }
                }

                // ---------------------------------------------------------------------------------------------------------------------------------------------------

                for (int i = 0; i < Nb; i++)
                    Faces.ShapeGroupParameters.Positions.Vectors.Push(lstout[i] * 300);

                for (int i = 0; i < Nb; i++)
                    Faces.ShapeGroupParameters.Positions.Vectors.Push(lstin[i] * 300);

                // ---------------------------------------------------------------------------------------------------------------------------------------------------

                int startout = 0;
                int startin = Nb;
                for (int i = 0; i < Nb; i++)
                    Faces.Indexes.Push(new int[] { startin + i, startin + (i + 1) % lstin.Length, startout + i, startout + i, startin + (i + 1) % lstin.Length, startout + (i + 1) % lstout.Length });

                // ---------------------------------------------------------------------------------------------------------------------------------------------------

                if (true)
                {
                    Vec3[] border = lstout;

                    lstout = lstin;
                    startout = startin;
                    while (lstout.Length > 3)
                    {
                        int n = lstout.Length / 3;
                        int nn = n - 3;
                        Array.Resize(ref lstin, nn * 3);
                        lstin[1 * n - 4] = 0.5 * (lstout[1 * n - 3] + lstout[1 * n + 1]);
                        lstin[2 * n - 7] = 0.5 * (lstout[2 * n - 3] + lstout[2 * n + 1]);
                        lstin[3 * n - 10] = 0.5 * (lstout[3 * n - 3] + lstout[(3 * n + 1) % lstout.Length]);
                        lstin[1 * n - 4].Z = 0.01 * (1 + Rnd.NextUDouble());
                        lstin[2 * n - 7].Z = 0.01 * (1 + Rnd.NextUDouble());
                        lstin[3 * n - 10].Z = 0.01 * (1 + Rnd.NextUDouble());
                        for (int i = 1; i < nn; i++)
                        {
                            lstin[i - 1 + 0 * nn] = (i * lstin[1 * nn - 1] + (nn - i) * lstin[3 * nn - 1]) / nn;
                            lstin[i - 1 + 1 * nn] = (i * lstin[2 * nn - 1] + (nn - i) * lstin[1 * nn - 1]) / nn;
                            lstin[i - 1 + 2 * nn] = (i * lstin[3 * nn - 1] + (nn - i) * lstin[2 * nn - 1]) / nn;
                            lstin[i - 1 + 0 * nn].Z = 0.01 * (1 + Rnd.NextUDouble());
                            lstin[i - 1 + 1 * nn].Z = 0.01 * (1 + Rnd.NextUDouble());
                            lstin[i - 1 + 2 * nn].Z = 0.01 * (1 + Rnd.NextUDouble());
                        }
                        startin = Faces.ShapeGroupParameters.Positions.Vectors.Max;
                        for (int i = 0; i < lstin.Length; i++)
                            Faces.ShapeGroupParameters.Positions.Vectors.Push(lstin[i] * 300);

                        for (int i = 0; i < nn; i++)
                        {
                            Faces.Indexes.Push(new int[] { startin + (i - 1 + 3 * nn) % lstin.Length, startin + (i + 3 * nn) % lstin.Length, startout + (i + 3 * n) % lstout.Length, startout + (i + 3 * n) % lstout.Length, startin + (i + 3 * nn) % lstin.Length, startout + (i + 1 + 3 * n) % lstout.Length });
                            Faces.Indexes.Push(new int[] { startin + (i - 1 + 1 * nn) % lstin.Length, startin + (i + 1 * nn) % lstin.Length, startout + (i + 1 * n) % lstout.Length, startout + (i + 1 * n) % lstout.Length, startin + (i + 1 * nn) % lstin.Length, startout + (i + 1 + 1 * n) % lstout.Length });
                            Faces.Indexes.Push(new int[] { startin + (i - 1 + 2 * nn) % lstin.Length, startin + (i + 2 * nn) % lstin.Length, startout + (i + 2 * n) % lstout.Length, startout + (i + 2 * n) % lstout.Length, startin + (i + 2 * nn) % lstin.Length, startout + (i + 1 + 2 * n) % lstout.Length });
                        }
                        Faces.Indexes.Push(new int[] {
                    startin + (1 * nn - 1 ) % lstin.Length, startout + (1 * n) % lstout.Length, startout + (1 * n - 3) % lstout.Length,
                    startin + (2 * nn - 1 ) % lstin.Length, startout + (2 * n) % lstout.Length, startout + (2 * n - 3) % lstout.Length,
                    startin + (3 * nn - 1 ) % lstin.Length, startout + (3 * n) % lstout.Length, startout + (3 * n - 3) % lstout.Length });
                        Faces.Indexes.Push(new int[] {
                    startout + (1 * n - 3) % lstout.Length, startout + (1 * n) % lstout.Length, startout + (1 * n - 2) % lstout.Length,
                    startout + (2 * n - 3) % lstout.Length, startout + (2 * n) % lstout.Length, startout + (2 * n - 2) % lstout.Length,
                    startout + (3 * n - 3) % lstout.Length, startout + (3 * n) % lstout.Length, startout + (3 * n - 2) % lstout.Length });
                        Faces.Indexes.Push(new int[] {
                    startout + (1 * n - 2) % lstout.Length, startout + (1 * n) % lstout.Length, startout + (1 * n - 1) % lstout.Length,
                    startout + (2 * n - 2) % lstout.Length, startout + (2 * n) % lstout.Length, startout + (2 * n - 1) % lstout.Length,
                    startout + (3 * n - 2) % lstout.Length, startout + (3 * n) % lstout.Length, startout + (3 * n - 1) % lstout.Length });

                        lstout = lstin;
                        startout = startin;
                    }
                    Faces.Indexes.Push(new int[] { startin, startin + 1, startin + 2 });

                    lstin = border;
                    startin = 0;
                    startout = Faces.ShapeGroupParameters.Positions.Vectors.Max;

                    lstout = lstend;

                    for (int i = 0; i < Nb; i++)
                        Faces.ShapeGroupParameters.Positions.Vectors.Push(lstout[i] * 300);

                    for (int i = 0; i < Nb; i++)
                        Faces.Indexes.Push(new int[] { startin + i, startin + (i + 1) % lstin.Length, startout + i, startout + i, startin + (i + 1) % lstin.Length, startout + (i + 1) % lstout.Length });
                }

                double MaxX = Faces.ShapeGroupParameters.Positions.Vectors.Values.Max(o => o.X);
                double MinX = Faces.ShapeGroupParameters.Positions.Vectors.Values.Min(o => o.X);
                MaxX = 0.75 / (MaxX - MinX);
                double MaxY = Faces.ShapeGroupParameters.Positions.Vectors.Values.Max(o => o.Y);
                double MinY = Faces.ShapeGroupParameters.Positions.Vectors.Values.Min(o => o.Y);
                MaxY = 0.75 / (MaxY - MinY);

                for (int i = 0; i < Faces.ShapeGroupParameters.Positions.Vectors.Max; i++)
                {
                    Faces.ShapeGroupParameters.Textures.Vectors.Push(new Vec2((Faces.ShapeGroupParameters.Positions.Vectors.Values[i].X - MinX) * MaxX, (Faces.ShapeGroupParameters.Positions.Vectors.Values[i].Y - MinY) * MaxY));
                }

                Vec3[] nms = new Vec3[Faces.ShapeGroupParameters.Positions.Vectors.Max];
                for (int i = 0; i < Faces.Indexes.Max; i += 3)
                {
                    int i1 = Faces.Indexes.Values[i + 0];
                    int i2 = Faces.Indexes.Values[i + 1];
                    int i3 = Faces.Indexes.Values[i + 2];
                    Vec3 p1 = Faces.ShapeGroupParameters.Positions.Vectors.Values[i1];
                    Vec3 p2 = Faces.ShapeGroupParameters.Positions.Vectors.Values[i2];
                    Vec3 p3 = Faces.ShapeGroupParameters.Positions.Vectors.Values[i3];
                    nms[i1] += Vec3.Cross(p3 - p1, p2 - p1);
                    nms[i2] += Vec3.Cross(p1 - p2, p3 - p2);
                    nms[i3] += Vec3.Cross(p2 - p3, p1 - p3);
                }

                Bitmap bmp = new Bitmap(600, 600);
                Graphics grp = Graphics.FromImage(bmp);
                //grp.Clear(Color.Yellow);
                TextureBrush brsGrass = new TextureBrush(Properties.Resources.Grass);
                TextureBrush brsSand = new TextureBrush(Properties.Resources.Sand);
                for (int i = 0; i < Faces.Indexes.Max - 126; i += 3)
                {
                    int i1 = Faces.Indexes.Values[i + 0];
                    int i2 = Faces.Indexes.Values[i + 1];
                    int i3 = Faces.Indexes.Values[i + 2];
                    Vec2 p1 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i1];
                    Vec2 p2 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i2];
                    Vec2 p3 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i3];
                    PointF[] pps = new PointF[]{
                        new PointF((float)p1.X, 600-(float)p1.Y),
                        new PointF((float)p2.X,600- (float)p2.Y),
                        new PointF((float)p3.X, 600-(float)p3.Y) };
                    grp.FillPolygon(brsGrass, pps);
                }
                for (int i = Faces.Indexes.Max - 126; i < Faces.Indexes.Max; i += 3)
                {
                    int i1 = Faces.Indexes.Values[i + 0];
                    int i2 = Faces.Indexes.Values[i + 1];
                    int i3 = Faces.Indexes.Values[i + 2];
                    Vec2 p1 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i1];
                    Vec2 p2 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i2];
                    Vec2 p3 = 600 * Faces.ShapeGroupParameters.Textures.Vectors.Values[i3];
                    PointF[] pps = new PointF[]{
                        new PointF((float)p1.X, 600-(float)p1.Y),
                        new PointF((float)p2.X,600- (float)p2.Y),
                        new PointF((float)p3.X, 600-(float)p3.Y) };
                    grp.FillPolygon(brsSand, pps);
                }

                Texture.KDBitmap = bmp;

                foreach (Vec3 v in nms)
                {
                    v.Normalize();
                    Faces.ShapeGroupParameters.Normals.Vectors.Push(v);
                }

                Bitmap bmptmp = new Bitmap(125, 125);
                Graphics grptmp = Graphics.FromImage(bmptmp);

                grptmp.TranslateTransform(62, 62);
                for (int i = 0; i < 8; i++)
                {
                    grptmp.RotateTransform(45);
                    grptmp.DrawImage(Properties.Resources.Bush, -109, -109);
                }
                grptmp.ResetTransform();
                grptmp.Dispose();

                grp.DrawImage(bmptmp, 475, 125);
                bmptmp.Dispose();

                double rc = 1 - 1 / System.Math.Sqrt(2);
                List<TBranch> Matrixes = new List<TBranch>();
                double h, r;
                Vec2 ptxt1, ptxt2, ptxt3, ptxt4, ptxt5;
                if (rank > 0)
                {
                    bmptmp = new Bitmap(125, 125);
                    grptmp = Graphics.FromImage(bmptmp);

                    Bitmap bmpr = (Bitmap)new ResourceManager(typeof(CiliaElements.Properties.Resources)).GetObject("Resource" + rank.ToString());// (Bitmap)new ResourceManager("Resources", typeof(TPlain).Assembly).GetObject("Resource" + rank.ToString());

                    grptmp.TranslateTransform(62, 62);
                    for (int i = 0; i < 8; i++)
                    {
                        grptmp.RotateTransform(45);
                        grptmp.DrawImage(bmpr, -109, -109);
                    }
                    grptmp.ResetTransform();
                    grptmp.Dispose();

                    grp.DrawImage(bmptmp, 475, 0);
                    bmptmp.Dispose();

                    grp.DrawImage(Properties.Resources.Leaf1, 350, 0);
                    bmptmp.Dispose();

                    ptxt1 = new Vec2(0.995, 0.995);
                    ptxt2 = new Vec2(0.795, 0.995);
                    ptxt3 = new Vec2(0.995, 0.795);
                    ptxt4 = new Vec2(0.795, 0.795);
                    ptxt5 = new Vec2(0.895, 0.895);
                    Vec2 ptxt1f = new Vec2(0.785, 0.995);
                    Vec2 ptxt2f = new Vec2(0.585, 0.995);
                    Vec2 ptxt3f = new Vec2(0.785, 0.795);
                    Vec2 ptxt4f = new Vec2(0.585, 0.795);
                    h = 60 + 100 * Rnd.NextUDouble(); h *= 0.5;
                    r = 11 + 7 * Rnd.NextUDouble(); r *= 0.5;

                    Matrixes.Add(new TBranch(Faces, this) { Matrix = Mtx4.CreateTranslation(0, 50 * System.Math.Sqrt(3), 0) });

                    nb = (int)(4 + 3 * Rnd.NextUDouble());
                    int nnb = 11 - nb;
                    double sh = 0.25 + 0.75 * Rnd.NextUDouble();
                    double sr = 0.25 + 0.75 * Rnd.NextUDouble();
                    double dh = 100;
                    for (int i = 0; i < nb; i++)
                    {
                        TBranch[] tbl = Matrixes.ToArray();
                        Matrixes.Clear();
                        foreach (TBranch m in tbl)
                        {
                            int k = m.Group.ShapeGroupParameters.Positions.Vectors.Max;
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(-r, -r, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(r, -r, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(r, r, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(-r, r, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(0, 0, h + dh), m.Matrix));
                            m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(0, 0, -0.1 * h), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(-1, -1, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(1, -1, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(1, 1, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(-1, 1, 0), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                            m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, -1), m.Matrix));
                            m.Group.ShapeGroupParameters.Textures.Vectors.Push(new Vec2[] { ptxt1, ptxt2, ptxt3, ptxt4, ptxt5, ptxt5 });
                            m.Group.Indexes.Push(new int[] { k + 0, k + 1, k + 4 });
                            m.Group.Indexes.Push(new int[] { k + 1, k + 2, k + 4 });
                            m.Group.Indexes.Push(new int[] { k + 2, k + 3, k + 4 });
                            m.Group.Indexes.Push(new int[] { k + 3, k + 0, k + 4 });
                            m.Group.Indexes.Push(new int[] { k + 1, k + 0, k + 5 });
                            m.Group.Indexes.Push(new int[] { k + 2, k + 1, k + 5 });
                            m.Group.Indexes.Push(new int[] { k + 3, k + 2, k + 5 });
                            m.Group.Indexes.Push(new int[] { k + 4, k + 3, k + 5 });
                            for (int j = 0; j < nnb; j++)
                            {
                                Mtx4 mm = m.Matrix;
                                mm = Mtx4.CreateTranslation(0, 0, dh + h * (0.3 + 0.7 * Rnd.NextUDouble())) * mm;
                                mm = Mtx4.CreateRotationZ(System.Math.PI * 2 * Rnd.NextUDouble()) * mm;
                                mm = Mtx4.CreateRotationX(System.Math.PI * 0.5 * Rnd.NextUDouble()) * mm;
                                Matrixes.Add(new TBranch(m) { Matrix = mm });
                            }
                        }
                        dh *= 0;
                        h *= sh;
                        r *= sr;
                    }
                    foreach (TBranch m in Matrixes)
                    {
                        int k = m.Group.ShapeGroupParameters.Positions.Vectors.Max;
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(2, 2, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(2, -2, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(5, 0, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                        m.Group.ShapeGroupParameters.Textures.Vectors.Push(new Vec2[] { ptxt4, ptxt2, ptxt3, ptxt1 });
                        m.Group.Indexes.Push(new int[] { k + 0, k + 1, k + 2 });
                        m.Group.Indexes.Push(new int[] { k + 1, k + 3, k + 2 });
                    }
                    Matrixes.Clear();
                }
                Vec4 mpoint = new Vec4(0, 50 * System.Math.Sqrt(3), 0, 1);
                for (int i = 0; i < 100;)
                {
                    Mtx4 m = Mtx4.CreateTranslation(300 * Rnd.NextUDouble() - 150, 150 * sqrt3 * Rnd.NextUDouble(), 0);
                    if ((m.Row3 - mpoint).LengthSquared < 22500)//  m.Row3.X + m.Row3.Y < 300)
                    {
                        i++;
                        Matrixes.Add(new TBranch(Faces, this) { Matrix = m });
                    }
                }
                h = 10;
                r = 1;
                ptxt1 = new Vec2(0.995, 0.785);
                ptxt2 = new Vec2(0.795, 0.785);
                ptxt3 = new Vec2(0.995, 0.585);
                ptxt4 = new Vec2(0.795, 0.585);
                ptxt5 = new Vec2(0.895, 0.685);

                for (int i = 0; i < 2; i++)
                {
                    TBranch[] tbl = Matrixes.ToArray();
                    Matrixes.Clear();
                    foreach (TBranch m in tbl)
                    {
                        int k = m.Group.ShapeGroupParameters.Positions.Vectors.Max;
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(-r, -r, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(r, -r, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(r, r, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(-r, r, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(0, 0, h), m.Matrix));
                        m.Group.ShapeGroupParameters.Positions.Vectors.Push(Vec3.Transform(new Vec3(0, 0, -0.1 * h), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(-1, -1, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(1, -1, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(1, 1, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(-1, 1, 0), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, 1), m.Matrix));
                        m.Group.ShapeGroupParameters.Normals.Vectors.Push(Vec3.Transform(new Vec3(0, 0, -1), m.Matrix));
                        m.Group.ShapeGroupParameters.Textures.Vectors.Push(new Vec2[] { ptxt1, ptxt2, ptxt3, ptxt4, ptxt5, ptxt5 });
                        m.Group.Indexes.Push(new int[] { k + 0, k + 1, k + 4 });
                        m.Group.Indexes.Push(new int[] { k + 1, k + 2, k + 4 });
                        m.Group.Indexes.Push(new int[] { k + 2, k + 3, k + 4 });
                        m.Group.Indexes.Push(new int[] { k + 3, k + 0, k + 4 });
                        m.Group.Indexes.Push(new int[] { k + 1, k + 0, k + 5 });
                        m.Group.Indexes.Push(new int[] { k + 2, k + 1, k + 5 });
                        m.Group.Indexes.Push(new int[] { k + 3, k + 2, k + 5 });
                        m.Group.Indexes.Push(new int[] { k + 4, k + 3, k + 5 });
                        if (i < 1)
                            for (int j = 0; j < 2; j++)
                            {
                                Mtx4 mm = m.Matrix;
                                mm = Mtx4.CreateTranslation(0, 0, h * (0.3 + 0.7 * Rnd.NextUDouble())) * mm;
                                mm = Mtx4.CreateRotationZ(System.Math.PI * 2 * Rnd.NextUDouble()) * mm;
                                mm = Mtx4.CreateRotationX(System.Math.PI * 0.5 * Rnd.NextUDouble()) * mm;
                                Matrixes.Add(new TBranch(m) { Matrix = mm });
                            }
                    }
                    h *= 0.5;
                    r *= 0.5;
                }
                grp.Dispose();

                //bmp.Save("c:\\temp\\" + PartNumber + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            //
        }

        #endregion Private Methods

        #region Private Classes

        private class TBranch
        {
            #region Public Fields

            public TFGroup Group;

            public Mtx4 Matrix;

            #endregion Public Fields

            #region Public Constructors

            public TBranch(TFGroup template, TPlain plain)
            {
                Group = plain.SolidElementConstruction.AddFGroup();
                Group.ShapeGroupParameters.Positions = template.ShapeGroupParameters.Positions;
                Group.ShapeGroupParameters.Normals = template.ShapeGroupParameters.Normals;
                Group.ShapeGroupParameters.Textures = template.ShapeGroupParameters.Textures;
                Group.GroupParameters.Texture = template.GroupParameters.Texture;
                plain.SolidElementConstruction.StartGroup.FGroups.Push(Group);
            }

            public TBranch(TBranch parent)
            {
                Group = parent.Group;
            }

            #endregion Public Constructors
        }

        #endregion Private Classes
    }
}