using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Math3D
{
    /// <summary>
    /// Represents a 4x4 matrix containing 3D rotation, scale, transform, and projection with Double-precision components.
    /// </summary>
    /// <seealso cref="Mtx4f"/>
    public struct Mtx5 : IEquatable<Mtx5>
    {
        #region Public Fields

        public static Mtx5 Identity = new Mtx5(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        public static double Ro = 5e5;//5e6;
        public static double RoInv;
        public static Mtx5 SwitchXY = Mtx5.CreateRotationZ(Math.PI / 2);

        public Vec4 Row0;

        public Vec4 Row1;

        public Vec4 Row2;

        public Vec4 Row3;

        public Vec4 Row4;

        #endregion Public Fields

        #region Public Constructors

        static Mtx5()
        {
            RoInv = 1 / Ro;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Mtx5(
            double m00, double m01, double m02, double m03,
            double m10, double m11, double m12, double m13,
            double m20, double m21, double m22, double m23,
            double m30, double m31, double m32, double m33,
            double m40, double m41, double m42, double m43)
        {
            Row0 = new Vec4(m00, m01, m02, m03);
            Row1 = new Vec4(m10, m11, m12, m13);
            Row2 = new Vec4(m20, m21, m22, m23);
            Row3 = new Vec4(m30, m31, m32, m33);
            Row4 = new Vec4(m40, m41, m42, m43);
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

        public double LengthSquared { get { return Row4.FullLengthSquared; } }

        #endregion Public Properties

        #region Public Methods

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx5 CreateFromAxisAngle(Vec3 axis, double angle)
        {
            Mtx5 result = Mtx5.Identity;
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
        public static Mtx5 CreateRotation(double a, double b, double c, double t)
        {
            Mtx5 result = Identity;
            double ct = System.Math.Cos(t);
            double st = System.Math.Sin(t);
            //result.Row0.X = a * a + (1 - a * a) * ct; result.Row1.X = a * b * (1 - ct) - c * st; result.Row2.X = a * c * (1 - ct) + b * st;
            //result.Row0.Y = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row2.Y = b * c * (1 - ct) - a * st;
            //result.Row0.Z = a * c * (1 - ct) - b * st; result.Row1.Z = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            result.Row0.X = a * a + (1 - a * a) * ct; result.Row0.Y = a * b * (1 - ct) - c * st; result.Row0.Z = a * c * (1 - ct) + b * st;
            result.Row1.X = a * b * (1 - ct) + c * st; result.Row1.Y = b * b + (1 - b * b) * ct; result.Row1.Z = b * c * (1 - ct) - a * st;
            result.Row2.X = a * c * (1 - ct) - b * st; result.Row2.Y = b * c * (1 - ct) + a * st; result.Row2.Z = c * c + (1 - c * c) * ct;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx5 CreateRotationX(double angle)
        {
            Mtx5 result = Identity;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);
            result.Row1.Y = cos;
            result.Row1.Z = sin;
            result.Row2.Y = -sin;
            result.Row2.Z = cos;
            return result;
        }

        public static Mtx5 CreateRotationY(double angle)
        {
            Mtx5 result = Identity;
            double cos = System.Math.Cos(angle);
            double sin = System.Math.Sin(angle);
            result.Row2.Z = cos;
            result.Row2.X = sin;
            result.Row0.Z = -sin;
            result.Row0.X = cos;
            return result;
        }

        public static Mtx5 CreateRotationZ(double angle)
        {
            Mtx5 result = Identity;
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
        public static Mtx5 CreateScale(double x, double y, double z, double w)
        {
            Mtx5 result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
            result.Row2.W = w;
            return result;
        }

        public static Mtx5 CreateScale(double d)
        {
            Mtx5 result = Identity;
            result.Row0.X = d;
            result.Row1.Y = d;
            result.Row2.Z = d;
            result.Row3.Z = d;
            return result;
        }

        public static Mtx5 CreateScale(Vec4 v)
        {
            Mtx5 result = Identity;
            result.Row0.X = v.X;
            result.Row1.Y = v.Y;
            result.Row2.Z = v.Z;
            result.Row3.W = v.W;
            return result;
        }

        public static Mtx5 CreateScaledTranslation(Vec4 Scale, Vec4 vector)
        {
            Mtx5 result = Identity;
            result.Row0.X = (float)Scale.X;
            result.Row1.Y = (float)Scale.Y;
            result.Row2.Z = (float)Scale.Z;
            result.Row3.W = (float)Scale.W;
            result.Row4.X = (float)vector.X;
            result.Row4.Y = (float)vector.Y;
            result.Row4.Z = (float)vector.Z;
            result.Row4.W = (float)vector.W;
            return result;
        }

        public static Mtx5 CreateScaledTranslation(double d, Vec4 v)
        {
            Mtx5 result = Identity;
            result.Row0.X = d;
            result.Row1.Y = d;
            result.Row2.Z = d;
            result.Row3.W = d;
            result.Row4.X = v.X;
            result.Row4.Y = v.Y;
            result.Row4.Z = v.Z;
            result.Row4.W = v.W;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx5 CreateTranslation(Vec4 vector)
        {
            Mtx5 result = Identity;
            result.Row4.X = vector.X;
            result.Row4.Y = vector.Y;
            result.Row4.Z = vector.Z;
            result.Row4.W = vector.W;
            return result;
        }

        public static Mtx5 CreateTranslation(double x, double y, double z)
        {
            Mtx5 result = Identity;
            result.Row3.X = x;
            result.Row3.Y = y;
            result.Row3.Z = z;
            return result;
        }

        public static Mtx5 Invert(Mtx5 mat)
        {
            int[] colIdx = { 0, 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            double[,] inverse = {
                                {mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W,0},
                                {mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W,0},
                                {mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W,0},
                                {mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W,0},
                                {mat.Row4.X, mat.Row4.Y, mat.Row4.Z, mat.Row4.W,1} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 5; i++)
            {
                // Find the largest pivot value
                double maxPivot = 0.0;
                for (int j = 0; j < 5; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (int k = 0; k < 5; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                double absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot)
                                {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (pivotIdx[k] > 0)
                            {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol)
                {
                    for (int k = 0; k < 5; ++k)
                    {
                        double f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                double pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                double oneOverPivot = 1.0 / pivot;
                inverse[icol, icol] = 1.0;
                for (int k = 0; k < 5; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 5; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        double f = inverse[j, icol];
                        inverse[j, icol] = 0.0;
                        for (int k = 0; k < 5; ++k)
                        {
                            inverse[j, k] -= inverse[icol, k] * f;
                        }
                    }
                }
            }

            for (int j = 4; j >= 0; --j)
            {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 5; ++k)
                {
                    double f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            mat.Row0 = new Vec4(inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3]);
            mat.Row1 = new Vec4(inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3]);
            mat.Row2 = new Vec4(inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3]);
            mat.Row3 = new Vec4(inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
            mat.Row4 = new Vec4(inverse[4, 0], inverse[4, 1], inverse[4, 2], inverse[4, 3]);
            return mat;
        }

        //public static void Move(Vec4 vec4, ref Mtx5 Matrix, ref Mtx5 iMatrix)
        //{
        //    MoveTo(Matrix.Row4 + vec4, ref Matrix, ref iMatrix);
        //}

        //public static void MoveTo(Vec4 vec, ref Mtx5 mm, ref Mtx5 imm)
        //{
        //    //Console.WriteLine();
        //    //Console.WriteLine(vec);
        //    //Console.WriteLine(Row3);
        //    vec *= RoInv;

        //    Mtx5 m = mm;
        //    m.Row3 = vec; m.Row3.Normalize();
        //    //Console.WriteLine(m.Row3);
        //    m.Row0 -= Vec4.Dot(m.Row3, m.Row0) * m.Row3; m.Row0.Normalize();
        //    if (Math.Round(Vec4.Dot(m.Row3, m.Row0), 4) != 0)
        //    {
        //        m.Row0 = new Vec4(m.Row3.Y, m.Row3.Z, m.Row3.W, -m.Row3.X);
        //        m.Row0 -= Vec4.Dot(m.Row3, m.Row0) * m.Row3; m.Row0.Normalize();
        //    }
        //    //Console.WriteLine(m.Row0);
        //    //Console.WriteLine(Vec4.Dot(m.Row3, m.Row0));
        //    m.Row1 -= Vec4.Dot(m.Row3, m.Row1) * m.Row3; m.Row1.Normalize();
        //    m.Row1 -= Vec4.Dot(m.Row0, m.Row1) * m.Row0; m.Row1.Normalize();
        //    if (Math.Round(Vec4.Dot(m.Row3, m.Row1), 4) != 0 || Math.Round(Vec4.Dot(m.Row0, m.Row1), 4) != 0)
        //    {
        //        m.Row1 = new Vec4(m.Row3.Y + m.Row0.Z, m.Row3.Z + m.Row0.W, m.Row3.W + m.Row0.X, -m.Row3.X - m.Row0.Y);
        //        m.Row1 -= Vec4.Dot(m.Row3, m.Row1) * m.Row3; m.Row1.Normalize();
        //        m.Row1 -= Vec4.Dot(m.Row0, m.Row1) * m.Row0; m.Row1.Normalize();
        //    }
        //    //Console.WriteLine(m.Row1);
        //    //Console.WriteLine(Vec4.Dot(m.Row3, m.Row1));
        //    //Console.WriteLine(Vec4.Dot(m.Row0, m.Row1));
        //    m.Row2 -= Vec4.Dot(m.Row3, m.Row2) * m.Row3; m.Row2.Normalize();
        //    m.Row2 -= Vec4.Dot(m.Row0, m.Row2) * m.Row0; m.Row2.Normalize();
        //    m.Row2 -= Vec4.Dot(m.Row1, m.Row2) * m.Row1; m.Row2.Normalize();
        //    if (Math.Round(Vec4.Dot(m.Row3, m.Row2), 4) != 0 || Math.Round(Vec4.Dot(m.Row0, m.Row2), 4) != 0 || Math.Round(Vec4.Dot(m.Row1, m.Row2), 4) != 0)
        //    {
        //        m.Row2 = new Vec4(m.Row3.Y + m.Row0.Z + m.Row1.W, m.Row3.Z + m.Row0.W + m.Row1.X, m.Row3.W + m.Row0.X + m.Row1.Y, -m.Row3.X - m.Row0.Y - m.Row1.Z);
        //        m.Row2 -= Vec4.Dot(m.Row3, m.Row2) * m.Row3; m.Row2.Normalize();
        //        m.Row2 -= Vec4.Dot(m.Row0, m.Row2) * m.Row0; m.Row2.Normalize();
        //        m.Row2 -= Vec4.Dot(m.Row1, m.Row2) * m.Row1; m.Row2.Normalize();
        //    }
        //    //Console.WriteLine(m.Row2);
        //    //Console.WriteLine(Vec4.Dot(m.Row3, m.Row2));
        //    //Console.WriteLine(Vec4.Dot(m.Row0, m.Row2));
        //    //Console.WriteLine(Vec4.Dot(m.Row1, m.Row2));

        //    m.Row4 = m.Row3 * Ro;
        //    //Console.WriteLine(this);
        //    Mtx5 im = m; im.Invert();
        //    mm = m; imm = im;
        //}

        //public static void MoveTo(Mtx5 mtx, ref Mtx5 Matrix, ref Mtx5 iMatrix)
        //{
        //    MoveTo(mtx.Row4, ref Matrix, ref iMatrix);
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator !=(Mtx5 left, Mtx5 right)
        {
            return !left.Equals(right);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx5 operator *(Mtx5 left, Mtx5 right)
        {
            double
                lM11 = left.Row0.X, lM12 = left.Row0.Y, lM13 = left.Row0.Z, lM14 = left.Row0.W,
                lM21 = left.Row1.X, lM22 = left.Row1.Y, lM23 = left.Row1.Z, lM24 = left.Row1.W,
                lM31 = left.Row2.X, lM32 = left.Row2.Y, lM33 = left.Row2.Z, lM34 = left.Row2.W,
                lM41 = left.Row3.X, lM42 = left.Row3.Y, lM43 = left.Row3.Z, lM44 = left.Row3.W,
                lM51 = left.Row4.X, lM52 = left.Row4.Y, lM53 = left.Row4.Z, lM54 = left.Row4.W,
                rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
                rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
                rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
                rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W,
                rM51 = right.Row4.X, rM52 = right.Row4.Y, rM53 = right.Row4.Z, rM54 = right.Row4.W;
            Mtx5 result = new Mtx5();
            result.Row0.X = (lM11 * rM11) + (lM12 * rM21) + (lM13 * rM31) + (lM14 * rM41);
            result.Row0.Y = (lM11 * rM12) + (lM12 * rM22) + (lM13 * rM32) + (lM14 * rM42);
            result.Row0.Z = (lM11 * rM13) + (lM12 * rM23) + (lM13 * rM33) + (lM14 * rM43);
            result.Row0.W = (lM11 * rM14) + (lM12 * rM24) + (lM13 * rM34) + (lM14 * rM44);
            result.Row1.X = (lM21 * rM11) + (lM22 * rM21) + (lM23 * rM31) + (lM24 * rM41);
            result.Row1.Y = (lM21 * rM12) + (lM22 * rM22) + (lM23 * rM32) + (lM24 * rM42);
            result.Row1.Z = (lM21 * rM13) + (lM22 * rM23) + (lM23 * rM33) + (lM24 * rM43);
            result.Row1.W = (lM21 * rM14) + (lM22 * rM24) + (lM23 * rM34) + (lM24 * rM44);
            result.Row2.X = (lM31 * rM11) + (lM32 * rM21) + (lM33 * rM31) + (lM34 * rM41);
            result.Row2.Y = (lM31 * rM12) + (lM32 * rM22) + (lM33 * rM32) + (lM34 * rM42);
            result.Row2.Z = (lM31 * rM13) + (lM32 * rM23) + (lM33 * rM33) + (lM34 * rM43);
            result.Row2.W = (lM31 * rM14) + (lM32 * rM24) + (lM33 * rM34) + (lM34 * rM44);
            result.Row3.X = (lM41 * rM11) + (lM42 * rM21) + (lM43 * rM31) + (lM44 * rM41);
            result.Row3.Y = (lM41 * rM12) + (lM42 * rM22) + (lM43 * rM32) + (lM44 * rM42);
            result.Row3.Z = (lM41 * rM13) + (lM42 * rM23) + (lM43 * rM33) + (lM44 * rM43);
            result.Row3.W = (lM41 * rM14) + (lM42 * rM24) + (lM43 * rM34) + (lM44 * rM44);
            result.Row4.X = (lM51 * rM11) + (lM52 * rM21) + (lM53 * rM31) + (lM54 * rM41) + rM51;
            result.Row4.Y = (lM51 * rM12) + (lM52 * rM22) + (lM53 * rM32) + (lM54 * rM42) + rM52;
            result.Row4.Z = (lM51 * rM13) + (lM52 * rM23) + (lM53 * rM33) + (lM54 * rM43) + rM53;
            result.Row4.W = (lM51 * rM14) + (lM52 * rM24) + (lM53 * rM34) + (lM54 * rM44) + rM54;
            return result;
        }

        public static Vec4 operator *(Vec4 left, Mtx5 right)
        {
            double
                lM51 = left.X, lM52 = left.Y, lM53 = left.Z, lM54 = left.W,
                rM11 = right.Row0.X, rM12 = right.Row0.Y, rM13 = right.Row0.Z, rM14 = right.Row0.W,
                rM21 = right.Row1.X, rM22 = right.Row1.Y, rM23 = right.Row1.Z, rM24 = right.Row1.W,
                rM31 = right.Row2.X, rM32 = right.Row2.Y, rM33 = right.Row2.Z, rM34 = right.Row2.W,
                rM41 = right.Row3.X, rM42 = right.Row3.Y, rM43 = right.Row3.Z, rM44 = right.Row3.W,
                rM51 = right.Row4.X, rM52 = right.Row4.Y, rM53 = right.Row4.Z, rM54 = right.Row4.W;
            Vec4 result = new Vec4();
            result.X = (lM51 * rM11) + (lM52 * rM21) + (lM53 * rM31) + (lM54 * rM41) + rM51;
            result.Y = (lM51 * rM12) + (lM52 * rM22) + (lM53 * rM32) + (lM54 * rM42) + rM52;
            result.Z = (lM51 * rM13) + (lM52 * rM23) + (lM53 * rM33) + (lM54 * rM43) + rM53;
            result.W = (lM51 * rM14) + (lM52 * rM24) + (lM53 * rM34) + (lM54 * rM44) + rM54;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static bool operator ==(Mtx5 left, Mtx5 right)
        {
            return left.Equals(right);
        }

        public static Mtx5 QuickInvert(Mtx5 mat)
        {
            double[,] inverse = {{mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W},
                                {mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W},
                                {mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W},
                                {mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W} };
            //
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
            mat.Row0 = new Vec4(inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3]);
            mat.Row1 = new Vec4(inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3]);
            mat.Row2 = new Vec4(inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3]);
            mat.Row3 = new Vec4(inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
            return mat;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public static Mtx5 Scale(double x, double y, double z)
        {
            Mtx5 result = Identity;
            result.Row0.X = x;
            result.Row1.Y = y;
            result.Row2.Z = z;
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public bool Equals(Mtx5 other)
        {
            return
                Row0 == other.Row0 &&
                Row1 == other.Row1 &&
                Row2 == other.Row2 &&
                Row3 == other.Row3 &&
                Row4 == other.Row4;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(object obj)
        {
            if (!(obj is Mtx5))
            {
                return false;
            }

            return this.Equals((Mtx5)obj);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return Row0.GetHashCode() ^ Row1.GetHashCode() ^ Row2.GetHashCode() ^ Row3.GetHashCode() ^ Row4.GetHashCode();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public void Invert()
        {
            this = Invert(this);
        }

        public override string ToString()
        {
            return string.Format("{0} \n{1} \n{2} \n{3} \n{4}", Row0, Row1, Row2, Row3, Row4);
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

    public class Mtx5Comparer : IEqualityComparer<Mtx5>
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

        public bool Equals(Mtx5 x, Mtx5 y)
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

        public int GetHashCode(Mtx5 obj)
        {
            return 0;
        }

        #endregion Public Methods
    }
}