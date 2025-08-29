using System.Drawing;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.Windows;

using Shared;
using Shared.Windows;

[assembly: CommandClass(typeof(NET_80_TEST.TestEx))]
namespace NET_80_TEST
{
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
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