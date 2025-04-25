using System.Drawing;

namespace CiliaElements.Elements.Fields
{
    public class TCheckField : TField
    {
        #region Public Fields

        public GetValue_CB GetValue;

        public SetValue_CB SetValue;

        public string Text;

        #endregion Public Fields

        #region Private Fields

        private Font ChkFt;

        #endregion Private Fields

        #region Public Constructors

        public TCheckField(StringFormat strft, Brush brs, Font ft, Rectangle rec, string text, GetValue_CB getValue, SetValue_CB setValue) : base(strft, brs, ft, rec)
        {
              GetValue = getValue; SetValue = setValue; Text = text;
            ChkFt = new Font(ft.FontFamily, ft.Size, FontStyle.Bold);
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate bool GetValue_CB();

        public delegate void SetValue_CB(bool s);

        #endregion Public Delegates

        #region Public Methods

        public override void DoClick()
        {
            SetValue(!GetValue());
            Control.ToRedraw = true;
        }

        public override void DoKeys(string s)
        {
            if (s == "") return;
            SetValue(!GetValue());
            Control.ToRedraw = true;
        }

        public override void Draw(Graphics grp)
        {
            base.Draw(grp);
            grp.DrawRectangle(Pens.Black, Rec);
            if (GetValue())
            {
                grp.FillRectangle(Brushes.Black, Rec.X + 2, Rec.Y + 2, 6, Rec.Height - 4);
                grp.DrawString(Text, ChkFt, Brs, Rec, StrFt);
            }
            else
            {
                grp.DrawRectangle(Pens.Black, Rec.X + 2, Rec.Y + 2, 6, Rec.Height - 4);
                grp.DrawString(Text, Ft, UnChecked, Rec, StrFt);
            }
        }

        Brush UnChecked = new SolidBrush(Color.FromArgb(64, 64, 64));

        public override string GetKeys() => "";

        #endregion Public Methods
    }
}