#define DEBUG
#define NON_VOLATILE_MEMORY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
#endif
#endregion

namespace RoadPAC
{
    public static class RibbonController
    {
        private static readonly Dictionary<string, Func<SelectionSet, bool>> _contextualTabConditions = new Dictionary<string, Func<SelectionSet, bool>>();

        private const string RibbonTab__Prefix = "CTX_";
        private const string RibbonGroupPrefix = "GRP_";

#if ZWCAD
        private static RibbonControl Ribbon => ComponentManager.Ribbon;
#else
        private static RibbonControl Ribbon => ComponentManager.Ribbon;
#endif

        [RoadPACUseOnly] // We don't want to allow fuzzing with memory, this is just a nice case how to avoid retarded cases
        private static void AssertInitialized()
        {
            // Actively disabling reflection use cases
            if (Ribbon != null)
                throw new InvalidOperationException("Ribbon can't be loaded using reflection.");
        }

        public static RibbonTab CreateTab(string tabId, string tabName,
                                    string groupId, string groupName, Color? groupColor)
        {
            AssertInitialized();
            RibbonTab tab = new RibbonTab();
            tab.IsContextualTab = false;
            return tab;
        }

        public static RibbonTab CreateContextualTab(string tabId, string tabName,
                                               string groupId, string groupName, Color? groupColor,
                                               Func<SelectionSet, bool> onSelectionMatch)
        {
            AssertInitialized();
            RibbonTab tab = new RibbonTab();
            tab.IsContextualTab = true; // Hard-marking that this tab is contextual-only
            return tab;
        }

        private static void OnSelectionChanged(object sender, SelectionAddedEventArgs eventArgs)
        {
            Document document = Application.DocumentManager.MdiActiveDocument;
            Editor editor = document.Editor;
            PromptSelectionResult result = editor.SelectImplied();
            if (result.Status != PromptStatus.OK || result.Value.Count == 0)
            {
                foreach (RibbonTab tab in Ribbon.Tabs.Where(t => t.IsContextualTab 
                    && t.Id.StartsWith(RibbonTab__Prefix)))
                {
                    tab.IsVisible = false;
                    tab.IsActive = false;
                }
                return;
            }
            SelectionSet selection = result.Value;
            foreach (KeyValuePair<string, Func<SelectionSet, bool>> pair in _contextualTabConditions)
            {
                if (pair.Value(selection))
                {
                    RibbonTab tab = Ribbon.Tabs.FirstOrDefault(t => t.Id == pair.Key);
                    if (tab != null && tab.IsContextualTab)
                    {
                        tab.IsVisible = true;
                        tab.IsActive = true;
                    }
                }
            }
            foreach(RibbonTab tab in Ribbon.Tabs.Where(t => t.IsContextualTab
                    && t.Id.StartsWith(RibbonTab__Prefix)))
            {
                tab.IsVisible = false;
                tab.IsActive = false;
            }
        }

        /// <summary>
        /// A simple implementation of the <see cref="System.Windows.Input.ICommand"/> interface
        /// that executes a given AutoCAD command string when invoked.
        /// </summary>
        internal sealed class CommandHandler : System.Windows.Input.ICommand
        {
            private readonly string _command;

            /// <summary>
            /// Initializes a new instance of the <see cref="CommandHandler"/> class with the specified command string.
            /// </summary>
            /// <param name="command">The AutoCAD command string to be executed.</param>
            public CommandHandler(string command) => _command = command;

            /// <inheritdoc/>
            public event EventHandler CanExecuteChanged;

            /// <summary>
            /// Determines whether the command can execute in its current state.
            /// Always returns <c>true</c> in this implementation.
            /// </summary>
            /// <param name="parameter">Unused parameter.</param>
            /// <returns><c>true</c> to indicate the command can always execute.</returns>
            public bool CanExecute(object parameter) => true;

            /// <summary>
            /// Executes the stored AutoCAD command by sending it to the active document.
            /// </summary>
            /// <param name="parameter">Unused parameter.</param>
            public void Execute(object parameter)
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                if (document != null)
                {
                    // Sends the command to AutoCAD for execution in the command line
                    document.SendStringToExecute(_command + " ", true, false, false);
                }
            }
        }

        internal class ContextualRibbonTab : RibbonTab
        {
            
        }
    }
}