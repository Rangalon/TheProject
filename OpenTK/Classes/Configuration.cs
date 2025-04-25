#region License

//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
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

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace OpenTK
{
    /// <summary>
    /// Provides information about the underlying OS and runtime.
    /// You must call <c>Toolkit.Init</c> before accessing members
    /// of this class.
    /// </summary>
    public sealed class Configuration
    {
        private static bool runningOnWindows;
        private static bool runningOnMono;
        private static volatile bool initialized;
        private static readonly object InitLock = new object();

        #region Constructors

        private Configuration()
        { }

        #endregion Constructors

        #region Public Methods

        #region public static Boolean RunningOnWindows

        /// <summary>Gets a System.Boolean indicating whether OpenTK is running on a Windows platform.</summary>
        public static bool RunningOnWindows { get { return runningOnWindows; } }

        #endregion public static Boolean RunningOnWindows

        #region RunningOnSDL2

        /// <summary>
        /// Gets a System.Boolean indicating whether OpenTK is running on the SDL2 backend.
        /// </summary>
        public static bool RunningOnSdl2
        {
            get;
            private set;
        }

        #endregion RunningOnSDL2

        #region public static Boolean RunningOnLinux

        #region public static Boolean RunningOnMacOS

        #region public static Boolean RunningOnMono

        /// <summary>
        /// Gets a System.Boolean indicating whether OpenTK is running on the Mono runtime.
        /// </summary>
        public static bool RunningOnMono { get { return runningOnMono; } }

        #endregion public static Boolean RunningOnMono

        #region --- Private Methods ---

        #region private static String DetectUnixKernel()

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct utsname
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string sysname;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string nodename;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string release;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string version;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string machine;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string extraJustInCase;
        }

        /// <summary>
        /// Detects the unix kernel by p/invoking uname (libc).
        /// </summary>
        /// <returns></returns>
        private static string DetectUnixKernel()
        {
            Console.WriteLine("Size: {0}", Marshal.SizeOf(typeof(utsname)).ToString(CultureInfo.InvariantCulture));
            Debug.Flush();
            utsname uts = new utsname();
            uname(out uts);

            Debug.WriteLine("System:");
            Debug.Indent();
            Debug.WriteLine(uts.sysname);
            Debug.WriteLine(uts.nodename);
            Debug.WriteLine(uts.release);
            Debug.WriteLine(uts.version);
            Debug.WriteLine(uts.machine);
            Debug.Unindent();

            return uts.sysname.ToString();
        }

        [DllImport("libc")]
        private static extern void uname(out utsname uname_struct);

        private static bool DetectMono()
        {
            // Detect the Mono runtime (code taken from http://mono.wikia.com/wiki/Detecting_if_program_is_running_in_Mono).
            Type t = Type.GetType("Mono.Runtime");
            return t != null;
        }

        //static Boolean DetectSdl2()
        //{
        //    Boolean supported = false;

        //    // Detect whether SDL2 is supported
        //    // We require:
        //    // - SDL2 version 2.0.0 or higher (previous beta
        //    //   versions are not ABI-compatible)
        //    // - Successful SDL2 initialization (sometimes the
        //    //   binary exists but fails to initialize correctly)
        //    var version = new Platform.SDL2.Version();
        //    try
        //    {
        //        version = Platform.SDL2.SDL.Version;
        //            if (version.Number >= 2000)
        //        {
        //            if (Platform.SDL2.SDL.WasInit(0))
        //            {
        //                supported = true;
        //            }
        //            else
        //            {
        //                // Attempt to initialize SDL2.
        //                var flags =
        //                    Platform.SDL2.SystemFlags.VIDEO |
        //                    Platform.SDL2.SystemFlags.TIMER;

        //                if (Platform.SDL2.SDL.Init(flags) == 0)
        //                {
        //                    supported = true;
        //                }
        //                else
        //                {
        //                    Console.WriteLine("SDL2 init failed with error: {0}",
        //                        Platform.SDL2.SDL.GetError());
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("SDL2 init failed with exception: {0}", e);
        //    }

        //    if (!supported)
        //    {
        //        Console.WriteLine("SDL2 is not supported");
        //    }
        //    else
        //    {
        //        Console.WriteLine("SDL2 is supported. Version is {0}.{1}.{2}",
        //            version.Major, version.Minor, version.Patch);
        //    }

        //    return supported;
        //}

        //static void DetectUnix(out Boolean unix, out Boolean linux, out Boolean macos)
        //{
        //    unix = linux = macos = false;

        //    String kernel_name = DetectUnixKernel();
        //    switch (kernel_name)
        //    {
        //        case null:
        //        case "":
        //            throw new PlatformNotSupportedException(
        //                "Unknown platform. Please file a bug report at http://www.opentk.com");

        //        case "Linux":
        //            linux = unix = true;
        //            break;

        //        case "Darwin":
        //            macos = unix = true;
        //            break;

        //        default:
        //            unix = true;
        //            break;
        //    }
        //}

        private static bool DetectWindows()
        {
            return
                System.Environment.OSVersion.Platform == PlatformID.Win32NT ||
                System.Environment.OSVersion.Platform == PlatformID.Win32S ||
                System.Environment.OSVersion.Platform == PlatformID.Win32Windows ||
                System.Environment.OSVersion.Platform == PlatformID.WinCE;
        }

        //static Boolean DetectX11()
        //{
        //    // Detect whether X is present.
        //    try { return OpenTK.Platform.X11.API.DefaultDisplay != IntPtr.Zero; }
        //    catch { return false; }
        //}

        #endregion private static String DetectUnixKernel()

        #endregion --- Private Methods ---

        #endregion public static Boolean RunningOnMacOS

        #region Internal Methods

        // Detects the underlying OS and runtime.
        internal static void Init(ToolkitOptions options)
        {
            lock (InitLock)
            {
                if (!initialized)
                {
                    runningOnMono = DetectMono();
                    runningOnWindows = DetectWindows();
                    //if (!runningOnWindows)
                    //{
                    //    DetectUnix(out runningOnUnix, out runningOnLinux, out runningOnMacOS);
                    //}

                    //if (options.Backend == PlatformBackend.Default)
                    //{
                    //    RunningOnSdl2 = DetectSdl2();
                    //}

                    //if ((runningOnLinux && !RunningOnSdl2) || options.Backend == PlatformBackend.PreferX11)
                    //{
                    //    runningOnX11 = DetectX11();
                    //}

                    initialized = true;
                }
            }
        }

        #endregion Internal Methods

        #endregion public static Boolean RunningOnLinux

        #endregion Public Methods
    }
}