﻿using System;
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton
    [RPPrivateUseOnly]
    [Description("This is an abstract base class for list type buttons. " +
        "List type buttons are buttons that support a drop-down list. " +
        "RibbonSplitButton, RibbonChecklistButton, RibbonMenuButton are examples of list type buttons.")]
    [XmlInclude(typeof(RibbonChecklistButtonDef))]
    [XmlInclude(typeof(RibbonMenuButtonDef))]
    [XmlInclude(typeof(RibbonRadioButtonGroupDef))]
    [XmlInclude(typeof(RibbonSplitButtonDef))]
    public abstract class RibbonListButtonDef : RibbonButtonDef
    {
        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        // RibbonCommandItem
#if !ZWCAD
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        [XmlElement("ApplicationMenuItem", typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonToggleButton", typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
        [XmlElement("ToolBarShareButton", typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
        [XmlElement("RibbonChecklistButton", typeof(RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonSplitButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("Gets or sets the value that indicates whether the list button is to behave like a split button. " +
            "If this property is true, the list button supports executing the button without opening the drop-down list, and the drop-down list is opened by clicking the arrow. " +
            "If it is false, the list button always opens the drop-down list when clicked, and items need to be executed from the drop-down list. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton_IsSplit
        public bool IsSplit { get; set; } = true;

        [RPInternalUseOnly]
        [XmlAttribute("IsSplit")]
        public string IsSplitDef
        {
            get => IsSplit.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsSplit = true;
                    return;
                }
                IsSplit = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates whether the drop-down list supports the grouping of items. " +
            "Grouping is accomplished by setting the property RibbonItem.GroupName for the drop-down items, so the items in the drop-down list should set the group name with that property. " +
            "If this property is true, grouping is enabled in the drop-down list. " +
            "If it is false, grouping is not enabled. " +
            "The default value is false.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton_IsGrouping
        public bool IsGrouping { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsGrouping")]
        public string IsGroupingDef
        {
            get => IsGrouping.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsGrouping = false;
                    return;
                }
                IsGrouping = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool AllowOrientation { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("AllowOrientation")]
        public string AllowOrientationDef
        {
            get => AllowOrientation.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    AllowOrientation = false;
                    return;
                }
                AllowOrientation = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonChecklistButton
        [RPPrivateUseOnly]
        [Description("This class is used to support the Checklist button in a ribbon. " +
            "The Checklist button displays a list of checkboxes in the drop down list. " +
            "The items in the drop down list should be of type RibbonCommandItem or RibbonSeparator. " +
            "Other items are not supported in the drop down list. An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonChecklistButtonDef : RibbonListButtonDef
        { }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonMenuButton
        [RPPrivateUseOnly]
        [Description("This class is used to support the menu button in a ribbon. " +
            "The Menu button displays a standard menu in the drop-down list. " +
            "The menu can be a nested menu with sub-menus. " +
            "The items in the drop-down list should be of type RibbonMenuItem or RibbonSeparator. " +
            "Other item types are not supported in the drop-down list. " +
            "An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonMenuButtonDef : RibbonListButtonDef
        { }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRadioButtonGroup
        [RPPrivateUseOnly]
        [Description("This class is used to support radio button groups in a ribbon. " +
            "This class contains a collection of RibbonToggleButton items, which act as radio buttons by supporting a mutually exclusive checkstate. " +
            "The radio buttons in the group use display properties like Size, ShowText, and ShowImage from this parent group and ignore the properties set in the radio button itself.")]
        public class RibbonRadioButtonGroupDef : RibbonListButtonDef
        {
            public RibbonRadioButtonGroupDef()
            {
                base.IsSplit = false;
                base.Width = double.NaN;
                base.AllowInToolBar = true;
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(3)]
            public int MaxRow { get; set; } = 3;

            [RPInternalUseOnly]
            [XmlAttribute("MaxRow")]
            public string MaxRowDef
            {
                get => MaxRow.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxRow = x;
                    else
                    {
                        MaxRow = 3;
                    }
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(10_000)]
            public int MaxColumn { get; set; } = 10_000;

            [RPInternalUseOnly]
            [XmlAttribute("MaxColumn")]
            public string MaxColumnDef
            {
                get => MaxColumn.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxColumn = x;
                    else
                    {
                        MaxColumn = 10_000;
                    }
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
            public System.Windows.Controls.Orientation ExpandOrientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;

            [RPInternalUseOnly]
            [XmlAttribute("ExpandOrientation")]
            public string ExpandOrientationDef
            {
                get => ExpandOrientation.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                        result = System.Windows.Controls.Orientation.Horizontal;
                    ExpandOrientation = result;
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(RibbonItemSize.Standard)]
            public RibbonItemSize CollapsedSize { get; set; } = RibbonItemSize.Standard;

            [RPInternalUseOnly]
            [XmlAttribute("CollapsedSize")]
            public string CollapsedSizeDef
            {
                get => CollapsedSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonItemSize result))
                        result = RibbonItemSize.Standard;
                    CollapsedSize = result;
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(true)]
            public bool CanCollapse { get; set; } = true;

            [RPInternalUseOnly]
            [XmlAttribute("CanCollapse")]
            public string CanCollapseDef
            {
                get => CanCollapse.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        CanCollapse = true;
                        return;
                    }
                    CanCollapse = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSplitButton
        [RPPrivateUseOnly]
        [Description("This class is used to support split buttons in a ribbon. " +
            "The items in the drop-down list should be of type RibbonCommandItem or RibbonSeparator. " +
            "Other items are not supported in the drop-down list. " +
            "An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonSplitButtonDef : RibbonListButtonDef
        {
            public RibbonSplitButtonDef() {
                base.IsSplit = true;
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(RibbonSplitButtonListStyle.List)]
            public RibbonSplitButtonListStyle ListStyle { get; set; } = RibbonSplitButtonListStyle.List;

            [RPInternalUseOnly]
            [XmlAttribute("ListStyle")]
            public string ListStyleDef
            {
                get => ListStyle.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSplitButtonListStyle result))
                        result = RibbonSplitButtonListStyle.List;
                    ListStyle = result;
                }
            }

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(RibbonImageSize.Large)]
            public RibbonImageSize ListImageSize { get; set; } = RibbonImageSize.Large;

            [RPInternalUseOnly]
            [XmlAttribute("ListImageSize")]
            public string ListImageSizeDef
            {
                get => ListImageSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonImageSize result))
                        result = RibbonImageSize.Large;
                    ListImageSize = result;
                }
            }
        }
    }
}