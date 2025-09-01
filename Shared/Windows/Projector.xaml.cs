using WPF = System.Windows.Controls;

using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Shared.Windows.Models;

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
                RPApp.Projector?.PublishProjectFileSelected(
                    target?.DataContext is TreeItem treeItem ? treeItem.File : null
                );
            }
        }

        [RPPrivateUseOnly]
        private void MenuTest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem
                && (menuItem.CommandParameter as ContextMenu ?? menuItem.Parent as ContextMenu) is ContextMenu ctx
                && ctx.PlacementTarget is TreeViewItem tvi
                && tvi.DataContext is TreeItem treeItem)
            {
                RPApp.Projector?.PublishProjectFileSelected(treeItem.File);
            }
        }
        #endregion
        [RPPrivateUseOnly]
        private bool IsAncestorOf<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T ancestor)
                    return true;
                current = VisualTreeHelper.GetParent(current);
            }
            return false;
        }

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