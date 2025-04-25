using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rann.Components
{
    public struct SDisplayArea
    {
        #region Public Fields

        public float H;
        public float W;
        public float X;
        public float Y;

        #endregion Public Fields

        #region Public Properties

        public float B { get => Y + H; }
        public float L { get => X; }
        public float R { get => X + W; }
        public float T { get => Y; }

        #endregion Public Properties
    }

    public abstract class TComponent : IDisposable, IReplacement
    {
        #region Public Fields

        public static List<TComponent> AllComponents = new List<TComponent>();
        public static TValueComponent HalfPI = new TValueComponent(Math.PI / 2);
        public static TValueComponent MinusOne = new TValueComponent(-1);
        public static TValueComponent One = new TValueComponent(1);
        public static TValueComponent PI = new TValueComponent(Math.PI);
        public static TValueComponent Zero = new TValueComponent(0);

        #endregion Public Fields

        #region Internal Fields

        [XmlIgnore]
        internal bool disposedValue;

        #endregion Internal Fields

        #region Private Fields

        private static readonly char[] Separators = { '(', ')', ',', '+', '-', '*', '/' };

        #endregion Private Fields

        #region Public Properties

        public abstract byte Level { get; }

        static int TmpId = 0;
        static object TmpLocker = new object();

        public static TParameterComponent CreateTmpParameter()
        {
            int id;
            lock (TmpLocker)
            {
                id = TmpId;
                TmpId++;
            }
            return new TParameterComponent("Tmp", id.ToString());
        }

        public static TComponent Factorize(TComponent rFormula, TParameterComponent alp)
        {
            if (!rFormula.GetParameters().Contains(alp)) return rFormula;
            if (rFormula is TOperandComponent oc)
            {
                TComponent[] lst = oc.Elements.Where(o => o.GetParameters().Contains(alp)).ToArray();
            }
            return rFormula;
        }

        public static void Solve(TParameterComponent hei, TParameterComponent xl)
        {
            if (xl.RFormula == null) return;

            TComponent left = xl;
            TComponent right = xl.RFormula;

            while (right != hei)// right.GetParameters().Contains(hei))
            {
                if (right is TComposedFunctionComponent cfc)
                {
                    switch (cfc.Type)
                    {
                        case EComposedFunction.Division:
                            {
                                if (cfc.MComponent.GetParameters().Contains(hei))
                                {
                                    if (cfc.SComponent.GetParameters().Contains(hei))
                                    {
                                        throw new Exception("not implemented");
                                    }
                                    else
                                    {
                                        right = cfc.MComponent;
                                        left *= cfc.SComponent;
                                    }
                                }
                                else
                                {
                                    throw new Exception("not implemented");
                                }
                            }
                            break;
                        default:
                            throw new Exception("not implemented");
                    }

                    //    }
                }
                else if (right is TOperandComponent oc)
                {
                    TComponent[] cmps = oc.Elements.Where(o => o.GetParameters().Contains(hei)).ToArray();
                    TComponent[] others = oc.Elements.Where(o => !o.GetParameters().Contains(hei)).ToArray();
                    switch (oc.Type)
                    {
                        case EOperand.Product:
                            switch (cmps.Length)
                            {
                                case 0: return;
                                case 1:
                                    {
                                        TParameterComponent p = CreateTmpParameter();
                                        right = cmps[0];
                                        foreach (TComponent cc in others)
                                            left /= cc;
                                    }
                                    break;
                                default:
                                    throw new Exception("not implemented");
                            }
                            break;
                        case EOperand.Sum:
                            switch (cmps.Length)
                            {
                                case 0: return;
                                case 1:
                                    {
                                        TParameterComponent p = CreateTmpParameter();
                                        right = cmps[0];
                                        foreach (TComponent cc in others)
                                            left -= cc;
                                    }
                                    break;
                                default:
                                    throw new Exception("not implemented");
                            }
                            break;
                        default:
                            throw new Exception("not implemented");
                    }
                }
                else
                {
                    throw new Exception("not implemented");
                }
            }
            hei.RFormula = left;
            xl.RFormula = null;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public abstract double Position { get; set; }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public abstract string Text { get; }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent ACos(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.ACos, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent ACot(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.ACot, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent ASin(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.ASin, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent ATan(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.ATan, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Cos(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Cos, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Cosh(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Cosh, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Cot(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Cot, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Coth(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Coth, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Exp(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Exp, iComponent).Reduced();
        }

        public static TParameterComponent GetParameter(string name, string subname)
        {
            TParameterComponent p = TParameterComponent.AllParameters.FirstOrDefault(o => o.Name == name && o.SubName == subname);
            if (p == null)
            {
                p = new TParameterComponent(name, subname);
            }
            return p;
        }

        public static TParameterComponent GetParameter(string name)
        {
            string[] wds = name.Split(':');
            if (wds.Length == 2) return GetParameter(wds[0], wds[1]);
            else return GetParameter(name, "");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TValueComponent GetValueComponent(double d)
        {
            if (d == 0)
                return Zero;
            else if (d == 1)
                return One;
            else if (d == -1)
                return MinusOne;
            else
                return new TValueComponent(d);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static implicit operator TComponent(double d) { return GetValueComponent(d); }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNotTrigo(TComponent iComp)
        {
            return !IsTrigo(iComp);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsNotValue(TComponent iComp)
        {
            return !IsValue(iComp);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsTrigo(TComponent iComp)
        {
            if (!(iComp is TFunctionComponent)) return false;
            TFunctionComponent cmp = (TFunctionComponent)iComp;
            switch (cmp.Type)
            {
                case EFunction.Cos:
                case EFunction.Sin:
                    return true;

                default:
                    return false;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool IsValue(TComponent iComp)
        {
            return iComp is TValueComponent;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Ln(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Ln, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator -(TComponent iComp1, double iComp2)
        {
            return iComp1 - GetValueComponent(iComp2);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator -(TComponent iComp1, TComponent iComp2)
        {
            //SortElements(ref iComp1, ref iComp2);

            if (iComp2 == 0) return iComp1;
            if (iComp1 == 0) return -iComp2;
            if (iComp1 == iComp2) return 0;

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Substraction)
                {
                    return (iComp1 + cComp2.SComponent) - cComp2.MComponent;
                }
            }
            {
                if (iComp1 is TComposedFunctionComponent cComp1 && cComp1.Type == EComposedFunction.Substraction)
                {
                    return cComp1.MComponent - (cComp1.SComponent + iComp2);
                }
            }

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Division)
                {
                    if (iComp1 is TComposedFunctionComponent cComp1 && cComp1.Type == EComposedFunction.Division)
                    {
                        if (cComp2.SComponent == cComp1.SComponent)
                            return (cComp1.MComponent - cComp2.MComponent) / cComp1.SComponent;
                        else
                            return (cComp1.MComponent * cComp2.SComponent - cComp2.MComponent * cComp1.SComponent) / (cComp1.SComponent * cComp2.SComponent);
                    }
                    else
                        return (iComp1 * cComp2.SComponent - cComp2.MComponent) / cComp2.SComponent;
                }
            }

            {
                if (iComp2 is TOperandComponent cComp2 && cComp2.Type == EOperand.Product)
                {
                    TValueComponent val = (TValueComponent)cComp2.Elements.FirstOrDefault(o => o is TValueComponent oo && oo.Position < 0);
                    if (val != null)
                    {
                        return iComp1 + (-iComp2);
                    }
                }
            }

            //if (iComp2 is TOperandComponent oComp2)
            //{
            //    if (oComp2.Type == EOperand.Product)
            //    {
            //        if (oComp2.Elements.Contains(-1))
            //        {
            //            return iComp1 + (-1 * iComp2);
            //        }
            //    }
            //}
            //if (iComp2 is TValueComponent v2)
            //{
            //    if (v2.Position == 0)
            //        return iComp1;
            //    else if (iComp1 is TValueComponent v1)
            //        return new TValueComponent(v1.Position - v2.Position);
            //}
            //else if (iComp1 is TValueComponent v1)
            //{
            //    if (v1.Position == 0)
            //        return -1 * iComp2;
            //}
            //if (iComp1 is TComposedFunctionComponent cComp1)
            //{
            //    if (cComp1.Type == EComposedFunction.Division)
            //    {
            //        return (cComp1.MComponent - iComp2 * cComp1.SComponent) / cComp1.SComponent;
            //    }
            //    else if (cComp1.Type == EComposedFunction.Substraction)
            //    {
            //        if (iComp2 is TComposedFunctionComponent ccComp2)
            //        {
            //            if (ccComp2.Type == EComposedFunction.Substraction)
            //            {
            //                return (cComp1.MComponent + ccComp2.SComponent) - (cComp1.SComponent + ccComp2.MComponent);
            //            }
            //        }
            //        return cComp1.MComponent - (cComp1.SComponent + iComp2);
            //    }
            //}
            //if (iComp2 is TComposedFunctionComponent cComp2)
            //{
            //    if (cComp2.Type == EComposedFunction.Division)
            //    {
            //        return (iComp1 * cComp2.SComponent - cComp2.MComponent) / cComp2.SComponent;
            //    }
            //    else if (cComp2.Type == EComposedFunction.Substraction)
            //    {
            //        return (iComp1 + cComp2.SComponent) - cComp2.MComponent;
            //    }
            //}

            return new TComposedFunctionComponent(EComposedFunction.Substraction, iComp1, iComp2).Reduced();
            //return (iComp1 + MinusOne * iComp2).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator -(TComponent iComp)
        {
            return (MinusOne * iComp).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator !=(TComponent iComp1, TComponent iComp2) { return !(iComp1 == iComp2); }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator *(TComponent iComp1, double iComp2)
        {
            return iComp1 * GetValueComponent(iComp2);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator *(double iComp1, TComponent iComp2)
        {
            return GetValueComponent(iComp1) * iComp2;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator *(TComponent iComp1, TComponent iComp2)
        {
            if (iComp1 == 0 || iComp2 == 0)
            {
                return 0;
            }

            SortElements(ref iComp1, ref iComp2);

            if (iComp1 == 1) return iComp2;
            if (iComp2 == 1) return iComp1;

            {
                if (iComp1 is TValueComponent vComp1 && iComp2 is TValueComponent vComp2)
                {
                    return vComp1.Position * vComp2.Position;
                }
            }

            {
                if (iComp1 is TValueComponent vComp1 && vComp1.Position < 0 && iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Substraction)
                    return -vComp1 * (cComp2.SComponent - cComp2.MComponent);
            }

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Division)
                {
                    return cComp2.MComponent * iComp1 / cComp2.SComponent;
                }
            }

            {
                //if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Substraction)
                //{
                //    return iComp1 * cComp2.MComponent - iComp1 * cComp2.SComponent;
                //}
            }

            //if (iComp2 is TComposedFunctionComponent)
            //{
            //    TComponent Mem = iComp1;
            //    iComp1 = iComp2;
            //    iComp2 = Mem;
            //}
            //if (iComp1 is TComposedFunctionComponent cCompd)
            //{
            //    if (cCompd.Type == EComposedFunction.Division)
            //    {
            //        return (cCompd.MComponent * iComp2) / cCompd.SComponent;
            //    }
            //}
            //if (iComp2 is TOperandComponent && ((TOperandComponent)iComp2).Type == EOperand.Sum)
            //{
            //    TComponent Mem = iComp1;
            //    iComp1 = iComp2;
            //    iComp2 = Mem;
            //}
            //if (iComp2 is TOperandComponent && ((TOperandComponent)iComp2).Type == EOperand.Product)
            //{
            //    TComponent Mem = iComp1;
            //    iComp1 = iComp2;
            //    iComp2 = Mem;
            //}
            //if (iComp2 is TComposedFunctionComponent)
            //{
            //    TComponent Mem = iComp1;
            //    iComp1 = iComp2;
            //    iComp2 = Mem;
            //}
            //if (iComp1 is TComposedFunctionComponent cComp && cComp.Type == EComposedFunction.Substraction && iComp2 is TValueComponent vComp && vComp.Position < 0)
            //{
            //    iComp2 = new TValueComponent(-vComp.Position);
            //    iComp1 = cComp.SComponent - cComp.MComponent;
            //}
            //if (iComp1 is TComposedFunctionComponent cComp1 && cComp1.Type == EComposedFunction.Substraction)
            //{
            //    return cComp1.MComponent * iComp2 - cComp1.SComponent * iComp2;
            //}
            ////

            ////
            //if (iComp1 is TOperandComponent && ((TOperandComponent)iComp1).Type == EOperand.Product)
            //{
            //    TOperandComponent iElmt1 = (TOperandComponent)iComp1;
            //    if (iComp2 is TOperandComponent && ((TOperandComponent)iComp2).Type == EOperand.Product)
            //    {
            //        TOperandComponent iElmt2 = (TOperandComponent)iComp2;
            //        TComponent[] Elmts = iElmt1.Elements;
            //        Array.Resize(ref Elmts, iElmt1.Elements.Length + iElmt2.Elements.Length);
            //        Array.Copy(iElmt2.Elements, 0, Elmts, iElmt1.Elements.Length, iElmt2.Elements.Length);
            //        return new TOperandComponent(EOperand.Product, Elmts).Reduced();
            //    }
            //    else
            //    {
            //        TComponent[] Elmts = iElmt1.Elements;
            //        Array.Resize(ref Elmts, iElmt1.Elements.Length + 1);
            //        Elmts[Elmts.Length - 1] = iComp2;
            //        return new TOperandComponent(EOperand.Product, Elmts).Reduced();
            //    }
            //}
            //else if (iComp1 is TOperandComponent && ((TOperandComponent)iComp1).Type == EOperand.Sum)
            //{
            //    TOperandComponent iElmt1 = (TOperandComponent)iComp1;
            //    TComponent[] Elmts = new TComponent[iElmt1.Elements.Length];
            //    for (int i = 0; i < iElmt1.Elements.Length; i++)
            //    {
            //        Elmts[i] = iElmt1.Elements[i] * iComp2;
            //    }
            //    TComponent cmp2 = new TOperandComponent(EOperand.Sum, Elmts).Reduced();
            //    return cmp2;
            //}

            return new TOperandComponent(EOperand.Product, new TComponent[2] { iComp1, iComp2 }).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator /(TComponent iComp1, TComponent iComp2)
        {
            //SortElements(ref iComp1, ref iComp2);

            if (iComp2 == 1) return iComp1;

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Division)
                {
                    return (iComp1 * cComp2.SComponent) / cComp2.MComponent;
                }
            }
            {
                if (iComp1 is TComposedFunctionComponent cComp1 && cComp1.Type == EComposedFunction.Division)
                {
                    return cComp1.MComponent / (cComp1.SComponent * iComp2);
                }
            }
            {
                if (iComp1 is TOperandComponent cComp1 && cComp1.Type == EOperand.Product )
                {
                    TComponent scomp2 = cComp1.Elements.FirstOrDefault(o => o.ToString() == iComp2.ToString());
                    if (scomp2 != null)
                    {
                        List<TComponent> cmps = cComp1.Elements.ToList();
                        cmps.Remove(scomp2);
                        TComponent c = cmps[0].Reduced();
                        for (int i = 1; i < cmps.Count; i++)
                            c *= cmps[i];
                        return c;

                    }
                }
            }




            //if (iComp1 is TComposedFunctionComponent cComp1)
            //{
            //    if (cComp1.Type == EComposedFunction.Division)
            //    {
            //        return cComp1.MComponent / (cComp1.SComponent * iComp2);
            //    }
            //}
            //else if (iComp2 is TComposedFunctionComponent cComp2)
            //{
            //    if (cComp2.Type == EComposedFunction.Division)
            //    {
            //        return (iComp1 * cComp2.SComponent) / cComp2.MComponent;
            //    }
            //}
            //else if (iComp1 is TOperandComponent oComp1)
            //{
            //    if (oComp1.Type == EOperand.Product)
            //    {
            //        if (iComp2 is TOperandComponent oComp2)
            //        {
            //            if (oComp2.Type == EOperand.Product)
            //            {
            //                List<TComponent> l1 = new List<TComponent>(oComp1.Elements);
            //                List<TComponent> l2 = new List<TComponent>(oComp2.Elements);
            //                foreach (TComponent c1 in l1.ToArray())
            //                {
            //                    if (l2.Contains(c1))
            //                    {
            //                        l1.Remove(c1);
            //                        l2.Remove(c1);
            //                    }
            //                }
            //                switch (l1.Count)
            //                {
            //                    case 0:
            //                        switch (l2.Count)
            //                        {
            //                            case 0:
            //                                return 1;

            //                            case 1:
            //                                return 1 / l2[0];

            //                            default:
            //                                return 1 / new TOperandComponent(EOperand.Product, l2.ToArray());
            //                        }
            //                    case 1:
            //                        switch (l2.Count)
            //                        {
            //                            case 0:
            //                                return l1[0];

            //                            case 1:
            //                                return l1[0] / l2[0];

            //                            default:
            //                                return l1[0] / new TOperandComponent(EOperand.Product, l2.ToArray());
            //                        }
            //                    default:
            //                        switch (l2.Count)
            //                        {
            //                            case 0:
            //                                return new TOperandComponent(EOperand.Product, l1.ToArray());

            //                            case 1:
            //                                return new TOperandComponent(EOperand.Product, l1.ToArray()) / l2[0];

            //                            default:
            //                                return new TComposedFunctionComponent(EComposedFunction.Division, new TOperandComponent(EOperand.Product, l1.ToArray()), new TOperandComponent(EOperand.Product, l2.ToArray()));
            //                        }
            //                }
            //            }
            //        }
            //        if (oComp1.Elements.Contains(iComp2))
            //        {
            //            List<TComponent> l1 = new List<TComponent>(oComp1.Elements);
            //            l1.Remove(iComp2);
            //            return new TOperandComponent(EOperand.Product, l1.ToArray()).Reduced();
            //        }
            //    }
            //}
            //else if (iComp2 is TOperandComponent oComp2)
            //{
            //    if (oComp2.Type == EOperand.Product)
            //    {
            //        if (oComp2.Elements.Contains(iComp1))
            //        {
            //            List<TComponent> l2 = new List<TComponent>(oComp2.Elements);
            //            l2.Remove(iComp1);
            //            return (1 / (new TOperandComponent(EOperand.Product, l2.ToArray()))).Reduced();
            //        }
            //    }
            //}
            //if (iComp1 == iComp2) return 1;

            return new TComposedFunctionComponent(EComposedFunction.Division, iComp1, iComp2).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator ^(TComponent iComp1, TComponent iComp2)
        {
            return new TComposedFunctionComponent(EComposedFunction.Exponent, iComp1, iComp2).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator ^(TComponent iComp1, double iComp2)
        {
            return new TComposedFunctionComponent(EComposedFunction.Exponent, iComp1, GetValueComponent(iComp2)).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent operator +(TComponent iComp1, TComponent iComp2)
        {
            SortElements(ref iComp1, ref iComp2);

            if (iComp1 == 0) return iComp2;
            if (iComp2 == 0) return iComp1;

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Division)
                {
                    if (iComp1 is TComposedFunctionComponent cComp1 && cComp1.Type == EComposedFunction.Division)
                    {
                        if (cComp2.SComponent == cComp1.SComponent)
                            return (cComp1.MComponent + cComp2.MComponent) / cComp1.SComponent;
                        else
                            return (cComp1.MComponent * cComp2.SComponent + cComp2.MComponent * cComp1.SComponent) / (cComp1.SComponent * cComp2.SComponent);
                    }
                    else
                        return (iComp1 * cComp2.SComponent + cComp2.MComponent) / cComp2.SComponent;
                }
            }

            {
                if (iComp2 is TComposedFunctionComponent cComp2 && cComp2.Type == EComposedFunction.Substraction)
                {
                    return (iComp1 + cComp2.MComponent) - cComp2.SComponent;
                }
            }

            //if (iComp2 is TOperandComponent && ((TOperandComponent)iComp2).Type == EOperand.Sum)
            //{
            //    TComponent Mem = iComp1;
            //    iComp1 = iComp2;
            //    iComp2 = Mem;
            //}
            //
            //if (iComp1 == 0 && iComp2 == 0)
            //{
            //    return 0;
            //}
            //
            //if (iComp1 is TOperandComponent oComp1)
            //{
            //    if (oComp1.Type == EOperand.Product)
            //    {
            //        if (oComp1.Elements.Contains(-1))
            //            return iComp2 - (-1 * iComp1);
            //    }
            //}
            //if (iComp2 is TOperandComponent oComp2)
            //{
            //    if (oComp2.Type == EOperand.Product)
            //    {
            //        if (oComp2.Elements.Contains(-1))
            //            return iComp1 - (-1 * iComp2);
            //    }
            //}
            ////
            //if (iComp1 is TOperandComponent && ((TOperandComponent)iComp1).Type == EOperand.Sum)
            //{
            //    TOperandComponent iElmt1 = (TOperandComponent)iComp1;
            //    if (iComp2 is TOperandComponent && ((TOperandComponent)iComp2).Type == EOperand.Sum)
            //    {
            //        TOperandComponent iElmt2 = (TOperandComponent)iComp2;
            //        TComponent[] Elmts = iElmt1.Elements;
            //        Array.Resize(ref Elmts, iElmt1.Elements.Length + iElmt2.Elements.Length);
            //        Array.Copy(iElmt2.Elements, 0, Elmts, iElmt1.Elements.Length, iElmt2.Elements.Length);
            //        return new TOperandComponent(EOperand.Sum, Elmts).Reduced();
            //    }
            //    else
            //    {
            //        TComponent[] Elmts = iElmt1.Elements;
            //        Array.Resize(ref Elmts, iElmt1.Elements.Length + 1);
            //        Elmts[Elmts.Length - 1] = iComp2;
            //        return new TOperandComponent(EOperand.Sum, Elmts).Reduced();
            //    }
            //}
            //else if (iComp1 is TComposedFunctionComponent cComp1)
            //{
            //    if (cComp1.Type == EComposedFunction.Division)
            //    {
            //        if (iComp2 is TComposedFunctionComponent cComp2)
            //        {
            //            if (cComp2.Type == EComposedFunction.Division)
            //            {
            //                if (cComp1.SComponent == cComp2.SComponent)
            //                {
            //                    return (cComp1.MComponent + cComp2.MComponent) / cComp1.SComponent;
            //                }
            //                else
            //                    return (cComp1.MComponent * cComp2.SComponent + cComp2.MComponent * cComp1.SComponent) / (cComp1.SComponent * cComp2.SComponent);
            //            }
            //        }
            //        return (cComp1.MComponent + iComp2 * cComp1.SComponent) / cComp1.SComponent;
            //    }
            //    else if (cComp1.Type == EComposedFunction.Substraction)
            //    {
            //        return (iComp2 + cComp1.MComponent) - cComp1.SComponent;
            //    }
            //}

            return new TOperandComponent(EOperand.Sum, new TComponent[2] { iComp1, iComp2 }).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator ==(TComponent iComp1, TComponent iComp2)
        {
            if ((object)iComp1 == null && (object)iComp2 == null) return true;
            if ((object)iComp1 == null || (object)iComp2 == null) return false;
            return iComp1.Text == iComp2.Text;
        }

        public static TComponent ParseFormula(string text)
        {
            if (text == "") return null;
            foreach (char c in Separators) text = text.Replace(c + "", " " + c + " ");
            while (text.Contains("  ")) text = text.Replace("  ", " ");
            string[] wds = text.Split(' ');
            return ParseWords(wds, 0, wds.Length);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Sin(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Sin, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Sinh(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Sinh, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static int SortByText(TComponent iComp1, TComponent iComp2)
        {
            return iComp1.Text.CompareTo(iComp2.Text);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Sqrt(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Sqrt, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Tan(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Tan, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static TComponent Tanh(TComponent iComponent)
        {
            return new TFunctionComponent(EFunction.Tanh, iComponent).Reduced();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj) { return base.Equals(obj); }

        public abstract TComponent Factorises(TComponent factor);

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract TComponent GetDifferential(TParameterComponent iParameter);

        public abstract TComponent[] GetFactors();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode() { return base.GetHashCode(); }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract TParameterComponent[] GetParameters();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public abstract TComponent Reduced();

        public abstract void Replace(TComponent oldComp, TComponent newComp);

        public abstract TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp);

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string ToString()
        {
            return Text;// + "=" + Math.Round(Position, 3).ToString();
        }

        #endregion Public Methods

        #region Protected Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    //if (GetType(this) != TValueComponent)
                    //    AllComponents.Remove(this);
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        #endregion Protected Methods

        #region Private Methods

        private static TComponent ParseWords(string[] wds, int v1, int v2)
        {
            Console.WriteLine(v1.ToString() + ":" + v2.ToString());
            string ss = wds[v1]; for (int ii = v1 + 1; ii < v2; ii++) ss += " " + wds[ii];
            Console.WriteLine(ss);

            if (v2 == v1 + 1)
            {
                if (double.TryParse(wds[v1], out double d))
                {
                    return new TValueComponent(d);
                }
                else
                    return GetParameter(wds[v1]);
            }

            List<TGroup> groups = new List<TGroup>();

            int i = v1;
            TGroup last = new TGroup(v1);
            while (i < v2)
            {
                switch (wds[i])
                {
                    case "(":
                        i++;
                        int n = 1;
                        while (i < wds.Length && n > 0)
                        {
                            switch (wds[i])
                            {
                                case "(": n++; break;
                                case ")": n--; break;
                            }
                            i++;
                        }
                        last.end = i;
                        break;

                    case "+":
                    case "/":
                    case "-":
                    case "*":
                    case "^":
                        last.end = i; groups.Add(last);
                        last = new TGroup(i)
                        {
                            end = i + 1
                        };
                        i++;
                        break;

                    default:
                        if (double.TryParse(wds[i], out double d))
                        {
                            last.end = i;
                            i++;
                        }
                        else
                            i++;
                        break;
                }
                if (last.end > 0)
                {
                    groups.Add(last);
                    last = new TGroup(i);
                }
            }
            last.end = i;
            groups.Add(last);
            // ---------------------
            groups.RemoveAll(o => o.start == o.end - 1 && wds[o.start] == "");
            groups.RemoveAll(o => o.start >= v2);
            groups.RemoveAll(o => o.start == o.end);
            // ------------------------
            foreach (TGroup g in groups)
            {
                ss = "\t" + wds[g.start];
                for (int ii = g.start + 1; ii < g.end; ii++) ss += " " + wds[ii];
                Console.WriteLine(ss);

                if (g.start == g.end - 1)
                {
                    if (double.TryParse(wds[g.start], out double d))
                    {
                        g.component = new TValueComponent(d);
                    }
                    else
                        g.component = GetParameter(wds[g.start]);
                }
                else if (Enum.TryParse(wds[g.start], out EFunction func))
                {
                    g.component = new TFunctionComponent(func, ParseWords(wds, g.start + 2, g.end - 1));
                }
                else if (wds[g.start] == "(" && wds[g.end - 1] == ")")
                    g.component = ParseWords(wds, g.start + 1, g.end - 1);
            }

            if (groups.Count > 0)
            {
                ss = string.Format("{0}:{1}:{2}", groups[0].start, groups[0].end, groups[0].component);
                for (int ii = 1; ii < groups.Count; ii++)
                    ss = " " + string.Format("{0}:{1}:{2}", groups[ii].start, groups[ii].end, groups[ii].component);
                Console.WriteLine(ss + "\n");
                if (groups.Count == 1)
                    return groups[0].component;
                else if (groups.Count == 3)
                {
                    switch (wds[groups[1].start])
                    {
                        case "+":
                            {
                                TOperandComponent ope = new TOperandComponent(EOperand.Sum, new TComponent[] { groups[0].component, groups[2].component });
                                return ope;
                            }
                        case "*":
                            {
                                TOperandComponent ope = new TOperandComponent(EOperand.Product, new TComponent[] { groups[0].component, groups[2].component });
                                return ope;
                            }
                        case "-":
                            {
                                TComposedFunctionComponent ope = new TComposedFunctionComponent(EComposedFunction.Substraction, groups[0].component, groups[2].component);
                                return ope;
                            }
                        case "/":
                            {
                                TComposedFunctionComponent ope = new TComposedFunctionComponent(EComposedFunction.Division, groups[0].component, groups[2].component);
                                return ope;
                            }
                        case "^":
                            {
                                TComposedFunctionComponent ope = new TComposedFunctionComponent(EComposedFunction.Exponent, groups[0].component, groups[2].component);
                                return ope;
                            }
                    }
                }
                else if (groups.Count == 2)
                {
                }
            }
            return new TValueComponent(0);
        }

        private static void SortElements(ref TComponent iComp1, ref TComponent iComp2)
        {
            if (iComp1.Level > iComp2.Level) { TComponent mem = iComp1; iComp1 = iComp2; iComp2 = mem; }
        }

        #endregion Private Methods

        #region Private Classes

        private class TGroup
        {
            #region Public Fields

            public TComponent component;
            public int end = -1;
            public int start;

            #endregion Public Fields

            #region Public Constructors

            public TGroup(int s)
            {
                start = s;
            }

            #endregion Public Constructors
        }

        #endregion Private Classes

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}