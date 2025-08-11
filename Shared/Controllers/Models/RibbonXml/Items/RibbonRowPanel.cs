using System; // Keep for .NET 4.6
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowPanel
    [RPPrivateUseOnly]
    [XmlInclude(typeof(RibbonRowPanelDef))]
    [XmlInclude(typeof(RibbonFlowPanelDef))]
    [XmlInclude(typeof(RibbonFoldPanelDef))]
    [Description("The RibbonRowPanel class is used to create a sub-panel within a panel. " +
        "RibbonRowPanel is a ribbon item that can be added to a RibbonPanelSource. " +
        "Items collection to create a sub-panel containing multiple sub-rows in a main row of items. " +
        "For example, a sub-panel could be used to create a row containing two large buttons and two rows of small buttons. " +
        "The items in the sub-panel are stored and managed in the Items collection. " +
        "The items can be organized into multiple rows by adding a RibbonRowBreak item at the index where the new row is to start.")]
    public class RibbonRowPanelDef : RibbonItemDef
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
        [XmlElement("RibbonFlowPanel", typeof(RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonFoldPanelDef))]
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
        [XmlElement("RibbonChecklistButton", typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        [RPInfoOut]
        [XmlIgnore]
        [RPInternalUseOnly]
        [DefaultValue(null)]
#if NET8_0_OR_GREATER
        public RibbonSubPanelSource? Source => SourceDef?.Transform(new RibbonSubPanelSource());
#else
        public RibbonSubPanelSource Source => SourceDef?.Transform(new RibbonSubPanelSource());
#endif
        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonSubPanelSource", typeof(RibbonSubPanelSourceDef))]
#if NET8_0_OR_GREATER
        public RibbonSubPanelSourceDef? SourceDef { get; set; } = null;
#else
        public RibbonSubPanelSourceDef SourceDef { get; set; } = null;
#endif

        [RPInfoOut]
        [XmlIgnore]
        [RPInternalUseOnly]
        [DefaultValue(100)]
        public int ResizePriority { get; set; } = 100;

        [RPInternalUseOnly]
        [XmlAttribute("ResizePriority")]
        public string ResizePriorityDef
        {
            get => ResizePriority.ToString();
            set
            {
                if (int.TryParse(value, out var x)) ResizePriority = x;
                else
                {
                    ResizePriority = 100;
                }
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(RibbonRowPanelResizeStyle.None)]
        public RibbonRowPanelResizeStyle SubPanelResizeStyle { get; set; } = RibbonRowPanelResizeStyle.None;

        [RPInternalUseOnly]
        [XmlAttribute("SubPanelResizeStyle")]
        public string SubPanelResizeStyleDef
        {
            get => SubPanelResizeStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonRowPanelResizeStyle result))
                    result = RibbonRowPanelResizeStyle.None;
                SubPanelResizeStyle = result;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [RPInternalUseOnly]
        [DefaultValue(false)]
        public bool IsTopJustified { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsTopJustified")]
        public string IsTopJustifiedDef
        {
            get => IsTopJustified.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsTopJustified = false;
                    return;
                }
                IsTopJustified = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
#if NET8_0_OR_GREATER // This was removed in AutoCAD 
                      // however keeping this does not affect usability, and its still used in .NET 4.6 and ZWCAD
#else
        [RPInfoOut]
        [XmlIgnore]
        [RPInternalUseOnly]
        [DefaultValue(false)]
        public bool AreItemsArrangedFromRightToLeft { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("AreItemsArrangedFromRightToLeft")]
        public string AreItemsArrangedFromRightToLeftDef
        {
            get => AreItemsArrangedFromRightToLeft.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    AreItemsArrangedFromRightToLeft = false;
                    return;
                }
                AreItemsArrangedFromRightToLeft = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
#endif

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel
        [RPPrivateUseOnly]
        public class RibbonFlowPanelDef : RibbonRowPanelDef
        {
            /*
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonSupportedSubPanelStyle.None)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_SupportedSubPanel
            public RibbonSupportedSubPanelStyle SupportedSubPanel { get; set; } = RibbonSupportedSubPanelStyle.None;

            [RPInternalUseOnly]
            [XmlAttribute("SupportedSubPanel")]
            public string SupportedSubPanelDef
            {
                get => SupportedSubPanel.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSupportedSubPanelStyle result))
                        result = RibbonSupportedSubPanelStyle.None;
                    SupportedSubPanel = result;
                }
            }
            */

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(3)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_MaxRowNumber
            public int MaxRowNumber { get; set; } = 3;

            [RPInternalUseOnly]
            [XmlAttribute("MaxRowNumber")]
            public string MaxRowNumberDef
            {
                get => MaxRowNumber.ToString();
                set
                {
                    if (int.TryParse(value, out var x)) MaxRowNumber = x;
                    else
                    {
                        MaxRowNumber = 3;
                    }
                }
            }

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel_AreColumnsStatic
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(false)]
            public bool AreColumnsStatic { get; set; } = false;

            [RPInternalUseOnly]
            [XmlAttribute("AreColumnsStatic")]
            public string AreColumnsStaticDef
            {
                get => AreColumnsStatic.ToString();
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        AreColumnsStatic = false;
                        return;
                    }
                    AreColumnsStatic = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel
        [RPPrivateUseOnly]
        public class RibbonFoldPanelDef : RibbonRowPanelDef
        {
            /*
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonSupportedSubPanelStyle.None)]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_SupportedSubPanel
            public RibbonSupportedSubPanelStyle SupportedSubPanel { get; set; } = RibbonSupportedSubPanelStyle.None;

            [RPInternalUseOnly]
            [XmlAttribute("SupportedSubPanel")]
            public string SupportedSubPanelDef
            {
                get => SupportedSubPanel.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonSupportedSubPanelStyle result))
                        result = RibbonSupportedSubPanelStyle.None;
                    SupportedSubPanel = result;
                }
            }
            */

            [RPInfoOut]
            [RPInternalUseOnly]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelResizeStyle.None)]
            public new RibbonFoldPanelResizeStyle SubPanelResizeStyle { get; set; } = RibbonFoldPanelResizeStyle.None;

            [RPInternalUseOnly]
            [XmlAttribute("SubPanelResizeStyle")]
            public new string SubPanelResizeStyleDef
            {
                get => SubPanelResizeStyle.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelResizeStyle result))
                        result = RibbonFoldPanelResizeStyle.None;
                    SubPanelResizeStyle = result;
                }
            }

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_DefaultSize
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Medium)]
            public RibbonFoldPanelSize DefaultSize { get; set; } = RibbonFoldPanelSize.Medium;

            [RPInternalUseOnly]
            [XmlAttribute("DefaultSize")]
            public string DefaultSizeDef
            {
                get => DefaultSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Medium;
                    DefaultSize = result;
                }
            }

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_MaxSize
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Large)]
            public RibbonFoldPanelSize MaxSize { get; set; } = RibbonFoldPanelSize.Large;

            [RPInternalUseOnly]
            [XmlAttribute("MaxSize")]
            public string MaxSizeDef
            {
                get => MaxSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Large;
                    MaxSize = result;
                }
            }

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_MinSize
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelSize.Small)]
            public RibbonFoldPanelSize MinSize { get; set; } = RibbonFoldPanelSize.Small;

            [RPInternalUseOnly]
            [XmlAttribute("MinSize")]
            public string MinSizeDef
            {
                get => MinSize.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelSize result))
                        result = RibbonFoldPanelSize.Small;
                    MinSize = result;
                }
            }
        }
    }
}