using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatSTEP
{
    internal class TDescriptiveRepresentationItem:IDisposable 
    {
        public string Tag, Value;

        public void Dispose()
        {
            Tag = null;Value = null;
        }
    }
}
