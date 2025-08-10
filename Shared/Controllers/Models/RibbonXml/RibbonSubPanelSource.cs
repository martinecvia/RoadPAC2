#pragma warning disable CS8625

using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
using Shared.Controllers.Models.RibbonXml.Items;
using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource
    [RPPrivateUseOnly]
    public class RibbonSubPanelSourceDef : BaseRibbonXml
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
        [XmlElement("DocumentItem", typeof(DocumentItemDef))]
#if ZWCAD
        // ZWCAD Does not support this yet
#else
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        [XmlElement("ApplicationMenuItem", typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonToggleButton", typeof(RibbonToggleButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Id
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Id")]
        [DefaultValue(null)]
        public string Id { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Description
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
        public string Description { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Name
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
        public string Name { get; set; } = null;

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Tag
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        public string Tag { get; set; } = null;
    }
}