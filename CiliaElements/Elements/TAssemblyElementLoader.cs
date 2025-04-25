
namespace CiliaElements
{
    public class TAssemblyElementLoader : TBaseElementLoader
    {

        #region Public Methods

  
        public override void Publish()
        {
            Element.State = EElementState.Pushed;
        }

        #endregion Public Methods

    }
}
