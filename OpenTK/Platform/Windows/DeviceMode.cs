using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class DeviceMode
    {
        internal DeviceMode()
        {
            Size = (short)Marshal.SizeOf(this);
        }

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string DeviceName;

        internal short SpecVersion;
        internal short DriverVersion;
        private readonly short Size;
        internal short DriverExtra;
        internal int Fields;

        internal POINT Position;
        internal int DisplayOrientation;
        internal int DisplayFixedOutput;

        internal short Color;
        internal short Duplex;
        internal short YResolution;
        internal short TTOption;
        internal short Collate;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        internal string FormName;

        internal short LogPixels;
        internal int BitsPerPel;
        internal int PelsWidth;
        internal int PelsHeight;
        internal int DisplayFlags;
        internal int DisplayFrequency;
        internal int ICMMethod;
        internal int ICMIntent;
        internal int MediaType;
        internal int DitherType;
        internal int Reserved1;
        internal int Reserved2;
        internal int PanningWidth;
        internal int PanningHeight;
    }
}