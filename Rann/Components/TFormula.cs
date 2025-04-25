using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rann.Components
{
    public class TFormula : IName, IRFormula, ILFormula, IReplacement
    {
        #region Public Fields

        [XmlIgnore]
        public TComponent LFormula;

        [XmlIgnore]
        public TComponent RFormula;

        #endregion Public Fields

        #region Public Constructors

        public TFormula()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        [XmlAttribute]
        public string LFormulaText
        {
            get
            {
                if (LFormula == null)
                    return "";
                return LFormula.ToString();
            }
            set
            {
                LFormula = TComponent.ParseFormula(value);
            }
        }

        [XmlAttribute]
        public string Name { get; set; }

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

        #endregion Public Properties

        #region Public Methods

        public void Replace(TComponent oldComp, TComponent newComp)
        {
            LFormula.Replace(oldComp, newComp);
            RFormula.Replace(oldComp, newComp);
        }

        #endregion Public Methods

        #region Internal Methods

        internal Array GetParameters()
        {
            return LFormula.GetParameters().Concat(RFormula.GetParameters()).ToArray();
        }

        #endregion Internal Methods
    }
}