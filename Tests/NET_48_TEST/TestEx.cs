using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

using Shared;
using Shared.Windows;

[assembly: CommandClass(typeof(NET_48_TEST.TestEx))]
namespace NET_48_TEST
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

        [CommandMethod("WINDOW_TEST")]
        public void WindowTest()
        {
            var window = new Projector();
        }

        public void Terminate()
        { }
    }
}