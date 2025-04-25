
using Math3D;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace CiliaElements.Elements.Control
{
    public class TTreeView : TControl
    {
        #region Public Fields

        public int LineGap;
        public IEnumerable RootObject;

        public SelectedObjectsChangedDelegate SelectedObjectsChanged;

        #endregion Public Fields

        #region Private Fields

        private static StringFormat FrmtNN = new StringFormat() { LineAlignment = StringAlignment.Near, Alignment = StringAlignment.Near };
        private List<TTreeNode> Nodes = new List<TTreeNode>();
        private float offsetY = 0;
        private TTreeNode OverFlownEnablingNode;

        private TTreeNode OverFlownExpandingNode;

        private TTreeNode OverFlownNode;

        #endregion Private Fields

        #region Public Constructors

        public TTreeView(string iPartNumber) : base(iPartNumber)
        {
        }

        #endregion Public Constructors

        #region Public Delegates

        public delegate void SelectedObjectsChangedDelegate(object[] objs);

        #endregion Public Delegates

        #region Private Enums

        private enum ETreeNodeExpanded
        {
            None, Expanded, Collapsed
        }

        #endregion Private Enums

        #region Public Methods
      

     
        public override bool Visible
        {
            get => base.Visible; set
            {
                base.Visible = value; 
                if (value)
                {
                    RedrawThread = TManager.CreateThread(ToRedrawTimer, "ToRedrawTimer TreeView", ThreadPriority.Lowest);
                }
                else
                    RedrawThread?.Abort();
            }
        }
         
        public override void Click()
        {
            if (OverFlownExpandingNode != null)
            {
                if (OverFlownExpandingNode.Obj is IExpandable)
                {
                    IExpandable i = (IExpandable)OverFlownExpandingNode.Obj;
                    i.Expanded = !i.Expanded;
                }
                return;
            }

            if (OverFlownEnablingNode != null)
            {
                if (OverFlownEnablingNode.Obj is IEnable)
                {
                    IEnable i = (IEnable)OverFlownEnablingNode.Obj;
                    i.Enabled = !i.Enabled;
                }
                return;
            }

            if (!TManager.KeyboardModifiers.HasFlag(EKeybordModifiers.Ctrl))
            {
                foreach (TTreeNode n in Nodes.Where(o => o.Selected && o != OverFlownNode))
                {
                    n.Selected = false;
                }
            }

            if (OverFlownNode != null && !OverFlownNode.Selected)
            {
                OverFlownNode.Selected = true;
                List<object> objs = new List<object>();
                if (SelectedObjectsChanged != null)
                {
                    foreach (TTreeNode n in Nodes.Where(o => o.Selected))
                    {
                        objs.Add(n.Obj);
                    }

                    SelectedObjectsChanged.Invoke(objs.ToArray());
                }
            }
        }

        public override void Compute(Graphics grp)
        {
            grp.TranslateTransform(0, offsetY);

            LineGap = (int)(Font.Size * 2);

            int y = 5, x = 5;

            TTreeNode n = Nodes.FirstOrDefault(o => o.Obj == RootObject);
            if (n == null) { n = new TTreeNode() { X = x, Y = y }; Nodes.Add(n); }
            n.Obj = RootObject;

            ComputeNode(n, ref y, ref x);

            List<TTreeNode> stc = new List<TTreeNode>();
            stc.Add(new TTreeNode() { Obj = RootObject });

            foreach (TTreeNode t in Nodes.ToArray())
            {
                SizeF szf = grp.MeasureString(t.Obj.ToString(), Font);
                //if (szf.Width + 10 > Width)
                //{
                //    Width = (int)(szf.Width + 10);
                //}
                if (t.Selected)
                {
                    grp.FillRectangle(SelectedBackBrush, t.X + 32, t.Y, szf.Width, szf.Height);
                }
                else if (t == OverFlownNode)
                {
                    grp.FillRectangle(OverFlownBackBrush, t.X + 32, t.Y, szf.Width, szf.Height);
                }

                if (t == OverFlownExpandingNode)
                {
                    grp.FillRectangle(OverFlownBackBrush, t.X, t.Y - 1, 12, 12);
                }

                if (t == OverFlownEnablingNode)
                {
                    grp.FillRectangle(OverFlownBackBrush, t.X + 16, t.Y - 1, 12, 12);
                }

                RectangleF r = new RectangleF(t.X + 32, t.Y, szf.Width, szf.Height);
                if (r.Right > grp.ClipBounds.Width * 0.5 + 2)
                {
                    r.Width -= (int)(r.Right - grp.ClipBounds.Width * 0.5 - 2);
                }

                grp.DrawString(t.Obj.ToString(), Font, Brushes.White, r, FrmtNN);

                if (t.Obj is IEnable)
                {
                    IEnable i = (IEnable)t.Obj;
                    grp.DrawRectangle(Pens.Turquoise, t.X + 16, t.Y - 1, 12, 12);
                    if (i.Enabled)
                    {
                        grp.DrawLine(Pens.Turquoise, t.X + 19, t.Y + 5, t.X + 23, t.Y + 9);
                        grp.DrawLine(Pens.Turquoise, t.X + 25, t.Y + 2, t.X + 23, t.Y + 9);
                    }
                }

                if (t.HasChilds)
                {
                    grp.DrawLine(Pens.Yellow, t.X + 9, t.Y + 5, t.X + 3, t.Y + 5);
                    if (t.TreeNodeExpanded == ETreeNodeExpanded.Collapsed)
                    {
                        grp.DrawLine(Pens.Yellow, t.X + 6, t.Y + 2, t.X + 6, t.Y + 8);
                    }
                    grp.DrawLine(Pens.Yellow, t.X - 2, t.Y + 5, t.X - 7, t.Y + 5);
                    grp.DrawRectangle(Pens.Yellow, t.X, t.Y - 1, 12, 12);
                }
                else
                {
                    grp.DrawLine(Pens.Yellow, t.X + 12, t.Y + 5, t.X - 7, t.Y + 5);
                }

                if (t.Prev != null)
                {
                    grp.DrawLine(Pens.Yellow, t.X - 7, t.Y + 5, t.X - 7, t.Prev.Y + 5);
                }
                else if (t.Parent != null)
                {
                    grp.DrawLine(Pens.Yellow, t.X - 7, t.Y + 5, t.X - 7, t.Parent.Y + LineGap - 3);
                }
                //if (t.o is IEnumerable)
                //{
                //    IEnumerable i = (IEnumerable)t.o;
                //    if (i.i.GetEnumerator().Current != null)
                //    {
                //        grp.DrawLine(Pens.White, 5, y + szf.Height, 5 + szf.Width, y + szf.Height);
                //        y += 2;
                //    }
                //}
            }
        }

        public override void MouseMove(Vec3 p)
        {
            TTreeNode[] nds;
            lock (Nodes) nds = Nodes.ToArray();
            Vec2 v = new Vec2(p.X, 1 - p.Z); v.X *= Width; v.Y *= Height; v.Y -= offsetY;
            OverFlownNode = nds.FirstOrDefault(o => o.X + 32 < v.X && o.Y < v.Y && o.Y + LineGap > v.Y);
            OverFlownExpandingNode = nds.FirstOrDefault(o => o.X < v.X && o.X + 16 > v.X && o.Y < v.Y && o.Y + LineGap > v.Y);
            OverFlownEnablingNode = nds.FirstOrDefault(o => o.X + 16 < v.X && o.X + 32 > v.X && o.Y < v.Y && o.Y + LineGap > v.Y);
        }

        #endregion Public Methods

        #region Internal Methods

        public override void MouseDrag(double dx, double dy)
        {
            offsetY -= (float)(dy * Height);
        }

        #endregion Internal Methods

        #region Private Methods

        private void ComputeNode(TTreeNode t, ref int y, ref int x)
        {
            if (t.Obj is IExpandable)
            {
                IExpandable i = (IExpandable)t.Obj;
                if (i.Expanded)
                {
                    t.TreeNodeExpanded = ETreeNodeExpanded.Expanded;
                    TTreeNode prev = null;
                    x += LineGap;
                    foreach (object oo in i)
                    {
                        if (!(oo is IDrawnInTree && !((IDrawnInTree)oo).DrawInTree))
                        {
                            t.HasChilds = true;
                            y += LineGap;

                            TTreeNode n = Nodes.FirstOrDefault(o => o.Obj == oo);
                            if (n == null) { n = new TTreeNode(); Nodes.Add(n); }
                            n.Obj = oo;
                            n.L = t.L + 1;
                            n.Prev = prev;
                            n.X = x;
                            n.Y = y;
                            n.Parent = t;
                            if (prev != null)
                            {
                                prev.Next = n;
                            }
                            ComputeNode(n, ref y, ref x);
                            prev = n;
                        }
                    }
                    x -= LineGap;
                }
                else
                {
                    t.TreeNodeExpanded = ETreeNodeExpanded.Collapsed;
                    Stack<TTreeNode> nds = new Stack<TTreeNode>(); nds.Push(t);
                    while (nds.Count > 0)
                    {
                        TTreeNode n = nds.Pop();
                        foreach (TTreeNode nn in Nodes.Where(o => o.Parent == n).ToArray())
                        {
                            Nodes.Remove(nn);
                            nds.Push(nn);
                        }
                    }
                    foreach (object oo in i)
                    {
                        if (!(oo is IDrawnInTree && !((IDrawnInTree)oo).DrawInTree))
                        {
                            t.HasChilds = true;
                        }
                    }
                }
            }
            else
            {
                t.TreeNodeExpanded = ETreeNodeExpanded.None;
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class TTreeNode
        {
            #region Public Fields

            public bool HasChilds = false;
            public int L;
            public TTreeNode Next;
            public object Obj;
            public TTreeNode Parent;
            public TTreeNode Prev;
            public ETreeNodeExpanded TreeNodeExpanded;
            public int X;
            public int Y;

            #endregion Public Fields

            #region Internal Fields

            internal bool Selected;

            #endregion Internal Fields
        }

        #endregion Private Classes
    }
}