using System;
using System.Globalization;

namespace CiliaElements.FormatJT
{
    internal class TGUID : IEquatable<TGUID>
    {
        #region Public Fields

        public static TGUID EndOfElements = new TGUID()
        {
            W1 = 0xffffffff,
            W21 = 0xffff,
            W22 = 0xffff,
            W31 = 0xff,
            W32 = 0xff,
            W33 = 0xff,
            W34 = 0xff,
            W35 = 0xff,
            W36 = 0xff,
            W37 = 0xff,
            W38 = 0xff
        };

        public static TGUID GeometricTransformAttributeElement = new TGUID()
        {
            W1 = 0x10dd1083,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static TGUID GroupNodeElement = new TGUID()
        {
            W1 = 0x10dd101b,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID InstanceNodeElement = new TGUID()
        {
            W1 = 0x10dd102a,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID LateLoadedPropertyAtomElement = new TGUID()
        {
            W1 = 0xe0b05be5,
            W21 = 0xfbbd,
            W22 = 0x11d1,
            W31 = 0xa3,
            W32 = 0xa7,
            W33 = 0x00,
            W34 = 0xaa,
            W35 = 0x00,
            W36 = 0xd1,
            W37 = 0x09,
            W38 = 0x54
        };

        public static TGUID MaterialAttributeElement = new TGUID()
        {
            W1 = 0x10dd1030,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };


        public static TGUID MetaDataNodeElement = new TGUID()
        {
            W1 = 0xce357245,
            W21 = 0x38fb,
            W22 = 0x11d1,
            W31 = 0xa5,
            W32 = 0x06,
            W33 = 0x00,
            W34 = 0x60,
            W35 = 0x97,
            W36 = 0xbd,
            W37 = 0xc6,
            W38 = 0xe1
        };

        public static bool operator ==(TGUID other, TGUID g2)
        {
            if (other.W1 != g2.W1) return false;
            if (other.W21 != g2.W21) return false;
            if (other.W22 != g2.W22) return false;
            if (other.W31 != g2.W31) return false;
            if (other.W32 != g2.W32) return false;
            if (other.W33 != g2.W33) return false;
            if (other.W34 != g2.W34) return false;
            if (other.W35 != g2.W35) return false;
            if (other.W36 != g2.W36) return false;
            if (other.W37 != g2.W37) return false;
            if (other.W38 != g2.W38) return false;
            return true;
        }

        public static bool operator !=(TGUID other, TGUID g2)
        {
            if (other.W1 != g2.W1) return true;
            if (other.W21 != g2.W21) return true;
            if (other.W22 != g2.W22) return true;
            if (other.W31 != g2.W31) return true;
            if (other.W32 != g2.W32) return true;
            if (other.W33 != g2.W33) return true;
            if (other.W34 != g2.W34) return true;
            if (other.W35 != g2.W35) return true;
            if (other.W36 != g2.W36) return true;
            if (other.W37 != g2.W37) return true;
            if (other.W38 != g2.W38) return true;
            return false;
        }

        public static TGUID PartitionNodeElement = new TGUID()
        {
            W1 = 0x10dd103e,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID PartNodeElement = new TGUID()
        {
            W1 = 0xce357244,
            W21 = 0x38fb,
            W22 = 0x11d1,
            W31 = 0xa5,
            W32 = 0x06,
            W33 = 0x00,
            W34 = 0x60,
            W35 = 0x97,
            W36 = 0xbd,
            W37 = 0xc6,
            W38 = 0xe1
        };

        public static TGUID RangeLODNodeElement = new TGUID()
        {
            W1 = 0x10dd104c,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID StringPropertyAtomElement = new TGUID()
        {
            W1 = 0x10dd106e,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID TriStripSetShapeNodeElement = new TGUID()
        {
            W1 = 0x10dd1077,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public static TGUID TriStripSetShapeLODElement = new TGUID()
        {
            W1 = 0x10dd10ab,
            W21 = 0x2ac8,
            W22 = 0x11d1,
            W31 = 0x9b,
            W32 = 0x6b,
            W33 = 0x00,
            W34 = 0x80,
            W35 = 0xc7,
            W36 = 0xbb,
            W37 = 0x59,
            W38 = 0x97
        };

        public uint W1;
        public ushort W21;
        public ushort W22;
        public byte W31;
        public byte W32;
        public byte W33;
        public byte W34;
        public byte W35;
        public byte W36;
        public byte W37;
        public byte W38;

        #endregion Public Fields

        #region Public Methods

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,"{0:X8}-{1:X4}-{2:X4}-{3:X2}-{4:X2}-{5:X2}-{6:X2}-{7:X2}-{8:X2}-{9:X2}-{10:X2}", W1, W21, W22, W31, W32, W33, W34, W35, W36, W37, W38);
        }

        #endregion Public Methods

        public bool Equals(TGUID other)
        {
            if (W1 != other.W1) return false;
            if (W21 != other.W21) return false;
            if (W22 != other.W22) return false;
            if (W31 != other.W31) return false;
            if (W32 != other.W32) return false;
            if (W33 != other.W33) return false;
            if (W34 != other.W34) return false;
            if (W35 != other.W35) return false;
            if (W36 != other.W36) return false;
            if (W37 != other.W37) return false;
            if (W38 != other.W38) return false;
            return true;
        }
    }
}