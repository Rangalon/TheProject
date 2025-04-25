
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CiliaElements.FormatBLEND
{
    public class TBlendElement : TSolidElement
    {
        #region Private Fields

        private string[] names = { };

        private Mtx4 StartMatrix = Mtx4.CreateScale(1);
        private BlendStructure[] types = { };

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            BinaryReader rdr = new BinaryReader(Fi.OpenRead());// new StreamReader(Fi.FullName);
            string s = "";
            for (int i = 0; i < 12; i++) s += rdr.ReadChar();

            List<BlendFileBlock> hds = new List<BlendFileBlock>();

            while (rdr.BaseStream.Position < rdr.BaseStream.Length)
            {
                BlendFileBlock b = new BlendFileBlock
                {
                    header = ""
                };
                b.header += rdr.ReadChar();
                b.header += rdr.ReadChar();
                b.header += rdr.ReadChar();
                b.header += rdr.ReadChar();
                b.Length = rdr.ReadInt32();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                rdr.ReadByte();
                b.SDNAi = rdr.ReadInt32();
                b.SDNAc = rdr.ReadInt32();
                b.Start = rdr.BaseStream.Position;
                hds.Add(b);
                rdr.BaseStream.Position += b.Length;
            }

            ParseDNA(rdr, hds.First(o => o.header == "DNA1"));

            foreach (BlendFileBlock b in hds.Where(o => o.header != "DNA1"))
                ParseBloc(rdr, b);

            TFGroup fgroup = SolidElementConstruction.AddFGroup();
            TCloud ps = SolidElementConstruction.AddCloud();
            TCloud ns = SolidElementConstruction.AddCloud();
            TTexture t = SolidElementConstruction.AddTexture(1, 0.9F, 0.9F, 0.9F);
            fgroup.GroupParameters.Texture = t;
            fgroup.ShapeGroupParameters.Positions = ps;
            fgroup.ShapeGroupParameters.Normals = ns;
            SolidElementConstruction.StartGroup.FGroups.Push(fgroup);

            rdr.Close();
            rdr.Dispose();

            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;

            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private void ParseBloc(BinaryReader rdr, BlendFileBlock b)
        {
            rdr.BaseStream.Position = b.Start;
            Array.Resize(ref b.Datas, b.SDNAc);
            BlendStructure bs = types[b.SDNAi];
            string l = "";
            ulong j = rdr.ReadUInt32();
            ulong k = rdr.ReadUInt32();
            for (int i = 0; i < b.Length - 8; i++) l += rdr.ReadChar();
        }

        private void ParseDNA(BinaryReader rdr, BlendFileBlock b)
        {
            rdr.BaseStream.Position = b.Start;
            int wcount;
            int i;
            string w;
            rdr.ReadBytes(4);
            // -----------------------------------
            rdr.ReadBytes(4);
            wcount = rdr.ReadInt32();
            Array.Resize(ref names, wcount);
            i = 0; w = "";
            while (i < wcount)
            {
                char c = rdr.ReadChar();
                if (c == (char)0)
                {
                    names[i] = w; i++; w = "";
                }
                else
                    w += c;
            }
            // -----------------------------------
            rdr.ReadByte();
            // -----------------------------------
            rdr.ReadBytes(4);
            wcount = rdr.ReadInt32();
            Array.Resize(ref types, wcount);
            i = 0; w = "";
            while (i < wcount)
            {
                char c = rdr.ReadChar();
                if (c == (char)0)
                {
                    types[i] = new BlendStructure() { Name = w }; i++; w = "";
                }
                else
                    w += c;
            }
            // -----------------------------------
            rdr.ReadByte();
            // -----------------------------------
            rdr.ReadBytes(4);
            for (int j = 0; j < wcount; j++) types[j].Length = rdr.ReadInt16();
            rdr.ReadByte();
            // -----------------------------------
            rdr.ReadByte();
            // -----------------------------------
            rdr.ReadBytes(4);
            wcount = rdr.ReadInt32();
            for (int j = 0; j < wcount; j++)
            {
                BlendStructure bs = types[rdr.ReadInt16()];
                short fcount = rdr.ReadInt16();
                Array.Resize(ref bs.Fields, fcount);
                for (int k = 0; k < fcount; k++)
                {
                    BlendField bf = new BlendField
                    {
                        Structure = types[rdr.ReadInt16()],
                        Name = names[rdr.ReadInt16()]
                    };
                    bs.Fields[k] = bf;
                }
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class BlendField
        {
            #region Public Fields

            public string Name;
            public BlendStructure Structure;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return string.Format("{0} {1}", Name, Structure);
            }

            #endregion Public Methods
        }

        private class BlendFieldData
        {
            //public string Name;
            //public BlendStructureData Value;
        }

        private class BlendFileBlock
        {
            #region Public Fields

            public BlendStructureData[] Datas = { };
            public string header;
            public int Length;
            public int SDNAc;
            public int SDNAi;
            public long Start;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return string.Format("{0} {1} {2} {3}", header, Length, SDNAi, SDNAc);
            }

            #endregion Public Methods
        }

        private class BlendStructure
        {
            #region Public Fields

            public BlendField[] Fields = { };
            public short Length;
            public string Name;

            #endregion Public Fields

            #region Public Methods

            public override string ToString()
            {
                return string.Format("{0} {1} {2}", Name, Fields.Length, Length);
            }

            #endregion Public Methods
        }

        private class BlendStructureData
        {
            //public BlendFieldData[] Fields;
            //public BlendStructure Structure;
        }

        #endregion Private Classes
    }
}