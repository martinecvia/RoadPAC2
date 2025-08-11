using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.Items.CommandItems
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonListButton
    [RPPrivateUseOnly]
    [Description("This is an abstract base class for list type buttons. " +
        "List type buttons are buttons that support a drop-down list. " +
        "RibbonSplitButton, RibbonChecklistButton, RibbonMenuButton are examples of list type buttons.")]
    [XmlInclude(typeof(RibbonChecklistButtonDef))]
    [XmlInclude(typeof(RibbonMenuButtonDef))]
    [XmlInclude(typeof(RibbonRadioButtonGroupDef))]
    [XmlInclude(typeof(RibbonSplitButtonDef))]
    public abstract class RibbonListButtonDef : RibbonButtonDef
    {
        [RPInternalUseOnly]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        // RibbonItem
        [XmlElement("RibbonCombo", typeof(RibbonListDef.RibbonComboDef))]
        [XmlElement("RibbonGallery", typeof(RibbonListDef.RibbonComboDef.RibbonGalleryDef))]
        [XmlElement("RibbonLabel", typeof(RibbonLabelDef))]
        [XmlElement("RibbonPanelBreak", typeof(RibbonPanelBreakDef))]
        [XmlElement("RibbonRowBreak", typeof(RibbonRowBreakDef))]
        [XmlElement("RibbonRowPanel", typeof(RibbonRowPanelDef))]
        [XmlElement("RibbonFlowPanel", typeof(RibbonRowPanelDef.RibbonFlowPanelDef))]
        [XmlElement("RibbonFoldPanel", typeof(RibbonRowPanelDef.RibbonFoldPanelDef))]
        [XmlElement("RibbonSeparator", typeof(RibbonSeparatorDef))]
        [XmlElement("RibbonSlider", typeof(RibbonSliderDef))]
        [XmlElement("RibbonSpinner", typeof(RibbonSpinnerDef))]
        [XmlElement("RibbonTextBox", typeof(RibbonTextBoxDef))]
        // RibbonCommandItem
#if !ZWCAD
        [XmlElement("ProgressBarSource", typeof(ProgressBarSourceDef))]
#endif
        [XmlElement("RibbonCheckBox", typeof(RibbonCheckBoxDef))]
        [XmlElement("RibbonMenuItem", typeof(RibbonMenuItemDef))]
        [XmlElement("ApplicationMenuItem", typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
        // RibbonButton
        [XmlElement("RibbonButton", typeof(RibbonButtonDef))]
        [XmlElement("RibbonToggleButton", typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
        [XmlElement("ToolBarShareButton", typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
        [XmlElement("RibbonChecklistButton", typeof(RibbonChecklistButtonDef))]
        [XmlElement("RibbonMenuButton", typeof(RibbonMenuButtonDef))]
        [XmlElement("RibbonRadioButtonGroup", typeof(RibbonRadioButtonGroupDef))]
        [XmlElement("RibbonSplitButton", typeof(RibbonSplitButtonDef))]
        public List<RibbonItemDef> ItemsDef { get; set; } = new List<RibbonItemDef>();

        public class RibbonChecklistButtonDef : RibbonListButtonDef
        { }

        public class RibbonMenuButtonDef : RibbonListButtonDef
        { }

        public class RibbonRadioButtonGroupDef : RibbonListButtonDef
        { }

        [RPPrivateUseOnly]
        [Description("This class is used to support split buttons in a ribbon. " +
            "The items in the drop-down list should be of type RibbonCommandItem or RibbonSeparator. " +
            "Other items are not supported in the drop-down list. " +
            "An exception is thrown if an unsupported item is added to the Items collection.")]
        public class RibbonSplitButtonDef : RibbonListButtonDef
        {
            public RibbonSplitButtonDef() {
                base.IsSplit = true;
            }
        }
    }
}