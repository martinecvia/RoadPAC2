using System.Diagnostics;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Shared;
using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;

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

            RibbonTab rpTab = RibbonController.CreateTab("MAIN", "RoadPAC");
            RibbonButton button = new RibbonButton
            {
                Text = "RoadPAC", // Untranslatable entity
                Name = "RP_TAB_MAIN.PROJECT_MANAGER.RUN_ROADPAC",
                ShowText = true,
                Size = RibbonItemSize.Large,
                Image = ResourceController.GetImageSource("rp_img_btn_manual_16"),
                LargeImage = ResourceController.GetImageSource("rp_img_btn_manual_32"),
                Orientation = System.Windows.Controls.Orientation.Vertical,
                ShowImage = true,
                CommandHandler = new CommandHandler("RP_RUN_ROADPAC")
            };
            RibbonPanelSource rpTabPanel = new RibbonPanelSource
            {
                Title = "{{ RP_TAB_MAIN.PROJECT_MANAGER.NAME }}",
                Name = "RP_TAB_MAIN.PROJECT_MANAGER",
            };
            rpTabPanel.Items.Add(button);
            rpTabPanel.Items.Add(new RibbonSeparator { SeparatorStyle = RibbonSeparatorStyle.Spacer }); // Vertical line
            rpTab.Panels.Add(new RibbonPanel { Source = rpTabPanel }); // Adding RibbonPanelSource early
            var ctxTab = RibbonController.CreateContextualTab("RP_CONTEXT1_TRASA", selection => {
                if (selection == null || selection.Count == 0)
                    return false;
                using (var transaction = document.TransactionManager.StartTransaction())
                {
                    foreach (var Id in selection.GetObjectIds())
                    {
                        var lookup = transaction.GetObject(Id, OpenMode.ForRead, false);
                        if (lookup is Line || lookup is Polyline)
                            return true;
                    }
                }
                return false;
            }, "Trasa");

            Ribbon.Tabs.Add(rpTab);
            Ribbon.Tabs.Add(ctxTab);

            var b = Ribbon.Tabs.Where(t => t.Name == "Output"); // To see whats happening there
            var x = ResourceController.LoadResourceRibbon<RibbonTabDef>("rp_RoadPAC");
            Debug.WriteLine(x);
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
