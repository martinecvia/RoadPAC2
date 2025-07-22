#define DEBUG
#define NON_VOLATILE_MEMORY

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers
{
    public class ContextualRibbonTab : RibbonTab
    {
        public bool Show()
        {
            return true;
        }

        public bool Hide()
        {
            return true;
        }
    }
}