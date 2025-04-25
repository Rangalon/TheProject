using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static System.Math;

namespace Math3D
{
    public enum ConvertFlag
    {
        EllipsoidToGeoid = -1,

        None = 0,

        GeoidToEllipsoid = 1
    }
    public sealed class Geoid : IDisposable
    {


        private static readonly uint pixel_size_ = 2;
        private static readonly uint pixel_max_ = 0xffffu;

        private const int stencilsize_ = 12;
        private const int nterms_ = ((3 + 1) * (3 + 2)) / 2;

        private const int c0_ = 240;
        private const int c0n_ = 372;
        private const int c0s_ = 372;

        private static readonly Memory<int> c3_ = new[]
        {
          9, -18, -88,    0,  96,   90,   0,   0, -60, -20,
         -9,  18,   8,    0, -96,   30,   0,   0,  60, -20,
          9, -88, -18,   90,  96,    0, -20, -60,   0,   0,
        186, -42, -42, -150, -96, -150,  60,  60,  60,  60,
         54, 162, -78,   30, -24,  -90, -60,  60, -60,  60,
         -9, -32,  18,   30,  24,    0,  20, -60,   0,   0,
         -9,   8,  18,   30, -96,    0, -20,  60,   0,   0,
         54, -78, 162,  -90, -24,   30,  60, -60,  60, -60,
        -54,  78,  78,   90, 144,   90, -60, -60, -60, -60,
          9,  -8, -18,  -30, -24,    0,  20,  60,   0,   0,
         -9,  18, -32,    0,  24,   30,   0,   0, -60,  20,
          9, -18,  -8,    0, -24,  -30,   0,   0,  60,  20,
        };
        private static readonly ReadOnlyMemory<int> c3n_ = new[]
        {
            0, 0, -131, 0,  138,  144, 0,   0, -102, -31,
            0, 0,    7, 0, -138,   42, 0,   0,  102, -31,
            62, 0,  -31, 0,    0,  -62, 0,   0,    0,  31,
        124, 0,  -62, 0,    0, -124, 0,   0,    0,  62,
        124, 0,  -62, 0,    0, -124, 0,   0,    0,  62,
            62, 0,  -31, 0,    0,  -62, 0,   0,    0,  31,
            0, 0,   45, 0, -183,   -9, 0,  93,   18,   0,
            0, 0,  216, 0,   33,   87, 0, -93,   12, -93,
            0, 0,  156, 0,  153,   99, 0, -93,  -12, -93,
            0, 0,  -45, 0,   -3,    9, 0,  93,  -18,   0,
            0, 0,  -55, 0,   48,   42, 0,   0,  -84,  31,
            0, 0,   -7, 0,  -48,  -42, 0,   0,   84,  31,
        };
        private static readonly ReadOnlyMemory<int> c3s_ = new[]{
             18,  -36, -122,   0,  120,  135, 0,   0,  -84, -31,
            -18,   36,   -2,   0, -120,   51, 0,   0,   84, -31,
             36, -165,  -27,  93,  147,   -9, 0, -93,   18,   0,
            210,   45, -111, -93,  -57, -192, 0,  93,   12,  93,
            162,  141,  -75, -93, -129, -180, 0,  93,  -12,  93,
            -36,  -21,   27,  93,   39,    9, 0, -93,  -18,   0,
              0,    0,   62,   0,    0,   31, 0,   0,    0, -31,
              0,    0,  124,   0,    0,   62, 0,   0,    0, -62,
              0,    0,  124,   0,    0,   62, 0,   0,    0, -62,
              0,    0,   62,   0,    0,   31, 0,   0,    0, -31,
            -18,   36,  -64,   0,   66,   51, 0,   0, -102,  31,
             18,  -36,    2,   0,  -66,  -51, 0,   0,  102,  31,
          };

        public readonly string Name;
        public readonly bool Cubic;
        public readonly double Eps;

        private readonly Stream _file;

        private readonly double _rlonres, _rlatres;
        private readonly string _description;
        private readonly DateTime? _datetime;
        private readonly double _offset, _scale, _maxerror, _rmserror;
        private readonly int _width, _height;
        private readonly long _datastart, _swidth;

        private readonly List<Memory<ushort>> _data = new List<Memory<ushort>>();

        private bool _cache;
        private int _xoffset = 0, _yoffset = 0, _xsize = 0, _ysize = 0;
        private int _ix, _iy;
        private double _v00, _v01, _v10, _v11;
        private readonly Memory<double> _t = new double[nterms_];

        public const int GEOGRAPHICLIB_GEOID_PGM_PIXEL_WIDTH = 2;

        internal const float FLT_MIN = 1.17549435082228750797e-38F;


        internal const double DBL_MIN = 2.2250738585072014e-308;

        internal const double DBL_EPSILON = 2.2204460492503131e-16;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngNormalize(double x)
        {
            x = IEEERemainder(x, 360d);
            return x != -180 ? x : 180;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double LatFix(double x) => Abs(x) > 90 ? double.NaN : x;


        public Geoid(string name, byte[] bytes, bool cubic = true)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            Name = name;
            Cubic = cubic;
            Eps = Sqrt(DBL_EPSILON);

            Debug.Assert(pixel_size_ == GEOGRAPHICLIB_GEOID_PGM_PIXEL_WIDTH, "pixel_t has the wrong size");

            MemoryStream ms = new MemoryStream(bytes);

            System.IO.Compression.GZipStream zrdr = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            byte[] buf = new byte[18671444];
            zrdr.Read(buf, 0, buf.Length);
            zrdr.Dispose(); ms.Dispose();
            ms = new MemoryStream(buf);
            _file = ms;

            using (StreamReader sr = new StreamReader(_file, Encoding.UTF8, true, bufferSize: 1, leaveOpen: true))
            {
                string s = sr.ReadLine();

                if (s != "P5")
                {
                    throw new Exception("Stream not in PGM format");
                }

                _offset = double.MaxValue;
                _scale = 0;
                _maxerror = _rmserror = -1;
                _description = "NONE";
                _datetime = null;

                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("#"))
                    {
                        Match match = Regex.Match(s, @"^#\s+([A-Za-z]+)\s+(.+)$");
                        if (!match.Success)
                        {
                            continue;
                        }

                        string key = match.Groups[1].Value;
                        if (key == "Description")
                        {
                            _description = match.Groups[2].Value.Trim();
                        }
                        else if (key == "DateTime")
                        {
                            _datetime = System.DateTime.Parse(match.Groups[2].Value.Trim());
                        }
                        else if (key == "Offset")
                        {
                            if (!double.TryParse(match.Groups[2].Value.Trim(), out _offset))
                            {
                                throw new Exception("Error reading offset");
                            }
                        }
                        else if (key == "Scale")
                        {
                            if (!double.TryParse(match.Groups[2].Value.Trim(), out _scale))
                            {
                                throw new Exception("Error reading scale");
                            }
                        }
                        else if (key == (Cubic ? "MaxCubicError" : "MaxBilinearError"))
                        {
                            // It's not an error if the error can't be read
                            double.TryParse(match.Groups[2].Value.Trim(), out _maxerror);
                        }
                        else if (key == (Cubic ? "RMSCubicError" : "RMSBilinearError"))
                        {
                            // It's not an error if the error can't be read
                            double.TryParse(match.Groups[2].Value.Trim(), out _rmserror);
                        }
                    }
                    else
                    {
                        string[] items = s.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (items.Length != 2 || !int.TryParse(items[0], out _width) || !int.TryParse(items[1], out _height))
                        {
                            throw new Exception("Error reading raster size");
                        }
                        break;
                    }
                }


                if (!uint.TryParse(s = sr.ReadLine(), out uint maxval))
                    throw new Exception("Error reading maxval");
                if (maxval != pixel_max_)
                    throw new Exception("Incorrect value of maxval");

                // HACK: Get start position of binary data.
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                sr.DiscardBufferedData();
                char[] buff = new char[1024];
                sr.ReadBlock(buff, 0, buff.Length);
                string ss = new string(buff);
                //Span<char> sp = buff.AsSpan();
                int end = ss.IndexOf(s + '\n') + s.Length + 1;// sp.IndexOf((s + '\n').AsSpan()) + s.Length + 1;

                // Add 1 for whitespace after maxval
                _datastart = Encoding.UTF8.GetByteCount(ss.Substring(0, end).ToArray());// sp.Slice(0, end).ToArray()); // +1 ?
                _swidth = _width;
            }

            if (_offset == double.MaxValue)
                throw new Exception("Offset not set");
            if (_scale == 0)
                throw new Exception("Scale not set");
            if (_scale < 0)
                throw new Exception("Scale must be positive");
            if (_height < 2 || _width < 2)
                // Coarsest grid spacing is 180deg.
                throw new Exception("Raster size too small");
            if ((_width & 1) != 0)
                // This is so that longitude grids can be extended thru the poles.
                throw new Exception("Raster width is odd");
            if ((_height & 1) == 0)
                // This is so that latitude grid includes the equator.
                throw new Exception("Raster height is even");

            _file.Seek(0, SeekOrigin.End);
            if (_datastart + pixel_size_ * _swidth * _height != _file.Position)
                // Possibly this test should be "<" because the file contains, e.g., a
                // second image.  However, for now we are more strict.
                throw new Exception("File has the wrong length");
            _rlonres = _width / 360.0;
            _rlatres = (_height - 1) / 180.0;
            _cache = false;
            _ix = _width;
            _iy = _height;


        }







        public void CacheClear()
        {

            _cache = false;
            try
            {
                _data.Clear();
            }
            catch
            {
            }

        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ConvertHeightToGeoid(double lat, double lon, double h) => h - Height(lat, lon);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ConvertHeightToEllipsoid(double lat, double lon, double h) => h + Height(lat, lon);




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Filepos(int ix, int iy)
        {
            _file.Seek(
                 (long)(_datastart +
                   pixel_size_ * ((uint)iy * _swidth + (uint)ix)), SeekOrigin.Begin);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Rawval(int ix, int iy)
        {
            if (ix < 0)
                ix += _width;
            else if (ix >= _width)
                ix -= _width;

            if (_cache && iy >= _yoffset && iy < _yoffset + _ysize &&
                ((ix >= _xoffset && ix < _xoffset + _xsize) ||
                 (ix + _width >= _xoffset && ix + _width < _xoffset + _xsize)))
            {
                return (double)_data[iy - _yoffset].Span
                            [ix >= _xoffset ? ix - _xoffset : ix + _width - _xoffset];
            }
            else
            {
                if (iy < 0 || iy >= _height)
                {
                    iy = iy < 0 ? -iy : 2 * (_height - 1) - iy;
                    ix += (ix < _width / 2 ? 1 : -1) * _width / 2;
                }
                try
                {
                    Filepos(ix, iy);
                    // initial values to suppress warnings in case get fails
                    byte a = (byte)_file.ReadByte(), b = (byte)_file.ReadByte();

                    int r = ((a) << 8) | (b);
                    if (pixel_size_ == 4)
                    {
                        a = (byte)_file.ReadByte(); b = (byte)_file.ReadByte();
                        r = (r << 16) | ((a) << 8) | (b);
                    }
                    return r;
                }
                catch (Exception e)
                {
                    string err = "Error reading ";
                    err += ": ";
                    err += e.Message;
                    throw new Exception(err);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double Height(double lat, double lon)
        {
            lat = LatFix(lat);
            if (double.IsNaN(lat) || double.IsNaN(lon))
            {
                return double.NaN;
            }
            lon = AngNormalize(lon);
            double
              fx = lon * _rlonres,
              fy = -lat * _rlatres;
            int
              ix = (int)Floor(fx),
              iy = Min((_height - 1) / 2 - 1, (int)Floor(fy));
            fx -= ix;
            fy -= iy;
            iy += (_height - 1) / 2;
            ix += ix < 0 ? _width : (ix >= _width ? -_width : 0);
            double v00 = 0, v01 = 0, v10 = 0, v11 = 0;
            double[] t = new double[nterms_];

            if (!(ix == _ix && iy == _iy))
            {
                if (!Cubic)
                {
                    v00 = Rawval(ix, iy);
                    v01 = Rawval(ix + 1, iy);
                    v10 = Rawval(ix, iy + 1);
                    v11 = Rawval(ix + 1, iy + 1);
                }
                else
                {
                    double[] v = new double[stencilsize_];

                    int k = 0;
                    v[k++] = Rawval(ix, iy - 1);
                    v[k++] = Rawval(ix + 1, iy - 1);
                    v[k++] = Rawval(ix - 1, iy);
                    v[k++] = Rawval(ix, iy);
                    v[k++] = Rawval(ix + 1, iy);
                    v[k++] = Rawval(ix + 2, iy);
                    v[k++] = Rawval(ix - 1, iy + 1);
                    v[k++] = Rawval(ix, iy + 1);
                    v[k++] = Rawval(ix + 1, iy + 1);
                    v[k++] = Rawval(ix + 2, iy + 1);
                    v[k++] = Rawval(ix, iy + 2);
                    v[k++] = Rawval(ix + 1, iy + 2);

                    ReadOnlySpan<int> c3x = iy == 0 ? c3n_.Span : (iy == _height - 2 ? c3s_.Span : c3_.Span);
                    int c0x = iy == 0 ? c0n_ : (iy == _height - 2 ? c0s_ : c0_);
                    for (int i = 0; i < nterms_; ++i)
                    {
                        t[i] = 0;
                        for (int j = 0; j < stencilsize_; ++j)
                            t[i] += v[j] * c3x[nterms_ * j + i];
                        t[i] /= c0x;
                    }
                }
            }
            else
            {
                if (!Cubic)
                {
                    v00 = _v00;
                    v01 = _v01;
                    v10 = _v10;
                    v11 = _v11;
                }
                else
                    _t.Span.CopyTo(t);
            }
            if (!Cubic)
            {
                double
                  a = (1 - fx) * v00 + fx * v01,
                  b = (1 - fx) * v10 + fx * v11,
                  c = (1 - fy) * a + fy * b,
                  h = _offset + _scale * c;

                _ix = ix;
                _iy = iy;
                _v00 = v00;
                _v01 = v01;
                _v10 = v10;
                _v11 = v11;

                return h;
            }
            else
            {
                double h = t[0] + fx * (t[1] + fx * (t[3] + fx * t[6])) +
                  fy * (t[2] + fx * (t[4] + fx * t[7]) +
                       fy * (t[5] + fx * t[8] + fy * t[9]));
                h = _offset + _scale * h;

                _ix = ix;
                _iy = iy;
                t.CopyTo(_t.Span);

                return h;
            }
        }

        public void Dispose()
        {
            _file.Dispose();
            _data.Clear();
        }
    }
}
