
using Math3D;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace CiliaElements.FormatCilia
{
    public class TCiliaSolid : TSolidElement
    {
        #region Private Fields

        private Mtx4 StartMatrix = Mtx4.CreateScale(1);

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            if (State == EElementState.Unknown)
            {
                FileStream rdr = Fi.OpenRead();
                GZipStream crdr = new GZipStream(rdr, System.IO.Compression.CompressionMode.Decompress);
                BinaryReader brdr = new BinaryReader(crdr);
                LaunchLoad(brdr);
                brdr.Dispose();
                crdr.Dispose();
            }
        }

        public void LaunchLoad(BinaryReader brdr)
        {
            // ------------------------------------------------------------------------
            Vec3[] vecs;
            Vec2f[] vecfs;
            Vec3 v = new Vec3();
            Vec2f vf = new Vec2f();
            vecs = new Vec3[brdr.ReadInt32()];
            for (int i = 0; i < vecs.Length; i++)
            {
                v.X = brdr.ReadDouble();
                v.Y = brdr.ReadDouble();
                v.Z = brdr.ReadDouble();
                vecs[i] = v;
            }
            DataPositions = vecs;
            vecs = new Vec3[vecs.Length];
            for (int i = 0; i < vecs.Length; i++)
            {
                v.X = brdr.ReadDouble();
                v.Y = brdr.ReadDouble();
                v.Z = brdr.ReadDouble();
                vecs[i] = v;
            }
            DataNormals = vecs;
            vecfs = new Vec2f[vecs.Length];
            for (int i = 0; i < vecs.Length; i++)
            {
                vf.X = brdr.ReadSingle();
                vf.Y = brdr.ReadSingle();
                vecfs[i] = vf;
            }
            DataTexts = vecfs;
            // ------------------------------------------------------------------------
            int[] idxs = new int[brdr.ReadInt32()];
            for (int i = 0; i < idxs.Length; i++)
            {
                idxs[i] = brdr.ReadInt32();
            }
            DataIndexes = idxs;
            // ------------------------------------------------------------------------
            FacesNumber = brdr.ReadInt32();
            FacesStart = brdr.ReadInt32();
            FacesStarter = brdr.ReadInt32();
            LinesNumber = brdr.ReadInt32();
            LinesStart = brdr.ReadInt32();
            LinesStarter = brdr.ReadInt32();
            PointsNumber = brdr.ReadInt32();
            PointsStart = brdr.ReadInt32();
            PointsStarter = brdr.ReadInt32();
            // ------------------------------------------------------------------------
            SBoundingBox3 b = new SBoundingBox3();
            b.MinPosition.X = brdr.ReadDouble();
            b.MinPosition.Y = brdr.ReadDouble();
            b.MinPosition.Z = brdr.ReadDouble();
            b.MinPosition.W = 1;
            b.MaxPosition.X = brdr.ReadDouble();
            b.MaxPosition.Y = brdr.ReadDouble();
            b.MaxPosition.Z = brdr.ReadDouble();
            b.MaxPosition.W = 1;
            BoundingBox = b;
            // ------------------------------------------------------------------------
            int imax = brdr.ReadInt32();
            List<TShape> shps = new List<TShape>();
            for (int i = 0; i < imax; i++)
            {
                TShape shp = new TShape();
                shp.BoundingBox.MinPosition.X = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.Y = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.Z = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.W = 1;
                shp.BoundingBox.MaxPosition.X = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.Y = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.Z = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.W = 1;
                shp.IndexesStart = brdr.ReadInt32();
                shp.IndexesEnd = brdr.ReadInt32();
                Surfaces.Push(shp);
            }
            Surfaces.Close();
            shps.AddRange(Surfaces.Values);
            // ------------------------------------------------------------------------
            imax = brdr.ReadInt32();
            for (int i = 0; i < imax; i++)
            {
                TShape shp = new TShape();
                shp.BoundingBox.MinPosition.X = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.Y = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.Z = brdr.ReadDouble();
                shp.BoundingBox.MinPosition.W = 1;
                shp.BoundingBox.MaxPosition.X = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.Y = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.Z = brdr.ReadDouble();
                shp.BoundingBox.MaxPosition.W = 1;
                shp.IndexesStart = brdr.ReadInt32();
                shp.IndexesEnd = brdr.ReadInt32();
                Curves.Push(shp);
            }
            Curves.Close();
            shps.AddRange(Curves.Values);
            // ------------------------------------------------------------------------
            Shapes = shps.ToArray();
            // ------------------------------------------------------------------------
            try
            {
                textureBmp = new System.Drawing.Bitmap(brdr.BaseStream);
            }
            catch { }
            // ------------------------------------------------------------------------
            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}