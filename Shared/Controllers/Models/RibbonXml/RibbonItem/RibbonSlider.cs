using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider
    public class RibbonSliderDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("This is IsSnapToTickEnabled, a member of class RibbonSlider. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_IsSnapToTickEnabled
        public bool IsSnapToTickEnabled { get; set; } = true; // Default value was not mentioned in official documentation,
                                                              // however in version 2017 there was true as default value

        [RPInternalUseOnly]
        [XmlAttribute("IsSnapToTickEnabled")]
        public string IsSnapToTickEnabledDef
        {
            get => IsSnapToTickEnabled.ToString();
            set
            {
                if (value == null)
                {
                    IsSnapToTickEnabled = true;
                    return;
                }
                IsSnapToTickEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
    }
}