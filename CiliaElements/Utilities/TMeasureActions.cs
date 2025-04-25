
using CiliaElements.Elements.Control;
using Math3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CiliaElements
{
    public class TMeasuredPoint
    {
        #region Public Fields

        public TLink Link;
        public TVectorPanel Panel;
        public Vec3 RelativePoint;

        #endregion Public Fields

        #region Private Fields

        private Vec3 absolutePoint;

        private TLink panelLink;

        private Vec4 viewPoint;

        #endregion Private Fields

        #region Public Constructors

        public TMeasuredPoint()
        {
            Panel = new TVectorPanel("Measure") { Title = "Measure", Height = 80, Width = 110 };
            panelLink = TManager.AttachElmt(Panel.OwnerLink, TManager.View.OwnerLink, null);
            panelLink.NodeName = "Loading Panel";
            panelLink.Enabled = false;
        }

        #endregion Public Constructors

        #region Public Properties

        public Vec3 AbsolutePoint
        {
            get
            {
                return absolutePoint;
            }
            set
            {
                absolutePoint = value;
                Mtx4 M =Mtx4.InvertL( Link.AbsoluteMatrix);
                //M.Invert();
                RelativePoint = Vec4.TransformPoint(value, M);
            }
        }

        public Vec4 ViewPoint
        {
            get
            {
                return viewPoint;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public void Update()
        {
            absolutePoint = Vec4.TransformPoint(RelativePoint, Link.AbsoluteMatrix);
            viewPoint = Vec4.Transform(absolutePoint, TManager.BaseLayer.PVMatrix);
            viewPoint /= viewPoint.W;
        }

        #endregion Public Methods
    }

    public class TMeasuredVector
    {
        #region Public Fields

        public bool MinimalMode;

        public TMeasuredPoint P1;

        public TMeasuredPoint P2;

        public TVectorPanel Panel;

        #endregion Public Fields

        #region Private Fields

        private static Random rnd = new Random();
        private TLink panelLink;
        private Vec3 vector;

        #endregion Private Fields

        #region Public Constructors

        public TMeasuredVector()
        {
            Panel = new TVectorPanel("Measure") { Title = "Measure", Height = 80, Width = 110 };
            panelLink = TManager.AttachElmt(Panel.OwnerLink, TManager.View.OwnerLink, null);
            panelLink.NodeName = "Loading Panel";
            panelLink.Enabled = false;
        }

        #endregion Public Constructors

        #region Public Properties

        public Vec3 Vector { get { return vector; } }

        #endregion Public Properties

        #region Public Methods

        public void Update()
        {
            if (MinimalMode)
            {
                List<Vec3> pts1 = new List<Vec3>();
                FillPointsList(P1.Link, pts1, P1.Link.ParentLink.Giving.Matrix);
                List<Vec3> pts2 = new List<Vec3>();
                FillPointsList(P2.Link, pts2, P2.Link.ParentLink.Giving.Matrix);
                //
                pts1 = pts1.Distinct().ToList();
                pts2 = pts2.Distinct().ToList();
                //

                //
                Stopwatch w = new Stopwatch(); ;

                //
                w.Reset(); w.Start();
                Vec3 pp1 = new Vec3();
                Vec3 pp2 = new Vec3();
                Vec3 pp;
                //foreach (Vec3 p1 in pts1)
                //    foreach (Vec3 p2 in pts2)
                //    {
                //        double d = (p1 - p2).LengthSquared;
                //        if (d < MinDistance)
                //        {
                //            MinDistance = d;
                //            pp1 = p1;
                //            pp2 = p2;
                //        }
                //    }
                //w.Stop();
                //double MinDistanceBack = MinDistance;
                //P1.AbsolutePoint = pp1;
                //P2.AbsolutePoint = pp2;

                //
                w.Reset(); w.Start();
                bool bRemove = true;
                int idx = 0;
                List<Vec3> buf;
                int is2 = 0;
                int is1 = 0;
                double l1;
                double l2 = double.MaxValue;
                double l;
                double nl;
                int idx2 = 0;
                while (bRemove)
                {
                    idx++;
                    bRemove = false;
                    for (int ii1 = 0; ii1 < 2; ii1++)
                    {
                        is1 = rnd.Next(pts1.Count - 1);
                        is2 = rnd.Next(pts2.Count - 1);
                        pp1 = pts1[is1];
                        pp2 = pts2[is2];
                        //
                        l1 = (pp2 - pp1).LengthSquared;
                        l = Math.Sqrt(l1);
                        nl = -l;
                        for (int i = pts2.Count - 1; i > -1; i--)
                        {
                            Vec3 p = pts2[i] - pp1;
                            if (p.X < l && p.Y < l && p.Z < l && p.X > nl && p.Y > nl && p.Z > nl && p.LengthSquared < l1)
                            {
                                pp2 = pts2[i];
                                l1 = p.LengthSquared;
                                l = Math.Sqrt(l1);
                                nl = -l;
                                idx2++;
                            }
                        }
                        //
                        l2 = l1;
                        for (int i = pts1.Count - 1; i > -1; i--)
                        {
                            Vec3 p = pts1[i] - pp2;
                            if (p.X < l && p.Y < l && p.Z < l && p.X > nl && p.Y > nl && p.Z > nl && p.LengthSquared < l2)
                            {
                                pp1 = pts1[i];
                                l2 = p.LengthSquared;
                                l = Math.Sqrt(l2);
                                nl = -l;
                                idx2++;
                            }
                        }
                        //
                        //
                        if (l2 != l1)
                        {
                            //pp1 = pts1[i1]; pts1.RemoveAt(i1);
                            pp = pts1[is1];
                            pts1.RemoveAt(is1);
                            l = Math.Sqrt(l1) - Math.Sqrt(l2);
                            l1 = l * l;
                            nl = -l;
                            //
                            for (int i = pts1.Count - 1; i > -1; i--)
                            {
                                Vec3 p = pts1[i] - pp;
                                if (p.X < l && p.Y < l && p.Z < l && p.X > nl && p.Y > nl && p.Z > nl && p.LengthSquared < l1)
                                {
                                    pts1.RemoveAt(i);
                                }
                            }
                            bRemove = true;
                            //pts1.Insert(0, pp1);
                        }
                        //
                        //pts1.Reverse();
                        buf = pts1; pts1 = pts2; pts2 = buf;
                        //pts2.Reverse();
                    }
                }
                //w.Stop();
                //w.Reset(); w.Start();
                pp1 = pts1[0];
                pp2 = pts2[0];
                l1 = (pp1 - pp2).LengthSquared;
                l = Math.Sqrt(l1);
                nl = -l;
                foreach (Vec3 p1 in pts1)
                {
                    foreach (Vec3 p2 in pts2)
                    {
                        Vec3 p = p2 - p1;
                        if (p.X < l && p.Y < l && p.Z < l && p.X > nl && p.Y > nl && p.Z > nl && p.LengthSquared < l1)
                        {
                            pp1 = p1;
                            pp2 = p2;
                            l1 = (pp1 - pp2).LengthSquared;
                            l = Math.Sqrt(l1);
                            nl = -l;
                        }
                    }
                }

                w.Stop();
                P1.AbsolutePoint = pp1;
                P2.AbsolutePoint = pp2;

                //if (MinDistanceBack != MinDistance)
                //{
                //    throw new Exception("miss!");
                //}
            }

            P1.Update();
            P2.Update();
            vector = P2.AbsolutePoint - P1.AbsolutePoint;
        }

        #endregion Public Methods

        #region Private Methods

        private void FillPointsList(TLink l, List<Vec3> pts, Mtx4 iMatrix)
        {
            iMatrix = l.Matrix * iMatrix;
            if (l.Child is TAssemblyElement)
            {
                foreach (TLink ll in l.Links.Values)
                {
                    FillPointsList(ll, pts, iMatrix);
                }
            }
            else
            {
                pts.AddRange(Array.ConvertAll(((TSolidElement)l.Child).DataPositions, iMatrix.TransformPoint));
            }
        }

        #endregion Private Methods
    }

    public abstract class TMeasureMinimalVectorAction
    {
        #region Public Fields

        public static List<TMeasuredVector> MeasuredVectors = new List<TMeasuredVector>();

        #endregion Public Fields

        #region Private Fields

        private static TMeasuredPoint p1 = null;

        #endregion Private Fields

        #region Public Methods

        public static void Abort()
        {
            TManager.NodeClicked -= NodeClickedEventHandler;
            p1 = null;
        }

        public static void NodeClickedEventHandler(TLink ClickedLink)
        {
            TManager.NodeClicked -= NodeClickedEventHandler;
            if (ClickedLink == null)
            {
                Perform();
            }
            else if (p1 == null)
            {
                p1 = new TMeasuredPoint() { Link = ClickedLink };
                Perform();
            }
            else
            {
                MeasuredVectors.Add(new TMeasuredVector() { P1 = p1, P2 = new TMeasuredPoint() { Link = ClickedLink }, MinimalMode = true });
                p1 = null;
                TManager.PendingAction = null;
            }
        }

        public static void Perform()
        {
            TManager.NodeClicked += NodeClickedEventHandler;
        }

        #endregion Public Methods
    }

    public abstract class TMeasurePointAction
    {
        #region Public Fields

        public static List<TMeasuredPoint> MeasuredPoints = new List<TMeasuredPoint>();

        #endregion Public Fields

        #region Public Methods

        public static void Abort()
        {
            TManager.PointClicked -= PointClickedEventHandler;
        }

        public static void Perform()
        {
            TManager.PointClicked += PointClickedEventHandler;
        }

        public static void PointClickedEventHandler(TLink ClickedLink, Vec3? ClickedPoint)
        {
            TManager.PointClicked -= PointClickedEventHandler;
            if (ClickedLink == null || ClickedPoint == null)
            {
                Perform();
            }
            else
            {
                MeasuredPoints.Add(new TMeasuredPoint() { Link = ClickedLink, AbsolutePoint = ClickedPoint.Value });
                TManager.PendingAction = null;
            }
        }

        #endregion Public Methods
    }

    public abstract class TMeasureVectorAction
    {
        #region Public Fields

        public static List<TMeasuredVector> MeasuredVectors = new List<TMeasuredVector>();

        #endregion Public Fields

        #region Private Fields

        private static TMeasuredPoint p1 = null;

        #endregion Private Fields

        #region Public Methods

        public static void Abort()
        {
            TManager.PointClicked -= PointClickedEventHandler;
            p1 = null;
        }

        public static void Perform()
        {
            TManager.PointClicked += PointClickedEventHandler;
        }

        public static void PointClickedEventHandler(TLink ClickedLink, Vec3? ClickedPoint)
        {
            TManager.PointClicked -= PointClickedEventHandler;
            if (ClickedLink == null || ClickedPoint == null)
            {
                Perform();
            }
            else if (p1 == null)
            {
                p1 = new TMeasuredPoint() { Link = ClickedLink, AbsolutePoint = ClickedPoint.Value };
                Perform();
            }
            else
            {
                MeasuredVectors.Add(new TMeasuredVector() { P1 = p1, P2 = new TMeasuredPoint() { Link = ClickedLink, AbsolutePoint = ClickedPoint.Value } });
                p1 = null;
                TManager.PendingAction = null;
            }
        }

        #endregion Public Methods
    }
}