using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Fields
{
    public class TLabelField : TField
    {
        #region Public Fields

        public GetValue_CB GetValue;

        #endregion Public Fields

        #region Public Constructors

        public TLabelField(StringFormat strft, Brush brs, Font ft, Rectangle rec, GetValue_CB getValue) : base(strft, brs, ft, rec)
        {
            GetValue = getValue;
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate string GetValue_CB();


        #endregion Public Delegates

        #region Public Methods

        public override void DoKeys(string s)
        {
        }

        public override void Draw(Graphics grp)
        {
            base.Draw(grp);
            grp.DrawString(GetValue.Invoke(), Ft, Brs, Rec, StrFt);
        }

        public override string GetKeys() => GetValue();

        #endregion Public Methods
    }
}
