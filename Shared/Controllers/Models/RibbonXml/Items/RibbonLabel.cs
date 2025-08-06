#if NET8_0_OR_GREATER
using System.ComponentModel;
using System.Xml.Serialization;
#endif

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel
    [RPPrivateUseOnly]
    public class RibbonLabelDef : RibbonItemDef
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// Value has only getter, cannot be changed to a different type other than Horizontal
        /// </summary>
        /// 
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("This is Orientation, a member of class RibbonLabel.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel_Orientation
        public System.Windows.Controls.Orientation Orientation { get; set; } = System.Windows.Controls.Orientation.Horizontal;

        [RPInternalUseOnly]
        [XmlAttribute("Orientation")]
        public string OrientationDef
        {
            get => Orientation.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out System.Windows.Controls.Orientation result))
                    result = System.Windows.Controls.Orientation.Horizontal;
                Orientation = result;
            }
        }
#endif
    }
}