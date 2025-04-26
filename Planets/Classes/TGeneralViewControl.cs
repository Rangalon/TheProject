using CiliaElements.Elements.Control;
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Planets.Classes
{
    public class TGeneralViewControl : TControl
    {
        private static StringFormat FrmtNN = new StringFormat() { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };

        public TGeneralViewControl() : base("GeneralView")
        {
        }

        public override void Click()
        {
        }

        readonly Font Ft8 = new Font("Verdana", 8);

        public override void Compute(Graphics grp)
        {
            grp.DrawString(string.Format("X: {0:####0}\nY: {1:####0}\nZ: {2:####0}\nW: {3:####0}\nN: {4:####0}", 
                THyperNavigation.Player.Position.X, THyperNavigation.Player.Position.Y, THyperNavigation.Player.Position.Z, THyperNavigation.Player.Position.W,THyperNavigation.Player.Position.FullLengthSquared ), Ft8, Brushes.White, 1, 1);
        }

        public override void MouseDrag(double dx, double dy)
        {
        }

        public override void MouseMove(Vec3 p)
        {
        }
        void ToRedrawTimer()
        {
            while (ToRedrawEnabled)
            {
                ToRedraw = true;
                Thread.Sleep(100);
            }
        }


        public override bool Visible
        {
            get => base.Visible; set
            {
                base.Visible = value;
                ToRedrawEnabled = value;
                if (value)
                {
                    Thread th = new Thread(ToRedrawTimer) { Priority = ThreadPriority.Lowest }; th.Start();
                }
            }
        }

    }
}
