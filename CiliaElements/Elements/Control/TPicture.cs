using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Control
{
    public class TPicture : TControl, IIcon
    {
        #region Public Fields

        public Action ActionToDo;

        public bool DeActivated;

        #endregion Public Fields

        #region Private Fields

        private static Pen BigRedPen = new Pen(Color.Red, 5);

        #endregion Private Fields

        #region Public Constructors

        public TPicture(string iPartNumber, int width, int height) : base(iPartNumber)
        {
            Height = height;
            Width = width;
        }

        #endregion Public Constructors

        #region Public Properties

        public Bitmap ForePicture { get; set; }

        //public TIconBar IconOwner { get; set; }

        public override bool Visible { get; set; }

        #endregion Public Properties

        #region Public Methods

     
        public override void Compute(Graphics grp)
        {
            grp.DrawImage(ForePicture, 0, 0, Width, Height);
          
        }

        public override void Click()
        { 
        }

        public override void MouseDrag(double dx, double dy)
        { 
        }

        public override void MouseMove(Vec3 p)
        { 
        }


        #endregion Public Methods

        #region Internal Methods



        #endregion Internal Methods
    }
}
