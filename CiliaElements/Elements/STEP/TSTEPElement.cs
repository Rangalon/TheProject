using CiliaElements.Internals;
using Math3D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

//using static System.Net.Mime.MediaTypeNames;

namespace CiliaElements.FormatSTEP
{
    public class TSTEPElement : TAssemblyElement
    {
        #region Public Fields

        public static Regex RgxAXIS2_PLACEMENT_3D = new Regex(@"AXIS2_PLACEMENT_3D[(]'(.*)',#(.*),#(.*),#(.*)[)];");
        public static Regex RgxCARTESIAN_POINT = new Regex(@"CARTESIAN_POINT[(]'(.*)',[(](.*),(.*),(.*)[)][)];");
        public static Regex RgxDESCRIPTIVE_REPRESENTATION_ITEM = new Regex(@"DESCRIPTIVE_REPRESENTATION_ITEM[(]'(.*)','(.*)'[)];");
        public static Regex RgxDIRECTION = new Regex(@"DIRECTION[(]'(.*)',[(](.*),(.*),(.*)[)][)];");
        public static Regex RgxDOCUMENT_FILE = new Regex(@"DOCUMENT_FILE[(]'(.*)','(.*)','(.*)',#(.*),'(.*)','(.*)'[)];");
        public static Regex RgxITEM_DEFINED_TRANSFORMATION = new Regex(@"ITEM_DEFINED_TRANSFORMATION[(]'(.*)',[$],#(.*),#(.*)[)];");
        public static Regex RgxNEXT_ASSEMBLY_USAGE_OCCURRENCE = new Regex(@"NEXT_ASSEMBLY_USAGE_OCCURRENCE[(]'(.*)','(.*)',[$],#(.*),#(.*),[$][)];");
        public static Regex RgxPRODUCT = new Regex(@"PRODUCT[(]'(.*)','(.*)',[$],[(]#(.*)[)][)];");
        public static Regex RgxPRODUCT_DEFINITION = new Regex(@"PRODUCT_DEFINITION[(]'(.*)',[$],#(.*),#(.*)[)];");
        public static Regex RgxPRODUCT_DEFINITION_FORMATION = new Regex(@"PRODUCT_DEFINITION_FORMATION[(]'(.*)',[$],#(.*)[)];");
        public static Regex RgxPRODUCT_DEFINITION_SHAPE = new Regex(@"PRODUCT_DEFINITION_SHAPE[(]'(.*)',[$],#(.*)[)];");
        public static Regex RgxPROPERTY_DEFINITION = new Regex(@"PROPERTY_DEFINITION[(]'(.*)',[$],#(.*)[)];");
        public static Regex RgxPROPERTY_DEFINITION_REPRESENTATION = new Regex(@"PROPERTY_DEFINITION_REPRESENTATION[(]#(.*),#(.*)[)];");
        public static Regex RgxREPRESENTATION = new Regex(@"REPRESENTATION[(]'(.*)',[(]#(.*)[)],#(.*)[)];");
        public static Regex RgxREPRESENTATION_RELATIONSHIP = new Regex(@"[(]REPRESENTATION_RELATIONSHIP[(]'(.*)',[$],#(.*),#(.*)[)]REPRESENTATION_RELATIONSHIP_WITH_TRANSFORMATION[(]#(.*)[)]SHAPE_REPRESENTATION_RELATIONSHIP[(][)][)];");
        public static Regex RgxSHAPE_DEFINITION_REPRESENTATION = new Regex(@"SHAPE_DEFINITION_REPRESENTATION[(]#(.*),#(.*)[)];");
        public static Regex RgxSHAPE_REPRESENTATION = new Regex(@"SHAPE_REPRESENTATION[(]'(.*)',[(](.*)[)],#(.*)[)];");

        public TDico<int, TReferenceRep> ReferencesReps = new TDico<int, TReferenceRep>();

        #endregion Public Fields

        #region Protected Fields

        protected Dictionary<int, object> Objects = new Dictionary<int, object>();

        #endregion Protected Fields

        #region Private Fields

        private static Dictionary<string, DoVerb_CB> Doers = new Dictionary<string, DoVerb_CB>();

        #endregion Private Fields

        #region Public Constructors


        public static Mtx4f Black = new Mtx4f();
        public static Mtx4f White = Mtx4f.Identity;
        public static Mtx4f LightGray = Mtx4f.Identity;
        public static Mtx4f LightBlue = Mtx4f.Identity; 


        static TSTEPElement()
        {
            Black = new Mtx4f(); Black.Row3.W = 1;
            White = Mtx4f.Identity;
            LightGray = Mtx4f.Identity; LightGray.Row0.X = 0.75f; LightGray.Row1.Y = 0.75f; LightGray.Row2.Z = 0.75f;
            LightBlue = Mtx4f.Identity; LightBlue.Row0.X = 0.75f; LightBlue.Row1.Y = 0.75f; LightBlue.Row2.Z = 1f;

            Doers.Add("CARTESIAN_POINT(", DoCARTESIAN_POINT);
            Doers.Add("DIRECTION(", DoDIRECTION);
            Doers.Add("AXIS2_PLACEMENT_3D(", DoAXIS2_PLACEMENT_3D);
            Doers.Add("ITEM_DEFINED_TRANSFORMATION(", DoITEM_DEFINED_TRANSFORMATION);
            Doers.Add("PRODUCT(", DoPRODUCT);
            Doers.Add("DOCUMENT_FILE(", DoDOCUMENT_FILE);
            Doers.Add("PRODUCT_DEFINITION_FORMATION(", DoPRODUCT_DEFINITION_FORMATION);
            Doers.Add("PRODUCT_DEFINITION(", DoPRODUCT_DEFINITION);
            Doers.Add("SHAPE_REPRESENTATION(", DoSHAPE_REPRESENTATION);
            Doers.Add("(REPRESENTATION_RELATIONSHIP(", DoREPRESENTATION_RELATIONSHIP);
            Doers.Add("PRODUCT_DEFINITION_SHAPE(", DoPRODUCT_DEFINITION_SHAPE);
            Doers.Add("SHAPE_DEFINITION_REPRESENTATION(", DoSHAPE_DEFINITION_REPRESENTATION);
            Doers.Add("NEXT_ASSEMBLY_USAGE_OCCURRENCE(", DoNEXT_ASSEMBLY_USAGE_OCCURRENCE);
            Doers.Add("PROPERTY_DEFINITION(", DoPROPERTY_DEFINITION);
            Doers.Add("PROPERTY_DEFINITION_REPRESENTATION(", DoPROPERTY_DEFINITION_REPRESENTATION);
            Doers.Add("REPRESENTATION(", DoREPRESENTATION);
            Doers.Add("DESCRIPTIVE_REPRESENTATION_ITEM(", DoDESCRIPTIVE_REPRESENTATION_ITEM);

            TDSM_Types_Enabling.Add("", false);
            TDSM_Types_Enabling.Add("Access", true);
            TDSM_Types_Enabling.Add("Beam", false);
            TDSM_Types_Enabling.Add("Box", false);
            TDSM_Types_Enabling.Add("Bulkhead", false);
            TDSM_Types_Enabling.Add("Crossbeam", false);
            TDSM_Types_Enabling.Add("Door Beam", false);
            TDSM_Types_Enabling.Add("Door Frame Segment", false);
            TDSM_Types_Enabling.Add("Door Skin", true);
            TDSM_Types_Enabling.Add("Doubler", false);
            TDSM_Types_Enabling.Add("Engine", true);
            TDSM_Types_Enabling.Add("Fan Blade", true);
            TDSM_Types_Enabling.Add("Flap Track", false);
            TDSM_Types_Enabling.Add("Floor Panel", false);
            TDSM_Types_Enabling.Add("Frame", false);
            TDSM_Types_Enabling.Add("Framing", false);
            TDSM_Types_Enabling.Add("Inlet Cone", true);
            TDSM_Types_Enabling.Add("Keel Beam", false);
            TDSM_Types_Enabling.Add("Major Fitting", false);
            TDSM_Types_Enabling.Add("Nacelle Cowl", true);
            TDSM_Types_Enabling.Add("Panel", false);
            TDSM_Types_Enabling.Add("Plate", false);
            TDSM_Types_Enabling.Add("Radome", true);
            TDSM_Types_Enabling.Add("Rib", false);
            TDSM_Types_Enabling.Add("Riblet", false);
            TDSM_Types_Enabling.Add("Roller Track", false);
            TDSM_Types_Enabling.Add("Scuff Plate", false);
            TDSM_Types_Enabling.Add("Seat Track", false);
            TDSM_Types_Enabling.Add("Skin panel", true);
            TDSM_Types_Enabling.Add("Slat Can", false);
            TDSM_Types_Enabling.Add("Spar", true);
            TDSM_Types_Enabling.Add("Stringer", false);
            TDSM_Types_Enabling.Add("Strut", false);
            TDSM_Types_Enabling.Add("Web", false);
            TDSM_Types_Enabling.Add("Window Framing", true);
            TDSM_Types_Enabling.Add("Window", true);

            TDSM_Types_Color.Add("", White);
            TDSM_Types_Color.Add("Access", White);
            TDSM_Types_Color.Add("Beam", White);
            TDSM_Types_Color.Add("Box", White);
            TDSM_Types_Color.Add("Bulkhead", White);
            TDSM_Types_Color.Add("Crossbeam", White);
            TDSM_Types_Color.Add("Door Beam", White);
            TDSM_Types_Color.Add("Door Frame Segment", White);
            TDSM_Types_Color.Add("Door Skin", White);
            TDSM_Types_Color.Add("Doubler", White);
            TDSM_Types_Color.Add("Engine", White);
            TDSM_Types_Color.Add("Fan Blade", Black);
            TDSM_Types_Color.Add("Flap Track", White);
            TDSM_Types_Color.Add("Floor Panel", White);
            TDSM_Types_Color.Add("Frame", White);
            TDSM_Types_Color.Add("Framing", White);
            TDSM_Types_Color.Add("Inlet Cone", White);
            TDSM_Types_Color.Add("Keel Beam", White);
            TDSM_Types_Color.Add("Major Fitting", White);
            TDSM_Types_Color.Add("Nacelle Cowl", White);
            TDSM_Types_Color.Add("Panel", White);
            TDSM_Types_Color.Add("Plate", White);
            TDSM_Types_Color.Add("Radome", White);
            TDSM_Types_Color.Add("Rib", White);
            TDSM_Types_Color.Add("Riblet", White);
            TDSM_Types_Color.Add("Roller Track", White);
            TDSM_Types_Color.Add("Scuff Plate", White);
            TDSM_Types_Color.Add("Seat Track", White);
            TDSM_Types_Color.Add("Skin panel", White);
            TDSM_Types_Color.Add("Slat Can", White);
            TDSM_Types_Color.Add("Spar", White);
            TDSM_Types_Color.Add("Stringer", White);
            TDSM_Types_Color.Add("Strut", White);
            TDSM_Types_Color.Add("Web", White);
            TDSM_Types_Color.Add("Window", Black);
            TDSM_Types_Color.Add("Window Framing", Black);
            // foreach (string k in TDSM_Types_Enabling.Keys) TDSM_Types_Enabling[k] = !TDSM_Types_Enabling[k];
        }

        static Dictionary<string, bool> TDSM_Types_Enabling = new Dictionary<string, bool>();
        static Dictionary<string, Mtx4f> TDSM_Types_Color = new Dictionary<string, Mtx4f>();


        #endregion Public Constructors

        #region Private Delegates

        private delegate object DoVerb_CB(TSTEPElement elmt, string line);

        #endregion Private Delegates

        #region Public Methods

        public override void LaunchLoad()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            string[] Text = System.IO.File.ReadAllLines(Fi.FullName).Where(o => o.StartsWith("#")).ToArray();
            //
            string line;
            int j;
            int rank;
            string body;

            //
            for (int i = 0; i < Text.Length; i++)
            {
                line = Text[i].Substring(1);
                j = line.IndexOf("=");
                rank = int.Parse(line.Substring(0, j));
                body = line.Substring(j + 1);
                foreach (string k in Doers.Keys)
                {
                    if (body.StartsWith(k))
                    {
                        Objects.Add(rank, Doers[k].Invoke(this, body));
                        break;
                    }
                }
            }

            Text = null;

            TRelationShip[] relations = Objects.Values.OfType<TRelationShip>().ToArray();

            foreach (KeyValuePair<int, object> item in Objects.Where(o => o.Value is TShapeRepresentation))//   TShapeRepresentation shprep in Objects.Values.OfType<TShapeRepresentation>())
            {
                TShapeRepresentation shprep = (TShapeRepresentation)item.Value;
                shprep.Relations = relations.Where(o => o.Id2 == item.Key).ToArray();
                foreach (TRelationShip relship in shprep.Relations) relship.Parent = shprep;
                foreach (TRelationShip relship in relations.Where(o => o.Id1 == item.Key)) relship.Children = shprep;
            }

            relations = null;

            TReferenceRep mainrep = Objects.Values.OfType<TReferenceRep>().First();


            foreach (TPropertyDefinitionRepresentation pdr in Objects.Values.OfType<TPropertyDefinitionRepresentation>())
            {
                TPropertyDefinition pd = (TPropertyDefinition)Objects[pdr.Id1];
                switch (pd.Property)
                {
                    case EPropertyType.external_ref:
                        {
                            TShapeRepresentation shprep = (TShapeRepresentation)Objects[pdr.Id2];
                            if ((TDSM_Types_Enabling[shprep.ReferenceRep.TDSM_Type]))
                            {
                                shprep.Document = (TDocument)pd.Obj;

                                TFile file = new TFile(new FileInfo(Fi.Directory.FullName + "\\" + shprep.Document.RelativePath), null);
                                TBaseElement Elmt = file.Element;
                                Elmt.OwnerLink.ToBeReplaced = true;
                                Elmt.OwnerLink.NodeName = Fi.Name;
                                shprep.Document.File = file;
                            }
                            else shprep.Exclude = true;
                        }
                        break;
                    case EPropertyType.Description:
                        {
                            TRepresentation rep = (TRepresentation)Objects[pdr.Id2];
                            TDescriptiveRepresentationItem drep = (TDescriptiveRepresentationItem)Objects[rep.Id1];
                            TReferenceRep refrep = (TReferenceRep)pd.Obj;
                            if (refrep.Description == null) refrep.Description = drep.Value;
                        }
                        break;
                    case EPropertyType.TDSM_type:
                        {
                            TRepresentation rep = (TRepresentation)Objects[pdr.Id2];
                            TDescriptiveRepresentationItem drep = (TDescriptiveRepresentationItem)Objects[rep.Id1];
                            TReferenceRep refrep = (TReferenceRep)pd.Obj;
                            refrep.TDSM_Type = drep.Value;
                        }
                        break;
                    case EPropertyType.TDSM_description:
                        {
                            TRepresentation rep = (TRepresentation)Objects[pdr.Id2];
                            TDescriptiveRepresentationItem drep = (TDescriptiveRepresentationItem)Objects[rep.Id1];
                            TReferenceRep refrep = (TReferenceRep)pd.Obj;
                            refrep.Description = drep.Value;
                        }
                        break;
                }
            }

            List<string[]> moverslist = new List<string[]>();
            FileInfo fi = new FileInfo(Fi.FullName + ".k");
            if (fi.Exists)
            {
                foreach (string s in System.IO.File.ReadAllLines(fi.FullName))
                {
                    moverslist.Add(s.Split(';'));
                }
            }

            FileInfo custotree = new FileInfo(Fi.FullName + ".custotree");
            if (custotree.Exists)
            {
                TLink lk;
                TLinkMover mover;
                TRelationShip[] allrelshps = Objects.Values.OfType<TRelationShip>().ToArray();
                string[] lines = System.IO.File.ReadAllLines(custotree.FullName);
                TLink currentparent = this.OwnerLink;
                Regex rgx = new Regex(@"\A(\t*)(.*)\Z");
                int i = 0;
                Mtx4 absoluteMatrix;
                foreach (string l in lines)
                {

                    Match m = rgx.Match(l);
                    if (i > m.Groups[1].Value.Length) currentparent = currentparent.ParentLink;
                    i = m.Groups[1].Value.Length;
                    string shortpn = m.Groups[2].Value.Split('#')[0];
                    //
                    TRelationShip[] relshps = allrelshps.Where(o =>
                        shortpn == o.Children.ReferenceRep.PartNumber
                        &&
                        currentparent.PartNumber.StartsWith(o.Parent.ReferenceRep.PartNumber)
                    ).ToArray();
                    //
                    switch (relshps.Length)
                    {
                        case 0:
                            if (currentparent.ParentLink == null) currentparent.PartNumber = l;
                            else throw new Exception("not expected!");
                            break;
                        case 1:
                            TRelationShip relshp = relshps[0];
                            if (relshp.Children.Document == null)
                            {
                                lk = new TLink
                                {
                                    Child = new TAssemblyElement(),
                                    ParentLink = currentparent,
                                    Matrix = relshp.Matrix,
                                    NodeName = relshp.Children.ReferenceRep.Description,
                                    PartNumber = m.Groups[2].Value
                                };
                                currentparent = lk;
                            }
                            else
                            {
                                lk = new TLink
                                {
                                    Child = relshp.Children.Document.File.Element,
                                    ParentLink = currentparent,
                                    Matrix = relshp.Matrix,
                                    NodeName = relshp.Children.ReferenceRep.Description,
                                    PartNumber = m.Groups[2].Value,
                                    DisplayColor = TDSM_Types_Color[relshp.Children.ReferenceRep.TDSM_Type],
                                    ToBeReplaced = true,
                                };
                            }
                            mover = IncludeSpecificsMovers(relshp, Mtx4.Identity, lk);
                            if (mover == null)
                            {
                                string[] mv = moverslist.FirstOrDefault(o => o[2] == lk.ParentLink.PartNumber && o[3] == lk.PartNumber);
                                if (mv != null) mover = TLinkMover.CreateFromWords(this, mv);
                            }
                            if (mover != null)
                            {
                                lk.ActionToDo = mover.Do;
                                mover.Links.Add(lk);
                            }
                            break;
                        default: throw new Exception("not expected!");
                    }

                    //if (relshp.Children.Document == null)
                    //{
                    //    l = new TLink
                    //    {
                    //        Child = new TAssemblyElement(),
                    //        ParentLink = pl,
                    //        Matrix = relshp.Matrix,
                    //        NodeName = relshp.Children.ReferenceRep.Description,// Name.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                    //        PartNumber = relshp.Children.ReferenceRep.PartNumber//.Replace("partmd_icn-aa-a-000000-a-fape3-", "")
                    //    };
                    //    WriteTree(l, relshp.Children.ReferenceRep, l.Matrix * absoluteMatrix, moverslist, wtr, level + 1);
                    //}
                    //else
                    //{
                    //    l = new TLink
                    //    {
                    //        Child = relshp.Children.Document.File.Element,
                    //        ParentLink = pl,
                    //        Matrix = relshp.Matrix,
                    //        NodeName = relshp.Children.ReferenceRep.Description,// Name.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                    //        PartNumber = relshp.Children.ReferenceRep.PartNumber,//.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                    //        DisplayColor = TDSM_Types_Color[relshp.Children.ReferenceRep.TDSM_Type],
                    //        ToBeReplaced = true
                    //    };
                    //    for (int i = 0; i <= level; i++) wtr.Write('\t');
                    //    wtr.WriteLine(relshp.Children.ReferenceRep.PartNumber);
                    //}
                }
            }
            else
            {
                StreamWriter wtr = new StreamWriter(Fi.FullName + ".tree");

                WriteTree(this.OwnerLink, mainrep, Mtx4.Identity, moverslist, wtr, 0);

                wtr.Close(); wtr.Dispose();
            }

            //
            ElementLoader.Publish();

            foreach (object o in Objects.Values)
                if (o is IDisposable i) i.Dispose();

            Objects.Clear(); Objects = null;

            TLinkMover.SaveAll();
        }

        #endregion Public Methods

        #region Private Methods

        private static object DoAXIS2_PLACEMENT_3D(TSTEPElement elmt, string line)
        {
            Match m = RgxAXIS2_PLACEMENT_3D.Match(line);
            Mtx4 mtx = new Mtx4
            {
                //
                Row3 = (Vec4)elmt.Objects[int.Parse(m.Groups[2].Value)],
                Row2 = (Vec4)elmt.Objects[int.Parse(m.Groups[3].Value)],
                Row0 = (Vec4)elmt.Objects[int.Parse(m.Groups[4].Value)]
            };
            mtx.Row1 = Vec4.Cross(mtx.Row2, mtx.Row0);
            //
            return mtx;
        }

        private static object DoCARTESIAN_POINT(TSTEPElement elmt, string line)
        {
            Match m = RgxCARTESIAN_POINT.Match(line);
            return new Vec4(double.Parse(m.Groups[2].Value) * 0.001, double.Parse(m.Groups[3].Value) * 0.001, double.Parse(m.Groups[4].Value) * 0.001, 1);
        }

        private static object DoDESCRIPTIVE_REPRESENTATION_ITEM(TSTEPElement elmt, string line)
        {
            Match m = RgxDESCRIPTIVE_REPRESENTATION_ITEM.Match(line);
            TDescriptiveRepresentationItem rep = new TDescriptiveRepresentationItem
            {
                Tag = m.Groups[1].Value,
                Value = m.Groups[2].Value
            };
            return rep;
        }

        private static object DoDIRECTION(TSTEPElement elmt, string line)
        {
            Match m = RgxDIRECTION.Match(line);
            return new Vec4(double.Parse(m.Groups[2].Value), double.Parse(m.Groups[3].Value), double.Parse(m.Groups[4].Value), 0);
        }

        private static object DoDOCUMENT_FILE(TSTEPElement elmt, string line)
        {
            Match m = RgxDOCUMENT_FILE.Match(line);
            TDocument doc = new TDocument
            {
                RelativePath = m.Groups[1].Value
            };
            return doc;
        }

        private static object DoITEM_DEFINED_TRANSFORMATION(TSTEPElement elmt, string line)
        {
            Match m = RgxITEM_DEFINED_TRANSFORMATION.Match(line);
            return elmt.Objects[int.Parse(m.Groups[3].Value)];
        }

        private static object DoNEXT_ASSEMBLY_USAGE_OCCURRENCE(TSTEPElement elmt, string line)
        {
            Match m = RgxNEXT_ASSEMBLY_USAGE_OCCURRENCE.Match(line);
            TReferenceRep refrep = (TReferenceRep)elmt.Objects[int.Parse(m.Groups[4].Value)];
            return refrep;
        }

        private static object DoPRODUCT(TSTEPElement elmt, string line)
        {
            Match m = RgxPRODUCT.Match(line);
            TReferenceRep refrep = new TReferenceRep
            {
                PartNumber = m.Groups[1].Value
            };
            return refrep;
        }

        private static object DoPRODUCT_DEFINITION(TSTEPElement elmt, string line)
        {
            Match m = RgxPRODUCT_DEFINITION.Match(line);
            TReferenceRep refrep = (TReferenceRep)elmt.Objects[int.Parse(m.Groups[2].Value)];
            refrep.Definition = m.Groups[1].Value;
            return refrep;
        }

        private static object DoPRODUCT_DEFINITION_FORMATION(TSTEPElement elmt, string line)
        {
            Match m = RgxPRODUCT_DEFINITION_FORMATION.Match(line);
            TReferenceRep refrep = (TReferenceRep)elmt.Objects[int.Parse(m.Groups[2].Value)];
            refrep.Formation = m.Groups[1].Value;
            return refrep;
        }

        private static object DoPRODUCT_DEFINITION_SHAPE(TSTEPElement elmt, string line)
        {
            Match m = RgxPRODUCT_DEFINITION_SHAPE.Match(line);
            TReferenceRep refrep = (TReferenceRep)elmt.Objects[int.Parse(m.Groups[2].Value)];
            return refrep;
        }

        private static object DoPROPERTY_DEFINITION(TSTEPElement elmt, string line)
        {
            Match m = RgxPROPERTY_DEFINITION.Match(line);
            TPropertyDefinition p = new TPropertyDefinition
            {
                Obj = elmt.Objects[int.Parse(m.Groups[2].Value)],
                Property = (EPropertyType)Enum.Parse(typeof(EPropertyType), m.Groups[1].Value.Replace(" ", "_"))
            };
            return p;
        }

        private static object DoPROPERTY_DEFINITION_REPRESENTATION(TSTEPElement elmt, string line)
        {
            Match m = RgxPROPERTY_DEFINITION_REPRESENTATION.Match(line);
            TPropertyDefinitionRepresentation pdr = new TPropertyDefinitionRepresentation
            {
                Id1 = int.Parse(m.Groups[1].Value),
                Id2 = int.Parse(m.Groups[2].Value)
            };
            return pdr;
        }

        private static object DoREPRESENTATION(TSTEPElement elmt, string line)
        {
            Match m = RgxREPRESENTATION.Match(line);
            TRepresentation rep = new TRepresentation
            {
                Id1 = int.Parse(m.Groups[2].Value)
            };
            return rep;
        }

        private static object DoREPRESENTATION_RELATIONSHIP(TSTEPElement elmt, string line)
        {
            Match m = RgxREPRESENTATION_RELATIONSHIP.Match(line);
            TRelationShip relship = new TRelationShip
            {
                Id1 = int.Parse(m.Groups[2].Value),
                Id2 = int.Parse(m.Groups[3].Value),
                Matrix = (Mtx4)elmt.Objects[int.Parse(m.Groups[4].Value)]
            };
            return relship;
        }

        private static object DoSHAPE_DEFINITION_REPRESENTATION(TSTEPElement elmt, string line)
        {
            Match m = RgxSHAPE_DEFINITION_REPRESENTATION.Match(line);
            TReferenceRep refrep = (TReferenceRep)elmt.Objects[int.Parse(m.Groups[1].Value)];
            TShapeRepresentation shprep = (TShapeRepresentation)elmt.Objects[int.Parse(m.Groups[2].Value)];
            refrep.Representation = shprep;
            shprep.ReferenceRep = refrep;
            return refrep;
        }

        private static object DoSHAPE_REPRESENTATION(TSTEPElement elmt, string line)
        {
            Match m = RgxSHAPE_REPRESENTATION.Match(line);
            TShapeRepresentation shprep = new TShapeRepresentation
            {
                Matrixes = m.Groups[2].Value,
                Name = m.Groups[1].Value
            };
            return shprep;
        }

        private void WriteTree(TLink pl, TReferenceRep mainrep, Mtx4 absoluteMatrix, List<string[]> moverslist, StreamWriter wtr, int level)
        {
            TLink l;
            TLinkMover mover;

            for (int i = 0; i < level; i++) wtr.Write('\t');
            wtr.WriteLine(mainrep.PartNumber);

            foreach (TRelationShip relshp in mainrep.Representation.Relations.Where(o => !o.Children.Exclude))
            {
                if (relshp.Children.Document == null)
                {
                    l = new TLink
                    {
                        Child = new TAssemblyElement(),
                        ParentLink = pl,
                        Matrix = relshp.Matrix,
                        NodeName = relshp.Children.ReferenceRep.Description,// Name.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                        PartNumber = relshp.Children.ReferenceRep.PartNumber//.Replace("partmd_icn-aa-a-000000-a-fape3-", "")
                    };
                    WriteTree(l, relshp.Children.ReferenceRep, l.Matrix * absoluteMatrix, moverslist, wtr, level + 1);
                }
                else
                {
                    l = new TLink
                    {
                        Child = relshp.Children.Document.File.Element,
                        ParentLink = pl,
                        Matrix = relshp.Matrix,
                        NodeName = relshp.Children.ReferenceRep.Description,// Name.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                        PartNumber = relshp.Children.ReferenceRep.PartNumber,//.Replace("partmd_icn-aa-a-000000-a-fape3-", ""),
                        DisplayColor = TDSM_Types_Color[relshp.Children.ReferenceRep.TDSM_Type],
                        ToBeReplaced = true
                    };
                    for (int i = 0; i <= level; i++) wtr.Write('\t');
                    wtr.WriteLine(relshp.Children.ReferenceRep.PartNumber);
                }
                mover = IncludeSpecificsMovers(relshp, absoluteMatrix, l);
                if (mover == null)
                {
                    string[] mv = moverslist.FirstOrDefault(o => o[1] == l.ParentLink.PartNumber && o[2] == l.PartNumber);
                    if (mv != null)
                    {
                        switch (mv[0])
                        {
                            case "TLinkFlappingMover":
                                mover = new TLinkFlappingMover(this, mv);
                                break;
                            case "TLinkTiltMover":
                                mover = new TLinkTiltMover(this, mv);
                                break;
                            case "TLinkRotationMover":
                                mover = new TLinkRotationMover(this, mv);
                                break;
                            default:
                                mover = null;
                                break;
                        }

                    }
                }
                if (mover != null) l.ActionToDo = mover.Do;
            }

        }


        TLinkMover IncludeSpecificsMovers(TRelationShip relshp, Mtx4 absoluteMatrix, TLink l)
        {
            switch (relshp.Children.ReferenceRep.PartNumber)
            {

                //case "partmd_icn-aa-a-000000-a-fape3-s0011-a-001-01":
                //    return new TLinkRotationMover(this, l, new Vec3(28.38055, -9.36478, -2.58672), 0.5 * (new Vec3(33.10228, -9.36309, -1.80234) + new Vec3(33.10231, -9.36328, -3.37314)), 2, absoluteMatrix);
                //case "partmd_icn-aa-a-000000-a-fape3-s0014-a-001-01":
                //    return new TLinkRotationMover(this, l, new Vec3(28.38055, 9.36478, -2.58672), 0.5 * (new Vec3(33.10228, 9.36309, -1.80234) + new Vec3(33.10231, 9.36328, -3.37314)), -2, absoluteMatrix);
                //return new TLinkRotationMover(this, l, new Vec3(28.38055, -9.36478, -2.58672), new Vec3(33.096325, -9.298435, -2.58653), 0.0008, absoluteMatrix);
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-01-s1":
                //    return new TLinkTiltMover(this, l, new Vec3(38.37625, 5.45308, -0.78453), new Vec3(38.60775, 8.00978, -0.27309), 2000, -0.0008, absoluteMatrix)
                //    {
                //        Tempo2 = 2000
                //    };
                //case "partmd_icn-aa-a-000000-a-fape3-s0087-a-001-01_a-330-800-wngsp-l-skinp-02-s2":
                //case "partmd_icn-aa-a-000000-a-fape3-s0087-a-001-01_a-330-800-wngsp-l-skinp-03-s3":
                //case "partmd_icn-aa-a-000000-a-fape3-s0087-a-001-01_a-330-800-wngsp-l-skinp-04-s4":
                //case "partmd_icn-aa-a-000000-a-fape3-s0087-a-001-01_a-330-800-wngsp-l-skinp-05-s5":
                //case "partmd_icn-aa-a-000000-a-fape3-s0087-a-001-01_a-330-800-wngsp-l-skinp-06-s6":
                //    return new TLinkTiltMover(this, l, new Vec3(38.66479, -9.17193, -0.08566), new Vec3(42.66784, -19.38090, 0.80463), 2000, 0.0008, absoluteMatrix)
                //    {
                //        Tempo2 = 2000
                //    };
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-02-s2":
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-03-s3":
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-04-s4":
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-05-s5":
                //case "partmd_icn-aa-a-000000-a-fape3-s0089-a-001-01_a-330-800-wngsp-r-skinp-06-s6":
                //    return new TLinkTiltMover(this, l, new Vec3(38.66479, 9.17193, -0.08566), new Vec3(42.66784, 19.38090, 0.80463), 2000, -0.0008, absoluteMatrix)
                //    {
                //        Tempo2 = 2000
                //    };
                default:
                    break;
            }
            return null;
        }


        #endregion Private Methods

        #region Private Structs


        #endregion Private Structs
    }
}