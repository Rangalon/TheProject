namespace CiliaElements
{
    public static partial class TManager
    {
        #region Private Methods

        static TLink OpenFileButton;

        private static void DecreaseSpatialSpeed()
        {
            //SpatialSpeed /= 2;
        }

        private static void DoerPhysics()
        {
            return;

            //         private static double AsteroidTextureHeight = 200;
            //private static double AsteroidTextureWidth = 400;
            //private static TLink HandlingAsteroid;

            //private static Vec3 SpatialAcceleration;

            //private static Vec3 SpatialSpeed;

            //private static double TextureOffsetRatio = 2.5;
            //TPolarFacetTemporaryDistanceComparer PolarFacetTemporaryDistanceComparer = new TPolarFacetTemporaryDistanceComparer();
            //DateTime LastComputeDate = DateTime.Now;
            //double OneThird = 1.0 / 3.0;
            //double One255 = 1.0 / 255.0;
            //Vec3 v0 = new Vec3(0, 0, 0);
            //while (true)
            //{
            //    if (HandlingAsteroid != null && TAsteroid.Facets != null)
            //    {
            //        DateTime ComputeDate = DateTime.Now;
            //        TimeSpan ts = ComputeDate - LastComputeDate;
            //        LastComputeDate = ComputeDate;
            //        //----------------------------------------------
            //        Vec3 u = 0.001 * (double)(ts.TotalMilliseconds) * SpatialSpeed;
            //        Mtx4 MM = Univers.Matrix; MM.Row3 -= u; //Univers.Matrix = MM;
            //        //----------------------------------------------
            //        Mtx4 M = MM * HandlingAsteroid.Matrix;
            //        Mtx4 MI = M; MI.Invert();
            //        Vec3 v0a = Vec4.TransformPoint(v0, MI);
            //        if (v0a.LengthSquared == 0) { v0a.X = 10; v0a.Y = 10; v0a.Z = 10; }
            //        Mtx4 mm;
            //        double l;
            //        Vec3 vv;
            //        Vec3 c1;
            //        Vec3 c2;
            //        Vec3 c3;
            //        Mtx3 m;
            //        //----------------------------------------------
            //        foreach (TPolarFacet pf in TAsteroid.Facets)
            //        {
            //            //pf.tmpP3 = Vec4.TransformPoint(pf.tmpP3, MM);

            //            //m = new Mtx3(pf.P2 - pf.P1, pf.P3 - pf.P1, Vec3.Cross(pf.P2 - pf.P1, pf.P3 - pf.P1));
            //            //m.Row2.Normalize();
            //            //m.Invert();
            //            //vv = Vec3.Transform(v0a - pf.P1, m);

            //            //pf.tmpr = vv.X + vv.Y;
            //            pf.tmpd = -(Vec3.Dot(v0a, pf.P1) + Vec3.Dot(v0a, pf.P2) + Vec3.Dot(v0a, pf.P3));

            //            //pf.tmpd = (pf.P1 - v0a).Length + (pf.P2 - v0a).Length + (pf.P3 - v0a).Length;
            //        }
            //        Array.Sort(TAsteroid.Facets, PolarFacetTemporaryDistanceComparer);
            //        TPolarFacet MainFacet = TAsteroid.Facets[0];
            //        c1 = getColorFromTextureCoordinates(MainFacet.t1);
            //        MainFacet.tmpP1 = MainFacet.P1 + MainFacet.N1 * MainFacet.P1.Length * TextureOffsetRatio * c1.Z;
            //        //pf.tmpP1 = Vec4.TransformPoint(pf.tmpP1, MM);
            //        c2 = getColorFromTextureCoordinates(MainFacet.t2);
            //        MainFacet.tmpP2 = MainFacet.P2 + MainFacet.N2 * MainFacet.P2.Length * TextureOffsetRatio * c2.Z;
            //        //pf.tmpP2 = Vec4.TransformPoint(pf.tmpP2, MM);
            //        c3 = getColorFromTextureCoordinates(MainFacet.t3);
            //        MainFacet.tmpP3 = MainFacet.P3 + MainFacet.N3 * MainFacet.P3.Length * TextureOffsetRatio * c3.Z;
            //        m = new Mtx3(MainFacet.tmpP1 - MainFacet.tmpP2, MainFacet.tmpP1 - MainFacet.tmpP3, v0a);
            //        Vec3 v;

            //        Vec4 nw;
            //        Vec4 nu;
            //        Vec4 nv;
            //        try
            //        {
            //            //foreach (TPolarFacet pf in TAsteroid.Facets)
            //            //{
            //            //    mm = pf.LP1.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(pf.P1, MM), 1); pf.LP1.Matrix = mm;
            //            //    mm = pf.LP2.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(pf.P2, MM), 1); pf.LP2.Matrix = mm;
            //            //    mm = pf.LP3.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(pf.P3, MM), 1); pf.LP3.Matrix = mm;
            //            //    //
            //            //    c1 = getColorFromTextureCoordinates(pf.t1);
            //            //    c2 = getColorFromTextureCoordinates(pf.t2);
            //            //    c3 = getColorFromTextureCoordinates(pf.t3);
            //            //    //
            //            //    mm = pf.LP1.Matrix; mm.Row3 += Vec4.TransformVector(pf.N1, MM) * pf.P1.Length * 2.5 * c1.Z; pf.LP1.Matrix = mm;
            //            //    mm = pf.LP2.Matrix; mm.Row3 += Vec4.TransformVector(pf.N2, MM) * pf.P2.Length * 2.5 * c2.Z; pf.LP2.Matrix = mm;
            //            //    mm = pf.LP3.Matrix; mm.Row3 += Vec4.TransformVector(pf.N3, MM) * pf.P3.Length * 2.5 * c3.Z; pf.LP3.Matrix = mm;
            //            //}

            //            //mm = CrossLink1.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P1, MM), 1); CrossLink1.Matrix = mm;
            //            //mm = CrossLink2.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P2, MM), 1); CrossLink2.Matrix = mm;
            //            //mm = CrossLink3.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P3, MM), 1); CrossLink3.Matrix = mm;

            //            //mm = CrossLink1.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P1, MM), 1); CrossLink1.Matrix = mm;
            //            //mm = CrossLink2.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P2, MM), 1); CrossLink2.Matrix = mm;
            //            //mm = CrossLink3.Matrix; mm.Row3 = new Vec4(Vec4.TransformPoint(MainFacet.P3, MM), 1); CrossLink3.Matrix = mm;
            //            ////
            //            //c1 = getColorFromTextureCoordinates(MainFacet.t1);
            //            //c2 = getColorFromTextureCoordinates(MainFacet.t2);
            //            //c3 = getColorFromTextureCoordinates(MainFacet.t3);
            //            ////
            //            //mm = CrossLink1.Matrix; mm.Row3 += Vec4.TransformVector(MainFacet.N1, MM) * MainFacet.P1.Length * TextureOffsetRatio * c1.Z; CrossLink1.Matrix = mm;
            //            //mm = CrossLink2.Matrix; mm.Row3 += Vec4.TransformVector(MainFacet.N2, MM) * MainFacet.P2.Length * TextureOffsetRatio * c2.Z; CrossLink2.Matrix = mm;
            //            //mm = CrossLink3.Matrix; mm.Row3 += Vec4.TransformVector(MainFacet.N3, MM) * MainFacet.P3.Length * TextureOffsetRatio * c3.Z; CrossLink3.Matrix = mm;
            //            ////
            //            //nw = new Vec4() + MainFacet.P1; nw.Normalize(); nu = new Vec4(nw.Y, nw.Z, -nw.X, 0); nv = Vec4.Cross(nw, nu); nu = Vec4.Cross(nv, nw);
            //            //mm = CrossLink1.Matrix; mm.Row0 = nu; mm.Row1 = nv; mm.Row2 = nw; CrossLink1.Matrix = mm;
            //            //nw = new Vec4() + MainFacet.P2; nw.Normalize(); nu = new Vec4(nw.Y, nw.Z, -nw.X, 0); nv = Vec4.Cross(nw, nu); nu = Vec4.Cross(nv, nw);
            //            //mm = CrossLink2.Matrix; mm.Row0 = nu; mm.Row1 = nv; mm.Row2 = nw; CrossLink2.Matrix = mm;
            //            //nw = new Vec4() + MainFacet.P3; nw.Normalize(); nu = new Vec4(nw.Y, nw.Z, -nw.X, 0); nv = Vec4.Cross(nw, nu); nu = Vec4.Cross(nv, nw);
            //            //mm = CrossLink3.Matrix; mm.Row0 = nu; mm.Row1 = nv; mm.Row2 = nw; CrossLink3.Matrix = mm;

            //            //vv = MainFacet.P1; l = vv.Length; vv.Normalize(); l *= c1.LengthSquared * 0.05; vv *= l; mm = CrossLink1.Matrix; mm.Row3 += vv; CrossLink1.Matrix = mm;
            //            //vv = MainFacet.P2; l = vv.Length; vv.Normalize(); l *= c2.LengthSquared * 0.05; vv *= l; mm = CrossLink2.Matrix; mm.Row3 += vv; CrossLink2.Matrix = mm;
            //            //vv = MainFacet.P3; l = vv.Length; vv.Normalize(); l *= c3.LengthSquared * 0.05; vv *= l; mm = CrossLink3.Matrix; mm.Row3 += vv; CrossLink3.Matrix = mm;
            //            //
            //            Mtx3 mi = m; mi.Invert();
            //            v = Vec3.Transform(MainFacet.tmpP1, mi);
            //            v.X = Math.Round(v.X, 6);
            //            v.Y = Math.Round(v.Y, 6);
            //            if (v.X < 0 || v.Y < 0 || v.X > 1 || v.Y > 1 || v.X + v.Y > 1)
            //            {
            //                Console.WriteLine(string.Format("{0:0.000} {1:0.000} {2:0.000} ", v.X, v.Y, v.X + v.Y));
            //            }
            //            v0a *= v.Z - 1;
            //            v0a = Vec4.TransformVector(v0a, M);
            //            MM.Row3 -= v0a;
            //            //
            //            //
            //            //
            //            //double d = c1.Z + (c2.Z - c1.Z) * v.X + (c3.Z - c1.Z) * v.Y;
            //            //////
            //            ////MM.Row3 += v.Z * m.Row2;
            //            //Mtx4 MMI = MM; MMI.Invert(); Vec4 v4 = Vec4.Transform(new Vec4(0, 0, 0, 1), MMI);
            //            //l = v4.Length; v4.W = 0; v4.Normalize(); v4 = Vec4.Transform(v4, MM);
            //            //MM.Row3 -= d * l * TextureOffsetRatio * v4;
            //            //Console.WriteLine(string.Format("h {0:0.000} {1:0.000} {2:0.000} {3:0.000}", (d * l * 0.05 * v4).Length, c1.LengthSquared, c2.LengthSquared, c3.LengthSquared));
            //        }
            //        catch { }
            //        //
            //        Univers.Matrix = MM;

            //        //Console.WriteLine(string.Format("MM {0:0} {1:0} {2:0} {3:0}", MM.Row3.X, MM.Row3.Y, MM.Row3.Z, MM.Row3.Length));
            //        //Console.WriteLine(string.Format("v {0:0} {1:0} {2:0} {3:0}", v.X, v.Y, v.Z, v.Length));
            //        //Vec4 u = M.Row3;
            //        //double n = u.Length;
            //        //u.X /= n; u.Y /= n; u.Z /= n;
            //        //if (double.IsNaN(u.X) || double.IsInfinity(u.X))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //u *= 1002;
            //        //if (double.IsNaN(u.X) || double.IsInfinity(u.X))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //u -= M.Row3; u.W = 0;
            //        //if (double.IsNaN(u.X) || double.IsInfinity(u.X))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //if (double.IsNaN(u.X) || double.IsInfinity(u.X))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //M = Univers.Matrix;
            //        //if (double.IsInfinity(M.Row3.Y))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //if (u.LengthSquared > 100000000 || double.IsInfinity(u.X) || double.IsInfinity(u.Y) || double.IsInfinity(u.Z) || double.IsNaN(u.X) || double.IsNaN(u.Y) || double.IsNaN(u.Z))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //Console.WriteLine(string.Format("{0:0} {1:0} {2:0} {3:0}", Univers.TakenMatrix.Row3.X, Univers.TakenMatrix.Row3.Y, Univers.TakenMatrix.Row3.Z, Univers.TakenMatrix.Row3.Length));
            //        //Console.WriteLine(string.Format("{0:0} {1:0} {2:0} {3:0}", u.X, u.Y, u.Z, u.Length));
            //        //M.Row3 += u;
            //        ////
            //        //u = new Vec4(0.001 * (double)ts.TotalMilliseconds * SpatialSpeed, 0);
            //        //if (u.LengthSquared > 0 || double.IsInfinity(u.X) || double.IsInfinity(u.Y) || double.IsInfinity(u.Z) || double.IsNaN(u.X) || double.IsNaN(u.Y) || double.IsNaN(u.Z))
            //        //{
            //        //    u.X = 0;
            //        //}
            //        //Console.WriteLine(string.Format("{0:0} {1:0} {2:0} {3:0}", u.X, u.Y, u.Z, u.Length));
            //        //M.Row3 += u;

            //        ////
            //        //Univers.Matrix = M;

            //        //Console.WriteLine(string.Format("{0:0} {1:0} {2:0} {3:0}", HandlingAsteroid.TakenMatrix.Row3.X, HandlingAsteroid.TakenMatrix.Row3.Y, HandlingAsteroid.TakenMatrix.Row3.Z, HandlingAsteroid.TakenMatrix.Row3.Length));
            //        Thread.Sleep(10);
            //    }
            //}
        }

        //private static Vec3 GetColorFromTextureCoordinates(Vec2 t)
        //{
        //    //int X = (int)t.X;
        //    //int Y = (int)t.Y;
        //    //double dX = Math.Round(t.X - X, 9);
        //    //double dY = Math.Round(t.Y - Y, 9);
        //    //if (dX > 0)
        //    //{
        //    //    X += 0;
        //    //}
        //    //if (dY > 0)
        //    //{
        //    //    Y += 0;
        //    //}

        //    Color c0 = TAsteroid.TextureBitmap.GetPixel((int)t.X, (int)t.Y);
        //    Vec3 v0 = new Vec3(c0.R, c0.G, c0.B) / 255;

        //    return v0;
        //}

        private static void IncreaseSpatialSpeed()
        {
            //if (SpatialSpeed.LengthSquared == 0) SpatialSpeed = new Vec3(1, 0, 0);
            //SpatialSpeed *= 2;
            //SpatialSpeed -= VIMatrix.Row2;
        }

        #endregion Private Methods
    }
}