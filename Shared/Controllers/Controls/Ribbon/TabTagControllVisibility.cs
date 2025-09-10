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
    internal class TabTagControllVisibility : BaseRibbonControl<RibbonTab>
    {
        public TabTagControllVisibility(RibbonTab target, RibbonTabDef source) : base(target, source)
        {
            if (!string.IsNullOrEmpty(source.Tag?.ToString()))
            {
                RPApp.Projector.CurrentProjectFileChanged += selected =>
                {
                    target.IsVisible = false;
                    try
                    {
                        Enum.TryParse<ProjectController.FClass>(source.Tag?.ToString(), out var flags);
                        bool IsVisible = selected?.Flag != null
                            && selected.Flag.HasFlag(flags);
                        if (target is RibbonController.ContextualRibbonTab _contextualTab)
                        {
                            if (IsVisible)
                                 RibbonController.ShowContextualTab(_contextualTab);
                            else RibbonController.HideContextualTab(_contextualTab);
                        }
                        else target.IsVisible = IsVisible;
                    }
                    catch
                    {
                        if (target is RibbonController.ContextualRibbonTab _contextualTab)
                             RibbonController.HideContextualTab(_contextualTab);
                        else target.IsVisible = false;
                    }
                };
            }
        }
    }
}