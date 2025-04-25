using CiliaElements.Format3DXml;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements.Elements.Internals
{
    public abstract class TDigit
    {

        #region Public Fields

        //public static Dictionary<int, Mtx4[]> Tables = new Dictionary<int, Mtx4[]>();
        public static T3DRepElement[] Characters;

        public static Mtx4f ColorAllBlack;
        public static Mtx4f ColorAllWhite;
        public static Mtx4f ColorFPS;
        public static Mtx4f ColorFPSC;
        public static Mtx4f ColorFPSG;
        public static Mtx4f ColorLoading;
        public static Mtx4f ColorMeasure;
        public static Mtx4f ColorOrigin;
        public static Mtx4f ColorOverfly;
        public static Mtx4f ColorReversed;
        public static Mtx4f ColorTarget;
        public static Mtx4f ColorTreeDark;
        public static Mtx4f ColorTreeLight;

        public static Mtx4 Digit2XIMatrix;
        public static Mtx4 Digit2XMatrix;
        public static Mtx4 Digit3ZMatrix;
        public static Mtx4 Digit4ZMatrix;
        public static Mtx4 DigithXMatrix;
        public static Mtx4 DigithZMatrix;
        public static Mtx4 DigithZXMatrix;
        public static Mtx4 DigitXIMatrix;
        public static Mtx4 DigitXIZIMatrix;
        public static Mtx4 DigitXMatrix;
        public static Mtx4 DigitZIMatrix;
        public static Mtx4 DigitZMatrix;
        public static Mtx4 DigitZXIMatrix;
        public static Mtx4 DigitZXMatrix;
        public static Mtx4 FileBarOffset;

        public static Mtx4 TargetOffset;
        public static Mtx4 TreeOffset;
        public static Mtx4 TreePosition;
        public static Mtx4 ViewBarOffset;
        public static Mtx4 ViewerOffset;

        #endregion Public Fields

        #region Public Constructors

        static TDigit()
        {
            TreePosition = Mtx4.Identity;
            ColorAllBlack = new Mtx4f() { Row3 = new Vec4f(0, 0, 0, 1F) };
            ColorAllWhite = new Mtx4f() { Row3 = new Vec4f(1, 1, 1, 1F) };
            ColorFPS = new Mtx4f(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 1F);
            ColorFPSC = new Mtx4f(1, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 1F);
            ColorFPSG = new Mtx4f(0.3F, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 1F);
            ColorLoading = new Mtx4f(0.3F, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1F);
            ColorMeasure = new Mtx4f(0.1F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 0.1F, 0, 0, 0, 0, 1F);
            ColorOrigin = new Mtx4f(0.9F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 0.1F, 0, 0, 0, 0, 1F);
            ColorOverfly = new Mtx4f(0.1F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 1F);
            ColorReversed = new Mtx4f(-1, 0, 0, 0, 0, -1, 0, 0, 0, 0, -1, 0, 1, 1, 1, 1F);
            ColorTarget = new Mtx4f(0.9F, 0, 0, 0, 0, 0.1F, 0, 0, 0, 0, 0.9F, 0, 0, 0, 0, 1F);
            ColorTreeDark = new Mtx4f(0.4F, 0, 0, 0, 0, 0.4F, 0, 0, 0, 0, 0.1F, 0, 0, 0, 0, 1F);
            ColorTreeLight = new Mtx4f(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0.3F, 0, 0, 0, 0, 1F);
            Characters = new T3DRepElement[256];
            DigitXMatrix = Mtx4.CreateTranslation(-0.15, 0, 0);
            Digit2XMatrix = DigitXMatrix * DigitXMatrix;
            DigitXIMatrix = Mtx4.CreateTranslation(0.15, 0, 0);
            Digit2XIMatrix = DigitXIMatrix * DigitXIMatrix;
            DigithXMatrix = Mtx4.CreateTranslation(-0.05F, 0, 0F);
            DigitZMatrix = Mtx4.CreateTranslation(0, 0, -0.3F);
            Digit3ZMatrix = DigitZMatrix * DigitZMatrix * DigitZMatrix;
            Digit4ZMatrix = Digit3ZMatrix * DigitZMatrix;
            DigithZMatrix = Mtx4.CreateTranslation(0, 0, -0.15F);
            DigitZIMatrix = Mtx4.CreateTranslation(0, 0, 0.3F);
            DigithZXMatrix = DigithZMatrix * DigitXMatrix;
            DigitZXMatrix = DigitZMatrix * DigitXMatrix;
            DigitZXIMatrix = DigitZMatrix * DigitXIMatrix;
            DigitXIZIMatrix = DigitXIMatrix * DigitZIMatrix;
            ViewBarOffset = TDigit.DigitZIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix;
            FileBarOffset = TDigit.DigitZMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix;
            TreeOffset = TDigit.DigitZMatrix * TDigit.DigitZMatrix * TDigit.DigitZMatrix * TDigit.DigitZMatrix;
            ViewerOffset = TDigit.DigithZMatrix;
            TargetOffset = TDigit.DigithZMatrix
                * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix
                * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix
                * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix
                * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix
                * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix * TDigit.DigitXIMatrix;
            //

        }

        #endregion Public Constructors

    }
}