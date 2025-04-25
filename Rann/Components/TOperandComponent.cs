using System;
using System.Collections.Generic;
using System.Linq;

namespace Rann.Components
{
    /// <summary>
    /// This class is used to managed all mathematic operand involving several Elements
    /// The order of Elements is not important
    /// Then, only two operand are concerned:
    /// - Sum
    /// - Product
    /// </summary>
    /// <remarks></remarks>

    public class TOperandComponent : TComponent
    {
        #region Public Fields

        public readonly EOperand Type;
        public TComponent[] Elements;

        #endregion Public Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public TOperandComponent(EOperand iType, TComponent[] iComponents)
        {
            Type = iType;
            Elements = iComponents;
            // Numeric values simplification
            TComponent[] LstValues = Array.FindAll<TComponent>(Elements, TComponent.IsValue);
            // We will have a look on numerical values
            if (LstValues.Length > 0)
            {
                Elements = Array.FindAll<TComponent>(Elements, TComponent.IsNotValue);
                // Initialisation
                double d = 0;

                // The starting value depend on the kind of the operand
                switch (Type)
                {
                    case EOperand.Product: d = 1; break;
                    case EOperand.Sum: d = 0; break;
                }

                // Then, we apply the computation for each numerical value
                foreach (TValueComponent value in LstValues)
                {
                    switch (Type)
                    {
                        case EOperand.Product: d *= value.Content; break;
                        case EOperand.Sum: d += value.Content; break;
                    }
                }

                switch (Type)
                {
                    case EOperand.Product:
                        if (d != 1)
                        {
                            Array.Resize(ref Elements, Elements.Length + 1);
                            Array.Copy(Elements, 0, Elements, 1, Elements.Length - 1);
                            TValueComponent v;
                            if (d == 0) v = 0;
                            else v = new TValueComponent(d);
                            Elements[0] = v;
                        }
                        else if (Elements.Length == 0)
                            Elements = new TComponent[] { 1 };
                        break;

                    case EOperand.Sum:
                        if (d != 0)
                        {
                            Array.Resize(ref Elements, Elements.Length + 1);
                            Array.Copy(Elements, 0, Elements, 1, Elements.Length - 1);
                            TValueComponent v;
                            if (d == 1) v = 1;
                            else v = new TValueComponent(d);
                            Elements[0] = v;
                        }
                        else if (Elements.Length == 0)
                            Elements = new TComponent[] { 0 };
                        break;
                }
            }

            //LstValues = Array.FindAll<TComponent>(Elements, TComponent.IsTrigo);
            //if (LstValues.Length > 1 && iType == EOperand.Product)
            //{
            //    Elements = Array.FindAll<TComponent>(Elements, TComponent.IsNotTrigo);
            //    TFunctionComponent iElmt1 = (TFunctionComponent)LstValues[0];
            //    TFunctionComponent iElmt2 = (TFunctionComponent)LstValues[1];
            //    if (iElmt2.Type == EFunction.Cos)
            //    {
            //        TFunctionComponent elmt = iElmt2;
            //        iElmt2 = iElmt1;
            //        iElmt1 = elmt;
            //    }
            //    TComponent iVal1 = iElmt1.Component;
            //    TComponent iVal2 = iElmt2.Component;
            //    TComponent res = 1;
            //    switch (iElmt1.Type)
            //    {
            //        case EFunction.Cos:
            //            switch (iElmt2.Type)
            //            {
            //                case EFunction.Cos:
            //                    res = 0.5 * Cos(iVal1 - iVal2) + 0.5 * Cos(iVal1 + iVal2);
            //                    break;

            //                case EFunction.Sin:
            //                    res = 0.5 * Sin(iVal1 + iVal2) - 0.5 * Sin(iVal1 - iVal2);
            //                    break;
            //            }
            //            break;

            //        case EFunction.Sin:
            //            res = 0.5 * Cos(iVal1 - iVal2) - 0.5 * Cos(iVal1 + iVal2);
            //            break;
            //    }
            //    for (int i = 2; i < LstValues.Length; i++) { res *= LstValues[i]; }
            //    Array.Resize(ref Elements, Elements.Length + 1);
            //    Elements[Elements.Length - 1] = res;
            //}
        }

        #endregion Public Constructors

        #region Public Properties

        public override byte Level
        {
            get
            {
                switch (Type)
                {
                    case EOperand.Product: return 42;
                    case EOperand.Sum: return 44;
                }
                return 40;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override double Position
        {
            get
            {
                double functionReturnValue = 0;
                switch (Type)
                {
                    case EOperand.Product:
                        functionReturnValue = 1;
                        break;

                    case EOperand.Sum:
                        functionReturnValue = 0;
                        break;
                }
                foreach (TComponent component in Elements)
                {
                    switch (Type)
                    {
                        case EOperand.Product:
                            functionReturnValue *= component.Position;
                            break;

                        case EOperand.Sum:
                            functionReturnValue += component.Position;
                            break;
                    }
                }
                return functionReturnValue;
            }
            set { throw new Exception("Not Allowed!"); }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string Text
        {
            get
            {
                string s = Elements[0].Text;
                for (int i = 1; i <= Elements.Length - 1; i++)
                {
                    s += " " + TextOperand(Type) + " " + Elements[i].Text;
                }
                return "(" + s + ")";
            }
        }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string TextOperand(EOperand iType)
        {
            switch (iType)
            {
                case EOperand.Product:
                    return "*";

                case EOperand.Sum:
                    return "+";
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is TOperandComponent)
            {
                return obj.ToString() == ToString();
            }
            return false;
        }

        public override TComponent Factorises(TComponent factor)
        {
            switch (Type)
            {
                case EOperand.Product:
                    List<TComponent> lst = Elements.ToList();
                    lst.Remove(factor);
                    return new TOperandComponent(EOperand.Product, lst.ToArray());

                case EOperand.Sum:
                    TComponent s = 0;
                    foreach (TComponent c in Elements)
                        s += c.Factorises(factor);
                    return s;
            }
            throw new Exception("not managed");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            TComponent functionReturnValue;
            switch (this.Type)
            {
                case EOperand.Product:
                    functionReturnValue = 0;
                    foreach (TComponent component in Elements)
                    {
                        TComponent diff = component.GetDifferential(iParameter);
                        foreach (TComponent subcomponent in Elements)
                        {
                            if ((!object.ReferenceEquals(subcomponent, component)))
                                diff *= subcomponent;
                        }
                        functionReturnValue += diff;
                    }

                    break;

                case EOperand.Sum:
                    functionReturnValue = 0;
                    foreach (TComponent component in Elements)
                    {
                        functionReturnValue += component.GetDifferential(iParameter);
                    }

                    break;

                default:
                    functionReturnValue = null;
                    break;
            }
            return functionReturnValue;
        }

        public override TComponent[] GetFactors()
        {
            List<TComponent> lst = new List<TComponent>();
            switch (Type)
            {
                case EOperand.Product:
                    lst.Add(this);
                    foreach (TComponent c in Elements)
                    {
                        lst.AddRange(c.GetFactors());
                    }
                    break;

                case EOperand.Sum:
                    break;
            }
            return lst.ToArray();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TParameterComponent[] GetParameters()
        {
            TParameterComponent[] functionReturnValue;
            List<TParameterComponent> lst = new List<TParameterComponent>();
            foreach (TComponent component in Elements)
            {
                foreach (TParameterComponent prmtr in component.GetParameters())
                { if (!lst.Contains(prmtr)) lst.Add(prmtr); }
            }
            functionReturnValue = lst.ToArray();
            GC.SuppressFinalize(lst);
            return functionReturnValue;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsLikeMe()
        {
            //if (iComp.GetType() == GetType(TOperandComponent))
            return false;
            //if (!(((TOperandComponent)iComp).Type == Type))
            //    return false;
            //return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool IsNotLikeMe()
        {
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override TComponent Reduced()
        {
            if (Elements.Length == 1) return Elements[0];
            switch (Type)
            {
                case EOperand.Product:
                    {
                        TOperandComponent ope = (TOperandComponent)Elements.FirstOrDefault(o => o is TOperandComponent oo && oo.Type == EOperand.Sum);
                        if (ope != null)
                        {
                            //TComponent s = 0;
                            //List<TComponent> lst = new List<TComponent>(Elements); lst.Remove(ope);
                            //TComponent p = new TOperandComponent(EOperand.Product, lst.ToArray());
                            //foreach (TComponent c in ope.Elements)
                            //    s += c * p;
                            //return s;
                        }
                        {
                            List<TComponent> lst = new List<TComponent>(Elements);
                            foreach (TOperandComponent oo in Elements.ToArray().Where(o => o is TOperandComponent oComp && oComp.Type == EOperand.Product))
                            {
                                lst.Remove(oo);
                                lst.AddRange(oo.Elements);
                            }
                            double d = 1;
                            foreach (TValueComponent val in lst.ToArray().OfType<TValueComponent>())
                            {
                                d *= val.Position;
                                lst.Remove(val);
                            }
                            if (d != 1) lst.Add(new TValueComponent(d));
                            Elements = lst.ToArray();
                        }
                    }
                    break;

                case EOperand.Sum:
                    {
                        TOperandComponent ope = (TOperandComponent)Elements.FirstOrDefault(o => o is TOperandComponent oo && oo.Type == EOperand.Product && oo.Elements.FirstOrDefault(ooo => ooo is TValueComponent vo && vo.Position < 0) != null);
                        if (ope != null)
                        {
                            List<TComponent> lst = new List<TComponent>(Elements);
                            lst.Remove(ope);
                            TComponent s = 0;
                            foreach (TComponent c in lst) s += c;
                            s -= -ope;
                            return s;
                        }
                        ope = (TOperandComponent)Elements.FirstOrDefault(o => o is TOperandComponent op && op.Type == EOperand.Sum);
                        if (ope != null)
                        {
                            List<TComponent> lst = new List<TComponent>(Elements);
                            foreach (TOperandComponent sum in Elements.OfType<TOperandComponent>().Where(o => o.Type == EOperand.Sum))
                            {
                                lst.Remove(sum);
                                lst.AddRange(sum.Elements);
                            }
                            Elements = lst.ToArray();
                        }
                        //TComponent[] cps = Elements.Where(o => o is TComposedFunctionComponent ocfc && ocfc.Type == EComposedFunction.Substraction).ToArray();
                        //if (cps.Length > 0)
                        //{
                        //    List<TComponent> lst = new List<TComponent>(Elements);
                        //    List<TComponent> slst = new List<TComponent>();
                        //    foreach (TComposedFunctionComponent cfc in cps.OfType<TComposedFunctionComponent>())
                        //    {
                        //        lst.Remove(cfc);
                        //        lst.Add(cfc.MComponent);
                        //        slst.Add(cfc.SComponent);
                        //    }
                        //    foreach (TComponent sc in slst.ToArray())
                        //    {
                        //        TOperandComponent mc = lst.OfType<TOperandComponent>().FirstOrDefault(o => o.Elements.Contains(sc));
                        //        if (mc != null)
                        //        {
                        //        }
                        //    }


                        //}
                    }
                    break;
            }

            Array.Sort(Elements, TComponentComparer.Default);
            return this;
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
            int i = Array.IndexOf(Elements, oldComp);
            while (i >= 0)
            {
                Elements[i] = newComp;
                i = Array.IndexOf(Elements, oldComp);
            }
            i = 0;
            while (i < Elements.Length)
            {
                if (Elements[i] != newComp)
                {
                    Elements[i].Replace(oldComp, newComp);
                    Elements[i] = Elements[i].Reduced();
                }
                i++;
            }
        }

        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            if (this.ToString() == iOldComp.ToString()) return iNewComp;
            switch (Type)
            {
                case EOperand.Product:
                    TComponent p = 1;
                    foreach (TComponent c in Elements) p *= c.ReplaceComponent(iOldComp, iNewComp);
                    return p;

                case EOperand.Sum:
                    TComponent s = 0;
                    foreach (TComponent c in Elements) s += c.ReplaceComponent(iOldComp, iNewComp);
                    return s;
            }
            return null;
        }

        #endregion Public Methods

        #region Protected Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GC.SuppressFinalize(Elements);
            }
            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Public Classes

        public class TComponentComparer : IComparer<TComponent>
        {
            #region Public Fields

            public static readonly TComponentComparer Default = new TComponentComparer();

            #endregion Public Fields

            #region Public Methods

            public int Compare(TComponent x, TComponent y)
            {
                return x.ToString().CompareTo(y.ToString());
            }

            #endregion Public Methods
        }

        #endregion Public Classes

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}