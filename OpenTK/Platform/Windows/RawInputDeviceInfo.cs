using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    /// \internal
    /// <summary>
    /// Defines the raw input data coming from any device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal class RawInputDeviceInfo
    {
        /// <summary>
        /// Size, in bytes, of the RawInputDeviceInfo structure.
        /// </summary>
        private readonly int Size = Marshal.SizeOf(typeof(RawInputDeviceInfo));

        /// <summary>
        /// Type of raw input data.
        /// </summary>
        internal RawInputDeviceType Type;

        internal DeviceStruct Device;

        [StructLayout(LayoutKind.Explicit)]
        internal struct DeviceStruct
        {
            [FieldOffset(0)]
            internal RawInputMouseDeviceInfo Mouse;

            [FieldOffset(0)]
            internal RawInputKeyboardDeviceInfo Keyboard;

            [FieldOffset(0)]
            internal RawInputHIDDeviceInfo HID;
        };
    }
}