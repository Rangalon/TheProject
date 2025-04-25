
using Math3D;
using System;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TButton : TControl, IIcon
    {
        #region Public Fields

        public Action ActionToDo;

        public bool DeActivated;

        #endregion Public Fields

        #region Private Fields

        private static Pen BigRedPen = new Pen(Color.Red, 5);

        #endregion Private Fields

        #region Public Constructors

        public TButton(string iPartNumber) : base(iPartNumber)
        {
            Height = 32;
            Width = 32;
        }

        #endregion Public Constructors

        #region Public Properties

        public Bitmap ForePicture { get; set; }

        //public TIconBar IconOwner { get; set; }

        public override bool Visible { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void Click()
        {
            //ToRedraw = true;
            ActionToDo?.Invoke();
        }

        public override void Compute(Graphics grp)
        {
            grp.DrawImage(ForePicture, 0, 0, Width, Height);
            if (DeActivated)
            {
                grp.DrawLine(BigRedPen, 5, 5, Width - 5, Height - 5);
                grp.DrawLine(BigRedPen, Width - 5, 5, 5, Height - 5);
            }
        }

        public override void MouseMove(Vec3 p)
        {
            //ToRedraw = true;
        }

        #endregion Public Methods

        #region Internal Methods

        public override void MouseDrag(double dx, double dy)
        {

        }

        #endregion Internal Methods
    }
}