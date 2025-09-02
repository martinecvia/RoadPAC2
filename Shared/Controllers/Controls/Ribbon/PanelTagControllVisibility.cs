using System;

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
    internal class PanelTagControllVisibility : BaseRibbonControl<RibbonPanel>
    {
        public PanelTagControllVisibility(RibbonPanel target, RibbonPanelDef source) : base(target, source)
        {
            if (!string.IsNullOrEmpty(source.Tag?.ToString()))
            {
                target.IsVisible = false;
                RPApp.Projector.CurrentProjectFileChanged += selected =>
                {
                    try
                    {
                        Enum.TryParse<ProjectController.FClass>(source.Tag?.ToString(), out var flags);
                        target.IsVisible = selected?.Flag != null
                            && selected.Flag.HasFlag(flags);
                    }
                    catch
                    {
                        target.IsVisible = false;
                    }
                };
            }
        }
    }
}
