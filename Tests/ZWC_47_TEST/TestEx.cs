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
            var tab = resource?.Transform(new RibbonTab());
            if (resource != null)
            {
                foreach (var panel in resource.PanelsDef)
                {
                    var panelRef = panel.Transform(new RibbonPanel());
                    panelRef.Source = panel.SourceDef?.Transform(RibbonPanelSourceDef.SourceFactory[panel.SourceDef.GetType()]());
                    if (panel.SourceDef != null && panel.SourceDef is RibbonPanelSpacer)
                        // RibbonPanelSpacer can't have any items in it,
                        // so why should we bother translating them over
                        continue;
                    foreach (var item in panel.SourceDef.ItemsDef)
                    {
                        try
                        {
                            var itemRef = item.Transform(RibbonItemDef.ItemsFactory[item.GetType()]());
                            // There should be a better way to handle nested items
                            if (item is RibbonRowPanelDef def1)
                                foreach (var itemDef in def1.ItemsDef)
                                    ((RibbonRowPanel)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                            if (item is RibbonListDef def2)
                                foreach (var itemDef in def2.ItemsDef)
                                    ((RibbonList)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                            panelRef.Source.Items.Add(itemRef);
                        }
                        catch (NotImplementedException exception) { } // Some items in ZWCAD are created but not implemented
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
