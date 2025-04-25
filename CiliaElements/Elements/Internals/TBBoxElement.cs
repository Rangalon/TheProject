using OpenTK;
using System;
namespace CiliaElements
{
    public class TBBoxElement : TInternal
    {
        #region "Shared Public Variables"

        public static Single sMax = 1 - sMin;
        public static Single sMin = -0.1F;

        #endregion

        #region "Public Functions"

        
        public override void LaunchLoad()
        {
            TCloud Positions = AddCloud();
            Positions.Vectors = new Vec3[8] { new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax), new Vec3(sMax, sMax, sMax) };
            //
            TTexture Texture = AddTexture();
            Texture.Color = new Vec4f(1, 1, 0, 1);
            //
            TShapeGroup Faces = AddLGroup();
            Faces.PositionsId = Positions.Id;
            Faces.TextureId = Texture.Id;
            //
            StartGroup.SizeGroupsIds(1);
            StartGroup.AddGroupId(Faces.Id);
            //
            Faces.Indexes = new Int32[20] { 0, 1, 3, 2, -1, 5, 4, 6, 7, -1, 4, 0, 2, 6, -1, 1, 5, 7, 3, -1 };
            //
            Publish();
            //
        }
        
        
        public TBBoxElement()
            : base("Box")
        {
        }
        

        #endregion

    }







}