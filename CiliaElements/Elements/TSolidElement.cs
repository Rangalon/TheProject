
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CiliaElements
{
    public sealed class TShapeComparer : IComparer<TShape>
    {
        #region Public Methods

        int IComparer<TShape>.Compare(TShape x, TShape y)
        {
            return y.ShapeParameters.Group.GroupParameters.Texture.A.CompareTo(x.ShapeParameters.Group.GroupParameters.Texture.A);
        }

        #endregion Public Methods
    }

    public class TSolidElement : TBaseElement
    {
        #region Public Fields

        public void ChangeTexture(Bitmap bmp)
        {
            lock (TextureDataLocker)
                if (TextureData != null)
                {
                    textureBmp.UnlockBits(TextureData);
                    TextureData = null;
                }
            textureBmp?.Dispose();
            textureBmp = bmp;

            lock (TextureDataLocker)
                TextureData = textureBmp.LockBits(new Rectangle(0, 0, textureBmp.Width, textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
         
            TManager.TexturesToUpdate.Push(this);
        }

        public TQuickStc<TShape> Curves = new TQuickStc<TShape>();

        public int HandleIndexes = -1;

        public int HandleNormals;

        public int HandlePositions;

        public int HandleTexts;

        public TQuickStc<TShape> Points = new TQuickStc<TShape>();

        public bool PointsOnly = false;

        public TSolidElementConstruction SolidElementConstruction;

        public Bitmap textureBmp;

        public int TextureId;

        public int TextureOffset;

        public bool Transparent = false;

        #endregion Public Fields

        #region Internal Fields

        internal int HandleVAO;
        internal BitmapData TextureData;
        public object TextureDataLocker = new object();
        public bool WillBeUpdated;

        #endregion Internal Fields

        #region Public Constructors

        public TSolidElement()
        {
            Surfaces = new TQuickStc<TShape>();
            OwnerLink = new TLink { Child = this };
            SolidElementConstruction = new TSolidElementConstruction() { Solid = this };
            ElementLoader = new TSolidElementLoader() { Element = this };
        }

        #endregion Public Constructors

        #region Public Properties

        public virtual int[] DataIndexes { get; set; } = { };

        public virtual Vec3[] DataNormals { get; set; } = { };

        public virtual Vec3[] DataPositions { get; set; } = { };

        public virtual Vec2f[] DataTexts { get; set; } = { };

        public virtual int FacesNumber { get; set; }

        public virtual int FacesStart { get; set; }

        public virtual int FacesStarter { get; set; }

        public virtual int LinesNumber { get; set; }

        public virtual int LinesStart { get; set; }

        public virtual int LinesStarter { get; set; }

        public virtual int PointsNumber { get; set; }

        public virtual int PointsStart { get; set; }

        public virtual int PointsStarter { get; set; }

        public virtual TShape[] Shapes { get; set; }

        public virtual TQuickStc<TShape> Surfaces { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static void CompressPack(BinaryWriter bwtr, Vec3[] DataPositions, Vec3[] DataNormals, Vec2f[] DataTexts, int[] DataIndexes, int FacesNumber, int LinesNumber, int PointsNumber, TQuickStc<TShape> Surfaces, TQuickStc<TShape> Curves, Bitmap textureBmp)
        {
            Vec3[] DPositions = (Vec3[])DataPositions.Clone();
            Vec3[] DNormals = (Vec3[])DataNormals.Clone();
            Vec2f[] DTexts = (Vec2f[])DataTexts.Clone();
            int[] DIndexes = (int[])DataIndexes.Clone();

            int DFacesNumber = FacesNumber;
            int DLinesNumber = LinesNumber;
            int DPointsNumber = PointsNumber;

            SBoundingBox3 bx = new SBoundingBox3();
            bx.MinPosition.X = DPositions.Min(o => o.X);
            bx.MinPosition.Y = DPositions.Min(o => o.Y);
            bx.MinPosition.Z = DPositions.Min(o => o.Z);
            bx.MaxPosition.X = DPositions.Max(o => o.X);
            bx.MaxPosition.Y = DPositions.Max(o => o.Y);
            bx.MaxPosition.Z = DPositions.Max(o => o.Z);

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TTmpShape[] DSurfaces = new TTmpShape[Surfaces.Max];
            List<Vec3>[] segs = new List<Vec3>[DPositions.Length];
            for (int i = 0; i < segs.Length; i++)
            {
                segs[i] = new List<Vec3>();
            }

            for (int i = 0; i < DSurfaces.Length; i++)
            {
                TShape surf = Surfaces.Values[i];
                TTmpShape tsurf = new TTmpShape() { BoundingBox = surf.BoundingBox, IndexesStart = surf.IndexesStart, IndexesEnd = surf.IndexesEnd };
                DSurfaces[i] = tsurf;
                tsurf.Triangles = new TTriangle[(surf.IndexesEnd - surf.IndexesStart) / 3];
                int j2 = 0;
                for (int j = surf.IndexesStart; j < surf.IndexesEnd; j += 3)
                {
                    TTriangle t = new TTriangle
                    {
                        I1 = DIndexes[j + 0],
                        I2 = DIndexes[j + 1],
                        I3 = DIndexes[j + 2],
                        Shape = tsurf
                    };
                    segs[t.I1].Add(DPositions[t.I2]); segs[t.I1].Add(DPositions[t.I3]);
                    segs[t.I2].Add(DPositions[t.I3]); segs[t.I2].Add(DPositions[t.I1]);
                    segs[t.I3].Add(DPositions[t.I1]); segs[t.I3].Add(DPositions[t.I2]);
                    tsurf.Triangles[j2] = t; j2++;
                }
            }
            TTmpShape[] DCurves = new TTmpShape[Curves.Max];
            for (int i = 0; i < DCurves.Length; i++)
            {
                TShape curv = Curves.Values[i];
                TTmpShape tcurv = new TTmpShape() { BoundingBox = curv.BoundingBox, IndexesStart = curv.IndexesStart, IndexesEnd = curv.IndexesEnd };
                DCurves[i] = tcurv;
                tcurv.Segments = new TSegment[(curv.IndexesEnd - curv.IndexesStart) / 2];
                int j2 = 0;
                for (int j = curv.IndexesStart; j < curv.IndexesEnd; j += 2)
                {
                    TSegment t = new TSegment
                    {
                        I1 = DIndexes[FacesNumber + j + 0],
                        I2 = DIndexes[FacesNumber + j + 1],
                        Shape = tcurv
                    };
                    tcurv.Segments[j2] = t; j2++;
                }
            }

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            //List<int> idxs = new List<int>();
            TManager.WriteLineConsole("Preparing Indexes");

            TIndexMove[] newidxs = new TIndexMove[DPositions.Length];
            bool[] bidxs = new bool[DPositions.Length];
            bool[] bidxsm = new bool[DPositions.Length];
            for (int i = 0; i < DPositions.Length; i++)
            {
                newidxs[i] = new TIndexMove() { Value = i, ToValue = i };
            }
            List<TTmpBox> boxes = new List<TTmpBox>();
            Stack<TTmpBox> stc = new Stack<TTmpBox>();

            stc.Push(new TTmpBox() { BoundingBox = bx, Indexes = newidxs });
            TManager.WriteLineConsole("Building Boxes for " + DPositions.Length.ToString() + " vertexes");

            while (stc.Count > 0)
            {
                TTmpBox box = stc.Pop();
                if (box.BoundingBox.Size.LengthSquared < 1e-2 || box.Indexes.Length < 1000)
                {
                    boxes.Add(box);
                }
                else
                {
                    foreach (TTmpBox b in box.Split(DPositions).Where(o => o.Indexes.Length > 0))
                    {
                        stc.Push(b);
                    }
                }
            }

            //idxs.AddRange(newidxs);
            int n = 0;
            // *******************************************************************
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // --- Heavy
            /*
            TManager.WriteLineConsole("Simplifying positions with " + boxes.Count.ToString() + " boxes");

            double dmax = 3e-2;
            foreach (TTmpBox box in boxes)
            {
                foreach (TIndexMove i1 in box.Indexes)
                {
                    if (!bidxs[i1.Value])
                    {
                        bidxs[i1.Value] = true;
                        Vec3 v1 = DPositions[i1.Value];

                        TIndexMove[] iis = box.Indexes.Where(o => !bidxs[o.Value] && DPositions[o.Value].X - v1.X < dmax && v1.X - DPositions[o.Value].X < dmax && DPositions[o.Value].Y - v1.Y < dmax && v1.Y - DPositions[o.Value].Y < dmax && DPositions[o.Value].Z - v1.Z < dmax && v1.Z - DPositions[o.Value].Z < dmax).ToArray();
                        if (iis.Length > 0)
                        {
                            //Vec3 ni = DNormals[i1.Value];

                            foreach (TIndexMove i2 in iis)
                            {
                                newidxs[i2.Value] = i1;
                                bidxsm[i2.Value] = true;
                                //ni += DNormals[i2.Value];
                                //vi += DPositions[i2.Value];

                                n++;

                                bidxs[i2.Value] = true;
                            }
                            double saa = 0, sbb = 0, scc = 0, sab = 0, sbc = 0, sca = 0, sad = 0, sbd = 0, scd = 0;
                            Array.Resize(ref iis, iis.Length + 1); iis[iis.Length - 1] = i1;
                            foreach (TIndexMove i in iis)
                            {
                                v1 = DPositions[i.Value];
                                foreach (Vec3 v2 in segs[i.Value])
                                {
                                    double ai = v1.X - v2.X;
                                    double bi = v1.Y - v2.Y;
                                    double ci = v1.Z - v2.Z;
                                    double dai = v2.X * v1.Y - v2.Y * v1.X;
                                    double dbi = v2.Y * v1.Z - v2.Z * v1.Y;
                                    double dci = v2.Z * v1.X - v2.X * v1.Z;
                                    saa += bi * bi + ci * ci;
                                    sbb += ai * ai + ci * ci;
                                    scc += ai * ai + bi * bi;
                                    sab -= ai * bi;
                                    sbc -= bi * ci;
                                    sca -= ci * ai;
                                    sad += bi * dai - ci * dci;
                                    sbd += ci * dbi - ai * dai;
                                    scd += ai * dci - bi * dbi;
                                }
                            }
                            Mtx3 m = new Mtx3();
                            m.Row0.X = saa; m.Row1.Y = sbb; m.Row2.Z = scc;
                            m.Row0.Y = sab; m.Row1.X = sab;
                            m.Row0.Z = sca; m.Row2.X = sca;
                            m.Row1.Z = sbc; m.Row2.Y = sbc;
                            m.Invert();
                            Vec3 r = new Vec3();
                            r.X = sad;
                            r.Y = sbd;
                            r.Z = scd;
                            r = Vec3.Transform(r, m);
                            DPositions[i1.Value] = r;
                        }
                    }
                }
            }
            */
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // --- Heavy
            TManager.WriteLineConsole("Removing " + n.ToString() + " positions");
            //int[] removedpos = removed.ToArray();
            if (false && bidxsm.Length > 0)
            {
                int idx = 0;
                int idxr = 0;
                while (!bidxsm[idxr])
                {
                    idxr++;
                }

                while (idx < newidxs.Length)
                {
                    while (idx < newidxs.Length && bidxsm[idx])
                    {
                        idx++;
                    }
                    if (idx < newidxs.Length && idx > idxr)
                    {
                        newidxs[idx].ToValue = idxr;
                        //for (int i = idx + 1; i < newidxs.Length; i++)
                        //{
                        //    if (newidxs[i] == idx)
                        //    {
                        //        newidxs[i] = idxr;
                        //    }
                        //}

                        DTexts[idxr] = DTexts[idx];
                        DNormals[idxr] = DNormals[idx];
                        DPositions[idxr] = DPositions[idx];
                        bidxsm[idx] = true;
                        bidxsm[idxr] = false;
                        while (!bidxsm[idxr])
                        {
                            idxr++;
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
            }
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TManager.WriteLineConsole("Resizing positions");
            {
                Vec2f[] dtf = DTexts;
                Array.Resize(ref dtf, DTexts.Length - n); DTexts = dtf;
            }
            {
                Vec3[] dtf = DNormals;
                Array.Resize(ref dtf, DNormals.Length - n); DNormals = dtf;
            }
            {
                Vec3[] dtf = DPositions;
                Array.Resize(ref dtf, DPositions.Length - n); DPositions = dtf;
            }
            TManager.WriteLineConsole("Remove " + n.ToString() + " vertexes");

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TManager.WriteLineConsole("Reordering and cleaning surfaces      ");
            DFacesNumber = 0;
            //int DNewLinesNumber = 0;
            List<Vec3> newpos = new List<Vec3>();
            List<Vec3> newnor = new List<Vec3>();
            List<Vec2f> newtex = new List<Vec2f>();
            //List<TTmpShape> NCurves = new List<TTmpShape>();

            int cpt = DSurfaces.Length;
            TManager.PushConsole();
            foreach (TTmpShape shp in DSurfaces)
            {
                TManager.WriteLineConsole(cpt.ToString() + " Cleaning surface with " + shp.Triangles.Length.ToString() + " triangles");
                cpt--;
                shp.IndexesStart = DFacesNumber;
                List<TTriangle> trgs = new List<TTriangle>();
                TManager.PushConsole();
                n = shp.Triangles.Length;
                foreach (TTriangle t in shp.Triangles)
                {
                    t.I1 = newidxs[t.I1].ToValue;
                    t.I2 = newidxs[t.I2].ToValue;
                    t.I3 = newidxs[t.I3].ToValue;
                    if (t.I1 != t.I2 && t.I2 != t.I3 && t.I3 != t.I1)
                    {
                        if (t.I1 > t.I2)
                        {
                            if (t.I2 > t.I3) // t3 t2 t1
                            { int m = t.I3; t.I3 = t.I1; t.I1 = m; m = 0; }
                            else if (t.I1 > t.I3) // t2 t3 t1
                            { int m = t.I3; t.I3 = t.I1; t.I1 = t.I2; t.I2 = m; m = 0; }
                            else // t2 t1 t3
                            { int m = t.I2; t.I2 = t.I1; t.I1 = m; m = 0; }
                        }
                        else if (t.I1 > t.I3) // t3 t1 t2
                        { int m = t.I3; t.I3 = t.I2; t.I2 = t.I1; t.I1 = m; m = 0; }
                        else if (t.I2 > t.I3) // t1 t3 t2
                        { int m = t.I3; t.I3 = t.I2; t.I2 = m; m = 0; }

                        trgs.Add(t);
                    }
                }
                TManager.WriteLineConsole((n - trgs.Count).ToString() + " useless triangles removed");
                if (trgs.Count > 0)
                {
                    trgs.Sort(TTriangleComparer.Default);
                    int i = 1; TTriangle tt = trgs[0]; n = 0;
                    while (i < trgs.Count)
                    {
                        TTriangle t = trgs[i];
                        if (t.I1 == tt.I1 && t.I2 == tt.I2 && t.I3 == tt.I3)
                        {
                            n++;
                            trgs.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                            tt = t;
                        }
                    }
                    shp.Triangles = trgs.ToArray();
                    TManager.WriteLineConsole(n.ToString() + " duplicated triangles removed");
                }
                TManager.PullConsole();
                foreach (TTriangle t in shp.Triangles)//.Triangles.Where(o => o.I1 != o.I2 && o.I2 != o.I3 && o.I3 != o.I1))
                {
                    //segs.Add(new TSegment() { I1 = t.I1, I2 = t.I2 });
                    //segs.Add(new TSegment() { I1 = t.I2, I2 = t.I3 });
                    //segs.Add(new TSegment() { I1 = t.I3, I2 = t.I1 });
                    Vec3 n123 = Vec3.Cross(DPositions[t.I2] - DPositions[t.I1], DPositions[t.I3] - DPositions[t.I1]);
                    n123.Normalize();
                    double n1 = Vec3.Dot(DNormals[t.I1], n123);
                    double n2 = Vec3.Dot(DNormals[t.I2], n123);
                    double n3 = Vec3.Dot(DNormals[t.I3], n123);

                    if ((n1 <= 0 && (n2 <= 0 || n3 <= 0)) || (n2 <= 0 && n3 <= 0))
                    {
                        n123 = -n123;
                        n1 = -n1;
                        n2 = -n2;
                        n3 = -n3;
                    }

                    if (n1 < 0.5)
                    {
                        newpos.Add(DPositions[t.I1]);
                        newnor.Add(n123);
                        newtex.Add(DTexts[t.I1]);
                        t.I1 = DPositions.Length + newpos.Count - 1;
                    }
                    if (n2 < 0.5)
                    {
                        newpos.Add(DPositions[t.I2]);
                        newnor.Add(n123);
                        newtex.Add(DTexts[t.I2]);
                        t.I2 = DPositions.Length + newpos.Count - 1;
                    }
                    if (n3 < 0.5)
                    {
                        newpos.Add(DPositions[t.I3]);
                        newnor.Add(n123);
                        newtex.Add(DTexts[t.I3]);
                        t.I3 = DPositions.Length + newpos.Count - 1;
                    }

                    DIndexes[DFacesNumber] = t.I1; DFacesNumber++;
                    DIndexes[DFacesNumber] = t.I2; DFacesNumber++;
                    DIndexes[DFacesNumber] = t.I3; DFacesNumber++;
                }
                shp.IndexesEnd = DFacesNumber;
            }
            TManager.PullConsole();
            n = DSurfaces.Length;
            DSurfaces = DSurfaces.Where(o => o.IndexesStart != o.IndexesEnd).ToArray();
            n -= DSurfaces.Length;
            TManager.WriteLineConsole("Remove " + n.ToString() + " surfaces");

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TManager.WriteLineConsole("Reordering and cleaning curves");
            DLinesNumber = 0;
            foreach (TTmpShape curv in DCurves)
            {
                curv.IndexesStart = DLinesNumber;
                foreach (TSegment t in curv.Segments)
                {
                    t.I1 = newidxs[t.I1].ToValue;
                    t.I2 = newidxs[t.I2].ToValue;
                    if (t.I1 != t.I2)
                    {
                        DIndexes[DFacesNumber + DLinesNumber] = t.I1; DLinesNumber++;
                        DIndexes[DFacesNumber + DLinesNumber] = t.I2; DLinesNumber++;
                    }
                }

                curv.IndexesEnd = DLinesNumber;
            }
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TManager.WriteLineConsole("Resizing Indexes");
            n = DPositions.Length;
            Array.Resize(ref DIndexes, DFacesNumber + DLinesNumber + DPointsNumber);
            Array.Resize(ref DPositions, n + newpos.Count); Array.Copy(newpos.ToArray(), 0, DPositions, n, newpos.Count);
            Array.Resize(ref DNormals, n + newnor.Count); Array.Copy(newnor.ToArray(), 0, DNormals, n, newpos.Count);
            Array.Resize(ref DTexts, n + newtex.Count); Array.Copy(newtex.ToArray(), 0, DTexts, n, newpos.Count);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            DoFile(bwtr, DPositions, DNormals, DTexts, DIndexes, DFacesNumber, DLinesNumber, DPointsNumber, DSurfaces, DCurves, bx, textureBmp);
        }

        public static void DoFile(BinaryWriter bwtr, Vec3[] DPositions, Vec3[] DNormals, Vec2f[] DTexts, int[] DIndexes, int DFacesNumber, int DLinesNumber, int DPointsNumber, TTmpShape[] DSurfaces, TTmpShape[] DCurves, SBoundingBox3 BoundingBox, Image textureBmp)
        {
            TManager.WriteLineConsole("Writing file");
            bwtr.Write(DPositions.Length);
            foreach (Vec3 v in DPositions)
            {
                bwtr.Write(v.X);
                bwtr.Write(v.Y);
                bwtr.Write(v.Z);
            }
            foreach (Vec3 v in DNormals)
            {
                bwtr.Write(v.X);
                bwtr.Write(v.Y);
                bwtr.Write(v.Z);
            }
            foreach (Vec2f v in DTexts)
            {
                bwtr.Write(v.X);
                bwtr.Write(v.Y);
            }
            // ------------------------------------------------------------------------
            Array.Resize(ref DIndexes, DFacesNumber + DLinesNumber + DPointsNumber);
            bwtr.Write(DIndexes.Length);
            foreach (int i in DIndexes)
            {
                bwtr.Write(i);
            }
            // ------------------------------------------------------------------------
            bwtr.Write(DFacesNumber);
            bwtr.Write(0);
            bwtr.Write(0);
            bwtr.Write(DLinesNumber);
            bwtr.Write(DFacesNumber);
            bwtr.Write(DFacesNumber * 4);
            bwtr.Write(DPointsNumber);
            bwtr.Write((DFacesNumber + DLinesNumber));
            bwtr.Write((DFacesNumber + DLinesNumber) * 4);
            // ------------------------------------------------------------------------
            bwtr.Write(BoundingBox.MinPosition.X);
            bwtr.Write(BoundingBox.MinPosition.Y);
            bwtr.Write(BoundingBox.MinPosition.Z);
            bwtr.Write(BoundingBox.MaxPosition.X);
            bwtr.Write(BoundingBox.MaxPosition.Y);
            bwtr.Write(BoundingBox.MaxPosition.Z);
            // ------------------------------------------------------------------------
            bwtr.Write(DSurfaces.Length);
            foreach (TTmpShape shp in DSurfaces)
            {
                bwtr.Write(shp.BoundingBox.MinPosition.X);
                bwtr.Write(shp.BoundingBox.MinPosition.Y);
                bwtr.Write(shp.BoundingBox.MinPosition.Z);
                bwtr.Write(shp.BoundingBox.MaxPosition.X);
                bwtr.Write(shp.BoundingBox.MaxPosition.Y);
                bwtr.Write(shp.BoundingBox.MaxPosition.Z);
                bwtr.Write(shp.IndexesStart);
                bwtr.Write(shp.IndexesEnd);
            }
            // ------------------------------------------------------------------------
            bwtr.Write(DCurves.Length);
            foreach (TTmpShape shp in DCurves)
            {
                bwtr.Write(shp.BoundingBox.MinPosition.X);
                bwtr.Write(shp.BoundingBox.MinPosition.Y);
                bwtr.Write(shp.BoundingBox.MinPosition.Z);
                bwtr.Write(shp.BoundingBox.MaxPosition.X);
                bwtr.Write(shp.BoundingBox.MaxPosition.Y);
                bwtr.Write(shp.BoundingBox.MaxPosition.Z);
                bwtr.Write(shp.IndexesStart);
                bwtr.Write(shp.IndexesEnd);
            }
            // ------------------------------------------------------------------------
            if (textureBmp != null)
            {
                textureBmp.Save(bwtr.BaseStream, ImageFormat.Png);
            }
            // ------------------------------------------------------------------------
        }

        public static void DoPack(BinaryWriter bwtr, Vec3[] DataPositions, Vec3[] DataNormals, Vec2f[] DataTexts, int[] DataIndexes, int FacesNumber, int LinesNumber, int PointsNumber, TQuickStc<TShape> Surfaces, TQuickStc<TShape> Curves, SBoundingBox3 BoundingBox, Bitmap textureBmp)
        {
            Vec3[] DPositions = (Vec3[])DataPositions.Clone();
            Vec3[] DNormals = (Vec3[])DataNormals.Clone();
            Vec2f[] DTexts = (Vec2f[])DataTexts.Clone();
            int[] DIndexes = (int[])DataIndexes.Clone();

            int DFacesNumber = FacesNumber;
            int DLinesNumber = LinesNumber;
            int DPointsNumber = PointsNumber;

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            TTmpShape[] DSurfaces = new TTmpShape[Surfaces.Max];

            for (int i = 0; i < DSurfaces.Length; i++)
            {
                TShape surf = Surfaces.Values[i];
                TTmpShape tsurf = new TTmpShape() { BoundingBox = surf.BoundingBox, IndexesStart = surf.IndexesStart, IndexesEnd = surf.IndexesEnd };
                DSurfaces[i] = tsurf;
                tsurf.Triangles = new TTriangle[(surf.IndexesEnd - surf.IndexesStart) / 3];
                int j2 = 0;
                for (int j = surf.IndexesStart; j < surf.IndexesEnd; j += 3)
                {
                    TTriangle t = new TTriangle
                    {
                        I1 = DIndexes[j + 0],
                        I2 = DIndexes[j + 1],
                        I3 = DIndexes[j + 2],
                        Shape = tsurf
                    };
                    tsurf.Triangles[j2] = t; j2++;
                }
            }
            TTmpShape[] DCurves = new TTmpShape[Curves.Max];
            for (int i = 0; i < DCurves.Length; i++)
            {
                TShape curv = Curves.Values[i];
                TTmpShape tcurv = new TTmpShape() { BoundingBox = curv.BoundingBox, IndexesStart = curv.IndexesStart, IndexesEnd = curv.IndexesEnd };
                DCurves[i] = tcurv;
                tcurv.Segments = new TSegment[(curv.IndexesEnd - curv.IndexesStart) / 2];
                int j2 = 0;
                for (int j = curv.IndexesStart; j < curv.IndexesEnd; j += 2)
                {
                    TSegment t = new TSegment
                    {
                        I1 = DIndexes[FacesNumber + j + 0],
                        I2 = DIndexes[FacesNumber + j + 1],
                        Shape = tcurv
                    };
                    tcurv.Segments[j2] = t; j2++;
                }
            }
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            DoFile(bwtr, DPositions, DNormals, DTexts, DIndexes, DFacesNumber, DLinesNumber, DPointsNumber, DSurfaces, DCurves, BoundingBox, textureBmp);
        }

        public virtual Vec4 GetClosestPointForPointFromSurface(Vec4 iPt)
        {
            Vec3 minV = new Vec3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
            Vec4? BestPt = default(Vec4?);
            for (int I = 0; I < SolidElementConstruction.FacesIndexes.Length; I += 3)
            {
                Vec3 pt1 = default(Vec3);
                //= DataPositions[DataIndices(I))
                Vec3 pt2 = default(Vec3);
                //= DataPositions[DataIndices(I + 1))
                Vec3 pt3 = default(Vec3);
                //= DataPositions[DataIndices(I + 2))

                Vec3 v0 = pt2 - pt1;
                Vec3 v1 = pt3 - pt1;
                Vec3 v2 = Vec3.Cross(v0, v1);
                Vec3 v = (Vec3)iPt - pt1;
                Mtx3 mat = new Mtx3(v0, v1, v2);
                Vec3 r = default(Vec3);
                try
                {
                    mat = Mtx3.Invert(mat);
                    r = new Vec3(Vec3.Dot(mat.Column0, v), Vec3.Dot(mat.Column1, v), Vec3.Dot(mat.Column2, v));
                }
                catch
                {
                    r = new Vec3(-1, -1, -1);
                }
                if (r.X >= 0 && r.Y >= 0 && r.X <= 1 && r.Y <= 1 && r.X + r.Y <= 1)
                {
                    Vec4 pt = iPt - new Vec4(r.Z * v2, 0);
                    if (!BestPt.HasValue)
                    {
                        BestPt = pt;
                    }
                    else if ((BestPt.Value - iPt).LengthSquared > (pt - iPt).LengthSquared)
                    {
                        BestPt = pt;
                    }
                }
            }
            if (!BestPt.HasValue)
            {
                return new Vec4();
            }

            return BestPt.Value;
        }

        public override FileInfo Pack()
        {
            TManager.PushConsole();
            TManager.WriteLineConsole("Packing of solid " + PartNumber);
            TManager.PushConsole();

            FileInfo fi = new FileInfo(TManager.DocumentsDirectory.FullName + "\\Parts\\" + PartNumber + ".CiliaSZ");
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            StreamWriter wtr = new StreamWriter(fi.FullName);
            GZipStream cwtr = new GZipStream(wtr.BaseStream, CompressionLevel.Optimal);
            BinaryWriter bwtr = new BinaryWriter(cwtr);

            TManager.WriteLineConsole("Cloning solid");

            CompressPack(bwtr, DataPositions, DataNormals, DataTexts, DataIndexes, FacesNumber, LinesNumber, PointsNumber, Surfaces, Curves, textureBmp);

            bwtr.Dispose();
            cwtr.Dispose();
            fi.Refresh();
            TManager.PullConsole();
            TManager.WriteLineConsole("Packing of solid done: " + (fi.Length / 1024).ToString() + " Ko");
            TManager.PullConsole();
            return fi;
        }

        #endregion Public Methods

        #region Public Classes

        public class TSegment
        {
            #region Public Fields

            public int I1;
            public int I2;
            public TTmpShape Shape;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return I1.ToString() + ";" + I2.ToString();
            }

            #endregion Public Methods
        }

        public class TTmpShape
        {
            #region Public Fields

            public TSegment[] Segments = { };
            public TTriangle[] Triangles = { };

            #endregion Public Fields

            #region Internal Fields

            internal SBoundingBox3 BoundingBox;
            internal int IndexesEnd;
            internal int IndexesStart;

            #endregion Internal Fields
        }

        public class TTriangle
        {
            #region Public Fields

            public int I1;
            public int I2;
            public int I3;
            public Vec3 N;
            public TTmpShape Shape;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return I1.ToString() + ";" + I2.ToString() + ";" + I3.ToString();
            }

            #endregion Public Methods
        }

        #endregion Public Classes

        #region Internal Classes

        internal class TIndexMove
        {
            #region Public Fields

            public int ToValue;
            public int Value;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return "=> " + ToValue.ToString();
            }

            #endregion Public Methods
        }

        internal class TTmpBox
        {
            #region Public Fields

            public SBoundingBox3 BoundingBox;
            public TIndexMove[] Indexes;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return Indexes.Length.ToString() + " Indexes";
            }

            #endregion Public Methods

            #region Internal Methods

            internal TTmpBox[] Split(Vec3[] DPositions)
            {
                TTmpBox[] bxs = new TTmpBox[] { new TTmpBox(), new TTmpBox(), new TTmpBox(), new TTmpBox(), new TTmpBox(), new TTmpBox(), new TTmpBox(), new TTmpBox() };
                //
                bxs[0].BoundingBox.MinPosition.X = BoundingBox.MinPosition.X; bxs[0].BoundingBox.MinPosition.Y = BoundingBox.MinPosition.Y; bxs[0].BoundingBox.MinPosition.Z = BoundingBox.MinPosition.Z;
                bxs[0].BoundingBox.MaxPosition.X = BoundingBox.CtrPosition.X; bxs[0].BoundingBox.MaxPosition.Y = BoundingBox.CtrPosition.Y; bxs[0].BoundingBox.MaxPosition.Z = BoundingBox.CtrPosition.Z;

                bxs[1].BoundingBox.MinPosition.X = BoundingBox.CtrPosition.X; bxs[1].BoundingBox.MinPosition.Y = BoundingBox.MinPosition.Y; bxs[1].BoundingBox.MinPosition.Z = BoundingBox.MinPosition.Z;
                bxs[1].BoundingBox.MaxPosition.X = BoundingBox.MaxPosition.X; bxs[1].BoundingBox.MaxPosition.Y = BoundingBox.CtrPosition.Y; bxs[1].BoundingBox.MaxPosition.Z = BoundingBox.CtrPosition.Z;

                bxs[2].BoundingBox.MinPosition.X = BoundingBox.MinPosition.X; bxs[2].BoundingBox.MinPosition.Y = BoundingBox.CtrPosition.Y; bxs[2].BoundingBox.MinPosition.Z = BoundingBox.MinPosition.Z;
                bxs[2].BoundingBox.MaxPosition.X = BoundingBox.CtrPosition.X; bxs[2].BoundingBox.MaxPosition.Y = BoundingBox.MaxPosition.Y; bxs[2].BoundingBox.MaxPosition.Z = BoundingBox.CtrPosition.Z;

                bxs[3].BoundingBox.MinPosition.X = BoundingBox.CtrPosition.X; bxs[3].BoundingBox.MinPosition.Y = BoundingBox.CtrPosition.Y; bxs[3].BoundingBox.MinPosition.Z = BoundingBox.MinPosition.Z;
                bxs[3].BoundingBox.MaxPosition.X = BoundingBox.MaxPosition.X; bxs[3].BoundingBox.MaxPosition.Y = BoundingBox.MaxPosition.Y; bxs[3].BoundingBox.MaxPosition.Z = BoundingBox.CtrPosition.Z;

                bxs[4].BoundingBox.MinPosition.X = BoundingBox.MinPosition.X; bxs[4].BoundingBox.MinPosition.Y = BoundingBox.MinPosition.Y; bxs[4].BoundingBox.MinPosition.Z = BoundingBox.CtrPosition.Z;
                bxs[4].BoundingBox.MaxPosition.X = BoundingBox.CtrPosition.X; bxs[4].BoundingBox.MaxPosition.Y = BoundingBox.CtrPosition.Y; bxs[4].BoundingBox.MaxPosition.Z = BoundingBox.MaxPosition.Z;

                bxs[5].BoundingBox.MinPosition.X = BoundingBox.CtrPosition.X; bxs[5].BoundingBox.MinPosition.Y = BoundingBox.MinPosition.Y; bxs[5].BoundingBox.MinPosition.Z = BoundingBox.CtrPosition.Z;
                bxs[5].BoundingBox.MaxPosition.X = BoundingBox.MaxPosition.X; bxs[5].BoundingBox.MaxPosition.Y = BoundingBox.CtrPosition.Y; bxs[5].BoundingBox.MaxPosition.Z = BoundingBox.MaxPosition.Z;

                bxs[6].BoundingBox.MinPosition.X = BoundingBox.MinPosition.X; bxs[6].BoundingBox.MinPosition.Y = BoundingBox.CtrPosition.Y; bxs[6].BoundingBox.MinPosition.Z = BoundingBox.CtrPosition.Z;
                bxs[6].BoundingBox.MaxPosition.X = BoundingBox.CtrPosition.X; bxs[6].BoundingBox.MaxPosition.Y = BoundingBox.MaxPosition.Y; bxs[6].BoundingBox.MaxPosition.Z = BoundingBox.MaxPosition.Z;

                bxs[7].BoundingBox.MinPosition.X = BoundingBox.CtrPosition.X; bxs[7].BoundingBox.MinPosition.Y = BoundingBox.CtrPosition.Y; bxs[7].BoundingBox.MinPosition.Z = BoundingBox.CtrPosition.Z;
                bxs[7].BoundingBox.MaxPosition.X = BoundingBox.MaxPosition.X; bxs[7].BoundingBox.MaxPosition.Y = BoundingBox.MaxPosition.Y; bxs[7].BoundingBox.MaxPosition.Z = BoundingBox.MaxPosition.Z;

                List<TIndexMove>[] lsts = new List<TIndexMove>[8];
                lsts[0] = new List<TIndexMove>();
                lsts[1] = new List<TIndexMove>();
                lsts[2] = new List<TIndexMove>();
                lsts[3] = new List<TIndexMove>();
                lsts[4] = new List<TIndexMove>();
                lsts[5] = new List<TIndexMove>();
                lsts[6] = new List<TIndexMove>();
                lsts[7] = new List<TIndexMove>();

                Vec4 ctr = BoundingBox.CtrPosition;

                foreach (TIndexMove i in Indexes)
                {
                    Vec3 v = DPositions[i.Value];
                    if (v.Z < ctr.Z)
                    {
                        if (v.Y < ctr.Y)
                        {
                            if (v.X < ctr.X)
                            {
                                lsts[0].Add(i);
                            }
                            else
                            {
                                lsts[1].Add(i);
                            }
                        }
                        else
                        {
                            if (v.X < ctr.X)
                            {
                                lsts[2].Add(i);
                            }
                            else
                            {
                                lsts[3].Add(i);
                            }
                        }
                    }
                    else
                    {
                        if (v.Y < ctr.Y)
                        {
                            if (v.X < ctr.X)
                            {
                                lsts[4].Add(i);
                            }
                            else
                            {
                                lsts[5].Add(i);
                            }
                        }
                        else
                        {
                            if (v.X < ctr.X)
                            {
                                lsts[6].Add(i);
                            }
                            else
                            {
                                lsts[7].Add(i);
                            }
                        }
                    }
                }

                for (int i = 0; i < 8; i++)
                {
                    bxs[i].Indexes = lsts[i].ToArray();
                }
                //
                return bxs;
            }

            #endregion Internal Methods
        }

        #endregion Internal Classes

        #region Private Classes

        private class TSegmentEqualityComparer : IEqualityComparer<TSegment>
        {
            #region Public Fields

            public static readonly TSegmentEqualityComparer Default = new TSegmentEqualityComparer();

            #endregion Public Fields

            #region Public Methods

            public bool Equals(TSegment x, TSegment y)
            {
                return ((x.I1 == y.I1 && x.I2 == y.I2) || (x.I1 == y.I2 && x.I2 == y.I1));
            }

            public int GetHashCode(TSegment obj)
            {
                return 0;
            }

            #endregion Public Methods
        }

        private class TTriangleComparer : IEqualityComparer<TTriangle>, IComparer<TTriangle>
        {
            #region Public Fields

            public static readonly TTriangleComparer Default = new TTriangleComparer();

            #endregion Public Fields

            #region Public Methods

            public int Compare(TTriangle x, TTriangle y)
            {
                if (x.I1 != y.I1)
                {
                    return x.I1.CompareTo(y.I1);
                }
                else if (x.I2 != y.I2)
                {
                    return x.I2.CompareTo(y.I2);
                }
                else
                {
                    return x.I3.CompareTo(y.I3);
                }
            }

            public bool Equals(TTriangle x, TTriangle y)
            {
                return (x.I1 == y.I1 && x.I2 == y.I2 && x.I3 == y.I3);
            }

            public int GetHashCode(TTriangle obj)
            {
                return 0;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}