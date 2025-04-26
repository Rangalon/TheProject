using CiliaElements;
using System;
using System.IO;
using System.Threading;

namespace TheProject.Planets
{
    public abstract class TUniverse
    {
        public static TLink Universe;
        public const int PlanetsQty = 256;

        static TUniverse()
        {
        }

        public static TTimer HyperEngineTimer;

        public static void Activate()
        {
            new Thread(ActivateThreaded).Start();
        }

        private static void ActivateThreaded()
        {
            while (TManager.View == null) Thread.Sleep(100);

            Universe = TManager.AttachElmt((new TAssemblyElement() { PartNumber = "Universe" }).OwnerLink, TManager.View.OwnerLink, null);
            Universe.ToBeReplaced = false;
            Universe.NodeName = "Universe";

            for (int i = 0; i < PlanetsQty; i++)
            {
                TPlanet planet = new TPlanet(i);
            }

            HyperEngineTimer = new TTimer("HyperSpace", 50, DoHyperSpace, Array.Empty<TTimer>());
            HyperEngineTimer.Activate();

            //GenerateRandom();
        }

        private static void GenerateRandom()
        {
            StreamWriter wtr = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Rnd.txt");
            Random rnd = new Random();
            for (int i = 0; i < 65536; i++)
            {
                wtr.WriteLine(rnd.NextDouble());
            }
            wtr.Close();
            wtr.Dispose();
        }

        private static void DoHyperSpace()
        {
        }
    }
}