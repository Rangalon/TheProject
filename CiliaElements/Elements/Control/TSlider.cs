
using Math3D;
using System;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TSlider : TControl, IIcon
    {
        #region Public Fields

        public Action ActionToDo;

        public bool DeActivated;

        public double Maximum = 1;
        public double Minimum = 0;
        public double Value = 0;

        public ValueChangedDelegate ValueChanged;

        #endregion Public Fields

        #region Private Fields

        private static Pen BigRedPen = new Pen(Color.Red, 5);

        private static Color DarkColor = Color.FromArgb(49, 49, 98);

        private static Pen DarkPen = new Pen(DarkColor, 3);

        private static Color LightColor = Color.FromArgb(112, 112, 141);

        private static Pen LightPen = new Pen(LightColor, 3);

        #endregion Private Fields

        #region Public Constructors

        public TSlider(string iPartNumber) : base(iPartNumber)
        {
            Height = 32;
            Width = 128;
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void ValueChangedDelegate(double d);

        #endregion Public Delegates

        #region Public Properties

        public Bitmap ForePicture { get; set; }

        public TIconBar IconOwner { get; set; }

        public override bool Visible { get { return IconOwner.Visible; } set { if (IconOwner != null) { IconOwner.Visible = value; } } }

        #endregion Public Properties

        #region Public Methods

        public override void Click()
        {
            ActionToDo?.Invoke();
        }

        public override void Compute(Graphics grp)
        {
            grp.DrawImage(ForePicture, 0, 0, Height, Height);

            grp.DrawLine(DarkPen, Height + 8, Height * 0.5F, Width - 8, Height * 0.5F);
            float x = (float)((Width - Height - 16) * (Value - Minimum) / (Maximum - Minimum) + Height + 8);
            if (TManager.OverFlownLink != null && TManager.OverFlownLink == OwnerLink)
            {
                grp.FillRectangle(OverFlownBackBrush, x - 8, Height * 0.5F - 8, 16, 16);
            }
            else
            {
                grp.FillRectangle(BackBrush, x - 8, Height * 0.5F - 8, 16, 16);
            }
            grp.DrawEllipse(LightPen, x - 3, Height * 0.5F - 3, 6, 6);
        }

        public override void MouseMove(Vec3 p)
        {
        }

        #endregion Public Methods

        #region Internal Methods

        public override void MouseDrag(double dx, double dy)
        {
            Value = System.Math.Min(Maximum, System.Math.Max(Minimum, Value + 3 * dx * (Maximum - Minimum)));
            ValueChanged?.Invoke(Value);
        }

        #endregion Internal Methods
    }
}