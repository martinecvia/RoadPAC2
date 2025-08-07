using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
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
                    panelRef.Source = panel.SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[panel.SourceDef.GetType()]());
                    foreach (var item in panel.SourceDef.ItemsDef)
                    {
                        var itemRef = item.Transform(RibbonItemDef.ItemsFactory[item.GetType()]());
                        panelRef.Source.Items.Add(itemRef);
                    }
                    tab?.Panels.Add(panelRef);
                }
                Ribbon.Tabs.Add(tab);
            }
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }
    }
}
