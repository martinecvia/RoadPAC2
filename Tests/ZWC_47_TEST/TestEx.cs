using System.Diagnostics;
using System.Linq;

using ZwSoft.Windows;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Runtime;
using ZwSoft.ZwCAD.Windows;

using Shared;
using Shared.Windows;

using System.Drawing;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
{ 
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
            RPApp app = new RPApp(Application.DocumentManager);
            if (RPApp.Projector.CurrentWorkingDirectory == null)
                RPApp.Projector.CurrentWorkingDirectory = @"C:\TEMP";
            RPApp.Projector.CurrentWorkingDirectoryChanged += (o) => Debug.WriteLine($"CurrentWorkingDirectoryChanged: {o}");
            RPApp.Projector.CurrentRouteChanged += (o) => Debug.WriteLine($"CurrentRouteChanged: {o}");
            RPApp.Projector.ProjectChanged += (o) => Debug.WriteLine($"ProjectChanged: {o}");
            RPApp.Projector.CurrentProjectFileChanged += (o) => Debug.WriteLine($"ProjectFileSelected: {o}");

            RPApp.FileWatcher.FileChanged += (s, o) => Debug.WriteLine($"FileChanged: {s}{o}");
            RPApp.FileWatcher.FileRenamed += (s, o, l) => Debug.WriteLine($"FileRenamed: {s}{l}->{o}");
            RPApp.FileWatcher.FileCreated += (s, o) => Debug.WriteLine($"FileCreated: {s}{o}");
            RPApp.FileWatcher.FileDeleted += (s, o) => Debug.WriteLine($"FileDeleted: {s}{o}");
        }

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = ComponentManager.Ribbon;
        }
        private PaletteSet _paletteSet;
        private Projector _projector;
        [CommandMethod("RP_PROSPECTOR")]
        public void RPProspector()
        {
            if (_paletteSet == null)
            {
                _projector = new Projector();
                _paletteSet = new PaletteSet($"RoadPAC2 / {RPApp.Projector.CurrentWorkingDirectory}")
                {
                    Size = new Size((int)_projector.Width, (int)_projector.Height),
                    DockEnabled = DockSides.Left | DockSides.Right,
                    KeepFocus = true
                };

                _paletteSet.AddVisual("Prospector", _projector);
            }
            _paletteSet.Name = $"RoadPAC2 / {RPApp.Projector.CurrentWorkingDirectory}";
            _projector.RefreshItems();
            _paletteSet.KeepFocus = true;
            _paletteSet.Visible = true;
        }

        [CommandMethod("RP_RIBBONVIEW")]
        public void RibbonView()
        {
            Shared.Controllers.RibbonController.CreateTab("rp_RoadPAC");
        }

        public void Terminate()
        { }
    }
}
