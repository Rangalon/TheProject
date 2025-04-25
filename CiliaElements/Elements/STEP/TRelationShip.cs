using Math3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatSTEP
{
    public class TRelationShip : IDisposable
    {
        public int Id1, Id2;
        public Mtx4 Matrix;
        internal TShapeRepresentation Parent;
        internal TShapeRepresentation Children;

        public void Dispose()
        {
            Parent = null; Children = null; 
        }
        public override string ToString()
        {
            return Id1.ToString() + " => " + Id2.ToString()+": "+Parent.ReferenceRep.PartNumber+" => "+Children.ReferenceRep.PartNumber ;
        }
    }
}
