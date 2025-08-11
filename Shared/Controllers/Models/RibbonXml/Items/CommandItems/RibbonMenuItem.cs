using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonMenuItem
    [RPPrivateUseOnly]
    [Description("This class is used to support hierarchical items. " +
        "This class is used in places where a hierarchy is supported. " +
        "Examples are application menu and RibbonMenuItem.")]
    public class RibbonMenuItemDef : RibbonCommandItemObservableCollectionDef
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem
        [RPPrivateUseOnly]
        [Description("The ApplicationMenuItem class is used to manage a single menu item and its sub-menu items in a hierarchical structure in the application menu. " +
            "This class supports the following menu item types: " +
            "- regular menu item, which executes a command. " +
            "- split button type menu item. " +
            "- popup menu item, which opens a sub-menu. " +
            "The split button type menu item is supported only in the first-level menu.")]
        public class ApplicationMenuItemDef : RibbonMenuItemDef
        {
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(false)]
            [Description("Gets or sets the value indicating whether or not the menu item is to be displayed as a split button type menu item. " +
                "If the value is true the menu item is displayed as a split button. " +
                "Clicking the left portion of the split button executes the item and clicking the right portion of the button opens the next sub-level menu. " +
                "This property is applicable only for main menu items (first-level menu items) and is ignored when set in menu items in a second or subsequent level. " +
                "Setting this value to true does not make sense if the menu item does not have a sub-menu. " +
                "The default value is false.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_IsSplit
            public bool IsSplit { get; set; } = false;

            [RPInternalUseOnly]
            [XmlAttribute("IsSplit")]
            public string IsSplitDef
            {
                get => IsSplit.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        IsSplit = false;
                        return;
                    }
                    IsSplit = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }

            [RPInfoOut]
            [XmlAttribute("SplitKeyTip")]
            [DefaultValue(null)]
            [Description("Gets or sets the keytip for the right portion of the split button menu item which is used to open the next level menu. " +
                "This property is applicable only for split buttons (i.e. IsSplit=true). " +
                "The default value is null.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_SplitKeyTip
#if NET8_0_OR_GREATER
            public string? SplitKeyTip { get; set; } = null;
#else
            public string SplitKeyTip { get; set; } = null;
#endif

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(false)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_IsPinable
            public bool IsPinable { get; set; } = false;

            [RPInternalUseOnly]
            [XmlAttribute("IsPinable")]
            public string IsPinableDef
            {
                get => IsPinable.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        IsPinable = false;
                        return;
                    }
                    IsPinable = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(0)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_ApplicationMenuItem_MaxDescriptionLines
            public int MaxDescriptionLines { get; set; } = 0;

            [RPInternalUseOnly]
            [XmlAttribute("MaxDescriptionLines")]
            public string MaxDescriptionLinesDef
            {
                get => MaxDescriptionLines.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxDescriptionLines = x;
                    else
                    {
                        MaxDescriptionLines = 0;
                    }
                }
            }
        }
    }
}