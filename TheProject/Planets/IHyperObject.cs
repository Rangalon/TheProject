using CiliaElements;
using Math3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheProject.Planets
{
    public interface IHyperObject
    {
        Vec4 Speed { get; set; }
        Vec4 Position { get; set; }
        Mtx4 Orientation { get; set; }

        TLink Link { get; set; }

    }
}
