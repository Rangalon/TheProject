
using Math3D;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TLabel : TControl
    {
        #region Public Fields

        public string Message;
        public string Title;

        #endregion Public Fields

        #region Private Fields

        private int LineGap;

        #endregion Private Fields

        #region Public Constructors

        public TLabel(string iPartNumber) : base(iPartNumber)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Click()
        {
        }

        public override void Compute(Graphics grp)
        {
            LineGap = (int)(Font.Size * 2);

            grp.DrawString(Title, Font, Brushes.White, 0, 0);
            grp.DrawString(Message, Font, Brushes.White, 0, 16);
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