using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CiliaElements
{
    public class TLinksQuickStc : IDisposable
    {
        #region Public Fields

        public int Max;
        public int Pos;
        public object PushLocker = new object();
        public TLink[] Values = new TLink[1];

        #endregion Public Fields

        public TLink[] Assemblies = Array.Empty<TLink>();
        public TLink[] Solids = Array.Empty<TLink>();

        #region Private Fields

        private int nb = 1;

        #endregion Private Fields

        #region Public Constructors



        #endregion Public Constructors

        #region Public Properties

        public bool Empty
        { get { return Max == Pos; } }

        public TLink Last { get { return Values[Max - 1]; } }

        public bool NotEmpty
        { get { return Max > Pos; } }

        #endregion Public Properties

        #region Public Methods

        //public event Action Emptied;
        //public event Action Filled;

      
       
        public void Dispose()
        {
            Values = null;
        }

      

        public void Push(TLink value)
        {
            lock (PushLocker)
            {
                if (Max >= nb)
                {
                    nb *= 2;
                    Array.Resize(ref Values, nb);
                    //while (Values.Length < nb) Thread.Sleep(1);
                }
                //else if (Max == 0) Filled?.Invoke();
                Values[Max] = value;

                Max++;
                UpdateSubLists();
            }
        }

        public void Push(TLink[] iValues)
        {
            lock (PushLocker)
            {
                int n = Max;
                Max += iValues.Length;
                if (Max >= nb)
                {
                    nb = 2 * Max;
                    Array.Resize(ref Values, nb);
                    //while (Values.Length < nb) Thread.Sleep(1);
                }
                //else if (n == 0) Filled?.Invoke();
                Array.Copy(iValues, 0, Values, n, iValues.Length);
                UpdateSubLists();
            }
        }

        //public void RemoveAt(int i)
        //{
        //    lock (PushLocker)
        //    {
        //        Array.Copy(Values, i + 1, Values, i, Max - i);
        //        Max--;
        //        UpdateSubLists();
        //    }
        //}

        //public void RemoveFirst()
        //{
        //    lock (PushLocker)
        //    {
        //        Array.Copy(Values, 1, Values, 0, Max - 1);
        //        Max--;
        //        UpdateSubLists();
        //    }
        //}

        //public void RemoveLast()
        //{
        //    Max--;
        //    UpdateSubLists();
        //}

        //public void Reset()
        //{
        //    lock (PushLocker)
        //    {
        //        Values = null;
        //        Max = 0;
        //        Pos = 0;
        //        Values = new TLink[nb];
        //        UpdateSubLists();
        //    }
        //}

        #endregion Public Methods

        #region Internal Methods

        internal void Postpone(TLink value)
        {
            lock (PushLocker)
            {
                int n = Max;
                Array.Copy(Values, Pos, Values, Pos - 1, Max - Pos);
                Values[Max - 1] = value;
                Pos--;
            }
        }

        internal void Remove(TLink obj)
        {
            lock (PushLocker)
            {
                int i = Array.IndexOf(Values, obj);
                Array.Copy(Values, i + 1, Values, i, Max - i - 1);
                Max--;
            }
        }

        internal void Reset(int n)
        {
            lock (PushLocker)
            {
                nb = n;
                Values = null;
                Max = 0;
                Pos = 0;
                Values = new TLink[nb];
            }
        }

        internal void ResetIfEmpty()
        {
            lock (PushLocker)
            {
                if (Empty)
                {
                    Values = null;
                    nb = 1;
                    Max = 0;
                    Pos = 0;
                    Values = new TLink[nb];
                }
            }
        }

        internal void Reverse()
        {
            Array.Reverse(Values);
        }

        #endregion Internal Methods

        void UpdateSubLists()
        {
            Assemblies = Values.Where(o => o != null && o.Child is TAssemblyElement).ToArray();
            Solids = Values.Where(o => o != null && o.Child is TSolidElement ).ToArray();
        }
    }
}