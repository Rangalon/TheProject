
using CiliaElements.Elements.Control;
using CiliaElements.Utilities;
using Math3D;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace CiliaElements
{
    public static partial class TManager
    {
        #region Private Methods

        private static void AbortAction()
        {
            switch (PendingAction.PartNumber)
            {
                case "MeasurePoint": TMeasurePointAction.Abort(); break;
                case "MeasureVector": TMeasureVectorAction.Abort(); break;
                case "MeasureMinimalVector": TMeasureMinimalVectorAction.Abort(); break;
            }
        }

        private static void CenterObject(Vec3 PtToSelect)
        {
            CenterMeMode = false;
            Target = PtToSelect;

            targetPanel.DisplayedVector = Target;
            Vec4 v = Vec4.Transform(Target, VMatrix);
            v.Z = 0;
            v.W = 0;
            Mtx4 m = VMatrix;
            m.Row3 -= v;
            VMatrix = m;
            //ResetMatrixes();
        }

        static int previousMouseButtons = 0;
        static int MouseButtons;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckMouseEntries()
        {
            
            // Mouse ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Mouse = OpenTK.Input.Mouse.GetCursorState();
            MouseButtons = Mouse.Buttons;
            if (MouseButtons != previousMouseButtons)
            {
                previousMouseButtons = MouseButtons;
                ////////////////////////////////////////////////////////////
                if ((MouseButtons % 2) == 1)
                {
                    if (!LeftButton)
                    {
                        Thread.Sleep(MouseTemporisation);
                        Mouse = OpenTK.Input.Mouse.GetCursorState();
                        MouseButtons = Mouse.Buttons;
                        if ((MouseButtons % 2) == 0)
                        {
                            DoClick();
                        }
                        else
                        {
                            LeftButton = true;
                        }
                    }
                }
                else
                {
                    LeftButton = false;
                }

                MouseButtons = MouseButtons >> 1;
                ////////////////////////////////////////////////////////////
                if ((MouseButtons % 2) == 1)
                {
                    if (!MiddleButton)
                    {
                        Thread.Sleep(MouseTemporisation);
                        Mouse = OpenTK.Input.Mouse.GetCursorState();
                        MouseButtons = Mouse.Buttons;
                        MouseButtons = MouseButtons >> 1;
                        if ((MouseButtons % 2) == 0)
                        {
                            OverFlyRecall = DoCenterObject;
                        }
                        else
                        {
                            MiddleButton = true;
                            PreviousMoveMode = EMoveMode.SideTranslation;
                        }
                    }
                }
                else
                {
                    MiddleButton = false;
                }

                MouseButtons = MouseButtons >> 1;
                ////////////////////////////////////////////////////////////
                if ((MouseButtons % 2) == 1)
                {
                    if (!RightButton)
                    {
                        Thread.Sleep(MouseTemporisation);
                        Mouse = OpenTK.Input.Mouse.GetCursorState();
                        MouseButtons = Mouse.Buttons;
                        MouseButtons = MouseButtons >> 1;
                        if ((MouseButtons % 2) == 0)
                        {
                            SwitchMoveMode();
                        }
                        else
                        {
                            RightButton = true;
                        }
                    }
                }
                else
                {
                    RightButton = false;
                    if (MiddleButton)
                    {
                        SwitchMoveMode();
                    }
                }
                MouseButtons = MouseButtons >> 1;
                ////////////////////////////////////////////////////////////
            }

        }

        private static void CheckMouseDrags(OpenTK.Input.MouseState mouse, Vec2 M, double d)
        { 
            if (!LeftButton && !MiddleButton)
            {
                // Picker Mod
                if (PickerMode)
                {
                    if (OverFlownPoint.HasValue)
                    {
                        OverFlyRecall = DoCenterObject;
                    }

                    SwitchPickerMode();
                }

                // If Left button is not clicked, reset the mousedown objects
                if (MovingPoint != null)
                {
                    MovingPoint = null;
                }
                MoveMode = EMoveMode.Null;
            }
            else if (MiddleButton || LeftButton)
            {
                if (MiddleButton)
                {
                    MoveMode = PreviousMoveMode;
                    if (RightButton)
                    {
                        if (MoveMode == EMoveMode.SideTranslation)
                        {
                            MoveMode = EMoveMode.SideRotation;
                        }
                        else
                        {
                            MoveMode = EMoveMode.Rotation;
                        }
                    }
                }
                else if (LeftButton)
                {
                    // Left button is clicked
                    
                    if (MoveMode == EMoveMode.Null)
                    {
                        if (Math.Abs(M.X) > 0.9 || Math.Abs(M.Y) > 0.9)
                        {
                            if (PreviousMoveMode == EMoveMode.Translation)
                            {
                                MoveMode = EMoveMode.SideTranslation;
                            }
                            else
                            {
                                MoveMode = EMoveMode.SideRotation;
                            }
                        }
                        else
                        {
                            if (PreviousMoveMode == EMoveMode.Translation)
                            {
                                MoveMode = EMoveMode.Translation;
                            }
                            else
                            {
                                MoveMode = EMoveMode.Rotation;
                            }
                        }
                    }
                    
                }
                //
                switch (MoveMode)
                {
                    case EMoveMode.Rotation:
                        MoveSolid = RepFaceRotate;
                        break;

                    case EMoveMode.SideRotation:
                        MoveSolid = RepSideRotate;
                        break;

                    case EMoveMode.Translation:
                        MoveSolid = RepFaceTranslate;
                        break;

                    case EMoveMode.SideTranslation:
                        MoveSolid = RepSideTranslate;
                        break;
                }
                //
                Vec4 V;
                
                if (MovingPoint == null  )
                {
                    //Initialization of mousedown objects
                    //SideMode = ConvertXY(ref mouseX, ref mouseY);
                    MovingCursor = Cursor;
                    MovingPoint = CursorPoint*0.01;
                    MovingVector = MovingPoint.Value - Target;
                    //
                }
                else if (previousMouse != null  )
                {
                    // mousedown object exist, let's move
                    double dx = +2 * (mouse.Position.X - previousMouse.Value.Position.X) * IWidth;
                    double dy = -2 * (mouse.Position.Y - previousMouse.Value.Position.Y) * IHeight;
                    if (OverFlownLink != null && OverFlownLink.Child is TControl c)
                    {
                        c.MouseDrag(dx, dy);
                    }
                    else
                    {
                        switch (MoveMode)
                        {
                            case EMoveMode.SideTranslation:
                                V = Vec4.Transform(Target, VMatrix);
                                V.Z *= 1 + dy;
                                V = Vec4.Transform(V, VIMatrix);
                                Mtx4 mt5 = Mtx4.CreateTranslation(V - Target);
                                if (EntitiesMoving && SelectedLinks.Length > 0)
                                {
                                    foreach (TLink l in SelectedLinks)
                                    {
                                        l.Matrix = l.Matrix * mt5;
                                    }
                                }
                                else
                                {
                                    VMatrix = mt5 * VMatrix;
                                }

                                break;

                            case EMoveMode.Translation:
                                V = Vec4.Transform(Target, SelectedLayer.PVMatrix);
                                V.X += dx * d;
                                V.Y += dy * d;
                                V = Vec4.Transform(V, SelectedLayer.PVIMatrix);
                                //
                                Mtx4 mt2 = Mtx4.CreateTranslation(V - Target);
                                if (EntitiesMoving && SelectedLinks.Length > 0)
                                {
                                    foreach (TLink l in SelectedLinks)
                                    {
                                        l.Matrix = l.Matrix * mt2;
                                    }
                                }
                                else
                                {
                                    VMatrix = mt2 * VMatrix;
                                }

                                break;

                            case EMoveMode.SideRotation:
                                Vec3 V3 = CursorPoint - Target;
                                Vec3 V4 = MovingPoint.Value - Target;
                                Vec3 inis = Vec3.Cross(V3, V4);
                                if (inis.LengthSquared > 0)
                                {
                                    double Angle = Vec3.Dot(V3, V4);
                                    Angle /= V3.Length * V4.Length;
                                    if (Angle > 1)
                                    {
                                        Angle = 1;
                                    }
                                    else if (Angle < -1)
                                    {
                                        Angle = -1;
                                    }

                                    Angle = 0.003 * Math.Acos(Angle);

                                    if (Vec3.Dot(VMatrix.Column2, inis) > 0)
                                    {
                                        inis = -(Vec3)VMatrix.Column2;
                                    }
                                    else
                                    {
                                        inis = (Vec3)VMatrix.Column2;
                                    }

                                    Mtx4 mt3 = Mtx4.CreateFromAxisAngle(inis, MovingVector.Length * Angle);
                                    if (EntitiesMoving && SelectedLinks.Length > 0)
                                    {
                                        foreach (TLink l in SelectedLinks)
                                        {
                                            l.Matrix = l.Matrix * mt3;
                                        }
                                    }
                                    else
                                    {
                                        V3 = Vec4.TransformPoint(Target, VMatrix);
                                        VMatrix = mt3 * VMatrix;
                                        Mtx4 m = VMatrix;
                                        m.Row3 -= Vec4.TransformPoint(Target, VMatrix) - V3;
                                        VMatrix = m;
                                    }
                                }

                                break;

                            case EMoveMode.Rotation:
                                Vec3 V31 = CursorPoint; V31.Z -= SelectedLayer.NearDistance;
                                Vec3 V41 = MovingPoint.Value; V41.Z -= SelectedLayer.NearDistance;
                                Vec3 inis1 = Vec3.Cross(V31, V41);
                                if (inis1.LengthSquared > 0)
                                {
                                    double Angle = Vec3.Dot(V31, V41);
                                    Angle /= V31.Length * V41.Length;
                                    if (Angle > 1)
                                    {
                                        Angle = 1;
                                    }
                                    else if (Angle < -1)
                                    {
                                        Angle = -1;
                                    }

                                    Angle = 10 * Math.Acos(Angle) / SelectedLayer.FarDistance;
                                    if (CenterMeMode)
                                    {
                                        Angle *= -1;
                                    }

                                    Mtx4 mt3 = Mtx4.CreateFromAxisAngle(inis1, MovingVector.Length * Angle);
                                    if (EntitiesMoving && SelectedLinks.Length > 0)
                                    {
                                        foreach (TLink l in SelectedLinks)
                                        {
                                            l.Matrix = l.Matrix * mt3;
                                        }
                                    }
                                    else
                                    {
                                        V3 = Vec4.TransformPoint(Target, VMatrix);
                                        VMatrix = mt3 * VMatrix;
                                        Mtx4 m = VMatrix;
                                        m.Row3 -= Vec4.TransformPoint(Target, VMatrix) - V3;
                                        VMatrix = m;
                                    }
                                }
                                break;
                        }
                    }
                    //
                    //ResetMatrixes();
                }
                

            }
        }


        private static void CheckOverFly()
        {
            TThreadGoverner w = TThreadGoverner.Pick;
            int pickIndex = -1;

            while (true)
            {
                lock (FlyingLocker)
                {
                    if (!MovingPoint.HasValue)
                    {
                        Vec2 crs;
                        if (PickerMode)
                        {
                            Vec4 pp = new Vec4() { Z = 1, W = 1 };
                            pp = Vec4.Transform(pp, CursorMatrix);
                            pp = Vec4.Transform(pp, BaseLayer.PMatrix);
                            crs.X = pp.X / pp.W;
                            crs.Y = pp.Y / pp.W;
                        }
                        else
                        {
                            crs = FlyingPoint;
                        }
                        //

                        //
                        TLink link = null;
                        TShape NodeToSelect = null;
                        Vec3? pt = null;
                        double zMin = double.PositiveInfinity;
                        w.Reset();
                        //
                        MainRayMatrix.Row0.X = -crs.X * xRatio;
                        MainRayMatrix.Row1.X = -crs.Y * yRatio;
                        PickingStack.Reset();
                        foreach (TControl c in TControl.Controls.Values.Where(o => o != null && o.Visible && o.OwnerLink.Pickable))
                        {
                            PickingStack.Push(c.OwnerLink);
                        }

                        PickingStack.Push(View.OwnerLink);
                        //
                        TSolidElement solid;
                        TLink iLink;
                        Mtx4 M;
                        Vec4 RayVector;
                        Vec4 OVector;
                        Trg3 t = new Trg3();
                        Vec3 p = new Vec3();
                        //
                        while (PickingStack.NotEmpty)
                        {
                            iLink = PickingStack.Pop();
                            if (iLink.Solid == null)
                            {
                                PickingStack.Push(Array.FindAll(iLink.Links.Values, o => o != null && o.Enabled && !o.Ethereal && o.Pickable));
                            }
                            else
                            {
                                //
                                solid = iLink.Solid;
                                if (solid.State == EElementState.Pushed && (!(solid is TControl tc) || tc.Visible))
                                {
                                    //
                                    M = iLink.Giving.Matrix;
                                    if (solid.PointsNumber > 0)
                                    {
                                        for (int i = solid.PointsStart; i < solid.PointsStart + solid.PointsNumber; i++)
                                        {
                                            Vec4 v = Vec4.Transform(Vec4.Transform(solid.DataPositions[solid.DataIndexes[i]], M), BaseLayer.PMatrix);
                                            v /= v.W;
                                            v -= crs;
                                            if (v.X * v.X + v.Y * v.Y < 0.00025)
                                            {
                                                link = iLink;
                                                pt = Vec4.TransformPoint(solid.DataPositions[solid.DataIndexes[i]], iLink.Giving.Matrix * VIMatrix);
                                            }
                                        }
                                    }
                                    //
                                    try
                                    {
                                        M = Mtx4.Invert(M);
                                    }
                                    catch { }
                                    finally
                                    {
                                        RayVector = Vec4.Transform(MainRayMatrix.Column0, M);
                                        OVector = Vec4.Transform(MainRayMatrix.Column1, M);
                                        //
                                        RayMatrix.Row0.X = RayVector.X; RayMatrix.Row0.Y = OVector.X;
                                        RayMatrix.Row1.X = RayVector.Y; RayMatrix.Row1.Y = OVector.Y;
                                        RayMatrix.Row2.X = RayVector.Z; RayMatrix.Row2.Y = OVector.Z;
                                        //
                                        if (GetClashFromRayAndBoundingBox(iLink.Child.BoundingBox, RayMatrix))
                                        {
                                            Array.ForEach(Array.FindAll(solid.Surfaces.Values, o => GetClashFromRayAndBoundingBox(o.BoundingBox, RayMatrix)), s =>
                                            {
                                                for (int I = s.IndexesStart; I < s.IndexesEnd; I += 3)
                                                {
                                                    t.O = solid.DataPositions[solid.DataIndexes[I]];
                                                    t.X = t.O - solid.DataPositions[solid.DataIndexes[I + 1]];
                                                    t.Y = t.O - solid.DataPositions[solid.DataIndexes[I + 2]];
                                                    if (RayMatrix.CheckRay(t, ref zMin, ref p))
                                                    {
                                                        if (s.Entity != null)
                                                        {
                                                            p = s.Entity.CheckRay(RayMatrix, solid.DataIndexes[I] - s.PositionsStart, solid.DataIndexes[I + 1] - s.PositionsStart, solid.DataIndexes[I + 2] - s.PositionsStart, p);
                                                        }

                                                        link = iLink;
                                                        NodeToSelect = s;
                                                        pt = Vec4.TransformPoint(p, iLink.Giving.Matrix * VIMatrix);
                                                        pickIndex = I;
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }
                                //
                            }
                        } //
                        w.Check();
                        if (pt == null)
                        {
                            OverFlownLink = null;
                            OverFlownPoint = null;
                            OverFlownShape = null;
                            OverFlownIndex = -1;
                        }
                        else if (link.Pickable)
                        {
                            OverFlownLink = link;
                            OverFlownPoint = pt;
                            OverFlownShape = NodeToSelect;
                            OverFlownIndex = pickIndex;
                            if (overFlownLink.Solid is TControl)
                            {
                                ((TControl)overFlownLink.Solid).MouseMove(p);
                            }
                        }
                        else
                        {
                            OverFlownPoint = pt;
                        }
                    }

                    if (OverFlyRecall != null)
                    {
                        OverFlyRecall();
                        OverFlyRecall = null;
                    }
                }

                if (OverFlownLink != null && targetPanel.Visible && !(overFlownLink.Child is TControl))
                {
                    overflownPanel.Visible = true;
                    overflownPanel.DisplayedVector = OverFlownPoint.Value;
                    overflownPanel.Title = OverFlownLink.NodeName;
                }
                else
                {
                    overflownPanel.Visible = false;
                }

                Thread.Sleep(20);
            }
        }

        private static void CheckTouches(double d)
        {


            if (touch1 != null && touch2 != null)
            {
                if (previousTouch1 != null && previousTouch2 != null)
                {
                    Vec2 p1 = (touch1.Value + touch2.Value) * 0.5;
                    Vec2 pp1 = (previousTouch1.Value + previousTouch2.Value) * 0.5;
                    Vec2 v1 = touch1.Value - touch2.Value;
                    Vec2 pv1 = previousTouch1.Value - previousTouch2.Value;

                    // Pitch to zoom
                    Vec4 V = Vec4.Transform(Target, VMatrix);
                    V.Z *= 1 + (pv1.LengthSquared - v1.LengthSquared) / pv1.LengthSquared;
                    V = Vec4.Transform(V, VIMatrix);
                    Mtx4 mt5 = Mtx4.CreateTranslation(V - Target);

                    // Move to move
                    V = Vec4.Transform(Target, SelectedLayer.PVMatrix);
                    V.X -= (pp1 - p1).X * d;
                    V.Y -= (pp1 - p1).Y * d;
                    V = Vec4.Transform(V, SelectedLayer.PVIMatrix);
                    Mtx4 mt2 = Mtx4.CreateTranslation(V - Target) * mt5;

                    // Move
                    if (EntitiesMoving && SelectedLinks.Length > 0)
                    {
                        foreach (TLink l in SelectedLinks)
                        {
                            l.Matrix = l.Matrix * mt2;
                        }
                    }
                    else
                    {
                        VMatrix = mt2 * VMatrix;
                    }

                    // Move to rotate
                    double a = v1.Y * pv1.X - v1.X * pv1.Y;
                    a /= v1.Length * pv1.Length;
                    a = Math.Asin(a);

                    Mtx4 mt3 = Mtx4.CreateFromAxisAngle((Vec3)VMatrix.Column2, a);
                    if (EntitiesMoving && SelectedLinks.Length > 0)
                    {
                        foreach (TLink l in SelectedLinks)
                        {
                            l.Matrix = l.Matrix * mt3;
                        }
                    }
                    else
                    {
                        Vec3 V3 = Vec4.TransformPoint(Target, VMatrix);
                        VMatrix = mt3 * VMatrix;
                        Mtx4 m = VMatrix;
                        m.Row3 -= Vec4.TransformPoint(Target, VMatrix) - V3;
                        VMatrix = m;
                    }
                }
                else
                {
                    MovingPoint = (Touch1Point + Touch2Point) * 0.5;
                }
            }
            else if (touch1 != null)
            {
                if (previousTouch1 != null && previousTouch2 == null)
                {
                    Vec3 V31 = Touch1Point; V31.Z -= SelectedLayer.NearDistance;
                    Vec3 V41 = MovingPoint.Value; V41.Z -= SelectedLayer.NearDistance;
                    Vec3 inis1 = Vec3.Cross(V31, V41);
                    if (inis1.LengthSquared > 0)
                    {
                        double Angle = Vec3.Dot(V31, V41);
                        Angle /= V31.Length * V41.Length;
                        if (Angle > 1)
                        {
                            Angle = 1;
                        }
                        else if (Angle < -1)
                        {
                            Angle = -1;
                        }

                        Angle = 10 * Math.Acos(Angle) / SelectedLayer.FarDistance;
                        if (CenterMeMode)
                        {
                            Angle *= -1;
                        }

                        Mtx4 mt3 = Mtx4.CreateFromAxisAngle(inis1, MovingVector.Length * Angle);
                        if (EntitiesMoving && SelectedLinks.Length > 0)
                        {
                            foreach (TLink l in SelectedLinks)
                            {
                                l.Matrix = l.Matrix * mt3;
                            }
                        }
                        else
                        {
                            Vec3 V3 = Vec4.TransformPoint(Target, VMatrix);
                            VMatrix = mt3 * VMatrix;
                            Mtx4 m = VMatrix;
                            m.Row3 -= Vec4.TransformPoint(Target, VMatrix) - V3;
                            VMatrix = m;
                        }
                    }
                }
                else
                {
                    MovingPoint = Touch1Point;
                }
            }
            else if (touch2 != null)
            {
                if (previousTouch1 == null && previousTouch2 != null)
                {
                    GlyphePoints.Push(TouchMatrix);
                }
                else
                {
                    MovingPoint = Touch2Point;

                    GlyphePoints.Reset();
                }
            }
            else if (previousTouch1 != null && previousTouch2 == null)
            {
                DoClick();
            }

            previousTouch1 = touch1;
            previousTouch2 = touch2;
        }

        static DateTime press1Date = new DateTime();
        static DateTime press2Date = new DateTime();
        static OpenTK.Input.TouchState previousTouch;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CheckTouchEntries()
        {



            previousTouch = Touch;
            Touch = OpenTK.Input.Touch.GetTouchesState();
            if (previousTouch.Position1 == null && Touch.Position1 != null)
            {
                press1Date = DateTime.Now;
            }
            else if (previousTouch.Position1 != null && Touch.Position1 == null)
            {
                if ((DateTime.Now - press1Date).TotalMilliseconds < 500)
                {
                    Vec2 v = new Vec2
                    {
                        X = previousTouch.Position1.Value.X - Left,
                        Y = previousTouch.Position1.Value.Y - Top
                    };
                    v.X = +2 * v.X * IWidth - 1;
                    v.Y = -2 * v.Y * IHeight + 1;
                    lock (FlyingLocker)
                    {
                        FlyingPoint = v;
                    }

                    OverFlyRecall = DoClick;
                }
            }
            if (previousTouch.Position2 == null && Touch.Position2 != null)
            {
                press2Date = DateTime.Now;
            }
            else if (previousTouch.Position2 != null && Touch.Position2 == null)
            {
                if ((DateTime.Now - press2Date).TotalMilliseconds < 500)
                {
                    Vec2 v = new Vec2
                    {
                        X = previousTouch.Position2.Value.X - Left,
                        Y = previousTouch.Position2.Value.Y - Top
                    };
                    v.X = +2 * v.X * IWidth - 1;
                    v.Y = -2 * v.Y * IHeight + 1;
                    lock (FlyingLocker)
                    {
                        FlyingPoint = v;
                    }

                    OverFlyRecall = DoCenterObject;
                }
            }

        }

        private static void DoCenterObject()
        {
            if (OverFlownPoint.HasValue)
            {
                CenterObject(OverFlownPoint.Value);
            }
        }

        private static void DoClick()
        {
            //
            if (OverFlownLink != null)
            {
                if (overFlownLink.Solid is TControl)
                {
                    ((TControl)overFlownLink.Solid).Click();
                }
                else if (PointClicked != null)
                {
                    if (OverFlownPoint.HasValue)
                    {
                        PointClicked(OverFlownLink, OverFlownPoint.Value);
                    }
                }
                else if (NodeClicked != null)
                {
                    NodeClicked(OverFlownLink);
                }
                else
                {
                    TLink l = OverFlownLink;
                    if (!Ctrl)
                    {
                        ClearSelection();
                    }

                    AddSelectedLink(l);
                }
            }
            else
            {
                ClearSelection();
            }
        }

        private static void PerformAction()
        {
            switch (PendingAction.PartNumber)
            {
                case "MeasurePoint": TMeasurePointAction.Perform(); break;
                case "MeasureVector": TMeasureVectorAction.Perform(); break;
                case "MeasureMinimalVector": TMeasureMinimalVectorAction.Perform(); break;
            }
        }

        #endregion Private Methods
    }
}