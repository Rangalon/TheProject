using System;
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

        public double PopDouble()
        {
            rank = (rank + 1) % 65536;
            return Values[rank];
        }

        public int PopInt(int max)

        {
            rank = (rank + 1) % 65536;
            return (int)(max * Values[rank]);
        }

        public int PopInt()

        {
            rank = (rank + 1) % 65536;
            return (int)(65536 * Values[rank]);
        }

        private static char[] Consonants = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        private static char[] Vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'y' };

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
    }
}