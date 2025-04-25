using Rann.Components;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CiliaElements
{
    public class TReference
    {
        #region Public Fields

        public bool Disabled;
        public TLink L;
        public List<TLink> LinksPath = new List<TLink>();
        public TVector4 O = new TVector4();
        public TVector4 P1 = new TVector4();
        public TVector4 P2 = new TVector4();
        public TVector4 P3 = new TVector4();
        public EReferenceType ReferenceType;
        public TVector4 U = new TVector4();
        public TVector4 V = new TVector4();
        public TVector4 W = new TVector4();

        #endregion Public Fields

        #region Private Fields

        private readonly TLink P;

        #endregion Private Fields

        #region Public Constructors

        public TReference(TLink iL, TLink iP, System.Xml.XmlNode iXmlElmt)
        {
            TManager.WorldReferences.Add(this);
            L = iL;
            P = iP;
            if (iXmlElmt != null)
            {
                foreach (System.Xml.XmlAttribute XmlAttr in iXmlElmt.Attributes)
                {
                    switch (XmlAttr.Name)
                    {
                        case "x":
                            O.X = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "y":
                            O.Y = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "z":
                            O.Z = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "ux":
                            U.X = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "uy":
                            U.Y = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "uz":
                            U.Z = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "vx":
                            V.X = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "vy":
                            V.Y = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "vz":
                            V.Z = new TValueComponent(double.Parse(XmlAttr.Value, CultureInfo.InvariantCulture));
                            break;

                        case "t":
                            ReferenceType = (EReferenceType)int.Parse(XmlAttr.Value, CultureInfo.InvariantCulture);
                            break;

                        default:
                            break;
                    }
                }
            }
            O /= 1000.0;
            O.W = new TValueComponent(1);
            if (U.LengthSquared.Position > 0) U.Normalize();
            if (V.LengthSquared.Position > 0) V.Normalize();
            W = U * V;
            if (W.LengthSquared.Position > 0) W.Normalize();
            P1 = O + U;
            P2 = O + V;
            P3 = (TVector4)(O - U - V);
            //
            TLink l = L;
            while (l != P)
            {
                LinksPath.Add(l);
                l = l.ParentLink;
            }
        }

        #endregion Public Constructors

        #region Public Methods

        public TVector4 GetVector(TVector4 iV)
        {
            foreach (TLink l in LinksPath) { iV = l.EquationsMatrix * iV; }
            return iV;
        }

        #endregion Public Methods
    }
}