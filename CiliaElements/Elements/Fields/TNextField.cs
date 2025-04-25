using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Fields
{
    public class TNextField : TField
    {
        #region Private Fields

        private Action ClickAction;

        #endregion Private Fields

        #region Public Constructors

        public string Text;

        public GetValue_CB GetValue;
        public delegate string GetValue_CB();
        static readonly StringFormat StrNN = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

        public TNextField(Brush brs, Font ft, Rectangle rec, string text, GetValue_CB getValue, Action clickAction) : base(StrNN, brs, ft, rec)
        {
            Text = text; GetValue = getValue; ClickAction = clickAction;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Rectangle Rec
        {
            get => base.Rec; set
            {
                base.Rec = value;
                path = new GraphicsPath();
                path.AddLine(value.X + value.Width / 2 - value.Height / 2, value.Y, value.X + value.Width / 2, value.Y + value.Height / 2);
                path.AddLine(value.X + value.Width / 2 - value.Height / 2, value.Y + value.Height, value.X + value.Width / 2 - value.Height / 2, value.Y);
                RecVal = new Rectangle(value.X + value.Width / 2,value.Y,value.Width /2,value.Height );
            }
        }

        Rectangle RecVal;

        GraphicsPath path;

        public override void Draw(Graphics grp)
        {
            base.Draw(grp);
            grp.DrawRectangle(Pens.Black, Rec);
            grp.FillPath(Brushes.Black, path);
            grp.DrawString(Text, Ft, Brs, Rec, StrFt);
            grp.DrawString(GetValue(), Ft, Brs, RecVal, StrFt);
        }

        public override void DoClick()
        {
            ClickAction();
        }

        public override void DoKeys(string s)
        { }

        public override string GetKeys() => GetValue();

        #endregion Public Methods 
    }
}

