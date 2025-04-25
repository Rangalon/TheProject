
using Math3D;

namespace CiliaElements
{
    public class TTextRange
    {
        #region Public Fields

        public char[] chrs = new char[] { };
        public float LineWidth = 1;
        public Mtx4 Matrix = Mtx4.Identity;
        public string Name;
        public Mtx4 Size = Mtx4.Identity;

        #endregion Public Fields

        #region Public Properties

        public string Text
        {
            get { return new string(chrs); }
            set { chrs = value.ToCharArray(); }
        }

        #endregion Public Properties
    }
}