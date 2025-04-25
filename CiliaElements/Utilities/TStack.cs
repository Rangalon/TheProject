using System;

namespace CiliaElements
{
    public class TStc<T> where T : IComparable<T>
    {
        #region Private Fields

        private readonly EPopMode PopMode;

        private Int32 _Count;

        private T[] _Objects = {
	};

        private object Locker = new object();

        #endregion Private Fields

        #region Public Constructors

        public TStc(EPopMode iPopMode)
        {
            PopMode = iPopMode;
        }

        #endregion Public Constructors

        #region Public Properties

        public Int32 Count
        {
            get { return _Count; }
        }

        public T[] Values
        {
            get
            {
                lock (Locker)
                {
                    return _Objects;
                }
            }
        }

        #endregion Public Properties

        #region Public Methods

        public T Pop()
        {
            T functionReturnValue = default(T);
            lock (Locker)
            {
                functionReturnValue = _Objects[_Count - 1];
                _Count--;
                Array.Resize(ref _Objects, _Count);
            }
            return functionReturnValue;
        }

        public void Push(T iObj)
        {
            lock (Locker)
            {
                Array.Resize(ref _Objects, _Count + 1);
                switch (PopMode)
                {
                    case EPopMode.FIFO:
                        _Objects[_Count] = iObj;
                        break;

                    case EPopMode.LIFO:
                        Array.Copy(_Objects, 0, _Objects, 1, _Count);
                        _Objects[0] = iObj;
                        break;

                    case EPopMode.Latest:
                        _Objects[_Count] = iObj;
                        for (Int32 j = 0; j < _Count; j++)
                        {
                            if (_Objects[j].CompareTo(iObj) == 1)
                            {
                                Array.Copy(_Objects, j, _Objects, j + 1, _Count - j);
                                _Objects[j] = iObj;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }

                        break;

                    case EPopMode.First:
                        _Objects[_Count] = iObj;
                        for (Int32 j = 0; j < _Count; j++)
                        {
                            if (_Objects[j].CompareTo(iObj) == -1)
                            {
                                Array.Copy(_Objects, j, _Objects, j + 1, _Count - j);
                                _Objects[j] = iObj;
                                break; // TODO: might not be correct. Was : Exit For
                            }
                        }

                        break;
                }
                _Count++;
            }
        }

        #endregion Public Methods
    }
}