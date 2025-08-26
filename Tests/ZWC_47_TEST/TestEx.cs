using System.Diagnostics;
using System.Linq;
using Shared;
using Shared.Controllers;
using ZwSoft.Windows;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Runtime;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
{ 
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
            ResourceController.LoadEmbeddedResources(); // This must happen before loading any ribbon, image etc.
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", selection =>
            {
                return selection.GetObjectIds().Any(o => o.ObjectClass.DxfName.ToLower() == "lwpolyline");
            });
            new RPApp();
        }

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = ComponentManager.Ribbon;
        }
        public void Terminate()
        { }
    }
}
