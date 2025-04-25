using System;
using System.Xml;

namespace CiliaElements.Format3DXml
{
    public class TReferenceRep
    {
        #region Public Fields

        public TFile File;
        public string FileName = "";
        public int Id;
        public string PartNumber;

        #endregion Public Fields

        #region Public Constructors

        public TReferenceRep(XmlNode iXmlElmt)
        {
            int.TryParse(iXmlElmt.Attributes.GetNamedItem("id").Value, out Id);
            PartNumber = iXmlElmt.Attributes.GetNamedItem("name").Value;
            FileName = iXmlElmt.Attributes.GetNamedItem("associatedFile").Value;
            FileName = FileName.Split(':')[2];
        }

        #endregion Public Constructors
    }
}