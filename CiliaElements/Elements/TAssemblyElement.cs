
using Math3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace CiliaElements
{
    public class TAssemblyElement : TBaseElement
    {
        #region Public Constructors

        public TAssemblyElement()
        {
            OwnerLink = new TLink { Child = this };

            ElementLoader = new TAssemblyElementLoader() { Element = this };
        }

        #endregion Public Constructors

        #region Public Methods

        public override FileInfo Pack()
        {
            TManager.PushConsole();
            TManager.WriteLineConsole("Packing of assembly " + PartNumber);
            TManager.PushConsole();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FileInfo fi = new FileInfo(TManager.DocumentsDirectory.FullName + "\\Assys\\" + PartNumber + ".CiliaSZ");
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            StreamWriter wtr = new StreamWriter(fi.FullName);
            GZipStream cwtr = new GZipStream(wtr.BaseStream, CompressionLevel.Optimal);
            BinaryWriter bwtr = new BinaryWriter(cwtr);

            Vec3[] DataPositions = { };
            Vec3[] DataNormals = { };
            Vec2f[] DataTexts = { };
            int[] FacesIndexes = { };
            int[] LinesIndexes = { };
            int[] PointsIndexes = { };
            int[] DataIndexes = { };
            int FacesNumber = 0;
            int LinesNumber = 0;
            int PointsNumber = 0;
            TQuickStc<TShape> Surfaces = new TQuickStc<TShape>();
            TQuickStc<TShape> Curves = new TQuickStc<TShape>();

            SBoundingBox3 bb = new SBoundingBox3();

            Stack<TLink> lnks = new Stack<TLink>(new TLink[] { OwnerLink });
            while (lnks.Count > 0)
            {
                TLink l = lnks.Pop();
                foreach (TLink ll in l.Links.Values.Where(o => o != null))
                {
                    lnks.Push(ll);
                }
                if (l.Solid != null)
                {
                    TSolidElement solid = l.Solid;
                    TManager.WriteLineConsole("Pushing " + solid.PartNumber);
                    if (bb.Size.LengthSquared == 0)
                    {
                        bb = solid.BoundingBox;
                    }
                    else { bb += solid.BoundingBox; }
                    int npb = DataPositions.Length;
                    int npa = npb + solid.DataPositions.Length;
                    Array.Resize(ref DataPositions, npa);
                    Array.Resize(ref DataNormals, npa);
                    Array.Resize(ref DataTexts, npa);
                    Array.Copy(solid.DataPositions, 0, DataPositions, npb, npa - npb);
                    Array.Copy(solid.DataNormals, 0, DataNormals, npb, npa - npb);
                    Array.Copy(solid.DataTexts, 0, DataTexts, npb, npa - npb);
                    int nb = FacesIndexes.Length;
                    int na = nb + solid.FacesNumber;
                    Array.Resize(ref FacesIndexes, na);
                    for (int i = 0; i < solid.FacesNumber; i++)
                    {
                        FacesIndexes[nb + i] = solid.DataIndexes[i] + npb;
                    }
                    foreach (TShape surf in solid.Surfaces.Values.Where(o => o != null))
                    {
                        TShape nsurf = new TShape
                        {
                            BoundingBox = surf.BoundingBox,
                            IndexesEnd = nb + surf.IndexesEnd,
                            IndexesStart = nb + surf.IndexesStart,
                        };
                        Surfaces.Push(nsurf);
                    }
                }
            }
            Surfaces.Close();
            Curves.Close();
            DataIndexes = FacesIndexes;
            FacesNumber = DataIndexes.Length;

            Bitmap textureBmp = new Bitmap(4, 4);
            Graphics grp = Graphics.FromImage(textureBmp);
            grp.Clear(Color.Black);
            grp.FillRectangle(Brushes.Blue, 0, 0, 4, 2);
            grp.Dispose();

            TSolidElement.CompressPack(bwtr, DataPositions, DataNormals, DataTexts, DataIndexes, FacesNumber, LinesNumber, PointsNumber, Surfaces, Curves, textureBmp);

            textureBmp.Dispose();

            bwtr.Dispose();
            cwtr.Dispose();
            sw.Stop();
            TManager.PullConsole();
            TManager.WriteLineConsole("Packing of assy done: " + (fi.Length / 1024).ToString() + " Ko");
            TManager.PullConsole();
            return fi;
        }

        public FileInfo Pack1()
        {
            TManager.PushConsole();
            TManager.WriteLineConsole("Packing of assembly " + PartNumber);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            FileInfo fi = new FileInfo(TManager.DocumentsDirectory.FullName + "\\Assys\\" + PartNumber + ".CiliaAZ");
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            StreamWriter wtr = new StreamWriter(fi.FullName);
            GZipStream cwtr = new GZipStream(wtr.BaseStream, CompressionLevel.Optimal);
            BinaryWriter bwtr = new BinaryWriter(cwtr);

            List<TBaseElement> elmts = new List<TBaseElement>();
            List<TLink> links = new List<TLink>();
            TManager.PushConsole();
            TManager.WriteLineConsole("Listing elements");
            Fill(elmts, this.OwnerLink, links);
            elmts.Add(this);
            // ------------------------------------
            //elmts.Sort(TBaseElementTypeComparer.Default);
            elmts = elmts.Distinct().ToList();
            // -----
            Dictionary<TBaseElement, long> adresses = new Dictionary<TBaseElement, long>();
            Dictionary<TBaseElement, int> ids = new Dictionary<TBaseElement, int>();
            TManager.WriteLineConsole("Creating IDs");
            foreach (TBaseElement elmt in elmts.OfType<TSolidElement>())
            {
                ids.Add(elmt, ids.Count);
            }
            foreach (TBaseElement elmt in elmts.OfType<TAssemblyElement>())
            {
                ids.Add(elmt, ids.Count);
            }
            // -----
            TSolidElement[] solids = elmts.OfType<TSolidElement>().ToArray();
            bwtr.Write(solids.Length);
            int idx = 0;
            foreach (TSolidElement solid in solids)
            {
                idx++;
                TManager.WriteLineConsole(idx.ToString() + "/" + ids.Count.ToString() + " Request packing solid " + solid.PartNumber);
                bwtr.Write(solid.PartNumber);
                FileInfo f = solid.Pack();
                BinaryReader rdr = new BinaryReader(f.OpenRead());
                byte[] buffer = new byte[rdr.BaseStream.Length];
                rdr.Read(buffer, 0, buffer.Length);
                rdr.Close();
                rdr.Dispose();
                bwtr.Write(buffer.Length);
                bwtr.Write(buffer);
            }
            //TAssemblyElement[] assys = elmts.OfType<TAssemblyElement>().ToArray();
            links = links.Distinct(TLinkChlidComparer.Default).ToList();
            bwtr.Write(links.Count);
            //links.Reverse();
            foreach (TLink link in links)
            {
                TAssemblyElement assy = (TAssemblyElement)link.Child;
                idx++;
                TManager.WriteLineConsole(idx.ToString() + "/" + ids.Count.ToString() + " Packing assy " + assy.PartNumber);
                bwtr.Write(assy.PartNumber);
                bwtr.Write(link.Links.Max);
                foreach (TLink l in link.Links.Values.Where(o => o != null))
                {
                    bwtr.Write(ids[l.Child]);
                    bwtr.Write(l.NodeName);
                    bwtr.Write(l.Matrix.Row0.X);
                    bwtr.Write(l.Matrix.Row0.Y);
                    bwtr.Write(l.Matrix.Row0.Z);
                    bwtr.Write(l.Matrix.Row0.W);
                    bwtr.Write(l.Matrix.Row1.X);
                    bwtr.Write(l.Matrix.Row1.Y);
                    bwtr.Write(l.Matrix.Row1.Z);
                    bwtr.Write(l.Matrix.Row1.W);
                    bwtr.Write(l.Matrix.Row2.X);
                    bwtr.Write(l.Matrix.Row2.Y);
                    bwtr.Write(l.Matrix.Row2.Z);
                    bwtr.Write(l.Matrix.Row2.W);
                    bwtr.Write(l.Matrix.Row3.X);
                    bwtr.Write(l.Matrix.Row3.Y);
                    bwtr.Write(l.Matrix.Row3.Z);
                    bwtr.Write(l.Matrix.Row3.W);
                }
            }

            bwtr.Dispose();
            cwtr.Dispose();
            sw.Stop();
            TManager.PullConsole();
            TManager.WriteLineConsole("Packing of assy done: " + (fi.Length / 1024).ToString() + " Ko");
            TManager.PullConsole();
            return fi;
        }

        #endregion Public Methods

        #region Private Methods

        private static void Fill(List<TBaseElement> elmts, TLink link, List<TLink> links)
        {
            foreach (TLink l in link.Links.Values.Where(o => o != null))
            {
                if (l.Child is TAssemblyElement assy)
                {
                    Fill(elmts, l, links);
                }
                elmts.Add(l.Child);
            }
            links.Add(link);
        }

        #endregion Private Methods
    }
}