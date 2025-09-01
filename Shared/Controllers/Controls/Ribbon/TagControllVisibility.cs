#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml;

namespace Shared.Controllers.Controls.Ribbon
{
    internal class TagControllVisibility : BaseRibbonControl<RibbonItem>
    {
        public TagControllVisibility(RibbonItem target, BaseRibbonXml source) : base(target, source)
        {
            RPApp.Projector.ProjectFileSelected += (selected) =>
            {
                if (selected == null)
                {
                    target.IsVisible = false;
                }
            }
        }
    }
}
