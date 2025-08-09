using System; // Keep for .NET 4.6
using System.ComponentModel;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.Windows;
#else
using Autodesk.Windows;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelBreak
    [RPPrivateUseOnly]
    [Description("This class is used to organize a ribbon panel into main and slide-out panels. " +
        "Add an object of this type to RibbonPanelSource.Items to move all the items after this item into the slide-out panel. " +
        "There can be only one object of this type in RibbonPanelSource. " +
        "If there are multiple RibbonPanelBreak objects in the same panel source, only the first one will be used.")]
    public class RibbonPanelBreakDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(RibbonSupportedSubPanelStyle.None)]
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