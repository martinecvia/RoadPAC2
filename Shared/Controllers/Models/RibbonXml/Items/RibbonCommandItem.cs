﻿using System.ComponentModel;
using System.Xml.Serialization;

using Shared.Controllers.Models.RibbonXml.Items.CommandItems;

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem
    [RPPrivateUseOnly]
#if ZWCAD
    // ZWCAD Does not support this
#else
    [XmlInclude(typeof(ProgressBarSourceDef))]
#endif
    [XmlInclude(typeof(RibbonButtonDef))]
    [XmlInclude(typeof(RibbonCheckBoxDef))]
    [XmlInclude(typeof(RibbonListButtonDef))]
    [XmlInclude(typeof(RibbonMenuItemDef))]
    [XmlInclude(typeof(RibbonMenuItemDef.ApplicationMenuItemDef))]
    [XmlInclude(typeof(RibbonToggleButtonDef))]
#if (NET8_0_OR_GREATER || ZWCAD)
    [XmlInclude(typeof(RibbonToggleButtonDef.ToolBarShareButtonDef))]
#endif
    public class RibbonCommandItemDef : RibbonItemDef
    {
        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(null)]
#if NET8_0_OR_GREATER
        public System.Windows.Input.ICommand? CommandHandler { get; set; } = null;
#else
        public System.Windows.Input.ICommand CommandHandler { get; set; } = null;
#endif

        [RPInternalUseOnly]
        [XmlAttribute("CommandHandler")]
        public string CommandHandlerDef
        {
            get => CommandHandler != null && CommandHandler is CommandHandler handler
                ? handler.Command : string.Empty;
            set
            {
                if (!string.IsNullOrEmpty(value))
                    CommandHandler = new CommandHandler(value);
            }
        }

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonCommandItem_IsCheckable
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        [Description("Gets or sets the value that indicates if this is a checkable item. " +
            "This property is used only by item types that support the toggling of items. " +
            "For example, items in a drop down list of a RibbonMenuItem or RibbonChecklistButton use this property. " +
            "The default value is false.")]
        public bool IsCheckable { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsCheckable")]
        public string IsCheckableDef
        {
            get => IsCheckable.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsCheckable = false;
                    return;
                }
                IsCheckable = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [RPInternalUseOnly]
        [XmlIgnore]
        [DefaultValue(false)]
        public bool IsActive { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("IsActive")]
        public string IsActiveDef
        {
            get => IsActive.ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    IsActive = false;
                    return;
                }
                IsActive = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }
    }
}
