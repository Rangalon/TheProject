
using Math3D;
using System;
using System.Globalization;
using System.Linq;

namespace CiliaElements.Format3DXml
{
    public class T3DRepElement : TSolidElement
    {
        #region Public Fields

        public T3DXmlElement Owner;

        public System.Xml.XmlTextReader Reader;

        public TReferenceRep referenceRep;

        public System.IO.Stream Stream;

        #endregion Public Fields

        #region Private Fields

        private static readonly char[] CloudSeparators = new char[] { ' ', ',' };

        private static Mtx4 StartMatrix = Mtx4.Scale(0.001, 0.001, 0.001);

        #endregion Private Fields

        #region Public Constructors

        public T3DRepElement()
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TFGroup Faces = null;
            int k;
            TCloud Positions;
            TTexture texture = null;
            Vec3 v = new Vec3();
            Vec2 v2 = new Vec2();
            int ElementsCount;
            int[] TblElements;
            TCloud Normals;
            TTextureCloud Textures;
            TLGroup edges = null;
            string[] TblValues;
            string iValues;
            int[] vs = new int[] { };
            string[] TblGroups;
            int n;
            int m2;
            int m1;
            int Depth;
            bool b5;
            bool b6;
            bool b7;
            bool b8;
            //
            if (Stream == null) return;
            Reader = new System.Xml.XmlTextReader(Stream)
            {
                WhitespaceHandling = System.Xml.WhitespaceHandling.Significant
            };
            bool bRepresentationInProgress = true;
            //
            while (Reader.Read())
            {
                switch (Reader.Name)
                {
                    case "Attr":
                        //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
                        break;

                    case "Feature":
                        //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
                        break;

                    case "Osm":
                        //while (Reader.Name != "Osm" || Reader.NodeType != System.Xml.XmlNodeType.EndElement) Reader.Read();
                        break;

                    case "xml":
                        break;

                    case "XMLRepresentation":
                        while (bRepresentationInProgress)
                        {
                            Reader.Read();
                            switch (Reader.Name)
                            {
                                case "Root":

                                    while (bRepresentationInProgress)
                                    {
                                        Reader.Read();
                                        switch (Reader.Name)
                                        {
                                            case "Rep":
                                                texture = null;
                                                //if (PartNumber.Contains("04_01"))
                                                //{
                                                //    bRepresentationInProgress = true;
                                                //}
                                                if (Reader.GetAttribute("xsi:type") == "PolygonalRepType")
                                                {
                                                    //
                                                    b5 = true;
                                                    Reader.Read();
                                                    while (b5)
                                                    {
                                                        switch (Reader.Name)
                                                        {
                                                            case "SurfaceAttributes":

                                                                b6 = true;
                                                                Reader.Read();
                                                                while (b6)
                                                                {
                                                                    switch (Reader.Name)
                                                                    {
                                                                        case "MaterialApplication":

                                                                            b7 = true;
                                                                            Reader.Read();
                                                                            while (b7)
                                                                            {
                                                                                switch (Reader.Name)
                                                                                {
                                                                                    case "MaterialId":
                                                                                        string id = Reader.GetAttribute("id").Split('#')[1];
                                                                                        texture = Owner.MaterialRefs[id];
                                                                                        Reader.Read();
                                                                                        break;
                                                                                    default:
                                                                                        b7 = false;
                                                                                        break;
                                                                                }
                                                                            }
                                                                            Reader.Read();
                                                                            break;
                                                                        default:
                                                                            b6 = false;
                                                                            break;
                                                                    }
                                                                }
                                                                Reader.Read();
                                                                break;
                                                            case "PolygonalLOD":

                                                                Depth = Reader.Depth;
                                                                Reader.Read();
                                                                while (Reader.Depth > Depth) Reader.Read();
                                                                Reader.Read();
                                                                break;

                                                            case "Faces":

                                                                Faces = SolidElementConstruction.AddFGroup();
                                                                SolidElementConstruction.StartGroup.FGroups.Push(Faces);
                                                                b6 = true;
                                                                Reader.Read();
                                                                while (b6)
                                                                {
                                                                    switch (Reader.Name)
                                                                    {
                                                                        case "Face":
                                                                            iValues = Reader.GetAttribute("triangles");
                                                                            if (iValues != null)
                                                                            {
                                                                                TblElements = Array.ConvertAll(iValues.TrimEnd().Split(' '), int.Parse);
                                                                                Faces.Indexes.Push(TblElements);
                                                                            }
                                                                            iValues = Reader.GetAttribute("strips");
                                                                            if (iValues != null)
                                                                            {
                                                                                TblGroups = iValues.Split(',');
                                                                                Array.Resize(ref vs, iValues.Length * 2 + 1);
                                                                                k = 0;
                                                                                Array.ForEach(TblGroups, s =>
                                                                                {
                                                                                    TblElements = Array.ConvertAll(s.Split(' '), int.Parse);
                                                                                    ElementsCount = TblElements.Length - 2;
                                                                                    if (ElementsCount == 1)
                                                                                    {
                                                                                        vs[k] = TblElements[0];
                                                                                        vs[k + 1] = TblElements[1];
                                                                                        vs[k + 2] = TblElements[2];
                                                                                        k += 3;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        vs[k] = TblElements[0];
                                                                                        m2 = TblElements[1];
                                                                                        vs[k + 1] = m2;
                                                                                        vs[k + 3] = m2;
                                                                                        for (int i = 2; i < ElementsCount; i++)
                                                                                        {
                                                                                            m2 = TblElements[i];
                                                                                            vs[k + 2] = m2;
                                                                                            vs[k + 4] = m2;
                                                                                            vs[k + 6] = m2;
                                                                                            k += 3;
                                                                                        }
                                                                                        m2 = TblElements[ElementsCount];
                                                                                        vs[k + 2] = m2;
                                                                                        vs[k + 4] = m2;
                                                                                        vs[k + 5] = TblElements[ElementsCount + 1];
                                                                                        k += 6;
                                                                                    }
                                                                                });
                                                                                //
                                                                                Array.Resize(ref vs, k);
                                                                                Faces.Indexes.Push(vs);
                                                                            }
                                                                            iValues = Reader.GetAttribute("fans");
                                                                            if (iValues != null)
                                                                            {
                                                                                TblGroups = iValues.Split(',');
                                                                                Array.Resize(ref vs, iValues.Length * 2 + 1);
                                                                                //vs = new Int32[iValues.Length * 2 + 1];
                                                                                k = 0;
                                                                                Array.ForEach(TblGroups, s =>
                                                                                {
                                                                                    TblElements = Array.ConvertAll(s.Split(' '), int.Parse);
                                                                                    m1 = TblElements[0];
                                                                                    vs[k + 1] = TblElements[1];
                                                                                    ElementsCount = TblElements.Length - 1;
                                                                                    for (int i = 2; i < ElementsCount; i++)
                                                                                    {
                                                                                        vs[k] = m1;
                                                                                        m2 = TblElements[i];
                                                                                        vs[k + 2] = m2;
                                                                                        vs[k + 4] = m2;
                                                                                        k += 3;
                                                                                    }
                                                                                    vs[k] = m1;
                                                                                    vs[k + 2] = TblElements[ElementsCount];
                                                                                    k += 3;
                                                                                });
                                                                                //
                                                                                Array.Resize(ref vs, k);
                                                                                Faces.Indexes.Push(vs);
                                                                            }
                                                                            //
                                                                            b7 = true;
                                                                            Reader.Read();
                                                                            while (b7)
                                                                            {
                                                                                switch (Reader.Name)
                                                                                {
                                                                                    case "SurfaceAttributes":
                                                                                        b8 = true;
                                                                                        Reader.Read();
                                                                                        while (b8)
                                                                                        {
                                                                                            switch (Reader.Name)
                                                                                            {
                                                                                                case "MaterialApplication":
                                                                                                    Depth = Reader.Depth;
                                                                                                    Reader.Read();
                                                                                                    while (Reader.Depth > Depth) Reader.Read();
                                                                                                    Reader.Read();
                                                                                                    break;

                                                                                                case "Color":
                                                                                                    if (texture == null)
                                                                                                        texture = SolidElementConstruction.AddTexture(
                                                                                                            float.Parse(Reader.GetAttribute("alpha"), CultureInfo.InvariantCulture),
                                                                                                            float.Parse(Reader.GetAttribute("red"), CultureInfo.InvariantCulture),
                                                                                                            float.Parse(Reader.GetAttribute("green"), CultureInfo.InvariantCulture),
                                                                                                            float.Parse(Reader.GetAttribute("blue"), CultureInfo.InvariantCulture));
                                                                                                    else
                                                                                                    {
                                                                                                        SolidElementConstruction.AddTexture(texture);
                                                                                                        texture.A = float.Parse(Reader.GetAttribute("alpha"), CultureInfo.InvariantCulture);
                                                                                                        texture.R = float.Parse(Reader.GetAttribute("red"), CultureInfo.InvariantCulture);
                                                                                                        texture.G = float.Parse(Reader.GetAttribute("green"), CultureInfo.InvariantCulture);
                                                                                                        texture.B = float.Parse(Reader.GetAttribute("blue"), CultureInfo.InvariantCulture);
                                                                                                    }
                                                                                                    Faces.GroupParameters.Texture = texture;
                                                                                                    Reader.Read();
                                                                                                    break;

                                                                                                default:
                                                                                                    b8 = false;
                                                                                                    break;
                                                                                            }
                                                                                        }
                                                                                        Reader.Read();
                                                                                        break;

                                                                                    default:
                                                                                        b7 = false;
                                                                                        break;
                                                                                }
                                                                            }
                                                                            Reader.Read();
                                                                            break;

                                                                        default:
                                                                            b6 = false;
                                                                            break;
                                                                    }
                                                                }
                                                                Reader.Read();
                                                                break;

                                                            case "Edges":
                                                                edges = SolidElementConstruction.AddLGroup();
                                                                switch (Owner.LS)
                                                                {
                                                                    case TManager.ELoadStuff.All:
                                                                    case TManager.ELoadStuff.EdgesOnly:
                                                                        SolidElementConstruction.StartGroup.LGroups.Push(edges);
                                                                        break;
                                                                }
                                                                Positions = SolidElementConstruction.AddCloud();
                                                                edges.ShapeGroupParameters.Positions = Positions;
                                                                k = 0;
                                                                b6 = true;
                                                                Reader.Read();
                                                                while (b6)
                                                                {
                                                                    switch (Reader.Name)
                                                                    {
                                                                        case "LineAttributes":
                                                                            b7 = true;
                                                                            Reader.Read();
                                                                            while (b7)
                                                                            {
                                                                                switch (Reader.Name)
                                                                                {
                                                                                    case "MaterialApplication":
                                                                                        Depth = Reader.Depth;
                                                                                        Reader.Read();
                                                                                        while (Reader.Depth > Depth) Reader.Read();
                                                                                        Reader.Read();
                                                                                        break;

                                                                                    case "Color":
                                                                                        texture = SolidElementConstruction.AddTexture(
                                                                                            float.Parse(Reader.GetAttribute("alpha"), CultureInfo.InvariantCulture),
                                                                                            float.Parse(Reader.GetAttribute("red"), CultureInfo.InvariantCulture),
                                                                                            float.Parse(Reader.GetAttribute("green"), CultureInfo.InvariantCulture),
                                                                                            float.Parse(Reader.GetAttribute("blue"), CultureInfo.InvariantCulture)
                                                                                            );
                                                                                        edges.GroupParameters.Texture = texture;
                                                                                        Reader.Read();
                                                                                        break;

                                                                                    default:
                                                                                        b7 = false;
                                                                                        break;
                                                                                }
                                                                            }
                                                                            Reader.Read();
                                                                            break;

                                                                        case "Polyline":
                                                                            TblGroups = Reader.GetAttribute("vertices").Split(',');
                                                                            n = edges.Indexes.Max;
                                                                            Array.ForEach(TblGroups, s =>
                                                                           {
                                                                               TblValues = s.Split(' ');// Array.ConvertAll(s.Split(' '), double.Parse);
                                                                               v.X = double.Parse(TblValues[0], CultureInfo.InvariantCulture);
                                                                               v.Y = double.Parse(TblValues[1], CultureInfo.InvariantCulture);
                                                                               v.Z = double.Parse(TblValues[2], CultureInfo.InvariantCulture);
                                                                               Positions.Vectors.Push(v);
                                                                               edges.Indexes.Push(k);
                                                                               k++;
                                                                               n++;
                                                                           });
                                                                            //
                                                                            edges.Indexes.Push(-1);
                                                                            Reader.Read();
                                                                            break;

                                                                        default:
                                                                            b6 = false;
                                                                            break;
                                                                    }
                                                                }
                                                                Reader.Read();
                                                                break;

                                                            case "VertexBuffer":
                                                                Positions = null;
                                                                Normals = null;
                                                                b6 = true;
                                                                Reader.Read();

                                                                while (b6)
                                                                {
                                                                    ElementsCount = 0;
                                                                    k = 0;

                                                                    switch (Reader.Name)
                                                                    {
                                                                        case "Positions":
                                                                            Positions = SolidElementConstruction.AddCloud();
                                                                            Faces.ShapeGroupParameters.Positions = Positions;
                                                                            Reader.Read();
                                                                            //
                                                                            TblValues = Reader.Value.Split(CloudSeparators);//Array.ConvertAll(Reader.Value.Split(CloudSeparators), double.Parse);
                                                                            ElementsCount = TblValues.Length;
                                                                            for (int i = 0; i < ElementsCount; i += 3)
                                                                            {
                                                                                v.X = double.Parse(TblValues[i], CultureInfo.InvariantCulture);
                                                                                v.Y = double.Parse(TblValues[i + 1], CultureInfo.InvariantCulture);
                                                                                v.Z = double.Parse(TblValues[i + 2], CultureInfo.InvariantCulture);
                                                                                //
                                                                                Positions.Vectors.Push(v);
                                                                                k++;
                                                                            }
                                                                            //
                                                                            Reader.Read();
                                                                            Reader.Read();
                                                                            break;

                                                                        case "Normals":
                                                                            Normals = SolidElementConstruction.AddCloud();
                                                                            Faces.ShapeGroupParameters.Normals = Normals;
                                                                            Reader.Read();
                                                                            //
                                                                            TblValues = Reader.Value.Split(CloudSeparators);// Array.ConvertAll(Reader.Value.Split(CloudSeparators), double.Parse);
                                                                            ElementsCount = TblValues.Length;
                                                                            for (int i = 0; i < ElementsCount; i += 3)
                                                                            {
                                                                                v.X = double.Parse(TblValues[i], CultureInfo.InvariantCulture);
                                                                                v.Y = double.Parse(TblValues[i + 1], CultureInfo.InvariantCulture);
                                                                                v.Z = double.Parse(TblValues[i + 2], CultureInfo.InvariantCulture);
                                                                                //
                                                                                Normals.Vectors.Push(v);
                                                                                k++;
                                                                            }
                                                                            //
                                                                            Reader.Read();
                                                                            Reader.Read();
                                                                            break;

                                                                        case "TextureCoordinates":
                                                                            Textures = SolidElementConstruction.AddTextureCloud();
                                                                            Faces.ShapeGroupParameters.Textures = Textures;
                                                                            if (Faces.GroupParameters.Texture.KDBitmap == null) Faces.GroupParameters.Texture.KDBitmap = new System.Drawing.Bitmap(1, 1);
                                                                            Reader.Read();
                                                                            //
                                                                            TblValues = Reader.Value.Split(CloudSeparators);// Array.ConvertAll(Reader.Value.Split(CloudSeparators), double.Parse);
                                                                            ElementsCount = TblValues.Length;
                                                                            for (int i = 0; i < ElementsCount; i += 2)
                                                                            {
                                                                                v2.X = double.Parse(TblValues[i], CultureInfo.InvariantCulture);
                                                                                v2.Y = double.Parse(TblValues[i + 1], CultureInfo.InvariantCulture);
                                                                                //
                                                                                Textures.Vectors.Push(v2);
                                                                                k++;
                                                                            }
                                                                            //
                                                                            Reader.Read();
                                                                            Reader.Read();
                                                                            break;

                                                                        default:
                                                                            b6 = false;
                                                                            break;
                                                                    }
                                                                }
                                                                Reader.Read();
                                                                break;

                                                            default:
                                                                b5 = false;
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;

                                            default:
                                                bRepresentationInProgress = false;
                                                break;
                                        }
                                    }
                                    //    SolidElementConstruction.StartGroup.AddGroupId(SolidElementConstruction.AddNGroup().GroupParameters.Id);
                                    break;
                            }
                        }
                        break;
                }
            }
            //
            //
            Faces = null;
            Positions = null;
            texture = null;
            TblElements = null;
            Normals = null;
            edges = null;
            TblValues = null;
            iValues = null;
            vs = null;
            TblGroups = null;
            //
            Reader.Close();
            Reader.Dispose();
            GC.SuppressFinalize(Reader); Reader = null;
            Stream.Close();
            Stream.Dispose();
            GC.SuppressFinalize(Stream); Stream = null;
            //
            GC.Collect();
            //  
            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;
            // 
            lock (Owner.Locker)
            {
                TInstanceRep ir = Owner.InstancesReps.Values.First(o => o.ChildId == referenceRep.Id);
                TReference3D r = Owner.References.Values.First(o => o.Id == ir.ParentId);
                PartNumber = r.PartNumber;
                CustomAttributes = r.Attributes;
                Owner.ReferencesReps.Remove(referenceRep.Id);
                Owner.References.Remove(r.Id);
                if (Owner.References.Count == 0) Owner.Clean();
            }
            //}
            referenceRep = null;
            Owner = null;
            //
            if (SolidElementConstruction.TextureClouds.Max > 0)
            {
                double
                    xmin = double.MaxValue, xmax = double.MinValue,
                    ymin = double.MaxValue, ymax = double.MinValue;
                TTextureCloud[] tcs = SolidElementConstruction.TextureClouds.Values.Where(o => o != null).ToArray();
                double ntc = tcs.Length; ntc = 1 / ntc;
                double ytc = 0;
                foreach (TTextureCloud tc in tcs)
                {
                    tc.Vectors.Close();
                    xmin = double.MaxValue; xmax = double.MinValue;
                    ymin = double.MaxValue; ymax = double.MinValue;
                    xmin = tc.Vectors.Values.Min(o => o.X);
                    ymin = tc.Vectors.Values.Min(o => o.Y);
                    xmax = tc.Vectors.Values.Max(o => o.X);
                    ymax = tc.Vectors.Values.Max(o => o.Y);
                    xmax -= xmin; xmax = 0.98 / xmax;
                    ymax -= ymin; ymax = (ntc - 0.02) / (ymax);// / (ymax * ntc);
                    for (int i = 0; i < tc.Vectors.Max; i++)
                    {
                        Vec2 vv = tc.Vectors.Values[i];
                        vv.X -= xmin; vv.Y -= ymin;
                        vv.X *= xmax; vv.Y *= ymax;
                        vv.X += 0.01; vv.Y += 0.01 + ytc;
                        tc.Vectors.Values[i] = vv;
                    }

                    xmin = double.MaxValue; xmax = double.MinValue;
                    ymin = double.MaxValue; ymax = double.MinValue;
                    xmin = tc.Vectors.Values.Min(o => o.X);
                    ymin = tc.Vectors.Values.Min(o => o.Y);
                    xmax = tc.Vectors.Values.Max(o => o.X);
                    ymax = tc.Vectors.Values.Max(o => o.Y);
                    ytc += ntc;
                }


            }
            //
            //if (PartNumber == "Corvette")
            //{
            //    foreach (TFGroup f in SolidElementConstruction.Groups.Values.OfType<TFGroup>())
            //    {
            //        if (f.GroupParameters.Texture.KDBitmap != null)
            //            f.GroupParameters.Texture.KDBitmap.Save("c:\\temp\\test.png");
            //    }
            //}

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}