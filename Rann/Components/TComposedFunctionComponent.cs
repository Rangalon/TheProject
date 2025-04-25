using System;
using System.Collections.Generic;
using System.Linq;

namespace Rann.Components
{
    /// <summary>
    /// This class is used to manage mathematic function which involve two components, for which the order is important
    /// - Substraction (we will transform it in addition with multiplication by -1)
    /// - Exponent
    /// - Division
    /// </summary>
    /// <remarks></remarks>

    public class TComposedFunctionComponent : TComponent
    {
        #region Public Fields

        public readonly EComposedFunction Type;

        //public static new Font Ft = new Font(TComponent.Ft, Drawing.FontStyle.Bold);
        public TComponent MComponent;

    

        /// <summary>
        /// Font used for component drawing
        /// </summary>
        /// <remarks></remarks>
        public TComponent SComponent;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Constructor of the class
        /// The only way to provide main and sub component is through the constructor
        /// </summary>
        /// <param name="iType"></param>
        /// <param name="iMComponent"></param>
        /// <param name="iSComponent"></param>
        /// <remarks></remarks>
        public TComposedFunctionComponent(EComposedFunction iType, TComponent iMComponent, TComponent iSComponent)
        {
            Type = iType;
            MComponent = iMComponent;
            SComponent = iSComponent;
        }

        #endregion Public Constructors

        #region Public Properties

        public override byte Level
        {
            get
            {
                switch (Type)
                {
                    case EComposedFunction.Division: return 56;
                    case EComposedFunction.Substraction: return 54;
                    case EComposedFunction.Exponent: return 52;
                }
                return 50;
            }
        }

        public override double Position
        {
            get
            {
                double functionReturnValue = 0;
                switch (Type)
                {
                    case EComposedFunction.Division:
                        if (MComponent.Text == SComponent.Text)
                            functionReturnValue = 1;
                        else
                            functionReturnValue = MComponent.Position / SComponent.Position;
                        break;

                    case EComposedFunction.Exponent:
                        functionReturnValue = Math.Pow(MComponent.Position, SComponent.Position);
                        break;

                    case EComposedFunction.Substraction:
                        functionReturnValue = MComponent.Position - SComponent.Position;
                        break;

                    default:
                        functionReturnValue = 0;
                        break;
                }
                return functionReturnValue;
            }
            set { throw new Exception("Not Allowed!"); }
        }

        /// <summary>
        /// Conversion of the component to String
        /// We put all the time parenthesis, in order to ease the save and reload of the formula
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string Text
        {
            get { return "(" + MComponent.Text + TextComposedFunction(Type) + SComponent.Text + ")"; }
        }

        #endregion Public Properties

        #region Public Methods

        public static string TextComposedFunction(EComposedFunction iType)
        {
            switch (iType)
            {
                case EComposedFunction.Exponent:
                    return "^";

                case EComposedFunction.Substraction:
                    return "-";

                case EComposedFunction.Division:
                    return "/";

                default:
                    return iType.ToString();
            }
        }

        public override TComponent Factorises(TComponent factor)
        {
            switch (Type)
            {
                case EComposedFunction.Division:
                    return MComponent.Factorises(factor) / SComponent.Factorises(factor);

                case EComposedFunction.Exponent:
                    throw new Exception("not managed");
                case EComposedFunction.Substraction:
                    return MComponent.Factorises(factor) - SComponent.Factorises(factor);
            }
            throw new Exception("not managed");
        }

        /// <summary>
        /// Common mathematic rules are applied to compute the differential
        /// </summary>
        /// <param name="iParameter"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            TComponent functionReturnValue = default(TComponent);
            TComponent MDiff = MComponent.GetDifferential(iParameter);
            TComponent SDiff = SComponent.GetDifferential(iParameter);
            //
            switch (Type)
            {
                case EComposedFunction.Division:
                    functionReturnValue = MDiff / SComponent - MComponent * SDiff / (SComponent ^ 2);
                    break;

                case EComposedFunction.Exponent:
                    if (SComponent is TValueComponent val && val == 2)
                        functionReturnValue = 2 * MDiff * MComponent;
                    else
                        functionReturnValue = SComponent * MDiff * (MComponent ^ (SComponent - 1)) + SDiff * TComponent.Ln(MComponent) * (MComponent ^ SComponent);
                    break;

                case EComposedFunction.Substraction:
                    functionReturnValue = MDiff - SDiff;
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
            List<TComponent> lst = new List<TComponent>();
            switch (Type)
            {
                case EComposedFunction.Division:
                    lst.Add(this);
                    lst.AddRange(MComponent.GetFactors());
                    foreach (TComponent c in SComponent.GetFactors())
                        lst.Add(1 / c);
                    break;

                case EComposedFunction.Exponent:
                    break;

                case EComposedFunction.Substraction:
                    lst.Add(this);
                    List<TComponent> mlst = new List<TComponent>(MComponent.GetFactors());
                    List<TComponent> slst = new List<TComponent>(SComponent.GetFactors());
                    foreach (TComponent c in mlst.ToArray())
                    {
                        if (slst.Contains(c))
                        {
                            lst.AddRange(c.GetFactors());
                            slst.Remove(c);
                        }
                    }
                    break;
            }
            return lst.ToArray();
        }

        //public override TComponent Clone(Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    return new TComposedFunctionComponent(Type, MComponent.Clone(iParameters), SComponent.Clone(iParameters));
        //}
        /// <summary>
        /// Parameters of the component are coming from both main and sub components
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override TParameterComponent[] GetParameters()
        {
            TParameterComponent[] functionReturnValue = null;
            List<TParameterComponent> lst = new List<TParameterComponent>();
            foreach (TComponent component in new TComponent[2] { MComponent, SComponent })
            {
                foreach (TParameterComponent prmtr in component.GetParameters()) { if (!lst.Contains(prmtr)) lst.Add(prmtr); }
            }
            functionReturnValue = lst.ToArray();
            GC.SuppressFinalize(lst);
            return functionReturnValue;
        }

        //            functionReturnValue.Width += ParenthesisWidth * 2;
        //            break;
        //    }
        //    functionReturnValue.Height += 4;
        //    return functionReturnValue;
        //}
        public override TComponent Reduced()
        {
            TComponent functionreturnvalue = default(TComponent);
            functionreturnvalue = this;
            switch (Type)
            {
                case EComposedFunction.Division:
                    // basic reduction
                    if (MComponent == 0)
                        functionreturnvalue = 0;
                    if (SComponent == 1)
                        functionreturnvalue = MComponent;
                    // numeric reduction
                    if (MComponent is TValueComponent && SComponent is TValueComponent)
                    {
                        functionreturnvalue = new TValueComponent(Position);
                    }
                    if (MComponent.ToString() == SComponent.ToString()) return 1;
                    // Factorisation and simplification?
                    if (MComponent is TOperandComponent mm1 && mm1.Type == EOperand.Product && SComponent is TOperandComponent mm2 && mm2.Type == EOperand.Product)
                    {
                        List<TComponent> mlst = new List<TComponent>(mm1.Elements);
                        List<TComponent> slst = new List<TComponent>(mm2.Elements);

                        foreach (TComponent c1 in mlst.ToArray())
                        {
                            TComponent c2 = slst.FirstOrDefault(o => o.ToString() == c1.ToString());
                            if (c2 != null)

                            {
                                mlst.Remove(c1);
                                slst.Remove(c2);
                            }
                        }
                        mm1.Elements = mlst.ToArray();
                        mm2.Elements = slst.ToArray();
                    }
                    //
                    //List<TComponent> mlst = new List<TComponent>(MComponent.GetFactors());
                    //List<TComponent> slst = new List<TComponent>(SComponent.GetFactors());

                    //foreach (TComponent c in mlst.ToArray())
                    //{
                    //    if (slst.Contains(c))
                    //    {
                    //        MComponent = MComponent.Factorises(c);
                    //        SComponent = SComponent.Factorises(c);
                    //        slst.Remove(c);
                    //    }
                    //}
                    break;

                case EComposedFunction.Exponent:
                    if (MComponent == 0)
                        functionreturnvalue = 0;
                    if (MComponent == 1)
                        functionreturnvalue = 1;
                    if (SComponent == 0)
                        functionreturnvalue = 1;
                    if (SComponent == 1)
                        functionreturnvalue = MComponent;
                    // numeric reduction
                    if (MComponent is TValueComponent && SComponent is TValueComponent)
                    {
                        functionreturnvalue = new TValueComponent(Position);
                    }

                    break;

                case EComposedFunction.Substraction:
                    if (MComponent.ToString() == SComponent.ToString()) return 0;
                    if (MComponent is TOperandComponent omcomp && omcomp.Type == EOperand.Sum)
                    {
                        if (omcomp.Elements.Contains(SComponent))
                        {
                            List<TComponent> lst = omcomp.Elements.ToList(); lst.Remove(SComponent);
                            TComponent s = 0;
                            foreach (TComponent c in lst)
                                s += c;
                            return s;
                        }
                        else if (SComponent is TOperandComponent oscomp && oscomp.Type == EOperand.Sum)
                        {
                            List<TComponent> omlst = omcomp.Elements.ToList();
                            List<TComponent> oslst = oscomp.Elements.ToList();
                            bool b = false;
                            foreach (TComponent c in omlst.ToArray())
                            {
                                if (oslst.Contains(c))
                                {
                                    omlst.Remove(c);
                                    oslst.Remove(c);
                                    b = true;
                                }
                            }
                            if (b)
                            {
                                TComponent om = 0;
                                foreach (TComponent c in omlst) om += c;
                                TComponent os = 0;
                                foreach (TComponent c in oslst) os += c;
                                return om - os;
                            }
                        }
                    }
                    else if (MComponent is TOperandComponent m1 && m1.Type == EOperand.Product && SComponent is TOperandComponent s1 && s1.Type == EOperand.Product)
                    {
                        foreach (TComponent cm in m1.Elements)
                            foreach (TComponent cs in s1.Elements)
                                if (cs.Text == cm.Text)
                                {
                                    TComponent nm = 1;
                                    foreach (TComponent ncm in m1.Elements.Where(o => o != cm))
                                        nm *= ncm;
                                    TComponent ns = 1;
                                    foreach (TComponent ncs in s1.Elements.Where(o => o != cs))
                                        ns *= ncs;
                                    return (nm - ns) * cm;
                                }
                    }

                    break;
            }

            return functionreturnvalue;
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
            if (MComponent == oldComp) MComponent = newComp; else MComponent.Replace(oldComp, newComp);
            if (SComponent == oldComp) SComponent = newComp; else SComponent.Replace(oldComp, newComp);
        }

        /// <summary>
        /// Kind of composed function
        /// </summary>
        /// <remarks></remarks>
        ///
        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            if (this.ToString() == iOldComp.ToString()) return iNewComp;
            TComponent mcomp = MComponent.ReplaceComponent(iOldComp, iNewComp);
            TComponent scomp = SComponent.ReplaceComponent(iOldComp, iNewComp);
            switch (Type)
            {
                case EComposedFunction.Division:
                    return mcomp / scomp;

                case EComposedFunction.Exponent:
                    return mcomp ^ scomp;

                case EComposedFunction.Substraction:
                    return mcomp - scomp;
            }
            return null;
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                MComponent = null;
                SComponent = null;
            }
            base.Dispose(disposing);
        }

        #endregion Protected Methods

        ///// <summary>
        ///// TryParse which can create the class
        ///// </summary>
        ///// <param name="iText"></param>
        ///// <param name="iComponent"></param>
        ///// <param name="iParameters"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public static new Boolean TryParse(String iText, ref TComponent iComponent, Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    // We will try with each type of composed function
        //    foreach (EComposedFunction Type in Enum.GetValues(typeof(EComposedFunction)))
        //    {
        //        // We will try to find some fields separated by the corresponding separator
        //        TComponent[] table = SplitField(iText, TextComposedFunction[Type].Trim, iParameters);
        //        // If no field, it's a loss
        //        if (table == null)
        //            continue;
        //        // If less or more than 2 fields, it's a loss
        //        if (table.Length != 2)
        //            continue;
        //        // It's a win!
        //        if (Type == EComposedFunction.Substraction)
        //        {
        //            // Special parse to not use substration
        //            iComponent = table(0) + (MinusOne * table(1));
        //        }
        //        else
        //        {
        //            // Special parse to not use substration
        //            iComponent = new TComposedFunctionComponent(Type, table(0), table(1));
        //        }
        //        // No need to continue
        //        break; // TODO: might not be correct. Was : Exit For
        //    }
        //    return iComponent != null;
        //}
        ///// <summary>
        ///// Special way to draw division, in order to have a more clear drawing
        ///// </summary>
        ///// <param name="iGrp"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public override System.Drawing.SizeF GetDrawingSize(Graphics iGrp)
        //{
        //    System.Drawing.SizeF functionReturnValue = default(System.Drawing.SizeF);
        //    switch (Type) {
        //        case EComposedFunction.Division:
        //            functionReturnValue = new SizeF();
        //            foreach (SizeF szF in {
        //                MComponent.GetDrawingSize(iGrp),
        //                new SizeF(3, 3),
        //                SComponent.GetDrawingSize(iGrp)
        //            }) {
        //                if (functionReturnValue.Height < szF.Height * 2 + 3)
        //                    functionReturnValue.Height = szF.Height * 2 + 3;
        //                if (functionReturnValue.Width < szF.Width)
        //                    functionReturnValue.Width = szF.Width;
        //            }

        //            break;
        //        case EComposedFunction.Exponent:
        //            functionReturnValue = new SizeF();
        //            foreach (SizeF szF in {
        //                MComponent.GetDrawingSize(iGrp),
        //                iGrp.MeasureString(TextComposedFunction[Type], Ft),
        //                SComponent.GetDrawingSize(iGrp)
        //            }) {
        //                functionReturnValue.Width += szF.Width;
        //                if (functionReturnValue.Height < szF.Height)
        //                    functionReturnValue.Height = szF.Height;
        //            }
        //private TComponent ReduceSimplify()
        //{
        //    switch (true)
        //    {
        //        case MComponent is TOperandComponent && ((TOperandComponent)MComponent).Type == EOperand.Product && SComponent is TOperandComponent && ((TOperandComponent)MComponent).Type == EOperand.Product:
        //            break;
        //        // Both are products
        //        case MComponent is TOperandComponent && ((TOperandComponent)MComponent).Type == EOperand.Product:
        //            // Main component only is a product
        //            TComponent prd = One;
        //            TComponent sComp = SComponent;
        //            foreach (TComponent comp in ((TOperandComponent)MComponent).Elements)
        //            {
        //                if ((sComp != null) && comp.Text == sComp.Text)
        //                {
        //                    sComp = null;
        //                }
        //                else
        //                {
        //                    prd *= comp;
        //                }
        //            }

        //            if (sComp == null)
        //                return prd;
        //            break;
        //        case SComponent is TOperandComponent && ((TOperandComponent)MComponent).Type == EOperand.Product:
        //            break;
        //        // Sub component only is a product
        //        default:
        //            // None is a product
        //            if (MComponent.Text == SComponent.Text)
        //                return One;
        //            break;
        //    }
        //    return this;
        //}

        ///// <summary>
        ///// Special way to draw division
        ///// </summary>
        ///// <param name="iGrp"></param>
        ///// <remarks></remarks>
        //public override void DrawContent(System.Drawing.Graphics iGrp)
        //{
        //    base.DrawContent(iGrp);
        //    SizeF szF = GetDrawingSize(iGrp);
        //    SizeF sz = default(SizeF);
        //    //
        //    switch (Type) {
        //        case EComposedFunction.Division:
        //            sz = MComponent.GetDrawingSize(iGrp);
        //            iGrp.TranslateTransform(szF.Width / 2 - sz.Width / 2, (szF.Height - 3) / 2 - sz.Height);
        //            MComponent.DrawContent(iGrp);
        //            iGrp.TranslateTransform(sz.Width / 2 - szF.Width / 2, sz.Height + 2);
        //            //
        //            iGrp.DrawLine(Pens.Black, 0, 0, szF.Width, 0);
        //            iGrp.TranslateTransform(0, 1);
        //            //
        //            sz = SComponent.GetDrawingSize(iGrp);
        //            iGrp.TranslateTransform(szF.Width / 2 - sz.Width / 2, 0);
        //            SComponent.DrawContent(iGrp);
        //            iGrp.TranslateTransform(sz.Width / 2 - szF.Width / 2, (szF.Height - 3) / 2);
        //            //
        //            iGrp.TranslateTransform(0, -szF.Height);
        //            break;
        //        case EComposedFunction.Exponent:
        //            DrawLeftParenthesis(iGrp, szF.Height);
        //            //
        //            foreach (TComponent comp in {
        //                MComponent,
        //                SComponent
        //            }) {
        //                //
        //                if ((!object.ReferenceEquals(comp, MComponent))) {
        //                    sz = iGrp.MeasureString(TextComposedFunction[Type], Ft);
        //                    iGrp.TranslateTransform(0, szF.Height / 2 - sz.Height / 2);
        //                    iGrp.DrawString(TextComposedFunction[Type], Ft, Brushes.DarkGreen, 0, 0);
        //                    iGrp.TranslateTransform(sz.Width, sz.Height / 2 - szF.Height / 2);
        //                }
        //                //
        //                sz = comp.GetDrawingSize(iGrp);
        //                iGrp.TranslateTransform(0, szF.Height / 2 - sz.Height / 2);
        //                comp.DrawContent(iGrp);
        //                iGrp.TranslateTransform(sz.Width, sz.Height / 2 - szF.Height / 2);
        //            }

        //            //
        //            DrawRightParenthesis(iGrp, szF.Height);
        //            //
        //            iGrp.TranslateTransform(-szF.Width, 0);
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}