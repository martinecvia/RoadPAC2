using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

using Shared;

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
        public void Terminate()
        { }
    }
}