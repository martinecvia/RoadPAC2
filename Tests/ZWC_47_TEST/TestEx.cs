using ZwSoft.Windows;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.Runtime;

using Shared;
using Shared.Windows;
using System.Linq;
using System.Diagnostics;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
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
            window.Show();
        }

        public void Terminate()
        { }
    }
}
