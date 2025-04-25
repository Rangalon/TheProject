using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CiliaElements.FormatSTEP
{
    public enum EPropertyType
    {
        external_ref,
        id,
        name,
        DMU_Type,
        Description,
        TDSM_description,
        TDSM_name,
        TDSM_type,
        TDSM_ataref,
        AREA_SOL,
        BOX,
        COG,
        VERSION,
        VOL_SOL,
        WRVIEW,
        PDMTYPE, 
        PDM_ENV,
    }

    internal class TPropertyDefinition : IDisposable
    {
        public void Dispose()
        {
            Obj = null; 
        }
        public object Obj;
        public EPropertyType Property;
    }
}
