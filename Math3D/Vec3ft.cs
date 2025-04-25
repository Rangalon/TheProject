using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Math3D
{
    public struct Vec3ft : IEquatable<Vec3ft>
    {
        #region Public Fields

        [XmlAttribute]
        public float U;

        [XmlAttribute]
        public float V;

        [XmlAttribute]
        public float X;

        [XmlAttribute]
        public float Y;

        [XmlAttribute]
        public float Z;

        #endregion Public Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec3ft(float x, float y, float z, float u, float v)
        {
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
        }

        #endregion Public Constructors

        #region Public Properties

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
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public float LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static Vec3ft Convert(Vec3 v)
        {
            return new Vec3ft() { X = (float)v.X, Y = (float)v.Y, Z = (float)v.Z };
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft CreatePolarVector(Vec2 pCoord, double R)
        {
            Vec3ft v = new Vec3ft();
            v.X = (float)(System.Math.Cos(pCoord.X) * System.Math.Cos(pCoord.Y) * R);
            v.Y = (float)(System.Math.Sin(pCoord.X) * System.Math.Cos(pCoord.Y) * R);
            v.Z = (float)(System.Math.Sin(pCoord.Y) * R);
            return v;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft Cross(Vec3ft left, Vec3ft right)
        {
            Vec3ft result;
            result = new Vec3ft(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X, 0, 0);
            return result;
        }

        public static explicit operator Vec3ft(Vec4f v4)
        {
            Vec3ft v3 = new Vec3ft();
            v3.X = v4.X;
            v3.Y = v4.Y;
            v3.Z = v4.Z;
            return v3;
        }

        public static explicit operator Vec3ft(Vec3 v4)
        {
            Vec3ft v3 = new Vec3ft();
            v3.X = (float)v4.X;
            v3.Y = (float)v4.Y;
            v3.Z = (float)v4.Z;
            return v3;
        }

        public static explicit operator Vec3ft(Vec4 v4)
        {
            Vec3ft v3 = new Vec3ft();
            v3.X = (float)v4.X;
            v3.Y = (float)v4.Y;
            v3.Z = (float)v4.Z;
            return v3;
        }

        public static Vec3ft operator -(Vec3ft left)
        {
            return new Vec3ft(-left.X, -left.Y, -left.Z, -left.U, -left.V);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft operator -(Vec3ft left, Vec3ft right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        public static Vec3ft operator -(Vec3ft left, Vec2f right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static bool operator !=(Vec3ft v1, Vec3ft v2)
        {
            return (v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft operator *(float scale, Vec3ft vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        public static Vec3ft operator *(Vec3ft vec, float scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            return vec;
        }

        public static Vec3ft operator *(Vec3ft vec, double scale)
        {
            vec.X *= (float)scale;
            vec.Y *= (float)scale;
            vec.Z *= (float)scale;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft operator /(Vec3ft vec, float scale)
        {
            float mult = 1.0f / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec3ft operator +(Vec3ft left, Vec3ft right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }

        public static Vec3ft operator +(Vec3ft left, Vec2f right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static bool operator ==(Vec3ft v1, Vec3ft v2)
        {
            return (v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z);
        }

        public static Vec3ft Transform(Vec3ft vec, Mtx4 mat)
        {
            Vec4 v4 = new Vec4(vec.X, vec.Y, vec.Z, 1.0);
            v4 = Vec4.Transform(v4, mat);
            return (Vec3ft)v4;
        }

        public static Vec3ft TransformPoint(Vec3ft vec, Mtx4f mat)
        {
            Vec4f v4 = new Vec4f(vec.X, vec.Y, vec.Z, 1.0F);
            v4 = Vec4f.Transform(v4, mat);
            return (Vec3ft)v4;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Vec3ft other)
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
            if (!(obj is Vec3ft))
            {
                return false;
            }

            return this.Equals((Vec3ft)obj);
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

        public Vec3ft Normalized()
        {
            Normalize();
            return this;
        }

        public override string ToString()
        {
            return X.ToString(CultureInfo.InvariantCulture) + ";" + Y.ToString(CultureInfo.InvariantCulture) + ";" + Z.ToString(CultureInfo.InvariantCulture) + ";" + U.ToString(CultureInfo.InvariantCulture) + ";" + V.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}