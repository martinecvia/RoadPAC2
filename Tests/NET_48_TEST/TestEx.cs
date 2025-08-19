using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

using Shared.Controllers;


[assembly: CommandClass(typeof(NET_48_TEST.TestEx))]
namespace NET_48_TEST
{
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            RibbonController.CreateTab("rp_RoadPAC");
        }

        public void Terminate()
        {

        }
    }
}