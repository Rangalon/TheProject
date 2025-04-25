
using Math3D; 
using System;
using System.Linq;
using System.Xml.Serialization;

[assembly: CLSCompliant(true)]

namespace Rann.Components
{
    public class TVector4 : TComponent, IName
    {
        #region Public Fields

        [XmlIgnore]
        public TComponent W = 0;

        [XmlIgnore]
        public TComponent X = 0;

        [XmlIgnore]
        public TComponent Y = 0;

        [XmlIgnore]
        public TComponent Z = 0;

        #endregion Public Fields

        #region Public Constructors

        public TVector4()
        {
            if (TCalculation.Current != null) TCalculation.Current.Vectors.Add(this);
        }

        #endregion Public Constructors

        #region Public Properties

        public TComponent LengthSquared
        {
            get { return X * X + Y * Y + Z * Z + W * W; }
        }

        public override byte Level => 60;

        public string Name { get; set; }

        [XmlIgnore]
        public override double Position { get => throw new NotImplementedException(); set { } }

        [XmlIgnore]
        public TComponent RFormula
        {
            set
            {
                TVector4 v = (TVector4)value;
                if (X is TParameterComponent prmtrX) prmtrX.RFormula = v.X; else X = v.X;
                if (Y is TParameterComponent prmtrY) prmtrY.RFormula = v.Y; else Y = v.Y;
                if (Z is TParameterComponent prmtrZ) prmtrZ.RFormula = v.Z; else Z = v.Z;
                if (W is TParameterComponent prmtrW) prmtrW.RFormula = v.W; else W = v.W;
            }
        }

        public override string Text => throw new NotImplementedException();

        public Vec4 Value
        {
            get
            {
                Vec4 v = new Vec4
                {
                    X = X.Position,
                    Y = Y.Position,
                    Z = Z.Position,
                    W = W.Position
                };
                return v;
            }
        }

        [XmlAttribute]
        public string WFormulaText
        {
            get
            {
                if (W == null)
                    return "";
                return W.ToString();
            }
            set
            {
                W = TComponent.ParseFormula(value);
            }
        }

        [XmlAttribute]
        public string XFormulaText
        {
            get
            {
                if (X == null)
                    return "";
                return X.ToString();
            }
            set
            {
                X = TComponent.ParseFormula(value);
            }
        }

        [XmlAttribute]
        public string YFormulaText
        {
            get
            {
                if (Y == null)
                    return "";
                return Y.ToString();
            }
            set
            {
                Y = TComponent.ParseFormula(value);
            }
        }

        [XmlAttribute]
        public string ZFormulaText
        {
            get
            {
                if (Z == null)
                    return "";
                return Z.ToString();
            }
            set
            {
                Z = TComponent.ParseFormula(value);
            }
        }

        #endregion Public Properties

        #region Public Methods
       
        public static TVector4 CreateParametersPoint(string iName)
        {
            TVector4 v = new TVector4() { Name = iName };
            v.X = new TParameterComponent(iName, "x");
            v.Y = new TParameterComponent(iName, "y");
            v.Z = new TParameterComponent(iName, "z");
            v.W = 1;
            return v;
        }

        public static TVector4 CreateParametersVector(string iName)
        {
            TVector4 v = new TVector4() { Name = iName };
            v.X = new TParameterComponent(iName, "x");
            v.Y = new TParameterComponent(iName, "y");
            v.Z = new TParameterComponent(iName, "z");
            return v;
        }

        public static TComponent Dot(TVector4 iV1, TVector4 iV2)
        {
            return iV1.X * iV2.X + iV1.Y * iV2.Y + iV1.Z * iV2.Z + iV1.W * iV2.W;
        }

        public static TComponent operator -(TVector4 iV1, TVector4 iV2)
        {
            TVector4 V = new TVector4
            {
                X = iV1.X - iV2.X,
                Y = iV1.Y - iV2.Y,
                Z = iV1.Z - iV2.Z,
                W = iV1.W - iV2.W
            };
            return V;
        }

        public static TVector4 operator *(TVector4 iV, TComponent iD)
        {
            TVector4 v = new TVector4
            {
                X = iV.X * iD,
                Y = iV.Y * iD,
                Z = iV.Z * iD,
                W = iV.W * iD
            };
            return v;
        }

        public static TVector4 operator *(TVector4 iV1, TVector4 iV2)
        {
            TVector4 V = new TVector4
            {
                X = iV1.Y * iV2.Z - iV1.Z * iV2.Y,
                Y = iV1.Z * iV2.X - iV1.X * iV2.Z,
                Z = iV1.X * iV2.Y - iV1.Y * iV2.X,
                W = 0
            };
            return V;
        }

        public static TVector4 operator /(TVector4 iV, double iD)
        {
            TVector4 v = new TVector4();
            TValueComponent C = new TValueComponent(iD);
            v.X = iV.X / C;
            v.Y = iV.Y / C;
            v.Z = iV.Z / C;
            v.W = iV.W / C;
            return v;
        }

        public static TVector4 operator /(TVector4 iV, TComponent iD)
        {
            TVector4 v = new TVector4
            {
                X = iV.X / iD,
                Y = iV.Y / iD,
                Z = iV.Z / iD,
                W = iV.W / iD
            };
            return v;
        }

        public static TVector4 operator +(TVector4 iV1, TVector4 iV2)
        {
            TVector4 V = new TVector4
            {
                X = iV1.X + iV2.X,
                Y = iV1.Y + iV2.Y,
                Z = iV1.Z + iV2.Z,
                W = iV1.W + iV2.W
            };
            return V;
        }

        public static TVector4 TransformPoint(Vec3 vec, TMatrix4 mat)
        {
            TVector4 result = new TVector4()
            {
                X = vec.X * mat.Xx + vec.Y * mat.Yx + vec.Z * mat.Zx + mat.Ox,
                Y = vec.X * mat.Xy + vec.Y * mat.Yy + vec.Z * mat.Zy + mat.Oy,
                Z = vec.X * mat.Xz + vec.Y * mat.Yz + vec.Z * mat.Zz + mat.Oz,
                W = One
            };
            return result;
        }

        public override TComponent Factorises(TComponent factor)
        {
            throw new NotImplementedException();
        }

        public override TComponent GetDifferential(TParameterComponent iParameter)
        {
            throw new NotImplementedException();
        }

        public override TComponent[] GetFactors()
        {
            return new TComponent[0];
        }

        public override TParameterComponent[] GetParameters()
        {
            return X.GetParameters().Concat(Y.GetParameters()).Concat(Z.GetParameters()).Concat(W.GetParameters()).ToArray();
        }

        public void Normalize()
        {
            TComponent n = TComponent.Sqrt(LengthSquared);
            X /= n;
            Y /= n;
            Z /= n;
            W /= n;
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
            return this;
        }

        public void SetFormulas(TVector4 iV)
        {
            ((TParameterComponent)X).RFormula = iV.X;
            ((TParameterComponent)Y).RFormula = iV.Y;
            ((TParameterComponent)Z).RFormula = iV.Z;
            ((TParameterComponent)W).RFormula = iV.W;
        }

        public void SetMaximums(Vec4 iV)
        {
            ((TParameterComponent)X).Maximum = iV.X;
            ((TParameterComponent)Y).Maximum = iV.Y;
            ((TParameterComponent)Z).Maximum = iV.Z;
            ((TParameterComponent)W).Maximum = iV.W;
        }

        public void SetMinimums(Vec4 iV)
        {
            ((TParameterComponent)X).Minimum = iV.X;
            ((TParameterComponent)Y).Minimum = iV.Y;
            ((TParameterComponent)Z).Minimum = iV.Z;
            ((TParameterComponent)W).Minimum = iV.W;
        }

        public override string ToString()
        {
            return X.ToString() + " ; " + Y.ToString() + " ; " + Z.ToString() + " ; " + W.ToString();
        }

        #endregion Public Methods
    }
}