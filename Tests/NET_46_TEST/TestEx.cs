using System;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Internal.Windows;
using Autodesk.Windows;
using Shared.Controllers;

// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-4E1AAFA9-740E-4097-800C-CAED09CDFF12
// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-C3F3C736-40CF-44A0-9210-55F6A939B6F2
// Developer site for 2017
// https://aps.autodesk.com/developer/overview/autocad
[assembly: CommandClass(typeof(NET_46_TEST.TestEx))]
namespace NET_46_TEST
{
    public class TestEx : IExtensionApplication
    {

        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            RibbonTab rpTab = RibbonController.CreateTab("MAIN", "{{ RP_TAB_MAIN.NAME }}");
            RibbonPanelSource rpTabPanel = new RibbonPanelSource
            {
                Title = "{{ RP_TAB_MAIN.PROJECT_MANAGER.NAME }}",
                Name = "RP_TAB_MAIN.PROJECT_MANAGER"
            };
            rpTab.Panels.Add(new RibbonPanel { Source = rpTabPanel }); // Adding RibbonPanelSource early
            var ctxTab = RibbonController.CreateContextualTab("RP_CONTEXT1_TRASA", "Trasa", selection => {
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
            });
            
            Ribbon.Tabs.Add(rpTab);
            Ribbon.Tabs.Add(ctxTab);

            var a = Ribbon.Tabs.Where(t => t.Name == "Parametric");
            var b = Ribbon.Tabs.Where(t => t.Name == "Output");
            ;

        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
