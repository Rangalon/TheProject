namespace Math3D
{
    public struct Rec
    {
        #region Public Fields

        public double H;
        public double W;
        public double X;
        public double Y;

        #endregion Public Fields

        #region Public Constructors

        public Rec(double v1, double v2, double v3, double v4)
        {
            X = v1; Y = v2; W = v3; H = v4;
        }

        #endregion Public Constructors
    }
}