#region License

//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 Stefanos Apostolopoulos
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
using System.Runtime.InteropServices;
using System.Security;

namespace OpenTK.Platform.Windows
{
#pragma warning disable 3019
#pragma warning disable 1591

    partial class Wgl
    {
        static Wgl()
        {
            EntryPointNames = new string[]
            {
                "wglCreateContextAttribsARB",
                "wglGetExtensionsStringARB",
                "wglGetPixelFormatAttribivARB",
                "wglGetPixelFormatAttribfvARB",
                "wglChoosePixelFormatARB",
                "wglMakeContextCurrentARB",
                "wglGetCurrentReadDCARB",
                "wglCreatePbufferARB",
                "wglGetPbufferDCARB",
                "wglReleasePbufferDCARB",
                "wglDestroyPbufferARB",
                "wglQueryPbufferARB",
                "wglBindTexImageARB",
                "wglReleaseTexImageARB",
                "wglSetPbufferAttribARB",
                "wglGetExtensionsStringEXT",
                "wglSwapIntervalEXT",
                "wglGetSwapIntervalEXT",
            };
            EntryPoints = new IntPtr[EntryPointNames.Length];
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglCreateContext", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr CreateContext(IntPtr hDc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglDeleteContext", ExactSpelling = true, SetLastError = true)]
        internal extern static bool DeleteContext(IntPtr oldContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglGetCurrentContext", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetCurrentContext();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglMakeCurrent", ExactSpelling = true, SetLastError = true)]
        internal extern static bool MakeCurrent(IntPtr hDc, IntPtr newContext);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglChoosePixelFormat", ExactSpelling = true, SetLastError = true)]
        internal extern static unsafe int ChoosePixelFormat(IntPtr hDc, ref PixelFormatDescriptor pPfd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglDescribePixelFormat", ExactSpelling = true, SetLastError = true)]
        internal extern static unsafe int DescribePixelFormat(IntPtr hdc, int ipfd, int cjpfd, ref PixelFormatDescriptor ppfd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglGetCurrentDC", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetCurrentDC();

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetProcAddress(string lpszProc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
        internal extern static IntPtr GetProcAddress(IntPtr lpszProc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglGetPixelFormat", ExactSpelling = true, SetLastError = true)]
        internal extern static int GetPixelFormat(IntPtr hdc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglSetPixelFormat", ExactSpelling = true, SetLastError = true)]
        internal extern static bool SetPixelFormat(IntPtr hdc, int ipfd, ref PixelFormatDescriptor ppfd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglSwapBuffers", ExactSpelling = true, SetLastError = true)]
        internal extern static bool SwapBuffers(IntPtr hdc);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Wgl.Library, EntryPoint = "wglShareLists", ExactSpelling = true, SetLastError = true)]
        internal extern static bool ShareLists(IntPtr hrcSrvShare, IntPtr hrcSrvSource);

        public static partial class Arb
        {
            [AutoGenerated(EntryPoint = "wglCreateContextAttribsARB")]
            public static
            IntPtr CreateContextAttribs(IntPtr hDC, IntPtr hShareContext, int[] attribList)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetExtensionsStringARB")]
            public static
            string GetExtensionsString(IntPtr hdc)
            {
               throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetPixelFormatAttribivARB")]
            public static
            bool GetPixelFormatAttrib(IntPtr hdc, int iPixelFormat, int iLayerPlane, int nAttributes, int[] piAttributes, [Out] int[] piValues)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetPixelFormatAttribivARB")]
            public static
            bool GetPixelFormatAttrib(IntPtr hdc, int iPixelFormat, int iLayerPlane, int nAttributes, ref int piAttributes, [Out] out int piValues)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetPixelFormatAttribfvARB")]
            [System.CLSCompliant(false)]
            public static
            bool GetPixelFormatAttrib(IntPtr hdc, int iPixelFormat, int iLayerPlane, int nAttributes, int[] piAttributes, [Out] float[] pfValues)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetPixelFormatAttribfvARB")]
            public static
            bool GetPixelFormatAttrib(IntPtr hdc, int iPixelFormat, int iLayerPlane, int nAttributes, ref int piAttributes, [Out] out float pfValues)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglChoosePixelFormatARB")]
            public static
            bool ChoosePixelFormat(IntPtr hdc, int[] piAttribIList, float[] pfAttribFList, int nMaxFormats, [Out] int[] piFormats, out int nNumFormats)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglChoosePixelFormatARB")]
            public static
            bool ChoosePixelFormat(IntPtr hdc, ref int piAttribIList, ref float pfAttribFList, int nMaxFormats, [Out] out int piFormats, [Out] out int nNumFormats)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglMakeContextCurrentARB")]
            public static
            bool MakeContextCurrent(IntPtr hDrawDC, IntPtr hReadDC, IntPtr hglrc)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetCurrentReadDCARB")]
            public static
            IntPtr GetCurrentReadDC()
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglCreatePbufferARB")]
            public static
            IntPtr CreatePbuffer(IntPtr hDC, int iPixelFormat, int iWidth, int iHeight, int[] piAttribList)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglCreatePbufferARB")]
            public static
            IntPtr CreatePbuffer(IntPtr hDC, int iPixelFormat, int iWidth, int iHeight, ref int piAttribList)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetPbufferDCARB")]
            public static
            IntPtr GetPbufferDC(IntPtr hPbuffer)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglReleasePbufferDCARB")]
            public static
            int ReleasePbufferDC(IntPtr hPbuffer, IntPtr hDC)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglDestroyPbufferARB")]
            public static
            bool DestroyPbuffer(IntPtr hPbuffer)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglQueryPbufferARB")]
            public static
            bool QueryPbuffer(IntPtr hPbuffer, int iAttribute, [Out] int[] piValue)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglQueryPbufferARB")]
            public static
            bool QueryPbuffer(IntPtr hPbuffer, int iAttribute, [Out] out int piValue)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglBindTexImageARB")]
            public static
            bool BindTexImage(IntPtr hPbuffer, int iBuffer)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglReleaseTexImageARB")]
            public static
            bool ReleaseTexImage(IntPtr hPbuffer, int iBuffer)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglSetPbufferAttribARB")]
            public static
            bool SetPbufferAttrib(IntPtr hPbuffer, int[] piAttribList)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglSetPbufferAttribARB")]
            public static
            bool SetPbufferAttrib(IntPtr hPbuffer, ref int piAttribList)
            {
                throw new NotImplementedException();
            }
        }

        public static partial class Ext
        {
            [AutoGenerated(EntryPoint = "wglGetExtensionsStringEXT")]
            public static
            string GetExtensionsString()
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglSwapIntervalEXT")]
            public static
            bool SwapInterval(int interval)
            {
                throw new NotImplementedException();
            }

            [AutoGenerated(EntryPoint = "wglGetSwapIntervalEXT")]
            public static
            int GetSwapInterval()
            {
                throw new NotImplementedException();
            }
        }

        [Slot(0)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern IntPtr wglCreateContextAttribsARB(IntPtr hDC, IntPtr hShareContext, int* attribList);

        [Slot(1)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr wglGetExtensionsStringARB(IntPtr hdc);

        [Slot(2)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern bool wglGetPixelFormatAttribivARB(IntPtr hdc, int iPixelFormat, int iLayerPlane, uint nAttributes, int* piAttributes, [Out] int* piValues);

        [Slot(3)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern bool wglGetPixelFormatAttribfvARB(IntPtr hdc, int iPixelFormat, int iLayerPlane, uint nAttributes, int* piAttributes, [Out] float* pfValues);

        [Slot(4)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern bool wglChoosePixelFormatARB(IntPtr hdc, int* piAttribIList, float* pfAttribFList, uint nMaxFormats, [Out] int* piFormats, [Out] uint* nNumFormats);

        [Slot(5)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool wglMakeContextCurrentARB(IntPtr hDrawDC, IntPtr hReadDC, IntPtr hglrc);

        [Slot(6)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr wglGetCurrentReadDCARB();

        [Slot(7)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern IntPtr wglCreatePbufferARB(IntPtr hDC, int iPixelFormat, int iWidth, int iHeight, int* piAttribList);

        [Slot(8)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr wglGetPbufferDCARB(IntPtr hPbuffer);

        [Slot(9)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int wglReleasePbufferDCARB(IntPtr hPbuffer, IntPtr hDC);

        [Slot(10)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool wglDestroyPbufferARB(IntPtr hPbuffer);

        [Slot(11)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern bool wglQueryPbufferARB(IntPtr hPbuffer, int iAttribute, [Out] int* piValue);

        [Slot(12)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool wglBindTexImageARB(IntPtr hPbuffer, int iBuffer);

        [Slot(13)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool wglReleaseTexImageARB(IntPtr hPbuffer, int iBuffer);

        [Slot(14)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal unsafe static extern bool wglSetPbufferAttribARB(IntPtr hPbuffer, int* piAttribList);

        [Slot(15)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr wglGetExtensionsStringEXT();

        [Slot(16)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern bool wglSwapIntervalEXT(int interval);

        [Slot(17)]
        [DllImport(Library, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        internal static extern int wglGetSwapIntervalEXT();
    }
}