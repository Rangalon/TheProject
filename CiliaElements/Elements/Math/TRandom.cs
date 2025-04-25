using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.Elements.Math
{
    public class TRandom
    {
        #region Public Fields

        public int Position = 0;

        #endregion Public Fields

        #region Private Fields

        private static readonly double[] doubles;
        private static readonly float[] floats;
        private static readonly long[] Int63s;
        private static readonly long[] Int64s;
        private static readonly int max = 1024 * 1024;
        private static readonly double[] udoubles;
        private static readonly float[] ufloats;

        private static int MaxInt = 4 * 1024 * 1024;
        private int laps = 0;

        #endregion Private Fields

        #region Public Constructors

        static TRandom()
        {
            doubles = new double[max];
            floats = new float[max];
            Int63s = new long[max];
            Int64s = new long[max];
            ufloats = new float[max];
            udoubles = new double[max];

            MemoryStream ms = new MemoryStream(Properties.Resources.Random);
            GZipStream zstr = new GZipStream(ms, CompressionMode.Decompress);
            BinaryReader bstr = new BinaryReader(zstr);
            for (int i = 0; i < max; i++)
            {
                uint j = bstr.ReadUInt32();
                //
                udoubles[i] = (double)j / MaxInt;
                ufloats[i] = (float)j / MaxInt;
                //
                j -= 32768;
                //
                floats[i] = 2 * j / MaxInt;
                doubles[i] = 2 * (double)j / MaxInt;
                Int64s[i] = j << 47;
                Int63s[i] = j << 46;
            }

            bstr.Close();
            bstr.Dispose();
            zstr.Close();
            zstr.Dispose();
            ms.Close();
            ms.Dispose();
            //Int64 i64 = Int64s.Max();
            //Int64 ii64 = Int64.MaxValue;
        }

        #endregion Public Constructors

        #region Public Methods

        public double NextDouble()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return doubles[Position];
            }
        }

        public float NextFloat()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return floats[Position];
            }
        }

        public long NextInt63()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return Int63s[Position];
            }
        }

        public long NextInt64()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return Int64s[Position];
            }
        }

        public double NextUDouble()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return udoubles[Position];
            }
        }

        public float NextUFloat()
        {
            lock (this)
            {
                Position++;
                if (Position == max) { Position = 0; laps++; }
                return ufloats[Position];
            }
        }

        #endregion Public Methods
    }
}