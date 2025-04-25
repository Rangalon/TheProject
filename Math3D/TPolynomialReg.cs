using System;

namespace Math3D
{
    public class TPolynomialReg
    {
        #region Public Fields

        public readonly int Rank, W;
        public double[] Xs = { }, Ys = { }, Ycs;

        #endregion Public Fields

        #region Public Constructors

        public TPolynomialReg(int rank, int w)
        {
            Rank = rank;
            W = w;
        }

        #endregion Public Constructors

        #region Public Properties

        public int Pos { get; private set; } = 0;

        #endregion Public Properties

        #region Public Methods

        public void AddValues(double x, double y)
        {
            Array.Resize(ref Xs, Pos + 1);
            Array.Resize(ref Ys, Pos + 1);
            Xs[Pos] = x; Ys[Pos] = y;
            Pos++;
        }

        public void Compute()
        {
            Ycs = new double[Pos];
            double[,] xs;
            double[] ys, ysr;
            double x, y;
            int current = 0, i;
            xs = new double[Rank + 1, Rank + 1];
            ys = new double[Rank + 1];
            //
            for (i = 0; i < (int)(Math.Min(Pos - 1, current + W)); i++)
            {
                x = Xs[i];
                y = Ys[i];
                //  wtr.WriteLine(x.ToString() + ';' + y.ToString());
                for (int j = 0; j <= Rank; j++)
                {
                    for (int k = 0; k <= Rank; k++)
                        xs[j, k] += Math.Pow(x, j + k);
                    ys[j] += y * Math.Pow(x, j);
                }
            }
            //

            for (current = 0; current < Pos; current++)
            {
                //xsr = new double[Rank + 1, Rank + 1];
                ysr = new double[Rank + 1];

                if (current + W < Pos)
                {
                    i = current + W;
                    x = Xs[i];
                    y = Ys[i];
                    //  wtr.WriteLine(x.ToString() + ';' + y.ToString());
                    for (int j = 0; j <= Rank; j++)
                    {
                        for (int k = 0; k <= Rank; k++)
                            xs[j, k] += Math.Pow(x, j + k);
                        ys[j] += y * Math.Pow(x, j);
                    }
                }
                if (current - W > -1)
                {
                    i = current - W;
                    x = Xs[i];
                    y = Ys[i];
                    //  wtr.WriteLine(x.ToString() + ';' + y.ToString());
                    for (int j = 0; j <= Rank; j++)
                    {
                        for (int k = 0; k <= Rank; k++)
                            xs[j, k] -= Math.Pow(x, j + k);
                        ys[j] -= y * Math.Pow(x, j);
                    }
                }

                //  wtr.Close();
                // TFunctions.BuildReversedMatrix(xs, ref xsr);
                TFunctions.BuildVariablesVector((double[,])xs.Clone(), ys, ref ysr);

                x = Xs[current];
                for (int j = 0; j <= Rank; j++)
                    Ycs[current] += ysr[j] * Math.Pow(x, j);
                if (Math.Abs(Ycs[current]) > 500)
                {
                }
            }
        }

        public void ComputeOld()
        {
            Ycs = new double[Pos];
            double[,] xs;
            double[] ys, ysr;
            double x, y;
            int current;
            for (current = 0; current < Pos; current++)
            {
                xs = new double[Rank + 1, Rank + 1];
                //  xsr = new double[Rank + 1, Rank + 1];
                ys = new double[Rank + 1];
                ysr = new double[Rank + 1];

                // StreamWriter wtr = new StreamWriter(@"C:\Users\to101757\Documents\csv.csv");
                for (int i = (int)(Math.Max(0, current - W)); i <= (int)(Math.Min(Pos - 1, current + W)); i++)
                {
                    x = Xs[i];
                    y = Ys[i];
                    //  wtr.WriteLine(x.ToString() + ';' + y.ToString());
                    for (int j = 0; j <= Rank; j++)
                    {
                        for (int k = 0; k <= Rank; k++)
                            xs[j, k] += Math.Pow(x, j + k);
                        ys[j] += y * Math.Pow(x, j);
                    }
                }
                //  wtr.Close();
                // TFunctions.BuildReversedMatrix(xs, ref xsr);
                TFunctions.BuildVariablesVector(xs, ys, ref ysr);

                x = Xs[current];
                for (int j = 0; j <= Rank; j++)
                    Ycs[current] += ysr[j] * Math.Pow(x, j);
                if (Math.Abs(Ycs[current]) > 500)
                {
                }
            }
        }

        #endregion Public Methods
    }
}