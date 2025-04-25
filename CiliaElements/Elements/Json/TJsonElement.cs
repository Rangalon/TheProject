
using CiliaElements.Elements.Internals;
using Math3D;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CiliaElements.FormatJson
{
    public class TJsonElement : TAssemblyElement
    {
        #region Public Fields


        public object Locker = new object();

        #endregion Public Fields

        #region Private Fields


        private readonly Dictionary<string, TFile> files = new Dictionary<string, TFile>();


        #endregion Private Fields

        #region Public Constructors

        public TJsonElement()
        {
        }


        public TJBloc MainBloc = null;

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {

            BinaryReader ms = new BinaryReader(Fi.OpenRead());
            MainBloc = new TJBloc();
            TJBloc ParentBloc = null;
            TJBloc CurrentBloc = MainBloc;
            while (ms.BaseStream.Position < ms.BaseStream.Length)
            {
                char c = (char)ms.ReadByte();
                switch (c)
                {
                    case '{':
                        CurrentBloc.Blocs = new List<TJBloc>();
                        ParentBloc = CurrentBloc;
                        CurrentBloc = new TJBloc() { Parent = ParentBloc };
                        break;
                    case '[':
                        CurrentBloc.Blocs = new List<TJBloc>();
                        ParentBloc = CurrentBloc;
                        CurrentBloc = new TJBloc() { Parent = ParentBloc };
                        break;
                    case '}':
                        if (ParentBloc.Parent .Name != null &&  ParentBloc.Parent.Name.Contains("DMUObjects"))
                        {
                            FinalizeJson(ParentBloc);
                            TJBloc name = ParentBloc.FindRecursive("Name");
                            if (name == null || name.Value == null) continue;


                            TAnnotation annotation = new TAnnotation(name.Value.Replace("\"", ""));
                            annotation.AddJsonBloc(ParentBloc);
                            TLink link = new TLink
                            {
                                Child = annotation,
                                ParentLink = this.OwnerLink,
                                Matrix = Mtx4.Identity,
                                NodeName = name.Value,
                                PartNumber = name.Value,
                                ToBeReplaced = true
                            };




                            //
                            TFile f = new TFile(annotation);
                            //
                        }
                        CurrentBloc = ParentBloc; ParentBloc = ParentBloc.Parent;
                        break;
                    case ']':
                        //if (ParentBloc.Name!=null && ParentBloc.Name.Contains("DMUObjects"))
                        //{
                        //    TJBloc name = CurrentBloc.FindRecursive("Name");
                        //    if (name == null || name.Value == null) continue;


                        //    TAnnotation annotation = new TAnnotation(name.Value.Replace("\"", ""));
                        //    TLink link = new TLink();
                        //    link.Child = annotation;
                        //    link.ParentLink = this.OwnerLink;
                        //    link.Matrix = Mtx4.Identity;
                        //    link.NodeName = name.Value;
                        //    link.PartNumber = name.Value;
                        //    link.ToBeReplaced = true;
                        //    //
                        //    annotation.AddJsonBloc(CurrentBloc);
                        //    TFile f = new TFile(annotation);
                        //    //
                        //}
                        CurrentBloc = ParentBloc; ParentBloc = ParentBloc.Parent;
                        break;
                    case ',':
                        CurrentBloc = new TJBloc() { Parent = ParentBloc };
                        break;
                    case '\"':
                        CurrentBloc.Name += c;
                        c = (char)ms.ReadByte();
                        CurrentBloc.Name += c;
                        while (c != '\"')
                        {
                            c = (char)ms.ReadByte();
                            CurrentBloc.Name += c;
                        }
                        break;
                    case '\r':
                    case '\n':
                    case ' ':
                        break;
                    default:
                        CurrentBloc.Name += c;
                        break;
                }
            }
            FinalizeJson(MainBloc);
            ms.Close();
            ms.Dispose();

            //TJBloc DMUObjects = MainBloc.FindRecursive("DMUObjects");
            //foreach (TJBloc b in DMUObjects.Blocs)
            //{
            //    TJBloc name = b.FindRecursive("Name");
            //    if (name == null ||name.Value == null) continue;


            //    TAnnotation annotation = new TAnnotation(name.Value.Replace("\"",""));
            //    TLink link = new TLink();
            //    link.Child = annotation;
            //    link.ParentLink = this.OwnerLink;
            //    link.Matrix = Mtx4.Identity;
            //    link.NodeName = name.Value;
            //    link.PartNumber = name.Value;
            //    link.ToBeReplaced = true;
            //    //
            //    annotation.AddJsonBloc(b);
            //    TFile f = new TFile(annotation);
            //    //
            //    //foreach (TJBloc bb in b.Blocs)
            //    //{
            //    //    switch (bb.Name)
            //    //    {
            //    //        case "PropertiesMarkerBase":
            //    //            break;
            //    //    }
            //    //}
            //    //
            //    //TJBloc marker = b.FindRecursive("type");
            //    //switch (marker.Value)
            //    //{
            //    //    case "Marker3DPicture":

            //    //        break;
            //    //}


            //}
            //
            //if (Fi.Exists)
            //    z = new ICSharpCode.SharpZipLib.Zip.ZipFile(Fi.FullName);
            //else
            //    z = new ICSharpCode.SharpZipLib.Zip.ZipFile(bytes);

            //List<ZipEntry> reps = new List<ZipEntry>();
            //Stack<ZipEntry> xmls = new Stack<ZipEntry>();
            //for (Int32 i = 0; i < z.Count; i++)
            //{
            //    ICSharpCode.SharpZipLib.Zip.ZipEntry entry = z[i];
            //    int j = entry.Name.LastIndexOf('.');
            //    string n = entry.Name.Substring(0, j);
            //    string e = entry.Name.Substring(j + 1, entry.Name.Length - j - 1);
            //    switch (e)
            //    {
            //        case "CiliaK":
            //            CiliaKs.Push(entry);
            //            break;

            //        case "3dxml":
            //            switch (n)
            //            {
            //                case "CATMaterialRef":
            //                    break;

            //                default:
            //                    xmls.Push(entry);
            //                    break;
            //            }
            //            break;

            //        case "3DRep":
            //            reps.Add(entry);
            //            break;
            //    }
            //}
            ////
            //lock (Locker)
            //{
            //    reps.Sort(ZipEntryComparer);
            //    foreach (ZipEntry e in reps)
            //    {
            //        TFile f = new TFile(new System.IO.FileInfo(Fi.FullName + ".z\\" + e.Name), z.GetInputStream(e));
            //        files.Add(e.Name, f);
            //        ((T3DRepElement)f.Element).Owner = this;
            //    }
            //    reps.Clear();
            //    //
            //    while (xmls.Count > 0)
            //    {
            //        ICSharpCode.SharpZipLib.Zip.ZipEntry entry = xmls.Pop();
            //        System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
            //        System.IO.Stream st = z.GetInputStream(entry);
            //        xmldoc.Load(st);
            //        st.Close();
            //        st.Dispose();
            //        //
            //        Stack<System.Xml.XmlNode> Stc = new Stack<System.Xml.XmlNode>();
            //        //
            //        Int32 RootId = -1;
            //        Stc.Push(xmldoc.ChildNodes[1]);
            //        while (Stc.Count > 0)
            //        {
            //            System.Xml.XmlNode XmlElmt = Stc.Pop();
            //            switch (XmlElmt.Name)
            //            {
            //                case "Model_3dxml":
            //                    foreach (System.Xml.XmlNode xmlnode in XmlElmt.ChildNodes)
            //                        Stc.Push(xmlnode);
            //                    break;

            //                case "ProductStructure":
            //                    RootId = Int32.Parse(XmlElmt.Attributes.GetNamedItem("root").Value, CultureInfo.InvariantCulture);
            //                    foreach (System.Xml.XmlNode xmlnode in XmlElmt.ChildNodes)
            //                        Stc.Push(xmlnode);
            //                    break;

            //                case "Reference3D":
            //                    Format3DXml.TReference3D reference = new Format3DXml.TReference3D(XmlElmt);
            //                    References.Add(reference.Id, reference);
            //                    break;

            //                case "Instance3D":
            //                    Format3DXml.TInstance3D instance = new Format3DXml.TInstance3D(XmlElmt);
            //                    Instances.Add(instance.Id, instance);
            //                    break;

            //                case "ReferenceRep":
            //                    Format3DXml.TReferenceRep referenceRep = new Format3DXml.TReferenceRep(XmlElmt);
            //                    ReferencesReps.Add(referenceRep.Id, referenceRep);
            //                    referenceRep.File = files[referenceRep.FileName];
            //                    ((T3DRepElement)referenceRep.File.Element).referenceRep = referenceRep;
            //                    break;
            //                //
            //                case "DefaultView":
            //                case "Header":
            //                    break;

            //                case "InstanceRep":
            //                    Format3DXml.TInstanceRep instanceRep = new Format3DXml.TInstanceRep(XmlElmt);
            //                    InstancesReps.Add(instanceRep.Id, instanceRep);
            //                    break;

            //                default:
            //                    throw new Exception("Unexpected Element!");
            //            }
            //        }
            //        TQuickStc<TInstanceLink> stc = new TQuickStc<TInstanceLink>();
            //        stc.Push(new TInstanceLink() { Id = RootId, Link = OwnerLink });
            //        while (stc.NotEmpty)
            //        {
            //            TInstanceLink l = stc.Pop();
            //            foreach (Format3DXml.TInstance3D instance in Instances.Values.Where(o => o.ParentId == l.Id))
            //            {
            //                TLink link = new TLink();
            //                link.CustomAttributes = instance.Attributes;
            //                link.Child = new TAssemblyElement();
            //                link.ParentLink = l.Link;
            //                link.Matrix = instance.Matrix;
            //                link.NodeName = instance.NodeName;
            //                link.PartNumber = References.Values.First(o => o.Id == instance.ChildId).PartNumber;
            //                stc.Push(new TInstanceLink() { Id = instance.ChildId, Link = link });
            //                Format3DXml.TInstanceRep instanceRep = InstancesReps.Values.FirstOrDefault(o => o.ParentId == instance.ChildId);
            //                if (instanceRep != null)
            //                {
            //                    Format3DXml.TReferenceRep referenceRep = ReferencesReps.Values.First(o => o.Id == instanceRep.ChildId);
            //                    link.PartNumber = referenceRep.PartNumber;
            //                    link.Child = referenceRep.File.Element;
            //                    link.ToBeReplaced = true;
            //                }
            //            }
            //        }
            //        stc.Dispose();
            //        //AddLink(RootId, OwnerLink, References, Instances, ReferencesReps, InstancesReps);

            //        //
            //        break;
            //    }

            //    TReference3D r = References.Values.First(o => o.Id == 1);
            //    PartNumber = r.PartNumber;
            //    CustomAttributes = r.Attributes;
            //    References.Remove(r.Id);
            //    if (References.Count == 0) Clean();
            //}
            ////
            //
            ElementLoader.Publish();
            //

            //
        }

        private void FinalizeJson(TJBloc mb)
        {
            if (mb.Name != null)
            {
                string[] tbl = mb.Name.Split(':');
                if (tbl.Length > 1)
                {
                    mb.Name = tbl[0].Replace("\"", "");
                    mb.Value = tbl[1];
                    for (int i = 2; i < tbl.Length; i++) mb.Value += ":" + tbl[i];
                }
                else
                    mb.Name = mb.Name.Replace("\"", "");

            }
            if (mb.Blocs != null)
            {
                foreach (TJBloc b in mb.Blocs)
                    FinalizeJson(b);
            }
        }

        private string GetValueFromMS(BinaryReader ms, char BeginChar, char EndChar, char SecondEndChar)
        {
            string v = null;
            v += BeginChar;

            int n = 1;
            while (n > 0)
            {
                char cc = (char)ms.ReadByte();
                if (cc == EndChar) n--;
                else if (cc == SecondEndChar) n--;
                //else if (cc == BeginChar) n++;
                v += cc;
            }
            return v;
        }

        #endregion Public Methods

        #region Private Structs


        #endregion Private Structs
    }

    public class TJBloc
    {
        #region Public Fields

        public List<TJBloc> Blocs;
        public string Name = null;
        private TJBloc parent = null;
        public TJBloc Parent { get { return parent; } set { if (parent != null) parent.Blocs.Remove(this); parent = value; parent.Blocs.Add(this); } }

        public TJBloc FirstChild { get { return Blocs[0]; } }

        public string Value = null;

        #endregion Public Fields

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name.Trim('\"'), Value);
        }

        #endregion Public Methods

        #region Internal Methods

        internal TJBloc Clone(TJBloc CloneParent)
        {
            TJBloc clone = new TJBloc
            {
                Parent = CloneParent
            };
            CloneParent.Blocs.Add(clone);
            //
            clone.Name = this.Name;
            clone.Value = this.Value;
            if (Blocs != null)
            {
                clone.Blocs = new List<TJBloc>();
                foreach (TJBloc b in Blocs)
                    b.Clone(clone);
            }
            return clone;
        }

        internal TJBloc Find(string v)
        {
            return Blocs.FirstOrDefault(o => o.Name == v);
        }

        internal TJBloc FindRecursive(string v)
        {
            if (this.Blocs == null) return null;
            TJBloc b = Find(v);
            if (b != null) return b;
            //
            foreach (TJBloc bb in Blocs)
            {
                b = bb.FindRecursive(v);
                if (b != null) return b;
            }
            //
            return null;
        }

        #endregion Internal Methods
    }
}