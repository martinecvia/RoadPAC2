#pragma warning disable CS8603
#pragma warning disable CS8625

using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab
    [RPPrivateUseOnly]
    [XmlRoot("RibbonTab")]
    [Description("The class RibbonTab is used to store and manage the contents of a ribbon tab.")]
    public class RibbonTabDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"Tab={Id}_{Title}_{Name}";
            set => _cookie = value;
        }

        [XmlIgnore]
        [Description("Gets the collection used to store the panels in the tab. " +
            "The default is an empty collection.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Panels
        public List<RibbonPanel> Panels
        {
            get
            {
                List<RibbonPanel> panels = new List<RibbonPanel>();
                if (PanelsDef == null)
                    return panels;
                foreach (RibbonPanelDef element in PanelsDef)
                    panels.Add(element.Transform(new RibbonPanel()));
                return panels;
            }
        }

        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonPanel")]
        public List<RibbonPanelDef> PanelsDef { get; set; } = new List<RibbonPanelDef>();

        [RPInfoOut]
        [XmlAttribute("Title")]
        [DefaultValue(null)]
        [Description("Gets or sets the tab title. " +
            "The title set with this property is displayed in the tab button for this tab in the ribbon. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Title
        public string Title { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("Title")]
        public XmlCDataSection TitleCData
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                    return null;
                return new XmlDocument().CreateCDataSection(Title);
            }
            set { Title = value?.Value; }
        }

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

        [RPInternalUseOnly]
        [XmlElement("Name")]
        public XmlCDataSection NameCData
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return null;
                return new XmlDocument().CreateCDataSection(Name);
            }
            set { Name = value?.Value; }
        }

        [RPInfoOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets a description text for the tab. " +
            "The description text is not currently used by the framework. " +
            "Applications can use this to store a description if it is required in other UI customization dialogs. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Description
        public string Description { get; set; } = null;

        [RPInternalUseOnly]
        [XmlElement("Description")]
        public XmlCDataSection DescriptionCData
        {
            get
            {
                if (string.IsNullOrEmpty(Description))
                    return null;
                return new XmlDocument().CreateCDataSection(Description);
            }
            set { Description = value?.Value; }
        }

        /*
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
        public string IsVisibleDef
        {
            get => IsVisible.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsVisible = true;
                    return;
                }
                IsVisible = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
        */

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether this tab is the active tab. " +
            "Hidden tabs and merged contextual tabs cannot be the active tab. " +
            "Setting this property to true for such tabs will fail, and no exception will be thrown.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_IsActive
        public bool IsActive { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsActive")]
        public string IsActiveDef
        {
            get => IsActive.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsActive = false;
                    return;
                }
                IsActive = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsEnabled { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsEnabled")]
        public string IsEnabledDef
        {
            get => IsEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsEnabled = true;
                    return;
                }
                IsEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(true)]
        public bool IsPanelEnabled { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsPanelEnabled")]
        public string IsPanelEnabledDef
        {
            get => IsPanelEnabled.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsPanelEnabled = true;
                    return;
                }
                IsPanelEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
        /*
        [RPInfoOut]
        [RPInternalUseOnly]
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
        public string IsContextualTabDef
        {
            get => IsContextualTab.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsContextualTab = false;
                    return;
                }
                IsContextualTab = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
        */
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsMergedContextualTab { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsMergedContextualTab")]
        public string IsMergedContextualTabDef
        {
            get => IsMergedContextualTab.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsMergedContextualTab = false;
                    return;
                }
                IsMergedContextualTab = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool AllowTearOffContextualPanels { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("AllowTearOffContextualPanels")]
        public string AllowTearOffContextualPanelsDef
        {
            get => AllowTearOffContextualPanels.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    AllowTearOffContextualPanels = false;
                    return;
                }
                AllowTearOffContextualPanels = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
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
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        [Description("Gets or sets custom data object in the tab. " +
            "This property can be used to store any object as a custom data object in a tab. " +
            "This data is not used by the framework. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonTab_Tag
        public string Tag { get; set; } = null;

        // Optional: IsAnonymous is used to handle contextual tabs
        //           but we want to do programatically
        //           However. This serializer can still be used as a "shell" for contextuals
    }
}