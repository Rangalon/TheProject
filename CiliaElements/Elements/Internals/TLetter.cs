using System;
using System.Collections.Generic;

namespace CiliaElements
{
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
            rdr.LoadXml(CiliaElements.Properties.Resources.letters);

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