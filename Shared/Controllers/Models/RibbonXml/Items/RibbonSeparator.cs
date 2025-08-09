﻿using System; // Keep for .NET 4.6
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
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSeparator
    [RPPrivateUseOnly]
    [Description("This class is used to support separators in a ribbon. Separators can be used to add space or divider lines between ribbon items.")]
    public class RibbonSeparatorDef : RibbonItemDef
    {
        public RibbonSeparatorDef()
        {
            base.ResizeStyle = RibbonItemResizeStyles.NoResize;
            base.MinWidth = 0.0;
            base.Width = double.NaN;
        }

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