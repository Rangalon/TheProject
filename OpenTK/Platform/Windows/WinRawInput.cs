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

using OpenTK.Input;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Windows
{
    //public class DigitizerData
    //{
    //    public byte usb_report_id;
    //    public TouchData[] touch =new TouchData[]{new TouchData(),new TouchData(),new TouchData(),new TouchData(),new TouchData(),new TouchData(),new TouchData(),new TouchData()};
    //    public byte active_touch_count;
    //}

    //public class TouchData
    //{
    //    // the good news is that bit packing makes reinterpreting the data fairly simple,
    //    // even in crazy cases where the data is half-aligned such as our interleaved
    //    // X and Y position
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte status = 1;
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte in_range = 1;
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte padding = 1;
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte touch_id = 5;
    //    public byte x_position_lsbyte;
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte x_position_msbyte = 4;
    //    //C++ TO C# CONVERTER TODO TASK: C# does not allow bit fields:
    //    public byte y_position_lsbyte = 4;
    //    public byte y_position_msbyte;
    //    // the exception is when data is stored across bytes, especially with
    //    // partial bits involed. in these cases, helper functions make things
    //    // easy to work with
    //    // helper function to get the X value from the multi-byte parameter
    //    public int X()
    //    {
    //        return (((x_position_msbyte & 0x0F) << 8) + x_position_lsbyte);
    //    }
    //    // helper function to get the Y value from the multi-byte parameter
    //    public int Y()
    //    {
    //        return (((y_position_msbyte & 0x0F) << 4) + y_position_lsbyte);
    //    }
    //}

    internal sealed class WinRawInput : WinInputBase
    {
        #region Private Fields

        private static readonly Guid DeviceInterfaceHid = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

        // Input event data.
        private static RawInput data = new RawInput();

        private IntPtr DevNotifyHandle;
        private MappedGamePadDriver gamepad_driver;
        private WinMMJoystick joystick_driver;
        private WinRawKeyboard keyboard_driver;
        private WinRawMouse mouse_driver;
        private WinRawTouch touch_driver;

        #endregion Private Fields

        #region Public Constructors

        public WinRawInput()
            : base()
        {
            Debug.WriteLine("Using WinRawInput.");
        }

        #endregion Public Constructors

        #region Public Properties

        public static int DeviceCount
        {
            get
            {
                int deviceCount = 0;
                Functions.GetRawInputDeviceList(null, ref deviceCount, API.RawInputDeviceListSize);
                return deviceCount;
            }
        }

        public override MappedGamePadDriver GamePadDriver
        {
            get { return gamepad_driver; }
        }

        public override WinMMJoystick JoystickDriver
        {
            get { return joystick_driver; }
        }

        public override WinRawKeyboard KeyboardDriver
        {
            get { return keyboard_driver; }
        }

        public override WinRawMouse MouseDriver
        {
            get { return mouse_driver; }
        }

        public override WinRawTouch TouchDriver
        {
            get { return touch_driver; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void CreateDrivers()
        {
            keyboard_driver = new WinRawKeyboard(Parent.Handle);
            mouse_driver = new WinRawMouse(Parent.Handle);
            touch_driver = new WinRawTouch(Parent.Handle);
            joystick_driver = new WinMMJoystick();
            //try
            //{
            //    gamepad_driver = new XInputJoystick();
            //}
            //catch (Exception)
            //{
                Console.WriteLine("[Win] XInput driver not supported, falling back to WinMM");
                gamepad_driver = new MappedGamePadDriver();
            //}

            DevNotifyHandle = RegisterForDeviceNotifications(Parent);
        }

        protected override void Dispose(bool manual)
        {
            if (!Disposed)
            {
                Functions.UnregisterDeviceNotification(DevNotifyHandle);
                base.Dispose(manual);
            }
        }

        // Processes the input Windows Message, routing the buffer to the correct Keyboard, Mouse or HID.
        protected override IntPtr WindowProcedure(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            ////System.Windows.Forms.MessageBox.Show("WindowProcedure");

            switch (message)
            {
                case WindowMessage.INPUT:
                    int size = 0;
                    // Get the size of the input buffer
                    Functions.GetRawInputData(lParam, GetRawInputDataEnum.INPUT, IntPtr.Zero, ref size, API.RawInputHeaderSize);

                    // Read the actual raw input structure
                    if (size == Functions.GetRawInputData(lParam, GetRawInputDataEnum.INPUT, out data, ref size, API.RawInputHeaderSize))
                    {
                        //Console.WriteLine( GetMessageExtraInfo().ToString() + " " + data.Data.HID.Count.ToString() + " " + data.Header.Type.ToString() + " " + data.Data.HID.RawData.ToString() + " " + data.Data.Mouse.LastX.ToString());
                        //for (uint index = 0; index < data.Data.HID.Count; index++)
                        //{
                        //    DigitizerData result = new DigitizerData((DigitizerData)data.Data.HID.RawData[data.Data.HID.SizeHid * index]);

                        //}
                        //switch (data.Data.HID.Count)
                        //{
                        //    //case 1: break;
                        //    default:
                        //        WinRawMouse.MouseTag = "HID " + data.Data.HID.Count.ToString(); break;
                        //}
                        switch (data.Header.Type)
                        {
                            case RawInputDeviceType.KEYBOARD:
                                if (((WinRawKeyboard)KeyboardDriver).ProcessKeyboardEvent(data))
                                    return IntPtr.Zero;
                                break;

                            case RawInputDeviceType.MOUSE:
                                //System.Windows.Forms.MessageBox.Show("Processing Mouse");
                                if (((WinRawMouse)MouseDriver).ProcessMouseEvent(data))
                                    return IntPtr.Zero;
                                break;

                            case RawInputDeviceType.HID:
                                //Console.WriteLine(GetMessageExtraInfo().ToString() + " " + data.Data.HID.Count.ToString() + " " + data.Header.Type.ToString() + " " + data.Data.HID.RawData.ToString() + " " + data.Data.Mouse.LastX.ToString());
                                //System.Windows.Forms.MessageBox.Show("Processing HID");
                                if (((WinRawTouch)TouchDriver).ProcessTouchEvent(data))
                                    return IntPtr.Zero;
                                break;

                            default:
                                System.Windows.Forms.MessageBox.Show("Processing");
                                //if (((WinRawTouch)TouchDriver).ProcessTouchEvent(data))
                                //    return IntPtr.Zero;
                                break;
                        }
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Wrong size");
                    break;

                case WindowMessage.DEVICECHANGE:
                    ((WinRawKeyboard)KeyboardDriver).RefreshDevices();
                    ((WinRawMouse)MouseDriver).RefreshDevices();
                    ((WinRawTouch)TouchDriver).RefreshDevices();
                    ((WinMMJoystick)JoystickDriver).RefreshDevices();
                    break;

                default:
                    //Console.WriteLine(message);
                    //System.Windows.Forms.MessageBox.Show(message.ToString());
                    break;
            }
            return base.WindowProcedure(handle, message, wParam, lParam);
        }

        #endregion Protected Methods

        #region Private Methods

        [DllImport("user32.dll")]
        private static extern uint GetMessageExtraInfo();

        private static IntPtr RegisterForDeviceNotifications(WinWindowInfo parent)
        {
            IntPtr dev_notify_handle;
            BroadcastDeviceInterface bdi = new BroadcastDeviceInterface();
            bdi.Size = BlittableValueType.StrideOf(bdi);
            bdi.DeviceType = DeviceBroadcastType.INTERFACE;
            bdi.ClassGuid = DeviceInterfaceHid;
            unsafe
            {
                dev_notify_handle = Functions.RegisterDeviceNotification(parent.Handle,
                    new IntPtr((void*)&bdi), DeviceNotification.WINDOW_HANDLE);
            }
            if (dev_notify_handle == IntPtr.Zero)
                Console.WriteLine("[Warning] Failed to register for device notifications. Error: {0}", Marshal.GetLastWin32Error());

            return dev_notify_handle;
        }

        #endregion Private Methods
    }
}