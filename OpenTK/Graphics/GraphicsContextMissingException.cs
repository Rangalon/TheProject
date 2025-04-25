using System.Globalization;

namespace OpenTK.Graphics
{
    /// <summary>
    /// Thrown when an operation that required GraphicsContext is performed, when no
    /// GraphicsContext is current in the calling thread.
    /// </summary>
    public class GraphicsContextMissingException : GraphicsContextException
    {
        /// <summary>
        /// Constructs a new GraphicsContextMissingException.
        /// </summary>
        public GraphicsContextMissingException()
            : base(string.Format(CultureInfo.InvariantCulture,
                "No context is current in the calling thread (ThreadId: {0}).",
                System.Threading.Thread.CurrentThread.ManagedThreadId))
        { }
    }
}