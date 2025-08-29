using System.Windows;
using System.Windows.Controls;
using System.Linq;
using static Shared.Controllers.RibbonController;




#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
using ApplicationServices = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.EditorInput;
#else
using Autodesk.Windows;
using ApplicationServices = Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
#endif
#endregion

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
            {
                var a = ComponentManager.Ribbon.Tabs.Where(t => t.Id.StartsWith("RP_TAB_")).FirstOrDefault();
                var b = a.Panels[3];
                b.IsVisible = false;
            }
        }
    }
}