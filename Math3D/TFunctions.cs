namespace Math3D
{
    public static class TFunctions
    {
        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BuildReversedMatrix(double[,] iSquareMatrix, ref double[,] oReversedMatrix)
        {
            // Reverse Square Matrix, Gauss method
            int Size = iSquareMatrix.GetUpperBound(0);
            oReversedMatrix = new double[Size + 1, Size + 1];
            for (int i = 0; i <= Size; i++)
            {
                oReversedMatrix[i, i] = 1;
            }
            double Coef;
            for (int i = 0; i <= Size; i++)
            {
                if (iSquareMatrix[i, i] == 0)
                {
                    for (int j = i + 1; j <= Size; j++)
                    {
                        if (iSquareMatrix[j, i] != 0)
                        {
                            for (int k = 0; k <= Size; k++)
                            {
                                iSquareMatrix[i, k] += iSquareMatrix[j, k];
                                oReversedMatrix[i, k] += oReversedMatrix[j, k];
                            }
                            break; // TODO: might not be correct. Was : Exit For
                        }
                    }
                }
                for (int j = 0; j <= Size; j++)
                {
                    if (j != i)
                    {
                        if (iSquareMatrix[j, i] != 0)
                        {
                            Coef = iSquareMatrix[j, i] / iSquareMatrix[i, i];
                            for (int k = 0; k <= Size; k++)
                            {
                                iSquareMatrix[j, k] -= Coef * iSquareMatrix[i, k];
                                oReversedMatrix[j, k] -= Coef * oReversedMatrix[i, k];
                            }
                        }
                    }
                }
            }
            for (int i = 0; i <= Size; i++)
            {
                Coef = iSquareMatrix[i, i];
                for (int j = 0; j <= Size; j++)
                {
                    oReversedMatrix[i, j] /= Coef;
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BuildSquares(double[,] iRectangleMatrix, double[] iRectangleVector, ref double[,] oSquareMatrix, ref double[] oSquareVector)
        {
            int Size = iRectangleMatrix.GetUpperBound(0);
            int Length = iRectangleMatrix.GetUpperBound(1);
            oSquareMatrix = new double[Size + 1, Size + 1];
            oSquareVector = new double[Size + 1];
            for (int i = 0; i <= Size; i++)
            {
                oSquareVector[i] = 0;
                for (int k = 0; k <= Length; k++)
                {
                    oSquareVector[i] += iRectangleMatrix[i, k] * iRectangleVector[k];
                }
                for (int j = 0; j <= Size; j++)
                {
                    oSquareMatrix[i, j] = 0;
                    for (int k = 0; k <= Length; k++)
                    {
                        oSquareMatrix[i, j] += iRectangleMatrix[i, k] * iRectangleMatrix[j, k];
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void BuildVariablesVector(double[,] iSquareDifferentialMatrix, double[] iSquareObjectivesVector, ref double[] oVariablesVector)
        {
            int Size = iSquareObjectivesVector.GetUpperBound(0);
            //
            oVariablesVector = new double[Size + 1];
            //
            double[,] ReversedMatrix = null;
            BuildReversedMatrix(iSquareDifferentialMatrix, ref ReversedMatrix);
            //
            for (int i = 0; i <= Size; i++)
            {
                oVariablesVector[i] = 0;
                for (int j = 0; j <= Size; j++)
                {
                    oVariablesVector[i] += iSquareObjectivesVector[j] * ReversedMatrix[i, j];
                }
            }
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}