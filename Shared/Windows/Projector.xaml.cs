using WPF = System.Windows.Controls;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ApplicationServices = ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Windows;
#else
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
            DataContext = new ProjectorViewModel();
        }
    }
}