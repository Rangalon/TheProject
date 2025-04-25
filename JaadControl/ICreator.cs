using System.Drawing;

namespace JaadWinControls
{
    public interface ICreator
    {
        #region Public Properties

        Rectangle Rct { get; set; }

        bool Started { get; }

        #endregion Public Properties

        #region Public Methods

        void Draw(Graphics grp);

        void Stop();

        #endregion Public Methods
    }

    public interface ITip
    {
        #region Public Properties

        string Tip { get; set; }

        #endregion Public Properties
    }
}