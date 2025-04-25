using System;

namespace Rann.Components
{
    /// <summary>
    /// This class is used to managed numerical values as components
    /// </summary>
    /// <remarks></remarks>

    public class TValueComponent : TComponent
    {
        #region Public Fields

        public readonly double Content;

        #endregion Public Fields

        #region Public Constructors

        public TValueComponent(double iContent)
        {
            if (Math.Abs(iContent) < 0.000001 && Math.Abs(iContent) > 0)
            {
                iContent = 0;
            }
            Content = iContent;
        }

        #endregion Public Constructors

        #region Public Properties

        public override byte Level => 10;
      
        public override double Position
        {
            get { return Content; }
            set { throw new Exception("Not Allowed!"); }
        }

        //public static new Boolean TryParse(String iText, ref TValueComponent iComponent, Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    Double d = 0;
        //    if (Double.TryParse(iText,out d))
        //        iComponent = new TValueComponent(d);
        //    return iComponent != null;
        //}
        public override string Text
        {
            get { return Content.ToString(); }
        }

        #endregion Public Properties

        #region Public Methods

        public static implicit operator TValueComponent(double d)
        {
            return GetValueComponent(d);
        }

        public override bool Equals(object obj)
        {
            if (obj is TValueComponent vComp)
            {
                return Position == vComp.Position;
            }
            return false;
        }

        public override TComponent Factorises(TComponent factor)
        {
            return this / factor;
        }

        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            return 0;
        }

        public override TComponent[] GetFactors()
        {
            return new TComponent[] { this };
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override TParameterComponent[] GetParameters()
        {
            return new TParameterComponent[0] { };
        }

        public override TComponent Reduced()
        {
            return this;
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
        }

        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            if (this.ToString() == iOldComp.ToString()) return iNewComp;
            return this;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Numerical values are not disposed
        /// </summary>
        /// <param name="disposing"></param>
        /// <remarks></remarks>
        protected override void Dispose(bool disposing)
        {
        }

        #endregion Protected Methods

        //public override TComponent Clone(Dictionary<String, TParameterComponent> iParameters = null)
        //{
        //    return this;
        //}
        //public override System.Drawing.SizeF GetDrawingSize(Graphics iGrp)
        //{
        //    String s = Strings.Format(Content, "0.000");
        //    while (s.EndsWith("0"))
        //    {
        //        s = s.Substring(0, s.Length - 1);
        //    }
        //    while (s.EndsWith("."))
        //    {
        //        s = s.Substring(0, s.Length - 1);
        //    }
        //    return iGrp.MeasureString(s, Ft);
        //}

        //public override void DrawContent(Graphics iGrp)
        //{
        //    base.DrawContent(iGrp);
        //    String s = Strings.Format(Content, "0.000");
        //    while (s.EndsWith("0"))
        //    {
        //        s = s.Substring(0, s.Length - 1);
        //    }
        //    while (s.EndsWith("."))
        //    {
        //        s = s.Substring(0, s.Length - 1);
        //    }
        //    iGrp.DrawString(s, Ft, Brushes.Black, 0, 0);
        //}
    }
}