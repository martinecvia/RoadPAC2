using System.Drawing;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

using Shared;
using Shared.Windows;

[assembly: CommandClass(typeof(NET_48_TEST.TestEx))]
namespace NET_48_TEST
{
    public class TestEx : IExtensionApplication
    {
        private PaletteSet _paletteSet;
        private Projector _projector;
        public void Initialize()
        {
            RPApp app = new RPApp(Application.DocumentManager);
            if (RPApp.Projector.CurrentWorkingDirectory == null)
                RPApp.Projector.CurrentWorkingDirectory = @"C:\TEMP";
        }

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