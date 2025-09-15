using System.Drawing;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

using Shared;
using Shared.Windows;

// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-4E1AAFA9-740E-4097-800C-CAED09CDFF12
// https://help.autodesk.com/view/ACD/2017/ENU/?guid=GUID-C3F3C736-40CF-44A0-9210-55F6A939B6F2
[assembly: CommandClass(typeof(NET_46_TEST.TestEx))]
namespace NET_46_TEST
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