#define DEBUG
#define NON_VOLATILE_MEMORY

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

// https://adndevblog.typepad.com/autocad/2012/04/displaying-a-contextual-tab-upon-entity-selection-using-ribbon-runtime-api.html
namespace Shared.Controllers.Models
{
    public class ContextualRibbonTab : RibbonTab
    {
        public void Show() => IsVisible = true;

        public void Hide() => IsVisible = false;
    }
}