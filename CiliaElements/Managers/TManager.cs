
using Math3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CiliaElements
{
    static partial class TManager
    {
        //public static ComputeContext CLContext;
        //public static ComputePlatform CLPlatform;
        //public static ComputeContextPropertyList CLProperties;

        //public static TCLTools CLTools;

        #region Public Constructors

        static TManager()
        {
            //CLPlatform = ComputePlatform.Platforms[0];
            //CLProperties = new ComputeContextPropertyList(CLPlatform);
            //CLContext = new ComputeContext(CLPlatform.Devices, CLProperties, null, IntPtr.Zero);
            //CLTools = new TCLTools(CLContext);

            //Mtx4 m = Mtx4.CreateTranslation(1, -2, 3) * Mtx4.CreateRotationX(1) * Mtx4.CreateRotationY(1) * Mtx4.CreateRotationZ(1);
            //m.Row2 = new Vec4();
            //Proj = m;
            //Proj.Value.Invert();

            ClickButton0 = DoClickButtonNull;
            ClickButton1 = DoClickButtonNull;
            ClickButton2 = DoClickButtonNull;
            ClickButton3 = DoClickButtonNull;
            ClickButton4 = DoClickButtonNull;
            ClickButton5 = DoClickButtonNull;
            ClickButton6 = DoClickButtonNull;
            ClickButton7 = DoClickButtonNull;
            ClickButton8 = DoClickButtonNull;
            ClickButton9 = DoClickButtonNull;
            ClickButton10 = DoClickButtonNull;
            RestorePadCommands();
            //
            ColorAllBlack = new Mtx4f() { Row3 = new Vec4f(0, 0, 0, 1) };
            ColorAllWhite = new Mtx4f() { Row3 = new Vec4f(1, 1, 1, 1) };
            ColorMeasure = new Mtx4f(0.1F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 0.1F, 0, 0, 0, 0, 1F);
            //Characters = new TSolidElement[256];

            MaxLoadingFiles = Environment.ProcessorCount - 1;

            DefineDrawFunction();

            Keys = new List<OpenTK.Input.Key>((OpenTK.Input.Key[])Enum.GetValues(typeof(OpenTK.Input.Key)));
            UsedKeys = new bool[Keys.Count];
            //
            Keys.Remove(OpenTK.Input.Key.ControlLeft);
            Keys.Remove(OpenTK.Input.Key.ControlRight);

            KeyPressedEvent += KeyPressed;
            Layers = new TLayer[LayersNumber];
            double zmin = MinNear;
            double zmax = zmin * FarRatio;
            for (int i = LayersNumber - 1; i > -1; i--)
            {
                Layers[i] = new TLayer() { NearDistance = zmin, FarDistance = zmax };
                zmin *= FarRatio;
                zmax *= FarRatio;
            }
            BaseLayer = new TLayer() { NearDistance = MinNear / FarRatio, FarDistance = MinNear };
            //MaxNear = Layers[LayersNumber - 1].ZMin;
            ResetPerspectives();
            //

            SelectedLinkChanged += TManager_SelectedLinkChanged;

            ConsoleWriter = new System.IO.StreamWriter(DocumentsDirectory.FullName + "\\" + Process.GetCurrentProcess().Id.ToString() + ".log") { AutoFlush = true };
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void NodeClickedEventHandler(TLink ClickedLink);

        public delegate void PointClickedEventHandler(TLink ClickedLink, Vec3? ClickedPoint);

        public delegate void RequestOpenFilesEventHandler(List<string> iFiles);

        public delegate void RequestSwitchBordersEventHandler();

        public delegate void RequestSwitchWindowStateEventHandler();

        #endregion Public Delegates

        #region Private Delegates

        private delegate void DrawSolidCB(TSolidElement s);

        public delegate void KeyPressedEventHandler(OpenTK.Input.Key iKey, EKeybordModifiers iModifiers);

        private delegate void SelectedLinkChangedEventHandler(TLink iSelectedLink);

        private delegate void StructureUpdatedEventHandler();

        #endregion Private Delegates

        #region Public Events

        public static event NodeClickedEventHandler NodeClicked;

        public static event PointClickedEventHandler PointClicked;

        public static event RequestOpenFilesEventHandler RequestOpenFiles;

        public static event RequestSwitchBordersEventHandler RequestSwitchBorders;

        public static event RequestSwitchWindowStateEventHandler RequestSwitchWindowState;

        #endregion Public Events

        #region Private Events

        public static event KeyPressedEventHandler KeyPressedEvent;

        private static event SelectedLinkChangedEventHandler SelectedLinkChanged;

        #endregion Private Events

        #region Public Properties

        public static TFile[][] FilesByStates
        {
            get
            {
                TFile[][] tbl = { };
                Array.Resize(ref tbl, ElementStates.Length);
                //
                for (int i = 0; i < ElementStates.Length; i++)
                {
                    tbl[i] = Array.FindAll(UsedFiles.Values, o => o != null && o.Element != null && o.Element.State == ElementStates[i] && !(o.Element is TInternal));
                }

                return tbl;
            }
        }

        public static double FilesVolume
        {
            get
            {
                long l = 0;
                foreach (TFile file in UsedFiles.Values.Where(o => o != null && o.Element != null && o.Element.State == EElementState.Pushed && !(o.Element is TInternal) && o.Element.Fi!=null && o.Element.Fi.Exists))
                {
                    l += file.Element.Fi.Length;
                }

                return l;
            }
        }

        public static double Height
        {
            get { return height; }
            set { if (height == value) { return; } height = value; HeightInt = (int)height; ResetPerspectives(); UpdateLayout(); }
        }

        public static TLink OverFlownLink
        {
            get { return overFlownLink; }
            set
            {
                if (overFlownLink == value)
                {
                    return;
                }

                if (overFlownLink != null)
                {
                    overFlownLink.State ^= ELinkState.OverFlown;
                }

                overFlownLink = value;
                if (overFlownLink != null)
                {
                    overFlownLink.State |= ELinkState.OverFlown;
                }
            }
        }

        public static TLink[] SelectedLinks
        {
            get { return selectedLinks.ToArray(); }
            //set
            //{
            //    if (object.ReferenceEquals(_SelectedLink, value))
            //        return;
            //    if (_SelectedLink != null)
            //        _SelectedLink.DeActivate();

            //    _SelectedLink = value;

            //    if (_SelectedLink != null)
            //        _SelectedLink.Activate();

            //    if (SelectedLinkChanged != null)
            //    {
            //        SelectedLinkChanged(value);
            //    }
            //}
        }

        public static TAssemblyElement View { get; private set; }

        public static double Width
        {
            get { return width; }
            set { if (width == value) { return; } width = value; WidthInt = (int)width; ResetPerspectives(); UpdateLayout(); }
        }

        #endregion Public Properties

        #region Private Properties

        private static Vec2 Cursor
        {
            set
            {
                cursor = value;
                Mtx4 RZ = Mtx4.SwitchXY;
                double a = Math.Atan2(cursor.X * Width, cursor.Y * Height);
                CursorMatrix = RZ * Mtx4.CreateRotationY(a + Math.PI) * CreateForeMatrix(-cursor.X, -cursor.Y, 0.15, 0.2);
                //

                Vec4 V = new Vec4(0, 0, SelectedLayer.FarDistance, 1);
                V = Vec4.Transform(V, SelectedLayer.PMatrix);
                V.X = V.Z * cursor.X;
                V.Y = V.Z * cursor.Y;
                V = Vec4.Transform(V, SelectedLayer.PVIMatrix);
                //
                CursorPoint = (Vec3)V;

                //
                if (MovingPoint.HasValue)
                {
                    a = Math.Atan2((cursor.X - MovingCursor.X) * Width, (cursor.Y - MovingCursor.Y) * Height);
                    MovingCursorMatrix = RZ * Mtx4.CreateRotationY(a) * CreateForeMatrix(-MovingCursor.X, -MovingCursor.Y, 0.15, 0.2);
                }

            }
            get { return cursor; }
        }

        private static int[] FilesStates
        {
            get
            {
                int[] tbl = { };
                Array.Resize(ref tbl, ElementStates.Length);
                //
                for (int i = 0; i < ElementStates.Length; i++)
                {
                    tbl[i] = UsedFiles.Values.Count(o => o.Element != null && o.Element.State == ElementStates[i] && !(o.Element is TInternal));
                }

                return tbl;
            }
        }

        private static bool Loading
        {
            get
            {
                return (UsedFiles.Values.FirstOrDefault(o => o != null && o.Element != null && o.Element.State != EElementState.Pushed) != null);
            }
        }

        public static double PerspectiveAngle
        {
            get { return perspectiveAngle; }
            set
            {
                if (value <= 0.02)
                {
                    value = 0.02;
                }
                else if (value > 1.55)
                {
                    value = 1.55;
                }
                iconSetPerspective.Value = value;
                perspectiveAngle = value;
                ResetPerspectives();
                UpdateLayout();
            }
        }

        private static Vec2? Touch1
        {
            set
            {
                touch1 = value;
                if (touch1 == null)
                {
                    return;
                }

                Vec2 v = new Vec2(touch1.Value.X - Left, touch1.Value.Y - Top);
                v.X = +2 * v.X * IWidth - 1;
                v.Y = -2 * v.Y * IHeight + 1;
                touch1 = v;
                //
                if (touch2 != null)
                {
                    v = (v + touch2.Value) * 0.5;
                }

                Mtx4 RZ = Mtx4.SwitchXY;
                double a = Math.Atan2(v.X * Width, v.Y * Height);
                TouchMatrix = RZ * Mtx4.CreateRotationY(a + Math.PI) * CreateForeMatrix(-v.X, -v.Y, 0.15, 0.2);
                //
                Vec4 V = new Vec4(0, 0, SelectedLayer.FarDistance, 1);
                V = Vec4.Transform(V, SelectedLayer.PMatrix);
                V.X = V.Z * touch1.Value.X;
                V.Y = V.Z * touch1.Value.Y;
                V = Vec4.Transform(V, SelectedLayer.PVIMatrix);
                //
                Touch1Point = (Vec3)V;
                //
                if (MovingPoint.HasValue)
                {
                    a = Math.Atan2((cursor.X - MovingCursor.X) * Width, (cursor.Y - MovingCursor.Y) * Height);
                    MovingCursorMatrix = RZ * Mtx4.CreateRotationY(a) * CreateForeMatrix(-MovingCursor.X, -MovingCursor.Y, 0.15, 0.2);
                }
            }
            get { return touch1; }
        }

        private static Vec2? Touch2
        {
            set
            {
                touch2 = value;

                if (touch2 == null)
                {
                    Vec2 min = new Vec2(double.MaxValue, double.MaxValue);
                    Vec2 max = new Vec2(double.MinValue, double.MinValue);
                    if (GlyphePoints.Max > 0)
                    {
                        Mtx4[] ms = GlyphePoints.Values.Where(o => o != null).ToArray();

                        //PerformedGlyphePoints.Reset();
                        foreach (Mtx4 m in ms)
                        {
                            if (m.Row3.X < min.X)
                            {
                                min.X = m.Row3.X;
                            }
                            else if (m.Row3.X > max.X)
                            {
                                max.X = m.Row3.X;
                            }

                            if (m.Row3.Y < min.Y)
                            {
                                min.Y = m.Row3.Y;
                            }
                            else if (m.Row3.Y > max.Y)
                            {
                                max.Y = m.Row3.Y;
                            }
                        }
                        Vec2 mid = (min + max) * 0.5;
                        //min = min + 0.2 * (mid - min);
                        //max = max - 0.2 * (mid - max);
                        int[] tbl = { 0, 0, 0, 0, 0, 0 };
                        Vec2 stp = (max - min) * 0.5;
                        List<Vec2> vs = new List<Vec2>();
                        foreach (Mtx4 m in ms)
                        {
                            Mtx4 M = m;
                            Vec4 r = M.Row3;
                            Vec2 p = ((Vec2)r - min) / stp;
                            Vec2 d = p;
                            p.X = Math.Round(p.X);
                            p.Y = Math.Round(p.Y);
                            d -= p;
                            if (Math.Abs(d.X) > 0.1 || Math.Abs(d.Y) > 0.1)
                            {
                                if (d.X * d.X > d.Y * d.Y) { d.Y = 0; } else if (d.X * d.X < d.Y * d.Y) { d.X = 0; } else { d.X = 0; d.Y = 0; }
                                p += d;
                                if (vs.FirstOrDefault(o => (o - p).LengthSquared < 0.01) == default(Vec2))
                                {
                                    vs.Add(p);
                                    if (d.X == 0)
                                    {
                                        tbl[(int)p.X]++;
                                    }

                                    if (d.Y == 0)
                                    {
                                        tbl[3 + (int)p.Y]++;
                                    }

                                    p = min + stp * p;
                                    r.X = p.X;
                                    r.Y = p.Y;
                                    M.Row3 = r;
                                    //try
                                    //{
                                    //    PerformedGlyphePoints.Push(M);
                                    //}
                                    //catch (Exception ex) { Console.WriteLine(ex.Message); }
                                }
                            }
                        }
                        byte b = 0;
                        foreach (int i in tbl)
                        {
                            b *= 2;
                            Console.Write(i.ToString() + " ");
                            if (i > 2)
                            {
                                b++;
                            }
                        }
                        Console.Write(b.ToString() + "\n");
                        switch (b)
                        {
                            case 45: OpenFiles(); break;
                            case 57: FitAll(); break;
                            case 60: FitSelected(); break;
                            case 20: FitBottom(); break;
                            case 17: FitTop(); break;
                            case 34: FitLeft(); break;
                            case 10: FitRight(); break;
                            case 51: FitFront(); break;
                            case 30: FitRear(); break;
                        }
                        GlyphePoints.Reset();
                    }
                    return;
                }

                Vec2 v = new Vec2(touch2.Value.X - Left, touch2.Value.Y - Top);
                v.X = +2 * v.X * IWidth - 1;
                v.Y = -2 * v.Y * IHeight + 1;
                touch2 = v;
                //
                if (touch1 != null)
                {
                    v = (v + touch1.Value) * 0.5;
                }

                Mtx4 RZ = Mtx4.SwitchXY;
                double a = Math.Atan2(v.X * Width, v.Y * Height);
                TouchMatrix = RZ * Mtx4.CreateRotationY(a + Math.PI) * CreateForeMatrix(-v.X, -v.Y, 0.15, 0.2);
                //
                Vec4 V = new Vec4(0, 0, SelectedLayer.FarDistance, 1);
                V = Vec4.Transform(V, SelectedLayer.PMatrix);
                V.X = V.Z * touch2.Value.X;
                V.Y = V.Z * touch2.Value.Y;
                V = Vec4.Transform(V, SelectedLayer.PVIMatrix);
                //
                Touch2Point = (Vec3)V;
                //
                if (MovingPoint.HasValue)
                {
                    a = Math.Atan2((cursor.X - MovingCursor.X) * Width, (cursor.Y - MovingCursor.Y) * Height);
                    MovingCursorMatrix = RZ * Mtx4.CreateRotationY(a) * CreateForeMatrix(-MovingCursor.X, -MovingCursor.Y, 0.15, 0.2);
                }
            }
            get { return touch2; }
        }

        #endregion Private Properties

        #region Public Methods

        public static void RestorePadCommands()
        {
            MoveAxis0 = DoMoveAxis0;
            MoveAxis1 = DoMoveAxis1;
            MoveAxis2 = DoMoveAxis2;
            MoveAxis3 = DoMoveAxis3;

            ClickButton4 = DoClickButton4;
            ClickButton5 = DoClickButton5;
            ClickButton6 = DoClickButton6;
            ClickButton7 = DoClickButton7;
        }

        #endregion Public Methods

        #region Private Methods

        private static void TManager_SelectedLinkChanged(TLink iSelectedLink)
        {
            if (selectedLinks.Count == 0)
            {
                linkPanel.DisplayedLink = null;
                linkPanel.Visible = false;
            }
            else
            {
                linkPanel.DisplayedLink = iSelectedLink;
                linkPanel.ToRedraw = true;
                linkPanel.Visible = true;
            }
        }

        #endregion Private Methods
    }
}