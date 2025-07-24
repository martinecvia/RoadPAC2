using System;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Internal.Windows;
#else
using Autodesk.Internal.Windows;
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml
{
    // Autodesk.Windows.RibbonItem
    //    Autodesk.Windows.RibbonCommandItem
    //    Autodesk.Windows.RibbonForm
    //    Autodesk.Windows.RibbonHwnd
    //    Autodesk.Windows.RibbonLabel
    //    Autodesk.Windows.RibbonList
    //    Autodesk.Windows.RibbonPanelBreak
    //    Autodesk.Windows.RibbonRowBreak
    //    Autodesk.Windows.RibbonRowPanel
    //    Autodesk.Windows.RibbonSeparator
    //    Autodesk.Windows.RibbonSlider
    //    Autodesk.Windows.RibbonSpinner
    //    Autodesk.Windows.RibbonTextBox
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem
    public class RibbonItemDef : BaseRibbonXml
    {
        [RPInfoOut]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        [Description("Gets or sets the description text for the ribbon item. " +
            "The description text is used in the application menu, tooltips, and drop-down lists in a RibbonListButton when the list style is set to Descriptive.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Description
        public string Description { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("GroupName")]
        [DefaultValue(null)]
        [Description("Gets or sets the description text for the ribbon item. " +
    "The description text is used in the application menu, tooltips, and drop-down lists in a RibbonListButton when the list style is set to Descriptive.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_GroupName
        public string GroupName { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(HighlightMode.None)]
        [Description("Gets or sets the highlight state for the ribbon item.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Highlight
        public HighlightMode Highlight { get; set; } = HighlightMode.None;

        [RPInternalUseOnly]
        [XmlAttribute("Highlight")]
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
        [Description("Gets or sets the item id. " +
            "This id is used as the automation id for the corresponding control in the UI. " +
            "The framework does not otherwise use or validate it. " +
            "It is up to the application to set and use this id. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Id
        public string Id { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("Image")]
        [DefaultValue(null)]
        [Description("Gets or sets the image to be used when the item is displayed in standard size. " +
            "Many ribbon items can appear in two sizes: Standard and Large. " +
            "Some ribbon items support only standard size (example: RibbonCombo) and some support both (example: RibbonButton). " +
            "Ribbon items also support ico files containing multiple images in different sizes (only the first two images will be used). " +
            "If the ico file contains multiple size images, use it to set only the Image or LargeImage property. " +
            "The visibility of the image in a ribbon is controlled by the ShowImage property, which must be set to true to make the image visible.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Image
        /// <summary>
        /// Gets or sets the image resource name (without extension) used when the ribbon item is displayed in standard size.
        /// </summary>
        /// <remarks>
        /// This replaces the original <see cref="System.Windows.Media.ImageSource"/> with an internal reference to an embedded image resource.
        /// The resource should typically be a PNG or ICO embedded in the assembly, and only the base name (no extension) is specified.
        /// 
        /// For ICO files with multiple sizes, only the first two images will be used. The visibility of this image is controlled
        /// by the <c>ShowImage</c> property, which must be set to <c>true</c> for the image to appear in the ribbon UI.
        /// </remarks>
        /// <example>
        /// Example: <c>Image="icon_button_save"</c> (resolved as icon_button_save.png or .ico from resources)
        /// </example>
        public string Image { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value indicating whether the item is enabled in the ribbon." +
            "If this property is true, the item is enabled in the ribbon; otherwise, it is disabled. " +
            "Disabled items do not respond any interaction. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsEnabled
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
        [Description("Gets or sets the value indicating whether F1 help is enabled in the item's tooltip. " +
            "If this value is true, F1 help is enabled in the item's tooltip; otherwise, F1 help is disabled in the item' s tooltip. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsToolTipEnabled
        public bool IsToolTipEnabled { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsToolTipEnabled")]
        public string IsToolTipEnabledDef
        {
            get => IsToolTipEnabled.ToString();
            set
            {
                if (value == null)
                {
                    IsToolTipEnabled = true;
                    return;
                }
                IsToolTipEnabled 
                    = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value to indicating whether the item is visible in the ribbon. " +
            "If this property is true, the item is visible in the ribbon; otherwise, the item is hidden in ribbon and does not occupy any space. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_IsVisible
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
        [XmlAttribute("KeyTip")]
        [DefaultValue(null)]
        [Description("Gets or sets the keytip for the item. " +
            "Keytips are displayed in the ribbon and are used to navigate the ribbon through the keyboard. " +
            "This property must be set for a ribbon item to support a keytip. " +
            "If this property is not set, a keytip will not appear for this item, and the item will not support activation through the keyboard. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_KeyTip
        public string KeyTip { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("LargeImage")]
        [DefaultValue(null)]
        [Description("Gets or sets the image to be used when the item is displayed in large size." +
            "Many ribbon items can appear in two sizes: Standard and Large. " +
            "Some ribbon items support only standard size (example: RibbonCombo) and some support both (example: RibbonButton). " +
            "Ribbon items also support ico files containing multiple images in different sizes (only the first two images will be used). " +
            "If the ico file contains multiple size images, use it to set only the Image or LargeImage property. " +
            "The visibility of the image in a ribbon is controlled by the ShowImage property, which must be set to true to make the image visible.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Image
        /// <summary>
        /// Gets or sets the image resource name (without extension) used when the ribbon item is displayed in large size.
        /// </summary>
        /// <remarks>
        /// This replaces the original <see cref="System.Windows.Media.ImageSource"/> with an internal reference to an embedded image resource.
        /// The resource should typically be a PNG or ICO embedded in the assembly, and only the base name (no extension) is specified.
        /// 
        /// For ICO files with multiple sizes, only the first two images will be used. The visibility of this image is controlled
        /// by the <c>ShowImage</c> property, which must be set to <c>true</c> for the image to appear in the ribbon UI.
        /// </remarks>
        /// <example>
        /// Example: <c>Image="icon_button_save"</c> (resolved as icon_button_save.png or .ico from resources)
        /// </example>
        public string LargeImage { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_MinWidth
        public double MinWidth { get; set; } = 100;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Name
        public string Name { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowImage
        public bool ShowImage { get; set; } = true;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowText
        public bool ShowText { get; set; } = false;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Size
        public RibbonItemSize Size { get; set; } = RibbonItemSize.Standard;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Text
        public string Text { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ToolTip
        public string ToolTip { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Width
        public double Width { get; set; } = 200;

    }
}
