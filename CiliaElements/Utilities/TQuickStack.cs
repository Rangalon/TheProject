using System;
using System.Threading;

namespace CiliaElements
{
    public class TQuickStc<T> : IDisposable
    {
        #region Public Fields

        public int Max;
        public int Pos;
        public object PushLocker = new object();
        public T[] Values = new T[1];

        #endregion Public Fields

        #region Private Fields

        private int nb = 1;

        #endregion Private Fields

        #region Public Constructors

        

        #endregion Public Constructors

        #region Public Properties

        public bool Empty
        { get { return Max == Pos; } }

        public T Last { get { return Values[Max - 1]; } }

        public bool NotEmpty
        { get { return Max > Pos; } }

        #endregion Public Properties

        #region Public Methods

        //public event Action Emptied;
        //public event Action Filled;

        public void Clear()
        {
            Values = null;
            Max = 0;
            Pos = 0;
        }

        public void Close()
        {
            Array.Resize(ref Values, Max);
        }

        public void Dispose()
        {
            Values = null;
        }

        public T Pop()
        {
            lock (PushLocker)
            {
                Pos++;
                return Values[Pos - 1];
            }
        }

        public void Push(T value)
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
            }
        }

        public void Push(T[] iValues)
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
            }
        }

        public void RemoveAt(int i)
        {
            lock (PushLocker)
            {
                Array.Copy(Values, i + 1, Values, i, Max - i);
                Max--;
            }
        }

        public void RemoveFirst()
        {
            lock (PushLocker)
            {
                Array.Copy(Values, 1, Values, 0, Max - 1);
                Max--;
            }
        }

        public void RemoveLast()
        {
            Max--;
        }

        public void Reset()
        {
            lock (PushLocker)
            {
                Values = null;
                Max = 0;
                Pos = 0;
                Values = new T[nb];
            }
        }

        #endregion Public Methods

        #region Internal Methods

        internal void Postpone(T value)
        {
            lock (PushLocker)
            {
                int n = Max;
                Array.Copy(Values, Pos, Values, Pos - 1, Max - Pos);
                Values[Max - 1] = value;
                Pos--;
            }
        }

        internal void Remove(T obj)
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
                Values = new T[nb];
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
                    Values = new T[nb];
                }
            }
        }

        internal void Reverse()
        {
            Array.Reverse(Values);
        }

        #endregion Internal Methods
    }
}