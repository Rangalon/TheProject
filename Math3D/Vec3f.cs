using System;
using System.Globalization;

namespace Math3D
{
    public struct Vec3f : IEquatable<Vec3f>
    {
        #region "Shared Public Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f Cross(Vec3f left, Vec3f right)
        {
            Vec3f result;
            result = new Vec3f(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f operator *(float scale, Vec3f vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        public static Vec3f operator *(Vec3f vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        public static Vec3f operator *(Vec3f vec, double scale)
        {
            vec.X *= (float)scale;
            vec.Y *= (float)scale;
            vec.Z *= (float)scale;
            return vec;
        }

        public override string ToString()
        {
            return X.ToString(CultureInfo.InvariantCulture) + ";" + Y.ToString(CultureInfo.InvariantCulture) + ";" + Z.ToString(CultureInfo.InvariantCulture);
        }



        public static explicit operator Vec3f(Ept v)
        {
            return new Vec3f((float)v.X, (float)v.Y, (float)v.Z);
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f CreatePolarVector(Vec2 pCoord, double R)
        {
            Vec3f v;
            v.X = (float)(Math.Cos(pCoord.X) * Math.Cos(pCoord.Y) * R);
            v.Y = (float)(Math.Sin(pCoord.X) * Math.Cos(pCoord.Y) * R);
            v.Z = (float)(Math.Sin(pCoord.Y) * R);
            return v;
        }

        public static explicit operator Vec3f(Vec4f v4)
        {
            Vec3f v3;
            v3.X = v4.X;
            v3.Y = v4.Y;
            v3.Z = v4.Z;
            return v3;
        }

        public static bool operator ==(Vec3f v1, Vec3f v2)
        {
            return (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z);
        }

        public static bool operator !=(Vec3f v1, Vec3f v2)
        {
            return (v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z);
        }

        public static Vec3f Transform(Vec3f vec, Mtx4 mat)
        {
            Vec4 v4 = new Vec4(vec.X, vec.Y, vec.Z, 1.0);
            v4 = Vec4.Transform(v4, mat);
            return (Vec3f)v4;
        }

        public static Vec3f TransformPoint(Vec3f vec, Mtx4f mat)
        {
            Vec4f v4 = new Vec4f(vec.X, vec.Y, vec.Z, 1.0F);
            v4 = Vec4f.Transform(v4, mat);
            return (Vec3f)v4;
        }

        public static explicit operator Vec3f(Vec3 v4)
        {
            Vec3f v3;
            v3.X = (float)v4.X;
            v3.Y = (float)v4.Y;
            v3.Z = (float)v4.Z;
            return v3;
        }

        public static explicit operator Vec3f(Vec4 v4)
        {
            Vec3f v3;
            v3.X = (float)v4.X;
            v3.Y = (float)v4.Y;
            v3.Z = (float)v4.Z;
            return v3;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f operator +(Vec3f left, Vec3f right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }

        public static Vec3f operator +(Vec3f left, Vec2f right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Vec3f operator -(Vec3f left)
        {
            return new Vec3f(-left.X, -left.Y, -left.Z);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f operator -(Vec3f left, Vec3f right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        public static Vec3f operator -(Vec3f left, Vec2f right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3f operator /(Vec3f vec, float scale)
        {
            float mult = 1.0f / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Shared Public Functions"

        #region "Public Variables"

        public float X;
        public float Y;
        public float Z;

        #endregion "Public Variables"

        #region "Public Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Vec3f other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Vec3f))
                return false;

            return this.Equals((Vec3f)obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Normalize()
        {
            float scale = 1.0f / this.Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
        }

        public Vec3f Normalized()
        {
            Normalize();
            return this;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Public Functions"

        #region "Public Properties"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float Length
        {
            get
            {
                return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion "Public Properties"

        public static Vec3f Convert(Vec3 v)
        {
            return new Vec3f() { X = (float)v.X, Y = (float)v.Y, Z = (float)v.Z };
        }
    }
}