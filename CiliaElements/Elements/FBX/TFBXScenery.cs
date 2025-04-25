using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatFBX
{
    internal class TFBXScenery
    {
        public float Version { get; private set; }
        public TFBXScenery(FileInfo fi)
        {
            BinaryReader brdr = new BinaryReader(fi.OpenRead());
            //
            byte[] bts;
            bts = brdr.ReadBytes(21);
            string s = Encoding.ASCII.GetString(bts);
            bts = brdr.ReadBytes(2);
            UInt32 u = brdr.ReadUInt32();
            Version = (float)u * 0.001f;
            s = Encoding.ASCII.GetString(bts);
            List<TNodeRecord> nodes = new List<TNodeRecord>();
            for (int i = 0; brdr.BaseStream.Position < brdr.BaseStream.Length; i++)
            {
                TNodeRecord record = new TNodeRecord(brdr);
                nodes.Add(record);
            }
            //
            brdr.Close(); brdr.Dispose();
        }


        public class TNodeRecordProperty : INodes
        {
            public UInt16 M1;
            public UInt16 M2;
            public UInt16 M3;
            public UInt16 M4;
            public UInt16 M5;
            public UInt16 M6;
            public byte[] Markers;
            public string Name;
            public object Value;

            public List<TNodeRecordProperty> Properties { get; set; }

            public INodes Parent { get; set; }

            public override string ToString()
            {
                return Name;
            }

        }

        public interface INodes
        {
            List<TNodeRecordProperty> Properties { get; set; }
            INodes Parent { get; set; }
        }



        public class TNodeRecord : INodes
        {
            public UInt32 EndOffset;
            public UInt32 NumProperties;
            public UInt32 PropertyListLen;
            public byte NameLen;
            public string Name;


            public INodes Parent { get; set; }

            public List<TNodeRecordProperty> Properties { get; set; }

            public TNodeRecord(BinaryReader brdr)
            {
                EndOffset = brdr.ReadUInt32();
                NumProperties = brdr.ReadUInt32();
                PropertyListLen = brdr.ReadUInt32();
                NameLen = brdr.ReadByte();
                Name = Encoding.ASCII.GetString(brdr.ReadBytes(NameLen));
                byte dataType; //= brdr.ReadByte();
                char c;//= (char)dataType;
                string s = "";
                INodes parent = this;
                byte nodetype;
                while (brdr.BaseStream.Position < EndOffset)
                {
                    if (parent.Properties == null) parent.Properties = new List<TNodeRecordProperty>();
                    nodetype = brdr.ReadByte();
                    TNodeRecordProperty p = new TNodeRecordProperty() { Parent = parent };
                    parent.Properties.Add(p);
                    p.Markers = brdr.ReadBytes(12);
                    p.Name = Encoding.ASCII.GetString(brdr.ReadBytes(p.Markers[11]));
                    s = ""; for (int i = 0; i < 12; i++) s += p.Markers[i].ToString() + " ";
                    Console.WriteLine(p.Name + " " + s);
                    if (p.Markers[7] > 0)
                    {
                        switch (p.Markers[3])
                        {
                            case 1:
                                dataType = brdr.ReadByte();
                                Console.WriteLine((char)dataType);
                                c = (char)dataType;
                                p.Value = GetValue(brdr, dataType);

                                break;
                            case 2:
                                object[] values = new object[p.Markers[0]];
                                for (int i = 0; i < p.Markers[0]; i++)
                                {

                                }
                                break;
                        }
                    }
                    else if (nodetype == 0)
                    {
                        parent = parent.Parent;
                    }
                    else
                    {
                        parent = p;
                    }
                }
                //
                //for (int i = 0; i < NumProperties; i++)
                //{
                //    byte dataType = brdr.ReadByte();
                //}
                //
                brdr.BaseStream.Position = EndOffset - 0;

            }

            object GetValue(BinaryReader brdr, byte dataType)
            {
                switch (dataType)
                {
                    case 73: return brdr.ReadInt32();
                    case 83: return Encoding.ASCII.GetString(brdr.ReadBytes((int)brdr.ReadUInt32()));
                    default: return null;
                }
            }

            public override string ToString()
            {
                return string.Format("{0}: {1} Properties", Name, NumProperties);
            }

        }
    }
}
