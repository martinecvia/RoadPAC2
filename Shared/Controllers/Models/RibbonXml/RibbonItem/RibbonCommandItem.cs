using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem
    public class RibbonCommandItemDef : RibbonItemDef
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem_IsCheckable
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates if this is a checkable item. " +
            "This property is used only by item types that support the toggling of items. " +
            "For example, items in a drop down list of a RibbonMenuItem or RibbonChecklistButton use this property. " +
            "The default value is false.")]
        public bool IsCheckable { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsCheckable")]
        public string IsCheckableDef
        {
            get => IsCheckable.ToString();
            set
            {
                if (value == null)
                {
                    IsCheckable = false;
                    return;
                }
                IsCheckable = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
    }
}
