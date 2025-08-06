#pragma warning disable CS8603
#pragma warning disable CS8625

using System;
using System.ComponentModel;
using System.Xml;
using System.Windows.Markup;
using System.Xml.Serialization;
using Shared.Controllers.Models.RibbonXml.Items;


#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel
    [RPPrivateUseOnly]
    [Description("The class RibbonPanel is used to store and manage the panel in a ribbon. " +
        "RibbonPanel displays the content of the RibbonPanelSource set in the Source property.")]
    public class RibbonPanelDef : BaseRibbonXml
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-__MEMBERTYPE_Properties_Autodesk_Windows_RibbonPanel
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Vertical)]
        [Description("Gets or sets the orientation to be used when the panel is floating. " +
            "This property is applicable only when the panel is floating. " +
            "The orientation of a floating panel can be horizontal or vertical. " +
            "The orientation can be switched by the user with the Orientation button in the panel frame. " +
            "Set the CanToggleOrientation property to false to hide the Orientation button and hinder the user from changing the orientation.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_FloatingOrientation
        public System.Windows.Controls.Orientation FloatingOrientation { get; set; } = System.Windows.Controls.Orientation.Vertical;

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
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool CanToggleOrientation { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("CanToggleOrientation")]
        public string CanToggleOrientationDef
        {
            get => CanToggleOrientation.ToString();
            set
            {
                if (value == null)
                {
                    CanToggleOrientation = true;
                    return;
                }
                CanToggleOrientation
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightPanelTitleBar
        public bool HighlightPanelTitleBar { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("HighlightPanelTitleBar")]
        public string HighlightPanelTitleBarDef
        {
            get => HighlightPanelTitleBar.ToString();
            set
            {
                if (value == null)
                {
                    HighlightPanelTitleBar = false;
                    return;
                }
                HighlightPanelTitleBar
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Accesses the highlight state for the ribbon panel's title bar.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_HighlightWhenCollapsed
        public bool HighlightWhenCollapsed { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("HighlightWhenCollapsed")]
        public string HighlightWhenCollapsedDef
        {
            get => HighlightWhenCollapsed.ToString();
            set
            {
                if (value == null)
                {
                    HighlightWhenCollapsed = false;
                    return;
                }
                HighlightWhenCollapsed
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
#if NET8_0_OR_GREATER
        [RPInfoOut]
        [XmlAttribute("Id")]
        [DefaultValue("")]
        [Description("Accesses the Id for the ribbon panel.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_Id
        public string Id { get; set; } = "";
#endif
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
        [DefaultValue(null)]
        [Description("Gets or sets the source that contains the ribbon items to be displayed by this panel. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanel_Source
        public RibbonPanelSource Source => SourceDef != null ? SourceDef.Transform(RibbonPanelSourceDef.SourceFactory[SourceDef.GetType()]()) : null;

        [RPInternalUseOnly]
        [RPValidation]
        [XmlElement("RibbonPanelSource", typeof(RibbonPanelSourceDef))]
        [XmlElement("RibbonPanelSpacer", typeof(RibbonPanelSourceDef.RibbonPanelSpacerDef))]
        public RibbonPanelSourceDef SourceDef { get; set; } = null;

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelBackground { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("CustomPanelBackground")]
        public XmlElement CustomPanelBackgroundDef
        {
            get
            {
                if (CustomPanelBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomPanelBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomPanelBackground = (System.Windows.Media.Brush) XamlReader.Parse(xaml);
                }
                else
                {
                    CustomPanelBackground = null;
                }
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomSlideOutPanelBackground { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("CustomSlideOutPanelBackground")]
        public XmlElement CustomSlideOutPanelBackgroundDef
        {
            get
            {
                if (CustomSlideOutPanelBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomSlideOutPanelBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomSlideOutPanelBackground = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                }
                else
                {
                    CustomSlideOutPanelBackground = null;
                }
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
        public System.Windows.Media.Brush CustomPanelTitleBarBackground { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("CustomPanelTitleBarBackground")]
        public XmlElement CustomPanelTitleBarBackgroundDef
        {
            get
            {
                if (CustomPanelTitleBarBackground == null)
                    return null;
                XmlDocument document = new XmlDocument();
                document.LoadXml(XamlWriter.Save(CustomPanelTitleBarBackground));
                return document.DocumentElement;
            }
            set
            {
                if (value != null)
                {
                    string xaml = value.OuterXml;
                    CustomPanelTitleBarBackground = (System.Windows.Media.Brush)XamlReader.Parse(xaml);
                }
                else
                {
                    CustomPanelTitleBarBackground = null;
                }
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsContextualTabThemeIgnored { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsContextualTabThemeIgnored")]
        public string IsContextualTabThemeIgnoredDef
        {
            get => IsContextualTabThemeIgnored.ToString();
            set
            {
                if (value == null)
                {
                    IsContextualTabThemeIgnored = true;
                    return;
                }
                IsContextualTabThemeIgnored
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
    }
}