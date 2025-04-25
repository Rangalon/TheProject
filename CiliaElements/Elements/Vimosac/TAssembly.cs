using OpenTK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatMDL
{
    public class TModel : TAssemblyElement
    {
        #region Private Fields

        private Mtx4 StartMatrix = Mtx4.CreateScale(1);

        #endregion Private Fields

        #region Public Methods

        public override void LaunchLoad()
        {
            StreamReader rdr;
            //

            //
            //

            char[] sps = new char[] { ' ' };
            rdr = new StreamReader(Fi.OpenRead());// new StreamReader(Fi.FullName);

            while (!rdr.EndOfStream)
            {
                string[] wds = rdr.ReadLine().Split(sps, StringSplitOptions.RemoveEmptyEntries);
                switch (wds[0])
                {
                    case "v": break;
                    case "f": break;
                    case "g": break;
                    default:
                        break;
                }
            }

            rdr.Close();
            rdr.Dispose();

            //uint pcount = 0;
            //uint fcount = 0;

            //string l = rdr.ReadLine();
            //while (l != "end_header")
            //{
            //    string[] ws = l.Split();
            //    switch (ws[0])
            //    {
            //        case "element":
            //            switch (ws[1])
            //            {
            //                case "vertex": pcount = uint.Parse(ws[2]); break;
            //                case "face": fcount = uint.Parse(ws[2]); break;
            //            }
            //            break;
            //    }
            //    l = rdr.ReadLine();
            //}

            //TFGroup fgroup = SolidElementConstruction.AddFGroup();
            //TCloud ps = SolidElementConstruction.AddCloud();
            //TCloud ns = SolidElementConstruction.AddCloud();
            //TTexture t = SolidElementConstruction.AddTexture(1, 0.9F, 0.9F, 0.9F);
            //fgroup.GroupParameters.Texture = t;
            //fgroup.ShapeGroupParameters.Positions = ps;
            //fgroup.ShapeGroupParameters.Normals = ns;
            //SolidElementConstruction.StartGroup.FGroups.Push(fgroup);

            //for (int i = 0; i < pcount; i++)
            //{
            //    string[] ws = rdr.ReadLine().Split();
            //    Vec3 v = new Vec3
            //    {
            //        X = float.Parse(ws[0], CultureInfo.InvariantCulture),
            //        Y = float.Parse(ws[1], CultureInfo.InvariantCulture),
            //        Z = float.Parse(ws[2], CultureInfo.InvariantCulture)
            //    };
            //    ps.Vectors.Push(v);
            //    v.X = float.Parse(ws[3], CultureInfo.InvariantCulture);
            //    v.Y = float.Parse(ws[4], CultureInfo.InvariantCulture);
            //    v.Z = float.Parse(ws[5], CultureInfo.InvariantCulture);
            //    ns.Vectors.Push(v);
            //}

            //for (int i = 0; i < fcount; i++)
            //{
            //    string[] ws = rdr.ReadLine().Split();
            //    fgroup.Indexes.Push(int.Parse(ws[1]));
            //    fgroup.Indexes.Push(int.Parse(ws[2]));
            //    fgroup.Indexes.Push(int.Parse(ws[3]));
            //}

            //rdr.Close();
            //rdr.Dispose();

            //SolidElementConstruction.StartGroup.GroupParameters.Matrix = StartMatrix;

            ElementLoader.Publish();
        }

        #endregion Public Methods
    }
}