#pragma warning disable CS8600, CS8602, CS8618, CS8622, CS8625
#pragma warning disable IDE0001, IDE0028, IDE0083, IDE0090, IDE0300, IDE0305

#define DEBUG
#define NON_VOLATILE_MEMORY

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.Diagnostics;
using System.Linq; // Keep for .NET 4.6
using System.Reflection;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.Windows;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
#if !NET8_0_OR_GREATER
using Autodesk.Internal.Windows;
#endif
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

namespace Shared.Controllers
{
    // https://adndevblog.typepad.com/autocad/2012/04/displaying-a-contextual-tab-upon-entity-selection-using-ribbon-runtime-api.html
    public static class RibbonController
    {
        public const string ControlsNamespace = "Shared.Controllers.Controls.Ribbon";
        public const string RibbonTab__Prefix = "RP_TAB_";  // RoadPAC prefix for tabs. so we can distinguish
                                                            // other tab's from AutoCAD.
                                                            // This also prevents using the same name from different applications.

        private static RibbonControl Ribbon => ComponentManager.Ribbon;
        private static bool _hasContextual = false;
        private static readonly Dictionary<string, object> _registeredControls = new Dictionary<string, object>();
        private static readonly Dictionary<string, ContextualRibbonTab> _activeContextualTabs = new Dictionary<string, ContextualRibbonTab>();
        private static readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions 
            = new Dictionary<string, Func<SelectionSet, bool>>();

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
                throw new InvalidOperationException("Ribbon can't be loaded using reflection or before application initializes properly.");
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
            if (Ribbon == null)
                return new T();
            RibbonTabDef tabDef = ResourceController.LoadResourceRibbon<RibbonTabDef>(tabId);
            T tab = tabDef?.Transform(new T()) ?? new T();
            if (Ribbon.Tabs.Contains(tab))
                return tab; // We really don't want to process same tab multiple times,
                            // there is no point in that
            Ribbon?.Tabs.Add(tab);
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
                    panelRef.UID = panelDef.Id; // For some reason panel can't have Id
                    RegisterControl(panelDef, panelRef);
                    if (panelDef.SourceDef == null)
                        continue;
                    tab.Panels.Add(panelRef);
                    panelDef.SourceDef.Cookie = panelDef.SourceDef.Cookie.Replace("%Parent", cookie);
                    cookie += $";{panelDef.SourceDef.Id}";
                    panelRef.Source = panelDef.SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[panelDef.SourceDef.GetType()]());
                    RegisterControl(panelDef.SourceDef, panelRef.Source);
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
            if (!string.IsNullOrEmpty(tabDescription))
                tab.Description = tabDescription;
            tab.UID = tab.Id;
            if (tabDef != null)
                RegisterControl(tabDef, tab);
            return tab;
        }

        public static ContextualRibbonTab CreateContextualTab(string tabId)
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            if (_activeContextualTabs.ContainsKey(RibbonTab__Prefix + tabId))
                return _activeContextualTabs[RibbonTab__Prefix + tabId];
            ContextualRibbonTab tab = (ContextualRibbonTab) Ribbon?.Tabs?
                .FirstOrDefault(t => t is ContextualRibbonTab _contextualTab && t.Id == RibbonTab__Prefix + tabId);
            if (tab != null)
                return tab;
            tab = CreateTab<ContextualRibbonTab>(tabId);
            tab.IsVisible = false;
            tab.IsContextualTab = true;
            tab.IsSelectionTab = false;
            ApplyOlderTheme(tab);
            // This protects stack agains multiple instances of Idle event registration
            if (!_hasContextual)
            {
                Application.Idle += OnApplicationIdle;
                _hasContextual = true;
            }
            return tab;
        }

        public static ContextualRibbonTab CreateContextualTab(string tabId, Func<SelectionSet, bool> onSelectionMatch)
        {
            ContextualRibbonTab tab = CreateContextualTab(tabId);
            tab.IsSelectionTab = true;
            if (_contextualTabConditions.Count == 0)
                Application.DocumentManager.MdiActiveDocument.ImpliedSelectionChanged += OnSelectionChanged;
            if (!_contextualTabConditions.ContainsKey(tab.Id))
                _contextualTabConditions.Add(tab.Id, onSelectionMatch);
            return tab;
        }

        [RPInternalUseOnly]
        internal static void HideContextualTab(string _contextualId)
        {
            if (_activeContextualTabs.ContainsKey(_contextualId))
            {
                var _contextualTab = _activeContextualTabs[_contextualId];
                if (_contextualTab is ContextualRibbonTab)
                {
                    Ribbon?.HideContextualTab(_contextualTab);
                    _contextualTab.IsVisible = false;
                    _activeContextualTabs.Remove(_contextualTab.Id);
                }
            }
        }

        [RPInternalUseOnly]
        internal static void ShowContextualTab(string _contextualId)
        {
            if (_activeContextualTabs.ContainsKey(_contextualId) 
                && _activeContextualTabs[_contextualId] is ContextualRibbonTab _contextualTab)
            {
                Ribbon?.ShowContextualTab(_contextualTab, false, true);
                _contextualTab.IsActive = true;
                return;
            }
            _contextualTab = (ContextualRibbonTab) Ribbon?.Tabs?
                .FirstOrDefault(t => t.Id == _contextualId && t is ContextualRibbonTab);
            if (_contextualTab != null)
                _activeContextualTabs.Add(_contextualId, _contextualTab);
        }

        [RPInternalUseOnly]
        internal static void HideContextualTab(ContextualRibbonTab _contextualTab)
            => HideContextualTab(_contextualTab.Id);

        [RPInternalUseOnly]
        internal static void ShowContextualTab(ContextualRibbonTab _contextualTab)
            => ShowContextualTab(_contextualTab.Id);

        [RPPrivateUseOnly]
        private static void OnApplicationIdle(object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
            foreach (var _contextualTab in _activeContextualTabs.Values)
            {
                if (!_contextualTab.IsContextualTab)
                {
                    _contextualTab.IsVisible = true;
                    _contextualTab.IsContextualTab = true;
                }
                if (!_contextualTab.IsVisible)
                {
                    Ribbon?.ShowContextualTab(_contextualTab, false, true);
                    _contextualTab.IsActive = true;
                }
            }
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
                foreach (var Id in _contextualTabConditions.Keys)
                {
                    var _contextualTab = Ribbon?.Tabs?.FirstOrDefault(t => t.Id == Id);
                    if (_contextualTab != null)
                    {
                        Ribbon?.HideContextualTab(_contextualTab);
                        _contextualTab.IsVisible = false;
                        _activeContextualTabs.Remove(Id);
                    }
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
                    ContextualRibbonTab _contextualTab;
                    if (_activeContextualTabs.ContainsKey(pair.Key)) _contextualTab = _activeContextualTabs[pair.Key];
                    else _contextualTab = (ContextualRibbonTab) Ribbon?.Tabs?.FirstOrDefault(t => t.Id == pair.Key && t is ContextualRibbonTab);
                    if (_contextualTab == null) // We dont need to draw or loop for tabs that does not exists anymore
                        continue;
                    if (pair.Value.Invoke(selection))
                    {
                        if (!_activeContextualTabs.ContainsKey(pair.Key))
                            _activeContextualTabs.Add(pair.Key, _contextualTab);
                    } 
                    else
                    {
                        Ribbon?.HideContextualTab(_contextualTab);
                        _contextualTab.IsVisible = false;
                        _activeContextualTabs.Remove(pair.Key);
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
                RegisterControl(itemDef, itemRef);
                return itemRef;
            }
            return null;
        }

        [RPPrivateUseOnly]
        private static void RegisterControl(BaseRibbonXml itemDef, object itemRef)
        {
            if (!string.IsNullOrEmpty(itemDef.UUID) && !_registeredControls.ContainsKey(itemDef.UUID))
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
                            _registeredControls.Add(itemDef.UUID, invoke);
                    }
                    catch (System.Exception)
                    { }
                }
            }
        }

        [RPPrivateUseOnly]
        private static void ApplyOlderTheme(ContextualRibbonTab tab)
        {
#if !NET8_0_OR_GREATER && !ZWCAD
            T CloneBrush<T>(T brush) where T : class
            => brush == null ? null : (brush as dynamic).Clone();
            // Older AutoCAD versions have bug with template,
            // so it will just pick random template in the current list and apply it to context
            // My fix is that I'll copy Theme from a Hatch tab
            RibbonTab slave = Ribbon?.Tabs?.FirstOrDefault(t => t.IsContextualTab
                && (t.Name == "Hatch Editor" || t.Id == "ACAD.RBN_01738148"
                || t.Name == "Vytváøení šraf"));
            if (tab.Theme == null
                && slave?.Theme is TabTheme theme)
            {
                tab.Theme = new TabTheme
                {
                    InnerBorder = CloneBrush(theme.InnerBorder),
                    OuterBorder = CloneBrush(theme.OuterBorder),
                    PanelBackground = CloneBrush(theme.PanelBackground),
                    PanelBackgroundVerticalLeft = CloneBrush(theme.PanelBackgroundVerticalLeft),
                    PanelBackgroundVerticalRight = CloneBrush(theme.PanelBackgroundVerticalRight),
                    PanelBorder = CloneBrush(theme.PanelBorder),
                    PanelDialogBoxLauncherBrush = CloneBrush(theme.PanelDialogBoxLauncherBrush),
                    PanelSeparatorBrush = CloneBrush(theme.PanelSeparatorBrush),
                    PanelTitleBackground = CloneBrush(theme.PanelTitleBackground),
                    PanelTitleBorderBrushVertical = CloneBrush(theme.PanelTitleBorderBrushVertical),
                    PanelTitleForeground = CloneBrush(theme.PanelTitleForeground),
                    RolloverTabHeaderForeground = CloneBrush(theme.RolloverTabHeaderForeground),
                    SelectedTabHeaderBackground = CloneBrush(theme.SelectedTabHeaderBackground),
                    SelectedTabHeaderForeground = CloneBrush(theme.SelectedTabHeaderForeground),
                    SlideoutPanelBorder = CloneBrush(theme.SlideoutPanelBorder),
                    TabHeaderBackground = CloneBrush(theme.TabHeaderBackground)
                };
            }
#endif
        }

        public class ContextualRibbonTab : RibbonTab
        {
            public bool IsSelectionTab { get; set; } = false;
        }
    }
}