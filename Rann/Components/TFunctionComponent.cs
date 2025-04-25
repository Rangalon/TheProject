using System;

namespace Rann.Components
{
    /// <summary>
    /// This class is used to managed all mathematic functin which requires only one component
    /// - Trigonometric functions
    /// - Hyperbolic functions
    /// - Exponent and logarithm
    /// </summary>
    /// <remarks></remarks>

    public class TFunctionComponent : TComponent
    {
        #region Public Fields

        public readonly EFunction Type;
        public TComponent Component;

        #endregion Public Fields

        #region Public Constructors

        public TFunctionComponent(EFunction iType, TComponent iComponent)
        {
            Type = iType;
            Component = iComponent;
        }

        #endregion Public Constructors

        #region Public Properties

        public override byte Level => 30;

        public override double Position
        {
            get
            {
                double functionReturnValue = 0;
                switch (Type)
                {
                    case EFunction.Cosh:
                        functionReturnValue = Math.Cosh(Component.Position);
                        break;

                    case EFunction.Sinh:
                        functionReturnValue = Math.Sinh(Component.Position);
                        break;

                    case EFunction.Tanh:
                        functionReturnValue = Math.Tanh(Component.Position);
                        break;

                    case EFunction.Coth:
                        functionReturnValue = 1 / Math.Tanh(Component.Position);
                        break;

                    case EFunction.Cos:
                        functionReturnValue = Math.Cos(Component.Position);
                        break;

                    case EFunction.ACos:
                        functionReturnValue = Math.Acos(Component.Position);
                        break;

                    case EFunction.Sin:
                        functionReturnValue = Math.Sin(Component.Position);
                        break;

                    case EFunction.ASin:
                        functionReturnValue = Math.Asin(Component.Position);
                        break;

                    case EFunction.Tan:
                        functionReturnValue = Math.Tan(Component.Position);
                        break;

                    case EFunction.ATan:
                        functionReturnValue = Math.Atan(Component.Position);
                        break;

                    case EFunction.Cot:
                        functionReturnValue = 1 / Math.Tan(Component.Position);
                        break;

                    case EFunction.ACot:
                        functionReturnValue = Math.Atan(1 / Component.Position);
                        break;

                    case EFunction.Exp:
                        functionReturnValue = Math.Exp(Component.Position);
                        break;

                    case EFunction.Ln:
                        functionReturnValue = Math.Log(Component.Position);
                        break;

                    case EFunction.Sqrt:
                        functionReturnValue = Math.Sqrt(Component.Position);
                        break;

                    default:
                        functionReturnValue = 0;
                        break;
                }
                return functionReturnValue;
            }
            set { throw new Exception("Not Allowed!"); }
        }

        public override string Text
        {
            get { return TextFunctionPrefix(Type) + Component.Text + TextFunctionSuffix(Type); }
        }

        #endregion Public Properties

        #region Public Methods

        public static string TextFunctionPrefix(EFunction iType)
        {
            switch (iType)
            {
                case EFunction.Exp:
                    return "e^(";

                default:
                    return iType.ToString().ToLower() + "(";
            }
        }

        public static string TextFunctionSuffix(EFunction iType)
        {
            switch (iType)
            {
                default:
                    return ")";
            }
        }

        public override TComponent Factorises(TComponent factor)
        {
            throw new Exception("not managed");
        }

        //public override void DrawContent(System.Drawing.Graphics iGrp)
        //{
        //    base.DrawContent(iGrp);
        //    SizeF szF = GetDrawingSize(iGrp);
        //    SizeF sz = default(SizeF);
        //    //
        //    sz = iGrp.MeasureString(TextFunctionPrefix[Type].Replace("(", ""), Ft);
        //    iGrp.TranslateTransform(0, szF.Height / 2 - sz.Height / 2);
        //    iGrp.DrawString(TextFunctionPrefix[Type].Replace("(", ""), Ft, Brushes.DarkBlue, 0, 0);
        //    iGrp.TranslateTransform(sz.Width, sz.Height / 2 - szF.Height / 2);
        //    //
        //    DrawLeftParenthesis(iGrp, szF.Height);
        //    //
        //    sz = Component.GetDrawingSize(iGrp);
        //    iGrp.TranslateTransform(0, szF.Height / 2 - sz.Height / 2);
        //    Component.DrawContent(iGrp);
        //    iGrp.TranslateTransform(sz.Width, sz.Height / 2 - szF.Height / 2);
        //    //
        //    DrawRightParenthesis(iGrp, szF.Height);
        //    //
        //    iGrp.TranslateTransform(-szF.Width, 0);
        //}
        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            TComponent functionReturnValue = default(TComponent);
            TComponent Diff = Component.GetDifferential(iParameter);
            if (Diff == null)
            {
            }
            //
            switch (Type)
            {
                case EFunction.Cosh:
                    functionReturnValue = Diff * Sinh(Component);
                    break;
                //Case EFunction.ACosh
                //    GetDifferential = Diff / (((Component ^ Two) - One) ^ Half)
                case EFunction.Sinh:
                    functionReturnValue = Diff * Cosh(Component);
                    break;
                //Case EFunction.ASinh
                //    GetDifferential = Diff / (((Component ^ Two) + One) ^ Half)
                case EFunction.Tanh:
                    functionReturnValue = Diff / (Cosh(Component) ^ 2);
                    break;
                //Case EFunction.ATanh
                //    GetDifferential = Diff / (One - Component ^ Two)
                case EFunction.Coth:
                    functionReturnValue = -Diff / (Sinh(Component) ^ 2);
                    break;
                //Case EFunction.ACoth
                //    GetDifferential = Diff / (One - Component ^ Two)
                case EFunction.Cos:
                    functionReturnValue = -Diff * Sin(Component);
                    break;

                case EFunction.ACos:
                    functionReturnValue = -Diff / ((1 - (Component ^ 2)) ^ 0.5);
                    break;

                case EFunction.Sin:
                    functionReturnValue = Diff * Cos(Component);
                    break;

                case EFunction.ASin:
                    functionReturnValue = Diff / ((1 - (Component ^ 2)) ^ 0.5);
                    break;

                case EFunction.Tan:
                    functionReturnValue = Diff / (Cos(Component) ^ 2);
                    break;

                case EFunction.ATan:
                    functionReturnValue = Diff / (1 + (Component ^ 2));
                    break;

                case EFunction.Cot:
                    functionReturnValue = -Diff / (Sin(Component) ^ 2);
                    break;

                case EFunction.ACot:
                    functionReturnValue = -Diff / (1 + (Component ^ 2));
                    break;

                case EFunction.Exp:
                    functionReturnValue = Diff * Exp(Component);
                    break;

                case EFunction.Ln:
                    functionReturnValue = Diff / Component;
                    break;

                case EFunction.Sqrt:
                    functionReturnValue = Diff * 0.5 * (Component ^ -0.5);
                    break;

                default:
                    functionReturnValue = null;
                    break;
            }
            return functionReturnValue;
            //
        }

        public override TComponent[] GetFactors()
        {
            return new TComponent[] { this };
        }

        //public override TComponent Clone(Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    return new TFunctionComponent(Type, Component.Clone(iParameters));
        //}
        public override TParameterComponent[] GetParameters()
        {
            return Component.GetParameters();
        }

        //                break;
        //        }
        //    }
        //    return iComponent != null;
        //}
        //public override System.Drawing.SizeF GetDrawingSize(Graphics iGrp)
        //{
        //    System.Drawing.SizeF functionReturnValue = default(System.Drawing.SizeF);
        //    functionReturnValue = new SizeF();
        //    foreach (SizeF szF in {
        //        iGrp.MeasureString(TextFunctionPrefix[Type].Replace("(", ""), Ft),
        //        Component.GetDrawingSize(iGrp)
        //    }) {
        //        functionReturnValue.Width += szF.Width;
        //        if (functionReturnValue.Height < szF.Height)
        //            functionReturnValue.Height = szF.Height;
        //    }
        //    functionReturnValue.Width += ParenthesisWidth * 2;
        //    functionReturnValue.Height += 4;
        //    return functionReturnValue;
        //}
        public override TComponent Reduced()
        {
            if (Component is TComposedFunctionComponent comp)
            {
                if (comp.Type == EComposedFunction.Substraction)
                {
                    if (Type == EFunction.Cos)
                        return Cos(comp.MComponent) * Cos(comp.SComponent) + Sin(comp.MComponent) * Sin(comp.SComponent);
                    else if (Type == EFunction.Sin)
                        return Sin(comp.MComponent) * Cos(comp.SComponent) - Cos(comp.MComponent) * Sin(comp.SComponent);
                }
            }
            else if (Component is TOperandComponent oper)
            {
                if (oper.Type == EOperand.Product)
                {
                    if (oper.Elements[0] is TValueComponent val && val.Position < 0)
                    {
                        TComponent nval = new TValueComponent(-val.Position);
                        for (int i = 1; i < oper.Elements.Length; i++)
                            nval *= oper.Elements[i];
                        if (Type == EFunction.Cos)
                            return Cos(nval);
                        else if (Type == EFunction.Sin)
                            return -Sin(nval);
                    }
                }
            }
            if (Component is TValueComponent) return new TValueComponent(Position);
            return this;
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
            if (Component == oldComp) Component = newComp; else Component.Replace(oldComp, newComp);
        }

        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            if (this.ToString() == iOldComp.ToString()) return iNewComp;
            return new TFunctionComponent(Type, Component.ReplaceComponent(iOldComp, iNewComp));
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Component = null;
            }
            base.Dispose(disposing);
        }

        #endregion Protected Methods


       
        //public static new Boolean TryParse(String iText, ref TFunctionComponent iComponent, Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    // We will try with each kind of function
        //    foreach (EFunction Type in Enum.GetValues(typeof(EFunction)))
        //    {
        //        // I know that you do not like "select case true"... but i do
        //        // The iText must start with the prefix of the function
        //        // The iText must end with the suffix of the function
        //        // The field inside the parenthesis must be a component
        //        switch (true)
        //        {
        //            case !iText.ToUpper.StartsWith(TextFunctionPrefix[Type].ToUpper):
        //                break;
        //            case !iText.ToUpper.EndsWith(TextFunctionSuffix[Type].ToUpper):
        //                break;
        //            default:
        //                TComponent comp = null;
        //                if (!TComponent.TryParse(iText.Substring(TextFunctionPrefix[Type].Length, iText.Length - TextFunctionPrefix[Type].Length - TextFunctionSuffix[Type].Length), comp, iParameters))
        //                    continue;
        //                // it's a win
        //                iComponent = new TFunctionComponent(Type, comp);
        //                // No need to continue
        //                break; // TODO: might not be correct. Was : Exit For
    }
}