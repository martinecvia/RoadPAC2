#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8622

#pragma warning disable IDE0028 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8
#pragma warning disable IDE0090 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8
#pragma warning disable IDE0305 // Simplifications cannot be made because of multiversion between .NET 4 and .NET 8

#define DEBUG
#define NON_VOLATILE_MEMORY

using System; // Keep for .NET 4.6
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
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
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models;
using Shared.Controllers.Models.RibbonXml;
using Shared.Controllers.Models.RibbonXml.Items;

namespace Shared.Controllers
{
    public static class RibbonController
    {
        [RPInternalUseOnly]
        internal static readonly string HasAnyContextualTabPropertyName = "HasAnyContextualTab";
        [RPInternalUseOnly]
        internal static readonly string IsSelectionHandledPropertyName = "IsSelectionHandled";

        private const string RibbonTab__Prefix = "RP_TAB_"; // RoadPAC prefix for tabs. so we can distinguish
                                                            // other tab's from AutoCAD.
                                                            // This also prevents using the same name from different applications.
        private const string RibbonGroupPrefix = "RP_GRP_";

        private static RibbonControl Ribbon => ComponentManager.Ribbon; // Should be same with ZWCAD

        [DefaultValue(false)]
        public static bool HasAnyContextualTab { get; private set; } = false;

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

        [RPPrivateUseOnly]
        private static T CreateTab<T>(string tabId,
#if NET8_0_OR_GREATER
                                      string? tabName = null,
                                      string? tabDescription = null) where T: RibbonTab, new()
#else
                                      string tabName = null,
                                      string tabDescription = null) where T : RibbonTab, new()
#endif
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            RibbonTabDef resource = ResourceController.LoadResourceRibbon<RibbonTabDef>(tabId);
            RibbonTab tab = resource?.Transform(new RibbonTab()) ?? new RibbonTab
            {
                Id = RibbonTab__Prefix + tabId       // We want to mark these tabs as RoadPAC ones.
                                                     // For further compatibility and to prevent being overriden.
            };
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
                        {
                            if (def1.SourceDef != null && def1.ItemsDef.Count != 0) // Source can't be set when Items is not empty.
                            {
                                foreach (var itemDef in def1.ItemsDef)
                                    def1.SourceDef.ItemsDef.Add(itemDef);           // To avoid InvalidOperationException we are effectively transferring everything to SubSource instead
                                foreach (var itemDef in def1.SourceDef.ItemsDef)
                                {
                                    if (itemDef is RibbonRowPanelDef || itemDef is RibbonPanelBreakDef)
                                        continue;
                                    ((RibbonRowPanel)itemRef).Source.Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                                }
                            }
                            else
                            {
                                foreach (var itemDef in def1.ItemsDef)
                                {
                                    if (itemDef is RibbonRowPanelDef || itemDef is RibbonPanelBreakDef)
                                        continue; // The following item types are not supported in this collection: RibbonRowPanel and RibbonPanelBreak.
                                                    // An exception is thrown if these objects are added to the collection.
                                    ((RibbonRowPanel)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                                }
                            }
                            continue; // We don't want to halt system with other if-checks
                        }
                        if (item is RibbonListDef def2)
                            foreach (var itemDef in def2.ItemsDef)
                                ((RibbonList)itemRef).Items.Add(itemDef.Transform(RibbonItemDef.ItemsFactory[itemDef.GetType()]()));
                        panelRef.Source.Items.Add(itemRef);
                        Debug.WriteLine($"Registering: {item}");
                    }
                    tab.Panels.Add(panelRef);
                }
            }
            tab.IsEnabled = true;
            if (!string.IsNullOrEmpty(tabName) || string.IsNullOrEmpty(tab.Name))
                tab.Name = tabName ?? tabId; tab.Title = tabName ?? tab.Name;
            if (!string.IsNullOrEmpty(tabDescription))
                tab.Description = tabDescription;
            Ribbon.Tabs.Add(tab);
            return (T) tab;
        }

        [RPPrivateUseOnly]
        private static readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions = new Dictionary<string, Func<SelectionSet, bool>>();

        public static RibbonTab CreateTab(string tabId,
#if NET8_0_OR_GREATER
            string? tabName = null,
            string? tabDescription = null)
#else
            string tabName = null,
            string tabDescription = null)
#endif
        => CreateTab<RibbonTab>(tabId, tabName, tabDescription);

        /// <summary>
        /// Creates a contextual ribbon tab that is shown conditionally based on the current selection in the drawing.
        /// </summary>
        /// <param name="tabId">The identifier of the ribbon tab resource (used to load its definition).</param>
        /// <param name="onSelectionMatch">
        /// A delegate that determines whether this contextual tab should be shown based on the current <see cref="SelectionSet"/>.
        /// </param>
        /// <param name="tabName">Optional override for the tab's display name.</param>
        /// <param name="tabDescription">Optional description shown in tooltips or documentation.</param>
        /// <returns>
        /// A <see cref="ContextualRibbonTab"/> instance configured to show conditionally during selection changes.
        /// </returns>
        /// <remarks>
        /// This method ensures contextual behavior is registered only once by attaching to <see cref="Document.ImpliedSelectionChanged"/>.
        /// The created tab is hidden by default and marked as anonymous to allow manual visibility control via <see cref="RibbonTab.IsVisible"/>.
        /// </remarks>
        public static ContextualRibbonTab CreateContextualTab(string tabId, 
            Func<SelectionSet, bool> onSelectionMatch, // Selector switch when this tab should be opened
#if NET8_0_OR_GREATER
            string? tabName = null,
            string? tabDescription = null)
#else
            string tabName = null,
            string tabDescription = null)
#endif
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            RibbonTabDef xml = ResourceController.LoadResourceRibbon<RibbonTabDef>(tabId);
            ContextualRibbonTab tab = CreateTab<ContextualRibbonTab>(tabId, tabName, tabDescription);
            _contextualTabConditions.Add(tab.Id, onSelectionMatch);
            if (!HasAnyContextualTab)
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                document.ImpliedSelectionChanged += OnSelectionChanged;
                HasAnyContextualTab = true;
            }
            tab.IsVisible = false;
            tab.IsAnonymous = true;             // This is crucial, since Ribbon#ShowContextualTab() is broken
                                                // because it disallows user to "intentionaly" show this tab, thus
                                                // RibbonTab#IsVisible property will be use to show or hide contextual tab
            return tab;
        }

        /// <summary>
        /// Gets a value indicating whether the current selection event has already been processed.
        /// </summary>
        /// <remarks>
        /// This flag is used internally to prevent redundant handling of selection changes
        /// and to allow external code to wait until the contextual ribbon tab becomes visible.
        ///
        /// This property is public to allow UI workflows that delay actions until the contextual tab is activated.
        /// </remarks>
        [DefaultValue(false)]
        public static bool IsSelectionHandled { get; private set; } = false;
        // public - to allow program to wait for property change, so user will see Contextual tab first

        [RPPrivateUseOnly]
        private static void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (IsSelectionHandled) // Sometimes events are fired multiple times per-say
                                    // so this should prevent any "unwanted" events after the first one.
                                    // Effectively for performance reasons and stutters
                return;
            if (eventArgs == null)  // Case that happens when AutoCAD's main thread is occupied
                                    // and event was fired in the middle of cleaning up databases
                                    // [bug at: Autodesk AutoCAD 2017 #11387]
                return;
            IsSelectionHandled = true;
            Document document = Application.DocumentManager.MdiActiveDocument;
            var result = document.Editor.SelectImplied();
            if (result.Status != PromptStatus.OK || result.Value == null || result.Value.Count == 0)
            {
                // Hide all contextual tabs
                // Logic should be further refined in future to keep tab open
                // if we want to, for now - all tabs will be closed after de-selection
                foreach (var tab in Ribbon.Tabs.Where(t => t is ContextualRibbonTab
                        && t.Id.StartsWith(RibbonTab__Prefix) 
                        && t.IsVisible))
                {
                    ((ContextualRibbonTab) tab).Hide();
                    Ribbon.UpdateLayout();
                    IsSelectionHandled = false;
                    return;
                }
            }
            var selection = result.Value;
            if (selection != null)
            {
                foreach (KeyValuePair<string, Func<SelectionSet, bool>> pair in _contextualTabConditions)
                {
                    if (pair.Value == null)
                        continue; // If for some reason Func<SelectionSet, bool>> will be null during tab creation
                                  // we will just skip handling this tab and treat it as normal one
                    if (pair.Value.Invoke(selection))
                    {
                        var tab = Ribbon.Tabs.FirstOrDefault(t => t.Id == pair.Key);
                        if (tab != null && tab is ContextualRibbonTab selected)
                        {
                            selected.Show();
                            Ribbon.UpdateLayout();
                            IsSelectionHandled = false;
                            return;
                        }
                    }
            }
            }
            // Release the lock
            IsSelectionHandled = false;
        }
    }
}