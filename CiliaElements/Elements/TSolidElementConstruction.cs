
using Math3D;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CiliaElements
{
    public class TSolidElementConstruction
    {
        #region Public Fields

        public int TextRange = 256;
        public int AllFacesNumber;
        public TQuickStc<TCloud> Clouds = new TQuickStc<TCloud>();

        //public Vec3[] DataNormals = { };
        //public Vec2f[] DataTexts = { };

        public TDico<string, object> Defs = new TDico<string, object>();
        public int[] FacesIndexes = { };
        public TQuickStc<TGroup> Groups = new TQuickStc<TGroup>();
        public Vec4 LastColor = new Vec4(0.5, 0.5, 0.5, 1);
        public int LastFace = 0;
        public int LastLine = 0;
        public int LastPoint = 0;
        public int LastPosition = 0;
        public int[] LinesIndexes = { };
        public int[] PointsIndexes = { };
        public int PositionsNumber;
        public int PositionsStart;
        public TSolidElement Solid;
        public TNGroup StartGroup = new TNGroup();
        public TQuickStc<TTextureCloud> TextureClouds = new TQuickStc<TTextureCloud>();
        public TQuickStc<TTexture> Textures = new TQuickStc<TTexture>();

        public double Unit = 1000;

        public Vec3f[] vsN;
        public Vec3f[] vsP;

        #endregion Public Fields

        #region Private Fields

        private static readonly int NoTextRange = 3;

        #endregion Private Fields

        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCloud AddCloud()
        {
            TCloud functionReturnValue = new TCloud();
            Clouds.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TCloud AddCloud(ref string iPendingDef)
        {
            TCloud c = AddCloud();
            if (!string.IsNullOrEmpty(iPendingDef))
            {
                Defs.Add(iPendingDef, c);
                iPendingDef = "";
            }

            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TFGroup AddFGroup()
        {
            TFGroup functionReturnValue = new TFGroup();
            Groups.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TLGroup AddLGroup()
        {
            TLGroup functionReturnValue = new TLGroup();
            Groups.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TNGroup AddNGroup(ref string iPendingDef)
        {
            TNGroup functionReturnValue = new TNGroup();
            Groups.Push(functionReturnValue);
            if (!string.IsNullOrEmpty(iPendingDef))
            {
                Defs.Add(iPendingDef, functionReturnValue);
                iPendingDef = "";
            }
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TNGroup AddNGroup()
        {
            TNGroup functionReturnValue = new TNGroup();
            Groups.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TPGroup AddPGroup()
        {
            TPGroup functionReturnValue = new TPGroup();
            Groups.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTexture AddTexture(float iA, float iR, float iG, float iB)
        {
            foreach (TTexture t in Textures.Values)
            {
                if (t != null && t.A == iA && t.R == iR && t.G == iG && t.B == iB)
                {
                    return t;
                }
            }

            TTexture functionReturnValue = new TTexture() { A = iA, R = iR, G = iG, B = iB };
            Textures.Push(functionReturnValue);
            return functionReturnValue;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddTexture(TTexture texture)
        {
            if (!Textures.Values.Contains(texture)) Textures.Push(texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTexture AddTexture(double iA, double iR, double iG, double iB)
        {
            foreach (TTexture t in Textures.Values)
            {
                if (t != null && t.A == (float)iA && t.R == (float)iR && t.G == (float)iG && t.B == (float)iB)
                {
                    return t;
                }
            }

            TTexture functionReturnValue = new TTexture() { A = (float)iA, R = (float)iR, G = (float)iG, B = (float)iB };
            Textures.Push(functionReturnValue);
            return functionReturnValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTexture AddTexture(ref string iPendingDef)
        {
            TTexture t = AddTexture(1, 0, 0, 0);
            if (!string.IsNullOrEmpty(iPendingDef))
            {
                Defs.Add(iPendingDef, t);
                iPendingDef = "";
            }
            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TTextureCloud AddTextureCloud()
        {
            TTextureCloud functionReturnValue = new TTextureCloud();
            TextureClouds.Push(functionReturnValue);
            return functionReturnValue;
        }

        public void Publish()
        {
            if (!(Solid is FormatCilia.TCiliaSolid))
            {
                Textures.Close();
                Clouds.Close();
                Groups.Close();
                StartGroup.PostProcess(Solid);
                //
                int tn = Textures.Max;
                int ts = NoTextRange;
                foreach (TTexture t in Textures.Values)
                {
                    if (t != null && t.KDBitmap != null) { ts = TextRange; break; }
                }

                if (tn > 0)
                {
                    Solid.textureBmp = new Bitmap(ts * 2, ts * tn);
                    Graphics grp = Graphics.FromImage(Solid.textureBmp);
                    grp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    grp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    grp.Clear(Color.DarkBlue);
                    for (int i = 0; i < tn; i++)
                    {
                        TTexture t = Textures.Values[i];
                        t.Zero = (double)i * 1 / tn;
                        if (t.KDBitmap != null)
                        {
                            t.One = 1 / (double)tn;
                            Image img = (Image)t.KDBitmap.Clone();
                            grp.DrawImage(img, 0, ts * i, ts, ts);
                            img.Dispose();
                            if (t.KEBitmap != null)
                            {
                                img = (Image)t.KEBitmap.Clone();
                                grp.DrawImage(img, ts, ts * i, ts, ts);
                                img.Dispose();
                            }
                        }
                        else
                        {
                            t.One = t.Zero + 0.5 / tn;
                            SolidBrush b = new SolidBrush(Color.FromArgb((int)(t.A * 255), (int)(t.R * 255), (int)(t.G * 255), (int)(t.B * 255)));
                            grp.FillRectangle(b, new Rectangle(0, ts * i, ts, ts));
                            b.Dispose();
                        }
                    }
                    grp.Dispose();
                }
                //
                Array.ForEach(Clouds.Values, c => { Array.Resize(ref c.Indexes, c.Vectors.Max); });
                //
                Array.ForEach(Groups.Values, g => { g.PostProcess(Solid); });
                //
                StartGroup.AbsoluteMatrixes.Push(Mtx4.Identity);
                {
                    TQuickStc<TNGroup> stc = new TQuickStc<TNGroup>();
                    stc.Push(StartGroup);
                    while (stc.NotEmpty)
                    {
                        TNGroup iGroup = stc.Pop();
                        while (iGroup.AbsoluteMatrixes.NotEmpty)
                        {
                            Mtx4 iMatrix = iGroup.AbsoluteMatrixes.Pop();
                            iMatrix = iGroup.GroupParameters.Matrix * iMatrix;
                            //
                            Array.ForEach(iGroup.NGroups.Values, g => { g.AbsoluteMatrixes.Push(iMatrix); });
                            stc.Push(iGroup.NGroups.Values);
                            //
                            Array.ForEach(iGroup.FGroups.Values, group =>
                            {
                                TShape Shape = new TShape() { ShapeParameters = new TShapeParameters() { Group = group }, Entity = group.ShapeGroupParameters.Entity, PositionsStart = LastPosition };
                                //
                                Shape.ShapeParameters.Matrix = group.GroupParameters.Matrix * iMatrix;
                                Shape.BoundingBox = SBoundingBox3.Transform(group.BoundingBox, Shape.ShapeParameters.Matrix);
                                LastPosition += group.CloudPositions.Length;
                                //
                                Solid.FacesNumber += group.FilteredIndexes.Length;
                                LastFace += group.FilteredIndexes.Length;
                                //
                                Solid.Surfaces.Push(Shape);
                                //
                            });

                            //
                            Array.ForEach(iGroup.LGroups.Values, group =>
                            {
                                TShape Shape = new TShape { ShapeParameters = new TShapeParameters() { Group = group }, Entity = group.ShapeGroupParameters.Entity, PositionsStart = LastPosition };
                                //
                                Shape.ShapeParameters.Matrix = group.GroupParameters.Matrix * iMatrix;
                                Shape.BoundingBox = SBoundingBox3.Transform(group.BoundingBox, Shape.ShapeParameters.Matrix);
                                LastPosition += group.CloudPositions.Length;
                                //
                                Solid.LinesNumber += group.FilteredIndexes.Length;
                                LastLine += group.FilteredIndexes.Length;
                                //
                                Solid.Curves.Push(Shape);
                            });

                            //
                            Array.ForEach(iGroup.PGroups.Values, group =>
                            {
                                TShape Shape = new TShape { ShapeParameters = new TShapeParameters() { Group = group }, Entity = group.ShapeGroupParameters.Entity, PositionsStart = LastPosition };
                                //
                                Shape.ShapeParameters.Matrix = group.GroupParameters.Matrix * iMatrix;
                                Shape.BoundingBox = SBoundingBox3.Transform(group.BoundingBox, Shape.ShapeParameters.Matrix);
                                LastPosition += group.CloudPositions.Length;
                                //
                                LastPoint += group.FilteredIndexes.Length;
                                //
                                Solid.Points.Push(Shape);
                                //
                            });
                        }
                    }
                    stc.Dispose();
                }
                //
                Solid.Surfaces.Close();
                Solid.Curves.Close();
                Solid.Points.Close();
                TShape[] tbls = Solid.Shapes; Array.Resize(ref tbls, Solid.Surfaces.Max + Solid.Curves.Max + Solid.Points.Max); Solid.Shapes = tbls;
                Array.Copy(Solid.Surfaces.Values, 0, Solid.Shapes, 0, Solid.Surfaces.Max);
                Array.Copy(Solid.Curves.Values, 0, Solid.Shapes, Solid.Surfaces.Max, Solid.Curves.Max);
                Array.Copy(Solid.Points.Values, 0, Solid.Shapes, Solid.Surfaces.Max + Solid.Curves.Max, Solid.Points.Max);
                //
                if (LastPosition > 0)
                {
                    //
                    //
                    Vec2f[] tblvf;
                    Vec3[] tblv;
                    tblvf = Solid.DataTexts; Array.Resize(ref tblvf, LastPosition); Solid.DataTexts = tblvf;
                    tblv = Solid.DataPositions; Array.Resize(ref tblv, LastPosition); Solid.DataPositions = tblv;
                    tblv = Solid.DataNormals; Array.Resize(ref tblv, LastPosition); Solid.DataNormals = tblv;
                    Array.Resize(ref FacesIndexes, LastFace);
                    Array.Resize(ref LinesIndexes, LastLine);
                    Array.Resize(ref PointsIndexes, LastPoint);
                    //
                    Solid.FacesStart = FacesIndexes.Length - Solid.FacesNumber;
                    //
                    int cpt;
                    cpt = 0; Array.ForEach(Solid.Surfaces.Values, shape => { shape.ShapeParameters.Group.Fill(Solid, shape, ref cpt); });
                    cpt = 0; Array.ForEach(Solid.Curves.Values, shape => { shape.ShapeParameters.Group.Fill(Solid, shape, ref cpt); });
                    cpt = 0; Array.ForEach(Solid.Points.Values, shape => { shape.ShapeParameters.Group.Fill(Solid, shape, ref cpt); });
                    //
                    Solid.FacesStart = FacesIndexes.Length - Solid.FacesNumber;
                    Solid.LinesStart = FacesIndexes.Length;
                    Solid.PointsStart = Solid.LinesStart + LinesIndexes.Length;
                    Solid.FacesStarter = Solid.FacesStart * 4;
                    Solid.LinesStarter = Solid.LinesStart * 4;
                    Solid.PointsStarter = Solid.PointsStart * 4;
                    PositionsStart = 0;
                    //
                    Solid.LinesNumber = LinesIndexes.Length;
                    Solid.PointsNumber = PointsIndexes.Length;
                    PositionsNumber = Solid.DataPositions.Length;
                    //
                    int[] tbli = Solid.DataIndexes; Array.Resize(ref tbli, Solid.PointsStart + Solid.PointsNumber); Solid.DataIndexes = tbli;
                    Array.Copy(FacesIndexes, 0, Solid.DataIndexes, 0, Solid.FacesNumber);
                    Array.Copy(LinesIndexes, 0, Solid.DataIndexes, Solid.LinesStart, Solid.LinesNumber);
                    Array.Copy(PointsIndexes, 0, Solid.DataIndexes, Solid.PointsStart, Solid.PointsNumber);
                    //
                    AllFacesNumber = Solid.FacesNumber;
                    FacesIndexes = null;
                    LinesIndexes = null;
                    PointsIndexes = null;
                }
                else
                {
                    Solid.DataPositions = new Vec3[] { };
                    Solid.DataIndexes = new int[] { };
                }
                //

                if (Solid.DataPositions.Length > 0)
                {
                    SBoundingBox3 bx = new SBoundingBox3(Solid.DataPositions[0]);
                    Array.ForEach(Solid.Surfaces.Values, o => { bx.AddBox(o.BoundingBox); o.ShapeParameters.Group = null; });
                    Array.ForEach(Solid.Curves.Values, o => { bx.AddBox(o.BoundingBox); o.ShapeParameters.Group = null; });
                    Array.ForEach(Solid.Points.Values, o => { bx.AddBox(o.BoundingBox); o.ShapeParameters.Group = null; });
                    Solid.BoundingBox = bx;
                }
                else
                {
                    Solid.BoundingBox = SBoundingBox3.Default;
                }
            }
            //
            //          
            lock (Solid.TextureDataLocker)
                if (Solid.textureBmp != null) Solid.TextureData = Solid.textureBmp.LockBits(new Rectangle(0, 0, Solid.textureBmp.Width, Solid.textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //
            Solid.State = EElementState.Published;
            //
            vsP = Array.ConvertAll(Solid.DataPositions, Vec3f.Convert);
            vsN = Array.ConvertAll(Solid.DataNormals, Vec3f.Convert);
            //

            //
            TManager.TotalFacets += Solid.FacesNumber / 3;
            TManager.TotalPositions += PositionsNumber;
            //z
            TManager.SolidsToBePushed.Push(Solid);
            //
        }

        #endregion Public Methods
    }
}