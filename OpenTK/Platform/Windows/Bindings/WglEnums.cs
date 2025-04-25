namespace OpenTK.Platform.Windows
{
#pragma warning disable 3019
#pragma warning disable 1591

    public enum ArbCreateContext
    {
        CoreProfileBit = 0x0001,
        CompatibilityProfileBit = 0x0002,
        DebugBit = 0x0001,
        ForwardCompatibleBit = 0x0002,
        MajorVersion = 0x2091,
        MinorVersion = 0x2092,
        LayerPlane = 0x2093,
        ContextFlags = 0x2094,
        ErrorInvalidVersion = 0x2095,
        ProfileMask = 0x9126
    }

    public enum WGL_ARB_pixel_format
    {
        ShareStencilArb = ((int)0x200d),
        AccumBitsArb = ((int)0x201d),
        NumberUnderlaysArb = ((int)0x2009),
        StereoArb = ((int)0x2012),
        MaxPbufferHeightArb = ((int)0x2030),
        TypeRgbaArb = ((int)0x202b),
        SupportGdiArb = ((int)0x200f),
        NeedSystemPaletteArb = ((int)0x2005),
        AlphaBitsArb = ((int)0x201b),
        ShareDepthArb = ((int)0x200c),
        SupportOpenglArb = ((int)0x2010),
        ColorBitsArb = ((int)0x2014),
        AccumRedBitsArb = ((int)0x201e),
        MaxPbufferWidthArb = ((int)0x202f),
        NumberOverlaysArb = ((int)0x2008),
        MaxPbufferPixelsArb = ((int)0x202e),
        NeedPaletteArb = ((int)0x2004),
        RedShiftArb = ((int)0x2016),
        AccelerationArb = ((int)0x2003),
        GreenBitsArb = ((int)0x2017),
        TransparentGreenValueArb = ((int)0x2038),
        PixelTypeArb = ((int)0x2013),
        AuxBuffersArb = ((int)0x2024),
        DrawToWindowArb = ((int)0x2001),
        RedBitsArb = ((int)0x2015),
        NumberPixelFormatsArb = ((int)0x2000),
        GenericAccelerationArb = ((int)0x2026),
        BlueBitsArb = ((int)0x2019),
        PbufferLargestArb = ((int)0x2033),
        AccumAlphaBitsArb = ((int)0x2021),
        TransparentArb = ((int)0x200a),
        FullAccelerationArb = ((int)0x2027),
        ShareAccumArb = ((int)0x200e),
        SwapExchangeArb = ((int)0x2028),
        SwapUndefinedArb = ((int)0x202a),
        TransparentAlphaValueArb = ((int)0x203a),
        PbufferHeightArb = ((int)0x2035),
        TransparentBlueValueArb = ((int)0x2039),
        SwapMethodArb = ((int)0x2007),
        StencilBitsArb = ((int)0x2023),
        DepthBitsArb = ((int)0x2022),
        GreenShiftArb = ((int)0x2018),
        TransparentRedValueArb = ((int)0x2037),
        DoubleBufferArb = ((int)0x2011),
        NoAccelerationArb = ((int)0x2025),
        TypeColorindexArb = ((int)0x202c),
        SwapLayerBuffersArb = ((int)0x2006),
        AccumBlueBitsArb = ((int)0x2020),
        DrawToPbufferArb = ((int)0x202d),
        AccumGreenBitsArb = ((int)0x201f),
        PbufferWidthArb = ((int)0x2034),
        TransparentIndexValueArb = ((int)0x203b),
        AlphaShiftArb = ((int)0x201c),
        DrawToBitmapArb = ((int)0x2002),
        BlueShiftArb = ((int)0x201a),
        SwapCopyArb = ((int)0x2029),
    }

    public enum WGL_ARB_multisample
    {
        SampleBuffersArb = ((int)0x2041),
        SamplesArb = ((int)0x2042),
    }
}