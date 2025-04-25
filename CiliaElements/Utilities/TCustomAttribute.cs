namespace CiliaElements
{
    public class TCustomAttr
    {
        #region Public Fields

        public string Name = "";
        public string Value = "";

        #endregion Public Fields

        #region Public Methods

        public override string ToString()
        {
            return Name + "=" + Value;
        }

        #endregion Public Methods
    }
}