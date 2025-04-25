
using Math3D;
using System.Drawing;
using System.Threading;

namespace CiliaElements.Elements.Control
{
    public class TVectorPanel : TControl
    {
        #region Public Fields

        public Vec3 DisplayedVector;
        public string Title;

        #endregion Public Fields

        #region Private Fields

        private int LineGap;

        #endregion Private Fields

        #region Public Constructors

        public TVectorPanel(string iPartNumber) : base(iPartNumber)
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
            LineGap = (int)(Font.Size * 2);

            int y = 0;

            grp.DrawString(string.Format("{0,27}", Title), Font, Brushes.White, 0, y); y += LineGap;
            grp.DrawString(string.Format("{0,27:0.000 000}", DisplayedVector.X), Font, Brushes.White, 0, y); y += LineGap;
            grp.DrawString(string.Format("{0,27:0.000 000}", DisplayedVector.Y), Font, Brushes.White, 0, y); y += LineGap;
            grp.DrawString(string.Format("{0,27:0.000 000}", DisplayedVector.Z), Font, Brushes.White, 0, y); y += LineGap;
            grp.DrawString(string.Format("{0,27:0.000 000}", DisplayedVector.Length), Font, Brushes.White, 0, y); y += LineGap;
        }

     
      
        public override bool Visible
        {
            get => base.Visible; set
            {
                if (base.Visible == value) return;
                base.Visible = value; 
                if (value)
                {
                    RedrawThread = TManager.CreateThread(ToRedrawTimer, "ToRedrawTimer Vector", ThreadPriority.Lowest);
                }
                else
                    RedrawThread?.Abort();
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