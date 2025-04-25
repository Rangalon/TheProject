using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements
{
    public abstract class TViewSettings
    {

        #region Public Constructors

        static TViewSettings()
        {
            DrawFaces = true;
            DrawLines = true;
            DrawPoints = true;
            DrawIcons = true;
            DrawMeasures = true;
            DrawInformation = true;
            PreviousMoveMode = EMoveMode.Translation;
            DefineDrawFunction();
        }

        #endregion Public Constructors

        #region Public Properties

        public static Boolean DrawAnnotations { get; private set; }
        public static Boolean DrawFaces { get; private set; }
        public static Boolean DrawIcons { get; private set; }
        public static Boolean DrawLines { get; private set; }
        public static Boolean DrawMeasures { get; private set; }
        public static Boolean DrawInformation { get; private set; }
        public static Boolean DrawPoints { get; private set; }
        public static Boolean DrawTree { get; private set; }
        public static Boolean EntitiesMoving { get; private set; }

        public static EMoveMode PreviousMoveMode { get; private set; }


        static public bool PickerMode { get; private set; }


        #endregion Public Properties

        #region Public Methods

        public static void SwitchDrawAnnotations() { DrawAnnotations = !DrawAnnotations; }

        public static void SwitchDrawFaces() { DrawFaces = !DrawFaces; DefineDrawFunction(); }

        public static void SwitchDrawIcons() { DrawIcons = !DrawIcons; }

        public static void SwitchDrawLines() { DrawLines = !DrawLines; DefineDrawFunction(); }

        public static void SwitchDrawMeasures() { DrawMeasures = !DrawMeasures; }

        public static void SwitchDrawPerformances() { DrawInformation = !DrawInformation; }
        public static void SwitchDrawPoints() { DrawPoints = !DrawPoints; DefineDrawFunction(); }

        public static void SwitchDrawTree() { DrawTree = !DrawTree; }

        static void DefineDrawFunction()
        {
            if (DrawFaces)
            {
                if (DrawLines) { if (DrawPoints) TManager.DrawSolid = TManager.Draw7; else TManager.DrawSolid = TManager.Draw6; }
                else { if (DrawPoints) TManager.DrawSolid = TManager.Draw5; else TManager.DrawSolid = TManager.Draw4; }
            }
            else
            {
                if (DrawLines) { if (DrawPoints) TManager.DrawSolid = TManager.Draw3; else TManager.DrawSolid = TManager.Draw2; }
                else { if (DrawPoints) TManager.DrawSolid = TManager.Draw1; else TManager.DrawSolid = TManager.Draw0; }
            }
        }

        public static TSolidElement MoveSolid;


        public static void SwitchEntitiesMoving()
        {
            EntitiesMoving = !EntitiesMoving;
            if (EntitiesMoving)
                TManager.OwnerLink.SaveLocation();
            else
                TManager.OwnerLink.RestoreLocation();
        }
        public static void SwitchMoveMode()
        {
            if (PreviousMoveMode == EMoveMode.Rotation)
                PreviousMoveMode = EMoveMode.Translation;
            else
                PreviousMoveMode = EMoveMode.Rotation;
        }

        public static void IncreaseCulling()
        {
            TManager.CullingParameter *= 1.2;
            TManager.UpdateCulling();
        }

        public static void DecreaseCulling()
        {
            TManager.CullingParameter /= 1.2;
            TManager.UpdateCulling();
        }

        #endregion Public Methods

        internal static void SwitchPickerMode()
        {
            PickerMode = !PickerMode;
        }
    }

}
