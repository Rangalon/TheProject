
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;

namespace CiliaElements
{
    public abstract class TSurface : TEntity, ISurface
    {
        #region Public Fields

        public double UMax;

        public double UMin;

        public double VMax;

        public double VMin;

        #endregion Public Fields

        #region Public Constructors

        public TSurface(TSolidElement iElement)
            : base(iElement)
        {
            SurfaceUVCoordinates = new Dictionary<int, Vec2>();
        }

        #endregion Public Constructors

        #region Public Properties

        public Dictionary<int, Vec2> SurfaceUVCoordinates { get; set; }

        #endregion Public Properties

        #region Public Methods

        public int BuildPoint(Vec2 iUV, Vec4 iColor, TCloud iPositions, TCloud iNormals)
        {
            int functionReturnValue;
            Vec3 pt = default;
            Vec3 n = default;
            GetPointAndNormal(iUV, ref pt, ref n, Element.SolidElementConstruction.Unit);
            //
            functionReturnValue = iPositions.Vectors.Max;
            //Array.Resize(ref iPositions.Vectors, functionReturnValue + 1);
            //Array.Resize(ref iNormals.Vectors, functionReturnValue + 1);
            iPositions.Vectors.Push(pt);
            iNormals.Vectors.Push(n);
            SurfaceUVCoordinates.Add(functionReturnValue, iUV);
            return functionReturnValue;
        }

        public int BuildPoint(Vec3 iPt, TCloud iPositions, TCloud iNormals)
        {
            int functionReturnValue;
            Vec3 pt = default;
            Vec3 n = default;
            Vec2 uv = default;
            GetPointAndNormal(iPt, ref pt, ref n, ref uv, Element.SolidElementConstruction.Unit);
            //
            functionReturnValue = iPositions.Vectors.Max;
            //Array.Resize(ref iPositions.Vectors, functionReturnValue + 1);
            //Array.Resize(ref iNormals.Vectors, functionReturnValue + 1);
            iPositions.Vectors.Push(pt);
            iNormals.Vectors.Push(n);
            SurfaceUVCoordinates.Add(functionReturnValue, uv);
            return functionReturnValue;
        }

        public abstract void GetPointAndNormal(Vec2 iUV, ref Vec3 oPt, ref Vec3 oNormal, double iUnit);

        public abstract void GetPointAndNormal(Vec3 iPt, ref Vec3 oPt, ref Vec3 oNormal, ref Vec2 oUV, double iUnit);

        #endregion Public Methods
    }
}