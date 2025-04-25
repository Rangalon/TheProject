using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace CiliaElements
{
    public abstract class TBaseElement
    {
        //Public Event Publishing(sender As System.Object, e As System.EventArgs)

        #region Public Fields

        public List<TCustomAttr> CustomAttributes;
        public TBaseElementLoader ElementLoader;
        public FileInfo Fi;
        public string PartNumber;

        #endregion Public Fields

        #region Public Properties

        public virtual SBoundingBox3 BoundingBox { get; set; }
        //public virtual SBoundingBox4 BoundingBox4 { get; set; }

        public TLink OwnerLink { get; set; }

        public virtual EElementState State { get; set; }

        #endregion Public Properties

        #region Public Methods

        public virtual void LaunchLoad()
        {
        }

        public virtual FileInfo Pack()
        {
            return null;
        }

        public override string ToString()
        {
            if (PartNumber == null)
            {
                return this.GetType().Name + ": No Partnumber";
            }

            return this.GetType().Name + ": " + PartNumber.ToString(CultureInfo.InvariantCulture);
        }

        //public void UpdateBoundingBox4(TLink l)
        //{
        //    SBoundingBox4 nb = SBoundingBox4.Default;
        //    UpdateBoundingBox4(ref nb, l, Mtx4.Identity);
        //    BoundingBox4 = nb;
        //}

        #endregion Public Methods

        #region Private Methods

        //private void UpdateBoundingBox4(ref SBoundingBox4 nb, TLink l, Mtx4 m)
        //{
        //    if (l.Child is TAssemblyElement)
        //    {
        //        foreach (TLink ll in l.Links.Values.Where(o => o != null))
        //            UpdateBoundingBox4(ref nb, ll, ll.Matrix * m);
        //    }
        //    else
        //    {
        //        if (l.Solid.DataPositions.Length == 0) return;
        //        if (nb.MaxPosition.X == double.NegativeInfinity)
        //            nb = new SBoundingBox4(l.Solid.DataPositions[0]);
        //        foreach (Vec3 p in l.Solid.DataPositions)
        //            nb.CheckPosition(Vec3.Transform(p, m));
        //    }
        //}

        #endregion Private Methods
    }

    public class TBaseElementTypeComparer : IComparer<TBaseElement>
    {
        #region Public Fields

        public static TBaseElementTypeComparer Default = new TBaseElementTypeComparer();

        #endregion Public Fields

        #region Public Methods

        public int Compare(TBaseElement x, TBaseElement y)
        {
            if (x is TSolidElement)
            {
                if (y is TSolidElement)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else if (y is TSolidElement)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion Public Methods
    }
}