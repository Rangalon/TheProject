using System;

namespace CiliaElements.Managers
{
    public class TRandom
    {
        #region Private Fields

        private int position = 0;
        private double[] tbl = new double[256];

        #endregion Private Fields

        #region Public Constructors

        public TRandom()
        {
            Random r = new Random();
            for (int i = 0; i < 256; i++)
                tbl[i] = r.NextDouble();
        }

        #endregion Public Constructors

        #region Public Methods

        public double Next()
        {
            double d = tbl[position];
            position = (position + 1) % 256;
            return d;
        }

        public void Reset()
        { position = 0; }

        #endregion Public Methods
    }
}