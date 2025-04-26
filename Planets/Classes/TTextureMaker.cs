using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planets.Classes
{
    public abstract class TTextureMaker
    {
        public static void GenerateTextures()
        {
            GenerateBody();
            GenerateGears();
        }

        static int Width = 1024;
        static int Height = 1024;

        static void GenerateBody()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics grp = Graphics.FromImage(bmp);
            grp.Clear(Color.Black);

            Random rnd = new Random();
            Pen ShadowPen = new Pen(Color.FromArgb(128, 0, 0, 0), 1);
            Pen LightPen = new Pen(Color.FromArgb(128, 255, 255, 255), 1);
            SolidBrush brs = new SolidBrush(Color.FromArgb(255, 64, 64, 64));

            grp.FillRectangle(brs, 0, 0, Width, Height);
            grp.DrawRectangle(ShadowPen, 0, 0, Width, Height);
            grp.DrawRectangle(ShadowPen, 0, 0, Width, Height);
            grp.DrawRectangle(LightPen, 0, 0, Width, Height);
            grp.FillRectangle(brs, 0, 0, Width, Height);
            for (int i = 0; i < 10000; i++)
            {
                Rectangle r = new Rectangle(
                    (int)(Width * rnd.NextDouble()), (int)(Height * rnd.NextDouble()),
                    (int)(128 * rnd.NextDouble()), (int)(128 * rnd.NextDouble()));
                r.X -= r.Width / 2;
                r.Y -= r.Height / 2;
                r.X = r.X >= 0 ? r.X : 0;
                r.Y = r.Y >= 0 ? r.Y : 0;
                r.Width = r.Right > Width ? r.Width + Width - r.Right : r.Width;
                r.Height = r.Bottom > Height ? r.Height + Height - r.Bottom : r.Height;
                grp.TranslateTransform(-1, -1); grp.DrawRectangle(ShadowPen, r);
                grp.TranslateTransform(+2, +2); grp.DrawRectangle(ShadowPen, r);
                grp.TranslateTransform(-1, -1); grp.DrawRectangle(LightPen, r);
                grp.TranslateTransform(+1, +1); grp.FillRectangle(brs, r);
                grp.TranslateTransform(-1, -1);
            }


            grp.Dispose();
            bmp.Save(@"C:\Users\to101757\Documents\91.C#\Planets\Resources\Body.png");
        }
        static void GenerateGears()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            Graphics grp = Graphics.FromImage(bmp);
            Pen P0 = new Pen(Color.FromArgb(64, 255, 0, 0), 10);
            Pen P1 = new Pen(Color.FromArgb(64, 255, 0, 0), 8);
            Pen P2 = new Pen(Color.FromArgb(64, 255, 0, 0), 6);
            Pen P3 = new Pen(Color.FromArgb(64, 255, 0, 0), 4);
            Pen P4 = new Pen(Color.FromArgb(64, 255, 0, 0), 2);

            grp.Clear(Color.Black);

            Pen[] Ps = new Pen[] { P0, P1, P2, P3, P4 };

            List<Point[]> pts = new List<Point[]>();
            for (int i = -256; i < 2048; i += 64)
            {
                Point[] ps = new Point[7];
                ps[0] = new Point(i, -64);
                ps[1] = new Point(i + 128, 128);
                ps[2] = new Point(i + 64, 320);
                ps[3] = new Point(i + 192, 512);
                ps[4] = new Point(i + 64, 704);
                ps[5] = new Point(i + 128, 896);
                ps[6] = new Point(i, 1088);
                pts.Add(ps);
            }

            for (int i = -256; i < 2048; i += 64)
            {
                Point[] ps = new Point[7];
                ps[0] = new Point(-64, i);
                ps[1] = new Point(128, i + 128);
                ps[2] = new Point(320, i + 64);
                ps[3] = new Point(512, i + 128);
                ps[4] = new Point(704, i + 64);
                ps[5] = new Point(896, i + 128);
                ps[6] = new Point(1088, i);
                pts.Add(ps);
            }

            foreach (Pen p in Ps)
                foreach (Point[] ps in pts)
                {
                    grp.DrawCurve(p, ps);
                }

            grp.Dispose();
            bmp.Save(@"C:\Users\to101757\Documents\91.C#\Planets\Resources\Gears.png");
        }
    }
}
