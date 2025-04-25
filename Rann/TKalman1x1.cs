using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rann
{
    public class TKalman1x1
    {
        #region Public Fields

        public readonly double MeasureNoiseCovariance;
        public readonly double NoiseCovariance;
        public readonly double StatePrecision;
        public double dY;

        public double Y;

        #endregion Public Fields

        #region Private Fields

        private SKMatrix bk;

        //
        private SKMatrix fk;

        private SVKMatrix hk;

        private SKMatrix I;

        private SKMatrix pk_1k_1;

        //
        private SKMatrix qk;

        // matrice qui relie l'état xk      à la mesure zk
        private SRMatrix rk;

        // La matrice de covariance de l'erreur (une mesure de la précision de l'état estimé).
        private SHKMatrix uk;

        private SHKMatrix xk_1k_1;

        #endregion Private Fields

        #region Public Constructors

        public TKalman1x1(double statePrecision, double noiseCovariance, double measureNoiseCovariance)
        {
            StatePrecision = statePrecision;
            NoiseCovariance = noiseCovariance;
            MeasureNoiseCovariance = measureNoiseCovariance;

            xk_1k_1 = new SHKMatrix(); // l'estimation de l'état à l'instant
            pk_1k_1 = SKMatrix.Identity(StatePrecision); // La matrice de covariance de l'erreur (une mesure de la précision de l'état estimé).
            I = SKMatrix.Identity(1);
            uk = new SHKMatrix(); // entrée de commande
                                  //
            fk = new SKMatrix(); // matrice qui relie l'état précédent k-1 à l'état actuel k

            fk.R0C0 = 1; fk.R1C0 = 1; fk.R1C1 = 1;
            //
            bk = new SKMatrix(); // matrice qui relie l'entrée de commande u à l'état x
            bk.R1C0 = 1;
            bk.R0C0 = 0;
            bk.R1C1 = 0;
            bk.R0C1 = 0;
            //
            qk = SKMatrix.Identity(NoiseCovariance); // matrice de covariance du bruit du processus (en anglais process noise)
                                                     //
            hk = new SVKMatrix();  // matrice qui relie l'état xk      à la mesure zk
            hk.R0C0 = 1;// hk.Values[1, 1] = 1;
            rk = new SRMatrix(MeasureNoiseCovariance); // matrice de covariance du bruit de mesure
        }

        #endregion Public Constructors

        // l'estimation de l'état à l'instant
        // entrée de commande
        // matrice qui relie l'état précédent k-1 à l'état actuel k

        // matrice qui relie l'entrée de commande u à l'état x

        // matrice de covariance du bruit du processus (en anglais process noise)

        // matrice de covariance du bruit de mesure

        #region Public Methods

        public void AddData(double y)
        {
            //Array.Resize(ref Inputs, Nb + 1); Array.Resize(ref Outputs, Nb + 1);
            //
            //SComplex ci = new SComplex() { Re = x, Im = y };
            //SComplex co = new SComplex();
            //
            SHKMatrix xkk_1 = fk * xk_1k_1 + bk * uk; // (état prédit)
            SKMatrix pkk_1 = fk * pk_1k_1 * fk + qk; // matrice d'estimation a priori de la covariance de l'erreur
                                                     //
            SRMatrix zk = new SRMatrix(y);  // observation ou mesure du processus à l'instant k
            SRMatrix yk = zk - hk * xkk_1; // (innovation)
            SRMatrix sk = hk * pkk_1 * hk.Transposed() + rk; // (covariance de l'innovation)
            SHKMatrix kk = pkk_1 * hk.Transposed() * sk.Inversed(); // (gain de Kalman optimal)
            SHKMatrix xkk = xkk_1 + kk * yk; // (état mis à jour)
            SKMatrix pkk = (I - kk * hk) * pkk_1; // (covariance mise à jour)
            uk.R0C1 = xkk.R0C0 - xk_1k_1.R0C0;
            ////
            //co.Im = xkk;

            Y = xkk.R0C0;// co.Im;
            dY = xkk.R0C0 - xk_1k_1.R0C0;
            ////
            xk_1k_1 = xkk;
            pk_1k_1 = pkk;
            //co.Re = ci.Re;
            //co.Im = xkk.Values[0, 0];
            //
            //Inputs[Nb] = ci;
            //Outputs[Nb] = co;
            //
            //Nb++;
        }

        #endregion Public Methods

        #region Public Structs

        //public int Nb = 0;
        //public SComplex[] Inputs = { };
        //public SComplex[] Outputs = { };
        public struct SHKMatrix
        {
            #region Public Fields

            public double R0C0;
            public double R0C1;

            #endregion Public Fields

            #region Public Methods

            public static SHKMatrix operator +(SHKMatrix iLMatrix, SHKMatrix iRMatrix)
            {
                return new SHKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 + iRMatrix.R0C0,
                    R0C1 = iLMatrix.R0C1 + iRMatrix.R0C1,
                };
            }

            #endregion Public Methods
        }

        public struct SKMatrix
        {
            #region Public Fields

            public double R0C0;
            public double R0C1;
            public double R1C0;
            public double R1C1;

            #endregion Public Fields

            #region Public Methods

            public static SKMatrix operator -(SKMatrix iLMatrix, SKMatrix iRMatrix)
            {
                return new SKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 - iRMatrix.R0C0,
                    R0C1 = iLMatrix.R0C1 - iRMatrix.R0C1,
                    R1C0 = iLMatrix.R1C0 - iRMatrix.R1C0,
                    R1C1 = iLMatrix.R1C1 - iRMatrix.R1C1,
                };
            }

            public static SHKMatrix operator *(SKMatrix iLMatrix, SHKMatrix iRMatrix)
            {
                return new SHKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0 + iLMatrix.R1C0 * iRMatrix.R0C1,
                    R0C1 = iLMatrix.R0C1 * iRMatrix.R0C0 + iLMatrix.R1C1 * iRMatrix.R0C1,
                };
            }

            public static SVKMatrix operator *(SVKMatrix iLMatrix, SKMatrix iRMatrix)
            {
                return new SVKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0 + iLMatrix.R1C0 * iRMatrix.R0C1,
                    R1C0 = iLMatrix.R0C0 * iRMatrix.R1C0 + iLMatrix.R1C0 * iRMatrix.R1C1,
                };
            }

            public static SKMatrix operator *(SKMatrix iLMatrix, SKMatrix iRMatrix)
            {
                return new SKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0 + iLMatrix.R1C0 * iRMatrix.R0C1,
                    R0C1 = iLMatrix.R0C1 * iRMatrix.R0C0 + iLMatrix.R1C1 * iRMatrix.R0C1,
                    R1C0 = iLMatrix.R0C0 * iRMatrix.R1C0 + iLMatrix.R1C0 * iRMatrix.R1C1,
                    R1C1 = iLMatrix.R0C1 * iRMatrix.R1C0 + iLMatrix.R1C1 * iRMatrix.R1C1,
                };
            }

            public static SKMatrix operator +(SKMatrix iLMatrix, SKMatrix iRMatrix)
            {
                return new SKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 + iRMatrix.R0C0,
                    R0C1 = iLMatrix.R0C1 + iRMatrix.R0C1,
                    R1C0 = iLMatrix.R1C0 + iRMatrix.R1C0,
                    R1C1 = iLMatrix.R1C1 + iRMatrix.R1C1,
                };
            }

            #endregion Public Methods

            #region Internal Methods

            internal static SKMatrix Identity(double v)
            {
                return new SKMatrix() { R0C0 = v, R1C1 = v };
            }

            #endregion Internal Methods
        }

        public struct SRMatrix
        {
            #region Public Fields

            public double R0C0;

            #endregion Public Fields

            #region Public Constructors

            public SRMatrix(double b)
            {
                R0C0 = b;
            }

            #endregion Public Constructors

            #region Public Methods

            public static SRMatrix operator -(SRMatrix iLMatrix, SRMatrix iRMatrix)
            {
                return new SRMatrix(iLMatrix.R0C0 - iRMatrix.R0C0);
            }

            public static TKalmanMatrix operator *(TKalmanMatrix iLMatrix, SRMatrix iRMatrix)
            {
                TKalmanMatrix result = new TKalmanMatrix(1, 2);
                result.Values[0, 0] = iLMatrix.Values[0, 0] * iRMatrix.R0C0;
                result.Values[0, 1] = iLMatrix.Values[0, 1] * iRMatrix.R0C0;
                //
                //for (int i = 0; i < iLMatrix.RankY; i++)
                //{
                //    for (int j = 0; j < iRMatrix.RankX; j++)
                //    {
                //        for (int k = 0; k < iLMatrix.RankX; k++)
                //        {
                //            result.Values[j, i] += iLMatrix.Values[k, i] * iRMatrix.Values[j, k];
                //        }
                //    }
                //}
                //
                return result;
            }

            public static SHKMatrix operator *(SHKMatrix iLMatrix, SRMatrix iRMatrix)
            {
                return new SHKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0,
                    R0C1 = iLMatrix.R0C1 * iRMatrix.R0C0,
                };
            }

            public static SRMatrix operator +(SRMatrix iLMatrix, SRMatrix iRMatrix)
            {
                return new SRMatrix(iLMatrix.R0C0 + iRMatrix.R0C0);
            }

            public static SRMatrix operator +(TKalmanMatrix iLMatrix, SRMatrix iRMatrix)
            {
                return new SRMatrix(iLMatrix.Values[0, 0] + iRMatrix.R0C0);
            }

            #endregion Public Methods

            #region Internal Methods

            internal SRMatrix Inversed()
            {
                return new SRMatrix(1 / R0C0);
            }

            #endregion Internal Methods
        }

        public struct SVKMatrix
        {
            #region Public Fields

            public double R0C0;
            public double R1C0;

            #endregion Public Fields

            #region Public Methods

            public static SRMatrix operator *(SVKMatrix iLMatrix, SHKMatrix iRMatrix)
            {
                return new SRMatrix() { R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0 + iLMatrix.R1C0 * iRMatrix.R0C1 };
            }

            public static SKMatrix operator *(SHKMatrix iLMatrix, SVKMatrix iRMatrix)
            {
                return new SKMatrix()
                {
                    R0C0 = iLMatrix.R0C0 * iRMatrix.R0C0,
                    R0C1 = iLMatrix.R0C1 * iRMatrix.R0C0,
                    R1C0 = iLMatrix.R0C0 * iRMatrix.R1C0,
                    R1C1 = iLMatrix.R0C1 * iRMatrix.R1C0,
                };
            }

            public static TKalmanMatrix operator *(SVKMatrix iLMatrix, TKalmanMatrix iRMatrix)
            {
                TKalmanMatrix result = new TKalmanMatrix(1, iRMatrix.RankY);
                return result;
            }

            #endregion Public Methods

            #region Internal Methods

            internal SHKMatrix Transposed()
            {
                return new SHKMatrix()
                {
                    R0C0 = R0C0,
                    R0C1 = R1C0,
                };
            }

            #endregion Internal Methods
        }

        #endregion Public Structs

        #region Public Classes

        public class TKalmanMatrix
        {
            #region Public Fields

            public double[,] Values = { };

            #endregion Public Fields

            #region Public Constructors

            public TKalmanMatrix(int iRankx, int iRanky)
            {
                RankX = iRankx;
                RankY = iRanky;
                Values = new double[RankX, RankY];
            }

            #endregion Public Constructors

            #region Public Properties

            public int RankX { get; private set; }
            public int RankY { get; private set; }

            #endregion Public Properties

            #region Public Methods

            public static TKalmanMatrix operator -(TKalmanMatrix iLMatrix, TKalmanMatrix iRMatrix)
            {
                TKalmanMatrix result = new TKalmanMatrix(iLMatrix.RankX, iRMatrix.RankY);
                //
                for (int i = 0; i < iLMatrix.RankY; i++)
                {
                    for (int j = 0; j < iLMatrix.RankX; j++)
                    {
                        result.Values[j, i] += iLMatrix.Values[j, i] - iRMatrix.Values[j, i];
                    }
                }
                //
                return result;
            }

            public static TKalmanMatrix operator *(TKalmanMatrix iLMatrix, TKalmanMatrix iRMatrix)
            {
                TKalmanMatrix result = new TKalmanMatrix(iRMatrix.RankX, iLMatrix.RankY);
                //
                for (int i = 0; i < iLMatrix.RankY; i++)
                {
                    for (int j = 0; j < iRMatrix.RankX; j++)
                    {
                        for (int k = 0; k < iLMatrix.RankX; k++)
                        {
                            result.Values[j, i] += iLMatrix.Values[k, i] * iRMatrix.Values[j, k];
                        }
                    }
                }
                //
                return result;
            }

            public static TKalmanMatrix operator +(TKalmanMatrix iLMatrix, TKalmanMatrix iRMatrix)
            {
                TKalmanMatrix result = new TKalmanMatrix(iLMatrix.RankX, iRMatrix.RankY);
                //
                for (int i = 0; i < iLMatrix.RankY; i++)
                {
                    for (int j = 0; j < iLMatrix.RankX; j++)
                    {
                        result.Values[j, i] += iLMatrix.Values[j, i] + iRMatrix.Values[j, i];
                    }
                }
                //
                return result;
            }

            public TKalmanMatrix Transposed()
            {
                TKalmanMatrix result = new TKalmanMatrix(RankY, RankX);
                //
                for (int i = 0; i < RankY; i++)
                {
                    for (int j = 0; j < RankX; j++)
                    {
                        result.Values[i, j] = Values[j, i];
                    }
                }
                return result;
            }

            #endregion Public Methods

            #region Internal Methods

            internal static TKalmanMatrix Identity(int rank)
            {
                TKalmanMatrix result = new TKalmanMatrix(rank, rank);
                for (int i = 0; i < result.RankX; i++)
                {
                    result.Values[i, i] = 1;
                }

                return result;
            }

            internal static TKalmanMatrix Identity(int rank, double v)
            {
                TKalmanMatrix result = new TKalmanMatrix(rank, rank);
                for (int i = 0; i < result.RankX; i++)
                {
                    result.Values[i, i] = v;
                }

                return result;
            }

            internal TKalmanMatrix Inversed()
            {
                TKalmanMatrix result = new TKalmanMatrix(RankY, RankX);
                //
                int[] colIdx = new int[RankX];
                int[] rowIdx = new int[RankX];
                int[] pivotIdx = new int[RankX];
                for (int i = 0; i < RankX; i++)
                {
                    pivotIdx[i] = -1;
                }

                for (int i = 0; i < RankX; i++)
                {
                    for (int j = 0; j < RankX; j++)
                    {
                        result.Values[i, j] = Values[i, j];
                    }
                }

                int icol = 0;
                int irow = 0;
                for (int i = 0; i < RankX; i++)
                {
                    double maxPivot = 0.0;
                    for (int j = 0; j < RankX; j++)
                    {
                        if (pivotIdx[j] != 0)
                        {
                            for (int k = 0; k < RankX; ++k)
                            {
                                if (pivotIdx[k] == -1)
                                {
                                    double absVal = System.Math.Abs(result.Values[j, k]);
                                    if (absVal > maxPivot)
                                    {
                                        maxPivot = absVal;
                                        irow = j;
                                        icol = k;
                                    }
                                }
                                else if (pivotIdx[k] > 0)
                                {
                                    return null;
                                }
                            }
                        }
                    }

                    ++(pivotIdx[icol]);

                    if (irow != icol)
                    {
                        for (int k = 0; k < RankX; ++k)
                        {
                            double f = result.Values[irow, k];
                            result.Values[irow, k] = result.Values[icol, k];
                            result.Values[icol, k] = f;
                        }
                    }

                    rowIdx[i] = irow;
                    colIdx[i] = icol;

                    double pivot = result.Values[icol, icol];

                    if (pivot == 0.0)
                    {
                        throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    }

                    double oneOverPivot = 1.0 / pivot;
                    result.Values[icol, icol] = 1.0;
                    for (int k = 0; k < RankX; ++k)
                    {
                        result.Values[icol, k] *= oneOverPivot;
                    }

                    for (int j = 0; j < RankX; ++j)
                    {
                        if (icol != j)
                        {
                            double f = result.Values[j, icol];
                            result.Values[j, icol] = 0.0;
                            for (int k = 0; k < RankX; ++k)
                            {
                                result.Values[j, k] -= result.Values[icol, k] * f;
                            }
                        }
                    }
                }

                for (int j = RankX - 1; j >= 0; --j)
                {
                    int ir = rowIdx[j];
                    int ic = colIdx[j];
                    for (int k = 0; k < RankX; ++k)
                    {
                        double f = result.Values[k, ir];
                        result.Values[k, ir] = result.Values[k, ic];
                        result.Values[k, ic] = f;
                    }
                }

                //result.Row0.X = inverse[0, 0];
                //result.Row0.Y = inverse[0, 1];
                //result.Row0.Z = inverse[0, 2];
                //result.Row1.X = inverse[1, 0];
                //result.Row1.Y = inverse[1, 1];
                //result.Row1.Z = inverse[1, 2];
                //result.Row2.X = inverse[2, 0];
                //result.Row2.Y = inverse[2, 1];
                //result.Row2.Z = inverse[2, 2];
                //
                TKalmanMatrix m = this * result;
                return result;
            }

            #endregion Internal Methods
        }

        #endregion Public Classes
    }
}