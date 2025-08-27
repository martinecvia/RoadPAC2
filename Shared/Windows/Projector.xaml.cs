using System.Windows;
using System.Windows.Controls;

using ApplicationServices = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;

using Shared.Windows.Tree;

namespace Shared.Windows
{
    public partial class Projector : Window
    {
        public Projector()
        {
            InitializeComponent();
            DataContext = new ProjectorViewModel();
        }

        private void MenuItem_Report_Click(object sender, RoutedEventArgs e)
        {
            Editor ed = ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            if (sender is MenuItem menuItem && menuItem.DataContext is TreeItem treeItem)
                ed.WriteMessage($"\nTreeView node: {treeItem.Name}");
        }
    }
}