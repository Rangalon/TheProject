using System.Drawing;
using System.Drawing.Imaging;

namespace CiliaElements
{
    public class TSolidElementLoader : TBaseElementLoader
    {
        #region Public Methods

        public override void Publish()
        {

            //
            ((TSolidElement)Element).SolidElementConstruction.Publish();
            //
            //
            Element.ElementLoader = null;
            Element = null;
        }

        #endregion Public Methods
    }
}