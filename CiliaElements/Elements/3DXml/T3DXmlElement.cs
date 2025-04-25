
using ICSharpCode.SharpZipLib.Zip;
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
//using static System.Net.Mime.MediaTypeNames;

namespace CiliaElements.Format3DXml
{
    public class T3DXmlElement : TAssemblyElement
    {
        #region Public Fields

        public Dictionary<int, TInstance3D> Instances = new Dictionary<int, TInstance3D>();

        public Dictionary<int, TInstanceRep> InstancesReps = new Dictionary<int, TInstanceRep>();

        public object Locker = new object();
        public Dictionary<int, TReference3D> References = new Dictionary<int, TReference3D>();

        public TDico<int, TReferenceRep> ReferencesReps = new TDico<int, TReferenceRep>();

        #endregion Public Fields

        #region Private Fields

        private static readonly TZipEntryComparer ZipEntryComparer = new TZipEntryComparer();
        private byte[] bytes;

        private Dictionary<string, TFile> files = new Dictionary<string, TFile>();

        private ZipFile z;

        #endregion Private Fields

        #region Public Constructors

        public T3DXmlElement()
        {
        }

        static readonly Bitmap DefaultBitmap;
        static T3DXmlElement()
        {
            DefaultBitmap = new Bitmap(10, 10);
            Graphics grp = Graphics.FromImage(DefaultBitmap);
            grp.Clear(Color.Yellow);
            grp.Dispose();
        }

        public T3DXmlElement(string name, byte[] bts, TManager.ELoadStuff ls = TManager.ELoadStuff.All)
        {
            LS = ls;
            PartNumber = name + ".3dxml";
            bytes = bts;
            Fi = new FileInfo(PartNumber);
        }

        public TManager.ELoadStuff LS;

        public T3DXmlElement(string name, FileInfo fi)
        {
            PartNumber = name + ".3dxml";
            StreamReader rdr = new StreamReader(fi.FullName);
            BinaryReader brdr = new BinaryReader(rdr.BaseStream);
            bytes = brdr.ReadBytes((int)fi.Length);
            brdr.Close(); brdr.Dispose(); rdr.Dispose();
            Fi = new FileInfo(PartNumber);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Clean()
        {
            //Thread.Sleep(100);
            //while (ReferencesReps.Count > 0) Thread.Sleep(100);
            //while (files.Values.FirstOrDefault(o => o.Element.State < EElementState.Published) != null) Thread.Sleep(100);

            References.Clear();
            Instances.Clear();
            InstancesReps.Clear();


            z.Close();
            z = null;
            bytes = null;
        }



        public Dictionary<string, TTexture> MaterialRefs = new Dictionary<string, TTexture>();

        public override void LaunchLoad()
        {
            //
            if (Fi.Exists)
                z = new ZipFile(Fi.FullName);
            else
                z = new ZipFile(new MemoryStream(bytes));

            List<ZipEntry> reps = new List<ZipEntry>();
            List<ZipEntry> mats = new List<ZipEntry>();
            List<ZipEntry> imgs = new List<ZipEntry>();
            Stack<ZipEntry> xmls = new Stack<ZipEntry>();


            Dictionary<string, Bitmap> RepImages = new Dictionary<string, Bitmap>();

            for (int i = 0; i < z.Count; i++)
            {
                ZipEntry entry = z[i];
                int j = entry.Name.LastIndexOf('.');
                string n = entry.Name.Substring(0, j);
                string e = entry.Name.Substring(j + 1, entry.Name.Length - j - 1);
                switch (e)
                {


                    case "3dxml":
                        switch (n)
                        {
                            case "CATMaterialRef":
                                mats.Add(entry);
                                break;
                            case "CATRepImage":
                                imgs.Add(entry);
                                break;
                            default:
                                xmls.Push(entry);
                                break;
                        }
                        break;

                    case "3DRep":
                        if (!entry.Name.Contains("Rendering")) reps.Add(entry);
                        break;
                }
            }
            //
            foreach (ZipEntry e in imgs) DoImages(z, e, RepImages);
            foreach (ZipEntry e in mats) DoMaterials(z, e, MaterialRefs, RepImages);
            //
            lock (Locker)
            {
                reps.Sort(ZipEntryComparer);
                foreach (ZipEntry e in reps)
                {
                    TFile f = new TFile(new FileInfo(Fi.FullName + ".z\\" + e.Name), z.GetInputStream(e));
                    files.Add(e.Name, f);
                    ((T3DRepElement)f.Element).Owner = this;
                }
                reps.Clear();
                //
                while (xmls.Count > 0)
                {
                    ZipEntry entry = xmls.Pop();
                    System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                    Stream st = z.GetInputStream(entry);
                    xmldoc.Load(st);
                    st.Close();
                    st.Dispose();
                    //
                    Stack<System.Xml.XmlNode> Stc = new Stack<System.Xml.XmlNode>();
                    //
                    int RootId = -1;
                    Stc.Push(xmldoc.ChildNodes[1]);
                    while (Stc.Count > 0)
                    {
                        System.Xml.XmlNode XmlElmt = Stc.Pop();
                        switch (XmlElmt.Name)
                        {
                            case "Model_3dxml":
                                foreach (System.Xml.XmlNode xmlnode in XmlElmt.ChildNodes)
                                    Stc.Push(xmlnode);
                                break;

                            case "ProductStructure":
                                RootId = int.Parse(XmlElmt.Attributes.GetNamedItem("root").Value, CultureInfo.InvariantCulture);
                                foreach (System.Xml.XmlNode xmlnode in XmlElmt.ChildNodes)
                                    Stc.Push(xmlnode);
                                break;

                            case "Reference3D":
                                TReference3D reference = new TReference3D(XmlElmt);
                                References.Add(reference.Id, reference);
                                break;

                            case "Instance3D":
                                TInstance3D instance = new TInstance3D(XmlElmt);
                                Instances.Add(instance.Id, instance);
                                break;

                            case "ReferenceRep":
                                TReferenceRep referenceRep = new TReferenceRep(XmlElmt);
                                ReferencesReps.Add(referenceRep.Id, referenceRep);
                                referenceRep.File = files[referenceRep.FileName];
                                ((T3DRepElement)referenceRep.File.Element).referenceRep = referenceRep;
                                break;
                            //
                            case "DefaultView":
                            case "Header":
                                break;

                            case "InstanceRep":
                                TInstanceRep instanceRep = new TInstanceRep(XmlElmt);
                                InstancesReps.Add(instanceRep.Id, instanceRep);
                                break;

                            default:
                                throw new Exception("Unexpected Element!");
                        }
                    }
                    TQuickStc<TInstanceLink> stc = new TQuickStc<TInstanceLink>();
                    //if (ReferencesReps.Count == 1 && Instances.Count == 0)
                    //{
                    //    TInstance3D i = new TInstance3D();
                    //    i.NodeName = "Solid";
                    //    i.ParentId = RootId;
                    //    i.ChildId = 1;
                    //    i.Matrix = Mtx4.Identity;
                    //    Instances.Add(i.Id, i);
                    //}
                    stc.Push(new TInstanceLink() { Id = RootId, Link = OwnerLink });
                    while (stc.NotEmpty)
                    {
                        TInstanceLink l = stc.Pop();
                        foreach (TInstance3D instance in Instances.Values.Where(o => o.ParentId == l.Id))
                        {
                            TLink link = new TLink
                            {
                                CustomAttributes = instance.Attributes,
                                Child = new TAssemblyElement(),
                                ParentLink = l.Link,
                                Matrix = instance.Matrix,
                                NodeName = instance.NodeName,
                                PartNumber = References.Values.First(o => o.Id == instance.ChildId).PartNumber
                            };
                            stc.Push(new TInstanceLink() { Id = instance.ChildId, Link = link });
                            TInstanceRep instanceRep = InstancesReps.Values.FirstOrDefault(o => o.ParentId == instance.ChildId);
                            if (instanceRep != null)
                            {
                                TReferenceRep referenceRep = ReferencesReps.Values.First(o => o.Id == instanceRep.ChildId);
                                link.PartNumber = referenceRep.PartNumber;
                                link.Child = referenceRep.File.Element;
                                link.ToBeReplaced = true;
                            }
                        }
                    }
                    stc.Dispose();
                    //AddLink(RootId, OwnerLink, References, Instances, ReferencesReps, InstancesReps);

                    //
                    break;
                }

                TReference3D r = References.Values.First(o => o.Id == 1);
                PartNumber = r.PartNumber;
                CustomAttributes = r.Attributes;
                if (References.Count > 1) References.Remove(r.Id);
                else
                {
                    TLink link = new TLink
                    {
                        Child = new TAssemblyElement(),
                        ParentLink = OwnerLink,
                        Matrix = Mtx4.Identity,
                        NodeName = "Solid",
                        PartNumber = References.Values.First(o => o.Id == 1).PartNumber
                    };
                    TInstanceRep instanceRep = InstancesReps.Values.FirstOrDefault(o => o.ParentId == 1);
                    if (instanceRep != null)
                    {
                        TReferenceRep referenceRep = ReferencesReps.Values.First(o => o.Id == instanceRep.ChildId);
                        link.PartNumber = referenceRep.PartNumber;
                        link.Child = referenceRep.File.Element;
                        link.ToBeReplaced = true;
                    }
                }
                if (References.Count == 0) Clean();
            }
            //
            //
            ElementLoader.Publish();
            //

            //
        }

        private void DoMaterials(ZipFile z, ZipEntry e, Dictionary<string, TTexture> materialRefs, Dictionary<string, Bitmap> repImages)
        {
            Stream stream = z.GetInputStream(e);

            if (stream == null) return;

            System.Xml.XmlTextReader Reader = new System.Xml.XmlTextReader(stream)
            {
                WhitespaceHandling = System.Xml.WhitespaceHandling.Significant
            };

            bool b0, b1, b2, b3, b4;
            b0 = true;
            Reader.Read();
            while (b0)
            {
                switch (Reader.Name)
                {
                    case "xml":
                        Reader.Read();
                        break;
                    case "Model_3dxml":
                        b1 = true;
                        Reader.Read();
                        while (b1)
                        {
                            switch (Reader.Name)
                            {
                                case "Model_3dxml":
                                    b1 = false;
                                    Reader.Read();
                                    break;
                                case "Header":
                                    b2 = true;
                                    Reader.Read();
                                    while (b2)
                                    {
                                        switch (Reader.Name)
                                        {
                                            case "Header":
                                                b2 = false;
                                                Reader.Read();
                                                break;
                                            default:
                                                Reader.Read();
                                                break;
                                        }
                                    }
                                    break;
                                case "CATMaterialRef":
                                    b2 = true;
                                    Reader.Read();
                                    Dictionary<string, Bitmap> matdoms = new Dictionary<string, Bitmap>();

                                    while (b2)
                                    {
                                        switch (Reader.Name)
                                        {
                                            case "CATMaterialRef":
                                                b2 = false;
                                                Reader.Read();
                                                break;
                                            case "CATMatReference":
                                                b3 = true;
                                                Reader.Read();
                                                while (b3)
                                                {
                                                    switch (Reader.Name)
                                                    {
                                                        case "CATMatReference":
                                                            b3 = false;
                                                            Reader.Read();
                                                            break;
                                                        default:
                                                            Reader.Read();
                                                            break;
                                                    }
                                                }
                                                break;
                                            case "MaterialDomain":
                                                string matdom = Reader.GetAttribute("id");
                                                b3 = true;
                                                Reader.Read();
                                                while (b3)
                                                {
                                                    switch (Reader.Name)
                                                    {
                                                        case "MaterialDomain":
                                                            b3 = false;
                                                            Reader.Read();
                                                            break;
                                                        case "PLM_ExternalID":
                                                            Reader.Read();
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "V_MatDomain":
                                                            Reader.Read();
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "PLMRelation":
                                                            b4 = true;
                                                            Reader.Read();
                                                            while (b4)
                                                            {
                                                                switch (Reader.Name)
                                                                {
                                                                    case "PLMRelation":
                                                                        b4 = false;
                                                                        Reader.Read();
                                                                        break;
                                                                    case "C_Semantics":
                                                                    case "C_Role":
                                                                        Reader.Read();
                                                                        Reader.Read();
                                                                        Reader.Read();
                                                                        break;
                                                                    case "Ids":
                                                                        Reader.Read();
                                                                        Reader.Read();
                                                                        string imgid = Reader.Value.Split('#')[1];
                                                                        Bitmap img = repImages[imgid];
                                                                        matdoms.Add(matdom, img);
                                                                        Reader.Read();
                                                                        Reader.Read();
                                                                        Reader.Read();
                                                                        break;
                                                                    default:
                                                                        Reader.Read();
                                                                        break;
                                                                }
                                                            }
                                                            break;
                                                        default:
                                                            Reader.Read();
                                                            break;
                                                    }
                                                }
                                                break;
                                            case "MaterialDomainInstance":
                                                b3 = true;
                                                Reader.Read();
                                                string aggby = "";
                                                string aggto = "";
                                                while (b3)
                                                {
                                                    switch (Reader.Name)
                                                    {
                                                        case "MaterialDomainInstance":
                                                            b3 = false;
                                                            Reader.Read();
                                                            break;
                                                        case "PLM_ExternalID":
                                                            Reader.Read();
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "IsAggregatedBy":
                                                            Reader.Read();
                                                            aggby = Reader.Value;
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "IsInstanceOf":
                                                            Reader.Read();
                                                            aggto = Reader.Value;
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        default:
                                                            Reader.Read();
                                                            break;
                                                    }
                                                }
                                                TTexture t = new TTexture();
                                                if (matdoms.ContainsKey(aggto))
                                                    t.KDBitmap = (Bitmap)matdoms[aggto].Clone();
                                                else
                                                    try
                                                    {
                                                        t.KDBitmap = (Bitmap)DefaultBitmap.Clone();
                                                    }
                                                    catch { }
                                                materialRefs.Add(aggby, t);
                                                break;
                                            default:
                                                throw new Exception("not expected");
                                        }
                                    }
                                    break;
                                default:
                                    throw new Exception("not expected");
                            }
                        }
                        break;
                    default:
                        if (!Reader.EOF)
                            throw new Exception("not expected");
                        b0 = false;
                        break;
                }
            }

            Reader.Dispose();
            stream.Dispose();
        }

        private void DoImages(ZipFile z, ZipEntry e, Dictionary<string, Bitmap> repImages)
        {
            Stream stream = z.GetInputStream(e);

            if (stream == null) return;

            System.Xml.XmlTextReader Reader = new System.Xml.XmlTextReader(stream)
            {
                WhitespaceHandling = System.Xml.WhitespaceHandling.Significant
            };

            bool b0, b1, b2, b3 ;
            b0 = true;
            Reader.Read();
            while (b0)
            {
                switch (Reader.Name)
                {
                    case "xml":
                        Reader.Read();
                        break;
                    case "Model_3dxml":
                        b1 = true;
                        Reader.Read();
                        while (b1)
                        {
                            switch (Reader.Name)
                            {
                                case "Model_3dxml":
                                    b1 = false;
                                    Reader.Read();
                                    break;
                                case "Header":
                                    b2 = true;
                                    Reader.Read();
                                    while (b2)
                                    {
                                        switch (Reader.Name)
                                        {
                                            case "Header":
                                                b2 = false;
                                                Reader.Read();
                                                break;
                                            default:
                                                Reader.Read();
                                                break;
                                        }
                                    }
                                    break;
                                case "CATRepImage":
                                    b2 = true;
                                    Reader.Read();
                                    while (b2)
                                    {
                                        switch (Reader.Name)
                                        {
                                            case "CATRepImage":
                                                b2 = false;
                                                Reader.Read();
                                                break;
                                            case "CATRepresentationImage":
                                                b3 = true;
                                                string id = Reader.GetAttribute("id");
                                                Reader.Read();
                                                while (b3)
                                                {
                                                    switch (Reader.Name)
                                                    {
                                                        case "CATRepresentationImage":
                                                            b3 = false;
                                                            Reader.Read();
                                                            break;
                                                        case "PLM_ExternalID":
                                                            Reader.Read();
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "V_PrimaryMimeType":
                                                            Reader.Read();
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        case "V_PrimaryFileName":
                                                            Reader.Read();
                                                            string n = Reader.Value;
                                                            ZipEntry ie = z.GetEntry(n);
                                                            Bitmap img = new Bitmap(z.GetInputStream(ie));
                                                            repImages.Add(id, img);
                                                            Reader.Read();
                                                            Reader.Read();
                                                            break;
                                                        default:
                                                            Reader.Read();
                                                            break;
                                                    }
                                                }
                                                break;
                                            default:
                                                Reader.Read();
                                                break;
                                        }
                                    }
                                    break;
                                default:
                                    throw new Exception("not expected");
                            }
                        }
                        break;
                    default:
                        if (!Reader.EOF)
                            throw new Exception("not expected");
                        b0 = false;
                        break;
                }
            }


            Reader.Dispose();
            stream.Dispose();


        }



        #endregion Public Methods

        #region Private Structs

        private struct TInstanceLink
        {
            #region Public Fields

            public int Id;
            public TLink Link;

            #endregion Public Fields
        }

        #endregion Private Structs
    }

    public class TZipEntryComparer : IComparer<ZipEntry>
    {
        #region Public Methods

        public int Compare(ZipEntry x, ZipEntry y)
        {
            return (y.Size.CompareTo(x.Size));
        }

        #endregion Public Methods
    }
}