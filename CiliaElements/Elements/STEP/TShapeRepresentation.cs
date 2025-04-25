using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatSTEP
{
    public class TShapeRepresentation : IDisposable
    {
        internal string Matrixes;
        internal string Name;

        public TRelationShip[] Relations;
        internal TReferenceRep ReferenceRep;
        internal TDocument Document;
        internal bool Exclude;

        public void Dispose()
        {
            Matrixes = null; Name = null;
            Relations = null; ReferenceRep = null;
            Document = null;  
        }
        public override string ToString()
        {
            return Name ;
        }
    }
}
