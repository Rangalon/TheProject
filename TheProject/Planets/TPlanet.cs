using CiliaElements;

namespace TheProject.Planets
{
    public class TPlanet : TAssemblyElement
    {
        public readonly TLink Link, GroundLink;
        public readonly TPlanetGround Ground;
        public readonly string Name;

        private static TRandom NamesRandom = new TRandom();

        public TRandom Random;

        public TPlanet(int rank)
        {
            Random = new TRandom(rank);
            Name = NamesRandom.PopString();
            if (!Name.EndsWith("a")) Name += "a";
            Link = TManager.AttachElmt((new TAssemblyElement() { PartNumber = Name }).OwnerLink, TUniverse.Universe, null);
            Link.NodeName = Name;
            Link.ToBeReplaced = false;
            Ground = new TPlanetGround(this);
            GroundLink = TManager.AttachElmt(Ground.OwnerLink, Link, null);
            GroundLink.ToBeReplaced = false;
            GroundLink.Pickable = false;
            GroundLink.NodeName = Ground.PartNumber;
        }

        public override void LaunchLoad()
        {
            ElementLoader.Publish();
        }
    }

    public class TPlanetGround : TInternal
    {
        public readonly TPlanet Planet;
        public readonly TRandom TopologyRnd;

        public TPlanetGround(TPlanet planet) : base(planet.Name + "-Ground")
        {
            Planet = planet;
            TopologyRnd = new TRandom(planet.Random.PopInt());
        }
    }
}