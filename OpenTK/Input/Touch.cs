using OpenTK.Platform.Windows;

namespace OpenTK.Input
{
    public static class Touch
    {
        #region Fields

        private static readonly WinRawTouch driver = Platform.Factory.Default.CreateTouchDriver();
        private static readonly object SyncRoot = new object();

        #endregion Fields

        public static TouchState GetTouchesState()
        {
            lock (SyncRoot)
            {
                return driver.GetTouchesState();
            }
        }

        public static TouchState GetState()
        {
            lock (SyncRoot)
            {
                return driver.GetState();
            }
        }
    }
}