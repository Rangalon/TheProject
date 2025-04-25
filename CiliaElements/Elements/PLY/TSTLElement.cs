
using Math3D;
using OpenTK;
using System;
using System.IO;

namespace CiliaElements.FormatSTL
{
    public class TStlElement : TSolidElement
    {
        #region Private Fields

        private Mtx4 StartMatrix = Mtx4.CreateScale(1);

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            BinaryReader rdr;
            //

            //
            rdr = new BinaryReader(Fi.OpenRead());// new StreamReader(Fi.FullName);
            for (int i = 0; i < 80; i++) rdr.ReadByte();
            uint FacetsCount = rdr.ReadUInt32();
            TFGroup fgroup = SolidElementConstruction.AddFGroup();
            TCloud ps = SolidElementConstruction.AddCloud();
            TCloud ns = SolidElementConstruction.AddCloud();
            TTexture t = SolidElementConstruction.AddTexture(1, 0.9F, 0.9F, 0.9F);
            fgroup.GroupParameters.Texture = t;
            fgroup.ShapeGroupParameters.Positions = ps;
            fgroup.ShapeGroupParameters.Normals = ns;
            SolidElementConstruction.StartGroup.FGroups.Push(fgroup);

            int cnt = 0;
            int pcnt = 0;
            while (cnt < FacetsCount)
            {
                Vec3 n = new Vec3
                {
                    X = rdr.ReadSingle(),
                    Y = rdr.ReadSingle(),
                    Z = rdr.ReadSingle()
                };
                ns.Vectors.Push(n);
                ns.Vectors.Push(n);
                ns.Vectors.Push(n);
                n.X = rdr.ReadSingle();
                n.Y = rdr.ReadSingle();
                n.Z = rdr.ReadSingle();
                ps.Vectors.Push(n);
                n.X = rdr.ReadSingle();
                n.Y = rdr.ReadSingle();
                n.Z = rdr.ReadSingle();
                ps.Vectors.Push(n);
                n.X = rdr.ReadSingle();
                n.Y = rdr.ReadSingle();
                n.Z = rdr.ReadSingle();
                ps.Vectors.Push(n);
                fgroup.Indexes.Push(pcnt); pcnt++;
                fgroup.Indexes.Push(pcnt); pcnt++;
                fgroup.Indexes.Push(pcnt); pcnt++;
                rdr.ReadUInt16();
                cnt++;
            }

            rdr.Close();
            rdr.Dispose();

            SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}