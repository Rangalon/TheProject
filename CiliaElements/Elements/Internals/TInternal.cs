using System.Globalization;

namespace CiliaElements
{
    public abstract class TInternal : TSolidElement
    {
        #region Public Fields

        public const int Infinity = 50;

        #endregion Public Fields

        #region Public Constructors

        public TInternal(string iPartNumber)
        {
            PartNumber = iPartNumber + ".internal";
            //
            if (iPartNumber.Length == 1)
            {
                Fi = new System.IO.FileInfo(((int)(iPartNumber.ToCharArray()[0])).ToString(CultureInfo.InvariantCulture) + ".internal");
            }
            else
            {
                Fi = new System.IO.FileInfo(PartNumber);
            }
            //
            TFile file = new TFile(Fi, null)
            {
                Element = this
            };
            //
        }

        #endregion Public Constructors

        #region Public Methods

        public TLink Attach(TBaseElement iView)
        {
            TLink link = new TLink
            {
                ToBeReplaced = true,
                Child = this,
                ParentLink = iView.OwnerLink,
                NodeName = PartNumber.Split('.')[0]
            };
            return link;
        }

        #endregion Public Methods
    }
}