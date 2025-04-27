using CiliaElements;
using CiliaElements.Elements.Internals;
using Math3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Math3D.TSnubDodecahedron;

namespace Universe
{
    public class TUniverse
    {

        public static bool DoerTilesEnabled = true; 
        public static TLink LkEarth;
        private static TLink LkTiles;
        private static void DoerGeoTiles()
        {
          
            //
          
            TSnubDodecahedron dodec = new TSnubDodecahedron(3);
            Ept e;
            Vec3 p0 = new Vec3(), p;
            Vec3 mem;
            foreach (TSDDHPoint point in dodec.Points)
            {
                mem = point.Coord;
                e = new Ept(Ept.A * point.Coord, 0);
                SRTM_Geoid.TSRTMGeoid.SetGround(ref e, TGeoTile.Delta);
                p = (Vec3)e;
                point.Coord = (Vec3)e;
                if (double.IsNaN(point.Coord.X))
                {
                }
            }

         
            double OneThird = 1.0 / 3.0;

            Dictionary<Vec3, TSDDHFacet> facets = new Dictionary<Vec3, TSDDHFacet>();
            foreach (TSDDHFacet f in dodec.Facets)
            {
                p = OneThird * (f.P1.Coord + f.P2.Coord + f.P3.Coord);
                e = new Ept(p, 0); SRTM_Geoid.TSRTMGeoid.SetGround(ref e, TGeoTile.Delta);
                facets.Add((Vec3)e, f);
            }

     
            Vec3[] fctsp0;
            TGeoTile geoTile;
            Mtx4 m;
            TLink link;
            TSDDHFacet facet;
            double searchRadius = 1;

          
            while (DoerTilesEnabled)
            {
                Thread.Sleep(100);
                m = Mtx4.Invert(LkEarth.Matrix);
                p = Vec3.Transform(p0, m);
                e = new Ept(p, 0);

                fctsp0 = facets.Keys.Where(o => Math.Abs(o.X - p.X) < searchRadius && Math.Abs(o.Y - p.Y) < searchRadius && Math.Abs(o.Z - p.Z) < searchRadius).ToArray();

                searchRadius = Math.Min(searchRadius * 2, 1e6);

                foreach (Vec3 v0 in fctsp0)
                {
                    facet = facets[v0];
                    facets.Remove(v0);
                    //
                    geoTile = new TGeoTile(facet.P1.Coord, facet.P2.Coord, facet.P3.Coord);
                    link = TManager.AttachElmt(geoTile.OwnerLink, LkTiles);
                    link.ToBeReplaced = false;
                    m = Mtx4.CreateTranslation(geoTile.P1);
                    link.Matrix = m;
                    link.NodeName = "Geo" + v0.ToString();
                    link.PendingMatrix = m;
                    //link.NoDiffuse = true;
                    //link.NoCulling = true;
                    link.ToBeReplaced = false;

                    while (geoTile.State != EElementState.Pushed && DoerTilesEnabled) Thread.Sleep(10);

                    lock (TManager.LinksToMove) TManager.LinksToMove.Push(link);
                    //

                    /*
                    geoTile = new TGeoTile(facet.P1.Coord, facet.P2.Coord, facet.P3.Coord, 20000);
                    link = TManager.AttachElmt(geoTile.OwnerLink, LkTiles);
                    link.ToBeReplaced = false;
                    m = Mtx4.CreateTranslation(geoTile.P1);
                    link.Matrix = m;
                    link.NodeName = "GeoSky" + v0.ToString();
                    link.PendingMatrix = m;
                    //link.NoDiffuse = true;
                    //link.NoCulling = true;
                    link.ToBeReplaced = false;

                    while (geoTile.State != EElementState.Pushed && DoerTilesEnabled) Thread.Sleep(10);

                    lock (TManager.LinksToMove) TManager.LinksToMove.Push(link);
                    */
                }
            }
        }

        public static void Setup()
        {
            new Thread(SetupThreaded).Start();
        }
        static void SetupThreaded()
        { 
            while(TManager.View==null || TManager.View.OwnerLink==null) Thread.Sleep(100);

            TAssemblyElement ass = new TAssemblyElement() { PartNumber = "Earth" };
            LkEarth = TManager.AttachElmt(ass.OwnerLink, TManager.View.OwnerLink, null);
            LkEarth.NodeName = ass.PartNumber;
            LkEarth.CanBeHighlighted = false;
            LkEarth.Pickable = true;

            ass = new TAssemblyElement() { PartNumber = "Tiles" };
            LkTiles = TManager.AttachElmt(ass.OwnerLink, LkEarth, null);
            LkTiles.NodeName = ass.PartNumber;
            LkTiles.CanBeHighlighted = false;
            LkTiles.Pickable = false;

            Mtx4 m = Mtx4.InvertL(Mtx4.CreateFromEptAndPsi(new Ept( 1.379693,43.616214, 250000), 0));
            LkEarth.Matrix = m;


            (new Thread(DoerGeoTiles)).Start();
        }
    }
}
