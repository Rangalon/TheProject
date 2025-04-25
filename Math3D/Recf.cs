using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math3D
{
    public struct Recf
    {
        #region Public Fields

        public float H;
        public float W;
        public float X;
        public float Y;

        #endregion Public Fields

        #region Public Constructors

        public Recf(float v1, float v2, float v3, float v4)
        {
            X = v1; Y = v2; W = v3; H = v4;
        }

        #endregion Public Constructors
    }
}
