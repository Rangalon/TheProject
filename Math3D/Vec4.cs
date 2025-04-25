using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Math3D
{
    public struct Vec4 : IEquatable<Vec4>
    {
        #region Public Fields

        [XmlAttribute]
        public double W;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlAttribute]
        public double X;

        [XmlAttribute]
        public double Y;

        [XmlAttribute]
        public double Z;

        #endregion Public Fields

        public static explicit operator Vec4(Ept v)
        {
            return new Vec4(v.X, v.Y, v.Z, 1);
        }


        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4(Vec3 v, double w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }

        public Vec4(Vec3f v, double w)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = w;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        #endregion Public Constructors

        #region Public Properties

        public double FullLength { get { return Math.Sqrt(W * W + X * X + Y * Y + Z * Z); } }

        public double FullLengthSquared { get { return W * W + X * X + Y * Y + Z * Z; } }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public double Length
        {
            get
            {
                return System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public double LengthSquared
        {
            get
            {
                return X * X + Y * Y + Z * Z;// + W * W;
            }
        }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 Cross(Vec4 left, Vec4 right)
        {
            Vec4 result = new Vec4(
                left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X,
                0);
            return result;
        }

        public static Vec4 Cross(Vec4 l, Vec4 m, Vec4 r)
        {
            //cv = Vec4.Cross(cm.Row0, cm.Row2, cm.Row1) - cm.Row3;
            //cv = Vec4.Cross(cm.Row1, cm.Row2, cm.Row3) - cm.Row0;
            //cv = Vec4.Cross(cm.Row2, cm.Row0, cm.Row3) - cm.Row1;
            //cv = Vec4.Cross(cm.Row3, cm.Row0, cm.Row1) - cm.Row2;
            //Vec4 result = new Vec4(
            //    +l.Y * m.Z * r.W - l.Y * m.W * r.Z + l.Z * m.W * r.Y - l.Z * m.Y * r.W + l.W * m.Y * r.Z - l.W * m.Z * r.Y,
            //    -l.Z * m.W * r.X + l.Z * m.X * r.W - l.W * m.X * r.Z + l.W * m.Z * r.X - l.X * m.Z * r.W + l.X * m.W * r.Z,
            //    +l.W * m.X * r.Y - l.W * m.Y * r.X + l.X * m.Y * r.W - l.X * m.W * r.Y + l.Y * m.W * r.X - l.Y * m.X * r.W,
            //    -l.X * m.Y * r.Z + l.X * m.Z * r.Y - l.Y * m.Z * r.X + l.Y * m.X * r.Z - l.Z * m.X * r.Y + l.Z * m.Y * r.X);
            //return ExtendedCross(ExtendedCross(l, m), r);
            Vec4 result = new Vec4(
                +l.Y * (m.Z * r.W - m.W * r.Z) - l.Z * (m.W * r.Y - m.Y * r.W) + l.W * (m.Y * r.Z - m.Z * r.Y),
                +l.Z * (m.W * r.X - m.X * r.W) - l.W * (m.X * r.Z - m.Z * r.X) + l.X * (m.Z * r.W - m.W * r.Z),
                +l.W * (m.X * r.Y - m.Y * r.X) - l.X * (m.Y * r.W - m.W * r.Y) + l.Y * (m.W * r.X - m.X * r.W),
                +l.X * (m.Y * r.Z - m.Z * r.Y) - l.Y * (m.Z * r.X - m.X * r.Z) + l.Z * (m.X * r.Y - m.Y * r.X)
                );
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static double Dot(Vec4 left, Vec4 right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

        public static double Dot(Vec4 left, Vec3f right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W;
        }

        public static explicit operator Vec2(Vec4 v4d)
        {
            return new Vec2(v4d.X, v4d.Y);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static explicit operator Vec4f(Vec4 v4d)
        {
            return new Vec4f((float)v4d.X, (float)v4d.Y, (float)v4d.Z, (float)v4d.W);
        }

        public static Vec4 ExtendedCross(Vec4 left, Vec4 right)
        {
            Vec4 result = new Vec4(
                left.Z * right.W - left.W * right.Z,
                left.W * right.X - left.X * right.W,
                left.X * right.Y - left.Y * right.X,
                left.Y * right.Z - left.Z * right.Y
                );
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 Normalize(Vec4 vec)
        {
            double scale = 1.0 / vec.Length;
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            vec.W *= scale;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator -(Vec4 vec)
        {
            vec.X = -vec.X;
            vec.Y = -vec.Y;
            vec.Z = -vec.Z;
            vec.W = -vec.W;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator -(Vec4 left, Vec4 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            left.W -= right.W;
            return left;
        }

        public static Vec4 operator -(Vec4 left, Vec2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static Vec4 operator -(Vec2 left, Vec4 right)
        {
            right.X = left.X - right.X;
            right.Y = left.Y - right.Y;
            right.Z = -right.Z;
            return right;
        }

        public static Vec4 operator -(Vec4 left, Vec3 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            left.Z -= right.Z;
            return left;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator !=(Vec4 left, Vec4 right)
        {
            return !left.Equals(right);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator *(double scale, Vec4 vec)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            vec.W *= scale;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator *(Vec4 vec, double scale)
        {
            vec.X *= scale;
            vec.Y *= scale;
            vec.Z *= scale;
            vec.W *= scale;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator /(Vec4 vec, double scale)
        {
            double mult = 1 / scale;
            vec.X *= mult;
            vec.Y *= mult;
            vec.Z *= mult;
            vec.W *= mult;
            return vec;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 operator +(Vec4 left, Vec4 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            left.W += right.W;
            return left;
        }

        public static Vec4 operator +(Vec4 left, Vec2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Vec4 operator +(Vec4 left, Vec3 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            left.Z += right.Z;
            return left;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator ==(Vec4 left, Vec4 right)
        {
            return left.Equals(right);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Vec4 Transform(Vec4 vec, Mtx4 mat)
        {
            Vec4 result = new Vec4(
               vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + vec.W * mat.Row3.X,
               vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + vec.W * mat.Row3.Y,
               vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + vec.W * mat.Row3.Z,
               vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + vec.W * mat.Row3.W);
            return result;
        }

        public static Vec4 Transform(Vec3 vec, Mtx4 mat)
        {
            Vec4 result = new Vec4(
               vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + mat.Row3.X,
               vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + mat.Row3.Y,
               vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + mat.Row3.Z,
               vec.X * mat.Row0.W + vec.Y * mat.Row1.W + vec.Z * mat.Row2.W + mat.Row3.W);
            return result;
        }

        public static Vec3 TransformPoint(Vec3 vec, Mtx4 mat)
        {
            Vec3 result = new Vec3(
               vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X + mat.Row3.X,
               vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y + mat.Row3.Y,
               vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z + mat.Row3.Z);
            return result;
        }

        public static Vec3 TransformVector(Vec3 vec, Mtx4 mat)
        {
            Vec3 result = new Vec3(
               vec.X * mat.Row0.X + vec.Y * mat.Row1.X + vec.Z * mat.Row2.X,
               vec.X * mat.Row0.Y + vec.Y * mat.Row1.Y + vec.Z * mat.Row2.Y,
               vec.X * mat.Row0.Z + vec.Y * mat.Row1.Z + vec.Z * mat.Row2.Z);
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Vec4 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z &&
                W == other.W;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Vec4))
                return false;

            return this.Equals((Vec4)obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Normalize()
        {
            double scale = 1.0 / this.Length;
            X *= scale;
            Y *= scale;
            Z *= scale;
            W *= scale;
        }

        public override string ToString()
        {
            //return X.ToString(CultureInfo.InvariantCulture) + ";" + Y.ToString(CultureInfo.InvariantCulture) + ";" + Z.ToString(CultureInfo.InvariantCulture) + ";" + W.ToString(CultureInfo.InvariantCulture);
            return Math.Round(X, 4).ToString(CultureInfo.InvariantCulture) + ";" + Math.Round(Y, 4).ToString(CultureInfo.InvariantCulture) + ";" + Math.Round(Z, 4).ToString(CultureInfo.InvariantCulture) + ";" + Math.Round(W, 4).ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}