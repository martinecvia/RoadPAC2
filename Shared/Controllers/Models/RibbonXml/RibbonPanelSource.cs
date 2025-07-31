#pragma warning disable CS8601
#pragma warning disable CS8603
#pragma warning disable CS8625

using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.RibbonItem
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSource
    public class RibbonPanelSourceDef : BaseRibbonXml
    {

        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer
        public class RibbonPanelSpacerDef : RibbonPanelSourceDef
        {
            /*
             * Creation:
            <RibbonPanelSpacerDef xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                  xmlns:xsd="http://www.w3.org/2001/XMLSchema">
              <LeftBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FFFF0000" /&gt;
              </LeftBorderBrush>
              <RightBorderBrush>
                &lt;SolidColorBrush xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" Color="#FF0000FF" /&gt;
              </RightBorderBrush>
            </RibbonPanelSpacerDef>
             */ 

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(null)]
            [Description("This is LeftBorderBrush, a member of class RibbonPanelSpacer.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_LeftBorderBrush
            public Brush LeftBorderBrush { get; set; } = null;

            [RPInternalUseOnly]
            [XmlElement("LeftBorderBrush")]
            public string LeftBorderBrushDef
            {
                get => LeftBorderBrush != null ? XamlWriter.Save(LeftBorderBrush) : null;
                set => LeftBorderBrush = !string.IsNullOrWhiteSpace(value) ? (Brush) XamlReader.Parse(value) : null;
            }

            [RPInfoOut]
            [XmlIgnore]
            [DefaultValue(null)]
            [Description("This is RightBorderBrush, a member of class RibbonPanelSpacer.")]
            // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonPanelSpacer_LeftBorderBrush
            public Brush RightBorderBrush { get; set; } = null;

            [RPInternalUseOnly]
            [XmlElement("RightBorderBrush")]
            public string RightBorderBrushDef
            {
                get => RightBorderBrush != null ? XamlWriter.Save(RightBorderBrush) : null;
                set => RightBorderBrush = !string.IsNullOrWhiteSpace(value) ? (Brush)XamlReader.Parse(value) : null;
            }
        }
    }
}