using System;

namespace CiliaElements
{
    public enum ETextHAlignment
    {
        Left, Center, Right
    }

    public enum EKeybordModifiers
    {
        None = 0,
        Ctrl = 1,
        Shift = 2
    }

    [Flags]
    public enum ELinkState
    {
        None = 0,
        Normal = 1,
        Selected = 2,
        SelectedNode = 4,
        InError = 8,
        Validated = 16,
        Disabled = 32,
        OverFlown = 64
    }

    public enum EIGSEntity
    {
        Null = 0,
        Weird = 406,
        CircularArc = 100,
        CompositeCurve = 102,
        ConicArc = 104,
        CopiousData = 106,
        Plane = 108,
        Line = 110,
        ParametricSplineCurve = 112,
        ParametricSplineSurface = 114,
        Point = 116,
        RuledSurface = 118,
        SurfaceOfRevolution = 120,
        TabulatedCylinder = 122,
        TransformationMatrix = 124,
        RationalBSplineCurve = 126,
        RationalBSplineSurface = 128,
        OffsetCurve = 130,
        OffsetSurface = 140,
        Boundary = 141,
        CurveOnAParametricSurface = 142,
        BoundedSurface = 143,
        TrimmedSurface = 144,
        Sphere = 158,
        ManifoldSolidBRepObject = 186,
        PlaneSurface = 190,
        RightCircularCylindricalSurface = 192,
        RightCircularConicalSurface = 194,
        SphericalSurface = 196,
        ToroidalSurface = 198,
        SubfigureDefinition = 308,
        ColorDefinition = 314,
        SingularSubfigureInstance = 408,
        Edge = 504,
        Loop = 508,
        Face = 510,
        Shell = 514
    }

    public enum EConstraintType
    {
        D,
        A
    }

    public enum EElementState
    {
        Unknown,
        Compiled,
        Published,
        Pushed
    }

    public enum EKinematicManagerState
    {
        Disabled
    }

    public enum EMoveMode
    {
        Null,
        Translation,
        Rotation,
        SideTranslation,
        SideRotation,
        Tree
    }

    public enum EReferenceType
    {
        Null = 0,
        Point = 11,
        Line = 10,
        Plane = 7
    }

    public enum ERepresentation
    {
        Unspecified,
        SoB,
        C,
        CorSoB
    }

    public enum EType
    {
        Unspecified,
        Projection,
        Intersection,
        Isoparametric
    }
}