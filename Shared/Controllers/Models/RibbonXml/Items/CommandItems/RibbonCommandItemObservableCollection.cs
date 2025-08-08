using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    [RPPrivateUseOnly]
    public class RibbonCommandItemObservableCollectionDef : RibbonCommandItemDef
    {
        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonCommandItem
        [XmlElement("DocumentItem", typeof(DocumentItemDef))]
#if ZWCAD
// ZWCAD Does not support this
#else
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        public List<RibbonCommandItemDef> ItemsDef { get; set; } = new List<RibbonCommandItemDef>();
    }
}
