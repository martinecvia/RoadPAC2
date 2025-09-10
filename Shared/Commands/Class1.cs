
using System;
using System.Collections.Generic;
using System.Text;

#if ZWCAD
    using ZwSoft.ZwCAD.Runtime;      
#else
    using Autodesk.AutoCAD.Runtime;
#endif

namespace Shared.Commands
{
    internal class CommandCls
    {
        [CommandMethod("RP_RIBBONVIEW")]
        public void RibbonView()
        {
            Shared.Controllers.RibbonController.CreateTab("rp_RoadPAC");
        }
    }
}
