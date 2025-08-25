using Shared;
using ZwSoft.Windows;
using ZwSoft.ZwCAD.Runtime;

[assembly: CommandClass(typeof(ZWC_47_TEST.TestEx))]
namespace ZWC_47_TEST
{ 
    public class TestEx : IExtensionApplication
    {
        public void Initialize() => new RPApp();

        [CommandMethod("HIT_BREAKPOINT")]
        public void Breakpoint()
        {
            var Ribbons = ComponentManager.Ribbon;
        }
        public void Terminate()
        { }
    }
}
