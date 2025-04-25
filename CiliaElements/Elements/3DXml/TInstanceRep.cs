using System;
using System.Xml;

namespace CiliaElements.Format3DXml
{
    public class TInstanceRep
    {
        #region Public Fields

        public int ChildId = -1;
        public int Id;
        public string NodeName;
        public int ParentId = -1;

        #endregion Public Fields

        #region Public Constructors

        public TInstanceRep(XmlNode iXmlElmt)
        {
            int.TryParse(iXmlElmt.Attributes.GetNamedItem("id").Value, out Id);
            NodeName = iXmlElmt.Attributes.GetNamedItem("name").Value;
            foreach (XmlElement XmlElmt in iXmlElmt.ChildNodes)
            {
                switch (XmlElmt.Name)
                {
                    case "IsAggregatedBy":
                        if (ParentId < 0)
                            int.TryParse(XmlElmt.InnerText, out ParentId);
                        else
                            throw new Exception("Parent Already valued");
                        break;

                    case "IsInstanceOf":
                        if (ChildId < 0)
                            int.TryParse(XmlElmt.InnerText, out ChildId);
                        else
                            throw new Exception("Child Already valued");
                        break;

                    default:
                        throw new Exception("Unplanned Element!");
                }
            }
        }

        #endregion Public Constructors
    }
}