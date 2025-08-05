namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonList
    public abstract class RibbonListDef : RibbonItemObservableCollectionDef
    {
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCombo
        public class RibbonComboDef : RibbonListDef
        {
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonGallery
            public class RibbonGalleryDef : RibbonComboDef
            {

            }
        }
    }
}