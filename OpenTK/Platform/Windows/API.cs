#region --- License ---

/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * Contributions from Erik Ylvisaker
 * See license.txt for license info
 */

#endregion --- License ---

#region --- Using Directives ---

using System;

#if !MINIMAL

using System.Drawing;

#endif

using System.Runtime.InteropServices;
using System.Security;
using System.Globalization;

#endregion --- Using Directives ---

/* TODO: Update the description of TimeBeginPeriod and other native methods. Update Timer. */

#pragma warning disable 3019    // CLS-compliance checking
#pragma warning disable 0649    // struct members not explicitly initialized
#pragma warning disable 0169    // field / method is never used.
#pragma warning disable 0414    // field assigned but never used.

namespace OpenTK.Platform.Windows
{
    /// \internal
    /// <summary>
    /// For internal use by OpenTK only!
    /// Exposes useful native WINAPI methods and structures.
    /// </summary>
    internal static class API
    {
        internal static short PixelFormatDescriptorSize = (short)Marshal.SizeOf(typeof(PixelFormatDescriptor));
        internal static short PixelFormatDescriptorVersion = 1;
        internal static int RawInputSize = Marshal.SizeOf(typeof(RawInput));
        internal static int RawInputDeviceSize = Marshal.SizeOf(typeof(RawInputDevice));
        internal static int RawInputHeaderSize = Marshal.SizeOf(typeof(RawInputHeader));
        internal static int RawInputDeviceListSize = Marshal.SizeOf(typeof(RawInputDeviceList));
        internal static int RawInputDeviceInfoSize = Marshal.SizeOf(typeof(RawInputDeviceInfo));
        internal static int RawMouseSize = Marshal.SizeOf(typeof(RawMouse));
        internal static int WindowInfoSize = Marshal.SizeOf(typeof(WindowInfo));
    }

    //    #endregion

    //    #region ExtendedStyleStruct

    //    struct ExtendedStyleStruct
    //    {
    //        public ExtendedWindowStyle Old;
    //        public ExtendedWindowStyle New;
    //    }

    //    #endregion

    //    #region PixelFormatDescriptor

    /// \internal
    /// <summary>
    /// Describes a pixel format. It is used when interfacing with the WINAPI to create a new Context.
    /// Found in WinGDI.h
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PixelFormatDescriptor
    {
        internal short Size;
        internal short Version;
        internal PixelFormatDescriptorFlags Flags;
        internal PixelType PixelType;
        internal byte ColorBits;
        internal byte RedBits;
        internal byte RedShift;
        internal byte GreenBits;
        internal byte GreenShift;
        internal byte BlueBits;
        internal byte BlueShift;
        internal byte AlphaBits;
        internal byte AlphaShift;
        internal byte AccumBits;
        internal byte AccumRedBits;
        internal byte AccumGreenBits;
        internal byte AccumBlueBits;
        internal byte AccumAlphaBits;
        internal byte DepthBits;
        internal byte StencilBits;
        internal byte AuxBuffers;
        internal byte LayerType;
        private readonly byte Reserved;
        internal int LayerMask;
        internal int VisibleMask;
        internal int DamageMask;
    }

    //    #endregion

    //    #region internal class LayerPlaneDescriptor

    //    /// \internal
    //    /// <summary>
    //    /// Describes the pixel format of a drawing surface.
    //    /// </summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct LayerPlaneDescriptor
    //    {
    //        internal static readonly Int16 Size = (Int16)Marshal.SizeOf(typeof(LayerPlaneDescriptor));
    //        internal Int16 Version;
    //        internal Int32 Flags;
    //        internal Byte PixelType;
    //        internal Byte ColorBits;
    //        internal Byte RedBits;
    //        internal Byte RedShift;
    //        internal Byte GreenBits;
    //        internal Byte GreenShift;
    //        internal Byte BlueBits;
    //        internal Byte BlueShift;
    //        internal Byte AlphaBits;
    //        internal Byte AlphaShift;
    //        internal Byte AccumBits;
    //        internal Byte AccumRedBits;
    //        internal Byte AccumGreenBits;
    //        internal Byte AccumBlueBits;
    //        internal Byte AccumAlphaBits;
    //        internal Byte DepthBits;
    //        internal Byte StencilBits;
    //        internal Byte AuxBuffers;
    //        internal Byte LayerPlane;
    //        Byte Reserved;
    //        internal Int32 crTransparent;
    //    }

    //    #endregion

    //    #region PointFloat

    //    /// \internal
    //    /// <summary>
    //    /// The <b>PointFloat</b> structure contains the x and y coordinates of a point.
    //    /// </summary>
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct PointFloat
    //    {
    //        /// <summary>
    //        /// Specifies the horizontal (x) coordinate of a point.
    //        /// </summary>
    //        internal Single X;
    //        /// <summary>
    //        /// Specifies the vertical (y) coordinate of a point.
    //        /// </summary>
    //        internal Single Y;
    //    };

    //    #endregion

    /*
    typedef struct _devicemode {
      BCHAR  dmDeviceName[CCHDEVICENAME];
      WORD   dmSpecVersion;
      WORD   dmDriverVersion;
      WORD   dmSize;
      WORD   dmDriverExtra;
      DWORD  dmFields;
      union {
        struct {
          short dmOrientation;
          short dmPaperSize;
          short dmPaperLength;
          short dmPaperWidth;
          short dmScale;
          short dmCopies;
          short dmDefaultSource;
          short dmPrintQuality;
        };
        POINTL dmPosition;
        DWORD  dmDisplayOrientation;
        DWORD  dmDisplayFixedOutput;
      };

      short  dmColor;
      short  dmDuplex;
      short  dmYResolution;
      short  dmTTOption;
      short  dmCollate;
      BYTE  dmFormName[CCHFORMNAME];
      WORD  dmLogPixels;
      DWORD  dmBitsPerPel;
      DWORD  dmPelsWidth;
      DWORD  dmPelsHeight;
      union {
        DWORD  dmDisplayFlags;
        DWORD  dmNup;
      }
      DWORD  dmDisplayFrequency;
    #if(WINVER >= 0x0400)
      DWORD  dmICMMethod;
      DWORD  dmICMIntent;
      DWORD  dmMediaType;
      DWORD  dmDitherType;
      DWORD  dmReserved1;
      DWORD  dmReserved2;
    #if (WINVER >= 0x0500) || (_WIN32_WINNT >= 0x0400)
      DWORD  dmPanningWidth;
      DWORD  dmPanningHeight;
    #endif
    #endif
    } DEVMODE;
    */

    //    #endregion DeviceMode class

    //    #region DisplayDevice

    //    #endregion

    //    #region Window Handling

    //    #region WindowClass
    //    [StructLayout(LayoutKind.Sequential)]
    //    internal struct WindowClass
    //    {
    //        internal ClassStyle Style;
    //        [MarshalAs(UnmanagedType.FunctionPtr)]
    //        internal WindowProcedure WindowProcedure;
    //        internal Int32 ClassExtraBytes;
    //        internal Int32 WindowExtraBytes;
    //        //[MarshalAs(UnmanagedType.
    //        internal IntPtr Instance;
    //        internal IntPtr Icon;
    //        internal IntPtr Cursor;
    //        internal IntPtr BackgroundBrush;
    //        //[MarshalAs(UnmanagedType.LPStr)]
    //        internal IntPtr MenuName;
    //        [MarshalAs(UnmanagedType.LPTStr)]
    //        internal String ClassName;
    //        //internal String ClassName;

    //        internal static Int32 SizeInBytes = Marshal.SizeOf(default(WindowClass));
    //    }
    //    #endregion

    //    #region ExtendedWindowClass

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct ExtendedWindowClass
    {
        public uint Size;
        public ClassStyle Style;

        //public WNDPROC WndProc;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public WindowProcedure WndProc;

        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr Instance;
        public IntPtr Icon;
        public IntPtr Cursor;
        public IntPtr Background;
        public IntPtr MenuName;
        public IntPtr ClassName;
        public IntPtr IconSm;

        public static uint SizeInBytes = (uint)Marshal.SizeOf(default(ExtendedWindowClass));
    }

    /// \internal
    /// <summary>
    /// The WindowPosition structure contains information about the size and position of a window.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowPosition
    {
        /// <summary>
        /// Handle to the window.
        /// </summary>
        internal IntPtr hwnd;

        /// <summary>
        /// Specifies the position of the window in Z order (front-to-back position).
        /// This member can be a handle to the window behind which this window is placed,
        /// or can be one of the special values listed with the SetWindowPos function.
        /// </summary>
        internal IntPtr hwndInsertAfter;

        /// <summary>
        /// Specifies the position of the left edge of the window.
        /// </summary>
        internal int x;

        /// <summary>
        /// Specifies the position of the top edge of the window.
        /// </summary>
        internal int y;

        /// <summary>
        /// Specifies the window width, in pixels.
        /// </summary>
        internal int cx;

        /// <summary>
        /// Specifies the window height, in pixels.
        /// </summary>
        internal int cy;

        /// <summary>
        /// Specifies the window position.
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        internal SetWindowPosFlags flags;
    }

    [Flags]
    internal enum SetWindowPosFlags : int
    {
        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        NOSIZE = 0x0001,

        /// <summary>
        /// Retains the current position (ignores the x and y parameters).
        /// </summary>
        NOMOVE = 0x0002,

        /// <summary>
        /// Retains the current Z order (ignores the hwndInsertAfter parameter).
        /// </summary>
        NOZORDER = 0x0004,

        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs.
        /// This applies to the client area, the nonclient area (including the title bar and scroll bars),
        /// and any part of the parent window uncovered as a result of the window being moved.
        /// When this flag is set, the application must explicitly invalidate or redraw any parts
        /// of the window and parent window that need redrawing.
        /// </summary>
        NOREDRAW = 0x0008,

        /// <summary>
        /// Does not activate the window. If this flag is not set,
        /// the window is activated and moved to the top of either the topmost or non-topmost group
        /// (depending on the setting of the hwndInsertAfter member).
        /// </summary>
        NOACTIVATE = 0x0010,

        /// <summary>
        /// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed.
        /// If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        FRAMECHANGED = 0x0020, /* The frame changed: send WM_NCCALCSIZE */

        /// <summary>
        /// Displays the window.
        /// </summary>
        SHOWWINDOW = 0x0040,

        /// <summary>
        /// Hides the window.
        /// </summary>
        HIDEWINDOW = 0x0080,

        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified,
        /// the valid contents of the client area are saved and copied back into the client area
        /// after the window is sized or repositioned.
        /// </summary>
        NOCOPYBITS = 0x0100,

        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        NOOWNERZORDER = 0x0200, /* Don't do owner Z ordering */

        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        NOSENDCHANGING = 0x0400, /* Don't send WM_WINDOWPOSCHANGING */

        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        DRAWFRAME = FRAMECHANGED,

        /// <summary>
        /// Same as the NOOWNERZORDER flag.
        /// </summary>
        NOREPOSITION = NOOWNERZORDER,

        DEFERERASE = 0x2000,
        ASYNCWINDOWPOS = 0x4000
    }

    /// \internal
    /// <summary>
    /// Defines information for the raw input devices.
    /// </summary>
    /// <remarks>
    /// If RIDEV_NOLEGACY is set for a mouse or a keyboard, the system does not generate any legacy message for that device for the application. For example, if the mouse TLC is set with RIDEV_NOLEGACY, WM_LBUTTONDOWN and related legacy mouse messages are not generated. Likewise, if the keyboard TLC is set with RIDEV_NOLEGACY, WM_KEYDOWN and related legacy keyboard messages are not generated.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        /// <summary>
        /// Top level collection Usage page for the raw input device.
        /// </summary>
        //internal USHORT UsagePage;
        internal short UsagePage;

        /// <summary>
        /// Top level collection Usage for the raw input device.
        /// </summary>
        //internal USHORT Usage;
        internal short Usage;

        /// <summary>
        /// Mode flag that specifies how to interpret the information provided by UsagePage and Usage.
        /// It can be zero (the default) or one of the following values.
        /// By default, the operating system sends raw input from devices with the specified top level collection (TLC)
        /// to the registered application as long as it has the window focus.
        /// </summary>
        internal RawInputDeviceFlags Flags;

        /// <summary>
        /// Handle to the target window. If NULL it follows the keyboard focus.
        /// </summary>
        internal IntPtr Target;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}, flags: {2}, window: {3}", UsagePage, Usage, Flags, Target);
        }
    }

    /// \internal
    /// <summary>
    /// Contains information about a raw input device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDeviceList
    {
        /// <summary>
        /// Handle to the raw input device.
        /// </summary>
        internal IntPtr Device;

        /// <summary>
        /// Type of device.
        /// </summary>
        internal RawInputDeviceType Type;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, Handle: {1}", Type, Device);
        }
    }

    /// \internal
    /// <summary>
    /// Contains the raw input from a device.
    /// </summary>
    /// <remarks>
    /// <para>The handle to this structure is passed in the lParam parameter of WM_INPUT.</para>
    /// <para>To get detailed information -- such as the header and the content of the raw input -- call GetRawInputData.</para>
    /// <para>To get device specific information, call GetRawInputDeviceInfo with the hDevice from RAWINPUTHEADER.</para>
    /// <para>Raw input is available only when the application calls RegisterRawInputDevices with valid device specifications.</para>
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RawInput
    {
        public RawInputHeader Header;
        public RawInputData Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RawInputData
    {
        [FieldOffset(0)]
        internal RawMouse Mouse;

        [FieldOffset(0)]
        internal RawKeyboard Keyboard;

        [FieldOffset(0)]
        internal RawHID HID;
    }

    /// \internal
    /// <summary>
    /// Contains the header information that is part of the raw input data.
    /// </summary>
    /// <remarks>
    /// To get more information on the device, use hDevice in a call to GetRawInputDeviceInfo.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RawInputHeader
    {
        /// <summary>
        /// Type of raw input.
        /// </summary>
        internal RawInputDeviceType Type;

        /// <summary>
        /// Size, in bytes, of the entire input packet of data. This includes the RawInput struct plus possible extra input reports in the RAWHID variable length array.
        /// </summary>
        internal int Size;

        /// <summary>
        /// Handle to the device generating the raw input data.
        /// </summary>
        internal IntPtr Device;

        /// <summary>
        /// Value passed in the wParam parameter of the WM_INPUT message.
        /// </summary>
        internal IntPtr Param;
    }

    /// \internal
    /// <summary>
    /// Contains information about the state of the keyboard.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawKeyboard
    {
        /// <summary>
        /// Scan code from the key depression. The scan code for keyboard overrun is KEYBOARD_OVERRUN_MAKE_CODE.
        /// </summary>
        //internal USHORT MakeCode;
        internal short MakeCode;

        /// <summary>
        /// Flags for scan code information. It can be one or more of the following.
        /// RI_KEY_MAKE
        /// RI_KEY_BREAK
        /// RI_KEY_E0
        /// RI_KEY_E1
        /// RI_KEY_TERMSRV_SET_LED
        /// RI_KEY_TERMSRV_SHADOW
        /// </summary>
        internal RawInputKeyboardDataFlags Flags;

        /// <summary>
        /// Reserved; must be zero.
        /// </summary>
        private readonly ushort Reserved;

        /// <summary>
        /// Microsoft Windows message compatible virtual-key code. For more information, see Virtual-Key Codes.
        /// </summary>
        //internal USHORT VKey;
        internal VirtualKeys VKey;

        /// <summary>
        /// Corresponding window message, for example WM_KEYDOWN, WM_SYSKEYDOWN, and so forth.
        /// </summary>
        //internal UINT Message;
        internal int Message;

        /// <summary>
        /// Device-specific additional information for the event.
        /// </summary>
        //internal ULONG ExtraInformation;
        internal int ExtraInformation;
    }

    /// \internal
    /// <summary>
    /// Contains information about the state of the mouse.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct RawMouse
    {
        /// <summary>
        /// Mouse state. This member can be any reasonable combination of the following.
        /// MOUSE_ATTRIBUTES_CHANGED
        /// Mouse attributes changed; application needs to query the mouse attributes.
        /// MOUSE_MOVE_RELATIVE
        /// Mouse movement data is relative to the last mouse position.
        /// MOUSE_MOVE_ABSOLUTE
        /// Mouse movement data is based on absolute position.
        /// MOUSE_VIRTUAL_DESKTOP
        /// Mouse coordinates are mapped to the virtual desktop (for a multiple monitor system).
        /// </summary>
        [FieldOffset(0)]
        public RawMouseFlags Flags;  // USHORT in winuser.h, but only INT works -- USHORT returns 0.

        [FieldOffset(4)]
        public RawInputMouseState ButtonFlags;

        /// <summary>
        /// If usButtonFlags is RI_MOUSE_WHEEL, this member is a signed value that specifies the wheel delta.
        /// </summary>
        [FieldOffset(6)]
        public ushort ButtonData;

        /// <summary>
        /// Raw state of the mouse buttons.
        /// </summary>
        [FieldOffset(8)]
        public uint RawButtons;

        /// <summary>
        /// Motion in the X direction. This is signed relative motion or absolute motion, depending on the value of usFlags.
        /// </summary>
        [FieldOffset(12)]
        public int LastX;

        /// <summary>
        /// Motion in the Y direction. This is signed relative motion or absolute motion, depending on the value of usFlags.
        /// </summary>
        [FieldOffset(16)]
        public int LastY;

        /// <summary>
        /// Device-specific additional information for the event.
        /// </summary>
        [FieldOffset(20)]
        public uint ExtraInformation;
    }

    /// \internal
    /// <summary>
    /// The RawHID structure describes the format of the raw input
    /// from a Human Interface Device (HID).
    /// </summary>
    /// <remarks>
    /// Each WM_INPUT can indicate several inputs, but all of the inputs
    /// come from the same HID. The size of the bRawData array is
    /// dwSizeHid * dwCount.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawHID
    {
        /// <summary>
        /// Size, in bytes, of each HID input in bRawData.
        /// </summary>
        internal int SizeHid;

        /// <summary>
        /// Number of HID inputs in bRawData.
        /// </summary>
        internal int Count;

        // The RawData field must be marshalled manually.
        ///// <summary>
        ///// Raw input data as an array of bytes.
        ///// </summary>

        //internal byte i1;
        // internal byte i2;

        internal byte b1;
        internal byte b2;
        internal byte b3;

        //internal UInt32 b4;
        //internal UInt32 b5;
        internal byte b41;

        internal byte b42;
        internal byte b43;
        internal byte b44;
        internal byte b51;
        internal byte b52;
        internal byte b53;
        internal byte b54;

        //internal UIntPtr i3;
        //internal UIntPtr i4;
        //internal UIntPtr i5;
        //internal UIntPtr i6;
        //internal UIntPtr i7;
        //internal UIntPtr i8;
        //internal UInt16 i4;
        //internal UInt16 i5;
        //internal UInt16 i6;
        //internal UInt16 i7;
        //internal UIntPtr RawData;
    }

    /// \internal
    /// <summary>
    /// Defines the raw input data coming from the specified Human Interface Device (HID).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputHIDDeviceInfo
    {
        /// <summary>
        /// Vendor ID for the HID.
        /// </summary>
        internal int VendorId;

        /// <summary>
        /// Product ID for the HID.
        /// </summary>
        internal int ProductId;

        /// <summary>
        /// Version number for the HID.
        /// </summary>
        internal int VersionNumber;

        /// <summary>
        /// Top-level collection Usage Page for the device.
        /// </summary>
        //internal USHORT UsagePage;
        internal short UsagePage;

        /// <summary>
        /// Top-level collection Usage for the device.
        /// </summary>
        //internal USHORT Usage;
        internal short Usage;
    }

    /// \internal
    /// <summary>
    /// Defines the raw input data coming from the specified keyboard.
    /// </summary>
    /// <remarks>
    /// For the keyboard, the Usage Page is 1 and the Usage is 6.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputKeyboardDeviceInfo
    {
        /// <summary>
        /// Type of the keyboard.
        /// </summary>
        internal int Type;

        /// <summary>
        /// Subtype of the keyboard.
        /// </summary>
        internal int SubType;

        /// <summary>
        /// Scan code mode.
        /// </summary>
        internal int KeyboardMode;

        /// <summary>
        /// Number of function keys on the keyboard.
        /// </summary>
        internal int NumberOfFunctionKeys;

        /// <summary>
        /// Number of LED indicators on the keyboard.
        /// </summary>
        internal int NumberOfIndicators;

        /// <summary>
        /// Total number of keys on the keyboard.
        /// </summary>
        internal int NumberOfKeysTotal;
    }

    /// \internal
    /// <summary>
    /// Defines the raw input data coming from the specified mouse.
    /// </summary>
    /// <remarks>
    /// For the keyboard, the Usage Page is 1 and the Usage is 2.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputMouseDeviceInfo
    {
        /// <summary>
        /// ID for the mouse device.
        /// </summary>
        internal int Id;

        /// <summary>
        /// Number of buttons for the mouse.
        /// </summary>
        internal int NumberOfButtons;

        /// <summary>
        /// Number of data points per second. This information may not be applicable for every mouse device.
        /// </summary>
        internal int SampleRate;

        /// <summary>
        /// TRUE if the mouse has a wheel for horizontal scrolling; otherwise, FALSE.
        /// </summary>
        /// <remarks>
        /// This member is only supported under Microsoft Windows Vista and later versions.
        /// </remarks>
        internal bool HasHorizontalWheel;
    }

    //    #endregion

    //    #endregion

    //    #region GetWindowLongOffsets

    //    #endregion

    //    #region Rectangle

    /// \internal
    /// <summary>
    /// Defines the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    /// <remarks>
    /// By convention, the right and bottom edges of the rectangle are normally considered exclusive. In other words, the pixel whose coordinates are (right, bottom) lies immediately outside of the the rectangle. For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including, the right column and bottom row of pixels. This structure is identical to the RECTL structure.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Win32Rectangle
    {
        /// <summary>
        /// Specifies the x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        internal int left;

        /// <summary>
        /// Specifies the y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        internal int top;

        /// <summary>
        /// Specifies the x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        internal int right;

        /// <summary>
        /// Specifies the y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        internal int bottom;

        internal int Width { get { return right - left; } }
        internal int Height { get { return bottom - top; } }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0},{1})-({2},{3})", left, top, right, bottom);
        }

        internal Rectangle ToRectangle()
        {
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        internal static Win32Rectangle From(Rectangle value)
        {
            Win32Rectangle rect = new Win32Rectangle();
            rect.left = value.Left;
            rect.right = value.Right;
            rect.top = value.Top;
            rect.bottom = value.Bottom;
            return rect;
        }

        internal static Win32Rectangle From(Size value)
        {
            Win32Rectangle rect = new Win32Rectangle();
            rect.left = 0;
            rect.right = value.Width;
            rect.top = 0;
            rect.bottom = value.Height;
            return rect;
        }
    }

    //    #endregion

    //    #region WindowInfo

    /// \internal
    /// <summary>
    /// Contains window information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowInfo
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// </summary>
        public int Size;

        /// <summary>
        /// Pointer to a RECT structure that specifies the coordinates of the window.
        /// </summary>
        public OpenTK.Platform.Windows.Win32Rectangle Window;

        /// <summary>
        /// Pointer to a RECT structure that specifies the coordinates of the client area.
        /// </summary>
        public OpenTK.Platform.Windows.Win32Rectangle Client;

        /// <summary>
        /// The window styles. For a table of window styles, see CreateWindowEx.
        /// </summary>
        public WindowStyle Style;

        /// <summary>
        /// The extended window styles. For a table of extended window styles, see CreateWindowEx.
        /// </summary>
        public ExtendedWindowStyle ExStyle;

        /// <summary>
        /// The window status. If this member is WS_ACTIVECAPTION, the window is active. Otherwise, this member is zero.
        /// </summary>
        public int WindowStatus;

        /// <summary>
        /// The width of the window border, in pixels.
        /// </summary>
        public uint WindowBordersX;

        /// <summary>
        /// The height of the window border, in pixels.
        /// </summary>
        public uint WindowBordersY;

        /// <summary>
        /// The window class atom (see RegisterClass).
        /// </summary>
        public int WindowType;

        /// <summary>
        /// The Microsoft Windows version of the application that created the window.
        /// </summary>
        public short CreatorVersion;
    }

    //    #endregion

    //    #region MonitorInfo

    internal struct MonitorInfo
    {
        public int Size;
        public OpenTK.Platform.Windows.Win32Rectangle Monitor;
        public OpenTK.Platform.Windows.Win32Rectangle Work;
        public int Flags;

        public static readonly int SizeInBytes = Marshal.SizeOf(default(MonitorInfo));
    }

    //    #endregion

    //    #region NcCalculateSize

    //    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //    internal struct NcCalculateSize
    //    {
    //        public Win32Rectangle NewBounds;
    //        public Win32Rectangle OldBounds;
    //        public Win32Rectangle OldClientRectangle;
    //        unsafe public WindowPosition* Position;
    //    }

    //    #endregion

    //    #region ShFileInfo

    //    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //    struct SHFILEINFO
    //    {
    //        public IntPtr hIcon;
    //        public Int32 iIcon;
    //        public uint dwAttributes;
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    //        public String szDisplayName;
    //        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
    //        public String szTypeName;
    //    };

    //    #endregion

    //    #region TrackMouseEventStructure

    internal struct TrackMouseEventStructure
    {
        public int Size;
        public TrackMouseEventFlags Flags;
        public IntPtr TrackWindowHandle;
        public int HoverTime;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(TrackMouseEventStructure));
    }

    //    #endregion

    //    #region BroadcastHeader

    //    struct BroadcastHeader
    //    {
    //        public Int32 Size;
    //        public DeviceBroadcastType DeviceType;
    //        Int32 dbch_reserved;
    //    }

    //    #endregion

    //    #region BroadcastDeviceInterface

    internal struct BroadcastDeviceInterface
    {
        public int Size;
        public DeviceBroadcastType DeviceType;
        private readonly int dbcc_reserved;
        public Guid ClassGuid;
        public char dbcc_name;
    }

    //    #endregion

    //    #region MouseMovePoint

    /// <summary>
    /// Contains information about the mouse's location in screen coordinates.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseMovePoint
    {
        /// <summary>
        /// The x-coordinate of the mouse.
        /// </summary>
        public int X;

        /// <summary>
        /// The y-coordinate of the mouse.
        /// </summary>
        public int Y;

        /// <summary>
        /// The time stamp of the mouse coordinate.
        /// </summary>
        public int Time;

        /// <summary>
        /// Additional information associated with this coordinate.
        /// </summary>
        private readonly IntPtr ExtraInfo;

        /// <summary>
        /// Returns the size of a MouseMovePoint in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(default(MouseMovePoint));
    }

    //    #endregion

    //    #region IconInfo

    /// \internal
    /// <summary>
    /// Contains information about an icon or a cursor.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct IconInfo
    {
        /// <summary>
        /// Specifies whether this structure defines an icon or a cursor. A
        /// value of TRUE specifies an icon; FALSE specifies a cursor
        /// </summary>
        public bool fIcon;

        /// <summary>
        /// The x-coordinate of a cursor's hot spot. If this structure defines
        /// an icon, the hot spot is always in the center of the icon, and
        /// this member is ignored.
        /// </summary>
        public int xHotspot;

        /// <summary>
        /// The y-coordinate of a cursor's hot spot. If this structure defines
        /// an icon, the hot spot is always in the center of the icon, and
        /// this member is ignored.
        /// </summary>
        public int yHotspot;

        /// <summary>
        /// The icon bitmask bitmap. If this structure defines a black and
        /// white icon, this bitmask is formatted so that the upper half is
        /// the icon AND bitmask and the lower half is the icon XOR bitmask.
        /// Under this condition, the height should be an even multiple of
        /// two. If this structure defines a color icon, this mask only
        /// defines the AND bitmask of the icon.
        /// </summary>
        public IntPtr hbmMask;

        /// <summary>
        /// A handle to the icon color bitmap. This member can be optional if
        /// this structure defines a black and white icon. The AND bitmask of
        /// hbmMask is applied with the SRCAND flag to the destination;
        /// subsequently, the color bitmap is applied (using XOR) to the
        /// destination by using the SRCINVERT flag.
        /// </summary>
        public IntPtr hbmColor;
    }

    //    #endregion

    //    #endregion

    //    #region --- Enums ---

    //    #region GetWindowLongOffset

    /// <summary>
    /// Window field offsets for GetWindowLong() and GetWindowLongPtr().
    /// </summary>
    internal enum GWL
    {
        WNDPROC = (-4),
        HINSTANCE = (-6),
        HWNDPARENT = (-8),
        STYLE = (-16),
        EXSTYLE = (-20),
        USERDATA = (-21),
        ID = (-12),
    }

    //    #endregion

    //    #region SizeMessage

    internal enum SizeMessage
    {
        MAXHIDE = 4,
        MAXIMIZED = 2,
        MAXSHOW = 3,
        MINIMIZED = 1,
        RESTORED = 0
    }

    //    #endregion

    //    #region NcCalcSizeOptions

    //    internal enum NcCalcSizeOptions
    //    {
    //        ALIGNTOP = 0x10,
    //        ALIGNRIGHT = 0x80,
    //        ALIGNLEFT = 0x20,
    //        ALIGNBOTTOM = 0x40,
    //        HREDRAW = 0x100,
    //        VREDRAW = 0x200,
    //        REDRAW = (HREDRAW | VREDRAW),
    //        VALIDRECTS = 0x400
    //    }

    //    #endregion

    //    #region DeviceCaps

    internal enum DeviceCaps
    {
        LogPixelsX = 88,
        LogPixelsY = 90
    }

    //    #endregion

    //    #region internal enum DisplayModeSettingsEnum

    internal enum DisplayModeSettingsEnum
    {
        CurrentSettings = -1,
        RegistrySettings = -2
    }

    //    #endregion

    //    #region internal enum DisplayDeviceStateFlags

    [Flags]
    internal enum DisplayDeviceStateFlags
    {
        None = 0x00000000,
        AttachedToDesktop = 0x00000001,
        MultiDriver = 0x00000002,
        PrimaryDevice = 0x00000004,
        MirroringDriver = 0x00000008,
        VgaCompatible = 0x00000010,
        Removable = 0x00000020,
        ModesPruned = 0x08000000,
        Remote = 0x04000000,
        Disconnect = 0x02000000,

        // Child device state
        Active = 0x00000001,

        Attached = 0x00000002,
    }

    //    #endregion

    //    #region internal enum ChangeDisplaySettingsEnum

    [Flags]
    internal enum ChangeDisplaySettingsEnum
    {
        // ChangeDisplaySettings types (found in winuser.h)
        UpdateRegistry = 0x00000001,

        Test = 0x00000002,
        Fullscreen = 0x00000004,
    }

    //    #endregion

    //    #region internal enum WindowStyle : uint

    [Flags]
    internal enum WindowStyle : uint
    {
        Overlapped = 0x00000000,
        Popup = 0x80000000,
        Child = 0x40000000,
        Minimize = 0x20000000,
        Visible = 0x10000000,
        Disabled = 0x08000000,
        ClipSiblings = 0x04000000,
        ClipChildren = 0x02000000,
        Maximize = 0x01000000,
        Caption = 0x00C00000,    // Border | DialogFrame
        Border = 0x00800000,
        DialogFrame = 0x00400000,
        VScroll = 0x00200000,
        HScreen = 0x00100000,
        SystemMenu = 0x00080000,
        ThickFrame = 0x00040000,
        Group = 0x00020000,
        TabStop = 0x00010000,

        MinimizeBox = 0x00020000,
        MaximizeBox = 0x00010000,

        Tiled = Overlapped,
        Iconic = Minimize,
        SizeBox = ThickFrame,
        TiledWindow = OverlappedWindow,

        // Common window styles:
        OverlappedWindow = Overlapped | Caption | SystemMenu | ThickFrame | MinimizeBox | MaximizeBox,

        PopupWindow = Popup | Border | SystemMenu,
        ChildWindow = Child
    }

    //    #endregion

    //    #region internal enum ExtendedWindowStyle : uint

    [Flags]
    internal enum ExtendedWindowStyle : uint
    {
        DialogModalFrame = 0x00000001,
        NoParentNotify = 0x00000004,
        Topmost = 0x00000008,
        AcceptFiles = 0x00000010,
        Transparent = 0x00000020,

        // #if(WINVER >= 0x0400)
        MdiChild = 0x00000040,

        ToolWindow = 0x00000080,
        WindowEdge = 0x00000100,
        ClientEdge = 0x00000200,
        ContextHelp = 0x00000400,
        // #endif

        // #if(WINVER >= 0x0400)
        Right = 0x00001000,

        Left = 0x00000000,
        RightToLeftReading = 0x00002000,
        LeftToRightReading = 0x00000000,
        LeftScrollbar = 0x00004000,
        RightScrollbar = 0x00000000,

        ControlParent = 0x00010000,
        StaticEdge = 0x00020000,
        ApplicationWindow = 0x00040000,

        OverlappedWindow = WindowEdge | ClientEdge,
        PaletteWindow = WindowEdge | ToolWindow | Topmost,
        // #endif

        // #if(_WIN32_WINNT >= 0x0500)
        Layered = 0x00080000,

        // #endif

        // #if(WINVER >= 0x0500)
        NoInheritLayout = 0x00100000, // Disable inheritence of mirroring by children

        RightToLeftLayout = 0x00400000, // Right to left mirroring
        // #endif /* WINVER >= 0x0500 */

        // #if(_WIN32_WINNT >= 0x0501)
        Composited = 0x02000000,

        // #endif /* _WIN32_WINNT >= 0x0501 */

        // #if(_WIN32_WINNT >= 0x0500)
        NoActivate = 0x08000000

        // #endif /* _WIN32_WINNT >= 0x0500 */
    }

    //    #endregion

    //    #region GetWindowLongOffsets enum

    internal enum GetWindowLongOffsets : int
    {
        WNDPROC = (-4),
        HINSTANCE = (-6),
        HWNDPARENT = (-8),
        STYLE = (-16),
        EXSTYLE = (-20),
        USERDATA = (-21),
        ID = (-12),
    }

    //    #endregion

    //    #region PixelFormatDescriptorFlags enum
    [Flags]
    internal enum PixelFormatDescriptorFlags : int
    {
        // PixelFormatDescriptor flags
        DOUBLEBUFFER = 0x01,

        STEREO = 0x02,
        DRAW_TO_WINDOW = 0x04,
        DRAW_TO_BITMAP = 0x08,
        SUPPORT_GDI = 0x10,
        SUPPORT_OPENGL = 0x20,
        GENERIC_FORMAT = 0x40,
        NEED_PALETTE = 0x80,
        NEED_SYSTEM_PALETTE = 0x100,
        SWAP_EXCHANGE = 0x200,
        SWAP_COPY = 0x400,
        SWAP_LAYER_BUFFERS = 0x800,
        GENERIC_ACCELERATED = 0x1000,
        SUPPORT_DIRECTDRAW = 0x2000,
        SUPPORT_COMPOSITION = 0x8000,

        // PixelFormatDescriptor flags for use in ChoosePixelFormat only
        DEPTH_DONTCARE = unchecked((int)0x20000000),

        DOUBLEBUFFER_DONTCARE = unchecked((int)0x40000000),
        STEREO_DONTCARE = unchecked((int)0x80000000)
    }

    //    #endregion

    //    #region PixelType

    internal enum PixelType : byte
    {
        RGBA = 0,
        INDEXED = 1
    }

    //    #endregion

    //    #region WindowPlacementOptions enum

    //    internal enum WindowPlacementOptions
    //    {
    //        TOP = 0,
    //        BOTTOM = 1,
    //        TOPMOST = -1,
    //        NOTOPMOST = -2
    //    }

    //    #endregion

    //    #region ClassStyle enum
    [Flags]
    internal enum ClassStyle
    {
        //None            = 0x0000,
        VRedraw = 0x0001,

        HRedraw = 0x0002,
        DoubleClicks = 0x0008,
        OwnDC = 0x0020,
        ClassDC = 0x0040,
        ParentDC = 0x0080,
        NoClose = 0x0200,
        SaveBits = 0x0800,
        ByteAlignClient = 0x1000,
        ByteAlignWindow = 0x2000,
        GlobalClass = 0x4000,

        Ime = 0x00010000,

        // #if(_WIN32_WINNT >= 0x0501)
        DropShadow = 0x00020000

        // #endif /* _WIN32_WINNT >= 0x0501 */
    }

    //    #endregion

    //    #region RawInputDeviceFlags enum

    [Flags]
    internal enum RawInputDeviceFlags : int
    {
        /// <summary>
        /// If set, this removes the top level collection from the inclusion list.
        /// This tells the operating system to stop reading from a device which matches the top level collection.
        /// </summary>
        REMOVE = 0x00000001,

        /// <summary>
        /// If set, this specifies the top level collections to exclude when reading a complete usage page.
        /// This flag only affects a TLC whose usage page is already specified with RawInputDeviceEnum.PAGEONLY.
        /// </summary>
        EXCLUDE = 0x00000010,

        /// <summary>
        /// If set, this specifies all devices whose top level collection is from the specified UsagePage.
        /// Note that usUsage must be zero. To exclude a particular top level collection, use EXCLUDE.
        /// </summary>
        PAGEONLY = 0x00000020,

        /// <summary>
        /// If set, this prevents any devices specified by UsagePage or Usage from generating legacy messages.
        /// This is only for the mouse and keyboard. See RawInputDevice Remarks.
        /// </summary>
        NOLEGACY = 0x00000030,

        /// <summary>
        /// If set, this enables the caller to receive the input even when the caller is not in the foreground.
        /// Note that Target must be specified in RawInputDevice.
        /// </summary>
        INPUTSINK = 0x00000100,

        /// <summary>
        /// If set, the mouse button click does not activate the other window.
        /// </summary>
        CAPTUREMOUSE = 0x00000200, // effective when mouse nolegacy is specified, otherwise it would be an error

        /// <summary>
        /// If set, the application-defined keyboard device hotkeys are not handled.
        /// However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled.
        /// By default, all keyboard hotkeys are handled.
        /// NOHOTKEYS can be specified even if NOLEGACY is not specified and Target is NULL in RawInputDevice.
        /// </summary>
        NOHOTKEYS = 0x00000200, // effective for keyboard.

        /// <summary>
        /// Microsoft Windows XP Service Pack 1 (SP1): If set, the application command keys are handled. APPKEYS can be specified only if NOLEGACY is specified for a keyboard device.
        /// </summary>
        APPKEYS = 0x00000400, // effective for keyboard.

        /// <summary>
        /// If set, this enables the caller to receive input in the background only if the foreground application
        /// does not process it. In other words, if the foreground application is not registered for raw input,
        /// then the background application that is registered will receive the input.
        /// </summary>
        EXINPUTSINK = 0x00001000,

        DEVNOTIFY = 0x00002000,
        //EXMODEMASK      = 0x000000F0
    }

    //    #endregion

    //    #region GetRawInputDataEnum

    internal enum GetRawInputDataEnum
    {
        INPUT = 0x10000003,
        HEADER = 0x10000005
    }

    //    #endregion

    //    #region RawInputDeviceInfoEnum

    internal enum RawInputDeviceInfoEnum
    {
        PREPARSEDDATA = 0x20000005,
        DEVICENAME = 0x20000007,  // the return valus is the character length, not the byte size
        DEVICEINFO = 0x2000000b
    }

    //    #endregion

    //    #region RawInputMouseState

    [Flags]
    internal enum RawInputMouseState : ushort
    {
        LEFT_BUTTON_DOWN = 0x0001,  // Left Button changed to down.
        LEFT_BUTTON_UP = 0x0002,  // Left Button changed to up.
        RIGHT_BUTTON_DOWN = 0x0004,  // Right Button changed to down.
        RIGHT_BUTTON_UP = 0x0008,  // Right Button changed to up.
        MIDDLE_BUTTON_DOWN = 0x0010,  // Middle Button changed to down.
        MIDDLE_BUTTON_UP = 0x0020,  // Middle Button changed to up.

        BUTTON_1_DOWN = LEFT_BUTTON_DOWN,
        BUTTON_1_UP = LEFT_BUTTON_UP,
        BUTTON_2_DOWN = RIGHT_BUTTON_DOWN,
        BUTTON_2_UP = RIGHT_BUTTON_UP,
        BUTTON_3_DOWN = MIDDLE_BUTTON_DOWN,
        BUTTON_3_UP = MIDDLE_BUTTON_UP,

        BUTTON_4_DOWN = 0x0040,
        BUTTON_4_UP = 0x0080,
        BUTTON_5_DOWN = 0x0100,
        BUTTON_5_UP = 0x0200,

        WHEEL = 0x0400,
        HWHEEL = 0x0800,
    }

    //    [Flags]
    //    internal enum RawInputTouchState : ushort
    //    {
    //        LEFT_BUTTON_DOWN = 0x0001,  // Left Button changed to down.
    //        LEFT_BUTTON_UP = 0x0002,  // Left Button changed to up.
    //        RIGHT_BUTTON_DOWN = 0x0004,  // Right Button changed to down.
    //        RIGHT_BUTTON_UP = 0x0008,  // Right Button changed to up.
    //        MIDDLE_BUTTON_DOWN = 0x0010,  // Middle Button changed to down.
    //        MIDDLE_BUTTON_UP = 0x0020,  // Middle Button changed to up.

    //        BUTTON_1_DOWN = LEFT_BUTTON_DOWN,
    //        BUTTON_1_UP = LEFT_BUTTON_UP,
    //        BUTTON_2_DOWN = RIGHT_BUTTON_DOWN,
    //        BUTTON_2_UP = RIGHT_BUTTON_UP,
    //        BUTTON_3_DOWN = MIDDLE_BUTTON_DOWN,
    //        BUTTON_3_UP = MIDDLE_BUTTON_UP,

    //        BUTTON_4_DOWN = 0x0040,
    //        BUTTON_4_UP = 0x0080,
    //        BUTTON_5_DOWN = 0x0100,
    //        BUTTON_5_UP = 0x0200,

    //        WHEEL = 0x0400,
    //        HWHEEL = 0x0800,
    //    }
    //    #endregion

    //    #region RawInputKeyboardDataFlags

    internal enum RawInputKeyboardDataFlags : short //: ushort
    {
        MAKE = 0,
        BREAK = 1,
        E0 = 2,
        E1 = 4,
        TERMSRV_SET_LED = 8,
        TERMSRV_SHADOW = 0x10
    }

    //    #endregion

    //    #region RawInputDeviceType

    internal enum RawInputDeviceType : int
    {
        MOUSE = 0,
        KEYBOARD = 1,
        HID = 2
    }

    //    #endregion

    //    #region RawMouseFlags

    /// <summary>
    /// Mouse indicator flags (found in winuser.h).
    /// </summary>
    [Flags]
    internal enum RawMouseFlags : ushort
    {
        /// <summary>
        /// LastX/Y indicate relative motion.
        /// </summary>
        MOUSE_MOVE_RELATIVE = 0x00,

        /// <summary>
        /// LastX/Y indicate absolute motion.
        /// </summary>
        MOUSE_MOVE_ABSOLUTE = 0x01,

        /// <summary>
        /// The coordinates are mapped to the virtual desktop.
        /// </summary>
        MOUSE_VIRTUAL_DESKTOP = 0x02,

        /// <summary>
        /// Requery for mouse attributes.
        /// </summary>
        MOUSE_ATTRIBUTES_CHANGED = 0x04,
    }

    //    /// <summary>
    //    /// Touch indicator flags (found in winuser.h).
    //    /// </summary>
    //    [Flags]
    //    internal enum RawTouchFlags : ushort
    //    {
    //        /// <summary>
    //        /// LastX/Y indicate relative motion.
    //        /// </summary>
    //        TOUCH_MOVE_RELATIVE = 0x00,
    //        /// <summary>
    //        /// LastX/Y indicate absolute motion.
    //        /// </summary>
    //        TOUCH_MOVE_ABSOLUTE = 0x01,
    //        /// <summary>
    //        /// The coordinates are mapped to the virtual desktop.
    //        /// </summary>
    //        TOUCH_VIRTUAL_DESKTOP = 0x02,
    //        /// <summary>
    //        /// Requery for touch attributes.
    //        /// </summary>
    //        TOUCH_ATTRIBUTES_CHANGED = 0x04,
    //    }

    //    #endregion

    //    #region VirtualKeys

    internal enum VirtualKeys : short
    {
        /*
         * Virtual Key, Standard Set
         */
        LBUTTON = 0x01,
        RBUTTON = 0x02,
        CANCEL = 0x03,
        MBUTTON = 0x04,   /* NOT contiguous with L & RBUTTON */

        XBUTTON1 = 0x05,   /* NOT contiguous with L & RBUTTON */
        XBUTTON2 = 0x06,   /* NOT contiguous with L & RBUTTON */

        /*
         * 0x07 : unassigned
         */

        BACK = 0x08,
        TAB = 0x09,

        /*
         * 0x0A - 0x0B : reserved
         */

        CLEAR = 0x0C,
        RETURN = 0x0D,

        SHIFT = 0x10,
        CONTROL = 0x11,
        MENU = 0x12,
        PAUSE = 0x13,
        CAPITAL = 0x14,

        KANA = 0x15,
        HANGEUL = 0x15,  /* old name - should be here for compatibility */
        HANGUL = 0x15,
        JUNJA = 0x17,
        FINAL = 0x18,
        HANJA = 0x19,
        KANJI = 0x19,

        ESCAPE = 0x1B,

        CONVERT = 0x1C,
        NONCONVERT = 0x1D,
        ACCEPT = 0x1E,
        MODECHANGE = 0x1F,

        SPACE = 0x20,
        PRIOR = 0x21,
        NEXT = 0x22,
        END = 0x23,
        HOME = 0x24,
        LEFT = 0x25,
        UP = 0x26,
        RIGHT = 0x27,
        DOWN = 0x28,
        SELECT = 0x29,
        PRINT = 0x2A,
        EXECUTE = 0x2B,
        SNAPSHOT = 0x2C,
        INSERT = 0x2D,
        DELETE = 0x2E,
        HELP = 0x2F,

        /*
         * 0 - 9 are the same as ASCII '0' - '9' (0x30 - 0x39)
         * 0x40 : unassigned
         * A - Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
         */

        LWIN = 0x5B,
        RWIN = 0x5C,
        APPS = 0x5D,

        /*
         * 0x5E : reserved
         */

        SLEEP = 0x5F,

        NUMPAD0 = 0x60,
        NUMPAD1 = 0x61,
        NUMPAD2 = 0x62,
        NUMPAD3 = 0x63,
        NUMPAD4 = 0x64,
        NUMPAD5 = 0x65,
        NUMPAD6 = 0x66,
        NUMPAD7 = 0x67,
        NUMPAD8 = 0x68,
        NUMPAD9 = 0x69,
        MULTIPLY = 0x6A,
        ADD = 0x6B,
        SEPARATOR = 0x6C,
        SUBTRACT = 0x6D,
        DECIMAL = 0x6E,
        DIVIDE = 0x6F,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        F13 = 0x7C,
        F14 = 0x7D,
        F15 = 0x7E,
        F16 = 0x7F,
        F17 = 0x80,
        F18 = 0x81,
        F19 = 0x82,
        F20 = 0x83,
        F21 = 0x84,
        F22 = 0x85,
        F23 = 0x86,
        F24 = 0x87,

        /*
         * 0x88 - 0x8F : unassigned
         */

        NUMLOCK = 0x90,
        SCROLL = 0x91,

        /*
         * NEC PC-9800 kbd definitions
         */
        OEM_NEC_EQUAL = 0x92,  // '=' key on numpad

        /*
         * Fujitsu/OASYS kbd definitions
         */
        OEM_FJ_JISHO = 0x92,  // 'Dictionary' key
        OEM_FJ_MASSHOU = 0x93,  // 'Unregister word' key
        OEM_FJ_TOUROKU = 0x94,  // 'Register word' key
        OEM_FJ_LOYA = 0x95,  // 'Left OYAYUBI' key
        OEM_FJ_ROYA = 0x96,  // 'Right OYAYUBI' key

        /*
         * 0x97 - 0x9F : unassigned
         */

        /*
         * L* & R* - left and right Alt, Ctrl and Shift virtual keys.
         * Used only as parameters to GetAsyncKeyState() and GetKeyState().
         * No other API or message will distinguish left and right keys in this way.
         */
        LSHIFT = 0xA0,
        RSHIFT = 0xA1,
        LCONTROL = 0xA2,
        RCONTROL = 0xA3,
        LMENU = 0xA4,
        RMENU = 0xA5,

        BROWSER_BACK = 0xA6,
        BROWSER_FORWARD = 0xA7,
        BROWSER_REFRESH = 0xA8,
        BROWSER_STOP = 0xA9,
        BROWSER_SEARCH = 0xAA,
        BROWSER_FAVORITES = 0xAB,
        BROWSER_HOME = 0xAC,

        VOLUME_MUTE = 0xAD,
        VOLUME_DOWN = 0xAE,
        VOLUME_UP = 0xAF,
        MEDIA_NEXT_TRACK = 0xB0,
        MEDIA_PREV_TRACK = 0xB1,
        MEDIA_STOP = 0xB2,
        MEDIA_PLAY_PAUSE = 0xB3,
        LAUNCH_MAIL = 0xB4,
        LAUNCH_MEDIA_SELECT = 0xB5,
        LAUNCH_APP1 = 0xB6,
        LAUNCH_APP2 = 0xB7,

        /*
         * 0xB8 - 0xB9 : reserved
         */

        OEM_1 = 0xBA,   // ';:' for US
        OEM_PLUS = 0xBB,   // '+' any country
        OEM_COMMA = 0xBC,   // ',' any country
        OEM_MINUS = 0xBD,   // '-' any country
        OEM_PERIOD = 0xBE,   // '.' any country
        OEM_2 = 0xBF,   // '/?' for US
        OEM_3 = 0xC0,   // '`~' for US

        /*
         * 0xC1 - 0xD7 : reserved
         */

        /*
         * 0xD8 - 0xDA : unassigned
         */

        OEM_4 = 0xDB,  //  '[{' for US
        OEM_5 = 0xDC,  //  '\|' for US
        OEM_6 = 0xDD,  //  ']}' for US
        OEM_7 = 0xDE,  //  ''"' for US
        OEM_8 = 0xDF,

        /*
         * 0xE0 : reserved
         */

        /*
         * Various extended or enhanced keyboards
         */
        OEM_AX = 0xE1,  //  'AX' key on Japanese AX kbd
        OEM_102 = 0xE2,  //  "<>" or "\|" on RT 102-key kbd.
        ICO_HELP = 0xE3,  //  Help key on ICO
        ICO_00 = 0xE4,  //  00 key on ICO

        PROCESSKEY = 0xE5,

        ICO_CLEAR = 0xE6,

        PACKET = 0xE7,

        /*
         * 0xE8 : unassigned
         */

        /*
         * Nokia/Ericsson definitions
         */
        OEM_RESET = 0xE9,
        OEM_JUMP = 0xEA,
        OEM_PA1 = 0xEB,
        OEM_PA2 = 0xEC,
        OEM_PA3 = 0xED,
        OEM_WSCTRL = 0xEE,
        OEM_CUSEL = 0xEF,
        OEM_ATTN = 0xF0,
        OEM_FINISH = 0xF1,
        OEM_COPY = 0xF2,
        OEM_AUTO = 0xF3,
        OEM_ENLW = 0xF4,
        OEM_BACKTAB = 0xF5,

        ATTN = 0xF6,
        CRSEL = 0xF7,
        EXSEL = 0xF8,
        EREOF = 0xF9,
        PLAY = 0xFA,
        ZOOM = 0xFB,
        NONAME = 0xFC,
        PA1 = 0xFD,
        OEM_CLEAR = 0xFE,

        Last
    }

    //    #endregion

    //    #region MouseKeys

    //    /// <summary>
    //    /// Enumerates available mouse keys (suitable for use in WM_MOUSEMOVE messages).
    //    /// </summary>
    //    enum MouseKeys
    //    {
    //        // Summary:
    //        //     No mouse button was pressed.
    //        None = 0,
    //        //
    //        // Summary:
    //        //     The left mouse button was pressed.
    //        Left = 0x0001,
    //        //
    //        // Summary:
    //        //     The right mouse button was pressed.
    //        Right = 0x0002,
    //        //
    //        // Summary:
    //        //     The middle mouse button was pressed.
    //        Middle = 0x0010,
    //        //
    //        // Summary:
    //        //     The first XButton was pressed.
    //        XButton1 = 0x0020,
    //        //
    //        // Summary:
    //        //     The second XButton was pressed.
    //        XButton2 = 0x0040,
    //    }

    //    #endregion

    //    #region QueueStatusFlags

    //    /// \internal
    //    /// <summary>
    //    /// Queue status flags for GetQueueStatus() and MsgWaitForMultipleObjects()
    //    /// </summary>
    //    [Flags]
    //    internal enum QueueStatusFlags
    //    {
    //        /// <summary>
    //        /// A WM_KEYUP, WM_KEYDOWN, WM_SYSKEYUP, or WM_SYSKEYDOWN message is in the queue.
    //        /// </summary>
    //        KEY = 0x0001,
    //        /// <summary>
    //        /// A WM_MOUSEMOVE message is in the queue.
    //        /// </summary>
    //        MOUSEMOVE = 0x0002,
    //        /// <summary>
    //        /// A mouse-button message (WM_LBUTTONUP, WM_RBUTTONDOWN, and so on).
    //        /// </summary>
    //        MOUSEBUTTON = 0x0004,
    //        /// <summary>
    //        /// A posted message (other than those listed here) is in the queue.
    //        /// </summary>
    //        POSTMESSAGE = 0x0008,
    //        /// <summary>
    //        /// A WM_TIMER message is in the queue.
    //        /// </summary>
    //        TIMER = 0x0010,
    //        /// <summary>
    //        /// A WM_PAINT message is in the queue.
    //        /// </summary>
    //        PAINT = 0x0020,
    //        /// <summary>
    //        /// A message sent by another thread or application is in the queue.
    //        /// </summary>
    //        SENDMESSAGE = 0x0040,
    //        /// <summary>
    //        /// A WM_HOTKEY message is in the queue.
    //        /// </summary>
    //        HOTKEY = 0x0080,
    //        /// <summary>
    //        /// A posted message (other than those listed here) is in the queue.
    //        /// </summary>
    //        ALLPOSTMESSAGE = 0x0100,
    //        /// <summary>
    //        /// A raw input message is in the queue. For more information, see Raw Input.
    //        /// Windows XP and higher only.
    //        /// </summary>
    //        RAWINPUT = 0x0400,
    //        /// <summary>
    //        /// A WM_MOUSEMOVE message or mouse-button message (WM_LBUTTONUP, WM_RBUTTONDOWN, and so on).
    //        /// </summary>
    //        MOUSE = MOUSEMOVE | MOUSEBUTTON,
    //        /// <summary>
    //        /// An input message is in the queue. This is composed of KEY, MOUSE and RAWINPUT.
    //        /// Windows XP and higher only.
    //        /// </summary>
    //        INPUT = MOUSE | KEY | RAWINPUT,
    //        /// <summary>
    //        /// An input message is in the queue. This is composed of QS_KEY and QS_MOUSE.
    //        /// Windows 2000 and earlier.
    //        /// </summary>
    //        INPUT_LEGACY = MOUSE | KEY,
    //        /// <summary>
    //        /// An input, WM_TIMER, WM_PAINT, WM_HOTKEY, or posted message is in the queue.
    //        /// </summary>
    //        ALLEVENTS = INPUT | POSTMESSAGE | TIMER | PAINT | HOTKEY,
    //        /// <summary>
    //        /// Any message is in the queue.
    //        /// </summary>
    //        ALLINPUT = INPUT | POSTMESSAGE | TIMER | PAINT | HOTKEY | SENDMESSAGE
    //    }

    //    #endregion

    //    #region WindowMessage

    internal enum WindowMessage : int
    {
        NULL = 0x0000,
        CREATE = 0x0001,
        DESTROY = 0x0002,
        MOVE = 0x0003,
        SIZE = 0x0005,
        ACTIVATE = 0x0006,
        SETFOCUS = 0x0007,
        KILLFOCUS = 0x0008,

        //              internal const uint SETVISIBLE           = 0x0009;
        ENABLE = 0x000A,

        SETREDRAW = 0x000B,
        SETTEXT = 0x000C,
        GETTEXT = 0x000D,
        GETTEXTLENGTH = 0x000E,
        PAINT = 0x000F,
        CLOSE = 0x0010,
        QUERYENDSESSION = 0x0011,
        QUIT = 0x0012,
        QUERYOPEN = 0x0013,
        ERASEBKGND = 0x0014,
        SYSCOLORCHANGE = 0x0015,
        ENDSESSION = 0x0016,

        //              internal const uint SYSTEMERROR          = 0x0017;
        SHOWWINDOW = 0x0018,

        CTLCOLOR = 0x0019,
        WININICHANGE = 0x001A,
        SETTINGCHANGE = 0x001A,
        DEVMODECHANGE = 0x001B,
        ACTIVATEAPP = 0x001C,
        FONTCHANGE = 0x001D,
        TIMECHANGE = 0x001E,
        CANCELMODE = 0x001F,
        SETCURSOR = 0x0020,
        MOUSEACTIVATE = 0x0021,
        CHILDACTIVATE = 0x0022,
        QUEUESYNC = 0x0023,
        GETMINMAXINFO = 0x0024,
        PAINTICON = 0x0026,
        ICONERASEBKGND = 0x0027,
        NEXTDLGCTL = 0x0028,

        //              internal const uint ALTTABACTIVE         = 0x0029;
        SPOOLERSTATUS = 0x002A,

        DRAWITEM = 0x002B,
        MEASUREITEM = 0x002C,
        DELETEITEM = 0x002D,
        VKEYTOITEM = 0x002E,
        CHARTOITEM = 0x002F,
        SETFONT = 0x0030,
        GETFONT = 0x0031,
        SETHOTKEY = 0x0032,
        GETHOTKEY = 0x0033,

        //              internal const uint FILESYSCHANGE        = 0x0034;
        //              internal const uint ISACTIVEICON         = 0x0035;
        //              internal const uint QUERYPARKICON        = 0x0036;
        QUERYDRAGICON = 0x0037,

        COMPAREITEM = 0x0039,

        //              internal const uint TESTING              = 0x003a;
        //              internal const uint OTHERWINDOWCREATED = 0x003c;
        GETOBJECT = 0x003D,

        //                      internal const uint ACTIVATESHELLWINDOW        = 0x003e;
        COMPACTING = 0x0041,

        COMMNOTIFY = 0x0044,
        WINDOWPOSCHANGING = 0x0046,
        WINDOWPOSCHANGED = 0x0047,
        POWER = 0x0048,
        COPYDATA = 0x004A,
        CANCELJOURNAL = 0x004B,
        NOTIFY = 0x004E,
        INPUTLANGCHANGEREQUEST = 0x0050,
        INPUTLANGCHANGE = 0x0051,
        TCARD = 0x0052,
        HELP = 0x0053,
        USERCHANGED = 0x0054,
        NOTIFYFORMAT = 0x0055,
        CONTEXTMENU = 0x007B,
        STYLECHANGING = 0x007C,
        STYLECHANGED = 0x007D,
        DISPLAYCHANGE = 0x007E,
        GETICON = 0x007F,

        // Non-Client messages
        SETICON = 0x0080,

        NCCREATE = 0x0081,
        NCDESTROY = 0x0082,
        NCCALCSIZE = 0x0083,
        NCHITTEST = 0x0084,
        NCPAINT = 0x0085,
        NCACTIVATE = 0x0086,
        GETDLGCODE = 0x0087,
        SYNCPAINT = 0x0088,

        //              internal const uint SYNCTASK       = 0x0089;
        NCMOUSEMOVE = 0x00A0,

        NCLBUTTONDOWN = 0x00A1,
        NCLBUTTONUP = 0x00A2,
        NCLBUTTONDBLCLK = 0x00A3,
        NCRBUTTONDOWN = 0x00A4,
        NCRBUTTONUP = 0x00A5,
        NCRBUTTONDBLCLK = 0x00A6,
        NCMBUTTONDOWN = 0x00A7,
        NCMBUTTONUP = 0x00A8,
        NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONDOWN = 0x00ab,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONUP = 0x00ac,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        NCXBUTTONDBLCLK = 0x00ad,

        INPUT = 0x00FF,

        KEYDOWN = 0x0100,
        KEYFIRST = 0x0100,
        KEYUP = 0x0101,
        CHAR = 0x0102,
        DEADCHAR = 0x0103,
        SYSKEYDOWN = 0x0104,
        SYSKEYUP = 0x0105,
        SYSCHAR = 0x0106,
        SYSDEADCHAR = 0x0107,
        KEYLAST = 0x0108,
        IME_STARTCOMPOSITION = 0x010D,
        IME_ENDCOMPOSITION = 0x010E,
        IME_COMPOSITION = 0x010F,
        IME_KEYLAST = 0x010F,
        INITDIALOG = 0x0110,
        COMMAND = 0x0111,
        SYSCOMMAND = 0x0112,
        TIMER = 0x0113,
        HSCROLL = 0x0114,
        VSCROLL = 0x0115,
        INITMENU = 0x0116,
        INITMENUPOPUP = 0x0117,

        //              internal const uint SYSTIMER       = 0x0118;
        MENUSELECT = 0x011F,

        MENUCHAR = 0x0120,
        ENTERIDLE = 0x0121,
        MENURBUTTONUP = 0x0122,
        MENUDRAG = 0x0123,
        MENUGETOBJECT = 0x0124,
        UNINITMENUPOPUP = 0x0125,
        MENUCOMMAND = 0x0126,

        CHANGEUISTATE = 0x0127,
        UPDATEUISTATE = 0x0128,
        QUERYUISTATE = 0x0129,

        //              internal const uint LBTRACKPOINT     = 0x0131;
        CTLCOLORMSGBOX = 0x0132,

        CTLCOLOREDIT = 0x0133,
        CTLCOLORLISTBOX = 0x0134,
        CTLCOLORBTN = 0x0135,
        CTLCOLORDLG = 0x0136,
        CTLCOLORSCROLLBAR = 0x0137,
        CTLCOLORSTATIC = 0x0138,
        MOUSEMOVE = 0x0200,
        MOUSEFIRST = 0x0200,
        LBUTTONDOWN = 0x0201,
        LBUTTONUP = 0x0202,
        LBUTTONDBLCLK = 0x0203,
        RBUTTONDOWN = 0x0204,
        RBUTTONUP = 0x0205,
        RBUTTONDBLCLK = 0x0206,
        MBUTTONDOWN = 0x0207,
        MBUTTONUP = 0x0208,
        MBUTTONDBLCLK = 0x0209,
        MOUSEWHEEL = 0x020A,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONDOWN = 0x020B,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONUP = 0x020C,

        /// <summary>
        /// Windows 2000 and higher only.
        /// </summary>
        XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// Windows Vista and higher only.
        /// </summary>
        MOUSEHWHEEL = 0x020E,

        PARENTNOTIFY = 0x0210,
        ENTERMENULOOP = 0x0211,
        EXITMENULOOP = 0x0212,
        NEXTMENU = 0x0213,
        SIZING = 0x0214,
        CAPTURECHANGED = 0x0215,
        MOVING = 0x0216,

        //              internal const uint POWERBROADCAST   = 0x0218;
        DEVICECHANGE = 0x0219,

        MDICREATE = 0x0220,
        MDIDESTROY = 0x0221,
        MDIACTIVATE = 0x0222,
        MDIRESTORE = 0x0223,
        MDINEXT = 0x0224,
        MDIMAXIMIZE = 0x0225,
        MDITILE = 0x0226,
        MDICASCADE = 0x0227,
        MDIICONARRANGE = 0x0228,
        MDIGETACTIVE = 0x0229,
        /* D&D messages */

        //              internal const uint DROPOBJECT     = 0x022A;
        //              internal const uint QUERYDROPOBJECT  = 0x022B;
        //              internal const uint BEGINDRAG      = 0x022C;
        //              internal const uint DRAGLOOP       = 0x022D;
        //              internal const uint DRAGSELECT     = 0x022E;
        //              internal const uint DRAGMOVE       = 0x022F;
        MDISETMENU = 0x0230,

        ENTERSIZEMOVE = 0x0231,
        EXITSIZEMOVE = 0x0232,
        DROPFILES = 0x0233,
        MDIREFRESHMENU = 0x0234,
        IME_SETCONTEXT = 0x0281,
        IME_NOTIFY = 0x0282,
        IME_CONTROL = 0x0283,
        IME_COMPOSITIONFULL = 0x0284,
        IME_SELECT = 0x0285,
        IME_CHAR = 0x0286,
        IME_REQUEST = 0x0288,
        IME_KEYDOWN = 0x0290,
        IME_KEYUP = 0x0291,
        NCMOUSEHOVER = 0x02A0,
        MOUSEHOVER = 0x02A1,
        NCMOUSELEAVE = 0x02A2,
        MOUSELEAVE = 0x02A3,
        CUT = 0x0300,
        COPY = 0x0301,
        PASTE = 0x0302,
        CLEAR = 0x0303,
        UNDO = 0x0304,
        RENDERFORMAT = 0x0305,
        RENDERALLFORMATS = 0x0306,
        DESTROYCLIPBOARD = 0x0307,
        DRAWCLIPBOARD = 0x0308,
        PAINTCLIPBOARD = 0x0309,
        VSCROLLCLIPBOARD = 0x030A,
        SIZECLIPBOARD = 0x030B,
        ASKCBFORMATNAME = 0x030C,
        CHANGECBCHAIN = 0x030D,
        HSCROLLCLIPBOARD = 0x030E,
        QUERYNEWPALETTE = 0x030F,
        PALETTEISCHANGING = 0x0310,
        PALETTECHANGED = 0x0311,
        HOTKEY = 0x0312,
        PRINT = 0x0317,
        PRINTCLIENT = 0x0318,
        HANDHELDFIRST = 0x0358,
        HANDHELDLAST = 0x035F,
        AFXFIRST = 0x0360,
        AFXLAST = 0x037F,
        PENWINFIRST = 0x0380,
        PENWINLAST = 0x038F,
        APP = 0x8000,
        USER = 0x0400,

        // Our "private" ones
        MOUSE_ENTER = 0x0401,

        ASYNC_MESSAGE = 0x0403,
        REFLECT = USER + 0x1c00,
        CLOSE_INTERNAL = USER + 0x1c01,

        // NotifyIcon (Systray) Balloon messages
        BALLOONSHOW = USER + 0x0002,

        BALLOONHIDE = USER + 0x0003,
        BALLOONTIMEOUT = USER + 0x0004,
        BALLOONUSERCLICK = USER + 0x0005
    }

    //    #endregion

    //    #region PeekMessageFlags

    [Flags]
    internal enum PeekMessageFlags : uint
    {
        NoRemove = 0,
        Remove = 1,
        NoYield = 2
    }

    //    #endregion

    //    #region ShowWindowCommand

    /// <summary>
    /// ShowWindow() Commands
    /// </summary>
    internal enum ShowWindowCommand
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        HIDE = 0,

        /// <summary>
        /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        /// </summary>
        SHOWNORMAL = 1,

        NORMAL = 1,

        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        SHOWMINIMIZED = 2,

        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        SHOWMAXIMIZED = 3,

        MAXIMIZE = 3,

        /// <summary>
        /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        /// </summary>
        SHOWNOACTIVATE = 4,

        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        SHOW = 5,

        /// <summary>
        /// Minimizes the specified window and activates the next top-level window in the Z order.
        /// </summary>
        MINIMIZE = 6,

        /// <summary>
        /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
        /// </summary>
        SHOWMINNOACTIVE = 7,

        /// <summary>
        /// Displays the window in its current size and position. This value is similar to SW_SHOW, except the window is not activated.
        /// </summary>
        SHOWNA = 8,

        /// <summary>
        /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
        /// </summary>
        RESTORE = 9,

        /// <summary>
        /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
        /// </summary>
        SHOWDEFAULT = 10,

        /// <summary>
        /// Windows 2000/XP: Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
        /// </summary>
        FORCEMINIMIZE = 11,

        //MAX             = 11,

        // Old ShowWindow() Commands
        //HIDE_WINDOW        = 0,
        //SHOW_OPENWINDOW    = 1,
        //SHOW_ICONWINDOW    = 2,
        //SHOW_FULLSCREEN    = 3,
        //SHOW_OPENNOACTIVATE= 4,
    }

    //    #endregion

    //    #region ShowWindowMessageIdentifiers

    //    /// <summary>
    //    /// Identifiers for the WM_SHOWWINDOW message
    //    /// </summary>
    //    internal enum ShowWindowMessageIdentifiers
    //    {
    //        PARENTCLOSING = 1,
    //        OTHERZOOM = 2,
    //        PARENTOPENING = 3,
    //        OTHERUNZOOM = 4,
    //    }

    //    #endregion

    //    #region GDI charset

    //    /// <summary>
    //    /// Enumerates the available character sets.
    //    /// </summary>
    //    internal enum GdiCharset
    //    {
    //        Ansi = 0,
    //        Default = 1,
    //        Symbol = 2,
    //        ShiftJIS = 128,
    //        Hangeul = 129,
    //        Hangul = 129,
    //        GB2312 = 134,
    //        ChineseBig5 = 136,
    //        OEM = 255,
    //        //#if(WINVER >= 0x0400)
    //        Johab = 130,
    //        Hebrew = 177,
    //        Arabic = 178,
    //        Greek = 161,
    //        Turkish = 162,
    //        Vietnamese = 163,
    //        Thai = 222,
    //        EastEurope = 238,
    //        Russian = 204,
    //        Mac = 77,
    //        Baltic = 186,
    //    }

    //    #endregion

    //    #region MapVirtualKeyType

    internal enum MapVirtualKeyType
    {
        /// <summary>uCode is a virtual-key code and is translated into a scan code. If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned. If there is no translation, the function returns 0.</summary>
        VirtualKeyToScanCode = 0,

        /// <summary>uCode is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys. If there is no translation, the function returns 0.</summary>
        ScanCodeToVirtualKey = 1,

        /// <summary>uCode is a virtual-key code and is translated into an unshifted character value in the low-order word of the return value. Dead keys (diacritics) are indicated by setting the top bit of the return value. If there is no translation, the function returns 0.</summary>
        VirtualKeyToCharacter = 2,

        /// <summary>Windows NT/2000/XP: uCode is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys. If there is no translation, the function returns 0.</summary>
        ScanCodeToVirtualKeyExtended = 3,

        VirtualKeyToScanCodeExtended = 4,
    }

    //    #endregion

    //    #region DwmWindowAttribute

    //    enum DwmWindowAttribute
    //    {
    //        NCRENDERING_ENABLED = 1,
    //        NCRENDERING_POLICY,
    //        TRANSITIONS_FORCEDISABLED,
    //        ALLOW_NCPAINT,
    //        CAPTION_BUTTON_BOUNDS,
    //        NONCLIENT_RTL_LAYOUT,
    //        FORCE_ICONIC_REPRESENTATION,
    //        FLIP3D_POLICY,
    //        EXTENDED_FRAME_BOUNDS,
    //        HAS_ICONIC_BITMAP,
    //        DISALLOW_PEEK,
    //        EXCLUDED_FROM_PEEK,
    //        LAST
    //    }

    //    #endregion

    //    #region ShGetFileIcon

    //    [Flags]
    //    enum ShGetFileIconFlags : Int32
    //    {
    //        /// <summary>get icon</summary>
    //        Icon = 0x000000100,
    //        /// <summary>get display name</summary>
    //        DisplayName = 0x000000200,
    //        /// <summary>get type name</summary>
    //        TypeName = 0x000000400,
    //        /// <summary>get attributes</summary>
    //        Attributes = 0x000000800,
    //        /// <summary>get icon location</summary>
    //        IconLocation = 0x000001000,
    //        /// <summary>return exe type</summary>
    //        ExeType = 0x000002000,
    //        /// <summary>get system icon index</summary>
    //        SysIconIndex = 0x000004000,
    //        /// <summary>put a link overlay on icon</summary>
    //        LinkOverlay = 0x000008000,
    //        /// <summary>show icon in selected state</summary>
    //        Selected = 0x000010000,
    //        /// <summary>get only specified attributes</summary>
    //        Attr_Specified = 0x000020000,
    //        /// <summary>get large icon</summary>
    //        LargeIcon = 0x000000000,
    //        /// <summary>get small icon</summary>
    //        SmallIcon = 0x000000001,
    //        /// <summary>get open icon</summary>
    //        OpenIcon = 0x000000002,
    //        /// <summary>get shell size icon</summary>
    //        ShellIconSize = 0x000000004,
    //        /// <summary>pszPath is a pidl</summary>
    //        PIDL = 0x000000008,
    //        /// <summary>use passed dwFileAttribute</summary>
    //        UseFileAttributes = 0x000000010,
    //        /// <summary>apply the appropriate overlays</summary>
    //        AddOverlays = 0x000000020,
    //        /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
    //        OverlayIndex = 0x000000040,
    //    }

    //    #endregion

    //    #region MonitorFrom

    internal enum MonitorFrom
    {
        Null = 0,
        Primary = 1,
        Nearest = 2,
    }

    //    #endregion

    //    #region CursorName

    internal enum CursorName : int
    {
        Arrow = 32512
    }

    //    #endregion

    //    #region TrackMouseEventFlags

    [Flags]
    internal enum TrackMouseEventFlags : uint
    {
        HOVER = 0x00000001,
        LEAVE = 0x00000002,
        NONCLIENT = 0x00000010,
        QUERY = 0x40000000,
        CANCEL = 0x80000000,
    }

    //    #endregion

    //    #region MouseActivate

    //    enum MouseActivate
    //    {
    //        ACTIVATE = 1,
    //        ACTIVATEANDEAT = 2,
    //        NOACTIVATE = 3,
    //        NOACTIVATEANDEAT = 4,
    //    }

    //    #endregion

    //    #region DeviceNotification

    internal enum DeviceNotification
    {
        WINDOW_HANDLE = 0x00000000,
        SERVICE_HANDLE = 0x00000001,
        ALL_INTERFACE_CLASSES = 0x00000004,
    }

    //    #endregion

    //    #region DeviceBroadcastType

    internal enum DeviceBroadcastType
    {
        OEM = 0,
        VOLUME = 2,
        PORT = 3,
        INTERFACE = 5,
        HANDLE = 6,
    }

    //    #endregion

    //    #endregion

    //    #region --- Callbacks ---

    [SuppressUnmanagedCodeSecurity]
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate IntPtr WindowProcedure(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam);

    //    #region Message

    [StructLayout(LayoutKind.Sequential), CLSCompliant(false)]
    internal struct MSG
    {
        internal IntPtr HWnd;
        internal WindowMessage Message;
        internal IntPtr WParam;
        internal IntPtr LParam;
        internal uint Time;
        internal POINT Point;
        //internal object RefObject;

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (int)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point);
        }
    }

    //    #endregion

    //    #region Point

    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        internal int X;
        internal int Y;

        internal POINT(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        internal Point ToPoint()
        {
            return new Point(X, Y);
        }

        public override string ToString()
        {
            return "Point {" + X.ToString(CultureInfo.InvariantCulture) + ", " + Y.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    //    #endregion

    //    #endregion
}

#pragma warning restore 3019
#pragma warning restore 0649
#pragma warning restore 0169
#pragma warning restore 0414