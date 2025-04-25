using System;

namespace CiliaElements.FormatJT
{
    internal class TBaseAttributeData : TData
    {
        #region Public Fields

        public uint FieldInhibitFlags;
        public byte StateFlags;

        #endregion Public Fields
    }

    internal class TBaseNodeData : TData
    {
        #region Public Fields

        public int AttributeCount;
        public int[] AttributeObjectsIDs;
        public int NodeFlags;

        #endregion Public Fields
    }

    internal class TBasePropertyAtomData : TData
    {
        #region Public Fields

        public uint StateFlags;

        #endregion Public Fields
    }

    internal class TBaseShapeData : TData
    {
        #region Public Fields

        public float Area;
        public TBaseNodeData BaseNodeData;
        public float CompressionLevel;
        public TRange NodeCountRange;
        public TRange PolygonCountRange;
        public SBoundingBox3 ReservedBBox;
        public int Size;
        public SBoundingBox3 UntransformedBBox;
        public TRange VertexCountRange;

        #endregion Public Fields
    }

    internal abstract class TData
    {
        #region Public Fields

        public short VersionNumber;

        #endregion Public Fields
    }

    internal class TGroupNodeData : TData
    {
        #region Public Fields

        public TBaseNodeData BaseNodeData;
        public int ChildCount;
        public int[] ChildNodeObjectsIDs;

        #endregion Public Fields
    }

    internal class TLODNodeData : TData
    {
        #region Public Fields

        public TGroupNodeData GroupNodeData;
        public int ReservedField;
        public float[] ReservedVector;

        #endregion Public Fields
    }

    internal class TMetaDataNodeData : TData
    {
        #region Public Fields

        public TGroupNodeData GroupNodeData;

        #endregion Public Fields
    }

    internal class TVertexShapeData : TData
    {
        #region Public Fields

        public TBaseShapeData BaseShapeData;
        public TQuantizationParameters QuantizationParameters;
        public ulong VertexBinding;
        public ulong VertexBinding2;

        #endregion Public Fields
    }
}