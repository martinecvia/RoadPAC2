#pragma warning disable

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

[assembly: CommandClass(typeof(NET_80_TEST.TestEx))]
namespace NET_80_TEST
{
    public class TestEx : IExtensionApplication
    {
        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            RibbonController.CreateTab("rp_RoadPAC");
            RibbonController.CreateContextualTab("rp_Contextual_SelectView", (_) =>
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
