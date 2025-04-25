
using Math3D;
using CiliaElements.Elements.Control;
using CiliaElements.Format3DXml;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Public Fields

        public static TLayer BaseLayer;
        public static Stopwatch BuildingWatch = new Stopwatch();
        public static DoJoystickButton ClickButton0;
        public static DoJoystickButton ClickButton1;
        public static DoJoystickButton ClickButton10;
        public static DoJoystickButton ClickButton2;
        public static DoJoystickButton ClickButton3;
        public static DoJoystickButton ClickButton4;
        public static DoJoystickButton ClickButton5;
        public static DoJoystickButton ClickButton6;
        public static DoJoystickButton ClickButton7;
        public static DoJoystickButton ClickButton8;
        public static DoJoystickButton ClickButton9;
        public static TLink CrossLink;
        public static DoKeys_CB DoKeys;

        //public static TQuickStc<TDummyAsteroid> DummyAsteroids = new TQuickStc<TDummyAsteroid>();
        public static EElementState[] ElementStates = (EElementState[])Enum.GetValues(typeof(EElementState));

        public static long FacetsNumber = 0;

        public static string FilesFilter =
            "All|*.xml;*.3dxml;*.jt;*.wrl;*.igs;*.dae;*.obj;*.stl;*.blend;*.ply;*.3ds;*.fbx;*.mdl;*.max,;*.zip;*.json;*.ciliasz;*.ciliaaz|" +
            "XML File|*.xml|" +
            "3DXml File|*.3dxml|" +
            "JT File|*.jt|" +
            "VRML File|*.wrl|" +
            "IGES File|*.igs|" +
            "Collada File|*.dae|" +
            "3D Studio File|*.3ds|" +
            "3DS Max File|*.max|" +
            "FilmBox File|*.fbx|" +
            "WaveFront File|*.obj|" +
            "StereoLithography File|*.stl|" +
            "Blender File|*.blend|" +
            "Polygon File|*.ply|" +
            "Vimosac File|*.mdl|" +
            "Json File|*.json|" +
            "ZIP file|*.zip|" +
            "Cilia Compressed Solid|*.ciliasz|" +
            "Cilia Compressed Assembly|*.ciliaaz";

        public static TQuickStc<TFile> FilesToload = new TQuickStc<TFile>();
        public static bool Focused = false;

        //private static TLink graphTreeLink;
        public static TLink GroundLink;

        //private static Mtx4 IconUnScaledMatrix = Mtx4.CreateScale(0.25, 0.25, 0.25);
        public static double IHeight;

        public static double IWidth;
        public static DateTime JoystickCheck;
        public static EKeybordModifiers KeyboardModifiers;
        public static DateTime LastJoystickCheck = DateTime.Now;
        public static double Left = 0;
        public static short LoadingCounter = 0;
        public static object LoadingCounterLocker = new object();
        public static string LogLine = ""; 
        public static DoJoystickAxis MoveAxis0;
        public static DoJoystickAxis MoveAxis1;
        public static DoJoystickAxis MoveAxis2;
        public static DoJoystickAxis MoveAxis3;
        public static TLink PendingAction = null;

        public static TPerformancesPanel performancesPanel;
        public static TThreadsPanel  threadsPanel;

        public static Mtx4? Proj;

        public static bool SetupDone;

     
   
        public static int SolidsNumber = 0;

        public static TQuickStc<TSolidElement> SolidsToBeGarbaged = new TQuickStc<TSolidElement>();

        public static TQuickStc<TSolidElement> SolidsToBePushed = new TQuickStc<TSolidElement>();

        public static TQuickStc<TSolidElement> SolidsToBeUpdated = new TQuickStc<TSolidElement>();

        //public static TLink Spider;
        public static TQuickStc<TSolidElement> TexturesToUpdate = new TQuickStc<TSolidElement>();

        public static TQuickStc<TLink> ToBeParsedLinks = new TQuickStc<TLink>();

        public static double Top = 0;

        public static int TotalFacets = 0;

        public static int TotalPositions = 0;

        public static TLink Univers;

        public static TQuickStc<TFile> UsedFiles = new TQuickStc<TFile>();

        public static bool[] UsedKeys;

        public static Mtx4 VIMatrix = Mtx4.Identity;

        //static public List<TConstraint> WorldConstraints = new List<TConstraint>();

        //static public List<TReference> WorldReferences = new List<TReference>();

        #endregion Public Fields

        #region Private Fields

        private static readonly double TranslationSpeed = -9e-2;

        private static TimeSpan AddedKeyTimeSpan = new TimeSpan(0, 0, 0, 0, 100);

        private static bool AddingKeys = false;

        private static TimeSpan AddingKeyTimeSpan = new TimeSpan(0, 0, 0, 0, 800);

        //private static DateTime BarsIconClicked = DateTime.Now;
        //private static Dictionary<TLink, TLink> BarsIcons = new Dictionary<TLink, TLink>();
        private static bool CenterMeMode = false;
         

        ////private static TSolidElement[] Characters;
        private static Thread CheckEntriesThread;
         

        private static Thread CheckOverFlyThread;
         

        private static ClearBufferMask ClearBufferMasks = ClearBufferMask.ColorBufferBit | ClearBufferMask.AccumBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit;

        private static Mtx4f ColorAllBlack;

        //private static Mtx4f ColorAllRed;
        private static Mtx4f ColorAllWhite;

        //private static Mtx4f ColorDisabled = Mtx4f.CreateOffsetColor(0.02F, 0.02F, 0.02F);
        //private static Mtx4f ColorFPS;
        //private static Mtx4f ColorFPSC;
        //private static Mtx4f ColorFPSG;
        //private static Mtx4f ColorGreen = Mtx4f.CreateOffsetColor(0, 1, 0);
        //private static Mtx4f ColorInError = Mtx4f.CreateOffsetColor(1, 0, 0);
        //private static Mtx4f ColorLoading;
        private static Mtx4f ColorMeasure;

        public static Mtx4f ColorNormal = Mtx4f.Identity;

        //private static Mtx4f ColorOrigin;
        public static Mtx4f ColorOverFlown = Mtx4f.CreateOffsetColor(0, 0.3F, 0.3F);

        //private static Mtx4f ColorRed = Mtx4f.CreateOffsetColor(1, 0, 0);
        public static Mtx4f ColorSelected = Mtx4f.CreateOffsetColor(0.3F, 0.15F, 0);

        //private static Mtx4f ColorSelectedNode = Mtx4f.CreateOffsetColor(1, 1, 0);
        //private static Mtx4f ColorTreeDark;
        //private static Mtx4f ColorTreeLight;
        //private static Mtx4f ColorValidated = Mtx4f.CreateOffsetColor(0, 1, 0);
        private static List<TCommand> Commands = new List<TCommand>();

        private static TLabel commandsPanel;

        //private static TLink commandsPanelLink;
        private static Thread ComputeThread;

        private static bool Ctrl;

        private static double CullingParameter = 2;

        private static Vec2 cursor;

        private static Mtx4 CursorMatrix;

        private static Vec3 CursorPoint;

        private static Thread DoerBuildingsThread;


        private static Thread DoerGarbageThread;

        private static Thread DoerLoadingsThread;

        private static Thread DoerPhysicsThread;

        private static bool DrawFaces;

        public static bool DrawIcons;

        private static bool DrawLines;

        private static bool DrawMeasures;

        private static bool DrawPoints;

        private static DrawSolidCB DrawSolid;

        private static bool EntitiesMoving;

        //private static Vec3 Eye = new Vec3(-10, -10, 1);
        private static int facetsNumber = 0;

        private static double FarRatio = 1000;

        private static object FlyingLocker = new object();

        private static Vec2 FlyingPoint;

        //private static double FrustrumXLimit = 1;
        //private static double FrustrumYLimit = 1;
        private static TQuickStc<Mtx4> GlyphePoints = new TQuickStc<Mtx4>();

        private static TTreeView graphTree;

        private static int HandleColorMatrix;

        private static int HandleFragmentShader;

        private static int HandleModelMatrix;
        private static int HandleViewMatrix;

        private static int HandleNoDiffuse;

        private static int HandleNoEffect;

        private static int HandleProjectionMatrix;

        private static int HandleShaderProgram;

        private static int HandleTextureOffset;

        private static int HandleVertexShader;

        private static double height = 10;

        public static double HeightCulling;

        private static int HeightInt;

        //private static double HeightIWidth;
        private static TButton iconMoveObjects;

        private static TButton iconMovingMode;

        //private static Mtx4 IconScaledMatrix = Mtx4.CreateScale(1.5, 1.5, 1.5);
        private static TSlider iconSetPerspective;

        private static TButton iconShowCoordinates;

        private static TButton iconShowFaces;

        private static TButton iconShowLines;

        private static TButton iconShowPerformances;

        private static TButton iconShowPoints;

        private static OpenTK.Input.JoystickState Joystick;

        private static DateTime[] JoystickButtons = new DateTime[16];

        private static bool JoystickTempCheck;

        private static Mtx4 JoystickTempMatrix;

        private static List<OpenTK.Input.Key> Keys;

        private static DateTime LastAddedKey = DateTime.Now;

        public static TLayer[] Layers;

        private static int LayersNumber = 16;

        private static bool LeftButton = false;

        private static TLinkPanel linkPanel;

        //private static TLink linkPanelLink;
        private static List<TLink> Links = new List<TLink>();

        private static TLabel loadingPanel;

        private static TLabel logsPanel;

        //private static TLink loadingPanelLink;
        private static Mtx4 MainRayMatrix = new Mtx4(0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0);

        private static int MaxLoadingFiles;

        private static bool MiddleButton = false;

        private static double MinNear = 0.001;

        private static OpenTK.Input.MouseState Mouse;

        private static ushort MouseTemporisation = 200;

        private static EMoveMode MoveMode;

        //private static Thread MoverSpiderThread;
        private static TSolidElement MoveSolid;

        private static Vec2 MovingCursor;

        private static Mtx4 MovingCursorMatrix;

        private static Vec3? MovingPoint;

        private static Vec3 MovingVector;

        private static int OverFlownIndex;

        //private static Vec3 Origin = new Vec3(0, 0, 0);
        //private static TLink OverFlownIconLink = null;
        private static TLink overFlownLink;

        private static TVectorPanel overflownPanel;

        //private static TLink overflownPanelLink;
        public static Vec3? OverFlownPoint;

        private static TShape OverFlownShape = null;

        private static Action OverFlyRecall = null;

        private static TLink OwnerLink;

        private static string pendingKeys = "";

        public static Mtx4? PendingViewPoint;

        //private static TLink performancesPanelLink;
        private static double perspectiveAngle = 0.52;

        private static bool PickerMode;

        private static TQuickStc<TLink> PickingStack = new TQuickStc<TLink>();

        //private static Mtx4 PProj;
        private static OpenTK.Input.JoystickState? PreviousJoystick;

        private static OpenTK.Input.MouseState? previousMouse;

        private static EMoveMode PreviousMoveMode;

        private static Vec2? previousTouch1;

        private static Vec2? previousTouch2;

        private static Mtx4 RayMatrix = new Mtx4(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0);

        private static T3DRepElement RepBBox;

        //private static T3DRepElement RepCollapse;
        private static T3DRepElement RepCursor;

        //private static T3DRepElement RepDisabled;
        //private static T3DRepElement RepEnabled;
        //private static T3DRepElement RepExpand;
        private static T3DRepElement RepFaceRotate;

        private static T3DRepElement RepFaceTranslate;

        private static T3DRepElement RepMeasureArrow;

        private static T3DRepElement RepMeasurePoint;

        private static T3DRepElement RepMovingPoint;

        private static T3DRepElement RepPicker;

        private static T3DRepElement RepSideRotate;

        private static T3DRepElement RepSideTranslate;

        private static T3DRepElement RepTarget;

        //private static T3DRepElement RepTreeBranch;
        //private static T3DRepElement RepTreeLine;
        //private static T3DRepElement RepTreeLink;
        //private static T3DRepElement RepTreeRoot;
        //private static T3DRepElement RepVisible;
        private static bool RightButton = false;

        private static double RotationSpeed = 1e-3;

        //private static TLink SelectedBarsIcon = null;
        private static TLayer SelectedLayer = null;

        private static List<TLink> selectedLinks = new List<TLink>();

        private static int solidsNumber = 0;

        //private static List<TLink> Spiders = new List<TLink>();
        //private static Dictionary<TReference, Vec4> SymbolsLocations = new Dictionary<TReference, Vec4>();

        //private static Rann.Systems.TOverConstrainedSystem Sys = new Rann.Systems.TOverConstrainedSystem();

        public static Vec3 Target = new Vec3(0, 0, 0);

        private static TVectorPanel targetPanel;

        //private static TLink targetPanelLink;
        //private static Mtx4f TextOverFlown = Mtx4f.CreateScaledTranslation(0.5F, 0, 0.8F, 0.8F);
        //private static Mtx4f TextSelected = Mtx4f.CreateScaledTranslation(0.5F, 1, 0.5F, 0);
        private static OpenTK.Input.TouchState Touch;

        private static Vec2? touch1;

        private static Vec3 Touch1Point;

        private static Vec2? touch2;

        private static Vec3 Touch2Point;

        private static Mtx4 TouchMatrix;

        private static Vec3 Up = new Vec3(0, 0, 1);

        private static Stack<TLink> UpdatedLinks = new Stack<TLink>();
         

        private static TVectorPanel viewerPanel;

        static Mtx4 vMatrix = Mtx4.Identity;
        public static Mtx4 VMatrix { get { return vMatrix; } set{ vMatrix = value; ViewMatrixChanged?.Invoke(value); } }

        public delegate void MatrixChangedCallBack(Mtx4 m);

        public static event MatrixChangedCallBack ViewMatrixChanged;

        private static double width = 10;

        public static double WidthCulling;

        public static double WidthIHeight;

        private static int WidthInt;

        private static double xRatio = 1;

        private static double yRatio = 1;

        #endregion Private Fields

        #region Public Delegates

        public delegate void DoJoystickAxis(TimeSpan dt, double f);

        public delegate void DoJoystickButton(OpenTK.Input.ButtonState b, double ms);

        public delegate void DoKeys_CB(TCommand cmd);

        #endregion Public Delegates

        #region Public Enums

        public enum ECommandState
        {
            Null, Success, Miss
        }

        #endregion Public Enums

        #region Private Methods

        private static void DoClickButton4(OpenTK.Input.ButtonState b, double ms)
        {
            if (b == OpenTK.Input.ButtonState.Released) return;
            double f = -0.5;
            JoystickTempCheck = true;
            f *= RotationSpeed * ms;
            JoystickTempMatrix = JoystickTempMatrix * Mtx4.CreateRotationZ(f);
        }

        private static void DoClickButton5(OpenTK.Input.ButtonState b, double ms)
        {
            if (b == OpenTK.Input.ButtonState.Released) return;
            double f = 0.5;
            JoystickTempCheck = true;
            f *= RotationSpeed * ms;
            JoystickTempMatrix = JoystickTempMatrix * Mtx4.CreateRotationZ(f);
        }

        private static void DoClickButton6(OpenTK.Input.ButtonState b, double ms)
        {
            if (b == OpenTK.Input.ButtonState.Released) return;
            double f = -0.5;
            JoystickTempCheck = true;
            f *= TranslationSpeed * ms;
            JoystickTempMatrix = Mtx4.CreateTranslation(JoystickTempMatrix.Row0.Y * f, JoystickTempMatrix.Row1.Y * f, JoystickTempMatrix.Row2.Y * f) * JoystickTempMatrix;
        }

        private static void DoClickButton7(OpenTK.Input.ButtonState b, double ms)
        {
            if (b == OpenTK.Input.ButtonState.Released) return;
            double f = +0.5;
            JoystickTempCheck = true;
            f *= TranslationSpeed * ms;
            JoystickTempMatrix = Mtx4.CreateTranslation(JoystickTempMatrix.Row0.Y * f, JoystickTempMatrix.Row1.Y * f, JoystickTempMatrix.Row2.Y * f) * JoystickTempMatrix;
        }

        private static void DoClickButtonNull(OpenTK.Input.ButtonState b, double ms)
        {
            if (b == OpenTK.Input.ButtonState.Released) return;
            else

            {
            }
        }

        #endregion Private Methods

        #region Public Classes

        public class TCommand
        {
            #region Public Fields

            public string Command;
            public ECommandState State;
            public string[] Words;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}