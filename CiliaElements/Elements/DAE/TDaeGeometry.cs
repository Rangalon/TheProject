
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CiliaElements.FormatDAE
{
    public class TDaeGeometry : TSolidElement
    {
        #region Public Fields

        public XmlReader rdr;

        public TDaeElement Owner;

        public Mtx4 StartMatrix = Mtx4.CreateScale(1);

        #endregion Public Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            TCloud c;
            Dictionary<string, TCloud> clds = new Dictionary<string, TCloud>();
            rdr.Read();
            Vec4f clr;
            int po = -1;
            int no = -1;
            int to = -1;
            string[] v = { };
            string[] p = { };
            TCloud pc = null;
            TCloud nc = null;
            int rk = 0;
            while (rdr.NodeType != XmlNodeType.EndElement)
            {
                switch (rdr.Name)
                {
                    case "mesh":
                        rdr.Read();
                        while (rdr.NodeType != XmlNodeType.EndElement)
                        {
                            switch (rdr.Name)
                            {
                                case "source":
                                    c = SolidElementConstruction.AddCloud();
                                    clds.Add("#" + rdr.GetAttribute("id"), c);
                                    rdr.Read();
                                    string[] s = null;
                                    int stp = 0;
                                    int nb = 0;
                                    while (rdr.NodeType != XmlNodeType.EndElement)
                                    {
                                        switch (rdr.Name)
                                        {
                                            case "float_array":
                                                nb = int.Parse(rdr.GetAttribute("count"));
                                                rdr.Read();
                                                if (nb > 0)
                                                {
                                                    s = rdr.Value.Trim().Split();
                                                    rdr.Read();
                                                    rdr.Read();
                                                }
                                                break;

                                            case "technique_common":
                                                rdr.Read();
                                                while (rdr.NodeType != XmlNodeType.EndElement)
                                                {
                                                    switch (rdr.Name)
                                                    {
                                                        case "accessor":
                                                            stp = int.Parse(rdr.GetAttribute("stride"));
                                                            DoGarbage();
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

                                    switch (stp)
                                    {
                                        case 2:
                                            for (int i = 0; i < nb; i += 2)
                                                c.Vectors.Push(new Vec3(double.Parse(s[i], CultureInfo.InvariantCulture), double.Parse(s[i + 1], CultureInfo.InvariantCulture), 0));
                                            break;

                                        case 3:
                                            for (int i = 0; i < nb; i += 3)
                                                c.Vectors.Push(new Vec3(double.Parse(s[i], CultureInfo.InvariantCulture), double.Parse(s[i + 1], CultureInfo.InvariantCulture), double.Parse(s[i + 2], CultureInfo.InvariantCulture)));
                                            break;
                                    }
                                    rdr.Read();
                                    break;

                                case "vertices":
                                    string id = rdr.GetAttribute("id");
                                    rdr.Read();
                                    c = clds[rdr.GetAttribute("source")];
                                    clds.Add("#" + id, c);
                                    rdr.Read();
                                    rdr.Read();
                                    break;

                                case "polylist":

                                    if (rdr.GetAttribute("material") != null)
                                        clr = Owner.colors[rdr.GetAttribute("material")];
                                    else
                                        clr = new Vec4f(1, 0, 0, 1);
                                    po = -1;
                                    no = -1;
                                    to = -1;
                                    pc = null;
                                    nc = null;
                                    v = null;
                                    p = null;
                                    rdr.Read();
                                    while (rdr.NodeType != XmlNodeType.EndElement)
                                    {
                                        switch (rdr.Name)
                                        {
                                            case "input":
                                                switch (rdr.GetAttribute("semantic"))
                                                {
                                                    case "VERTEX":
                                                        pc = clds[rdr.GetAttribute("source")];
                                                        po = int.Parse(rdr.GetAttribute("offset"));
                                                        break;

                                                    case "NORMAL":
                                                        nc = clds[rdr.GetAttribute("source")];
                                                        no = int.Parse(rdr.GetAttribute("offset"));
                                                        break;

                                                    case "TEXCOORD":
                                                        to = int.Parse(rdr.GetAttribute("offset"));
                                                        break;
                                                }

                                                rdr.Read();
                                                break;

                                            case "vcount":
                                                rdr.Read();
                                                v = rdr.Value.Trim().Split();
                                                rdr.Read();
                                                rdr.Read();
                                                break;

                                            case "p":
                                                rdr.Read();
                                                p = rdr.Value.Trim().Split();
                                                rdr.Read();
                                                rdr.Read();
                                                break;

                                            default:
                                                DoGarbage();
                                                break;
                                        }
                                    }
                                    TFGroup fg = SolidElementConstruction.AddFGroup();

                                    fg.GroupParameters.Texture = SolidElementConstruction.AddTexture(clr.W, clr.X, clr.Y, clr.Z);
                                    fg.ShapeGroupParameters.Positions = SolidElementConstruction.AddCloud();
                                    fg.ShapeGroupParameters.Normals = SolidElementConstruction.AddCloud();
                                    rk = 0;
                                    stp = Math.Max(Math.Max(to, no), po) + 1;
                                    while (rk < p.Length)
                                    {
                                        fg.Indexes.Push(fg.ShapeGroupParameters.Positions.Vectors.Max);
                                        if (po >= 0) { fg.ShapeGroupParameters.Positions.Vectors.Push(pc.Vectors.Values[int.Parse(p[rk + po])]); }
                                        if (no >= 0) { fg.ShapeGroupParameters.Normals.Vectors.Push(nc.Vectors.Values[int.Parse(p[rk + no])]); }
                                        rk += stp;
                                    }
                                    SolidElementConstruction.StartGroup.FGroups.Push(fg);
                                    rdr.Read();
                                    break;

                                case "lines":
                                    if (rdr.GetAttribute("material") != null)
                                        clr = Owner.colors[rdr.GetAttribute("material")];
                                    else
                                        clr = new Vec4f(1, 0, 0, 1);
                                    po = -1;
                                    no = -1;
                                    to = -1;
                                    pc = null;
                                    nc = null;
                                    v = null;
                                    p = null;
                                    rdr.Read();
                                    while (rdr.NodeType != XmlNodeType.EndElement)
                                    {
                                        switch (rdr.Name)
                                        {
                                            case "input":
                                                switch (rdr.GetAttribute("semantic"))
                                                {
                                                    case "VERTEX":
                                                        pc = clds[rdr.GetAttribute("source")];
                                                        po = int.Parse(rdr.GetAttribute("offset"));
                                                        break;

                                                    case "NORMAL":
                                                        nc = clds[rdr.GetAttribute("source")];
                                                        no = int.Parse(rdr.GetAttribute("offset"));
                                                        break;

                                                    case "TEXCOORD":
                                                        to = int.Parse(rdr.GetAttribute("offset"));
                                                        break;
                                                }

                                                rdr.Read();
                                                break;

                                            case "vcount":
                                                rdr.Read();
                                                v = rdr.Value.Trim().Split();
                                                rdr.Read();
                                                rdr.Read();
                                                break;

                                            case "p":
                                                rdr.Read();
                                                p = rdr.Value.Trim().Split();
                                                rdr.Read();
                                                rdr.Read();
                                                break;

                                            default:
                                                DoGarbage();
                                                break;
                                        }
                                    }
                                    TLGroup lg = SolidElementConstruction.AddLGroup();

                                    lg.GroupParameters.Texture = SolidElementConstruction.AddTexture(clr.W, clr.X, clr.Y, clr.Z);
                                    lg.ShapeGroupParameters.Positions = SolidElementConstruction.AddCloud();
                                    rk = 0;
                                    stp = Math.Max(Math.Max(to, no), po) + 1;
                                    while (rk < p.Length)
                                    {
                                        lg.Indexes.Push(lg.ShapeGroupParameters.Positions.Vectors.Max);
                                        if (po >= 0) { lg.ShapeGroupParameters.Positions.Vectors.Push(pc.Vectors.Values[int.Parse(p[rk + po])]); }
                                        rk += stp;
                                    }
                                    SolidElementConstruction.StartGroup.LGroups.Push(lg);
                                    rdr.Read();
                                    break;

                                default:
                                    DoGarbage();
                                    break;
                            }
                        }
                        break;

                    default:
                        DoGarbage();
                        break;
                }
            }

            //

            //
            //while (Reader.Read())
            //{
            //    switch (Reader.Name)
            //    {
            //        case "Attr":
            //            //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
            //            break;

            //        case "Feature":
            //            //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
            //            break;

            //        case "Osm":
            //            //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
            //            break;

            //        case "xml":
            //            break;

            //        case "XMLRepresentation":
            //            while (bRepresentationInProgress)
            //            {
            //                Reader.Read();
            //                switch (Reader.Name)
            //                {
            //                    case "Root":

            //                        while (bRepresentationInProgress)
            //                        {
            //                            Reader.Read();
            //                            switch (Reader.Name)
            //                            {
            //                                case "Rep":
            //                                    if (Reader.GetAttribute("xsi:type") == "PolygonalRepType")
            //                                    {
            //                                        //
            //                                        b5 = true;
            //                                        Reader.Read();
            //                                        while (b5)
            //                                        {
            //                                            switch (Reader.Name)
            //                                            {
            //                                                case "PolygonalLOD":
            //                                                    Depth = Reader.Depth;
            //                                                    Reader.Read();
            //                                                    while (Reader.Depth > Depth) Reader.Read();
            //                                                    Reader.Read();
            //                                                    break;
            //                                                case "Faces":
            //                                                    Faces = SolidElementConstruction.AddFGroup();
            //                                                    SolidElementConstruction.StartGroup.FGroups.Push(Faces);
            //                                                    b6 = true;
            //                                                    Reader.Read();
            //                                                    while (b6)
            //                                                    {
            //                                                        switch (Reader.Name)
            //                                                        {
            //                                                            case "Face":
            //                                                                iValues = Reader.GetAttribute("triangles");
            //                                                                if (iValues != null)
            //                                                                {
            //                                                                    TblElements = Array.ConvertAll(iValues.TrimEnd().Split(' '), Int32.Parse);
            //                                                                    Faces.Indexes.Push(TblElements);
            //                                                                }
            //                                                                iValues = Reader.GetAttribute("strips");
            //                                                                if (iValues != null)
            //                                                                {
            //                                                                    TblGroups = iValues.Split(',');
            //                                                                    Array.Resize(ref vs, iValues.Length * 2 + 1);
            //                                                                    k = 0;
            //                                                                    Array.ForEach(TblGroups, s =>
            //                                                                    {
            //                                                                        TblElements = Array.ConvertAll(s.Split(' '), Int32.Parse);
            //                                                                        ElementsCount = TblElements.Length - 2;
            //                                                                        if (ElementsCount == 1)
            //                                                                        {
            //                                                                            vs[k] = TblElements[0];
            //                                                                            vs[k + 1] = TblElements[1];
            //                                                                            vs[k + 2] = TblElements[2];
            //                                                                            k += 3;
            //                                                                        }
            //                                                                        else
            //                                                                        {
            //                                                                            vs[k] = TblElements[0];
            //                                                                            m2 = TblElements[1];
            //                                                                            vs[k + 1] = m2;
            //                                                                            vs[k + 3] = m2;
            //                                                                            for (int i = 2; i < ElementsCount; i++)
            //                                                                            {
            //                                                                                m2 = TblElements[i];
            //                                                                                vs[k + 2] = m2;
            //                                                                                vs[k + 4] = m2;
            //                                                                                vs[k + 6] = m2;
            //                                                                                k += 3;
            //                                                                            }
            //                                                                            m2 = TblElements[ElementsCount];
            //                                                                            vs[k + 2] = m2;
            //                                                                            vs[k + 4] = m2;
            //                                                                            vs[k + 5] = TblElements[ElementsCount + 1];
            //                                                                            k += 6;
            //                                                                        }
            //                                                                    });
            //                                                                    //
            //                                                                    Array.Resize(ref vs, k);
            //                                                                    Faces.Indexes.Push(vs);
            //                                                                }
            //                                                                iValues = Reader.GetAttribute("fans");
            //                                                                if (iValues != null)
            //                                                                {
            //                                                                    TblGroups = iValues.Split(',');
            //                                                                    Array.Resize(ref vs, iValues.Length * 2 + 1);
            //                                                                    //vs = new Int32[iValues.Length * 2 + 1];
            //                                                                    k = 0;
            //                                                                    Array.ForEach(TblGroups, s =>
            //                                                                    {
            //                                                                        TblElements = Array.ConvertAll(s.Split(' '), Int32.Parse);
            //                                                                        m1 = TblElements[0];
            //                                                                        vs[k + 1] = TblElements[1];
            //                                                                        ElementsCount = TblElements.Length - 1;
            //                                                                        for (int i = 2; i < ElementsCount; i++)
            //                                                                        {
            //                                                                            vs[k] = m1;
            //                                                                            m2 = TblElements[i];
            //                                                                            vs[k + 2] = m2;
            //                                                                            vs[k + 4] = m2;
            //                                                                            k += 3;
            //                                                                        }
            //                                                                        vs[k] = m1;
            //                                                                        vs[k + 2] = TblElements[ElementsCount];
            //                                                                        k += 3;
            //                                                                    });
            //                                                                    //
            //                                                                    Array.Resize(ref vs, k);
            //                                                                    Faces.Indexes.Push(vs);
            //                                                                }
            //                                                                //
            //                                                                b7 = true;
            //                                                                Reader.Read();
            //                                                                while (b7)
            //                                                                {
            //                                                                    switch (Reader.Name)
            //                                                                    {
            //                                                                        case "SurfaceAttributes":
            //                                                                            b8 = true;
            //                                                                            Reader.Read();
            //                                                                            while (b8)
            //                                                                            {
            //                                                                                switch (Reader.Name)
            //                                                                                {
            //                                                                                    case "MaterialApplication":
            //                                                                                        Depth = Reader.Depth;
            //                                                                                        Reader.Read();
            //                                                                                        while (Reader.Depth > Depth) Reader.Read();
            //                                                                                        Reader.Read();
            //                                                                                        break;
            //                                                                                    case "Color":
            //                                                                                        texture = SolidElementConstruction.AddTexture();
            //                                                                                        Faces.GroupParameters.Texture = texture;
            //                                                                                        texture.R = float.Parse(Reader.GetAttribute("red"), CultureInfo.InvariantCulture);
            //                                                                                        texture.G = float.Parse(Reader.GetAttribute("green"), CultureInfo.InvariantCulture);
            //                                                                                        texture.B = float.Parse(Reader.GetAttribute("blue"), CultureInfo.InvariantCulture);
            //                                                                                        texture.A = float.Parse(Reader.GetAttribute("alpha"), CultureInfo.InvariantCulture);
            //                                                                                        Reader.Read();
            //                                                                                        break;
            //                                                                                    default:
            //                                                                                        b8 = false;
            //                                                                                        break;
            //                                                                                }
            //                                                                            }
            //                                                                            Reader.Read();
            //                                                                            break;
            //                                                                        default:
            //                                                                            b7 = false;
            //                                                                            break;
            //                                                                    }
            //                                                                }
            //                                                                Reader.Read();
            //                                                                break;
            //                                                            default:
            //                                                                b6 = false;
            //                                                                break;
            //                                                        }
            //                                                    }
            //                                                    Reader.Read();
            //                                                    break;

            //                                                case "Edges":
            //                                                    edges = SolidElementConstruction.AddLGroup();
            //                                                    SolidElementConstruction.StartGroup.LGroups.Push(edges);
            //                                                    Positions = SolidElementConstruction.AddCloud();
            //                                                    edges.ShapeGroupParameters.Positions = Positions;
            //                                                    k = 0;
            //                                                    b6 = true;
            //                                                    Reader.Read();
            //                                                    while (b6)
            //                                                    {
            //                                                        switch (Reader.Name)
            //                                                        {
            //                                                            case "LineAttributes":
            //                                                                b7 = true;
            //                                                                Reader.Read();
            //                                                                while (b7)
            //                                                                {
            //                                                                    switch (Reader.Name)
            //                                                                    {
            //                                                                        case "MaterialApplication":
            //                                                                            Depth = Reader.Depth;
            //                                                                            Reader.Read();
            //                                                                            while (Reader.Depth > Depth) Reader.Read();
            //                                                                            Reader.Read();
            //                                                                            break;
            //                                                                        case "Color":
            //                                                                            texture = SolidElementConstruction.AddTexture();
            //                                                                            edges.GroupParameters.Texture = texture;
            //                                                                            texture.R = float.Parse(Reader.GetAttribute("red"), CultureInfo.InvariantCulture);
            //                                                                            texture.G = float.Parse(Reader.GetAttribute("green"), CultureInfo.InvariantCulture);
            //                                                                            texture.B = float.Parse(Reader.GetAttribute("blue"), CultureInfo.InvariantCulture);
            //                                                                            texture.A = float.Parse(Reader.GetAttribute("alpha"), CultureInfo.InvariantCulture);
            //                                                                            Reader.Read();
            //                                                                            break;
            //                                                                        default:
            //                                                                            b7 = false;
            //                                                                            break;
            //                                                                    }
            //                                                                }
            //                                                                Reader.Read();
            //                                                                break;
            //                                                            case "Polyline":
            //                                                                TblGroups = Reader.GetAttribute("vertices").Split(',');
            //                                                                n = edges.Indexes.Max;
            //                                                                Array.ForEach(TblGroups, s =>
            //                                                                {
            //                                                                    TblValues = s.Split(' ');// Array.ConvertAll(s.Split(' '), double.Parse);
            //                                                                    v.X = double.Parse(TblValues[0], CultureInfo.InvariantCulture);
            //                                                                    v.Y = double.Parse(TblValues[1], CultureInfo.InvariantCulture);
            //                                                                    v.Z = double.Parse(TblValues[2], CultureInfo.InvariantCulture);
            //                                                                    Positions.Vectors.Push(v);
            //                                                                    edges.Indexes.Push(k);
            //                                                                    k++;
            //                                                                    n++;
            //                                                                });
            //                                                                //
            //                                                                edges.Indexes.Push(-1);
            //                                                                Reader.Read();
            //                                                                break;
            //                                                            default:
            //                                                                b6 = false;
            //                                                                break;
            //                                                        }
            //                                                    }
            //                                                    Reader.Read();
            //                                                    break;
            //                                                case "VertexBuffer":
            //                                                    Positions = null;
            //                                                    Normals = null;
            //                                                    b6 = true;
            //                                                    Reader.Read();
            //                                                    while (b6)
            //                                                    {
            //                                                        ElementsCount = 0;
            //                                                        k = 0;
            //                                                        switch (Reader.Name)
            //                                                        {
            //                                                            case "Positions":
            //                                                                Positions = SolidElementConstruction.AddCloud();
            //                                                                Faces.ShapeGroupParameters.Positions = Positions;
            //                                                                Reader.Read();
            //                                                                //
            //                                                                TblValues = Reader.Value.Split(CloudSeparators);//Array.ConvertAll(Reader.Value.Split(CloudSeparators), double.Parse);
            //                                                                ElementsCount = TblValues.Length;
            //                                                                for (int i = 0; i < ElementsCount; i += 3)
            //                                                                {
            //                                                                    v.X = double.Parse(TblValues[i], CultureInfo.InvariantCulture);
            //                                                                    v.Y = double.Parse(TblValues[i + 1], CultureInfo.InvariantCulture);
            //                                                                    v.Z = double.Parse(TblValues[i + 2], CultureInfo.InvariantCulture);
            //                                                                    //
            //                                                                    Positions.Vectors.Push(v);
            //                                                                    k++;
            //                                                                }
            //                                                                //
            //                                                                Reader.Read();
            //                                                                Reader.Read();
            //                                                                break;
            //                                                            case "Normals":
            //                                                                Normals = SolidElementConstruction.AddCloud();
            //                                                                Faces.ShapeGroupParameters.Normals = Normals;
            //                                                                Reader.Read();
            //                                                                //
            //                                                                TblValues = Reader.Value.Split(CloudSeparators);// Array.ConvertAll(Reader.Value.Split(CloudSeparators), double.Parse);
            //                                                                ElementsCount = TblValues.Length;
            //                                                                for (int i = 0; i < ElementsCount; i += 3)
            //                                                                {
            //                                                                    v.X = double.Parse(TblValues[i], CultureInfo.InvariantCulture);
            //                                                                    v.Y = double.Parse(TblValues[i + 1], CultureInfo.InvariantCulture);
            //                                                                    v.Z = double.Parse(TblValues[i + 2], CultureInfo.InvariantCulture);
            //                                                                    //
            //                                                                    Normals.Vectors.Push(v);
            //                                                                    k++;
            //                                                                }
            //                                                                //
            //                                                                Reader.Read();
            //                                                                Reader.Read();
            //                                                                break;
            //                                                            default:
            //                                                                b6 = false;
            //                                                                break;
            //                                                        }
            //                                                    }
            //                                                    Reader.Read();
            //                                                    break;
            //                                                default:
            //                                                    b5 = false;
            //                                                    break;
            //                                            }
            //                                        }
            //                                    }
            //                                    break;
            //                                default:
            //                                    bRepresentationInProgress = false;
            //                                    break;
            //                            }
            //                        }
            //                        //    SolidElementConstruction.StartGroup.AddGroupId(SolidElementConstruction.AddNGroup().GroupParameters.Id);
            //                        break;

            //                }
            //            }
            //            break;

            //    }
            //}
            //
            //Faces = null;
            //Positions = null;
            //texture = null;
            //TblElements = null;
            //Normals = null;
            //edges = null;
            //TblValues = null;
            //iValues = null;
            //vs = null;
            //TblGroups = null;
            //
            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;
            //
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
            if (rdr.NodeType == XmlNodeType.EndElement) rdr.Read();
        }

        #endregion Private Methods
    }
}