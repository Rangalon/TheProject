
using Math3D;
using OpenTK.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CiliaElements
{
    public interface IDrawnInTree
    {
        #region Public Properties

        bool DrawInTree { get; set; }

        #endregion Public Properties
    }

    public interface IEnable
    {
        #region Public Properties

        bool Enabled { get; set; }

        #endregion Public Properties
    }

    public interface IExpandable : IEnumerable
    {
        #region Public Properties

        bool Expanded { get; set; }

        #endregion Public Properties
    }

    public class TLink : IExpandable, IDrawnInTree, IEnable
    {
        #region Public Fields

        public static Regex RgxAll = new Regex(@".*");

        public Action ActionToDo;

        public bool Activated = false;

        public Mtx4f BackColor = Mtx4f.Identity;

        public bool CanBeHighlighted = true;
        public List<TCustomAttr> CustomAttributes;

        public Mtx4f? DisplayColor;


        public TFile File;

        public bool Fixed = false;

        public string Fn = "";

        public Mtx4 ForeColor = Mtx4.CreateScale(-1, -1, -1) * Mtx4.CreateTranslation(1, 1, 1);

        //public Mtx4 GivingMatrix = Mtx4.Identity;

        //public ELinkState GivingState = ELinkState.None;

        //public Mtx4 ITakenMatrix = Mtx4.Identity;

        public TLinksQuickStc Links = new TLinksQuickStc();

        public bool MoveState = false;

        public Mtx4 MovingRotation = Mtx4.CreateRotationZ(0.02);


        public Mtx4 PendingMatrix;
        public bool Pickable = true;
        public string PN = "";

        public bool Replaceable;

        public bool Shadowed;

        public TSolidElement Solid = null;

        public TLink[] Solids;

        public ELinkState State = ELinkState.None;

        public TLink[] SubAssemblies;

        //public Mtx4 TakenMatrix = Mtx4.Identity;

        public SLink Giving;

        //public ELinkState TakenState = ELinkState.None;

        public List<TTextRange> TextRanges = new List<TTextRange>();

        public bool ToBeReplaced;

        public bool ToBeUpdated = false;

        #endregion Public Fields

        #region Private Fields

        private TBaseElement _Child = null;

        private string _NodeName = "";

        private Mtx4? _OriginMatrix;

        private TLink _ParentLink;


        private bool enabled = true;

        //private Mtx4 imatrix = Mtx4.Identity;

        private Mtx4 matrix = Mtx4.Identity;

        public bool NoEffect;
        public bool Ethereal = false;
        public bool NoDiffuse;
        public bool NoCulling;

        #endregion Private Fields

        #region Public Constructors

        public TLink()
        {
            Giving = new SLink(this);
            DrawInTree = true;
        }

        #endregion Public Constructors

        #region Public Properties

        public Mtx4 AbsoluteMatrix
        {
            get
            {
                TLink l = ParentLink;
                Mtx4 M = Matrix;
                while (l != null)
                {
                    M *= l.Matrix;
                    l = l.ParentLink;
                }
                return M;
            }
        }

        [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        [System.ComponentModel.Category("Product")]
        public TBaseElement Child
        {
            get { return _Child; }
            set
            {
                if (object.ReferenceEquals(_Child, value))
                {
                    return;
                }

                _Child = value;
                if (_Child is TSolidElement)
                {
                    Solid = (TSolidElement)value;
                }
                RequestUpdate();
            }
        }

        public bool DrawInTree { get; set; }

        [System.ComponentModel.Browsable(false)]
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (enabled == value)
                {
                    return;
                }

                enabled = value;
                RequestUpdate();
            }
        }

        public bool Expanded { get; set; }

        public string FileName
        {
            get
            {
                if (Child != null && Child.Fi != null)
                {
                    return Child.Fi.FullName;
                }

                return Fn;
            }
            set
            {
                Fn = value;
                if (!string.IsNullOrEmpty(value))
                {
                    TFile tf = TManager.UsedFiles.Values.FirstOrDefault(o => o != null && o.Element != null && o.Element.Fi != null && o.Element.Fi.FullName.ToUpper(CultureInfo.InvariantCulture) == value.ToUpper(CultureInfo.InvariantCulture));
                    if (tf == null)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(value);
                        tf = new TFile(fi, null);
                        //File.Parent = Me.Parent
                    }
                    Child = tf.Element;
                    ToBeReplaced = true;
                }
            }
        }



        //[System.ComponentModel.Browsable(false)]
        //public Mtx4 IMatrix
        //{
        //    get
        //    {
        //        return imatrix;
        //    }
        //}

        [System.ComponentModel.Browsable(false)]
        public Mtx4 Matrix
        {
            get { return matrix; }
            set
            {
                if (Fixed)
                {
                    return;
                }

                matrix = value;
                //imatrix = matrix; imatrix.Invert();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string NodeName
        {
            get { return _NodeName; }
            set
            {
                _NodeName = value;
                RequestUpdate();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public TAssemblyElement Parent
        {
            get
            {
                if (_ParentLink == null)
                {
                    return null;
                }

                return (TAssemblyElement)_ParentLink.Child;
            }
        }

        [System.ComponentModel.Browsable(false)]
        public TLink ParentLink
        {
            get { return _ParentLink; }
            set
            {
                if (_ParentLink == value)
                {
                    return;
                }

                if (_ParentLink != null)
                {
                    _ParentLink.Links.Remove(this);
                }

                _ParentLink = value;
                if (_ParentLink != null) { _ParentLink.Links.Push(this); }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string PartNumber
        {
            get
            {
                if ((Child != null))
                {
                    return Child.PartNumber;
                }

                return "";
            }
            set
            {
                PN = value;
                if (Child == null)
                {
                    Child = new TAssemblyElement
                    {
                        OwnerLink = this,
                        State = EElementState.Pushed
                    };
                }

                Child.PartNumber = value;
                RequestUpdate();
            }
        }

        [System.ComponentModel.Browsable(false)]
        public int Points
        {
            get
            {
                int functionReturnValue;
                if (Child is TSolidElement)
                {
                    return ((TSolidElement)Child).PointsNumber;
                }
                else
                {
                    functionReturnValue = 0;
                    foreach (TLink link in Links.Values)
                    {
                        functionReturnValue += link.Points;
                    }
                    return functionReturnValue;
                }
            }
        }

        [System.ComponentModel.Browsable(false)]
        public TLink TopLink
        {
            get
            {
                if (Parent == null)
                {
                    return this;
                }
                else
                {
                    return Parent.OwnerLink.TopLink;
                }
            }
        }

        public bool Transparent
        {
            get
            {
                return (Child is TSolidElement s && s.Transparent);
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static void AttachElmt(TLink iElmtLink, TLink iLink)
        {
            TLink link = new TLink
            {
                ParentLink = iLink,
                Child = iElmtLink.Child,
                NodeName = iElmtLink.NodeName,
                Matrix = iElmtLink.Matrix
            };
            if (iElmtLink.Child is TSolidElement)
            {
                link.FileName = iElmtLink.FileName;
                link.ToBeReplaced = true;
            }
            foreach (TLink linktmp in iElmtLink.Links.Values)
            {
                AttachElmt(linktmp, link);
            }
        }

        public TLink Find(string iPartNumber, string iNodeName)
        {
            Stack<TLink> lnks = new Stack<TLink>();
            lnks.Push(this);
            while (lnks.Count > 0)
            {
                TLink l = lnks.Pop();
                if ((iPartNumber == null || iPartNumber == l.PartNumber) && (iNodeName == null || iNodeName == l.NodeName))
                {
                    lnks.Clear();
                    GC.SuppressFinalize(lnks);
                    return l;
                }
                foreach (TLink ll in l.Links.Values.Where(o => o != null))
                {
                    lnks.Push(ll);
                }
            }
            return null;
        }

        public TLink Find(Regex iPartNumber, Regex iNodeName)
        {
            Stack<TLink> lnks = new Stack<TLink>();
            lnks.Push(this);
            while (lnks.Count > 0)
            {
                TLink l = lnks.Pop();
                if ((iPartNumber == null || iPartNumber.IsMatch(l.PartNumber)) && (iNodeName == null || iNodeName.IsMatch(l.NodeName)))
                {
                    lnks.Clear();
                    GC.SuppressFinalize(lnks);
                    return l;
                }
                foreach (TLink ll in l.Links.Values.Where(o => o != null))
                {
                    lnks.Push(ll);
                }
            }
            return null;
        }

        public TLink[] FindAll(Regex iPartNumber, Regex iNodeName)
        {
            Stack<TLink> lnks = new Stack<TLink>();
            List<TLink> lst = new List<TLink>();
            lnks.Push(this);
            while (lnks.Count > 0)
            {
                TLink l = lnks.Pop();
                if ((iPartNumber == null || iPartNumber.IsMatch(l.PartNumber)) && (iNodeName == null || iNodeName.IsMatch(l.NodeName)))
                {
                    lst.Add(l);
                }

                foreach (TLink ll in l.Links.Values.Where(o => o != null))
                {
                    lnks.Push(ll);
                }
            }
            lnks.Clear();
            GC.SuppressFinalize(lnks);
            return lst.ToArray();
        }

        public TBaseElement FindElement(string iPn)
        {
            TBaseElement functionReturnValue;
            functionReturnValue = null;
            if (Child == null)
            {
                return null;
            }

            if (Child.PartNumber != null && Child.PartNumber.ToUpperInvariant() == iPn.ToUpperInvariant())
            {
                return Child;
            }
            else
            {
                foreach (TLink link in Links.Values)
                {
                    functionReturnValue = link.FindElement(iPn);
                    if ((functionReturnValue != null))
                    {
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
            }
            return functionReturnValue;
        }

        public TLink FindFromNodeName(string iNodeName)
        {
            TLink functionReturnValue;
            foreach (TLink link in Links.Values)
            {
                if (link == null)
                {
                    break; // TODO: might not be correct. Was : Exit For
                }

                if (link.NodeName == iNodeName)
                {
                    return link;
                }

                if (link.Child is TAssemblyElement)
                {
                    functionReturnValue = link.FindFromNodeName(iNodeName);
                    if ((functionReturnValue != null))
                    {
                        return functionReturnValue;
                    }
                }
            }
            return null;
        }

        public TLink FindFromPartNumber(string iPartNumber)
        {
            TLink functionReturnValue;
            foreach (TLink link in Links.Values)
            {
                if (link == null)
                {
                    break; // TODO: might not be correct. Was : Exit For
                }

                if (link.PartNumber == iPartNumber)
                {
                    return link;
                }

                if (link.Child is TAssemblyElement)
                {
                    functionReturnValue = link.FindFromPartNumber(iPartNumber);
                    if ((functionReturnValue != null))
                    {
                        return functionReturnValue;
                    }
                }
            }
            return null;
        }

        public TLink FindFromPath(string iPath)
        {
            TLink current = this;
            string[] Words = iPath.Split('/');
            for (int i = 0; i < Words.Length; i++)
            {
                TLink inst = current.Links.Values.FirstOrDefault(o => o.NodeName == Words[i]);
                if (inst == null)
                {
                    return null;
                }

                current = inst;
            }
            return current;
        }

        public Vec4 GetClosestPointForPointFromSurface(Vec4 iPt)
        {
            Mtx4 mt = Mtx4.InvertL(Matrix);

            //mt.Invert();
            iPt = Vec4.Transform(iPt, mt);
            //
            Vec4? BestPt;
            if (Child is TSolidElement)
            {
                BestPt = ((TSolidElement)Child).GetClosestPointForPointFromSurface(iPt);
            }
            else
            {
                BestPt = default;
                foreach (TLink link in Links.Values)
                {
                    if (link.Enabled)
                    {
                        Vec4? pt = link.GetClosestPointForPointFromSurface(iPt);
                        if (!pt.HasValue)
                        {
                        }
                        else if (!BestPt.HasValue)
                        {
                            BestPt = pt;
                        }
                        else if ((BestPt.Value - iPt).LengthSquared > (pt.Value - iPt).LengthSquared)
                        {
                            BestPt = pt;
                        }
                    }
                }
            }
            //
            if (!BestPt.HasValue)
            {
                return new Vec4();
            }

            return Vec4.Transform(BestPt.Value, Matrix);
        }

        public IEnumerator GetEnumerator()
        {
            return Links.Values.Where(o => o != null).GetEnumerator();
        }

        public bool InSelectionTree()
        {
            if (Selected())
            {
                return true;
            }

            foreach (TLink l in Links.Values)
            {
                if (l.InSelectionTree())
                {
                    return true;
                }
            }

            return false;
        }

        public TLink Item(string iNodeName)
        {
            foreach (TLink link in Links.Values)
            {
                if (System.Text.RegularExpressions.Regex.Match(link.NodeName, iNodeName).Success)
                {
                    return link;
                }
            }
            return null;
        }


        public void RequestUpdate()
        {
            ToBeUpdated = true;
        }

        public void RestoreLocation()
        {
            if (!_OriginMatrix.HasValue)
            {
                throw new Exception("Position not saved!");
            }

            Matrix = _OriginMatrix.Value;
            _OriginMatrix = null;
            foreach (TLink l in Links.Values)
            {
                if (l != null)
                {
                    l.RestoreLocation();
                }
            }
        }

        public void SaveLocation()
        {
            if (_OriginMatrix.HasValue)
            {
                throw new Exception("Position already saved!");
            }

            _OriginMatrix = Matrix;
            foreach (TLink l in Links.Values)
            {
                if (l != null)
                {
                    l.SaveLocation();
                }
            }
        }

        public bool Selected()
        {
            foreach (TLink l in TManager.SelectedLinks)
            {
                if (l == this)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ShouldSerializeLinksForSerialization()
        {
            return (Links.Max > 0);
        }

        public bool ShouldSerializeMatrix()
        {
            return !Matrix.Equals(Mtx4.Identity);
        }

        public override string ToString()
        {
            return NodeName + " (" + PartNumber + ")";
        }

        internal void DoCompute()
        {

            // TLink[] links = Array.FindAll(Links.Values, o => o != null && o.Enabled && !o.ToBeReplaced);
            ////
            //   TLink[] ls = Array.FindAll(links, o => o.Child is TAssemblyElement);



            Array.ForEach(Links.Assemblies, l =>
            {
                l.Giving.State = l.State | State;
                l.Giving.Matrix = l.Matrix * Giving.Matrix;
                l.DoCompute();
            });

            // Array.ForEach(Array.FindAll(links, o => o.Solid != null && o.Solid.State == EElementState.Pushed && o.DrawInTree), link =>
            Array.ForEach(Array.FindAll(Links.Solids, o => 
            o.enabled && !o.ToBeReplaced && o.Solid.State == EElementState.Pushed && o.DrawInTree), link =>
            {
                link.Giving.Matrix = link.Matrix * Giving.Matrix;
                link.Giving.State = Giving.State | link.State;
                //ss = link.NodeName;
                TSolidElement s = link.Solid;
                SBoundingBox3 box = SBoundingBox3.Transform(s.BoundingBox, link.Giving.Matrix);
                double zmin = -box.MaxPosition.Z;
                double zmax = -box.MinPosition.Z;
                //
                box = box.Get2DBox(TManager.BaseLayer.PMatrix);
                Vec4 v1 = box.MinPosition;
                Vec4 v2 = box.MaxPosition;
                if (v1.X < 1 && v1.Y < 1 && v2.X > -1 && v2.Y > -1 && v2.X - v1.X > TManager.WidthCulling && v2.Y - v1.Y > TManager.HeightCulling)
                {
                    foreach (TShape sp in s.Shapes)
                    {
                        box = SBoundingBox3.Transform(sp.BoundingBox, link.Giving.Matrix);
                        box = box.Get2DBox(TManager.BaseLayer.PMatrix);
                        v1 = box.MinPosition;
                        v2 = box.MaxPosition;
                        if (v2.X - v1.X > TManager.WidthCulling && v2.Y - v1.Y > TManager.HeightCulling)
                        {
                            TManager.BaseLayer.SolidsBuffer.Push(link.Giving);
                            //
                            link.Giving.Selected = false;
                            if (link.DisplayColor.HasValue)
                                link.Giving.Color = link.DisplayColor.Value;
                            else if (link.CanBeHighlighted && link.State.HasFlag(ELinkState.Selected))
                            {
                                link.Giving.Color = TManager.ColorSelected;
                                link.Giving.Selected = true;
                            }
                            else if (link.CanBeHighlighted && link.State.HasFlag(ELinkState.OverFlown))
                                link.Giving.Color = TManager.ColorOverFlown;
                            else
                                link.Giving.Color = TManager.ColorNormal;
                            //
                            if (link.NoEffect)
                                link.Giving.NoEffectValue = 1;
                            else
                                link.Giving.NoEffectValue = 0;
                            if (link.NoDiffuse)
                                link.Giving.NoDiffuseValue = 1;
                            else
                                link.Giving.NoDiffuseValue = 0;
                            //
                            Array.ForEach(TManager.Layers, layer =>
                             {
                               if (zmax >= layer.NearDistance && zmin <= layer.FarDistance)
                               {
                                   layer.SolidsBuffer.Push(link.Giving);
                               }
                           });
                            break;
                        }
                    }
                }
            });
        }


        #endregion Public Methods
    }

    public class TLinkChlidComparer : IEqualityComparer<TLink>
    {
        #region Public Fields

        public static readonly TLinkChlidComparer Default = new TLinkChlidComparer();

        #endregion Public Fields

        #region Public Methods

        public bool Equals(TLink x, TLink y)
        {
            return x.Child == y.Child;
        }

        public int GetHashCode(TLink obj)
        {
            return obj.Child.GetHashCode();
        }

        #endregion Public Methods
    }
}