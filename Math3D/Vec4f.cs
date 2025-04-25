using System;

namespace Math3D
{
    public struct Vec4f : IEquatable<Vec4f>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vec4f(float ix, float iy, float iz, float iw)
        {
            X = ix;
            Y = iy;
            Z = iz;
            W = iw;
        }

        public static Vec4f operator +(Vec4f left, Vec4f right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            left.W += right.W;
            return left;
        }

        public static Vec4f operator *(float left, Vec4f right)
        {
            right.X *= left;
            right.Y *= left;
            right.Z *= left;
            return right;
        }

        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        public static Vec4f operator -(Vec4f left, Vec4f right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            left.W -= right.W;
            return left;
        }

        public static Vec4f Transform(Vec4f vec, Mtx4f mat)
        {
            Vec4f result = new Vec4f(
               vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + vec.W * mat.Row3.X,
               vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + vec.W * mat.Row3.Y,
               vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + vec.W * mat.Row3.Z,
               vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + vec.W * mat.Row3.W);
            return result;
        }

        public static bool operator ==(Vec4f left, Vec4f right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec4f left, Vec4f right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vec4f))
                return false;

            return this.Equals((Vec4f)obj);
        }

        public bool Equals(Vec4f other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }
    }
}