#if NET8_0_OR_GREATER
using System.ComponentModel;
using System.Xml.Serialization;
#endif

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel
    [RPPrivateUseOnly]
    public class RibbonLabelDef : RibbonItemDef
    {
        public RibbonLabelDef() {
            base.ResizeStyle = RibbonItemResizeStyles.NoResize;
            base.ShowText = true;
            base.IsToolTipEnabled = false;
            base.Width = double.NaN;
            base.MinWidth = 0.0;
        }
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