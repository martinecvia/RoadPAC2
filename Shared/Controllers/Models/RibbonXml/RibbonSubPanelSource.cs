#pragma warning disable CS8625

using System.Collections.Generic; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

using Shared.Controllers.Models.RibbonXml.Items;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource
    [RPPrivateUseOnly]
    public class RibbonSubPanelSourceDef : BaseRibbonXml
    {
        private string _cookie;
        public override string Cookie
        {
            get => _cookie ?? $"%Parent:SubPanelSource={Id}__{Name}";
            set => _cookie = value;
        }

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
        [XmlElement("RibbonChecklistButton", typeof(RibbonListButtonDef.RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonListButtonDef.RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonListButtonDef.RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonListButtonDef.RibbonSplitButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Description
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Description")]
        [DefaultValue(null)]
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

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Name
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Name")]
        [DefaultValue(null)]
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

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSubPanelSource_Tag
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlAttribute("Tag")]
        [DefaultValue(null)]
        public string Tag { get; set; } = null;
    }
}