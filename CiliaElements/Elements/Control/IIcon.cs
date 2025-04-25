using System.Drawing;

namespace CiliaElements.Elements.Control
{
    public interface IIcon
    {
        #region Public Properties

        Bitmap ForePicture { get; set; }

        int Height { get; set; }
        //TIconBar IconOwner { get; set; }

        TLink OwnerLink { get; set; }

        bool Visible { get; set; }

        int Width { get; set; }

        #endregion Public Properties
    }
}