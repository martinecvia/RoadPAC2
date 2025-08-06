using System;
using System.Collections.Generic;
using System.Xml.Serialization;

#region O_PROGRAM_DETERMINE_CAD_PLATFORM 
#if ZWCAD
using ZwSoft.ZwCAD.Windows;
#else
using Autodesk.Windows;
using Shared.Controllers.Models.RibbonXml.Items.CommandItems;
#endif
#endregion

namespace Shared.Controllers.Models.RibbonXml.Items
{
    public class RibbonItemObservableCollectionDef : RibbonItemDef
    {
        [XmlIgnore]
        public virtual List<RibbonItem> Items
        {
            get
            {
                List<RibbonItem> items = new List<RibbonItem>();
                if (ItemsDef == null)
                    return items;
                foreach (RibbonItemDef element in ItemsDef)
                    items.Add(Transform(ItemsFactory[element.GetType()](), element));
                return items;
            }
        }

        [RPInternalUseOnly]
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

    }
}
