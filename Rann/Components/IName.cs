using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rann.Components
{
    public interface IName
    {
        #region Public Properties

        string Name { get; set; }

        #endregion Public Properties
    }

    public interface IReplacement
    {
        #region Public Methods

        void Replace(TComponent oldComp, TComponent newComp);

        #endregion Public Methods
    }
}