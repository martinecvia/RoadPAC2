using System.ComponentModel;

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton
    [RPPrivateUseOnly]
    [Description("This is an abstract base class for list type buttons. " +
        "List type buttons are buttons that support a drop-down list. " +
        "RibbonSplitButton, RibbonChecklistButton, RibbonMenuButton are examples of list type buttons.")]
    public abstract class RibbonListButtonDef : RibbonButtonDef
    {

    }
}