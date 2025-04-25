using System;
using System.Text.RegularExpressions;

namespace JaadWinControls
{
    public class TFilterCreator : TCreator
    {
        #region Public Fields

        public Regex SearchRegex = null;
        public string SearchString = "";

        #endregion Public Fields

        #region Public Properties

        public override bool Visible
        {
            get
            {
                return (SearchString != "");
            }
            set
            {
                throw new Exception("not expected");
            }
        }

        #endregion Public Properties

        #region Public Methods

        public override void Update()
        {
            return;
        }

        #endregion Public Methods
    }
}