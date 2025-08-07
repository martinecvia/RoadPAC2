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
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        public List<RibbonCommandItemDef> ItemsDef { get; set; } = new List<RibbonCommandItemDef>();
    }
}
