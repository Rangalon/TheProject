using System;

namespace Math3D
{
    public struct Mtx4f : IEquatable<Mtx4f>
    {
        #region Public Fields

        public static readonly Mtx4f Identity = new Mtx4f(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public Vec4f Row0;

        public Vec4f Row1;

        public Vec4f Row2;

        public Vec4f Row3;

        #endregion Public Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Mtx4f(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33)
        {
            Row0 = new Vec4f(m00, m01, m02, m03);
            Row1 = new Vec4f(m10, m11, m12, m13);
            Row2 = new Vec4f(m20, m21, m22, m23);
            Row3 = new Vec4f(m30, m31, m32, m33);
        }

        #endregion Public Constructors

        #region Public Properties

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4f Column0
        {
            get { return new Vec4f(Row0.X, Row1.X, Row2.X, Row3.X); }
            set { Row0.X = value.X; Row1.X = value.Y; Row2.X = value.Z; Row3.X = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4f Column1
        {
            get { return new Vec4f(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
            set { Row0.Y = value.X; Row1.Y = value.Y; Row2.Y = value.Z; Row3.Y = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4f Column2
        {
            get { return new Vec4f(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
            set { Row0.Z = value.X; Row1.Z = value.Y; Row2.Z = value.Z; Row3.Z = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4f Column3
        {
            get { return new Vec4f(Row0.W, Row1.W, Row2.W, Row3.W); }
            set { Row0.W = value.X; Row1.W = value.Y; Row2.W = value.Z; Row3.W = value.W; }
        }

        #endregion Public Properties

        #region Public Methods

        public static Mtx4f CreateOffsetColor(float r, float g, float b)
        {
            Mtx4f m = new Mtx4f();
            m.Row0.X = 1;
            m.Row1.Y = 1;
            m.Row2.Z = 1;
            m.Row3.X = r;
            m.Row3.Y = g;
            m.Row3.Z = b;
            m.Row3.W = 1;
            return m;
        }

        public static Mtx4f CreateRotationX(double angle)
        {
            Mtx4f result = Identity;
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;
            return result;
        }

        public static Mtx4f CreateRotationY(double angle)
        {
            Mtx4f result = Identity;
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            result.Row2.Z = cos;
            result.Row2.X = sin;
            result.Row0.Z = -sin;
            result.Row0.X = cos;
            return result;
        }

        public static Mtx4f CreateRotationZ(double angle)
        {
            Mtx4f result = Identity;
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            result.Row0.X = cos;
            result.Row0.Y = sin;
            result.Row1.X = -sin;
            result.Row1.Y = cos;
            return result;
        }

        public static Mtx4f CreateScale(float x, float y, float z)
        {
            Mtx4f result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
            return result;
        }

        public static Mtx4f CreateScaledTranslation(Vec4 Scale, Vec4 vector)
        {
            Mtx4f result = Identity;
            result.Row0.X = (float)Scale.X;
            result.Row1.Y = (float)Scale.Y;
            result.Row2.Z = (float)Scale.Z;
            result.Row3.X = (float)vector.X;
            result.Row3.Y = (float)vector.Y;
            result.Row3.Z = (float)vector.Z;
            return result;
        }

        public static Mtx4f CreateScaledTranslation(float s, float x, float y, float z)
        {
            Mtx4f result = Identity;
            result.Row0.X = s;
            result.Row1.Y = s;
            result.Row2.Z = s;
            result.Row3.X = x;
            result.Row3.Y = y;
            result.Row3.Z = z;
            return result;
        }

        public static Mtx4f CreateStaticColor(float r, float g, float b)
        {
            Mtx4f m = new Mtx4f();
            m.Row3.X = r;
            m.Row3.Y = g;
            m.Row3.Z = b;
            m.Row3.W = 1;
            return m;
        }

        public static Mtx4f CreateTranslation(Vec4f vector)
        {
            Mtx4f result = Identity;
            result.Row3.X = vector.X;
            result.Row3.Y = vector.Y;
            result.Row3.Z = vector.Z;
            return result;
        }

        public static Mtx4f CreateTranslation(float x, float y, float z)
        {
            Mtx4f result = Identity;
            result.Row3.X = x;
            result.Row3.Y = y;
            result.Row3.Z = z;
            return result;
        }

        public static Mtx4f CreateTranslation(Vec4 vector)
        {
            Mtx4f result = Identity;
            result.Row3.X = (float)vector.X;
            result.Row3.Y = (float)vector.Y;
            result.Row3.Z = (float)vector.Z;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static explicit operator Mtx4f(Mtx4 m)
        {
            Mtx4f result;
            result.Row0 = (Vec4f)m.Row0;
            result.Row1 = (Vec4f)m.Row1;
            result.Row2 = (Vec4f)m.Row2;
            result.Row3 = (Vec4f)m.Row3;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------

        public static Mtx4f operator *(Mtx4f left, Mtx4f right)
        {
            float lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
                lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
                lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
                lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
                rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
                rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
                rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
                rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W;
            Mtx4f result;
            result.Row0.X = (((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31)) + (lM14 * rM41);
            result.Row0.Y = (((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32)) + (lM14 * rM42);
            result.Row0.Z = (((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33)) + (lM14 * rM43);
            result.Row0.W = (((lM11 * rM14) + (lM12 * rM24)) + (lM13 * rM34)) + (lM14 * rM44);
            result.Row1.X = (((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31)) + (lM24 * rM41);
            result.Row1.Y = (((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32)) + (lM24 * rM42);
            result.Row1.Z = (((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33)) + (lM24 * rM43);
            result.Row1.W = (((lM21 * rM14) + (lM22 * rM24)) + (lM23 * rM34)) + (lM24 * rM44);
            result.Row2.X = (((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31)) + (lM34 * rM41);
            result.Row2.Y = (((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32)) + (lM34 * rM42);
            result.Row2.Z = (((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33)) + (lM34 * rM43);
            result.Row2.W = (((lM31 * rM14) + (lM32 * rM24)) + (lM33 * rM34)) + (lM34 * rM44);
            result.Row3.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
            result.Row3.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
            result.Row3.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
            result.Row3.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
            return result;
        }

        public static Mtx4f operator *(Mtx4f left, Mtx4 right)
        {
            float lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
                lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
                lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
                lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
                rM11 = (float)right.Row0.X, rM12 = (float)right.Row0.Y, rM13 = (float)right.Row0.Z, rM14 = (float)right.Row0.W,
                rM21 = (float)right.Row1.X, rM22 = (float)right.Row1.Y, rM23 = (float)right.Row1.Z, rM24 = (float)right.Row1.W,
                rM31 = (float)right.Row2.X, rM32 = (float)right.Row2.Y, rM33 = (float)right.Row2.Z, rM34 = (float)right.Row2.W,
                rM41 = (float)right.Row3.X, rM42 = (float)right.Row3.Y, rM43 = (float)right.Row3.Z, rM44 = (float)right.Row3.W;
            Mtx4f result;
            result.Row0.X = (((lM11 * rM11) + (lM12 * rM21)) + (lM13 * rM31)) + (lM14 * rM41);
            result.Row0.Y = (((lM11 * rM12) + (lM12 * rM22)) + (lM13 * rM32)) + (lM14 * rM42);
            result.Row0.Z = (((lM11 * rM13) + (lM12 * rM23)) + (lM13 * rM33)) + (lM14 * rM43);
            result.Row0.W = (((lM11 * rM14) + (lM12 * rM24)) + (lM13 * rM34)) + (lM14 * rM44);
            result.Row1.X = (((lM21 * rM11) + (lM22 * rM21)) + (lM23 * rM31)) + (lM24 * rM41);
            result.Row1.Y = (((lM21 * rM12) + (lM22 * rM22)) + (lM23 * rM32)) + (lM24 * rM42);
            result.Row1.Z = (((lM21 * rM13) + (lM22 * rM23)) + (lM23 * rM33)) + (lM24 * rM43);
            result.Row1.W = (((lM21 * rM14) + (lM22 * rM24)) + (lM23 * rM34)) + (lM24 * rM44);
            result.Row2.X = (((lM31 * rM11) + (lM32 * rM21)) + (lM33 * rM31)) + (lM34 * rM41);
            result.Row2.Y = (((lM31 * rM12) + (lM32 * rM22)) + (lM33 * rM32)) + (lM34 * rM42);
            result.Row2.Z = (((lM31 * rM13) + (lM32 * rM23)) + (lM33 * rM33)) + (lM34 * rM43);
            result.Row2.W = (((lM31 * rM14) + (lM32 * rM24)) + (lM33 * rM34)) + (lM34 * rM44);
            result.Row3.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
            result.Row3.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
            result.Row3.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
            result.Row3.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Mtx4f))
                return false;

            return this.Equals((Mtx4f)obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Mtx4f other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}