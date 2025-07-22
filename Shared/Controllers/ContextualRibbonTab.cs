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
        public void Show() => IsVisible = true;

        public void Hide() => IsVisible = false;
    }
}