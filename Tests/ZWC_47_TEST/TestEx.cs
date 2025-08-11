using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

using ZwSoft.Windows;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Runtime;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
{ 
    public class TestEx : IExtensionApplication
    {
        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        public void Initialize()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            var resource = ResourceController.LoadResourceRibbon<RibbonTabDef>("rp_RoadPAC");
            if (resource != null)
            {
                var tab = resource.Transform(new RibbonTab());
                foreach (var panel in resource.PanelsDef)
                {
                    var panelRef = panel.Transform(new RibbonPanel());
                    panelRef.Source = panel.SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[panel.SourceDef.GetType()]());
                    foreach (var item in panel.SourceDef.ItemsDef)
                    {
                        var itemRef = item.Transform(RibbonItemDef.ItemsFactory[item.GetType()]());
                        if (item is RibbonRowPanelDef def1)
                        {
                            if (def1.SourceDef != null && def1.ItemsDef.Count != 0) // Source can't be set when Items is not empty.
                                foreach (var itemDef in def1.ItemsDef)
                                    def1.SourceDef.ItemsDef.Add(itemDef);           // To avoid InvalidOperationException we are effectively transferring everything to SubSource instead
                            foreach (var itemDef in def1.ItemsDef)
                            {
                                if (itemDef is RibbonRowPanelDef || itemDef is RibbonPanelBreakDef)
                                    continue; // The following item types are not supported in this collection: RibbonRowPanel and RibbonPanelBreak.
                                              // An exception is thrown if these objects are added to the collection.
                                ((RibbonRowPanel)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                            }
                        }
                        if (item is RibbonListDef def2)
                            foreach (var itemDef in def2.ItemsDef)
                                ((RibbonList)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                        panelRef.Source.Items.Add(itemRef);
                        Debug.WriteLine($"Registering: {item}");
                    }
                    tab.Panels.Add(panelRef);
                }
                Ribbon.Tabs.Add(tab);
            }
            document.Editor.WriteMessage("Registrace proběhla ! (Test: ZWCAD_DLL)");
        }

        public void Terminate()
        {
            
        }
    }
}
