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
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonRowBreak
    [RPPrivateUseOnly]
    [Description("This class is used to organize the ribbon items in a panel into multiple rows. " +
        "Add an object of this class to any position in a RibbonPanelSource.Items or RibbonRowPanel.Items collection to move subsequent items to the next row.")]
    public class RibbonRowBreakDef : RibbonItemDef
    { }
}
