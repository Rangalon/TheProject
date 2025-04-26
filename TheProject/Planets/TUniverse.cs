using CiliaElements;
using Math3D;
using System;
using System.Collections.Generic;
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

            List<Vec4 > positions=new List<Vec4>();
            Vec4 vec = new Vec4();
            for (double i1 = -7.5; i1 < 8; i1++)
            {
                vec.X = i1;
                for (double i2 = -7.5; i2 < 8; i2++)
                {
                    vec.Y = i2;
                    for (double i3 = -7.5; i3 < 8; i3++)
                    {
                        vec.Z = i3;
                        for (double i4 = -7.5; i4 < 8; i4++)
                        {
                            vec.W = 1;// i4;
                            positions.Add(vec);
                        }
                    }
                }
            }


            TRandom posrandom=new TRandom();


            for (int i = 0; i < PlanetsQty; i++)
            {
                TPlanet planet = new TPlanet(i);
                vec = positions[posrandom.PopInt(positions.Count)];
                planet.Position  = 1000*vec;
                positions.RemoveAll(o => (o - vec).FullLengthSquared < 2);
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