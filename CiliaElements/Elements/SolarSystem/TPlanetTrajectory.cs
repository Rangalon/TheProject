using CiliaElements.Managers;
using MySolarSystem;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CiliaElements.Elements.SolarSystem
{
    public class TPlanetTrajectory : TInternal
    {
        public TBody Body;
        public TPlanetTrajectory(TBody iBody)
            : base("Traj_" + iBody.Name)
        {
            Body = iBody;
            Trajectories.Push(this);
        }
        public override void LaunchLoad()
        {
            TCloud Positions = SolidElementConstruction.AddCloud();
            List<Vec3> lst = new List<Vec3>();
            foreach (TBodyPosition bp in Body.Positions)
                Positions.Vectors.Push(bp.V);
            //
            TTexture Texture = SolidElementConstruction.AddTexture();
            Texture.Color = new Vec4f(0, 1, 0, 1);
            //
            TLGroup Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            //Array.Resize(ref Lines.Indexes, (n + 1) * 6);
            for (Int32 i = 0; i < Body.Positions.Length - 1; i++)
            {
                Lines.Indexes.Push(i);
                Lines.Indexes.Push(i + 1);
                Lines.Indexes.Push(-1);
            }
            //
            //Dim Faces As New TGroup With {.PositionsId = Positions.Id, .TextureId = Texture.Id, .Kind = EGroupKind.F}
            //AddGroup(Faces)
            //StartGroup.AddGroupId(Faces.Id)
            //Faces.Indexes.Add(0)
            //Faces.Indexes.Add(1)
            //Faces.Indexes.Add(2)
            //Faces.Indexes.Add(4 * (2 * Infinity / GroundStep + 1) - 3)
            //Faces.Indexes.Add(0)
            //Faces.Indexes.Add(1)
            //
            ElementLoader.Publish();
        }

        public TLink PlanetLink;
        public static TQuickStack<TPlanetTrajectory> Trajectories = new TQuickStack<TPlanetTrajectory>();

        public static bool EarthNotPushed = true;

        static DateTime LastCompute;

        public static void Activate()
        {
            EarthNotPushed = false;
            TAssemblyElement ass = new TAssemblyElement();
            ass.PartNumber = "System";
            foreach (TBody b in TBodies.Bodies.FindAll(o => o.Radius != 0 && o.Name != null && o.Name != "" && o.Id < 1000))
            {
                TLink l = new TLink();
                if (TModelsManager.Internals.PlanetLinks.ContainsKey(b.Id))
                    l.Child = TModelsManager.Internals.PlanetLinks[b.Id].Solid;
                else
                    l.Child = TModelsManager.Internals.PlanetLinks[1].Solid;
                l.NodeName = b.Name;
                l.CustomAttributes = new List<TCustomAttribute>();
                l.CustomAttributes.Add(new TCustomAttribute() { Name = "Id", Value = b.Id.ToString() });
                l.ToBeReplaced = false;
                TModelsManager.AttachElmt(l, ass.OwnerLink, TModelsManager.ToBeParsedLinks);
                //
                TPlanetTrajectory ptj = new TPlanetTrajectory(b);
                ptj.Attach(ass);
                //
            }
            TPlanetTrajectory.SolarSystemLink = TModelsManager.AttachElmt(ass.OwnerLink, TModelsManager.View.OwnerLink, TModelsManager.ToBeParsedLinks);
            TModelsManager.BuildIndexes();
            //
            Trajectories.Close();
            foreach (TPlanetTrajectory ptj in Trajectories.Values)
            {
                ptj.PlanetLink = SolarSystemLink.Links.Values.First(o => o.CustomAttributes != null && o.CustomAttributes.First(oo => oo.Name == "Id").Value == ptj.Body.Id.ToString());
            }

            LastCompute = DateTime.Now;
            AttractionLink = TModelsManager.Internals.TripLink.Links.Values.First(o => o.PartNumber == "Attraction");
            Sun = TBodies.Bodies.FirstOrDefault(o => o.Name == "Sun");
            if (Sun != null) SolarLink = Trajectories.Values.FirstOrDefault(o => o.Body == Sun).PlanetLink;
            TModelsManager.Internals.TripLink.Visible = true;
        }

        static TBody Sun;

        static TQuickStack<Vec3> Forces = new TQuickStack<Vec3>();

        static TLink AttractionLink;

        public static void Update()
        {
            DateTime nw = DateTime.Now;
            double dt = (double)(nw - LastCompute).Ticks / (double)TimeSpan.TicksPerSecond;
            LastCompute = nw;
            //
            if (Sun != null && Sun.Radius > 0) Sun.Radius *= -1;
            NearestPlanetLink = Trajectories.Values.FirstOrDefault(o => o.Body.Positions != null);
            foreach (TPlanetTrajectory ptj in Trajectories.Values.Where(o => o.Body.Positions != null))
            {
                if ((ptj.PlanetLink.Matrix * SolarSystemLink.Matrix * TViewManager.VMatrix).Row3.LengthSquared < (NearestPlanetLink.PlanetLink.Matrix * SolarSystemLink.Matrix * TViewManager.VMatrix).Row3.LengthSquared) NearestPlanetLink = ptj;
                int i1 = 0; while (i1 < ptj.Body.Positions.Length - 1 && ptj.Body.Positions[i1].Dt < nw) i1++;
                int i0 = i1 - 1;
                Vec3 V = ptj.Body.Positions[i0].V + (ptj.Body.Positions[i1].V - ptj.Body.Positions[i0].V) * ((double)(nw - ptj.Body.Positions[i0].Dt).Ticks) / ((double)(ptj.Body.Positions[i1].Dt - ptj.Body.Positions[i0].Dt).Ticks);
                ptj.PlanetLink.Matrix = Mtx4.CreateRotationX(-ptj.Body.ObliquityToOrbit) * Mtx4.CreateScaledTranslation(ptj.Body.Radius, V);
            }





            //
            Forces.Reset();
            Vec3 ma = new Vec3();
            //foreach (TPlanetTrajectory ptj in Trajectories.Values.Where(o => o.Body.Mass > 0))
            //{
            //    Vec3 v = (Vec3)(ptj.PlanetLink.Matrix * SolarSystemLink.Matrix * TViewManager.VMatrix).Row3;
            //    if (v.LengthSquared > 0)
            //    {
            //        double l = G * ptj.Body.Mass * 1000 / v.LengthSquared;
            //        v.Normalize();
            //        Forces.Push(v * l);
            //    }
            //}
            foreach (Vec3 v in Forces.Values)
                ma += v;
            ma /= 1000;
            Attraction = ma;
            Mtx4 MA = Mtx4.Identity;
            MA.Row1.X = -ma.X; MA.Row1.Y = -ma.Y; MA.Row1.Z = -ma.Z;
            MA.Row1.Normalize();
            MA.Row2.X = MA.Row1.Y; MA.Row2.Y = MA.Row1.Z; MA.Row2.Z = -MA.Row1.X;
            MA.Row0 = Vec4.Cross(MA.Row1, MA.Row2);
            MA.Row0.Normalize();
            MA.Row2 = Vec4.Cross(MA.Row0, MA.Row1);
            AttractionLink.Matrix = MA * TViewManager.VIMatrix;

            if (Reverse)
                ma += (Vec3)(TViewManager.Accelerator * 1000 * TViewManager.VIMatrix.Row2);
            else
                ma -= (Vec3)(TViewManager.Accelerator * 1000 * TViewManager.VIMatrix.Row2);
            Speed += ma * dt;

            //Mtx4 m = Mtx4.CreateTranslation(Speed);
            //TPlanetTrajectory.SolarSystemLink.Matrix *= m;
            if (NearestPlanetLink != null)
            {
                Mtx4 MM = NearestPlanetLink.PlanetLink.Matrix * SolarSystemLink.Matrix * TViewManager.VMatrix;
                Mtx4 M = Mtx4.Identity;
                M.Row2 = -MM.Row3;
                M.Row2.Normalize();
                M.Row1 = Vec4.Cross(M.Row2, TViewManager.VMatrix.Row2);
                M.Row1.Normalize();
                M.Row0 = Vec4.Cross(M.Row1, M.Row2);
                M.Row0.Normalize();
                M.Row3 = MM.Row3 + M.Row2 * Math.Abs(NearestPlanetLink.Body.Radius);
                M.Row3.W = 1;
                TModelsManager.Internals.LandmarksLink.Matrix = M * TViewManager.VIMatrix;
            }
        }

        public static bool Reverse;

        static double G = 6.67408e-11;

        public static Vec3 Attraction;
        public static Vec3 Speed;

        static Timer Clock;
        public static TLink SolarSystemLink;
        public static TLink SolarLink;
        public static TPlanetTrajectory NearestPlanetLink;
        public static TPlanetTrajectory TargetedPlanetLink;

    }
}

