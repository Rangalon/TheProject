#region License

//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2013 Stefanos Apostolopoulos for the Open Toolkit Library
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

#region --- Using Directives ---

using Math3D;
using System;

#if !MINIMAL
#endif

using System.Text;

#endregion --- Using Directives ---

namespace OpenTK.Graphics.OpenGL
{
    /// <summary>
    /// OpenGL bindings for .NET, implementing the full OpenGL API, including extensions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains all OpenGL enums and functions defined in the latest OpenGL specification.
    /// The official .spec files can be found at: http://opengl.org/registry/.
    /// </para>
    /// <para> A valid OpenGL context must be created before calling any OpenGL function.</para>
    /// <para>
    /// Use the GL.Load and GL.LoadAll methods to prepare function entry points prior to use. To maintain
    /// cross-platform compatibility, this must be done for both core and extension functions. The GameWindow
    /// and the GLControl class will take care of this automatically.
    /// </para>
    /// <para>
    /// You can use the GL.SupportsExtension method to check whether any given category of extension functions
    /// exists in the current OpenGL context. Keep in mind that different OpenGL contexts may support different
    /// extensions, and under different entry points. Always check if all required extensions are still supported
    /// when changing visuals or pixel formats.
    /// </para>
    /// <para>
    /// You may retrieve the entry point for an OpenGL function using the GL.GetDelegate method.
    /// </para>
    /// </remarks>
    /// <see href="http://opengl.org/registry/"/>
    public sealed partial class GL : GraphicsBindingsBase
    {
        private static int[] EntryPointNameOffsets;
        private static byte[] EntryPointNames;
        private static IntPtr[] EntryPoints;
        internal const string Library = "opengl32.dll";
        internal const string CLLibrary = "opencl.dll";
        private static readonly object sync_root = new object();

        /// <summary>
        ///
        /// </summary>
        /// <param name="program"></param>
        /// <param name="info"></param>
        public static void GetProgramInfoLog(int program, out string info)
        {
            unsafe
            {
                int length;
                GL.GetProgram(program, OpenTK.Graphics.OpenGL.GetProgramParameterName.InfoLogLength, out length); if (length == 0)
                {
                    info = string.Empty;
                    return;
                }
                StringBuilder sb = new StringBuilder(length * 2);
                GL.GetProgramInfoLog((uint)program, sb.Capacity, &length, sb);
                info = sb.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="program"></param>
        /// <returns></returns>
        public static string GetProgramInfoLog(int program)
        {
            string info;
            GetProgramInfoLog(program, out info);
            return info;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="info"></param>
        public static void GetShaderInfoLog(int shader, out string info)
        {
            unsafe
            {
                int length;
                GL.GetShader(shader, ShaderParameter.InfoLogLength, out length);
                if (length == 0)
                {
                    info = string.Empty;
                    return;
                }
                StringBuilder sb = new StringBuilder(length * 2);
                GL.GetShaderInfoLog((uint)shader, sb.Capacity, &length, sb);
                info = sb.ToString();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shader"></param>
        /// <returns></returns>
        public static string GetShaderInfoLog(int shader)
        {
            string info;
            GetShaderInfoLog(shader, out info);
            return info;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="String"></param>
        public static void ShaderSource(int shader, string @String)
        {
            unsafe
            {
                int length = @String.Length;
                GL.ShaderSource((uint)shader, 1, new string[] { @String }, &length);
            }
        }

        public static void Vertex3(Vec3f v)
        {
            GL.Vertex3(v.X, v.Y, v.Z);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="location"></param>
        /// <param name="transpose"></param>
        /// <param name="matrix"></param>
        ///
        public static void Uniform4fv(int location, Vec4 vector)
        {
            Vec4f v = (Vec4f)vector;
            unsafe
            {
                float* ptr = &v.X;
                GL.Uniform4fv(location, 1, ptr);
            }
        }

        public static void UniformMatrix4(int location, bool transpose, Mtx4 matrix)
        {
            Mtx4f mf = (Mtx4f)matrix;
            unsafe
            {
                float* matrix_ptr = &mf.Row0.X;
                GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
            }
        }

        public static void UniformMatrix4(int location, bool transpose, ref Mtx4f matrix)
        {
            unsafe
            {
                fixed (float* matrix_ptr = &matrix.Row0.X)
                {
                    GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
                }
            }
        }

        public static void UniformMatrix4d(int location, bool transpose, ref Mtx4 matrix)
        {
            unsafe
            {
                fixed (double* matrix_ptr = &matrix.Row0.X)
                {
                    GL.UniformMatrix4d(location, 1, transpose, matrix_ptr);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public GL()
        {
            _EntryPointsInstance = EntryPoints;
            _EntryPointNamesInstance = EntryPointNames;
            _EntryPointNameOffsetsInstance = EntryPointNameOffsets;
        }

        /// <summary>
        ///
        /// </summary>
        protected override object SyncRoot
        {
            get { return sync_root; }
        }
    }
}