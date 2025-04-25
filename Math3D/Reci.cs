using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math3D
{
    public struct Reci
    {
        #region Public Fields

        public int H;
        public int W;
        public int X;
        public int Y;

        #endregion Public Fields

        #region Public Constructors

        public Reci(int v1, int v2, int v3, int v4)
        {
            X = v1; Y = v2; W = v3; H = v4;
        }

        #endregion Public Constructors
    }
}
