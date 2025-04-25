 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CiliaElements
{
    public class TTimer
    {
        #region Public Fields

        public static List<TTimer> Timers = new List<TTimer>();
        public bool Activated { get; set; } = true;
        public byte Iteration { get; set; }
        public Action JobToDo;
        public double Mean { get; set; } = 0;
        public double Duration { get; set; } = 0;
        public string Name { get; set; } = "";
        public long Period { get; set; }
        public TTimer[] ThreadsToWaitFor { get; set; }

        #endregion Public Fields

        #region Private Fields

        private Thread th;

        #endregion Private Fields

        #region Public Constructors

        public TTimer(string name, int period, Action jobToDo, TTimer[] threadsToWaitFor)
        {
            Name = name;
            Period = period * 10000;
            JobToDo = jobToDo;
            ThreadsToWaitFor = threadsToWaitFor;
            th = new Thread(DoTheJobWithConditions);
            th.Start(); 
            lock (Timers) Timers.Add(this);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Activate()
        {
            Activated = true;
        }

        public void Kill()
        {
            lock (Timers) Timers.Remove(this);
            Activated = false;
            th.Abort();
        }

        public override string ToString()
        {
            return string.Format("{0} Mean: {1:00}ms Dur: {2:00}ms", Name, Mean, Duration); 
        }

        #endregion Public Methods

        #region Private Methods

        private bool CheckThreads(byte[] bts, int n, bool[] bls)
        {
            byte c;
            TTimer th;
            int i;
            for (i = 0; i < n; i++)
            {
                if (!bls[i])
                {
                    th = ThreadsToWaitFor[i];
                    lock (th)
                    {
                        c = th.Iteration;
                        if (c == bts[i]) return true;
                    }
                    bls[i] = true;
                    bts[i] = c;
                }
            }
            return false;
        }

        //private void DoTheJob()
        //{
        //    //try
        //    //{
        //    long SleepPeriod = (long)(Period * 0.2);
        //    Stopwatch sw = Stopwatch.StartNew();
        //    long l = sw.ElapsedTicks + Period;
        //    while (TSystEnv.Activated)
        //    {
        //        if (Activated) JobToDo.Invoke();
        //        lock (this) Iteration = (byte)(Iteration == 255 ? 0 : Iteration++);
        //        while (l - sw.ElapsedTicks > SleepPeriod) Thread.Sleep(1);
        //        while (l > sw.ElapsedTicks) ;
        //        l += Period;
        //    }
        //    //}

        //    //catch (Exception ex)
        //    //{
        //    //    TSystEnv.Inst.Logger.LogStuff(ex.Message + "\n" + ex.StackTrace);
        //    //}
        //}

        public static bool AllActivated = true;

        private void DoTheJobWithConditions()
        {
            //try
            //{
            long SleepPeriod = (long)(Period * 0.3);
            Stopwatch sw = Stopwatch.StartNew();
            long l = sw.ElapsedTicks + Period;
            byte[] bts = new byte[ThreadsToWaitFor.Length];
            bool[] bls = new bool[ThreadsToWaitFor.Length];
            int n = bts.Length;
            int i;
            for (i = 0; i < n; i++) bts[i] = 255;
            long last, next = 0;
            double k = 0.9, mk = 1e-4 * (1.0 - k);
            while (AllActivated)
            {
                for (i = 0; i < n; i++) bls[i] = false;
                while (CheckThreads(bts, n, bls) && AllActivated) Thread.Sleep(1);
                last = next; next = sw.ElapsedTicks; Mean = k * Mean + mk * (next - last);
                if (Activated) JobToDo.Invoke();
                Duration = k * Duration + mk * (sw.ElapsedTicks - next);
                lock (this) Iteration = (byte)(Iteration == 255 ? 0 : Iteration + 1);
                while (l - sw.ElapsedTicks > SleepPeriod) Thread.Sleep(1);
                while (l > sw.ElapsedTicks) ;
                l += Period;
            }
            //}
            //catch (Exception ex)
            //{
            //    TSystEnv.Inst.Logger.LogStuff(ex.Message + "\n" + ex.StackTrace);
            //}
        }

        #endregion Private Methods
    }
}