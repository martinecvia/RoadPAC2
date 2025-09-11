using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Shared.Windows.Models;
using System.Windows.Input;
using System.Diagnostics;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ApplicationServices = ZwSoft.ZwCAD.ApplicationServices;
#else
using ApplicationServices = Autodesk.AutoCAD.ApplicationServices;
#endif
#endregion

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
        }

        public void RefreshItems() =>
            DataContext = new ProjectorViewModel();
        #region PRIVATE
        #region EVENTS
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
                }
            }
        }

        [RPPrivateUseOnly]
        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectorViewModel viewModel)
                viewModel.SearchText = string.Empty;
        }

        [RPPrivateUseOnly]
        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            void CollapseAll(ItemsControl parent)
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
            CollapseAll(ProjectTree);
        }

        [RPPrivateUseOnly]
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem
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

            if (e.NewValue is TreeItem treeItem)
            {
                if (treeItem.IsRouteNode)
                {
                    // tohle je jen rychlej fix, chci aby se při kliknutí na název trasy nezobrazoval kontextový panel pro směr
                    // pokud jde zařídit lépe, udělej jinak
                    RPApp.Projector.CurrentProjectFile = null;
                    return;
                }
                
                RPApp.Projector.CurrentProjectFile = treeItem.File;
            }
            else
            {
                RPApp.Projector.CurrentProjectFile = null;
            }
        }
        #endregion
        [RPPrivateUseOnly]
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                    return ancestor;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        #endregion
    }
}