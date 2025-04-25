using Math3D;

namespace OpenTK.Input
{
    public struct TouchFinger
    {
        public int Id { get; set; }
        public Vec2f Position { get; set; }
        public Vec2f Delta { get; set; }
        public float Pressure { get; set; }
        public float X { get { return Position.X; } }
        public float Y { get { return Position.Y; } }

        public TouchFinger(int id, Vec2f position, Vec2f delta, float pressure)
            : this()
        {
            Id = id;
            Position = position;
            Delta = delta;
            Pressure = pressure;
        }
    }
}