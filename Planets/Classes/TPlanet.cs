using CiliaElements;
using Math3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planets.Classes
{
    public class TPlanet : TAssemblyElement, IHyperObject
    {
        public static readonly TRandom TopologyRnd = new TRandom();

        public Vec4 Position { get; set; }
        public Mtx4 Orientation { get; set; } = Mtx4.Identity;
        public Vec4 Speed { get; set; }
        public static void GeneratePlanets()
        {
            TManager.CrossLink.Enabled = false;
            TManager.GroundLink.Enabled = false;
            THyperNavigation.Universe = TManager.AttachElmt((new TAssemblyElement() { PartNumber = "Universe" }).OwnerLink, TManager.View.OwnerLink, null);
            THyperNavigation.Universe.ToBeReplaced = false;
            for (int i = 0; i < THyperNavigation.Planets.Length; i++)
                THyperNavigation.Planets[i] = new TPlanet();
        }

        public override string ToString()
        {
            return string.Format("{0:X2} {1}", IdPlanet, Position);
        }


        static int IdPlanets;
        public readonly int IdPlanet;
        public TLink Link { get; set; }
        public TPlanet()
        {
            IdPlanet = IdPlanets; IdPlanets++;
            PartNumber = string.Format("Planet {0:X2}", IdPlanet);
            Link = TManager.AttachElmt(OwnerLink, THyperNavigation.Universe, null);
            Link.NodeName = PartNumber;
            PlanetGround = new TPlanetGround(
                string.Format("P{0:X2}", IdPlanet),
                (Vec3f)(new Vec3(0.3 + 0.3 * TopologyRnd.Next(), 0.3 + 0.3 * TopologyRnd.Next(), 0.3 + 0.3 * TopologyRnd.Next())),
                new TRandom(IdPlanet));
            PlanetGroundLink = TManager.AttachElmt(PlanetGround.OwnerLink, Link, null);
            PlanetGroundLink.ToBeReplaced = false;
            PlanetGroundLink.Pickable = false;
        }

        public TPlanetGround PlanetGround;
        TLink PlanetGroundLink;

        public override void LaunchLoad()
        {

            ElementLoader.Publish();
        }
    }
}
