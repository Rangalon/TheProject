
using Math3D;
using System;

namespace CiliaElements
{
    public struct SBoundingBox3
    {
        #region Public Fields

        public static readonly SBoundingBox3 Default;
        public Vec4 MaxPosition;
        public Vec4 MinPosition;

        #endregion Public Fields

        #region Public Constructors

        static SBoundingBox3()
        {
            Default = new SBoundingBox3();
            Default.MaxPosition = new Vec4(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, 1);
            Default.MinPosition = new Vec4(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, 1);
        }

        public SBoundingBox3(Vec4 v)
        {
            MaxPosition = v;
            MinPosition = MaxPosition;
        }

        public SBoundingBox3(Vec3 v)
        {
            MaxPosition = new Vec4(v, 1);
            MinPosition = MaxPosition;
        }

        #endregion Public Constructors

        #region Public Properties

        public Vec4 CtrPosition
        {
            get { return (MaxPosition + MinPosition) * 0.5; }
        }

        public Vec4 Size
        {
            get { return (MaxPosition - MinPosition); }
        }

        #endregion Public Properties

        #region Public Methods

        public static SBoundingBox3 operator +(SBoundingBox3 b1, SBoundingBox3 b2)
        {
            SBoundingBox3 b = new SBoundingBox3();
            b.MinPosition.X = Math.Min(b1.MinPosition.X, b2.MinPosition.X);
            b.MinPosition.Y = Math.Min(b1.MinPosition.Y, b2.MinPosition.Y);
            b.MinPosition.Z = Math.Min(b1.MinPosition.Z, b2.MinPosition.Z);
            b.MaxPosition.X = Math.Min(b1.MaxPosition.X, b2.MaxPosition.X);
            b.MaxPosition.Y = Math.Min(b1.MaxPosition.Y, b2.MaxPosition.Y);
            b.MaxPosition.Z = Math.Min(b1.MaxPosition.Z, b2.MaxPosition.Z);
            return b;
        }

        public static SBoundingBox3 Transform(SBoundingBox3 iBox, Mtx4 iMatrix)
        {
            Vec4 v = iBox.MaxPosition;
            v = Vec4.Transform(v, iMatrix);
            SBoundingBox3 b = new SBoundingBox3(v);
            //
            Vec4 dv = iBox.MaxPosition - iBox.MinPosition;
            v -= dv.X * iMatrix.Row0; b.CheckPosition(v);
            v -= dv.Y * iMatrix.Row1; b.CheckPosition(v);
            v += dv.X * iMatrix.Row0; b.CheckPosition(v);
            v -= dv.Z * iMatrix.Row2; b.CheckPosition(v);
            v += dv.Y * iMatrix.Row1; b.CheckPosition(v);
            v -= dv.X * iMatrix.Row0; b.CheckPosition(v);
            v -= dv.Y * iMatrix.Row1; b.CheckPosition(v);
            return b;
        }

        public void AddBox(SBoundingBox3 iBox)
        {
            MinPosition.X = Math.Min(MinPosition.X, iBox.MinPosition.X);
            MinPosition.Y = Math.Min(MinPosition.Y, iBox.MinPosition.Y);
            MinPosition.Z = Math.Min(MinPosition.Z, iBox.MinPosition.Z);
            MaxPosition.X = Math.Max(MaxPosition.X, iBox.MaxPosition.X);
            MaxPosition.Y = Math.Max(MaxPosition.Y, iBox.MaxPosition.Y);
            MaxPosition.Z = Math.Max(MaxPosition.Z, iBox.MaxPosition.Z);
        }

        public void CheckPosition(Vec3f iPosition)
        {
            if (MaxPosition.X == double.NegativeInfinity)
            {
                MaxPosition.X = iPosition.X;
                MaxPosition.Y = iPosition.Y;
                MaxPosition.Z = iPosition.Z;
                MinPosition.X = iPosition.X;
                MinPosition.Y = iPosition.Y;
                MinPosition.Z = iPosition.Z;
            }
            else
            {
                MinPosition.X = Math.Min(MinPosition.X, iPosition.X);
                MinPosition.Y = Math.Min(MinPosition.Y, iPosition.Y);
                MinPosition.Z = Math.Min(MinPosition.Z, iPosition.Z);
                MaxPosition.X = Math.Max(MaxPosition.X, iPosition.X);
                MaxPosition.Y = Math.Max(MaxPosition.Y, iPosition.Y);
                MaxPosition.Z = Math.Max(MaxPosition.Z, iPosition.Z);
            }
        }

        public void CheckPosition(Vec3 iPosition)
        {
            MinPosition.X = Math.Min(MinPosition.X, iPosition.X);
            MinPosition.Y = Math.Min(MinPosition.Y, iPosition.Y);
            MinPosition.Z = Math.Min(MinPosition.Z, iPosition.Z);
            MaxPosition.X = Math.Max(MaxPosition.X, iPosition.X);
            MaxPosition.Y = Math.Max(MaxPosition.Y, iPosition.Y);
            MaxPosition.Z = Math.Max(MaxPosition.Z, iPosition.Z);
        }

        public void CheckPosition(Vec4 iPosition)
        {
            if (MaxPosition.X == double.NegativeInfinity)
            {
                MaxPosition.X = iPosition.X;
                MaxPosition.Y = iPosition.Y;
                MaxPosition.Z = iPosition.Z;
                MinPosition.X = iPosition.X;
                MinPosition.Y = iPosition.Y;
                MinPosition.Z = iPosition.Z;
            }
            else
            {
                MinPosition.X = Math.Min(MinPosition.X, iPosition.X);
                MinPosition.Y = Math.Min(MinPosition.Y, iPosition.Y);
                MinPosition.Z = Math.Min(MinPosition.Z, iPosition.Z);
                MaxPosition.X = Math.Max(MaxPosition.X, iPosition.X);
                MaxPosition.Y = Math.Max(MaxPosition.Y, iPosition.Y);
                MaxPosition.Z = Math.Max(MaxPosition.Z, iPosition.Z);
            }
        }

        public SBoundingBox3 Get2DBox(Mtx4 iPMatrix)
        {
            SBoundingBox3 b = new SBoundingBox3();
            double w = 1 / Math.Max(iPMatrix.Row2.W * MinPosition.Z, iPMatrix.Row2.W * MaxPosition.Z);
            b.MinPosition.X = iPMatrix.Row0.X * MinPosition.X * w; b.MaxPosition.X = b.MinPosition.X;
            b.MinPosition.Y = iPMatrix.Row1.Y * MinPosition.Y * w; b.MaxPosition.Y = b.MinPosition.Y;
            double d = iPMatrix.Row0.X * MaxPosition.X * w; b.MinPosition.X = Math.Min(b.MinPosition.X, d); b.MaxPosition.X = Math.Max(b.MaxPosition.X, d);
            d = iPMatrix.Row1.Y * MaxPosition.Y * w; b.MinPosition.Y = Math.Min(b.MinPosition.Y, d); b.MaxPosition.Y = Math.Max(b.MaxPosition.Y, d);
            return b;
        }

        public Vec4[] GetCorners(Mtx4 iMatrix)
        {
            Vec4[] table = new Vec4[8];
            table[0] = Vec4.Transform(new Vec4(MaxPosition.X, MaxPosition.Y, MaxPosition.Z, 1), iMatrix);
            table[1] = Vec4.Transform(new Vec4(MinPosition.X, MaxPosition.Y, MaxPosition.Z, 1), iMatrix);
            table[2] = Vec4.Transform(new Vec4(MaxPosition.X, MinPosition.Y, MaxPosition.Z, 1), iMatrix);
            table[3] = Vec4.Transform(new Vec4(MinPosition.X, MinPosition.Y, MaxPosition.Z, 1), iMatrix);
            table[4] = Vec4.Transform(new Vec4(MaxPosition.X, MaxPosition.Y, MinPosition.Z, 1), iMatrix);
            table[5] = Vec4.Transform(new Vec4(MinPosition.X, MaxPosition.Y, MinPosition.Z, 1), iMatrix);
            table[6] = Vec4.Transform(new Vec4(MaxPosition.X, MinPosition.Y, MinPosition.Z, 1), iMatrix);
            table[7] = Vec4.Transform(new Vec4(MinPosition.X, MinPosition.Y, MinPosition.Z, 1), iMatrix);
            return table;
        }

        public double GetMinimalSquaredDistance(Vec4 oVector)
        {
            double dx, dy, dz;
            if (oVector.X < MinPosition.X) dx = MinPosition.X - oVector.X; else if (oVector.X > MaxPosition.X) dx = oVector.X - MaxPosition.X; else dx = 0;
            if (oVector.Y < MinPosition.Y) dy = MinPosition.Y - oVector.Y; else if (oVector.Y > MaxPosition.Y) dy = oVector.Y - MaxPosition.Y; else dy = 0;
            if (oVector.Z < MinPosition.Z) dz = MinPosition.Z - oVector.Z; else if (oVector.Z > MaxPosition.Z) dz = oVector.Z - MaxPosition.Z; else dz = 0;
            return dx * dx + dy * dy + dz * dz;
        }

        public void ResetBox(Vec3 iPosition)
        {
            MaxPosition.X = iPosition.X;
            MaxPosition.Y = iPosition.Y;
            MaxPosition.Z = iPosition.Z;
            MinPosition.X = iPosition.X;
            MinPosition.Y = iPosition.Y;
            MinPosition.Z = iPosition.Z;
        }

        #endregion Public Methods
    }
}