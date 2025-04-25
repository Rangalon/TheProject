using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rann.Components
{
    /// <summary>
    /// This class is used to manage parameters for functions
    /// It can be a variable or an objective
    /// It can also have a formula, if it's value is dependant of others parameters
    /// </summary>
    /// <remarks></remarks>

    public class TParameterComponent : TComponent, IName, IRFormula
    {
        #region Public Fields

        [XmlAttribute]
        public bool Bounded;

        [XmlAttribute]
        public bool Enabled;

        [XmlAttribute]
        public double Maximum;

        [XmlAttribute]
        public double Minimum;

        [XmlAttribute]
        public double Modulo;

        [XmlAttribute]
        public bool Movable;

        [XmlIgnore]
        public TComponent RFormula;

        [XmlAttribute]
        public double StartingPosition;

        [XmlAttribute]
        public double Weight;

        #endregion Public Fields

        #region Internal Fields

        internal static List<TParameterComponent> AllParameters = new List<TParameterComponent>();

        #endregion Internal Fields

        #region Private Fields

        [XmlIgnore]
        private double _Position;

        #endregion Private Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public TParameterComponent(string iName, string iSubName)
        {
            AllParameters.Add(this);
            if (TCalculation.Current != null) lock (TCalculation.Current.Parameters) TCalculation.Current.Parameters.Add(this);
            Name = iName;
            SubName = iSubName;
            Checked = true;
        }

        public TParameterComponent()
        {
            AllParameters.Add(this);
            if (TCalculation.Current != null) lock (TCalculation.Current.Parameters) TCalculation.Current.Parameters.Add(this);
            Name = "P" + AllParameters.Count.ToString();
        }

        #endregion Public Constructors

        #region Public Properties

        [XmlAttribute]
        public bool Checked { get; set; } = false;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [System.ComponentModel.Browsable(false)]
        [XmlIgnore]
        public TComponent FormulaRatio
        {
            get
            {
                if (RFormula == null)
                {
                    return (this - new TValueComponent(Middle)) / new TValueComponent(Range);
                }
                else
                {
                    return (RFormula - new TValueComponent(Middle)) / new TValueComponent(Range);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public bool IsBlocked
        {
            get
            {
                if (!Bounded)
                    return false;
                return Math.Abs(PositionRatio) >= 1;
            }
        }

        public override byte Level => 20;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public double Middle
        {
            get { return (Maximum + Minimum) / 2; }
        }

        [XmlAttribute]
        public string Name { get; set; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public override double Position
        {
            get
            {
                if (RFormula == null)
                {
                    return _Position;
                }
                else
                {
                    return RFormula.Position;
                }
            }
            set { throw new Exception("Not Allowed!"); }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public double PositionRatio
        {
            get { return (Position - Middle) / Range; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public EState PositionState
        {
            get
            {
                if (double.IsNaN(PositionRatio)) return EState.Incoherent;
                int i = -(int)(-Math.Abs(Math.Round(PositionRatio, 3)) / 0.25);
                if (i < 4)
                    return EState.Ok;
                else if (i < 5)
                    return EState.Warning;
                else if (i < 41)
                    return EState.Bad;
                else
                    return EState.Incoherent;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [System.ComponentModel.Browsable(false)]
        [XmlIgnore]
        public double Range
        {
            get
            {
                if (Maximum == Minimum)
                    return 1E-05;
                return (Maximum - Minimum) / 2;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlAttribute]
        public string RFormulaText
        {
            get
            {
                if (RFormula == null)
                    return "";
                return RFormula.ToString();
            }
            set
            {
                RFormula = TComponent.ParseFormula(value);
            }
        }

        [XmlAttribute]
        public string SubName { get; set; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public override string Text
        {
            get
            {
                string s;
                if (SubName == "") s = Name;
                else s = Name + "." + SubName;
                // if (RFormula != null) s += " = " + RFormula.Text;
                return s;
            }
        }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void AddInfluence(TParameterComponent iParameter, double iCoefficient)
        {
            // If the coefficient is null, it's useless
            if (iCoefficient == 0)
                return;
            // If there is at least one influence, then the position of the parameter must be managed through a formula
            TComponent influence = default(TComponent);
            // If the formula does not exist yet, we create it from the numerical value of the parameter
            if (RFormula == null)
                RFormula = new TValueComponent(Position);
            // Then, we enrich the formula with the influence
            influence = iParameter * new TValueComponent(iCoefficient);
            RFormula = RFormula + influence;
            // Then, if the initial value of the influence is not null, we move the initial numerical value of the formula, in order to respect the numerical value of the position
            if (influence.Position != 0)
                RFormula -= new TValueComponent(influence.Position);
        }

        public override TComponent Factorises(TComponent factor)
        {
            if (this == factor) return 1;
            return this / factor;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            switch (iParameter.Text == Text)
            {
                case true:
                    return 1;

                default:
                    if (RFormula == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return RFormula.GetDifferential(iParameter);
                    }
            }
        }

        public override TComponent[] GetFactors()
        {
            return new TComponent[] { this };
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TParameterComponent[] GetParameters()
        {
            if (RFormula == null)
            {
                return new TParameterComponent[1] { this };
            }
            else
            {
                return RFormula.GetParameters();
            }
        }

        public void PushFormula()
        {
            foreach (TParameterComponent prmtr in TCalculation.Current.Parameters.Where(o => o.RFormula != null))
            {
                prmtr.RFormula = prmtr.RFormula.ReplaceComponent(this, RFormula);
            }

            foreach (TMatrix4 mtx in TCalculation.Current.Matrixes)
            {
                mtx.Xx = mtx.Xx.ReplaceComponent(this, RFormula);
                mtx.Xy = mtx.Xy.ReplaceComponent(this, RFormula);
                mtx.Xz = mtx.Xz.ReplaceComponent(this, RFormula);
                mtx.Xw = mtx.Xw.ReplaceComponent(this, RFormula);
                mtx.Yx = mtx.Yx.ReplaceComponent(this, RFormula);
                mtx.Yy = mtx.Yy.ReplaceComponent(this, RFormula);
                mtx.Yz = mtx.Yz.ReplaceComponent(this, RFormula);
                mtx.Yw = mtx.Yw.ReplaceComponent(this, RFormula);
                mtx.Zx = mtx.Zx.ReplaceComponent(this, RFormula);
                mtx.Zy = mtx.Zy.ReplaceComponent(this, RFormula);
                mtx.Zz = mtx.Zz.ReplaceComponent(this, RFormula);
                mtx.Zw = mtx.Zw.ReplaceComponent(this, RFormula);
                mtx.Ox = mtx.Ox.ReplaceComponent(this, RFormula);
                mtx.Oy = mtx.Oy.ReplaceComponent(this, RFormula);
                mtx.Oz = mtx.Oz.ReplaceComponent(this, RFormula);
                mtx.Ow = mtx.Ow.ReplaceComponent(this, RFormula);
            }

            foreach (TVector4 vec in TCalculation.Current.Vectors)
            {
                vec.X = vec.X.ReplaceComponent(this, RFormula);
                vec.Y = vec.Y.ReplaceComponent(this, RFormula);
                vec.Z = vec.Z.ReplaceComponent(this, RFormula);
                vec.W = vec.W.ReplaceComponent(this, RFormula);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TComponent Reduced()
        {
            return this;
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
            if (RFormula == null) return;
            if (RFormula == oldComp) RFormula = newComp; else RFormula.Replace(oldComp, newComp);
        }

        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            if (this.ToString() == iOldComp.ToString()) return iNewComp;
            return this;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetPosition(double iValue)
        {
            // First, manage modulo if declared
            if (Modulo != 0)
            {
                iValue -= Minimum;
                while (iValue < 0)
                {
                    iValue += Modulo;
                }
                iValue = iValue % Modulo + Minimum;
            }
            // Second, manage frontiers if they must be respected
            if (Bounded)
            {
                if (iValue > Maximum)
                    iValue = Maximum;
                if (iValue < Minimum)
                    iValue = Minimum;
            }
            // Then, we can set the position
            if (RFormula != null)
            {
                RFormula += new TValueComponent(iValue - Position);
            }
            else
            {
                _Position = iValue;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void SetPositionRatio(double iDouble)
        {
            SetPosition(iDouble * Range + Middle);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            if (RFormula == null) return base.ToString();
            return base.ToString() + " {" + RFormula.ToString() + "}";
        }

        #endregion Public Methods
      
        #region Protected Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if ((RFormula != null)) { RFormula.Dispose(); GC.SuppressFinalize(RFormula); }
            }
            base.Dispose(disposing);
        }

        #endregion Protected Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    public class TParameterComponentNameComparer : IComparer<TParameterComponent>
    {
        #region Public Methods

        int IComparer<TParameterComponent>.Compare(TParameterComponent x, TParameterComponent y)
        {
            if (x.Name != y.Name) return x.Name.CompareTo(y.Name);
            return x.SubName.CompareTo(y.SubName);
        }

        #endregion Public Methods
    }

    public class TParameterComponentPositionRatioComparer : IComparer<TParameterComponent>
    {
        #region Public Methods

        int IComparer<TParameterComponent>.Compare(TParameterComponent x, TParameterComponent y)
        {
            return Math.Abs(x.PositionRatio).CompareTo(Math.Abs(y.PositionRatio));
        }

        #endregion Public Methods
    }
}