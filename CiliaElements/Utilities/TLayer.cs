
using Math3D;

namespace CiliaElements
{
    public class TLayer
    {
        #region Public Fields

        public double FarDistance;
        public SLink[] GivingSolids = new SLink[] { };
        public double NearDistance;
        public Mtx4 PIMatrix = Mtx4.Identity;
        public Mtx4 PMatrix = Mtx4.Identity;
        public Mtx4f PMatrixf;
        public Mtx4 PVIMatrix = Mtx4.Identity;
        public Mtx4 PVMatrix = Mtx4.Identity;

        public TQuickStc<SLink> SolidsBuffer = new TQuickStc<SLink>();
        public TQuickStc<SLink> PointsOnlysBuffer = new TQuickStc<SLink>();
        public SLink[] TakenSolids = new SLink[] { };

        #endregion Public Fields
    }

    public struct SLink
    {
        public SLink(TLink l)
        {
            Link = l;
            State = l.State;
            Matrix = l.Matrix;
            Color = Mtx4f.Identity;
            Selected = false;
            NoEffectValue = 0;
            NoDiffuseValue = 0;
        }

        public TLink Link;
        public Mtx4 Matrix;
        public ELinkState State;
        internal Mtx4f Color;
        internal bool Selected;
        internal int NoEffectValue;
        internal int NoDiffuseValue;
    }
}