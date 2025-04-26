using Math3D;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TheProject
{
    public class TRandom
    {
        private static double[] Values = new double[65536];

        static TRandom()
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            string[] lines = Properties.Resources.Rnd.Split('\n');
            for (int i = 0; i < 65536; i++)
            {
                Values[i] = Convert.ToDouble(lines[i]);
            }
        }

        private int rank;

        public TRandom()
        { }

        public TRandom(int rk)
        {
            rank = rk;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3f PopColor(float min, float rng)
        {
            Vec3f v=new Vec3f();
            v.X = min + rng * (float)PopDouble();
            v.Y = min + rng * (float)PopDouble();
            v.Z = min + rng * (float)PopDouble();
            return v;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double PopDouble()
        {
            PushRank();
            return Values[rank];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PopInt(int max)

        {
            PushRank();
            return (int)(max * Values[rank]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PopInt()

        {
            PushRank();
            return (int)(65536 * Values[rank]);
        }

        private static char[] Consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        private static char[] Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string PopString()
        {
            int nb = PopInt(6) + 4;
            string val = ("" + Consonants[PopInt(20)]).ToUpper();
            for (int i = 1; i < nb;)
            {
                val += Vowels[PopInt(6)];
                val += Consonants[PopInt(6)];
                i += 2;
            }
            return val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte PopByte(int max)
        {
            PushRank();
            return (byte)(max * Values[rank]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PushRank()
        {
            rank = (rank + 1) % 65536;
        }
    }
}