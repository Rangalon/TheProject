
using Math3D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace CiliaElements.Format3DXml
{
    public class TInstance3D
    {
        #region Public Fields

        public List<TCustomAttr> Attributes;
        public int ChildId = -1;
        public int Id;
        public Mtx4 Matrix = Mtx4.Identity;
        public string NodeName;
        public int ParentId = -1;

        #endregion Public Fields

        #region Public Constructors

        public TInstance3D()
        {
        }

        public TInstance3D(XmlNode iXmlElmt)
        {
            TCustomAttr a;
            int.TryParse(iXmlElmt.Attributes.GetNamedItem("id").Value, out Id);
            NodeName = iXmlElmt.Attributes.GetNamedItem("name").Value;
            foreach (XmlElement XmlElmt in iXmlElmt.ChildNodes)
            {
                switch (XmlElmt.Name)
                {
                    case "V_description":
                        if (Attributes == null) Attributes = new List<TCustomAttr>();
                        a = new TCustomAttr
                        {
                            Name = XmlElmt.Name,
                            Value = XmlElmt.InnerText.Trim()
                        };
                        Attributes.Add(a);
                        break;

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

                    case "RelativeMatrix":
                        string[] table = XmlElmt.InnerText.Split(' ');// Array.ConvertAll(XmlElmt.InnerText.Split(' '), double.Parse);
                        Matrix.Row0.X = double.Parse(table[0], CultureInfo.InvariantCulture);
                        Matrix.Row0.Y = double.Parse(table[1], CultureInfo.InvariantCulture);
                        Matrix.Row0.Z = double.Parse(table[2], CultureInfo.InvariantCulture);
                        Matrix.Row1.X = double.Parse(table[3], CultureInfo.InvariantCulture);
                        Matrix.Row1.Y = double.Parse(table[4], CultureInfo.InvariantCulture);
                        Matrix.Row1.Z = double.Parse(table[5], CultureInfo.InvariantCulture);
                        Matrix.Row2.X = double.Parse(table[6], CultureInfo.InvariantCulture);
                        Matrix.Row2.Y = double.Parse(table[7], CultureInfo.InvariantCulture);
                        Matrix.Row2.Z = double.Parse(table[8], CultureInfo.InvariantCulture);
                        Matrix.Row3.X = double.Parse(table[9], CultureInfo.InvariantCulture) * 0.001;
                        Matrix.Row3.Y = double.Parse(table[10], CultureInfo.InvariantCulture) * 0.001;
                        Matrix.Row3.Z = double.Parse(table[11], CultureInfo.InvariantCulture) * 0.001;
                        break;

                    default:
                        throw new Exception("Unplanned Element!");
                }
            }
        }

        #endregion Public Constructors
    }
}