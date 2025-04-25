
using Math3D;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace CiliaElements.FormatCilia
{
    public class TCiliaAssembly : TAssemblyElement
    {
        #region Public Methods

        public override void LaunchLoad()
        {
            //
            FileStream rdr = Fi.OpenRead();
            GZipStream crdr = new GZipStream(rdr, System.IO.Compression.CompressionMode.Decompress);
            BinaryReader brdr = new BinaryReader(crdr);

            List<TBaseElement> elmts = new List<TBaseElement>();
            TSolidElement[] solids = new TSolidElement[brdr.ReadInt32()];
            for (int i = 0; i < solids.Length; i++)
            {
                TCiliaSolid solid = new TCiliaSolid() { PartNumber = brdr.ReadString() };
                solid.Fi = new FileInfo(solid.PartNumber);
                //
                int length = brdr.ReadInt32();
                byte[] buffer = new byte[length];
                brdr.Read(buffer, 0, length);
                MemoryStream ms = new MemoryStream();
                ms.Write(buffer, 0, length);
                ms.Position = 0;
                GZipStream ccwtr = new GZipStream(ms, CompressionMode.Decompress);
                BinaryReader bbrdr = new BinaryReader(ccwtr);
                solid.LaunchLoad(bbrdr);
                bbrdr.Dispose();
                ccwtr.Dispose();
                ms.Dispose();
                elmts.Add(solid);

                TFile f = new TFile(solid);
            }

            TAssemblyElement[] assys = new TAssemblyElement[brdr.ReadInt32()];
            assys[assys.Length - 1] = this;
            for (int i = 0; i < assys.Length - 1; i++)
            {
                assys[i] = new TAssemblyElement();
                elmts.Add(assys[i]);
            }
            for (int i = 0; i < assys.Length; i++)
            {
                TAssemblyElement assy = assys[i];
                assy.PartNumber = brdr.ReadString();
                assy.Fi = new FileInfo(assy.PartNumber);

                int ls = brdr.ReadInt32();
                for (int j = 0; j < ls; j++)
                {
                    TBaseElement sassy = elmts[brdr.ReadInt32()];
                    TLink l = TManager.AttachElmt(sassy.OwnerLink, assy.OwnerLink, null);
                    l.NodeName = brdr.ReadString();
                    Mtx4 m = new Mtx4();
                    m.Row0.X = brdr.ReadDouble();
                    m.Row0.Y = brdr.ReadDouble();
                    m.Row0.Z = brdr.ReadDouble();
                    m.Row0.W = brdr.ReadDouble();
                    m.Row1.X = brdr.ReadDouble();
                    m.Row1.Y = brdr.ReadDouble();
                    m.Row1.Z = brdr.ReadDouble();
                    m.Row1.W = brdr.ReadDouble();
                    m.Row2.X = brdr.ReadDouble();
                    m.Row2.Y = brdr.ReadDouble();
                    m.Row2.Z = brdr.ReadDouble();
                    m.Row2.W = brdr.ReadDouble();
                    m.Row3.X = brdr.ReadDouble();
                    m.Row3.Y = brdr.ReadDouble();
                    m.Row3.Z = brdr.ReadDouble();
                    m.Row3.W = brdr.ReadDouble();
                    l.Matrix = m;
                }
            }

            brdr.Dispose();
            crdr.Dispose();

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}