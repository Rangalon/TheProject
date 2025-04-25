using CiliaElements.Elements.Control;
using System.Drawing;

namespace CiliaElements.Elements.Fields

{
    public abstract class TField
    {
        #region Public Fields

        public Brush ActivatedBrush = new SolidBrush(Color.FromArgb(64, 255, 255, 255));
        public object BoundObject;
        public Brush Brs;
        public TControl Control;
        public Font Ft;
        public virtual Rectangle Rec { get; set; }
        public StringFormat StrFt;
        public bool Visible = true;

        #endregion Public Fields

        #region Public Constructors

        public TField(StringFormat strft, Brush brs, Font ft, Rectangle rec)
        {
            StrFt = strft; Brs = brs; Ft = ft; Rec = rec;
        }

        #endregion Public Constructors

        #region Public Methods

        public virtual void DoClick()
        { }

        public abstract void DoKeys(string s);

        public virtual void Draw(Graphics grp)
        {
            if (this == TManager.ActiveField)
                grp.FillRectangle(ActivatedBrush, Rec.X - 1, Rec.Y - 1, Rec.Width + 2, Rec.Height + 2);
            if (this == TManager.OverFlownField)
                grp.DrawRectangle(Pens.Yellow, Rec.X - 1, Rec.Y - 1, Rec.Width + 2, Rec.Height + 2);
        }

        public abstract string GetKeys();

        #endregion Public Methods
    }
}