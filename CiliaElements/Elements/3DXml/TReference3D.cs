using System;
using System.Collections.Generic;
using System.Xml;

namespace CiliaElements.Format3DXml
{
    public class TReference3D
    {
        #region Public Fields

        public int Id;
        public List<TCustomAttr> Attributes;

        public string PartNumber;

        #endregion Public Fields

        #region Public Constructors

        public TReference3D(XmlNode iXmlElmt)
        {
            TCustomAttr a;
            int.TryParse(iXmlElmt.Attributes.GetNamedItem("id").Value, out Id);
            PartNumber = iXmlElmt.Attributes.GetNamedItem("name").Value;

            foreach (XmlElement XmlElmt in iXmlElmt.ChildNodes)
            {
                switch (XmlElmt.Name)
                {
                    case "PLM_ExternalID":
                    case "V_version":
                    case "V_description":
                    case "V_Name":
                        if (Attributes == null) Attributes = new List<TCustomAttr>();
                        a = new TCustomAttr
                        {
                            Name = XmlElmt.Name,
                            Value = XmlElmt.InnerText.Trim()
                        };
                        Attributes.Add(a);
                        break;         
                    default:
                        throw new Exception("Unplanned Element!");
                }
            }
        }

        #endregion Public Constructors
    }
}