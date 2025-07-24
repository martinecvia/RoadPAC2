using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Internal.Windows;
#else
using Autodesk.Internal.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml.RibbonItem;

// https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem
namespace Shared.Controllers.Models.RibbonXml
{
    [XmlRoot("RibbonTab")]
    public class RibbonTabDef : BaseRibbonXml
    {
        [RPInfoOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets a description text for the tab. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Description
        public string Description { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(HighlightMode.None)]
        [Description("Sets and gets the current highlight mode that indicates the display status of the New Badge on the upper-right corner of the ribbon tab.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Highlight
        public HighlightMode Highlight { get; set; } = HighlightMode.None;

        [RPInternalUseOnly]
        [XmlAttribute("Highlight")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string HighlightDef
        {
            get => Highlight.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out HighlightMode result))
                    result = HighlightMode.None;
                Highlight = result;
            }
        }

        [RPInfoOut]
        [XmlAttribute("Id")]
        [DefaultValue(null)]
        [Description("Gets or sets the id for the tab. " +
            "The framework does not use or validate this id. " +
            "It is left to the applications to set this id and use it. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Id
        public string Id { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the value that indicates whether this tab is the active tab. " +
            "Hidden tabs and merged contextual tabs cannot be the active tab. " +
            "Setting this property to true for such tabs will fail, and no exception will be thrown.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsActive
        public bool? IsActive { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("IsActive")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsActiveDef
        {
            get => IsActive?.ToString();
            set
            {
                if (value == null)
                {
                    IsActive = null;
                    return;
                }
                IsActive = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Assesses whether the tab is regular tab or contextual tab. " +
            "If it is true the tab is contextual tab, and false if it is regular tab. " +
            "This is a dependency property registered with WPF. " +
            "Please see the Microsoft API for more information. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsContextualTab
        public bool IsContextualTab { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsContextualTab")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsContextualTabDef
        {
            get => IsContextualTab.ToString();
            set
            {
                if (value == null)
                {
                    IsContextualTab = false;
                    return;
                }
                IsContextualTab = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether the tab is visible in the ribbon. " +
            "If the value is true, the tab is visible in the ribbon. " +
            "If the value is false, it is hidden in ribbon. " +
            "Both visible and hidden tabs are available in the ribbon by right-clicking the menu under the Tabs menu option, which allows the user to show or hide the tabs. " +
            "If the tab's IsAnonymous property is set to false, it is not included in the right-click menu, and the user cannot control its visibility. " +
            "If an active tab is hidden, the next or previous visible tab is set as the active tab. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsVisible
        public bool IsVisible { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsVisible")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
        [XmlIgnore]
        [DefaultValue(null)]
        [Description("Gets or sets the value that indicates whether the tab is visible in the ribbon. " +
            "If the value is true, the tab is visible in the ribbon. " +
            "If the value is false, it is hidden in ribbon. " +
            "Both visible and hidden tabs are available in the ribbon by right-clicking the menu under the Tabs menu option, which allows the user to show or hide the tabs. " +
            "If the tab's IsAnonymous property is set to false, it is not included in the right-click menu, and the user cannot control its visibility. " +
            "If an active tab is hidden, the next or previous visible tab is set as the active tab. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsVisited
        public bool? IsVisited { get; set; } = null;

        [RPInternalUseOnly]
        [XmlAttribute("IsVisited")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string IsVisitedDef
        {
            get => IsVisited.ToString();
            set
            {
                if (value == null)
                {
                    IsVisited = null;
                    return;
                }
                IsVisited = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the keytip for the tab. " +
            "Keytips are displayed in the ribbon when navigating the ribbon with the keyboard. " +
            "If this property is null or empty, the keytip will not appear for this tab, and the tab will not support activation through the keyboard. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_KeyTip
        public string KeyTip { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of the ribbon tab. " +
            "The framework uses the Title property of the tab to display the tab title in the ribbon. " +
            "The name property is not currently used by the framework. " +
            "Applications can use this property to store a longer name for the tab if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Name
        public string Name { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets custom data object in the tab. " +
            "This property can be used to store any object as a custom data object in a tab. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Tag
        public string Tag { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("Title")]
        [DefaultValue(null)]
        [Description("Gets or sets the tab title. " +
            "The title set with this property is displayed in the tab button for this tab in the ribbon. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Title
        public string Title { get; set; } = null;

        [RPInfoOut]
        [XmlElement("RibbonPanel")]
        [Description("Gets the collection used to store the panels in the tab. " +
            "The default is an empty collection.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Panels
        public List<RibbonPanelDef> Panels { get; set; } = new List<RibbonPanelDef>();

        // Optional: IsAnonymous is used to handle contextual tabs
        //           but we want to do programatically
        //           However. This serializer can still be used as a "shell" for contextuals

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsEnabled")]
        [EditorBrowsable(EditorBrowsableState.Never)]
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
    }
}
