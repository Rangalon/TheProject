using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Math3D
{
    public struct SComplex
    {
        #region Public Fields

        public double I;
        public double R;

        #endregion Public Fields

        #region Public Properties

        public double Abs
        {
            get
            {
                return Math.Sqrt(R * R + I * I);
            }
            set
            {
                if (value == 0)
                {
                    R = 0; I = 0;
                }
                else
                {
                    double n = AbsSquared;
                    if (n == 0)
                        R = value;
                    else
                    {
                        n = value / Math.Sqrt(n);
                        R *= n;
                        I *= n;
                    }
                }
            }
        }

        public double AbsSquared
        {
            get
            {
                return R * R + I * I;
            }
        }

        public double Phi
        {
            get
            {
                return Math.Atan2(I, R);
            }
            set
            {
                double n = Abs;
                R = n * Math.Cos(value);
                I = n * Math.Sin(value);
            }
        }

        public double RSquared { get => R * R; }

        #endregion Public Properties

        #region Public Methods

        public static SComplex operator -(SComplex c1, SComplex c2)
        {
            return new SComplex() { R = c1.R - c2.R, I = c1.I - c2.I };
        }
        public static SComplex operator +(SComplex c1, SComplex c2)
        {
            return new SComplex() { R = c1.R + c2.R, I = c1.I + c2.I };
        }

        public static SComplex operator *(SComplex c1, SComplex c2)
        {
            return new SComplex() { R = c1.R * c2.R - c1.I * c2.I, I = c1.I * c2.R + c2.I * c1.R };
        }

        public static SComplex operator *(SComplex c1, double d)
        {
            return new SComplex() { R = c1.R * d, I = c1.I * d };
        }
        public override string ToString()
        {
            return string.Format("{0:0.000};{1:0.000} ", Abs, Phi);
        }

        #endregion Public Methods
    }
}
