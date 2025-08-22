#define DEBUG
#define NON_VOLATILE_MEMORY

using System.Linq;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Internal.Windows;
using ZwSoft.Windows;
#else
using Autodesk.Internal.Windows;
using Autodesk.Windows;
#endif
#endregion

// https://adndevblog.typepad.com/autocad/2012/04/displaying-a-contextual-tab-upon-entity-selection-using-ribbon-runtime-api.html
namespace Shared.Controllers.Models
{
    public class ContextualRibbonTab : RibbonTab
    {
        private static RibbonControl Ribbon => ComponentManager.Ribbon;
        private RibbonTab _previouslyOpenedTab { get; set; } = Ribbon.ActiveTab;
        private RibbonTab ThemeTemplate => Ribbon.Tabs.FirstOrDefault(t => t.IsContextualTab
            && (t.Name == "Hatch Editor" || t.Id == "ACAD.RBN_01738148" 
            || t.Name == "Vytváøení šraf" || t.KeyTip == "CH" || t.Id == "ZWCAD.ID_HatchCreate"));

        public void Show()
        {
            IsVisible = true;
            if (ThemeTemplate?.Theme is TabTheme theme)
            {
                Theme = new TabTheme
                {
                    InnerBorder = CloneBrush(theme.InnerBorder),
                    OuterBorder = CloneBrush(theme.OuterBorder),
                    PanelBackground = CloneBrush(theme.PanelBackground),
                    PanelBackgroundVerticalLeft = CloneBrush(theme.PanelBackgroundVerticalLeft),
                    PanelBackgroundVerticalRight = CloneBrush(theme.PanelBackgroundVerticalRight),
                    PanelBorder = CloneBrush(theme.PanelBorder),
                    PanelDialogBoxLauncherBrush = CloneBrush(theme.PanelDialogBoxLauncherBrush),
                    PanelSeparatorBrush = CloneBrush(theme.PanelSeparatorBrush),
                    PanelTitleBackground = CloneBrush(theme.PanelTitleBackground),
                    PanelTitleBorderBrushVertical = CloneBrush(theme.PanelTitleBorderBrushVertical),
                    PanelTitleForeground = CloneBrush(theme.PanelTitleForeground),
                    RolloverTabHeaderForeground = CloneBrush(theme.RolloverTabHeaderForeground),
                    SelectedTabHeaderBackground = CloneBrush(theme.SelectedTabHeaderBackground),
                    SelectedTabHeaderForeground = CloneBrush(theme.SelectedTabHeaderForeground),
                    SlideoutPanelBorder = CloneBrush(theme.SlideoutPanelBorder),
                    TabHeaderBackground = CloneBrush(theme.TabHeaderBackground)
                };
            }
            if (Ribbon.ActiveTab != null &&
                !(Ribbon.ActiveTab is ContextualRibbonTab) && !Ribbon.ActiveTab.IsContextualTab &&
                Ribbon.ActiveTab.IsVisible &&
                Ribbon.ActiveTab.IsEnabled)
                _previouslyOpenedTab = Ribbon.ActiveTab;
            Ribbon.ActiveTab = this;
        }

        public void Hide()
        {
            IsVisible = false;
            if (_previouslyOpenedTab != null)
                Ribbon.ActiveTab = _previouslyOpenedTab;
        }
        private static T CloneBrush<T>(T brush) where T : class
            => brush == null ? null : (brush as dynamic).Clone();
    }
}