
using Math3D;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TIconBarButton : TControl, IIcon
    {
        #region Public Fields

        public TIconBar IconBar;

        #endregion Public Fields

        #region Public Constructors

        public TIconBarButton(string iPartNumber) : base(iPartNumber)
        {
            Height = 32;
            Width = 32;
        }

        #endregion Public Constructors

        #region Public Properties

        public Bitmap ForePicture { get; set; }

        #endregion Public Properties

        //public TIconBar IconOwner { get; set; }

        #region Public Methods

        public override void Click()
        {
            //if (!IconBar.Visible)
            //{
            //    TIconBarButton i = IconOwner.Icons.OfType<TIconBarButton>().FirstOrDefault(o => o.IconBar.Visible);
            //    if (i != null)
            //    {
            //        i.Click();
            //    }
            //}
            IconBar.Visible = !IconBar.Visible;
        }

        public override void Compute(Graphics grp)
        {
            if (IconBar.Visible)
            {
                grp.Clear(SelectedBackColor);
            }
            grp.DrawImage(ForePicture, 0, 0, Width, Height);
        }

        public override void MouseMove(Vec3 p)
        {
        }

        #endregion Public Methods

        #region Internal Methods

        public override void MouseDrag(double dx, double dy)
        {
        }

        #endregion Internal Methods
    }
}