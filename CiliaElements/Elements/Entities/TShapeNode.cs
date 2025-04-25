using System;

namespace CiliaElements
{
    public class TShapeNode
    {
        #region Public Fields

        //public TBoundingBox BoundingBox = new TBoundingBox();
        public TShapeNode[] FaceNodes;
        public TShapeNode[] GroupNodes;

        //public TShapeNode[] Nodes = { };
        public TShapeNode[] LineNodes;

        public TShapeNode[] PointNodes;
        public TShape Shape;
        public TShapeNodeParameters ShapeNodeParameters = new TShapeNodeParameters();

        #endregion Public Fields

        #region Public Methods

        //public void UpdateFace()
        //{
        //    //if (Shape.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = Shape.PositionsStart;
        //    //if (Shape.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = Shape.PositionsEnd;
        //    //if (Shape.FacesStart < ShapeNodeParameters.FacesStart) ShapeNodeParameters.FacesStart = Shape.FacesStart;
        //    //if (Shape.FacesEnd > ShapeNodeParameters.FacesEnd) ShapeNodeParameters.FacesEnd = Shape.FacesEnd;
        //    //
        //    BoundingBox.AddBox(Shape.BoundingBox);
        //}

        //public void UpdateGroup()
        //{
        //    for (Int32 i = 0; i < GroupNodes.Length; i++)
        //    {
        //        TShapeNode node = GroupNodes[i]; node.UpdateGroup();
        //        //
        //        //if (node.ShapeNodeParameters.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = node.ShapeNodeParameters.PositionsStart;
        //        //if (node.ShapeNodeParameters.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = node.ShapeNodeParameters.PositionsEnd;
        //        //if (node.ShapeNodeParameters.FacesStart < ShapeNodeParameters.FacesStart) ShapeNodeParameters.FacesStart = node.ShapeNodeParameters.FacesStart;
        //        //if (node.ShapeNodeParameters.FacesEnd > ShapeNodeParameters.FacesEnd) ShapeNodeParameters.FacesEnd = node.ShapeNodeParameters.FacesEnd;
        //        //
        //        BoundingBox.AddBox(node.BoundingBox);
        //        node.ShapeNodeParameters = null;
        //    }
        //    for (Int32 i = 0; i < ShapeNodeParameters.FacesQty; i++)
        //    {
        //        TShapeNode node = FaceNodes[i]; node.UpdateFace();
        //        //
        //        //if (node.ShapeNodeParameters.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = node.ShapeNodeParameters.PositionsStart;
        //        //if (node.ShapeNodeParameters.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = node.ShapeNodeParameters.PositionsEnd;
        //        //if (node.ShapeNodeParameters.FacesStart < ShapeNodeParameters.FacesStart) ShapeNodeParameters.FacesStart = node.ShapeNodeParameters.FacesStart;
        //        //if (node.ShapeNodeParameters.FacesEnd > ShapeNodeParameters.FacesEnd) ShapeNodeParameters.FacesEnd = node.ShapeNodeParameters.FacesEnd;
        //        //
        //        BoundingBox.AddBox(node.BoundingBox);
        //        node.ShapeNodeParameters = null;
        //    }
        //    for (Int32 i = 0; i < ShapeNodeParameters.LinesQty; i++)
        //    {
        //        TShapeNode node = LineNodes[i]; node.UpdateLine();
        //        //
        //        //if (node.ShapeNodeParameters.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = node.ShapeNodeParameters.PositionsStart;
        //        //if (node.ShapeNodeParameters.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = node.ShapeNodeParameters.PositionsEnd;
        //        //
        //        BoundingBox.AddBox(node.BoundingBox);
        //        node.ShapeNodeParameters = null;
        //    }
        //    for (Int32 i = 0; i < ShapeNodeParameters.PointsQty; i++)
        //    {
        //        TShapeNode node = PointNodes[i]; node.UpdatePoint();
        //        //
        //        //if (node.ShapeNodeParameters.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = node.ShapeNodeParameters.PositionsStart;
        //        //if (node.ShapeNodeParameters.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = node.ShapeNodeParameters.PositionsEnd;
        //        //
        //        BoundingBox.AddBox(node.BoundingBox);
        //        node.ShapeNodeParameters = null;
        //    }
        //}

        //public void UpdateLine()
        //{
        //    //if (Shape.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = Shape.PositionsStart;
        //    //if (Shape.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = Shape.PositionsEnd;
        //    //
        //    BoundingBox.AddBox(Shape.BoundingBox);
        //}

        //public void UpdatePoint()
        //{
        //    //if (Shape.PositionsStart < ShapeNodeParameters.PositionsStart) ShapeNodeParameters.PositionsStart = Shape.PositionsStart;
        //    //if (Shape.PositionsEnd > ShapeNodeParameters.PositionsEnd) ShapeNodeParameters.PositionsEnd = Shape.PositionsEnd;
        //    //
        //    BoundingBox.AddBox(Shape.BoundingBox);
        //}

        #endregion Public Methods
    }

    public class TShapeNodeParameters
    {
        #region Public Fields

        public Int32 FacesQty;
        public Int32 GroupsQty;
        public Int32 LinesQty;
        public Int32 PointsQty;

        #endregion Public Fields

        //public Int32 PositionsEnd = Int32.MinValue;
        //public Int32 PositionsStart = Int32.MinValue;
        //public Int32 FacesEnd = Int32.MinValue;
        //public Int32 FacesStart = Int32.MinValue;
    }
}