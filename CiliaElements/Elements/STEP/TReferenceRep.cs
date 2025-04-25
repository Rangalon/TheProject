using System;
using System.Xml;

namespace CiliaElements.FormatSTEP
{
    public class TReferenceRep : IDisposable
    {
        public void Dispose()
        {
            PartNumber = null; Formation = null;
            Definition = null; Representation = null;
            Description = null; TDSM_Type = null;
        }
        #region Public Fields

        public string PartNumber;
        internal string Formation;
        internal string Definition;
        internal TShapeRepresentation Representation;
        internal string Description;
        internal string TDSM_Type;

        #endregion Public Fields

        #region Public Constructors

        public override string ToString()
        {
            return PartNumber;
        }

        #endregion Public Constructors
    }
}