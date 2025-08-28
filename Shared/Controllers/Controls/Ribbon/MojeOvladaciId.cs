using System;
using System.Collections.Generic;
using System.Text;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Controls.Ribbon
{
    class MojeOvladaciId : BaseRibbonControl<RibbonButton>
    {
        public MojeOvladaciId(RibbonButton target, RibbonButtonDef source) : base(target, source)
        {
            target.Text = RPApp.IsAcad + "1";
        }
    }
}
