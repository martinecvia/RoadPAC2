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
                        if (item is RibbonRowPanelDef def1)
                            foreach (var itemDef in def1.ItemsDef)
                                ((RibbonRowPanel)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
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
                    $"  - Items: {panel.Source.Items} [default: []]\n" +
                    $"  - Description: {panel.Source.Description} {TryGetDefault(panel.Source, "Description")}\n" +
                    $"  - DialogLauncher: {panel.Source.DialogLauncher} {TryGetDefault(panel.Source, "DialogLauncher")}\n" +
                    $"{TryVisit(panel.Source?.DialogLauncher)}" +
                    $"  - Id: {panel.Source.Id} {TryGetDefault(panel.Source, "Id")}\n" +
                    $"  - Name: {panel.Source.Name} {TryGetDefault(panel.Source, "Name")}\n" +
                    $"  - KeyTip: {panel.Source.KeyTip} {TryGetDefault(panel.Source, "KeyTip")}\n" +
                    $"  - Tag: {panel.Source.Tag} {TryGetDefault(panel.Source, "Tag")}\n" +
                    $"  - Title: {panel.Source.Title} {TryGetDefault(panel.Source, "Title")}\n" +
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

        [CommandMethod("INSPECT_PANEL_ITEM")]
        public void InspectPanelItem()
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
                PromptIntegerOptions intOpt = new PromptIntegerOptions("Index of item:");
                PromptIntegerResult intResult = editor.GetInteger(intOpt);
                if (intResult.Status == PromptStatus.OK)
                {
                    if (panel.Source == null)
                    {
                        editor.WriteMessage("Error, NoSource");
                        return;
                    }
                    RibbonItem item = panel.Source.Items[intResult.Value];
                    Debug.WriteLine($"--- Item({item.GetType().Name}): {item.Name ?? item.GetType().Name}  ---\n");
                    PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
                    foreach (var property in properties)
                    {
                        var declaringSetterType = property.SetMethod.DeclaringType;
                        var inheritedFrom = (declaringSetterType != null && declaringSetterType != item.GetType()) ? $"{declaringSetterType.Name}->" : "";
                        Debug.WriteLine($"{inheritedFrom}{property.Name}: {property.GetValue(item)} {TryGetDefault(item, property.Name)}");
                    }
                }
            }
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
            }
            catch
            {
                return "[default: Error]";
            }
        }

        private object TryVisit(object source)
        {
            if (source == null)
                return null;
            try
            {
                PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
                string result = "";
                foreach (var property in properties)
                    result = result + $"    - {property.Name}: {property.GetValue(source)}\n";
                return result; ;
            }
            catch (System.Exception) { }
            return null;
        }

        private void Compare(object ob1, object ob2)
        {
            if (ob1 == null || ob2 == null)
                return;
            PropertyInfo[] prop1 = ob1.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
            PropertyInfo[] prop2 = ob2.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Where(property => property.SetMethod != null && property.SetMethod.IsPublic && !property.SetMethod.IsStatic).ToArray();
            Dictionary<string, PropertyInfo> dict1 = new Dictionary<string, PropertyInfo>();
            foreach (var p1 in prop1) { dict1[p1.Name] = p1; }
            Dictionary<string, PropertyInfo> dict2 = new Dictionary<string, PropertyInfo>();
            foreach (var p2 in prop2) { dict2[p2.Name] = p2; }
            Debug.WriteLine($"--- Compare: {ob1}/{ob2}");
            Debug.WriteLine($"Number of properties: {dict1.Count}/{dict2.Count}");
            foreach (var item in dict2) { if (!dict1.ContainsKey(item.Key)) Debug.WriteLine($"1: Is missing {item.Key} property"); }
            foreach (var item in dict1)
            {
                // Check what properties are missing
                if (!dict2.ContainsKey(item.Key))
                    Debug.WriteLine($"2: Is missing {item.Key} property");
                else
                {
                    // Check their values
                    object val1 = item.Value.GetValue(ob1);
                    object val2 = dict2[item.Key].GetValue(ob2);
                    if (val1?.ToString() != val2?.ToString())
                        Debug.WriteLine($"{item.Key}: {val1}/{val2}");
                }
            }
        }
    }
}
