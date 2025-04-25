using Rann.Components;
using System.Globalization;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        //private static System.IO.StreamWriter wtr;

        #region Public Methods

        public static void BuildConstraints()
        {
            //try{
            //if (wtr == null) wtr = new System.IO.StreamWriter("c:\\temp\\TConstraintsManager.tst") { AutoFlush = true };
            Links.Clear();
            Sys.Parameters.Clear();
            //
            foreach (TConstraint Cst in WorldConstraints)
            {
                if (!Links.Contains(Cst.R1.L) && !Cst.R1.L.Fixed && !Cst.R1.L.Activated)
                {
                    Links.Add(Cst.R1.L);
                }

                if (!Links.Contains(Cst.R2.L) && !Cst.R2.L.Fixed && !Cst.R2.L.Activated)
                {
                    Links.Add(Cst.R2.L);
                }
            }
            //
            foreach (TReference r in WorldReferences)
            {
                foreach (TLink l in r.LinksPath)
                {
                    l.UpdateEquationsMatrix();
                }
            }
            //
            int Cpt = 0;
            foreach (TConstraint Cst in WorldConstraints)
            {
                foreach (TVector4 V in Cst.GetVectors())
                {
                    TParameterComponent PX = new TParameterComponent("P" + Cpt.ToString(CultureInfo.InvariantCulture), "x") { Enabled = true, Movable = false, RFormula = V.X };
                    Cpt++;
                    TParameterComponent PY = new TParameterComponent("P" + Cpt.ToString(CultureInfo.InvariantCulture), "y") { Enabled = true, Movable = false, RFormula = V.Y };
                    Cpt++;
                    TParameterComponent PZ = new TParameterComponent("P" + Cpt.ToString(CultureInfo.InvariantCulture), "z") { Enabled = true, Movable = false, RFormula = V.Z };
                    Cpt++;
                    PX.Weight = 10;
                    PY.Weight = 10;
                    PZ.Weight = 10;
                    Sys.AddParameter(PX);
                    Sys.AddParameter(PY);
                    Sys.AddParameter(PZ);
                }
            }
            //
            //foreach (TLink l in Links)
            //{
            //    foreach (TParameterComponent p in l.EquationsSystem.Parameters.Values)
            //    {
            //        Sys.AddParameter(p);
            //    }
            //}
            //
            Sys.BuildSystem();
            //foreach (TParameterComponent prmtr in Sys.Parameters.Values) { wtr.WriteLine(prmtr.ToString(CultureInfo.InvariantCulture)); }
            //foreach (TLink link in Links)
            //{
            //    foreach (TParameterComponent prmtr in link.EquationsSystem.Parameters.Values) { wtr.WriteLine(prmtr.ToString(CultureInfo.InvariantCulture)); }
            //}
            //   }
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0}\n{1}\n{2}", ex.Message, ex.Source, ex.StackTrace));
            //}
        }

        public static double[,] BuildReversedMatrix(double[,] iSquareMatrix)
        {
            // Reverse Square Matrix, Gauss method
            double Coef;
            int Size = iSquareMatrix.GetUpperBound(0);
            double[,] oReversedMatrix = new double[Size + 1, Size + 1];
            for (int i = 0; i <= Size; i++)
            {
                oReversedMatrix[i, i] = 1;
            }
            for (int i = 0; i <= Size; i++)
            {
                for (int j = i + 1; j <= Size && iSquareMatrix[i, i] == 0; j++)
                {
                    if (iSquareMatrix[j, i] != 0)
                    {
                        for (int k = 0; k <= Size; k++)
                        {
                            iSquareMatrix[i, k] += iSquareMatrix[j, k];
                            oReversedMatrix[i, k] += oReversedMatrix[j, k];
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
            return oReversedMatrix;
        }

        #endregion Public Methods

        #region Private Methods

        private static void DoerConstraints()
        {
            //try{
            //if (UpdateInProgress) return;
            while (true)
            {
                //
                foreach (TReference r in WorldReferences)
                {
                    foreach (TLink l in r.LinksPath)
                    {
                        l.UpdateEquationsMatrix();
                    }
                }
                //
                //State = "Building";
                //Build();
                Sys.Solve();
                foreach (TLink l in Links) { l.PushEquationsMatrix(); }
                Thread.Sleep(100);
                // State = "Waiting ; " + sw.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);
            }
            //   }
            //catch (Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0}\n{1}\n{2}", ex.Message, ex.Source, ex.StackTrace));
            //}
        }

        #endregion Private Methods
    }
}