using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System;
using System.Windows.Threading;
using System.Threading;
using Shared.Controllers.Models.Project;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using AcApp = ZwSoft.ZwCAD.ApplicationServices;
#else
using AcApp = Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

using Shared.Windows.Models;

namespace Shared.Windows
{
    // https://stackoverflow.com/questions/15681352/transitioning-from-windows-forms-to-wpf/15684569#15684569
    // https://through-the-interface.typepad.com/through_the_interface/2009/08/hosting-wpf-content-inside-an-autocad-palette.html
    public partial class Projector : UserControl
    {
        public ProjectorViewModel ViewModel => DataContext as ProjectorViewModel;
        public Projector()
        {
            InitializeComponent();
            if (ViewModel == null)
                DataContext = new ProjectorViewModel();

            PreviewMouseDown += Projector_PreviewMouseDown;
            // Automatically change currently selected route
            if (RPApp.Projector != null)
            {
                RPApp.Projector.CurrentRouteChanged += (_, to) => {
                    if (string.IsNullOrEmpty(to)) return;
                    // Our UI is executed from a different thread
                    RunSynchronized(() => {
                        foreach (TreeItem projectItem in ViewModel.FilteredItems)
                            projectItem.IsActiveRoute = projectItem.File.Root == to;
                    });
                };
            }
        }

        public void RefreshItems() =>
            DataContext = new ProjectorViewModel();
        #region PRIVATE
        [RPPrivateUseOnly]
        private void RunSynchronized(Action callback)
        {
            if (Dispatcher.CheckAccess())
                callback();
            else
                Dispatcher.Invoke(callback);
        }
        #region EVENTS
        [RPPrivateUseOnly]
        private void ClearSearch_Click(object sender, RoutedEventArgs e) => SearchBar.Text = null;

        [RPPrivateUseOnly]
        private void Collapse_Click(object sender, RoutedEventArgs e) => CollapseAll(ProjectTree);

        [RPPrivateUseOnly]
        private void Projector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var target = FindAncestor<TreeViewItem>(Mouse.DirectlyOver as DependencyObject);
                // This means that user have clicked elsewhere
                if (RPApp.Projector != null && target?.DataContext == null)
                {
                    RPApp.Projector.CurrentProjectFile = null;
                    // https://stackoverflow.com/questions/491111/how-to-deselect-all-selected-items-in-a-wpf-treeview-when-clicking-on-some-empty
                    void DeselectAll(TreeItem item)
                    {
                        if (item == null) return;
                        item.IsSelected = false;
                        foreach (TreeItem child in item)
                            DeselectAll(child);
                    }
                    foreach (TreeItem item in ViewModel.FilteredItems)
                        DeselectAll(item);
                    ViewModel.IsTableTable = false;
                    ViewModel.Table = null;
                }
            }
        }

        [RPPrivateUseOnly]
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem menuItem
                && (menuItem.CommandParameter as ContextMenu ?? menuItem.Parent as ContextMenu) is ContextMenu context
                && context.PlacementTarget is TreeViewItem treeView
                && treeView.DataContext is TreeItem treeItem)
            {
                if (RPApp.Projector != null)
                    RPApp.Projector.CurrentProjectFile = treeItem.File;
            }
        }

        [RPPrivateUseOnly]
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (RPApp.Projector == null)
                return;
            if (!(e.NewValue is TreeItem treeItem))
            {
                RPApp.Projector.CurrentProjectFile = null;
                return;
            }
            //if (!treeItem.IsRouteNode)
            RPApp.Projector.CurrentProjectFile = treeItem.File;
            // Table handler
            if (treeItem.File is BaseProjectXml projectFile)
            {
                ViewModel.IsTableTable = true;
                ViewModel.Table = new System.Collections.ObjectModel.ObservableCollection<ProjectorViewModel.GridRows>
                {
                    new ProjectorViewModel.GridRows { Label = "Terrain", Value = projectFile.TerrainModelFile ?? "#NaN" },
                };
            } 
            else { ViewModel.IsTableTable = false; }
            if (treeItem.File?.Root != null)
            {
                // We don't really want to changed current route if it's the same as before
                if (RPApp.Projector.CurrentRoute != null && RPApp.Projector.CurrentRoute == treeItem.File.Root)
                    return;
                RPApp.Projector.CurrentRoute = treeItem.File.Root;
                try
                {
                    RPApp.RDPHelper.CreateConfigRDP(RPApp.Projector.CurrentWorkingDirectory, treeItem.File.Root);
                    foreach (TreeItem projectItem in ViewModel.FilteredItems)
                        projectItem.IsActiveRoute = projectItem.File.Root == treeItem.File.Root;
                }
                finally
                {
                    var _document = AcApp.Application.DocumentManager.MdiActiveDocument;
                    if (_document != null && treeItem.File.Root != null)
                        _document.Editor.WriteMessage(string.Format("Changed current project to: {0}\n", treeItem.File.Root));
                }
            }
        }
        #endregion
        [RPPrivateUseOnly]
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                    return ancestor;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        [RPPrivateUseOnly]
        private void CollapseAll(ItemsControl parent)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    treeViewItem.IsExpanded = false;
                    if (treeViewItem.HasItems)
                        CollapseAll(treeViewItem);
                }
            }
        }
        #endregion
    }
}