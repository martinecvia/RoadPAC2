using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

using Shared;

[assembly: CommandClass(typeof(NET_80_TEST.TestEx))]
namespace NET_80_TEST
{
    public class TestEx : IExtensionApplication
    {
        public void Initialize()
        {
            using (var application = new RPApp()) ;
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