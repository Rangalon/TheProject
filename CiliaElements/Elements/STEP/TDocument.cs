using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatSTEP
{
    public class TDocument : IDisposable
    {
        public string RelativePath;
        internal TFile File;

        public void Dispose()
        {
            RelativePath = null; File = null;
        }

        public override string ToString()
        {
            return RelativePath;
        }
    }
}
