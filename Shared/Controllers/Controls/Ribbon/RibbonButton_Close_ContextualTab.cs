#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml;

[assembly: CommandClass(typeof(Shared.Controllers.Controls.Ribbon.
    RibbonButton_Close_ContextualTab.RibbonButton_Close_ContextualTab_Commander))]
namespace Shared.Controllers.Controls.Ribbon
{
    public class RibbonButton_Close_ContextualTab : BaseRibbonControl<RibbonButton>
    {
        public RibbonButton_Close_ContextualTab(RibbonButton target, BaseRibbonXml source)
            : base(target, source) { }

        public class RibbonButton_Close_ContextualTab_Commander
        {
            [CommandMethod("RP_AECCLCTX")]
            public static void CloseContextualTab()
            {
                if (ComponentManager.Ribbon.ActiveTab is RibbonTab selected
                    && selected.Id.StartsWith(RibbonController.RibbonTab__Prefix))
                    RibbonController.HideContextualTab(selected);
            }
        }
    }
}