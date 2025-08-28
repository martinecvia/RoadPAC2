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
            RPApp.FileWatcher.FileCreated += (p, f) => Debug.WriteLine($"{f} created");
            RPApp.FileWatcher.FileChanged += (p, f) => Debug.WriteLine($"{f} changed");
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

        [CommandMethod("TEST_FILES")]
        public void TestFiles()
        {
            var files = RPApp.FileWatcher.Files.Values.FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(string.Join(",", files));
        }

        public void Terminate()
        { }
    }
}
