using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

using Shared.Controllers;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;

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
            ResourceController.LoadEmbeddedResources(); // To load icons, configuration files etc.
            /*
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
            */
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
                    tab.Panels.Add(panelRef);
                }
                Ribbon.Tabs.Add(tab);
            }
        }

        public void Terminate()
        {
            throw new NotImplementedException();
        }

        [CommandMethod("INSPECT_TAB")]
        public void InspectTab()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            PromptStringOptions options = new PromptStringOptions("Name of tab:");
            options.AllowSpaces = true;
            PromptResult result = editor.GetString(options);
            if (result.Status == PromptStatus.OK)
            {
                string tabName = result.StringResult;
                RibbonTab tab = Ribbon.Tabs.Where(t => t.Name == tabName || t.Title == tabName).FirstOrDefault();
                if (tab == null)
                {
                    editor.WriteMessage("Error, NoTab");
                    return;
                }
                Debug.WriteLine($"--- Tab: {tabName} ---\n" +
                    $"Description: {tab.Description} {TryGetDefault(tab, "Description")}\n" +
                    $"Id: {tab.Id} {TryGetDefault(tab, "Id")}\n" +
                    $"IsActive: {tab.IsActive} {TryGetDefault(tab, "IsActive")}\n" +
                    $"IsContextualTab: {tab.IsContextualTab} {TryGetDefault(tab, "IsContextualTab")}\n" +
                    $"IsVisible: {tab.IsVisible} {TryGetDefault(tab, "IsVisible")}\n" +
                    $"IsVisited: {tab.IsVisited} {TryGetDefault(tab, "IsVisited")}\n" +
                    $"KeyTip: {tab.KeyTip} {TryGetDefault(tab, "KeyTip")}\n" +
                    $"Name: {tab.Name} {TryGetDefault(tab, "Name")}\n" +
                    $"Tag: {tab.Tag} {TryGetDefault(tab, "Tag")}\n" +
                    $"Title: {tab.Title} {TryGetDefault(tab, "Title")}\n" +
                    $"IsEnabled: {tab.IsEnabled} {TryGetDefault(tab, "IsEnabled")}\n" +
                    $"IsPanelEnabled: {tab.IsPanelEnabled} {TryGetDefault(tab, "IsPanelEnabled")}\n" +
                    $"IsMergedContextualTab: {tab.IsMergedContextualTab} {TryGetDefault(tab, "IsMergedContextualTab")}\n" +
                    $"AllowTearOffContextualPanels: {tab.AllowTearOffContextualPanels} {TryGetDefault(tab, "AllowTearOffContextualPanels")}\n" +
                    $"--- Panels({tab.Panels.Count}) ---");
                var panels = tab.Panels.Select(p => $"({p.GetType().Name}[{p.Source?.GetType()?.Name}]){p.Source?.Title}");
                Debug.WriteLine(string.Join(", ", panels));
                return;
            }
            editor.WriteMessage("Error");
        }

        [CommandMethod("INSPECT_PANEL")]
        public void InspectPanel()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            PromptStringOptions options = new PromptStringOptions("Name of panel:");
            options.AllowSpaces = true;
            PromptResult result = editor.GetString(options);
            if (result.Status == PromptStatus.OK)
            {
                string panelName = result.StringResult;
                RibbonTab tab = Ribbon.Tabs.Where(t => t.Panels.Any(p => p.Source?.Title == panelName)).FirstOrDefault();
                if (tab == null)
                {
                    editor.WriteMessage("Error, NoTab");
                    return;
                }
                RibbonPanel panel = tab.Panels.Where(p => p.Source?.Title == panelName).FirstOrDefault();
                if (panel == null)
                {
                    editor.WriteMessage("Error, NoPanel");
                    return;
                }
                Debug.WriteLine($"--- Panel({panel.GetType().Name}[{panel.Source?.GetType()?.Name}]): {panelName} ---\n" +
                    $"FloatingOrientation: {panel.FloatingOrientation} {TryGetDefault(panel, "FloatingOrientation")}\n" +
                    $"CanToggleOrientation: {panel.CanToggleOrientation} {TryGetDefault(panel, "CanToggleOrientation")}\n" +
                    $"HighlightPanelTitleBar: {panel.HighlightPanelTitleBar} {TryGetDefault(panel, "HighlightPanelTitleBar")}\n" +
                    $"HighlightWhenCollapsed: {panel.HighlightWhenCollapsed} {TryGetDefault(panel, "HighlightWhenCollapsed")}\n" +
                    $"Id: {panel.Id} {TryGetDefault(panel, "Id")}\n" +
                    $"IsEnabled: {panel.IsEnabled} {TryGetDefault(panel, "IsEnabled")}\n" +
                    $"IsVisible: {panel.IsVisible} {TryGetDefault(panel, "IsVisible")}\n" +
                    $"Source: {panel.Source} {TryGetDefault(panel, "Source")}\n" +
                    $"CustomPanelBackground: {panel.CustomPanelBackground} {TryGetDefault(panel, "CustomPanelBackground")}\n" +
                    $"CustomSlideOutPanelBackground: {panel.CustomSlideOutPanelBackground} {TryGetDefault(panel, "CustomSlideOutPanelBackground")}\n" +
                    $"CustomPanelTitleBarBackground: {panel.CustomPanelTitleBarBackground} {TryGetDefault(panel, "CustomPanelTitleBarBackground")}\n" +
                    $"IsContextualTabThemeIgnored: {panel.IsContextualTabThemeIgnored} {TryGetDefault(panel, "IsContextualTabThemeIgnored")}\n");
                if (panel.Source != null && panel.Source.Items.Count > 0)
                {
                    var items = panel.Source.Items.Select(i => $"({i.GetType().Name}){i.Text ?? "Id:" + i.Id}");
                    Debug.WriteLine($"--- Items({panel.Source.Items.Count}) ---\n" +
                        $"{string.Join(",", items)}");
                }
                return;
            }
            editor.WriteMessage("Error");
        }

        [CommandMethod("INSPECT_ITEM")]
        public void InspectItem()
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            PromptStringOptions options = new PromptStringOptions("Text(or Id) of item:");
            options.AllowSpaces = true;
            PromptResult result = editor.GetString(options);
            if (result.Status == PromptStatus.OK)
            {
                string itemName = result.StringResult;
                RibbonTab tab = Ribbon.Tabs.Where(t => t.Panels.Any(p => p.Source != null
                    && p.Source.Items.Any(i => i.Text == itemName || i.Id == itemName))).FirstOrDefault();
                if (tab == null)
                {
                    editor.WriteMessage("Error, NoTab");
                    return;
                }
                RibbonPanel panel = tab.Panels.Where(p => p.Source != null && p.Source.Items.Any(i => i.Text == itemName || i.Id == itemName)).FirstOrDefault();
                if (panel == null)
                {
                    editor.WriteMessage("Error, NoPanel");
                    return;
                }
                List<RibbonItem> items = panel.Source.Items.Where(i => i.Text == itemName || i.Id == itemName).ToList();
                if (items.Count == 0)
                {
                    editor.WriteMessage("Error, NoItem");
                    return;
                }
                foreach (RibbonItem item in items)
                {
                    Debug.WriteLine($"--- Item({item.GetType().Name}): {itemName}  ---\n");
                    PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
                    foreach (var property in properties)
                    {
                        var declaringSetterType = property.SetMethod.DeclaringType;
                        var inheritedFrom = (declaringSetterType != null && declaringSetterType != item.GetType()) ? $"{declaringSetterType.Name}->" : "";
                        Debug.WriteLine($"{inheritedFrom}{property.Name}: {property.GetValue(item)} {TryGetDefault(item, property.Name)}");
                    }
                }
                return;
            }
            editor.WriteMessage("Error");
        }

        private object TryGetDefault(object source, string propertyName)
        {
            try
            {
                PropertyInfo propertyInfo = source.GetType().GetProperties()
                    .Where(p => p.Name == propertyName).FirstOrDefault();
                if (propertyInfo == null)
                    return "[default: Error]";
                DefaultValueAttribute defaultAttribute = (DefaultValueAttribute)propertyInfo.GetCustomAttributes(false).Where(a => a is DefaultValueAttribute).FirstOrDefault();
                if (defaultAttribute == null)
                    return "";
                if (defaultAttribute.Value is string def && string.IsNullOrEmpty(def) && propertyInfo.PropertyType.Name is "String")
                    return $"[default: '']";
                return $"[default: {defaultAttribute.Value ?? "null"}]";
            } catch
            {
                return "[default: Error]";
            }
        }
    }
}
