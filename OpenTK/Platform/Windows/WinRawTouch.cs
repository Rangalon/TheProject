using Math3D;
using Microsoft.Win32;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenTK.Platform.Windows
{
    public class WinRawTouch    
    {
        #region "Shared Functions"

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
            //System.Windows.Forms.MessageBox.Show("Registering");
            RawInputDevice[] rid = new RawInputDevice[1];

            rid[0] = new RawInputDevice();

            //rid[0].UsagePage = 1;
            //rid[0].Usage = 2;
            //rid[0].Flags = RawInputDeviceFlags.INPUTSINK;
            //rid[0].Target = window;

            rid[0].UsagePage = 0x0d;
            rid[0].Usage = 0;
            rid[0].Flags = RawInputDeviceFlags.INPUTSINK | RawInputDeviceFlags.PAGEONLY;
            rid[0].Target = window;

            if (!Functions.RegisterRawInputDevices(rid, 1, API.RawInputDeviceSize))
            {
                //System.Windows.Forms.MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "[Warning] Raw input registration failed with error: {0}. Device: {1}",                    Marshal.GetLastWin32Error(), rid[0].ToString()));
                Console.WriteLine("[Warning] Raw input registration failed with error: {0}. Device: {1}", Marshal.GetLastWin32Error(), rid[0].ToString());
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show(String.Format(CultureInfo.InvariantCulture, "Registered touch {0}", device));
                Console.WriteLine("Registered touch {0}", device);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Shared Functions"

        #region "Variables"

        private readonly Dictionary<ContextHandle, int> rawids = new Dictionary<ContextHandle, int>();
        private readonly List<TouchState> tich = new List<TouchState>();
        private readonly List<string> names = new List<string>();
        private readonly IntPtr Window;
        private readonly object UpdateLock = new object();

        #endregion "Variables"

        #region "Public Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public TouchState GetState()
        {
            lock (UpdateLock)
            {
                return master;
            }
        }

        public TouchState GetTouchesState()
        {
            lock (UpdateLock)
            {
                return master;
            }
        }

        private TouchState master = new TouchState();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool ProcessTouchEvent(RawInput rin)
        {
            RawHID raw = rin.Data.HID;
            ContextHandle handle = new ContextHandle(rin.Header.Device);
            //
            TouchState touch = new TouchState() { Position1 = master.Position1, Position2 = master.Position2 };
            if (!rawids.ContainsKey(handle)) { RefreshDevices(); }
            //
            if (tich.Count == 0) return false;
            //
            Vec2? v;
            if (rin.Data.HID.b2 % 2 == 1) v = new Vec2((double)(rin.Data.HID.b41 + 256 * rin.Data.HID.b42) * Screen.PrimaryScreen.Bounds.Width / 6830, (double)(rin.Data.HID.b51 + 256 * rin.Data.HID.b52) * Screen.PrimaryScreen.Bounds.Height / 3840);
            else v = null;
            //if (v.HasValue)
            //{
            //    Console.WriteLine(string.Format(" {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}", rin.Data.HID.b1, rin.Data.HID.b2, rin.Data.HID.b3, rin.Data.HID.b41, rin.Data.HID.b42, rin.Data.HID.b43, rin.Data.HID.b44, rin.Data.HID.b51, rin.Data.HID.b52, rin.Data.HID.b53, rin.Data.HID.b54, v.Value));
            //}
            //foreach (byte b in new byte[] { rin.Data.HID.b1, rin.Data.HID.b2, rin.Data.HID.b3, rin.Data.HID.b41, rin.Data.HID.b42, rin.Data.HID.b43, rin.Data.HID.b44, rin.Data.HID.b51, rin.Data.HID.b52, rin.Data.HID.b53, rin.Data.HID.b54 })
            //{
            //    byte bb = b;
            //    for (int i = 0; i < 8; i++)
            //    {
            //        Console.Write((bb % 2).ToString() + " ");
            //        bb /= 2;
            //    }
            //    Console.Write("\n");
            //}

            if (rin.Data.HID.b3 % 2 == 1) touch.Position2 = v;
            else touch.Position1 = v;
            //
            lock (UpdateLock)
            {
                master = touch;
                return true;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void RefreshDevices()
        {
            lock (UpdateLock)
            {
                // Mark all devices as disconnected. We will check which of those
                // are connected later on.
                for (int i = 0; i < tich.Count; i++)
                {
                    TouchState state = tich[i];
                    state.IsConnected = false;
                    tich[i] = state;
                }

                int count = WinRawInput.DeviceCount;
                RawInputDeviceList[] ridl = new RawInputDeviceList[count];
                for (int i = 0; i < count; i++)
                    ridl[i] = new RawInputDeviceList();
                Functions.GetRawInputDeviceList(ridl, ref count, API.RawInputDeviceListSize);

                // Discover touch devices
                foreach (RawInputDeviceList dev in ridl)
                {
                    ContextHandle id = new ContextHandle(dev.Device);
                    if (rawids.ContainsKey(id))
                    {
                        // Device already registered, mark as connected
                        TouchState state = tich[rawids[id]];
                        state.IsConnected = true;
                        tich[rawids[id]] = state;
                        continue;
                    }

                    // Unregistered device, find what it is
                    string name = GetDeviceName(dev);
                    if (name.ToUpperInvariant().Contains("ROOT"))
                    {
                        // This is a terminal services device, skip it.
                        continue;
                    }
                    else if (dev.Type == RawInputDeviceType.HID)
                    {
                        // This is a touch or a USB touch device. In the latter case, discover if it really is a
                        // touch device by qeurying the registry.
                        RegistryKey regkey = FindRegistryKey(name);
                        if (regkey == null)
                            continue;

                        string deviceDesc = (string)regkey.GetValue("DeviceDesc");
                        string deviceClass = (string)regkey.GetValue("Class") as string;
                        if (deviceClass == null)
                        {
                            // Added to address OpenTK issue 3198 with touch on Windows 8
                            string deviceClassGUID = (string)regkey.GetValue("ClassGUID");
                            RegistryKey classGUIDKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\" + deviceClassGUID);
                            deviceClass = classGUIDKey != null ? (string)classGUIDKey.GetValue("Class") : string.Empty;
                        }

                        // deviceDesc remained null on a new Win7 system - not sure why.
                        // Since the description is not vital information, use a dummy description
                        // when that happens.
                        if (string.IsNullOrEmpty(deviceDesc))
                            deviceDesc = "Windows Touch " + tich.Count;
                        else
                            deviceDesc = deviceDesc.Substring(deviceDesc.LastIndexOf(';') + 1);

                        if (!string.IsNullOrEmpty(deviceDesc) && deviceDesc.ToUpperInvariant().Contains("TOUCH SCREEN"))
                        {
                            //if (!String.IsNullOrEmpty(deviceClass)) System.Windows.Forms.MessageBox.Show(deviceClass);
                            //if (!String.IsNullOrEmpty(deviceDesc)) System.Windows.Forms.MessageBox.Show(deviceDesc);
                            if (!rawids.ContainsKey(new ContextHandle(dev.Device)))
                            {
                                // Register the device:
                                //if (!String.IsNullOrEmpty(deviceClass)) System.Windows.Forms.MessageBox.Show(deviceClass);
                                //if (!String.IsNullOrEmpty(deviceDesc)) System.Windows.Forms.MessageBox.Show(deviceDesc);

                                RawInputDeviceInfo info = new RawInputDeviceInfo();
                                int devInfoSize = API.RawInputDeviceInfoSize;
                                Functions.GetRawInputDeviceInfo(dev.Device, RawInputDeviceInfoEnum.DEVICEINFO, info, ref devInfoSize);

                                RegisterRawDevice(Window, deviceDesc);
                                TouchState state = new TouchState();
                                state.IsConnected = true;
                                tich.Add(state);
                                names.Add(deviceDesc);
                                rawids.Add(new ContextHandle(dev.Device), tich.Count - 1);
                            }
                        }
                    }
                    else
                    {
                        ////System.Windows.Forms.MessageBox.Show(name + " - " + dev.Type.ToString());
                    }
                }
                ////System.Windows.Forms.MessageBox.Show(tich.Count.ToString());
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public WinRawTouch(IntPtr window)
        {
            Debug.WriteLine("Using WinRawTouch.");
            Debug.Indent();

            if (window == IntPtr.Zero)
                throw new ArgumentNullException("window");

            Window = window;
            RefreshDevices();

            Debug.Unindent();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Public Functions"

        #region "Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerable<TouchEventArgs> DecodeMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, float dpiX, float dpiY)
        {
            // More than one touchinput may be associated with a touch message,
            // so an array is needed to get all event information.
            int inputCount = (ushort)(wParam.ToInt32() & 0xffff); // Number of touch inputs, actual per-contact messages

            TOUCHINPUT[] inputs; // Array of TOUCHINPUT structures
            inputs = new TOUCHINPUT[inputCount]; // Allocate the storage for the parameters of the per-contact messages
            try
            {
                // Unpack message parameters into the array of TOUCHINPUT structures, each
                // representing a message for one single contact.
                if (!Functions.GetTouchInputInfo(lParam, inputCount, inputs, Marshal.SizeOf(inputs[0])))
                {
                    // Get touch info failed.
                    throw new Exception("Error calling GetTouchInputInfo API");
                }

                // For each contact, dispatch the message to the appropriate message
                // handler.
                // Note that for WM_TOUCHDOWN you can get down & move notifications
                // and for WM_TOUCHUP you can get up & move notifications
                // WM_TOUCHMOVE will only contain move notifications
                // and up & down notifications will never come in the same message
                for (int i = 0; i < inputCount; i++)
                {
                    TouchEventArgs touchEventArgs = new TouchEventArgs();//HWndWrapper, dpiX, dpiY, ref inputs[i]);
                    yield return touchEventArgs;
                }
            }
            finally
            {
                Functions.CloseTouchInputHandle(lParam);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Functions"
    }
}