using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Shared.Controllers.Models.RibbonXml.Items
{
    // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider
    [RPPrivateUseOnly]
    public class RibbonSliderDef : RibbonItemDef
    {
        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(true)]
        [Description("This is IsSnapToTickEnabled, a member of class RibbonSlider. " +
            "The default value is true.")]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_IsSnapToTickEnabled
        public bool IsSnapToTickEnabled { get; set; } = true; // Default value was not mentioned in official documentation,
                                                              // however in version 2017 there was true as default value

        [RPInternalUseOnly]
        [XmlAttribute("IsSnapToTickEnabled")]
        public string IsSnapToTickEnabledDef
        {
            get => IsSnapToTickEnabled.ToString();
            set
            {
                if (value == null)
                {
                    IsSnapToTickEnabled = true;
                    return;
                }
                IsSnapToTickEnabled = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Maximum
        public double Maximum { get; set; } = 0.0d;

        [XmlAttribute("Maximum")]
        [RPInternalUseOnly]
        public string MaximumDef
        {
            get => Maximum.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Maximum = result;
                    return;
                }
                Maximum = 0.0d;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(0.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Minimum
        public double Minimum { get; set; } = 0.0d;

        [XmlAttribute("Minimum")]
        [RPInternalUseOnly]
        public string MinimumDef
        {
            get => Minimum.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Minimum = result;
                    return;
                }
                Minimum = 0.0d;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(false)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonItem_TextBox1Editable
        public bool TextBox1Editable { get; set; } = false;

        [RPInternalUseOnly]
        [XmlAttribute("TextBox1Editable")]
        public string TextBox1EditableDef
        {
            get => TextBox1Editable.ToString();
            set
            {
                if (value == null)
                {
                    TextBox1Editable = false;
                    return;
                }
                TextBox1Editable = value.Trim().ToUpper() == "TRUE"; // This is more reliable than bool#TryParse method
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Text
        public string TextBox1Text { get; set; } = null;

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(Visibility.Collapsed)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Visibility
        public Visibility TextBox1Visibility { get; set; } = Visibility.Collapsed;

        [RPInternalUseOnly]
        [XmlAttribute("TextBox1Visibility")]
        public string TextBox1VisibilityDef
        {
            get => TextBox1Visibility.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out Visibility result))
                    result = Visibility.Collapsed;
                TextBox1Visibility = result;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(27.0d)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TextBox1Width
        public double TextBox1Width { get; set; } = 27.0d;

        [XmlAttribute("TextBox1Width")]
        [RPInternalUseOnly]
        public string TextBox1WidthDef
        {
            get => TextBox1Width.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    TextBox1Width = result;
                    return;
                }
                TextBox1Width = 27.0d;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(TickPlacement.None)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_TickPlacement
        public TickPlacement TickPlacement { get; set; } = TickPlacement.None;

        [RPInternalUseOnly]
        [XmlAttribute("TickPlacement")]
        public string TickPlacementDef
        {
            get => TickPlacement.ToString();
            set
            {
                if (!Enum.TryParse(value, true, out TickPlacement result))
                    result = TickPlacement.None;
                TickPlacement = result;
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(null)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Ticks
        public DoubleCollection Ticks { get; set; } = null;

        [XmlAttribute("Ticks")]
        [RPInternalUseOnly]
        public string TicksDef
        {
            get => string.Join(" ", Ticks?.Select(val => val.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                try
                {
                    char[] separators = new[] { ' ', ',', ';' };
                    Ticks = new DoubleCollection(
                        value.Split(separators, System.StringSplitOptions.RemoveEmptyEntries)
                        .Select(val => double.Parse(val, System.Globalization.CultureInfo.InvariantCulture)));
                }
                catch {
                    Ticks = null;
                }
            }
        }

        [RPInfoOut]
        [XmlIgnore]
        [DefaultValue(0.0)]
        // https://help.autodesk.com/view/OARX/2026/CSY/?guid=OARX-ManagedRefGuide-Autodesk_Windows_RibbonSlider_Value
        public double Value { get; set; } = 0.0d;

        [XmlAttribute("Value")]
        [RPInternalUseOnly]
        public string ValueDef
        {
            get => Value.ToString();
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (double.TryParse(value, out var result))
                {
                    Value = result;
                    return;
                }
                Value = 0.0d;
            }
        }
    }
}