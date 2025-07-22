using System;
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
            RibbonTab tab = RibbonController.CreateTab("RP_MAIN", "RoadPAC");
            ContextualRibbonTab ctxTab = RibbonController.CreateContextualTab("RP_CONTEXT1_TRASA", "Trasa", selection => {
                if (selection == null || selection.Count == 0)
                    return false;
                using (var transaction = document.TransactionManager.StartTransaction())
                {
                    foreach (var Id in selection.GetObjectIds())
                    {
                        var lookup = transaction.GetObject(Id, OpenMode.ForRead, false);
                        if (lookup is Line)
                            return true;
                    }
                }
                return false;
            });

            // Create a ribbon tab panel
            RibbonPanelSource panelSource = new RibbonPanelSource
            {
                Title = "My Panel",
                Name = "MyPanel"
            };

            // Add a button to the panel
            RibbonButton button = new RibbonButton
            {
                Text = "Do Something",
                Name = "MyActionButton",
                ShowText = true,
                Size = RibbonItemSize.Large,
                Orientation = System.Windows.Controls.Orientation.Vertical
            };
            panelSource.Items.Add(button);
            tab.Panels.Add(new RibbonPanel { Source = panelSource });

            Ribbon.Tabs.Add(tab);
            Ribbon.Tabs.Add(ctxTab);
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
