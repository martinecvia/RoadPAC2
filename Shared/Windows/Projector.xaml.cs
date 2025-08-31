using WPF = System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Shared.Windows.Models;
using static Shared.Controllers.ProjectController;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
using ApplicationServices = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
using ApplicationServices = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
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
            RPApp.FileWatcher.FileCreated += (o, s) => Debug.WriteLine(s);
        }

        public void RefreshItems() =>
            DataContext = new ProjectorViewModel();

        private void TreeViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Clicked !");
            TreeViewItem item = (TreeViewItem)sender;
            var treeViewItem = FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);
            if (treeViewItem != null)
            {
                // Get the data context (your TreeItem object)
                var treeItem = treeViewItem.DataContext as TreeItem;

                if (treeItem != null)
                {
                    ProjectFile file = treeItem.File;
                    string displayName = treeItem.DisplayName;
                    MessageBox.Show($"Clicked: {displayName} (ID: {file})");
                }
            }
            item.Focusable = true;
            item.Focus();
            item.Focusable = false;
            e.Handled = true;
        }

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
    }
}