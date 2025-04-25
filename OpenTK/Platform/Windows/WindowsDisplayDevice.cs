using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    /// \internal
    /// <summary>
    /// The DISPLAY_DEVICE structure receives information about the display device specified by the iDevNum parameter of the EnumDisplayDevices function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class WindowsDisplayDevice
    {
        internal WindowsDisplayDevice()
        {
            size = (short)Marshal.SizeOf(this);
        }

        private readonly int size;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string DeviceString;

        internal DisplayDeviceStateFlags StateFlags;    // DWORD

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string DeviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        internal string DeviceKey;
    }
}