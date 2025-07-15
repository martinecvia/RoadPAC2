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

        private static void AssertInitialized()
        {
            if (Ribbon != null)
                throw new InvalidOperationException("Ribbon is not ready.");
        }

        public static void CreateContextualTab(string tabId, string tabName,
                                               string groupId, string groupName, Color? groupColor,
                                               Func<SelectionSet, bool> onSelectionMatch)
        {
            AssertInitialized();
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

        private class CommandHandler : System.Windows.Input.ICommand
        {
            private readonly string _command;
            public CommandHandler(string command) => _command = command;

            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                Document document = Application.DocumentManager.MdiActiveDocument;
                if (document != null)
                {
                    document.SendStringToExecute(_command + " ", true, false, false); 
                }
            }
        }

        internal class ContextualRibbonTab : RibbonTab
        {
            
        }
    }
}