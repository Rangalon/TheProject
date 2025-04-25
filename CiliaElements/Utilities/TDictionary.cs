using System;
using System.Globalization;
namespace CiliaElements
{
    public class TDico<T1, T2> where T1 : IComparable<T1>
    {
        #region Private Fields

        private int _Count = 0;
        private T1[] _Keys = { };
        private T2[] values = { };

        private readonly object Locker = new object();

        #endregion Private Fields

        #region Public Properties

        public int Count
        {
            get { return _Count; }
        }

        public T1[] Keys
        {
            get { return _Keys; }
        }

        public T2[] Values
        {
            get { return values; }
        }

        #endregion Public Properties

        #region Public Indexers

        public T2 this[T1 iK]
        {
            get
            {
                T2 functionReturnValue = default(T2);
                lock (Locker)
                    functionReturnValue = values[GetIndexOfK(iK)];
                return functionReturnValue;
            }
            set
            {
                lock (Locker)
                    values[GetIndexOfK(iK)] = value;
            }
        }

        #endregion Public Indexers

        #region Public Methods

        public void Add(T1 iK, T2 iV)
        {
            lock (Locker)
            {
                _Count++;
                Array.Resize(ref _Keys, _Count);
                Array.Resize(ref values, _Count);
                values[_Count - 1] = iV;
                _Keys[_Count - 1] = iK;
            }
        }

        public void Clear()
        {
            lock (Locker)
            {
                _Keys = null;
                values = null;
                Array.Resize(ref _Keys, 0);
                Array.Resize(ref values, 0);
                _Count = 0;
            }
        }

        public bool ContainsKey(T1 iK)
        {
            return GetIndexOfK(iK) != -1;
        }

        public void Remove(T1 iK)
        {
            lock (Locker)
            {
                if (!ContainsKey(iK))
                    throw new Exception("Unknown key:" + iK.ToString());
                for (int i = GetIndexOfK(iK); i < Count - 1; i++)
                {
                    values[i] = values[i + 1];
                    _Keys[i] = _Keys[i + 1];
                }
                _Count--;
                Array.Resize(ref _Keys, _Count);
                Array.Resize(ref values, _Count);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private int GetIndexOfK(T1 iK)
        {
            int j = -1;
            lock (Locker)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (_Keys[i].ToString() == iK.ToString())
                        j = i;
                }
            }
            return j;
        }

        #endregion Private Methods
    }
}