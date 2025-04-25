
using Math3D;
using System.Collections.Generic;

namespace CiliaElements
{
    public class TGround : TInternal
    {
        #region Public Fields

        public const int GroundStep = 10;

        #endregion Public Fields

        #region Public Constructors

        //
        public TGround()
            : base("Ground")
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void LaunchLoad()
        {
            TCloud Positions;
            List<Vec3> lst;
            int n;
            TTexture Texture = SolidElementConstruction.AddTexture(0.25F, 1, 1, 1);
            // ----------------------------------------------------------

            Positions = SolidElementConstruction.AddCloud();
            lst = new List<Vec3>();
            for (int i = -Infinity; i <= Infinity; i += GroundStep)
            {
                foreach (int j in new int[2] { Infinity, -Infinity })
                {
                    lst.Add(new Vec3(i, j, 0));
                    lst.Add(new Vec3(j, i, 0));
                }
            }
            Positions.Vectors.Push(lst.ToArray());
            //

            //
            TLGroup Lines = SolidElementConstruction.AddLGroup();
            Lines.ShapeGroupParameters.Positions = Positions;
            Lines.GroupParameters.Texture = Texture;
            //
            SolidElementConstruction.StartGroup.LGroups.Push(Lines);
            n = Infinity * 2 / GroundStep;
            //Array.Resize(ref Lines.Indexes, (n + 1) * 6);
            for (int i = 0; i <= n; i++)
            {
                Lines.Indexes.Push(i * 4);
                Lines.Indexes.Push(i * 4 + 2);
                Lines.Indexes.Push(-1);
                Lines.Indexes.Push(i * 4 + 1);
                Lines.Indexes.Push(i * 4 + 3);
                Lines.Indexes.Push(-1);
            }
            // ----------------------------------------------------------
            //Positions = SolidElementConstruction.AddCloud();
            //lst = new List<Vec3>();
            //for (Int32 i = -Infinity; i <= Infinity; i += GroundStep)
            //{
            //    for (Int32 j = -Infinity; j <= Infinity; j += GroundStep)
            //    {
            //        lst.Add(new Vec3(i, j, 0));
            //    }
            //}
            //Positions.Vectors.Push(lst.ToArray());

            //TPGroup Points = SolidElementConstruction.AddPGroup();
            //Points.ShapeGroupParameters.Positions = Positions;
            //Points.GroupParameters.Texture = Texture;
            ////
            //SolidElementConstruction.StartGroup.PGroups.Push(Points);

            //n = (Infinity / GroundStep + 1) * (Infinity / GroundStep + 1);
            //for (Int32 i = 0; i <= n; i++)
            //{
            //    Points.Indexes.Push(i * 4);
            //    Points.Indexes.Push(i * 4 + 2);
            //    Points.Indexes.Push(-1);
            //    Points.Indexes.Push(i * 4 + 1);
            //    Points.Indexes.Push(i * 4 + 3);
            //    Points.Indexes.Push(-1);
            //}
            //
            // ----------------------------------------------------------

            //
            ElementLoader.Publish();
            //
        }

        #endregion Public Methods
    }
}