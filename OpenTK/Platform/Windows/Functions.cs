using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OpenTK.Platform.Windows
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TOUCHINPUT
    {
        public int x;
        public int y;
        private readonly System.IntPtr hSource;
        public int dwID;
        public int dwFlags;
        public int dwMask;
        public int dwTime;
        private readonly System.IntPtr dwExtraInfo;
        public int cxContact;
        public int cyContact;
    }

    internal static class Functions
    {
        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern bool AdjustWindowRect([In, Out] ref Win32Rectangle lpRect, WindowStyle dwStyle, bool bMenu);

        [DllImport("user32.dll", EntryPoint = "AdjustWindowRectEx", CallingConvention = CallingConvention.StdCall, SetLastError = true), SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustWindowRectEx(ref Win32Rectangle lpRect, WindowStyle dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu, ExtendedWindowStyle dwExStyle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int ChangeDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, DeviceMode lpDevMode, IntPtr hwnd, ChangeDisplaySettingsEnum dwflags, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(ref OpenTK.Platform.Windows.Win32Rectangle rcClip);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(IntPtr rcClip);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateIconIndirect(ref IconInfo iconInfo);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CreateWindowEx(ExtendedWindowStyle ExStyle, IntPtr ClassAtom, IntPtr WindowName, WindowStyle Style, int X, int Y, int Width, int Height, IntPtr HandleToParentWindow, IntPtr Menu, IntPtr Instance, IntPtr Param);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public extern static IntPtr DefWindowProc(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        internal static extern int DescribePixelFormat(IntPtr deviceContext, int pixel, int pfdSize, ref PixelFormatDescriptor pixelFormat);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(IntPtr windowHandle);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        internal static extern IntPtr DispatchMessage(ref MSG msg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDisplayDevices([MarshalAs(UnmanagedType.LPTStr)] string lpDevice, int iDevNum, [In, Out] WindowsDisplayDevice lpDisplayDevice, int dwFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, DisplayModeSettingsEnum iModeNum, [In, Out] DeviceMode lpDevMode, int dwFlags);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool EnumDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, int iModeNum, [In, Out] DeviceMode lpDevMode, int dwFlags);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal extern static bool GetClientRect(IntPtr windowHandle, out Win32Rectangle clientRectangle);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern bool GetCursorPos(ref POINT point);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern bool GetTouchInputInfo(ref POINT point);

        [DllImport("user32")]
        public static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);

        public enum TouchWindowFlag : uint
        {
            None = 0x0,
            FineTouch = 0x1,
            WantPalm = 0x2
        }

        [DllImport("user32")]
        public static extern bool RegisterTouchWindow(System.IntPtr hWnd, TouchWindowFlag flags);

        [DllImport("user32")]
        public static extern bool GetTouchInputInfo(System.IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

        [DllImport("user32")]
        public static extern void CloseTouchInputHandle(System.IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, DeviceCaps nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetIconInfo(IntPtr hIcon, out IconInfo pIconInfo);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        internal static extern int GetMessage(ref MSG msg, IntPtr windowHandle, int messageFilterMin, int messageFilterMax);

        [DllImport("User32.dll")]
        internal static extern int GetMessageTime();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        [DllImport("user32", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
        unsafe internal static extern int GetMouseMovePointsEx(uint cbSize, MouseMovePoint* pointsIn, MouseMovePoint* pointsBufferOut, int nBufPoints, uint resolution);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(IntPtr handle, string funcname);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(IntPtr handle, IntPtr funcname);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr RawInput, GetRawInputDataEnum Command, [Out] IntPtr Data, [In, Out] ref int Size, int SizeHeader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetRawInputData(IntPtr RawInput, GetRawInputDataEnum Command, [Out] out RawInput Data, [In, Out] ref int Size, int SizeHeader);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In, Out] IntPtr Data, [In, Out] ref uint Size);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetRawInputDeviceInfo(IntPtr Device, [MarshalAs(UnmanagedType.U4)] RawInputDeviceInfoEnum Command, [In, Out] RawInputDeviceInfo Data, [In, Out] ref int Size);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetRawInputDeviceList([In, Out] RawInputDeviceList[] RawInputDeviceList, [In, Out] ref int NumDevices, int Size);

        internal static UIntPtr GetWindowLong(IntPtr handle, GetWindowLongOffsets index)
        {
            if (IntPtr.Size == 4)
                return (UIntPtr)GetWindowLongInternal(handle, index);

            return GetWindowLongPtrInternal(handle, index);
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongPtr")]
        private static extern UIntPtr GetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr), In, Out] StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr intPtr);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool KillTimer(IntPtr hWnd, UIntPtr uIDEvent);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        public static IntPtr LoadCursor(CursorName lpCursorName)
        {
            return LoadCursor(IntPtr.Zero, new IntPtr((int)lpCursorName));
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string dllName);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint MapVirtualKey(VirtualKeys vkey, MapVirtualKeyType uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFrom dwFlags);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, PeekMessageFlags flags);

        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessage(
            IntPtr hWnd,
            WindowMessage Msg,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern ushort RegisterClassEx(ref ExtendedWindowClass window_class);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient,
            IntPtr NotificationFilter, DeviceNotification Flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterRawInputDevices(
            RawInputDevice[] RawInputDevices,
            int NumDevices,
            int Size
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReleaseDC(IntPtr hwnd, IntPtr DC);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        //internal static extern Boolean ScreenToClient(HWND hWnd, ref POINT point);
        internal static extern bool ScreenToClient(IntPtr hWnd, ref Point point);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr SetCapture(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll")]
        internal static extern void SetLastError(int dwErrCode);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetParent(IntPtr child, IntPtr newParent);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetPixelFormat(IntPtr dc, int format, ref PixelFormatDescriptor pfd);

        [DllImport("user32.dll")]
        internal static extern bool SetProcessDPIAware();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, Functions.TimerProc lpTimerFunc);

        internal static IntPtr SetWindowLong(IntPtr handle, GetWindowLongOffsets item, IntPtr newValue)
        {
            // SetWindowPos defines its error condition as an IntPtr.Zero retval and a non-0 GetLastError.
            // We need to SetLastError(0) to ensure we are not detecting on older error condition (from another function).

            IntPtr retval = IntPtr.Zero;
            SetLastError(0);

            if (IntPtr.Size == 4)
                retval = new IntPtr(SetWindowLongInternal(handle, item, newValue.ToInt32()));
            else
                retval = SetWindowLongPtrInternal(handle, item, newValue);

            if (retval == IntPtr.Zero)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                    throw new PlatformException(string.Format(CultureInfo.InvariantCulture, "Failed to modify window border. Error: {0}", error));
            }

            return retval;
        }

        internal static IntPtr SetWindowLong(IntPtr handle, WindowProcedure newValue)
        {
            return SetWindowLong(handle, GetWindowLongOffsets.WNDPROC, Marshal.GetFunctionPointerForDelegate(newValue));
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLongInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, int dwNewLong);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtrInternal(IntPtr hWnd, GetWindowLongOffsets nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(
            IntPtr handle,
            IntPtr insertAfter,
            int x, int y, int cx, int cy,
            SetWindowPosFlags flags
        );

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowCursor(bool show);

        [DllImport("user32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern bool ShowWindow(IntPtr hWnd, ShowWindowCommand nCmdShow);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SwapBuffers(IntPtr dc);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool TrackMouseEvent(ref TrackMouseEventStructure lpEventTrack);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll")]
        internal static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int UnregisterClass(IntPtr className, IntPtr instance);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterDeviceNotification(IntPtr Handle);

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate void TimerProc(IntPtr hwnd, WindowMessage uMsg, UIntPtr idEvent, int dwTime);
    }
}