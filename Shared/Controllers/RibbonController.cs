#define DEBUG
#define NON_VOLATILE_MEMORY

#define INTERNALS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;

#if INTERNALS
using Autodesk.Internal.Windows;
#endif
using Autodesk.Windows;
#endif
using Shared.Controllers.Models;
#endregion

// [RibbonTab]
// - Description:
// Gets or sets a description text for the tab.
// The description text is not currently used by the framework. Applications can use this to store a description if it is required in other UI customization dialogs. The default value is null.

// - Highlight:
// Sets and gets the current highlight mode that indicates the display status of the New Badge on the upper-right corner of the ribbon tab.

// - Id:
// The framework does not use or validate this id. It is left to the applications to set this id and use it.
// The default value is null.

// - IsActive:
// Gets or sets the value that indicates whether this tab is the active tab.
// Hidden tabs and merged contextual tabs cannot be the active tab. Setting this property to true for such tabs will fail, and no exception will be thrown.     

// - IsContextualTab:
// Assesses whether the tab is regular tab or contextual tab. If it is true the tab is contextual tab, and false if it is regular tab. This is a dependency property registered with WPF. Please see the Microsoft API for more information.
// The default value is false.

// - IsVisible:
// Gets or sets the value that indicates whether the tab is visible in the ribbon.
// If the value is true, the tab is visible in the ribbon. If the value is false, it is hidden in ribbon. Both visible and hidden tabs are available in the ribbon by right-clicking the menu under the Tabs menu option, which allows the user to show or hide the tabs.
// If the tab's IsAnonymous property is set to false, it is not included in the right-click menu, and the user cannot control its visibility.
// If an active tab is hidden, the next or previous visible tab is set as the active tab.
// The default value is true.

// - IsVisited:
// Gets or sets whether the panel tab is visited.

// - KeyTip:
// Gets or sets the keytip for the tab.
// Keytips are displayed in the ribbon when navigating the ribbon with the keyboard. If this property is null or empty, the keytip will not appear for this tab, and the tab will not support activation through the keyboard. 
// The default value is null.

// - Name:
// Gets or sets the name of the ribbon tab.

// - Tag:
// Gets or sets custom data object in the tab.

// - Title:
// Gets or sets the tab title. The title set with this property is displayed in the tab button for this tab in the ribbon.
// The default value is null.

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

        public static RibbonTab CreateTab(string tabId, string tabName,
                                          string tabDescription = null)
        {
#if NON_VOLATILE_MEMORY
            AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            Assert.IsNotNull(tabName, nameof(tabName));
            var tab = new RibbonTab
            {
                Id = RibbonTab__Prefix + tabId, // We want to add mark those tabs as RoadPAC ones´.
                                                // For further compatibility and to prevent being overriden.
                Name = tabName,
                Title = tabName,
                Description = tabDescription,
                IsEnabled = true
            };
            return tab;
        }

        [RPPrivateUseOnly]
        private static readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions = new Dictionary<string, Func<SelectionSet, bool>>();

        public static ContextualRibbonTab CreateContextualTab(string tabId, string tabName, 
            Func<SelectionSet, bool> onSelectionMatch, // Selector switch when this tab should be opened
            string tabDescription = null)
        {
#if NON_VOLATILE_MEMORY
           AssertInitialized();
#endif
            Assert.IsNotNull(tabId, nameof(tabId));
            Assert.IsNotNull(tabName, nameof(tabName));
            var tab = new ContextualRibbonTab
            {
                Id = RibbonTab__Prefix + tabId, // We want to add mark those tabs as RoadPAC ones´.
                                                // For further compatibility and to prevent being overriden.
                Name = tabName,
                Title = tabName,
                Description = tabDescription,
                IsEnabled = true,
                IsAnonymous = true,             // This is crucial, since Ribbon#ShowContextualTab() is broken
                                                // because it disallows user to "intentionaly" show this tab, thus
                                                // RibbonTab#IsVisible property will be use to show or hide contextual tab
                IsVisible = false
            };
            _contextualTabConditions.Add(tab.Id, onSelectionMatch);
            if (!HasAnyContextualTab)
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                document.ImpliedSelectionChanged += OnSelectionChanged;
                HasAnyContextualTab = true;
            }
            return tab;
        }

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
            // Release the lock
            IsSelectionHandled = false;
        }
    }
}