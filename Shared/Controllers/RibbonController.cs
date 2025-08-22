#pragma warning disable CS8600, CS8602, CS8618, CS8622, CS8625
#pragma warning disable IDE0001, IDE0028, IDE0083, IDE0090, IDE0300, IDE0305

#define DEBUG
#define NON_VOLATILE_MEMORY

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Diagnostics;
using System.Linq; // Keep for .NET 4.6
using System.Reflection;
using System.Runtime.CompilerServices;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

namespace Shared.Controllers
{
    public static class RibbonController
    {
        public const string RibbonTab__Prefix = "RP_TAB_";  // RoadPAC prefix for tabs. so we can distinguish
                                                            // other tab's from AutoCAD.
                                                            // This also prevents using the same name from different applications.

        private static RibbonControl Ribbon => ComponentManager.Ribbon;

        private static readonly string ControlsNamespace = "Shared.Controllers.Controls.Ribbon";
        private static readonly Dictionary<string, object> RegisteredControls = new Dictionary<string, object>();
        private static readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions 
            = new Dictionary<string, Func<SelectionSet, bool>>();

        private static bool HasAnyContextualTab { get; set; } = false;

        private static RibbonTab _contextualTab = null;

        [DefaultValue(false)]
        private static bool IsSelectionHandled { get; set; } = false;

        /// <summary>
        /// Ensures that the ribbon system has been properly initialized before use.
        /// This method is intended to prevent loading via reflection or in unsupported contexts.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the ribbon has not been initialized, which may indicate
        /// improper access such as reflection-based loading or an invalid initialization sequence.
        /// </exception>
        [RPPrivateUseOnly]
        private static void AssertInitialized()
        {
            // Prevent usage in cases where the ribbon is not properly initialized,
            // such as when loaded via reflection or outside the intended application lifecycle.
            if (Ribbon == null)
                throw new InvalidOperationException("Ribbon can't be loaded using reflection.");
        }

        public static RibbonTab CreateTab(string tabId, string tabName = null, string tabDescription = null) 
            => CreateTab<RibbonTab>(tabId, tabName, tabDescription);

        [RPPrivateUseOnly]
        private static T CreateTab<T>(string tabId,
                                      string tabName = null,
                                      string tabDescription = null) where T : RibbonTab, new()
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            RibbonTabDef tabDef = ResourceController.LoadResourceRibbon<RibbonTabDef>(tabId);
            T tab = tabDef?.Transform(new T()) ?? new T();
            Ribbon.Tabs.Add(tab);
            tab.Id = RibbonTab__Prefix + tabId;       // We want to mark these tabs as RoadPAC ones.
                                                      // For further compatibility and to prevent being overriden.
            if (tabDef != null)
            {
                foreach (var panelDef in tabDef.PanelsDef)
                {
                    // Setting up cookie must happend before transforming to reference item
                    string cookie = tab.Id;
                    panelDef.Cookie = panelDef.Cookie.Replace("%Parent", cookie);
                    cookie += $";{panelDef.Id}";
                    var panelRef = panelDef.Transform(new RibbonPanel());
                    if (panelDef.SourceDef == null)
                        continue;
                    panelRef.UID = panelDef.Id; // For some reason panel can't have Id
                    tab.Panels.Add(panelRef);
                    panelDef.SourceDef.Cookie = panelDef.SourceDef.Cookie.Replace("%Parent", cookie);
                    cookie += $";{panelDef.SourceDef.Id}";
                    panelRef.Source = panelDef.SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[panelDef.SourceDef.GetType()]());
                    foreach (var itemDef in panelDef.SourceDef.ItemsDef)
                    {
                        itemDef.Cookie = itemDef.Cookie.Replace("%Parent", cookie);
                        cookie += $";{itemDef.Id}";
                        var itemRef = ProcessRibbonItem(itemDef, panelDef, cookie, currentDepth: 0); // Directly setting currentDepth to zero here,
                                                                                                     // because sometimes C# keeps refference to previous currentDepth, which is odd 
                                                                                                     // even tho function has default value defined,
                                                                                                     // so some might think it will take the default value.
                                                                                                     // This is a compiler issue with .NET 4.6 NDP46-KB3045557-x86-x64
                        if (itemRef != null) // null RibbonItem definitions will break cad instance
                            panelRef.Source.Items.Add(itemRef);
                    }
                }
            }
            tab.IsEnabled = true;
            if (!string.IsNullOrEmpty(tabName) || string.IsNullOrEmpty(tab.Name))
                tab.Name = tabName ?? tabId; tab.Title = tabName ?? tab.Name;
            if (!string.IsNullOrEmpty(tabDescription))
                tab.Description = tabDescription;
            tab.UID = tab.Id;
            return tab;
        }

        public static void CreateContextualTab(string tabId, Func<SelectionSet, bool> onSelectionMatch)
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            _contextualTabConditions.Add(tabId, onSelectionMatch);
            if (!HasAnyContextualTab)
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                document.ImpliedSelectionChanged += OnSelectionChanged;
                HasAnyContextualTab = true;
            };
        }

        [RPInternalUseOnly]
        internal static void HideContextualTab()
        {
            Application.Idle -= OnSelectionIdle;
            if (_contextualTab != null)
            {
                Ribbon.HideContextualTab(_contextualTab);
                _contextualTab.IsVisible = false;
                _contextualTab = null;
                _contextualIdx = null;
            }
        }

        private static string _contextualIdx = null;

        [RPPrivateUseOnly]
        private static void OnSelectionIdle(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            if (_contextualTab == null && _contextualIdx != null) {
                _contextualTab = CreateTab(_contextualIdx);
                _contextualTab.IsVisible = true;
                _contextualTab.IsContextualTab = true;
            }
            if (!_contextualTab.IsVisible)
            {
                Ribbon.ShowContextualTab(_contextualTab, false, true);
                _contextualTab.IsActive = true;
            }
            if (!_contextualTab.IsActive)
                _contextualTab.IsActive = true;
        }

        [RPPrivateUseOnly]
        private static void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
            Document document = Application.DocumentManager.MdiActiveDocument;
            PromptSelectionResult result = document.Editor.SelectImplied();
            if (result.Status != PromptStatus.OK || result.Value == null || result.Value.Count == 0)
            {
                Application.Idle -= OnSelectionIdle;
                if (_contextualTab != null)
                {
                    Ribbon.HideContextualTab(_contextualTab);
                    _contextualTab.IsVisible = false;
                    // Possibly remove duplicates ?
                    _contextualTab = null;
                    _contextualIdx = null;
                }
                return;
            }
            SelectionSet selection = result.Value;
            if (selection != null)
            {
                foreach (KeyValuePair<string, Func<SelectionSet, bool>> pair in _contextualTabConditions)
                {
                    if (pair.Value == null)
                        continue; // If for some reason Func<SelectionSet, bool>> will be null during tab creation
                                  // we will just skip handling this tab and treat it as normal one
                    if (pair.Value.Invoke(selection))
                    {
                        _contextualIdx = pair.Key;
                        if (_contextualTab == null || !_contextualTab.IsVisible)
                            Application.Idle += OnSelectionIdle;
                        return;
                    }
                }
            }
        }

        [RPPrivateUseOnly]
#if NET8_0_OR_GREATER
        private static RibbonItem? ProcessRibbonItem(RibbonItemDef itemDef, RibbonPanelDef panelDef,
#else
        private static RibbonItem ProcessRibbonItem(RibbonItemDef itemDef, RibbonPanelDef panelDef,
#endif
            string cookie,
            int currentDepth = 0) // this signalizes how many hops had happend during reccursion,
                                  // we don't want to be stack-overflowed, so depth is actually checked limited
        {
            // Maximal depth check
            if (currentDepth < 4 || RibbonItemDef.ItemsFactory.ContainsKey(itemDef.GetType()))
            {
                itemDef.Cookie = itemDef.Cookie.Replace("%Parent", cookie);
                var itemRef = itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]());
                switch (itemDef)
                {
                    case RibbonRowPanelDef item:
                        List<RibbonItemDef> children;
                        if (item.SourceDef != null && item.ItemsDef.Count != 0)
                        {
                            item.SourceDef.Cookie = item.SourceDef.Cookie.Replace("%Parent", item.Id);
                            cookie += $";{item.SourceDef.Id}";
                            item.SourceDef.ItemsDef.AddRange(item.ItemsDef);
                            children = item.SourceDef.ItemsDef;
                        }
                        else
                        {
                            children = item.ItemsDef;
                        }
                        var target = ((RibbonRowPanel)itemRef).Source?.Items ?? ((RibbonRowPanel)itemRef).Items;
                        foreach (var childDef in children)
                        {
                            // The following item types are not supported in this collection: RibbonRowPanel and RibbonPanelBreak
                            if (childDef is RibbonRowPanelDef || childDef is RibbonPanelBreakDef)
                                continue;
                            var childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                target.Add(childRef);
                        }
                        break;
                    case RibbonListDef.RibbonComboDef item:
                        // If ItemsBinding is set to a valid binding, this collection should not be modified
                        // An exception is thrown if the Items collection is modified when ItemsBinding is not null
                        if (((RibbonList)itemRef).ItemsBinding == null && item.ItemsDef.Count > 0)
                        {
                            // Either Items or ItemsBinding can be used to manage the collection, but not both
                            foreach (var childDef in item.ItemsDef)
                            {
                                var childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                                if (childRef != null)
                                    ((RibbonList)itemRef).Items.Add(childRef);
                            }
                        }
                        foreach (var childDef in item.MenuItemsDef)
                        {
                            var childRef = (RibbonCommandItem)ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                ((RibbonCombo)itemRef).MenuItems.Add(childRef);
                        }
                        break;
                    case RibbonListButtonDef item:

                        foreach (var childDef in item.ItemsDef)
                        {
                            // Set of rules for each implementation of RibbonListButtonDef
                            switch (item)
                            {
                                case RibbonListButtonDef.RibbonMenuButtonDef _:
                                    if (!(childDef is RibbonMenuItemDef) && !(childDef is RibbonSeparatorDef))
                                        continue;
                                    break;
                                case RibbonListButtonDef.RibbonRadioButtonGroupDef _:
                                    if (!(childDef is RibbonToggleButtonDef))
                                        continue;
                                    break;
                                default:
                                    if (!(childDef is RibbonCommandItemDef) && !(childDef is RibbonSeparatorDef))
                                        continue;
                                    break;
                            }
                            var childRef = ProcessRibbonItem(childDef, panelDef, $"{cookie}", currentDepth + 1);
                            if (childRef != null)
                                ((RibbonListButton)itemRef).Items.Add(childRef);
                        }
                        break;
                        { } // Little C# hack for better memory management
                }
                if (!string.IsNullOrEmpty(itemDef.UUID) && !RibbonController.RegisteredControls.ContainsKey(itemDef.UUID))
                {
                    Type wrapperType = Assembly.GetExecutingAssembly()
                        .GetType($"{RibbonController.ControlsNamespace}.{itemDef.Id}", false, true);
                    if (wrapperType != null)
                    {
                        try
                        {
                            // We'll try to invoke our Id, and our target so we can individualy control each control
                            var invoke = wrapperType.GetConstructors()
                                .FirstOrDefault()?.Invoke(new object[] { itemRef, itemDef });
                            if (invoke != null)
                                RibbonController.RegisteredControls.Add(itemDef.UUID, invoke);
                        }
                        catch (System.Exception exception)
                        {
                            Debug.WriteLine($"{wrapperType.Name}: {exception.Message}");
                        }
                    }
                }
                return itemRef;
            }
            return null;
        }
    }
}