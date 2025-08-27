using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
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

        [CommandMethod("WINDOW_TEST")]
        public void WindowTest()
        {
            var window = new Projector();
        }

        public void Terminate()
        { }
    }
}