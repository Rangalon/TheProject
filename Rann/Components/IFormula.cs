using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rann.Components
{
    public interface ILFormula
    {
        #region Public Properties

        string LFormulaText { get; set; }

        #endregion Public Properties
    }

    public interface IRFormula
    {
        #region Public Properties

        string RFormulaText { get; set; }

        #endregion Public Properties
    }
}