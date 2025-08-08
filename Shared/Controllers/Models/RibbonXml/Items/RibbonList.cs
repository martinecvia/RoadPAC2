using System;
using System.ComponentModel;
using System.Windows;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList
    [RPPrivateUseOnly]
    [XmlInclude(typeof(RibbonComboDef))]
    [XmlInclude(typeof(RibbonComboDef.RibbonGalleryDef))]
    public abstract class RibbonListDef : RibbonItemObservableCollectionDef
    {
        public RibbonListDef()
        {
            base.ShowImage = false;
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the drop-down list should support the grouping of items. " +
            "Only RibbonCombo supports grouping. " +
            "RibbonGallery does not support grouping. " +
            "Grouping is done using the property RibbonItem.GroupName in the drop-down items. " +
            "If this property is true, grouping is enabled in the drop-down list. " +
            "If it is false, grouping is not enabled. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsVirtualizing
        public bool IsGrouping { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsGrouping")]
        public string IsGroupingDef
        {
            get => IsGrouping.ToString();
            set
            {
                // RibbonGallery does not support grouping
                if (string.IsNullOrEmpty(value) || this is RibbonComboDef.RibbonGalleryDef)
                {
                    IsGrouping = false;
                    return;
                }
                IsGrouping = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(double.NaN)]
        [Description("Gets or sets the maximum height of the drop-down window that is displayed when a drop-down item is opened. " +
            "The height must be in device independent units. " +
            "The actual drop-down height depends on the number of items in the list and will not exceed the value set in this property. " +
            "The default value is a calculated value that is based on system max screen height parameters.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList_MaxDropDownHeight
        public double MaxDropDownHeight { get; set; } = double.NaN;

        [XmlAttribute("MaxDropDownHeight")]
        [RPInternalUseOnly]
        public string MaxDropDownHeightDef
        {
            get => MaxDropDownHeight.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    MaxDropDownHeight = result;
                    return;
                }
                MaxDropDownHeight = double.NaN;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(double.NaN)]
        [Description("Gets or sets the width of the drop-down window that is displayed when a drop-down item is opened. " +
            "The width must be in device independent units. " +
            "The default value is NaN. " +
            "The minimum drop-down width is equal to the control width. " +
            "Thus, if the value set in this property is less than the control width, the value is ignored.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList_DropDownWidth
        public double DropDownWidth { get; set; } = double.NaN;

        [XmlAttribute("DropDownWidth")]
        [RPInternalUseOnly]
        public string DropDownWidthDef
        {
            get => DropDownWidth.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    DropDownWidth = result;
                    return;
                }
                DropDownWidth = double.NaN;
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo
        [RPPrivateUseOnly]
        public class RibbonComboDef : RibbonListDef
        {
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(null)]
            [Description("Gets or sets the command handler to be called when the RibbonCombo menu items are executed. " +
                "The command is routed to the first command handler found while searching in the following order: " +
                "1. the command handler set in the item. " +
                "2. the command handler set in the RibbonCombo. " +
                "3. the command handler set in the root control that contains this item (ribbon, Quick Access Toolbar, menu, or status bar). " +
                "4. the global command handler set in ComponentManager.CommandHandler. " +
                "The default value is null.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_CommandHandler
#if NET8_0_OR_GREATER
            public System.Windows.Input.ICommand? CommandHandler { get; set; } = null;
#else
        public System.Windows.Input.ICommand CommandHandler { get; set; } = null;
#endif

            [RPInternalUseOnly]
            [XmlAttribute("CommandHandler")]
            public string CommandHandlerDef
            {
                get => CommandHandler != null && CommandHandler is CommandHandler handler
                    ? handler.Command : string.Empty;
                set
                {
                    if (!string.IsNullOrEmpty(value))
                        CommandHandler = new CommandHandler(value);
                }
            }

            [RPInfoOut]
            [XmlAttribute("EditableText")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_EditableText
            [Description("Gets or sets the editable text in the combo box. This property is applicable only if IsEditable is true. " +
                "The default value is null.")]
#if NET8_0_OR_GREATER
            public string? EditableText { get; set; } = null;
#else
            public string EditableText { get; set; } = null;
#endif

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(false)]
            [Description("Gets or sets the value that indicates whether the combo box text is editable. " +
                "If this property is true, the combo box allows text to be entered that is not in the list. " +
                "The entered text is not added to the list. " +
                "The entered text can be validated in the EditableTextChanging event. " +
                "The default value is false.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsEditable
            public bool IsEditable { get; set; } = false;

            [RPInternalUseOnly]
            [XmlAttribute("IsEditable")]
            public string IsEditableDef
            {
                get => IsEditable.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        IsEditable = false;
                        return;
                    }
                    IsEditable = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }
#if NET8_0_OR_GREATER
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(true)]
            [Description("Gets or sets a value that indicates if virtualization is enabled for the combo box." +
                "If the value of this property is true the combo box enables virtualization. " +
                "If it is false it disables virtualization. " +
                "The combo box performs better and uses less memory when virtualization is enabled. " +
                "When virtualization is enabled all the items in the combo box will be displayed in a uniform size. " +
                "Also hiding items in the combo box is not supported when virtualization is enabled. " +
                "Disable virtualization if the combo box needs to display items in various sizes. " +
                "The default value is true.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo_IsVirtualizing
            public bool IsVirtualizing { get; set; } = true;

            [RPInternalUseOnly]
            [XmlAttribute("IsVirtualizing")]
            public string IsVirtualizingDef
            {
                get => IsVirtualizing.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        IsVirtualizing = true;
                        return;
                    }
                    IsVirtualizing = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }
#endif
            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(double.NaN)]
            public double ResizableBoxWidth { get; set; } = double.NaN;

            [XmlAttribute("ResizableBoxWidth")]
            [RPInternalUseOnly]
            public string ResizableBoxWidthDef
            {
                get => ResizableBoxWidth.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value)) return;
                    if (double.TryParse(value, out var result))
                    {
                        ResizableBoxWidth = result;
                        return;
                    }
                    ResizableBoxWidth = double.NaN;
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlAttribute("TextPath")]
            [DefaultValue("Text")]
            public string TextPath { get; set; } = "Text";

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(true)]
            public bool IsTextSearchEnabled { get; set; } = true;

            [RPInternalUseOnly]
            [XmlAttribute("IsTextSearchEnabled")]
            public string IsTextSearchEnabledDef
            {
                get => IsTextSearchEnabled.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        IsTextSearchEnabled = true;
                        return;
                    }
                    IsTextSearchEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonGallery
            [RPPrivateUseOnly]
            public class RibbonGalleryDef : RibbonComboDef
            {
                public RibbonGalleryDef()
                {
                    base.ResizeStyle = RibbonItemResizeStyles.ResizeWidth | RibbonItemResizeStyles.Collapse;
                }
                [RPInfoOut]
                [XmlIgnore]
                [Description("Gets or sets the display mode of the gallery. " +
                    "The display mode is used to specify the appearance of the gallery in the ribbon. " +
                    "The default value is Window.")]
                // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonGallery_DisplayMode
                public GalleryDisplayMode DisplayMode { get; set; } = GalleryDisplayMode.Window;

                [RPInternalUseOnly]
                [XmlAttribute("DisplayMode")]
                public string DisplayModeDef
                {
                    get => DisplayMode.ToString();
                    set
                    {
                        if (!Enum.TryParse(value, true, out GalleryDisplayMode result))
                            result = GalleryDisplayMode.Window;
                        DisplayMode = result;
                    }
                }

                [RPInfoOut]
                [RPInternalUseOnly]
                [XmlIgnore]
                [DefaultValue(double.NaN)]
                [TypeConverter(typeof(LengthConverter))]
                public double ItemWidth { get; set; } = double.NaN;

                [XmlAttribute("ItemWidth")]
                [RPInternalUseOnly]
                public string ItemWidthDef
                {
                    get => ItemWidth.ToString();
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;
                        if (double.TryParse(value, out var result))
                        {
                            ItemWidth = result;
                            return;
                        }
                        ItemWidth = double.NaN;
                    }
                }

                [RPInfoOut]
                [RPInternalUseOnly]
                [XmlIgnore]
                [DefaultValue(double.NaN)]
                [TypeConverter(typeof(LengthConverter))]
                public double ItemHeight { get; set; } = double.NaN;

                [XmlAttribute("ItemHeight")]
                [RPInternalUseOnly]
                public string ItemHeightDef
                {
                    get => ItemHeight.ToString();
                    set
                    {
                        if (string.IsNullOrEmpty(value)) return;
                        if (double.TryParse(value, out var result))
                        {
                            ItemHeight = result;
                            return;
                        }
                        ItemHeight = double.NaN;
                    }
                }
            }
        }
    }
}