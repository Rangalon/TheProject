
using Math3D;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;

namespace CiliaElements.Elements.Control
{
    public abstract class TControl : TSolidElement
    {
        #region Public Fields

        public static Thread ComputeThread;
        public static TQuickStc<TControl> Controls = new TQuickStc<TControl>();
        public Mtx4 DisplayMatrix;
        public Font Font = new Font("Verdana", 8);

        public bool ToRedraw = true;

        #endregion Public Fields

        #region Protected Fields

        protected static SolidBrush BackBrush;
        protected static Color BackColor = Color.FromArgb(16, 16, 16, 16);
        protected static Color DeeperBackColor = Color.FromArgb(255, 4, 4, 4);
        protected static SolidBrush OverFlownBackBrush;
        protected static Color OverFlownBackColor = Color.FromArgb(64, 0, 255, 255);
        protected static SolidBrush SelectedBackBrush;
        protected static Color SelectedBackColor = Color.FromArgb(64, 255, 255, 0);

        #endregion Protected Fields

        #region Public Constructors

        static TControl()
        {
            ComputeThread = TManager.CreateThread(ComputeAllControls, "ComputeAllControls", ThreadPriority.Lowest);
            BackBrush = new SolidBrush(BackColor);
            OverFlownBackBrush = new SolidBrush(OverFlownBackColor);
            SelectedBackBrush = new SolidBrush(SelectedBackColor);
        }

        public TControl(string iPartNumber)
        {
            Visible = false;
            PartNumber = iPartNumber + ".control";
            //
            Fi = new System.IO.FileInfo(PartNumber);
            //
            _ = new TFile(Fi, null) { Element = this };
            //
            Width = 50;
            Height = 50;
            //
            Controls.Push(this);
        }

        #endregion Public Constructors

        #region Public Properties

        public int Height { get; set; }
        public virtual bool Visible { get; set; } = true;
        public int Width { get; set; }

        #endregion Public Properties

        #region Public Methods

        public TLink Attach(TBaseElement iView)
        {
            TLink link = new TLink
            {
                ToBeReplaced = true,
                Child = this,
                ParentLink = iView.OwnerLink,
                NodeName = PartNumber.Split('.')[0]
            };
            return link;
        }

        public abstract void Click();

        public abstract void Compute(Graphics grp);

        public override void LaunchLoad()
        {
            TCloud Positions, Normals;
            TTextureCloud Textures;
            TTexture Texture = SolidElementConstruction.AddTexture(0.25F, 1, 1, 1);
            Texture.KDBitmap = new Bitmap(10, 10);
            // ----------------------------------------------------------
            Positions = SolidElementConstruction.AddCloud();
            Positions.Vectors.Push(new Vec3[]
            {
                new Vec3(0,0.001,0),
                new Vec3(0,0.001,1),
                new Vec3(1,0.001,0),
                new Vec3(1,0.001,1)
            }
                );
            Normals = SolidElementConstruction.AddCloud();
            Normals.Vectors.Push(new Vec3[]
            {
                new Vec3(0,0,-1),
                new Vec3(0,0,-1),
                new Vec3(0,0,-1),
                new Vec3(0,0,-1)
            }
                );
            Textures = SolidElementConstruction.AddTextureCloud();
            Textures.Vectors.Push(new Vec2[]
            {
                new Vec2(0,0),
                new Vec2(0,1),
                new Vec2(1,0),
                new Vec2(1,1)
            });
            //
            TFGroup Faces = SolidElementConstruction.AddFGroup();
            Faces.ShapeGroupParameters.Positions = Positions;
            Faces.ShapeGroupParameters.Normals = Normals;
            Faces.ShapeGroupParameters.Textures = Textures;
            Faces.GroupParameters.Texture = Texture;
            Faces.Indexes.Push(new int[] { 0, 2, 1, 2, 3, 1 });
            //
            SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            //
            ElementLoader.Publish();
        }

        public abstract void MouseDrag(double dx, double dy);

        public abstract void MouseMove(Vec3 p);

        #endregion Public Methods

        #region Internal Methods

        internal bool HasMouse(Vec2 crs)
        {
            return false;
            // throw new NotImplementedException();
        }

        #endregion Internal Methods

        #region Private Methods

        internal static bool ComputeAllControlsEnabled = true;

        internal int[] BackGround;

        internal Thread RedrawThread;

        int? BackgroundLength = null;

        Rectangle RenderingRectangle;

        public void SetBackground(Bitmap bmp)
        {
            RenderingRectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData d = bmp.LockBits(RenderingRectangle, ImageLockMode.ReadOnly, bmp.PixelFormat);
            BackgroundLength = RenderingRectangle.Height * (d.Stride >> 2);
            BackGround = new int[BackgroundLength.Value];
            System.Runtime.InteropServices.Marshal.Copy(d.Scan0, BackGround, 0, BackgroundLength.Value);
            bmp.UnlockBits(d);
        }

        internal static void StopAll()
        {
            ComputeAllControlsEnabled = false;
        }

        internal void ToRedrawTimer()
        {
            while (ComputeAllControlsEnabled)
            {
                ToRedraw = true;
                Thread.Sleep(100);
            }
        }
        private static void ComputeAllControls()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;

            Bitmap bmp;
            Graphics grp;
            int w, h;
            while (ComputeAllControlsEnabled)
            {
                Thread.Sleep(1);
                foreach (TControl c in Controls.Values.Where(o => o != null && o.ToRedraw && o.State == EElementState.Pushed && o.Visible))
                {
                    try
                    {
                        w = c.Width; h = c.Height;
                        bmp = new Bitmap(c.Width * 4, c.Height * 2);
                    }
                    catch
                    {
                        bmp = null;
                    }
                    if (bmp != null)
                    {
                        //
                        if (c.BackgroundLength.HasValue)
                        {
                            BitmapData bmpdata = bmp.LockBits(c.RenderingRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
                            System.Runtime.InteropServices.Marshal.Copy(c.BackGround, 0, bmpdata.Scan0, c.BackgroundLength.Value);
                            bmp.UnlockBits(bmpdata);
                            grp = Graphics.FromImage(bmp);
                            grp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                            grp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                            grp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            grp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                        }
                        else
                        {
                            //
                            grp = Graphics.FromImage(bmp);
                            grp.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                            if (TManager.OverFlownLink != null && TManager.OverFlownLink == c.OwnerLink)
                                grp.Clear(DeeperBackColor);
                            else
                                grp.Clear(BackColor);
                        }
                        grp.ScaleTransform(2, 2);
                        try
                        {
                            c.Compute(grp);
                        }
                        catch
                        {
                        }
                        grp.Dispose();

                        c.ChangeTexture(bmp);

                        c.ToRedraw = false;

                    }
                }
            }
            foreach (TControl c in Controls.Values.Where(o => o != null && o.ToRedraw && o.State == EElementState.Pushed && o.Visible))
            {
                c.Visible = false;
            }
        }
        #endregion Private Methods
    }
}