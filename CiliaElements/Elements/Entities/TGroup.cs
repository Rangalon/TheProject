
using Math3D;
using System;
using System.Linq;

namespace CiliaElements
{
    public class TFGroup : TShapeGroup
    {
        #region Public Methods

        public override void Fill(TSolidElement iSolid, TShape iShape, ref int n)
        {
            iShape.IndexesStart = n;
            iShape.IndexesEnd = n + FilteredIndexes.Length;
            //
            if (GroupParameters.Texture == null)
            {
                return;
            }
            //
            Array.Copy(Array.ConvertAll(FilteredIndexes, iShape.OffsetIndice), 0, iSolid.SolidElementConstruction.FacesIndexes, iShape.IndexesStart, FilteredIndexes.Length);
            //
            TemporaryMatrix = iShape.ShapeParameters.Matrix;

            //Vec3[] vvs = iSolid.SolidElementConstruction.CLTools.RunMovePoints(CloudPositions, TemporaryMatrix.Value);

            //Array.Copy(vvs, 0, iSolid.DataPositions, iShape.PositionsStart, CloudPositions.Length);
            Array.Copy(Array.ConvertAll(CloudPositions, ConvertVec3), 0, iSolid.DataPositions, iShape.PositionsStart, CloudPositions.Length);
            Array.Copy(Array.ConvertAll(CloudNormals, ConvertVec3), 0, iSolid.DataNormals, iShape.PositionsStart, CloudPositions.Length);
            if (ShapeGroupParameters.Textures != null && GroupParameters.Texture.KDBitmap != null)
            {
                n += 0;
                Array.Copy(Array.ConvertAll(CloudTextures, GroupParameters.Texture.ConvertVec2f), 0, iSolid.DataTexts, iShape.PositionsStart, CloudPositions.Length);
            }
            else
            {
                Vec2f v = new Vec2f(0.25F, (float)GroupParameters.Texture.One);
                for (int i = iShape.PositionsStart; i < iShape.PositionsStart + CloudPositions.Length; i++)
                {
                    iSolid.DataTexts[i] = v;
                }
            }

            TemporaryMatrix = null;
            //
            n = iShape.IndexesEnd;
        }

        public override void PostProcess(TSolidElement iSolid)
        {
            if (GroupParameters.Texture == null)
            {
                return;
            }
            //
            //
            int nb = 0;
            Indexes.Close();

            bool bNormals = ShapeGroupParameters.Normals != null && ShapeGroupParameters.Normals.Vectors.Max == ShapeGroupParameters.Positions.Vectors.Max;

            if (bNormals)
            {
                ShapeGroupParameters.Normals.Vectors.Close();
            }

            ShapeGroupParameters.Positions.Vectors.Close();

            Array.Resize(ref FilteredIndexes, Indexes.Max);
            if (ShapeGroupParameters.Textures != null && ShapeGroupParameters.Textures.Vectors.Max == ShapeGroupParameters.Positions.Vectors.Max)
            {
                ShapeGroupParameters.Textures.Vectors.Close();
                //
                BoundingBox.ResetBox(ShapeGroupParameters.Positions.Vectors.Values[Indexes.Values[0]]);
                for (int j = 0; j < Indexes.Max; j++)
                {
                    int i = Indexes.Values[j];
                    if (!ShapeGroupParameters.Positions.Indexes[i].HasValue)
                    {
                        ShapeGroupParameters.Positions.Indexes[i] = nb;
                        nb++;
                        BoundingBox.CheckPosition(ShapeGroupParameters.Positions.Vectors.Values[i]);
                    }
                    FilteredIndexes[j] = ShapeGroupParameters.Positions.Indexes[i].Value;
                }
                //
                Array.Resize(ref CloudPositions, nb);
                Array.Resize(ref CloudNormals, nb);
                Array.Resize(ref CloudTextures, nb);
                //
                if (bNormals)
                {
                    foreach (int i in Indexes.Values.Where(o => ShapeGroupParameters.Positions.Indexes[o].HasValue))
                    {
                        CloudPositions[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Positions.Vectors.Values[i];
                        CloudNormals[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Normals.Vectors.Values[i] * 1000;
                        CloudTextures[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Textures.Vectors.Values[i];
                        ShapeGroupParameters.Positions.Indexes[i] = null;
                    }
                }
                else
                {
                    foreach (int i in Indexes.Values.Where(o => ShapeGroupParameters.Positions.Indexes[o].HasValue))
                    {
                        CloudPositions[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Positions.Vectors.Values[i];
                        CloudTextures[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Textures.Vectors.Values[i];
                        ShapeGroupParameters.Positions.Indexes[i] = null;
                    }
                } 
            }
            else
            {
                //
                BoundingBox.ResetBox(ShapeGroupParameters.Positions.Vectors.Values[Indexes.Values[0]]);
                for (int j = 0; j < Indexes.Max; j++)
                {
                    int i = Indexes.Values[j];
                    if (!ShapeGroupParameters.Positions.Indexes[i].HasValue)
                    {
                        ShapeGroupParameters.Positions.Indexes[i] = nb;
                        nb++;
                        BoundingBox.CheckPosition(ShapeGroupParameters.Positions.Vectors.Values[i]);
                    }
                    FilteredIndexes[j] = ShapeGroupParameters.Positions.Indexes[i].Value;
                }
                //
                Array.Resize(ref CloudPositions, nb);
                Array.Resize(ref CloudNormals, nb);
                //
                if (ShapeGroupParameters.Normals != null)
                {
                    foreach (int i in Indexes.Values.Where(o => ShapeGroupParameters.Positions.Indexes[o].HasValue))
                    {
                        CloudPositions[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Positions.Vectors.Values[i];
                        CloudNormals[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Normals.Vectors.Values[i] * 1000;
                        ShapeGroupParameters.Positions.Indexes[i] = null;
                    }
                }
                else
                {
                    foreach (int i in Indexes.Values.Where(o => ShapeGroupParameters.Positions.Indexes[o].HasValue))
                    {
                        CloudPositions[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Positions.Vectors.Values[i];
                        ShapeGroupParameters.Positions.Indexes[i] = null;
                    }
                }
            }

            if (!bNormals)
            {
                nb = CloudPositions.Length;
                Vec3[] Vecs = new Vec3[nb];
                //int[] Inds = new int[nb];
                //
                for (int j = FilteredIndexes.Length - 3; j > -1; j += -3)
                {
                    int i0 = FilteredIndexes[j];
                    int i1 = FilteredIndexes[j + 1];
                    int i2 = FilteredIndexes[j + 2];
                    Vec3 v = Vec3.Cross(CloudPositions[i0] - CloudPositions[i1], CloudPositions[i0] - CloudPositions[i2]);

                    if (v.LengthSquared > 0)
                    {
                        //v.Normalize();
                        Vecs[i0] += v;
                        Vecs[i1] += v;
                        Vecs[i2] += v;
                        //Inds[i0]++;
                        //Inds[i1]++;
                        //Inds[i2]++;
                    }
                }
                //
                for (int i = nb - 1; i > -1; i--)
                {
                    //Vecs[i] /= Inds[i];
                    Vecs[i].Normalize();
                    CloudNormals[i] = Vecs[i] * 1000;
                }

                //
                Vecs = null;
                //Inds = null;
            }
            //

            //
            //n += GroupParameters.Usage;
        }

        #endregion Public Methods
    }

    public abstract class TGroup
    {
        #region Public Fields

        public SBoundingBox3 BoundingBox = SBoundingBox3.Default;

        public TGroupParameters GroupParameters = new TGroupParameters();

        #endregion Public Fields

        #region Public Methods

        public virtual void Dispose()
        {
            GroupParameters = null;
        }

        public abstract void Fill(TSolidElement iSolid, TShape iShape, ref int n);

        public abstract void PostProcess(TSolidElement iSolid);

        #endregion Public Methods
    }

    public class TGroupParameters
    {
        #region Public Fields

        public int GroupsNumber = 0;
        public Mtx4 Matrix = Mtx4.Identity;
        public TTexture Texture = null;

        #endregion Public Fields

        //public Int32 Usage = 0;

        #region Internal Methods

        internal void Dispose()
        {
            Texture = null;
        }

        #endregion Internal Methods
    }

    public class TLGroup : TShapeGroup
    {
        #region Public Methods

        public override void Fill(TSolidElement iSolid, TShape iShape, ref int n)
        {
            iShape.IndexesStart = n;
            iShape.IndexesEnd = n + FilteredIndexes.Length;
            Array.Copy(Array.ConvertAll(FilteredIndexes, iShape.OffsetIndice), 0, iSolid.SolidElementConstruction.LinesIndexes, iShape.IndexesStart, FilteredIndexes.Length);
            //
            TemporaryMatrix = iShape.ShapeParameters.Matrix;
            Vec2f v = new Vec2f(0.25F, (float)GroupParameters.Texture.One);
            for (int i = CloudPositions.Length - 1; i > -1; i--)
            {
                iSolid.DataPositions[iShape.PositionsStart + i] = Vec4.TransformPoint(CloudPositions[i], TemporaryMatrix.Value);
                iSolid.DataTexts[iShape.PositionsStart + i] = v;
            }

            TemporaryMatrix = null;
            //
            n = iShape.IndexesEnd;
        }

        public override void PostProcess(TSolidElement iSolid)
        {
            if (Indexes.Max == 0)
            {
                return;
            }

            int nb = 0;
            int nbr = 0;
            ShapeGroupParameters.Positions.Vectors.Close();
            BoundingBox.ResetBox(ShapeGroupParameters.Positions.Vectors.Values[Indexes.Values[0]]);
            if (Indexes.Last == -1)
            {
                Indexes.RemoveLast();
            }

            Indexes.Close();
            foreach (int i in Indexes.Values)
            {
                if (i > -1)
                {
                    if (!ShapeGroupParameters.Positions.Indexes[i].HasValue)
                    {
                        ShapeGroupParameters.Positions.Indexes[i] = nb;
                        nb++;
                        BoundingBox.CheckPosition(ShapeGroupParameters.Positions.Vectors.Values[i]);
                    }
                }
                else
                {
                    nbr++;
                }
            }
            //
            Array.Resize(ref FilteredIndexes, (Indexes.Max - nbr * 2) * 2);
            int j = 0;
            for (int i = 0; i < Indexes.Max - 1; i++)
            {
                if (Indexes.Values[i + 1] < 0) { i++; continue; }
                FilteredIndexes[j] = ShapeGroupParameters.Positions.Indexes[Indexes.Values[i]].Value;
                FilteredIndexes[j + 1] = ShapeGroupParameters.Positions.Indexes[Indexes.Values[i + 1]].Value;
                j += 2;
            }
            //
            Array.Resize(ref CloudPositions, nb);
            //
            foreach (int i in Indexes.Values.Where(o => o > -1 && ShapeGroupParameters.Positions.Indexes[o].HasValue))
            {
                CloudPositions[ShapeGroupParameters.Positions.Indexes[i].Value] = ShapeGroupParameters.Positions.Vectors.Values[i];
                ShapeGroupParameters.Positions.Indexes[i] = null;
                nb--;
                if (nb == 0)
                {
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
            //
            //n += GroupParameters.Usage;
        }

        #endregion Public Methods
    }

    public class TNGroup : TGroup
    {
        #region Public Fields

        public TQuickStc<Mtx4> AbsoluteMatrixes = new TQuickStc<Mtx4>();

        public TQuickStc<TFGroup> FGroups = new TQuickStc<TFGroup>();
        public TQuickStc<TLGroup> LGroups = new TQuickStc<TLGroup>();
        public TQuickStc<TNGroup> NGroups = new TQuickStc<TNGroup>();
        public TQuickStc<TPGroup> PGroups = new TQuickStc<TPGroup>();

        #endregion Public Fields

        #region Public Methods

        public override void Dispose()
        {
            GroupParameters = null;
            NGroups.Dispose();
            FGroups.Dispose();
            LGroups.Dispose();
            PGroups.Dispose();
            AbsoluteMatrixes.Dispose();
            NGroups = null;
            FGroups = null;
            LGroups = null;
            PGroups = null;
            AbsoluteMatrixes.Clear();
        }

        public override void Fill(TSolidElement iSolid, TShape iShape, ref int n)
        {
            throw new NotImplementedException();
        }

        public override void PostProcess(TSolidElement iSolid)
        {
            NGroups.Close();
            FGroups.Close();
            PGroups.Close();
            LGroups.Close();
        }

        #endregion Public Methods
    }

    public class TPGroup : TShapeGroup
    {
        #region Public Methods

        public override void Fill(TSolidElement iSolid, TShape iShape, ref int n)
        {
            Array.Copy(Array.ConvertAll(FilteredIndexes, iShape.OffsetIndice), 0, iSolid.SolidElementConstruction.PointsIndexes, 0, FilteredIndexes.Length);
            //
            TemporaryMatrix = iShape.ShapeParameters.Matrix;
            Vec2f v = new Vec2f(0.25F, (float)GroupParameters.Texture.One);
            for (int i = CloudPositions.Length - 1; i > -1; i--)
            {
                iSolid.DataPositions[iShape.PositionsStart + i] = Vec4.TransformPoint(CloudPositions[i], TemporaryMatrix.Value);
                iSolid.DataTexts[iShape.PositionsStart + i] = v;
            }
            TemporaryMatrix = null;
        }

        public override void PostProcess(TSolidElement iSolid)
        {
            //
            Indexes.Close();
            ShapeGroupParameters.Positions.Vectors.Close();
            Array.Resize(ref FilteredIndexes, ShapeGroupParameters.Positions.Vectors.Max);
            BoundingBox.ResetBox(ShapeGroupParameters.Positions.Vectors.Values[Indexes.Values[0]]);
            for (int i = 0; i < ShapeGroupParameters.Positions.Vectors.Max; i++)
            {
                Vec3 v = ShapeGroupParameters.Positions.Vectors.Values[i];
                FilteredIndexes[i] = i;
                //CloudPositions.Length)
                BoundingBox.CheckPosition(v);
            }
            //
            //
            CloudPositions = ShapeGroupParameters.Positions.Vectors.Values;
            //
        }

        #endregion Public Methods
    }

    public abstract class TShapeGroup : TGroup
    {
        #region Public Fields

        public Vec3[] CloudNormals = { };
        public Vec3[] CloudPositions = { };
        public Vec2[] CloudTextures = { };
        public int[] FilteredIndexes = { };
        public TQuickStc<int> Indexes = new TQuickStc<int>();
        public TShapeGroupParameters ShapeGroupParameters = new TShapeGroupParameters();

        public Mtx4? TemporaryMatrix;

        #endregion Public Fields

        #region Public Methods

        public Vec3 ConvertVec3(Vec3 v)
        {
            return Vec4.TransformPoint(v, TemporaryMatrix.Value);
        }

        public override void Dispose()
        {
            CloudPositions = null;
            CloudNormals = null;
            FilteredIndexes = null;
            Indexes = null;
            ShapeGroupParameters.Dispose();
            ShapeGroupParameters = null;
            GroupParameters.Dispose();
            GroupParameters = null;
        }

        #endregion Public Methods
    }

    public class TShapeGroupParameters
    {
        #region Public Fields

        public TEntity Entity;
        public TCloud Normals;
        public TCloud Positions;
        public TTextureCloud Textures;

        #endregion Public Fields

        #region Internal Methods

        internal void Dispose()
        {
            Entity = null;
            Normals = null;
            Positions = null;
        }

        #endregion Internal Methods
    }
}