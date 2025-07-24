using System;
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelBreak
    public class RibbonPanelBreakDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(RibbonSupportedSubPanelStyle.None)]
        [Description("This is SupportedSubPanel, a member of class RibbonPanelBreak.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelBreak_SupportedSubPanel
        public RibbonSupportedSubPanelStyle SupportedSubPanel { get; set; } = RibbonSupportedSubPanelStyle.None;

        [RPInternalUseOnly]
        [XmlAttribute("SupportedSubPanel")]
        public string SupportedSubPanelDef
        {
            get => SupportedSubPanel.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out RibbonSupportedSubPanelStyle result))
                    result = RibbonSupportedSubPanelStyle.None;
                SupportedSubPanel = result;
            }
        }
    }
}