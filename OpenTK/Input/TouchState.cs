using Math3D;
using System.Globalization;

namespace OpenTK.Input
{
    public struct TouchState
    {
        private bool is_connected;

        private Vec2? position1;
        private Vec2? position2;

        public Vec2? Position1 { get { return position1; } set { position1 = value; } }
        public Vec2? Position2 { get { return position2; } set { position2 = value; } }

        public bool IsConnected
        {
            get { return is_connected; }
            internal set { is_connected = value; }
        }

        public int Count;

        internal void MergeBits(TouchState other)
        {
            IsConnected |= other.IsConnected;
        }

        public override string ToString()
        {
            return "TouchState " + IsConnected.ToString(CultureInfo.InvariantCulture) + " " + Count.ToString(CultureInfo.InvariantCulture);
        }
    }
}