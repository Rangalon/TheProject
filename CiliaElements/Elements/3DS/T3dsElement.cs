
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Format3DS
{
    public class T3dsElement : TAssemblyElement
    {
        #region Public Fields

        public BinaryReader BReader;

        public FileStream FReader;

        public float MasterScale = 1;

        public int MeshVersion;

        public List<TSolidElement> Solids = new List<TSolidElement>();

        public Dictionary<string, TSolidElement> SolidsIds = new Dictionary<string, TSolidElement>();

        public Dictionary<string, TTexture> TexturesIds = new Dictionary<string, TTexture>();

        public byte Version;

        #endregion Public Fields

        #region Public Enums

        public enum EMarker
        {
            Null = 0,
            UseSolidBackground = 0x1201,
            SolidBackground = 0x1200,
            AmbientLight = 0x2100,
            ViewportLayout = 0x7001,
            KFData = 0xb000,
            NamedObject = 0x4000,
            MasterScale = 0x100,
            MAIN3DS = 0x4d4d,
            M3DVersion = 0x2,
            MeshVersion = 0x3d3e,
            Edit3DS = 0x3d3d,
            EditConfig2 = 0x3e3d,
            MaterialEntry = 0xafff,
            NTriObject = 0x4100,
            NCamera = 0x4700,
            PointArray = 0x4110,
            FaceArray = 0x4120,
            TexVerts = 0x4140,
            MeshMatrix = 0x4160,
            MatName = 0xa000,
            MatAmbient = 0xa010,
            MatDiffuse = 0xa020,
            MatSpecular = 0xa030,
            Shininess = 0xa040,
            Shin2Pct = 0xa041,
            MatTexMap = 0xa200,
            Color24 = 0x11,
            LinColor24 = 0x12,
            MeshMatGroup = 0x4130,
            Transparency = 0xa050,
            Shading = 0xa100,
            SmoothGroup = 0x4150,
            KFHDR = 0xb00a,
            KFSEG = 0xb008,
            KFCURTIME = 0xb009,
            AMBIENTNODETAG = 0xb001,
            OBJECTNODETAG = 0xb002,
            CAMERANODETAG = 0xb003,
            TARGETNODETAG = 0xb004,
            LIGHTNODETAG = 0xb005,
            LTARGETNODETAG = 0xb006,
            SPOTLIGHTNODETAG = 0xb007,
            NODEID = 0xb030,
            NODEHDR = 0xb010,
            PIVOT = 0xb013,
            POSTRACKTAG = 0xb020,
            ROTTRACKTAG = 0xb021,
            SCLTRACKTAG = 0xb022
        }

        #endregion Public Enums

        #region Public Methods

        public void GetChunks(long iStart, long iEnd)
        {
            FReader.Position = iStart;
            while (FReader.Position < iEnd)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.MAIN3DS:
                        GetChunks(chunk.Start, chunk.Finish);
                        break;

                    case EMarker.M3DVersion:
                        Version = BReader.ReadByte();
                        break;

                    case EMarker.Edit3DS:
                        GetChunks(chunk.Start, chunk.Finish);
                        break;

                    case EMarker.EditConfig2:
                        GetChunks(chunk.Start, chunk.Finish);
                        break;

                    case EMarker.MeshVersion:
                        MeshVersion = BReader.ReadInt16();
                        break;

                    case EMarker.MaterialEntry:
                        GetMaterial(chunk);
                        break;

                    case EMarker.MasterScale:
                        MasterScale = BReader.ReadSingle() / 1000;
                        break;

                    case EMarker.NamedObject:
                        GetNamedObject(chunk);
                        break;

                    case EMarker.ViewportLayout:
                        break;

                    case EMarker.AmbientLight:
                        break;

                    case EMarker.SolidBackground:
                        break;

                    case EMarker.UseSolidBackground:
                        break;

                    case EMarker.KFData:
                        GetKFData(chunk);
                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
        }

        public string GetString()
        {
            string functionReturnValue = null;
            functionReturnValue = "";
            char c = BReader.ReadChar();
            while (c != (char)0)
            {
                functionReturnValue += c;
                c = BReader.ReadChar();
            }
            return functionReturnValue;
        }

        public override void LaunchLoad()
        {
            FReader = Fi.OpenRead();
            BReader = new BinaryReader(FReader);
            GetChunks(0, FReader.Length);
            BReader.Close();
            BReader.Dispose();
            FReader.Close();
            FReader.Dispose();
            //
            if (OwnerLink.Links.Max == 0)
            {
                foreach (TSolidElement solid in Solids)
                {
                    TLink link = new TLink { NodeName = solid.PartNumber };
                    link.Child = solid;
                    link.ParentLink = this.OwnerLink;
                }
            }
            //
            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private TChunk GetChunk()
        {
            TChunk functionReturnValue = default(TChunk);
            functionReturnValue.Marker = (EMarker)BReader.ReadUInt16();
            functionReturnValue.Length = BReader.ReadUInt32();
            functionReturnValue.Start = FReader.Position;
            functionReturnValue.Finish = functionReturnValue.Start + functionReturnValue.Length - 6;
            return functionReturnValue;
        }

        private void GetColor(TTexture iTexture, TChunk iParentChunk)
        {
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.LinColor24:
                    case EMarker.Color24:
                        iTexture.R = (float)(BReader.ReadByte()) / 256F;
                        iTexture.G = (float)(BReader.ReadByte()) / 256F;
                        iTexture.B = (float)(BReader.ReadByte()) / 256F;
                        iTexture.A = 1;
                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
        }

        private void GetFaceArray(TSolidElement iSolid, TFGroup iGroup, TChunk iParentChunk)
        {
            int n = BReader.ReadUInt16() * 3;
            //Array.Resize(ref iGroup.Indexes, n);
            n -= 1;
            for (int i = 0; i <= n; i += 3)
            {
                iGroup.Indexes.Push(BReader.ReadUInt16());
                iGroup.Indexes.Push(BReader.ReadUInt16());
                iGroup.Indexes.Push(BReader.ReadUInt16());
                BReader.ReadUInt16();
            }
            //
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.MeshMatGroup:
                        //TTexture Texture = iSolid.SolidElementConstruction.AddTexture();
                        //Texture.Color = TexturesIds[GetString()].Color;
                        iGroup.GroupParameters.Texture = TexturesIds[GetString()];
                        int i = BReader.ReadUInt16();
                        break;

                    case EMarker.SmoothGroup:
                        break;

                    default:
                        //chunk = chunk
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
        }

        private void GetKFData(TChunk iParentChunk)
        {
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.KFHDR:
                        break;

                    case EMarker.KFSEG:
                        break;

                    case EMarker.KFCURTIME:
                        break;

                    case EMarker.AMBIENTNODETAG:
                        break;

                    case EMarker.OBJECTNODETAG:
                        ParseObjectNode(chunk);
                        break;

                    case EMarker.CAMERANODETAG:
                        break;

                    case EMarker.TARGETNODETAG:
                        break;

                    case EMarker.LIGHTNODETAG:
                        break;

                    case EMarker.LTARGETNODETAG:
                        break;

                    case EMarker.SPOTLIGHTNODETAG:
                        break;

                    case EMarker.NODEID:
                        break;

                    case EMarker.NODEHDR:
                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
        }

        private void GetMaterial(TChunk iParentChunk)
        {
            TTexture texture = new TTexture();
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.MatName:
                        TexturesIds.Add(GetString(), texture);
                        break;

                    case EMarker.MatAmbient:
                        break;
                    //GetColor(texture, chunk)
                    case EMarker.MatDiffuse:
                        break;
                    //GetColor(texture, chunk)
                    case EMarker.MatSpecular:
                        GetColor(texture, chunk);
                        break;

                    case EMarker.MatTexMap:
                        break;

                    case EMarker.Shininess:
                        break;

                    case EMarker.Shin2Pct:
                        break;

                    case EMarker.Transparency:
                        break;

                    case EMarker.Shading:
                        break;

                    default:
                        break;
                    //Throw New Exception("Unplanned chunk " + chunk.Marker.ToString)
                }
                FReader.Position = chunk.Finish;
            }
        }

        private void GetNTriObject(TChunk iParentChunk, TSolidElement iSolid)
        {
            TCloud cloud = null;
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.PointArray:
                        cloud = iSolid.SolidElementConstruction.AddCloud();
                        GetPointArray(cloud);
                        break;

                    case EMarker.FaceArray:
                        TFGroup group = iSolid.SolidElementConstruction.AddFGroup();
                        group.ShapeGroupParameters.Positions = cloud;
                        iSolid.SolidElementConstruction.StartGroup.FGroups.Push(group);
                        GetFaceArray(iSolid, group, chunk);
                        break;

                    case EMarker.TexVerts:
                        break;

                    case EMarker.MeshMatrix:
                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
        }

        private void GetNamedObject(TChunk iParentChunk)
        {
            //
            TSolidElement solid = new TSolidElement
            {
                PartNumber = GetString()
            };
            solid.Fi = new FileInfo(Fi.FullName + "\\" + solid.PartNumber + ".InternalSolid");
            Solids.Add(solid);
            //
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.NTriObject:
                        GetNTriObject(chunk, solid);
                        break;

                    case EMarker.NCamera:
                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
            //
            //TFile file = new TFile(solid);
        }

        private void GetPointArray(TCloud iCloud)
        {
            int n = BReader.ReadUInt16() - 1;
            //Array.Resize(ref iCloud.Vectors, n + 1);
            for (int i = 0; i <= n; i++)
            {
                Vec3 v = default(Vec3);
                v.X = BReader.ReadSingle() * MasterScale;
                v.Y = BReader.ReadSingle() * MasterScale;
                v.Z = BReader.ReadSingle() * MasterScale;
                iCloud.Vectors.Push(v);
            }
        }

        private void ParseObjectNode(TChunk iParentChunk)
        {
            //int NodeId = 0;
            string NodeName = "";
            //int ParentId = 0;
            Mtx4 mtx = Mtx4.Identity;
            ushort fl;
            uint dw1;
            uint dw2;
            uint nk;
            Vec3 v;
            while (FReader.Position < iParentChunk.Finish)
            {
                TChunk chunk = GetChunk();
                switch (chunk.Marker)
                {
                    case EMarker.NODEHDR:
                        NodeName = GetString();
                        int fl1 = BReader.ReadUInt16();
                        int fl2 = BReader.ReadUInt16();
                        //ParentId =
                        BReader.ReadUInt16();
                        break;

                    case EMarker.NODEID:
                        //NodeId =
                        BReader.ReadUInt16();
                        break;

                    case EMarker.PIVOT:
                        v.X = BReader.ReadSingle();
                        v.Y = BReader.ReadSingle();
                        v.Z = BReader.ReadSingle();
                        break;

                    case EMarker.ROTTRACKTAG:
                        BReader.ReadByte();
                        fl = BReader.ReadUInt16();
                        dw1 = BReader.ReadUInt32();
                        dw2 = BReader.ReadUInt32();
                        nk = BReader.ReadUInt32();
                        //
                        for (int j = 1; j <= nk; j++)
                        {
                            int i = BReader.ReadInt32();
                            int flg = BReader.ReadUInt16();
                            float a = BReader.ReadSingle();
                            v.X = BReader.ReadSingle();
                            v.Y = BReader.ReadSingle();
                            v.Z = BReader.ReadSingle();
                        }

                        break;

                    case EMarker.SCLTRACKTAG:
                        fl = BReader.ReadUInt16();
                        dw1 = BReader.ReadUInt32();
                        dw2 = BReader.ReadUInt32();
                        nk = BReader.ReadUInt32();
                        //
                        for (int j = 1; j <= nk; j++)
                        {
                            uint i = BReader.ReadUInt32();
                            int flg = BReader.ReadUInt16();
                            mtx.Row0.X = BReader.ReadSingle();
                            mtx.Row1.Y = BReader.ReadSingle();
                            mtx.Row2.Z = BReader.ReadSingle();
                        }

                        break;

                    case EMarker.POSTRACKTAG:
                        fl = BReader.ReadUInt16();
                        dw1 = BReader.ReadUInt32();
                        dw2 = BReader.ReadUInt32();
                        nk = BReader.ReadUInt32();
                        //
                        for (int j = 1; j <= nk; j++)
                        {
                            uint i = BReader.ReadUInt32();
                            int flg = BReader.ReadUInt16();
                            mtx.Row3.X = BReader.ReadSingle() / 1000;
                            mtx.Row3.Y = BReader.ReadSingle() / 1000;
                            mtx.Row3.Z = BReader.ReadSingle() / 1000;
                        }

                        break;

                    default:
                        throw new Exception("Unplanned chunk " + chunk.Marker.ToString());
                }
                FReader.Position = chunk.Finish;
            }
            TFile file = TManager.UsedFiles.Values.First(o => o.Element.Fi.FullName.ToUpper(CultureInfo.InvariantCulture) == (Fi.FullName + "\\" + NodeName + ".InternalSolid").ToUpper(CultureInfo.InvariantCulture));
            TLink link = new TLink
            {
                NodeName = NodeName,
                Matrix = mtx
            };
            link.Child = file.Element;
            link.ParentLink = this.OwnerLink;
        }

        #endregion Private Methods

        #region Private Structs

        private struct TChunk
        {
            #region Public Fields

            public EMarker Marker;
            public long Length;
            public long Start;
            public long Finish;

            #endregion Public Fields
        }

        #endregion Private Structs
    }
}