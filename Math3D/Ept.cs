using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace Math3D
{
    public struct Ept
    {
        // According to https://fr.wikipedia.org/wiki/WGS_84

        #region Public Fields

        //public static Geoid Geoid = new Geoid("egm2008-5", Properties.Resources.egm2008_5);
        //public static readonly Geoid Geoid;

        public readonly static double A = 6378137.0;
        public readonly static double BigR2 = A * A;
        public readonly static double PI = 3.141592653589793;
        public readonly static double TwoPi = PI * 2.0;
        public readonly static double Earth_Flatness = 1.0 / 298.257223563;
        public readonly static double EF3 = (1.0 - Earth_Flatness);
        public readonly static double SmallR = A * EF3;
        public readonly static double BigR2SmallR2 = A * A - SmallR * SmallR;
        public readonly static double BigR4SmallR4 = BigR2SmallR2 * BigR2SmallR2;
        public readonly static double DegToRad = PI / 180.0;
        public readonly static double FeetToMeter = 0.3048;
        public readonly static double NMToMeter = 1852;
        public readonly static double MeterToFeet = 1 / FeetToMeter;
        public readonly static double MeterToNM = 1 / NMToMeter;
        public readonly static double e2 = (2.0 - Earth_Flatness) * Earth_Flatness;
        public readonly static double EF2 = (1.0 - Earth_Flatness) * (1.0 - Earth_Flatness);
        public readonly static double FourThree = 4.0 / 3.0;
        public readonly static double MinusHeightBigR2SmallR2 = -8.0 * BigR2SmallR2;
        public readonly static double OneBigR = 1.0 / A;
        public readonly static double OneThree = 1.0 / 3.0;
        public readonly static double PiErr = TwoPi - 1e-10;
        public readonly static double RadToDeg = 180.0 / PI;

        #endregion Public Fields

        #region Private Fields


        [XmlIgnore]
        private double hei;

        [XmlIgnore]
        private double lat;

        [XmlIgnore]
        private double lon;

        #endregion Private Fields

        #region Public Constructors

        static Ept()
        {
            try
            {
                //Assembly ass = typeof(Ept).Assembly;
                //FileInfo fi = new FileInfo(ass.Location);
                //Geoid = new Geoid("egm2008-5", fi.Directory.FullName);
                //Geoid = new Geoid("egm2008-5", Properties.Resources.egm2008_5);
                //double h = Geoid.ConvertHeightToEllipsoid(43.616214, 1.379693, 151);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);

            }
        }

        //public double GetGeoidHeight() => Geoid.ConvertHeightToGeoid(lat, lon, hei);


        public Ept(double ilon, double ilat, double ihei)
        {
            hei = ihei;
            lon = ilon;
            lat = ilat;
            X = 0; Y = 0; Z = 0;
            ComputeLatLonPoint();
        }

        public Ept(Ept p1, Ept p2) : this(0.5 * (p1 + p2), 0)
        {
            hei = 0;
            ComputeLatLonPoint();
        }

        public Ept(Vec3 vec, double deltaE) : this(vec.X, vec.Y, vec.Z, deltaE)
        { }

        public Ept(double x, double y, double z, double deltaE)
        {
            X = x; Y = y; Z = z;
            double tlon = Math.Atan2(y, x);
            lon = tlon * RadToDeg;
            //if (lon > PI) lon -= TwoPi;

            double b = SmallR;
            if (z < 0) b *= -1;
            double bz = b * z;

            double r = Math.Sqrt(x * x + y * y);
            double bigrr = OneBigR / r;
            double bigrr2 = bigrr * bigrr;

            double e = (bz - BigR2SmallR2) * bigrr;

            double p = FourThree * ((bz * bz - BigR4SmallR4) * bigrr2 + 1);
            double q = MinusHeightBigR2SmallR2 * bz * bigrr2;
            double d = p * p * p + q * q;

            double v;
            double g;
            double t;

            if (d >= 0)
            {
                d = Math.Sqrt(d);
                v = Math.Pow(d - q, OneThree) - Math.Pow(d + q, OneThree);
            }
            else
                v = 2 * Math.Sqrt(-p) * Math.Cos(Math.Acos(q / (p * Math.Sqrt(-p))) * OneThree);


            if (v * v < Math.Abs(p)) v = -(v * v * v + 2 * q) / (3 * p);

            g = (Math.Sqrt(e * e + v) + e) * 0.5;
            t = Math.Sqrt(g * g + ((bz + BigR2SmallR2) * bigrr - v * g) / (2 * g - e)) - g;

            double tlat = Math.Atan(A * (1 - t * t) / (2 * b * t));
            lat = RadToDeg * tlat;

            hei = (r - A * t) * Math.Cos(tlat) + (z - b) * Math.Sin(tlat) + deltaE;
            ComputeLatLonPoint();
        }

        #endregion Public Constructors

        #region Public Properties

        [XmlAttribute]
        public double Hei { get { return hei; } set { hei = value; ComputeLatLonPoint(); } }

        [XmlAttribute]
        public double Lat { get { return lat; } set { lat = value; ComputeLatLonPoint(); } }

        public double Length { get { return Math.Sqrt(X * X + Y * Y + Z * Z); } }

        [XmlAttribute]
        public double Lon { get { return lon; } set { lon = value; ComputeLatLonPoint(); } }

        [XmlIgnore]
        public double X { get; private set; }

        [XmlIgnore]
        public double Y { get; private set; }

        [XmlIgnore]
        public double Z { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public static Vec3 operator -(Ept p1, Ept p2)
        {
            p1.Hei = 0; p2.hei = 0;
            return new Vec3(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
        }

        public static bool operator !=(Ept p1, Ept p2)
        {
            return (p1.lon != p2.lon || p1.lat != p2.lat || p1.hei != p2.hei);
        }

        public static Vec3 operator +(Ept p1, Ept p2)
        {
            p1.Hei = 0; p2.hei = 0;
            return new Vec3(p2.X + p1.X, p2.Y + p1.Y, p2.Z + p1.Z);
        }

        public static Vec3 operator +(Ept p1, Vec3 p2)
        {
            p1.Hei = 0;
            return new Vec3(p2.X + p1.X, p2.Y + p1.Y, p2.Z + p1.Z);
        }

        public static Vec3 operator +(Vec3 p1, Ept p2)
        {
            p2.hei = 0;
            return new Vec3(p2.X + p1.X, p2.Y + p1.Y, p2.Z + p1.Z);
        }

        public static bool operator ==(Ept p1, Ept p2)
        {
            return (p1.lon != p2.lon && p1.lat != p2.lat && p1.hei != p2.hei);
        }

        public static Vec3 Transform(Ept vec, Mtx4 mat)
        {
            return Vec3.Transform(new Vec3(vec.X, vec.Y, vec.Z), mat);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeLatLonPoint()
        {



            double lon = DegToRad * Lon;
            double lat = DegToRad * Lat;
            double slat = Math.Sin(lat);
            double g = A / Math.Sqrt(1 - e2 * slat * slat);
            Z = (g * EF2 + hei) * slat;
            g = (g + hei) * Math.Cos(lat);
            X = g * Math.Cos(lon);
            Y = g * Math.Sin(lon);


        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", Lon, Lat);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static Ept FromGeoid(double v1, double v2, double v3) => new Ept(v1, v2, v3 + v3 - (new Ept(v1, v2, v3)).GetGeoidHeight());

        #endregion Public Methods

        //public override string ToString()
        //{
        //    return X.ToString(CultureInfo.InvariantCulture) + ";" + Y.ToString(CultureInfo.InvariantCulture) + ";" + Z.ToString(CultureInfo.InvariantCulture) + ";" + Lon.ToString(CultureInfo.InvariantCulture) + ";" + Lat.ToString(CultureInfo.InvariantCulture);
        //}
    }
}