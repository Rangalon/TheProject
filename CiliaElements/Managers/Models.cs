
using CiliaElements.Elements.Control;
using CiliaElements.Format3DXml;
using Math3D;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Public Fields

        public static TLink GenericAircraft;

        #endregion Public Fields

        #region Private Fields

        private static bool DoerComputeActivated;

        #endregion Private Fields

        #region Public Methods

        public static void AddSelectedLink(TLink iLink)
        {
            iLink.State |= ELinkState.Selected;
            selectedLinks.Add(iLink); SelectedLinkChanged?.Invoke(iLink);
        }

        public static TLink AttachAndLoadElmt(TBaseElement child)
        {
            child.LaunchLoad();
            TLink l = AttachElmt(child.OwnerLink, View.OwnerLink, ToBeParsedLinks);
            View.ElementLoader.Publish();
            return l;
        }

        public static TLink AttachElmt(TSolidElement e)
        {
            _ = new TFile(e);
            e.OwnerLink.ToBeReplaced = true;
            e.OwnerLink.NodeName = e.Fi.Name;
            //
            TLink l = AttachElmt(e.OwnerLink, View.OwnerLink, ToBeParsedLinks);
            View.ElementLoader.Publish();
            //BuildIndexes();
            return l;
        }

        public static TLink AttachElmt(TSolidElement iSolid, TLink iParentLink, TQuickStc<TLink> iStc=null)
        {
            return AttachElmt(iSolid.OwnerLink, iParentLink, iStc);
        }

        public static TLink AttachElmt(TLink iElmtLink, TLink iParentLink, TQuickStc<TLink> iStc=null)
        {
            if (iElmtLink == null)
            {
                return null;
            }

            TLink link = new TLink
            {
                CustomAttributes = iElmtLink.CustomAttributes,
                TextRanges = iElmtLink.TextRanges,
                Child = iElmtLink.Child,
                ParentLink = iParentLink,
                NodeName = iElmtLink.NodeName,
                Matrix = iElmtLink.Matrix,
                State = iElmtLink.State,
                ForeColor = iElmtLink.ForeColor,
                BackColor = iElmtLink.BackColor
            };
            //If link.Child.Fi IsNot Nothing Then link.File = UsedFiles(link.Child.Fi.FullName.ToUpper)
            if (iElmtLink.Child is TSolidElement | iElmtLink.ToBeReplaced)
            {
                link.FileName = iElmtLink.FileName;
                link.ToBeReplaced = true;
                if ((iStc != null))
                {
                    iStc.Push(link);
                }
            }
            //
            foreach (TLink linktmp in iElmtLink.Links.Values)
            {
                AttachElmt(linktmp, link, iStc);
            }
            //
            return link;
        }

        public static void ClearSelection()
        {
            foreach (TLink l in selectedLinks)
            {
                l.State ^= ELinkState.Selected;
            }

            selectedLinks.Clear();
            SelectedLinkChanged?.Invoke(null);
        }


        public static void LoadFile(FileInfo iFi)
        {
            switch (iFi.Extension.ToUpperInvariant())
            {
                //case ".CILIAK":
                //    System.Xml.XmlDocument XmlDoc = new System.Xml.XmlDocument();
                //    XmlDoc.Load(iFi.FullName);
                //    //
                //    foreach (System.Xml.XmlNode XmlElmt in XmlDoc.SelectNodes("//CONSTRAINT"))
                //    {
                //        string[] Tbl1 = XmlElmt.Attributes.GetNamedItem("p1").Value.Split('/');
                //        TLink RootLink = View.OwnerLink.FindFromPartNumber(Tbl1[0]);
                //        TLink L1 = RootLink;
                //        for (int i = 1; i < Tbl1.Length - 1; i++)
                //        {
                //            L1 = L1.FindFromNodeName(Tbl1[i]);
                //        }

                //        if (XmlElmt.Attributes.GetNamedItem("p2") != null)
                //        {
                //            TLink L2 = RootLink;
                //            string[] Tbl2 = XmlElmt.Attributes.GetNamedItem("p2").Value.Split('/');
                //            for (int i = 1; i < Tbl2.Length - 1; i++)
                //            {
                //                L2 = L2.FindFromNodeName(Tbl2[i]);
                //            }

                //            TReference R1 = new TReference(L1, RootLink, XmlElmt.SelectSingleNode("./R1"));
                //            TReference R2 = new TReference(L2, RootLink, XmlElmt.SelectSingleNode("./R2"));
                //            L1.References.Add(R1);
                //            L2.References.Add(R2);
                //            TConstraint C = new TConstraint(R1, R2)
                //            {
                //                ConstraintType = (EConstraintType)int.Parse(XmlElmt.Attributes.GetNamedItem("t").Value, CultureInfo.InvariantCulture),
                //                MinTarget = double.Parse(XmlElmt.Attributes.GetNamedItem("min").Value, CultureInfo.InvariantCulture)
                //            };
                //            C.MinTarget = double.Parse(XmlElmt.Attributes.GetNamedItem("min").Value, CultureInfo.InvariantCulture);
                //            RootLink.Constraints.Add(C);
                //        }
                //        else if (XmlElmt.Attributes.GetNamedItem("p1") != null)
                //        {
                //            L1.Fixed = true;
                //            //for (Int32 i = 1; i < Tbl1.Length - 1; i++)
                //            //    L1 = L1.FindItem(Tbl1[i]);
                //            //TReference R1 = new TReference(L1, RootLink, null);
                //            //L1.References.Add(R1);
                //            //TConstraint C = new TConstraint(RootLink, R1, null);
                //            //C.ConstraintType = (EConstraintType)Int32.Parse(XmlElmt.Attributes.GetNamedItem("t").Value);
                //            //C.Target = double.Parse(XmlElmt.Attributes.GetNamedItem("v").Value);
                //            //RootLink.Constraints.Add(C);
                //        }
                //    }
                //    //
                //    BuildConstraints();
                //    break;

                default:
                    TFile file = new TFile(iFi, null);
                    TBaseElement Elmt = file.Element;
                    Elmt.OwnerLink.ToBeReplaced = true;
                    Elmt.OwnerLink.NodeName = iFi.Name;
                    //
                    AttachElmt(Elmt.OwnerLink, View.OwnerLink, ToBeParsedLinks);
                    View.ElementLoader.Publish();

                    break;
            }
        }

        public static void PackSelected()
        {
            Thread th = CreateThread(PackSelectedThread, "PackSelectedThread");
        }

        public static void PackSelectedThread()
        {
            foreach (TLink l in selectedLinks.ToArray())
            {
                l.Child.Pack();
            }
        }

        public static List<Action> PushModels = new List<Action>();

        public static TIconBar IconsBar;
        public static void PushSetup()
        {
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Building Task";
            DoerBuildingsThread = CreateThread(DoerBuildings, "DoerBuildings", ThreadPriority.Lowest, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Loading Task";
            DoerLoadingsThread = CreateThread(DoerLoadings, "DoerLoadings", ThreadPriority.AboveNormal, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Physics Task";
            DoerPhysicsThread = CreateThread(DoerPhysics, "DoerPhysics", ThreadPriority.Lowest, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Creating View";
            View = new TAssemblyElement();
            View.OwnerLink.PartNumber = "World";
            View.OwnerLink.NodeName = "World";
            View.OwnerLink.Expanded = true;
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Creating Shaders";
            HandleVertexShader = GL.CreateShader(ShaderType.VertexShader);
            HandleFragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(HandleVertexShader, SourceVertexShader);
            GL.ShaderSource(HandleFragmentShader, SourceFragmentShader);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Compiling Shaders";
            GL.CompileShader(HandleVertexShader);
            GL.CompileShader(HandleFragmentShader);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Creating Shading Program";
            HandleShaderProgram = GL.CreateProgram();
            GL.AttachShader(HandleShaderProgram, HandleVertexShader);
            GL.AttachShader(HandleShaderProgram, HandleFragmentShader);
            GL.LinkProgram(HandleShaderProgram);
            GL.UseProgram(HandleShaderProgram);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Creating Handlers";
            HandleProjectionMatrix = GL.GetUniformLocation(HandleShaderProgram, "projection_matrix");
            HandleColorMatrix = GL.GetUniformLocation(HandleShaderProgram, "color_matrix");
            HandleNoEffect = GL.GetUniformLocation(HandleShaderProgram, "no_effect");
            HandleNoDiffuse = GL.GetUniformLocation(HandleShaderProgram, "no_diffuse");
            HandleTextureOffset = GL.GetUniformLocation(HandleShaderProgram, "texture_offset");
            HandleModelMatrix = GL.GetUniformLocation(HandleShaderProgram, "model_matrix");
            HandleViewMatrix = GL.GetUniformLocation(HandleShaderProgram, "view_matrix");

            //GL.UniformMatrix4(HandleColorMatrix, false, ref ColorNormal);

            //Mtx4f m = Mtx4f.Identity;
            //GL.UniformMatrix4(HandleModelMatrix, false, ref m);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Creating Environment Objects";
            CrossLink = AttachElmt((new TCross()).OwnerLink, View.OwnerLink, null);
            CrossLink.NodeName = "Cross";
            //for (int i = 0; i < 256; i++)
            //{
            //    Characters[i] = CrossLink.Solid;
            //}

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //AttachElmt((new TSnubDodecahedron()).OwnerLink, View.OwnerLink, null).Pickable = false;

            GroundLink = AttachElmt((new TGround()).OwnerLink, View.OwnerLink, null);
            GroundLink.NodeName = "Ground";
            //CrossLink1 = AttachElmt((new TCross()).OwnerLink, View.OwnerLink, null);
            //CrossLink1.NodeName = "Cross1";
            //CrossLink2 = AttachElmt((new TCross()).OwnerLink, View.OwnerLink, null);
            //CrossLink2.NodeName = "Cross2";
            //CrossLink3 = AttachElmt((new TCross()).OwnerLink, View.OwnerLink, null);
            //CrossLink3.NodeName = "Cross3";
            //Univers = AttachElmt((new TAssemblyElement() { PartNumber = "Univers" }).OwnerLink, View.OwnerLink, null);
            //Univers.NodeName = "Univers";
            //TDummyAsteroid.asteroid = new TAsteroid();
            //TLink lk = AttachElmt(TDummyAsteroid.asteroid.OwnerLink, Univers, null);
            //lk.NodeName = "Sphere";
            //lk.Enabled = false;
            //lk = AttachElmt(TPlain.Default.OwnerLink, Univers, null);
            //lk.NodeName = "Plain";
            //lk.Enabled = false;
            //for (int i = 0; i < 8; i++)
            //{
            //    lk = AttachElmt(TPlain.Resources[i].OwnerLink, Univers, null);
            //    lk.NodeName = "Resource" + i.ToString();
            //    lk.Enabled = false;
            //}
            //lk = AttachElmt(TWater.Default.OwnerLink, Univers, null);
            //lk.NodeName = "Water";
            //lk.Enabled = false;
            //HandlingAsteroid = lk;

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //LogLine = "Loading Spider";
            //T3DXmlElement s = new T3DXmlElement("Spider", Properties.Resources.Spider);
            //s.LaunchLoad();
            //LogLine = "GenericAircraft";
            //s = new T3DXmlElement("GenericAircraft", new FileInfo(@"\\sns.corp\Apps\ENGINEERING\EV\EVI\FTSA_Dev\FT-OnBoard\Models\Aircraft.3dxml"));
            //s.LaunchLoad();
            //GenericAircraft = AttachElmt(s.OwnerLink, View.OwnerLink, null);
            //GenericAircraft.Enabled = false;
            //LogLine = "EarthSky";
            //s = new T3DXmlElement("EarthSky", new FileInfo(@"\\sns.corp\Apps\ENGINEERING\EV\EVI\FTSA_Dev\FT-OnBoard\Models\EarthSky.3dxml"));
            //s.LaunchLoad();
            //TLink ll = AttachElmt(s.OwnerLink, View.OwnerLink, null);
            //ll.Matrix = Mtx4.CreateScale(1000000, 1000000, 1000000);
            //ll.DisplayColor = Mtx4f.CreateStaticColor(0.3f, 0.3f, 0.6f);
            //Spider = AttachElmt(s.OwnerLink, View.OwnerLink, null);
            //Spider.NodeName = "Spider";
            //Spider.Expanded = true;
            //MoverSpiderThread = CreateThread(MoverSpider) { Priority = ThreadPriority.Lowest }; MoverSpiderThread.Start();

            foreach (Action a in PushModels) a.Invoke();

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            graphTree = new TTreeView("GraphTree")
            {
                RootObject = View.OwnerLink
            };
            graphTree.SelectedObjectsChanged += GraphTree_SelectedObjectsChanged;

            performancesPanel = new TPerformancesPanel("Performances"); performancesPanel.OwnerLink.Pickable = false;

            threadsPanel = new TThreadsPanel("Threads");// threadsPanel.OwnerLink.Pickable = false;

            linkPanel = new TLinkPanel("LinkPanel"); linkPanel.OwnerLink.Pickable = false;


            targetPanel = new TVectorPanel("Target") { Title = "Target" }; targetPanel.OwnerLink.Pickable = false;

            viewerPanel = new TVectorPanel("Viewer") { Title = "Viewer" }; viewerPanel.OwnerLink.Pickable = false;

            overflownPanel = new TVectorPanel("Overflown"); overflownPanel.OwnerLink.Pickable = false;

            loadingPanel = new TLabel("Loading") { Title = "Loading" }; loadingPanel.OwnerLink.Pickable = false;


            commandsPanel = new TLabel("Commands") { Title = "Commands" }; commandsPanel.OwnerLink.Pickable = false;


            logsPanel = new TLabel("Logs") { Title = "Logs", Message = "" }; logsPanel.OwnerLink.Pickable = false;


            IconsBar = new TIconBar("Main Bar");
            TButton icon;
            TIconBarButton iconbaricon;
            //
            icon = new TButton("Open Files") { ForePicture = Properties.Resources.folder, ActionToDo = TManager.OpenFiles }; IconsBar.Icons.Add(icon);
            OpenFileButton = icon.OwnerLink;
            //
            iconbaricon = new TIconBarButton("Views") { ForePicture = Properties.Resources.Views, IconBar = new TIconBar("Views Bar") }; IconsBar.Icons.Add(iconbaricon);
            iconbaricon.IconBar.IconBarOwner = iconbaricon;
            icon = new TButton("View All") { ForePicture = Properties.Resources.View_All, ActionToDo = FitAll }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Sel") { ForePicture = Properties.Resources.View_Sel, ActionToDo = FitSelected }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Top") { ForePicture = Properties.Resources.View_Top, ActionToDo = FitTop }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Bottom") { ForePicture = Properties.Resources.View_Bottom, ActionToDo = FitBottom }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Front") { ForePicture = Properties.Resources.View_Front, ActionToDo = FitFront }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Rear") { ForePicture = Properties.Resources.View_Rear, ActionToDo = FitRear }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Left") { ForePicture = Properties.Resources.View_Left, ActionToDo = FitLeft }; iconbaricon.IconBar.Icons.Add(icon);
            icon = new TButton("View Right") { ForePicture = Properties.Resources.View_Right, ActionToDo = FitRight }; iconbaricon.IconBar.Icons.Add(icon);
            iconbaricon.IconBar.Visible = false;
            //
            iconbaricon = new TIconBarButton("Settings") { ForePicture = Properties.Resources.gear, IconBar = new TIconBar("Settings Bar") }; IconsBar.Icons.Add(iconbaricon);
            iconbaricon.IconBar.IconBarOwner = iconbaricon;
            iconShowFaces = new TButton("Show Faces") { ForePicture = Properties.Resources.Draw_Faces, ActionToDo = SwitchDrawFaces }; iconbaricon.IconBar.Icons.Add(iconShowFaces);
            iconShowLines = new TButton("Show Lines") { ForePicture = Properties.Resources.Draw_Lines, ActionToDo = SwitchDrawLines }; iconbaricon.IconBar.Icons.Add(iconShowLines);
            iconShowPoints = new TButton("Show Points") { ForePicture = Properties.Resources.Draw_Points, ActionToDo = SwitchDrawPoints }; iconbaricon.IconBar.Icons.Add(iconShowPoints);
            iconShowCoordinates = new TButton("Show Coordinates") { ForePicture = Properties.Resources.Draw_Coordinates, ActionToDo = SwitchDrawCoordinates }; iconbaricon.IconBar.Icons.Add(iconShowCoordinates);
            iconShowPerformances = new TButton("Show Performances") { ForePicture = Properties.Resources.Draw_Performances, ActionToDo = SwitchDrawPerformances }; iconbaricon.IconBar.Icons.Add(iconShowPerformances);
            iconSetPerspective = new TSlider("Set Perspective Angle") { ForePicture = Properties.Resources.Perspective, Minimum = 0.1, Maximum = Math.PI * 0.5 - 0.1 }; iconSetPerspective.IconOwner = iconbaricon.IconBar; iconbaricon.IconBar.Icons.Add(iconSetPerspective);
            iconSetPerspective.ValueChanged += IconSetPerspective_ValueChanged;
            iconbaricon.IconBar.Visible = false;
            //
            iconMoveObjects = new TButton("Move Objects") { ForePicture = Properties.Resources.anchor, ActionToDo = SwitchEntitiesMoving }; IconsBar.Icons.Add(iconMoveObjects);
            iconMovingMode = new TButton("Moving Mode") { ForePicture = Properties.Resources.Rotation, ActionToDo = SwitchMoveMode }; IconsBar.Icons.Add(iconMovingMode);
            //
            //TManager.AttachElmt(bar.OwnerLink, View.OwnerLink, null); 
            //TManager.AttachElmt(OpenFileButton , View.OwnerLink, null); OpenFileButton.ToBeReplaced = false;
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //LogLine = "Loading Icons";
            //T3DXmlElement ViewBar = new T3DXmlElement("ViewBar", Properties.Resources.ViewBar);
            //ViewBar.LaunchLoad();
            //IconsBar = TManager.AttachElmt(ViewBar.OwnerLink, View.OwnerLink, null);
            //IconsBar.NodeName = IconsBar.PartNumber; IconsBar.DrawInTree = false;
            //IconsBar.Expanded = true;
            //IconsBar.IconLink = true;
            //Stack<TLink> lnks = new Stack<TLink>();
            //foreach (TLink ll in IconsBar.Links.Values.Where(o => o != null))
            //{
            //    lnks.Push(ll);
            //    TLink lll = IconsBar.Links.Values.FirstOrDefault(o => o != null && o.PartNumber == ll.PartNumber + "Icons");
            //    if (lll != null)
            //    {
            //        lll.Enabled = false;
            //        TManager.BarsIcons.Add(ll, lll);
            //    }
            //}
            //while (lnks.Count > 0)
            //{
            //    TLink l = lnks.Pop();
            //    switch (l.PartNumber)
            //    {
            //        case "Open Files": l.ActionToDo = TManager.OpenFiles; break;
            //        case "View Treee": l.ActionToDo = TManager.SwitchDrawTree; break;
            //        case "Top View": l.ActionToDo = TManager.FitTop; break;
            //        case "Bottom View": l.ActionToDo = TManager.FitBottom; break;
            //        case "Left View": l.ActionToDo = TManager.FitLeft; break;
            //        case "Right View": l.ActionToDo = TManager.FitRight; break;
            //        case "Front View": l.ActionToDo = TManager.FitFront; break;
            //        case "Rear View": l.ActionToDo = TManager.FitRear; break;
            //        case "Fit Object": l.ActionToDo = TManager.FitSelected; break;
            //        case "Fit All": l.ActionToDo = TManager.FitAll; break;
            //        case "View Surfaces": l.ActionToDo = TManager.SwitchDrawFaces; break;
            //        case "View Lines": l.ActionToDo = TManager.SwitchDrawLines; break;
            //        case "View Points": l.ActionToDo = TManager.SwitchDrawPoints; break;
            //        case "View Annotations": l.ActionToDo = TManager.SwitchDrawAnnotations; break;
            //        case "View Measures": l.ActionToDo = TManager.SwitchDrawMeasures; break;
            //        case "Fixed": l.ActionToDo = TManager.SwitchEntitiesMoving; break;
            //        case "Rotate": l.ActionToDo = TManager.SwitchMoveMode; break;
            //        default: foreach (TLink ll in l.Links.Values.Where(o => o != null)) { lnks.Push(ll); } break;
            //    }
            //}
            //
            SwitchDrawFaces();
            SwitchDrawLines();
            SwitchDrawPoints();
            SwitchDrawIcons();
            SwitchDrawMeasures();
            //SwitchDrawCoordinates();
            //SwitchDrawPerformances();
            //SwitchDrawTree();
            SwitchMoveMode();
            PerspectiveAngle = 0.5;
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Final Building";
            VMatrix = Mtx4.LookAt(new Vec3(-45, -40, 33), Target, Up);

            Up = (Vec3)VMatrix.Row2;

            OwnerLink = View.OwnerLink;

            View.ElementLoader.Publish();
            //
            //PerspectiveAngleSlider = IconsBar.Links.Values.First(o => o.PartNumber == "SettingsIcons").Links.Values.First(o => o.PartNumber == "Perspective Angle").Links.Values.First(o => o.PartNumber == "Slider");
            //
            Stack<TLink> stc = new Stack<TLink>();
            stc.Push(View.OwnerLink);
            while (stc.Count > 0)
            {
                TLink l;
                l = stc.Pop();
                if (l == null)
                {
                    continue;
                }

                foreach (TLink link in l.Links.Values)
                {
                    stc.Push(link);
                }
                if (l.ToBeReplaced)
                {
                    ToBeParsedLinks.Push(l);
                }
            }

            GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //
            GL.Enable(EnableCap.DepthTest);
            ResetPerspectives();

            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Compute Task";
            DoerComputeActivated = true; ComputeThread = CreateThread(DoerCompute, "DoerCompute", ThreadPriority.Lowest, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Inputs Task";
            CheckEntriesThread = CreateThread(CheckEntries, "CheckEntries", ThreadPriority.Normal, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //LogLine = "Launching Mouse Task";
            //CheckMouseThread = CreateThread(CheckMouse, "CheckMouse", ThreadPriority.Normal, true);
            //// ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //LogLine = "Launching Joystick Task";
            //CheckJoystickThread = CreateThread(CheckJoystick, "CheckJoystick", ThreadPriority.Highest, true);
            //// ---------------------------------------------------------------------------------------------------------------------------------------------------------
            //LogLine = "Launching Touch Task";
            //CheckTouchesThread = CreateThread(CheckTouches, "CheckTouches", ThreadPriority.Highest, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Overfly Task";
            CheckOverFlyThread = CreateThread(CheckOverFly, "CheckOverFly", ThreadPriority.Normal, true);
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------
            LogLine = "Launching Garbage Task";
            DoerGarbageThread = CreateThread(DoerGarbage, "DoerGarbage", ThreadPriority.Lowest, true);
            LogLine = "Done!";
            UpdateLayout();
            SetupDone = true;
            Thread th = CreateThread(LoadModels, "LoadModels");
        }



        static void CheckEntries()
        {
            while (true)
            {
                if (Focused)
                {
                    CheckKeyBoardEntries();
                    CheckMouseEntries();
                    CheckTouchEntries();
                    CheckJoystickEntries();
                }
                else
                {
                    PreviousJoystick = null;
                }
                Thread.Sleep(10);
            }
        }

        public static readonly List<Thread> ThreadsList = new List<Thread>();

        public static Thread CreateThread(ThreadStart start, string name, ThreadPriority prio = ThreadPriority.Lowest, bool isBack = false)
        {
            ThreadsList.RemoveAll(o => o.ThreadState == ThreadState.Stopped);
            Thread th = new Thread(start) { Name = name, Priority = ThreadPriority.Lowest, IsBackground = false };
            ThreadsList.Add(th); th.Start();
            return th;
        }

        public static void StopDoers()
        {
            DoerComputeActivated = false;
            DoerBuildingsThread.Abort();
            DoerGarbageThread.Abort();
            DoerLoadingsThread.Abort();
            CheckEntriesThread.Abort();
            CheckOverFlyThread.Abort();
            ComputeThread.Abort();
            //MoverSpiderThread.Abort();
            TControl.StopAll();
        }

        #endregion Public Methods

        #region Private Methods

        private static void BuildBox(TLink iLink, Mtx4 iMtx, List<Vec4> iPoints)
        {
            if (iLink == null || !iLink.Enabled || iLink == CrossLink || iLink == GroundLink)
            {
                return;
            }

            iMtx = iLink.Matrix * iMtx;
            if (iLink.Child is TAssemblyElement)
            {
                foreach (TLink link in iLink.Links.Values)
                {
                    BuildBox(link, iMtx, iPoints);
                }
            }
            else if (iLink.Child.State > EElementState.Compiled && !(iLink.Child is TInternal))
            {
                iPoints.AddRange(iLink.Child.BoundingBox.GetCorners(iMtx));
            }
        }

        private static void DoerBuildings()
        {
            while (true)
            {
                if (ToBeParsedLinks.NotEmpty)
                {
                    BuildingWatch.Start();
                    while (ToBeParsedLinks.NotEmpty)
                    {
                        TLink ilink = ToBeParsedLinks.Pop();
                        if (ilink.File == null)
                        {
                            ilink.File = UsedFiles.Values.FirstOrDefault(o => o.Element.Fi.FullName.ToUpper(CultureInfo.InvariantCulture) == ilink.Child.Fi.FullName.ToUpper(CultureInfo.InvariantCulture));
                        }
                        //
                        TFile loader = ilink.File;
                        if (loader != null && loader.Element.State > EElementState.Compiled)
                        {
                            ilink.Child = loader.Element;
                            //
                            if (ilink.Child is TSolidElement)
                            {
                                UpdatedLinks.Push(ilink);
                            }
                            else if (ilink.Child is TAssemblyElement assembly)
                            {
                                Array.ForEach(assembly.OwnerLink.Links.Values, link => { AttachElmt(link, ilink, ToBeParsedLinks); });
                            }
                        }
                        else
                        {
                            ToBeParsedLinks.Postpone(ilink);
                        }

                        ilink.ToBeReplaced = false;
                    }
                    BuildingWatch.Stop();
                }
                Thread.Sleep(100);
            }
        }



        private static void DoerGarbage()
        {
            while (true)
            {
                if (SolidsToBeGarbaged.NotEmpty)
                {
                    TSolidElement Solid = SolidsToBeGarbaged.Pop();
                    TSolidElementConstruction SolidElementConstruction = Solid.SolidElementConstruction;
                    //
                    if (!(Solid is FormatCilia.TCiliaSolid))
                    {
                        Array.ForEach(SolidElementConstruction.Clouds.Values, c => { c.Vectors = null; c.Indexes = null; });
                        Array.ForEach(SolidElementConstruction.Groups.Values, group => { group.Dispose(); });
                    }

                    SolidElementConstruction.Clouds = null;
                    //
                    SolidElementConstruction.Groups = null;
                    SolidElementConstruction.StartGroup.Dispose();
                    SolidElementConstruction.StartGroup = null;
                    //
                    SolidElementConstruction.Defs.Clear();
                    SolidElementConstruction.Defs = null;
                    //
                    SolidElementConstruction.Textures.Dispose();
                    SolidElementConstruction.Textures = null;
                    //
                    //
                    if (!Solid.WillBeUpdated)
                    {
                        SolidElementConstruction.vsP = null;
                        SolidElementConstruction.vsN = null;
                        Solid.SolidElementConstruction = null;
                    }
                    Solid = null;
                }
                else
                {
                    SolidsToBeGarbaged.ResetIfEmpty();
                    SolidsToBePushed.ResetIfEmpty();
                }
                Thread.Sleep(10);
            }
        }

        private static void GraphTree_SelectedObjectsChanged(object[] objs)
        {
            ClearSelection();
            foreach (TLink l in objs)
            {
                AddSelectedLink(l);
            }
            SelectedLinkChanged?.Invoke((TLink)objs[objs.Length - 1]);
        }

        private static void IconSetPerspective_ValueChanged(double d)
        {
            PerspectiveAngle = d;
        }

        private static void LoadModels()
        {
            LogLine = "Loading Symbols";
            T3DXmlElement Symbols = new T3DXmlElement("Symbols", Properties.Resources.Symbols);
            Symbols.LaunchLoad();
            RepFaceRotate = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "FaceRotate").Child;
            RepFaceTranslate = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "FaceTranslate").Child;
            RepMeasureArrow = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "MeasureArrow").Child;
            RepMeasurePoint = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "MeasurePoint").Child;
            RepSideRotate = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "SideRotate").Child;
            RepSideTranslate = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "SideTranslate").Child;
            RepBBox = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "BBox").Child;
            RepPicker = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "Picker").Child;
            RepCursor = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "Cursor").Child;
            RepMovingPoint = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "MovingPoint").Child;
            RepTarget = (T3DRepElement)Symbols.OwnerLink.Links.Values.First(o => o.PartNumber == "Target").Child;
            MoveSolid = RepFaceTranslate;
            // ---------------------------------------------------------------------------------------------------------------------------------------------------------

            //
        }

        public enum ELoadStuff
        {
            All,
            FacesOnly,
            EdgesOnly
        }

        public static TLink LoadResource(string name, byte[] bts, TBaseElement assy, ELoadStuff ls = ELoadStuff.All)
        {
            TFile file = new TFile(name, bts, ls);
            TBaseElement Elmt = file.Element;
            Elmt.OwnerLink.ToBeReplaced = true;
            Elmt.OwnerLink.NodeName = name;
            //
            TLink l = AttachElmt(Elmt.OwnerLink, assy.OwnerLink, ToBeParsedLinks);
            View.ElementLoader.Publish();
            //
            return l;
        }

        #endregion Private Methods
    }
}