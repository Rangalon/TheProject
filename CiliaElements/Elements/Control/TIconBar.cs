
using Math3D;
using System.Collections.Generic;
using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public class TIconBar : TControl
    {
        #region Public Fields

        public TIconBarButton IconBarOwner;

        public List<IIcon> Icons = new List<IIcon>();

        #endregion Public Fields

        #region Public Constructors

        public TIconBar(string iPartNumber) : base(iPartNumber)
        {
            Height = 36;
        }

        #endregion Public Constructors

        #region Public Properties

        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (base.Visible == value)
                {
                    return;
                }

                base.Visible = value;
                foreach (IIcon i in Icons)
                {
                    i.Visible = value;
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override void Click()
        {
        }

        public override void Compute(Graphics grp)
        {
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