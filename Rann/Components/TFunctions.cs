using System;

namespace Rann.Components
{
    public enum EComposedFunction
    {
        Exponent,
        Substraction,
        Division
    }

    public enum EFunction
    {
        Cosh,
        Sinh,
        Tanh,
        Coth,

        //ACosh
        //ASinh
        //ATanh
        //ACoth
        Cos,

        Sin,
        Tan,
        Cot,
        ACos,
        ASin,
        ATan,
        ACot,
        Exp,
        Ln,
        Sqrt
    }

    public enum EOperand
    {
        Sum,
        Product
    }

    public enum EState
    {
        Nominal = 0,
        Ok = 1,
        Warning = 2,
        Bad = 3,
        Incoherent = 4
    }

    public static class TFunctions
    {
        #region Public Fields

        public const int ParenthesisWidth = 3;

        #endregion Public Fields

        //Public Shared Function ACosh(iComponent As TComponent) As TComponent
        //    ACosh = New TFunction(TFunction.EFunction.ACosh, iComponent).Reduced
        //End Function

        //Public Shared Function ASinh(iComponent As TComponent) As TComponent
        //    ASinh = New TFunction(TFunction.EFunction.ASinh, iComponent).Reduced
        //End Function

        //Public Shared Function ATanh(iComponent As TComponent) As TComponent
        //    ATanh = New TFunction(TFunction.EFunction.ATanh, iComponent).Reduced
        //End Function

        //Public Shared Function ACoth(iComponent As TComponent) As TComponent
        //    ACoth = New TFunction(TFunction.EFunction.ACoth, iComponent).Reduced
        //End Function

        ///// <summary>
        ///// Split a String field to subfields, according to specified separator
        ///// </summary>
        ///// <param name="iString"></param>
        ///// <param name="iSeparator"></param>
        ///// <param name="iParameters"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public static TComponent[] SplitField(String iString, char iSeparator, Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    TComponent[] functionReturnValue = null;
        //    List<TComponent> lst = new List<TComponent>();
        //    Int32 pos = 0;
        //    Int32 nb = 0;
        //    TComponent comp = null;
        //    while (pos < iString.Length)
        //    {
        //        switch (iString.Substring(pos, 1))
        //        {
        //            case iSeparator:
        //                if (TComponent.TryParse(iString.Substring(0, pos), comp, iParameters))
        //                {
        //                    lst.Add(comp);
        //                    comp = null;
        //                }
        //                else
        //                {
        //                    return null;
        //                }
        //                iString = iString.Substring(pos + 1);
        //                pos = 0;
        //                break;
        //            case "(":
        //                pos = pos;
        //                do
        //                {
        //                    if (pos == iString.Length)
        //                        return null;
        //                    switch (iString.Substring(pos, 1))
        //                    {
        //                        case "(":
        //                            nb += 1;
        //                            break;
        //                        case ")":
        //                            nb -= 1;
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    pos += 1;
        //                } while (nb > 0);

        //                break;
        //            default:
        //                pos += 1;
        //                break;
        //        }
        //    }
        //    switch (true)
        //    {
        //        case lst.Count == 0:
        //            functionReturnValue = null;
        //            break;
        //        case iString.Length == 0:
        //            functionReturnValue = null;
        //            break;
        //        case !TComponent.TryParse(iString, comp, iParameters):
        //            functionReturnValue = null;
        //            break;
        //        default:
        //            lst.Add(comp);
        //            functionReturnValue = lst.ToArray;
        //            break;
        //    }
        //    GC.SuppressFinalize(lst);
        //    return functionReturnValue;
        //}

        /// <summary>
        /// Cleaning of the text field, before parsing to formula
        /// </summary>
        /// <param name="iString"></param>
        /// <remarks></remarks>

        #region Public Methods

        public static void CleanField(ref string iString)
        {
            // No space
            iString = iString.Replace(" ", "");

            // if field does not start by a parenthesis, end of the cleaning
            if (!iString.StartsWith("("))
                return;

            // Split the field in groups inside parenthesis
            // Position in the String
            int pos = 0;

            // Level of parenthesis
            int nb = 0;

            // Parse the String regarding parenthesis (only first level)
            do
            {
                switch (iString.Substring(pos, 1))
                {
                    case "(":
                        nb += 1;
                        break;

                    case ")":
                        nb -= 1;
                        break;

                    default:
                        break;
                }
                pos += 1;
            } while (pos < iString.Length && nb > 0);

            // Check how much of the String is inside parenthesis
            if (nb > 0)
                return;
            else if (pos < iString.Length)
                return;
            else
                iString = iString.Substring(1, iString.Length - 2);
        }

        #endregion Public Methods

        ///// <summary>
        ///// Draw left parenthesis for component
        ///// </summary>
        ///// <param name="iGrp"></param>
        ///// <param name="iHeight"></param>
        ///// <remarks></remarks>
        //public static void DrawLeftParenthesis(Graphics iGrp, Single iHeight)
        //{
        //    iHeight = -Conversion.Int(-iHeight) - 1;
        //    Single iL = 0;
        //    Single iR = ParenthesisWidth - 1;
        //    iGrp.DrawLine(Pens.Black, iR, 0, iL, iR - iL);
        //    iGrp.DrawLine(Pens.Black, iL, iR - iL, iL, iHeight - (iR - iL));
        //    iGrp.DrawLine(Pens.Black, iL, iHeight - (iR - iL), iR, iHeight);
        //    iGrp.TranslateTransform(ParenthesisWidth, 0);
        //}

        ///// <summary>
        ///// Draw right parenthesis for component
        ///// </summary>
        ///// <param name="iGrp"></param>
        ///// <param name="iHeight"></param>
        ///// <remarks></remarks>
        //public static void DrawRightParenthesis(Graphics iGrp, Single iHeight)
        //{
        //    iHeight = -Conversion.Int(-iHeight) - 1;
        //    Single iL = 0;
        //    Single iR = ParenthesisWidth - 1;
        //    iGrp.DrawLine(Pens.Black, iL, 0, iR, iR - iL);
        //    iGrp.DrawLine(Pens.Black, iR, iR - iL, iR, iHeight - (iR - iL));
        //    iGrp.DrawLine(Pens.Black, iR, iHeight - (iR - iL), iL, iHeight);
        //    iGrp.TranslateTransform(ParenthesisWidth, 0);
        //}
    }
}