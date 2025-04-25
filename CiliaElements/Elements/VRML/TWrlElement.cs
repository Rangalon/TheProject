
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CiliaElements.FormatWRL
{
    public class TWrlElement : TSolidElement
    {
        #region Public Fields

        public Vec4f BackGroundColor;
        public string Info = "";

        #endregion Public Fields

        #region Private Fields

        private List<string> NavigationTypes = new List<string>();
        private Stack<string> stc;

        #endregion Private Fields

        #region Public Constructors

        public TWrlElement()
            : base()
        {
            //iPartNumber, iParentLink, iNodeName, iMatrix, ifixed) ', iMatrix)
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            //Pgr = FrmProgress.Add("Reading WRL " + PartNumber, Color.LightGray)
            //Pgr.SetMinimum(1)

            if (Fi.Exists)
            {
                System.IO.StreamReader rdr = new System.IO.StreamReader(Fi.FullName);
                string Text = rdr.ReadToEnd().Replace((char)10, ' ').Replace(",", " ").Replace("}", "} ").Replace("\"", " \" ").Replace("[", " [ ").Replace("]", " ] ");
                rdr.Dispose();
                //
                string[] table = Text.Split(new char[4] { (char)13, (char)10, ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                //
                //Pgr = FrmProgress.Add("Parsing WRL " + PartNumber, Color.LightGray)
                Array.Reverse(table);
                stc = new Stack<string>(table);
                //Pgr.SetMinimum(stc.Count)
                ParseMain();
                //Pgr.Remove()
                //
            }
            else
            {
                //Pgr.SetMinimum(1)
                //Pgr.Incremente()
                //Pgr.Remove()
            }
            //
            //Save(Fi.Directory)
            ElementLoader.Publish();
        }

        public void ParseAppearance(TGroup iParentGroup)
        {
            string LastWord = null;
            string pendingDef = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "material":
                    case "Material":
                    case "texture":
                    case "ImageTexture":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Material":
                                TTexture texture = SolidElementConstruction.AddTexture(ref pendingDef);
                                iParentGroup.GroupParameters.Texture = texture;
                                ParseMaterial(texture);
                                break;

                            case "ImageTexture":
                                stc.Pop();
                                stc.Pop();
                                ParseText();
                                stc.Pop();
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    case "USE":
                        iParentGroup.GroupParameters.Texture = ((TTexture)SolidElementConstruction.Defs[stc.Pop()]);
                        break;

                    case "DEF":
                        pendingDef = stc.Pop();
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseBackground()
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "skyColor":
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "skyColor":
                                TTexture texture = new TTexture();
                                ParseColor(texture);
                                stc.Pop();
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseChildren(TNGroup iParentGroup)
        {
            string LastWord = null;
            string PendingDef = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "Group":
                    case "Shape":
                    case "Transform":
                    case "Billboard":
                    case "TimeSensor":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Group":
                            case "Transform":
                            case "Billboard":
                                TNGroup group = SolidElementConstruction.AddNGroup(ref PendingDef);
                                group.GroupParameters.Texture = iParentGroup.GroupParameters.Texture;
                                iParentGroup.NGroups.Push(group);
                                //group.Name += " " + LastWord
                                ParseTransform(@group);
                                break;

                            case "Shape":
                                ParseShape(iParentGroup);
                                break;

                            case "TimeSensor":
                                stc.Pop();
                                stc.Pop();
                                stc.Pop();
                                stc.Pop();
                                stc.Pop();
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "]":
                        break;

                    case "USE":
                        string def = stc.Pop();
                        TGroup g = (TGroup)SolidElementConstruction.Defs[def];
                        if (g is TNGroup) iParentGroup.NGroups.Push(g as TNGroup);
                        else if (g is TFGroup) iParentGroup.FGroups.Push(g as TFGroup);
                        else if (g is TLGroup) iParentGroup.LGroups.Push(g as TLGroup);
                        else if (g is TPGroup) iParentGroup.PGroups.Push(g as TPGroup);
                        break;

                    case "DEF":
                        PendingDef = stc.Pop();
                        break;
                    //ParseDef()
                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "]") break;
            }
        }

        public void ParseColor(TTexture iTexture)
        {
            iTexture.R = float.Parse(stc.Pop(), CultureInfo.InvariantCulture);
            iTexture.G = float.Parse(stc.Pop(), CultureInfo.InvariantCulture);
            iTexture.B = float.Parse(stc.Pop(), CultureInfo.InvariantCulture);
            iTexture.A = 1;
            //ParseColor.red = CSng(stc.Pop()) * 255
            //ParseColor.green = CSng(stc.Pop()) * 255
            //ParseColor.blue = CSng(stc.Pop()) * 255
            //'ParseColor.red = 0 'Rnd() * 128 + 127
            //'ParseColor.green = 0 'Rnd() * 128 + 127
            //'ParseColor.blue = 255 ' Rnd() * 128 + 127
            //LastColor = New Vector4d(ParseColor.red / 255, ParseColor.green / 255, ParseColor.blue / 255, 1)
        }

        public void ParseFaceSet(TNGroup iParentGroup)
        {
            int decalage = DataPositions.Length;
            string LastWord = null;
            string PendingDef = "";
            TFGroup group = SolidElementConstruction.AddFGroup();
            group.GroupParameters.Texture = iParentGroup.GroupParameters.Texture;
            //
            iParentGroup.FGroups.Push(group);
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "Coordinate":
                    case "coord":
                    case "normal":
                    case "Normal":
                    case "coordIndex":
                    case "texCoord":
                    case "TextureCoordinate":
                    case "texCoordIndex":
                        break;

                    case "solid":
                    case "ccw":
                        stc.Pop();
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Coordinate":
                                TCloud cloud1 = SolidElementConstruction.AddCloud(ref PendingDef);
                                @group.ShapeGroupParameters.Positions = cloud1;
                                ParsePointCoordinate(cloud1);
                                break;

                            case "Normal":
                                TCloud cloud2 = SolidElementConstruction.AddCloud(ref PendingDef);
                                @group.ShapeGroupParameters.Normals = cloud2;
                                ParseNormalCoordinate(cloud2);
                                break;

                            case "TextureCoordinate":
                                TCloud cloud3 = new TCloud();
                                //AddCloud(cloud, PendingDef)
                                PendingDef = "";
                                //group.PositionsId = cloud.Id
                                ParseTextureCoordinate(cloud3);
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "coordIndex":
                                int n = @group.Indexes.Max;
                                while (stc.Count > 0)
                                {
                                    word = stc.Pop();
                                    switch (word)
                                    {
                                        case "]":
                                            break;

                                        default:
                                            //Array.Resize(ref @group.Indexes, n + 3);
                                            int i1 = Convert.ToInt32(word, CultureInfo.InvariantCulture);
                                            int i2 = Convert.ToInt32(stc.Pop(), CultureInfo.InvariantCulture);
                                            int i3 = Convert.ToInt32(stc.Pop(), CultureInfo.InvariantCulture);
                                            @group.Indexes.Push(i1);
                                            @group.Indexes.Push(i2);
                                            @group.Indexes.Push(i3);
                                            n += 3;
                                            stc.Pop();
                                            // le -1
                                            break;
                                    }
                                    if (word == "]") break;
                                }
                                break;

                            case "texCoordIndex":
                                while (stc.Count > 0)
                                {
                                    word = stc.Pop();
                                    switch (word)
                                    {
                                        case "]":
                                            break;

                                        default:
                                            int i1 = Convert.ToInt32(word, CultureInfo.InvariantCulture);
                                            int i2 = Convert.ToInt32(stc.Pop(), CultureInfo.InvariantCulture);
                                            int i3 = Convert.ToInt32(stc.Pop(), CultureInfo.InvariantCulture);
                                            stc.Pop();
                                            // le -1
                                            break;
                                    }
                                    if (word == "]") break;
                                }
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    case "USE":
                        string def = stc.Pop();
                        if (def.StartsWith("_coord", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Positions = ((TCloud)SolidElementConstruction.Defs[def]);
                        else if (def.StartsWith("_normal", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Normals = ((TCloud)SolidElementConstruction.Defs[def]);
                        break;
                    //PushDef()
                    case "DEF":
                        PendingDef = stc.Pop();
                        break;
                    //ParseDef()
                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseInfo()
        {
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "]":
                        break;

                    case "\"":
                        Info = ParseText();
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                if (word == "]") break;
            }
        }

        public void ParseLineSet(TNGroup iParentGroup)
        {
            int decalage = DataPositions.Length;
            string LastWord = null;
            TLGroup group = SolidElementConstruction.AddLGroup();
            group.GroupParameters.Texture = iParentGroup.GroupParameters.Texture;
            iParentGroup.LGroups.Push(group);
            string PendingDef = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "Coordinate":
                    case "coord":
                    case "coordIndex":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Coordinate":
                                TCloud cloud = SolidElementConstruction.AddCloud(ref PendingDef);
                                group.ShapeGroupParameters.Positions = cloud;
                                ParsePointCoordinate(cloud);
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "coordIndex":
                                //Dim ptFrom As Nullable(Of Vector3d) = Nothing
                                while (stc.Count > 0)
                                {
                                    word = stc.Pop();
                                    switch (word)
                                    {
                                        case "]":
                                            break;

                                        case "-1":
                                            break;

                                        default:
                                            //Array.Resize(ref @group.Indexes, @group.Indexes.Length + 1);
                                            @group.Indexes.Push(int.Parse(word, CultureInfo.InvariantCulture));
                                            break;
                                        //Dim ptTo As Vector3d = Points(CInt(word) + decalage)

                                        //If ptFrom.HasValue And True Then
                                        //    AddPointAndNormal(ptFrom.Value, New Vector3d, BlackColor, False)
                                        //    AddPointAndNormal(ptTo, New Vector3d, BlackColor, False)
                                        //    LinesIndexes.Add(PointsCount - 2)
                                        //    LinesIndexes.Add(PointsCount - 1)
                                        //End If
                                        //ptFrom = ptTo
                                    }
                                    if (word == "]") break;
                                }
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    case "USE":
                        string def = stc.Pop();
                        if (def.StartsWith("_coord", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Positions = ((TCloud)SolidElementConstruction.Defs[def]);
                        else if (def.StartsWith("_normal", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Normals = ((TCloud)SolidElementConstruction.Defs[def]);
                        break;

                    case "DEF":
                        PendingDef = stc.Pop();
                        break;
                    //ParseDef()
                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseMain()
        {
            string LastWord = null;
            string pendingdef = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "WorldInfo":
                    case "NavigationInfo":
                    case "Background":
                    case "#VRML":
                    case "V2.0":
                    case "utf8":
                    case "Viewpoint":
                    case "Transform":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "WorldInfo":
                                ParseWorldInfo();
                                break;

                            case "NavigationInfo":
                                ParseNavigationInfo();
                                break;

                            case "Background":
                                ParseBackground();
                                break;

                            case "Viewpoint":
                                ParseViewpoint();
                                break;

                            case "Transform":
                                TNGroup group = SolidElementConstruction.AddNGroup(ref pendingdef);
                                group.GroupParameters.Texture = SolidElementConstruction.StartGroup.GroupParameters.Texture;

                                SolidElementConstruction.StartGroup.NGroups.Push(group);
                                ParseTransform(group);
                                //  New matrix4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1))
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "DEF":
                        pendingdef = stc.Pop();
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
            }
        }

        public void ParseMaterial(TTexture iTexture)
        {
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "emissiveColor":
                    case "diffuseColor":
                        ParseColor(iTexture);
                        break;

                    case "transparency":
                        stc.Pop();
                        break;

                    case "ambientIntensity":
                        stc.Pop();
                        break;

                    case "shininess":
                        stc.Pop();
                        break;

                    case "specularColor":
                        stc.Pop();
                        stc.Pop();
                        stc.Pop();
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                if (word == "}") break;
            }
        }

        public void ParseNavigationInfo()
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "type":
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "type":
                                ParseType();
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseNormalCoordinate(TCloud iCloud)
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "vector":
                    case "[":
                        break;

                    case "]":
                        break;

                    case "}":
                        break;

                    default:
                        Vec3 pt = new Vec3(double.Parse(word, CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture));
                        if (pt.LengthSquared != 1) pt.Normalize();
                        iCloud.Vectors.Push(pt);
                        break;
                    //Dim v As Vector3d = Vector4.Transform(pt, iMtx).Xyz
                    //AddNormal(v, LastColor)
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParsePointCoordinate(TCloud iCloud)
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "point":
                    case "[":
                        break;

                    case "]":
                        break;

                    case "}":
                        break;

                    default:
                        Vec3 pt = new Vec3(double.Parse(word, CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture));
                        // Array.Resize(ref iCloud.Vectors, iCloud.Vectors.Length + 1);
                        iCloud.Vectors.Push(pt);
                        break;
                    //AddPoint((Vector4.Transform(pt, iMtx).Xyz), False)
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParsePointSet(TNGroup iParentGroup)
        {
            string LastWord = null;
            TPGroup group = SolidElementConstruction.AddPGroup();
            group.GroupParameters.Texture = iParentGroup.GroupParameters.Texture;
            //
            iParentGroup.PGroups.Push(group);
            string PendingDef = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "Coordinate":
                    case "coord":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Coordinate":
                                TCloud cloud = SolidElementConstruction.AddCloud(ref PendingDef);
                                group.ShapeGroupParameters.Positions = cloud;
                                ParsePointCoordinate(cloud);
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    case "USE":
                        string def = stc.Pop();
                        if (def.StartsWith("_coord", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Positions = ((TCloud)SolidElementConstruction.Defs[def]);
                        else if (def.StartsWith("_normal", StringComparison.Ordinal))
                            @group.ShapeGroupParameters.Normals = ((TCloud)SolidElementConstruction.Defs[def]);
                        else
                            throw new Exception("Unknown Cloud " + def);

                        break;

                    case "DEF":
                        PendingDef = stc.Pop();
                        break;
                    //ParseDef()
                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseShape(TNGroup iParentGroup)
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "appearance":
                    case "Appearance":
                    case "geometry":
                    case "PointSet":
                    case "IndexedFaceSet":
                    case "IndexedLineSet":
                        break;

                    case "{":
                        switch (LastWord)
                        {
                            case "Appearance": ParseAppearance(iParentGroup); break;
                            case "PointSet": ParsePointSet(iParentGroup); break;
                            case "IndexedFaceSet": ParseFaceSet(iParentGroup); break;
                            case "IndexedLineSet": ParseLineSet(iParentGroup); break;
                            default: throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;
                    //Case "USE"
                    //    PushDef()
                    case "DEF":
                        stc.Pop();
                        break;
                    //    ParseDef()
                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public string ParseText()
        {
            string functionReturnValue = null;
            functionReturnValue = "";
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "\"":
                        break;

                    default:
                        functionReturnValue = (functionReturnValue + " " + word).Trim();
                        break;
                }
                if (word == "\"") break;
            }
            return functionReturnValue;
        }

        public void ParseTextureCoordinate(TCloud iCloud)
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "point":
                    case "[":
                        break;

                    case "]":
                        break;

                    case "}":
                        break;

                    default:
                        stc.Pop();
                        break;
                    //iCloud.Vectors.Add(pt.Xyz)
                    //AddPoint((Vector4.Transform(pt, iMtx).Xyz), False)
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        public void ParseTransform(TNGroup iParentGroup)
        {
            string LastWord = null;
            while (stc.Count > 0 && LastWord != "}")
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "children":
                        break;

                    case "inisOfRotation":
                        stc.Pop();
                        stc.Pop();
                        stc.Pop();
                        break;

                    case "rotation":
                        Mtx4 m1 = Mtx4.CreateFromAxisAngle(new Vec3(double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture)), double.Parse(stc.Pop(), CultureInfo.InvariantCulture));
                        iParentGroup.GroupParameters.Matrix = m1 * iParentGroup.GroupParameters.Matrix;
                        break;

                    case "scaleOrientation":
                        Mtx4 m2 = Mtx4.CreateFromAxisAngle(new Vec3(double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture)), double.Parse(stc.Pop(), CultureInfo.InvariantCulture));
                        break;

                    case "translation":
                        Mtx4 m3 = Mtx4.CreateTranslation(new Vec4(double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), 0));
                        iParentGroup.GroupParameters.Matrix = m3 * iParentGroup.GroupParameters.Matrix;
                        break;

                    case "scale":
                        Mtx4 m4 = Mtx4.Scale(double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture), double.Parse(stc.Pop(), CultureInfo.InvariantCulture));
                        iParentGroup.GroupParameters.Matrix = m4 * iParentGroup.GroupParameters.Matrix;
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "children":
                                TNGroup group = SolidElementConstruction.AddNGroup();
                                group.GroupParameters.Texture = iParentGroup.GroupParameters.Texture;
                                iParentGroup.NGroups.Push(group);
                                ParseChildren(group);
                                break;
                        }
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
            }
        }

        public void ParseType()
        {
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "]":
                        break;

                    case "\"":
                        NavigationTypes.Add(ParseText());
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                if (word == "]") break;
            }
        }

        public void ParseViewpoint()
        {
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "position":
                        stc.Pop(); stc.Pop(); stc.Pop();
                        break;

                    case "orientation":
                        stc.Pop(); stc.Pop(); stc.Pop(); stc.Pop();
                        break;

                    case "fieldOfView":
                        stc.Pop();
                        break;

                    case "description":
                        stc.Pop();
                        ParseText();
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                if (word == "}") break;
            }
        }

        public void ParseWorldInfo()
        {
            string LastWord = null;
            while (stc.Count > 0)
            {
                string word = stc.Pop();
                switch (word)
                {
                    case "info":
                        break;

                    case "[":
                        switch (LastWord)
                        {
                            case "info":
                                ParseInfo();
                                break;

                            default:
                                throw new Exception("Unplanned word " + word);
                        }
                        break;

                    case "}":
                        break;

                    default:
                        throw new Exception("Unplanned word " + word);
                }
                LastWord = word;
                if (word == "}") break;
            }
        }

        #endregion Public Methods
    }
}