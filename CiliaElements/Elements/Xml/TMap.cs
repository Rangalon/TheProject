
using Math3D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Xml;

namespace CiliaElements.FormatXml
{
    public class TFin
    {
        #region Public Fields

        public Vector6 Anchor;
        public Vector6 Location;
        public string Name;

        #endregion Public Fields

        #region Public Methods

        public override string ToString()
        {
            return Name + " " + Location.ToString();
        }

        #endregion Public Methods
    }

    public class TFinNameComparer : IEqualityComparer<TFin>
    {
        #region Private Fields

        private static VectorComparer MeterComparer = new VectorComparer(0.00005F);

        #endregion Private Fields

        #region Public Methods

        bool IEqualityComparer<TFin>.Equals(TFin x, TFin y)
        {
            if (!x.Name.Equals(y.Name)) return false;
            if (!MeterComparer.Check(x.Location, y.Location)) return false;

            return true;
        }

        int IEqualityComparer<TFin>.GetHashCode(TFin obj)
        {
            return this.GetHashCode();
        }

        #endregion Public Methods
    }

    public class TMap : TAssemblyElement
    {
        #region Private Fields

        private static readonly string[] CabinGroups = new string[] { "Lavatory", "SeatRail", "PinSeat", "Galley", "Window", "CAS", "Partition", "Stowage", "CrewSeat" };
        private static readonly string[] CASItems = new string[] { "Light", "Bell" };
        private static readonly string[] ElecGroups = new string[] { "Branches", "Extremities", "Derivations", "XRefs" };
        private static readonly string[] OxyGroups = new string[] { "Masks" };
        private static readonly string[] PinItems = new string[] { "Phone", "Light", "Bell", "USB", "IFE" };
        private static Vec4 OffsetVector = new Vec4() { Y = 0.1 };
        private static Mtx4 SymbolsMatrix = Mtx4.CreateScale(0.5);
        private static Mtx4 YXPlan = Mtx4.CreateRotationX(Math.PI) * Mtx4.CreateRotationZ(Math.PI * 0.5);
        private static Mtx4 YZPlan = Mtx4.CreateRotationX(Math.PI * 0.5) * Mtx4.CreateRotationZ(Math.PI * 0.5);
        private XmlDocument doc = new XmlDocument();

        #endregion Private Fields

        #region Public Methods

        public void AddCabinGroups()
        {
            foreach (string s in CabinGroups)
            {
                TLink link = new TLink
                {
                    Child = new TAssemblyElement(),
                    ParentLink = OwnerLink,
                    Matrix = Mtx4.Identity,
                    NodeName = s
                };
            }
        }

        public void AddElecGroups()
        {
            foreach (string s in ElecGroups)
            {
                TLink link = new TLink
                {
                    Child = new TAssemblyElement(),
                    ParentLink = OwnerLink,
                    Matrix = Mtx4.Identity,
                    NodeName = s
                };
            }
        }

        public void AddOxyGroups()
        {
            foreach (string s in OxyGroups)
            {
                TLink link = new TLink
                {
                    Child = new TAssemblyElement(),
                    ParentLink = OwnerLink,
                    Matrix = Mtx4.Identity,
                    NodeName = s
                };
            }
        }

        public override void LaunchLoad()
        {
            doc.Load(Fi.FullName);
            //
            if (doc.SelectNodes("/EPAC-TDU-Information").Count > 0) { LoadCabin1(); }
            //
            if (doc.SelectNodes("/Project").Count > 0) { LoadCabin2(); }
            //
            if (doc.SelectNodes("/EL").Count > 0) { LoadElec1(); }
            //
            if (doc.SelectNodes("/OXYs").Count > 0) { LoadOxy1(); }
            //
            if (doc.SelectNodes("/WIRING_STRUCTURE").Count > 0) { LoadWiring(); }
            //
            ElementLoader.Publish();
        }

        #endregion Public Methods

        #region Private Methods

        private static Mtx4 GetMatrix2(XmlElement e, int n)
        {
            Mtx4 m = new Mtx4();// Matrix4d.CreateScale(v.Y, v.X, 15) * Matrix4d.CreateTranslation(new Vector4d(0, 0 * v.X * 0.1 - (n - 1) * 0.5 * v.X * 0.1, 0, 0)) * Matrix4d.CreateRotationZ(a);
            XmlElement rp = (XmlElement)e.SelectSingleNode("./Properties/GeometricItem.RelativePosition/Transform3D");
            XmlElement bb = (XmlElement)e.SelectSingleNode("./Properties/GeometricItem.BoundingBox");
            //
            if (rp != null && bb != null)
            {
                m.Row0.X = double.Parse(rp.SelectSingleNode("./M11").InnerText, CultureInfo.InvariantCulture);
                m.Row0.Y = double.Parse(rp.SelectSingleNode("./M21").InnerText, CultureInfo.InvariantCulture);
                m.Row0.Z = double.Parse(rp.SelectSingleNode("./M31").InnerText, CultureInfo.InvariantCulture);
                m.Row1.X = double.Parse(rp.SelectSingleNode("./M12").InnerText, CultureInfo.InvariantCulture);
                m.Row1.Y = double.Parse(rp.SelectSingleNode("./M22").InnerText, CultureInfo.InvariantCulture);
                m.Row1.Z = double.Parse(rp.SelectSingleNode("./M32").InnerText, CultureInfo.InvariantCulture);
                m.Row2.X = double.Parse(rp.SelectSingleNode("./M13").InnerText, CultureInfo.InvariantCulture);
                m.Row2.Y = double.Parse(rp.SelectSingleNode("./M23").InnerText, CultureInfo.InvariantCulture);
                m.Row2.Z = double.Parse(rp.SelectSingleNode("./M33").InnerText, CultureInfo.InvariantCulture);
                //
                Vec3 sc = new Vec3
                {
                    X = double.Parse(bb.SelectSingleNode("./XMin").InnerText, CultureInfo.InvariantCulture) - double.Parse(bb.SelectSingleNode("./XMin").InnerText, CultureInfo.InvariantCulture),
                    Y = double.Parse(bb.SelectSingleNode("./YMin").InnerText, CultureInfo.InvariantCulture) - double.Parse(bb.SelectSingleNode("./YMin").InnerText, CultureInfo.InvariantCulture),
                    Z = double.Parse(bb.SelectSingleNode("./ZMin").InnerText, CultureInfo.InvariantCulture) - double.Parse(bb.SelectSingleNode("./ZMin").InnerText, CultureInfo.InvariantCulture)
                };
                sc.Y /= n;
                m = Mtx4.CreateScale(sc * 0.01) * m;
                //
                //m.Row3.X = (double.Parse(rp.SelectSingleNode("./X").InnerText) + (double.Parse(bb.SelectSingleNode("./XMin").InnerText) + double.Parse(bb.SelectSingleNode("./XMin").InnerText)) * 0.5) / 1000;
                //m.Row3.Y = (double.Parse(rp.SelectSingleNode("./Y").InnerText) + (double.Parse(bb.SelectSingleNode("./YMin").InnerText) + double.Parse(bb.SelectSingleNode("./YMin").InnerText)) * 0.5 - sc.Y * (n - 1) * 0.5) / 1000;
                //m.Row3.Z = (double.Parse(rp.SelectSingleNode("./Z").InnerText) + (double.Parse(bb.SelectSingleNode("./ZMin").InnerText) + double.Parse(bb.SelectSingleNode("./ZMin").InnerText)) * 0) / 1000;
                m.Row3.X = (double.Parse(rp.SelectSingleNode("./X").InnerText, CultureInfo.InvariantCulture) + double.Parse(bb.SelectSingleNode("./XMin").InnerText, CultureInfo.InvariantCulture)) / 1000;
                m.Row3.Y = (double.Parse(rp.SelectSingleNode("./Y").InnerText, CultureInfo.InvariantCulture) + double.Parse(bb.SelectSingleNode("./YMin").InnerText, CultureInfo.InvariantCulture)) / 1000;
                m.Row3.Z = (double.Parse(rp.SelectSingleNode("./Z").InnerText, CultureInfo.InvariantCulture) + double.Parse(bb.SelectSingleNode("./ZMin").InnerText, CultureInfo.InvariantCulture)) / 1000;
                m.Row3.W = 1;
            }
            else
            {
                return Mtx4.Identity;
            }
            return m;
        }

        private static Vec4 GetOffset(Mtx4 m)
        {
            return Vec4.Transform(OffsetVector, m);
        }

        private void AddBranche1(XmlElement e)
        {
            TElement s = new TElement(e);
            TLink l = new TLink
            {
                FileName = s.Fi.FullName,
                NodeName = s.PartNumber,
                Child = s,
                Matrix = Mtx4.CreateTranslation(0, 0, 0)
            }; OwnerLink.FindFromNodeName("Branches").Links.Push(l);
        }

        private void AddCAS1(XmlElement e)
        {
            //
            Vec4 p = new Vec4();
            double a = 0;
            //
            p.X = double.Parse(e.GetAttribute("X-Position"), CultureInfo.InvariantCulture);
            p.Y = -double.Parse(e.GetAttribute("Y-Position"), CultureInfo.InvariantCulture);
            Vec2 v = new Vec2
            {
                X = double.Parse(e.SelectSingleNode("./Ca-seat/@Width").Value, CultureInfo.InvariantCulture),
                Y = double.Parse(e.SelectSingleNode("./Ca-seat/@Depth").Value, CultureInfo.InvariantCulture)
            };
            a = double.Parse(e.SelectSingleNode("./Ca-seat/@Angle").Value, CultureInfo.InvariantCulture) + Math.PI;
            switch (e.GetAttribute("Column"))
            {
                case "LH":
                    p.Y -= v.X;
                    break;

                case "RH":
                case "CTR":
                    break;
            }
            //
            p /= 1000;
            v /= 100;
            //
            Mtx4 Sc = Mtx4.CreateScale(v.Y, v.X, 12);
            TLink link = new TLink
            {
                Child = new TAssemblyElement(),
                ParentLink = OwnerLink.FindFromNodeName("CrewSeat"),
                Matrix = Mtx4.Identity,
                NodeName = OwnerLink.FindFromNodeName("CrewSeat").Links.Max.ToString(CultureInfo.InvariantCulture)
            };
            //
            Mtx4 m = Sc * Mtx4.CreateRotationZ(a);
            m.Row3 += p;
            if (a == Math.PI)
                m.Row3 += Vec4.Transform(new Vec4(-0.1, -0.1, 0, 0), m);
            TLink l = TManager.AttachElmt(TManager.View.OwnerLink.FindFromPartNumber("Seat"), link, null);
            l.NodeName = "Seat";
            l.Matrix = m;
            //
            m = Mtx4.CreateRotationZ(a);
            m.Row3 += p;
            Vec4 ox = Vec4.Transform(new Vec4(0.06, 0, 0, 0), m);
            Vec4 oy = Vec4.Transform(new Vec4(0, 0.05, 0, 0), m);
            m.Row3 -= 3 * oy;
            if (a == Math.PI)
                m.Row3 += Vec4.Transform(new Vec4(-0.06, -0.05, 0.05, 0), Sc * m);
            else
                m.Row3 += Vec4.Transform(new Vec4(0.05, 0.05, 0.05, 0), Sc * m);
            //
            link.ForeColor = Mtx4.CreateStaticColor(1, 1, 1);
            link.TextRanges.Add(new TTextRange()
            {
                Matrix = YZPlan * m * Mtx4.CreateTranslation(0, 0, 0.2) * Mtx4.CreateTranslation(3 * oy),
                Text = link.NodeName,
                Size = Mtx4.CreateScale(0.03),
                LineWidth = 30
            });
            link.TextRanges.Add(new TTextRange()
            {
                Matrix = YXPlan * m * Mtx4.CreateTranslation(0, 0, -0.1) * Mtx4.CreateTranslation(6 * oy) * Mtx4.CreateTranslation(-ox),
                Text = link.NodeName,
                Size = Mtx4.CreateScale(0.03),
                LineWidth = 30
            });
            //
            m = SymbolsMatrix * m;
            //
            foreach (string s in CASItems)
            {
                m.Row3 += oy;
                l = TManager.AttachElmt(TManager.View.OwnerLink.FindFromPartNumber(s), link, null);
                l.NodeName = link.NodeName + " " + l.PartNumber;
                l.Matrix = m;
                if (DateTime.Now.Ticks % 3 == 0) l.State = ELinkState.InError;
            }
            //
        }

        private void AddCAS2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Seat").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("CAS").Links.Push(l);
        }

        private void AddExtremity1(XmlElement e)
        {
            if (e.SelectSingleNode("./INFO3D_COORD/Point") == null) return;
            Vec4 p = new Vec4(
                   double.Parse(e.SelectSingleNode("./INFO3D_COORD/Point/@X").Value, CultureInfo.InvariantCulture),
                   double.Parse(e.SelectSingleNode("./INFO3D_COORD/Point/@Y").Value, CultureInfo.InvariantCulture),
                   double.Parse(e.SelectSingleNode("./INFO3D_COORD/Point/@Z").Value, CultureInfo.InvariantCulture),
                   0)
            {
                W = 1
            };
            //
            Vec4 x;
            if (e.SelectSingleNode("./INFO3D_COORD/Vector") != null)
            {
                x = new Vec4(
                    double.Parse(e.SelectSingleNode("./INFO3D_COORD/Vector/@X").Value, CultureInfo.InvariantCulture),
                    double.Parse(e.SelectSingleNode("./INFO3D_COORD/Vector/@Y").Value, CultureInfo.InvariantCulture),
                    double.Parse(e.SelectSingleNode("./INFO3D_COORD/Vector/@Z").Value, CultureInfo.InvariantCulture),
                    0);
            }
            else
            {
                x = new Vec4(1, 0, 0, 0);
            }
            x.W = 0;
            //
            //el = (System.Xml.XmlElement)el.ParentNode.ParentNode.ParentNode;
            //System.Xml.XmlNodeList lst = el.SelectNodes("./SEGEXT");
            //for (int i = 0; i < lst.Count; i++)
            //{
            //    if (lst[i].InnerText == e.GetAttribute("ID"))
            //    {
            //        if (i == 0) x *= -1;

            //    }
            //}

            //
            Vec4 y;
            if (x.X == x.Y && x.Y == x.Z) y = new Vec4(0, 1, 0, 0);
            else y = new Vec4(x.Y, x.Z, x.X, 0);
            //
            Vec4 z = Vec4.Cross(x, y);
            y = Vec4.Cross(z, x);

            x.Normalize();
            y.Normalize();
            z.Normalize();
            //
            TElement s = new TElement(e);
            TLink l = new TLink
            {
                FileName = s.Fi.FullName,
                NodeName = s.PartNumber
            }; Mtx4 m = Mtx4.Identity;
            m.Row0 = x;
            m.Row1 = y;
            m.Row2 = z;
            //m.Invert();
            //p.X += 4;
            m.Row3 = p;
            l.Child = s; l.Matrix = m;
            OwnerLink.FindFromNodeName("Extremities").Links.Push(l);
        }

        private void AddGalley1(XmlElement e)
        {
            Vec4 p = new Vec4();
            //
            double a = 0;
            p.X = double.Parse(e.GetAttribute("X-Position"), CultureInfo.InvariantCulture);
            p.Y = -double.Parse(e.GetAttribute("Y-Position"), CultureInfo.InvariantCulture);
            Vec2 v = new Vec2
            {
                X = double.Parse(e.SelectSingleNode("./Galley/@Width").Value, CultureInfo.InvariantCulture),
                Y = double.Parse(e.SelectSingleNode("./Galley/@Depth").Value, CultureInfo.InvariantCulture)
            };
            switch (e.SelectSingleNode("./Galley/@Type").Value)
            {
                case "LONGITUDINAL":
                    a += Math.PI * 0.5;
                    double d = v.X;
                    v.X = v.Y;
                    v.Y = d;
                    switch (e.GetAttribute("Column"))
                    {
                        case "LH":
                            p.X += v.Y;
                            p.Y -= v.X;
                            break;

                        case "RH":
                        case "CTR":
                            p.Y += v.X;
                            break;
                    }
                    break;

                default:
                    switch (e.GetAttribute("Column"))
                    {
                        case "LH":
                            //p.X += v.Y;
                            p.Y -= v.X;
                            break;

                        case "RH":
                        case "CTR":
                            break;
                    }
                    break;
            }

            //
            p /= 1000;
            v /= 100;
            //
            switch (e.SelectSingleNode("./Galley/@Type").Value)
            {
                case "LONGITUDINAL":
                    double d = v.X;
                    v.X = v.Y;
                    v.Y = d;
                    break;
            }
            switch (e.SelectSingleNode("./Galley/@Trolley-orientation").Value)
            {
                case "LH":
                case "FWD":
                    switch (e.SelectSingleNode("./Galley/@Type").Value)
                    {
                        case "LONGITUDINAL":
                            break;

                        default:
                            p.X += v.Y * 0.1;
                            p.Y += v.X * 0.1;
                            break;
                    }
                    a += Math.PI;
                    break;
            }
            //
            double n = e.SelectNodes(".//Symbol-slot").Count;
            v.X /= n;
            //
            for (int i = 0; i < n; i++)
            {
                Mtx4 m = Mtx4.CreateScale(v.Y, v.X, 12) * Mtx4.CreateTranslation(new Vec4(0, i * v.X * 0.1, 0, 0)) * Mtx4.CreateRotationZ(a);
                m.Row3 += p; TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Galley").Child;
                TLink l = new TLink
                {
                    FileName = Fi.FullName,
                    NodeName = i.ToString(CultureInfo.InvariantCulture),
                    Child = s,
                    Matrix = m
                }; OwnerLink.FindFromNodeName("Galley").Links.Push(l);
            }
        }

        private void AddGalley2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Galley").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("Galley").Links.Push(l);
        }

        private void AddLavatory1(XmlElement e)
        {
            Vec4 p = new Vec4();
            //
            double a = 0;
            p.X = double.Parse(e.GetAttribute("X-Position"), CultureInfo.InvariantCulture);
            p.Y = -double.Parse(e.GetAttribute("Y-Position"), CultureInfo.InvariantCulture);
            Vec2 v = new Vec2
            {
                X = double.Parse(e.SelectSingleNode("./Lavatory/@Width").Value, CultureInfo.InvariantCulture),
                Y = double.Parse(e.SelectSingleNode("./Lavatory/@Depth").Value, CultureInfo.InvariantCulture)
            };
            switch (e.SelectSingleNode("./Lavatory/@Door-location").Value)
            {
                case "FROM-LONGITUDINAL-AISLE":
                    a += Math.PI * 0.5;
                    double d = v.X;
                    v.X = v.Y;
                    v.Y = d;
                    switch (e.GetAttribute("Column"))
                    {
                        case "LH":
                            //p.Y -= v.Y;
                            //p.X += v.X;
                            break;

                        case "RH":
                        case "CTR":
                            //p.Y += v.Y;
                            a += Math.PI;
                            break;
                    }
                    break;

                default:
                    switch (e.SelectSingleNode("./@Position-at-door").Value)
                    {
                        case "FWD":
                            //p.Y -= v.Y;
                            //p.X += v.X;
                            break;

                        default:
                            a += Math.PI;
                            break;
                    }
                    break;
            }

            //

            //
            double n = 1;
            if (a == 0)
            {
            }
            else if (a == Math.PI * 0.5)
            {
                p.X += v.X;
                p.Y -= v.Y;
            }
            else if (a == Math.PI)
            {
                p.X += v.Y;
                //p.Y -= v.Y;
            }
            else if (a == 3 * Math.PI * 0.5)
            {
                p.Y += v.Y;
            }
            p /= 1000;
            v /= 100;
            v.X /= n;
            //
            for (int i = 0; i < n; i++)
            {
                Mtx4 m = Mtx4.CreateScale(v.Y, v.X, 15) * Mtx4.CreateTranslation(new Vec4(0, 0 * i * v.X * 0.1, 0, 0)) * Mtx4.CreateRotationZ(a);
                m.Row3 += p;
                //
                TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Lavatory").Child;
                TLink l = new TLink
                {
                    FileName = Fi.FullName,
                    NodeName = i.ToString(CultureInfo.InvariantCulture),
                    Child = s,
                    Matrix = m
                }; OwnerLink.FindFromNodeName("Lavatory").Links.Push(l);
                //
            }
        }

        private void AddLavatory2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Lavatory").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("Lavatory").Links.Push(l);
        }

        private void AddOxy1(XmlElement e)
        {
            //
            Vec4 p = new Vec4
            {
                //
                X = double.Parse(e.GetAttribute("x"), CultureInfo.InvariantCulture),
                Y = -double.Parse(e.GetAttribute("y"), CultureInfo.InvariantCulture) - 50
            };
            //
            p /= 1000;
            //
            TLink link = new TLink
            {
                Child = new TAssemblyElement(),
                ParentLink = OwnerLink.FindFromNodeName("Masks"),
                Matrix = Mtx4.Identity,
                NodeName = e.GetAttribute("w")
            };
            //
            Mtx4 m = Mtx4.CreateTranslation(p.X, p.Y, 2);
            TLink l = TManager.AttachElmt(TManager.View.OwnerLink.FindFromPartNumber("OXY"), link, null);
            l.NodeName = e.GetAttribute("w");
            l.Matrix = m;
            //
            link.ForeColor = Mtx4.CreateStaticColor(1, 1, 0);
            link.TextRanges.Add(new TTextRange()
            {
                Matrix = YXPlan * m * Mtx4.CreateTranslation(0.1, +0.2, 0),
                Text = e.GetAttribute("k"),
                Size = Mtx4.CreateScale(0.06),
                LineWidth = 30
            });
            link.TextRanges.Add(new TTextRange()
            {
                Matrix = YXPlan * m * Mtx4.CreateTranslation(-0.1, +0.2, 0),
                Text = e.GetAttribute("w"),
                Size = Mtx4.CreateScale(0.03),
                LineWidth = 30
            });
        }

        private void AddPaxSeat1(XmlElement e)
        {
            //
            Vec4 p = new Vec4();
            double a = 0;
            //
            p.X = double.Parse(e.GetAttribute("X-Position"), CultureInfo.InvariantCulture);
            p.Y = -double.Parse(e.GetAttribute("Y-Position"), CultureInfo.InvariantCulture);
            Vec2 v = new Vec2
            {
                X = double.Parse(e.SelectSingleNode("./PassengerSeat/@Width").Value, CultureInfo.InvariantCulture),
                Y = double.Parse(e.SelectSingleNode("./PassengerSeat/@Depth").Value, CultureInfo.InvariantCulture)
            };
            a = double.Parse(e.SelectSingleNode("./PassengerSeat/@Angle").Value, CultureInfo.InvariantCulture);
            switch (e.GetAttribute("Column"))
            {
                case "LH":
                    p.Y -= v.X;
                    break;

                case "RH":
                case "CTR":
                    break;
            }
            //
            p /= 1000;
            v /= 100;
            //
            string[] tbl;
            if (e.SelectSingleNode("./PassengerSeat/@Row-number") == null)
                tbl = new string[] { "?" };
            else
                tbl = e.SelectSingleNode("./PassengerSeat/@Row-number").Value.ToString(CultureInfo.InvariantCulture).Split(new char[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            v.X /= tbl.Length;
            //
            Mtx4 Sc = Mtx4.CreateScale(v.Y, v.X, 12);
            for (int i = 0; i < tbl.Length; i++)
            {
                TLink link = new TLink
                {
                    Child = new TAssemblyElement(),
                    ParentLink = OwnerLink.FindFromNodeName("PinSeat"),
                    Matrix = Mtx4.Identity,
                    NodeName = e.SelectSingleNode("./PassengerSeat/@Seat-number").Value.ToString(CultureInfo.InvariantCulture) + tbl[i]
                };
                //
                Mtx4 m = Sc * Mtx4.CreateTranslation(new Vec4(0, i * v.X * 0.1, 0, 0)) * Mtx4.CreateRotationZ(a);
                m.Row3 += p;
                if (a == Math.PI)
                    m.Row3 += Vec4.Transform(new Vec4(-0.1, -0.1, 0, 0), m);
                TLink l = TManager.AttachElmt(TManager.View.OwnerLink.FindFromPartNumber("Seat"), link, null);
                l.NodeName = "Seat";
                l.Matrix = m;
                //
                m = Mtx4.CreateTranslation(new Vec4(0, i * v.X * 0.1, 0, 0)) * Mtx4.CreateRotationZ(a);
                m.Row3 += p;
                Vec4 ox = Vec4.Transform(new Vec4(0.06, 0, 0, 0), m);
                Vec4 oy = Vec4.Transform(new Vec4(0, 0.05, 0, 0), m);
                m.Row3 -= 3 * oy;
                if (a == Math.PI)
                    m.Row3 += Vec4.Transform(new Vec4(-0.06, -0.05, 0.05, 0), Sc * m);
                else
                    m.Row3 += Vec4.Transform(new Vec4(0.05, 0.05, 0.05, 0), Sc * m);
                //
                link.ForeColor = Mtx4.CreateStaticColor(1, 1, 1);
                link.TextRanges.Add(new TTextRange()
                {
                    Matrix = YZPlan * m * Mtx4.CreateTranslation(0, 0, 0.2) * Mtx4.CreateTranslation(3 * oy),
                    Text = link.NodeName,
                    Size = Mtx4.CreateScale(0.03),
                    LineWidth = 15
                });
                link.TextRanges.Add(new TTextRange()
                {
                    Matrix = YXPlan * m * Mtx4.CreateTranslation(0, 0, -0.1) * Mtx4.CreateTranslation(6 * oy) * Mtx4.CreateTranslation(-ox),
                    Text = link.NodeName,
                    Size = Mtx4.CreateScale(0.03),
                    LineWidth = 15
                });
                //
                m = SymbolsMatrix * m;
                //
                foreach (string s in PinItems)
                {
                    m.Row3.Y += 0.05;
                    l = TManager.AttachElmt(TManager.View.OwnerLink.FindFromPartNumber(s), link, null);
                    l.NodeName = link.NodeName + " " + l.PartNumber;
                    l.Matrix = m;
                    if (DateTime.Now.Ticks % 3 == 0) l.State = ELinkState.InError;
                }
                //
            }
        }

        private void AddPaxSeat2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Seat").Child;
            int n = e.SelectNodes("./Children/SingleSeat").Count;
            Mtx4 m = GetMatrix2(e, n);
            Vec4 v = GetOffset(m);
            for (int i = 0; i < n; i++)
            {
                TLink l = new TLink
                {
                    FileName = Fi.FullName,
                    NodeName = e.GetAttribute("Name"),
                    Child = s,
                    Matrix = m
                }; OwnerLink.FindFromNodeName("PinSeat").Links.Push(l);
                m.Row3 += v;
            }
        }

        private void AddSeatRail2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("SeatRail").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("SeatRail").Links.Push(l);
        }

        private void AddStowage1(XmlElement e)
        {
            Vec4 p = new Vec4();
            //
            double a = 0;
            p.X = double.Parse(e.GetAttribute("X-Position"), CultureInfo.InvariantCulture);
            p.Y = -double.Parse(e.GetAttribute("Y-Position"), CultureInfo.InvariantCulture);
            Vec2 v = new Vec2
            {
                X = double.Parse(e.SelectSingleNode("./Stowage/@Width").Value, CultureInfo.InvariantCulture),
                Y = double.Parse(e.SelectSingleNode("./Stowage/@Depth").Value, CultureInfo.InvariantCulture)
            };
            //switch (e.SelectSingleNode("./Stowage/@Door-location").Value)
            //{
            //    case "FROM-LONGITUDINAL-AISLE":
            //        a -= Math.PI * 0.5;
            //        Double d = v.X;
            //        v.X = v.Y;
            //        v.Y = d;
            //        break;
            //}

            switch (e.GetAttribute("Column"))
            {
                case "LH":
                    p.Y -= v.X;
                    break;

                case "RH":
                case "CTR":
                    //a += Math.PI;
                    break;
            }
            //
            p /= 1000;
            v /= 100;
            //
            //switch (e.SelectSingleNode("./Stowage/@Type").Value)
            //{
            //    case "LONGITUDINAL":
            //        Double d = v.X;
            //        v.X = v.Y;
            //        v.Y = d;
            //        break;
            //}
            //switch (e.SelectSingleNode("./Stowage/@Trolley-orientation").Value)
            //{
            //    case "LH":
            //    case "FWD":
            //        a += Math.PI;
            //        break;
            //}
            //
            double n = 1;
            v.X /= n;
            //
            for (int i = 0; i < n; i++)
            {
                Mtx4 m = Mtx4.CreateScale(v.Y, v.X, 15) * Mtx4.CreateTranslation(new Vec4(0, i * v.X * 0.1, 0, 0)) * Mtx4.CreateRotationZ(a);
                m.Row3 += p; TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Stowage").Child;
                TLink l = new TLink
                {
                    FileName = Fi.FullName,
                    NodeName = i.ToString(CultureInfo.InvariantCulture),
                    Child = s,
                    Matrix = m
                }; OwnerLink.FindFromNodeName("Stowage").Links.Push(l);
            }
        }

        private void AddStowage2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Stowage").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("Stowage").Links.Push(l);
        }

        private void AddWindow2(XmlElement e)
        {
            TSolidElement s = (TSolidElement)TManager.View.OwnerLink.FindFromPartNumber("Window").Child;
            TLink l = new TLink
            {
                FileName = Fi.FullName,
                NodeName = e.GetAttribute("Name"),
                Child = s,
                Matrix = GetMatrix2(e, 1)
            }; OwnerLink.FindFromNodeName("Window").Links.Push(l);
        }

        private void LoadCabin1()
        {
            AddCabinGroups();
            foreach (XmlElement e in doc.SelectNodes("//Epac-Tdu"))
            {
                switch (e.GetAttribute("Category"))
                {
                    case "CA-SEAT":
                        AddCAS1(e);
                        break;

                    case "P-SEAT":
                        AddPaxSeat1(e);
                        break;

                    case "GALLEY":
                        AddGalley1(e);
                        break;

                    case "LAVATORY":
                        AddLavatory1(e);
                        break;

                    case "STOWAGE":
                        AddStowage1(e);
                        break;
                }
            }
        }

        private void LoadCabin2()
        {
            AddCabinGroups();
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//Properties/.."))
            //    AddLavatory2(e);

            foreach (XmlElement e in doc.SelectNodes("//Lavatory"))
                AddLavatory2(e);
            foreach (XmlElement e in doc.SelectNodes("//SeatRail"))
                AddSeatRail2(e);
            foreach (XmlElement e in doc.SelectNodes("//PaxSeat"))
                AddPaxSeat2(e);
            foreach (XmlElement e in doc.SelectNodes("//Galley"))
                AddGalley2(e);
            foreach (XmlElement e in doc.SelectNodes("//Window"))
                AddWindow2(e);
            foreach (XmlElement e in doc.SelectNodes("//CAS"))
                AddCAS2(e);
            foreach (XmlElement e in doc.SelectNodes("//Stowage"))
                AddStowage2(e);
        }

        private void LoadElec1()
        {
            AddElecGroups();
            foreach (XmlElement e in doc.SelectNodes("//BRANCHES/BRA"))
                AddBranche1(e);
            foreach (XmlElement e in doc.SelectNodes("//COMPONENTS/EXT"))
                AddExtremity1(e);

            //
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//COMPONENTS/EXT"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddExtremity1(e);
            //}
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//NODES/DER"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddDerivation1(e);
            //}
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//NODES/REN"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddXRef1(e);
            //}
            //doc.Save("c:\\temp\\ " + Fi.Name);
        }

        private void LoadOxy1()
        {
            AddOxyGroups();
            foreach (XmlElement e in doc.SelectNodes("/OXYs/OXY"))
                AddOxy1(e);

            //
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//COMPONENTS/EXT"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddExtremity1(e);
            //}
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//NODES/DER"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddDerivation1(e);
            //}
            //foreach (System.Xml.XmlElement e in doc.SelectNodes("//NODES/REN"))
            //{
            //    e.SetAttribute("ID", PartNumber + "." + e.GetAttribute("ID"));
            //    AddXRef1(e);
            //}
            //doc.Save("c:\\temp\\ " + Fi.Name);
        }

        private void LoadWiring()
        {
            foreach (XmlElement e in doc.SelectNodes("/WIRING_STRUCTURE/PRD"))
                ParseWiringPrd(e, OwnerLink);
        }

        private void ParseWiringPrd(XmlElement iPrd, TLink iOwnerLink)
        {
            Mtx4 m = Mtx4.Identity;
            m.Row0.X = double.Parse(iPrd.Attributes.GetNamedItem("Xx").Value, CultureInfo.InvariantCulture);
            m.Row0.Y = double.Parse(iPrd.Attributes.GetNamedItem("Xy").Value, CultureInfo.InvariantCulture);
            m.Row0.Z = double.Parse(iPrd.Attributes.GetNamedItem("Xz").Value, CultureInfo.InvariantCulture);
            m.Row1.X = double.Parse(iPrd.Attributes.GetNamedItem("Yx").Value, CultureInfo.InvariantCulture);
            m.Row1.Y = double.Parse(iPrd.Attributes.GetNamedItem("Yy").Value, CultureInfo.InvariantCulture);
            m.Row1.Z = double.Parse(iPrd.Attributes.GetNamedItem("Yz").Value, CultureInfo.InvariantCulture);
            m.Row2.X = double.Parse(iPrd.Attributes.GetNamedItem("Zx").Value, CultureInfo.InvariantCulture);
            m.Row2.Y = double.Parse(iPrd.Attributes.GetNamedItem("Zy").Value, CultureInfo.InvariantCulture);
            m.Row2.Z = double.Parse(iPrd.Attributes.GetNamedItem("Zz").Value, CultureInfo.InvariantCulture);
            m.Row3.X = 0.001 * double.Parse(iPrd.Attributes.GetNamedItem("Ox").Value, CultureInfo.InvariantCulture);
            m.Row3.Y = 0.001 * double.Parse(iPrd.Attributes.GetNamedItem("Oy").Value, CultureInfo.InvariantCulture);
            m.Row3.Z = 0.001 * double.Parse(iPrd.Attributes.GetNamedItem("Oz").Value, CultureInfo.InvariantCulture);

            string pn = iPrd.GetAttribute("pn");
            TLink plink = new TLink
            {
                ParentLink = iOwnerLink,
                Matrix = m,
                NodeName = iPrd.GetAttribute("nodename").Replace('|', ' ')
            };
            if (iPrd.ChildNodes.Count > 0)
            {
                plink.Child = new TAssemblyElement() { PartNumber = pn };
                foreach (XmlElement e in iPrd.SelectNodes("./PRD"))
                    ParseWiringPrd(e, plink);
                foreach (XmlElement e in iPrd.SelectNodes("./CRV"))
                {
                    TWire w = new TWire(pn + "  - " + plink.Links.Max.ToString(), e.SelectNodes("./PT"));
                    while (w.State < EElementState.Published) Thread.Sleep(10);
                    TLink link = new TLink
                    {
                        ParentLink = plink,
                        Matrix = Mtx4.Identity,
                        NodeName = w.PartNumber,
                        Child = w
                    };
                }
            }
            else if (plink.NodeName.StartsWith("V"))
                plink.Child = new TAssemblyElement() { PartNumber = pn };
            else if (plink.NodeName.ToLower().Contains("backshell"))
                plink.Child = new TAssemblyElement() { PartNumber = pn };
            else if (plink.NodeName.ToLower().Contains("dummy"))
                plink.Child = new TAssemblyElement() { PartNumber = pn };
            else
                plink.Child = new TBox(plink.NodeName + " - " + pn);
        }

        #endregion Private Methods
    }
}