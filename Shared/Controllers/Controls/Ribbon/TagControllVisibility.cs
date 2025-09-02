using System;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml.Items;

namespace Shared.Controllers.Controls.Ribbon
{
    internal class TagControllVisibility : BaseRibbonControl<RibbonItem>
    {
        public TagControllVisibility(RibbonItem target, RibbonItemDef source) : base(target, source)
        {
            if (!string.IsNullOrEmpty(source.Tag?.ToString()))
            {
                target.IsVisible = false;
                RPApp.Projector.CurrentProjectFileChanged += selected =>
                {
                    try
                    {
                        target.IsVisible = selected?.Flag != null
                            && Enum.TryParse<ProjectController.FClass>(source.Tag?.ToString(), out var flags)
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
