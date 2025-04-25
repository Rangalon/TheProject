namespace Math3D
{
    public struct Trg3
    {
        public Vec3 O;
        public Vec3 X;
        public Vec3 Y;

        public static bool operator ==(Trg3 t1, Trg3 t2)
        {
            if (t1.O != t2.O) return false;
            if (t1.X != t2.X) return false;
            if (t1.Y != t2.Y) return false;
            return true;
        }

        public static bool operator !=(Trg3 t1, Trg3 t2)
        {
            if (t1.O != t2.O) return true;
            if (t1.X != t2.X) return true;
            if (t1.Y != t2.Y) return true;
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}