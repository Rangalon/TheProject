using Rann.Components;
using System;

namespace CiliaElements
{
    public class TConstraint
    {
        #region Public Fields

        public EConstraintType ConstraintType;
        public bool Disabled;
        public double MaxTarget;
        public double MinTarget;
        public TReference R1;
        public TReference R2;

        #endregion Public Fields

        #region Public Constructors

        public TConstraint(TReference iR1, TReference iR2)
        {
            TManager.WorldConstraints.Add(this);
            if (iR1.ReferenceType < iR2.ReferenceType)
            {
                R1 = iR2;
                R2 = iR1;
            }
            else
            {
                R1 = iR1;
                R2 = iR2;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public string ReferencesType
        {
            get
            {
                return R1.ReferenceType.ToString() + "-" + R2.ReferenceType.ToString();
            }
        }

        #endregion Public Properties

        #region Public Methods

        public TVector4[] GetVectors()
        {
            switch (R1.ReferenceType)
            {
                case EReferenceType.Point:
                    switch (R2.ReferenceType)
                    {
                        case EReferenceType.Point:
                            TVector4 O1 = R1.GetVector(R1.O);
                            TVector4 O2 = R2.GetVector(R2.O);
                            return new TVector4[1] { (TVector4)(O1 - O2) };

                        case EReferenceType.Line:
                            TVector4 P = R1.GetVector(R1.P1);
                            TVector4 O = R2.GetVector(R2.O);
                            TVector4 U = R2.GetVector(R2.U);
                            TVector4 V = (TVector4)(P - O);
                            V = (TVector4)(V - (U * TVector4.Dot(U, V)));
                            return new TVector4[1] { V };

                        case EReferenceType.Plane:
                            throw new Exception("Not managed!");
                        default:
                            throw new Exception("Not managed!");
                    }
                case EReferenceType.Line:
                    switch (R2.ReferenceType)
                    {
                        case EReferenceType.Point:
                            throw new Exception("Not managed!");
                        case EReferenceType.Line:
                            TVector4 P11 = R1.GetVector(R1.P1);
                            TVector4 P31 = R1.GetVector(R1.P3);
                            TVector4 P12 = R1.GetVector(R2.P1);
                            TVector4 P32 = R1.GetVector(R2.P3);
                            TVector4 O1 = R1.GetVector(R1.O);
                            TVector4 U1 = R1.GetVector(R1.U);
                            TVector4 O2 = R2.GetVector(R2.O);
                            TVector4 U2 = R2.GetVector(R2.U);
                            TVector4 V11 = (TVector4)(P11 - O2 - U2 * TVector4.Dot(U2, (TVector4)(P11 - O2)));
                            TVector4 V31 = (TVector4)(P31 - O2 - U2 * TVector4.Dot(U2, (TVector4)(P31 - O2)));
                            TVector4 V12 = (TVector4)(P12 - O1 - U1 * TVector4.Dot(U1, (TVector4)(P12 - O1)));
                            TVector4 V32 = (TVector4)(P32 - O1 - U1 * TVector4.Dot(U1, (TVector4)(P32 - O1)));
                            return new TVector4[4] { V11, V31, V12, V32 };

                        case EReferenceType.Plane:
                            throw new Exception("Not managed!");
                        default:
                            throw new Exception("Not managed!");
                    }
                case EReferenceType.Plane:
                    switch (R2.ReferenceType)
                    {
                        case EReferenceType.Point:
                            throw new Exception("Not managed!");
                        case EReferenceType.Line:
                            throw new Exception("Not managed!");
                        case EReferenceType.Plane:
                            TVector4 P11 = R1.GetVector(R1.P1);
                            TVector4 P21 = R1.GetVector(R1.P2);
                            TVector4 P31 = R1.GetVector(R1.P3);
                            TVector4 P12 = R1.GetVector(R2.P1);
                            TVector4 P22 = R1.GetVector(R2.P2);
                            TVector4 P32 = R1.GetVector(R2.P3);
                            //
                            TVector4 O1 = R1.GetVector(R1.O);
                            TVector4 W1 = R1.GetVector(R1.W);
                            TVector4 O2 = R2.GetVector(R2.O);
                            TVector4 W2 = R2.GetVector(R2.W);
                            //
                            TVector4 V11 = W2 * TVector4.Dot(W2, (TVector4)(P11 - O2));
                            TVector4 V21 = W2 * TVector4.Dot(W2, (TVector4)(P21 - O2));
                            TVector4 V31 = W2 * TVector4.Dot(W2, (TVector4)(P31 - O2));
                            TVector4 V12 = W1 * TVector4.Dot(W1, (TVector4)(P12 - O1));
                            TVector4 V22 = W1 * TVector4.Dot(W1, (TVector4)(P22 - O1));
                            TVector4 V32 = W1 * TVector4.Dot(W1, (TVector4)(P32 - O1));
                            //
                            return new TVector4[6] { V11, V21, V31, V12, V22, V32 };

                        default:
                            throw new Exception("Not managed!");
                    }
                default:
                    throw new Exception("Not managed!");
            }
        }

        #endregion Public Methods
    }
}