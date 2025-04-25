using System;

namespace OpenTK.Platform.Windows
{
    internal static class Constants
    {
        internal const int DISP_CHANGE_SUCCESSFUL = 0;
        internal const int DM_BITSPERPEL = 0x00040000;
        internal const int DM_DISPLAYFREQUENCY = 0x00400000;
        internal const int DM_LOGPIXELS = 0x00020000;
        internal const int DM_PELSHEIGHT = 0x00100000;
        internal const int DM_PELSWIDTH = 0x00080000;
        internal const int ERROR_POINT_NOT_FOUND = 1171;
        internal const int GMMP_USE_DISPLAY_POINTS = 1;
        internal static readonly IntPtr MESSAGE_ONLY = new IntPtr(-3);
    }
}