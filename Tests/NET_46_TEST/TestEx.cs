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
        public void Initialize() {
            RPApp _ = new RPApp(Application.DocumentManager);
        }

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = ComponentManager.Ribbon;
        }

        private PaletteSet _paletteSet;
        [CommandMethod("RP_PROSPECTOR")]
        public void RPProspector()
        {
            if (_paletteSet == null)
            {
                var control = new Projector();
                _paletteSet = new PaletteSet("RoadPAC2")
                {
                    Size = new Size((int)control.Width, (int)control.Height),
                    DockEnabled = DockSides.Left | DockSides.Right,
                    KeepFocus = true
                };

                _paletteSet.AddVisual("Prospector", control);
            }
            _paletteSet.KeepFocus = true;
            _paletteSet.Visible = true;
        }

        public void Terminate()
        { }
    }
}