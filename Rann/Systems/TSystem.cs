using Rann.Components;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rann.Systems
{
    public abstract class TSystem : IDisposable
    {
        #region Public Fields

        [XmlIgnore]
        public readonly List<TParameterComponent[]> Solutions = new List<TParameterComponent[]>();

        [XmlIgnore]
        public Dictionary<string, TParameterComponent> Parameters = new Dictionary<string, TParameterComponent>();

        #endregion Public Fields

        #region Internal Fields

        [XmlIgnore]
        internal bool disposedValue;

        #endregion Internal Fields

        #region Private Fields

        private static TParameterComponentNameComparer ParameterNameComparer = new TParameterComponentNameComparer();
        private static TParameterComponentPositionRatioComparer ParameterPositionRatioComparer = new TParameterComponentPositionRatioComparer();

        #endregion Private Fields

        #region Public Properties

        [XmlArray("Targets")]
        [XmlArrayItem("Target")]
        public TParameterComponent[] TargetsForSerialization
        {
            get
            {
                List<TParameterComponent> lst = new List<TParameterComponent>(Parameters.Values);
                foreach (TParameterComponent p in lst.ToArray())
                    if (p.Movable) lst.Remove(p);
                lst.Sort(ParameterPositionRatioComparer);
                return lst.ToArray();
            }
            set
            {
                foreach (TParameterComponent p in value)
                    AddParameter(p);
            }
        }

        [XmlArray("Variables")]
        [XmlArrayItem("Variable")]
        public TParameterComponent[] VariablesForSerialization
        {
            get
            {
                List<TParameterComponent> lst = new List<TParameterComponent>(Parameters.Values);
                foreach (TParameterComponent p in lst.ToArray())
                    if (!p.Movable) lst.Remove(p);
                lst.Sort(ParameterNameComparer);
                return lst.ToArray();
            }
            set
            {
                foreach (TParameterComponent p in value)
                    AddParameter(p);
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void AddParameter(TParameterComponent iParameter)
        {
            Parameters.Add(iParameter.Name, iParameter);
        }

        public void AddParametersMatrix(TMatrix4 iParametersMatrix)
        {
            //AddParametersVector(iParametersMatrix.Row0);
            //AddParametersVector(iParametersMatrix.Row1);
            //AddParametersVector(iParametersMatrix.Row2);
            //AddParametersVector(iParametersMatrix.Row3);
        }

        public void AddParametersVector(TVector4 iParametersVector)
        {
            AddParameter((TParameterComponent)iParametersVector.X);
            AddParameter((TParameterComponent)iParametersVector.Y);
            AddParameter((TParameterComponent)iParametersVector.Z);
            AddParameter((TParameterComponent)iParametersVector.W);
        }

        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void GetParameters(ref TParameterComponent[] oObjectives, ref TParameterComponent[] oVariables)
        {
            // Clear or create the list
            List<TParameterComponent> Objectives = new List<TParameterComponent>();
            List<TParameterComponent> Variables = new List<TParameterComponent>();

            // Check each parameter of the system

            foreach (TParameterComponent objective in Parameters.Values)
            {
                // Put back all parameters to the inintiale position
                if (objective.RFormula == null)
                    objective.SetPosition(objective.StartingPosition);

                // Only enabled parameters can be taken into account
                if (objective.Enabled)
                {
                    // Only not movable parameters can be used as objectives
                    if (!objective.Movable)
                    {
                        // Only linked parameters can be used as objectives
                        if (objective.RFormula != null)
                        {
                            // Then, the objective must be linked to at least one enabled variable, or we will be able to do nothing with it
                            // So check all the sub-parameters the objective is linked to
                            foreach (TParameterComponent variable in objective.GetParameters())
                            {
                                // Only Enabled and Movable variable are interesting
                                if (variable.Enabled && variable.Movable)
                                {
                                    // Both variable and objective are interesting, so add them to the list
                                    if (!Objectives.Contains(objective)) Objectives.Add(objective);
                                    if (!Variables.Contains(variable)) Variables.Add(variable);
                                }
                            }
                        }
                    }
                }
            }
            oObjectives = Objectives.ToArray();
            oVariables = Variables.ToArray();
            Objectives.Clear();
            GC.SuppressFinalize(Objectives);
            Variables.Clear();
            GC.SuppressFinalize(Variables);
        }

        public bool IsASuccess(TParameterComponent[] Objectives)
        {
            foreach (TParameterComponent objective in Objectives)
            {
                // No need to take the control parameter into account
                if (objective.Name == "Reserved")
                    continue;
                // it at least one objective is not satisfied, then it's a loss
                if (objective.PositionState > EState.Warning)
                    return false;
            }

            // All objectives are satisfied, it's a win
            return true;
        }

        public void Solve()
        {
            //if (InProgress)
            //    return;
            //

            // Initialization of solutions
            Solutions.Clear();

            // Creation of the form
            //_Frm = New FProgress(Me)
            //_Frm.Show()

            SolveThread();
        }

        #endregion Public Methods

        #region Internal Methods

        internal abstract void SolveThread();

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                //If Not _Frm Is Nothing AndAlso Not _Frm.IsDisposed Then _Frm.Dispose()
                Parameters.Clear();
                Solutions.Clear();
                GC.SuppressFinalize(Parameters);
                GC.SuppressFinalize(Solutions);
                //if ((Thread != null))
                //    GC.SuppressFinalize(Thread);

                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        #endregion Protected Methods
    }
}