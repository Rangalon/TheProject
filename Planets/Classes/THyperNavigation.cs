using CiliaElements;
using CiliaElements.Format3DXml;
using CiliaElements.Utilities;
using Math3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using static Planets.Classes.TPlanetGround;

namespace Planets.Classes
{
    public abstract class THyperNavigation
    {

        public static TGeneralViewControl GeneralViewControl;
        public class TPlayer : TAssemblyElement, IHyperObject, IGrounded
        {
            public TPlanet Planet { get; set; }

            public Vec4 Speed { get; set; } = new Vec4(0.005, 0, 0, 0);
            public Vec4 Position { get; set; } = new Vec4(0, 1, 0, 0);
            public Mtx4 Orientation { get; set; } = Mtx4.Identity;


            public TLink Link { get; set; }


            public TPlayer()
            {
                PartNumber = "Player";
                Link = TManager.AttachElmt(OwnerLink, THyperNavigation.Universe, null);
                Link.NodeName = PartNumber;
            }

            public TGroundFacet Facet { get; set; }
        }

        public static Vec4 TripleCross(Vec4 v1, Vec4 v2, Vec4 v3)
        {
            Vec4 v = new Vec4();
            //
            v.Z = v1.W * (v2.X * v3.Y - v2.Y * v3.X) - v1.X * (v2.Y * v3.W - v2.W * v3.Y) + v1.Y * (v2.W * v3.X - v2.X * v3.W);
            v.W = v1.X * (v2.Y * v3.Z - v2.Z * v3.Y) - v1.Y * (v2.Z * v3.X - v2.X * v3.Z) + v1.Z * (v2.X * v3.Y - v2.Y * v3.X);
            v.X = v1.Y * (v2.Z * v3.W - v2.W * v3.Z) - v1.Z * (v2.W * v3.Y - v2.Y * v3.W) + v1.W * (v2.Y * v3.Z - v2.Z * v3.Y);
            v.Y = v1.Z * (v2.W * v3.X - v2.X * v3.W) - v1.W * (v2.X * v3.Z - v2.Z * v3.X) + v1.X * (v2.Z * v3.W - v2.W * v3.Z);
            //
            return v;
        }

        public static TPlayer Player;

        public static readonly TRandom TopologyRnd = new TRandom();

        public static readonly TPlanet[] Planets = new TPlanet[256];

        public static TLink Universe;



        public static readonly double Radius = 5000;

        public static void Activate()
        {
            TPlanet.GeneratePlanets();
            int i = 0;
            double k = (int)(Math.Pow(Planets.Length, 0.25)) - 1;
            double dt = 2 / k;
            for (double w = -1; w < 1.01; w += dt)
                for (double x = -1; x < 1.01; x += dt)
                    for (double y = -1; y < 1.01; y += dt)
                        for (double z = -1; z < 1.01; z += dt, i++)
                            Planets[i].Position = new Vec4(x + 0.0002, y + 0.0003, z + 0.0005, w + 0.0007);

            Planets[0].Position = new Vec4(0, 85, 65, Radius);

            foreach (TPlanet p in Planets)
            {
                Vec4 v = p.Position;
                v.Normalize();
                v *= Radius;
                p.Position = v;
            }


            Thread th = new Thread(DoerHyperNavigation); th.Start();
        }
        public static readonly double IRadius;



        public static TLink Corvette;
        public static TLink CarrierMk1;

        public static Boolean Enabled = false;


        static void DoerHyperNavigation()
        {
            GeneralViewControl = new TGeneralViewControl();
            GeneralViewControl.Visible = true;

            TLink l = TManager.LoadResource("Planets", Properties.Resources.Planets, TManager.View, TManager.ELoadStuff.FacesOnly);
            TThreadGoverner tg = new TThreadGoverner(100);
            Vec4 v;
            TLink shadow = null;
            TLink rock = null;
            TLink picker = null;
            TLink dockyard = null;
            Mtx4 m;
            TGroundFacet f;
            Vec4 dt, vx, vy, vz;
            double dmin = double.MaxValue, d;
            //Thread.Sleep(30000);
            while (shadow == null || Corvette == null || picker == null || CarrierMk1 == null)
            {
                shadow = l.Links.Values.LastOrDefault(o => o != null && o.PartNumber == "Carrier-Mk1");
                rock = l.Links.Values.LastOrDefault(o => o != null && o.PartNumber == "Rock");
                dockyard = l.Links.Values.LastOrDefault(o => o != null && o.PartNumber == "ConstructionYard");
                Corvette = l.Links.Values.LastOrDefault(o => o != null && o.PartNumber == "Corvette");
                picker = l.Links.Values.FirstOrDefault(o => o != null && o.PartNumber == "Picker");
                CarrierMk1 = l.Links.Values.FirstOrDefault(o => o != null && o.PartNumber == "Carrier-Mk1");
            }
            //l.Enabled = false;

            Player = new TPlayer();

            TVehicle vehicle = new TVehicle();
            TManager.AttachElmt(CarrierMk1.Solid.OwnerLink, vehicle.Link, null).ToBeReplaced = false;



            //foreach (TLink ll in l.Links.Values.Where(o => o != null && o.PartNumber != "Corvette")) ll.Enabled = false;
            //Corvette.Enabled = true;

            //rock.Enabled = false;
            //dockyard.Enabled = false;


            Corvette.Matrix = Mtx4.Identity;

            List<IHyperObject> HyperObjects = new List<IHyperObject>();
            HyperObjects.AddRange(Planets);
            HyperObjects.Add(Player);
            HyperObjects.Add(vehicle);


            picker.Ethereal = true;
            shadow.DisplayColor = Mtx4f.CreateStaticColor(0.1f, 0.1f, 0.1f);
            Player.Position = new Vec4(Radius, 0, 0, 0);
            //
            Enabled = true;
            //
            while (Planets.FirstOrDefault(o => o.PlanetGround.GroundFacets == null) != null) Thread.Sleep(100);
            //
            while (Enabled)
            {
                tg.Reset();
                //
                GeneralViewControl.Height = (int)(TManager.Height - 50);
                GeneralViewControl.Width = (int)Math.Max(250, TManager.Width * 0.1);
                GeneralViewControl.OwnerLink.Giving.Matrix = Mtx4.CreateScale(GeneralViewControl.Width, 1, GeneralViewControl.Height) * TManager.CreateForeMatrix(0.99, -0.99 + 2 * GeneralViewControl.Height * TManager.IHeight, 0.001, 0.002);

                //
                //m = Player.Orientation;
                //m.Row3 += IRadius * Vec4.Transform(Player.Speed, Player.Orientation);
                //m.Row3.Normalize();
                ////
                //vx = TripleCross(m.Row1, m.Row2, m.Row3); vx.Normalize(); if (Vec4.Dot(vx, m.Row0) < 0) m.Row0 = -vx; else m.Row0 = vx;
                //vy = TripleCross(m.Row2, m.Row3, m.Row0); vy.Normalize(); if (Vec4.Dot(vy, m.Row1) < 0) m.Row1 = -vy; else m.Row1 = vy;
                //vz = TripleCross(m.Row3, m.Row0, m.Row1); vz.Normalize(); if (Vec4.Dot(vz, m.Row2) < 0) m.Row2 = -vz; else m.Row2 = vz;
                ////Vec4 vw = TripleCross(m.Row0, m.Row1, m.Row2);

                //Player.Orientation = m;
                //Player.Position = Player.Orientation.Row3 * Radius;
                //
                //for (int i = 0; i < TManager.UsedKeys.Length; i++)
                //    if (TManager.UsedKeys[i]) Console.WriteLine(i);
                //
                m = Mtx4.Identity;
                if (TManager.UsedKeys[47])
                    m = Mtx4.CreateRotationZ(-0.01) * m;
                if (TManager.UsedKeys[48])
                    m = Mtx4.CreateRotationZ(0.01) * m;
                if (TManager.UsedKeys[46])
                    m = Mtx4.CreateRotationY(0.01) * m;
                if (TManager.UsedKeys[45])
                    m = Mtx4.CreateRotationY(-0.01) * m;
                if (TManager.UsedKeys[55])
                    m = Mtx4.CreateRotationX(0.01) * m;
                if (TManager.UsedKeys[57])
                    m = Mtx4.CreateRotationX(-0.01) * m;
                Universe.Matrix *= m;

                //m.Invert();
                m = m * Universe.Matrix * Player.Orientation;
                m.Invert();
                if (TManager.UsedKeys[59])
                    Player.Speed += 0.001 * m.Row0;
                if (TManager.UsedKeys[58])
                    Player.Speed *= 0.99;
                //double n01 = Vec4.Dot(m.Row0, m.Row1);
                //double n02 = Vec4.Dot(m.Row0, m.Row2);
                //double n03 = Vec4.Dot(m.Row0, m.Row3);
                //double n12 = Vec4.Dot(m.Row1, m.Row2);
                //double n13 = Vec4.Dot(m.Row1, m.Row3);
                //double n23 = Vec4.Dot(m.Row2, m.Row3);

                //Console.WriteLine(Player.Position);

                // 
                foreach (IHyperObject p in HyperObjects)
                {
                    dt = p.Position - Player.Position;
                    if (dt.FullLengthSquared < 1e7)
                    {
                        p.Link.Enabled = true;
                        dt = Vec4.Transform(dt, Player.Orientation);
                        m = p.Link.Matrix;
                        m.Row3 = new Vec4(dt.X, dt.Y, dt.Z, 1);
                        p.Link.Matrix = m;
                        //
                        if (p.Speed.X != 0 || p.Speed.Y != 0 || p.Speed.Z != 0 || p.Speed.W != 0)
                        {
                            m = p.Orientation;
                            m.Row3 += IRadius * Vec4.Transform(p.Speed, p.Orientation);
                            m.Row3.Normalize();
                            //
                            vx = TripleCross(m.Row1, m.Row2, m.Row3); vx.Normalize(); if (Vec4.Dot(vx, m.Row0) < 0) m.Row0 = -vx; else m.Row0 = vx;
                            vy = TripleCross(m.Row2, m.Row3, m.Row0); vy.Normalize(); if (Vec4.Dot(vy, m.Row1) < 0) m.Row1 = -vy; else m.Row1 = vy;
                            vz = TripleCross(m.Row3, m.Row0, m.Row1); vz.Normalize(); if (Vec4.Dot(vz, m.Row2) < 0) m.Row2 = -vz; else m.Row2 = vz;
                            //Vec4 vw = TripleCross(m.Row0, m.Row1, m.Row2);

                            p.Orientation = m;
                            p.Position = Player.Orientation.Row3 * Radius;
                        }
                        //
                    }
                    else
                        p.Link.Enabled = false;
                    //
                    if (p is IGrounded grounded)
                    {
                        ComputePlanet(grounded);
                    }
                }
                //
                //ComputePlanet(Player);
                //

                {
                    Vec4 n = Player.Planet.Position - Player.Position;
                    n.Normalize();
                    Player.Speed += 0.00001 * n;
                }


                m = Player.Planet.Link.Matrix * Universe.Matrix;
                m.Invert();

                f = null;
                dmin = double.MaxValue;
                foreach (TGroundFacet ff in Player.Planet.PlanetGround.GroundFacets)
                {
                    if (ff != null)
                    {
                        d = (ff.Matrix.Row3 - m.Row3).LengthSquared;
                        if (d < dmin)
                        {
                            dmin = d;
                            f = ff;
                        }
                    }
                }

                m.Invert();

                if (f != null)
                {

                    m = f.Matrix * m;
                    v = m.Row2;
                    m.Invert();

                    m = Mtx4.CreateTranslation((0.1 - m.Row3.Z) * v);
                    m.Row0 -= v * (Vec4.Dot(v, m.Row0));
                    m.Row1 -= v * (Vec4.Dot(v, m.Row1));
                    m.Row2 -= v * (Vec4.Dot(v, m.Row2));
                    shadow.Matrix = m;

                    m = Universe.Matrix;
                    //m.Invert();

                    Vec4 n = f.Matrix.Row2;
                    n = Vec4.Transform(n, m);

                    n = Vec4.Cross(n, new Vec4(0, 0, 1, 0));
                    double ds = Math.Asin(n.FullLength);
                    n.Normalize();

                    m = Mtx4.CreateRotation(n, -0.01 * ds);
                    Universe.Matrix *= m;

                }


                //
                if (TManager.OverFlownLink == null && TManager.OverFlownPoint.HasValue)
                {
                    picker.Matrix = Mtx4.CreateTranslation(TManager.OverFlownPoint.Value);
                }
                //
                tg.Check();
            }
        }

        static THyperNavigation()
        {
            IRadius = 1 / Radius;
        }

        public static void ComputePlanet(IGrounded obj)
        {
            double dmin = double.MaxValue, d;
            Vec4 dt;
            foreach (TPlanet p in Planets)
            {
                dt = p.Position - obj.Position;
                d = dt.FullLengthSquared;
                if (d < dmin)
                {
                    dmin = d;
                    obj.Planet = p;
                }
            }
        }
    }

    public interface IGrounded : IHyperObject
    {
        TPlanet Planet { get; set; }
        TGroundFacet Facet { get; set; }
    }

    public class TVehicle : TAssemblyElement, IHyperObject, IGrounded
    {
        public TPlanet Planet { get; set; }

        public Vec4 Speed { get; set; } = new Vec4(0.005, 0, 0, 0);
        public Vec4 Position { get; set; } = new Vec4(0, 1, 0, 0);
        public Mtx4 Orientation { get; set; } = Mtx4.Identity;


        public TLink Link { get; set; }


        public TVehicle()
        {
            PartNumber = "V" + GetHashCode();
            Link = TManager.AttachElmt(OwnerLink, THyperNavigation.Universe, null);
            Link.NodeName = PartNumber;
            Link.ToBeReplaced = false;
        }

        public TGroundFacet Facet { get; set; }
    }

    public interface IHyperObject
    {
        Vec4 Speed { get; set; }
        Vec4 Position { get; set; }
        Mtx4 Orientation { get; set; }

        TLink Link { get; set; }

    }
}
