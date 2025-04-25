using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements
{
    public struct SBoundingBox4
    {
        #region Public Fields

        public static readonly SBoundingBox4 Default;
        public Vec4 MaxPosition;
        public Vec4 MinPosition;

        #endregion Public Fields

        #region Public Constructors

        static SBoundingBox4()
        {
            Default = new SBoundingBox4();
            Default.MaxPosition = new Vec4(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            Default.MinPosition = new Vec4(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
        }

        public SBoundingBox4(Vec4 v)
        {
            MaxPosition = v;
            MinPosition = MaxPosition;
        }

        public SBoundingBox4(Vec3 v)
        {
            MaxPosition = new Vec4(v, 0);
            //MaxPosition.Normalize();
            //MaxPosition *= Mtx5.Ro;
            MinPosition = MaxPosition;
        }

        #endregion Public Constructors

        #region Public Properties

        public Vec4 Size
        {
            get { return (MaxPosition - MinPosition); }
        }

        #endregion Public Properties

        #region Public Methods

        public static explicit operator SBoundingBox3(SBoundingBox4 b4)
        {
            SBoundingBox3 b3 = new SBoundingBox3
            {
                MaxPosition = b4.MaxPosition,
                MinPosition = b4.MinPosition
            };
            b3.MaxPosition.W = 1;
            b3.MinPosition.W = 1;
            return b3;
        }

        public static SBoundingBox4 Transform(SBoundingBox4 iBox, Mtx5 iMatrix)
        {
            Vec4 v = iBox.MaxPosition * iMatrix;
            SBoundingBox4 b = new SBoundingBox4(v); // MMMM 15
                                                    //
            Vec4 dv = iBox.MaxPosition - iBox.MinPosition;
            Vec4 dX = dv.X * iMatrix.Row0;
            Vec4 dY = dv.Y * iMatrix.Row1;
            Vec4 dZ = dv.Z * iMatrix.Row2;
            Vec4 dW = dv.W * iMatrix.Row3;

            v -= dX; b.CheckPosition(v); // mMMM 14
            v -= dY; b.CheckPosition(v); // mmMM 12
            v += dX; b.CheckPosition(v); // MmMM 13
            v -= dZ; b.CheckPosition(v); // MmmM 09
            v -= dX; b.CheckPosition(v); // mmmM 08
            v += dY; b.CheckPosition(v); // mMmM 10
            v += dX; b.CheckPosition(v); // MMmM 11
            v -= dW; b.CheckPosition(v); // MMmm 03
            v -= dX; b.CheckPosition(v); // mMmm 02
            v -= dY; b.CheckPosition(v); // mmmm 00
            v += dX; b.CheckPosition(v); // Mmmm 01
            v += dZ; b.CheckPosition(v); // MmMm 05
            v -= dX; b.CheckPosition(v); // mmMm 04
            v += dY; b.CheckPosition(v); // mMMm 06
            v += dX; b.CheckPosition(v); // MMMm 07

            return b;
        }

        public static SBoundingBox4 Transform(SBoundingBox4 iBox, Mtx4 iMatrix)
        {
            Vec4 v = iBox.MaxPosition * iMatrix;
            SBoundingBox4 b = new SBoundingBox4(v); // MMMM 15
                                                    //
            Vec4 dv = iBox.MaxPosition - iBox.MinPosition;
            Vec4 dX = dv.X * iMatrix.Row0;
            Vec4 dY = dv.Y * iMatrix.Row1;
            Vec4 dZ = dv.Z * iMatrix.Row2;
            Vec4 dW = dv.W * iMatrix.Row3;

            v -= dX; b.CheckPosition(v); // mMMM 14
            v -= dY; b.CheckPosition(v); // mmMM 12
            v += dX; b.CheckPosition(v); // MmMM 13
            v -= dZ; b.CheckPosition(v); // MmmM 09
            v -= dX; b.CheckPosition(v); // mmmM 08
            v += dY; b.CheckPosition(v); // mMmM 10
            v += dX; b.CheckPosition(v); // MMmM 11
            v -= dW; b.CheckPosition(v); // MMmm 03
            v -= dX; b.CheckPosition(v); // mMmm 02
            v -= dY; b.CheckPosition(v); // mmmm 00
            v += dX; b.CheckPosition(v); // Mmmm 01
            v += dZ; b.CheckPosition(v); // MmMm 05
            v -= dX; b.CheckPosition(v); // mmMm 04
            v += dY; b.CheckPosition(v); // mMMm 06
            v += dX; b.CheckPosition(v); // MMMm 07

            return b;
        }

        public void CheckPosition(Vec3 iPosition)
        {
            MinPosition.X = Math.Min(MinPosition.X, iPosition.X);
            MinPosition.Y = Math.Min(MinPosition.Y, iPosition.Y);
            MinPosition.Z = Math.Min(MinPosition.Z, iPosition.Z);
            MinPosition.W = Math.Min(MinPosition.W, 0);
            MaxPosition.X = Math.Max(MaxPosition.X, iPosition.X);
            MaxPosition.Y = Math.Max(MaxPosition.Y, iPosition.Y);
            MaxPosition.Z = Math.Max(MaxPosition.Z, iPosition.Z);
            MaxPosition.W = Math.Max(MaxPosition.W, 0);
        }

        public void CheckPosition(Vec4 iPosition)
        {
            MinPosition.X = Math.Min(MinPosition.X, iPosition.X);
            MinPosition.Y = Math.Min(MinPosition.Y, iPosition.Y);
            MinPosition.Z = Math.Min(MinPosition.Z, iPosition.Z);
            MinPosition.W = Math.Min(MinPosition.W, iPosition.W);
            MaxPosition.X = Math.Max(MaxPosition.X, iPosition.X);
            MaxPosition.Y = Math.Max(MaxPosition.Y, iPosition.Y);
            MaxPosition.Z = Math.Max(MaxPosition.Z, iPosition.Z);
            MaxPosition.W = Math.Max(MaxPosition.W, iPosition.W);
        }

        #endregion Public Methods
    }
}