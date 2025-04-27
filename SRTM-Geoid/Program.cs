using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_SRTM_Geoid
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(TSRTMGeoid.GetHeight(43.61591, 1.38002));
        }
    }
}
