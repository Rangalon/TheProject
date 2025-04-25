using System.Collections.Generic;

namespace CiliaElements
{
    public interface IOpenFileDialog
    {
        #region Public Properties

        IEnumerable<string> FileNames { get; set; }
        string Filter { get; set; }

        bool Multiselect { get; set; }

        #endregion Public Properties

        #region Public Methods

        void Reset();

        void ShowDialog();

        #endregion Public Methods
    }
}