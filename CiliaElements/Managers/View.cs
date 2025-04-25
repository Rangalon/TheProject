
using CiliaElements.Elements.Control;
using Math3D;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Public Fields

        public static readonly Stack<TLink> LinksToMove = new Stack<TLink>();

        #endregion Public Fields

        #region Private Fields

        private static readonly StreamWriter ConsoleWriter;
        private static readonly DirectoryInfo documentsDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Cilia");
        private static int ConsoleLevel = 0;

        #endregion Private Fields

        #region Public Properties

        public static DirectoryInfo DocumentsDirectory
        {
            get
            {
                if (!documentsDirectory.Exists)
                {
                    documentsDirectory.Create();
                }

                return documentsDirectory;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public class TTextureHandler
        {
            public int TextureId;

            internal BitmapData TextureData;
            public Bitmap Bmp;

            public TTextureHandler(double v1, double v2, double v3)
            {
                Bmp = new Bitmap(4, 4);
                Graphics grp = Graphics.FromImage(Bmp);
                grp.Clear(Color.FromArgb((byte)(v1 * 255), (byte)(v2 * 255), (byte)(v3 * 255)));
                grp.Dispose();
                TextureData = Bmp.LockBits(new Rectangle(0, 0, Bmp.Width, Bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                lock (TexturesToGenerate) TexturesToGenerate.Push(this);
            }
        }


        public static Stack<TTextureHandler> TexturesToGenerate = new Stack<TTextureHandler>();

        public static void GenerateTexture(TTextureHandler handler)
        {
            handler.TextureId = GL.GenTexture();
            GL.ActiveTexture(handler.TextureId);
            GL.BindTexture(TextureTarget.Texture2D, handler.TextureId);

            BitmapData data = handler.TextureData;  // Solid.textureBmp.LockBits(new Rectangle(0, 0, Solid.textureBmp.Width, Solid.textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);


            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        public static Mtx4 CreateForeMatrix(double x, double y, double k, double h)
        {
            Mtx4 M = new Mtx4();
            M.Row0.X = k / (TManager.Height * BaseLayer.PMatrix.Row1.Y);
            M.Row1.Z = -0.004 * h;
            M.Row2.Y = M.Row0.X;
            M.Row3.X = 0.0005 * x * xRatio; M.Row3.Y = 0.0005 * y * yRatio; M.Row3.Z = -0.0005; M.Row3.W = 1;
            return M;
        }

        public static void DrawView()
        {

            lock (TexturesToGenerate)
                while (TexturesToGenerate.Count > 0) GenerateTexture(TexturesToGenerate.Pop());

            if (SolidsToBePushed.NotEmpty)
            {
                TSolidElement Solid = SolidsToBePushed.Pop();
                TSolidElementConstruction SolidElementConstruction = Solid.SolidElementConstruction;
                //
                GL.GenBuffers(1, out Solid.HandlePositions);
                GL.GenBuffers(1, out Solid.HandleNormals);
                GL.GenBuffers(1, out Solid.HandleTexts);
                GL.GenBuffers(1, out Solid.HandleIndexes);
                GL.GenVertexArrays(1, out Solid.HandleVAO);
                //////
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandlePositions);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(SolidElementConstruction.vsP.Length * 12), SolidElementConstruction.vsP, BufferUsageHint.StreamDraw);
                //
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleNormals);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(SolidElementConstruction.vsN.Length * 12), SolidElementConstruction.vsN, BufferUsageHint.StreamDraw);
                //   
                Solid.TextureId = GL.GenTexture();
                GL.ActiveTexture(Solid.TextureId);
                GL.BindTexture(TextureTarget.Texture2D, Solid.TextureId);

                lock (Solid.TextureDataLocker)
                {
                    if (Solid.TextureData != null)
                    {
                        BitmapData data = Solid.TextureData;  // Solid.textureBmp.LockBits(new Rectangle(0, 0, Solid.textureBmp.Width, Solid.textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                    }
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                //
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleTexts);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Solid.DataTexts.Length * 8), Solid.DataTexts, BufferUsageHint.StreamDraw);
                //
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, Solid.HandleIndexes);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(Solid.DataIndexes.Length * 4), Solid.DataIndexes, BufferUsageHint.DynamicDraw);
                //
                GL.BindVertexArray(Solid.HandleVAO);
                ////
                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandlePositions);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 12, IntPtr.Zero);
                GL.BindAttribLocation(HandleShaderProgram, 0, "in_position");
                //
                GL.EnableVertexAttribArray(1);
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleNormals);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 12, IntPtr.Zero);
                GL.BindAttribLocation(HandleShaderProgram, 1, "in_normal");
                //
                GL.EnableVertexAttribArray(2);
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleTexts);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8, IntPtr.Zero);
                GL.BindAttribLocation(HandleShaderProgram, 2, "in_text");
                //
                Solid.State = EElementState.Pushed;
                //
                SolidsToBeGarbaged.Push(Solid);
            }

            if (SolidsToBeUpdated.NotEmpty)
            {
                TSolidElement Solid = SolidsToBeUpdated.Pop();
                TSolidElementConstruction SolidElementConstruction = Solid.SolidElementConstruction;
                //
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandlePositions);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(SolidElementConstruction.vsP.Length * 12), SolidElementConstruction.vsP, BufferUsageHint.StreamDraw);
                //
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleNormals);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(SolidElementConstruction.vsN.Length * 12), SolidElementConstruction.vsN, BufferUsageHint.StreamDraw);
                //
                GL.BindBuffer(BufferTarget.ArrayBuffer, Solid.HandleTexts);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Solid.DataTexts.Length * 8), Solid.DataTexts, BufferUsageHint.StreamDraw);
                //
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, Solid.HandleIndexes);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(Solid.DataIndexes.Length * 4), Solid.DataIndexes, BufferUsageHint.DynamicDraw);
                //
            }
            // 
            for (int i = 0; TexturesToUpdate.NotEmpty && i < 20; i++)
            {
                TSolidElement Solid = TexturesToUpdate.Pop();
                GL.ActiveTexture(Solid.TextureId);
                GL.BindTexture(TextureTarget.Texture2D, Solid.TextureId);

                lock (Solid.TextureDataLocker)
                {
                    BitmapData data = Solid.TextureData;  //textureBmp.LockBits(new Rectangle(0, 0, Solid.textureBmp.Width, Solid.textureBmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);                     
                    if (data != null) GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                }

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.LinearDetailSgis);
            }
            //
            GL.Viewport(0, 0, WidthInt, HeightInt);
            //
            GL.ClearColor(0.0025F, 0.0025F, 0.0025F, 1);
            //
            solidsNumber = 0;
            facetsNumber = 0;
            //
            // ---------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------
            lock (BaseLayer)
            {
                BaseLayer.TakenSolids = BaseLayer.GivingSolids;
                Array.ForEach(Layers, l => { /*lock (l)*/ l.TakenSolids = l.GivingSolids; });
            }
            //
            GL.UniformMatrix4(HandleViewMatrix, false, Mtx4.Identity );

            // ---------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------
            GL.Clear(ClearBufferMasks);
            Array.ForEach(Array.FindAll(Layers, o => o.TakenSolids.Length > 0), layer =>
            {
                GL.Clear(ClearBufferMask.DepthBufferBit);
                GL.UniformMatrix4(HandleProjectionMatrix, false, ref layer.PMatrixf);
                //
                Array.ForEach(layer.TakenSolids, gvg =>
                      {
                          //
                          solidsNumber++;
                          //
                          if (gvg.Selected)
                          {
                              //
                              SBoundingBox3 b = gvg.Link.Child.BoundingBox;
                              GL.UniformMatrix4(HandleModelMatrix, false, Mtx4.CreateScaledTranslation(b.Size, b.MinPosition) * gvg.Matrix);
                              GL.Uniform1i(HandleNoDiffuse, 0);
                              GL.BindTexture(TextureTarget.Texture2D, RepBBox.TextureId);
                              GL.BindVertexArray(RepBBox.HandleVAO);
                              GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepBBox.HandleIndexes);
                              GL.DrawElements(BeginMode.Triangles, 144, DrawElementsType.UnsignedInt, 0);
                              // 
                          }
                          //
                          GL.UniformMatrix4(HandleColorMatrix, false, ref gvg.Color);
                          // 
                          GL.Uniform1i(HandleNoEffect, gvg.NoEffectValue);
                          //
                          GL.Uniform1i(HandleTextureOffset, gvg.Link.Solid.TextureOffset);
                          GL.BindTexture(TextureTarget.Texture2D, gvg.Link.Solid.TextureId);
                          GL.BindVertexArray(gvg.Link.Solid.HandleVAO);
                          GL.BindBuffer(BufferTarget.ElementArrayBuffer, gvg.Link.Solid.HandleIndexes);
                          GL.UniformMatrix4(HandleModelMatrix, false, gvg.Matrix);
                          
                          DrawSolid(gvg.Link.Solid);
                          // 
                      });
                //
                //Array.ForEach(layer.TakenSolids.Where(o => o.Link.Solid.PointsOnly == true).ToArray(), gvg =>
                //{
                //    //
                //    solidsNumber++;
                //    //
                //    if (gvg.State.HasFlag(ELinkState.Selected))
                //    {
                //        //
                //        SBoundingBox3 b = gvg.Link.Child.BoundingBox;
                //        GL.UniformMatrix4(HandleModelMatrix, false, Mtx4.CreateScaledTranslation(b.Size, b.MinPosition) * gvg.Matrix);
                //        GL.Uniform1i(HandleNoDiffuse, 0);
                //        GL.BindTexture(TextureTarget.Texture2D, RepBBox.TextureId);
                //        GL.BindVertexArray(RepBBox.HandleVAO);
                //        GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepBBox.HandleIndexes);
                //        GL.DrawElements(BeginMode.Triangles, 144, DrawElementsType.UnsignedInt, 0);
                //        // 
                //    }
                //    GL.UniformMatrix4(HandleColorMatrix, false, ref gvg.Color);
                //    //
                //    GL.BindVertexArray(gvg.Link.Solid.HandleVAO);
                //    GL.BindBuffer(BufferTarget.ElementArrayBuffer, gvg.Link.Solid.HandleIndexes);
                //    GL.UniformMatrix4(HandleModelMatrix, false, gvg.Matrix);
                //    DrawSolid(gvg.Link.Solid);
                //});
                //
            });
            //
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.UniformMatrix4(HandleProjectionMatrix, false, ref BaseLayer.PMatrixf);
            //
            if (Touch1 != null)
            {
                GL.BindVertexArray(RepCursor.HandleVAO);
                GL.BindTexture(TextureTarget.Texture2D, RepCursor.TextureId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepCursor.HandleIndexes);
                GL.UniformMatrix4(HandleModelMatrix, false, TouchMatrix);
                GL.DrawElements(BeginMode.Triangles, RepCursor.FacesNumber, DrawElementsType.UnsignedInt, 0);
            }
            else if (Touch2 != null)
            {
                GL.BindTexture(TextureTarget.Texture2D, RepCursor.TextureId);
                GL.BindVertexArray(RepCursor.HandleVAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepCursor.HandleIndexes);
                foreach (Mtx4 m in GlyphePoints.Values.Where(o => o != null))
                {
                    GL.UniformMatrix4(HandleModelMatrix, false, m);
                    GL.DrawElements(BeginMode.Triangles, RepCursor.FacesNumber, DrawElementsType.UnsignedInt, 0);
                }
            }
            else if (PickerMode)
            {
                GL.BindTexture(TextureTarget.Texture2D, RepPicker.TextureId);
                GL.BindVertexArray(RepPicker.HandleVAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepPicker.HandleIndexes);
                GL.UniformMatrix4(HandleModelMatrix, false, CursorMatrix);
                GL.DrawElements(BeginMode.Triangles, RepPicker.FacesNumber, DrawElementsType.UnsignedInt, 0);
            }
            else if (RepCursor != null)
            {
                GL.BindTexture(TextureTarget.Texture2D, RepCursor.TextureId);
                GL.BindVertexArray(RepCursor.HandleVAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepCursor.HandleIndexes);
                GL.UniformMatrix4(HandleModelMatrix, false, CursorMatrix);
                GL.DrawElements(BeginMode.Triangles, RepCursor.FacesNumber, DrawElementsType.UnsignedInt, 0);
                //
                if (MovingPoint.HasValue)
                {
                    GL.BindTexture(TextureTarget.Texture2D, RepMovingPoint.TextureId);
                    GL.BindVertexArray(RepMovingPoint.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepMovingPoint.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, MovingCursorMatrix);
                    GL.DrawElements(BeginMode.Triangles, RepMovingPoint.FacesNumber, DrawElementsType.UnsignedInt, 0);

                    Vec3 v = Vec3.Transform(Target, BaseLayer.PVMatrix);
                    v /= v.Z;
                    Mtx4 mm = Mtx4.SwitchXY * CreateForeMatrix(-v.X, -v.Y, 0.15, 0.2);

                    GL.BindTexture(TextureTarget.Texture2D, RepTarget.TextureId);
                    GL.BindVertexArray(RepTarget.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepTarget.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, mm);
                    GL.DrawElements(BeginMode.Triangles, RepTarget.FacesNumber, DrawElementsType.UnsignedInt, 0);

                    GL.BindTexture(TextureTarget.Texture2D, MoveSolid.TextureId);
                    GL.BindVertexArray(MoveSolid.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, MoveSolid.HandleIndexes);
                    GL.LineWidth(.00001F);
                    GL.UniformMatrix4(HandleColorMatrix, false, ref ColorAllWhite);
                    GL.UniformMatrix4(HandleModelMatrix, false, (CreateForeMatrix(0, 0, 0.3, 8)));
                    GL.DrawElements(BeginMode.Lines, MoveSolid.LinesNumber, DrawElementsType.UnsignedInt, MoveSolid.LinesStarter);
                }
            }
            //
            if (DrawIcons)
            {
                GL.UniformMatrix4(HandleColorMatrix, false, ref ColorNormal);
                foreach (TControl c in TControl.Controls.Values.Where(o => o != null && o.Visible))
                {
                    GL.Uniform1i(HandleNoDiffuse, 1);
                    GL.BindTexture(TextureTarget.Texture2D, c.TextureId);
                    GL.BindVertexArray(c.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, c.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, c.OwnerLink.Giving.Matrix);//CreateForeMatrix(0, 0, 0.3, 0.2)); //
                    GL.DrawElements(BeginMode.Triangles, c.FacesNumber, DrawElementsType.UnsignedInt, 0);
                }
            }
            //
            if (DrawMeasures)
            {
                GL.UniformMatrix4(HandleColorMatrix, false, ref ColorMeasure);
                GL.Uniform1i(HandleNoDiffuse, 0);
                foreach (TMeasuredPoint p in TMeasurePointAction.MeasuredPoints)
                {
                    p.Update();
                    Vec3 vp = p.AbsolutePoint;
                    Vec4 v = p.ViewPoint;
                    GL.Uniform1i(HandleNoDiffuse, 1);
                    GL.BindTexture(TextureTarget.Texture2D, RepMeasurePoint.TextureId);
                    GL.BindVertexArray(RepMeasurePoint.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepMeasurePoint.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, CreateForeMatrix(-v.X, -v.Y, 0.3, 0.2));
                    GL.DrawElements(BeginMode.Lines, RepMeasurePoint.LinesNumber, DrawElementsType.UnsignedInt, RepMeasurePoint.LinesStarter);
                    p.Panel.DisplayedVector = vp;
                    p.Panel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(p.Panel.Width, 1, p.Panel.Height) * CreateForeMatrix(-v.X, -v.Y, 0.001, 0.002);
                }
                foreach (TMeasuredVector V in TMeasureVectorAction.MeasuredVectors)
                {
                    V.Update();
                    Vec4 v1 = V.P1.ViewPoint;
                    Vec4 v2 = V.P2.ViewPoint;
                    DrawMeasureArrow(v1, v1 - v2);
                    //
                    GL.Uniform1i(HandleNoDiffuse, 1);
                    GL.BindTexture(TextureTarget.Texture2D, RepMeasurePoint.TextureId);
                    GL.BindVertexArray(RepMeasurePoint.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepMeasurePoint.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, CreateForeMatrix(-v1.X, -v1.Y, 0.3, 0.2));
                    GL.DrawElements(BeginMode.Lines, RepMeasurePoint.LinesNumber, DrawElementsType.UnsignedInt, RepMeasurePoint.LinesStarter);
                    GL.UniformMatrix4(HandleModelMatrix, false, CreateForeMatrix(-v2.X, -v2.Y, 0.3, 0.2));
                    GL.DrawElements(BeginMode.Lines, RepMeasurePoint.LinesNumber, DrawElementsType.UnsignedInt, RepMeasurePoint.LinesStarter);
                    //
                    v2 = (v1 + v2) * 0.5;
                    V.Panel.DisplayedVector = V.Vector;
                    V.Panel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(V.Panel.Width, 1, V.Panel.Height) * CreateForeMatrix(-v2.X, -v2.Y, 0.001, 0.002);
                }
                foreach (TMeasuredVector V in TMeasureMinimalVectorAction.MeasuredVectors)
                {
                    V.Update();
                    Vec4 v1 = V.P1.ViewPoint;
                    Vec4 v2 = V.P2.ViewPoint;
                    DrawMeasureArrow(v1, v1 - v2);
                    //
                    GL.Uniform1i(HandleNoDiffuse, 1);
                    GL.BindTexture(TextureTarget.Texture2D, RepMeasurePoint.TextureId);
                    GL.BindVertexArray(RepMeasurePoint.HandleVAO);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepMeasurePoint.HandleIndexes);
                    GL.UniformMatrix4(HandleModelMatrix, false, CreateForeMatrix(-v1.X, -v1.Y, 0.3, 0.2));
                    GL.DrawElements(BeginMode.Lines, RepMeasurePoint.LinesNumber, DrawElementsType.UnsignedInt, RepMeasurePoint.LinesStarter);
                    GL.UniformMatrix4(HandleModelMatrix, false, CreateForeMatrix(-v2.X, -v2.Y, 0.3, 0.2));
                    GL.DrawElements(BeginMode.Lines, RepMeasurePoint.LinesNumber, DrawElementsType.UnsignedInt, RepMeasurePoint.LinesStarter);
                    //
                    v2 = (v1 + v2) * 0.5;
                    V.Panel.DisplayedVector = V.Vector;
                    V.Panel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(V.Panel.Width, 1, V.Panel.Height) * CreateForeMatrix(-v2.X, -v2.Y, 0.001, 0.002);
                }
            }
            //
            FacetsNumber = facetsNumber;
            SolidsNumber = solidsNumber;
            //
        }

        public static void FitFront()
        {
            Reach(new Vec4(1, 0, 0, 0), new Vec4(0, 0, 1, 0));
        }

        public static void FitLeft()
        {
            Reach(new Vec4(0, 1, 0, 0), new Vec4(0, 0, 1, 0));
        }

        public static void FitRear()
        {
            Reach(new Vec4(-1, 0, 0, 0), new Vec4(0, 0, 1, 0));
        }

        public static void FitRight()
        {
            Reach(new Vec4(0, -1, 0, 0), new Vec4(0, 0, 1, 0));
        }

        public static void FitSelected()
        {
            if (SelectedLinks.Length == 0)
            {
                FitAll();
            }

            List<Vec4> pts = new List<Vec4>();
            foreach (TLink l in SelectedLinks)
            {
                Mtx4 mtx = Mtx4.Identity;
                TLink link = l;
                while (link.Parent != null)
                {
                    link = link.Parent.OwnerLink;
                    mtx *= link.Matrix;
                }
                BuildBox(l, mtx, pts);
            }
            FitAll(10, pts);
            CenterTarget(pts);
        }

        public static void FitTop()
        {
            Reach(new Vec4(0, 0, -1, 0), new Vec4(1, 0, 0, 0));
        }

        public static bool GetClashFromRayAndBoundingBox(SBoundingBox3 BoundingBox, Mtx4 M)
        {
            Vec4 sz = BoundingBox.Size;
            byte i = 0;
            if (sz.X != 0)
            {
                if (sz.Y != 0)
                {
                    if (M.CheckRayXY(BoundingBox.MinPosition, -sz.X, -sz.Y))
                    {
                        i++;
                    }

                    if (M.CheckRayXY(BoundingBox.MaxPosition, sz.X, sz.Y))
                    {
                        i++;
                    }
                }
                if (sz.Z != 0)
                {
                    if (M.CheckRayZX(BoundingBox.MinPosition, -sz.Z, -sz.X))
                    {
                        i++;
                    }

                    if (M.CheckRayZX(BoundingBox.MaxPosition, sz.Z, sz.X))
                    {
                        i++;
                    }
                }
            }
            if (sz.Y != 0 && sz.Z != 0)
            {
                if (M.CheckRayYZ(BoundingBox.MinPosition, -sz.Y, -sz.Z))
                {
                    i++;
                }

                if (M.CheckRayYZ(BoundingBox.MaxPosition, sz.Y, sz.Z))
                {
                    i++;
                }
            }
            return (i == 2);
        }

        public static void PullConsole()
        { ConsoleLevel--; }

        public static void PushConsole()
        { ConsoleLevel++; }

        public static void SetViewPoint(Vec3 iOrigin, Vec3 iEyeDirection, Vec3 iUpDirection, Vec3 iTarget)
        {

            Mtx4 m = new Mtx4();
            m.Row1 += iUpDirection;
            m.Row2 -= iEyeDirection;
            m.Row0 = Vec4.Cross(m.Row1, m.Row2);
            m.Row1 = Vec4.Cross(m.Row2, m.Row0);
            m.Row0.Normalize();
            m.Row1.Normalize();
            m.Row2.Normalize();
            m.Row3 = new Vec4(iOrigin, 1);
            m = Mtx4.InvertL(m);//     m.Invert();
            PendingViewPoint = m;
            Target = iTarget;
            targetPanel.DisplayedVector = Target;
        }

        public static void SetViewPoint(Vec3 iOrigin, Vec3 iEyeDirection, Vec3 iUpDirection)
        {
            SetViewPoint(iOrigin, iEyeDirection, iUpDirection, Target);
        }

        public static void SwitchDrawTree()
        {
            graphTree.Visible = !graphTree.Visible;
        }

        public static void WriteLineConsole(string l)
        {
            string[] ls = logsPanel.Message.Split('\n');
            if (ls.Length == 50)
            {
                Array.Copy(ls, 1, ls, 0, ls.Length - 1);
            }
            else
            {
                Array.Resize(ref ls, ls.Length + 1);
                //Array.Copy(ls, 0, ls, 1, ls.Length - 1);
            }

            ls[ls.Length - 1] = DateTime.Now.ToLongTimeString() + " ";
            for (int i = 0; i < ConsoleLevel; i++)
            {
                ls[ls.Length - 1] += "--";
            }
            ls[ls.Length - 1] += " " + l;
            string ss = "";
            foreach (string s in ls.Where(o => o != ""))
            {
                ss += s + '\n';
            }
            ss = ss.Substring(0, ss.Length - 1);
            logsPanel.Message = ss;

            ConsoleWriter.WriteLine(ls[ls.Length - 1]);
        }

        #endregion Public Methods

        #region Private Methods

        private static void CenterMe()
        {
            Target = (Vec3)VIMatrix.Row3;

            targetPanel.DisplayedVector = Target;
            CenterMeMode = true;
        }

        private static void CenterTarget(List<Vec4> iPoints)
        {
            double XMin = double.MaxValue;
            double XMax = double.MinValue;
            double yMin = double.MaxValue;
            double yMax = double.MinValue;
            double zMin = double.MaxValue;
            double zMax = double.MinValue;

            foreach (Vec4 pt in iPoints.Where(o => !double.IsNaN(o.X) && !double.IsNaN(o.Y) && !double.IsNaN(o.Z)))
            {
                if (pt.Z > zMax)
                {
                    zMax = pt.Z;
                }
                else if (pt.Z < zMin)
                {
                    zMin = pt.Z;
                }

                if (pt.X > XMax)
                {
                    XMax = pt.X;
                }
                else if (pt.X < XMin)
                {
                    XMin = pt.X;
                }

                if (pt.Y > yMax)
                {
                    yMax = pt.Y;
                }
                else if (pt.Y < yMin)
                {
                    yMin = pt.Y;
                }
            }

            Target = new Vec3((XMax + XMin) * 0.5, (yMax + yMin) * 0.5, (zMax + zMin) * 0.5);
            targetPanel.DisplayedVector = Target;
        }

        private static void DecreaseCulling()
        {
            CullingParameter /= 1.2;
            UpdateLayout();
        }

        private static void DefineDrawFunction()
        {
            if (DrawFaces)
            {
                if (DrawLines)
                {
                    if (DrawPoints)
                    {
                        DrawSolid = Draw7;
                    }
                    else
                    {
                        DrawSolid = Draw6;
                    }
                }
                else
                {
                    if (DrawPoints)
                    {
                        DrawSolid = Draw5;
                    }
                    else
                    {
                        DrawSolid = Draw4;
                    }
                }
            }
            else
            {
                if (DrawLines)
                {
                    if (DrawPoints)
                    {
                        DrawSolid = Draw3;
                    }
                    else
                    {
                        DrawSolid = Draw2;
                    }
                }
                else
                {
                    if (DrawPoints)
                    {
                        DrawSolid = Draw1;
                    }
                    else
                    {
                        DrawSolid = Draw0;
                    }
                }
            }
        }


        private static void Draw0(TSolidElement s)
        {
        }

        private static void Draw1(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Points, s.PointsNumber, DrawElementsType.UnsignedInt, s.PointsStarter);
        }

        private static void Draw2(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Lines, s.LinesNumber, DrawElementsType.UnsignedInt, s.LinesStarter);
        }

        private static void Draw3(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Lines, s.LinesNumber, DrawElementsType.UnsignedInt, s.LinesStarter);
            GL.DrawElements(BeginMode.Points, s.PointsNumber, DrawElementsType.UnsignedInt, s.PointsStarter);
        }

        private static void Draw4(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 0);
            GL.DrawElements(BeginMode.Triangles, s.FacesNumber, DrawElementsType.UnsignedInt, s.FacesStarter);
            facetsNumber += s.FacesNumber;
        }

        private static void Draw5(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 0);
            GL.DrawElements(BeginMode.Triangles, s.FacesNumber, DrawElementsType.UnsignedInt, s.FacesStarter);
            facetsNumber += s.FacesNumber;
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Points, s.PointsNumber, DrawElementsType.UnsignedInt, s.PointsStarter);
        }

        private static void Draw6(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 0);
            GL.DrawElements(BeginMode.Triangles, s.FacesNumber, DrawElementsType.UnsignedInt, s.FacesStarter);
            facetsNumber += s.FacesNumber;
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Lines, s.LinesNumber, DrawElementsType.UnsignedInt, s.LinesStarter);
        }

        private static void Draw7(TSolidElement s)
        {
            GL.Uniform1i(HandleNoDiffuse, 0);
            GL.DrawElements(BeginMode.Triangles, s.FacesNumber, DrawElementsType.UnsignedInt, s.FacesStarter);
            facetsNumber += s.FacesNumber;
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Lines, s.LinesNumber, DrawElementsType.UnsignedInt, s.LinesStarter);
            GL.DrawElements(BeginMode.Points, s.PointsNumber, DrawElementsType.UnsignedInt, s.PointsStarter);
        }

        private static void DrawMeasureArrow(Vec4 v1, Vec4 v)
        {
            GL.BindTexture(TextureTarget.Texture2D, RepMeasureArrow.TextureId);
            GL.BindVertexArray(RepMeasureArrow.HandleVAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, RepMeasureArrow.HandleIndexes);
            Mtx4 iMatrix = CreateForeMatrix(-v1.X, -v1.Y, 0.03, 0.2);
            iMatrix.Row0.X = 0.0005 * v.X * xRatio;
            iMatrix.Row2.Y = 0.0005 * v.Y * yRatio;
            GL.UniformMatrix4(HandleModelMatrix, false, (iMatrix));
            GL.Uniform1i(HandleNoDiffuse, 1);
            GL.DrawElements(BeginMode.Lines, RepMeasureArrow.LinesNumber, DrawElementsType.UnsignedInt, RepMeasureArrow.LinesStarter);
        }

        //static TFPS[] fpss;
        public static void FitAll() { FitAll(10); }

        private static void FitAll(double iSteps, List<Vec4> iPoints = null)
        {
            if (iPoints == null)
            {
                iPoints = new List<Vec4>();
                BuildBox(View.OwnerLink, Mtx4.Identity, iPoints);
            }
            //
            if (iPoints.Count == 0) { return; }
            //
            for (double i = iSteps; i >= 1; i--)
            {
                double XMin = double.MaxValue;
                double XMax = double.MinValue;
                double yMin = double.MaxValue;
                double yMax = double.MinValue;
                double zMin = double.MaxValue;
                double zMax = double.MinValue;

                foreach (Vec4 pto in iPoints)
                {
                    Vec4 pt = Vec4.Transform(pto, VMatrix);
                    if (pt.Z > zMax)
                    {
                        zMax = pt.Z;
                    }

                    if (pt.Z < zMin)
                    {
                        zMin = pt.Z;
                    }

                    if (pt.X > XMax)
                    {
                        XMax = pt.X;
                    }

                    if (pt.X < XMin)
                    {
                        XMin = pt.X;
                    }

                    if (pt.Y > yMax)
                    {
                        yMax = pt.Y;
                    }

                    if (pt.Y < yMin)
                    {
                        yMin = pt.Y;
                    }
                }
                //
                Mtx4 m = VMatrix;
                m.Row3.X -= (XMax + XMin) / (2 * i);
                m.Row3.Y -= (yMax + yMin) / (2 * i);
                VMatrix = m;
                //
                XMax = (XMax - XMin) * 0.5;
                yMax = (yMax - yMin) * 0.5;
                //
                double tx = XMax / Math.Tan(PerspectiveAngle);
                double ty = yMax / Math.Tan(PerspectiveAngle);
                if (tx < ty)
                {
                    tx = ty;
                }

                zMax += +tx;
                //
                m = VMatrix;
                m.Row3.Z -= zMax / i;
                VMatrix = m;
                //
                //ResetMatrixes();
            }
        }

        private static void FitBottom()
        {
            Reach(new Vec4(0, 0, 1, 0), new Vec4(-1, 0, 0, 0));
        }

        private static void IncreaseCulling()
        {
            CullingParameter *= 1.2;
            UpdateLayout();
        }

        private static void KeyPressed(OpenTK.Input.Key iKey, EKeybordModifiers iModifiers)
        {
            switch (iKey)
            {
                case OpenTK.Input.Key.F1: SwitchDrawIcons(); break;
                case OpenTK.Input.Key.F2: SwitchDrawPerformances(); break;
                case OpenTK.Input.Key.F3: SwitchDrawTree(); break;
                case OpenTK.Input.Key.F12: SwitchDrawPerformances(); break;
                case OpenTK.Input.Key.F10: SwitchDrawThreads(); break;
                case OpenTK.Input.Key.F11: SwitchDrawLogs(); break;
                case OpenTK.Input.Key.F4: RequestSwitchWindowState?.Invoke(); break;
                case OpenTK.Input.Key.F5: break;
                case OpenTK.Input.Key.F6: RequestSwitchBorders?.Invoke(); break;
                case OpenTK.Input.Key.O: if (iModifiers.HasFlag(EKeybordModifiers.Ctrl)) { OpenFiles(); } break;
                case OpenTK.Input.Key.Plus: if (iModifiers.HasFlag(EKeybordModifiers.Ctrl)) { IncreaseCulling(); } break;
                case OpenTK.Input.Key.Minus: if (iModifiers.HasFlag(EKeybordModifiers.Ctrl)) { DecreaseCulling(); } break;
                case OpenTK.Input.Key.Up: if (iModifiers.HasFlag(EKeybordModifiers.Ctrl)) { IncreaseSpatialSpeed(); } break;
                case OpenTK.Input.Key.Down: if (iModifiers.HasFlag(EKeybordModifiers.Ctrl)) { DecreaseSpatialSpeed(); } break;
                default:
                    break;
            }
        }

        //public delegate void KeyPressedCallBack(OpenTK.Input.Key iKey, EKeybordModifiers iModifiers);

        // public static event KeyPressedCallBack KeyPressed;

        private static void OpenFiles()
        {
            if (RequestOpenFiles == null)
            {
                return;
            }

            List<string> lst = new List<string>();
            RequestOpenFiles(lst);
            foreach (string f in lst) { LoadFile(new FileInfo(f)); }
        }

        private static void Reach(Vec4 iViewVector, Vec4 iUpVector)
        {
            Mtx4 m = Mtx4.Identity;
            m.Row0 = Vec4.Cross(iUpVector, -iViewVector);
            m.Row1 = Vec4.Cross(-iViewVector, m.Row0);
            m.Row2 = -iViewVector;
            m.Row0.Normalize();
            m.Row1.Normalize();
            m.Row2.Normalize();
            m *= VMatrix;

            double ct = (m.Row0.X + m.Row1.Y + m.Row2.Z - 1) * 0.5;
            if (ct < -1)
            {
                ct = 1;
            }
            else if (ct > 1)
            {
                ct = 1;
            }

            double st = Math.Sqrt(1 - ct * ct);

            double a = 0;
            double b = 1;
            double c = 0;
            double t = Math.Atan2(st, ct);

            if (st > 0)
            {
                st = 0.5 / st;
                a = (m.Row1.Z - m.Row2.Y) * st;
                b = (m.Row2.X - m.Row0.Z) * st;
                c = (m.Row0.Y - m.Row1.X) * st;
            }

            Mtx4 mr3 = Mtx4.CreateRotation(a, b, c, t * 0.01);

            Vec4 tt = Vec4.Transform(Target, VMatrix);
            for (int i = 0; i < 100; i++)
            {
                m = VMatrix * mr3;
                Vec4 ttt = Vec4.Transform(Target, m);
                m.Row3 -= ttt - tt;
                VMatrix = m;
                VMatrix.Row0.Normalize();
                VMatrix.Row1.Normalize();
                VMatrix.Row2.Normalize();

                Thread.Sleep(10);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ResetPerspectives()
        {
            IHeight = 1 / height;
            IWidth = 1 / width;
            WidthIHeight = width * IHeight;
            //
            if (width != 0 && height != 0)
            {
                Array.ForEach(Layers, layer =>
                {
                    layer.PMatrix = Mtx4.CreatePerspectiveFieldOfView(PerspectiveAngle, WidthIHeight, layer.NearDistance, layer.NearDistance * FarRatio);
                    layer.PMatrixf = (Mtx4f)layer.PMatrix;
                    layer.PIMatrix = Mtx4.InvertP(layer.PMatrix);
                    //layer.PIMatrix.Invert();
                });
                //
                BaseLayer.PMatrix = Mtx4.CreatePerspectiveFieldOfView(PerspectiveAngle, WidthIHeight, BaseLayer.NearDistance, BaseLayer.NearDistance * FarRatio);
                BaseLayer.PMatrixf = (Mtx4f)BaseLayer.PMatrix;
                BaseLayer.PIMatrix = Mtx4.InvertP(BaseLayer.PMatrix);
                //BaseLayer.PIMatrix.Invert();
            }
            xRatio = (BaseLayer.PMatrix.Row2.Z + BaseLayer.PMatrix.Row3.Z) / BaseLayer.PMatrix.Row0.X;
            yRatio = (BaseLayer.PMatrix.Row2.Z + BaseLayer.PMatrix.Row3.Z) / BaseLayer.PMatrix.Row1.Y;
            //
            //

            ////
            //
        }

        private static void SwitchDrawCoordinates()
        {
            targetPanel.Visible = !targetPanel.Visible;
            viewerPanel.Visible = !viewerPanel.Visible;
            iconShowCoordinates.DeActivated = !targetPanel.Visible;
        }

        private static void SwitchDrawFaces()
        {
            DrawFaces = !DrawFaces; DefineDrawFunction();
            iconShowFaces.DeActivated = !DrawFaces;
        }

        private static void SwitchDrawIcons()
        {
            DrawIcons = !DrawIcons;
        }

        private static void SwitchDrawLines()
        {
            DrawLines = !DrawLines; DefineDrawFunction();
            iconShowLines.DeActivated = !DrawLines;
        }

        private static void SwitchDrawLogs()
        {
            logsPanel.Visible = !logsPanel.Visible;
        }

        private static void SwitchDrawMeasures()
        {
            DrawMeasures = !DrawMeasures;
        }

        private static void SwitchDrawPerformances()
        {
            performancesPanel.Visible = !performancesPanel.Visible;
            iconShowPerformances.DeActivated = !performancesPanel.Visible;
        }
        private static void SwitchDrawThreads()
        {
            threadsPanel.Visible = !threadsPanel.Visible;
            //iconShowPerformances.DeActivated = !threadsPanel.Visible;
        }

        private static void SwitchDrawPoints()
        {
            DrawPoints = !DrawPoints; DefineDrawFunction();
            iconShowPoints.DeActivated = !DrawPoints;
        }

        private static void SwitchEntitiesMoving()
        {
            EntitiesMoving = !EntitiesMoving;
            if (EntitiesMoving)
            {
                OwnerLink.SaveLocation();
            }
            else
            {
                OwnerLink.RestoreLocation();
            }
            iconMoveObjects.DeActivated = EntitiesMoving;
        }

        private static void SwitchMoveMode()
        {
            if (MiddleButton)
            {
                if (PreviousMoveMode == EMoveMode.Translation)
                {
                    PreviousMoveMode = EMoveMode.SideTranslation;
                }
                else
                {
                    PreviousMoveMode = EMoveMode.Translation;
                }
            }
            else
            {
                if (PreviousMoveMode == EMoveMode.Rotation)
                {
                    PreviousMoveMode = EMoveMode.Translation;
                }
                else
                {
                    PreviousMoveMode = EMoveMode.Rotation;
                }
            }
            iconMovingMode.DeActivated = (PreviousMoveMode == EMoveMode.Translation);
        }

        private static void SwitchPickerMode()
        {
            PickerMode = !PickerMode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UpdateLayout()
        {
            WidthCulling = CullingParameter * IWidth;
            HeightCulling = CullingParameter * IHeight;
            //
            graphTree.Height = (int)(Height - 50);
            graphTree.Width = (int)Math.Max(250, Width * 0.1);
            graphTree.OwnerLink.Giving.Matrix = Mtx4.CreateScale(graphTree.Width, 1, graphTree.Height) * CreateForeMatrix(0.99, -0.99 + 2 * graphTree.Height * IHeight, 0.001, 0.002);
            //
            linkPanel.Height = 300;
            linkPanel.Width = 220;
            linkPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(linkPanel.Width, 1, linkPanel.Height) * CreateForeMatrix(-0.99 + 2 * linkPanel.Width * IWidth, -0.99 + 2 * linkPanel.Height * IHeight, 0.001, 0.002);

            foreach (TIconBar ib in TControl.Controls.Values.OfType<TIconBar>())
            {
                int w = 4;
                foreach (IIcon i in ib.Icons)
                {
                    w += i.Width + 4;
                }
                ib.Width = w;
                w -= 8;
                if (ib.IconBarOwner == null)
                {
                    ib.OwnerLink.Giving.Matrix = Mtx4.CreateScale(ib.Width, 1, 4) * CreateForeMatrix(ib.Width * IWidth, 0.99, 0.001, 0.1);
                    foreach (IIcon i in ib.Icons)
                    {
                        i.OwnerLink.Giving.Matrix = Mtx4.CreateScale(i.Width, 1, i.Height) * CreateForeMatrix(w * IWidth, 0.99 - 6 * IHeight, 0.001, 0.002);
                        w -= 2 * i.Width + 8;
                    }
                }
                else
                {
                    ib.OwnerLink.Giving.Matrix = Mtx4.CreateScale(ib.Width, 1, 4) * CreateForeMatrix(ib.Width * IWidth, 0.99 - 2.2 * ib.Height * IHeight, 0.001, 0.1);
                    foreach (IIcon i in ib.Icons)
                    {
                        i.OwnerLink.Giving.Matrix = Mtx4.CreateScale(i.Width, 1, i.Height) * CreateForeMatrix(w * IWidth, 0.99 - 2.2 * ib.Height * IHeight - 6 * IHeight, 0.001, 0.002);
                        w -= 2 * i.Width + 8;
                    }
                }
            }

            performancesPanel.Height = 200;
            performancesPanel.Width = 400;
            performancesPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(performancesPanel.Width, 1, performancesPanel.Height) * CreateForeMatrix(performancesPanel.Width * IWidth, 0.99 - 160 * IHeight, 0.001, 0.002);
            threadsPanel.Height = 200;
            threadsPanel.Width = 400;
            threadsPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(threadsPanel.Width, 1, threadsPanel.Height) * CreateForeMatrix(threadsPanel.Width * IWidth, 0.99 - 160 * IHeight, 0.001, 0.002);

            targetPanel.Height = 80;
            targetPanel.Width = 110;
            targetPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(targetPanel.Width, 1, targetPanel.Height) * CreateForeMatrix(0.01 + 2 * targetPanel.Width * IWidth, -0.99 + 2 * targetPanel.Height * IHeight, 0.001, 0.002);

            viewerPanel.Height = targetPanel.Height;
            viewerPanel.Width = targetPanel.Width;
            viewerPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(viewerPanel.Width, 1, viewerPanel.Height) * CreateForeMatrix(-0.01, -0.99 + 2 * viewerPanel.Height * IHeight, 0.001, 0.002);

            overflownPanel.Height = targetPanel.Height;
            overflownPanel.Width = targetPanel.Width;

            loadingPanel.Height = 16;
            loadingPanel.Width = 50;
            loadingPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(loadingPanel.Width, 1, loadingPanel.Height) * CreateForeMatrix(0.99, 0.99, 0.001, 0.002);

            commandsPanel.Height = 80;
            commandsPanel.Width = 200;
            commandsPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(commandsPanel.Width, 1, commandsPanel.Height) * CreateForeMatrix(0.02 + 2 * targetPanel.Width * IWidth + 2 * commandsPanel.Width * IWidth, -0.99 + 2 * targetPanel.Height * IHeight, 0.001, 0.002);

            logsPanel.Height = 800;
            logsPanel.Width = 600;
            logsPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(logsPanel.Width, 1, logsPanel.Height) * CreateForeMatrix(0.02 + 0 * commandsPanel.Width * IWidth + 0 * logsPanel.Width * IWidth, -0.99 + 2 * logsPanel.Height * IHeight, 0.001, 0.002);
        }

        #endregion Private Methods

        #region Public Classes

        public class TLinkTransparencyComparer : IComparer<SLink>
        {
            #region Public Fields

            public static readonly TLinkTransparencyComparer Default = new TLinkTransparencyComparer();

            #endregion Public Fields

            #region Public Methods

            public int Compare(SLink x, SLink y)
            {
                if (x.Link == null)
                {
                    if (y.Link == null)
                        return 0;
                    else
                        return true.CompareTo(false);
                }
                else if (y.Link == null)
                    return false.CompareTo(true);
                else
                    return x.Link.Transparent.CompareTo(y.Link.Transparent);
            }

            #endregion Public Methods
        }

        #endregion Public Classes
    }
}