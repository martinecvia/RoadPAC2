using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowPanel
    [XmlInclude(typeof(RibbonRowPanelDef))]
    [XmlInclude(typeof(RibbonFlowPanelDef))]
    [XmlInclude(typeof(RibbonFoldPanelDef))]
    [Description("The RibbonRowPanel class is used to create a sub-panel within a panel. " +
        "RibbonRowPanel is a ribbon item that can be added to a RibbonPanelSource. " +
        "Items collection to create a sub-panel containing multiple sub-rows in a main row of items. " +
        "For example, a sub-panel could be used to create a row containing two large buttons and two rows of small buttons. " +
        "The items in the sub-panel are stored and managed in the Items collection. " +
        "The items can be organized into multiple rows by adding a RibbonRowBreak item at the index where the new row is to start.")]
    public class RibbonRowPanelDef : RibbonItemObservableCollectionDef
    {

        [XmlIgnore]
        public override List<RibbonItem> Items => Items; // virtual can cause problems

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFlowPanel
        public class RibbonFlowPanelDef : RibbonRowPanelDef
        {
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
                    if (value == null)
                    {
                        AreColumnsStatic = false;
                        return;
                    }
                    AreColumnsStatic = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
                }
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel
        public class RibbonFoldPanelDef : RibbonRowPanelDef
        {
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

            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonFoldPanel_SubPanelResizeStyle
            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(RibbonFoldPanelResizeStyle.None)]
            public RibbonFoldPanelResizeStyle SubPanelResizeStyle { get; set; } = RibbonFoldPanelResizeStyle.None;

            [RPInternalUseOnly]
            [XmlAttribute("SubPanelResizeStyle")]
            public string SubPanelResizeStyleDef
            {
                get => SubPanelResizeStyle.ToString();
                set
                {
                    if (!Enum.TryParse(value, true, out RibbonFoldPanelResizeStyle result))
                        result = RibbonFoldPanelResizeStyle.None;
                    SubPanelResizeStyle = result;
                }
            }
        }
    }
}
