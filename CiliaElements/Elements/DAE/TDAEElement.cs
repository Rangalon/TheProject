
using Math3D;
using ICSharpCode.SharpZipLib.Zip;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace CiliaElements.FormatDAE
{
    public class TDaeElement : TAssemblyElement
    {
        #region Public Fields

        public Dictionary<string, Vec4f> colors = new Dictionary<string, Vec4f>();
        public ZipFile zf;

        #endregion Public Fields

        #region Private Fields

        private XmlReader rdr;

        private Mtx4 StartMatrix;

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            //

            Stream str = Fi.OpenRead();
            rdr = XmlReader.Create(str, new XmlReaderSettings() { IgnoreWhitespace = true });

            while (rdr.Name != "COLLADA") rdr.Read();

            TQuickStc<TBaseElement> Elmts = new TQuickStc<TBaseElement>();

            rdr.Read();
            while (rdr.NodeType != XmlNodeType.EndElement)
            {
                switch (rdr.Name)
                {
                    case "asset":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "unit":
                                    switch (rdr.GetAttribute("name"))
                                    {
                                        case "meter":
                                            StartMatrix = Mtx4.CreateScale(1);
                                            break;
                                    }
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    case "library_effects":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "effect":
                                    string id = "#" + rdr.GetAttribute("id");
                                    rdr.Read();
                                    while (rdr.NodeType != XmlNodeType.EndElement)
                                    {
                                        switch (rdr.Name)
                                        {
                                            case "profile_COMMON":
                                                rdr.Read();
                                                while (rdr.NodeType != XmlNodeType.EndElement)
                                                {
                                                    switch (rdr.Name)
                                                    {
                                                        case "technique":
                                                            rdr.Read();
                                                            while (rdr.NodeType != XmlNodeType.EndElement)
                                                            {
                                                                switch (rdr.Name)
                                                                {
                                                                    case "lambert":
                                                                    case "phong":
                                                                        rdr.Read();
                                                                        while (rdr.NodeType != XmlNodeType.EndElement)
                                                                        {
                                                                            switch (rdr.Name)
                                                                            {
                                                                                case "diffuse":
                                                                                    rdr.Read();
                                                                                    rdr.Read();
                                                                                    string[] s = rdr.Value.Trim().Split();
                                                                                    colors.Add(id, new Vec4f(float.Parse(s[0], CultureInfo.InvariantCulture), float.Parse(s[1], CultureInfo.InvariantCulture), float.Parse(s[2], CultureInfo.InvariantCulture), float.Parse(s[3], CultureInfo.InvariantCulture)));
                                                                                    rdr.Read();
                                                                                    rdr.Read();
                                                                                    rdr.Read();
                                                                                    break;

                                                                                default:
                                                                                    DoGarbage();
                                                                                    break;
                                                                            }
                                                                        }
                                                                        rdr.Read();
                                                                        break;

                                                                    default:
                                                                        DoGarbage();
                                                                        break;
                                                                }
                                                            }
                                                            rdr.Read();
                                                            break;

                                                        default:
                                                            DoGarbage();
                                                            break;
                                                    }
                                                }
                                                rdr.Read();
                                                break;

                                            default:
                                                DoGarbage();
                                                break;
                                        }
                                    }
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    case "library_materials":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "material":
                                    string id = rdr.GetAttribute("id");
                                    rdr.Read();
                                    Vec4f v = colors[rdr.GetAttribute("url")];
                                    rdr.Read();
                                    rdr.Read();
                                    colors.Add(id, v);
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    case "library_geometries":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "geometry":
                                    TDaeGeometry g = new TDaeGeometry() { PartNumber = "#" + rdr.GetAttribute("id"), Owner = this, StartMatrix = this.StartMatrix, rdr = this.rdr, Fi = new FileInfo(this.Fi.FullName + "\\" + rdr.GetAttribute("id") + ".daegeometry") };
                                    TFile f = new TFile(g);
                                    g.LaunchLoad();
                                    Elmts.Push(g);
                                    rdr.Read();
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    case "library_visual_scenes":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "visual_scene":
                                    TAssemblyElement ass = new TAssemblyElement() { PartNumber = "#" + rdr.GetAttribute("id"), Fi = new FileInfo(this.Fi.FullName + "\\" + rdr.GetAttribute("id") + ".daescene") };
                                    Elmts.Push(ass);
                                    rdr.Read();
                                    while (rdr.NodeType != XmlNodeType.EndElement)
                                    {
                                        switch (rdr.Name)
                                        {
                                            case "node":
                                                TLink link = new TLink
                                                {
                                                    NodeName = rdr.GetAttribute("id"),
                                                    Matrix = Mtx4.Identity
                                                };
                                                rdr.Read();
                                                while (rdr.NodeType != XmlNodeType.EndElement)
                                                {
                                                    switch (rdr.Name)
                                                    {
                                                        case "matrix":
                                                            rdr.Read();
                                                            string[] s = rdr.Value.Trim().Split();
                                                            Mtx4 M = new Mtx4
                                                            {
                                                                Row0 = new Vec4(double.Parse(s[0], CultureInfo.InvariantCulture), double.Parse(s[4], CultureInfo.InvariantCulture), double.Parse(s[8], CultureInfo.InvariantCulture), double.Parse(s[12], CultureInfo.InvariantCulture)),
                                                                Row1 = new Vec4(double.Parse(s[1], CultureInfo.InvariantCulture), double.Parse(s[5], CultureInfo.InvariantCulture), double.Parse(s[9], CultureInfo.InvariantCulture), double.Parse(s[13], CultureInfo.InvariantCulture)),
                                                                Row2 = new Vec4(double.Parse(s[2], CultureInfo.InvariantCulture), double.Parse(s[6], CultureInfo.InvariantCulture), double.Parse(s[10], CultureInfo.InvariantCulture), double.Parse(s[14], CultureInfo.InvariantCulture)),
                                                                Row3 = new Vec4(double.Parse(s[3], CultureInfo.InvariantCulture), double.Parse(s[7], CultureInfo.InvariantCulture), double.Parse(s[11], CultureInfo.InvariantCulture), double.Parse(s[15], CultureInfo.InvariantCulture))
                                                            };
                                                            link.Matrix = M;
                                                            rdr.Read();
                                                            rdr.Read();
                                                            break;

                                                        case "instance_geometry":
                                                            link.Child = Elmts.Values.FirstOrDefault(o => o != null && o.PartNumber == rdr.GetAttribute("url")); ;
                                                            DoGarbage();
                                                            break;

                                                        default:
                                                            DoGarbage();
                                                            break;
                                                    }
                                                }
                                                if (link.Child != null)
                                                {
                                                    link.ParentLink = ass.OwnerLink;
                                                    link.PartNumber = link.Child.PartNumber;
                                                }
                                                rdr.Read();
                                                break;

                                            default:
                                                DoGarbage();
                                                break;
                                        }
                                    }
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    case "scene":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "instance_visual_scene":

                                    TLink link = TManager.AttachElmt(Elmts.Values.FirstOrDefault(o => o != null && o.PartNumber == rdr.GetAttribute("url")).OwnerLink, this.OwnerLink, null);
                                    link.Matrix = Mtx4.Identity;
                                    link.NodeName = link.Child.PartNumber;
                                    link.PartNumber = link.Child.PartNumber;
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        rdr.Read();
                        break;

                    default:
                        DoGarbage();
                        break;
                }
                //while(rdr.NodeType==XmlNodeType.EndElement )
            }

            rdr.Close();
            rdr.Dispose();
            str.Close();
            str.Dispose();

            //SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;

            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private void DoGarbage()
        {
            Console.WriteLine("Garbage: " + rdr.Name);
            int d = rdr.Depth;
            rdr.Read();
            while (rdr.Depth > d) rdr.Read();
            if (rdr.NodeType == XmlNodeType.EndElement && rdr.Depth == d) rdr.Read();
        }

        private StreamReader GetStream(string FileName)
        {
            if (zf != null)
            {
                FileName = FileName.Substring(FileName.LastIndexOf('\\') + 1);
                ZipEntry ze = zf.GetEntry(FileName);
                return new StreamReader(zf.GetInputStream(ze));
            }
            else
                return new StreamReader(FileName);
        }

        #endregion Private Methods
    }
}