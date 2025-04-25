using System;

namespace OpenTK.Platform.Dummy
{
    internal class DummyWindowInfo : IWindowInfo
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion IDisposable Members

        public IntPtr Handle
        {
            get { return IntPtr.Zero; }
        }
    }
}