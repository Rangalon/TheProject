using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Math3D
{
    /// <summary>
    /// Represents a 4x4 matrix containing 3D rotation, scale, transform, and projection with Double-precision components.
    /// </summary>
    /// <seealso cref="Mtx4f"/>
    public struct Mtx4 : IEquatable<Mtx4>
    {
        #region Public Fields

        public static Mtx4 Identity = new Mtx4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        public static Mtx4 SwitchXY = Mtx4.CreateRotationZ(Math.PI / 2);

        public Vec4 Row0;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Vec4 Row1;

        public Vec4 Row2;

        public Vec4 Row3;

        #endregion Public Fields

        #region Public Constructors

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Mtx4(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
            Row3 = new Vec4(m30, m31, m32, m33);
        }

        #endregion Public Constructors

        #region Public Properties

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public Vec4 Column0
        {
            get { return new Vec4(Row0.X, Row1.X, Row2.X, Row3.X); }
            set { Row0.X = value.X; Row1.X = value.Y; Row2.X = value.Z; Row3.X = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public Vec4 Column1
        {
            get { return new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
            set { Row0.Y = value.X; Row1.Y = value.Y; Row2.Y = value.Z; Row3.Y = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public Vec4 Column2
        {
            get { return new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
            set { Row0.Z = value.X; Row1.Z = value.Y; Row2.Z = value.Z; Row3.Z = value.W; }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        [XmlIgnore]
        public Vec4 Column3
        {
            get { return new Vec4(Row0.W, Row1.W, Row2.W, Row3.W); }
            set { Row0.W = value.X; Row1.W = value.Y; Row2.W = value.Z; Row3.W = value.W; }
        }

        public bool IsIdentity
        {
            get
            {
                if (Row0.X != 1)
                {
                    return false;
                }

                if (Row0.Y != 0)
                {
                    return false;
                }

                if (Row0.Z != 0)
                {
                    return false;
                }

                if (Row0.W != 0)
                {
                    return false;
                }

                if (Row1.X != 0)
                {
                    return false;
                }

                if (Row1.Y != 1)
                {
                    return false;
                }

                if (Row1.Z != 0)
                {
                    return false;
                }

                if (Row1.W != 0)
                {
                    return false;
                }

                if (Row2.X != 0)
                {
                    return false;
                }

                if (Row2.Y != 0)
                {
                    return false;
                }

                if (Row2.Z != 1)
                {
                    return false;
                }

                if (Row2.W != 0)
                {
                    return false;
                }

                if (Row3.X != 0)
                {
                    return false;
                }

                if (Row3.Y != 0)
                {
                    return false;
                }

                if (Row3.Z != 0)
                {
                    return false;
                }

                if (Row3.W != 1)
                {
                    return false;
                }

                return true;
            }
        }

        #endregion Public Properties

        #region Public Methods

        public static Mtx4 CreateFromEptAndPsy(Ept e, double psy)
        {


            Mtx4 mtx4 = new Mtx4();
            mtx4.Row3 = (Vec4)e;
            Ept ep = new Ept(e.X, e.Y, e.Z, 100);
            mtx4.Row2 = (Vec4)ep - (Vec4)e;
            mtx4.Row0 = new Vec4(0, 0, 1, 0);
            mtx4.Row1 = Vec4.Cross(mtx4.Row2, mtx4.Row0);
            mtx4.Row0 = Vec4.Cross(mtx4.Row1, mtx4.Row2);
            //
            mtx4.Normalize();
            mtx4 = Mtx4.CreateRotationZ(-psy * Ept.DegToRad) * mtx4;
            //
            return mtx4;
        }

        public void Normalize()
        {
            double n;
            n = 1 / Math.Sqrt(Row0.X * Row0.X + Row0.Y * Row0.Y + Row0.Z * Row0.Z);
            Row0 = new Vec4(Row0.X * n, Row0.Y * n, Row0.Z * n, 0);
            n = 1 / Math.Sqrt(Row1.X * Row1.X + Row1.Y * Row1.Y + Row1.Z * Row1.Z);
            Row1 = new Vec4(Row1.X * n, Row1.Y * n, Row1.Z * n, 0);
            n = 1 / Math.Sqrt(Row2.X * Row2.X + Row2.Y * Row2.Y + Row2.Z * Row2.Z);
            Row2 = new Vec4(Row2.X * n, Row2.Y * n, Row2.Z * n, 0);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreateFromAxisAngle(Vec3 axis, double angle)
        {
            Mtx4 result = Mtx4.Identity;
            // normalize and create a local copy of the vector.
            axis.Normalize();
            double axisX = axis.X, axisY = axis.Y, axisZ = axis.Z;

            // calculate angles
            double cos = System.Math.Cos(-angle);
            double sin = System.Math.Sin(-angle);
            double t = 1.0f - cos;

            // do the conversion math once
            double tXX = t * axisX * axisX,
                tXY = t * axisX * axisY,
                tXZ = t * axisX * axisZ,
                tYY = t * axisY * axisY,
                tYZ = t * axisY * axisZ,
                tZZ = t * axisZ * axisZ;

            double sinX = sin * axisX,
                sinY = sin * axisY,
                sinZ = sin * axisZ;

            result.Row0.X = tXX + cos;
            result.Row0.Y = tXY - sinZ;
            result.Row0.Z = tXZ + sinY;
            result.Row1.X = tXY + sinZ;
            result.Row1.Y = tYY + cos;
            result.Row1.Z = tYZ - sinX;
            result.Row2.X = tXZ - sinY;
            result.Row2.Y = tYZ + sinX;
            result.Row2.Z = tZZ + cos;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreatePerspectiveFieldOfView(double fovy, double aspect, double zNear, double zFar)
        {
            //string s = string.Format("fovy:{0}\naspect:{1}\nznear:{2}\nzfar:{3}\n", fovy, aspect, zNear, zFar);
            //if (fovy <= 0 || 2 * fovy > Math.PI)
            //    throw new ArgumentOutOfRangeException("fovy");
            //if (aspect <= 0)
            //    throw new ArgumentOutOfRangeException("aspect");
            //if (zNear <= 0)
            //    throw new ArgumentOutOfRangeException("zNear");
            //if (zFar <= 0)
            //    throw new ArgumentOutOfRangeException("zFar");

            double yMax = zNear * System.Math.Tan(fovy);
            double yMin = -yMax;
            double xMin = yMin * aspect;
            double xMax = yMax * aspect;

            //Vec4 vMax = new Vec4(0, 0, zFar, 1);
            //Vec4 vMin = new Vec4(0, 0, zNear, 1);

            CreatePerspectiveOffCenter(xMin, xMax, yMin, yMax, zNear, zFar, out Mtx4 result);
            //result.Row2.Z /= Math.Pow(1000,  field);
            //result.Row3.Z /= Math.Pow(1000,  field);
            //vMax = Vector4d.Transform(vMax, result);
            //vMin = Vector4d.Transform(vMin, result);
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void CreatePerspectiveOffCenter(double left, double right, double bottom, double top, double zNear, double zFar, out Mtx4 result)
        {
            if (zNear <= 0)
            {
                throw new ArgumentOutOfRangeException("zNear");
            }

            if (zFar <= 0)
            {
                throw new ArgumentOutOfRangeException("zFar");
            }

            if (zNear >= zFar)
            {
                throw new ArgumentOutOfRangeException("zNear");
            }

            double x = (2.0 * zNear) / (right - left);
            double y = (2.0 * zNear) / (top - bottom);
            double a = (right + left) / (right - left);
            double b = (top + bottom) / (top - bottom);
            double c = -(zFar + zNear) / (zFar - zNear);
            double d = -(2.0 * zFar * zNear) / (zFar - zNear);

            result = new Mtx4(x, 0, 0, 0,
                                 0, y, 0, 0,
                                 a, b, c, -1,
                                 0, 0, d, 0);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreateRotation(double q0, double q1, double q2, double q3)
        {
            Mtx4 result = Identity;
            //double ct = System.Math.Cos(q3);
            //double st = System.Math.Sin(q3);
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row1.X = a * b * (1 - ct) - c * st; result.Row2.X = a * c * (1 - ct) + b * st;
            //result.Row0.Y = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row2.Y = b * c * (1 - ct) - a * st;
            //result.Row0.Z = a * c * (1 - ct) - b * st; result.Row1.Z = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row0.Y = a * b * (1 - ct) - c * st; result.Row0.Z = a * c * (1 - ct) + b * st;
            //result.Row1.X = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row1.Z = b * c * (1 - ct) - a * st;
            //result.Row2.X = a * c * (1 - ct) - b * st; result.Row2.Y = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            result.Row0.X = 2 * (q0 * q0 + q1 * q1) - 1; result.Row0.Y = 2 * (q1 * q2 - q0 * q3); result.Row0.Z = 2 * (q1 * q3 + q0 * q2);
            result.Row1.X = 2 * (q1 * q2 + q0 * q3); result.Row1.Y = 2 * (q0 * q0 + q2 * q2) - 1; result.Row1.Z = 2 * (q2 * q3 - q0 * q1);
            result.Row2.X = 2 * (q1 * q3 - q0 * q2); result.Row2.Y = 2 * (q2 * q3 + q0 * q1); result.Row2.Z = 2 * (q0 * q0 + q3 * q3) - 1;
            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreateRotation(Vec4 n, double t)
        {
            Mtx4 result = Identity;
            double c = Math.Cos(t), s = Math.Sin(t);
            double cbis = 1 - c;
            double ux = n.X, uy = n.Y, uz = n.Z;
            //double ct = System.Math.Cos(q3);
            //double st = System.Math.Sin(q3);
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row1.X = a * b * (1 - ct) - c * st; result.Row2.X = a * c * (1 - ct) + b * st;
            //result.Row0.Y = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row2.Y = b * c * (1 - ct) - a * st;
            //result.Row0.Z = a * c * (1 - ct) - b * st; result.Row1.Z = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row0.Y = a * b * (1 - ct) - c * st; result.Row0.Z = a * c * (1 - ct) + b * st;
            //result.Row1.X = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row1.Z = b * c * (1 - ct) - a * st;
            //result.Row2.X = a * c * (1 - ct) - b * st; result.Row2.Y = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            result.Row0.X = ux * ux * cbis + c; result.Row0.Y = ux * uy * cbis - uz * s; result.Row0.Z = ux * uz * cbis + uy * s;
            result.Row1.X = ux * uy * cbis + uz * s; result.Row1.Y = uy * uy * cbis + c; result.Row1.Z = uy * uz * cbis - ux * s;
            result.Row2.X = ux * uz * cbis - uy * s; result.Row2.Y = uy * uz * cbis + ux * s; result.Row2.Z = uz * uz * cbis + c;
            return result;
        }
        public static Mtx4 CreateRotation(Vec3 n, double t)
        {
            Mtx4 result = Identity;
            double c = Math.Cos(t), s = Math.Sin(t);
            double cbis = 1 - c;
            double ux = n.X, uy = n.Y, uz = n.Z;
            //double ct = System.Math.Cos(q3);
            //double st = System.Math.Sin(q3);
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row1.X = a * b * (1 - ct) - c * st; result.Row2.X = a * c * (1 - ct) + b * st;
            //result.Row0.Y = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row2.Y = b * c * (1 - ct) - a * st;
            //result.Row0.Z = a * c * (1 - ct) - b * st; result.Row1.Z = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row0.Y = a * b * (1 - ct) - c * st; result.Row0.Z = a * c * (1 - ct) + b * st;
            //result.Row1.X = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row1.Z = b * c * (1 - ct) - a * st;
            //result.Row2.X = a * c * (1 - ct) - b * st; result.Row2.Y = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            result.Row0.X = ux * ux * cbis + c; result.Row0.Y = ux * uy * cbis - uz * s; result.Row0.Z = ux * uz * cbis + uy * s;
            result.Row1.X = ux * uy * cbis + uz * s; result.Row1.Y = uy * uy * cbis + c; result.Row1.Z = uy * uz * cbis - ux * s;
            result.Row2.X = ux * uz * cbis - uy * s; result.Row2.Y = uy * uz * cbis + ux * s; result.Row2.Z = uz * uz * cbis + c;
            return result;
        }

        public static Mtx4 CreateRotationX(double angle)
        {
            Mtx4 result = Identity;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;
            return result;
        }

        public static Mtx4 CreateRotationY(double angle)
        {
            Mtx4 result = Identity;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);
            result.Row2.Z = cos;
            result.Row2.X = sin;
            result.Row0.Z = -sin;
            result.Row0.X = cos;
            return result;
        }

        public static Mtx4 CreateRotationZ(double angle)
        {
            Mtx4 result = Identity;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);
            result.Row0.X = cos;
            result.Row0.Y = sin;
            result.Row1.X = -sin;
            result.Row1.Y = cos;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreateScale(double x, double y, double z)
        {
            Mtx4 result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
            return result;
        }

        public static Mtx4 CreateScale(double d)
        {
            Mtx4 result = Identity;
            result.Row0.X = d;
            result.Row1.Y = d;
            result.Row2.Z = d;
            return result;
        }

        public static Mtx4 CreateScale(Vec3 v)
        {
            Mtx4 result = Identity;
            result.Row0.X = v.X;
            result.Row1.Y = v.Y;
            result.Row2.Z = v.Z;
            return result;
        }

        public static Mtx4 CreateScaledTranslation(Vec4 Scale, Vec4 vector)
        {
            Mtx4 result = Identity;
            result.Row0.X = (float)Scale.X;
            result.Row1.Y = (float)Scale.Y;
            result.Row2.Z = (float)Scale.Z;
            result.Row3.X = (float)vector.X;
            result.Row3.Y = (float)vector.Y;
            result.Row3.Z = (float)vector.Z;
            return result;
        }

        public static Mtx4 CreateScaledTranslation(double d, Vec3 v)
        {
            Mtx4 result = Identity;
            result.Row0.X = d;
            result.Row1.Y = d;
            result.Row2.Z = d;
            result.Row3.X = v.X;
            result.Row3.Y = v.Y;
            result.Row3.Z = v.Z;
            return result;
        }

        public static Mtx4 CreateStaticColor(double r, double g, double b)
        {
            Mtx4 m = new Mtx4();
            m.Row3.X = r;
            m.Row3.Y = g;
            m.Row3.Z = b;
            m.Row3.W = 1;
            return m;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 CreateTranslation(Vec4 vector)
        {
            Mtx4 result = Identity;
            result.Row3.X = vector.X;
            result.Row3.Y = vector.Y;
            result.Row3.Z = vector.Z;
            return result;
        }

        public static Mtx4 CreateTranslation(Ept vector)
        {
            Mtx4 result = Identity;
            result.Row3.X = vector.X;
            result.Row3.Y = vector.Y;
            result.Row3.Z = vector.Z;
            return result;
        }

        public static Mtx4 CreateTranslation(Vec3 vector)
        {
            Mtx4 result = Identity;
            result.Row3.X = vector.X;
            result.Row3.Y = vector.Y;
            result.Row3.Z = vector.Z;
            return result;
        }

        public static Mtx4 CreateTranslation(double x, double y, double z)
        {
            Mtx4 result = Identity;
            result.Row3.X = x;
            result.Row3.Y = y;
            result.Row3.Z = z;
            return result;
        }

        public static explicit operator Mtx4(Mtx5 v)
        {
            Mtx4 m = new Mtx4
            {
                Row0 = v.Row0,
                Row1 = v.Row1,
                Row2 = v.Row2,
                Row3 = v.Row4
            };
            m.Row0.W = 0;
            m.Row1.W = 0;
            m.Row2.W = 0;
            m.Row3.W = 1;
            return m;
        }

        public static Mtx4 Invert(Mtx4 m)
        {
            // Do not touch anuthing!!!
            Mtx4 mi = new Mtx4();
            //
            double d2Z3W = m.Row2.Z * m.Row3.W - m.Row2.W * m.Row3.Z;
            double d2W3X = m.Row2.W * m.Row3.X - m.Row2.X * m.Row3.W;
            double d2X3Y = m.Row2.X * m.Row3.Y - m.Row2.Y * m.Row3.X;
            double d2Y3Z = m.Row2.Y * m.Row3.Z - m.Row2.Z * m.Row3.Y;
            double d2X3Z = m.Row2.X * m.Row3.Z - m.Row2.Z * m.Row3.X;
            double d2Y3W = m.Row2.Y * m.Row3.W - m.Row2.W * m.Row3.Y;
            //
            mi.Row0.X = m.Row1.Y * d2Z3W - m.Row1.Z * d2Y3W + m.Row1.W * d2Y3Z;
            mi.Row1.X = -(m.Row1.Z * d2W3X + m.Row1.W * d2X3Z + m.Row1.X * d2Z3W);
            mi.Row2.X = m.Row1.W * d2X3Y + m.Row1.X * d2Y3W + m.Row1.Y * d2W3X;
            mi.Row3.X = -(m.Row1.X * d2Y3Z - m.Row1.Y * d2X3Z + m.Row1.Z * d2X3Y);
            //
            double det = 1 / (m.Row0.X * mi.Row0.X + m.Row0.Y * mi.Row1.X + m.Row0.Z * mi.Row2.X + m.Row0.W * mi.Row3.X);
            mi.Row0.X *= det; mi.Row1.X *= det; mi.Row2.X *= det; mi.Row3.X *= det;
            //
            mi.Row0.Y = -(m.Row0.Y * d2Z3W - m.Row0.Z * d2Y3W + m.Row0.W * d2Y3Z) * det;
            mi.Row1.Y = (m.Row0.Z * d2W3X + m.Row0.W * d2X3Z + m.Row0.X * d2Z3W) * det;
            mi.Row2.Y = -(m.Row0.W * d2X3Y + m.Row0.X * d2Y3W + m.Row0.Y * d2W3X) * det;
            mi.Row3.Y = (m.Row0.X * d2Y3Z - m.Row0.Y * d2X3Z + m.Row0.Z * d2X3Y) * det;
            //
            double d0Z1W = (m.Row0.Z * m.Row1.W - m.Row0.W * m.Row1.Z) * det;
            double d0W1X = (m.Row0.W * m.Row1.X - m.Row0.X * m.Row1.W) * det;
            double d0X1Y = (m.Row0.X * m.Row1.Y - m.Row0.Y * m.Row1.X) * det;
            double d0Y1Z = (m.Row0.Y * m.Row1.Z - m.Row0.Z * m.Row1.Y) * det;
            double d0X1Z = (m.Row0.X * m.Row1.Z - m.Row0.Z * m.Row1.X) * det;
            double d0Y1W = (m.Row0.Y * m.Row1.W - m.Row0.W * m.Row1.Y) * det;
            //
            mi.Row0.Z = (m.Row3.Y * d0Z1W - m.Row3.Z * d0Y1W + m.Row3.W * d0Y1Z);
            mi.Row1.Z = -(m.Row3.Z * d0W1X + m.Row3.W * d0X1Z + m.Row3.X * d0Z1W);
            mi.Row2.Z = (m.Row3.W * d0X1Y + m.Row3.X * d0Y1W + m.Row3.Y * d0W1X);
            mi.Row3.Z = -(m.Row3.X * d0Y1Z - m.Row3.Y * d0X1Z + m.Row3.Z * d0X1Y);
            //
            mi.Row0.W = -(m.Row2.Y * d0Z1W - m.Row2.Z * d0Y1W + m.Row2.W * d0Y1Z);
            mi.Row1.W = (m.Row2.Z * d0W1X + m.Row2.W * d0X1Z + m.Row2.X * d0Z1W);
            mi.Row2.W = -(m.Row2.W * d0X1Y + m.Row2.X * d0Y1W + m.Row2.Y * d0W1X);
            mi.Row3.W = (m.Row2.X * d0Y1Z - m.Row2.Y * d0X1Z + m.Row2.Z * d0X1Y);
            // 
            return mi;
        }

        public void Transpose()
        {
            double mem;
            mem = Row0.Y; Row0.Y = Row1.X; Row1.X = mem; mem = Row0.Z; Row0.Z = Row2.X; Row2.X = mem; mem = Row0.W; Row0.W = Row3.X; Row3.X = mem;
            mem = Row1.Z; Row1.Z = Row2.Y; Row2.Y = mem; mem = Row1.W; Row1.W = Row3.Y; Row3.Y = mem;
            mem = Row2.W; Row2.W = Row3.Z; Row3.Z = mem;
        }
  
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 LookAt(Vec3 eye, Vec3 target, Vec3 up)
        {
            Vec3 z = Vec3.Normalize(eye - target);
            Vec3 x = Vec3.Normalize(Vec3.Cross(up, z));
            Vec3 y = Vec3.Normalize(Vec3.Cross(z, x));

            Mtx4 rot = new Mtx4(x.X, y.X, z.X, 0.0,
                                        x.Y, y.Y, z.Y, 0.0,
                                        x.Z, y.Z, z.Z, 0.0,
                                        0, 0, 0, 1);

            Mtx4 trans = Mtx4.CreateTranslation(-eye);

            return trans * rot;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator !=(Mtx4 left, Mtx4 right)
        {
            return !left.Equals(right);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 operator *(Mtx4 left, Mtx4 right)
        {
            double lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
                lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
                lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
                lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
                rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
                rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
                rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
                rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W;
            Mtx4 result;
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

        public static Vec4 operator *(Vec4 left, Mtx4 right)
        {
            double
                lM41 = left.X, lM42 = left.Y, lM43 = left.Z, lM44 = left.W,
                rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
                rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
                rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
                rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W;
            Vec4 result;

            result.X = (((lM41 * rM11) + (lM42 * rM21)) + (lM43 * rM31)) + (lM44 * rM41);
            result.Y = (((lM41 * rM12) + (lM42 * rM22)) + (lM43 * rM32)) + (lM44 * rM42);
            result.Z = (((lM41 * rM13) + (lM42 * rM23)) + (lM43 * rM33)) + (lM44 * rM43);
            result.W = (((lM41 * rM14) + (lM42 * rM24)) + (lM43 * rM34)) + (lM44 * rM44);
            return result;
        }
        public static Mtx4 operator *(Mtx4 left, double d)
        {
            Mtx4 m = new Mtx4();
            m.Row0.X = left.Row0.X * d; m.Row0.Y = left.Row0.Y * d; m.Row0.Z = left.Row0.Z * d; m.Row0.W = left.Row0.W * d;
            m.Row1.X = left.Row1.X * d; m.Row1.Y = left.Row1.Y * d; m.Row1.Z = left.Row1.Z * d; m.Row1.W = left.Row1.W * d;
            m.Row2.X = left.Row2.X * d; m.Row2.Y = left.Row2.Y * d; m.Row2.Z = left.Row2.Z * d; m.Row2.W = left.Row2.W * d;
            m.Row3.X = left.Row3.X * d; m.Row3.Y = left.Row3.Y * d; m.Row3.Z = left.Row3.Z * d; m.Row3.W = left.Row3.W * d;
            return m;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator ==(Mtx4 left, Mtx4 right)
        {
            return left.Equals(right);
        }


        public static Mtx4 InvertP(Mtx4 mat)
        {
            //if (mat.Row0.W != 0 || mat.Row0.Z != 0 || mat.Row0.Y != 0 || mat.Row1.W != 0 || mat.Row1.Z != 0 || mat.Row1.X != 0 || mat.Row2.X != 0 || mat.Row2.Y != 0 || mat.Row3.X != 0 || mat.Row3.Y != 0) throw new Exception("QuickInvertPV not possible!\n" + mat.ToString());
            Mtx4 m = new Mtx4();
            //
            double d = 1 / (mat.Row2.Z * mat.Row3.W - mat.Row2.W * mat.Row3.Z);
            m.Row0.X = 1 / mat.Row0.X;
            m.Row1.Y = 1 / mat.Row1.Y;
            m.Row2.Z = d * mat.Row3.W;
            m.Row2.W = -d * mat.Row2.W;
            m.Row3.Z = -d * mat.Row3.Z;
            m.Row3.W = d * mat.Row2.Z;
            //
            return m;
        }
        public static Mtx4 InvertL(Mtx4 mat)
        {
            //if (mat.Row0.W != 0 || mat.Row1.W != 0 || mat.Row2.W != 0 || mat.Row3.W != 1) throw new Exception("QuickInvert not possible!\n" + mat.ToString());
            double a1 = mat.Row1.Y * mat.Row2.Z - mat.Row1.Z * mat.Row2.Y;
            double a2 = mat.Row1.Z * mat.Row2.X - mat.Row1.X * mat.Row2.Z;
            double a3 = mat.Row1.X * mat.Row2.Y - mat.Row1.Y * mat.Row2.X;
            double d = 1 / (a1 * mat.Row0.X + a2 * mat.Row0.Y + a3 * mat.Row0.Z);
            Mtx4 m = new Mtx4();
            m.Row0.X = d * a1;
            m.Row0.Y = d * (mat.Row2.Y * mat.Row0.Z - mat.Row2.Z * mat.Row0.Y);
            m.Row0.Z = d * (mat.Row0.Y * mat.Row1.Z - mat.Row0.Z * mat.Row1.Y);
            m.Row1.X = d * a2;
            m.Row1.Y = d * (mat.Row2.Z * mat.Row0.X - mat.Row2.X * mat.Row0.Z);
            m.Row1.Z = d * (mat.Row0.Z * mat.Row1.X - mat.Row0.X * mat.Row1.Z);
            m.Row2.X = d * a3;
            m.Row2.Y = d * (mat.Row2.X * mat.Row0.Y - mat.Row2.Y * mat.Row0.X);
            m.Row2.Z = d * (mat.Row0.X * mat.Row1.Y - mat.Row0.Y * mat.Row1.X);
            m.Row3.X = -(m.Row0.X * mat.Row3.X + m.Row1.X * mat.Row3.Y + m.Row2.X * mat.Row3.Z);
            m.Row3.Y = -(m.Row0.Y * mat.Row3.X + m.Row1.Y * mat.Row3.Y + m.Row2.Y * mat.Row3.Z);
            m.Row3.Z = -(m.Row0.Z * mat.Row3.X + m.Row1.Z * mat.Row3.Y + m.Row2.Z * mat.Row3.Z);
            m.Row3.W = 1;
            //Mtx4 mm = m * mat;
            //if (Math .Round( mm.Row0.X,3) != 1 || Math.Round(mm.Row1.Y, 3) != 1 || Math.Round(mm.Row2.Z, 3) != 1 || Math.Round(mm.Row3.W, 3) != 1)
            //{
            //    Console.WriteLine(mat);
            //    Console.WriteLine(m);
            //    Console.WriteLine(mm);
            //    mm.Row3.W = 1;
            //}
            return m;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx4 Scale(double x, double y, double z)
        {
            Mtx4 result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
            return result;
        }

        public static Mtx4 SpecialProduct(Mtx4 left, Mtx4 right)
        {
            double lM11 = left.Column0.X, lM12 = left.Column0.Y, lM13 = left.Column0.Z, lM14 = left.Column0.W,
      lM21 = left.Column1.X, lM22 = left.Column1.Y, lM23 = left.Column1.Z, lM24 = left.Column1.W,
      lM31 = left.Column2.X, lM32 = left.Column2.Y, lM33 = left.Column2.Z, lM34 = left.Column2.W,
      lM41 = left.Column3.X, lM42 = left.Column3.Y, lM43 = left.Column3.Z, lM44 = left.Column3.W,
      rM11 = right.Column0.X, rM12 = right.Column0.Y, rM13 = right.Column0.Z, rM14 = right.Column0.W,
      rM21 = right.Column1.X, rM22 = right.Column1.Y, rM23 = right.Column1.Z, rM24 = right.Column1.W,
      rM31 = right.Column2.X, rM32 = right.Column2.Y, rM33 = right.Column2.Z, rM34 = right.Column2.W,
      rM41 = right.Column3.X, rM42 = right.Column3.Y, rM43 = right.Column3.Z, rM44 = right.Column3.W;
            Mtx4 result;
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

        public Vec3? CheckRay(Vec3 PA, Vec3 VB, Vec3 VC, ref double ZMin)
        {
            if (Vec3.Cross(VB, VC).LengthSquared == 0)
            {
                return null;
            }

            // convert the matrix to an array for easy looping
            double[,] inverse = {{Row0.X, Row0.Y, VB.X, VC.X},
                                {Row1.X, Row1.Y,VB.Y, VC.Y},
                                {Row2.X, Row2.Y, VB.Z, VC.Z},
                                {Row3.X, Row3.Y, 0, 0} };

            double oneOverPivot;
            //
            //
            oneOverPivot = 1.0 / inverse[0, 0];
            inverse[0, 0] = oneOverPivot;
            inverse[0, 1] *= oneOverPivot;
            inverse[0, 2] *= oneOverPivot;
            inverse[0, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[1, 0];
            inverse[1, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 0];
            inverse[2, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 0];
            inverse[3, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[0, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[1, 1];
            inverse[1, 0] *= oneOverPivot;
            inverse[1, 1] = oneOverPivot;
            inverse[1, 2] *= oneOverPivot;
            inverse[1, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[0, 1];
            inverse[0, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[0, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[0, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[0, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 1];
            inverse[2, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[2, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 1];
            inverse[3, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[3, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[1, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[2, 2];
            inverse[2, 0] *= oneOverPivot;
            inverse[2, 1] *= oneOverPivot;
            inverse[2, 2] = oneOverPivot;
            inverse[2, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[0, 2];
            inverse[0, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[0, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[0, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[0, 3] -= inverse[2, 3] * oneOverPivot;
            oneOverPivot = inverse[1, 2];
            inverse[1, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[1, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[2, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 2];
            inverse[3, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[3, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[2, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[3, 3];
            inverse[3, 0] *= oneOverPivot;
            inverse[3, 1] *= oneOverPivot;
            inverse[3, 2] *= oneOverPivot;
            inverse[3, 3] = oneOverPivot;
            //
            oneOverPivot = inverse[0, 3];
            inverse[0, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[0, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[0, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[0, 3] = -inverse[3, 3] * oneOverPivot;
            oneOverPivot = inverse[1, 3];
            inverse[1, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[1, 3] = -inverse[3, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 3];
            inverse[2, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[2, 3] = -inverse[3, 3] * oneOverPivot;

            //
            double Z = inverse[0, 0] * PA.X + inverse[0, 1] * PA.Y + inverse[0, 2] * PA.Z + inverse[0, 3];
            if (Z > 0 && Z < ZMin)
            {
                double alpha = inverse[2, 0] * PA.X + inverse[2, 1] * PA.Y + inverse[2, 2] * PA.Z + inverse[2, 3];
                if (alpha >= 0 && alpha <= 1)
                {
                    double beta = inverse[3, 0] * PA.X + inverse[3, 1] * PA.Y + inverse[3, 2] * PA.Z + inverse[3, 3];
                    if (beta >= 0 && beta <= 1 && alpha + beta <= 1)
                    {
                        Vec3 pt = PA - alpha * VB - beta * VC;
                        ZMin = Z;
                        return pt;
                    }
                }
            }

            return null;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool CheckRay(Trg3 t, ref double ZMin, ref Vec3 oPoint)
        {
            if (Vec3.Cross(t.X, t.Y).LengthSquared == 0)
            {
                return false;
            }

            // convert the matrix to an array for easy looping
            double[,] inverse = {{Row0.X, Row0.Y, t.X.X, t.Y.X},
                                {Row1.X, Row1.Y,t.X.Y, t.Y.Y},
                                {Row2.X, Row2.Y, t.X.Z, t.Y.Z},
                                {Row3.X, Row3.Y, 0, 0} };

            double oneOverPivot;
            //
            //
            oneOverPivot = 1.0 / inverse[0, 0];
            inverse[0, 0] = oneOverPivot;
            inverse[0, 1] *= oneOverPivot;
            inverse[0, 2] *= oneOverPivot;
            inverse[0, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[1, 0];
            inverse[1, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 0];
            inverse[2, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 0];
            inverse[3, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[0, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[1, 1];
            inverse[1, 0] *= oneOverPivot;
            inverse[1, 1] = oneOverPivot;
            inverse[1, 2] *= oneOverPivot;
            inverse[1, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[0, 1];
            inverse[0, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[0, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[0, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[0, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 1];
            inverse[2, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[2, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 1];
            inverse[3, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[3, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[1, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[2, 2];
            inverse[2, 0] *= oneOverPivot;
            inverse[2, 1] *= oneOverPivot;
            inverse[2, 2] = oneOverPivot;
            inverse[2, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[0, 2];
            inverse[0, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[0, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[0, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[0, 3] -= inverse[2, 3] * oneOverPivot;
            oneOverPivot = inverse[1, 2];
            inverse[1, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[1, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[2, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 2];
            inverse[3, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[3, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[2, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[3, 3];
            inverse[3, 0] *= oneOverPivot;
            inverse[3, 1] *= oneOverPivot;
            inverse[3, 2] *= oneOverPivot;
            inverse[3, 3] = oneOverPivot;
            //
            oneOverPivot = inverse[0, 3];
            inverse[0, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[0, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[0, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[0, 3] = -inverse[3, 3] * oneOverPivot;
            oneOverPivot = inverse[1, 3];
            inverse[1, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[1, 3] = -inverse[3, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 3];
            inverse[2, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[2, 3] = -inverse[3, 3] * oneOverPivot;

            //
            double Z = inverse[0, 0] * t.O.X + inverse[0, 1] * t.O.Y + inverse[0, 2] * t.O.Z + inverse[0, 3];
            if (Z > 0 && Z < ZMin)
            {
                double alpha = inverse[2, 0] * t.O.X + inverse[2, 1] * t.O.Y + inverse[2, 2] * t.O.Z + inverse[2, 3];
                if (alpha >= 0 && alpha <= 1)
                {
                    double beta = inverse[3, 0] * t.O.X + inverse[3, 1] * t.O.Y + inverse[3, 2] * t.O.Z + inverse[3, 3];
                    if (beta >= 0 && beta <= 1 && alpha + beta <= 1)
                    {
                        oPoint = t.O - alpha * t.X - beta * t.Y;
                        ZMin = Z;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckRayXY(Vec4 p, double x, double y)
        {
            double[,] inverse = {
                                { Row0.X, Row0.Y, x, 0 },
                                { Row1.X, Row1.Y, 0, y },
                                { Row2.X, Row2.Y, 0, 0 },
                                { Row3.X, Row3.Y, 0, 0 } };
            //
            double oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[0, 0];
            inverse[0, 0] = oneOverPivot;
            inverse[0, 1] *= oneOverPivot;
            inverse[0, 2] *= oneOverPivot;
            inverse[0, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[1, 0];
            inverse[1, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 0];
            inverse[2, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 0];
            inverse[3, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[0, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[1, 1];
            inverse[1, 0] *= oneOverPivot;
            inverse[1, 1] = oneOverPivot;
            inverse[1, 2] *= oneOverPivot;
            inverse[1, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[2, 1];
            inverse[2, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[2, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 1];
            //inverse[3, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[3, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[1, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[2, 2];
            inverse[2, 0] *= oneOverPivot;
            inverse[2, 1] *= oneOverPivot;
            inverse[2, 2] = oneOverPivot;
            inverse[2, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[3, 2];
            //inverse[3, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[3, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[2, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[3, 3];
            //inverse[3, 0] *= oneOverPivot;
            inverse[3, 1] *= oneOverPivot;
            inverse[3, 2] *= oneOverPivot;
            inverse[3, 3] = oneOverPivot;
            //
            oneOverPivot = inverse[2, 3];
            //inverse[2, 0] -= inverse[3, 0] * oneOverPivot;
            //inverse[2, 1] -= inverse[3, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[2, 3] = -inverse[3, 3] * oneOverPivot;

            //
            double d;
            d = inverse[2, 0] * p.X + inverse[2, 2] * p.Z + inverse[2, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            d = inverse[3, 1] * p.Y + inverse[3, 2] * p.Z + inverse[3, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            return true;
        }

        public bool CheckRayYZ(Vec4 p1, double y, double z)
        {
            double[,] inverse = {{Row0.X, Row0.Y, 0, 0},
                                {Row1.X, Row1.Y, y,0},
                                {Row2.X, Row2.Y, 0, z},
                                {Row3.X, Row3.Y, 0,0} };
            double oneOverPivot;
            //
            //
            oneOverPivot = 1.0 / inverse[0, 0];
            inverse[0, 0] = oneOverPivot;
            inverse[0, 1] *= oneOverPivot;
            inverse[0, 2] *= oneOverPivot;
            inverse[0, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[1, 0];
            inverse[1, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 0];
            inverse[2, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 0];
            inverse[3, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[0, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[1, 1];
            inverse[1, 0] *= oneOverPivot;
            inverse[1, 1] = oneOverPivot;
            inverse[1, 2] *= oneOverPivot;
            inverse[1, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[2, 1];
            inverse[2, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[2, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 1];
            inverse[3, 0] -= inverse[1, 0] * oneOverPivot;
            //inverse[3, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[1, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[2, 2];
            inverse[2, 0] *= oneOverPivot;
            inverse[2, 1] *= oneOverPivot;
            inverse[2, 2] = oneOverPivot;
            inverse[2, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[3, 2];
            inverse[3, 0] -= inverse[2, 0] * oneOverPivot;
            //inverse[3, 1] -= inverse[2, 1] * oneOverPivot;
            inverse[3, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[2, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[3, 3];
            inverse[3, 0] *= oneOverPivot;
            //inverse[3, 1] *= oneOverPivot;
            inverse[3, 2] *= oneOverPivot;
            inverse[3, 3] = oneOverPivot;
            //
            oneOverPivot = inverse[2, 3];
            inverse[2, 0] -= inverse[3, 0] * oneOverPivot;
            //inverse[2, 1] -= inverse[3, 1] * oneOverPivot;
            //inverse[2, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[2, 3] = -inverse[3, 3] * oneOverPivot;
            //
            double d;
            d = inverse[2, 0] * p1.X + inverse[2, 1] * p1.Y + inverse[2, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            d = inverse[3, 0] * p1.X + inverse[3, 2] * p1.Z + inverse[3, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            return true;
        }

        public bool CheckRayZX(Vec4 p1, double z, double x)
        {
            double[,] inverse = {{Row0.X, Row0.Y, 0, x},
                                {Row1.X, Row1.Y, 0, 0},
                                {Row2.X, Row2.Y, z,0},
                                {Row3.X, Row3.Y, 0,0} };
            double oneOverPivot;
            //
            //
            oneOverPivot = 1.0 / inverse[0, 0];
            inverse[0, 0] = oneOverPivot;
            inverse[0, 1] *= oneOverPivot;
            inverse[0, 2] *= oneOverPivot;
            inverse[0, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[1, 0];
            inverse[1, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[1, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[1, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[1, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[2, 0];
            inverse[2, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[0, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 0];
            inverse[3, 0] = -inverse[0, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[0, 1] * oneOverPivot;
            inverse[3, 2] -= inverse[0, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[0, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[1, 1];
            inverse[1, 0] *= oneOverPivot;
            inverse[1, 1] = oneOverPivot;
            inverse[1, 2] *= oneOverPivot;
            inverse[1, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[2, 1];
            inverse[2, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[2, 1] = -inverse[1, 1] * oneOverPivot;
            inverse[2, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[2, 3] -= inverse[1, 3] * oneOverPivot;
            oneOverPivot = inverse[3, 1];
            inverse[3, 0] -= inverse[1, 0] * oneOverPivot;
            inverse[3, 1] = -inverse[1, 1] * oneOverPivot;
            //inverse[3, 2] -= inverse[1, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[1, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[2, 2];
            inverse[2, 0] *= oneOverPivot;
            inverse[2, 1] *= oneOverPivot;
            inverse[2, 2] = oneOverPivot;
            inverse[2, 3] *= oneOverPivot;
            //
            oneOverPivot = inverse[3, 2];
            inverse[3, 0] -= inverse[2, 0] * oneOverPivot;
            inverse[3, 1] -= inverse[2, 1] * oneOverPivot;
            //inverse[3, 2] = -inverse[2, 2] * oneOverPivot;
            inverse[3, 3] -= inverse[2, 3] * oneOverPivot;
            //
            oneOverPivot = 1.0 / inverse[3, 3];
            inverse[3, 0] *= oneOverPivot;
            inverse[3, 1] *= oneOverPivot;
            //inverse[3, 2] *= oneOverPivot;
            inverse[3, 3] = oneOverPivot;
            //
            oneOverPivot = inverse[2, 3];
            //inverse[2, 0] -= inverse[3, 0] * oneOverPivot;
            inverse[2, 1] -= inverse[3, 1] * oneOverPivot;
            //inverse[2, 2] -= inverse[3, 2] * oneOverPivot;
            inverse[2, 3] = -inverse[3, 3] * oneOverPivot;
            //
            double d;
            d = inverse[2, 1] * p1.Y + inverse[2, 2] * p1.Z + inverse[2, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            d = inverse[3, 0] * p1.X + inverse[3, 1] * p1.Y + inverse[3, 3]; if (d < 0 || d > 1)
            {
                return false;
            }

            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Mtx4 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Mtx4))
            {
                return false;
            }

            return this.Equals((Mtx4)obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
   

        public override string ToString()
        {
            return string.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        public Vec3 TransformPoint(Vec3 vec)
        {
            Vec3 result = new Vec3(
               vec.X * Row0.X + vec.Y * Row1.X + vec.Z * Row2.X + Row3.X,
               vec.X * Row0.Y + vec.Y * Row1.Y + vec.Z * Row2.Y + Row3.Y,
               vec.X * Row0.Z + vec.Y * Row1.Z + vec.Z * Row2.Z + Row3.Z);
            return result;
        }

        #endregion Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
    }

    public class Mtx4Comparer : IEqualityComparer<Mtx4>
    {
        #region Public Methods

        public bool CheckXY0(Vec4 x, Vec4 y)
        {
            if (Math.Round(x.X, 6) != Math.Round(y.X, 6))
            {
                return false;
            }

            if (Math.Round(x.Y, 6) != Math.Round(y.Y, 6))
            {
                return false;
            }

            if (Math.Round(x.Z, 6) != Math.Round(y.Z, 6))
            {
                return false;
            }

            return true;
        }

        public bool CheckXY1(Vec4 x, Vec4 y)
        {
            if (Math.Round(x.X, 3) != Math.Round(y.X, 3))
            {
                return false;
            }

            if (Math.Round(x.Y, 3) != Math.Round(y.Y, 3))
            {
                return false;
            }

            if (Math.Round(x.Z, 3) != Math.Round(y.Z, 3))
            {
                return false;
            }

            return true;
        }

        public bool Equals(Mtx4 x, Mtx4 y)
        {
            if (!CheckXY0(x.Row0, y.Row0))
            {
                return false;
            }

            if (!CheckXY0(x.Row1, y.Row1))
            {
                return false;
            }

            if (!CheckXY0(x.Row2, y.Row2))
            {
                return false;
            }

            if (!CheckXY1(x.Row3, y.Row3))
            {
                return false;
            }

            return true;
        }

        public int GetHashCode(Mtx4 obj)
        {
            return 0;
        }

        #endregion Public Methods
    }
}