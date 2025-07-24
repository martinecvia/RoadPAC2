using System.ComponentModel;
using System.Xml.Serialization;
using System;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSeparator
    public class RibbonSeparatorDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(RibbonSeparatorStyle.Line)]
        [Description("Gets or sets the value specifying the separator style. " +
            "Separator styles are used to set the appearance of the separator in a ribbon.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSeparator_SeparatorStyle
        public RibbonSeparatorStyle SeparatorStyle { get; set; } = RibbonSeparatorStyle.Line;

        [RPInternalUseOnly]
        [XmlAttribute("SeparatorStyle")]
        public string SeparatorStyleDef
        {
            get => SeparatorStyle.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonSeparatorStyle result))
                    result = RibbonSeparatorStyle.Line;
                SeparatorStyle = result;
            }
        }
    }
}