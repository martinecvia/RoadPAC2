using System;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    public class RibbonPanelDef : BaseRibbonXml
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-__MEMBERTYPE_Properties_Autodesk_Windows_RibbonPanel
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("Gets or sets the orientation to be used when the panel is floating. " +
            "This property is applicable only when the panel is floating. " +
            "The orientation of a floating panel can be horizontal or vertical. " +
            "The orientation can be switched by the user with the Orientation button in the panel frame. " +
            "Set the CanToggleOrientation property to false to hide the Orientation button and hinder the user from changing the orientation.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_FloatingOrientation
        public System.Windows.Controls.Orientation FloatingOrientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;

        [RPInternalUseOnly]
        [XmlAttribute("FloatingOrientation")]
        public string FloatingOrientationDef
        {
            get => FloatingOrientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                FloatingOrientation = result;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightPanelTitleBar
        public bool? HighlightPanelTitleBar { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("HighlightPanelTitleBar")]
        public string HighlightPanelTitleBarDef
        {
            get => HighlightPanelTitleBar?.ToString();
            set
            {
                if (value == null)
                {
                    HighlightPanelTitleBar = null;
                    return;
                }
                HighlightPanelTitleBar
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightWhenCollapsed
        public bool? HighlightWhenCollapsed { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("HighlightWhenCollapsed")]
        public string HighlightWhenCollapsedDef
        {
            get => HighlightWhenCollapsed?.ToString();
            set
            {
                if (value == null)
                {
                    HighlightWhenCollapsed = null;
                    return;
                }
                HighlightWhenCollapsed
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlAttribute("Id")]
        [DefaultValue(null)]
        [Description("Accesses the Id for the ribbon panel.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_Id
        public string Id { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether this panel is enabled. " +
            "If the value is true, the panel is enabled. " +
            "If the value is false, the panel is disabled. " +
            "When a panel is disabled all the items in the panel are disabled. " +
            "The default value is true. " +
            "To disable all panels in a tab use RibbonTab.IsPanelEnabled.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_IsEnabled
        public bool IsEnabled { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsEnabled")]
        public string IsEnabledDef
        {
            get => IsEnabled.ToString();
            set
            {
                if (value == null)
                {
                    IsEnabled = true;
                    return;
                }
                IsEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("If the value is true, the panel is enabled. " +
            "If the value is false, the panel is disabled. " +
            "When a panel is disabled all the items in the panel are disabled. " +
            "The default value is true. " +
            "To disable all panels in a tab use RibbonTab.IsPanelEnabled. " +
            "" +
            "If the value is true, the panel is visible in the ribbon. " +
            "If the value is false, it is hidden in the ribbon. " +
            "Both visible and hidden panels of a tab are available in the ribbon's right-click menu under the Panels menu option, which allows the user to show or hide the panels. " +
            "If the panel's IsAnonymous property is set to false, it is not included in the right-click menu and the user cannot control its visibility. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_IsVisible
        public bool IsVisible { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsVisible")]
        public string IsVisibleDef
        {
            get => IsVisible.ToString();
            set
            {
                if (value == null)
                {
                    IsVisible = true;
                    return;
                }
                IsVisible = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }


        [RPInfoOut]
        [XmlElement("RibbonPanelSource")]
        [DefaultValue(null)]
        [Description("Gets or sets the source that contains the ribbon items to be displayed by this panel. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_Source
        public RibbonPanelSourceDef Source { get; set; } = null;

    }
}
