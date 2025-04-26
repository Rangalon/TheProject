using System;

namespace Planets.Classes
{
    public class TRandom
    {
        #region Private Fields

        private int position = 0;
        private static double[] tbl;

        #endregion Private Fields

        #region Public Constructors

        public TRandom()
        {
        }
        static TRandom()
        {
            Random r = new Random(-1);
            tbl = new double[NbMax];
            for (int i = 0; i < NbMax; i++)
                tbl[i] = r.NextDouble();
        }


        static readonly int NbMax = 521;

        public TRandom(int idPlanet) : base()
        {
            position = idPlanet;
        }

        #endregion Public Constructors

        #region Public Methods

        public double Next()
        {
            double d = tbl[position];
            position = (position + 1) % NbMax;
            return d;
        }

        public void Reset()
        { position = 0; }

        #endregion Public Methods
    }
}
