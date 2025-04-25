
using Math3D;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TLinkPanel : TControl
    {
        #region Public Fields

        public TLink DisplayedLink;

        #endregion Public Fields

        #region Private Fields

        private int LineGap;

        #endregion Private Fields

        #region Public Constructors

        public TLinkPanel(string iPartNumber) : base(iPartNumber)
        {
            Font = new Font("Fixedsys", 8);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Click()
        {
           
        }

        public override void Compute(Graphics grp)
        {
            TLink dl = DisplayedLink;
            if (dl == null)
            {
                return;
            }
            LineGap = (int)(Font.Size * 2);

            int y = 0;
            if (dl.Solid != null)
            {
                Vec4 v1, v2;
                grp.DrawString(string.Format("{0,27} {1,27}", dl.NodeName, dl.PartNumber), Font, Brushes.White, 0, y); y += LineGap;
                v2 = dl.Solid.BoundingBox.MinPosition; v1 = dl.Matrix.Row3;
                grp.DrawString(string.Format("{0,27} {1,27}", "Origin", "Min"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.X, v2.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Y, v2.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Z, v2.Z), Font, Brushes.White, 0, y); y += LineGap;
                v2 = dl.Solid.BoundingBox.MaxPosition; v1 = dl.Matrix.Row0;
                grp.DrawString(string.Format("{0,27} {1,27}", "X Axis", "Max"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.X, v2.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Y, v2.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Z, v2.Z), Font, Brushes.White, 0, y); y += LineGap;
                v2 = dl.Solid.BoundingBox.CtrPosition; v1 = dl.Matrix.Row1;
                grp.DrawString(string.Format("{0,27} {1,27}", "Y Axis", "Center"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.X, v2.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Y, v2.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Z, v2.Z), Font, Brushes.White, 0, y); y += LineGap;
                v2 = dl.Solid.BoundingBox.Size; v1 = dl.Matrix.Row2;
                grp.DrawString(string.Format("{0,27} {1,27}", "Z Axis", "Size"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.X, v2.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Y, v2.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} {1,27:0.000 000}", v1.Z, v2.Z), Font, Brushes.White, 0, y); y += LineGap;
                //
            }
            else
            {
                Vec4 v1;
                grp.DrawString(string.Format("{0,27}", dl.NodeName), Font, Brushes.White, 0, y); y += LineGap;
                v1 = dl.Matrix.Row3;
                grp.DrawString(string.Format("{0,27}", "Origin"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Z), Font, Brushes.White, 0, y); y += LineGap;
                v1 = dl.Matrix.Row0;
                grp.DrawString(string.Format("{0,27}", "X Axis"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Z), Font, Brushes.White, 0, y); y += LineGap;
                v1 = dl.Matrix.Row1;
                grp.DrawString(string.Format("{0,27}", "Y Axis"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000} ", v1.Z), Font, Brushes.White, 0, y); y += LineGap;
                v1 = dl.Matrix.Row2;
                grp.DrawString(string.Format("{0,27}", "Z Axis"), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.X), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Y), Font, Brushes.White, 0, y); y += LineGap;
                grp.DrawString(string.Format("{0,27:0.000 000}", v1.Z), Font, Brushes.White, 0, y); y += LineGap;
            }
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