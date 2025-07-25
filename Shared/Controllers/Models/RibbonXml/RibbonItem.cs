﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
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

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(100d)]
        [Description("Gets or sets the minimum width of the item. " +
            "This property is used only by resizeable items. " +
            "Resizeable items are items whose width is resized to fit in the space available. " +
            "For example, RibbonCombo and RibbonGallery are resizeable items, and RibbonButton is not a resizeable item. " +
            "If there is enough space, resizeable items are displayed in full width, which is set with the Width property. " +
            "When there is not enough space, the item will be resized to fit in the available space. " +
            "The width of the item will not go below MinWidth. " +
            "The value for this property must be zero or positive and cannot be Infinity. " +
            "The value must be in device-independent units. " +
            "The default value is 100.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_MinWidth
        public double MinWidth { get; set; } = 100d;

        [XmlAttribute("MinWidth")]
        [RPInternalUseOnly]
        public string MinWidthDef
        {
            get => MinWidth.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    MinWidth = result;
                    return;
                }
                MinWidth = 100d;
            }
        }

        [RPInfoOut]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        [Description("Gets or sets the name of a ribbon item. " +
            "There are two properties that support text content in ribbon items: Text and Name. " +
            "The Text property is meant for short text and is used wherever space is premium (for example, text that appears directly in ribbon face). " +
            "The Name property is meant for longer text and is used wherever space is not an issue (for example, text that appears in a drop-down list, tooltip, etc.). " +
            "Both properties can be set for a ribbon item. " +
            "The visibility of text in the ribbon is controlled by the ShowText property, which must be true to make the text visible. " +
            "Text may also be suppressed in one or more panels in a horizontal ribbon when there is not enough space to display all the panels. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Name
        public string Name { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value indicating whether the item image is visible. " +
            "If the value is true, the image is visible in the ribbon, provided that a valid image has been set with the Image property. " +
            "If the value is false, the image is not visible. " +
            "The default value is true. " +
            "Derived classes may override this default.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowImage
        public bool ShowImage { get; set; } = true;

        [XmlAttribute("ShowImage")]
        [RPInternalUseOnly]
        public string ShowImageDef
        {
            get => ShowImage.ToString();
            set
            {
                if (value == null)
                {
                    ShowImage = true;
                    return;
                }
                ShowImage = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value indicating whether the item text is visible. " +
            "If the value is true, the text for the item is visible in the ribbon, provided that a valid text has been set for this item with the Text property. " +
            "If the value is false, the text is not visible in the ribbon. " +
            "The default value is false. " +
            "Derived classes may override this default.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ShowText
        public bool ShowText { get; set; } = false;

        [XmlAttribute("ShowText")]
        [RPInternalUseOnly]
        public string ShowTextDef
        {
            get => ShowText.ToString();
            set
            {
                if (value == null)
                {
                    ShowText = false;
                    return;
                }
                ShowText = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(RibbonItemSize.Standard)]
        [Description("Gets or sets the size with which the item is displayed. " +
            "This property is supported only by RibbonButton and RibbonLabel as well as classes derived from them. " +
            "Other ribbon items will ignore this property. " +
            "The default value is Standard.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Size
        public RibbonItemSize Size { get; set; } = RibbonItemSize.Standard;

        [XmlAttribute("Size")]
        [RPInternalUseOnly]
        public string SizeDef
        {
            get => Size.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonItemSize result))
                    result = RibbonItemSize.Standard;
                Size = result;
            }
        }

        [RPInfoOut]
        [XmlAttribute("Text")]
        [DefaultValue(null)]
        [Description("Gets or sets the text of a ribbon item. There are two properties that support text content in ribbon items: Text and Name. " +
            "The Text property is meant for short text and is used wherever space is premium (for example, text that appears directly in ribbon face). " +
            "The Name property is meant for longer text and is used wherever space is not an issue (for example, text that appears in a drop-down list, tooltip, etc.). " +
            "Both properties can be set for a ribbon item. " +
            "The visibility of text in the ribbon is controlled by the ShowText property, which must be true to make the text visible. " +
            "Text may also be suppressed in one or more panels in a horizontal ribbon when there is not enough space to display all the panels. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Text
        public string Text { get; set; } = null;

        [RPInfoOut]
        [XmlAttribute("ToolTip")]
        [DefaultValue(null)]
        [Description("Gets or sets an object to be used as the tooltip for the item. " +
            "If the object is a string, it displays the string as a tooltip without any formatting. " +
            "If the object is of any other type, and the object has a data template, the tooltip will use the data template to display the object. " +
            "If this property is null, a default tooltip of type RibbonToolTip is created at runtime using the Name, Text, and Description properties of the item. " +
            "If this property is set to an empty string, the tooltip will be suppressed, and the item will not show any tooltip. " +
            "ToolTip can also be suppressed by setting the IsToolTipEnabled property to false. " +
            "The default value is null.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_ToolTip
        public string ToolTip { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(200d)]
        [Description("Gets or sets the width of the item. " +
            "This property is used only by resizeable items. " +
            "Resizeable items are items whose width is resized to fit in the space available. " +
            "For example, RibbonCombo and RibbonGallery are resizeable items, and RibbonButton is not a resizeable item. " +
            "If there is enough space, resizeable items are displayed in full width. " +
            "When there is not enough space, the item will be resized to fit in the available space. " +
            "The width of the item will not go below MinWidth. " +
            "The value for this property must be zero or positive and cannot be Infinity. " +
            "The value must be in device-independent units. " +
            "The default value is 200.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_Width
        public double Width { get; set; } = 200d;

        [XmlAttribute("Width")]
        [RPInternalUseOnly]
        public string WidthDef
        {
            get => MinWidth.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Width = result;
                    return;
                }
                Width = 200d;
            }
        }
    }
}