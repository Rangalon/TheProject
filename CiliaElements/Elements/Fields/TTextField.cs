using System.Drawing;

namespace CiliaElements.Elements.Fields
{
    public class TTextField : TField
    {
        #region Public Fields

        public GetValue_CB GetValue;

        public SetValue_CB SetValue;

        #endregion Public Fields

        #region Public Constructors

        public TTextField(StringFormat strft, Brush brs, Font ft, Rectangle rec, GetValue_CB getValue, SetValue_CB setValue) : base(strft, brs, ft, rec)
        {
            GetValue = getValue; SetValue = setValue;
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate string GetValue_CB();

        public delegate void SetValue_CB(string s);

        #endregion Public Delegates

        #region Public Methods

        public override void DoKeys(string s)
        {
            SetValue(s);
        }

        public override void Draw(Graphics grp)
        {
            base.Draw(grp);
            if (this != TManager.ActiveField)
                grp.DrawString(GetValue.Invoke(), Ft, Brs, Rec, StrFt);
            else
                grp.DrawString(TManager.PendingKeys, Ft, Brs, Rec, StrFt);
        }

        public override string GetKeys() => GetValue();

        #endregion Public Methods
    }
}