using Rann.Components;
using System;

namespace Rann.Systems
{
    public class TOverConstrainedSystem : TSystem
    {
        //private static System.Xml.Serialization.XmlSerializer XS = new System.Xml.Serialization.XmlSerializer(typeof(TOverConstrainedSystem));

        #region Private Fields

        [System.Xml.Serialization.XmlIgnore]
        private TComponent[,] DifferentialFormulasMatrix = null;

        [System.Xml.Serialization.XmlIgnore]
        private TParameterComponent[] Objectives = null;

        [System.Xml.Serialization.XmlIgnore]
        private TParameterComponent[] Variables = null;

        #endregion Private Fields

        #region Public Methods

        public static bool IsNotReserved(TParameterComponent iParameter)
        {
            return !IsReserved(iParameter);
        }

        public static bool IsReserved(TParameterComponent iParameter)
        {
            return iParameter.Name == "Reserved";
        }

        public void BuildSystem()
        {
            // Build the lists
            GetParameters(ref Objectives, ref Variables);
            DifferentialFormulasMatrix = BuildDifferentialMatrix(Variables, Objectives);
        }

        public void Save(System.IO.FileInfo iFi)
        {
            //System.IO.StreamWriter wtr = new System.IO.StreamWriter(iFi.FullName);
            //XS.Serialize(wtr, this);
            //wtr.Close();
            //wtr.Dispose();
        }

        #endregion Public Methods

        #region Internal Methods

        internal override void SolveThread()
        {
            if (DifferentialFormulasMatrix == null) return;
            // Initialize power variable
            // Update form progress bar
            //If MaxPower > 0 Then _Frm.SetMaxSteps(Math.Log(MaxPower) / Math.Log(2) + 1)

            // Build the control parameter, and reset the variables
            //For Each variable As TParameterComponent In Variables
            //    Dim p0 As New TParameterComponent("Reserved") With {.Formula = Zero}
            //    p0.Maximum = 1
            //    p0.Minimum = -1
            //    p0.Movable = False
            //    p0.Enabled = True
            //    p0.Bounded = False
            //    variable.SetPosition(variable.StartingPosition)
            //    p0.Formula += variable.FormulaRatio - New TValueComponent(variable.PositionRatio)
            //    Objectives = Objectives.Concat({p0}).ToArray
            //Next
            // Add the control parameter to objectives
            // Counter for computations
            int cpt = 0;
            // Loop while the power is smaller or equal to the maximum
            // Initiliaze Retry
            bool Retry = true;
            // Prepare differential matrix formulas
            // Loop while the retry is requested and the computation number lower than 100
            while (Retry & cpt < 100)
            {
                //foreach (TParameterComponent p in Objectives)
                //{
                //    if (p.Name == "Reserved")
                //        p.Formula -= new TValueComponent(p.Position);
                //}
                // Force the update of the progress form, to prove that it's still alive
                // Reset the retry
                Retry = false;
                // Prepare differential matrix
                double[,] DifferentialMatrix = null;
                // Fill differential matrix
                DifferentialMatrix = TFunctions.FillDifferentialMatrix(DifferentialFormulasMatrix);
                // Prepare objectives vector
                double[] ObjectivesVector = null;
                // Fill objectives vector
                ObjectivesVector = BuildObjectivesVector(Objectives);
                Array.Resize(ref ObjectivesVector, ObjectivesVector.Length + Variables.Length);
                // Prepare square matrix
                double[,] SquareDifferentialMatrix = null;
                // Prepera square vector
                double[] SquareObjectivesVector = null;
                // Fill square matrix & vector
                TFunctions.BuildSquares(DifferentialMatrix, ObjectivesVector, ref SquareDifferentialMatrix, ref SquareObjectivesVector);
                // Prepare variables vector
                double[] VariablesVector = null;
                // Fill variable vector
                TFunctions.BuildVariablesVector(SquareDifferentialMatrix, SquareObjectivesVector, ref VariablesVector);
                // Apply the modification to each variable
                for (int i = 0; i <= Variables.Length - 1; i++)
                {
                    // Do it only if it's a numerical value
                    if (!double.IsInfinity(VariablesVector[i]) && !double.IsNaN(VariablesVector[i]))
                    {
                        //if (VariablesVector[i] > 0.05) VariablesVector[i] = 0.05;
                        //if (VariablesVector[i] < -0.05) VariablesVector[i] = -0.05;

                        // Apply modifications on variables
                        Variables[i].SetPositionRatio(Variables[i].PositionRatio + VariablesVector[i]);
                        // Set the retry if at least one variable has been modified significantly
                        if (Math.Abs(VariablesVector[i]) > 0.0001 && !Variables[i].IsBlocked)
                            Retry = true;
                    }
                }
                //
                cpt += 1;
            }
            // Update progress form
            //_Frm.Incremente()
            // Move to next power
            // Stop computations if all objectives are satisfied
            // Warn that the solver has finished
            //Finish(cpt, Array.FindAll<TParameterComponent>(Objectives, IsNotReserved), Variables);
        }

        #endregion Internal Methods

        #region Private Methods

        private TComponent[,] BuildDifferentialMatrix(TParameterComponent[] iVariables, TParameterComponent[] iObjectives)
        {
            // Initialization of the matrix
            TComponent[,] oDifferentialMatrix = new TComponent[iVariables.Length, iObjectives.Length];

            // Work with each variable
            for (int j = 0; j <= iVariables.Length - 1; j++)
            {
                TParameterComponent variable = iVariables[j];
                // Work with each objective
                for (int i = 0; i <= iObjectives.Length - 1; i++)
                {
                    TParameterComponent objective = iObjectives[i];
                    // If there is a formula, then we can build a differential
                    TComponent diff = objective.RFormula.GetDifferential(variable);
                    if (diff == 0)
                    {
                        oDifferentialMatrix[j, i] = 0;
                    }
                    else
                    {
                        // Simplified function when iPower=1 (x^0=1)
                        oDifferentialMatrix[j, i] = -new TValueComponent(variable.Range / objective.Range) * diff;

                        // Apply weight of the parameter
                        oDifferentialMatrix[j, i] = oDifferentialMatrix[j, i] * new TValueComponent(objective.Weight);
                    }
                }
            }
            return oDifferentialMatrix;
        }

        private double[] BuildObjectivesVector(TParameterComponent[] iObjectives)
        {
            // Initialization of the vector
            double[] oObjectiveVector = new double[iObjectives.Length];
            // Work with each objective
            for (int i = 0; i <= iObjectives.Length - 1; i++)
            {
                oObjectiveVector[i] = iObjectives[i].PositionRatio;
                oObjectiveVector[i] *= iObjectives[i].Weight;
            }
            return oObjectiveVector;
        }

        #endregion Private Methods
    }
}