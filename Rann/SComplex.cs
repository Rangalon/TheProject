using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rann
{


    [StructLayout(LayoutKind.Sequential)]
    public struct SComplex
    {
        #region Public Fields

        public double Im;
        public double Re;

        #endregion Public Fields

        #region Public Methods

        public static SComplex operator +(SComplex a, SComplex b)
        {
            a.Re = a.Re + b.Re;
            a.Im = a.Im + b.Im;
            return a;
        }

        public override string ToString()
        {
            return String.Format("( {0}, {1}i )", this.Re, this.Im);
        }

        public static SComplex operator *(SComplex a, SComplex b)
        {
            // (x + yi)(u + vi) = (xu – yv) + (xv + yu)i.
            double x = a.Re, y = a.Im;
            double u = b.Re, v = b.Im;

            a.Re = x * u - y * v;
            a.Im = x * v + y * u;

            return a;
        }

        public static SComplex operator /(SComplex a, double f)
        {
            a.Re = a.Re / f;
            a.Im = a.Im / f;

            return a;
        }

        #endregion Public Methods

    }
}
