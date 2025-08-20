using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Shared.Controllers;


[assembly: CommandClass(typeof(NET_48_TEST.TestEx))]
namespace NET_48_TEST
{
    public class TestEx : IExtensionApplication
    {
        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", selection =>
            {
                return true;
            });
        }

        public void Terminate()
        { }

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = Ribbon;
        }
    }
}