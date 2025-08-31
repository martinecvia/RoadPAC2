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
