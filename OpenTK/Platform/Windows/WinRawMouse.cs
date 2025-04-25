#region License

//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

#endregion License

using Math3D;
using Microsoft.Win32;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    //public enum TouchWindowFlag : uint
    //{
    //    None = 0x0,
    //    FineTouch = 0x1,
    //    WantPalm = 0x2
    //}

    /// \internal
    /// <summary>
    /// Contains methods to register for and process mouse WM_INPUT messages.
    /// </summary>
    public class WinRawMouse
    {
        #region "Shared Functions"

        //[DllImport("user32")]
        //public static extern bool RegisterTouchWindow(System.IntPtr hWnd, TouchWindowFlag flags);

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static RegistryKey FindRegistryKey(string name)
        {
            if (name.Length < 4)
                return null;

            // remove the \??\
            name = name.Substring(4);

            string[] split = name.Split('#');
            if (split.Length < 3)
                return null;

            string id_01 = split[0];    // ACPI (Class code)
            string id_02 = split[1];    // PNP0303 (SubClass code)
            string id_03 = split[2];    // 3&13c0b0c5&0 (Protocol code)
            // The final part is the class GUID and is not needed here

            string findme = string.Format(CultureInfo.InvariantCulture,
                @"System\CurrentControlSet\Enum\{0}\{1}\{2}",
                id_01, id_02, id_03);

            RegistryKey regkey = Registry.LocalMachine.OpenSubKey(findme);
            return regkey;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private static string GetDeviceName(RawInputDeviceList dev)
        {
            // get name size
            uint size = 0;
            Functions.GetRawInputDeviceInfo(dev.Device, RawInputDeviceInfoEnum.DEVICENAME, IntPtr.Zero, ref size);

            // get actual name
            IntPtr name_ptr = Marshal.AllocHGlobal((IntPtr)size);
            Functions.GetRawInputDeviceInfo(dev.Device, RawInputDeviceInfoEnum.DEVICENAME, name_ptr, ref size);
            string name = Marshal.PtrToStringAnsi(name_ptr);
            Marshal.FreeHGlobal(name_ptr);

            return name;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        private static void RegisterRawDevice(IntPtr window, string device)
        {
            RawInputDevice[] rid = new RawInputDevice[1];
            // Mouse is 1/2 (page/id). See http://www.microsoft.com/whdc/device/input/HID_HWID.mspx
            rid[0] = new RawInputDevice();

            rid[0].UsagePage = 1;
            rid[0].Usage = 2;
            rid[0].Flags = RawInputDeviceFlags.INPUTSINK;
            rid[0].Target = window;

            if (!Functions.RegisterRawInputDevices(rid, 1, API.RawInputDeviceSize))
            {
                Console.WriteLine("[Warning] Raw input registration failed with error: {0}. Device: {1}",
                    Marshal.GetLastWin32Error(), rid[0].ToString());
            }
            else
            {
                Console.WriteLine("Registered mouse {0}", device);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Shared Functions"

        #region "Variables"

        private readonly Dictionary<ContextHandle, int> rawids = new Dictionary<ContextHandle, int>();
        private readonly List<MouseState> mice = new List<MouseState>();
        private readonly List<string> names = new List<string>();
        private readonly IntPtr Window;
        private readonly object UpdateLock = new object();

        #endregion "Variables"

        #region "Public Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MouseState GetCursorState()
        {
            // For simplicity, get hardware state
            // and simply overwrite its x and y location
            POINT p = new POINT();
            Functions.GetCursorPos(ref p);

            MouseState state = GetState();
            state.Position = new Vec2(p.X, p.Y);
            return state;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MouseState GetState()
        {
            lock (UpdateLock)
            {
                MouseState master = new MouseState();
                foreach (MouseState ms in mice)
                {
                    master.MergeBits(ms);
                }
                return master;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public MouseState GetState(int index)
        {
            lock (UpdateLock)
            {
                if (mice.Count > index)
                    return mice[index];
                else
                    return new MouseState();
            }
        }

        //StreamWriter wtr = new StreamWriter("c:\\temp\\mouse.txt");

        public static string MouseTag = "";

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool ProcessMouseEvent(RawInput rin)
        {
            RawMouse raw = rin.Data.Mouse;
            ContextHandle handle = new ContextHandle(rin.Header.Device);

            MouseState mouse;
            if (!rawids.ContainsKey(handle)) RefreshDevices();

            //System.Windows.Forms.MessageBox.Show("Mouse 111");
            if (mice.Count == 0) return false;

            //MouseTag = String.Format(CultureInfo.InvariantCulture, "Mouse {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}", raw.ButtonFlags, raw.Flags, raw.LastX, raw.LastY, raw.ButtonData, raw.ExtraInformation, raw.RawButtons, rin.Data.HID.Count, rin.Data.HID.SizeHid, rin.Data.HID.RawData);
            //   wtr.Flush();
            //   f.SetValues((ushort)raw.ButtonFlags, (ushort)raw.Flags, raw.LastX, raw.LastY);

            // Note:For some reason, my Microsoft Digital 3000 keyboard reports 0
            // as rin.Header.Device for the "zoom-in/zoom-out" buttons.
            // That's problematic, because no device has a "0" id.
            // As a workaround, we'll add those buttons to the first device (if any).
            //System.Windows.Forms.MessageBox.Show("Mouse 1111");
            int mouse_handle = rawids.ContainsKey(handle) ? rawids[handle] : 0;
            //System.Windows.Forms.MessageBox.Show("Mouse 11111");
            mouse = mice[mouse_handle];

            //System.Windows.Forms.MessageBox.Show("Mouse 111111");

            // Set and release capture of the mouse to fix http://www.opentk.com/node/2133, Patch by Artfunkel
            if ((raw.ButtonFlags & RawInputMouseState.LEFT_BUTTON_DOWN) != 0)
            {
                mouse.EnableBit((int)MouseButton.Left);
                Functions.SetCapture(Window);
            }
            if ((raw.ButtonFlags & RawInputMouseState.LEFT_BUTTON_UP) != 0)
            {
                mouse.DisableBit((int)MouseButton.Left);
                Functions.ReleaseCapture();
            }
            if ((raw.ButtonFlags & RawInputMouseState.RIGHT_BUTTON_DOWN) != 0)
            {
                mouse.EnableBit((int)MouseButton.Right);
                Functions.SetCapture(Window);
            }
            if ((raw.ButtonFlags & RawInputMouseState.RIGHT_BUTTON_UP) != 0)
            {
                mouse.DisableBit((int)MouseButton.Right);
                Functions.ReleaseCapture();
            }
            if ((raw.ButtonFlags & RawInputMouseState.MIDDLE_BUTTON_DOWN) != 0)
            {
                mouse.EnableBit((int)MouseButton.Middle);
                Functions.SetCapture(Window);
            }
            if ((raw.ButtonFlags & RawInputMouseState.MIDDLE_BUTTON_UP) != 0)
            {
                mouse.DisableBit((int)MouseButton.Middle);
                Functions.ReleaseCapture();
            }
            if ((raw.ButtonFlags & RawInputMouseState.BUTTON_4_DOWN) != 0)
            {
                mouse.EnableBit((int)MouseButton.Button1);
                Functions.SetCapture(Window);
            }
            if ((raw.ButtonFlags & RawInputMouseState.BUTTON_4_UP) != 0)
            {
                mouse.DisableBit((int)MouseButton.Button1);
                Functions.ReleaseCapture();
            }
            if ((raw.ButtonFlags & RawInputMouseState.BUTTON_5_DOWN) != 0)
            {
                mouse.EnableBit((int)MouseButton.Button2);
                Functions.SetCapture(Window);
            }
            if ((raw.ButtonFlags & RawInputMouseState.BUTTON_5_UP) != 0)
            {
                mouse.DisableBit((int)MouseButton.Button2);
                Functions.ReleaseCapture();
            }

            if ((raw.Flags & RawMouseFlags.MOUSE_MOVE_ABSOLUTE) != 0)
            {
                mouse.Position = new Vec2(raw.LastX, raw.LastY);
            }
            else
            {   // Seems like MOUSE_MOVE_RELATIVE is the default, unless otherwise noted.
                mouse.Position += new Vec2(raw.LastX, raw.LastY);
            }

            lock (UpdateLock)
            {
                mice[mouse_handle] = mouse;
                return true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void RefreshDevices()
        {
            //if (OpenTK.Input.Touch.GetTouchesState().IsConnected)
            //{
            //    Console.WriteLine("Touch State Connected");
            //    //    //System.Windows.Forms.MessageBox.Show("Registration...");
            //    //    RegisterTouchWindow(Window, TouchWindowFlag.None);
            //    WinRawMouse.MultiTouch = true;
            //    //    //System.Windows.Forms.MessageBox.Show(b.ToString());
            //    //    //System.Windows.Forms.MessageBox.Show("Registered!");
            //}

            lock (UpdateLock)
            {
                // Mark all devices as disconnected. We will check which of those
                // are connected later on.
                for (int i = 0; i < mice.Count; i++)
                {
                    MouseState state = mice[i];
                    state.IsConnected = false;
                    mice[i] = state;
                }

                int count = WinRawInput.DeviceCount;
                RawInputDeviceList[] ridl = new RawInputDeviceList[count];
                for (int i = 0; i < count; i++)
                    ridl[i] = new RawInputDeviceList();
                Functions.GetRawInputDeviceList(ridl, ref count, API.RawInputDeviceListSize);

                // Discover mouse devices
                foreach (RawInputDeviceList dev in ridl)
                {
                    ContextHandle id = new ContextHandle(dev.Device);
                    if (rawids.ContainsKey(id))
                    {
                        // Device already registered, mark as connected
                        MouseState state = mice[rawids[id]];
                        state.IsConnected = true;
                        mice[rawids[id]] = state;
                        continue;
                    }

                    // Unregistered device, find what it is
                    string name = GetDeviceName(dev);
                    if (name.ToUpperInvariant().Contains("ROOT"))
                    {
                        // This is a terminal services device, skip it.
                        continue;
                    }
                    else if (dev.Type == RawInputDeviceType.MOUSE || dev.Type == RawInputDeviceType.HID)
                    {
                        // This is a mouse or a USB mouse device. In the latter case, discover if it really is a
                        // mouse device by qeurying the registry.
                        RegistryKey regkey = FindRegistryKey(name);
                        if (regkey == null)
                            continue;

                        string deviceDesc = (string)regkey.GetValue("DeviceDesc");
                        string deviceClass = (string)regkey.GetValue("Class") as string;
                        if (deviceClass == null)
                        {
                            // Added to address OpenTK issue 3198 with mouse on Windows 8
                            string deviceClassGUID = (string)regkey.GetValue("ClassGUID");
                            RegistryKey classGUIDKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\" + deviceClassGUID);
                            deviceClass = classGUIDKey != null ? (string)classGUIDKey.GetValue("Class") : string.Empty;
                        }

                        // deviceDesc remained null on a new Win7 system - not sure why.
                        // Since the description is not vital information, use a dummy description
                        // when that happens.
                        if (string.IsNullOrEmpty(deviceDesc))
                            deviceDesc = "Windows Mouse " + mice.Count;
                        else
                            deviceDesc = deviceDesc.Substring(deviceDesc.LastIndexOf(';') + 1);

                        if (!string.IsNullOrEmpty(deviceClass) && deviceClass.ToUpperInvariant().Equals("MOUSE"))
                        {
                            if (!rawids.ContainsKey(new ContextHandle(dev.Device)))
                            {
                                // Register the device:
                                RawInputDeviceInfo info = new RawInputDeviceInfo();
                                int devInfoSize = API.RawInputDeviceInfoSize;
                                Functions.GetRawInputDeviceInfo(dev.Device, RawInputDeviceInfoEnum.DEVICEINFO, info, ref devInfoSize);

                                RegisterRawDevice(Window, deviceDesc);
                                MouseState state = new MouseState();
                                state.IsConnected = true;
                                mice.Add(state);
                                names.Add(deviceDesc);
                                rawids.Add(new ContextHandle(dev.Device), mice.Count - 1);
                            }
                        }
                        else if (!string.IsNullOrEmpty(deviceClass) && deviceDesc.ToUpperInvariant().Contains("TOUCH"))
                        {
                            //if (!rawids.ContainsKey(new ContextHandle(dev.Device)))
                            //{
                            //    WinRawMouse.MultiTouch = true;
                            //    // Register the device:
                            //    RawInputDeviceInfo info = new RawInputDeviceInfo();
                            //    Int32 devInfoSize = API.RawInputDeviceInfoSize;
                            //    Functions.GetRawInputDeviceInfo(dev.Device, RawInputDeviceInfoEnum.DEVICEINFO, info, ref devInfoSize);

                            //    RegisterRawDevice(Window, deviceDesc);
                            //    MouseState state = new MouseState();
                            //    state.IsConnected = true;
                            //    mice.Add(state);
                            //    names.Add(deviceDesc);
                            //    rawids.Add(new ContextHandle(dev.Device), mice.Count - 1);
                            //}
                        }
                    }
                    //else
                    //{
                    //    ////System.Windows.Forms.MessageBox.Show(name + " - " + dev.Type.ToString());
                    //}
                }
                ////System.Windows.Forms.MessageBox.Show(mice.Count.ToString());
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetPosition(double x, double y)
        {
            Functions.SetCursorPos((int)x, (int)y);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //FMouse f;
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        static WinRawMouse()
        {
        }

        public WinRawMouse(IntPtr window)
        {
            //f = new FMouse();
            //f.Show();

            Debug.WriteLine("Using WinRawMouse.");
            Debug.Indent();

            if (window == IntPtr.Zero)
                throw new ArgumentNullException("window");

            Window = window;
            RefreshDevices();

            Debug.Unindent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Public Functions"
    }
}