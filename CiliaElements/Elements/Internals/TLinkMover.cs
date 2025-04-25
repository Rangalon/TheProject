using Math3D;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CiliaElements.Internals
{
    public class TLinkFlappingMover : TLinkMover
    {
        #region Public Fields

        public readonly Vec3 Axis, AxisOrigin;

        public readonly double Duration, Speed;

        public int Tempo1, Tempo2, Tempo3;

        #endregion Public Fields

        #region Private Fields

        private Mtx4 mtxt, mtxti;

        #endregion Private Fields

        #region Public Constructors

        public TLinkFlappingMover(TBaseElement owner, string[] mv) : base(owner, mv)
        {
            AxisOrigin.X = double.Parse(mv[4]);
            AxisOrigin.Y = double.Parse(mv[5]);
            AxisOrigin.Z = double.Parse(mv[6]);
            Axis.X = double.Parse(mv[7]);
            Axis.Y = double.Parse(mv[8]);
            Axis.Z = double.Parse(mv[9]);
            Duration = double.Parse(mv[10]);
            Speed = double.Parse(mv[11]);
            Tempo1 = int.Parse(mv[12]);
            Tempo2 = int.Parse(mv[13]);
            Tempo3 = int.Parse(mv[14]);
            //
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        public TLinkFlappingMover(TBaseElement owner, TLink children, Vec3 axisOrigin, Vec3 axis, double duration, double speed, Mtx4 absoluteMatrix) : base(owner, children)
        {
            AxisOrigin = axisOrigin;
            Axis = axis - AxisOrigin;
            Axis.Normalize();
            Duration = duration;
            Speed = speed;
            //
            absoluteMatrix = Mtx4.Invert(absoluteMatrix);
            Axis = Vec4.TransformVector(Axis, absoluteMatrix);
            AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Do(TLink sender)
        {
            SavePosition(sender);
            //
            while (perpetual)
            {
                Thread.Sleep(Tempo1);
                //
                DoRotate(sender, Axis, Duration, Speed, 0, mtxt, mtxti);
                //
                Thread.Sleep(Tempo2);
                //
                DoRotate(sender, Axis, 2 * Duration, -Speed, Duration, mtxt, mtxti);
                //
                Thread.Sleep(Tempo2);
                //
                DoRotate(sender, Axis, Duration, Speed, Duration, mtxt, mtxti);
                //
                Thread.Sleep(Tempo3);
            }
            //
            sender.Matrix = RecallPosition(sender);
        }

        public override void Save(StreamWriter wtr)
        {
            base.Save(wtr);
            wtr.Write(AxisOrigin); wtr.Write(";");
            wtr.Write(Axis); wtr.Write(";");
            wtr.Write(Duration); wtr.Write(";");
            wtr.Write(Speed); wtr.Write(";");
            wtr.Write(Tempo1); wtr.Write(";");
            wtr.Write(Tempo2); wtr.Write(";");
            wtr.Write(Tempo3); wtr.Write(";");
        }

        public override void SetValue(TLink sender, double value)
        {
            SavePosition(sender);
            //ApplyDisplayColor(CiliaElements.FormatSTEP.TSTEPElement.LightBlue, sender);
            DoPositionRotation(sender, Axis, value, mtxt, mtxti);
        }

        public override void SetValue(double value)
        {
        }

        #endregion Public Methods
    }

    public abstract class TLinkMover
    {
        #region Public Fields

        public static void ApplyDisplayColor(Mtx4f clr, TLink lk)
        {
            if (lk.Solid != null)
                lk.DisplayColor = clr;
            else
                foreach (TLink l in lk.Links.Values.Where(o => o != null)) ApplyDisplayColor(clr, l);
        }


        public static List<TLinkMover> Movers = new List<TLinkMover>();
        public readonly TBaseElement Owner;
        public readonly string Parent, Children;
        public List<TLink> Links = new List<TLink>();
        public string Name;

        #endregion Public Fields

        #region Internal Fields

        internal static bool perpetual = true;
        internal static Dictionary<TLink, Mtx4> SavedPositions = new Dictionary<TLink, Mtx4>();

        #endregion Internal Fields

        #region Protected Fields

        protected static DateTime OriginStart = DateTime.Now;

        #endregion Protected Fields

        #region Private Fields

        private static Dictionary<string, ConstructorInfo> Constructors = new Dictionary<string, ConstructorInfo>();
        private static List<TBaseElement> owners = new List<TBaseElement>();

        #endregion Private Fields

        #region Public Constructors

        static TLinkMover()
        {
            Assembly ass = typeof(TLinkMover).Assembly;
            foreach (Type tp in ass.GetTypes().Where(o => o.IsSubclassOf(typeof(TLinkMover))))
            {
                ConstructorInfo[] cis = tp.GetConstructors();
                foreach (ConstructorInfo ci in cis)
                {
                    ParameterInfo[] args = ci.GetParameters();
                    if (args.Length == 2 && args[1].ParameterType == typeof(string[])) Constructors.Add(tp.Name, ci);
                }
            }
        }

        public TLinkMover(TBaseElement owner, TLink children)
        {
            Owner = owner;
            Movers.Add(this);
            if (!owners.Contains(owner)) owners.Add(owner);
            Parent = children.ParentLink.PartNumber;
            Children = children.PartNumber;
        }

        public TLinkMover(TBaseElement owner, string[] mv)
        {
            Owner = owner;
            Movers.Add(this);
            if (!owners.Contains(owner)) owners.Add(owner);
            Name = mv[1];
            Parent = mv[2];
            Children = mv[3];
        }

        #endregion Public Constructors

        #region Public Methods

        public static Mtx4 RecallPosition(TLink l)
        {
            return SavedPositions[l];
        }

        public static void SavePosition(TLink l)
        {
            if (!SavedPositions.ContainsKey(l)) SavedPositions.Add(l, l.Matrix);
        }

        public abstract void Do(TLink sender);

        public virtual void Save(StreamWriter wtr)
        {
            wtr.Write(this.GetType().Name); wtr.Write(";");
            wtr.Write(Name); wtr.Write(";");
            wtr.Write(Parent); wtr.Write(";");
            wtr.Write(Children); wtr.Write(";");
        }

        public abstract void SetValue(TLink sender, double value);
        public abstract void SetValue(double value);

        #endregion Public Methods

        #region Internal Methods

        internal static TLinkMover CreateFromWords(TBaseElement elmt, string[] mv) => (TLinkMover)Constructors[mv[0]].Invoke(new Object[] { elmt, mv });

        internal static void DoMoveRotation(TLink sender, Vec3 Axis, double speed, Mtx4 mtxt, Mtx4 mtxti)
        {
            Mtx4 mtxr;
            Mtx4 originMtx = RecallPosition(sender);
            mtxr = Mtx4.CreateRotation(Axis, 10 * speed * (DateTime.Now - OriginStart).TotalSeconds);
            sender.Matrix = originMtx * mtxti * mtxr * mtxt;
        }

        internal static void DoPositionRotation(TLink sender, Vec3 Axis, double angle, Mtx4 mtxt, Mtx4 mtxti)
        {
            Mtx4 mtxr;
            Mtx4 originMtx = RecallPosition(sender);
            mtxr = Mtx4.CreateRotation(Axis, angle);
            sender.Matrix = originMtx * mtxti * mtxr * mtxt;
        }

        internal static void DoRotate(TLink sender, Vec3 Axis, double duration, double speed, double untime, Mtx4 mtxt, Mtx4 mtxti)
        {
            Mtx4 mtxr;
            Mtx4 originMtx = RecallPosition(sender);
            DateTime dtstart = DateTime.Now;
            TimeSpan ts = DateTime.Now - dtstart;
            while (ts.TotalMilliseconds < duration)
            {
                mtxr = Mtx4.CreateRotation(Axis, (ts.TotalMilliseconds - untime) * speed);
                sender.PendingMatrix = originMtx * mtxti * mtxr * mtxt;
                lock (TManager.LinksToMove) TManager.LinksToMove.Push(sender);
                //Console.WriteLine(sender.ToString() + ": " + ((ts.TotalMilliseconds - untime) * speed).ToString());
                Thread.Sleep(10);
                ts = DateTime.Now - dtstart;
            }
            mtxr = Mtx4.CreateRotation(Axis, (duration - untime) * speed);
            sender.PendingMatrix = originMtx * mtxti * mtxr * mtxt;
            lock (TManager.LinksToMove) TManager.LinksToMove.Push(sender);
        }

        internal static void SaveAll()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            foreach (TBaseElement owner in owners)
            {
                TLinkMover[] mvs = Movers.Where(o => o.Owner == owner).ToArray();
                FileInfo fi = new FileInfo(owner.Fi.FullName + ".k.tmp");
                StreamWriter wtr = new StreamWriter(fi.FullName);
                foreach (TLinkMover m in mvs)
                {
                    m.Save(wtr);
                    wtr.WriteLine();
                }
                wtr.Close(); wtr.Dispose();
            }
        }

        internal virtual void Transpose(Mtx4 absoluteMatrix)
        { }

        #endregion Internal Methods
    }

    public class TLinkRotationMover : TLinkMover
    {
        #region Public Fields

        public readonly Vec3 Axis, AxisOrigin;

        public double Speed;

        #endregion Public Fields

        #region Private Fields

        private Mtx4 mtxt, mtxti;

        #endregion Private Fields

        #region Public Constructors
        public override void SetValue(double value)
        {
        }

        public TLinkRotationMover(TBaseElement owner, TLink children, Vec3 axisOrigin, Vec3 axis, double speed, Mtx4 absoluteMatrix) : base(owner, children)
        {
            AxisOrigin = axisOrigin;
            Axis = axis - AxisOrigin;
            Axis.Normalize();
            Speed = speed;
            //
            absoluteMatrix = Mtx4.Invert(absoluteMatrix);
            Axis = Vec4.TransformVector(Axis, absoluteMatrix);
            AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        public TLinkRotationMover(TBaseElement owner, string[] mv) : base(owner, mv)
        {
            AxisOrigin.X = double.Parse(mv[4]);
            AxisOrigin.Y = double.Parse(mv[5]);
            AxisOrigin.Z = double.Parse(mv[6]);
            Axis.X = double.Parse(mv[7]);
            Axis.Y = double.Parse(mv[8]);
            Axis.Z = double.Parse(mv[9]);
            Speed = double.Parse(mv[10]);
            //
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Do(TLink sender)
        {
            //
            SavePosition(sender);
            //
            while (perpetual)
            {
                Thread.Sleep(10);

                DoMoveRotation(sender, Axis, -Speed, mtxt, mtxti);
            }
            //
            sender.Matrix = RecallPosition(sender);
        }

        public override void Save(StreamWriter wtr)
        {
            base.Save(wtr);
            wtr.Write(AxisOrigin); wtr.Write(";");
            wtr.Write(Axis); wtr.Write(";");
            wtr.Write(Speed); wtr.Write(";");
        }

        public override void SetValue(TLink sender, double value)
        {
            SavePosition(sender);
            DoPositionRotation(sender, Axis, 10 * Speed * (DateTime.Now - OriginStart).TotalSeconds, mtxt, mtxti);
        }

        #endregion Public Methods

        //internal override void Transpose(Mtx4 absoluteMatrix)
        //{
        //    absoluteMatrix = Mtx4.Invert(absoluteMatrix);
        //    Axis = Vec4.TransformVector(Axis, absoluteMatrix);
        //    AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
        //    mtxt = Mtx4.CreateTranslation(AxisOrigin);
        //    mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        //}
    }

    public class TLinkTiltMover : TLinkMover
    {
        #region Public Fields

        public readonly Vec3 Axis, AxisOrigin;
        public readonly double Duration, Speed;
        public int Tempo1, Tempo2, Tempo3;

        #endregion Public Fields

        #region Private Fields

        private Mtx4 mtxt, mtxti;
        public double PrevValue = 0;

        #endregion Private Fields

        #region Public Constructors
        public override void SetValue(double value)
        {
            if (value == PrevValue) return;
            if (value == 1)
            {
                foreach (TLink sender in Links)
                    (new Thread(Tilt) { Priority = ThreadPriority.Highest }).Start(sender);
            }
            else if (value == 0)
            {
                foreach (TLink sender in Links)
                    (new Thread(Untilt) { Priority = ThreadPriority.Highest }).Start(sender);
            }
            PrevValue = value;
        }

        public TLinkTiltMover(TBaseElement owner, TLink children, Vec3 axisOrigin, Vec3 axis, double duration, double speed, Mtx4 absoluteMatrix) : base(owner, children)
        {
            AxisOrigin = axisOrigin;
            Axis = axis - AxisOrigin;
            Axis.Normalize();
            Duration = duration;
            Speed = speed;
            //
            absoluteMatrix = Mtx4.Invert(absoluteMatrix);
            Axis = Vec4.TransformVector(Axis, absoluteMatrix);
            AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        public TLinkTiltMover(TBaseElement owner, string[] mv) : base(owner, mv)
        {
            AxisOrigin.X = double.Parse(mv[4]);
            AxisOrigin.Y = double.Parse(mv[5]);
            AxisOrigin.Z = double.Parse(mv[6]);
            Axis.X = double.Parse(mv[7]);
            Axis.Y = double.Parse(mv[8]);
            Axis.Z = double.Parse(mv[9]);
            Duration = double.Parse(mv[10]);
            Speed = double.Parse(mv[11]);
            Tempo1 = int.Parse(mv[12]);
            Tempo2 = int.Parse(mv[13]);
            Tempo3 = int.Parse(mv[14]);
            //
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Do(TLink sender)
        {
            //
            SavePosition(sender);
            //
            while (perpetual)
            {
                Thread.Sleep(Tempo1);
                //
                DoRotate(sender, Axis, Duration, Speed, 0, mtxt, mtxti);
                //
                Thread.Sleep(Tempo2);
                //
                DoRotate(sender, Axis, Duration, -Speed, Duration, mtxt, mtxti);
                //
                Thread.Sleep(Tempo3);
            }
            //
            sender.Matrix = RecallPosition(sender);
        }

        public override void Save(StreamWriter wtr)
        {
            base.Save(wtr);
            wtr.Write(AxisOrigin); wtr.Write(";");
            wtr.Write(Axis); wtr.Write(";");
            wtr.Write(Duration); wtr.Write(";");
            wtr.Write(Speed); wtr.Write(";");
            wtr.Write(Tempo1); wtr.Write(";");
            wtr.Write(Tempo2); wtr.Write(";");
            wtr.Write(Tempo3); wtr.Write(";");
        }

        public override void SetValue(TLink sender, double value)
        {
            if (value == 1)
                (new Thread(Tilt) { Priority = ThreadPriority.Highest }).Start(sender);
            else if (value == 0)
                (new Thread(Untilt) { Priority = ThreadPriority.Highest }).Start(sender);
            PrevValue = value;
        }

        #endregion Public Methods

        #region Private Methods

        private void Tilt(object sender)
        {
            Console.WriteLine("Tilt " + sender.ToString());
            //
            SavePosition((TLink)sender);
            //
            Thread.Sleep(Tempo1);
            //
            DoRotate((TLink)sender, Axis, Duration, Speed, 0, mtxt, mtxti);
        }

        private void Untilt(object sender)
        {
            Console.WriteLine("Untilt " + sender.ToString());
            //
            SavePosition((TLink)sender);
            //
            Thread.Sleep(Tempo2);
            //
            DoRotate((TLink)sender, Axis, Duration, -Speed, Duration, mtxt, mtxti);
        }

        #endregion Private Methods

        //internal override void Transpose(Mtx4 absoluteMatrix)
        //{
        //    absoluteMatrix = Mtx4.Invert(absoluteMatrix);
        //    Axis = Vec4.TransformVector(Axis, absoluteMatrix);
        //    AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
        //    mtxt = Mtx4.CreateTranslation(AxisOrigin);
        //    mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        //}
    }

    public class TLinkTiltUntiltMover : TLinkMover
    {
        #region Public Fields

        public readonly Vec3 Axis, AxisOrigin;
        public readonly double Duration, Speed;
        public int Tempo1, Tempo2, Tempo3;

        #endregion Public Fields

        #region Private Fields

        private Mtx4 mtxt, mtxti;

        public double PrevValue = 0;

        #endregion Private Fields

        #region Public Constructors

        public TLinkTiltUntiltMover(TBaseElement owner, TLink children, Vec3 axisOrigin, Vec3 axis, double duration, double speed, Mtx4 absoluteMatrix) : base(owner, children)
        {
            AxisOrigin = axisOrigin;
            Axis = axis - AxisOrigin;
            Axis.Normalize();
            Duration = duration;
            Speed = speed;
            //
            absoluteMatrix = Mtx4.Invert(absoluteMatrix);
            Axis = Vec4.TransformVector(Axis, absoluteMatrix);
            AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        public TLinkTiltUntiltMover(TBaseElement owner, string[] mv) : base(owner, mv)
        {
            AxisOrigin.X = double.Parse(mv[4]);
            AxisOrigin.Y = double.Parse(mv[5]);
            AxisOrigin.Z = double.Parse(mv[6]);
            Axis.X = double.Parse(mv[7]);
            Axis.Y = double.Parse(mv[8]);
            Axis.Z = double.Parse(mv[9]);
            Duration = double.Parse(mv[10]);
            Speed = double.Parse(mv[11]);
            Tempo1 = int.Parse(mv[12]);
            Tempo2 = int.Parse(mv[13]);
            Tempo3 = int.Parse(mv[14]);
            //
            mtxt = Mtx4.CreateTranslation(AxisOrigin);
            mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        }

        #endregion Public Constructors

        #region Public Methods

        public override void Do(TLink sender)
        {
            //
            SavePosition(sender);
            //
            while (perpetual)
            {
                Thread.Sleep(Tempo1);
                //
                DoRotate(sender, Axis, Duration, Speed, 0, mtxt, mtxti);
                //
                Thread.Sleep(Tempo2);
                //
                DoRotate(sender, Axis, Duration, -Speed, Duration, mtxt, mtxti);
                //
                Thread.Sleep(Tempo3);
            }
            //
            sender.Matrix = RecallPosition(sender);
        }

        public override void Save(StreamWriter wtr)
        {
            base.Save(wtr);
            wtr.Write(AxisOrigin); wtr.Write(";");
            wtr.Write(Axis); wtr.Write(";");
            wtr.Write(Duration); wtr.Write(";");
            wtr.Write(Speed); wtr.Write(";");
            wtr.Write(Tempo1); wtr.Write(";");
            wtr.Write(Tempo2); wtr.Write(";");
            wtr.Write(Tempo3); wtr.Write(";");
        }

        public override void SetValue(TLink sender, double value)
        {
            if (value == 1)
                (new Thread(TiltUntilt)).Start(sender);
            else if (value == 0)
                (new Thread(TiltUntilt)).Start(sender);
            PrevValue = value;
        }


        public override void SetValue(double value)
        {
            if (value == PrevValue) return;
            if (value == 1)
            {
                foreach (TLink sender in Links)
                    (new Thread(TiltUntilt) { Priority = ThreadPriority.Highest }).Start(sender);
            }
            else if (value == 0)
            {
                foreach (TLink sender in Links)
                    (new Thread(TiltUntilt) { Priority = ThreadPriority.Highest }).Start(sender);
            }
            PrevValue = value;
        }

        #endregion Public Methods

        #region Private Methods

        private void TiltUntilt(object sender)
        {
            SavePosition((TLink)sender);
            //
            Thread.Sleep(Tempo1);
            //
            DoRotate((TLink)sender, Axis, Duration, Speed, 0, mtxt, mtxti);
            //
            Thread.Sleep(Tempo2);
            //
            DoRotate((TLink)sender, Axis, Duration, -Speed, Duration, mtxt, mtxti);
            // 
        }

        #endregion Private Methods

        //internal override void Transpose(Mtx4 absoluteMatrix)
        //{
        //    absoluteMatrix = Mtx4.Invert(absoluteMatrix);
        //    Axis = Vec4.TransformVector(Axis, absoluteMatrix);
        //    AxisOrigin = Vec4.TransformPoint(AxisOrigin, absoluteMatrix);
        //    mtxt = Mtx4.CreateTranslation(AxisOrigin);
        //    mtxti = Mtx4.CreateTranslation(-AxisOrigin);
        //}
    }
}