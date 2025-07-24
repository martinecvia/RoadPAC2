using System.Collections.Generic;
using System.Xml.Serialization;
using Shared.Controllers.Models.RibbonXml.RibbonItem;

// https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem
namespace Shared.Controllers.Models.RibbonXml
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem
    public class RibbonCommandItemDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonLabel
    public class RibbonFormDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonHwnd
    public class RibbonHwndDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList
    public class RibbonLabelDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList
    public class RibbonListDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelBreak
    public class RibbonPanelBreakDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowBreak
    public class RibbonRowBreakDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowPanel
    public class RibbonRowPanelDef
    {

    }

    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider
    public class RibbonSliderDef
    {

    }

    [XmlRoot("RibbonTab")]
    public class RibbonTabDef
    {
        [XmlElement("RibbonTextBox")] public List<RibbonTextBoxDef> Items { get; set; } = new List<RibbonTextBoxDef>();
    }
}
