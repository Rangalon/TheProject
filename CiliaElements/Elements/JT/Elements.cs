using OpenTK;
using System;
using System.Globalization;

namespace CiliaElements.FormatJT
{
    public interface IDimensionableElement
    {
        #region Public Properties

        string Unit { get; set; }

        #endregion Public Properties
    }

    public interface IReferenceableElement
    {
        #region Public Properties

        string NodeName { get; set; }
        string PartNumber { get; set; }

        #endregion Public Properties
    }

    internal abstract class TElement
    {
        #region Public Fields

        public int ElementAttributes;
        public TLogicalElementHeader LogicalElementHeader;

        #endregion Public Fields

        #region Public Methods

        public override string ToString()
        {
            return ElementAttributes.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Methods
    }

    internal class TGeometricTransformAttributeElement : TElement
    {
        #region Public Fields

        public TBaseAttributeData BaseAttributeData;
        public Mtx4 ElementValues;
        public short VersionNumber;

        #endregion Public Fields
    }

    internal class TGroupNodeElement : TElement
    {
        #region Public Fields

        public TGroupNodeData GroupNodeData;

        #endregion Public Fields
    }

    internal class TInstanceNodeElement : TElement, IReferenceableElement
    {
        #region Public Fields

        public TBaseNodeData BaseNodeData;
        public int ChildNodeObjectId;
        public short VersionNumber;

        #endregion Public Fields

        #region Public Properties

        public string NodeName { get; set; }
        public string PartNumber { get; set; }

        #endregion Public Properties
    }

    internal class TLateLoadedPropertyAtomElement : TElement
    {
        #region Public Fields

        public TBasePropertyAtomData BasePropertyAtomData;
        public int PayloadObjectID;
        public int Reserved;
        public TGUID SegmentID;
        public int SegmentType;
        public short VersionNumber;

        #endregion Public Fields
    }

    internal class TMaterialAttributeElement : TElement, IDimensionableElement
    {
        #region Public Fields

        public Vec4f AmbientColor;
        public TBaseAttributeData BaseAttributeData;
        public ushort DataFlags;
        public Vec4f DiffuseColor;
        public Vec4f EmissionColor;
        public float Reflectivity;
        public float Shininess;
        public Vec4f SpecularColor;
        public short VersionNumber;

        #endregion Public Fields

        #region Public Properties

        public string Unit { get; set; }

        #endregion Public Properties
    }

    internal class TMetaDataNodeElement : TElement, IDimensionableElement
    {
        #region Public Fields

        public TMetaDataNodeData NodeData;

        #endregion Public Fields

        #region Public Properties

        public string Unit { get; set; }

        #endregion Public Properties
    }

    internal class TPartitionNodeElement : TElement, IReferenceableElement
    {
        #region Public Fields

        public float Area;
        public SBoundingBox3 BBox;
        public string FileName;
        public TGroupNodeData GroupNodeData;
        public TRange NodeCountRange;
        public int PartitionFlags;
        public TRange PolygonCountRange;
        public TRange VertexCountRange;

        #endregion Public Fields

        #region Public Properties

        public string NodeName { get; set; }
        public string PartNumber { get; set; }

        #endregion Public Properties
    }

    internal class TPartNodeElement : TElement, IReferenceableElement, IDimensionableElement
    {
        #region Public Fields

        public TGroupNodeElement GroupNodeElement;
        public TMetaDataNodeData NodeData;
        public TRangeLODNodeElement RangeLODNodeElement;
        public int ReservedField;
        public TTriStripSetShapeNodeElement TriStripSetShapeNodeElement;
        public short VersionNumber;

        #endregion Public Fields

        #region Public Properties

        public string NodeName { get; set; }
        public string PartNumber { get; set; }
        public string Unit { get; set; }

        #endregion Public Properties
    }

    internal class TRangeLODNodeElement : TElement
    {
        #region Public Fields

        public Vec4 Center;
        public TLODNodeData LODNodeData;
        public float[] RangeLimits;
        public short VersionNumber;

        #endregion Public Fields
    }

    internal class TStringPropertyAtomElement : TElement
    {
        #region Public Fields

        public TBasePropertyAtomData BasePropertyAtomData;
        public string Value;
        public short VersionNumber;

        #endregion Public Fields
    }

    internal class TTriStripSetShapeNodeElement : TElement
    {
        #region Public Fields

        public TVertexShapeData VertexShapeData;

        #endregion Public Fields
    }
}