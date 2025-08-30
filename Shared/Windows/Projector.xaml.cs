using WPF = System.Windows.Controls;
using System.Diagnostics;


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
            RPApp.FileWatcher.FileCreated += (s, o) => Debug.WriteLine("FileCreated");
            RPApp.FileWatcher.FileDeleted += (s, o) => Debug.WriteLine("FileDeleted");
            RPApp.FileWatcher.FileRenamed += (s, o, l) => Debug.WriteLine("FileRenamed");
            RPApp.Projector.ProjectChanged += (s) => Debug.WriteLine("ProjectChanged");
        }

        public void Show() =>
            DataContext = new ProjectorViewModel();
    }
}