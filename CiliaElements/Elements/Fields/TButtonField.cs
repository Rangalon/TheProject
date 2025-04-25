using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CiliaElements.Elements.Fields
{
    public class TButtonField : TField
    {
        #region Private Fields

        private Action ClickAction;

        #endregion Private Fields

        #region Public Constructors

        public string Text;

        public TButtonField(StringFormat strft, Brush brs, Font ft, Rectangle rec, string text, Action clickAction) : base(strft, brs, ft, rec)
        {
            Text = text; ClickAction = clickAction;
        }

        #endregion Public Constructors

        #region Public Methods

        public override Rectangle Rec
        {
            get => base.Rec; set
            {
                base.Rec = value;
                path = new GraphicsPath();
                path.AddLine(value.X, value.Y + 3, value.X + 3, value.Y);
                path.AddLine(value.Right - 3, value.Y, value.Right, value.Y + 3);
                path.AddLine(value.Right, value.Bottom - 3, value.Right - 3, value.Bottom);
                path.AddLine(value.X + 3, value.Bottom, value.X, value.Bottom - 3);
                path.AddLine(value.X, value.Bottom - 3, value.X, value.Y + 3);

            }
        }

        GraphicsPath path;

        public override void Draw(Graphics grp)
        {
            base.Draw(grp);
            grp.DrawPath(Pens.Black, path);
            grp.DrawString(Text, Ft, Brs, Rec, StrFt);
        }

        public override void DoClick()
        {
            ClickAction();
        }

        public override void DoKeys(string s)
        { }

        public override string GetKeys() => "";

        #endregion Public Methods
    }
}