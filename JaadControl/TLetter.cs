using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace JaadWinControls
{
    public struct SLine
    {
        #region Public Fields

        public Vec4 EndPoint;
        public Vec4 StartPoint;

        #endregion Public Fields

        #region Public Constructors

        public SLine(String iXs, String iYs, String iXe, String iYe)
        {
            StartPoint = new Vec4(-double.Parse(iXs, CultureInfo.InvariantCulture) * TLetter.LetterWidth * 0.5, double.Parse(iYs, CultureInfo.InvariantCulture) * TLetter.LetterHeigth * 0.5, 0, 1);
            EndPoint = new Vec4(-double.Parse(iXe, CultureInfo.InvariantCulture) * TLetter.LetterWidth * 0.5, double.Parse(iYe, CultureInfo.InvariantCulture) * TLetter.LetterHeigth * 0.5, 0, 1);
        }

        public SLine(Single iXs, Single iYs, Single iXe, Single iYe)
        {
            StartPoint = new Vec4(-iXs * TLetter.LetterWidth * 0.5, iYs * TLetter.LetterHeigth * 0.5, 0, 1);
            EndPoint = new Vec4(-iXe * TLetter.LetterWidth * 0.5, iYe * TLetter.LetterHeigth * 0.5, 0, 1);
        }

        #endregion Public Constructors

        #region Public Methods

        public static bool operator !=(SLine s1, SLine s2)
        {
            if (s1.EndPoint != s2.EndPoint) return true;
            if (s1.StartPoint != s2.StartPoint) return true;
            return false;
        }

        public static bool operator ==(SLine s1, SLine s2)
        {
            if (s1.EndPoint != s2.EndPoint) return false;
            if (s1.StartPoint != s2.StartPoint) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Public Methods
    }

    public class TLetter
    {
        #region Public Fields

        public const Double DisplayFloor = 0.2;
        public const Double LetterGap = LetterHeigth * 0.4;
        public const Double LetterHeigth = 2.2;
        public const Double LetterScale = 0.01;
        public const Double LetterWidth = LetterHeigth;
        public static Dictionary<char, TLetter> Letters;

        #endregion Public Fields

        #region Private Fields

        private static Dictionary<char, SLine[]> lettersDico;

        #endregion Private Fields

        #region Public Constructors

        static TLetter()
        {
            Letters = new Dictionary<char, TLetter>();
            lettersDico = new Dictionary<char, SLine[]>();

            System.Xml.XmlDocument rdr = new System.Xml.XmlDocument();
            rdr.LoadXml(JaadControl.Properties.Resources.letters);

            foreach (System.Xml.XmlElement XmlLetter in rdr.SelectNodes("//LETTER"))
            {
                SLine[] tbl = { };
                Array.Resize(ref tbl, XmlLetter.ChildNodes.Count);
                for (Int32 i = 0; i < XmlLetter.ChildNodes.Count; i++)
                {
                    System.Xml.XmlNode Elmt = XmlLetter.ChildNodes[i];
                    tbl[i] = new SLine(Elmt.Attributes.GetNamedItem("x1").Value, Elmt.Attributes.GetNamedItem("y1").Value, Elmt.Attributes.GetNamedItem("x2").Value, Elmt.Attributes.GetNamedItem("y2").Value);
                }
                lettersDico.Add(XmlLetter.GetAttribute("letter").ToCharArray()[0], tbl);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public static Dictionary<char, SLine[]> LettersDico
        {
            get { return lettersDico; }
        }

        #endregion Public Properties
    }
}