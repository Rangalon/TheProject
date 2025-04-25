using OpenTK;
using System;
using System.Globalization;

namespace CiliaElements
{
    public struct SLine
    {
        #region Public Fields

        public Vec4 EndPoint;
        public Vec4 StartPoint;

        #endregion Public Fields

        #region Public Constructors

        public SLine(String iXs, String iYs, String iXe, String iYe)
        {
            StartPoint = new Vec4(-double.Parse(iXs, CultureInfo.InvariantCulture) * TLetter.LetterWidth * 0.5, double.Parse(iYs, CultureInfo.InvariantCulture) * TLetter.LetterHeigth * 0.5, 0, 1);
            EndPoint = new Vec4(-double.Parse(iXe, CultureInfo.InvariantCulture) * TLetter.LetterWidth * 0.5, double.Parse(iYe, CultureInfo.InvariantCulture) * TLetter.LetterHeigth * 0.5, 0, 1);
        }

        public SLine(Single iXs, Single iYs, Single iXe, Single iYe)
        {
            StartPoint = new Vec4(-iXs * TLetter.LetterWidth * 0.5, iYs * TLetter.LetterHeigth * 0.5, 0, 1);
            EndPoint = new Vec4(-iXe * TLetter.LetterWidth * 0.5, iYe * TLetter.LetterHeigth * 0.5, 0, 1);
        }

        #endregion Public Constructors

        #region Public Methods

        public static bool operator ==(SLine s1, SLine s2)
        {
            if (s1.EndPoint != s2.EndPoint) return false;
            if (s1.StartPoint != s2.StartPoint) return false;
            return true;
        }

        public static bool operator !=(SLine s1, SLine s2)
        {
            if (s1.EndPoint != s2.EndPoint) return true;
            if (s1.StartPoint != s2.StartPoint) return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Public Methods
    }
}