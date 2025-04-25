using CiliaElements.Elements.Internals;
using CiliaElements.Elements.Math;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static CiliaElements.TAsteroid;

namespace CiliaElements
{
    public class TDummyAsteroid : TInternal
    {
        #region Public Fields

        public static TAsteroid asteroid;

        public List<TFacetLink> Facets = new List<TFacetLink>();
        public TRandom Random = new TRandom();

        #endregion Public Fields

        #region Public Constructors

        public TDummyAsteroid(int iIndex, TLink pLink) : base("DummyAst" + iIndex.ToString())
        {
            Random.Position = iIndex;
            HandleVAO = asteroid.HandleVAO;
            HandleIndexes = asteroid.HandleIndexes;

            textureBmp = new Bitmap(TSolidElementConstruction.TextRange, TSolidElementConstruction.TextRange);
            Graphics grp = Graphics.FromImage(textureBmp);
            Color BackColor = Color.FromArgb(128, 5, 5, 5);
            grp.CompositingQuality = CompositingQuality.AssumeLinear;
            grp.InterpolationMode = InterpolationMode.Bilinear;
            grp.SmoothingMode = SmoothingMode.HighSpeed;
            grp.Clear(Color.FromArgb(255, 55, 55, 55));
            for (int i = 0; i < 100; i++)
            {
                Rectangle r = new Rectangle(
                    (int)(240 * (Random.NextUFloat() - 1) + 8),
                    (int)(512 * Random.NextUFloat() - 128),
                    (int)(TSolidElementConstruction.TextRange * Random.NextUFloat() + 10),
                    (int)(TSolidElementConstruction.TextRange * Random.NextUFloat()) + 10);
                float start = (int)(360 * Random.NextUFloat());
                float arc = (int)(360 * Random.NextUFloat());
                Pen pn = new Pen(Color.FromArgb((int)(256 * Random.NextUFloat()), (int)(256 * Random.NextUFloat()), (int)(256 * Random.NextUFloat()), (int)(256 * Random.NextUFloat())), (int)(TSolidElementConstruction.TextRange * Random.NextFloat()));
                //grp.DrawLine(pn, r.Left, r.Top, r.Right, r.Bottom);
                grp.DrawArc(pn, r, start, arc);
                r.X += 240;
                //grp.DrawLine(pn, r.Left, r.Top, r.Right, r.Bottom);
                grp.DrawArc(pn, r, start, arc);
                r.X += 240;
                //grp.DrawLine(pn, r.Left, r.Top, r.Right, r.Bottom);
                grp.DrawArc(pn, r, start, arc);
                pn.Dispose();
            }
            //switch (iIndex)
            //{
            //    case 43732:
            //    case 33295:
            //    case 10680:
            //        grp.DrawString("Galon's\nLab", FT50, Brushes.Red, 0, 0);
            //        grp.DrawString("Galon's\nLab", FT50, Brushes.Red, 128, 128);
            //        break;
            //}
            grp.Dispose();
            //
            List<int> indexes = new List<int>();
            int nb;
            for (int k = 0; k < 4; k++)
            {
                nb = 2 + (int)((20 - indexes.Count) * 0.25 * Random.NextUDouble());
                for (int i = 0; i < nb; i++)
                {
                    int j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                    while (indexes.Contains(j)) j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                    TFacetLink fl = new TFacetLink() { Facet = TAsteroid.Facets[j], Link = new TLink() { NodeName = j.ToString(), ParentLink = pLink, Child = TPlain.Resources[k], Matrix = TAsteroid.Facets[j].GroundMatrix } };
                    Facets.Add(fl); indexes.Add(j);
                }
            }
            for (int k = 4; k < 8; k++)
            {
                nb = (int)((20 - indexes.Count) * 0.25 * Random.NextUDouble());
                for (int i = 0; i < nb; i++)
                {
                    int j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                    while (indexes.Contains(j)) j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                    TFacetLink fl = new TFacetLink() { Facet = TAsteroid.Facets[j], Link = new TLink() { NodeName = j.ToString(), ParentLink = pLink, Child = TPlain.Resources[k], Matrix = TAsteroid.Facets[j].GroundMatrix } };
                    Facets.Add(fl); indexes.Add(j);
                }
            }

            nb = (int)((TAsteroid.Facets.Length - indexes.Count) * (0.1 + 0.5 * Random.NextUDouble()));
            for (int i = 0; i < nb; i++)
            {
                int j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                while (indexes.Contains(j)) j = (int)(TAsteroid.Facets.Length * Random.NextUDouble());
                TFacetLink fl = new TFacetLink() { Facet = TAsteroid.Facets[j], Link = new TLink() { NodeName = j.ToString(), ParentLink = pLink, Child = TPlain.Default, Matrix = TAsteroid.Facets[j].GroundMatrix } };
                Facets.Add(fl); indexes.Add(j);
            }
            for (int i = 0; i < TAsteroid.Facets.Length; i++)
            {
                if (!indexes.Contains(i))
                {
                    TFacetLink fl = new TFacetLink() { Facet = TAsteroid.Facets[i], Link = new TLink() { NodeName = i.ToString(), ParentLink = pLink, Child = TWater.Default, Matrix = TAsteroid.Facets[i].GroundMatrix } };
                    Facets.Add(fl);
                }
            }
            //
            //bmp.Save("c:\\temp\\" + iIndex.ToString() + ".png", ImageFormat.Png);
            TManager.DummyAsteroids.Push(this);
        }

        #endregion Public Constructors

        #region Public Properties

        public override SBoundingBox3 BoundingBox { get => asteroid.BoundingBox; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int[] DataIndexes { get => asteroid.DataIndexes; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override Vec3[] DataPositions { get => asteroid.DataPositions; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int FacesNumber { get => asteroid.FacesNumber; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int FacesStart { get => asteroid.FacesStart; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int FacesStarter { get => asteroid.FacesStarter; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int LinesNumber { get => asteroid.LinesNumber; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int LinesStart { get => asteroid.LinesStart; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int LinesStarter { get => asteroid.LinesStarter; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int PointsNumber { get => asteroid.PointsNumber; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int PointsStart { get => asteroid.PointsStart; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override int PointsStarter { get => asteroid.PointsStarter; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override TShape[] Shapes { get => asteroid.Shapes; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override EElementState State { get => asteroid.State; set => throw new Exception("Dummy Asteroid must not be modified!"); }

        public override TQuickStc<TShape> Surfaces { get => asteroid.Surfaces; set { GC.SuppressFinalize(value); } }

        #endregion Public Properties

        #region Public Classes

        public class TFacetLink
        {
            #region Public Fields

            public TFacet Facet;
            public TLink Link;

            #endregion Public Fields
        }

        #endregion Public Classes
    }
}