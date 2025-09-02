using WPF = System.Windows.Controls;

using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Shared.Windows.Models;
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
    public partial class Projector : WPF.UserControl
    {
        public ProjectorViewModel ViewModel => DataContext as ProjectorViewModel;
        public Projector()
        {
            InitializeComponent();
            if (ViewModel == null)
                DataContext = new ProjectorViewModel();
        }

        public void RefreshItems() =>
            DataContext = new ProjectorViewModel();
        #region PRIVATE
        #region EVENTS
        [RPPrivateUseOnly]
        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectorViewModel viewModel)
                viewModel.SearchText = string.Empty;
        }

        [RPPrivateUseOnly]
        private void CollapseExceptSelected_Click(object sender, RoutedEventArgs e) =>
            CollapseAllExcept(ProjectTree, ProjectTree.SelectedItem);

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
            RPApp.Projector.CurrentProjectFile = e.NewValue is TreeItem treeItem 
                ? treeItem.File 
                : null;
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

        [RPPrivateUseOnly]
        private void CollapseAllExcept(ItemsControl parent, object except)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem)
                {
                    bool IsExceptItem()
                    {
                        if (treeViewItem.DataContext == except)
                            return true;
                        if (treeViewItem == except)
                            return true;
                        return false;
                    }
                    if (!IsExceptItem())
                        treeViewItem.IsExpanded = false;

                    if (treeViewItem.HasItems)
                        CollapseAllExcept(treeViewItem, except);
                }
            }
        }
        #endregion
    }
}