using System.Diagnostics;
using System.Drawing;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.Windows;

using Shared;
using Shared.Windows;

// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-4E1AAFA9-740E-4097-800C-CAED09CDFF12
// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-C3F3C736-40CF-44A0-9210-55F6A939B6F2
[assembly: CommandClass(typeof(NET_46_TEST.TestEx))]
namespace NET_46_TEST
{
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
            RPApp app = new RPApp(Application.DocumentManager);
            if (RPApp.Projector.CurrentWorkingDirectory == null)
                RPApp.Projector.CurrentWorkingDirectory = @"C:\TEMP";
            RPApp.Projector.CurrentWorkingDirectoryChanged += (f, t) => Debug.WriteLine($"CurrentWorkingDirectoryChanged: {f}->{t}");
            RPApp.Projector.CurrentRouteChanged += (f, t) => Debug.WriteLine($"CurrentRouteChanged: {f}->{t}");
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

        public void Terminate()
        { }
    }
}