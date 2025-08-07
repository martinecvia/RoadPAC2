using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonButton
    [RPPrivateUseOnly]
    public class RibbonButtonDef : RibbonCommandItemDef
    {
        public RibbonButtonDef()
        {
            base.MinWidth = 0;
            base.Width = double.NaN;
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(System.Windows.Controls.Orientation.Horizontal)]
        [Description("Accesses the orientation of text and image. " +
            "This is a dependency property. " +
            "The default value is Horizontal.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonButton_Orientation
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
    }
}
