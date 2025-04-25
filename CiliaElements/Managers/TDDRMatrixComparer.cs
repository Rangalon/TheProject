using OpenTK;
using System.Collections.Generic;

namespace CiliaElements.Managers
{
    public class TDDRMatrixComparer : IComparer<Mtx4>
    {
        #region Public Methods

        public int Compare(Mtx4 x, Mtx4 y)
        {
            Vec4 c = TViewManager.VMatrix.Column2;
            //Vector4d v = Vector4d.Transform(x.Row3, TViewManager.VIMatrix);
            return Vec4.Dot(y.Row3, c).CompareTo(Vec4.Dot(x.Row3, c));
        }

        #endregion Public Methods
    }
}