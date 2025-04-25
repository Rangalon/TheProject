



using System;
using System.Collections.Generic;
using OpenTK;

namespace CiliaElements
{

    public class TSymbol : TInternal
    {
        public TSymbol(System.Xml.XmlElement iElmt, String iName)
            : base(iName)
        {
            //
            TGroup group;
            //
            System.Xml.XmlElement Positions = (System.Xml.XmlElement)iElmt.SelectSingleNode("./POSITIONS");
            String[] tbl = Positions.InnerText.Split(';');
            TCloud cloud = AddCloud();
            cloud.Vectors = new Vector3[(tbl.Length - 1) / 3];
            for (int i = 0; i < tbl.Length - 1; i += 3)
            {
                Vector3d v;
                v.X = double.Parse(tbl[i]);
                v.Y = double.Parse(tbl[i + 1]);
                v.Z = double.Parse(tbl[i + 2]);
                cloud.Vectors[i / 3] = (Vector3)v;
            }
            //
            foreach (System.Xml.XmlElement LinesIndices in iElmt.SelectNodes("./LINESINDICES"))
            {
                tbl = LinesIndices.InnerText.Split(';');
                group = AddGroup();
                group.Indexes = new Int32[tbl.Length - 1];
                group.Kind = EGroupKind.L;
                for (int i = 0; i < tbl.Length - 1; i++) group.Indexes[i] = Int32.Parse(tbl[i]);
                //
                TTexture texture = AddTexture();
                texture.X = double.Parse(LinesIndices.GetAttribute("r"));
                texture.Y = double.Parse(LinesIndices.GetAttribute("g"));
                texture.Z = double.Parse(LinesIndices.GetAttribute("b"));
                texture.W = 1;
                //
                group.PositionsId = cloud.Id;
                group.TextureId = texture.Id;
                StartGroup.ExtendGroupsIds();
                StartGroup.AddGroupId(group.Id);
            }

            //
            foreach (System.Xml.XmlElement PointsIndices in iElmt.SelectNodes("./POINTSINDICES"))
            {
                tbl = PointsIndices.InnerText.Split(';');
                group = AddGroup();
                group.Indexes = new Int32[tbl.Length - 1];
                group.Kind = EGroupKind.P;
                TCloud pcloud = AddCloud();
                Array.Resize(ref pcloud.Vectors, tbl.Length - 1);
                for (int i = 0; i < tbl.Length - 1; i++)
                {
                    group.Indexes[i] = Int32.Parse(tbl[i]);
                    pcloud.Vectors[i] = cloud.Vectors[group.Indexes[i]];
                }
                //
                TTexture texture = AddTexture();
                texture.X = double.Parse(PointsIndices.GetAttribute("r"));
                texture.Y = double.Parse(PointsIndices.GetAttribute("g"));
                texture.Z = double.Parse(PointsIndices.GetAttribute("b"));
                texture.W = 1;
                //
                group.PositionsId = pcloud.Id;
                group.TextureId = texture.Id;
                StartGroup.ExtendGroupsIds();
                StartGroup.AddGroupId(group.Id);
            }
        }

        public override void LaunchLoad()
        {
            //

            Publish();
        }

    }
}
