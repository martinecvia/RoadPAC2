#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models;
using Shared.Controllers.Models.RibbonXml;
using static Shared.Controllers.Controls.Ribbon.RibbonButton_Close_ContextualTab;

[assembly: CommandClass(typeof(RibbonButton_Close_ContextualTab_Commander))]
namespace Shared.Controllers.Controls.Ribbon
{
    public class RibbonButton_Close_ContextualTab : BaseRibbonControl<RibbonButton>
    {
        public RibbonButton_Close_ContextualTab(RibbonButton target, BaseRibbonXml source) 
            : base(target, source) { }

        public class RibbonButton_Close_ContextualTab_Commander
        {
            [CommandMethod("RP_AECCLCTX")]
            public void CloseContextualTab()
            {
                if (ComponentManager.Ribbon.ActiveTab is ContextualRibbonTab selected)
                    selected.Hide();
            }
        }
    }
}