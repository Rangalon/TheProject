
using System;
using System.Collections.Generic;
using OpenTK;

namespace CiliaElements
{
    public class TStateBox : TInternal
    {
        #region "Shared Public Variables"

        public static Single sMax = 0.5F;
        public static Single sMin = -0.5F;

        #endregion

        #region "Public Functions"

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override void LaunchLoad()
        {
            TCloud Positions = AddCloud();
            Positions.Vectors = new Vector3[8] { 
                new Vector3(sMin, sMin, sMin), 
                new Vector3(sMin, sMin, sMax), 
                new Vector3(sMin, sMax, sMin), 
                new Vector3(sMin, sMax, sMax), 
                new Vector3(sMax, sMin, sMin), 
                new Vector3(sMax, sMin, sMax), 
                new Vector3(sMax, sMax, sMin), 
                new Vector3(sMax, sMax, sMax) };
            //
            for (byte b = 7; b < 8; b++)
            {
                TTexture Texture = AddTexture();
                Texture.Color = new Vector4d(b >> 2, (b >> 1) % 2, b % 2, 1);
                //
                TGroup Faces = AddGroup();
                Faces.PositionsId = Positions.Id;
                Faces.NormalsId = Positions.Id;
                Faces.TextureId = Texture.Id;
                Faces.Kind = EGroupKind.F;
                //
                StartGroup.ExtendGroupsIds();
                StartGroup.AddGroupId(Faces.Id);
                //
                Faces.Indexes = new Int32[] { 
                    0, 1, 2, 2, 1, 3,
                    0, 4, 1, 1, 4, 5,
                    2, 3, 6, 6, 3, 7,
                    4, 6, 5, 5, 6, 7,
                    0, 2, 4, 4, 2, 6,
                    1, 5, 3, 3, 5, 7 };
            }
            //
            Publish();
            //
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public TStateBox()
            : base("StateBox")
        {
        }
        //------------------------------------------------------------------------------------------------------------------------------------------------------

        #endregion

    }
}
