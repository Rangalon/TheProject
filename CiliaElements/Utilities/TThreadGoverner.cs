using System;
using System.Threading;

namespace CiliaElements.Utilities
{
    public class TThreadGoverner
    {
        public static readonly TThreadGoverner Compute = new TThreadGoverner(1000);
        public static readonly TThreadGoverner GPU = new TThreadGoverner(1000);
        public static readonly TThreadGoverner CPU = new TThreadGoverner(1000);
        public static readonly TThreadGoverner Draw = new TThreadGoverner(1000);
        public static readonly TThreadGoverner Pick = new TThreadGoverner(2000);


        public readonly double MaxPS;
        public double PossiblePS { get => 1000 / AvrgDuration; }

        DateTime Start;
        public void Reset()
        {
            Start = DateTime.Now;
        }

        double minspan;

        public TThreadGoverner(double maxps)
        {
            MaxPS = maxps;
            minspan = 1000 / maxps;
        }

        double AvrgDuration;
        TimeSpan ts;
        double span;
        public void Check()
        {
            ts = DateTime.Now - Start;
            AvrgDuration = 0.99 * AvrgDuration + 0.01 * ts.TotalMilliseconds; 
            span = minspan - ts.TotalMilliseconds;
            if (span > 0) Thread.Sleep((int)span);
        }
    }
}
