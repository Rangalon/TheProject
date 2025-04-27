 
using Math3D;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace SRTM_Geoid
{
    public static class TSRTMGeoid
    {
        #region Public Fields

        public static readonly bool Activated = false;
        public static double Step = 0.0083333333333333333333333333333333;
        public static double StepInversed = 1.0 / Step;

        #endregion Public Fields

        #region Private Fields

        private static BinaryReader brdr;

        #endregion Private Fields

        #region Public Constructors

        public static FileInfo DataFile = new FileInfo("C:\\Users\\Public\\Documents\\SRTM-Geoid\\Data.bin");
        
        public static readonly List<string> Messages = new List<string>();

        static TSRTMGeoid()
        {
            StreamReader rdr = new StreamReader(DataFile.FullName );
            brdr = new BinaryReader(rdr.BaseStream);
            Activated = true;
        }

        #endregion Public Constructors

        #region Public Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetHeight(double lat, double lon)
        {
            Vec2 M = new Vec2(((lon + 180) * StepInversed) % 43200, ((-lat + 90) * StepInversed) % 21600);
            //
            Vec2i P0 = (Vec2i)M; M -= P0;
            double d0 = 1 / M.LengthSquared;
            double h0 = Table(P0.X, P0.Y);
            if (d0 > 1e4) return h0;
            //
            P0.X += 1; M.X -= 1;
            double h1 = Table(P0.X, P0.Y);
            double d1 = 1 / M.LengthSquared;
            if (d1 > 1e4) return h1;
            //
            P0.Y += 1; M.Y -= 1;
            double h2 = Table(P0.X, P0.Y);
            double d2 = 1 / M.LengthSquared;
            if (d2 > 1e4) return h2;
            //
            P0.X -= 1; M.X += 1;
            double h3 = Table(P0.X, P0.Y);
            double d3 = 1 / M.LengthSquared;
            if (d3 > 1e4) return h3;
            //
            return (d0 * h0 + d1 * h1 + d2 * h2 + d3 * h3) / (d0 + d1 + d2 + d3);
        }

        public static void SetGround(ref Ept p1)
        {
            p1.Hei = GetHeight(p1.Lat, p1.Lon);
        }

        public static void SetGround(ref Ept p1, double delta)
        {
            p1.Hei = GetHeight(p1.Lat, p1.Lon) + delta;
        }

        #endregion Public Methods

        #region Private Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short Table(int ilon, int ilat)
        {
            //while (ilon < 0) ilon += 43200;
            ilon %= 43200; ilat %= 21600; ilon = ilon * 43200 + ilat * 2;
            short i;
            lock (brdr.BaseStream)
            {
                brdr.BaseStream.Position = ilon;
                i = brdr.ReadInt16();
            }
            return i;
        }

        #endregion Private Methods
    }
}