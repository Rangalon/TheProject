using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Private Methods

        private static void DoerLoadings()
        {
            while (true)
            {
                lock (LoadingCounterLocker)
                {
                    if (FilesToload.NotEmpty && LoadingCounter < MaxLoadingFiles)
                    {
                        FilesToload.Pop().StartAsThread();
                    }
                }
                Thread.Sleep(10);
            }
        }

        #endregion Private Methods
    }
}