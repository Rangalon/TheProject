using CiliaElements.Elements;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZLIB;

namespace CiliaElements.FormatJT
{
    public class TJTAssembly : TAssemblyElement
    {
        #region Private Fields

        private List<TElement> Elements = new List<TElement>();

        private BinaryReader rdr;

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            //StreamWriter wtr = new StreamWriter("c:\\temp\\test.txt");
            //rdr = new BinaryReader(Fi.OpenRead());
            //int ii = 0;
            //while (rdr.BaseStream.Position < rdr.BaseStream.Length)
            //{
            //    wtr.Write(string.Format("{0:X2}", rdr.ReadByte()));
            //    ii++;
            //    if (ii == 8 || ii == 16 || ii == 24) wtr.Write(" ");
            //    if (ii == 32) { wtr.Write("\n"); ii = 0; }
            //}
            //rdr.Close();
            //rdr.Dispose();
            //wtr.Close();
            //wtr.Dispose();

            rdr = new BinaryReader(Fi.OpenRead());

            TFileHeader FileHeader = ReadFileHeader();
            //
            rdr.BaseStream.Position = FileHeader.TOCOffset;
            //
            int EntryCount = rdr.ReadInt32();
            TSegment[] segments = new TSegment[EntryCount];
            for (int i = 0; i < EntryCount; i++)
            {
                segments[i] = ReadTOC();
            }

            for (int i = 0; i < EntryCount; i++)
            {
                rdr.BaseStream.Position = segments[i].Offset;
                // Segment Header Check;
                if (!segments[i].ID.Equals(ReadGUID()))
                {
                    throw new Exception("not expected!");
                }

                if (segments[i].Kind != rdr.ReadUInt32())
                {
                    throw new Exception("not expected!");
                }

                if (segments[i].Length != rdr.ReadInt32())
                {
                    throw new Exception("not expected!");
                }
                //
                if (segments[i].ZLib)
                {
                    segments[i].CompressionFlag = rdr.ReadInt32();
                    segments[i].CompressedDataLength = rdr.ReadInt32();
                    segments[i].CompressionAlgorithm = rdr.ReadByte();
                    switch (segments[i].Kind)
                    {
                        case 1: ParseAsLSG(segments[i]); break;
                        case 4: ParseAsMetaData(segments[i]); break;
                    }
                }
                else
                {
                    // Logical Element Header
                    segments[i].ElementLength = rdr.ReadInt32(); // Ok
                    // Element Header
                    segments[i].ElementID = ReadGUID(); // Ok

                    if (segments[i].ElementID == TGUID.TriStripSetShapeLODElement)
                    {
                        // TriStripSetShapeLODElement p105

                        // Logical Element Header p32
                        TLogicalElementHeader leh1 = ReadLogicalElementHeader();

                        // Vertex Shape LOD Data p94
                        {
                            byte vs0 = rdr.ReadByte();
                            ulong VertexBindings = rdr.ReadUInt64();
                            short vs1 = rdr.ReadInt16();
                            uint oid = rdr.ReadUInt32();
                            short vs2 = rdr.ReadInt16();

                            List<int[]> FaceDegrees = new List<int[]>();
                            for (int j = 0; j < 8; j++)
                            {
                                FaceDegrees.Add(Int32CDP.ReadInt32CDP(rdr));
                            }

                            int[] VertexValences = Int32CDP.ReadInt32CDP(rdr);
                            int[] VertexGroups = Int32CDP.ReadInt32CDP(rdr);
                            int[] VertexFlags = Int32CDP.ReadInt32CDP(rdr);

                            List<int[]> FaceAttributeMasks = new List<int[]>();
                            for (int j = 0; j < 8; j++)
                            {
                                FaceAttributeMasks.Add(Int32CDP.ReadInt32CDP(rdr));
                            }

                            int[] FaceAttributeMask8_30 = Int32CDP.ReadInt32CDP(rdr);
                            int[] FaceAttributeMask8_4 = Int32CDP.ReadInt32CDP(rdr);
                            uint HighDegreeFaceAttributeMasks = rdr.ReadUInt32();

                            int[] SplitFaceSyms = Int32CDP.ReadInt32CDP(rdr);
                            int[] SplitFacePositions = Int32CDP.ReadInt32CDP(rdr);
                            uint CompositeHash = rdr.ReadUInt32();

                            VertexBindings = rdr.ReadUInt64();

                            byte BitsPerVertex = rdr.ReadByte();
                            byte NormalBitsFactor = rdr.ReadByte();
                            byte BitsPerTextureCoord = rdr.ReadByte();
                            byte BitsPerColor = rdr.ReadByte();

                            int NumberofTopologicalVertices = rdr.ReadInt32();
                            int NumberofVertexAttributes = rdr.ReadInt32();
                            int UniqueVertexCount = rdr.ReadInt32();
                            byte NumberComponents = rdr.ReadByte();

                            List<SUniformQuantizerData> UniformQuantizerDatas = new List<SUniformQuantizerData>();

                            for (int ii = 0; ii < NumberComponents; ii++)
                            {
                                SUniformQuantizerData u = new SUniformQuantizerData
                                {
                                    Min = rdr.ReadSingle(),
                                    Max = rdr.ReadSingle(),
                                    bits = rdr.ReadByte()
                                };
                                UniformQuantizerDatas.Add(u);
                            }

                            List<int[]> XArrays = new List<int[]>();
                            for (int ii = 0; ii < 3; ii++)
                            {
                                XArrays.Add(Int32CDP.ReadInt32CDP(rdr));
                            }

                            List<int[]> YArrays = new List<int[]>();
                            for (int ii = 0; ii < 3; ii++)
                            {
                                YArrays.Add(Int32CDP.ReadInt32CDP(rdr));
                            }

                            List<int[]> ZArrays = new List<int[]>();
                            for (int ii = 0; ii < 3; ii++)
                            {
                                ZArrays.Add(Int32CDP.ReadInt32CDP(rdr));
                            }

                            int VertexCoordinateHash = rdr.ReadInt32();

                            int NormalsCount = rdr.ReadInt32();
                            NumberComponents = rdr.ReadByte();
                            byte QuantizationBits = rdr.ReadByte();

                            if (QuantizationBits > 0)
                            {
                                int[] SextantCodes = Int32CDP.ReadInt32CDP(rdr);
                                int[] OctantCodes = Int32CDP.ReadInt32CDP(rdr);
                                int[] ThetaCodes = Int32CDP.ReadInt32CDP(rdr);
                                int[] PsiCodes = Int32CDP.ReadInt32CDP(rdr);
                            }

                            int VertexNormalHash = rdr.ReadInt32();
                            VertexNormalHash += 0;
                        }
                    }
                    else
                    {
                    }
                }
            }
            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private void AddGeometricTransformAttributeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TGeometricTransformAttributeElement g = new TGeometricTransformAttributeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                BaseAttributeData = ReadBaseAttributeData(ds),
                VersionNumber = ds.ReadInt16(),
                ElementValues = ReadMtx4(ds)
            };
            Elements.Add(g);
        }

        private void AddGroupNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds, TPartNodeElement PartNodeElement)
        {
            TGroupNodeElement g = new TGroupNodeElement
            {
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                GroupNodeData = ReadGroupNodeData(ds)
            };
            PartNodeElement.GroupNodeElement = g;
            //List<int> lst = new List<int>();
            //for (int i = 0; i < 100; i++) lst.Add(ds.ReadByte());
        }

        private void AddInstanceNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TInstanceNodeElement InstanceNE = new TInstanceNodeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                BaseNodeData = ReadBaseNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                ChildNodeObjectId = ds.ReadInt32()
            };
            Elements.Add(InstanceNE);
        }

        private void AddLateLoadedPropertyAtomElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TLateLoadedPropertyAtomElement l = new TLateLoadedPropertyAtomElement
            {
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                BasePropertyAtomData = ReadBasePropertyAtom(ds),
                VersionNumber = ds.ReadInt16(),
                SegmentID = ReadGUID(ds),
                SegmentType = ds.ReadInt32(),
                PayloadObjectID = ds.ReadInt32(),
                Reserved = ds.ReadInt32()
            };
            Elements.Add(l);
            Elements.Add(null);
        }

        private void AddLinks(TMetaDataNodeElement MetaDataNodeElement, TLink iLink)
        {
            foreach (int i in MetaDataNodeElement.NodeData.GroupNodeData.ChildNodeObjectsIDs)
            {
                TLink link = new TLink
                {
                    Child = new TAssemblyElement(),
                    ParentLink = iLink,
                    //link.Matrix = instance.Matrix;
                    NodeName = ((IReferenceableElement)Elements[i]).NodeName
                };
                link.PartNumber = link.NodeName;
                //foreach (int j in Elements[i])
                //{
                //    TElement e = Elements[j];
                //}

                if (Elements[i] is TInstanceNodeElement n)
                {
                    foreach (int j in n.BaseNodeData.AttributeObjectsIDs)
                    {
                        TElement ee = Elements[j];
                        ee = null;
                    }
                }

                if (Elements[i].ElementAttributes % 16 > 7)
                {
                    //TGeometricTransformAttributeElement e = Elements.OfType<TGeometricTransformAttributeElement>().First();
                    //link.Matrix = e.ElementValues;
                    //Elements.Remove(e);
                }
                //link.PartNumber = iReferences.Values.First(o => o.Id == instance.ChildId).PartNumber;
                if (Elements[i + 1] is TMetaDataNodeElement m)
                {
                    AddLinks(m, link);
                }
            }
        }

        private void AddMaterialAttributeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TMaterialAttributeElement m = new TMaterialAttributeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                BaseAttributeData = ReadBaseAttributeData(ds),
                VersionNumber = ds.ReadInt16(),
                DataFlags = ds.ReadUInt16()
            };
            m.AmbientColor.X = ds.ReadSingle();
            m.AmbientColor.Y = ds.ReadSingle();
            m.AmbientColor.Z = ds.ReadSingle();
            m.AmbientColor.W = ds.ReadSingle();
            m.DiffuseColor.X = ds.ReadSingle();
            m.DiffuseColor.Y = ds.ReadSingle();
            m.DiffuseColor.Z = ds.ReadSingle();
            m.DiffuseColor.W = ds.ReadSingle();
            m.SpecularColor.X = ds.ReadSingle();
            m.SpecularColor.Y = ds.ReadSingle();
            m.SpecularColor.Z = ds.ReadSingle();
            m.SpecularColor.W = ds.ReadSingle();
            m.EmissionColor.X = ds.ReadSingle();
            m.EmissionColor.Y = ds.ReadSingle();
            m.EmissionColor.Z = ds.ReadSingle();
            m.EmissionColor.W = ds.ReadSingle();
            m.Shininess = ds.ReadSingle();
            if (m.VersionNumber == 2)
            {
                m.Reflectivity = ds.ReadSingle();
            }

            Elements.Add(m);
        }

        private void AddMetaDataNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TMetaDataNodeElement MetaDataNE = new TMetaDataNodeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                NodeData = ReadMetaDataNodeData(ds)
            };
            Elements.Add(MetaDataNE);
        }

        private void AddPartitionNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TPartitionNodeElement PartitionNE = new TPartitionNodeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                GroupNodeData = ReadGroupNodeData(ds),
                PartitionFlags = ds.ReadInt32(),
                FileName = ReadMbString(ds),
                BBox = ReadBBoxf(ds),
                Area = ds.ReadSingle(),
                VertexCountRange = ReadRange(ds),
                NodeCountRange = ReadRange(ds),
                PolygonCountRange = ReadRange(ds)
            };
            if (PartitionNE.PartitionFlags == 1)
            {
                PartitionNE.BBox = ReadBBoxf(ds);
            }

            Elements.Add(PartitionNE);
        }

        private void AddPartNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TPartNodeElement PartNE = new TPartNodeElement
            {
                ElementAttributes = ElementAttributes,
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                NodeData = ReadMetaDataNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                ReservedField = ds.ReadInt32()
            };
            Elements.Add(PartNE);
        }

        private void AddRangeLODNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds, TPartNodeElement PartNodeElement)
        {
            TRangeLODNodeElement r = new TRangeLODNodeElement
            {
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                LODNodeData = ReadLODNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                RangeLimits = ReadVecF32(ds)
            };
            r.Center.X = ds.ReadSingle();
            r.Center.Y = ds.ReadSingle();
            r.Center.Z = ds.ReadSingle();
            //r.Center.W = ds.ReadSingle();
            PartNodeElement.RangeLODNodeElement = r;
            //List<int> lst = new List<int>();
            //for (int i = 0; i < 100; i++) lst.Add(ds.ReadByte());
        }

        private void AddStringPropertyAtomElement(int ElementAttributes, ZLIB.ZLIBStream ds)
        {
            TStringPropertyAtomElement s = new TStringPropertyAtomElement
            {
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                BasePropertyAtomData = ReadBasePropertyAtom(ds),
                VersionNumber = ds.ReadInt16(),
                Value = ReadMbString(ds)
            };
            Elements.Add(s);
        }

        private void AddTriStripSetShapeNodeElement(int ElementAttributes, ZLIB.ZLIBStream ds, TPartNodeElement PartNodeElement)
        {
            TTriStripSetShapeNodeElement t = new TTriStripSetShapeNodeElement
            {
                LogicalElementHeader = ReadLogicalElementHeader(ds),
                VertexShapeData = ReadVertexShapeData(ds)
            };
            PartNodeElement.TriStripSetShapeNodeElement = t;
            //List<int> lst = new List<int>();
            //for (int i = 0; i < 100; i++) lst.Add(ds.ReadByte());
        }

        private bool CanRead()
        {
            return rdr.BaseStream.Position < rdr.BaseStream.Length;
        }

        private ZLIB.ZLIBStream GetZLibStream(TSegment iSegment)
        {
            MemoryStream ms = new MemoryStream(rdr.ReadBytes(iSegment.CompressedDataLength))
            {
                Position = 0
            };
            ZLIB.ZLIBStream ds = new ZLIB.ZLIBStream(ms, CompressionMode.Decompress);
            //ms.Dispose();
            return ds;
        }

        private void ParseAsLSG(TSegment iSegment)
        {
            ZLIB.ZLIBStream ds = GetZLibStream(iSegment);
            //
            while (ds.CanRead)
            {
                int ElementAttributes = ds.ReadInt32();
                TGUID g = ReadGUID(ds);
                if (g == TGUID.InstanceNodeElement)
                {
                    AddInstanceNodeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.PartNodeElement)
                {
                    AddPartNodeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.MetaDataNodeElement)
                {
                    AddMetaDataNodeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.PartitionNodeElement)
                {
                    AddPartitionNodeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.TriStripSetShapeNodeElement)
                {
                    AddTriStripSetShapeNodeElement(ElementAttributes, ds, (TPartNodeElement)Elements.Last(o => o is TPartNodeElement));
                }
                else if (g == TGUID.GroupNodeElement)
                {
                    AddGroupNodeElement(ElementAttributes, ds, (TPartNodeElement)Elements.Last(o => o is TPartNodeElement));
                }
                else if (g == TGUID.RangeLODNodeElement)
                {
                    AddRangeLODNodeElement(ElementAttributes, ds, (TPartNodeElement)Elements.Last(o => o is TPartNodeElement));
                }
                else if (g == TGUID.GeometricTransformAttributeElement)
                {
                    AddGeometricTransformAttributeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.MaterialAttributeElement)
                {
                    AddMaterialAttributeElement(ElementAttributes, ds);
                }
                else if (g == TGUID.EndOfElements)
                {
                    break;
                }
                else
                {
                    throw new Exception("not expected");
                }
            }
            //
            foreach (TPartNodeElement p in Elements.OfType<TPartNodeElement>().ToArray())
            {
                Elements.Add(p.TriStripSetShapeNodeElement);
                Elements.Add(p.GroupNodeElement);
                Elements.Add(p.RangeLODNodeElement);
            }
            //
            while (ds.CanRead)
            {
                int ElementAttributes = ds.ReadInt32();
                TGUID g = ReadGUID(ds);
                if (g == TGUID.EndOfElements)
                {
                    break;
                }
                else if (g == TGUID.StringPropertyAtomElement)
                {
                    AddStringPropertyAtomElement(ElementAttributes, ds);
                }
                else if (g == TGUID.LateLoadedPropertyAtomElement)
                {
                    AddLateLoadedPropertyAtomElement(ElementAttributes, ds);
                }
                else
                {
                    throw new Exception("not expected");
                }
            }
            //
            short VersionNumber = ds.ReadInt16();
            int ElementPropertyTableCount = ds.ReadInt32();
            //
            for (int i = 0; i < ElementPropertyTableCount; i++)
            {
                TElementPropertyTable t = ReadElementPropertyTable(ds);
                TElement e = Elements[t.ElementObjectID];
                foreach (int k in t.ElementProperties.Keys.Where(o => t.ElementProperties[o] < Elements.Count))
                {
                    TElement p = Elements[t.ElementProperties[k]];
                    switch (k)
                    {
                        case 17:
                        case 21:
                        case 63:
                        case 184:
                            break;

                        case 19:
                        case 23:
                        case 65:
                        case 186:
                            ((IReferenceableElement)e).NodeName = (p as TStringPropertyAtomElement).Value;
                            break;

                        case 20:
                        case 24:
                        case 66:
                        case 187:
                            ((IDimensionableElement)e).Unit = (p as TStringPropertyAtomElement).Value;
                            break;

                        case 22:
                        case 26:
                        case 68:
                        case 189:
                            break;

                        case 190: break;
                        case 191: break;
                        case 192: break;
                        default: throw new Exception("not expected");
                    }
                }
            }
            //
            AddLinks((TMetaDataNodeElement)Elements.First(o => o is TMetaDataNodeElement), this.OwnerLink);
            //
        }

        private void ParseAsMetaData(TSegment iSegment)
        {
            //ZLIB.ZLIBStream ds = GetZLibStream(iSegment);
            ////
            //TMetaDataNodeElement MetaDataNE = new TMetaDataNodeElement();
            //MetaDataNE.LogicalElementHeader = ReadLogicalElementHeaderFull(ds);
            //MetaDataNE.NodeData = ReadMetaDataNodeData(ds);
        }

        private TBaseAttributeData ReadBaseAttributeData(ZLIB.ZLIBStream ds)
        {
            TBaseAttributeData b = new TBaseAttributeData
            {
                VersionNumber = ds.ReadInt16(),
                StateFlags = (byte)ds.ReadByte(),
                FieldInhibitFlags = ds.ReadUInt32()
            };
            return b;
        }

        private TBaseNodeData ReadBaseNodeData(ZLIB.ZLIBStream ds)
        {
            TBaseNodeData b = new TBaseNodeData
            {
                VersionNumber = ds.ReadInt16(),
                NodeFlags = ds.ReadInt32(),
                AttributeCount = ds.ReadInt32()
            };
            b.AttributeObjectsIDs = new int[b.AttributeCount];
            for (int i = 0; i < b.AttributeCount; i++)
            {
                b.AttributeObjectsIDs[i] = ds.ReadInt32();
            }

            return b;
        }

        private TBasePropertyAtomData ReadBasePropertyAtom(ZLIB.ZLIBStream ds)
        {
            TBasePropertyAtomData b = new TBasePropertyAtomData
            {
                VersionNumber = ds.ReadInt16(),
                StateFlags = ds.ReadUInt32()
            };
            return b;
        }

        private TBaseShapeData ReadBaseShapeData(ZLIB.ZLIBStream ds)
        {
            TBaseShapeData b = new TBaseShapeData
            {
                BaseNodeData = ReadBaseNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                ReservedBBox = ReadBBoxf(ds),
                UntransformedBBox = ReadBBoxf(ds),
                Area = ds.ReadSingle(),
                VertexCountRange = ReadRange(ds),
                NodeCountRange = ReadRange(ds),
                PolygonCountRange = ReadRange(ds),
                Size = ds.ReadInt32(),
                CompressionLevel = ds.ReadSingle()
            };
            return b;
        }

        private SBoundingBox3 ReadBBoxf(ZLIB.ZLIBStream ds)
        {
            SBoundingBox3 b = SBoundingBox3.Default;
            b.MinPosition.X = ds.ReadSingle();
            b.MinPosition.Y = ds.ReadSingle();
            b.MinPosition.Z = ds.ReadSingle();
            b.MaxPosition.X = ds.ReadSingle();
            b.MaxPosition.Y = ds.ReadSingle();
            b.MaxPosition.Z = ds.ReadSingle();
            return b;
        }

        private TElementPropertyTable ReadElementPropertyTable(ZLIB.ZLIBStream ds)
        {
            TElementPropertyTable t = new TElementPropertyTable
            {
                ElementObjectID = ds.ReadInt32()
            };
            int KeyPropertyAtomObjectID = ds.ReadInt32();
            while (KeyPropertyAtomObjectID != 0)
            {
                t.ElementProperties.Add(KeyPropertyAtomObjectID, ds.ReadInt32());
                KeyPropertyAtomObjectID = ds.ReadInt32();
            }
            return t;
        }

        private TFileHeader ReadFileHeader()
        {
            TFileHeader f = new TFileHeader
            {
                Version = ReadString(80),
                ByteOrder = rdr.ReadByte(),
                ReservedField = rdr.ReadInt32(),
                TOCOffset = rdr.ReadInt32()
            };
            if (f.ReservedField == 0)
            {
                f.LSGSegmentID = ReadGUID();
            }
            else
            {
                f.ReservedID = ReadGUID();
            }

            return f;
        }

        private TGroupNodeData ReadGroupNodeData(ZLIB.ZLIBStream ds)
        {
            TGroupNodeData g = new TGroupNodeData
            {
                BaseNodeData = ReadBaseNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                ChildCount = ds.ReadInt32()
            };
            g.ChildNodeObjectsIDs = new int[g.ChildCount];
            for (int i = 0; i < g.ChildCount; i++)
            {
                g.ChildNodeObjectsIDs[i] = ds.ReadInt32();
            }

            return g;
        }

        private TGUID ReadGUID()
        {
            TGUID g = new TGUID
            {
                W1 = rdr.ReadUInt32(),
                W21 = rdr.ReadUInt16(),
                W22 = rdr.ReadUInt16(),
                W31 = rdr.ReadByte(),
                W32 = rdr.ReadByte(),
                W33 = rdr.ReadByte(),
                W34 = rdr.ReadByte(),
                W35 = rdr.ReadByte(),
                W36 = rdr.ReadByte(),
                W37 = rdr.ReadByte(),
                W38 = rdr.ReadByte()
            };
            return g;
        }

        private TGUID ReadGUID(ZLIB.ZLIBStream ds)
        {
            TGUID g = new TGUID
            {
                W1 = ds.ReadUInt32(),
                W21 = ds.ReadUInt16(),
                W22 = ds.ReadUInt16(),
                W31 = (byte)ds.ReadByte(),
                W32 = (byte)ds.ReadByte(),
                W33 = (byte)ds.ReadByte(),
                W34 = (byte)ds.ReadByte(),
                W35 = (byte)ds.ReadByte(),
                W36 = (byte)ds.ReadByte(),
                W37 = (byte)ds.ReadByte(),
                W38 = (byte)ds.ReadByte()
            };
            return g;
        }

        private TInt32ProbabilityContextsMk2 ReadInt32ProbabilityContextsMk2()
        {
            TInt32ProbabilityContextsMk2 I = new TInt32ProbabilityContextsMk2();
            //
            for (int i = 0; i < 16; i++)
            {
                I.ProbabilityContextTableEntrycount[i] = rdr.ReadUInt32();
            }

            for (int i = 0; i < 6; i++)
            {
                I.NumberSymbolBits[i] = rdr.ReadUInt32();
            }

            for (int i = 0; i < 6; i++)
            {
                I.NumberOccurrenceCountBits[i] = rdr.ReadUInt32();
            }

            for (int i = 0; i < 6; i++)
            {
                I.NumberValueBits[i] = rdr.ReadUInt32();
            }

            for (int i = 0; i < 32; i++)
            {
                I.MinValue[i] = rdr.ReadUInt32();
            }
            //
            return I;
        }

        private TLODNodeData ReadLODNodeData(ZLIB.ZLIBStream ds)
        {
            TLODNodeData l = new TLODNodeData
            {
                GroupNodeData = ReadGroupNodeData(ds),
                VersionNumber = ds.ReadInt16(),
                ReservedVector = ReadVecF32(ds),
                ReservedField = ds.ReadInt32()
            };
            return l;
        }

        private TLogicalElementHeader ReadLogicalElementHeader(ZLIB.ZLIBStream ds)
        {
            TLogicalElementHeader e = new TLogicalElementHeader
            {
                ElementLength = ds.ReadInt32(),
                Data = new byte[1]
            };
            e.Data[0] = (byte)ds.ReadByte();
            return e;
        }

        private TLogicalElementHeader ReadLogicalElementHeader()
        {
            TLogicalElementHeader e = new TLogicalElementHeader
            {
                ElementLength = rdr.ReadInt32(),
                Data = new byte[4]
            };
            e.Data = rdr.ReadBytes(4);
            //e.Data = new byte[e.ElementLength];
            //e.Data = rdr.ReadBytes(e.ElementLength);
            return e;
        }

        private TLogicalElementHeader ReadLogicalElementHeaderFull(ZLIB.ZLIBStream ds)
        {
            TLogicalElementHeader e = new TLogicalElementHeader();
            ds.ReadInt32();
            ds.ReadInt32();
            ds.ReadByte();
            e.ElementLength = ds.ReadInt32();
            e.Data = new byte[e.ElementLength];
            for (int i = 0; i < e.ElementLength; i++)
            {
                e.Data[i] = (byte)ds.ReadByte();
            }

            return e;
        }

        private string ReadMbString(ZLIB.ZLIBStream ds)
        {
            string s = "";
            int nb = ds.ReadInt32();
            for (int i = 0; i < nb; i++)
            {
                s += (char)(ds.ReadInt16());
            }
            return s;
        }

        private TMetaDataNodeData ReadMetaDataNodeData(ZLIB.ZLIBStream ds)
        {
            TMetaDataNodeData m = new TMetaDataNodeData
            {
                GroupNodeData = ReadGroupNodeData(ds),
                VersionNumber = ds.ReadInt16()
            };
            return m;
        }

        private Mtx4 ReadMtx4(ZLIB.ZLIBStream ds)
        {
            Mtx4 M = Mtx4.Identity;
            ushort StoredValuesMask = ds.ReadUInt16();
            if (StoredValuesMask > 32767) { M.Row0.X = ds.ReadDouble(); StoredValuesMask -= 32768; }
            if (StoredValuesMask > 16383) { M.Row0.Y = ds.ReadDouble(); StoredValuesMask -= 16384; }
            if (StoredValuesMask > 8191) { M.Row0.Z = ds.ReadDouble(); StoredValuesMask -= 8192; }
            if (StoredValuesMask > 4095) { M.Row0.W = ds.ReadDouble(); StoredValuesMask -= 4096; }
            if (StoredValuesMask > 2047) { M.Row1.X = ds.ReadDouble(); StoredValuesMask -= 2048; }
            if (StoredValuesMask > 1023) { M.Row1.Y = ds.ReadDouble(); StoredValuesMask -= 1024; }
            if (StoredValuesMask > 511) { M.Row1.Z = ds.ReadDouble(); StoredValuesMask -= 512; }
            if (StoredValuesMask > 255) { M.Row1.W = ds.ReadDouble(); StoredValuesMask -= 256; }
            if (StoredValuesMask > 127) { M.Row2.X = ds.ReadDouble(); StoredValuesMask -= 128; }
            if (StoredValuesMask > 63) { M.Row2.Y = ds.ReadDouble(); StoredValuesMask -= 64; }
            if (StoredValuesMask > 31) { M.Row2.Z = ds.ReadDouble(); StoredValuesMask -= 32; }
            if (StoredValuesMask > 15) { M.Row2.W = ds.ReadDouble(); StoredValuesMask -= 16; }
            if (StoredValuesMask > 7) { M.Row3.X = ds.ReadDouble() * 0.001; StoredValuesMask -= 8; }
            if (StoredValuesMask > 3) { M.Row3.Y = ds.ReadDouble() * 0.001; StoredValuesMask -= 4; }
            if (StoredValuesMask > 1) { M.Row3.Z = ds.ReadDouble() * 0.001; StoredValuesMask -= 2; }
            if (StoredValuesMask > 0) { M.Row3.W = ds.ReadDouble() * 0.001; StoredValuesMask -= 1; }
            return M;
        }

        private TQuantizationParameters ReadQuantizationParameters(ZLIB.ZLIBStream ds)
        {
            TQuantizationParameters q = new TQuantizationParameters
            {
                BitsPerVertex = (byte)ds.ReadByte(),
                NormalsBitsFactor = (byte)ds.ReadByte(),
                BitsPerTextureCoord = (byte)ds.ReadByte(),
                BitsPerColor = (byte)ds.ReadByte()
            };
            return q;
        }

        private TRange ReadRange(ZLIB.ZLIBStream ds)
        {
            TRange r = new TRange
            {
                MinCount = ds.ReadInt32(),
                MaxCount = ds.ReadInt32()
            };
            return r;
        }

        private string ReadString(int length)
        {
            string s = "";
            for (int i = 0; i < 80; i++)
            {
                s += (char)rdr.ReadByte();
            }

            return s;
        }

        private TSegment ReadTOC()
        {
            TSegment t = new TSegment
            {
                ID = ReadGUID(),
                Offset = rdr.ReadInt32(),
                Length = rdr.ReadInt32(),
                Attributes = rdr.ReadUInt32()
            };
            return t;
        }

        private TUniformQuantizerData ReadUniformQuantizerData()
        {
            TUniformQuantizerData u = new TUniformQuantizerData
            {
                Min = rdr.ReadSingle(),
                Max = rdr.ReadSingle(),
                NumberOfBits = rdr.ReadByte()
            };
            return u;
        }

        private float[] ReadVecF32(ZLIBStream ds)
        {
            int Nb = ds.ReadInt32();
            float[] tbl = new float[Nb];
            for (int i = 0; i < Nb; i++)
            {
                tbl[i] = ds.ReadSingle();
            }

            return tbl;
        }

        private uint[] ReadVecUInt32()
        {
            int Nb = rdr.ReadInt32();
            uint[] tbl = new uint[Nb];
            for (int i = 0; i < Nb; i++)
            {
                tbl[i] = rdr.ReadUInt32();
            }

            return tbl;
        }

        private TVertexShapeData ReadVertexShapeData(ZLIB.ZLIBStream ds)
        {
            TVertexShapeData v = new TVertexShapeData
            {
                BaseShapeData = ReadBaseShapeData(ds),
                VersionNumber = ds.ReadInt16(),
                VertexBinding = ds.ReadUInt64(),
                QuantizationParameters = ReadQuantizationParameters(ds)
            };
            if (v.VersionNumber != 1)
            {
                v.VertexBinding2 = ds.ReadUInt64();
            }

            return v;
        }

        #endregion Private Methods

        #region Public Classes

        public class SUniformQuantizerData
        {
            #region Public Fields

            public byte bits;
            public float Max;
            public float Min;

            #endregion Public Fields
        }

        #endregion Public Classes

        #region Private Classes

        private class TMeshCoderInputData
        {
            #region Public Methods

            public void Read(BinaryReader rdr)
            {
            }

            #endregion Public Methods
        }

        private class TUniformQuantizerData
        {
            #region Public Fields

            public float Max;
            public float Min;
            public byte NumberOfBits;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", Min, Max, NumberOfBits);
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }

    internal class TElementPropertyTable
    {
        #region Public Fields

        public int ElementObjectID;
        public Dictionary<int, int> ElementProperties = new Dictionary<int, int>();

        #endregion Public Fields
    }

    internal class TFileHeader
    {
        #region Public Fields

        public byte ByteOrder;
        public TGUID LSGSegmentID;
        public int ReservedField;
        public TGUID ReservedID;
        public int TOCOffset;
        public string Version;

        #endregion Public Fields
    }

    internal class TInt32ProbabilityContextsMk2
    {
        #region Public Fields

        public uint[] MinValue = new uint[32];
        public uint[] NumberOccurrenceCountBits = new uint[6];
        public uint[] NumberSymbolBits = new uint[6];
        public uint[] NumberValueBits = new uint[6];
        public uint[] ProbabilityContextTableEntrycount = new uint[16];

        #endregion Public Fields
    }

    internal class TLogicalElementHeader
    {
        #region Public Fields

        public byte[] Data;
        public int ElementLength;

        #endregion Public Fields
    }

    internal class TQuantizationParameters
    {
        #region Public Fields

        public byte BitsPerColor;
        public byte BitsPerTextureCoord;
        public byte BitsPerVertex;
        public byte NormalsBitsFactor;

        #endregion Public Fields
    }

    internal class TRange
    {
        #region Public Fields

        public int MaxCount;
        public int MinCount;

        #endregion Public Fields
    }

    internal class TSegment
    {
        #region Public Fields

        public int CompressedDataLength;
        public byte CompressionAlgorithm;
        public int CompressionFlag;
        public TGUID ElementID;
        public int ElementLength;
        public TGUID ID;
        public byte Kind;
        public int Length;

        //public byte ObjectBaseType;
        public int Offset;

        #endregion Public Fields

        #region Private Fields

        private uint attributes;

        #endregion Private Fields

        #region Public Properties

        public uint Attributes
        {
            get { return attributes; }
            set
            {
                attributes = value;
                Kind = (byte)(attributes >> 24);
            }
        }

        public bool ZLib
        {
            get
            {
                if (Kind < 6)
                {
                    return true;
                }

                if (Kind < 17)
                {
                    return false;
                }

                return true;
            }
        }

        #endregion Public Properties

        //public Int32 TextureCoord;
    }
}