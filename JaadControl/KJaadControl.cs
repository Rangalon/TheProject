using JaadControl;
using Math3D;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace JaadWinControls
{
    public partial class KJaadControl : GLControl.GLControl
    {
        #region Public Fields

        public static Vec3f BckColor = new Vec3f(0, 0, 0);
        public List<TCreator> Creators = new List<TCreator>();

        public Vec2f CursorPoint;
        public Thread DoerDrawThread;
        public long Duration = 0;
        public TFilterCreator FilterCreator;
        public float MaxSize;

        public Dictionary<object, Vec3ft[]> ObjectLineStrips = new Dictionary<object, Vec3ft[]>();
        public Dictionary<object, Vec3ft[]> ObjectStrips = new Dictionary<object, Vec3ft[]>();
        public Vec2f Offset = new Vec2f(0, 0);

        public object OverflownObject;
        public Recall Recaller;
        public List<object> RecallerObjets = new List<object>();
        public float ScaleFactor = 0.001F;

        public List<object> SelectedObjects = new List<object>();

        #endregion Public Fields

        #region Protected Fields

        protected bool mouseMoved;

        #endregion Protected Fields

        #region Private Fields

        private static readonly float Ratio = 2F;

        private readonly TLabel TipLabel;
        private bool CtrlDown;
        private float dpix;

        private float dpiy;
        private bool GLInitialized = false;

        private bool liteMode = true;

        private bool MiddleDown = false;

        private Vec2f? MousePoint = null;

        private MouseEventArgs MousePointEventArgs = null;

        private bool ShftDown;

        #endregion Private Fields

        #region Public Constructors

        public KJaadControl()
        {
            InitializeComponent();

            if (DesignMode)
            {
                BackgroundImageLayout = ImageLayout.Zoom;
                return;
            }

            SetStyle(ControlStyles.Selectable, true);

            Load += KGLControl_Load;

            this.Focus();

            this.Resize += this.KJaadControl_Resize;

            MouseMove += MouseMoved;
            MouseUp += MouseUped;
            MouseDown += MouseDowned;
            MouseClick += MouseClicked;
            MouseWheel += MouseWheeled;
            KeyDown += KeyDowned;
            KeyUp += KeyUpped;
            TButton z;
            z = new TButton() { Control = this, Do = ZoomIn, Picture = JaadControl.Properties.Resources.baseline_zoom_in_black_18dp, Visible = true };
            z.Tip = "Zoom in";
            z.Rect = new RectangleF(0.030F, 0.055F, 0.015F, -0.015F);
            Creators.Add(z);
            z = new TButton() { Control = this, Do = ZoomOut, Picture = JaadControl.Properties.Resources.baseline_zoom_out_black_18dp, Visible = true };
            z.Tip = "Zoom out";
            z.Rect = new RectangleF(0.005F, 0.055F, 0.020F, -0.015F);
            Creators.Add(z);
            z = new TButton() { Control = this, Do = ZoomO, Picture = JaadControl.Properties.Resources.origin, Visible = true };
            z.Tip = "Go to Origin";
            z.Rect = new RectangleF(0.030F, 0.075F, 0.015F, -0.015F);
            Creators.Add(z);
            z = new TButton() { Control = this, Do = ZoomAll, Picture = JaadControl.Properties.Resources.baseline_zoom_out_map_black_18dp, Visible = true };
            z.Tip = "Show all";
            z.Rect = new RectangleF(0.005F, 0.075F, 0.020F, -0.015F);
            Creators.Add(z);
            z = new TButton(176, 94) { Control = this, Picture = JaadControl.Properties.Resources.Galon_s_Lab, Visible = true };
            z.Tip = "Galon's Lab";
            z.Rect = new RectangleF(0.005F, 0.035F, 0.020F, -0.030F);
            Creators.Add(z);
            TipLabel = new TLabel(4, 40) { Control = this, Visible = true, Text = "-" };
            TipLabel.Rect = new RectangleF(0.065F, 0.035F, 0.3F, -0.03F);
            Creators.Add(TipLabel);
            FilterCreator = new TFilterCreator() { Control = this };
            FilterCreator.Tip = "Highlight searched equipments";
            FilterCreator.Rect = new RectangleF(0.15F + 0.05F * Creators.FindAll(o => o.Rect.Top == 0.03F).Count, 0.03F, 0.09F, -0.01F);
            Creators.Add(FilterCreator);

            Thread th = new Thread(WaitForParentForm); th.Start();
        }

        #endregion Public Constructors

        #region Public Enums

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        #endregion Public Enums

        #region Public Properties

        public virtual bool LiteMode
        {
            get
            {
                return liteMode;
            }
            set
            {
                liteMode = value;
            }
        }

        public object SelectedObject
        {
            get
            {
                if (SelectedObjects.Count == 0)
                {
                    return null;
                }

                return SelectedObjects[SelectedObjects.Count - 1];
            }
            set
            {
                if (!CtrlDown)
                {
                    SelectedObjects.Clear();
                }

                if (value != null && !SelectedObjects.Contains(value))
                {
                    SelectedObjects.Add(value);
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static Vec3ft[] GetLoop(Vec3ft[] vs)
        {
            List<Vec3ft> lst = new List<Vec3ft>();
            int i = 0;
            while (i < vs.Length)
            {
                lst.Add(vs[i]); i += 2;
            }
            //
            if (i == vs.Length)
            {
                i -= 1;
            }
            else
            {
                i -= 3;
            }
            //
            while (i > 0)
            {
                lst.Add(vs[i]); i -= 2;
            }
            return lst.ToArray();
        }

        public static Vec3ft[] GetStrip(RectangleF r, float layer)
        {
            Vec3ft[] v = new Vec3ft[]
            {
                new Vec3ft(r.Left,r.Top,layer,0,0),
                new Vec3ft(r.Left,r.Bottom,layer,0,1),
                new Vec3ft(r.Right,r.Top,layer,1,0),
                new Vec3ft(r.Right,r.Bottom,layer,1,1)
            };
            return v;
        }

        public static Vec3ft[] GetStrip(RectangleF r)
        {
            return GetStrip(r, 0);
        }

        public void ClearRecaller()
        {
            Recaller = null;
            RecallerObjets.Clear();
        }

        public virtual void DoerFrame()
        {
        }

        public virtual object[] GetFoundObject()
        {
            return new object[] { };
        }

        public Vec2f[] GetStrip(Vec2f[] pts)
        {
            List<Vec2f> lst = new List<Vec2f>();
            foreach (Vec2f p in pts)
            {
                Vec2f v = p;
                TransformPoint(ref v);
                lst.Add(v);
            }
            pts = lst.ToArray();
            lst.Clear();
            int j = 1;
            for (int i = 0; i < pts.Length - 2; i++)
            {
                lst.Add(pts[0]);
                lst.Add(pts[j]);
                j++;
                lst.Add(pts[j]);
            }
            return lst.ToArray();
        }

        public Vec3ft[] GetStrip(Vec3ft[] pts)
        {
            List<Vec3ft> lst = new List<Vec3ft>();
            foreach (Vec3ft p in pts)
            {
                Vec3ft v = p;
                TransformPoint(ref v);
                lst.Add(v);
            }
            pts = lst.ToArray();
            lst.Clear();
            int j = 1;
            for (int i = 0; i < pts.Length - 2; i++)
            {
                lst.Add(pts[0]);
                lst.Add(pts[j]);
                j++;
                lst.Add(pts[j]);
            }
            return lst.ToArray();
        }

        public Vec3f[] GetTriangles(Vec3f[] path, float PathWidth, float Layer)
        {
            List<Vec3f> lstt = new List<Vec3f>();
            Vec3f vn;
            Vec3f vd = new Vec3f();
            vn = (path[1] - path[0]).Normalized();
            vd.X = -vn.Y; vd.Y = vn.X;
            Vec3f v;
            v = path[0] + 0.1F * vd * PathWidth; TransformPoint(ref v, Layer); lstt.Add(v);
            v = path[0] - 0.1F * vd * PathWidth; TransformPoint(ref v, Layer); lstt.Add(v);
            for (int i = 0; i < path.Length - 2; i++)
            {
                vn = (path[i + 1] - path[i]).Normalized();
                vd.X = -vn.Y; vd.Y = vn.X;
                vd *= PathWidth;
                v = (path[i + 1] - path[i]).Normalized() + (path[i + 1] - path[i + 2]).Normalized();
                double k = (v.Y * vd.X - v.X * vd.Y) / (v.X * vn.Y - v.Y * vn.X);// (vd.X / v.X - vn.X * vd.Y / vn.Y) / (1 - vn.X * v.Y / vn.Y);
                v = path[i + 1] + vd + (float)k * vn; TransformPoint(ref v, Layer); lstt.Add(v);
                v = path[i + 1] - vd - (float)k * vn; TransformPoint(ref v, Layer); lstt.Add(v);
            }
            vn = (path[path.Length - 1] - path[path.Length - 2]).Normalized();
            vd.X = -vn.Y; vd.Y = vn.X;
            v = path[path.Length - 1] + 0.1F * vd * PathWidth; TransformPoint(ref v, Layer); lstt.Add(v);
            v = path[path.Length - 1] - 0.1F * vd * PathWidth; TransformPoint(ref v, Layer); lstt.Add(v);
            return lstt.ToArray();
        }

        public virtual void KGLControl_Load(object sender, System.EventArgs e)
        {
            if (DesignMode)
            {
                return;
            }

            RegisterTouchWindow(this.Handle, 0);

            Load -= KGLControl_Load;

            //Application.Idle += IsIdled;

            if (GLInitialized)
            {
                return;
            }
            //
            //GL.ClearColor(0, 0, 0, 1);//Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.SampleAlphaToCoverage);
            //
            GLInitialized = true;
            //
            DoerDrawThread = new Thread(DoerDraw) { Priority = ThreadPriority.Highest }; DoerDrawThread.Start();
        }

        public virtual void MouseDowned(object sender, MouseEventArgs e)
        {
            MousePoint = new Vec2f(e.X, e.Y);
            MousePointEventArgs = e;
            MiddleDown = (e.Button == MouseButtons.Middle);
        }

        public virtual void MouseMoved(object sender, MouseEventArgs e)
        {
            if (MousePoint.HasValue)
            {
                mouseMoved = true;
                if (e.Button == MouseButtons.Left)
                {
                    float dx = (MousePoint.Value.X - e.X) / (ScaleFactor * MaxSize);
                    float dy = (MousePoint.Value.Y - e.Y) / (ScaleFactor * MaxSize);
                    if (OverflownObject != null)
                    {
                        if (!SelectedObjects.Contains(OverflownObject))
                        {
                            if (!CtrlDown)
                            {
                                SelectedObjects.Clear();
                            }

                            SelectedObjects.Add(OverflownObject);
                        }
                        foreach (object o in SelectedObjects)
                        {
                            MoveActiveObject(o, dx, dy);
                        }
                    }
                    else
                    {
                        Offset.X -= dx;
                        Offset.Y -= dy;
                    }
                    MousePoint = new Vec2f(e.X, e.Y);
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    Vec2f v1 = Untransform(MousePointEventArgs);
                    Vec2f v0 = Untransform(e);
                    if ((v0 - v1).LengthSquared > 100)
                    {
                        v0 *= 1;
                    }

                    TransformPoint(ref v0);
                    TransformPoint(ref v1);
                    Vec2f v2 = new Vec2f(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y));
                    Vec2f v3 = new Vec2f(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y));
                    SelectedObjects.Clear();
                    foreach (object oo in ObjectStrips.Keys)
                    {
                        if (ObjectStrips[oo].FirstOrDefault(o => o.X >= v3.X && o.X <= v2.X && o.Y >= v3.Y && o.Y <= v2.Y).LengthSquared > 0)
                        {
                            SelectedObjects.Add(oo);
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    Vec2f v1 = Untransform(MousePointEventArgs);
                    float k = 32 * (MousePoint.Value.Y - e.Y) / Height;
                    if (k > 0.9F)
                    {
                        k = 0.9F;
                    }

                    if (k < -9)
                    {
                        k = -9;
                    }

                    ScaleFactor *= 1 - k;
                    Vec2f v2 = Untransform(MousePointEventArgs);
                    Offset.X += v2.X - v1.X;
                    Offset.Y += v2.Y - v1.Y;
                    MousePoint = new Vec2f(e.X, e.Y);
                }
            }
            else
            {
                CursorPoint.X = e.X / (float)Width / ScaleFactor - Offset.X;
                CursorPoint.Y = (e.Y + Width - Height) / (float)Width / ScaleFactor - Offset.Y;

                Vec3ft v = new Vec3ft(e.X, Height - e.Y, 0, 0, 0);

                v.X /= MaxSize;
                v.Y /= MaxSize;
                //v.X = v.X / Scale - Offset.X;
                //v.Y = (1-v.Y) / Scale - Offset.Y;
                OverflownObject = null;
                //
                this.Cursor = Cursors.Default;
                foreach (TCreator c in Creators)
                {
                    if (c.Visible && v.X > c.Rect.Left * dpix && v.X < c.Rect.Right * dpix && v.Y < c.Rect.Top * dpiy && v.Y > c.Rect.Bottom * dpiy)
                    {
                        OverflownObject = c;
                        break;
                    }
                }

                if (OverflownObject == null)
                {
                    float zmax = float.MaxValue;
                    foreach (object o in ObjectStrips.Keys.ToArray())
                    {
                        if (ObjectStrips.ContainsKey(o))
                        {
                            Vec3ft[] vs = ObjectStrips[o];
                            if (vs.Length > 2)
                            {
                                Vec3ft v0;
                                Vec3ft v1 = vs[0];
                                Vec3ft v2 = vs[1];
                                for (int i = 2; i < vs.Length; i++)
                                {
                                    v0 = v1; v1 = v2; v2 = vs[i];
                                    Vec3ft va = v1 - v0;
                                    Vec3ft vb = v2 - v0;
                                    Vec3ft vm = v - v0;
                                    double d = va.X * vb.Y - va.Y * vb.X;
                                    double a = (+vb.Y * vm.X - vb.X * vm.Y) / d;
                                    double b = (-va.Y * vm.X + va.X * vm.Y) / d;
                                    if (a > 0 && b > 0 && a < 1 && b < 1 && a + b < 1)
                                    {
                                        if (OverflownObject == null || v0.Z < zmax)
                                        {
                                            OverflownObject = o;
                                            zmax = v0.Z;
                                        }
                                    }
                                }
                                //if (OverflownObject != null)
                                //{
                                //    return;
                                //}
                            }
                        }
                    }
                    foreach (object o in ObjectLineStrips.Keys.ToArray())
                    {
                        if (ObjectLineStrips.ContainsKey(o))
                        {
                            Vec3ft[] vs = ObjectLineStrips[o];
                            if (vs.Length > 1)
                            {
                                Vec3ft v0;
                                Vec3ft v1 = vs[0];
                                for (int i = 1; i < vs.Length && OverflownObject == null; i++)
                                {
                                    v0 = v1; v1 = vs[i];
                                    Vec3ft va = v1 - v0;
                                    if (va.LengthSquared > 0)
                                    {
                                        float l = va.Length;
                                        va /= l;
                                        Vec3ft vm = v - v0;
                                        double d = va.X * -va.X - va.Y * va.Y;
                                        double a = (-va.X * vm.X - va.Y * vm.Y) / d;
                                        double b = (-va.Y * vm.X + va.X * vm.Y) / d;
                                        if (a > -5 * ScaleFactor && b > -5 * ScaleFactor && a < l + 5 * ScaleFactor && b < 5 * ScaleFactor)
                                        {
                                            OverflownObject = o;
                                        }
                                    }
                                }
                                //if (OverflownObject != null)
                                //{
                                //    return;
                                //}
                            }
                        }
                    }
                }

                if (OverflownObject == null)
                {
                    this.Cursor = Cursors.Default;
                    TipLabel.Visible = false;
                }
                else
                {
                    this.Cursor = Cursors.Hand;
                    if (OverflownObject is ITip i) { TipLabel.Text = i.Tip; TipLabel.Visible = TipLabel.Text != null; }
                    else
                    {
                        TipLabel.Visible = false;
                    }
                }
            }
        }

        public virtual void MouseUped(object sender, MouseEventArgs e)
        {
            MousePoint = null;
            MiddleDown = false;
            mouseMoved = false;
        }

        public virtual void MoveActiveObject(object o, float dx, float dy)
        { }

        public void TakeScreenshot()
        {
            if (GraphicsContext.CurrentContext == null)
            {
                throw new GraphicsContextMissingException();
            }

            int w = ClientSize.Width;
            int h = ClientSize.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //bmp.Save("c:\\temp\\test.png", System.Drawing.Imaging.ImageFormat.Png);
            ParentForm.BackgroundImage = bmp;
            Visible = false;
        }

        public Vec3ft[] TransformLineStrip(PointF[] pts)
        {
            Vec3ft[] vs = new Vec3ft[pts.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                Vec3ft v = new Vec3ft(pts[i].X, pts[i].Y, 0, 0, 0);
                v += Offset;
                v *= ScaleFactor;
                v.Y = 1 - v.Y;
                vs[i] = v;
            }

            return vs;
        }

        public void TransformPoint(ref Vec3f v, float layer)
        {
            v += Offset;

            v *= ScaleFactor;

            v.Y = 1 - v.Y;
            v.Z = layer;
        }

        public void TransformPoint(ref Vec3ft v, float layer)
        {
            v += Offset;

            v *= ScaleFactor;

            v.Y = 1 - v.Y;
            v.Z = layer;
        }

        public void TransformPoint(ref Vec2f v)
        {
            v += Offset;

            v *= ScaleFactor;

            v.Y = 1 - v.Y;
        }

        public void TransformPoint(ref Vec3ft v)
        {
            v += Offset;

            v *= ScaleFactor;

            v.Y = 1 - v.Y;
        }

        public void TransformPoints(ref Vec3f[] r)
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i] += Offset;
                r[i] *= ScaleFactor;
                r[i].Y = 1 - r[i].Y;
            }
        }

        public void TransformPoints(ref Vec3ft[] r)
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i] += Offset;
                r[i] *= ScaleFactor;
                r[i].Y = 1 - r[i].Y;
            }
        }

        public void TransformPoints(ref Vec3ft[] r, ILocalisable p)
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i].X += p.X;
                r[i].Y += p.Y;
            }
            TransformPoints(ref r);
        }

        public void TransformPoints(ref Vec3ft[] r, float x, float y)
        {
            for (int i = 0; i < r.Length; i++)
            {
                r[i].X += x;
                r[i].Y += y;
            }
            TransformPoints(ref r);
        }

        public void TransformPoints(ref Vec3ft[] r, ILocalisable p, double A)
        {
            double ca = Math.Cos(A);
            double sa = Math.Sin(A);
            for (int i = 0; i < r.Length; i++)
            {
                Vec3ft rr = r[i];
                r[i].X = (float)(rr.X * ca + rr.Y * sa + p.X);
                r[i].Y = (float)(-rr.X * sa + rr.Y * ca + p.Y);
            }
            TransformPoints(ref r);
        }

        public void TransformRectangle(ref RectangleF r)
        {
            r.X += Offset.X;
            r.Y += Offset.Y;

            r.X *= ScaleFactor;
            r.Y *= ScaleFactor;
            r.Width *= ScaleFactor;
            r.Height *= -ScaleFactor;

            r.Y = 1 - r.Y;
        }

        public void TransformVector3(ref Vec3f r, float iLayer)
        {
            r.X += Offset.X;
            r.Y += Offset.Y;

            r.X *= ScaleFactor;
            r.Y *= ScaleFactor;
            r.Z = iLayer;

            r.Y = 1 - r.Y;
        }

        public void UnTransformPoint(ref Vec3f v)
        {
            v.Y = 1 - v.Y;
            v /= ScaleFactor;
            v -= Offset;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\b':
                    if (FilterCreator.SearchString.Length > 1)
                    {
                        FilterCreator.SearchString = FilterCreator.SearchString.Substring(0, FilterCreator.SearchString.Length - 1);
                    }
                    else if (FilterCreator.SearchString.Length == 1)
                    {
                        FilterCreator.SearchString = "";
                        FilterCreator.SearchRegex = null;
                        return;
                    }
                    break;

                default:
                    FilterCreator.SearchString += e.KeyChar;
                    break;
            }
            try { FilterCreator.SearchRegex = new Regex(FilterCreator.SearchString); }
            catch { FilterCreator.SearchRegex = null; }
        }

        #endregion Protected Methods

        #region Private Methods

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32.dll")]
        private static extern bool RegisterTouchWindow(IntPtr hWnd, int flags);

        private void DoerDraw()
        {
            Refresh_CB refresher = new Refresh_CB(RenderFrame);
            Stopwatch sww = new Stopwatch();
            while (true)
            {
                sww.Restart();
                Thread.Sleep(10);

                this.Invoke(refresher);
                sww.Stop();
            }
        }

        private void DoZoom(EZoomMode z)
        {
            float xMin = float.MaxValue; float xMax = float.MinValue;
            float yMin = float.MaxValue; float yMax = float.MinValue;

            foreach (Vec3ft[] vs in ObjectStrips.Values)
            {
                foreach (Vec3ft vv in vs)
                {
                    if (xMin > vv.X)
                    {
                        xMin = vv.X;
                    }

                    if (xMax < vv.X)
                    {
                        xMax = vv.X;
                    }

                    if (yMin > vv.Y)
                    {
                        yMin = vv.Y;
                    }

                    if (yMax < vv.Y)
                    {
                        yMax = vv.Y;
                    }
                }
            }

            Vec2f v = Offset;
            switch (z)
            {
                case EZoomMode.In:
                    v.X -= (Width * 0.5F / MaxSize) / (ScaleFactor * Ratio);
                    v.Y -= (1 - Height * 0.5F / MaxSize) / (ScaleFactor * Ratio);
                    Offset = v;
                    ScaleFactor *= Ratio;
                    break;

                case EZoomMode.Out:
                    v.X += (Width * 0.5F / MaxSize) / (ScaleFactor);
                    v.Y += (1 - Height * 0.5F / MaxSize) / (ScaleFactor);
                    Offset = v;
                    ScaleFactor /= Ratio;
                    break;

                case EZoomMode.O:
                    v.X -= (xMin - 0.05F) / ScaleFactor;
                    v.Y -= (Height / MaxSize - yMax - 0.05F * Height / MaxSize) / ScaleFactor;
                    Offset = v;
                    break;

                case EZoomMode.All:
                    v.X -= (xMin - 0.05F) / ScaleFactor;
                    v.Y -= (Height / MaxSize - yMax - 0.05F * Height / MaxSize) / ScaleFactor;
                    Offset = v;
                    ScaleFactor /= 1.1F * Math.Max((xMax - xMin) * MaxSize / Width, (yMax - yMin) * MaxSize / Height);
                    break;
            }
        }

        private void KeyDowned(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 16: ShftDown = true; break;
                case 17: CtrlDown = true; break;
            }
        }

        private void KeyUpped(object sender, KeyEventArgs e)
        {
            switch (e.KeyValue)
            {
                case 16: ShftDown = false; break;
                case 17: CtrlDown = false; break;
            }
        }

        private void KJaadControl_Resize(object sender, EventArgs e)
        {
            MaxSize = Math.Max(Height, Width);
        }

        private void MouseClicked(object sender, MouseEventArgs e)
        {
            SetStyle(ControlStyles.Selectable, true);
            this.Focus();

            if (OverflownObject != null)
            {
                if (OverflownObject is TCreator c)
                {
                    c.Do?.Invoke(c);
                }
                else
                {
                    SelectedObject = OverflownObject;
                }
            }
            else
            {
                SelectedObject = null;
            }
            //
            if (Recaller != null)
            {
                this.Invoke(Recaller);
            }

            //Vector2 v = new Vector2(e.X, Height - e.Y);
            //v.X /= MaxSize;
            //v.Y /= MaxSize;
            ////v.X = v.X / Scale - Offset.X;
            ////v.Y = (1-v.Y) / Scale - Offset.Y;
            //foreach (TCreator c in Creators)
            //{
            //    if (v.X > c.Rect.Left && v.X < c.Rect.Right && v.Y < c.Rect.Top && v.Y > c.Rect.Bottom)
            //    {
            //        c.Start();
            //    }
            //}
        }

        private void MouseWheeled(object sender, MouseEventArgs e)
        {
            if (ShftDown)
            {
                if (e.Delta > 0)
                {
                    Offset.X += (0.1F * Width / MaxSize) / ScaleFactor;
                }
                else if (e.Delta < 0)
                {
                    Offset.X -= (0.1F * Width / MaxSize) / ScaleFactor;
                }
            }
            else if (CtrlDown)
            {
                Vec2f v1 = Untransform(e);
                //ParentForm.Text = String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4} {5}", v.X, v.Y, Offset.X, Offset.Y, (MaxSize - Width) / MaxSize / ScaleFactor, (MaxSize - Height) / MaxSize / ScaleFactor);
                if (e.Delta > 0)
                {
                    ZoomIn(null);
                }
                else if (e.Delta < 0)
                {
                    ZoomOut(null);
                }

                Vec2f v2 = Untransform(e);
                Offset.X += v2.X - v1.X;
                Offset.Y += v2.Y - v1.Y;
            }
            else
            {
                if (e.Delta > 0)
                {
                    Offset.Y += (0.1F * Height / MaxSize) / ScaleFactor;
                }
                else if (e.Delta < 0)
                {
                    Offset.Y -= (0.1F * Height / MaxSize) / ScaleFactor;
                }
            }
        }

        private void RenderFrame()
        {
            float rr = Math.Max(Width, Height);

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            dpix = 20 * GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSX) / rr;
            dpiy = 20 * GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY) / rr;
            g.Dispose();

            Stopwatch w = new Stopwatch();
            w.Start();

            GL.ClearColor(BckColor.X, BckColor.Y, BckColor.Z, 1);

            MaxSize = Math.Max(Height, Width);
            ObjectStrips.Clear();
            ObjectLineStrips.Clear();

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.AccumBufferBit);
            //GL.Disable(EnableCap.Lighting);
            //GL.Disable(EnableCap.ColorMaterial);
            //GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
            //GL.Disable(EnableCap.Texture1D);
            //GL.Disable(EnableCap.Texture2D);
            //GL.Disable(EnableCap.Blend);

            //reset depth and color to be consistent over multiple frames

            //GL.Viewport(0, 0, 1000, 1000);// 0, 0, Width , Height );//view.Viewport.Left * Window.Width, view.Viewport.Top * Window.Height, view.Viewport.Right * Window.Width, view.Viewport.Bottom * Window.Height);
            GL.Viewport(-(int)MaxSize, -(int)MaxSize, (int)MaxSize * 2, (int)MaxSize * 2);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(Left, Top, Right, Bottom, -1f, 1f);

            GL.MatrixMode(MatrixMode.Modelview);
            //

            DoerFrame();
            //DoSpider();
            //
            GL.Color3(1, 1, 1);
            GL.Enable(EnableCap.Texture2D);
            foreach (TCreator c in Creators.FindAll(o => o.Visible))
            {
                c.Update();
                RectangleF r = c.Rect;
                r.X *= dpix; r.Width *= dpix;
                r.Y *= dpiy; r.Height *= dpiy;

                GL.BindTexture(TextureTarget.Texture2D, c.Renderer.Texture);

                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(r.Left, r.Top, -0.3F);
                GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(r.Right, r.Top, -0.3F);
                GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(r.Right, r.Bottom, -0.3F);
                GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(r.Left, r.Bottom, -0.3F);
                GL.End();
            }
            GL.Disable(EnableCap.Texture2D);

            if (OverflownObject != null)
            {
                if (ObjectStrips.ContainsKey(OverflownObject))
                {
                    GL.LineWidth(2F);
                    GL.Color3(1, 1, 0);
                    GL.Begin(PrimitiveType.LineLoop);
                    foreach (Vec3ft v in GetLoop(ObjectStrips[OverflownObject]))
                    {
                        GL.Vertex3(v.X, v.Y, -0.9F);
                    }

                    GL.End();
                }
                if (ObjectLineStrips.ContainsKey(OverflownObject))
                {
                    GL.LineWidth(2);
                    GL.Color3(1, 1, 0);
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Vec3ft v in ObjectLineStrips[OverflownObject])
                    {
                        GL.Vertex3(v.X, v.Y, -0.9F);
                    }

                    GL.End();
                }
            }

            if (SelectedObject != null)
            {
                if (ObjectStrips.ContainsKey(SelectedObject))
                {
                    GL.LineWidth(3);
                    GL.Color3(0.849F, 0.849F, 0.849F);
                    GL.Begin(PrimitiveType.LineLoop);
                    foreach (Vec3ft v in GetLoop(ObjectStrips[SelectedObject]))
                    {
                        GL.Vertex3(v.X, v.Y, -0.9F);
                    }

                    GL.End();
                }
                if (ObjectLineStrips.ContainsKey(SelectedObject))
                {
                    GL.LineWidth(3);
                    GL.Color3(0.849F, 0.849F, 0.849F);
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Vec3ft v in ObjectLineStrips[SelectedObject])
                    {
                        GL.Vertex3(v.X, v.Y, -0.9F);
                    }

                    GL.End();
                }
            }
            foreach (object o in SelectedObjects)
            {
                if (ObjectStrips.ContainsKey(o))
                {
                    GL.LineWidth(1.5F);
                    GL.Color3(0.151F, 0.151F, 0.849F);
                    GL.Begin(PrimitiveType.LineLoop);
                    foreach (Vec3ft v in GetLoop(ObjectStrips[o]))
                    {
                        GL.Vertex3(v.X, v.Y, -0.9F);
                    }

                    GL.End();
                }
                if (ObjectLineStrips.ContainsKey(SelectedObject))
                {
                    GL.LineWidth(1.5F);
                    GL.Color3(0.151F, 0.151F, 0.849F);
                    GL.Begin(PrimitiveType.LineStrip);
                    if (ObjectLineStrips.ContainsKey(o))
                        foreach (Vec3ft v in ObjectLineStrips[o])
                        {
                            GL.Vertex3(v.X, v.Y, -0.9F);
                        }

                    GL.End();
                }
            }

            foreach (object o in RecallerObjets)
            {
                if (ObjectStrips.ContainsKey(o))
                {
                    GL.LineWidth(3);
                    GL.Color3(0, 1, 0);
                    GL.Begin(PrimitiveType.LineLoop);
                    foreach (Vec3ft v in GetLoop(ObjectStrips[o]))
                    {
                        GL.Vertex3(v.X, v.Y, -0.3F);
                    }

                    GL.End();
                }

                if (ObjectLineStrips.ContainsKey(o))
                {
                    GL.LineWidth(3);
                    GL.Color3(0, 1, 0);
                    GL.Begin(PrimitiveType.LineStrip);
                    foreach (Vec3ft v in ObjectLineStrips[o])
                    {
                        GL.Vertex3(v.X, v.Y, -0.2F);
                    }

                    GL.End();
                }
            }

            if (FilterCreator != null && FilterCreator.SearchRegex != null)
            {
                double k = 2 * TimeSpan.TicksPerSecond;
                k = 0.5F * (DateTime.Now.Ticks % k) / k + 0.5F;// 0.000000000000000036F * (float)DateTime.Now.Ticks;

                object[] objs = GetFoundObject();
                foreach (object o in objs)
                {
                    if (ObjectStrips.ContainsKey(o))
                    {
                        GL.Color3((float)(0.9F * k), (float)(0.9F * k), (float)(0.5F * k));

                        GL.LineWidth(5);

                        GL.Begin(PrimitiveType.LineLoop);
                        foreach (Vec3ft vv in GetLoop(ObjectStrips[o]))
                        {
                            GL.Vertex3(vv.X, vv.Y, vv.Z);
                        }

                        GL.End();

                        GL.Begin(PrimitiveType.Lines);
                        GL.Vertex3(ObjectStrips[o][0].X, ObjectStrips[o][0].Y, 0.01F);
                        GL.Vertex3(FilterCreator.Rect.Left, FilterCreator.Rect.Top, 0.01F);
                        GL.End();
                    }
                }
            }

            if (MousePoint.HasValue)
            {
                if (MiddleDown)
                {
                    Vec2f v1 = Untransform(MousePointEventArgs);
                    Vec2f v0 = Untransform();
                    if ((v0 - v1).LengthSquared > 100)
                    {
                        v0 *= 1;
                    }

                    TransformPoint(ref v0);
                    TransformPoint(ref v1);
                    Vec2f v2 = new Vec2f(Math.Max(v0.X, v1.X), Math.Max(v0.Y, v1.Y));
                    Vec2f v3 = new Vec2f(Math.Min(v0.X, v1.X), Math.Min(v0.Y, v1.Y));
                    //
                    GL.Color4(0.2F, 0.2F, 0.2F, 0.2F);
                    GL.Begin(PrimitiveType.TriangleStrip);
                    GL.Vertex3(-1, -1, -0.9F);
                    GL.Vertex3(1, -1, -0.9F);
                    GL.Vertex3(v2.X, v3.Y, -0.9F);
                    GL.Vertex3(1, 1, -0.9F);
                    GL.Vertex3(v2.X, v2.Y, -0.9F);
                    GL.Vertex3(-1, 1, -0.9F);
                    GL.Vertex3(v3.X, v2.Y, -0.9F);
                    GL.Vertex3(-1, -1, -0.9F);
                    GL.Vertex3(v3.X, v3.Y, -0.9F);
                    GL.Vertex3(v2.X, v3.Y, -0.9F);
                    GL.End();
                    GL.Color4(1, 1, 1, 1);
                    GL.Begin(PrimitiveType.LineStrip);
                    GL.Vertex3(v2.X, v3.Y, -0.9F);
                    GL.Vertex3(v2.X, v2.Y, -0.9F);
                    GL.Vertex3(v3.X, v2.Y, -0.9F);
                    GL.Vertex3(v3.X, v3.Y, -0.9F);
                    GL.Vertex3(v2.X, v3.Y, -0.9F);
                    GL.End();
                }
            }

            //GL.Flush();
            SwapBuffers();
            GL.Finish();
            w.Stop();

            Duration = w.ElapsedMilliseconds;

            //Update();
            //Refresh();
            //ParentForm.Update();
            //ParentForm.Refresh();
            //Graphics grp = this.CreateGraphics();
            //grp.Dispose();

            //Thread.Sleep(100);

            //TakeScreenshot();
        }

        private Vec2f Untransform()
        {
            Point p = PointToClient(new Point(Cursor.Position.X, Cursor.Position.Y));
            Vec2f v1 = new Vec2f((MaxSize - Width + p.X) / MaxSize / ScaleFactor - Offset.X, (MaxSize - Height + p.Y) / MaxSize / ScaleFactor - Offset.Y);
            //Vector2 v1 = new Vector2(e.X / MaxSize / ScaleFactor - Offset.X + (MaxSize - Width) / MaxSize / ScaleFactor, e.Y / MaxSize / ScaleFactor - Offset.Y + (MaxSize - Height) / MaxSize / ScaleFactor);
            return v1;
        }

        private Vec2f Untransform(MouseEventArgs e)
        {
            Vec2f v1 = new Vec2f((MaxSize - Width + e.X) / MaxSize / ScaleFactor - Offset.X, (MaxSize - Height + e.Y) / MaxSize / ScaleFactor - Offset.Y);
            //Vector2 v1 = new Vector2(e.X / MaxSize / ScaleFactor - Offset.X + (MaxSize - Width) / MaxSize / ScaleFactor, e.Y / MaxSize / ScaleFactor - Offset.Y + (MaxSize - Height) / MaxSize / ScaleFactor);
            return v1;
        }

        private void WaitForParentForm()
        {
            //while (ParentForm == null)
            //{
            //    Thread.Sleep(10);
            //}

            //ParentForm.MouseMove += MouseMoved;
            //ParentForm.MouseUp += MouseUped;
            //ParentForm.MouseDown += MouseDowned;
            //ParentForm.MouseClick += MouseClicked;
            //ParentForm.MouseDoubleClick += MouseDoubleClicked;
            //ParentForm.MouseWheel += MouseWheeled;
            //ParentForm.KeyDown += KeyDowned;
            //ParentForm.KeyUp += KeyUpped;
            //ParentForm.Resize += ParentForm_Resize;
        }

        private void ZoomAll(TCreator c)
        { DoZoom(EZoomMode.All); }

        private void ZoomIn(TCreator c)
        { DoZoom(EZoomMode.In); }

        private void ZoomO(TCreator c)
        { DoZoom(EZoomMode.O); }

        private void ZoomOut(TCreator c)
        { DoZoom(EZoomMode.Out); }

        #endregion Private Methods
    }
}