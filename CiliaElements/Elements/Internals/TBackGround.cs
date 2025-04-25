


using System;
using OpenTK;

namespace CiliaElements
{
    public class TBackGround : TInternal
    {


        public TBackGround(Vector4d iColor, String iPrefixe)
            : base(iPrefixe + "BackGround")
        {
            TTexture Texture = AddTexture();
            Texture.Color = iColor;
        }
        public override void LaunchLoad()
        {
            TCloud Positions = AddCloud();
            Array.Resize(ref Positions.Vectors, 4);

            Positions.Vectors[0] = (Vector3)new Vector3d(TLetter.LetterGap / 2, -TLetter.LetterGap / 2, 0.1);
            Positions.Vectors[1] = (Vector3)new Vector3d(-TLetter.LetterWidth - TLetter.LetterGap / 2, -TLetter.LetterGap / 2, 0.1);
            Positions.Vectors[2] = (Vector3)new Vector3d(TLetter.LetterGap / 2, TLetter.LetterHeigth + TLetter.LetterGap / 2, 0.1);
            Positions.Vectors[3] = (Vector3)new Vector3d(-TLetter.LetterWidth - TLetter.LetterGap / 2, TLetter.LetterHeigth + TLetter.LetterGap / 2, 0.1);
            //
            //
            TGroup Faces = AddGroup();
            Faces.PositionsId = Positions.Id;
            Faces.TextureId = Textures[0].Id;
            Faces.Kind = EGroupKind.F;
            StartGroup.SizeGroupsIds(1);
            StartGroup.AddGroupId(Faces.Id);
            Faces.Indexes = new Int32[6] { 0, 1, 2, 1,3, 2 };
            //
            Publish();
            //
        }
    }

    
    
    
    
    
    
}