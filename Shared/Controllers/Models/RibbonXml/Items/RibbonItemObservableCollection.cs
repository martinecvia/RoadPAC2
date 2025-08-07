using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

namespace Shared.Controllers.Models.RibbonXml.Items
{
    [RPPrivateUseOnly]
    public class RibbonItemObservableCollectionDef : RibbonItemDef
    {
        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();
    }
}
