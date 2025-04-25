using Math3D; 
using System;
using System.Xml.Serialization;

namespace Rann.Components
{
    public class TMatrix4 : TComponent, IName
    {
        #region Public Fields

        public TComponent Ow = 1;

        public TComponent Ox = 0;

        public TComponent Oy = 0;

        public TComponent Oz = 0;

        public TComponent Xw = 0;

        public TComponent Xx = 1;

        public TComponent Xy = 0;

        public TComponent Xz = 0;

        public TComponent Yw = 0;

        public TComponent Yx = 0;

        public TComponent Yy = 1;

        public TComponent Yz = 0;

        public TComponent Zw = 0;

        public TComponent Zx = 0;

        public TComponent Zy = 0;

        public TComponent Zz = 1;

        #endregion Public Fields

        #region Public Constructors

        public TMatrix4()
        {
            if (TCalculation.Current != null) TCalculation.Current.Matrixes.Add(this);
        }

        #endregion Public Constructors

        #region Public Properties

        public override byte Level => throw new NotImplementedException();

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        public override double Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Text
        {
            get
            {
                string s = string.Format("Ox = {0}", Ox.Text);
                s += "\n" + string.Format("Oy = {0}", Oy.Text);
                s += "\n" + string.Format("Oz = {0}", Oz);

                s += "\n" + string.Format("Xx = {0}", Xx);
                s += "\n" + string.Format("Xy = {0}", Xy);
                s += "\n" + string.Format("Xz = {0}", Xz);

                s += "\n" + string.Format("Yx = {0}", Yx);
                s += "\n" + string.Format("Yy = {0}", Yy);
                s += "\n" + string.Format("Yz = {0}", Yz);

                s += "\n" + string.Format("Zx = {0}", Zx);
                s += "\n" + string.Format("Zy = {0}", Zy);
                s += "\n" + string.Format("Zz = {0}", Zz);
                return s;
            }
        }

        public Mtx4 Value
        {
            get
            {
                Mtx4 m = new Mtx4();
                m.Row0 = new Vec4(Xx.Position, Xy.Position, Xz.Position, Xw.Position);
                m.Row1 = new Vec4(Yx.Position, Yy.Position, Yz.Position, Yw.Position);
                m.Row2 = new Vec4(Zx.Position, Zy.Position, Zz.Position, Zw.Position);
                m.Row3 = new Vec4(Ox.Position, Oy.Position, Oz.Position, Ow.Position);
                return m;
            }
        }

        #endregion Public Properties

        #region Public Methods
      
        public static TMatrix4 CreateParametersMatrix(string iName)
        {
            TMatrix4 m = new TMatrix4() { Name = iName };
            m.Xx = new TParameterComponent(iName, "Xx");
            m.Xy = new TParameterComponent(iName, "Xy");
            m.Xz = new TParameterComponent(iName, "Xz");
            m.Xw = new TParameterComponent(iName, "Xw");
            m.Yx = new TParameterComponent(iName, "Yx");
            m.Yy = new TParameterComponent(iName, "Yy");
            m.Yz = new TParameterComponent(iName, "Yz");
            m.Yw = new TParameterComponent(iName, "Yw");
            m.Zx = new TParameterComponent(iName, "Zx");
            m.Zy = new TParameterComponent(iName, "Zy");
            m.Zz = new TParameterComponent(iName, "Zz");
            m.Zw = new TParameterComponent(iName, "Zw");
            m.Ox = new TParameterComponent(iName, "Ox");
            m.Oy = new TParameterComponent(iName, "Oy");
            m.Oz = new TParameterComponent(iName, "Oz");
            m.Ow = new TParameterComponent(iName, "Ow");
            //m.Row0 = TVector4.CreateParametersVector(iName + ":R0");
            //m.Row1 = TVector4.CreateParametersVector(iName + ":R1");
            //m.Row2 = TVector4.CreateParametersVector(iName + ":R2");
            //m.Row3 = TVector4.CreateParametersVector(iName + ":R3");
            return m;
        }

        public static TMatrix4 CreateParametersMatrix(string iName, Mtx4 iMat)
        {
            TMatrix4 m = new TMatrix4();
            //m.Row0 = TVector4.CreateParametersVector(iName + ":R0", iMat.Row0);
            //m.Row1 = TVector4.CreateParametersVector(iName + ":R1", iMat.Row1);
            //m.Row2 = TVector4.CreateParametersVector(iName + ":R2", iMat.Row2);
            //m.Row3 = TVector4.CreateParametersVector(iName + ":R3", iMat.Row3);
            return m;
        }

        public static TMatrix4 CreateRotationX(TComponent iVariable)
        {
            TMatrix4 m = new TMatrix4();
            m.Xx = 1;
            m.Yy = TComponent.Cos(iVariable);
            m.Yz = TComponent.Sin(iVariable);
            m.Zz = TComponent.Cos(iVariable);
            m.Zy = -TComponent.Sin(iVariable);
            m.Ow = 1;
            return m;
        }

        public static TMatrix4 CreateRotationY(TComponent iVariable)
        {
            TMatrix4 m = new TMatrix4();
            m.Yy = 1;
            m.Zz = TComponent.Cos(iVariable);
            m.Zx = TComponent.Sin(iVariable);
            m.Xx = TComponent.Cos(iVariable);
            m.Xz = -TComponent.Sin(iVariable);
            m.Ow = 1;
            return m;
        }

        public static TMatrix4 CreateRotationZ(TComponent iVariable)
        {
            TMatrix4 m = new TMatrix4();
            m.Zz = 1;
            m.Xx = TComponent.Cos(iVariable);
            m.Xy = TComponent.Sin(iVariable);
            m.Yy = TComponent.Cos(iVariable);
            m.Yx = -TComponent.Sin(iVariable);
            m.Ow = 1;
            return m;
        }

        public static TMatrix4 CreateTranslation(TComponent iX, TComponent iY, TComponent iZ)
        {
            TMatrix4 m = new TMatrix4();
            m.Xx = 1;
            m.Yy = 1;
            m.Zz = 1;
            m.Ox = iX;
            m.Oy = iY;
            m.Oz = iZ;
            m.Ow = 1;
            return m;
        }

        public static TMatrix4 Invert(TMatrix4 mat)
        {
            Int32[] colIdx = { 0, 0, 0, 0 };
            Int32[] rowIdx = { 0, 0, 0, 0 };
            Int32[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            TComponent[,] inverse = {
                {mat.Xx, mat.Xy, mat.Xz, mat.Xw},
                {mat.Yx, mat.Yy, mat.Yz, mat.Yw},
                {mat.Zx, mat.Zy, mat.Zz, mat.Zw},
                {mat.Ox, mat.Oy, mat.Oz, mat.Ow} };
            Int32 icol = 0;
            Int32 irow = 0;
            for (Int32 i = 0; i < 4; i++)
            {
                // Find the largest pivot value
                for (Int32 j = 0; j < 4; j++)
                {
                    if (pivotIdx[j] != 0)
                    {
                        for (Int32 k = 0; k < 4; ++k)
                        {
                            if (pivotIdx[k] == -1)
                            {
                                if (inverse[j, k] != 0)
                                {
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
                    for (Int32 k = 0; k < 4; ++k)
                    {
                        TComponent f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                TComponent pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0)
                {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                TComponent oneOverPivot = 1.0 / pivot;
                inverse[icol, icol] = 1.0;
                for (Int32 k = 0; k < 4; ++k)
                {
                    inverse[icol, k] *= oneOverPivot;
                }

                // Do elimination of non-diagonal elements
                for (Int32 j = 0; j < 4; ++j)
                {
                    // check this isn't on the diagonal
                    if (icol != j)
                    {
                        TComponent f = inverse[j, icol];
                        inverse[j, icol] = 0.0;
                        for (Int32 k = 0; k < 4; ++k)
                        {
                            inverse[j, k] -= inverse[icol, k] * f;
                        }
                    }
                }
            }

            for (Int32 j = 3; j >= 0; --j)
            {
                Int32 ir = rowIdx[j];
                Int32 ic = colIdx[j];
                for (Int32 k = 0; k < 4; ++k)
                {
                    TComponent f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }

            TMatrix4 m = new TMatrix4();
            m.Xx = inverse[0, 0]; m.Xy = inverse[0, 1]; m.Xz = inverse[0, 2]; m.Xw = inverse[0, 3];
            m.Yx = inverse[1, 0]; m.Yy = inverse[1, 1]; m.Yz = inverse[1, 2]; m.Yw = inverse[1, 3];
            m.Zx = inverse[2, 0]; m.Zy = inverse[2, 1]; m.Zz = inverse[2, 2]; m.Zw = inverse[2, 3];
            m.Ox = inverse[3, 0]; m.Oy = inverse[3, 1]; m.Oz = inverse[3, 2]; m.Ow = inverse[3, 3];

            return m;
        }

        public static TMatrix4 operator *(TMatrix4 m1, TMatrix4 iM1)
        {
            TMatrix4 m = new TMatrix4();

            m.Xx = iM1.Xx * m1.Xx + iM1.Yx * m1.Xy + iM1.Zx * m1.Xz + iM1.Ox * m1.Xw;
            m.Xy = iM1.Xy * m1.Xx + iM1.Yy * m1.Xy + iM1.Zy * m1.Xz + iM1.Oy * m1.Xw;
            m.Xz = iM1.Xz * m1.Xx + iM1.Yz * m1.Xy + iM1.Zz * m1.Xz + iM1.Oz * m1.Xw;
            m.Xw = iM1.Xw * m1.Xx + iM1.Yw * m1.Xy + iM1.Zw * m1.Xz + iM1.Ow * m1.Xw;

            m.Yx = iM1.Xx * m1.Yx + iM1.Yx * m1.Yy + iM1.Zx * m1.Yz + iM1.Ox * m1.Yw;
            m.Yy = iM1.Xy * m1.Yx + iM1.Yy * m1.Yy + iM1.Zy * m1.Yz + iM1.Oy * m1.Yw;
            m.Yz = iM1.Xz * m1.Yx + iM1.Yz * m1.Yy + iM1.Zz * m1.Yz + iM1.Oz * m1.Yw;
            m.Yw = iM1.Xw * m1.Yx + iM1.Yw * m1.Yy + iM1.Zw * m1.Yz + iM1.Ow * m1.Yw;

            m.Zx = iM1.Xx * m1.Zx + iM1.Yx * m1.Zy + iM1.Zx * m1.Zz + iM1.Ox * m1.Zw;
            m.Zy = iM1.Xy * m1.Zx + iM1.Yy * m1.Zy + iM1.Zy * m1.Zz + iM1.Oy * m1.Zw;
            m.Zz = iM1.Xz * m1.Zx + iM1.Yz * m1.Zy + iM1.Zz * m1.Zz + iM1.Oz * m1.Zw;
            m.Zw = iM1.Xw * m1.Zx + iM1.Yw * m1.Zy + iM1.Zw * m1.Zz + iM1.Ow * m1.Zw;

            m.Ox = iM1.Xx * m1.Ox + iM1.Yx * m1.Oy + iM1.Zx * m1.Oz + iM1.Ox * m1.Ow;
            m.Oy = iM1.Xy * m1.Ox + iM1.Yy * m1.Oy + iM1.Zy * m1.Oz + iM1.Oy * m1.Ow;
            m.Oz = iM1.Xz * m1.Ox + iM1.Yz * m1.Oy + iM1.Zz * m1.Oz + iM1.Oz * m1.Ow;
            m.Ow = iM1.Xw * m1.Ox + iM1.Yw * m1.Oy + iM1.Zw * m1.Oz + iM1.Ow * m1.Ow;

            return m;
        }

        public static TVector4 operator *(TMatrix4 iM, TVector4 iV)
        {
            TVector4 V = new TVector4();
            V.X = iM.Xx * iV.X + iM.Yx * iV.Y + iM.Zx * iV.Z + iM.Ox * iV.W;
            V.Y = iM.Xy * iV.X + iM.Yy * iV.Y + iM.Zy * iV.Z + iM.Oy * iV.W;
            V.Z = iM.Xz * iV.X + iM.Yz * iV.Y + iM.Zz * iV.Z + iM.Oz * iV.W;
            V.W = iM.Xw * iV.X + iM.Yw * iV.Y + iM.Zw * iV.Z + iM.Ow * iV.W;
            return V;
        }

        public override TComponent Factorises(TComponent factor)
        {
            throw new NotImplementedException();
        }

        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            throw new NotImplementedException();
        }

        public TMatrix4 GetDifferentialMatrix(TParameterComponent iParameter)
        {
            TMatrix4 m = new TMatrix4();
            m.Ox = Ox.GetDifferential(iParameter);
            m.Oy = Oy.GetDifferential(iParameter);
            m.Oz = Oz.GetDifferential(iParameter);
            m.Ow = Ow.GetDifferential(iParameter);
            m.Xx = Xx.GetDifferential(iParameter);
            m.Xy = Xy.GetDifferential(iParameter);
            m.Xz = Xz.GetDifferential(iParameter);
            m.Xw = Xw.GetDifferential(iParameter);
            m.Yx = Yx.GetDifferential(iParameter);
            m.Yy = Yy.GetDifferential(iParameter);
            m.Yz = Yz.GetDifferential(iParameter);
            m.Yw = Yw.GetDifferential(iParameter);
            m.Zx = Zx.GetDifferential(iParameter);
            m.Zy = Zy.GetDifferential(iParameter);
            m.Zz = Zz.GetDifferential(iParameter);
            m.Zw = Zw.GetDifferential(iParameter);
            return m;
        }

        public override TComponent[] GetFactors()
        {
            throw new NotImplementedException();
        }

        public override TParameterComponent[] GetParameters()
        {
            throw new NotImplementedException();
        }

        public TMatrix4 Invert()
        {
            throw new NotImplementedException();
        }

        public void Parametrize(string name)
        {
            Name = name;
            TParameterComponent prm;
            if (!(Ox is TValueComponent)) { prm = new TParameterComponent(name, "Ox") { RFormula = Ox }; Ox = prm; }
            if (!(Oy is TValueComponent)) { prm = new TParameterComponent(name, "Oy") { RFormula = Oy }; Oy = prm; }
            if (!(Oz is TValueComponent)) { prm = new TParameterComponent(name, "Oz") { RFormula = Oz }; Oz = prm; }
            if (!(Ow is TValueComponent)) { prm = new TParameterComponent(name, "Ow") { RFormula = Ow }; Ow = prm; }
            if (!(Xx is TValueComponent)) { prm = new TParameterComponent(name, "Xx") { RFormula = Xx }; Xx = prm; }
            if (!(Xy is TValueComponent)) { prm = new TParameterComponent(name, "Xy") { RFormula = Xy }; Xy = prm; }
            if (!(Xz is TValueComponent)) { prm = new TParameterComponent(name, "Xz") { RFormula = Xz }; Xz = prm; }
            if (!(Xw is TValueComponent)) { prm = new TParameterComponent(name, "Xw") { RFormula = Xw }; Xw = prm; }
            if (!(Yx is TValueComponent)) { prm = new TParameterComponent(name, "Yx") { RFormula = Yx }; Yx = prm; }
            if (!(Yy is TValueComponent)) { prm = new TParameterComponent(name, "Yy") { RFormula = Yy }; Yy = prm; }
            if (!(Yz is TValueComponent)) { prm = new TParameterComponent(name, "Yz") { RFormula = Yz }; Yz = prm; }
            if (!(Yw is TValueComponent)) { prm = new TParameterComponent(name, "Yw") { RFormula = Yw }; Yw = prm; }
            if (!(Zx is TValueComponent)) { prm = new TParameterComponent(name, "Zx") { RFormula = Zx }; Zx = prm; }
            if (!(Zy is TValueComponent)) { prm = new TParameterComponent(name, "Zy") { RFormula = Zy }; Zy = prm; }
            if (!(Zz is TValueComponent)) { prm = new TParameterComponent(name, "Zz") { RFormula = Zz }; Zz = prm; }
            if (!(Zw is TValueComponent)) { prm = new TParameterComponent(name, "Zw") { RFormula = Zw }; Zw = prm; }
        }

        public override TComponent Reduced()
        {
            throw new NotImplementedException();
        }

        public override void Replace(TComponent oldComp, TComponent newComp)
        {
            throw new NotImplementedException();
        }

        public override TComponent ReplaceComponent(TComponent iOldComp, TComponent iNewComp)
        {
            throw new NotImplementedException();
        }

        public void SetFormulas(TMatrix4 iM)
        {
            //Row0.SetFormulas(iM.Row0);
            //Row1.SetFormulas(iM.Row1);
            //Row2.SetFormulas(iM.Row2);
            //Row3.SetFormulas(iM.Row3);
        }

        public void SetMaximums(Mtx4 iM)
        {
            //Row0.SetMaximums(iM.Row0);
            //Row1.SetMaximums(iM.Row1);
            //Row2.SetMaximums(iM.Row2);
            //Row3.SetMaximums(iM.Row3);
        }

        public void SetMinimums(Mtx4 iM)
        {
            //Row0.SetMinimums(iM.Row0);
            //Row1.SetMinimums(iM.Row1);
            //Row2.SetMinimums(iM.Row2);
            //Row3.SetMinimums(iM.Row3);
        }

        public TMatrix4 Transpose()
        {
            TMatrix4 m = new TMatrix4();
            m.Xx = Xx; m.Xy = Yx; m.Xz = Zx; m.Xw = Ox;
            m.Yx = Xy; m.Yy = Yy; m.Yz = Zy; m.Yw = Oy;
            m.Zx = Xz; m.Zy = Yz; m.Zz = Zz; m.Zw = Oz;
            m.Ox = Xw; m.Oy = Yw; m.Oz = Zw; m.Ow = Ow;
            return m;
        }

        #endregion Public Methods
    }
}