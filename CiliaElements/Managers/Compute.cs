using CiliaElements.Utilities;
using Math3D;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CiliaElements
{
    public static partial class TManager
    {

        private static void DoerCompute()
        {
            TQuickStc<TLink> stc = new TQuickStc<TLink>();
            TThreadGoverner w = TThreadGoverner.Compute;
            while (DoerComputeActivated)
            {
               

                lock (LinksToMove)
                {
                    while (LinksToMove.Count > 0)
                    {
                        TLink l = LinksToMove.Pop();
                        l.Matrix = l.PendingMatrix;
                    }
                }

                double d = Vec4.Transform(Target, VMatrix).Length;
                SelectedLayer = Layers.FirstOrDefault(o => o.NearDistance <= d && o.FarDistance >= d);
                if (SelectedLayer == null)
                {
                    SelectedLayer = Layers[Layers.Length - 1];
                }
                //
                //
                if (!Focused)
                {
                    previousMouse = null;
                }
                else if (PendingViewPoint.HasValue)
                {
                    VMatrix = PendingViewPoint.Value;

                    PendingViewPoint = null;
                    VIMatrix = Mtx4.InvertL(VMatrix);
                    viewerPanel.DisplayedVector = (Vec3)VIMatrix.Row3;
                    //
                    BaseLayer.PVMatrix = VMatrix * BaseLayer.PMatrix;
                    BaseLayer.PVIMatrix = Mtx4.Invert(BaseLayer.PVMatrix);// BaseLayer.PVIMatrix.Invert();
                    foreach (TLayer l in Layers)
                    {
                        l.PVMatrix = VMatrix * l.PMatrix;
                        l.PVIMatrix = Mtx4.Invert(l.PVMatrix); //l.PVIMatrix.Invert();
                    }
                }
                else
                {
                    Vec2 M = new Vec2
                    {
                        X = Mouse.Position.X - Left,
                        Y = Mouse.Position.Y - Top
                    };
                    //
                    M.X = +2 * M.X * IWidth - 1;
                    M.Y = -2 * M.Y * IHeight + 1;
                    //
                    Cursor = M;
                    Touch1 = Touch.Position1;
                    Touch2 = Touch.Position2;
                    //
                    if (Touch1 == null && Touch2 == null)
                    {
                        previousTouch1 = null;
                        previousTouch2 = null;
                        FlyingPoint = Cursor;
                        CheckMouseDrags(Mouse, M, d);
                    }
                    else
                    {
                        previousMouse = null;
                        CheckTouches(d);
                    }

                    // -------------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------------
                    // -------------------------------------------------------------------------------------------
                    // Memorize last mouse position
                    previousMouse = Mouse;

                    //
                    VIMatrix = Mtx4.InvertL(VMatrix);
                    viewerPanel.DisplayedVector = (Vec3)VIMatrix.Row3;
                    //
                    BaseLayer.PVMatrix = VMatrix * BaseLayer.PMatrix;
                    BaseLayer.PVIMatrix = Mtx4.Invert(BaseLayer.PVMatrix);
                    foreach (TLayer l in Layers)
                    {
                        l.PVMatrix = VMatrix * l.PMatrix;
                        l.PVIMatrix = Mtx4.Invert(l.PVMatrix);
                    }
                    //
                    Cursor = M;
                    overflownPanel.OwnerLink.Giving.Matrix = Mtx4.CreateScale(targetPanel.Width, 1, targetPanel.Height) * CreateForeMatrix(-Cursor.X, -Cursor.Y, 0.001, 0.002);

                    Touch1 = Touch.Position1;
                    Touch2 = Touch.Position2;
                    //
                    if (MovingPoint.HasValue)
                    {
                        if (Touch1 == null && Touch2 == null)
                        {
                            MovingPoint = CursorPoint;
                            MovingVector = MovingPoint.Value - Target;
                        }
                        else if (Touch1 != null && Touch2 != null)
                        {
                            MovingPoint = (Touch1Point + Touch2Point) * 0.5;
                        }
                        else if (Touch1 != null)
                        {
                            MovingPoint = Touch1Point;
                        }
                        else if (Touch2 != null)
                        {
                            MovingPoint = Touch2Point;
                        }
                    }
                }
                //
                CrossLink.Matrix = Mtx4.CreateTranslation(Target);
                //
                w.Reset();
                View.OwnerLink.Giving.State = ELinkState.None;
                View.OwnerLink.Giving.Matrix = VMatrix;
           

                BaseLayer.SolidsBuffer.Reset();
                Array.ForEach(Layers, l => { l.SolidsBuffer.Reset(); });
                 
                View.OwnerLink.DoCompute();

               
                //
               lock (BaseLayer)
                {
                    Array.Resize(ref BaseLayer.GivingSolids, BaseLayer.SolidsBuffer.Max);
                    Array.Copy(BaseLayer.SolidsBuffer.Values, 0, BaseLayer.GivingSolids, 0, BaseLayer.SolidsBuffer.Max);

                    foreach (TLayer l in Layers)
                    {
                        Array.Sort(l.SolidsBuffer.Values, TLinkTransparencyComparer.Default);
                        //lock (l)
                        {
                            Array.Resize(ref l.GivingSolids, l.SolidsBuffer.Max);
                            Array.Copy(l.SolidsBuffer.Values, 0, l.GivingSolids, 0, l.SolidsBuffer.Max);
                        }
                    }
                }
                //
                loadingPanel.Visible = ToBeParsedLinks.NotEmpty;
                //
                w.Check();
            }
            stc.Dispose();
        }
         
    }

}
